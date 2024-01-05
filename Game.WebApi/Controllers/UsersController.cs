﻿using AutoMapper;
using GameApp.Domain;
using GameApp.WebApi.Services.Users;
using GameApp.WebApi.Services.Users.Dto;
using Microsoft.AspNetCore.Mvc;

namespace GameApp.WebApi.Controllers
{
    public class UsersController : GameAppController
    {
        private readonly IUserService _userService;

        public UsersController(GameContext context, IMapper mapper, IUserService userService) : base(context, mapper)
        {
            _userService = userService;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Create(CreateUserDto input)
        {
            try
            {
                await _userService.Create(input);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);  // TODO [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]  ?
            }
        }

        [HttpGet("[action]")]
        [ProducesResponseType(typeof(IEnumerable<UserDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(string? searchString = null)
        {
            return Ok(await _userService.GetAll(searchString));
        }

        [HttpPost("[action]")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<IActionResult> ChangeReady(int userId)
        {
            try
            {
                return Ok(await _userService.ChangeReady(userId));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // TODO Нужно ли удалять?
        // TODO Нужен ли поиск по id?
    }
}
