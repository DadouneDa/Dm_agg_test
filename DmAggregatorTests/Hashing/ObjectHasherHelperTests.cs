using Microsoft.VisualStudio.TestTools.UnitTesting;
using DmAggregator.Hashing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using System.Net;

namespace DmAggregator.Hashing.Tests
{
    [TestClass()]
    public class ObjectHasherHelperTests
    {
        class Poco
        {
            public int MyInt { get; set; } = 17;

            public string MyStr { get; set; } = "str1";

            public bool MyBool { get; set; } = true;

            public string MyStr2 { get; set; } = "str2";

        }

        [TestMethod()]
        public void ComputeHashTest()
        {
            Poco poco1 = new Poco();
            Poco poco2 = new Poco();
            var hash1 = ObjectHasherHelper.ComputeHash(poco1);
            var hash2 = ObjectHasherHelper.ComputeHash(poco2);

            hash1.Should().BeEquivalentTo(hash2);

            poco1.MyInt++;
            var hash3 = ObjectHasherHelper.ComputeHash(poco1);
            hash3.Should().NotBeEquivalentTo(hash2);

            poco1.MyInt--;
            var hash4 = ObjectHasherHelper.ComputeHash(poco1);
            hash4.Should().BeEquivalentTo(hash1);

        }

        [TestMethod()]
        public void ComputeHashReusableTest()
        {
            Poco poco1 = new Poco();
            var hash1 = ObjectHasherHelper.ComputeHash(poco1);
            var hash2 = ObjectHasherHelper.ComputeHashReusable(poco1);

            hash1.Should().BeEquivalentTo(hash2);

            for (int i = 0; i < 10; i++)
            {
                var hash3 = ObjectHasherHelper.ComputeHashReusable(poco1);
                hash3.Should().BeEquivalentTo(hash1);
            }
        }

        [TestMethod()]
        public void ComputeHashReusableIgnorePropertiesTest()
        {
            Poco poco1 = new Poco { MyStr = "aaa", MyStr2 = "aaa222" };
            Poco poco2 = new Poco { MyStr = "bbb", MyStr2 = "bbb222" };

            var clone1 = System.Text.Json.JsonSerializer.Deserialize<Poco>(System.Text.Json.JsonSerializer.Serialize(poco1));
            var clone2 = System.Text.Json.JsonSerializer.Deserialize<Poco>(System.Text.Json.JsonSerializer.Serialize(poco2));

            clone1.Should().BeEquivalentTo(poco1);
            clone2.Should().BeEquivalentTo(poco2);

            var hash1 = ObjectHasherHelper.ComputeHashReusable(poco1);
            var hash2 = ObjectHasherHelper.ComputeHashReusable(poco2);

            hash1.Should().NotBeEquivalentTo(hash2);

            hash1 = ObjectHasherHelper.ComputeHashReusable(poco1, new[] { typeof(Poco).GetProperty(nameof(Poco.MyStr)) ?? throw new ApplicationException(), typeof(Poco).GetProperty(nameof(Poco.MyStr2)) ?? throw new ApplicationException() });
            hash2 = ObjectHasherHelper.ComputeHashReusable(poco2, new[] { typeof(Poco).GetProperty(nameof(Poco.MyStr2)) ?? throw new ApplicationException(), typeof(Poco).GetProperty(nameof(Poco.MyStr)) ?? throw new ApplicationException() });

            hash1.Should().BeEquivalentTo(hash2);

            poco1.Should().BeEquivalentTo(clone1);
            poco2.Should().BeEquivalentTo(clone2);

        }
    }
}