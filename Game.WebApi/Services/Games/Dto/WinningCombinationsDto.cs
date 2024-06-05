namespace GameApp.WebApi.Services.Games.Dto
{
    public class WinningCombinationsDto
    {
        public List<List<int>> Horizontal { get; }
        public List<List<int>> Vertical { get; }
        public List<int> LeftDiagonal { get; }
        public List<int> RightDiagonal { get; }

        public WinningCombinationsDto() 
        {
            Horizontal = new List<List<int>>{
                                new () { 0, 1, 2 },
                                new () { 3, 4, 5 },
                                new () { 6, 7, 8 }
            };
            Vertical = new List<List<int>>{
                                new () { 0, 3, 5 },
                                new () { 1, 4, 7 },
                                new () { 2, 5, 8 }
            };
            LeftDiagonal = new List<int> { 0, 4, 8 };
            RightDiagonal = new List<int>{ 6, 4, 2 };
        }
    }
}
