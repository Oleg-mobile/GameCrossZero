using GameApp.Domain.Models;

namespace GameApp.WebApi.Services.Games.Dto
{
	public class InfoGameDto
	{
		public int WinnerId { get; set; }

		public User Winner { get; set; }

		public int WhoseMoveId { get; set; }

		public User WhoseMove { get; set; }
	}
}
