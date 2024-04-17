namespace GameApp.WebApi.Services.Games.Dto
{
	public class InfoGameDto
	{
		public int WhoseMoveId { get; set; }
		public bool IsMyFigureCross { get; set; }
		public IEnumerable<StepDto> Steps { get; set; }
	}
}
