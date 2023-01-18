using DmAggregator.Models;
using DmAggregator.Services.Ovoc;
using Microsoft.AspNetCore.Mvc;

namespace DmAggregator.Controllers
{
    /// <summary>
    /// Dummy OVOC receiver
    /// </summary>
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class OvocDummy : ControllerBase
    {
        private readonly ILogger<OvocDummy> _logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        public OvocDummy(ILogger<OvocDummy> logger)
        {
            this._logger = logger;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost(OvocService.ShortMultiKeepAlivePath)]
        public ActionResult MultiShortKeepAlive(ShortMultiKeepAliveRequest req)
        {
            this._logger.LogInformation($"Received MultiShortKeepAlive with {req.Endpoints!.Count()} entries: {string.Join(',', req.Endpoints!)}");
            return Ok();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost(OvocService.FullMultiKeepAlivePath)]
        public ActionResult MultiFullKeepAlive(FullMultiKeepAliveRequest req)
        {
            this._logger.LogInformation($"Received MultiFullKeepAlive with {req.Requests!.Count()} entries: {string.Join(',', req.Requests!.Select(x => x.MAC!))}");
            return Ok();
        }
    }
}
