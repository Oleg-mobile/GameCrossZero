using AutoMapper;
using GameApp.Domain;
using Microsoft.AspNetCore.Mvc;

namespace GameApp.WebApi.Controllers
{
    [Route("api/[controller]")]   //  TODO [HttpPost("[action]")]  ???
	[ApiController]
    public abstract class GameAppController : ControllerBase
    {
        protected GameContext Context { get; private set; }
        protected IMapper Mapper { get; set; }

        protected GameAppController(GameContext context, IMapper mapper)
        {
            Context = context;
            Mapper = mapper;
        }
    }
}
