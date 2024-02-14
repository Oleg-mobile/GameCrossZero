using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace GameApp.WebApi.Hubs
{
	[Authorize]
	public class GameHub : Hub
	{
		public static readonly Dictionary<string, string> _connectionUsers = new Dictionary<string, string>();

		public override Task OnConnectedAsync()
		{
			try
			{
				var connectionId = Context.ConnectionId;

				if (!_connectionUsers.ContainsKey(IdentityName))
				{
					_connectionUsers.Add(IdentityName, connectionId);
				}
				else
				{
					_connectionUsers[IdentityName] = connectionId;
				}
			}
			catch (Exception ex)
			{
				Clients.Caller.SendAsync("onError", "OnConnected:" + ex.Message);
			}
			return base.OnConnectedAsync();
		}

		public override Task OnDisconnectedAsync(Exception exception)
		{
			try
			{
				//
			}
			catch (Exception ex)
			{
				Clients.Caller.SendAsync("onError", "OnDisconnected: " + ex.Message);
			}

			return base.OnDisconnectedAsync(exception);
		}

		private string IdentityName
		{
			get { return Context.User.Identity.Name; }
		}
	}
}
