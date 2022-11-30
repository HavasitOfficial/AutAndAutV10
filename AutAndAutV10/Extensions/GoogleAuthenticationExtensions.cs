using AutAndAutV10.ExternalAuthentication;

namespace AutAndAutV10.Extensions
{
    public static class GoogleAuthenticationExtensions
    {
        //user login
        public static IUmbracoBuilder AddMemberGoogleAuthentication(this IUmbracoBuilder builder)
        {
            // Register GoogleMemberExternalLoginProviderOptions here rather than require it in startup
            builder.Services.ConfigureOptions<GoogleMemberExternalLoginProviderOptions>();

            builder.AddMemberExternalLogins(logins =>
            {
                logins.AddMemberLogin(
                    memberAuthenticationBuilder =>
                    {
                        memberAuthenticationBuilder.AddGoogle(
                            // The scheme must be set with this method to work for the Umbraco members
                            memberAuthenticationBuilder.SchemeForMembers(GoogleMemberExternalLoginProviderOptions.SchemeName),
                            options =>
                            {
                                options.ClientId = "427005332189-5b5kvqt54e00juad81qd6drplm3ju542.apps.googleusercontent.com";
                                options.ClientSecret = "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";
                            });
                    });
            });
            return builder;
        }
    }
}
