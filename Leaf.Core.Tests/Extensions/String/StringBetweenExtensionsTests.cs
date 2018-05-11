using Microsoft.VisualStudio.TestTools.UnitTesting;
using Leaf.Core.Extensions.String;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leaf.Core.Extensions.String.Tests
{
    [TestClass()]
    public class StringBetweenExtensionsTests
    {
        private const string BetweenLeft = "<h1>";
        private const string BetweenRight = "</h1>";

        private const string BetweenTextFirst = "text 1";
        private const string BetweenTextSecond = "text 2";
        private const string BetweensText = BetweenLeft + BetweenTextFirst + BetweenRight + BetweenLeft + BetweenTextSecond + BetweenRight + " hello world";

        private const string BetweenNotExisingLeft = "<nothing>";
        private const string BetweenNotExisingRight = "</nothing>";

        [TestMethod]
        public void BetweensOrEmptyTest()
        {
            var res = BetweensText.BetweensOrEmpty(BetweenLeft, BetweenRight);
            Assert.IsNotNull(res);
            Assert.IsTrue(res.Length == 2);

            Assert.AreEqual(BetweenTextFirst, res[0]);
            Assert.AreEqual(BetweenTextSecond, res[1]);

            res = BetweensText.BetweensOrEmpty(BetweenNotExisingLeft, BetweenNotExisingRight);
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

            res = BetweensText.Betweens(BetweenNotExisingLeft, BetweenNotExisingRight);
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
                BetweensText.BetweensEx(BetweenNotExisingLeft, BetweenNotExisingRight);
            });
        }

        [TestMethod]
        public void BetweenTest()
        {
            string res = BetweensText.Between(BetweenLeft, BetweenRight);
            Assert.IsFalse(string.IsNullOrEmpty(res));
            Assert.AreEqual(BetweenTextFirst, res);

            res = BetweensText.Between(BetweenNotExisingLeft, BetweenNotExisingRight);
            Assert.IsNull(res);
        }

        [TestMethod]
        public void BetweenOrEmptyTest()
        {
            string res = BetweensText.BetweenOrEmpty(BetweenLeft, BetweenRight);
            Assert.IsFalse(string.IsNullOrEmpty(res));
            Assert.AreEqual(BetweenTextFirst, res);

            res = BetweensText.BetweenOrEmpty(BetweenNotExisingLeft, BetweenNotExisingRight);
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
                BetweensText.BetweenEx(BetweenNotExisingLeft, BetweenNotExisingRight);
            });
        }

        [TestMethod]
        public void BetweenLastTest()
        {
            string res = BetweensText.BetweenLast(BetweenRight, BetweenLeft);
            Assert.IsFalse(string.IsNullOrEmpty(res));
            Assert.AreEqual(BetweenTextSecond, res);

            res = BetweensText.BetweenLast(BetweenNotExisingRight, BetweenNotExisingLeft);
            Assert.IsNull(res);
        }

        [TestMethod]
        public void BetweenLastOrEmptyTest()
        {
            string res = BetweensText.BetweenLastOrEmpty(BetweenRight, BetweenLeft);
            Assert.IsFalse(string.IsNullOrEmpty(res));
            Assert.AreEqual(BetweenTextSecond, res);

            res = BetweensText.BetweenLastOrEmpty(BetweenNotExisingRight, BetweenNotExisingLeft);
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
                BetweensText.BetweenLastEx(BetweenNotExisingRight, BetweenNotExisingLeft);
            });

        }
    }
}