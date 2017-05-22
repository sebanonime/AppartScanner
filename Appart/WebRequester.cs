using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Appart
{
    public static class WebRequester
    {
        /// <summary>
        /// envoie une requete Http et récupere le code html de la page
        /// </summary>
        /// <param name="Url">Url de la page web a appller</param>
        /// <returns>Le code Html de la page</returns>
        public static String SendHttpRequest(String Url, string encoding)
        {
            try
            {
                ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(AcceptAllCertifications);
                //on crée la requête et on l'envoie sur le net
                HttpWebRequest PgReq = (HttpWebRequest)WebRequest.Create(Url);

                PgReq.Method = "GET";
                PgReq.Accept = "text/html";
                PgReq.UserAgent = @"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.106 Safari/537.36";

                //// pour passer le proxy
                IWebProxy proxy = null;

                //if (Param_Connexion != 1)
                //{

                //    if (Param_Connexion == 2)
                proxy = WebRequest.DefaultWebProxy; // recupere infos proxy par defaut (config IE)
                                                    //    if (Param_Connexion == 3)
                                                    //        proxy = new WebProxy(ProxyAdressse, ProxyPort);

                //    if (String.IsNullOrEmpty(ProxyLogin))
                //        throw new AlloCineException("Login Vide.\r\nRenseigner les parametres de connexion");

                //    proxy.Credentials = new NetworkCredential(ProxyLogin, Crypto.Decrypt(ProxyPassword));
                //    PgReq.Proxy = proxy;
                //}

                WebResponse PgResp = PgReq.GetResponse();

                Encoding encode;
                if (encoding == "Default")
                {
                    encode = Encoding.Default;
                }
                else
                {
                    encode = Encoding.GetEncoding(encoding);
                }

                StreamReader sr = new StreamReader(PgResp.GetResponseStream(), encode);

                //On récuperer le resultat HTML
                String FullPage = sr.ReadToEnd();

                sr.Close();// on ferme le flux

                return FullPage;
            }
            catch(Exception ex)
            {
                Console.WriteLine("exception: " + ex.Message);
                return string.Empty;
            }            
        }

        public static bool AcceptAllCertifications(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certification, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
    }
}
