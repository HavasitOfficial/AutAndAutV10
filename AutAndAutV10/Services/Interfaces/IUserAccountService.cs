using AutAndAutV10.Models;
using ModelsBuilder;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Security;

namespace AutAndAutV10.Services.Interfaces
{
    public interface IUserAccountService
    {
        void GeneratePasswordToken(string memberEmail, Umbraco.Cms.Core.Security.MemberIdentityUser memberIdentityUser);
        Task CheckEmailAndPasswordToken(ForgottenPasswordResetModel passwordModel, MemberIdentityUser getCurrentMemberAsync);
        ModelsBuilder.Member GetMemberProfile(MemberIdentityUser memberIdentityUser);
        Task<MemberIdentityUser> CreateMemberWithIdentityAsync(string name, string email, string memberTypeAlias, IEnumerable<string> roles);
    }
}
