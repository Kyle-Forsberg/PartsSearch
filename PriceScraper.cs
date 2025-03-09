using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Threading;
using HtmlAgilityPack;
using MySql.Data;
using MySql.Data.MySqlClient;
using static System.Net.WebRequestMethods;


// https://www.urotuning.com/pages/search-results?q=02A141165M
// https://www.urotuning.com/pages/search-results?q= PARTNUM
// https://www.ecstuning.com/Search/SiteSearch/02A141165M/ 
namespace CheapestPart
{
    public class PriceScraper
    {
        public HtmlWeb Web = new HtmlWeb();

        public PriceLink ScrapePrice(string link)
        {
            if (link == null) { Console.WriteLine("NULL LINK, FAIL TO FIND PART"); return null; }
            Console.WriteLine("LOADING LINK : " + link);
            var document = Web.Load(link);
            var productNode = document.DocumentNode.SelectSingleNode("//span[@id='price']");
            double price = double.Parse(productNode.InnerHtml);

            PriceLink priceLink = new PriceLink(link, price);
            Console.WriteLine("\nPRICE OF THE PART:\t" + price + "\n");
            return priceLink;
        }
        public List<string> SearchPart(string partnum)
        {
            string link = "https://www.ecstuning.com/Search/SiteSearch/" + partnum;
            Console.WriteLine("LINK + " + link);
            var document = Web.Load(link);                                          
            var anchorNodes = document.DocumentNode.QuerySelectorAll("a.listingThumbWrap");

            if (anchorNodes == null) {
                Console.WriteLine("Failed to get link from search, NODE WAS NULL"); 
                return null; 
            }
            List<string> results = new List<string>();

            foreach (var anchorNode in anchorNodes)
            {
                var href = anchorNode.GetAttributeValue("href", string.Empty);
                results.Add("https://www.ecstuning.com" + href);
            }

            return results;


        }


    }
}


///a[contains(@class, 'findify-components--cards--product') and contains(@class, 'findify-search-card')]