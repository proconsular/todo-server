using Microsoft.AspNetCore.Mvc;
using Todo.Constants;

namespace Todo.Controllers {
    /// <summary>
    /// Controller used for verifying the health of the API.
    /// </summary>
    [Route(ControllerConstants.DefaultRoute)]
    [ApiController]
    public class HealthController : ControllerBase {
        [HttpGet]
        public IActionResult Check() {
            return Ok();
        }
    }
}