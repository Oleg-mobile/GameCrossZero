using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace GameApp.WebApi.Hubs
{
	[Authorize]
	public class GameHub : Hub
	{
		private static readonly Dictionary<string, string> _connectionUsers = new Dictionary<string, string>();


	}
}
