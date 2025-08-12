using System.Globalization;
using PuppeteerSharp.BrowserData;

namespace PartsSearch3;

class Program
{
    
    
    
    public static async Task Main(string[] args)
    {
        if (args.Length != 1)
        {
            Console.WriteLine("Usage: PartsSearch <Partnumber>");
            return;
        }
        ECSscraper ecs = new ECSscraper();
        FCPscraper fcp = new FCPscraper();
        UROscraper uro = new UROscraper();
        
        var fcptask = fcp.SearchResults(args[0]);
        var ecstask = ecs.SearchResults(args[0]);
        var urotask = uro.SearchResults(args[0]);
        
        var results = await Task.WhenAll(fcptask, ecstask, urotask);
        
        var allListings = results.SelectMany(list => list).ToList();
        allListings.Sort((a, b) => a.Price.CompareTo(b.Price));

        foreach (var listing in allListings)
        {
            Console.WriteLine(listing);
        }
    }
}