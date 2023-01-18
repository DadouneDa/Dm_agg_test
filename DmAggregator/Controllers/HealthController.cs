using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DmAggregator.Controllers
{
    /// <summary>
    /// Health probe controller
    /// </summary>
    [ApiController]
    [Route("api/health")]
    public class HealthController : ControllerBase
    {
        /// <summary>
        /// Liveness test
        /// </summary>
        /// <returns></returns>
        [HttpGet("")]
        [AllowAnonymous]
        public ActionResult Liveness()
        {
            return Ok();
        }
    }
}
