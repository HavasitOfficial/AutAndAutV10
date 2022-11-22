using AutAndAutV10.Models;
using AutAndAutV10.Services.Interfaces;
using ModelsBuilder;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Web;

namespace AutAndAutV10.Services
{
    public class UserAccountService : IUserAccountService
    {
        private readonly ISendEmailService _sendEmailService;
        private readonly IForgottenPasswordService _forgottenPasswordService;
        private readonly IPublishedValueFallback _publishedValueFallback;
        private readonly IMemberManager _memberManager;
        private readonly ISiteSettingsService _siteSettingsService;
        private readonly IUmbracoContextAccessor _context;

        private const string EMAIL_VIEW_PATH = "D:/Egyetem/AutAndAutV9/Views/Partials/ForgotPasswordLink.cshtml";
        private const string EMAIL_SUBJECT = "Password Reset";
        public UserAccountService(ISendEmailService sendEmailService, IForgottenPasswordService forgottenPasswordService, IPublishedValueFallback publishedValueFallback, IMemberManager memberManager, ISiteSettingsService siteSettingsService, IUmbracoContextAccessor context)
        {
            _sendEmailService = sendEmailService;
            _forgottenPasswordService = forgottenPasswordService;
            _publishedValueFallback = publishedValueFallback;
            _memberManager = memberManager;
            _siteSettingsService = siteSettingsService;
            _context = context;
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
                {"{memberName}", memberName},
                {"{url}", url}
            };

            var message = new Message(memberEmail,
                EMAIL_SUBJECT, messageBody,
                dictionary);

            _sendEmailService.SendEmail(message);
        }

        private static string GetForgottenPasswordViewPath()
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), EMAIL_VIEW_PATH);
            return System.IO.File.Exists(filePath) ? System.IO.File.ReadAllText(filePath) : filePath;
        }
        public Member GetMemberProfile(MemberIdentityUser memberIdentityUser)
        {
            return new Member(_memberManager.AsPublishedMember(memberIdentityUser), _publishedValueFallback);
        }
    }
}
