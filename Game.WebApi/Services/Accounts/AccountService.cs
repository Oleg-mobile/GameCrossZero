using AutoMapper;
using GameApp.Domain;
using System.Security.Claims;

namespace GameApp.WebApi.Services.Accounts
{
	public class AccountService : GameAppService, IAccountService
	{
		public AccountService(GameContext context, IMapper mapper) : base(context, mapper)
		{
		}

		public IEnumerable<Claim> GetIdentity(string username, string password)
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

			return null;
		}
	}
}
