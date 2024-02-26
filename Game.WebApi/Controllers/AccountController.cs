using GameApp.WebApi.Services.Accounts;
using GameApp.WebApi.Services.Users;
using GameApp.WebApi.Services.Users.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace GameApp.WebApi.Controllers
{
	public class AccountController : GameAppController
	{
		private readonly IConfiguration _configuration;
		private readonly IUserService _userService;
		private readonly IAccountService _accountService;

		public AccountController(IUserService userService, IConfiguration configuration, IAccountService accountService)
		{
			_configuration = configuration;
			_userService = userService;
			_accountService = accountService;
		}

		[HttpPost("[action]")]
		[ProducesResponseType(typeof(TokenDto), StatusCodes.Status200OK)]
		public IActionResult Login(string username, string password)
		{
			var identity = _accountService.GetIdentity(username, password);
			if (identity == null)
			{
				return BadRequest(new { errorText = "Invalid username or password." });
			}

			var now = DateTime.UtcNow;

			var jwt = new JwtSecurityToken(
			issuer: _configuration["JwtSettings:Issuer"],
			audience: _configuration["JwtSettings:Audience"],
			notBefore: now,
					claims: identity,
					expires: now.Add(TimeSpan.FromMinutes(60)),
					signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["JwtSettings:SecretKey"])), SecurityAlgorithms.HmacSha256));
			var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

			return Ok(new TokenDto
			{
				Token = encodedJwt,
				Expires = now.Add(TimeSpan.FromMinutes(60))
			});
		}

		[HttpPost("[action]")]
		public async Task<IActionResult> Register(string username, string password)
		{
			try
			{
				var createUserDto = new CreateUserDto
				{
					Login = username,
					Password = password
				};

				await _userService.Create(createUserDto);

				return Ok();
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}
	}
}
