using System.Collections.Concurrent;
using System.Diagnostics.Metrics;
using System.Diagnostics.Tracing;

namespace DmAggregator.ApplicationInsights
{
    /// <summary>
    /// Aggregator <see cref="EventSource"/> counters
    /// </summary>
    [EventSource(Name = AggregatorEventCountersSource.EventSourceName)]
    public sealed class AggregatorEventCountersSource : EventSource
    {
        /// <summary>
        /// 
        /// </summary>
        public const string EventSourceName = "DmAggregator";

        /// <summary>
        /// Singleton instance
        /// </summary>
        public static readonly AggregatorEventCountersSource Log = new AggregatorEventCountersSource();

        /// <summary>
        /// Application counters
        /// </summary>
        public ConcurrentDictionary<AggregatorCounters, double> Counters { get; } = new ConcurrentDictionary<AggregatorCounters, double>()
        {
            [AggregatorCounters.IgnoredNoCustomers] = 0,
            [AggregatorCounters.CustomerNotFound] = 0,
            [AggregatorCounters.KeepAliveCounter] = 0,
            [AggregatorCounters.CacheHitCounter] = 0,
            [AggregatorCounters.CacheChangedCounter] = 0,
            [AggregatorCounters.CacheNotFoundCounter] = 0,
            [AggregatorCounters.RejectedMacProcessed] = 0,
            [AggregatorCounters.OvocFullKeepAliveSend] = 0,
            [AggregatorCounters.OvocFullKeepAliveEntries] = 0,
            [AggregatorCounters.OvocFullKeepAliveSendFail] = 0,
            [AggregatorCounters.OvocShortKeepAliveSend] = 0,
            [AggregatorCounters.OvocShortKeepAliveEntries] = 0,
            [AggregatorCounters.OvocShortKeepAliveSendFail] = 0,
        };

        private readonly List<DiagnosticCounter> _eventSourceCounters;

        private AggregatorEventCountersSource()
        {
            // Note - counters are implemented as polling counters to simplify display in Swagger.
            // For IncrementingEventCounter there is no easy way to get current value.
            this._eventSourceCounters = new List<DiagnosticCounter>
            {
                new IncrementingPollingCounter(AggregatorCounters.IgnoredNoCustomers.ToString(), this,
                () => this.Counters[AggregatorCounters.IgnoredNoCustomers])
                {
                    DisplayRateTimeScale = TimeSpan.FromMinutes(1),
                    DisplayUnits = "count/minute",
                },
                new IncrementingPollingCounter(AggregatorCounters.CustomerNotFound.ToString(), this,
                () => this.Counters[AggregatorCounters.CustomerNotFound])
                {
                    DisplayRateTimeScale = TimeSpan.FromMinutes(1),
                    DisplayUnits = "count/minute",
                },
                new IncrementingPollingCounter(AggregatorCounters.KeepAliveCounter.ToString(), this,
                () => this.Counters[AggregatorCounters.KeepAliveCounter])
                {
                    DisplayRateTimeScale = TimeSpan.FromMinutes(1),
                    DisplayUnits = "count/minute",
                },
                new IncrementingPollingCounter(AggregatorCounters.CacheHitCounter.ToString(), this,
                () => this.Counters[AggregatorCounters.CacheHitCounter])
                {
                    DisplayRateTimeScale = TimeSpan.FromMinutes(1),
                    DisplayUnits = "count/minute",
                },
                new IncrementingPollingCounter(AggregatorCounters.CacheChangedCounter.ToString(), this,
                () => this.Counters[AggregatorCounters.CacheChangedCounter])
                {
                    DisplayRateTimeScale = TimeSpan.FromMinutes(1),
                    DisplayUnits = "count/minute",
                },
                new IncrementingPollingCounter(AggregatorCounters.CacheNotFoundCounter.ToString(), this,
                () => this.Counters[AggregatorCounters.CacheNotFoundCounter])
                {
                    DisplayRateTimeScale = TimeSpan.FromMinutes(1),
                    DisplayUnits = "count/minute",
                },
                new IncrementingPollingCounter(AggregatorCounters.RejectedMacProcessed.ToString(), this,
                () => this.Counters[AggregatorCounters.RejectedMacProcessed])
                {
                    DisplayRateTimeScale = TimeSpan.FromMinutes(1),
                    DisplayUnits = "count/minute",
                },
                new IncrementingPollingCounter(AggregatorCounters.OvocFullKeepAliveSend.ToString(), this,
                () => this.Counters[AggregatorCounters.OvocFullKeepAliveSend])
                {
                    DisplayRateTimeScale = TimeSpan.FromMinutes(1),
                    DisplayUnits = "count/minute",
                },
                new IncrementingPollingCounter(AggregatorCounters.OvocFullKeepAliveEntries.ToString(), this,
                () => this.Counters[AggregatorCounters.OvocFullKeepAliveEntries])
                {
                    DisplayRateTimeScale = TimeSpan.FromMinutes(1),
                    DisplayUnits = "count/minute",
                },
                new IncrementingPollingCounter(AggregatorCounters.OvocFullKeepAliveSendFail.ToString(), this,
                () => this.Counters[AggregatorCounters.OvocFullKeepAliveSendFail])
                {
                    DisplayRateTimeScale = TimeSpan.FromMinutes(1),
                    DisplayUnits = "count/minute",
                },
                new IncrementingPollingCounter(AggregatorCounters.OvocShortKeepAliveSend.ToString(), this,
                () => this.Counters[AggregatorCounters.OvocShortKeepAliveSend])
                {
                    DisplayRateTimeScale = TimeSpan.FromMinutes(1),
                    DisplayUnits = "count/minute",
                },
                new IncrementingPollingCounter(AggregatorCounters.OvocShortKeepAliveEntries.ToString(), this,
                () => this.Counters[AggregatorCounters.OvocShortKeepAliveEntries])
                {
                    DisplayRateTimeScale = TimeSpan.FromMinutes(1),
                    DisplayUnits = "count/minute",
                },
                new IncrementingPollingCounter(AggregatorCounters.OvocShortKeepAliveSendFail.ToString(), this,
                () => this.Counters[AggregatorCounters.OvocShortKeepAliveSendFail])
                {
                    DisplayRateTimeScale = TimeSpan.FromMinutes(1),
                    DisplayUnits = "count/minute",
                },
            };

            // Verify
            if (this._eventSourceCounters.Count != Enum.GetNames<AggregatorCounters>().Length || this._eventSourceCounters.Count != this.Counters.Count)
            {
                throw new ApplicationException("Counters mismatch");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetCounterNames()
        {
            return Enum.GetNames(typeof(AggregatorCounters));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="counter"></param>
        /// <param name="increment"></param>
        /// <returns></returns>
        public double IncCounter(AggregatorCounters counter, double increment = 1)
        {
            return this.Counters.AddOrUpdate(counter, increment, (_, val) => val + increment);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            foreach (var item in this._eventSourceCounters)
            {
                item.Dispose();
            }

            this._eventSourceCounters.Clear();

            base.Dispose(disposing);
        }
    }
}
