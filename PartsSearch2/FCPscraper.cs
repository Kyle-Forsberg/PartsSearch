using System.Data.Common;
using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
namespace PartsSearch2;

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
        var resp = await client.SendAsync(req);
        resp.EnsureSuccessStatusCode();
        return await resp.Content.ReadAsStringAsync();
    }
    
    
    public async Task<List<Listing>?> FCPsearch(string partNumber)
    {
        var links = await SearchResultsFCP(partNumber);
        if (links == null || links.Count == 0)
        {
            Console.WriteLine("No results found on FCP Euro");
        }
        List<Listing>? results = new List<Listing>();
        List<Task<Listing>> tasks = new List<Task<Listing>>();

        foreach (var link in links)
        {
            tasks.Add(FindPricesFCP(link, partNumber));
        }
        Listing[] listings = await Task.WhenAll(tasks);
        foreach (var listing in listings)
        {
            if(listing == null){continue;}
            if(listing.Price == null){continue;}
            results.Add(listing);
        }
        return results;
        //this.finalResults = results;
    }
    
    public async Task<List<string>?> SearchResultsFCP(string partNumber)
    {
        //makes the search in the site, and then returns links to each listing
        //hence it returning a list
        List<string> results = new List<string>();
        string link = "https://www.fcpeuro.com/Parts/?keywords=" + partNumber;   //base link for doing searches
        string html = await GetHtml(link);
                //httpclient runs async so we need to await it
        var doc = new HtmlDocument();
        doc.LoadHtml(html);
        var nodes = doc.DocumentNode.SelectNodes("//div[contains(@class, 'grid-x') and contains(@class, 'hit')]");
        //above line is from version 1.0 directly, and has not been inspected since
        if (nodes == null)
        {
            Console.WriteLine("No results found on FCP euro :(");
            return null;
        }
        foreach (var node in nodes)
        {
            string? href = node.GetAttributeValue("data-href", string.Empty);
            if (href != string.Empty)
            {
                results.Add("https://www.fcpeuro.com" + href);
            }
        }
        return results;
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