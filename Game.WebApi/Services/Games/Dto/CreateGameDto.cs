namespace GameApp.WebApi.Services.Games.Dto
{
    public class CreateGameDto
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        public int WinnerId { get; set; }
        public int WhoseMoveId { get; set; }
    }
}
