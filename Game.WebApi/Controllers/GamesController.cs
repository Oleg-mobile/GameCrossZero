using AutoMapper;
using GameApp.Domain;
using GameApp.Domain.Migrations;
using GameApp.Domain.Models;
using GameApp.WebApi.Dto.Games;
using GameApp.WebApi.Dto.Users;
using Microsoft.AspNetCore.Mvc;

namespace GameApp.WebApi.Controllers
{
    public class GamesController : GameAppController
    {
        public GamesController(GameContext context, IMapper mapper) : base(context, mapper)
        {
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Create(CreateGameDto input)
        {
            var isExist = Context.Games.Any(u => u.Id == input.Id);

            if (isExist)
            {
                return BadRequest($"Игра с Id = {input.Id} уже существует");
            }

            var game = Mapper.Map<Game>(input);
            await Context.Games.AddAsync(game);
            await Context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Start(int roomId)
        {
            var isExist = Context.Rooms.Any(r => r.Id == roomId);

            // TODO в валидатор
            if (!isExist)
            {
                return BadRequest($"Комната с Id = {roomId} не существует");
            }

            var playersInRoom = Context.Users.Count(u => u.CurrentRoomId == roomId);
            if (playersInRoom < Utils.Constants.maxNumberOfPlayers)
            {
                return BadRequest($"В комнате не достаточно игроков для игры");
            }

            var playersReadyToPlay = Context.Users.Count(u => u.isReadyToPlay == true);
            if (playersReadyToPlay < Utils.Constants.maxNumberOfPlayers)
            {
                return BadRequest($"Не все игроки готовы играть");
            }

            Random shapeSelection = new Random();
            var shape = shapeSelection.Next(2) == 1;
            var players = Context.Users.Where(u => u.CurrentRoomId == roomId).ToList();
            var firsrPlayer = players[0];
            var secondPlayer = players[1];

            var game = new CreateGameDto();
            if (shape)
            {
                game.WhoseMoveId = firsrPlayer.Id;
            }
            else
            {
                game.WhoseMoveId = secondPlayer.Id;
            }
            game.RoomId = roomId;
            Create(game);

            // Игры пользователей
            var userGame = new UserGame();


            // Прогресс игры
            var gameProgress = new GameProgress();

            return Ok();
        }
    }
}
