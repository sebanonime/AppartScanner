using Appart.Adapter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Appart
{
    public class AppartWeb
    {
        private string webSiteHost;

        //private IAppartAdapter adapter;

        //private Dictionary<string, IAppartAdapter> adapterFactory = new Dictionary<string, IAppartAdapter>()
        //{
        //    { "www.properties-v.com", new PropertiesvAdapter() },
        //    { "www.bsi94.com", new Bsi94Adapter() }
        //};
        private List<AgenceConfig> config;
        public List<Appartement> AppartInCache { get; set; }

        public AppartWeb()
        {
            Logger.Debug("Start AppartWeb");
            var configLoader = new ConfigLoader();
            this.config = configLoader.LoadConfigFile();

            this.AppartInCache = configLoader.LoadAppartFile();
        }
        

        public List<Appartement> RetrieveAllAppart(string webSite = null)
        {
            var allAppart = new List<Appartement>();
            foreach (var config in this.config)
            {
                if (string.IsNullOrEmpty(webSite) || config.WebSite == webSite)
                {
                    Console.Write("{0}: ", config.WebSite);
                    var apparts = this.SearchAppartOfWebSite(config);
                    Console.WriteLine("");
                    allAppart.AddRange(apparts);
                }
            }
            
            return allAppart;
        }

        private List<Appartement> SearchAppartOfWebSite(AgenceConfig config)
        {
            var url = config.SearchResultUrl;
            this.webSiteHost = config.SearchResultUrl.GetHost();
            var allAppartOfWebSite = new List<Appartement>();

            int pageNbr = 1;
            while (true)
            {
                var urlWithPage = url.Replace("%PAGE_NBR%", pageNbr.ToString());
                Logger.Info($"Search page {pageNbr} in {urlWithPage}");
                Console.Write(" page {0}: ", pageNbr);
                var webPage = WebRequester.SendHttpRequest(urlWithPage, config.Encoding);
                if (string.IsNullOrEmpty(webPage))
                {
                    Logger.Error($"Search result page failed for {urlWithPage}");
                }
                else
                {
                    Logger.Info($"Search result page OK for {urlWithPage}");
                }
                

                if (string.IsNullOrEmpty(webPage))
                {
                    break;
                }

                if (pageNbr > 10)
                {
                    Console.WriteLine("WE HAVE MORE THAN 10 PAGES !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                    break;
                }

                if (!TryParseSearchResultWebPage(config, webPage, out List<Appartement> allAppartOfWebPage))
                {
                    break;
                }

                allAppartOfWebSite.AddRange(allAppartOfWebPage);
                this.AppartInCache.AddRange(allAppartOfWebPage);

                if (ConfigLoader.TestMode)
                {
                    break;
                }

                if (url.Contains("%PAGE_NBR%"))
                {
                    pageNbr++;
                }
                else
                {
                    break;
                }
            }

            return allAppartOfWebSite;
        }

        private bool TryParseSearchResultWebPage(AgenceConfig config, string webPage, out List<Appartement> allAppart)
        {
            string url = config.SearchResultUrl;
            string detailWebPageUrl = "Wait";
            allAppart = new List<Appartement>();
            int appartFound = 0;
            while (!string.IsNullOrEmpty(detailWebPageUrl))
            {
                detailWebPageUrl = config.Adapter.GetDetailUrl(ref webPage).AddHostIfMissing(url);                
                if (config.Adapter.PageEnded(webPage))
                {
                    break;
                }

                if (!string.IsNullOrEmpty(detailWebPageUrl))
                {
                    Logger.Debug($"Detail Web Page Url : {detailWebPageUrl}");
                    if (!allAppart.Any(o => o.UrlDetail == detailWebPageUrl))
                    {                        
                        appartFound++;
                        if (!this.AppartExistInCache(detailWebPageUrl))
                        {
                            if (this.TryRetrieveAppartInfo(detailWebPageUrl, config, out Appartement appart))
                            {
                                //if (appart.MatchCriteria())
                                {
                                    allAppart.Add(appart);
                                }
                            }
                        }
                        else
                        {
                            Logger.Debug("Appart already in cache");
                        }
                    }
                }
                else
                {
                    Logger.Error($"Detail Web Page Url not found");
                }

                if (ConfigLoader.TestMode)
                {
                    break;
                }
            }

            Console.Write($" {allAppart.Count} new Appart on {appartFound}");
            return appartFound > 0;
        }

        private bool AppartExistInCache(string detailWebPageUrl)
        {
            return this.AppartInCache.Exists(o => o.UrlDetail == detailWebPageUrl);
        }

        public bool TryRetrieveAppartInfo(string url, AgenceConfig config, out Appartement appart)
        {
            var webPage = WebRequester.SendHttpRequest(url, config.Encoding);
            if (string.IsNullOrEmpty(webPage))
            {
                Logger.Error("Detail web page not found");
                appart = null;
                return false;
            }

            Logger.Info($"Detail web page found: {url}");
            appart = new Appartement();
            appart.ToCheck = true;
            appart.WebSite = url.GetHost();
            appart.UrlDetail = url;
            appart.Date = DateTime.Now;

            config.Adapter.SetAppartInfo(appart, webPage, url);

            if (!string.IsNullOrEmpty(webPage))
            {
                if (appart.AllDataAvailable())
                {

                }

                return true;
            }

            return false;

            //return TryParseDetailWebPage(webPage, out appart);
        }

        //private bool TryParseDetailWebPage(string webPage, out Appartement appart)
        //{
        //    appart = new Appartement();
        //    appart.UrlDetail = webPage;
        //    appart.Ville = webPage.SearchBetween("<figure>", "</figure>", out webPage).CleanText();
        //    appart.Prix = webPage.SearchBetween(@"class=""tag price"">", "€</span>", out webPage).ToInt();
        //    appart.Surface = webPage.SearchBetween("Surface habitable :", "m<sup>", out webPage).CleanText().ToInt();
        //    appart.NbrPiece = webPage.SearchBetween("Pièces :", "</dd>", out webPage).CleanText().ToInt();
        //    appart.NbrChambre = webPage.SearchBetween("Chambres :", "</dd>", out webPage).CleanText().ToInt();

        //    if (!string.IsNullOrEmpty(webPage))
        //    {
        //        if (appart.AllDataAvailable())
        //        {

        //        }

        //        return true;
        //    }

        //    return false;
        //}
    }
}
