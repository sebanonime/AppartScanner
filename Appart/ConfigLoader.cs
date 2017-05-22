using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Appart.Adapter;
using System.Configuration;

namespace Appart
{
    public class SearchItem
    {
        public string Key { get; set; }

        public string Begin { get; set; }

        public string End { get; set; }

        public override string ToString()
        {
            return $"{Key}-{Begin}-{End}";
        }
    }

    public class AgenceConfig
    {
        public AgenceConfig()
        {
            this.SearchItems = new Dictionary<string, SearchItem>();
        }

        public IAppartAdapter Adapter { get; set; }
        public string WebSite { get; set; }
        public string SearchResultUrl { get; set; }

        public string DetailUrlBegin { get; set; }
        public string DetailUrlEnd{ get; set; }

        public Dictionary<string, SearchItem> SearchItems { get; set; }
        public string EndOfPage { get; internal set; }
        public string Encoding { get; internal set; }
    }

    public class ConfigLoader
    {
        public static string WebSiteTest { get; }

        public static bool TestMode => !string.IsNullOrEmpty(WebSiteTest);

        static ConfigLoader()
        {
            WebSiteTest = ConfigurationManager.AppSettings["WebSiteTest"];
        }

        public List<AgenceConfig> LoadConfigFile()
        {
            var configs = new List<AgenceConfig>();
            var allDirInfo = new DirectoryInfo("Config");
            foreach (var file in allDirInfo.GetFiles())
            {
                var url = file.Name;
                configs.Add(this.ReadConfigFile(url, file));
            }

            return configs;
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

        private AgenceConfig ReadConfigFile(string url, FileInfo file)
        {            
            var agenceConfig = new AgenceConfig();
            agenceConfig.WebSite = file.Name.Replace(".txt","");
            var allLines = File.ReadAllLines(file.FullName);
            var adapterName = allLines[1];
            agenceConfig.Encoding = allLines[3];
            agenceConfig.SearchResultUrl = allLines[5];
            agenceConfig.DetailUrlBegin = allLines[7];
            agenceConfig.DetailUrlEnd= allLines[8];
            agenceConfig.EndOfPage = allLines[10];

            for (int i = 11; i < allLines.Length; i++)
            {
                var currentItem = new SearchItem();
                if (i + 2 < allLines.Length)
                {
                    var key = allLines[i].TrimStart('#');
                    i++;
                    var begin = allLines[i];
                    i++;
                    var end = allLines[i];
                    currentItem.Key = key.TrimStart('#');

                    currentItem.Begin = begin;
                    currentItem.End = end;
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
    }
}
