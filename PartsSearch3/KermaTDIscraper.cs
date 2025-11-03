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
    
    

    public async Task<List<Listing>> SearchResults(string partnumber)
    {
        var page = await BrowserManager.Instance.GetNewPageAsync();
        
        try
        {
            string url = $"https://kermatdi.com/search.html?q={partnumber}";
            await page.GoToAsync(url, new NavigationOptions
            {
                WaitUntil = new[] { WaitUntilNavigation.Networkidle2 },
                Timeout = 30000
            });
            await Task.Delay(3000);
        
            var html = await page.GetContentAsync();
            var doc = new HtmlDocument();

            doc.LoadHtml(html);
            var nodes = doc.DocumentNode.SelectNodes("//section[contains(concat(' ', normalize-space(@class), ' '), ' Sui-ProductListItem--root ')]") ?? doc.DocumentNode.SelectNodes("//section[contains(@class, 'productListItem_root__')]");

            List<Listing> resultsList = new List<Listing>();

            if (nodes == null || nodes.Count == 0)
            {
                await page.CloseAsync();
                return new List<Listing>();
            }

            foreach (var node in nodes)
            {
                var linkNode = node.SelectSingleNode(".//a[contains(concat(' ', normalize-space(@class), ' '), ' Sui-ProductListItem--title ')]") ?? node.SelectSingleNode(".//a[contains(@class, 'productListItem_imageContainer__')]") ?? node.SelectSingleNode(".//a[@href]");

                string href = linkNode?.GetAttributeValue("href", string.Empty) ?? string.Empty;
                if (!string.IsNullOrEmpty(href) && !href.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                {
                    href = "https://kermatdi.com" + href;
                }

                var brandNode = node.SelectSingleNode(".//p[contains(concat(' ', normalize-space(@class), ' '), ' Sui-ProductListItem--brand_name ')]");
                string brand = brandNode?.InnerText.Trim() ?? string.Empty;

                var priceNode = node.SelectSingleNode(".//p[contains(concat(' ', normalize-space(@class), ' '), ' Sui-ProductListItem--price ')]");
                string priceStr = priceNode?.InnerText ?? "0";
            
                priceStr = Regex.Replace(priceStr, @"[^\d.]", "");
                double price = 0;
                double.TryParse(priceStr, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out price);

                if (!string.IsNullOrEmpty(href))
                {
                    resultsList.Add(new Listing(partnumber, href, brand, price));
                }
            }

            await page.CloseAsync();
            return resultsList;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error scraping KermaTDI: {e.Message}");
            await page.CloseAsync();
            return new List<Listing>();
        }
    }
    
}