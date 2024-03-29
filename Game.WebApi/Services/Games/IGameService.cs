﻿using GameApp.WebApi.Services.Games.Dto;
using GameApp.WebApi.Services.Rooms.Dto;

namespace GameApp.WebApi.Services.Games
{
    public interface IGameService
    {
        Task CreateAsync(CreateGameDto input);
        Task StartAsync(int roomId);
		Task<InfoGameDto> GetInfoAsync(int roomId);
	}
}
