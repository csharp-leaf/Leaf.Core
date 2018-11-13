using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Leaf.Core.Extensions.String.Tests
{
    [TestClass]
    public class StringBetweenExtensionsTests
    {
        private const string BetweenLeft = "<h1>";
        private const string BetweenRight = "</h1>";

        private const string BetweenTextFirst = "text 1";
        private const string BetweenTextSecond = "text 2";
        private const string BetweensText = BetweenLeft + BetweenTextFirst + BetweenRight + BetweenLeft + BetweenTextSecond + BetweenRight + " hello world";

        private const string BetweenNotExistingLeft = "<nothing>";
        private const string BetweenNotExistingRight = "</nothing>";

        [TestMethod]
        public void BetweensOrEmptyTest()
        {
            var res = BetweensText.BetweensOrEmpty(BetweenLeft, BetweenRight);
            Assert.IsNotNull(res);
            Assert.IsTrue(res.Length == 2);

            Assert.AreEqual(BetweenTextFirst, res[0]);
            Assert.AreEqual(BetweenTextSecond, res[1]);

            res = BetweensText.BetweensOrEmpty(BetweenNotExistingLeft, BetweenNotExistingRight);
            Assert.IsNotNull(res);
            Assert.IsTrue(res.Length == 0);
        }

        [TestMethod]
        public void BetweensTest()
        {
            var res = BetweensText.Betweens(BetweenLeft, BetweenRight);
            Assert.IsNotNull(res);
            Assert.IsTrue(res.Length == 2);

            Assert.AreEqual(BetweenTextFirst, res[0]);
            Assert.AreEqual(BetweenTextSecond, res[1]);

            res = BetweensText.Betweens(BetweenNotExistingLeft, BetweenNotExistingRight);
            Assert.IsNull(res);
        }

        [TestMethod]
        public void BetweensExTest()
        {
            var res = BetweensText.BetweensEx(BetweenLeft, BetweenRight);
            Assert.IsTrue(res.Length == 2);

            Assert.AreEqual(BetweenTextFirst, res[0]);
            Assert.AreEqual(BetweenTextSecond, res[1]);

            Assert.ThrowsException<StringBetweenException>(() => {
                BetweensText.BetweensEx(BetweenNotExistingLeft, BetweenNotExistingRight);
            });
        }

        [TestMethod]
        public void BetweenTest()
        {
            string res = BetweensText.Between(BetweenLeft, BetweenRight);
            Assert.IsFalse(string.IsNullOrEmpty(res));
            Assert.AreEqual(BetweenTextFirst, res);

            res = BetweensText.Between(BetweenNotExistingLeft, BetweenNotExistingRight);
            Assert.IsNull(res);
        }

        [TestMethod]
        public void BetweenOrEmptyTest()
        {
            string res = BetweensText.BetweenOrEmpty(BetweenLeft, BetweenRight);
            Assert.IsFalse(string.IsNullOrEmpty(res));
            Assert.AreEqual(BetweenTextFirst, res);

            res = BetweensText.BetweenOrEmpty(BetweenNotExistingLeft, BetweenNotExistingRight);
            Assert.IsNotNull(res);
            Assert.AreEqual(string.Empty, res);
        }

        [TestMethod]
        public void BetweenExTest()
        {
            string res = BetweensText.BetweenEx(BetweenLeft, BetweenRight);
            Assert.IsFalse(string.IsNullOrEmpty(res));
            Assert.AreEqual(BetweenTextFirst, res);

            Assert.ThrowsException<StringBetweenException>(() => {
                BetweensText.BetweenEx(BetweenNotExistingLeft, BetweenNotExistingRight);
            });
        }

        [TestMethod]
        public void BetweenLastTest()
        {
            string res = BetweensText.BetweenLast(BetweenRight, BetweenLeft);
            Assert.IsFalse(string.IsNullOrEmpty(res));
            Assert.AreEqual(BetweenTextSecond, res);

            res = BetweensText.BetweenLast(BetweenNotExistingRight, BetweenNotExistingLeft);
            Assert.IsNull(res);
        }

        [TestMethod]
        public void BetweenLastOrEmptyTest()
        {
            string res = BetweensText.BetweenLastOrEmpty(BetweenRight, BetweenLeft);
            Assert.IsFalse(string.IsNullOrEmpty(res));
            Assert.AreEqual(BetweenTextSecond, res);

            res = BetweensText.BetweenLastOrEmpty(BetweenNotExistingRight, BetweenNotExistingLeft);
            Assert.IsNotNull(res);
            Assert.AreEqual(string.Empty, res);
        }

        [TestMethod]
        public void BetweenLastExTest()
        {
            string res = BetweensText.BetweenLastEx(BetweenRight, BetweenLeft);
            Assert.IsFalse(string.IsNullOrEmpty(res));
            Assert.AreEqual(BetweenTextSecond, res);

            Assert.ThrowsException<StringBetweenException>(() => {
                BetweensText.BetweenLastEx(BetweenNotExistingRight, BetweenNotExistingLeft);
            });

        }
    }
}