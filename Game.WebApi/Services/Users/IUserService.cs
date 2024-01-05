using GameApp.WebApi.Services.Users.Dto;

namespace GameApp.WebApi.Services.Users
{
    public interface IUserService
    {
        Task Create(CreateUserDto input);
        Task<IEnumerable<UserDto>> GetAll(string? searchString = null);
        Task<bool> ChangeReady(int userId);
        Task<int> GetId(string login);
    }
}
