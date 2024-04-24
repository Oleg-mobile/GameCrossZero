namespace GameApp.WebApi.Services.Games.Dto
{
	public class InfoGameDto
	{
		public bool IsMyStep { get; set; }
		public bool IsMyFigureCross { get; set; }
		public IEnumerable<StepDto> Steps { get; set; }
		public int GameId { get; set; }
	}
}
