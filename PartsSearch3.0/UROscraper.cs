using System.Threading.Channels;

namespace PartsSearch3;

using System.Net;
using System.Threading.Tasks;
using PuppeteerSharp;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
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
    
    
    //Urotuning how I loathe you

    public async Task<List<Listing>> SearchResults(string partnumber)
    {


        await new BrowserFetcher().DownloadAsync();     //i guess this has to download chromium yipee
        
        var browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = true });

        var page = await browser.NewPageAsync();
        
        var url = $"https://www.urotuning.com/pages/search-results?q={Uri.EscapeDataString(partnumber)}";
        await page.GoToAsync(url);
        
        await page.WaitForSelectorAsync(".findify-main-price");


        var rawJson = await page.EvaluateFunctionAsync<string>(
            "() => JSON.stringify((() => { " +
            "const results = [];" +
            "const priceEls = document.querySelectorAll('.findify-main-price');" +
            "const titleEls = document.querySelectorAll('.findify-components--cards--product--title');" +
            "const linkEls = document.querySelectorAll('.findify-components--cards--product.findify-search-card');" +
            "const brandEls = document.querySelectorAll('.findify-components--product-brand-container img');" +
            "for (let i = 0; i < priceEls.length; i++) {" +
            "  const priceText = priceEls[i]?.innerText.trim().replace(/[^\\d\\.]/g, '') || '';" +
            "  const price = parseFloat(priceText) || 0;" +
            "  const title = titleEls[i]?.innerText.trim() || '';" +
            "  const link = linkEls[i]?.getAttribute('href') || '';" +
            "  const brand = brandEls[i]?.getAttribute('alt')?.trim() || '';" +
            "  results.push({ Partnumber: title, Link: link, Brand: brand, Price: price });" +
            "}" +
            "return results;" +
            "})())"
        );
        //Console.WriteLine(rawJson);
        var listings = System.Text.Json.JsonSerializer.Deserialize<List<Listing>?>(rawJson);
        foreach(var listing in listings)
        {
            if (listing.Brand != null)
            {
                if (listing.Brand.Length <= 3)
                {
                    listing.Brand = listing.Brand.ToUpper();
                }
                else
                {
                    listing.Brand = listing.Brand.Substring(0, 1).ToUpper() + listing.Brand.Substring(1);
                    //capitalize first letter, surely there is a better way to do this!
                }
            }

            if (listing.Link != null)
            {
                listing.Link = "https://www.urotuning.com" +  listing.Link;
            }
        }
        await browser.CloseAsync();
        return listings;
    }
    
}
