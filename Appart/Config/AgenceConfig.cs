using System.Collections.Generic;
using Appart.Adapter;

namespace Appart.Config
{
    public class AgenceConfig
    {
        public AgenceConfig()
        {
            this.SearchItems = new Dictionary<string, ConfigSearchItem>();
        }

        public IAppartAdapter Adapter { get; set; }
        public string WebSite { get; set; }
        public string SearchResultUrl { get; set; }

        public string DetailUrlBegin { get; set; }
        public string DetailUrlEnd{ get; set; }

        public Dictionary<string, ConfigSearchItem> SearchItems { get; set; }
        public string EndOfPage { get; internal set; }
        public string Encoding { get; internal set; }
    }
}
