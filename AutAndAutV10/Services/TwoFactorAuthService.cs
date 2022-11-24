using AutAndAutV10.Services.Interfaces;
using AutAndAutV10.TwoFactor;
using Google.Authenticator;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Web.Common.Security;

namespace AutAndAutV10.Services
{
    public class TwoFactorAuthService : ITwoFactorAuthService
    {
        private readonly ITwoFactorLoginService _twoFactorLoginService;
        private readonly IMemberManager _memberManager;
        private readonly IMemberSignInManager _memberSignInManager;
        public TwoFactorAuthService(ITwoFactorLoginService twoFactorLoginService, IMemberManager memberManager, IMemberSignInManager memberSignInManager)
        {
            _twoFactorLoginService = twoFactorLoginService;
            _memberManager = memberManager;
            _memberSignInManager = memberSignInManager;
        }
        public Task<bool> IsEnabledTwoFactor(Guid memberKey)
        {
            return _twoFactorLoginService.IsTwoFactorEnabledAsync(memberKey);
        }
        public async Task<string> GetAccountSecretKeyAsync(string sessionGuidString, string sessionMemberKey)
        {
            Guid.TryParse(sessionGuidString, out var sessionGuid);
            var sercretKey = await _twoFactorLoginService.GetSecretForUserAndProviderAsync(sessionGuid, nameof(UmbracoAppAuthenticator));

            return sercretKey ?? string.Empty;
        }

        public bool ValidateTwoFacthor(string accountSecretKey, string twoFactorCode)
        {
            var tfa = new TwoFactorAuthenticator();
            return tfa.ValidateTwoFactorPIN(accountSecretKey, twoFactorCode);
        }

        public async Task<bool> TryLoginAndRedirectAsync(bool isValidCode, string sessionEmail)
        {
            if (isValidCode && sessionEmail != null)
            {
                var twoFactorMember = await _memberManager.FindByEmailAsync(sessionEmail);
                if (_memberSignInManager.SignInAsync(twoFactorMember, false).IsCompletedSuccessfully)
                {
                    return true;
                }
            }
            return false;
        }

        public Task<bool> IsTwoFactorEnabledAsync(Guid memberKey)
        {
            return _twoFactorLoginService.IsTwoFactorEnabledAsync(memberKey);
        }
    }
}
