using Microsoft.AspNetCore.Mvc;

namespace GameApp.Mvc.Controllers
{
    public class GamesController : Controller
    {
        public IActionResult Index() 
        {
            return View();
        }
    }
}
