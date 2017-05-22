using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;

namespace Appart
{
    class Program
    {
        static void Main(string[] args)
        {
            Logger.Info("Start AppartScanner");

            
            var apartWeb = new AppartWeb();
            var allNewAppart = apartWeb.RetrieveAllAppart(ConfigLoader.WebSiteTest);

            DisplayResult(apartWeb, allNewAppart);

            if (!ConfigLoader.TestMode)
            {
                SaveInFile(apartWeb.AppartInCache);
            }

            Console.ReadLine();
        }

        private static void DisplayResult(AppartWeb apartWeb, List<Appartement> allNewAppart)
        {
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("**********************************************************");
            Console.WriteLine("*                SUMMARY                                 *");
            Console.WriteLine("**********************************************************");
            Console.WriteLine("");

            var groupResu = allNewAppart.GroupBy(o => o.WebSite);
            foreach (var item in groupResu)
            {
                Console.WriteLine("{0}: {1} new appart, {2} Match. Total={3}", 
                    item.Key, 
                    item.Count(), 
                    item.Count(o => o.MatchCriteria), 
                    apartWeb.AppartInCache.Count(o => o.WebSite == item.Key));

            }

            Console.WriteLine("");
            Console.WriteLine("Summary: {0} new appart Match. {1} new on {2}", allNewAppart.Count(o => o.MatchCriteria), allNewAppart.Count, apartWeb.AppartInCache.Count);
        }

        private static void SaveInFile(List<Appartement> allAppart)
        {
            var list = new List<string> { Appartement.GetHeader() };
            foreach (var appart in allAppart)
            {
                list.Add(appart.ToCsv());
                //Console.WriteLine("New appart: {0}", appart);
            }

            File.WriteAllLines("AppartWeb.csv", list);
        }
    }
}
