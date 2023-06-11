using AutoMapper;
using GameApp.Domain;
using Microsoft.AspNetCore.Mvc;

namespace GameApp.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public abstract class GameController : ControllerBase
    {
        protected GameContext Context { get; private set; }
        protected Mapper Mapper { get; set; }

        protected GameController(GameContext context, Mapper mapper)
        {
            Context = context;
            Mapper = mapper;
        }
    }
}
