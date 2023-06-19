using AutoMapper;
using GameApp.Domain.Models;
using GameApp.WebApi.Dto.Games;
using GameApp.WebApi.Dto.Rooms;
using GameApp.WebApi.Dto.Users;

namespace GameApp.WebApi.Mapping
{
    public class DtoToEntityProfile : Profile
    {
        public DtoToEntityProfile()
        {
            CreateMap<CreateUserDto, User>().ReverseMap();
            CreateMap<CreateRoomDto, Room>().ReverseMap();
            CreateMap<CreateGameDto, Game>().ReverseMap();
            CreateMap<GetRoomDto, Room>();
        }
    }
}
