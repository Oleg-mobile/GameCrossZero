namespace GameApp.WebApi.Dto.Rooms
{
    public class EnterRoomDto
    {
        public int RoomId { get; set; }
        public int UserId { get; set; }
        public string? Password { get; set; }
    }
}
