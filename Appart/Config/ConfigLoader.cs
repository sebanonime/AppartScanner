using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Appart.Adapter;
using System.Configuration;

namespace Appart.Config
{
    public class ConfigLoader
    {
        public static string WebSiteTest { get; }
        
        public static string WebSiteDetailTest { get; set; }

        public static int TestModeNbrMaxItem { get; }

        public static bool TestMode => !string.IsNullOrEmpty(WebSiteTest);

        static ConfigLoader()
        {
            WebSiteTest = ConfigurationManager.AppSettings["WebSiteTest"];
            WebSiteDetailTest = ConfigurationManager.AppSettings["WebSiteDetailTest"];
            TestModeNbrMaxItem = ConfigurationManager.AppSettings["TestModeNbrMaxItem"].ToInt();
        }

        public List<AgenceConfig> LoadConfigFile()
        {
            var configs = new List<AgenceConfig>();
            var allDirInfo = new DirectoryInfo("AppartConfig");
            foreach (var file in allDirInfo.GetFiles())
            {
                var url = file.Name;
                configs.Add(this.ReadConfigFile(url, file));
            }

            return configs;
        }

        private AgenceConfig ReadConfigFile(string url, FileInfo file)
        {            
            var agenceConfig = new AgenceConfig();
            agenceConfig.WebSite = file.Name.Replace(".txt","");
            var allLines = File.ReadAllLines(file.FullName, Encoding.Default);
            var adapterName = allLines[1];
            agenceConfig.Encoding = allLines[3];
            agenceConfig.SearchResultUrl = allLines[5];
            agenceConfig.DetailUrlBegin = allLines[7];
            agenceConfig.DetailUrlEnd= allLines[8];
            agenceConfig.EndOfPage = allLines[10];

            for (int i = 11; i < allLines.Length; i++)
            {
                var currentItem = new ConfigSearchItem();
                if (i + 2 < allLines.Length)
                {
                    var keyWithParam = allLines[i].TrimStart('#').Split('[', ']');
                    currentItem.Key = keyWithParam[0];
                    currentItem.SearchType = ConfigSearchType.Default;
                    if (keyWithParam.Length > 1)
                    {
                        currentItem.SearchType = (ConfigSearchType)Enum.Parse(typeof(ConfigSearchType), keyWithParam[1]);
                    }

                    currentItem.Begin = allLines[++i];
                    currentItem.End = allLines[++i];
                    if (currentItem.SearchType == ConfigSearchType.RegexpGroup)
                    {
                        currentItem.Regex = allLines[++i];
                        currentItem.RegexGroupId = allLines[++i].ToInt();
                    }
                    Logger.Debug($"SearchItem({url}): {currentItem}");
                    agenceConfig.SearchItems.Add(currentItem.Key, currentItem);
                }
            }

            agenceConfig.Adapter = this.GetAdapter(adapterName, agenceConfig);

            return agenceConfig;
        }
        
        private IAppartAdapter GetAdapter(string adapterName, AgenceConfig agenceConfig)
        {
            switch(adapterName)
            {
                case "Default":
                    return new DefaultAdapter(agenceConfig);
                case "WithSearchBetween":
                    return new AdapterWithSearchBetween(agenceConfig);
                case "AgencesopportunityAdapter":
                    return new AgencesopportunityAdapter(agenceConfig);
            }

            return null;
        }
        
        public List<Appartement> LoadAppartFile()
        {
            var resu = new List<Appartement>();
            var fileName = "AppartWeb.csv";
            if (File.Exists(fileName))
            {
                var allApart = File.ReadAllLines(fileName);

                for (int i = 1; i < allApart.Length; i++)
                {
                    var line = allApart[i];
                    var splittedLines = line.Split(';');

                    //Prix; Ville; Surface; NbrChambre; NbrPiece; WebSite; UrlDetail
                    var appart = new Appartement();
                    appart.Ville = splittedLines[0].Trim();
                    appart.Prix = splittedLines[1].Trim().ToInt();
                    appart.Surface = splittedLines[2].Trim().ToInt();
                    appart.NbrChambre = splittedLines[3].Trim().ToInt();
                    appart.NbrPiece = splittedLines[4].Trim().ToInt();
                    appart.Date = splittedLines[5].Trim().ToDate();
                    appart.WebSite = splittedLines[7].Trim();
                    appart.ToCheck = splittedLines[8].Trim().ToBool();
                    appart.UrlDetail = splittedLines[9].Trim();

                    resu.Add(appart);
                }
            }

            return resu;
        }

    }
}
