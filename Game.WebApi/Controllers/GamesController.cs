using GameApp.WebApi.Services.Games;
using GameApp.WebApi.Services.Games.Dto;
using GameApp.WebApi.Services.Users.Dto;
using Microsoft.AspNetCore.Mvc;

namespace GameApp.WebApi.Controllers
{
	public class GamesController : GameAppController
	{
		private readonly IGameService _gameService;

		public GamesController(IGameService gameService)
		{
			_gameService = gameService;
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
			return Ok(await _gameService.GetInfoAsync(roomId));
		}
	}
}
