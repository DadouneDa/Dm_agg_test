namespace DmAggregator.Hashing
{
    /// <summary>
    /// 
    /// </summary>
    public class StreamWrapper : Stream
    {
        private readonly Stream _stream;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        public StreamWrapper(Stream stream)
        {
            this._stream = stream;
        }

        /// <inheritdoc/>
        public override bool CanRead => _stream.CanRead;

        /// <inheritdoc/>
        public override bool CanSeek => _stream.CanSeek;

        /// <inheritdoc/>
        public override bool CanWrite => _stream.CanWrite;

        /// <inheritdoc/>
        public override long Length => _stream.Length;

        /// <inheritdoc/>
        public override long Position { get => _stream.Position; set => _stream.Position = value; }

        /// <inheritdoc/>
        public override void Flush()
        {
            _stream.Flush();
        }

        /// <inheritdoc/>
        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            return _stream.ReadAsync(buffer, offset, count, cancellationToken);
        }

        /// <inheritdoc/>
        public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            return _stream.ReadAsync(buffer, cancellationToken);
        }

        /// <inheritdoc/>
        public override int Read(byte[] buffer, int offset, int count)
        {
            return _stream.Read(buffer, offset, count);
        }

        /// <inheritdoc/>
        public override long Seek(long offset, SeekOrigin origin)
        {
            return _stream.Seek(offset, origin);
        }

        /// <inheritdoc/>
        public override void SetLength(long value)
        {
            _stream.SetLength(value);
        }

        /// <inheritdoc/>
        public override void Write(byte[] buffer, int offset, int count)
        {
            _stream.Write(buffer, offset, count);
        }

        // Note - internal stream is closed/disposed by whoever created it.
    }
}
