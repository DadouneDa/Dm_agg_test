namespace DmAggregator.Aggregation
{
    /// <summary>
    /// Background sender read and write interface
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IBackgroundSenderQueue<T> 
    {
        /// <summary>
        /// Add entry. Note - this should always succeeds immediately, and in case full oldest entry must be discarded.
        /// </summary>
        /// <param name="item"></param>
        void EnqueueItem(T item);

        /// <summary>
        /// Returns all entries that should be sent
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<IEnumerable<T>> ReadBatchAsync(CancellationToken cancellationToken);
    }
}
