using AutoMapper;
using GameApp.Domain;
using GameApp.Domain.Models;
using GameApp.WebApi.Dto.Rooms;
using GameApp.WebApi.Utils;
using Microsoft.AspNetCore.Mvc;

namespace GameApp.WebApi.Controllers
{
    public class RoomsController : GamesController
    {
        public RoomsController(GameContext context, IMapper mapper) : base(context, mapper)
        {
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
            IQueryable<Room> query = Context.Rooms;
            var roomsDto = Mapper.Map<IEnumerable<CreateRoomDto>>(query.ToList()).Select(r => new
            {
                r.Name,
                r.ManagerId,
                IsProtected = r.Password == null ? false : true,
                CountPlayersInRoom = Context.Users.Count(u => u.CurrentRoomId == r.Id)
            });
            return Ok(roomsDto);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Enter(EnterRoomDto input)
        {
            var isRoomExist = Context.Rooms.Any(r => r.Id == input.RoomId);
            var isUserExist = Context.Users.Any(u => u.Id == input.UserId);
            if (!isRoomExist)
            {
                return BadRequest($"Комната с Id = {input.RoomId} не существует");
            }

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
        public async Task<IActionResult> Exit(EnterRoomDto input)
        {
            var isRoomExist = Context.Rooms.Any(r => r.Id == input.RoomId);
            var isUserExist = Context.Users.Any(u => u.Id == input.UserId);
            if (!isRoomExist)
            {
                return BadRequest($"Комната с Id = {input.RoomId} не существует");
            }

            if (!isUserExist)
            {
                return BadRequest($"Пользователь с Id = {input.UserId} не существует");
            }

            var isUserInRoom = Context.Users.Any(u => u.CurrentRoomId == input.RoomId && u.Id == input.UserId);
            if (!isUserInRoom)
            {
                return BadRequest($"Пользователь с Id = {input.UserId} не находится в комнате");
            }

            var user = Context.Users.Find(input.UserId);
            user.CurrentRoomId = null;
            Context.Users.Update(user);
            await Context.SaveChangesAsync();

            return Ok();
        }
    }
}
