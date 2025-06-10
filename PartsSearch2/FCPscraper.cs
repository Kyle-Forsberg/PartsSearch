using System.Data.Common;
using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
namespace PartsSearch2;

public class FCPscraper
{
    public List<string>? SearchResultsFCP(string partNumber)
    {
        //makes the search in the site, and then returns links to each listing
        //hence it returning a list
        List<string> results = new List<string>();
        string link = "https://www.fcpeuro.com/Parts/?keywords=" + partNumber;   //base link for doing searches
        var web = new HtmlWeb();
        var doc = web.Load(link);
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
                results.Add(href);
            }
        }

        return results;

        //successful search, return LINKS to every product page of a match
    }
    
    
    // PLEASE FINISH THE METHOD BELOW THANKS =====================================================================
    
    public List<Listing>? FindPricesFCP(List<string> Links)
    {
        //this takes each of the links, finds the prices and other bits of it
        //organizes them into the listing class
        //and returns a list of those listings
        
        //null list check
        if(Links.Count==0){Console.WriteLine("Null link, check that error to ensure Find Price is not called with a null link");}
        
        var web = new HtmlWeb();
        List<Listing> results = new List<Listing>();
        //load page and init list;
        
        
        foreach(string link in Links)
        {
            //main looping around FCP euro
            
        }
        return results;
        
    }
}