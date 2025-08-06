namespace PartsSearch3;

using System.Net;
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
        //here is the main asynchronus method for searching.
        List<Listing> results = new List<Listing>();
        List<string> links = await this.SearchResultsURO(partNumber);
        
        
        return results;
    }
    
    
    
    //Urotuning how I loathe you
    //this must be done the old way, since I get the old 403 forbidden from them this way.
    //maybe that has been my problem all along?
    public async Task<List<string?>> SearchResultsURO(string partNumber)
    { 
        List<string> results = new List<string>();
        string link = "https://www.urotuning.com/pages/search-results?q=" + partNumber;
        string html = await client.GetStringAsync(link);
        if (html == null) { return null; }

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

    public async Task<Listing?> GetListing(string partNumber)
    {
        Listing result = null;
        
        
        return result;
    }
    
}