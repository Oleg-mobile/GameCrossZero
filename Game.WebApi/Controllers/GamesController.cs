using GameApp.WebApi.Hubs;
using GameApp.WebApi.Services.Games;
using GameApp.WebApi.Services.Games.Dto;
using GameApp.WebApi.Services.Users.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace GameApp.WebApi.Controllers
{
	public class GamesController : GameAppController
	{
		private readonly IGameService _gameService;
		private readonly IHubContext<GameHub> _gameHub;

		public GamesController(IGameService gameService, IHubContext<GameHub> gameHub = null)
		{
			_gameService = gameService;
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
		public async Task<IActionResult> Start(int roomId)
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

			//await _gameHub.Clients
			//	.Client(GameHub._connectionUsers[currentRoom.Opponent.Login])
			//	.SendAsync("PlayerEntered", playerData);

			return Ok(await _gameService.GetInfoAsync(roomId));
		}
	}
}
