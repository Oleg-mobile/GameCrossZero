using GameApp.WebApi.Services.Games.Dto;

namespace GameApp.WebApi.Services.Games
{
    public interface IGameService
    {
        Task Create(CreateGameDto input);
        Task Start(int roomId);
    }
}
