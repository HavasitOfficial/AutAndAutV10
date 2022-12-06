using AutAndAutV10.Services;
using AutAndAutV10.Services.Interfaces;
using Umbraco.Cms.Core.Composing;

namespace AutAndAutV10.Composers
{
    public class ServicesComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.Components().Append<MigrationComposer>();
            builder.Services.AddTransient<ISendEmailService, SendEmailService>();
            builder.Services.AddScoped<IUserAccountService, UserAccountService>();
            builder.Services.AddTransient<ISiteSettingsService, SiteSettingsService>();
            builder.Services.AddTransient<ITwoFactorAuthService, TwoFactorAuthService>();
            builder.Services.AddTransient<IAccountDataService, AccountDataService>();

            builder.Services.AddScoped<IForgottenPasswordService, ForgottenPasswordService>();
        }
    }
}
