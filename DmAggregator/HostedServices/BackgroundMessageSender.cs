using DmAggregator.Aggregation;
using DmAggregator.Models;
using DmAggregator.Services.Ovoc;

namespace DmAggregator.HostedServices
{
    /// <summary>
    /// Worker service for sending both short and full messages
    /// </summary>
    public class BackgroundMessageSender : BackgroundService
    {
        private readonly BackgroundMessageSenderConfig _backgroundMessageSenderConfig;
        private readonly TimeSpan _timeThreshold;
        private readonly IBackgroundSenderQueue<IPPKeepAliveRequest> _fullBackgroundQ;
        private readonly IBackgroundSenderQueue<string> _shortBackgroundQ;
        private readonly IOvocService _ovocService;
        private readonly ILogger<BackgroundMessageSender> _logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="backgroundMessageSenderConfig"></param>
        /// <param name="fullBackgroundQ"></param>
        /// <param name="shortBackgroundQ"></param>
        /// <param name="ovocService"></param>
        /// <param name="logger"></param>
        public BackgroundMessageSender(
            BackgroundMessageSenderConfig backgroundMessageSenderConfig,
            IBackgroundSenderQueue<IPPKeepAliveRequest> fullBackgroundQ,
            IBackgroundSenderQueue<string> shortBackgroundQ,
            IOvocService ovocService,
            ILogger<BackgroundMessageSender> logger
            )
        {
            this._backgroundMessageSenderConfig = backgroundMessageSenderConfig;
            this._timeThreshold = TimeSpan.FromSeconds(backgroundMessageSenderConfig.TimeThresholdSeconds);
            this._fullBackgroundQ = fullBackgroundQ;
            this._shortBackgroundQ = shortBackgroundQ;
            this._ovocService = ovocService;
            this._logger = logger;
        }

        /// <inheritdoc/>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var cts = new CancellationTokenSource(this._timeThreshold))
                    {
                        using (var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken, cts.Token))
                        {
                            var fullMessageTask = this._fullBackgroundQ.ReadBatchAsync(linkedCts.Token);
                            var shortMessageTask = this._shortBackgroundQ.ReadBatchAsync(linkedCts.Token);

                            var completed = await Task.WhenAny(fullMessageTask, shortMessageTask);

                            // cancel the other
                            linkedCts.Cancel();

                            await Task.WhenAll(fullMessageTask, shortMessageTask);

                            var fullEntries = await fullMessageTask;

                            var shortEntries = await shortMessageTask;

                            // Always send full entries before short entries, so that Ovoc will recognize the short entries!
                            if (fullEntries.Any())
                            {
                                // Bug fix - don't filter fullEntries = fullEntries.DistinctBy(x => x.MAC);
                                // because two entries here can be different and order is critical.
                                // First entry will be "status: started", second entry will be "status: registered"
                                // If filtered then onlt "started" will be sent.
                                await this._ovocService.SendFullMultiKeepAlive(
                                    new FullMultiKeepAliveRequest { Requests = fullEntries }
                                    );
                            }

                            if (shortEntries.Any())
                            {
                                // Don't send duplicates.
                                // Here it's ok to filter, because entries are only MAC address and really redundant
                                shortEntries = shortEntries.Distinct();

                                await this._ovocService.SendShortMultiKeepAlive(
                                    new ShortMultiKeepAliveRequest { Endpoints = shortEntries }
                                    );
                            }
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                }
                catch (Exception ex)
                {
                    this._logger.LogError(ex, "Error reading/processing queue entries");
                }
            }
        }
    }
}
