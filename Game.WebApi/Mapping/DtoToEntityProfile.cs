using AutoMapper;
using GameApp.Domain.Models;
using GameApp.WebApi.Dto.Rooms;
using GameApp.WebApi.Dto.Users;

namespace GameApp.WebApi.Mapping
{
    public class DtoToEntityProfile : Profile
    {
        public DtoToEntityProfile()
        {
            CreateMap<AddUserDto, User>().ReverseMap();
            CreateMap<CreateRoomDto, Room>().ReverseMap();
            CreateMap<GetRoomDto, Room>();
        }
    }
}
