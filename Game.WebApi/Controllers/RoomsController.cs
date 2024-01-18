using AutoMapper;
using GameApp.Domain;
using GameApp.WebApi.Services.Rooms;
using GameApp.WebApi.Services.Rooms.Dto;
using GameApp.WebApi.Services.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameApp.WebApi.Controllers
{
    [Authorize]
    public class RoomsController : GameAppController
    {
        private readonly IRoomService _roomService;
        private readonly IUserService _userService;

		public RoomsController(GameContext context, IMapper mapper, IRoomService roomService, IUserService userService) : base(context, mapper)
		{
			_roomService = roomService;
			_userService = userService;
		}

		[HttpPost("[action]")]
        public async Task<IActionResult> Create(CreateRoomDto input)
        {
            try
            {
				var managerId = await _userService.GetId(User.Identity!.Name!);
				await _roomService.Create(input, managerId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("[action]")]
        [ProducesResponseType(typeof(IEnumerable<RoomDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _roomService.GetAll());
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Enter(EnterRoomDto input)
        {
            try
            {
				var userId = await _userService.GetId(User.Identity!.Name!);
				await _roomService.Enter(input, userId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Exit(ExitRoomDto input)
        {
            try
            {
                await _roomService.Exit(input);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);   //  TODO ошибка, если комната одна и её id у юзера в CurrentRoomId
			}
        }

        [HttpDelete("[action]")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _roomService.Delete(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("[action]")]
        [ProducesResponseType(typeof(CurrentRoomDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCurrentRoom()
        {
			var playerId = await _userService.GetId(User.Identity!.Name!);

            return Ok(await _roomService.GetCurrentRoom(playerId));
        }
    }
}
