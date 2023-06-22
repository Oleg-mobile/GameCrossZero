using AutoMapper;
using GameApp.Domain;
using GameApp.Domain.Models;
using GameApp.WebApi.Services.Rooms.Dto;

namespace GameApp.WebApi.Services.Rooms
{
    public class RoomService : GameAppService, IRoomService
    {
        public RoomService(GameContext context, IMapper mapper) : base(context, mapper)
        {
        }

        public async Task Create(CreateRoomDto input)
        {
            var isExist = Context.Rooms.Any(r => r.Name == input.Name);

            if (isExist)
            {
                throw new Exception($"Комната с названием {input.Name} уже существует");
            }

            var room = Mapper.Map<Room>(input);

            await Context.Rooms.AddAsync(room);
            await Context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            throw new NotImplementedException();
        }

        public async Task Enter(EnterRoomDto input)
        {
            throw new NotImplementedException();
        }

        public async Task Exit(ExitRoomDto input)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<RoomDto>> GetAll()
        {
            throw new NotImplementedException();
        }
    }
}
