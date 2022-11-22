using AutAndAutV10.Models;
using Microsoft.AspNetCore.Mvc;

namespace AutAndAutV10.ViewComponents
{
    public class MemberLoginViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View("~/Views/Partials/MemberLogin.cshtml", new LoginModel());
        }
    }
}
