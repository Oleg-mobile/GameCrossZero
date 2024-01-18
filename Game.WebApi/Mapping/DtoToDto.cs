using AutoMapper;
using GameApp.WebApi.Services.Rooms.Dto;

namespace GameApp.WebApi.Mapping
{
    public class DtoToDto : Profile
    {
        public DtoToDto()
        {
            CreateMap<CreateRoomDto, EnterRoomDto>();
        }
    }
}
