namespace GameApp.WebApi.Dto.Rooms
{
    public class GetRoomDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ManagerId { get; set; }
        public bool IsProtected { get; set; }
        public int CountPlayersInRoom { get; set; }
    }
}
