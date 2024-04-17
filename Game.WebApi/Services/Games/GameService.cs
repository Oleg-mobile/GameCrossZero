﻿using AutoMapper;
using FluentValidation;
using GameApp.Domain;
using GameApp.Domain.Models;
using GameApp.WebApi.Services.Games.Dto;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace GameApp.WebApi.Services.Games
{
    public class GameService : GameAppService, IGameService
    {
        private readonly IValidator<int> _startGameValidator;

        public GameService(GameContext context, IMapper mapper, IValidator<int> startGameValidator) : base(context, mapper)
        {
            _startGameValidator = startGameValidator;
        }

        public async Task CreateAsync(CreateGameDto input)
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
            await CreateAsync(game);
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

        public async Task<InfoGameDto> GetInfoAsync(int roomId, int userId)
        {
            var room = await Context.Rooms.FindAsync(roomId);
            if (!room.CurrentGameId.HasValue)
            {
                throw new Exception("У данной комнаты нет игры");
            }

			var game = await Context.Games.FindAsync(room.CurrentGameId);

            var userGame = await Context.UserGames.FirstAsync(ug => ug.GameId == room.CurrentGameId && ug.UserId == userId);
            
            // Include!

            var gameProgress = await Context.GameProgresses.FirstAsync(gp => gp.GameId == game.Id && gp.UserId == userId);

			var steps = new List<StepDto>
			{
				new StepDto
				{
					IsCross = userGame.IsCross,
					CellNumber = gameProgress.Cell
				}
			};

			return new InfoGameDto
            {
                WhoseMoveId = game!.WhoseMoveId,
                IsMyFigureCross = userGame.IsCross,
                Steps = steps
			};
        }
	}
}
