using AutAndAutV10.ExternalAuthentication;

namespace AutAndAutV10.Extensions
{
    public static class FacebookAuthenticationExtensions
    {
        public static IUmbracoBuilder AddMemberFacebookAuthentication(this IUmbracoBuilder builder)
        {
            // Register GoogleMemberExternalLoginProviderOptions here rather than require it in startup
            builder.Services.ConfigureOptions<FacebookMemberExternalLoginProviderOptions>();

            builder.AddMemberExternalLogins(logins =>
            {
                logins.AddMemberLogin(
                    memberAuthenticationBuilder =>
                    {
                        memberAuthenticationBuilder.AddFacebook(
                            // The scheme must be set with this method to work for the Umbraco members
                            memberAuthenticationBuilder.SchemeForMembers(FacebookMemberExternalLoginProviderOptions.SchemeName),
                            options =>
                            {
                                options.AppId = "649814716675785";
                                options.AppSecret = "5cb68907a46e08c4b2d6be7527a057a1";
                            });
                    });
            });
            return builder;
        }
    }
}
