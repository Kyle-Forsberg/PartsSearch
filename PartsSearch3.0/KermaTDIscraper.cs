using HtmlAgilityPack;

namespace PartsSearch3;

using HtmlAgilityPack;
using System.Text.RegularExpressions;
using PuppeteerSharp;
using System.IO;
using System.Threading.Tasks;

public class KermaTDIscraper
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

    public async Task<List<Listing>> SearchResults(string partnumber)
    {
        await new BrowserFetcher().DownloadAsync();

        var browser = await Puppeteer.LaunchAsync(new LaunchOptions
        {
            Headless = true
        });

        var page = await browser.NewPageAsync();
        
        await page.SetUserAgentAsync(
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) " +
            "AppleWebKit/537.36 (KHTML, like Gecko) " +
            "Chrome/115.0.0.0 Safari/537.36"
        );
   
        string url = $"https://kermatdi.com/search.html?q={partnumber}";
        await page.GoToAsync(url, new NavigationOptions
        {
            WaitUntil = new[] { WaitUntilNavigation.Networkidle2 }
        });
        await Task.Delay(3000); //shame but need to ensure all the stuff is loaded before we continue
        
        var html = await page.GetContentAsync();
        var doc = new HtmlDocument();

        doc.LoadHtml(html);
        var nodes = doc.DocumentNode.SelectNodes("//section[contains(concat(' ', normalize-space(@class), ' '), ' Sui-ProductListItem--root ')]") ?? doc.DocumentNode.SelectNodes("//section[contains(@class, 'productListItem_root__')]");
                //evil sorcery

        List<Listing> resultsList = new List<Listing>();

        if (nodes == null || nodes.Count == 0)
        {
            Console.WriteLine("No results found on Kermatdi :(");
            return new List<Listing>();
        }

        foreach (var node in nodes)
        {
            //look for any of these, they all work
            var linkNode = node.SelectSingleNode(".//a[contains(concat(' ', normalize-space(@class), ' '), ' Sui-ProductListItem--title ')]") ?? node.SelectSingleNode(".//a[contains(@class, 'productListItem_imageContainer__')]") ?? node.SelectSingleNode(".//a[@href]");

            string href = linkNode?.GetAttributeValue("href", string.Empty) ?? string.Empty;
            //ensure the href is not relative, we want the whole shebang 
            if (!string.IsNullOrEmpty(href) && !href.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            {
                
                href = "https://kermatdi.com" + href;
            }

            var brandNode = node.SelectSingleNode(".//p[contains(concat(' ', normalize-space(@class), ' '), ' Sui-ProductListItem--brand_name ')]");
            string brand = brandNode?.InnerText.Trim() ?? string.Empty;

            var priceNode = node.SelectSingleNode(".//p[contains(concat(' ', normalize-space(@class), ' '), ' Sui-ProductListItem--price ')]");
            string priceStr = priceNode?.InnerText ?? "0";
            
            // strip everything except digits and dot
            priceStr = Regex.Replace(priceStr, @"[^\d.]", "");
            double price = 0;
            double.TryParse(priceStr, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out price);

            if (!string.IsNullOrEmpty(href))
            {
                resultsList.Add(new Listing(partnumber, href, brand, price));
            }
        }

        return resultsList;
    }
    
    
}