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
		public RoomService(GameContext context, IMapper mapper) : base(context, mapper)
		{
		}

		public async Task Create(CreateRoomDto input, int managerId)
		{
			var isRoomExist = Context.Rooms.Any(r => r.Name == input.Name);

			if (isRoomExist)
			{
				throw new Exception($"Комната с названием {input.Name} уже существует");
			}

			var room = Mapper.Map<Room>(input);
			room.ManagerId = managerId;

			await Context.Rooms.AddAsync(room);
			await Context.SaveChangesAsync();
			await Enter(room.Id, input.Password, managerId);
		}

		public async Task Delete(int userId)
		{
			var user = await Context.Users.FindAsync(userId);
			var roomId = user?.CurrentRoomId;
			var room = await Context.Rooms.FindAsync(roomId) ?? throw new Exception($"Комната с Id = {roomId} не существует");
			var users = Context.Users.Where(u => u.CurrentRoomId == roomId);
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

		public async Task Enter(int roomId, string password, int userId)
		{
			if (string.IsNullOrWhiteSpace(password))
			{
				password = null;
			}
			var isExist = await Context.Users.AnyAsync(u => u.CurrentRoomId == roomId && u.Id == userId);
			if (isExist)
			{
				throw new Exception($"Пользователь с Id={userId} уже находится в комнате с Id={roomId}");
			}

			var isPassswordCorrect = await Context.Rooms.AnyAsync(r => r.Password == password && r.Id == roomId);
			if (!isPassswordCorrect)
			{
				throw new Exception($"Не верный пароль");
			}

			var user = await Context.Users.FindAsync(userId);
			user.CurrentRoomId = roomId;
			Context.Users.Update(user);
			await Context.SaveChangesAsync();
		}

		public async Task Exit(int userId)
		{
			var currentRoomId = await Context.Users.Where(u => u.Id == userId).Select(u => u.CurrentRoomId).FirstAsync() ?? 
				throw new Exception("Пользователь не находится в комнате");
			var usersCount = Context.Users.Count(u => u.CurrentRoomId == currentRoomId);
			if (usersCount == 1)
			{
				await Delete(userId);
			}
			else
			{
				var room = await Context.Rooms.FindAsync(currentRoomId);
				if (room.ManagerId == userId)
				{
					var newRoomManager = await Context.Users.FirstAsync(u => u.CurrentRoomId == currentRoomId && u.Id != userId);
					room.ManagerId = newRoomManager.Id;
					Context.Rooms.Update(room);
				}
			}

			var user = await Context.Users.FindAsync(userId);
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

		public async Task<CurrentRoomDto> GetCurrentRoom(int userId)
		{

			var player = await Context.Users.Include(u => u.CurrentRoom).FirstAsync(r => r.Id == userId);

			if (player.CurrentRoom is null)
			{
				return null;
			}

			var opponent = Context.Users.FirstOrDefault(u => u.CurrentRoomId == player.CurrentRoomId && u.Id != player.Id);

			return new CurrentRoomDto
			{
				IsGameStarted = player.CurrentRoom.CurrentGameId != null,
				IsPlayerRoomManager = player.CurrentRoom.ManagerId == userId,
				Player = Mapper.Map<UserDto>(player),
				Opponent = Mapper.Map<UserDto>(opponent),
				RoomName = player.CurrentRoom.Name,
				Id = player.CurrentRoom.Id
			};
		}
	}
}
