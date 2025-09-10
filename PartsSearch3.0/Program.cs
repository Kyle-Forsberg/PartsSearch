using System.Globalization;
using PuppeteerSharp.BrowserData;
using System.Diagnostics;

namespace PartsSearch3;

using Spectre.Console;
using System.Globalization;

class Program
{
    
    public static async Task Main1(string[] args)
    {
        if (args.Length < 1)
        {
            Console.WriteLine("Usage: PartsSearch <Partnumber>");
            return;

        }

        if (args.Length > 1)
        {
            for (int i = 1; i < args.Length; i++)
            {
                args[0] += args[i];
                //combine all args into the first one
                //should just make it its own variable but whatever this works for now
            }
        }
        ECSscraper ecs = new ECSscraper();
        FCPscraper fcp = new FCPscraper();
        UROscraper uro = new UROscraper();
        KermaTDIscraper kerma = new KermaTDIscraper();
   
        
        var fcptask = fcp.SearchResults(args[0]);
        var ecstask = ecs.SearchResults(args[0]);
        var urotask = uro.SearchResults(args[0]);
        var kermaTask = kerma.SearchResults(args[0]);
        
        var results = await Task.WhenAll(fcptask, ecstask, urotask, kermaTask);
        var allListings = results.SelectMany(list => list).ToList();

       allListings.Sort((a, b) => a.Price.CompareTo(b.Price));

        foreach (var listing in allListings)
        {
            Console.WriteLine(listing);
        }
    }
    
    
    public static async Task Main(string[] args)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        if (args.Length < 1)
        {
            Console.WriteLine("Usage: PartsSearch <Partnumber>");
            return;
        }

        if (args.Length > 1)
        {
            for (int i = 1; i < args.Length; i++)
            {
                args[0] += args[i];
                //combine all args into the first one
                //should just make it its own variable but whatever this works for now
            }
        }

        ECSscraper ecs = new ECSscraper();
        FCPscraper fcp = new FCPscraper();
        UROscraper uro = new UROscraper();
        KermaTDIscraper kerma = new KermaTDIscraper();
        
        var fcptask = fcp.SearchResults(args[0]);
        var ecstask = ecs.SearchResults(args[0]);
        var urotask = uro.SearchResults(args[0]);
        var kermaTask = kerma.SearchResults(args[0]);
        
        var results = await Task.WhenAll(fcptask, ecstask, urotask,  kermaTask);
        
        var allListings = results.SelectMany(list => list).ToList();
        allListings.Sort((a, b) => a.Price.CompareTo(b.Price));
        
        var table = new Table();
        table.Border(TableBorder.Rounded);
        table.AddColumn("[bold yellow]Price[/]");
        table.AddColumn("[bold cyan]Brand[/]");
        table.AddColumn("[bold green]Link[/]");

        foreach (var listing in allListings)
        {
            table.AddRow(
                $"${listing.Price:F2}",
                listing.Brand ?? "[grey]N/A[/]",
                listing.Link ?? ""
            );
        }

        AnsiConsole.Write(table);
        stopwatch.Stop();
        TimeSpan ts = stopwatch.Elapsed;
        Console.WriteLine($"Total runtime: {ts.TotalMilliseconds/1000} seconds");

    }
    
    
}