using GameApp.WebApi.Services.Games.Dto;

namespace GameApp.WebApi.Services.Games
{
    public interface IGameService
    {
        Task CreateAsync(CreateGameDto input);
        Task StartAsync(int roomId);
    }
}
