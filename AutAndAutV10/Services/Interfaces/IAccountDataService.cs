using Microsoft.AspNetCore.Authentication;
using Umbraco.Cms.Core.Security;

namespace AutAndAutV10.Services.Interfaces
{
    public interface IAccountDataService
    {
        string GetUserEmail(AuthenticateResult authenticateResult);
        string GetUserName(AuthenticateResult authenticateResult);
    }
}
