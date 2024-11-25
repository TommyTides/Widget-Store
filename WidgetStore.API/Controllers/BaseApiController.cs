using Microsoft.AspNetCore.Mvc;

namespace WidgetStore.API.Controllers
{
    /// <summary>
    /// Base API controller that provides common functionality for all controllers
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public abstract class BaseApiController : ControllerBase
    {
    }
}