using GameApp.WebApi.Hubs;
using GameApp.WebApi.Services.Games;
using GameApp.WebApi.Services.Games.Dto;
using GameApp.WebApi.Services.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace GameApp.WebApi.Controllers
{
	public class GamesController : GameAppController
	{
		private readonly IGameService _gameService;
		private readonly IUserService _userService;
		private readonly IHubContext<GameHub> _gameHub;

		public GamesController(IGameService gameService, IUserService userService, IHubContext<GameHub> gameHub = null)
		{
			_gameService = gameService;
			_userService = userService;
			_gameHub = gameHub;
		}

		[HttpPost("[action]")]
		public async Task<IActionResult> Create(CreateGameDto input)
		{
			try
			{
				await _gameService.CreateAsync(input);
				return Ok();
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpPost("[action]")]
		public async Task<IActionResult> Start([FromQuery] int roomId)
		{
			try
			{
				await _gameService.StartAsync(roomId);
				return Ok();
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpGet("[action]")]
		[ProducesResponseType(typeof(InfoGameDto), StatusCodes.Status200OK)]
		public async Task<IActionResult> GetInfo(int roomId)
		{
			var userId = await _userService.GetId(User.Identity!.Name!);

			return Ok(await _gameService.GetInfoAsync(roomId, userId));
		}
	}
}
