using System.ComponentModel.DataAnnotations.Schema;

namespace GameApp.Domain.Models
{
    public class UserGame
    {
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        public int GameId { get; set; }

        [ForeignKey("GameId")]
        public Game Game { get; set; }

        public bool IsCross { get; set; }
    }
}
