using DmAggregator.Models;
using DmAggregator.Services.Ovoc;
using System.Collections.Concurrent;

namespace DmAggregator.HostedServices
{
    /// <summary>
    /// Background service that periodically polls Ovoc DM customers and stores them in singleton cache.
    /// IPP KA are only allowed from these valid customers
    /// </summary>
    public class BackgroundOvocPolling : BackgroundService
    {
        private readonly IOvocService _ovocService;
        private readonly BackgroudOvocPollingConfig _config;
        private readonly OvocCache _ovocCache;
        private readonly ILogger<BackgroundOvocPolling> _logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ovocService"></param>
        /// <param name="config"></param>
        /// <param name="ovocCache"></param>
        /// <param name="logger"></param>
        public BackgroundOvocPolling(IOvocService ovocService, BackgroudOvocPollingConfig config,
            OvocCache ovocCache,
            ILogger<BackgroundOvocPolling> logger)
        {
            this._ovocService = ovocService;
            this._config = config;
            this._ovocCache = ovocCache;
            this._logger = logger;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await this.GetDmCustomers();

                await this.GetRejectedEndpoints();

                await Task.Delay(TimeSpan.FromSeconds(this._config.PollingPeriodSeconds), stoppingToken);
            }
        }

        private async Task GetDmCustomers()
        {
            try
            {
                var response = await this._ovocService.GetDmCustomers();

                // Update singleton
                this._ovocCache.Customers = response.Customers
                    .Where(x => x.CustomerId != null)
                    .ToArray();

                this._ovocCache.GetCustomersLastUpdateUtc = DateTime.UtcNow;
                this._ovocCache.GetCustomersSuccessReads++;
            }
            catch (Exception ex)
            {
                this._ovocCache.GetCustomersFailReads++;
                this._logger.LogWarning(ex, "Failed to get OVOC customers");
            }
        }

        private async Task GetRejectedEndpoints()
        {
            try
            {
                var response = await this._ovocService.GetOvocRejectedEndpoints();


                var rejectedMacs = response.RejectedEndpoints
                    .Where(x => x.Mac != null)
                    .DistinctBy(x => x.Mac)
                    .ToDictionary(x => x.Mac, x => (object?)null);

                // Update singleton
                this._ovocCache.RejectedMacsOrig = new ConcurrentDictionary<string, object?>(rejectedMacs);
                this._ovocCache.RejectedMacs = new ConcurrentDictionary<string, object?>(this._ovocCache.RejectedMacsOrig);

                this._ovocCache.RejectedMacsLastUpdateUtc = DateTime.UtcNow;
                this._ovocCache.RejectedMacsSuccessReads++;
            }
            catch (Exception ex)
            {
                this._ovocCache.RejectedMacsFailReads++;
                this._logger.LogWarning(ex, "Failed to get OVOC rejected endpoints");
            }
        }
    }
}
