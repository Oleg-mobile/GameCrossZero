namespace GameApp.WebApi.Services.Games.Dto
{
    public static class WinningCombinations
    {
        public static IReadOnlyList<WinningCombination> Items { get; } = new List<WinningCombination>
        {
            new WinningCombination(WinningCombinationType.Horizontal, new int[] { 0, 1, 2 }),
            new WinningCombination(WinningCombinationType.Horizontal, new int[] { 3, 4, 5 }),
            new WinningCombination(WinningCombinationType.Horizontal, new int[] { 6, 7, 8 }),

            new WinningCombination(WinningCombinationType.Vertical, new int[] { 0, 3, 5 }),
            new WinningCombination(WinningCombinationType.Vertical, new int[] { 1, 4, 7 }),
            new WinningCombination(WinningCombinationType.Vertical, new int[] { 2, 5, 8 }),

            new WinningCombination(WinningCombinationType.LeftDiagonal, new int[] { 0, 4, 8 }),
            new WinningCombination(WinningCombinationType.RightDiagonal, new int[] { 2, 4, 6 }),
        };
    }

    public class WinningCombination : List<int>
    {
        public WinningCombinationType Type { get; private set; }

        public WinningCombination(WinningCombinationType type, IEnumerable<int> cells) : base(cells)
        {
            Type = type;
        }
    }

    public enum WinningCombinationType
    {
        Horizontal,
        Vertical,
        LeftDiagonal,
        RightDiagonal
   }
}
