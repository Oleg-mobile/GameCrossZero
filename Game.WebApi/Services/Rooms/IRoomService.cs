using GameApp.WebApi.Services.Rooms.Dto;

namespace GameApp.WebApi.Services.Rooms
{
    public interface IRoomService
    {
        Task Create(CreateRoomDto input, int managerId);
        Task<IEnumerable<RoomDto>> GetAll();
        Task Enter(int roomId, string password, int userId);
        Task Exit(int userId);
        Task Delete(int userId);
        Task<CurrentRoomDto> GetCurrentRoom(int userId);
    }
}
