namespace PartsSearch3;

class Program
{
    
    
    
    public static async Task Main(string[] args)
    {
        string partnumber = Console.ReadLine();
        ECSscraper ecsscraper = new ECSscraper();
        var ListingsECS = await ecsscraper.SearchResultsECS(partnumber);
        //still running synchronously while subsections are in development
        foreach (var l in ListingsECS)
        {
            Console.WriteLine(l);
        }
        
        FCPscraper fcpscraper = new FCPscraper();
        var ListingsFCP = await fcpscraper.SearchResultsFCP(partnumber);
        foreach (var l in ListingsFCP)
        {
            Console.WriteLine(l);
        }
    }
}