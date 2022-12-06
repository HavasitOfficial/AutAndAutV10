using AutAndAutV10.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace AutAndAutV10.Services
{
    public class AccountDataService : IAccountDataService
    {
        public string GetUserEmail(AuthenticateResult authenticateResult)
        {
            return authenticateResult.Principal.FindFirstValue(ClaimTypes.Email)
                ?? authenticateResult.Principal.FindFirstValue("email")
                ?? throw new Exception("Missing email claim");
        }

        public string GetUserName(AuthenticateResult authenticateResult)
        {
            return authenticateResult.Principal.FindFirstValue(ClaimTypes.Name)
                ?? authenticateResult.Principal.FindFirstValue("name")
                ?? throw new Exception("Missing email claim");
        }
    }
}
