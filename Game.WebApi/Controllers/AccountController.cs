using AutoMapper;
using GameApp.Domain;
using GameApp.WebApi.Services.Users;
using GameApp.WebApi.Services.Users.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GameApp.WebApi.Controllers
{
    // TODO вынести методы в сервис
    public class AccountController : GameAppController
    {
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;

        // TODO mapper в контроллерах и в сервисах
        public AccountController(GameContext context, IMapper mapper, IUserService userService, IConfiguration configuration = null) : base(context, mapper)
        {
            _configuration = configuration;
            _userService = userService;
        }

        [HttpPost("[action]")]
        [ProducesResponseType(typeof(TokenDto), StatusCodes.Status200OK)]
        public IActionResult Login(string username, string password)
        {
            var identity = GetIdentity(username, password);
            if (identity == null)
            {
                return BadRequest(new { errorText = "Invalid username or password." });
            }

            var now = DateTime.UtcNow;
            // создаем JWT-токен
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

        private IEnumerable<Claim> GetIdentity(string username, string password)
        {
            var person = Context.Users.FirstOrDefault(x => x.Login == username && x.Password == password);
            if (person != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, person.Login),
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, person.Role.ToString())
                };

                return claims;
            }

            // если пользователя не найдено
            return null;
        }

        [HttpPost("[action]")]
        public IActionResult Register(string username, string password)
        {
            var isExist = Context.Users.Any(x => x.Login == username);
            if (isExist)
            {
                return BadRequest(new { errorText = "The user already exists" });
            }

            CreateUserDto createUserDto = new CreateUserDto  // TODO создать свою дто?
            {
                Login = username,
                Password = password
            };

            _userService.Create(createUserDto);

            return Ok();
        }
    }
}
