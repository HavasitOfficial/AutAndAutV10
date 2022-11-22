using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Security;

namespace AutAndAutV10.TwoFactor
{
    public class UmbracoAppAuthenticatorComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            var identityBuilder = new MemberIdentityBuilder(builder.Services);
            identityBuilder.AddTwoFactorProvider<UmbracoAppAuthenticator>(UmbracoAppAuthenticator.Name);
        }
    }
}
