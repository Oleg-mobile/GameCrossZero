using AutoMapper;
using FluentValidation;
using GameApp.Domain;
using GameApp.Domain.Migrations;
using GameApp.Domain.Models;
using GameApp.WebApi.Services.Games.Dto;
using GameApp.WebApi.Services.Rooms.Dto;
using GameApp.WebApi.Validators.Rooms;

namespace GameApp.WebApi.Services.Games
{
    public class GameService : GameAppService, IGameService
    {
        private readonly IValidator<CreateGameDto> _createGameValidator;

        public GameService(GameContext context, IMapper mapper, IValidator<CreateGameDto> createGameValidator) : base(context, mapper)
        {
            _createGameValidator = createGameValidator;
        }

        public async Task Create(CreateGameDto input)
        {
            var isExist = Context.Games.Any(u => u.Id == input.Id);

            if (isExist)
            {
                throw new Exception($"Игра с Id = {input.Id} уже существует");
            }

            var game = Mapper.Map<Game>(input);
            await Context.Games.AddAsync(game);
            await Context.SaveChangesAsync();
        }

        public async Task Start(int roomId)
        {
            //TODO в валидатор
            var isExist = Context.Rooms.Any(r => r.Id == roomId);
            if (!isExist)
            {
                throw new Exception($"Комната с Id = {roomId} не существует");
            }

            var playersInRoom = Context.Users.Count(u => u.CurrentRoomId == roomId);
            if (playersInRoom < Utils.Constants.maxNumberOfPlayers)
            {
                throw new Exception($"В комнате не достаточно игроков для игры");
            }

            var playersReadyToPlay = Context.Users.Count(u => u.isReadyToPlay == true);
            if (playersReadyToPlay < Utils.Constants.maxNumberOfPlayers)
            {
                throw new Exception($"Не все игроки готовы играть");
            }

            //try
            //{
            //    _createGameValidator.ValidateAndThrow();
            //}
            //catch (ValidationException ex)
            //{
            //    var message = ex.Errors?.First().ErrorMessage ?? ex.Message;
            //    throw new Exception(message);
            //}

            var shapeSelection = new Random();
            var isCross = shapeSelection.Next(2) == 1;

            var players = Context.Users.Where(u => u.CurrentRoomId == roomId).ToList();
            var firsrPlayer = players[0];
            var secondPlayer = players[1];

            var game = new CreateGameDto();
            if (isCross)
            {
                game.WhoseMoveId = firsrPlayer.Id;
            }
            else
            {
                game.WhoseMoveId = secondPlayer.Id;
            }

            game.RoomId = roomId;
            await Create(game);
            Context.SaveChanges();

            var userGame = new UserGame
            {
                GameId = game.Id,
                UserId = firsrPlayer.Id,
                IsCross = isCross
            };
            Context.UserGames.Add(userGame);

            userGame = new UserGame
            {
                GameId = game.Id,
                UserId = secondPlayer.Id,
                IsCross = !isCross
            };
            Context.UserGames.Add(userGame);

            Context.SaveChanges();
        }
    }
}
