using Microsoft.AspNetCore.Mvc;

namespace AutAndAutV10.Services.Interfaces
{
    public interface ITwoFactorAuthService
    {
        public Task<bool> IsEnabledTwoFactor(Guid memberKey);
        public Task<string> GetAccountSecretKeyAsync(string sessionGuidString, string sessionMemberKey);
        public bool ValidateTwoFacthor(string accountSecretKey, string twoFactorCode);
        public Task<bool> TryLoginAndRedirectAsync(bool isValidCode, string sessionEmail);

    }
}
