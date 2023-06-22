using AutoMapper;
using GameApp.Domain;

namespace GameApp.WebApi.Services
{
    public class GameAppService
    {
        protected GameContext Context { get; private set; }
        protected IMapper Mapper { get; set; }

        public GameAppService(GameContext context, IMapper mapper)
        {
            Context = context;
            Mapper = mapper;
        }
    }
}
