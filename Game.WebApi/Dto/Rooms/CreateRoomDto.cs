namespace GameApp.WebApi.Dto.Rooms
{
    public class CreateRoomDto
    {
        public string Name { get; set; }
        public string? Password { get; set; }
        public int ManagerId { get; set; }

    }
}
