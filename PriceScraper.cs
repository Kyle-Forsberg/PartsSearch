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


        public void SearchPartNumber(string partnum)
        {
            Console.WriteLine("Searching for part number " + partnum + "\n");
            PriceScraper scraper = new PriceScraper();
            var ECSlinks = scraper.SearchPartECS(partnum);
            var FCPlinks = scraper.SearchPartFCPEuro(partnum);
            var priceList = new List<PriceLink>();

            foreach (var link in ECSlinks)
            {
                priceList.Add(scraper.ScrapePriceECS(link));
            }
            foreach(var link in FCPlinks)
            {
                priceList.Add(scraper.ScrapePriceFCPEuro(link));
            }  
            priceList = priceList.OrderBy(price => price.GetPrice()).ToList();
            foreach (var price in priceList)
            {
                Console.WriteLine(price);
            }

        }

        public PriceLink ScrapePriceECS(string link)
        {
            if (link == null) { Console.WriteLine("NULL LINK, FAIL TO FIND PART"); return null; }
            //Console.WriteLine("LOADING LINK : " + link);
            PriceLink priceLink;
            var document = Web.Load(link);
            var productNode = document.DocumentNode.SelectSingleNode("//span[@id='price']");
            double price = double.Parse(productNode.InnerHtml);
            var productBrandNode = document.DocumentNode.QuerySelector("a#brandLink");              //grabs the href to the brand from the products main page
            string brandName;
            if (productBrandNode != null)
            {                                                                                        //get the title from it, sicne that will hold our brands name, the actualy html element looks like this: <a href="/b-genuine-volkswagen-audi-parts/" title="Genuine Volkswagen Audi" id="brandLink">
                brandName = productBrandNode.GetAttributeValue("title", string.Empty);
            }
            else
            {
                brandName = "Unknown Brand";
            }
            while(brandName.Length < 16)
            {
                brandName += " ";
            }
            priceLink = new PriceLink(link, price, brandName);
            //Console.WriteLine("\nPRICE OF THE PART:\t" + price + "\n");
            return priceLink;
        }
        public List<string> SearchPartECS(string partnum)
        {
            string link = "https://www.ecstuning.com/Search/SiteSearch/" + partnum;
            //Console.WriteLine("LINK + " + link);
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
        public PriceLink ScrapePriceFCPEuro(string link)
        {
            if (link == null) { Console.WriteLine("NULL LINK, FAIL TO FIND PART"); return null; }
            //Console.WriteLine("LOADING LINK : " + link);
            PriceLink priceLink;
            var document = Web.Load(link);

            var divNode = document.DocumentNode.SelectSingleNode("//div[contains(@class, 'listing__amount')]");
            var spanNode = divNode?.SelectSingleNode(".//span");
            if (spanNode == null)
            {
                Console.WriteLine("NULL SPAN NODE");
                return null;
            }
            double price = double.Parse(spanNode.InnerHtml.Substring(1));
            var brandDivNode = document.DocumentNode.SelectSingleNode("//div[contains(@class, 'listing__brand')]");
            var brandSpanNode = brandDivNode?.SelectSingleNode(".//span");

            string brandName;
            if (brandSpanNode == null)
            {
                brandName = "Unknown Brand";
            }
            else
            {
                brandName = brandSpanNode.InnerHtml;
                brandName = brandName.Substring(1);
            }

            while (brandName.Length < 16)
            {
                brandName += " ";
            }
            priceLink = new PriceLink(link, price, brandName);
            //Console.WriteLine("\nPRICE OF THE PART:\t" + price + "\n");
            return priceLink;
        }



        public List<string> SearchPartFCPEuro(string partnum)
        {
            string link = "https://www.fcpeuro.com/Parts/?keywords=" + partnum;
            var document = Web.Load(link);
            var anchorNodes = document.DocumentNode.SelectNodes("//div[contains(@class, 'grid-x') and contains(@class, 'hit')]");
            if (anchorNodes == null)
            {
                Console.WriteLine("FCP PART NULL");
                return null;
            }
            List <string> results = new List<string>();
            foreach(var anchorNode in anchorNodes)
            {
                var href = anchorNode.GetAttributeValue("data-href",string.Empty);
                if (href != string.Empty)
                {
                    results.Add("https://www.fcpeuro.com" + href);
                }
                //Console.WriteLine(href);
            }
            return results;

        }

        public List<string> SearchPartUroTuning(string partnum)
        {
            string link = "https://www.urotuning.com/pages/search-results?q=" + partnum;
            var document = Web.Load(link);
            //var anchorNodes = document.DocumentNode.SelectNodes("//a[contains(@class, 'findify-components--cards--product') and contains(@class, 'findify-search-card')]");
            var anchorNodes = document.DocumentNode.QuerySelectorAll("a.findify-search-card");
            //it does not seem to be null, but we still cannot get any of the data from it, what a shame. 
            if (anchorNodes == null)
            {
                Console.WriteLine("Uro PART NULL");
                //return null;
            }
            List<string> results = new List<string>();
            int i = 1;
            foreach (var anchorNode in anchorNodes)
            {
                Console.WriteLine("Iteration : " + i);
                var href = anchorNode.GetAttributeValue("href", string.Empty);
                if (href != string.Empty)
                {
                    results.Add("https://www.fcpeuro.com" + href);
                }
                Console.WriteLine("LINK : "+ href);
                i++;
            }
            Console.WriteLine("here is the TO String for the anchornodes:::");
            Console.WriteLine(anchorNodes.ToString());
            return results;

        }


    }
}


///a[contains(@class, 'findify-components--cards--product') and contains(@class, 'findify-search-card')]