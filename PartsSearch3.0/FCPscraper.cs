using System.Data.Common;
using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
namespace PartsSearch3;

public class FCPscraper
{
    private static readonly HttpClient client = new HttpClient();
    
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
    
    
    public async Task<List<Listing>> SearchResults(string partNumber)
    {
        //makes the search in the site, and then returns links to each listing
        //hence it returning a list

        string link = "https://www.fcpeuro.com/Parts/?keywords=" + partNumber;   //base link for doing searches
        string html = await GetHtml(link);
        if (html.Length == 0)   //check to make sure we found a usable link before we move on
        {
            return new List<Listing>();
        }
                //httpclient runs async so we need to await it
        var doc = new HtmlDocument();
        doc.LoadHtml(html);
        var nodes = doc.DocumentNode.SelectNodes("//div[contains(@class, 'grid-x') and contains(@class, 'hit')]");
        //above line is from version 1.0 directly, and has not been inspected since]
        List<Listing>? resultsList = new List<Listing>();
        if (nodes.Count == 0)
        {
            Console.WriteLine("No results found on FCP euro :(");
            return new List<Listing>();
        }
        foreach (var node in nodes)
        {
            string? href = node.GetAttributeValue("data-href", string.Empty);
            string? pricestr =  node.GetAttributeValue("data-price", string.Empty);
            string brand = node.GetAttributeValue("data-brand", string.Empty);
            
            // all this info is avalible right there, might aswell take it from here.
            if (href != string.Empty)
            {
                resultsList.Add(new Listing(partNumber, "https://www.fcpeuro.com" + href, brand, double.Parse(pricestr)));
            }
        }

        return resultsList;
        //successful search, return LINKS to every product page of a match
    }
   
}