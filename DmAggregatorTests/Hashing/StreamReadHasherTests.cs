using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Http;
using FluentAssertions;
using System.Threading;
using DmAggregator.Hashing;

namespace DmAggregatorTests.Hashing
{
    [TestClass()]
    public class StreamReadHasherTests
    {
        private Dictionary<string, string> _sha1 = new Dictionary<string, string>
        {
            ["1234"] = "7110eda4d09e062aa5e4a390b0a572ac0d2c0220",
            ["SHA-1 produces a message digest based on principles similar to those used by Ronald L. Rivest of MIT."] = "c5d40c29c9b1cf48fdabce14474c2ab7f8bcf92f",
        };

        // Credit: https://stackoverflow.com/a/311179/16404952
        public static byte[] StringToByteArray(String hex)
        {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }


        [TestMethod()]
        public void ReadTest()
        {
            foreach (var item in this._sha1)
            {
                using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(item.Key)))
                {
                    var httpContext = new DefaultHttpContext();
                    using (StreamReadHasher streamReadHasher = new StreamReadHasher(httpContext, ms))
                    {
                        var buffer = new byte[10];
                        int read, total = 0;
                        int offset = 1;
                        do
                        {
                            read = streamReadHasher.Read(buffer, offset, buffer.Length - offset);
                            total += read;
                        } while (read > 0);

                        total.Should().Be(item.Key.Length);

                        var hash = httpContext.GetRequestBodyHash();
                        hash.Should().NotBeEmpty();
                        hash.Should().Equal(StringToByteArray(item.Value));
                    }
                }
            }
        }

        [TestMethod()]
        public async Task ReadAsyncTest()
        {
            foreach (var item in this._sha1)
            {
                using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(item.Key)))
                {
                    var httpContext = new DefaultHttpContext();
                    using (StreamReadHasher streamReadHasher = new StreamReadHasher(httpContext, ms))
                    {
                        var buffer = new byte[10];
                        int read, total = 0;
                        int offset = 1;
                        do
                        {
                            read = await streamReadHasher.ReadAsync(buffer, offset, buffer.Length - offset, CancellationToken.None);
                            total += read;
                        } while (read > 0);

                        total.Should().Be(item.Key.Length);

                        var hash = httpContext.GetRequestBodyHash();
                        hash.Should().NotBeEmpty();
                        hash.Should().Equal(StringToByteArray(item.Value));
                    }
                }
            }
        }

        [TestMethod()]
        public void ReadSpanTest()
        {
            foreach (var item in this._sha1)
            {
                using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(item.Key)))
                {
                    var httpContext = new DefaultHttpContext();
                    using (StreamReadHasher streamReadHasher = new StreamReadHasher(httpContext, ms))
                    {
                        var buffer = new byte[10];
                        int read, total = 0;
                        int offset = 1;
                        var span = new Span<byte>(buffer, offset, buffer.Length - offset);
                        do
                        {
                            read = streamReadHasher.Read(span);
                            total += read;
                        } while (read > 0);

                        total.Should().Be(item.Key.Length);

                        var hash = httpContext.GetRequestBodyHash();
                        hash.Should().NotBeEmpty();
                        hash.Should().Equal(StringToByteArray(item.Value));
                    }
                }
            }
        }

        [TestMethod()]
        public async Task MemoryReadAsyncTest()
        {
            foreach (var item in this._sha1)
            {
                using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(item.Key)))
                {
                    var httpContext = new DefaultHttpContext();
                    using (StreamReadHasher streamReadHasher = new StreamReadHasher(httpContext, ms))
                    {
                        var buffer = new byte[10];
                        int read, total = 0;
                        int offset = 1;
                        var memory = new Memory<byte>(buffer, offset, buffer.Length - offset);
                        do
                        {
                            read = await streamReadHasher.ReadAsync(memory);
                            total += read;
                        } while (read > 0);

                        total.Should().Be(item.Key.Length);

                        var hash = httpContext.GetRequestBodyHash();
                        hash.Should().NotBeEmpty();
                        hash.Should().Equal(StringToByteArray(item.Value));
                    }
                }
            }
        }
    }
}