using Microsoft.AspNetCore.Mvc;

namespace GameApp.Mvc.Controllers
{
    public class RoomsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
