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
