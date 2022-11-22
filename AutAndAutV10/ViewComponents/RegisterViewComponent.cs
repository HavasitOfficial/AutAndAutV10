using AutAndAutV10.Models;
using Microsoft.AspNetCore.Mvc;

namespace AutAndAutV10.ViewComponents
{
    public class RegisterViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View("~/Views/Partials/RegisterPartial.cshtml", new RegisterModel());
        }
    }
}
