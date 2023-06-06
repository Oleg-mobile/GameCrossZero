using Game.Domain.Enums;

namespace Game.WebApi.Dto.Users
{
    public class AddUserDto
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public string? Nickname { get; set; }
        public RoleType Role { get; set; }
        public string? Avatar { get; set; }
    }
}
