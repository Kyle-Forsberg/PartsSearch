using System.Net.WebSockets;
using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace PartsSearch3;

public class ECSscraper
{
    private static readonly HttpClient client = new HttpClient();

    public string? GetHtml(string url)
    {
        
        var req = new HttpRequestMessage(HttpMethod.Get, url);
        req.Headers.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) " +
                                       "AppleWebKit/537.36 (KHTML, like Gecko) " +
                                       "Chrome/114.0.0.0 Safari/537.36");
        try
        {
            var resp = client.Send(req);
            resp.EnsureSuccessStatusCode();
            return resp?.Content?.ReadAsStringAsync().Result;
        }
        catch (Exception e)
        {
            Console.WriteLine("Nothing found on FCP euro");
        }

        return null;
    }
    
    public List<Listing> SearchResultsECS(string partNumber)
    {
        //makes the search in the site, and then returns links to each listing
        //hence it returning a list
        string link = "https://www.ecstuning.com/Search/SiteSearch/" + partNumber;
        string html = GetHtml(link);
        //Console.WriteLine(html);
        var doc = new HtmlDocument();
        doc.LoadHtml(html);
        var nodes = doc.DocumentNode.SelectNodes("//div[contains(@class,'productListBox')]");
        if (nodes == null)
        {
            Console.WriteLine("No search results found on ECS");
            return null;    //;(
        }

        List<Listing> resultsList = new List<Listing>();
        
        foreach (var node in nodes)
        {
            var priceNode = node.SelectSingleNode(".//span[@class='price productListPrice']/text()[normalize-space()]");
            if (priceNode == null)
            {
                //Console.WriteLine("Null price node");
                continue;
            }
  
            double price = double.Parse(priceNode.InnerText.Trim().Substring(1));
            var href = node.GetAttributeValue("href",string.Empty);
            Listing l = new Listing(partNumber, "https://www.ecstuning.com" + href, "UNKNOWN BRAND", price);
            
            
            resultsList.Add(l);
            //so currently, this function can return both the hrefs for the listings and their price,
            //without having to go over to the products main page, that's what we are looking for. 
            
        }

        return resultsList;
        //successful search, return LINKS to every product page of a match
    }
    
}