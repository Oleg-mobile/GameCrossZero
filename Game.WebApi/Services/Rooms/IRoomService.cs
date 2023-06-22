using GameApp.WebApi.Services.Rooms.Dto;

namespace GameApp.WebApi.Services.Rooms
{
    public interface IRoomService
    {
        Task Create(CreateRoomDto input);
        Task<IEnumerable<RoomDto>> GetAll();
        Task Enter(EnterRoomDto input);
        Task Exit(ExitRoomDto input);
        Task Delete(int id);
    }
}
