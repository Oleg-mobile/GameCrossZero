using AutoMapper;
using FluentValidation;
using GameApp.Domain;
using GameApp.Domain.Models;
using GameApp.WebApi.Dto.Rooms;
using GameApp.WebApi.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GameApp.WebApi.Controllers
{
    public class RoomsController : GameAppController
    {
        private readonly IValidator<ExitRoomDto> _exitRoomValidator;
        private readonly IValidator<EnterRoomDto> _enterRoomValidator;

        public RoomsController(GameContext context, IMapper mapper, IValidator<ExitRoomDto> exitRoomValidator, IValidator<EnterRoomDto> enterRoomValidator) : base(context, mapper)
        {
            _exitRoomValidator = exitRoomValidator;
            _enterRoomValidator = enterRoomValidator;
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
            var roomsDto = Mapper.Map<IEnumerable<GetRoomDto>>(await query.Select(r => new GetRoomDto()
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

            var isExist = Context.Users.Any(u => u.CurrentRoomId == input.RoomId);
            if (!isExist)
            {
                // TODO ссылается на игру

                Delete(input.RoomId);
                return Ok();   // TODO Нужен?
            }

            var user = await Context.Users.FindAsync(input.UserId);
            user.CurrentRoomId = null;
            user.isReadyToPlay = false;
            Context.Users.Update(user);

            var room = await Context.Rooms.FindAsync(input.RoomId);
            if (room.ManagerId == input.UserId)
            {
                var newRoomManager = await Context.Users.FirstOrDefaultAsync(u => u.CurrentRoomId == input.RoomId);
                room.ManagerId = newRoomManager.Id;
                Context.Rooms.Update(room);
            }

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
