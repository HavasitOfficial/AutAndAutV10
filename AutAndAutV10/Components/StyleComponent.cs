using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.WebAssets;

namespace AutAndAutV10.Components
{
    public class StyleComponent : IComponent
    {
        private readonly IRuntimeMinifier _runtimeMinifier;

        public StyleComponent(IRuntimeMinifier runtimeMinifier) => _runtimeMinifier = runtimeMinifier;

        public void Initialize()
        {
            _runtimeMinifier.CreateCssBundle("main-css-bundle",
                BundlingOptions.NotOptimizedAndComposite,
                new[]
                {
                    "~/css/footer.css",
                    "~/css/header.css",
                    "~/css/home.css",
                    "~/css/login.css",
                    "~/css/styles.css",
                    "~/css/topNavigation.css",
                    "~/css/contentpage.css"
                });
        }

        public void Terminate()
        {
            throw new System.NotImplementedException();
        }
    }
    public class MyComposer : ComponentComposer<StyleComponent> { }
}
