using System;
using System.CommandLine;
using System.CommandLine.Parsing;
using System.CommandLine.Builder;
using PartsSearch2;
using Timer = System.Timers.Timer;

class Program
{
    public static async Task Main(string[] args)
    {
        var program = new Program();
        Console.WriteLine("Searching far and wide for the best deal");
        await program.MainAsync(args[0]);
    }

    public async Task MainAsync(string partnum)
    {
        var ps = new PriceScraper();
        await ps.SearchPartNumber(partnum);
    }
}