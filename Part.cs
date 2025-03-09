using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheapestPart
{
    public class Part
    {
        string Partnumber { get; set; } 
        public string? Partname { get; set; }
        List<Part>? AltPartNumbers { get; set; }
        List<PriceLink>? PriceLinks { get; set; }
        
        public Part(string Partnumber)
        {
            this.Partnumber = Partnumber;
            FindPrices();        //added this here so the FindAlternate adds parts with prices already found. 
        }

        public void FindPrices()
        {
            //this here will call the web scraper, hopefully like so
            //PriceLinks.add(ScrapeUroPrice, FCP, ECS.....);

        }

        public void FindAlternateParts()
        {
            //create some sort of lookup for alternate parts
            //call FindPrices for each one, so
            //FOR EACH (alternate part IN where i fint alt parts){
                //AltPartNumbers.add(new Part(partnumber))
            //}
        }



    }
}
