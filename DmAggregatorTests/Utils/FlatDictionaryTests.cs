using Microsoft.VisualStudio.TestTools.UnitTesting;
using DmAggregator.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using System.Globalization;

namespace DmAggregator.Utils.Tests
{
    [TestClass()]
    public class FlatDictionaryTests
    {
        enum MyEnum
        {
            EnumVal1,
            EnumVal2
        };

        class MyClass
        {
            public string? MyNull { get; set; }

            public string? MyStr { get; set; }

            public MyEnum MyEnum { get; set; }

            public int MyInt { get; set; }

            public bool MyBool { get; set; }

            public DateTime MyDateTime { get; set; }

            public int? MyNullInt { get; set; }

            public TimeSpan MySpan { get; set; }

            public TimeSpan MyNullSpan { get; set; }

            public MyOtherClass? OtherClass { get; set; }

            public string[]? MyStrArray { get; set; }

            public MyOtherClass[]? MyOtherClassArray { get; set; }
        }

        class MyOtherClass
        {
            public string? MyStr2 { get; set; }

            public MyEnum MyEnum { get; set; }

            public int MyInt2 { get; set; }

            public bool MyBool2 { get; set; }
        }

        [TestMethod()]
        public void FromObjectTest()
        {
            DateTime now = DateTime.Now;

            MyClass myClass = new MyClass
            {
                MyStr = "zzz",
                MyEnum = MyEnum.EnumVal2,
                MyBool = true,
                MyDateTime = now,
                MyInt = 17,
                MyNullInt = 18,
                MyNullSpan = TimeSpan.FromDays(3),
                MySpan = TimeSpan.FromHours(21),
                OtherClass = new MyOtherClass
                {
                    MyStr2 = "QQQ",
                    MyBool2 = true,
                    MyEnum = MyEnum.EnumVal2,
                    MyInt2 = 88,
                },
                MyStrArray = new[] { "ab", "cd" },
                MyOtherClassArray = new MyOtherClass[] { new MyOtherClass { MyStr2 = "zz" } }
            };

            var dict = FlatDictionary.FromObject(myClass);

            dict.Should().BeEquivalentTo(new Dictionary<string, string>
            {
                ["myStr"] = "zzz",
                ["myEnum"] = "EnumVal2",
                ["myBool"] = true.ToString(),
                ["myDateTime"] = now.ToString("o"),
                ["myInt"] = "17",
                ["myNullInt"] = "18",
                ["myNullSpan"] = TimeSpan.FromDays(3).ToString(),
                ["mySpan"] = TimeSpan.FromHours(21).ToString(),
                ["otherClass.myStr2"] = "QQQ",
                ["otherClass.myBool2"] = true.ToString(),
                ["otherClass.myEnum"] = "EnumVal2",
                ["otherClass.myInt2"] = "88",
                ["myStrArray.0"] = "ab",
                ["myStrArray.1"] = "cd",
                ["myOtherClassArray.0.myStr2"] = "zz",
                ["myOtherClassArray.0.myBool2"] = false.ToString(),
                ["myOtherClassArray.0.myEnum"] = "EnumVal1",
                ["myOtherClassArray.0.myInt2"] = "0",
            });

            dict.Keys.Contains(nameof(MyClass.MyNull), StringComparer.OrdinalIgnoreCase).Should().BeFalse();
            dict.Keys.Where(k => k.Contains(nameof(MyClass.OtherClass), StringComparison.OrdinalIgnoreCase)).Should().NotBeEmpty();
        }

        [TestMethod]
        public void MaxDepthTest()
        {
            DateTime now = DateTime.Now;

            MyClass myClass = new MyClass
            {
                MyStr = "zzz",
                MyEnum = MyEnum.EnumVal2,
                MyBool = true,
                MyDateTime = now,
                MyInt = 17,
                MyNullInt = 18,
                MyNullSpan = TimeSpan.FromDays(3),
                MySpan = TimeSpan.FromHours(21),
                OtherClass = new MyOtherClass
                {
                    MyStr2 = "QQQ",
                    MyBool2 = true,
                    MyEnum = MyEnum.EnumVal2,
                    MyInt2 = 88,
                },
                MyStrArray = new[] { "ab", "cd" },
                MyOtherClassArray = new MyOtherClass[] { new MyOtherClass { MyStr2 = "zz" } }
            };

            var dict = FlatDictionary.FromObject(myClass, maxDepth: 1);

            dict.Should().BeEquivalentTo(new Dictionary<string, string>
            {
                ["myStr"] = "zzz",
                ["myEnum"] = "EnumVal2",
                ["myBool"] = true.ToString(),
                ["myDateTime"] = now.ToString("o"),
                ["myInt"] = "17",
                ["myNullInt"] = "18",
                ["myNullSpan"] = TimeSpan.FromDays(3).ToString(),
                ["mySpan"] = TimeSpan.FromHours(21).ToString(),
                ["myStrArray.0"] = "ab",
                ["myStrArray.1"] = "cd",
            });

            dict.Keys.Contains(nameof(MyClass.MyNull), StringComparer.OrdinalIgnoreCase).Should().BeFalse();
            dict.Keys.Where(k => k.Contains(nameof(MyClass.OtherClass), StringComparison.OrdinalIgnoreCase)).Should().BeEmpty();
        }


    }
}