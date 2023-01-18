using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StackExchange.Redis;
using FluentAssertions;
using DmAggregator.Services.Redis;

namespace DmAggregator.Redis.Tests
{
    [TestClass()]
    public class IDatabaseScriptExtensionsTests
    {
        private static IDatabase s_db = default!;

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            // For local development run:
            // docker run --name mycontainer -d -p 6379:6379 redis
            // and use connection string "localhost"
            var connection = ConnectionMultiplexer.Connect("localhost");
            s_db = connection.GetDatabase();
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            s_db.Multiplexer.Close();
            s_db.Multiplexer.Dispose();
        }


        [TestMethod()]
        public async Task ScriptStringGetSetExpiryTest()
        {
            string key = "testKey";
            string val = "testVal17";

            await s_db.KeyDeleteAsync(key);

            await s_db.StringSetAsync(key, val);

            (await s_db.StringGetAsync(key)).Should().Be(val);

            (await s_db.KeyExpireTimeAsync(key)).Should().BeNull();

            int expireSeconds = 10;

            string? getVal = await s_db.ScriptStringGetSetExpiry(key, TimeSpan.FromSeconds(expireSeconds));

            getVal.Should().Be(val);

            var ttl = await s_db.KeyExpireTimeAsync(key);

            ttl.Should().NotBeNull();
            ttl.Should().BeAfter(DateTime.UtcNow);

            TimeSpan ts = ttl.Value - DateTime.UtcNow;

            ts.Should().BeLessThanOrEqualTo(TimeSpan.FromSeconds(expireSeconds));

            await Task.Delay(ts);

            await Task.Delay(TimeSpan.FromSeconds(1));

            getVal = await s_db.StringGetAsync(key);

            getVal.Should().BeNull();

            await s_db.StringSetAsync(key, val);

            (await s_db.StringGetAsync(key)).Should().Be(val);

            (await s_db.KeyExpireTimeAsync(key)).Should().BeNull();

            getVal = await s_db.ScriptStringGetSetExpiry(key, TimeSpan.FromSeconds(expireSeconds));

            await Task.Delay(TimeSpan.FromSeconds(expireSeconds / 2));
            getVal = await s_db.ScriptStringGetSetExpiry(key, TimeSpan.FromSeconds(expireSeconds));
            ttl = await s_db.KeyExpireTimeAsync(key);

            ttl.Should().BeCloseTo(DateTime.UtcNow.AddSeconds(expireSeconds), TimeSpan.FromSeconds(2));

            await Task.Delay(TimeSpan.FromSeconds(expireSeconds));
            await Task.Delay(TimeSpan.FromSeconds(1));
            getVal = await s_db.StringGetAsync(key);

            getVal.Should().BeNull();

            getVal = await s_db.ScriptStringGetSetExpiry("No such key", TimeSpan.FromSeconds(expireSeconds));
            getVal.Should().BeNull();
        }
    }
}