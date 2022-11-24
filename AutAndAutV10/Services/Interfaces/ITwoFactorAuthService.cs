using Microsoft.AspNetCore.Mvc;

namespace AutAndAutV10.Services.Interfaces
{
    public interface ITwoFactorAuthService
    {
        public Task<bool> IsEnabledTwoFactor(Guid memberKey);
        public Task<string> GetAccountSecretKeyAsync(string sessionGuidString);
        public bool ValidateTwoFactor(string accountSecretKey, string twoFactorCode);
        public Task<bool> TryLoginAndRedirectAsync(bool isValidCode, string sessionEmail);
        public Task<bool> IsTwoFactorEnabledAsync(Guid memberKey);

    }
}
