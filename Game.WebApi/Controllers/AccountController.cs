using AutoMapper;
using GameApp.Domain;
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

        // TODO лишний mapper во всех контроллерах
        public AccountController(GameContext context, IMapper mapper, IConfiguration configuration = null) : base(context, mapper)
        {
            _configuration = configuration;
        }

        [HttpPost("[action]")]
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

            return Ok(encodedJwt);
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
        public IActionResult Register()
        {
            return Ok();
        }
    }
}
