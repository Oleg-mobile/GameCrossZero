using Microsoft.AspNetCore.Mvc;

namespace GameApp.WebApi.Controllers
{
	[Route("api/[controller]")]   //  TODO [HttpPost("[action]")]  ???
	[ApiController]
	public abstract class GameAppController : ControllerBase
	{
		protected string? CurrentUserLogin => User.Identity?.Name;

		protected GameAppController()
		{
		}
	}
}
