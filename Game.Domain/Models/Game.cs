using System.ComponentModel.DataAnnotations.Schema;

namespace GameApp.Domain.Models
{
    public class Game : Entity
    {
        public int RoomId { get; set; }

        [ForeignKey("RoomId")]
        public Room Room { get; set; }

        public int? WinnerId { get; set; }

        [ForeignKey("WinnerId")]
        public User? Winner { get; set; }

        public int? WhoseMoveId { get; set; }

        [ForeignKey("WhoseMoveId")]
        public User? WhoseMove { get; set; }
    }
}
