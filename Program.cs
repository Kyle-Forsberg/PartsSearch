using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Threading;
using HtmlAgilityPack;
using MySql.Data;
using MySql.Data.MySqlClient;
using static System.Net.WebRequestMethods;

namespace CheapestPart
{
    public class FindCheapestPart
    {
        public static void Main(string[]args)
        {
            //string partNumber = args[0];

            Console.WriteLine("CHEAPEST PARTS: ");
            //step 1: create the parts class                                            DONE
            //1.1 create part number, part name, and pricelists for various sellers     Progress
            //1.2 create PriceLink class as a abstraction to make rest easier.          DONE
            //step 2: find prices based off of part numbers                             Progress
            //2.1 find urotunings prices                                                Issues
            //2.2 find ECS prices                                                       Progess
            //2.3 find FCP euro prices
            //2.4 find KermaTDI prices
            //2.5 find amazon prices
            //step 3 load those prices into the classes made for them
            //step 4 find alternative partnumbers for that part and repeat step 3
            //4.1 check for repeat partnumbers to make sure we dont infinitely loop parts around
            //step 5 return link to cheapest found part.
            SearchPart("02A141165M");




        }

        public static List<PriceLink> SearchPart(string partnumber)
        {
            PriceScraper scraper = new PriceScraper();
            var links = scraper.SearchPart(partnumber);
            Console.WriteLine(links);
            var PriceList = new List<PriceLink>();
            foreach (var link in links)
            {
                PriceList.Add(scraper.ScrapePrice(link));
            }
            foreach (var price in PriceList)
            {
                Console.WriteLine(price);
            }
            return PriceList;
        }




    }
}