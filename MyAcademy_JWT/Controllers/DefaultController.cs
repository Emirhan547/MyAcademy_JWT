using Microsoft.AspNetCore.Mvc;

namespace MyAcademy_JWT.Controllers
{
    public class DefaultController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
