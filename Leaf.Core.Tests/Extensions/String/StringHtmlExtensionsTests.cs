using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Leaf.Core.Extensions.String.Tests
{
    [TestClass]
    public class StringHtmlExtensionsTests
    {
        private readonly string[] _htmlItems = {
            "1st", "2nd", "3rd"
        };

        private const string Html = @"
        <ui class=""list"">
	        <li class=""list__item"">1st</li>
	        <li class=""list__item"" id=""list__item--second"">2nd</li>
	        <li class=""list__item"">3rd</li>
        </ui>";

        private const string NotExistSelector = "something";
        private const string ListItemClassName = "list__item";

        private const string AttributeName = "id";
        private const string AttributeValue = "list__item--second";

        [TestMethod]
        public void InnerHtmlByClassTest()
        {
            Assert.AreEqual(_htmlItems[0], Html.InnerHtmlByClass(ListItemClassName));
            Assert.IsNull(Html.InnerHtmlByClass(NotExistSelector));
            Assert.AreEqual(string.Empty, Html.InnerHtmlByClass(NotExistSelector, 0, StringComparison.Ordinal, string.Empty));
        }

        [TestMethod]
        public void InnerHtmlByAttributeTest()
        {
            Assert.AreEqual(_htmlItems[1], Html.InnerHtmlByAttribute(AttributeName, AttributeValue));
            Assert.IsNull(Html.InnerHtmlByClass(NotExistSelector));
            Assert.AreEqual(string.Empty, Html.InnerHtmlByAttribute(AttributeName, NotExistSelector, 0, StringComparison.Ordinal, string.Empty));
        }

        [TestMethod]
        public void InnerHtmlByClassAllTest()
        {
            var items = Html.InnerHtmlByClassAll(ListItemClassName);
            Assert.AreEqual(_htmlItems.Length, items.Length);

            for (int i = 0; i < items.Length; i++)
                Assert.AreEqual(_htmlItems[i], items[i]);
        }
    }
}