using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace PartsSearch2;
//https://www.ecstuning.com/Search/SiteSearch/02A141165M/ 
//https://www.urotuning.com/pages/search-results?q= PARTNUM
//https://www.fcpeuro.com/Parts/?keywords= + partnum;


public class PriceScraper
{       
        public HtmlWeb Web = new HtmlWeb();
        private string ECSlink = "https://www.ecstuning.com/Search/SiteSearch/";
        private string UROlink = "https://www.urotuning.com/pages/search-results?q=";
        private string FCPlink = "https://www.fcpeuro.com/Parts/?keywords=";
        public ECSscraper ecs = new ECSscraper();
        
        private void SearchPartNumber(string partNumber)
        {
                //this is the main method of this class
                //this should call all the other functions and search all sites
                
                

        }
 
        public List<string>? SearchResultsECS(string partNumber)        //currenty string. change to Listing class later
        {
                return null;
        }
        //need to ensure that each of these can be null, since some will be inevitably
        public List<Listing>? FindPricesECS(List<string> Links)
        {
                return null;
        }
}