using AutoMapper;
using FluentValidation;
using GameApp.Domain;
using GameApp.Domain.Models;
using GameApp.WebApi.Services.Games.Dto;
using Microsoft.EntityFrameworkCore;
using System.Linq;

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

		public async Task<StepInfoDto> DoStepAsync(int cellsNumber, int userId, int opponentId)
		{
			
            var user = await Context.Users.Include(u => u.CurrentRoom).FirstAsync(r => r.Id == userId);
			if (user is null)
			{
                throw new Exception($"Игрока с Id={userId} не существует");
            }

			var game = await Context.Games.FirstAsync(g => g.Id == user.CurrentRoom.CurrentGameId && g.RoomId == user.CurrentRoom.Id);
			if (game.WhoseMoveId != userId)
			{
                throw new Exception("Ход другого игрока");
            }

            var gameProgressByGame = await Context.GameProgresses.Where(gp => gp.GameId == game.Id).ToListAsync();
			bool isCellBusy = gameProgressByGame.Any(gp => gp.Cell == cellsNumber);
			if (isCellBusy)
			{
                throw new Exception($"Ячейка {cellsNumber} занята");
            }
			var numberOfSteps = gameProgressByGame.Count();

			if (numberOfSteps >= Utils.Constants.maxCell)  // или 8?
			{
                throw new Exception($"Игра {game.Id} завершена");
            }

            game.WhoseMoveId = opponentId;

			var gameProgress = new GameProgress()
			{
				GameId = game.Id,
				UserId = userId,
				Cell = cellsNumber,
				StrokeNumber = numberOfSteps + 1
			};

			var gameProgressByUser = gameProgressByGame.Where(gp => gp.UserId == userId).Select(gp => gp.Cell).ToList();
			gameProgressByUser.Add(cellsNumber);
			var hash = new HashSet<int>(gameProgressByUser);
            var winnerPositions = new WinningCombinationsDto();
			bool isWin = false;

			foreach (var position in winnerPositions.Horizontal)
			{
                if (hash.IsSupersetOf(position))
                {
					isWin = true;
					// Горизонтальная победа
                }
            }

            foreach (var position in winnerPositions.Vertical)
            {
                if (hash.IsSupersetOf(position))
                {
                    isWin = true;
                    // Вертикальная победа
                }
            }

            if (hash.IsSupersetOf(winnerPositions.LeftDiagonal))
            {
                isWin = true;
                // Левая диагональная победа
            }

            if (hash.IsSupersetOf(winnerPositions.RightDiagonal))
            {
                isWin = true;
                // Правая диагональная победа
            }

            if (isWin)
			{
                game.WhoseMoveId = null;
				game.WinnerId = userId;
            }

            Context.GameProgresses.Add(gameProgress);
            Context.Games.Update(game);
            await Context.SaveChangesAsync();

            return new StepInfoDto
			{
				CellNumber = cellsNumber,
				IsGameFinished = game.WhoseMove == null
			};
        }
    }
}
