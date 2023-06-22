namespace GameApp.WebApi.Services.Rooms.Dto
{
    public class RoomDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ManagerId { get; set; }
        public bool IsProtected { get; set; }
        public int CountPlayersInRoom { get; set; }
    }
}
