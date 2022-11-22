using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
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
        private readonly IMemberService _memberService;
        private readonly IMemberSignInManager _memberSignInManager;
        private readonly IScopeProvider _coreScopeProvider;
        private readonly ITwoFactorLoginService _twoFactorLoginService;

        public const string SessionMemberKey = "_MemberKey";
        public const string SessionMemberEmail = "_MemberEmail";

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
            IScopeProvider coreScopeProvider,
            ITwoFactorLoginService twoFactorLoginService)
            : base(umbracoContextAccessor, databaseFactory, services, appCaches, profilingLogger, publishedUrlProvider)
        {
            _memberManager = memberManager;
            _memberService = memberService;
            _memberSignInManager = memberSignInManager;
            _coreScopeProvider = coreScopeProvider;
            _twoFactorLoginService = twoFactorLoginService;
        }

        //google user login
        public IActionResult GoogleLogin(string returnUrl)
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = Url.Action("GoogleResponse"),
                Items = { { "returnUrl", returnUrl } }
            };

            return Challenge(properties, Constants.Security.MemberExternalAuthenticationTypePrefix + GoogleDefaults.AuthenticationScheme);
        }

        //google user login
        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync("Identity.External");

            if (!result.Succeeded) throw new Exception("Missing external cookie");

            var email = result.Principal.FindFirstValue(ClaimTypes.Email)
                ?? result.Principal.FindFirstValue("email")
                ?? throw new Exception("Missing email claim");

            var user = await _memberManager.FindByEmailAsync(email);

            var name = result.Principal.FindFirstValue(ClaimTypes.Name)
                ?? result.Principal.FindFirstValue("name")
                ?? throw new Exception("Missing email claim");

            if (user == null)
            {
                _memberService.CreateMemberWithIdentity(email, email, name ?? email, GoogleMember.ModelTypeAlias);

                user = await _memberManager.FindByEmailAsync(email);
                await _memberManager.AddToRolesAsync(user, new[] { "GoogleUser" });
            }
            var isEnabledTwoFactor = _twoFactorLoginService.IsTwoFactorEnabledAsync(user.Key);
            if (isEnabledTwoFactor.Result)
            {
                var sessionKey = user.Key.ToString();
                HttpContext.Session.SetString(SessionMemberKey, sessionKey);
                HttpContext.Session.SetString(SessionMemberEmail, user.Email);
                return Redirect("/login/twofactorvalidatePage");
            }
            else
            {
                await HttpContext.SignOutAsync("Identity.External");
                await _memberSignInManager.SignInAsync(user, false);
            }

            var returnUrl = result.Properties?.Items["returnUrl"];
            if (returnUrl == null || !Url.IsLocalUrl(returnUrl)) returnUrl = "~/";
            return new RedirectResult(returnUrl);
        }
    }
}
