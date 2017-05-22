using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Appart
{
    public static class WebPageParseTools
    {

        #region Methodes Utiles 

        public static string CleanText(this string input)
        {
            string resu = input.Replace("<b>", "").Replace("</b>", "").Replace("<a>", "").Replace("</a>", "").Replace("<br>", "").Replace("<br />", "")
                .Replace("<br/>", "")
                .Replace("<dt>", "").Replace("</dt>", "")
                .Replace("<dd>", "").Replace("</dd>", "")
                .Replace("Ã©", "é").Replace("Ã¨", "è").Replace("Ãª", "ê").Replace("Â»", "\"").Replace("Ã?", "E").Replace("Ã", "à").Replace("àª", "ê").Replace("%27", "'")
                .Replace("\r\n", "").Replace("  ", "").Replace("&nbsp;", "").Replace("<i>", "").Replace("</i>", "").Replace("&quot;", "\"")
                .Replace("<h4>", "").Replace("<b>", "").Replace("</b>", "").Replace("â?¦", "...").Replace("à§", "ç").Replace("à¶", "ö").Replace("à´", "ô").Replace("à®", "î").Replace("à¹", "ù")
                .Replace("à¢", "a").Replace("'?¦", "").Replace("â??", "'").Replace("'??", "\'").Replace("à«", "ë").Replace("Â«", "\"").
                Replace("&amp;", "&").Replace("&#39;", "'").Replace("&#233;", "é").Replace("&#232;", "è")
                .Replace("</span>", "").Replace("<span>", "").Trim();

            return resu;
        }

        public static string CleanUrl(this string input)
        {
            return input.Replace(@"<a href=""", "").CleanText();
        }

        public static string AddHostIfMissing(this string input, string fullUrl)
        {
            if (string.IsNullOrEmpty(input))
            {
                return null;
            }

            var myUri = new Uri(fullUrl);
            string host = fullUrl.GetHost();

            if (!input.StartsWith(host))
            {
                input = myUri.Scheme + "://" + host + (input.StartsWith("/") ? "" : "/") + input;
            }

            return input;
        }

        public static string GetHost (this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return null;
            }

            var myUri = new Uri(input);
            return myUri.Host;  // host is "www.contoso.com"
        }

        public static int ToInt(this string input)
        {
            if(double.TryParse(input.Replace(" ", ""), out double result))
            {
                return (int)result;
            }

            return -666;
        }

        public static bool ToBool(this string input)
        {
            if (bool.TryParse(input.Replace(" ", ""), out bool result))
            {
                return result;
            }

            return false;
        }

        public static DateTime ToDate(this string input)
        {
            if (DateTime.TryParse(input, out DateTime result))
            {
                return result;
            }

            return new DateTime();
        }

        public static string Search(string Source, string strSearch)
        {
            int debut = Source.IndexOf(strSearch);
            if (debut == -1)
                return "";
            int fin = Source.Length;//Source.IndexOf(Convert.ToChar(13), debut);

            return Source.Substring(debut - 1, (fin - debut) + 1);
        }

        public static string SearchLine(string Source, string strSearch)
        {
            int debut = Source.IndexOf(strSearch);
            if (debut == -1)
                return "";
            int fin = Source.IndexOf(Convert.ToChar(13), debut);
            if (fin == -1)
                return "";

            return Source.Substring(debut - 1, (fin - debut) + 1);
        }

        public static string SearchLineContain(this string Source, string strSearch)
        {
            int keywordIdx = Source.IndexOf(strSearch);
            if (keywordIdx == -1)
                return "";
            int fin = Source.IndexOf(Convert.ToChar(13), keywordIdx);

            int debut = int.MaxValue;
            while (debut != -1 && keywordIdx > 0 && debut >= fin)
            {
                debut = Source.IndexOf(Convert.ToChar(13), keywordIdx);
                keywordIdx -= 50;
            }

            if (debut == -1 || debut == 0)
                return "";

            if (fin == -1)
                return "";

            return Source.Substring(debut - 1, (fin - debut) + 1);
        }

        public static string SearchBetween(this string Source, string StrDebut, string strFin, out string remainingPage)
        {
            if (string.IsNullOrEmpty(Source))
            {
                remainingPage = null;
                return "";
            }

            remainingPage = Source;
            int fin = -1;
            int debut = Source.IndexOf(StrDebut);
            if (debut == -1)
            {
                remainingPage = null;
                return "";
            }

            if (strFin == null || (strFin != " " && strFin.Trim().Length == 0))
                fin = Source.Length;
            else
            {
                fin = Source.IndexOf(strFin, debut + StrDebut.Length);
                if (fin == -1)
                    return "";
            }
            remainingPage = Source.Substring(fin);
            var resu = Source.Substring(debut + StrDebut.Length, fin - (debut + StrDebut.Length));
            return resu;

        }
        #endregion

    }
}
