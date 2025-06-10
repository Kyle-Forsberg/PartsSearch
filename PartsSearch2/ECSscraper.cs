using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
namespace PartsSearch2;

public class ECSscraper
{
    public List<Listing>? ECSsearch(string partNumber)
    {
        var links = SearchResultsECS(partNumber);
        return FindPricesECS(links);
    }
    
    
    public List<string>? SearchResultsECS(string partNumber)
    {
        //makes the search in the site, and then returns links to each listing
        //hence it returning a list
        string link = "https://www.ecstuning.com/Search/SiteSearch/" + partNumber;
        HtmlWeb web = new HtmlWeb();
        var doc = web.Load(link);
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

    public List<Listing>? FindPricesECS(List<string> Links)
    {
        //this takes each of the links, finds the prices and other bits of it
        //organizes them into the listing class
        //and returns a list of those listings
        if(Links.Count==0){Console.WriteLine("Null link, check that error to ensure Find Price is not called with a null link");}
        var web = new HtmlWeb();
        List<Listing> results = new List<Listing>();
        foreach(string link in Links)
        {
            //Console.WriteLine($"ITERATION FOR {link}");
            Listing listing;
            var doc = web.Load(link);
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