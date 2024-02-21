using AutoMapper;
using FluentValidation;
using GameApp.Domain;
using GameApp.Domain.Models;
using GameApp.WebApi.Services.Rooms.Dto;
using GameApp.WebApi.Services.Users.Dto;
using Microsoft.EntityFrameworkCore;

namespace GameApp.WebApi.Services.Rooms
{
	public class RoomService : GameAppService, IRoomService
	{
		private readonly IValidator<ExitRoomDto> _exitRoomValidator;
		private readonly IValidator<EnterRoomDto> _enterRoomValidator;

		public RoomService(GameContext context, IMapper mapper, IValidator<ExitRoomDto> exitRoomValidator, IValidator<EnterRoomDto> enterRoomValidator) : base(context, mapper)
		{
			_exitRoomValidator = exitRoomValidator;
			_enterRoomValidator = enterRoomValidator;
		}

		public async Task Create(CreateRoomDto input, int managerId)
		{
			var isExist = Context.Rooms.Any(r => r.Name == input.Name);

			if (isExist)
			{
				throw new Exception($"Комната с названием {input.Name} уже существует");
			}

			var room = Mapper.Map<Room>(input);
			room.ManagerId = managerId;

			await Context.Rooms.AddAsync(room);
			await Context.SaveChangesAsync();

			var enterRoomDto = Mapper.Map<EnterRoomDto>(input);
			enterRoomDto.RoomId = room.Id;
			await Enter(enterRoomDto, managerId);
		}

		public async Task Delete(int id)
		{
			var room = await Context.Rooms.FindAsync(id);

			if (room is null)
			{
				throw new Exception($"Комната с Id = {id} не существует");
			}

			var users = Context.Users.Where(u => u.CurrentRoomId == id);
			if (users is not null)
			{
				foreach (var u in users)
				{
					u.CurrentRoomId = null;
					Context.Users.Update(u);
				}
			}

			Context.Rooms.Remove(room);
			await Context.SaveChangesAsync();
		}

		public async Task Enter(EnterRoomDto input, int userId)
		{
			try
			{
				var isExist = await Context.Users.AnyAsync(u => u.CurrentRoomId == input.RoomId && u.Id == userId);
				if (isExist)
				{
					return;
				}

				_enterRoomValidator.ValidateAndThrow(input);
			}
			catch (ValidationException ex)
			{
				var message = ex.Errors?.First().ErrorMessage ?? ex.Message;
				throw new Exception(message);
			}

			var user = await Context.Users.FindAsync(userId);
			user.CurrentRoomId = input.RoomId;
			Context.Users.Update(user);
			await Context.SaveChangesAsync();
		}

		public async Task Exit(ExitRoomDto input)
		{
			try
			{
				_exitRoomValidator.ValidateAndThrow(input);
			}
			catch (ValidationException ex)
			{
				var message = ex.Errors?.First().ErrorMessage ?? ex.Message;
				throw new Exception(message);
			}

			var usersCount = Context.Users.Count(u => u.CurrentRoomId == input.RoomId);
			if (usersCount == 1)
			{
				await Delete(input.RoomId);
			}
			else
			{
				var room = await Context.Rooms.FindAsync(input.RoomId);
				if (room.ManagerId == input.UserId)
				{
					var newRoomManager = await Context.Users.FirstAsync(u => u.CurrentRoomId == input.RoomId && u.Id != input.UserId);
					room.ManagerId = newRoomManager.Id;
					Context.Rooms.Update(room);
				}
			}

			var user = await Context.Users.FindAsync(input.UserId);
			user.CurrentRoomId = null;
			user.IsReadyToPlay = false;
			Context.Users.Update(user);

			await Context.SaveChangesAsync();
		}

		public async Task<IEnumerable<RoomDto>> GetAll()
		{
			IQueryable<Room> query = Context.Rooms;

			return Mapper.Map<IEnumerable<RoomDto>>(await query.Select(r => new RoomDto()
			{
				Id = r.Id,
				Name = r.Name,
				ManagerId = r.ManagerId,
				IsProtected = !string.IsNullOrWhiteSpace(r.Password),
				CountPlayersInRoom = Context.Users.Count(u => u.CurrentRoomId == r.Id)
			}).ToListAsync());
		}

		public async Task<CurrentRoomDto> GetCurrentRoom(int playerId)
		{

			var player = await Context.Users.Include(u => u.CurrentRoom).FirstAsync(r => r.Id == playerId);

			if (player.CurrentRoom is null)
			{
				return null;
			}

			var opponent = Context.Users.FirstOrDefault(u => u.CurrentRoomId == player.CurrentRoomId && u.Id != player.Id);

			return new CurrentRoomDto
			{
				IsGameStarted = player.CurrentRoom.CurrentGameId != null,
				IsPlayerRoomManager = player.CurrentRoom.ManagerId == playerId,
				Player = Mapper.Map<UserDto>(player),
				Opponent = Mapper.Map<UserDto>(opponent),
				RoomName = player.CurrentRoom.Name
			};
		}
	}
}
