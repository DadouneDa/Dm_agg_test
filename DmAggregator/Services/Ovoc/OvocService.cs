using DmAggregator.ApplicationInsights;
using DmAggregator.Models;
using DmAggregator.Utils;

namespace DmAggregator.Services.Ovoc
{
    /// <inheritdoc/>
    public class OvocService : IOvocService
    {
        /// <summary>
        /// 
        /// </summary>
        public const string ShortMultiKeepAlivePath = "/rest/v1/ipphoneMgrStatus/multi-keep-alive";

        /// <summary>
        /// 
        /// </summary>
        public const string FullMultiKeepAlivePath = "/rest/v1/ipphoneMgrStatus/multi-full-keep-alive";

        /// <summary>
        /// 
        /// </summary>
        public const string RejectedEndpointsPath = "/ovoc/v1/topology/rejectedEndpoints";

        /// <summary>
        /// Get DM customers path
        /// <![CDATA[ovoc/v1/topology/customers?detail=1&fields=customerId&filter=(customerSource='DM')]]>
        /// </summary>
        public static string GetCustomersPath => "ovoc/v1/topology/customers" +
            QueryString.Create(new Dictionary<string, string?>
            {
                ["detail"] = "1",
                ["fields"] = "customerId",
                ["filter"] = "(customerSource='DM')"
            }).Value;


        private readonly OvocServiceConfig _config;
        private readonly HttpClient _httpClient;
        private readonly ILogger<OvocService> _logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        /// <param name="httpClient"></param>
        /// <param name="logger"></param>
        public OvocService(OvocServiceConfig config, HttpClient httpClient, ILogger<OvocService> logger)
        {
            _config = config;
            _httpClient = httpClient;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task SendShortMultiKeepAlive(ShortMultiKeepAliveRequest req)
        {
            if (req is null)
            {
                throw new ArgumentNullException(nameof(req));
            }

            try
            {
                // var resp = await this._httpClient.PostAsJsonAsync(ShortMultiKeepAlivePath, req);
                var resp = await _httpClient.PostAsJsonNonChunkedAsync(ShortMultiKeepAlivePath, req);
                resp.EnsureSuccessStatusCode();

                AggregatorEventCountersSource.Log.IncCounter(AggregatorCounters.OvocShortKeepAliveSend);
                AggregatorEventCountersSource.Log.IncCounter(AggregatorCounters.OvocShortKeepAliveEntries, req.Endpoints.Count());
            }
            catch (Exception ex)
            {
                AggregatorEventCountersSource.Log.IncCounter(AggregatorCounters.OvocShortKeepAliveSendFail);
                _logger.LogWarning(ex, "OVOC send short keep-alive error");
            }
        }

        /// <inheritdoc/>
        public async Task SendFullMultiKeepAlive(FullMultiKeepAliveRequest req)
        {
            if (req is null)
            {
                throw new ArgumentNullException(nameof(req));
            }

            try
            {
                // var resp = await this._httpClient.PostAsJsonAsync(FullMultiKeepAlivePath, req);
                var resp = await _httpClient.PostAsJsonNonChunkedAsync(FullMultiKeepAlivePath, req);
                resp.EnsureSuccessStatusCode();

                AggregatorEventCountersSource.Log.IncCounter(AggregatorCounters.OvocFullKeepAliveSend);
                AggregatorEventCountersSource.Log.IncCounter(AggregatorCounters.OvocFullKeepAliveEntries, req.Requests.Count());
            }
            catch (Exception ex)
            {
                AggregatorEventCountersSource.Log.IncCounter(AggregatorCounters.OvocFullKeepAliveSendFail);
                _logger.LogWarning(ex, "OVOC send full keep-alive error");
            }
        }

        /// <inheritdoc/>
        public async Task<OvocCustomersResponse> GetDmCustomers()
        {
            return (await _httpClient.GetFromJsonAsync<OvocCustomersResponse>(GetCustomersPath))!;
        }

        /// <inheritdoc/>
        public async Task<OvocRejectedEndpointsResponse> GetOvocRejectedEndpoints()
        {
            return (await _httpClient.GetFromJsonAsync<OvocRejectedEndpointsResponse>(RejectedEndpointsPath))!;
        }
    }
}
