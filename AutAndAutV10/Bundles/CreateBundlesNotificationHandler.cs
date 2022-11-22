using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.WebAssets;
using Umbraco.Cms.Core;

namespace AutAndAutV10.Bundles
{
    public class CreateBundlesNotificationHandler : INotificationHandler<UmbracoApplicationStartingNotification>
    {
        private readonly IRuntimeMinifier _runtimeMinifier;
        private readonly IRuntimeState _runtimeState;
        public CreateBundlesNotificationHandler(IRuntimeState runtimeState, IRuntimeMinifier runtimeMinifier)
        {
            _runtimeState = runtimeState;
            _runtimeMinifier = runtimeMinifier;
        }
        public void Handle(UmbracoApplicationStartingNotification notification)
        {
            if (_runtimeState.Level == RuntimeLevel.Run)
            {
                _runtimeMinifier.CreateCssBundle("registered-css-bundle",
                    BundlingOptions.NotOptimizedAndComposite,
                    new[] {
                        "~/css/footer.css",
                    "~/css/header.css",
                    "~/css/home.css",
                    "~/css/login.css",
                    "~/css/styles.css",
                    "~/css/topNavigation.css", });
            }
        }
    }
}
