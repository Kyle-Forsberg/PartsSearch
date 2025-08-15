using System.Globalization;
using PuppeteerSharp.BrowserData;

namespace PartsSearch3;

using Spectre.Console;
using System.Globalization;

class Program
{
    
    
    
    public static async Task Main2(string[] args)
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
    
    
    public static async Task Main(string[] args)
    {
        if (args.Length != 1)
        {
            AnsiConsole.MarkupLine("[red]Usage:[/] PartsSearch <Partnumber>");
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

        // Create a table
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
    }
    
    
}