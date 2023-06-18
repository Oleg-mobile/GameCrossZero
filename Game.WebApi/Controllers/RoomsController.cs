using AutoMapper;
using FluentValidation;
using GameApp.Domain;
using GameApp.Domain.Models;
using GameApp.WebApi.Dto.Rooms;
using GameApp.WebApi.Utils;
using Microsoft.AspNetCore.Mvc;

namespace GameApp.WebApi.Controllers
{
    public class RoomsController : GameAppController
    {
        private readonly IValidator<ExitRoomDto> _exitRoomValidator;
        public RoomsController(GameContext context, IMapper mapper, IValidator<ExitRoomDto> exitRoomValidator) : base(context, mapper)
        {
            _exitRoomValidator = exitRoomValidator;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Create(CreateRoomDto input)
        {
            var isExist = Context.Rooms.Any(r => r.Name == input.Name);
            if (isExist)
            {
                return BadRequest($"Комната с названием {input.Name} уже существует");
            }

            var room = Mapper.Map<Room>(input);
            await Context.Rooms.AddAsync(room);
            await Context.SaveChangesAsync();
            return Ok();
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetAll()
        {
            //var roomsDto = Context.Rooms.ToList().Select(r => new
            //{
            //    r.Name,
            //    r.ManagerId,
            //    IsProtected = r.Password == null ? false : true,
            //    CountPlayersInRoom = Context.Users.Count(u => u.CurrentRoomId == r.Id)
            //});

            IQueryable<Room> query = Context.Rooms;
            var roomsDto = Mapper.Map<IEnumerable<GetRoomDto>>(query.Select(r => new GetRoomDto()
            {
                Name = r.Name,
                ManagerId = r.ManagerId,
                IsProtected = r.Password == null ? false : true,
                CountPlayersInRoom = Context.Users.Count(u => u.CurrentRoomId == r.Id)
            }).ToList());

            return Ok(roomsDto);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Enter(EnterRoomDto input)
        {
            var isRoomExist = Context.Rooms.Any(r => r.Id == input.RoomId);
            if (!isRoomExist)
            {
                return BadRequest($"Комната с Id = {input.RoomId} не существует");
            }

            var isUserExist = Context.Users.Any(u => u.Id == input.UserId);
            if (!isUserExist)
            {
                return BadRequest($"Пользователь с Id = {input.UserId} не существует");
            }

            var countPlayersInRoom = Context.Users.Count(u => u.CurrentRoomId == input.RoomId);
            if (countPlayersInRoom >= Constants.maxNumberOfPlayers)
            {
                return BadRequest($"Превышено количество пользователей для комнаты");
            }

            var isUserInRoom = Context.Users.Any(u => u.CurrentRoomId == input.RoomId && u.Id == input.UserId);
            if (isUserInRoom)
            {
                return BadRequest($"Пользователь с Id = {input.UserId} уже находится в комнате");
            }

            var room = Context.Rooms.Find(input.RoomId);
            if (!string.IsNullOrEmpty(room.Password) && room.Password != input.Password)
            {
                BadRequest("Не верный пароль");
            }

            var user = Context.Users.Find(input.UserId);
            user.CurrentRoomId = input.RoomId;
            Context.Users.Update(user);
            await Context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Exit(ExitRoomDto input)
        {
            try
            {
                _exitRoomValidator.ValidateAndThrow(input);
            }
            catch (ValidationException ex)
            {
                var message = ex.Errors?.First().ErrorMessage ?? ex.Message;
                return BadRequest(message);
            }

            var user = Context.Users.Find(input.UserId);
            user.CurrentRoomId = null;
            user.isReadyToPlay = false;
            Context.Users.Update(user);

            var listPlayersInRoom = Context.Users.Where(u => u.CurrentRoomId == input.RoomId).ToList();
            if (listPlayersInRoom.Count == 0)
            {
                // удалить комнату
            }

            var room = Context.Rooms.Find(input.RoomId);
            if (listPlayersInRoom.Count == 1 && room.ManagerId == input.UserId)
            {
                room.ManagerId = listPlayersInRoom[0].Id;
                Context.Rooms.Update(room);
            }

            await Context.SaveChangesAsync();
            return Ok();
        }
    }
}
