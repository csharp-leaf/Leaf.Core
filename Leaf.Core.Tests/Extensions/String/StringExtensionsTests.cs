using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Leaf.Core.Extensions.String.Tests
{
    [TestClass]
    public class StringExtensionsTests
    {
        [TestMethod]
        public void ContainsIgnoreCaseTest()
        {
            const string helloWorld = "hello WoRLd";

            Assert.IsTrue(helloWorld.ContainsIgnoreCase("hello") && helloWorld.ContainsIgnoreCase("world"));
            Assert.IsFalse(helloWorld.ContainsIgnoreCase("something"));
        }

        [TestMethod]
        public void ContainsTest()
        {
            var test = new[] { "hello", "world", "c#", "is", "awesome" };

            Assert.IsTrue(test.Contains("hello"));
            Assert.IsTrue(test.Contains("hEllO", StringComparison.OrdinalIgnoreCase));

            Assert.IsFalse(test.Contains("something"));
            Assert.IsFalse(test.Contains("sOmething", StringComparison.OrdinalIgnoreCase));
        }

        [TestMethod]
        public void ToUpperFirstTest()
        {
            const string hello = "hello";
            const string helloRandomCase = "hEllO";
            Assert.AreNotEqual(hello, helloRandomCase);

            const string helloUpperFirst = "Hello";
            Assert.AreNotEqual(hello, helloUpperFirst);

            Assert.AreEqual(helloUpperFirst, hello.ToUpperFirst(false));
            Assert.AreEqual(helloUpperFirst, helloRandomCase.ToUpperFirst());
            Assert.AreNotEqual(helloUpperFirst, helloRandomCase.ToUpperFirst(false));
        }

        [TestMethod]
        public void GetJsonValueTest()
        {
            const string json = "{\"boolean\":true,\"string\":\"hello\",\"endString\":\"world\"}";
            
            Assert.AreEqual("true", json.GetJsonValue("boolean"));
            Assert.AreEqual("hello", json.GetJsonValue("string"));
            Assert.AreEqual("world", json.GetJsonValue("endString", "\"}"));
            Assert.AreEqual("world", json.GetJsonValue("endString", "\""));

            Assert.IsNull(json.GetJsonValue("notFoundKey", "\"}"));
            Assert.IsNull(json.GetJsonValue("notFoundKey", "\""));
        }

        [TestMethod]
        public void GetJsonValueExTest()
        {
            const string json = "{\"boolean\":true,\"string\":\"hello\",\"endString\":\"world\"}";
            
            Assert.AreEqual("true", json.GetJsonValueEx("boolean"));
            Assert.AreEqual("hello", json.GetJsonValueEx("string"));
            Assert.AreEqual("world", json.GetJsonValueEx("endString", "\"}"));
            Assert.AreEqual("world", json.GetJsonValueEx("endString", "\""));

            Assert.ThrowsException<StringBetweenException>(() => {
                json.GetJsonValueEx("notFoundKey", "\"}");
                json.GetJsonValueEx("notFoundKey", "\"");
            });
        }

        [TestMethod]
        public void GetThousandNumberFormatInfo()
        {
            const long val = 100_000_000;
            string res = val.ToString("N0", StringExtensions.ThousandNumberFormatInfo);

            Assert.AreEqual("100 000 000", res);
        }
    }
}