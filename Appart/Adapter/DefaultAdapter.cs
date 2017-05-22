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

                    if (string.IsNullOrEmpty(searchResu))
                    {
                        Logger.Error($"   -{item.Key} not found");
                        File.WriteAllText($"WebPage-{appart.WebSite}.txt", fullWebPage);
                    }
                    else
                    {
                        Logger.Debug($"   -{item.Key}={searchResu}");
                    }
                    

                    switch (item.Key)
                    {
                        case "Ville":
                            appart.Ville = this.ProcessVilleOrCodePostal(searchResu);
                            break;
                        case "Surface":
                            appart.Surface = searchResu.ToInt();
                            break;
                        case "Prix":
                            appart.Prix = searchResu.ToInt();
                            break;
                        case "NbrChambre":
                            appart.NbrChambre = searchResu.ToInt();
                            break;
                        case "NbrPiece":
                            appart.NbrPiece = searchResu.ToInt();
                            break;
                    }
                }
            }
        }

        protected string ProcessVilleOrCodePostal(string villeOrCodePostal)
        {

            if (int.TryParse(villeOrCodePostal, out int codePostal))
            {
                switch(codePostal)
                {
                    case 94300:
                        return "VINCENNES";
                    case 94120:
                        return "FONTENAY SOUS BOIS";
                    case 93100:
                        return "MONTREUIL";
                    case 94160:
                        return "SAINT MANDE";
                    case 94130:
                        return "NOGENT SUR MARNE";
                }
            }

            return villeOrCodePostal.ToUpper().Replace("-", " ");
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
