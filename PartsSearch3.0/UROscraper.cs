using System.Threading.Channels;

namespace PartsSearch3;

using System.Net;
using System.Threading.Tasks;
using PuppeteerSharp;
//this needed to simulate a real browser to
//access all those pesky javascript elements that have 
//made this so hard in the past
using HtmlAgilityPack;

//main price is stored in
// <span class="findify-main-price">[what we need]</span.

public class UROscraper
{
    //dreaded Uro Tuning scraper
    //seeing as how UroTunings Website is such a hellscape, I dont forsee this going well
    
    
    public static HttpClientHandler handler = new HttpClientHandler()
    {
        UseCookies = true,
        AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
    };
    private static readonly HttpClient client = new HttpClient(handler);
    
    public async Task<string> GetHtml(string url)
    {
        var req = new HttpRequestMessage(HttpMethod.Get, url);
        req.Headers.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) " +
                                       "AppleWebKit/537.36 (KHTML, like Gecko) " +
                                       "Chrome/114.0.0.0 Safari/537.36");
        req.Headers.Accept.ParseAdd("text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
        req.Headers.AcceptLanguage.ParseAdd("en-US,en;q=0.5");
        req.Headers.Add("Connection", "keep-alive");
        req.Headers.Add("Upgrade-Insecure-Requests", "1");
        try
        {
            var resp = await client.SendAsync(req);
            resp.EnsureSuccessStatusCode();
            return await resp?.Content.ReadAsStringAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine("Nothing found on FCP euro");
        }
        return null;
    }

    public async Task<List<Listing>?> UROsearch(string partNumber)
    {
        //yeah little redundant ill get things in order later
        List<Listing> results = await this.GetData(partNumber);
        return results;
        
    }
    
    //Urotuning how I loathe you
    //this must be done the old way, since I get the old 403 forbidden from them this way.
    //maybe that has been my problem all along?
    public async Task<List<string?>> SearchResultsURO(string partNumber)
    { 
        List<string> results = new List<string>();
        string link = "https://www.urotuning.com/pages/search-results?q=" + partNumber;
        string html = await GetHtml(link);
        if (html == null) {
            Console.WriteLine("Link was not found");
            return null; 
        }

        //Console.WriteLine(html);    //lets just see what is actually reported
        var doc = new HtmlDocument(); 
        doc.LoadHtml(html);
        
        var nodes = doc.DocumentNode.SelectNodes("//span[contains(@class, 'findify-main-price')]");
        Console.WriteLine("Found " + nodes.Count + " results");
        foreach (var node in nodes)
        {
            Console.WriteLine(node.InnerText);
        }
        return null;
    }
    
    public async Task<List<Listing>?> GetData(string partnumber)
    {
        Console.WriteLine("start of get data");

        await new BrowserFetcher().DownloadAsync();     //i guess this has to download chromium yipee
        
        var browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = true });
        Console.WriteLine("checkpoint 0");
        var page = await browser.NewPageAsync();
        Console.WriteLine("checkpoint 1");

        var url = $"https://www.urotuning.com/pages/search-results?q={Uri.EscapeDataString(partnumber)}";
        await page.GoToAsync(url);
        Console.WriteLine("checkpoint 2");

        await page.WaitForSelectorAsync(".findify-main-price");
        Console.WriteLine("checkpoint 3");

        var products = await page.EvaluateFunctionAsync<Listing[]>(@"() => {
            const results = [];
            const priceEls = document.querySelectorAll('.findify-main-price');
            const titleEls = document.querySelectorAll('.findify-components--cards--product--title');
            const linkEls = document.querySelectorAll('.findify-components--cards--product--image a');
            const brandEls = document.querySelectorAll('.findify-components--cards--product--brand');
            
            for (let i = 0; i < priceEls.length; i++) {
                const priceText = priceEls[i].innerText.trim().replace(/[^\d\.]/g, '');
                const price = parseFloat(priceText) || 0;

                const title = titleEls[i] ? titleEls[i].innerText.trim() : "";
                const link = linkEls[i] ? linkEls[i].href : "";
                const brand = brandEls[i] ? brandEls[i].innerText.trim() : "";

                results.push({
                    Partnumber: title,
                    Link: link,
                    Brand: brand,
                    Price: price
                });
            }
            return results;
        }");



        await browser.CloseAsync();
        Console.WriteLine("end of get data");
        return products != null ? new List<Listing>(products) : null;
    }
    
}
