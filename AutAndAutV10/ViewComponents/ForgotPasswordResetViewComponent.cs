using AutAndAutV10.Models;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Web;

namespace AutAndAutV10.ViewComponents
{
    public class ForgotPasswordResetViewComponent : ViewComponent
    {
        private readonly IUmbracoContextAccessor _umbracoContextAccessor;
        public ForgotPasswordResetViewComponent(IUmbracoContextAccessor umbracoContextAccessor)
        {
            _umbracoContextAccessor = umbracoContextAccessor;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View("~/Views/ForgotPasswordReset.cshtml", new ForgottenPasswordResetModel());
        }
    }
}
