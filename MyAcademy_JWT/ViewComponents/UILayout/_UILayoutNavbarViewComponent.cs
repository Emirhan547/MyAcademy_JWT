using Microsoft.AspNetCore.Mvc;

namespace MyAcademy_JWT.ViewComponents.UILayout
{
    public class _UILayoutNavbarViewComponent:ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
