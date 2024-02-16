using AutoMapper;
using GameApp.Domain;
using GameApp.WebApi.Hubs;
using GameApp.WebApi.Services.Rooms;
using GameApp.WebApi.Services.Users;
using GameApp.WebApi.Services.Users.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace GameApp.WebApi.Controllers
{
    [Authorize]
    public class UsersController : GameAppController
    {
        private readonly IUserService _userService;
        private readonly IRoomService _roomService;
        private readonly IHubContext<GameHub> _gameHub;

        public UsersController(GameContext context, IMapper mapper, IUserService userService, IHubContext<GameHub> hubContext, IRoomService roomService) : base(context, mapper)
        {
            _userService = userService;
            _gameHub = hubContext;
            _roomService = roomService;
        }

        [HttpGet("[action]")]
		[ProducesResponseType(typeof(UserShortDto), StatusCodes.Status200OK)]
		public async Task<IActionResult> GetAvatarAsync()
		{
			try
			{
				var avatar = await _userService.GetAvatarAsync(CurrentUserLogin!);
                return Ok(new UserShortDto
                {
                    Login = CurrentUserLogin!,
                    Avatar = avatar
                });
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);  // TODO [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]  ?
			}
		}

        [HttpGet("[action]")]
        [ProducesResponseType(typeof(IEnumerable<UserDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(string? searchString = null)
        {
            return Ok(await _userService.GetAll(searchString));
        }

        [HttpPost("[action]")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<IActionResult> ChangeReady()
        {
            try
            {
				var userId = await _userService.GetId(User.Identity!.Name!);
                var isReady = await _userService.ChangeReady(userId);
                var currentRoom = await _roomService.GetCurrentRoom(userId);

                if (currentRoom.Opponent is not null)
                {
                    await _gameHub.Clients
                        .Client(GameHub._connectionUsers[currentRoom.Opponent.Login])
                        .SendAsync("ChangeReady", isReady);
                }

				return Ok(isReady);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
