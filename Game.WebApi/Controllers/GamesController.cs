using AutoMapper;
using GameApp.Domain;

namespace GameApp.WebApi.Controllers
{
    public class GamesController : GameAppController
    {
        public GamesController(GameContext context, IMapper mapper) : base(context, mapper)
        {
        }
    }
}
