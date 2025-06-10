using System;
using System.CommandLine;
using System.CommandLine.Parsing;
using System.CommandLine.Builder;
using PartsSearch2;


class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Hello Parts Search 2");
        string partnum = "021905106C";
        PrintListings(SearchPart(partnum));


    }

    public static void PrintListings(List<Listing> listings)
    {
        foreach (Listing listing in listings)
        {
            Console.WriteLine(listing);
        }
    }

    public static List<Listing> SearchPart(string partnum)
    {
        var fcp = new FCPscraper();
        var ecs = new ECSscraper();
        var results = fcp.FCPsearch(partnum);
        results.AddRange(ecs.ECSsearch(partnum));
        results.Sort((a , b) => a.Price.CompareTo(b.Price));
        //above line sorts by price with this sick ass lambda 
        return results;
        
    }
    
    
}