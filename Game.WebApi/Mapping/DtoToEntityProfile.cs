﻿using AutoMapper;
using GameApp.Domain.Models;
using GameApp.WebApi.Dto.Rooms;
using GameApp.WebApi.Services.Games.Dto;
using GameApp.WebApi.Services.Users.Dto;

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
            CreateMap<GetRoomDto, Room>();
        }
    }
}
