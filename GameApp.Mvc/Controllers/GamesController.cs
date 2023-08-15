using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameApp.Mvc.Controllers
{
    [Authorize]
    public class GamesController : Controller
    {
        public IActionResult Index() 
        {
            return View();
        }
    }
}
