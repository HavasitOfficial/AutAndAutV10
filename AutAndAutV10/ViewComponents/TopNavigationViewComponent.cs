using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Web;

namespace AutAndAutV10.ViewComponents
{
    [ViewComponent(Name = "TopNavigation")]
    public class TopNavigationViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View("TopNavigation");
        }
    }
}
