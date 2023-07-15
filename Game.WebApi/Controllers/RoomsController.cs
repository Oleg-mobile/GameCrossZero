using AutoMapper;
using GameApp.Domain;
using GameApp.WebApi.Services.Rooms;
using GameApp.WebApi.Services.Rooms.Dto;
using Microsoft.AspNetCore.Mvc;

namespace GameApp.WebApi.Controllers
{
    public class RoomsController : GameAppController
    {
        private readonly IRoomService _roomService;

        public RoomsController(GameContext context, IMapper mapper, IRoomService roomService) : base(context, mapper)
        {
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
            return Ok(await _roomService.GetAll());
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Enter(EnterRoomDto input)
        {
            try
            {
                await _roomService.Enter(input);
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
                return BadRequest(ex.Message);
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
        public async Task<IActionResult> GetCurrentRoom()
        {
            return Ok(await _roomService.GetCurrentRoom());
        }
    }
}
