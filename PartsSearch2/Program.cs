using System;
using System.CommandLine;
using System.CommandLine.Parsing;
using System.CommandLine.Builder;
using PartsSearch2;


class Program
{
    public static void Main(string[] args)
    {
        var originalFColor = Console.ForegroundColor;
        var originalBColor = Console.BackgroundColor;
        //Console.BackgroundColor = ConsoleColor.DarkBlue;
        //Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("Hello Parts Search 2");
        // partnumber for a VR6  Coil pack is 021905106C
        string partnum = "021905106C";
        PriceScraper ps = new PriceScraper();
        var links = ps.SearchResultsECS(partnum);
        var listings = ps.FindPricesECS(links);
        foreach(Listing listing in listings)
        {
            Console.WriteLine(listing);
        }
        
        
        
        
        
        Console.BackgroundColor = originalBColor;
        Console.ForegroundColor = originalFColor;
        
        
    }
}