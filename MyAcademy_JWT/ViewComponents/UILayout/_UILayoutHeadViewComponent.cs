using Microsoft.AspNetCore.Mvc;

namespace MyAcademy_JWT.ViewComponents.UILayout
{
    public class _UILayoutHeadViewComponent:ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
