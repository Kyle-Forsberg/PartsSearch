using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheapestPart
{
    public class PriceLink
    {
        double Price { get; set; }
        string Link { get; set; }

        string? Brand { get; set; }

        public PriceLink(string link, double price)
        {
            this.Link = link;
            this.Price = price;
        }
        public PriceLink(string link, double price, string brandName)
        {
            this.Link = link;
            this.Price = price;
            this.Brand = brandName;
        }
        public override string ToString()
        {   
            string str = string.Empty;
            if (Price != null)
            {
                str += this.Price + "\t";
            }
            if (Brand != null)
            {
                str += this.Brand.Trim() + "\t";
            }
            if (Link != null)
            {
                str += this.Link.Trim() + "\t";
            }
            return str;   
        }
        public void FindPrice()
        {
            //this will go to the think and pull the price from there, utilizing the theoretical webscraper class
            //this.Price = ScrapePrice(this.Link);

        }
        public double GetPrice()
        {
            return this.Price;
        }
    }



}
