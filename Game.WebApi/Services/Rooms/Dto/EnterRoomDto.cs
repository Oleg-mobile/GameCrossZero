namespace GameApp.WebApi.Services.Rooms.Dto
{
    public class EnterRoomDto
    {
        public int RoomId { get; set; }
        public int UserId { get; set; }
        public string? Password { get; set; }
    }
}
