using System.Security.Claims;

namespace GameApp.WebApi.Services.Accounts
{
	public interface IAccountService
	{
		IEnumerable<Claim> GetIdentity(string username, string password);
	}
}
