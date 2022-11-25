using Microsoft.Extensions.Options;
using ModelsBuilder;
using Umbraco.Cms.Core;
using Umbraco.Cms.Web.Common.Security;

namespace AutAndAutV10.ExternalAuthentication
{
    public class GoogleMemberExternalLoginProviderOptions : IConfigureNamedOptions<MemberExternalLoginProviderOptions>
    {
        public const string SchemeName = "Google";
        public void Configure(string name, MemberExternalLoginProviderOptions options)
        {
            if (name != Constants.Security.MemberExternalAuthenticationTypePrefix + SchemeName)
            {
                return;
            }

            Configure(options);
        }

        public void Configure(MemberExternalLoginProviderOptions options) =>
            options.AutoLinkOptions = new MemberExternalSignInAutoLinkOptions(

                autoLinkExternalAccount: true,

                defaultCulture: null,

                defaultIsApproved: true,

                defaultMemberTypeAlias: GoogleMember.ModelTypeAlias,

                defaultMemberGroups: Array.Empty<string>()
            )
            {
                OnAutoLinking = (autoLinkUser, loginInfo) =>
                {

                },
                OnExternalLogin = (user, loginInfo) =>
                {
                    return true;
                }
            };
    }
}
