using AutAndAutV10.Models;
using ModelsBuilder;
using Umbraco.Cms.Core.Security;

namespace AutAndAutV10.Services.Interfaces
{
    public interface IUserAccountService
    {
        void GeneratePasswordToken(string memberEmail, Umbraco.Cms.Core.Security.MemberIdentityUser memberIdentityUser);
        Task CheckEmailAndPasswordToken(ForgottenPasswordResetModel passwordModel, MemberIdentityUser getCurrentMemberAsync);
        Member GetMemberProfile(MemberIdentityUser memberIdentityUser);
    }
}
