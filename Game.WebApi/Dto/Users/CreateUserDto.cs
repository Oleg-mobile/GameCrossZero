using GameApp.Domain.Enums;

namespace GameApp.WebApi.Dto.Users
{
    public class CreateUserDto
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public string? Nickname { get; set; }
        public RoleType Role { get; set; }
        public string? Avatar { get; set; }
        public int Raiting { get; set; }
    }
}
