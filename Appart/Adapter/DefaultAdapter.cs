using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Appart.Config;
using System.Text.RegularExpressions;

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
            var previousSearchBegin = string.Empty;
            var previousSearchEnd = string.Empty;
            string searchResu = string.Empty;
            foreach (var item in this.agenceConfig.SearchItems.Values)
            {
                if (!string.IsNullOrEmpty(item.Begin) && !string.IsNullOrEmpty(item.End))
                {
                    if (previousSearchBegin != item.Begin || previousSearchEnd != item.End)
                    {
                        searchResu = webPage.SearchBetween(@item.Begin, @item.End, out webPage);
                        previousSearchBegin = item.Begin;
                        previousSearchEnd = item.End;
                    }

                    Logger.Debug($"{appart.WebSite}: Search {item.Key} between '{item.Begin}' and '{item.End}'. Result={searchResu}. Url={url}");

                    string finalSearchResu = string.Empty;
                    if (item.SearchType == ConfigSearchType.RegexpGroup)
                    {
                        var resuMatch = Regex.Match(searchResu, item.Regex);                        
                        if (resuMatch.Success)
                        {                            
                            finalSearchResu = resuMatch.Groups[item.RegexGroupId].Value;
                        }
                        else
                        {
                            Logger.Error($"Regex match({item.RegexGroupId}) not found in {searchResu} with Regex {item.Regex}");
                        }
                    }
                    else
                    {
                        finalSearchResu = searchResu;
                    }

                    finalSearchResu = finalSearchResu.CleanText();

                    switch (item.Key)
                    {
                        case "Ville":
                            appart.Ville = this.ProcessVilleOrCodePostal(finalSearchResu);
                            break;
                        case "Surface":
                            appart.Surface = finalSearchResu.ToInt();
                            break;
                        case "Prix":
                            appart.Prix = finalSearchResu.Replace("€", "").Trim().ToInt();
                            break;
                        case "NbrChambre":
                            appart.NbrChambre = finalSearchResu.ToInt();
                            break;
                        case "NbrPiece":
                            appart.NbrPiece = finalSearchResu.ToInt();
                            break;
                    }

                    if (ConfigLoader.TestMode)
                    {
                        File.WriteAllText($@"Logs\WebPage-{appart.WebSite}.txt", fullWebPage);
                    }

                    if (string.IsNullOrEmpty(finalSearchResu))
                    {
                        Logger.Error($"   -{item.Key} not found");                        
                    }
                    else
                    {
                        Logger.Debug($"   -{item.Key}={finalSearchResu}. {appart.AllDataAvailable()} - {appart}");
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

            int codePostal;
            string ville;
            if (int.TryParse(villeOrCodePostal, out codePostal)
                && mapping.TryGetValue(codePostal, out ville))
            {
                return ville;
            }

            var codePostalToRemove = mapping.Keys.FirstOrDefault(o => villeOrCodePostal.Contains(o.ToString()));
            villeOrCodePostal = villeOrCodePostal.Replace(codePostalToRemove.ToString(), "");

            return villeOrCodePostal.ToUpper().Replace("-", " ").CleanText();
        }

        public bool PageEnded (string webPage)
        {
            if (string.IsNullOrEmpty(this.agenceConfig.EndOfPage))
            {
                return false;
            }

            return !webPage.Contains(this.agenceConfig.EndOfPage);
            //return string.IsNullOrEmpty(webPage.SearchLineContain(this.agenceConfig.EndOfPage));
        }
    }
}
