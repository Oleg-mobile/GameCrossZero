using AutoMapper;
using FluentValidation;
using GameApp.Domain;
using GameApp.Domain.Models;
using GameApp.WebApi.Services.Rooms.Dto;
using GameApp.WebApi.Utils;
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

        public async Task Create(CreateRoomDto input)
        {
            var isExist = Context.Rooms.Any(r => r.Name == input.Name);

            if (isExist)
            {
                throw new Exception($"Комната с названием {input.Name} уже существует");
            }

            var room = Mapper.Map<Room>(input);

            await Context.Rooms.AddAsync(room);
            await Context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var room = await Context.Rooms.FindAsync(id);

            if (room is null)
            {
                throw new Exception($"Комната с Id = {id} не существует");
            }

            Context.Rooms.Remove(room);
            await Context.SaveChangesAsync();
        }

        public async Task Enter(EnterRoomDto input)
        {
            try
            {
                _enterRoomValidator.ValidateAndThrow(input);
            }
            catch (ValidationException ex)
            {
                var message = ex.Errors?.First().ErrorMessage ?? ex.Message;
                throw new Exception(message);
            }

            // TODO Добавить в валидатор?
            var countPlayersInRoom = Context.Users.Count(u => u.CurrentRoomId == input.RoomId);
            if (countPlayersInRoom >= Constants.maxNumberOfPlayers)
            {
                throw new Exception($"Превышено количество пользователей для комнаты");
            }
            // TODO Добавить в валидатор?
            var room = await Context.Rooms.FindAsync(input.RoomId);
            if (!string.IsNullOrEmpty(room.Password) && room.Password != input.Password)
            {
                throw new Exception("Не верный пароль");
            }

            var user = await Context.Users.FindAsync(input.UserId);
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
            user.isReadyToPlay = false;
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
                IsProtected = r.Password == null ? false : true,
                CountPlayersInRoom = Context.Users.Count(u => u.CurrentRoomId == r.Id)
            }).ToListAsync());
        }
    }
}
