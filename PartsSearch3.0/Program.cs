using System.Globalization;
using PuppeteerSharp.BrowserData;

namespace PartsSearch3;

class Program
{
    
    
    
    public static async Task Main(string[] args)
    {
        Console.WriteLine("Please be fr");
        string partnumber = Console.ReadLine();
        ECSscraper ecsscraper = new ECSscraper();
        var ListingsECS = await ecsscraper.SearchResultsECS(partnumber);
        Console.WriteLine("be fr 1.5");
        //still running synchronously while subsections are in development
        foreach (var l in ListingsECS)
        {
            Console.WriteLine(l);
        }

        Console.WriteLine("Be fr 2");
        FCPscraper fcpscraper = new FCPscraper();
        var ListingsFCP = await fcpscraper.SearchResultsFCP(partnumber);
        foreach (var l in ListingsFCP)
        {
            Console.WriteLine(l);
        }
        Console.WriteLine("Be fr 3");
        UROscraper uroscraper = new UROscraper();
        Console.WriteLine("pretty please write something after this line");
        var uroresults = await uroscraper.UROsearch(partnumber);
        foreach (var uro in uroresults)
        {
            Console.WriteLine(uro);
        }
        Console.WriteLine("STOP RUN");
    }
}