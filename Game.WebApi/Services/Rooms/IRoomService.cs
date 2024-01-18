using GameApp.WebApi.Services.Rooms.Dto;

namespace GameApp.WebApi.Services.Rooms
{
    public interface IRoomService
    {
        Task Create(CreateRoomDto input, int managerId);
        Task<IEnumerable<RoomDto>> GetAll();
        Task Enter(EnterRoomDto input, int userId);
        Task Exit(ExitRoomDto input);
        Task Delete(int id);
        Task<CurrentRoomDto> GetCurrentRoom(int playerId);
    }
}
