using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appart.Adapter
{
    public class DefaultAdapter : IAppartAdapter
    {
        protected AgenceConfig agenceConfig;

        public DefaultAdapter(AgenceConfig agenceConfig)
        {
            this.agenceConfig = agenceConfig;
        }

        public virtual string GetDetailUrl(ref string webPage)
        {
            var startUrl = this.agenceConfig.DetailUrlBegin;
            var resu = webPage.SearchBetween(startUrl, @"""", out webPage).CleanUrl().CleanText();
            if (string.IsNullOrEmpty(resu))
            {
                return null;
            }

            return (startUrl.CleanUrl() + resu).CleanUrl();
        }

        public virtual void SetAppartInfo(Appartement appart, string webPage, string url)
        {
            var fullWebPage = webPage;
            foreach (var item in this.agenceConfig.SearchItems.Values)
            {
                if (!string.IsNullOrEmpty(item.Begin) && !string.IsNullOrEmpty(item.End))
                {
                    Logger.Debug($"{appart.WebSite}: Search {item.Key} between '{item.Begin}' and '{item.End}'. Url={url}");
                    var searchResu = webPage.SearchBetween(@item.Begin, @item.End, out webPage).CleanText();

                    switch (item.Key)
                    {
                        case "Ville":
                            appart.Ville = this.ProcessVilleOrCodePostal(searchResu);
                            break;
                        case "Surface":
                            appart.Surface = searchResu.ToInt();
                            break;
                        case "Prix":
                            appart.Prix = searchResu.Replace("€", "").Trim().ToInt();
                            break;
                        case "NbrChambre":
                            appart.NbrChambre = searchResu.ToInt();
                            break;
                        case "NbrPiece":
                            appart.NbrPiece = searchResu.ToInt();
                            break;
                    }

                    if (ConfigLoader.TestMode)
                    {
                        File.WriteAllText($@"Logs\WebPage-{appart.WebSite}.txt", fullWebPage);
                    }

                    if (string.IsNullOrEmpty(searchResu))
                    {
                        Logger.Error($"   -{item.Key} not found");                        
                    }
                    else
                    {
                        Logger.Debug($"   -{item.Key}={searchResu}. {appart.AllDataAvailable()} - {appart}");
                    }
                }
            }
        }

        protected string ProcessVilleOrCodePostal(string villeOrCodePostal)
        {
            var mapping = new Dictionary<int, string>
            {
                { 94300, "VINCENNES" },
                { 94120, "FONTENAY SOUS BOIS" },
                { 93100, "MONTREUIL" },
                { 94160, "SAINT MANDE" },
                { 94130, "NOGENT SUR MARNE" },

            };

            if (int.TryParse(villeOrCodePostal, out int codePostal) 
                && mapping.TryGetValue(codePostal, out string ville))
            {
                return ville;
            }

            var codePostalToRemove = mapping.Keys.Where(o => villeOrCodePostal.Contains(o.ToString())).FirstOrDefault();
            villeOrCodePostal = villeOrCodePostal.Replace(codePostalToRemove.ToString(), "");

            return villeOrCodePostal.ToUpper().Replace("-", " ").CleanText();
        }

        public bool PageEnded (string webPage)
        {
            if (string.IsNullOrEmpty(this.agenceConfig.EndOfPage))
            {
                return false;
            }

            return string.IsNullOrEmpty(webPage.SearchLineContain(this.agenceConfig.EndOfPage));
        }
    }
}
