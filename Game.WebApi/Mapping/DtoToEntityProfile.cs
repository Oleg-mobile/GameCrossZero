using AutoMapper;
using GameApp.Domain.Models;
using GameApp.WebApi.Dto.Games;
using GameApp.WebApi.Dto.Users;
using GameApp.WebApi.Services.Rooms.Dto;

namespace GameApp.WebApi.Mapping
{
    public class DtoToEntityProfile : Profile
    {
        public DtoToEntityProfile()
        {
            CreateMap<CreateUserDto, User>().ReverseMap();
            CreateMap<User, GetUserDto>();
            CreateMap<CreateRoomDto, Room>().ReverseMap();
            CreateMap<CreateGameDto, Game>().ReverseMap();
            CreateMap<RoomDto, Room>();
        }
    }
}
