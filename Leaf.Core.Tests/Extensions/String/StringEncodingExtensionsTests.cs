using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Leaf.Core.Extensions.String.Tests
{
    [TestClass()]
    public class StringEncodingExtensionsTests
    {
        private const string DecodedJson = "{\"msg\":\"привет мир!\"}";
        private const string EncodedJson = "{\"msg\":\"\\u043f\\u0440\\u0438\\u0432\\u0435\\u0442 \\u043c\\u0438\\u0440!\"}";


        [TestMethod]
        public void EncodeJsonUnicodeTest()
        {
            Assert.AreEqual(EncodedJson, DecodedJson.EncodeJsonUnicode());
        }

        [TestMethod]
        public void DecodeJsonUnicodeTest()
        {
            Assert.AreEqual(DecodedJson, EncodedJson.DecodeJsonUnicode());
        }
    }
}