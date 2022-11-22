using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Web;

namespace AutAndAutV10.ViewComponents
{
    [ViewComponent(Name = "TopNavigation")]
    public class TopNavigationViewComponent : ViewComponent
    {
        private readonly IUmbracoContextAccessor _umbracoContextAccessor;
        public TopNavigationViewComponent(IUmbracoContextAccessor umbracoContextAccessor)
        {
            _umbracoContextAccessor = umbracoContextAccessor;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View("TopNavigation");
        }
    }
}
