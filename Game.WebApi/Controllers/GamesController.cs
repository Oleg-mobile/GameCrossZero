using AutoMapper;
using GameApp.Domain;
using GameApp.Domain.Migrations;
using GameApp.Domain.Models;
using GameApp.WebApi.Services.Games;
using GameApp.WebApi.Services.Games.Dto;
using Microsoft.AspNetCore.Mvc;

namespace GameApp.WebApi.Controllers
{
    public class GamesController : GameAppController
    {
        private readonly IGameService _gameService;

        public GamesController(GameContext context, IMapper mapper, IGameService gameService) : base(context, mapper)
        {
            _gameService = gameService;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Create(CreateGameDto input)
        {
            try
            {
                await _gameService.Create(input);
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
                await _gameService.Start(roomId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
