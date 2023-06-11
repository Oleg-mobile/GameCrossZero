using AutoMapper;
using GameApp.Domain;
using GameApp.Domain.Models;
using GameApp.WebApi.Dto.Users;
using Microsoft.AspNetCore.Mvc;

namespace GameApp.WebApi.Controllers
{
    public class UsersController : GameController
    {
        public UsersController(GameContext context, Mapper mapper) : base(context, mapper)
        {
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Add(AddUserDto input)
        {
            var isExist = Context.Users.Any(u => u.Login == input.Login);

            if (isExist)
            {
                return BadRequest($"Пользователь {input.Login} уже существует");
            }

            var user = Mapper.Map<User>(input);
            Context.Users.Add(user);
            Context.SaveChanges();

            return Ok();
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetAll(string? searchString = null)
        {
            IQueryable<User> query = Context.Users;

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(u => u.Login.Trim().ToLower().Contains(searchString.Trim().ToLower()));
            }

            var usersDto = Mapper.Map<IList<AddUserDto>>(query.ToList());
            return Ok(usersDto);
        }
    }
}
