namespace GameApp.Domain.Models
{
    public class Room : Entity
    {
        public string Name { get; set; }
        public string? Password { get; set; }
    }
}
