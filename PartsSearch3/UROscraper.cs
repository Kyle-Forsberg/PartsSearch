using System.Threading.Channels;

namespace PartsSearch3;

using System.Net;
using System.Threading.Tasks;
using PuppeteerSharp;
using System;
using System.Collections.Generic;
using HtmlAgilityPack;

//main price is stored in
// <span class="findify-main-price">[what we need]</span.

public class UROscraper
{
    //dreaded Uro Tuning scraper

    //Urotuning how I loathe you

    public async Task<List<Listing>> SearchResults(string partnumber)
    {
        var page = await BrowserManager.Instance.GetNewPageAsync();
        
        try
        {
            var url = $"https://www.urotuning.com/pages/search-results?q={Uri.EscapeDataString(partnumber)}";
            await page.GoToAsync(url, new NavigationOptions
            {
                WaitUntil = new[] { WaitUntilNavigation.Networkidle2 },
                Timeout = 30000
            });
            await page.WaitForSelectorAsync(".findify-main-price", new WaitForSelectorOptions { Timeout = 10000 });
        
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
        
        var listings = System.Text.Json.JsonSerializer.Deserialize<List<Listing>?>(rawJson) ?? new List<Listing>();
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
                }
            }

            if (listing.Link != null)
            {
                listing.Link = "https://www.urotuning.com" +  listing.Link;
            }
        }
        
        await page.CloseAsync();
        return listings;
    }
    catch (Exception e)
    {
        Console.WriteLine($"Error scraping UROtuning: {e.Message}");
        await page.CloseAsync();
        return new List<Listing>();
    }
    }
}
