using GameApp.WebApi.Services.Users.Dto;

namespace GameApp.WebApi.Services.Rooms.Dto
{
    public class CurrentRoomDto
    {
        public int Id { get; set; }
        public bool IsPlayerRoomManager { get; set; }
        public bool IsGameStarted { get; set; }
        public UserDto Opponent { get; set; }
        public UserDto Player { get; set; }
        public string RoomName { get; set; }
    }
}
