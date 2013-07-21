using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YahooFinance;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;

namespace UnitTestProject1
{
    [TestClass]
    public class YahooFinanceTests
    {
        [TestMethod]
		public void GetQuoteXmlFor_YHOO()
        {
			String result = Quoter.GetQuoteXml("YHOO");

            Assert.IsNotNull(result);

			// Load the string into an XML document.
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(result);
			XmlNode newNode = doc.DocumentElement;

			Assert.AreEqual(1, newNode.ChildNodes.Count);
        }

		[TestMethod]
		public void GetQuotesFor_YHOO()
		{
			List<RawQuote> result = Quoter.GetQuotes("YHOO");

			Assert.IsNotNull(result);
			Assert.AreEqual(1, result.Count);
		}
    }
}
