using AutAndAutV10.Models;
using Microsoft.AspNetCore.Mvc;

namespace AutAndAutV10.ViewComponents
{
    public class TwoFactorValidateViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View("TwoFactorValidate", new TwoFactoryValidateModel());
        }
    }
}
