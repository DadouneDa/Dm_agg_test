using DmAggregator.ApplicationInsights;
using DmAggregator.Hashing;
using DmAggregator.Models;
using DmAggregator.Services.Ovoc;
using DmAggregator.Services.Redis;
using DmAggregator.Utils;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Caching.Distributed;
using System.Collections.Concurrent;
using System.Reflection;

namespace DmAggregator.Aggregation
{
    /// <summary>
    /// 
    /// </summary>
    public partial class Aggregator
    {
        private readonly ILogger<Aggregator> _logger;
        private readonly AggregatorConfig _aggregatorConfig;
        private readonly IRedisCacheService _cache;
        private readonly IBackgroundSenderQueue<string> _shortBackgroundQ;
        private readonly IBackgroundSenderQueue<IPPKeepAliveRequest> _fullBackgroundQ;
        private readonly OvocCache _ovocCache;
        private readonly TelemetryClient _telemetryClient;
        private readonly TimeSpan _cacheTtlTimespan;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="aggregatorConfig"></param>
        /// <param name="cache"></param>
        /// <param name="shortBackgroundQ"></param>
        /// <param name="fullBackgroundQ"></param>
        /// <param name="ovocCache"></param>
        /// <param name="telemetryClient"></param>
        public Aggregator(
            ILogger<Aggregator> logger,
            AggregatorConfig aggregatorConfig,
            IRedisCacheService cache,
            IBackgroundSenderQueue<string> shortBackgroundQ,
            IBackgroundSenderQueue<IPPKeepAliveRequest> fullBackgroundQ,
            OvocCache ovocCache,
            TelemetryClient telemetryClient
            )
        {
            this._logger = logger;
            this._aggregatorConfig = aggregatorConfig;
            this._cache = cache;
            this._shortBackgroundQ = shortBackgroundQ;
            this._fullBackgroundQ = fullBackgroundQ;
            this._ovocCache = ovocCache;
            this._telemetryClient = telemetryClient;
            this._cacheTtlTimespan = TimeSpan.FromSeconds(this._aggregatorConfig.CacheTtlSeconds);
        }

        // static empty response
        private static readonly Ok<IppKeepAliveResponse> s_emptyResponse = TypedResults.Ok(new IppKeepAliveResponse { Requests = new string[] { }, });

        // Properties to ignore when computing hash.
        // They're randomly generated for each request
        private static readonly PropertyInfo[] s_ignoreProperties =
            new PropertyInfo[]
            {
                typeof(IPPKeepAliveRequest).GetProperty(nameof(IPPKeepAliveRequest.EmsUserPassword)) ?? throw new ApplicationException(),
                typeof(IPPKeepAliveRequest).GetProperty(nameof(IPPKeepAliveRequest.SessionId)) ?? throw new ApplicationException()
            };

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        public async Task<Results<Ok< IppKeepAliveResponse>, UnprocessableEntity<string>, NotFound<string>>> AddEntry(IPPKeepAliveRequest entry)
        {
            AggregatorEventCountersSource.Log.IncCounter(AggregatorCounters.KeepAliveCounter);

            string mac = entry.MAC ?? throw new ArgumentNullException(nameof(entry.MAC));

            // Send custom event details as configured
            var props = this._aggregatorConfig.FullKaCustomEvent ?
                FlatDictionary.FromObject(entry) :
                new Dictionary<string, string>
                {
                    [AppInsightsConsts.MacProperty] = mac
                };

            Results<Ok<IppKeepAliveResponse>, UnprocessableEntity<string>, NotFound<string>> response;

            // First of all verify customers cache contains some entries
            if (this._ovocCache.Customers == null)
            {
                // Return 422 if failed to get customers
                AggregatorEventCountersSource.Log.IncCounter(AggregatorCounters.IgnoredNoCustomers);
                props.Add(AppInsightsConsts.CustomerStatusProperty, AppInsightsConsts.NoCustomersValue);
                response = TypedResults.UnprocessableEntity("No customers");
            }
            else if (!this._ovocCache.Customers.Any(x => x.CustomerId == entry.CustomerId))
            {
                // return 404 if customer not found
                AggregatorEventCountersSource.Log.IncCounter(AggregatorCounters.CustomerNotFound);
                props.Add(AppInsightsConsts.CustomerStatusProperty, AppInsightsConsts.NotFoundValue);
                response = TypedResults.NotFound($"Unknown customer '{entry.CustomerId}'");
            }
            else
            {
                props.Add(AppInsightsConsts.CustomerStatusProperty, AppInsightsConsts.ValidValue);
                if (this._ovocCache.RejectedMacs?.ContainsKey(mac) == true)
                {
                    // This MAC should be processed as if it's not cached
                    await this._cache.KeyDeleteAsync(mac);

                    // Clear for next time
                    this._ovocCache.RejectedMacs.TryRemove(mac, out _);

                    AggregatorEventCountersSource.Log.IncCounter(AggregatorCounters.RejectedMacProcessed);
                    props.Add(AppInsightsConsts.RejectedMacProperty, true.ToString());

                    // continue normally
                }

                // Note, in the past hashing was computed on the IPP KeepAlive body only, and performed by the middleware.
                // However, hashing must now also include non-body parts, such as IP address, customerId.
                // Therefore, request body hash is not used anymore.
                // Instead, it is computed manually

                // NOTE - Ignore SessionId and EmsUserPassword because they are randomized in each request
                byte[] hash = ObjectHasherHelper.ComputeHashReusable(entry, s_ignoreProperties);

                // Get entry and refresh its timeout, if exists
                RedisIppEntry? redisIppEntry = await this._cache.GetObjectSetExpiry<RedisIppEntry>(mac, this._cacheTtlTimespan);

                var cachedHash = redisIppEntry?.LastKeepAliveHash;

                IEnumerable<string>? actions;

                if (cachedHash != null && Enumerable.SequenceEqual(hash, cachedHash))
                {
                    // Same hash 
                    AggregatorEventCountersSource.Log.IncCounter(AggregatorCounters.CacheHitCounter);
                    props.Add(AppInsightsConsts.CacheStatusProperty, AppInsightsConsts.CacheHitValue);

                    // Add to short KA queue
                    this._shortBackgroundQ.EnqueueItem(mac);

                    actions = await this.GetActionsAndUpdateEntry(mac, redisIppEntry!, forceSave: false);
                }
                else
                {
                    // no redis entry or cache mismatch
                    if (redisIppEntry != null)
                    {
                        AggregatorEventCountersSource.Log.IncCounter(AggregatorCounters.CacheChangedCounter);
                        props.Add(AppInsightsConsts.CacheStatusProperty, AppInsightsConsts.CacheChangedValue);
                    }
                    else
                    {
                        AggregatorEventCountersSource.Log.IncCounter(AggregatorCounters.CacheNotFoundCounter);
                        props.Add(AppInsightsConsts.CacheStatusProperty, AppInsightsConsts.NotFoundValue);

                        redisIppEntry = new RedisIppEntry();
                    }

                    // Update hash
                    redisIppEntry.LastKeepAliveHash = hash;

                    // Add to full KA queue
                    this._fullBackgroundQ.EnqueueItem(entry);

                    actions = await this.GetActionsAndUpdateEntry(mac, redisIppEntry, forceSave: true);
                }

                if (actions != null)
                {
                    props.Add(AppInsightsConsts.ActionsProperty, string.Join('|', actions));
                    response = TypedResults.Ok(new IppKeepAliveResponse { Requests = actions });
                }
                else
                {
                    response = s_emptyResponse;
                }
            }

            // Send custom event
            this._telemetryClient.TrackEvent(AppInsightsConsts.IppKeepAliveEvent, props);
            return response;
        }

        private async Task<IEnumerable<string>?> GetActionsAndUpdateEntry(string mac, RedisIppEntry redisIppEntry, bool forceSave)
        {
            var actions = redisIppEntry.Actions;
            redisIppEntry.Actions = null;

            if (actions != null || forceSave)
            {
                // Save with expiration
                await this._cache.SetObject(mac, redisIppEntry, this._cacheTtlTimespan);
            }
            else
            {
                // TTL was already set at GET
            }

            var utcNowTicks = DateTimeOffset.UtcNow.UtcTicks;

            return actions?
                .Where(x => x.ExpiredUtcTicks > utcNowTicks)
                .Select(x => x.Body);
        }
    }
}
