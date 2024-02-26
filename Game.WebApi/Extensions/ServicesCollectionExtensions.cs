using GameApp.WebApi.Services.Accounts;
using GameApp.WebApi.Services.Games;
using GameApp.WebApi.Services.Rooms;
using GameApp.WebApi.Services.Users;

namespace GameApp.WebApi.Extensions
{
    public static class ServicesCollectionExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddTransient<IRoomService, RoomService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IGameService, GameService>();
            services.AddTransient<IAccountService, AccountService>();

            return services;
        }
    }
}
