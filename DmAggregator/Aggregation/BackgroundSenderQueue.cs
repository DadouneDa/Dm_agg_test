using System.Threading.Channels;
using DmAggregator.Models;

namespace DmAggregator.Aggregation
{
    /// <summary>
    /// Background sender queue implementation
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BackgroundSenderQueue<T> : IBackgroundSenderQueue<T>
    {
        private readonly Channel<T> _channel;
        private readonly BackgroundSenderQueueConfig _queueConfig;
        private readonly ILogger<BackgroundSenderQueue<T>> _logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="queueConfig"></param>
        /// <param name="logger"></param>
        public BackgroundSenderQueue(BackgroundSenderQueueConfig queueConfig, ILogger<BackgroundSenderQueue<T>> logger)
        {
            this._queueConfig = queueConfig;
            this._logger = logger;

            var options = new BoundedChannelOptions(queueConfig.Capacity)
            {
                // Note - oldest are dropped. There is no back pressure
                FullMode = BoundedChannelFullMode.DropOldest,
                SingleReader = true,
            };

            this._channel = Channel.CreateBounded<T>(options);
        }

        /// <inheritdoc/>
        public void EnqueueItem(T item)
        {
            // Note - always succeeds immediately because channel is bounded and FullMode = BoundedChannelFullMode.DropOldest
            this._channel.Writer.TryWrite(item);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<T>> ReadBatchAsync(CancellationToken cancellationToken)
        {
            List<T> result = new List<T>();

            try
            {
                while (result.Count < this._queueConfig.NumThreshold)
                {
                    // Try to read sync first
                    if (this._channel.Reader.TryRead(out var item))
                    {
                    }
                    else
                    {
                        // No items ready.
                        // Wait with cancellationToken that will be canceled with timeout or when other Q completed.
                        item = await this._channel.Reader.ReadAsync(cancellationToken);
                    }

                    result.Add(item);
                }
            }
            catch (OperationCanceledException)
            {
                // reached on timeout or canceled when other Q completed
            }

            return result;
        }
    }
}
