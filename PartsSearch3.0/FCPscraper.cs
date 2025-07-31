using System.Data.Common;
using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
namespace PartsSearch3;

public class FCPscraper
{
    private static readonly HttpClient client = new HttpClient();
    private List<Listing>? finalResults;
    
    public async Task<string> GetHtml(string url)
    {
        var req = new HttpRequestMessage(HttpMethod.Get, url);
        req.Headers.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) " +
                                       "AppleWebKit/537.36 (KHTML, like Gecko) " +
                                       "Chrome/114.0.0.0 Safari/537.36");
        try
        {
            var resp = await client.SendAsync(req);
            resp.EnsureSuccessStatusCode();
            return await resp.Content.ReadAsStringAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine("Nothing found on FCP euro");
        }
        return null;
    }
    
    
    public async Task<List<Listing>?> FCPsearch(string partNumber)
    {
        var listings = SearchResultsFCP(partNumber);
        return await listings;
        //this is now just a wrapper of the other function but its nice to keep things orderly
    }
    
    public async Task<List<Listing>?> SearchResultsFCP(string partNumber)
    {
        //makes the search in the site, and then returns links to each listing
        //hence it returning a list
        List<string> results = new List<string>();
        string link = "https://www.fcpeuro.com/Parts/?keywords=" + partNumber;   //base link for doing searches
        string html = await GetHtml(link);
        if (html == null)   //check to make sure we found a usable link before we move on
        {
            return null;
        }
                //httpclient runs async so we need to await it
        var doc = new HtmlDocument();
        doc.LoadHtml(html);
        var nodes = doc.DocumentNode.SelectNodes("//div[contains(@class, 'grid-x') and contains(@class, 'hit')]");
        //above line is from version 1.0 directly, and has not been inspected since]
        List<Listing>? resultsList = new List<Listing>();
        if (nodes == null)
        {
            Console.WriteLine("No results found on FCP euro :(");
            return null;
        }
        foreach (var node in nodes)
        {
            string? href = node.GetAttributeValue("data-href", string.Empty);
            string? pricestr =  node.GetAttributeValue("data-price", string.Empty);
            string brand = node.GetAttributeValue("data-brand", string.Empty);
            
            // all this info is avalible right there, might aswell take it from here.
            if (href != string.Empty)
            {
                results.Add("https://www.fcpeuro.com" + href);
                resultsList.Add(new Listing(partNumber, link, brand, double.Parse(pricestr)));

            }
        }

        return resultsList;
        //successful search, return LINKS to every product page of a match
    }
    
    public async Task<Listing?> FindPricesFCP(string link, string partnumber)
    {
        //this takes each of the links, finds the prices and other bits of it
        //organizes them into the listing class
        //and returns a list of those listings
        
        //null list check
        if (link.Length == 0) { Console.WriteLine("Null link, check that error to ensure Find Price is not called with a null link"); }

        var web = new HtmlWeb();
        Listing result;
        //load page and init list;
        
        string html = await GetHtml(link);
        var doc = new HtmlDocument();
        doc.LoadHtml(html);
        var div = doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'listing__amount')]");
        var span = div?.SelectSingleNode(".//span");
        if (span != null)
        {
            double price = double.Parse(span.InnerHtml.Substring(1));   //think this removes the currency, from old ver
            var brandDiv = doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'listing__brand')]");
            var brandSpan = brandDiv?.SelectSingleNode(".//span");

            string brandname = string.Empty;
            if (brandSpan == null)
            {
                brandname = "Unknown Brand";
            }
            else
            {
                brandname = brandSpan.InnerHtml.Substring(1).Trim('\n', '\t'); 
            }
            
            return new Listing(partnumber, link, brandname, price);
        }

        return null;

    }
}