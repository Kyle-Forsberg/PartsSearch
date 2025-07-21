namespace PartsSearch3;

class Program
{
    public static void Main(string[] args)
    {
        ECSscraper ecsscraper = new ECSscraper();
        var Listings = ecsscraper.SearchResultsECS("021905106C");
        foreach (var l in Listings)
        {
            Console.WriteLine(l);
        }
    }
}