using AutoMapper;
using FluentValidation;
using GameApp.Domain;
using GameApp.Domain.Models;
using GameApp.WebApi.Services.Rooms;
using GameApp.WebApi.Services.Rooms.Dto;
using GameApp.WebApi.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GameApp.WebApi.Controllers
{
    public class RoomsController : GameAppController
    {
        private readonly IValidator<ExitRoomDto> _exitRoomValidator;
        private readonly IValidator<EnterRoomDto> _enterRoomValidator;
        private readonly IRoomService _roomService;

        public RoomsController(GameContext context, IMapper mapper, IValidator<ExitRoomDto> exitRoomValidator, IValidator<EnterRoomDto> enterRoomValidator, IRoomService roomService) : base(context, mapper)
        {
            _exitRoomValidator = exitRoomValidator;
            _enterRoomValidator = enterRoomValidator;
            _roomService = roomService;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Create(CreateRoomDto input)
        {
            try
            {
                await _roomService.Create(input);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetAll()
        {
            IQueryable<Room> query = Context.Rooms;
            var roomsDto = Mapper.Map<IEnumerable<RoomDto>>(await query.Select(r => new RoomDto()
            {
                Id = r.Id,
                Name = r.Name,
                ManagerId = r.ManagerId,
                IsProtected = r.Password == null ? false : true,
                CountPlayersInRoom = Context.Users.Count(u => u.CurrentRoomId == r.Id)
            }).ToListAsync());

            return Ok(roomsDto);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Enter(EnterRoomDto input)
        {
            try
            {
                _enterRoomValidator.ValidateAndThrow(input);
            }
            catch (ValidationException ex)
            {
                var message = ex.Errors?.First().ErrorMessage ?? ex.Message;
                return BadRequest(message);
            }

            // TODO Добавить в валидатор?
            var countPlayersInRoom = Context.Users.Count(u => u.CurrentRoomId == input.RoomId);
            if (countPlayersInRoom >= Constants.maxNumberOfPlayers)
            {
                return BadRequest($"Превышено количество пользователей для комнаты");
            }
            // TODO Добавить в валидатор?
            var room = await Context.Rooms.FindAsync(input.RoomId);
            if (!string.IsNullOrEmpty(room.Password) && room.Password != input.Password)
            {
                BadRequest("Не верный пароль");
            }

            var user = await Context.Users.FindAsync(input.UserId);
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

            var usersCount = Context.Users.Count(u => u.CurrentRoomId == input.RoomId);
            if (usersCount == 1)
            {
                Delete(input.RoomId);
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
            return Ok();
        }

        [HttpDelete("[action]")]
        public async Task<IActionResult> Delete(int id)
        {
            var room = await Context.Rooms.FindAsync(id);

            if (room is null)
            {
                return BadRequest($"Комната с Id = {id} не существует");
            }

            Context.Rooms.Remove(room);
            Context.SaveChangesAsync();
            return Ok();
        }
    }
}
