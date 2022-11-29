using AutAndAutV10.Models;
using AutAndAutV10.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Logging;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Cms.Web.Common.Filters;
using Umbraco.Cms.Web.Common.Security;
using Umbraco.Cms.Web.Website.Controllers;
using Member = ModelsBuilder.Member;
using RegisterModel = AutAndAutV10.Models.RegisterModel;

namespace AutAndAutV10.Controllers
{
    public class MemberController : SurfaceController
    {
        private readonly IMemberManager _memberManager;
        private readonly IMemberService _memberService;
        private readonly IUserAccountService _userAccountService;
        private readonly IMemberSignInManager _memberSignInManager;
        private readonly ITwoFactorAuthService _twoFactorAuthService;

        public const string SessionMemberKey = "_MemberKey";
        public const string SessionMemberEmail = "_MemberEmail";

        public MemberController(
            IUmbracoContextAccessor umbracoContextAccessor,
            IUmbracoDatabaseFactory databaseFactory,
            ServiceContext services,
            AppCaches appCaches,
            IProfilingLogger profilingLogger,
            IPublishedUrlProvider publishedUrlProvider,
            IMemberManager memberManager,
            IMemberService memberService,
            IMemberSignInManager memberSignInManager,
            IUserAccountService userAccountService,
            ITwoFactorAuthService twoFactorAuthService)
            : base(umbracoContextAccessor, databaseFactory, services, appCaches, profilingLogger, publishedUrlProvider)
        {
            _memberManager = memberManager;
            _memberService = memberService;
            _memberSignInManager = memberSignInManager;
            _userAccountService = userAccountService;
            _twoFactorAuthService = twoFactorAuthService;
        }

        public IActionResult RenderLogin()
        {
            return PartialView("MemberLogin", new LoginModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitLogin(LoginModel model)
        {
            if (ModelState.IsValid)
            {
				if (await _memberManager.ValidateCredentialsAsync(model.Username, model.Password))
                {
					MemberIdentityUser currentMember = await _memberManager.FindByEmailAsync(model.Username);
					var isEnabledTwoFactor = _twoFactorAuthService.IsTwoFactorEnabledAsync(currentMember.Key);
                    if (isEnabledTwoFactor.Result)
                    {
                        var sessionKey = currentMember.Key.ToString();
                        HttpContext.Session.SetString(SessionMemberKey, sessionKey);
                        HttpContext.Session.SetString(SessionMemberEmail, model.Username);
                        return Redirect("/login/twofactorvalidatePage");
                    }
                    else
                    {
                        var result = await _memberSignInManager.PasswordSignInAsync(model.Username, model.Password, false, true);
                        if (result.Succeeded) return Redirect("/");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "The username or password is invalid.");
                }
            }
            return CurrentUmbracoPage();
        }
		public async Task<IActionResult> TwoFactorValidatePage()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> VerifyAuthCode(TwoFactoryValidateModel twoFactory)
		{
            if (twoFactory == null || string.IsNullOrEmpty(twoFactory.Code))
            {
                return BadRequest();
            }
            var twoFactorcode = twoFactory.Code;
            var sessionMemberGuid = HttpContext.Session.GetString(SessionMemberKey);

            var accountSecretKey =await _twoFactorAuthService.GetAccountSecretKeyAsync(sessionMemberGuid ?? string.Empty);
            var isValidCode = _twoFactorAuthService.ValidateTwoFactor(accountSecretKey, twoFactorcode);
            var sessionEmail = HttpContext.Session.GetString(SessionMemberEmail);

            if (await _twoFactorAuthService.TryLoginAndRedirectAsync(isValidCode, sessionEmail ?? string.Empty))
            {
                return Redirect("/");
            }

            ModelState.AddModelError("", "The Code is invalid.");
            return CurrentUmbracoPage();
        }

		[HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateUmbracoFormRouteString]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return CurrentUmbracoPage();
            }

            if (await _memberManager.FindByEmailAsync(model.Email) != null)
            {
                TempData["Success"] = false;
                TempData["errorMessage"] = "User already registered!";

                return CurrentUmbracoPage();
            }

            var identityMember = MemberIdentityUser.CreateNew(model.Email, model.Email, Member.ModelTypeAlias, true, model.Name);
            var identityResult = await _memberManager.CreateAsync(identityMember, model.Password);

            if (identityResult.Succeeded)
            {
                var member = _memberService.GetByKey(identityMember.Key);

                _memberService.AssignRoles(new[] { member.Username }, new[] { "Member" });
                _memberService.Save(member);

                TempData["Success"] = true;
                await _memberSignInManager.SignInAsync(identityMember, false);
            }
            return Redirect("/");
        }

        [HttpPost]
        public async Task<IActionResult> SubmitLogout()
        {
            TempData.Clear();
            await _memberSignInManager.SignOutAsync();
            return Redirect("/");
        }

        [Authorize]
        public IActionResult RenderSecurityFromMember()
        {
            return PartialView("SecurityUserPage");
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ForgottenPasswordRequest(ForgottenPasswordRequestModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var member = await _memberManager.FindByEmailAsync(model.MemberEmail);

            if (member != null)
            {
                _userAccountService.GeneratePasswordToken(model.MemberEmail, member);
            }
            return Redirect("/");
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ForgottenPasswordReset(ForgottenPasswordResetModel model)
        {
            
            if (model.Password != model.ConfirmPassword)
            {
                ModelState.AddModelError("", "The passwords do not match.");
                return CurrentUmbracoPage();
            }

            string memberToken = HttpContext.Request.Query["t"].ToString();
            if (string.IsNullOrEmpty(memberToken))
            {
                ModelState.AddModelError("", "The Code is invalid.");
                return CurrentUmbracoPage();
            }
            model.ForgottenPasswordToken = memberToken;

            var member = await _memberManager.FindByEmailAsync(model.Email);
            if (member == null )
            {
                return BadRequest();
            }
            await _userAccountService.CheckEmailAndPasswordToken(model, member);
            await _memberSignInManager.PasswordSignInAsync(member.UserName, model.Password, false, true);

            return Redirect("/");
        }

        [AllowAnonymous]
        public async Task<IActionResult> RedirectToForgotPage()
        {
            return Redirect("/login/forgotpassword");
        }
    }
}
