using Microsoft.AspNetCore.Mvc;

namespace MyAcademy_JWT.ViewComponents.UILayout
{
    public class _UILayoutScriptViewComponent:ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
