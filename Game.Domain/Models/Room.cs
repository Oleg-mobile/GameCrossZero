using System.ComponentModel.DataAnnotations.Schema;

namespace GameApp.Domain.Models
{
    public class Room : Entity
    {
        public string Name { get; set; }
        public string? Password { get; set; }
        public int ManagerId { get; set; }

        [ForeignKey("ManagerId")]
        public User Manager { get; set; }
        public int? CurrentGameId { get; set; }

        [ForeignKey("CurrentGameId")]
        public Game? CurrentGame { get; set; }
    }
}
