using AutoMapper;
using GameApp.Domain;
using Microsoft.AspNetCore.Mvc;

namespace GameApp.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public abstract class GamesController : ControllerBase
    {
        protected GameContext Context { get; private set; }
        protected IMapper Mapper { get; set; }

        protected GamesController(GameContext context, IMapper mapper)
        {
            Context = context;
            Mapper = mapper;
        }
    }
}
