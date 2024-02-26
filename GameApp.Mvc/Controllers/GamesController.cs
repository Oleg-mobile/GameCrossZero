using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameApp.Mvc.Controllers
{
    [Authorize]
    public class GamesController : Controller
    {
        [Route("[controller]/{roomId:int}")]
        public IActionResult Index(int roomId)  // TODO красивые ссылки
        {
            return View(roomId);
        }
    }
}
