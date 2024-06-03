using AutoMapper;
using FluentValidation;
using GameApp.Domain;
using GameApp.Domain.Models;
using GameApp.WebApi.Services.Games.Dto;
using Microsoft.EntityFrameworkCore;

namespace GameApp.WebApi.Services.Games
{
	public class GameService : GameAppService, IGameService
	{
		private readonly IValidator<int> _startGameValidator;

		public GameService(GameContext context, IMapper mapper, IValidator<int> startGameValidator) : base(context, mapper)
		{
			_startGameValidator = startGameValidator;
		}

		public async Task<int> CreateAsync(CreateGameDto input)
		{
            var room = await Context.Rooms.FindAsync(input.RoomId);
            if (room.CurrentGameId is not null)
            {
                throw new Exception($"В комнате с id = {input.RoomId} игра уже создана");
            }

            var game = Mapper.Map<Game>(input);
            await Context.Games.AddAsync(game);
            await Context.SaveChangesAsync();

            return game.Id;
        }

		public async Task StartAsync(int roomId)
		{
			try
			{
				_startGameValidator.ValidateAndThrow(roomId);
			}
			catch (ValidationException ex)
			{
				var message = ex.Errors?.First().ErrorMessage ?? ex.Message;
				throw new Exception(message);
			}

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
			int gameId = await CreateAsync(game);
			await Context.SaveChangesAsync();

			var userGame = new UserGame
			{
				GameId = gameId,
				UserId = firsrPlayer.Id,
				IsCross = isCross
			};
			Context.UserGames.Add(userGame);

			userGame = new UserGame
			{
				GameId = gameId,
				UserId = secondPlayer.Id,
				IsCross = !isCross
			};
			Context.UserGames.Add(userGame);

			var room = await Context.Rooms.FindAsync(roomId);
			room.CurrentGameId = gameId;
			Context.Rooms.Update(room);

			await Context.SaveChangesAsync();
		}

		public async Task<InfoGameDto> GetInfoAsync(int roomId, int userId)
		{
			var room = await Context.Rooms.FindAsync(roomId);
			if (!room.CurrentGameId.HasValue)
			{
				throw new Exception("У данной комнаты нет игры");
			}

			var game = await Context.Games.FindAsync(room.CurrentGameId);

			var userGame = await Context.UserGames.FirstAsync(ug => ug.GameId == room.CurrentGameId && ug.UserId == userId);

			// TODO Include!

			var gameProgress = await Context.GameProgresses.Where(gp => gp.GameId == game.Id).ToListAsync();

			var steps = gameProgress.Select(gp => new StepDto
			{
				IsCross = gp.UserId == userId ? userGame.IsCross : !userGame.IsCross,
				CellNumber = gp.Cell
			});

			return new InfoGameDto
			{
				IsMyStep = game!.WhoseMoveId == userId,
				IsMyFigureCross = userGame.IsCross,
				Steps = steps,
				GameId = game.Id
            };
		}

		public async Task DoStepAsync(int cellsNumber, int userId)
		{
            var gameProgress = await Context.GameProgresses.Where(gp => gp.UserId == userId).ToListAsync();


        }

    }
}
