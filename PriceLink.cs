using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheapestPart
{
    public class PriceLink
    {
        double? Price { get; set; }
        string Link { get; set; }

        public PriceLink(string link, double price)
        {
            this.Link = link;
            this.Price = price;
        }
        public override string ToString()
        {
            return this.Link + "  " + this.Price;   
        }
        public void FindPrice()
        {
            //this will go to the think and pull the price from there, utilizing the theoretical webscraper class
            //this.Price = ScrapePrice(this.Link);

        }
    }



}
