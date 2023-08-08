using Microsoft.AspNetCore.Mvc;

namespace GameApp.Mvc.Controllers
{
	public class AccountController : Controller
	{
		public IActionResult Login()
		{
			return View();
		}
	}
}
