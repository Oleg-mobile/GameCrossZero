using System.ComponentModel.DataAnnotations.Schema;

namespace GameApp.Domain.Models
{
    public class GameProgress : Entity
    {
        public int GameId { get; set; }

        [ForeignKey("GameId")]
        public Game Game { get; set; }

        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        public int Cell { get; set; }
        public int StrokeNumber { get; set; }
    }
}
