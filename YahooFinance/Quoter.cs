using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;

namespace YahooFinance
{
    public class Quoter
    {
        public static readonly String QUOTE_URL_BASE = @"http://download.finance.yahoo.com/d/quotes.csv";

        /// <summary>
        /// This function accepts a comma delimited string of stock symbols as input parameter
        /// and builds a valid XML return document.
        /// </summary>
        /// <param name="symbols">A bunch of stock symbols
        ///    seperated by space or comma</param>
        /// <returns>Return stock quote data in XML format</returns>
        public static string GetQuoteXml(string symbols)
        {

            // Set the return string to null.
            string result = null;
            try
            {
                string[] symbolArray = symbols.Replace(",", " ").Split(' ');
                StreamReader strm = makeQuoteCall(symbols);
                result = quoteXmlFromStream(strm, symbolArray);
                strm.Close();
            }
            catch(Exception e)
            {
                // Handle exceptions.
                throw e;
            }
            // Return the stock quote data in XML format.
            return result;
        }

		public static List<RawQuote> GetQuotes(String symbols){
			// Set the return string to null.
			List<RawQuote> result = null;
			try
			{
				string[] symbolArray = symbols.Replace(",", " ").Split(' ');
				StreamReader strm = makeQuoteCall(symbols);
				result = rawQuoteFromStream(strm, symbolArray);
				strm.Close();
			}
			catch (Exception e)
			{
				// Handle exceptions.
				throw e;
			}
			// Return the stock quote data in XML format.
			return result;			
		}

        private static StreamReader makeQuoteCall(String symbol)
        {
            // Use Yahoo finance service to download stock data from Yahoo
            string quoteUrl = QUOTE_URL_BASE + "?s=" + symbol + "&f=sl1d1t1c1hgvbap2";
            
            // Initialize a new WebRequest.
            HttpWebRequest webreq = (HttpWebRequest)WebRequest.Create(quoteUrl);
            // Get the response from the Internet resource.
            HttpWebResponse webresp = (HttpWebResponse)webreq.GetResponse();
            // Read the body of the response from the server.
            StreamReader strm =
              new StreamReader(webresp.GetResponseStream(), Encoding.ASCII);

            return strm;
        }

		private static List<RawQuote> rawQuoteFromStream(StreamReader strm, string[] symbols)
		{
			List<RawQuote> quotes = new List<RawQuote>(symbols.Length);
			String content = null;

			// Create a quote for each symbol
			for (int i = 0; i < symbols.Length; i++)
            {
                if (symbols[i].Trim() == "") continue;

                content = strm.ReadLine().Replace("\"", "");
                // E.G. content:
                //YHOO,29.11,7/19/2013,4:00pm,-0.55,29.715,29.04,20756878,29.05,29.25,-1.85%
                string[] contents = content.ToString().Split(',');

				RawQuote quote = new RawQuote();

                // If contents[2] = "N/A". the stock symbol is invalid.
                if (contents[2] != "N/A")
                {
					quote.Symbol = contents[0];
					quote.LastTradePrice = contents[1];
					quote.LastTradeDate = contents[2];
					quote.LastTradeTime = contents[3];
					quote.Change = contents[4];
					quote.DayHigh = contents[5];
					quote.DayLow = contents[6];
					quote.Volume = contents[7];
					quote.Bid = contents[8];
					quote.Ask = contents[9];
				}

				quotes.Add(quote);
			}

			return quotes;
		}

        private static string quoteXmlFromStream(StreamReader strm, string[] symbols)
        {
            StringBuilder result = new StringBuilder();
			result.Append("<StockQuotes>");

			// Loop through each line from the stream,
			// building the return XML Document string
            for (int i = 0; i < symbols.Length; i++)
            {
                // If the symbol is empty, skip it.
                if (symbols[i].Trim() == "") continue;

                string oneQuoteResultLine = strm.ReadLine().Replace("\"", "");
                // E.G.:
                //YHOO,29.11,7/19/2013,4:00pm,-0.55,29.715,29.04,20756878,29.05,29.25,-1.85%

                string[] contents = oneQuoteResultLine.ToString().Split(',');

                // If contents[2] = "N/A". the stock symbol is invalid.
                if (contents[2] == "N/A")
                {
                    // Construct XML via strings.
                    result.Append("<Stock>");
                    // "<" and ">" are illegal
                    // in XML elements. Replace the characters "<"
                    // and ">" to "&gt;" and "&lt;".
                    result.Append("<Symbol>" + symbols[i].ToUpper() + " is invalid.</Symbol>");
                    result.Append("<Last></Last>");
                    result.Append("<Date></Date>");
                    result.Append("<Time></Time>");
                    result.Append("<Change></Change>");
                    result.Append("<High></High>");
                    result.Append("<Low></Low>");
                    result.Append("<Volume></Volume>");
                    result.Append("<Bid></Bid>");
                    result.Append("<Ask></Ask>");
                    result.Append("<Ask></Ask>");
                    result.Append("</Stock>");
                }
                else
                {
                    //construct XML via strings.
                    result.Append("<Stock>");
                    result.Append("<Symbol>" + contents[0] + "</Symbol>");
                    result.Append("<Last>" + contents[1] + "</Last>");
                    result.Append("<Date>" + contents[2] + "</Date>");
                    result.Append("<Time>" + contents[3] + "</Time>");
                    // "<" and ">" are illegal in XML elements.
                    // Replace the characters "<" and ">"
                    // to "&gt;" and "&lt;".
                    if (contents[4].Trim().Substring(0, 1) == "-")
                        result.Append("<Change>&lt;span style='color:red'&gt;" +
                               contents[4] + "(" + contents[10] + ")" +
                               "&lt;span&gt;</Change>");
                    else if (contents[4].Trim().Substring(0, 1) == "+")
                        result.Append("<Change>&lt;span style='color:green'&gt;" +
                               contents[4] + "(" + contents[10] + ")" +
                               "&lt;span&gt;</Change>");
                    else
                        result.Append("<Change>" + contents[4] + "(" +
                               contents[10] + ")" + "</Change>");
                    result.Append("<High>" + contents[5] + "</High>");
                    result.Append("<Low>" + contents[6] + "</Low>");
                    result.Append("<Volume>" + contents[7] + "</Volume>");
                    result.Append("<Bid>" + contents[8] + "</Bid>");
                    result.Append("<Ask>" + contents[9] + "</Ask>");
                    result.Append("</Stock>");
                }
            }
            // Set the return string
            result.Append("</StockQuotes>");

            return result.ToString();
        }

    }
}
