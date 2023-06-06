using Game.WebApi.Dto.Users;
using Microsoft.AspNetCore.Mvc;

namespace Game.WebApi.Controllers
{
    public class UsersController : GameController
    {
        public async Task<IActionResult> Add(AddUserDto input)
        {
            return Ok();
        }
    }
}
