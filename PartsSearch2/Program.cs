using System;
using System.CommandLine;
using System.CommandLine.Parsing;
using System.CommandLine.Builder;
using PartsSearch2;


class Program
{
    public static async Task Main(string[] args)
    {
        var program = new Program();
        await program.MainAsync();
    }
    public async Task MainAsync()
    {
        Console.WriteLine("Hello Parts Search 2");
        string partnum = "021905106C";
        
        var searchTask = SearchPart(partnum);
        await Task.WhenAll(searchTask);
        var searchResult = searchTask.Result;
        PrintListings(searchResult);


    }

    public static void PrintListings(List<Listing> listings)
    {
        foreach (Listing listing in listings)
        {
            Console.WriteLine(listing);
        }
    }

    public static async Task<List<Listing>?> SearchPart(string partnum)
    {
        var fcp = new FCPscraper();
        var ecs = new ECSscraper();

        
        
        var fcpTask = fcp.FCPsearch(partnum);
        var ecsTask = ecs.ECSsearch(partnum);
        await Task.WhenAll(fcpTask,ecsTask);
        var fcpResults = await fcpTask;
        var ecsResults = await ecsTask;

        List<Listing> results = new List<Listing>();
        foreach (Listing listing in fcpResults)
        {
            if (listing == null)
            {
                continue;
            }

            results.Add(listing);
        }

        foreach (Listing listing in ecsResults)
        {
            if (listing == null)
            {
                continue;
            }
            results.Add(listing);
        }
        
        
        results.Sort((a , b) => a.Price.CompareTo(b.Price));
        //above line sorts by price with this sick ass lambda 
        return results;
        
    }
    
    
}