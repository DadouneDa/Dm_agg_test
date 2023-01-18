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
    public class RedisObjectExtensionsTests
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

        enum MyEnum
        {
            EnumVal1,
            EnumVal2,
        }

        class MyPoco
        {
            public int MyInt { get; set; }

            public string? MyString { get; set; }

            public string? MyNull { get; set; }

            public DateTime? MyDateTime { get; set; }

            public MyEnum MyEnum { get; set; }
        }

        class MyPocoX : MyPoco
        {
            public string? MyString2 { get; set; }
        }

        [TestMethod()]
        public async Task GetSetObjectTest()
        {
            var p1 = new MyPoco
            {
                MyInt = 17,
                MyString = "hello",
                MyDateTime = new DateTime(2022, 11, 22, 17, 24, 07),
                MyEnum = MyEnum.EnumVal2,
            };

            string key = "test:key:1";

            await s_db.SetObject(key, p1);

            var p2 = await s_db.GetObject<MyPoco>(key);

            p1.Should().BeEquivalentTo(p2);

            var p3 = await s_db.GetObject<MyPoco>(key + "ZZZ");
            p3.Should().BeNull();

            (await s_db.KeyDeleteAsync(key)).Should().BeTrue();
        }

        [TestMethod()]
        public async Task SchemeChangeTest()
        {
            var p1 = new MyPoco
            {
                MyInt = 17,
                MyString = "hello",
                MyDateTime = new DateTime(2022, 11, 22, 17, 24, 07),
                MyEnum = MyEnum.EnumVal2,
            };

            string key = "test:key:1";

            await s_db.SetObject(key, p1);

            MyPocoX? px = await s_db.GetObject<MyPocoX>(key);

            px.Should().BeEquivalentTo(new MyPocoX
            {
                MyInt = p1.MyInt,
                MyDateTime = p1.MyDateTime,
                MyEnum = p1.MyEnum,
                MyString = p1.MyString,
                MyNull = p1.MyNull,
                MyString2 = null
            });
        }
        [TestMethod()]
        public async Task ByteArrayTest()
        {
            var bytes = Encoding.UTF8.GetBytes("hello");

            string key = "test:key:1";

            await s_db.SetObject(key, bytes);

            var bytes2 = await s_db.GetObject<byte[]>(key);

            bytes2.Should().BeEquivalentTo(bytes);

            Encoding.UTF8.GetString(bytes2!).Should().Be("hello");

            (await s_db.KeyDeleteAsync(key)).Should().BeTrue();
        }
    }
}