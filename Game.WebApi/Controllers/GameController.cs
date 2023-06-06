using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Game.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public abstract class GameController : ControllerBase
    {
    }
}
