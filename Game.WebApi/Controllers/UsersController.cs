using AutoMapper;
using GameApp.Domain;
using GameApp.Domain.Models;
using GameApp.WebApi.Dto.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GameApp.WebApi.Controllers
{
    public class UsersController : GameAppController
    {
        public UsersController(GameContext context, IMapper mapper) : base(context, mapper)
        {
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Create(CreateUserDto input)
        {
            var isExist = Context.Users.Any(u => u.Login == input.Login);

            if (isExist)
            {
                return BadRequest($"Пользователь {input.Login} уже существует");
            }

            var user = Mapper.Map<User>(input);
            await Context.Users.AddAsync(user);
            await Context.SaveChangesAsync();

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

            var usersDto = Mapper.Map<IEnumerable<GetUserDto>>(await query.ToListAsync());
            return Ok(usersDto);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> ChangeReady(int userId)
        {
            var user = Context.Users.FirstOrDefault(u => u.Id == userId);

            if (user is null)
            {
                return BadRequest($"Пользователь с {userId} не существует");
            }

            user.isReadyToPlay = !user.isReadyToPlay;
            await Context.SaveChangesAsync();
            return Ok(user.isReadyToPlay);
        }

        // TODO Нужно ли удалять. Что делать со статусами?
        // TODO Нужен ли поиск по id?
    }
}
