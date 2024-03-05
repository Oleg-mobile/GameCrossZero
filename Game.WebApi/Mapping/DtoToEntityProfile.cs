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
            CreateMap<CreateUserDto, User>();
			CreateMap<User, UserDto>();
            CreateMap<CreateRoomDto, Room>().ForMember(dest => dest.Password, opt => opt.MapFrom(src =>
			    string.IsNullOrWhiteSpace(src.Password) ? null : src.Password.Trim()));
            CreateMap<CreateGameDto, Game>();
			CreateMap<InfoGameDto, Game>().ReverseMap();
			CreateMap<RoomDto, Room>();
        }
    }
}
