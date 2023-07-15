using AutoMapper;
using GameApp.Domain;
using GameApp.Domain.Models;
using GameApp.WebApi.Services.Users.Dto;
using Microsoft.EntityFrameworkCore;

namespace GameApp.WebApi.Services.Users
{
    public class UserService : GameAppService, IUserService
    {
        public UserService(GameContext context, IMapper mapper) : base(context, mapper)
        {
        }

        public async Task<bool> ChangeReady(int userId)
        {
            var user = Context.Users.FirstOrDefault(u => u.Id == userId) ?? throw new Exception($"Пользователь с {userId} не существует");
            user.IsReadyToPlay = !user.IsReadyToPlay;
            await Context.SaveChangesAsync();

            return user.IsReadyToPlay;
        }

        public async Task Create(CreateUserDto input)
        {
            var isExist = Context.Users.Any(u => u.Login == input.Login);

            if (isExist)
            {
                throw new Exception($"Пользователь {input.Login} уже существует");
            }

            var user = Mapper.Map<User>(input);
            await Context.Users.AddAsync(user);
            await Context.SaveChangesAsync();
        }

        public async Task<IEnumerable<UserDto>> GetAll(string? searchString = null)
        {
            IQueryable<User> query = Context.Users;

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(u => u.Login.Trim().ToLower().Contains(searchString.Trim().ToLower()));
            }

            return Mapper.Map<IEnumerable<UserDto>>(await query.ToListAsync());
        }
    }
}
