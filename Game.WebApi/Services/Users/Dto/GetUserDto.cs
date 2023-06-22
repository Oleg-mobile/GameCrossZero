using GameApp.Domain.Enums;

namespace GameApp.WebApi.Services.Users.Dto
{
    public class GetUserDto
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string? Nickname { get; set; }
        public RoleType Role { get; set; }
        public string? Avatar { get; set; }
        public int Raiting { get; set; }
        public int? CurrentRoomId { get; set; }
        public bool isReadyToPlay { get; set; }
    }
}
