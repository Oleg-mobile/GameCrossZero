using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameApp.Mvc.Controllers
{
    [Authorize]
    public class RoomsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
