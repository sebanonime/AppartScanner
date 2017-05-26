using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using Appart.Config;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Appart
{
    class Program
    {
        static void Main(string[] args)
        {
            Logger.Info("Start AppartScanner");

            var configLoader = new ConfigLoader();
            var allConfig = configLoader.LoadConfigFile();
            var allAppartInCache = configLoader.LoadAppartFile();
            var allNewAppart = new ConcurrentBag<Appartement>();

            Parallel.ForEach(allConfig, (config) =>
            {
                if (string.IsNullOrEmpty(ConfigLoader.WebSiteTest) || ConfigLoader.WebSiteTest == config.WebSite)
                {
                    Logger.Info($"Start to search {config.WebSite}");
                    var appartInCacheForWebSite = allAppartInCache.Where(o => o.WebSite == config.WebSite).ToList();
                    var apartWeb = new AppartWebSearcher(config, appartInCacheForWebSite);
                    var result = apartWeb.RetrieveAllAppart();
                    foreach (var item in result)
                    {
                        allNewAppart.Add(item);
                    }
                }
            });

            allAppartInCache.AddRange(allNewAppart);
            DisplayResult(allAppartInCache, allNewAppart);

            if (!ConfigLoader.TestMode)
            {
                SaveInFile(allAppartInCache);
            }

            Console.ReadLine();
        }

        private static void DisplayResult(List<Appartement> allAppartInCache, ConcurrentBag<Appartement> allNewAppart)
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
                    allAppartInCache.Count(o => o.WebSite == item.Key));

            }

            Console.WriteLine("");
            Console.WriteLine("Summary: {0} new appart Match. {1} new on {2}", allNewAppart.Count(o => o.MatchCriteria), allNewAppart.Count, allAppartInCache.Count);
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
