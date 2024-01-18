using AutoMapper;
using GameApp.Domain;
using GameApp.WebApi.Services.Users;
using GameApp.WebApi.Services.Users.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameApp.WebApi.Controllers
{
    [Authorize]
    public class UsersController : GameAppController
    {
        private readonly IUserService _userService;

        public UsersController(GameContext context, IMapper mapper, IUserService userService) : base(context, mapper)
        {
            _userService = userService;
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
				return Ok(await _userService.ChangeReady(userId));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // TODO Нужно ли удалять?
        // TODO Нужен ли поиск по id?
    }
}
