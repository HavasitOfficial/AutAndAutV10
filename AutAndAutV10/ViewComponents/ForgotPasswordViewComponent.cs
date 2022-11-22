using AutAndAutV10.Models;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Web;

namespace AutAndAutV10.ViewComponents
{
    [ViewComponent(Name = "ForgotPassword")]
    public class ForgotPasswordViewComponent : ViewComponent
    {
        private readonly IUmbracoContextAccessor _umbracoContextAccessor;
        public ForgotPasswordViewComponent(IUmbracoContextAccessor umbracoContextAccessor)
        {
            _umbracoContextAccessor = umbracoContextAccessor;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View("~/Views/Partials/ForgotPasswordRequest.cshtml", new ForgottenPasswordRequestModel());
        }
    }
}
