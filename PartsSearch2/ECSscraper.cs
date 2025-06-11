using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
namespace PartsSearch2;

public class ECSscraper
{
    private static readonly HttpClient client = new HttpClient();

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
    
    public async Task<List<Listing>?> ECSsearch(string partNumber)
    {
        var links = await SearchResultsECS(partNumber);
        var results = await FindPricesECS(links);
        return results;
    }
    
    
    public async Task<List<string>?> SearchResultsECS(string partNumber)
    {
        //makes the search in the site, and then returns links to each listing
        //hence it returning a list
        string link = "https://www.ecstuning.com/Search/SiteSearch/" + partNumber;
        string html = await GetHtml(link);
        var doc = new HtmlDocument();
        doc.LoadHtml(html);
        var nodes = doc.DocumentNode.SelectNodes("//a[@class='listingThumbWrap']");
        if (nodes == null)
        {
            Console.WriteLine("No search results found on ECS");
            return null;    //;(
        }
        List<string> results = new List<string>();
        foreach (var node in nodes)
        {
            var href = node.GetAttributeValue("href",string.Empty);
                        
            //Console.WriteLine($"HREF VALUE FOR DEBUGGING: {"https://www.ecstuning.com" + href}");
            results.Add("https://www.ecstuning.com" + href);
        }
        return results;
        //successful search, return LINKS to every product page of a match
    }

    public async Task<List<Listing>?> FindPricesECS(List<string> Links)
    {
        //this takes each of the links, finds the prices and other bits of it
        //organizes them into the listing class
        //and returns a list of those listings
        if(Links.Count==0){Console.WriteLine("Null link, check that error to ensure Find Price is not called with a null link");}
        
        List<Listing> results = new List<Listing>();
        foreach(string link in Links)
        {
            var html = await GetHtml(link);
            //Console.WriteLine($"ITERATION FOR {link}");
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            Listing listing;
            var productNode = doc.DocumentNode.SelectSingleNode("//span[@id='price']");
            var available = doc.DocumentNode.SelectSingleNode("//div[@class='product_isnotavailable']");
            if (available != null)
            {
                //Console.WriteLine("No longer available product found on ECS");
                continue;
            }
            double price = double.Parse(productNode.InnerHtml);
            string? brand = doc.DocumentNode.SelectSingleNode("//a[@id='brandLink']")?.GetAttributeValue("title",string.Empty);

            string? partnumber = doc.DocumentNode.SelectSingleNode("//dd[@class='mfg-part-definition']//span").InnerHtml;
            //selects the partnumber hopefully fromt that mfgpartdef dd
            if(partnumber !=null && brand!=null)
            {
                //Console.WriteLine($"Retrieved partnumber {partnumber} FROM part listing wooop woop");
                listing = new Listing(partnumber,link,brand,price);
                results.Add(listing);
            }
            
            else if(partnumber!=null)
            {
                Console.WriteLine("No part number found on product page");
                listing = new Listing(link,price);
                results.Add(listing);
            }
            
            //by this point it has added that iteration to the list and moved on
            
        }
        return results;
        
    }
}