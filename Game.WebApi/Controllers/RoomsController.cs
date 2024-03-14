using GameApp.Domain.Models;
using GameApp.WebApi.Hubs;
using GameApp.WebApi.Services.Rooms;
using GameApp.WebApi.Services.Rooms.Dto;
using GameApp.WebApi.Services.Users;
using GameApp.WebApi.Services.Users.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace GameApp.WebApi.Controllers
{
	[Authorize]
	public class RoomsController : GameAppController
	{
		private readonly IRoomService _roomService;
		private readonly IUserService _userService;
		private readonly IHubContext<GameHub> _gameHub;

		public RoomsController(IRoomService roomService, IUserService userService, IHubContext<GameHub> hubContext)
		{
			_roomService = roomService;
			_userService = userService;
			_gameHub = hubContext;
		}

		[HttpPost("[action]")]
		public async Task<IActionResult> Create(CreateRoomDto input)
		{
			try
			{
				var managerId = await _userService.GetId(User.Identity!.Name!);
				await _roomService.Create(input, managerId);
				return Ok();
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpGet("[action]")]
		[ProducesResponseType(typeof(IEnumerable<RoomDto>), StatusCodes.Status200OK)]
		public async Task<IActionResult> GetAll()
		{
			return Ok(await _roomService.GetAll());
		}

		[HttpPost("[action]")]
		public async Task<IActionResult> Enter(EnterRoomDto input)
		{
			try
			{
				var playerId = await _userService.GetId(User.Identity!.Name!);
				await _roomService.Enter(input.RoomId, input.Password, playerId);
				var currentRoom = await _roomService.GetCurrentRoom(playerId);

				if (currentRoom.Opponent is not null && GameHub._connectionUsers.ContainsKey(currentRoom.Opponent.Login))
				{
					var playerData = new UserShortDto
					{
						Login = currentRoom.Player.Login,
						Avatar = currentRoom.Player.Avatar
					};

					await _gameHub.Clients
						.Client(GameHub._connectionUsers[currentRoom.Opponent.Login])
						.SendAsync("PlayerEntered", playerData);
				}

				return Ok(true);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpPost("[action]")]
		public async Task<IActionResult> Exit()
		{
			try
			{
				var playerId = await _userService.GetId(User.Identity!.Name!);
				var currentRoom = await _roomService.GetCurrentRoom(playerId);

				if (currentRoom is null)
				{
					return BadRequest("Вы не находитесь в комнате");
				}

				if (currentRoom.Opponent is not null && GameHub._connectionUsers.ContainsKey(currentRoom.Opponent.Login))
				{
					var playerData = new UserShortDto
					{
						Login = "Пока никого нет...",
						Avatar = "/img/zhdun.ico"
					};

					await _gameHub.Clients
						.Client(GameHub._connectionUsers[currentRoom.Opponent.Login])
						.SendAsync("PlayerIsOut", playerData);
				}

				await _roomService.Exit(playerId);

				return Ok();
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);   //  TODO ошибка, если комната одна и её id у юзера в CurrentRoomId
			}
		}

		[HttpDelete("[action]")]
		public async Task<IActionResult> Delete()
		{
			try
			{
				var playerId = await _userService.GetId(User.Identity!.Name!);
				await _roomService.Delete(playerId);
				return Ok();
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpGet("[action]")]
		[ProducesResponseType(typeof(CurrentRoomDto), StatusCodes.Status200OK)]
		public async Task<IActionResult> GetCurrentRoom()
		{
			var playerId = await _userService.GetId(User.Identity!.Name!);

			return Ok(await _roomService.GetCurrentRoom(playerId));
		}
	}
}
