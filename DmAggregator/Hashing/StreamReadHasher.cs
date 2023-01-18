using System.Security.Cryptography;
using System.Text;

namespace DmAggregator.Hashing
{
    /// <summary>
    /// Computes hash on read of wrapped internal stream
    /// </summary>
    public class StreamReadHasher : StreamWrapper
    {
        private static readonly byte[] _emptyBuffer = new byte[0];

        private readonly HttpContext _context;
        private readonly Stream _internalStream;
        private readonly Lazy<HashAlgorithm> _hash;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="internalStream"></param>
        public StreamReadHasher(HttpContext context, Stream internalStream) : base(internalStream)
        {
            this._context = context;
            this._internalStream = internalStream;

            // Create hash only when needed, otherwise it's created and disposed for each request without a body as well.
            this._hash = new Lazy<HashAlgorithm>(() => SHA1.Create());
        }

        /// <inheritdoc/>
        public override int Read(byte[] buffer, int offset, int count)
        {
            int read = this._internalStream.Read(buffer, offset, count);

            this.ComputeHashInternal(buffer, offset, read);

            return read;
        }

        /// <inheritdoc/>
        public override int Read(Span<byte> buffer)
        {
            int read = this._internalStream.Read(buffer);

            var array = read != 0 ? buffer.Slice(0, read).ToArray() : _emptyBuffer;

            this.ComputeHashInternal(array, 0, read);

            return read;
        }

        /// <inheritdoc/>
        public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            int read = await this._internalStream.ReadAsync(buffer, offset, count, cancellationToken);

            this.ComputeHashInternal(buffer, offset, read);

            return read;
        }

        /// <inheritdoc/>
        public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            var read = await this._internalStream.ReadAsync(buffer, cancellationToken);

            var array = read != 0 ? buffer.Slice(0, read).ToArray() : _emptyBuffer;

            this.ComputeHashInternal(array, 0, read);

            return read;
        }

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this._hash.IsValueCreated)
                {
                    this._hash.Value.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        private void ComputeHashInternal(byte[] buffer, int offset, int count)
        {
            if (count != 0)
            {
                this._hash.Value.TransformBlock(buffer, offset, count, null, 0);
            }
            else
            {
                this._hash.Value.TransformFinalBlock(_emptyBuffer, 0, 0);

                this._context.SetRequestBodyHash(this._hash.Value.Hash!);
            }
        }
    }
}
