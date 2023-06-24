using AutoMapper;
using GameApp.Domain.Models;
using GameApp.WebApi.Services.Games.Dto;
using GameApp.WebApi.Services.Rooms.Dto;
using GameApp.WebApi.Services.Users.Dto;

namespace GameApp.WebApi.Mapping
{
    public class DtoToEntityProfile : Profile
    {
        public DtoToEntityProfile()
        {
            CreateMap<CreateUserDto, User>().ReverseMap();
            CreateMap<User, UserDto>();
            CreateMap<CreateRoomDto, Room>().ReverseMap();
            CreateMap<CreateGameDto, Game>().ReverseMap();
            CreateMap<RoomDto, Room>();
        }
    }
}
