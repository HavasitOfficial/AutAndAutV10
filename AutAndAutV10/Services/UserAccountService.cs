using AutAndAutV10.Models;
using AutAndAutV10.Services.Interfaces;
using ModelsBuilder;
using Umbraco.Cms.Core.Models.Membership;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;

namespace AutAndAutV10.Services
{
    public class UserAccountService : IUserAccountService
    {
        private readonly ISendEmailService _sendEmailService;
        private readonly IForgottenPasswordService _forgottenPasswordService;
        private readonly IPublishedValueFallback _publishedValueFallback;
        private readonly IMemberManager _memberManager;
        private readonly IMemberService _memberService;
        private readonly ISiteSettingsService _siteSettingsService;
        private readonly IUmbracoContextAccessor _context;
        private readonly IConfiguration _configuration;

        private readonly string EMAIL_VIEW_PATH;
        private readonly string EMAIL_SUBJECT;
        private const string MemberName= "{memberName}";
        private const string URL = "{url}";

        public UserAccountService(ISendEmailService sendEmailService, IForgottenPasswordService forgottenPasswordService, IPublishedValueFallback publishedValueFallback, IMemberManager memberManager, ISiteSettingsService siteSettingsService, IUmbracoContextAccessor context, IConfiguration configuration, IMemberService memberService)
        {
            _sendEmailService = sendEmailService;
            _forgottenPasswordService = forgottenPasswordService;
            _publishedValueFallback = publishedValueFallback;
            _memberManager = memberManager;
            _siteSettingsService = siteSettingsService;
            _context = context;
            _configuration = configuration;
            EMAIL_VIEW_PATH = _configuration.GetValue<string>("AutAndAutOptions:EmailViewPath");
            EMAIL_SUBJECT = _configuration.GetValue<string>("AutAndAutOptions:EmailSubject");
            _memberService = memberService;
        }

        public async Task CheckEmailAndPasswordToken(ForgottenPasswordResetModel passwordModel,
            MemberIdentityUser memberIdentityUser)
        {
            var records =
            _forgottenPasswordService.GetForgottenPasswordByEmail(passwordModel.Email);
            if (_forgottenPasswordService.ValidateToken(passwordModel.ForgottenPasswordToken, records))
            {
                var member = GetMemberProfile(memberIdentityUser);
                if (member != null)
                {
                    var token = await _memberManager.GeneratePasswordResetTokenAsync(memberIdentityUser);
                    await _memberManager.ResetPasswordAsync(memberIdentityUser, token, passwordModel.Password);

                    _forgottenPasswordService.DeleteForgottenPasswordByEmail(passwordModel.Email);
                }
            }
        }

        public void GeneratePasswordToken(string memberEmail, MemberIdentityUser memberIdentityUser)
        {
            var member = GetMemberProfile(memberIdentityUser);
            var messageBody = GetForgottenPasswordViewPath();

            var content = _context.GetRequiredUmbracoContext().PublishedRequest.PublishedContent;
            var siteSettings = _siteSettingsService.GetSiteSettings(content, _publishedValueFallback);
            var resetPassWordUrl = siteSettings.ForgotPasswordResetPage?.Url(mode: UrlMode.Absolute);

            var token = _forgottenPasswordService.CreateToken(memberEmail);
            var fullResetPasswordUrl = resetPassWordUrl.Remove(resetPassWordUrl.Length - 1) + "?t=" + token;

            SendPasswordResetEmail(messageBody, memberEmail, fullResetPasswordUrl, member.Name);
        }

        private void SendPasswordResetEmail(string messageBody, string memberEmail, string url, string memberName)
        {
            var dictionary = new Dictionary<string, string>()
            {
                {MemberName, memberName},
                {URL, url}
            };

            var message = new Message(memberEmail,
                EMAIL_SUBJECT, messageBody,
                dictionary);

            _sendEmailService.SendEmail(message);
        }

        private string GetForgottenPasswordViewPath()
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), EMAIL_VIEW_PATH);
            return System.IO.File.Exists(filePath) ? System.IO.File.ReadAllText(filePath) : filePath;
        }

        public Member GetMemberProfile(MemberIdentityUser memberIdentityUser)
        {
            return new Member(_memberManager.AsPublishedMember(memberIdentityUser), _publishedValueFallback);
        }

        public async Task<MemberIdentityUser> CreateMemberWithIdentityAsync(string name, string email, string memberTypeAlias, IEnumerable<string> roles)
        {
            _memberService.CreateMemberWithIdentity(email, email, name ?? email, memberTypeAlias);

            var user = await _memberManager.FindByEmailAsync(email);
            await _memberManager.AddToRolesAsync(user, roles);

            return user;
        }
    }
}
