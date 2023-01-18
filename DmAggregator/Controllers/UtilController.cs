using DmAggregator.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;

namespace DmAggregator.Controllers
{
    /// <summary>
    /// Test Controller
    /// </summary>
    [ApiController]
    [Route("api/util")]
    public class UtilController : ControllerBase
    {
        /// <summary>
        /// Get Application Insights counters
        /// </summary>
        /// <returns></returns>
        [HttpGet("counters")]
        public ConcurrentDictionary<AggregatorCounters, double> GetCounters()
        {
            return AggregatorEventCountersSource.Log.Counters;
        }

        /// <summary>
        /// Get version info
        /// </summary>
        /// <returns></returns>
        [HttpGet("version")]
        public ActionResult GetVersionInfo()
        {
            string assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version!.ToString();
            FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);

            return Ok(new
            {
                ProductVersion = fileVersionInfo.ProductVersion,
                FileVersion = fileVersionInfo.FileVersion,
                AssemblyVersion = assemblyVersion,
            });
        }
    }
}
