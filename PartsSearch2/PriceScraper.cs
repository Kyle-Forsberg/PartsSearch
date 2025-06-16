using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.CommandLine;
using System.CommandLine.Parsing;
using System.CommandLine.Builder;

namespace PartsSearch2;
//https://www.ecstuning.com/Search/SiteSearch/02A141165M/ 
//https://www.urotuning.com/pages/search-results?q= PARTNUM
//https://www.fcpeuro.com/Parts/?keywords= + partnum;


public class PriceScraper
{       
        public HtmlWeb Web = new HtmlWeb();
        private FCPscraper fcp = new FCPscraper();
        private ECSscraper ecs = new ECSscraper();
        
        public async Task SearchPartNumber(string partNumber)
        {
                //this is the main method of this class
                //this should call all the other functions and search all sites
                var searchTask = SearchPart(partNumber);
                await Task.WhenAll(searchTask);
                var searchResult = searchTask.Result;
                PrintListings(searchResult);

        }
        public static void PrintListings(List<Listing> listings)
        {
                int i = 0;
                foreach (Listing listing in listings)
                {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write("$");
                        if (i < listings.Count / 3)
                        {
                                Console.ForegroundColor = ConsoleColor.DarkCyan;
                        }
                        else if (i < (listings.Count - (listings.Count / 3)))
                        {
                                Console.ForegroundColor = ConsoleColor.DarkYellow;
                        }
                        else
                        {
                                Console.ForegroundColor = ConsoleColor.DarkRed;
                        }

                        Console.Write(listing.Price);
                        Console.ResetColor();
                        Console.Write($": {listing.Brand.PadRight(20)} | {listing.Link}\n");
                        i++;
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