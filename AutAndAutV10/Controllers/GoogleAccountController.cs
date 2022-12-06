using AutAndAutV10.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ModelsBuilder;
using System.Security.Claims;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Logging;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Scoping;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Cms.Web.Common.Security;
using Umbraco.Cms.Web.Website.Controllers;

namespace AutAndAutV10.Controllers
{
    public class GoogleAccountController : SurfaceController
    {
        private readonly IMemberManager _memberManager;
        private readonly IMemberSignInManager _memberSignInManager;
        private readonly ITwoFactorAuthService _twoFactorAuthService;
        private readonly IAccountDataService _accountDataService;
        private readonly IUserAccountService _userAccountService;

        public const string SessionMemberKey = "_MemberKey";
        public const string SessionMemberEmail = "_MemberEmail";
        private readonly IEnumerable<string> roles = new[] { "GoogleUser" };

        public GoogleAccountController(
            IUmbracoContextAccessor umbracoContextAccessor,
            IUmbracoDatabaseFactory databaseFactory,
            ServiceContext services,
            AppCaches appCaches,
            IProfilingLogger profilingLogger,
            IPublishedUrlProvider publishedUrlProvider,
            IMemberManager memberManager,
            IMemberService memberService,
            IMemberSignInManager memberSignInManager,
            ITwoFactorAuthService twoFactorAuthService,
            IAccountDataService accountDataService,
            IUserAccountService userAccountService)
            : base(umbracoContextAccessor, databaseFactory, services, appCaches, profilingLogger, publishedUrlProvider)
        {
            _memberManager = memberManager;
            _memberSignInManager = memberSignInManager;
            _twoFactorAuthService = twoFactorAuthService;
            _accountDataService = accountDataService;
            _userAccountService = userAccountService;
        }

        public IActionResult GoogleLogin()
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = Url.Action("GoogleResponse")
            };

            return Challenge(properties, Constants.Security.MemberExternalAuthenticationTypePrefix + GoogleDefaults.AuthenticationScheme);
        }

        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(IdentityConstants.ExternalScheme);

            if (!result.Succeeded) throw new Exception("Missing external cookie");

            var email = _accountDataService.GetUserEmail(result);
            var name = _accountDataService.GetUserName(result);

            var user = await _memberManager.FindByEmailAsync(email);

            if (user == null)
            {
                user = await _userAccountService.CreateMemberWithIdentityAsync(name, email, GoogleMember.ModelTypeAlias, roles);
            }

            var isEnabledTwoFactor = _twoFactorAuthService.IsTwoFactorEnabledAsync(user.Key);
            if (isEnabledTwoFactor.Result)
            {
                var sessionKey = user.Key.ToString();
                HttpContext.Session.SetString(SessionMemberKey, sessionKey);
                HttpContext.Session.SetString(SessionMemberEmail, user.Email);
                return Redirect("/login/twofactorvalidatePage");
            }

            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            await _memberSignInManager.SignInAsync(user, false);

            return Redirect("/");
        }
    }
}
