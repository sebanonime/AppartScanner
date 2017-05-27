using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appart
{
    public class Appartement
    {
        public string Id { get; set; }

        public string UrlDetail { get; set; }

        public int Prix { get; set; }

        public string Ville { get; set; }

        public int Surface { get; set; }

        public int NbrChambre { get; set; }

        public int NbrPiece { get; set; }

        public string WebSite { get; set; }

        public DateTime Date { get; set; }

        public string Comments { get; set; }

        public bool AllDataAvailable()
        {
            return this.Prix > 0
                && !string.IsNullOrEmpty(this.Ville)
                && this.Surface > 0
                && this.NbrPiece > 0;
        }

        public bool MatchCriteria
        {
            get
            {
                if (this.Surface < 85
                    || this.Prix > 830000
                    || this.NbrPiece < 4)
                {
                    return false;
                }

                var listVille = new List<string> { "FONTENAY", "VINCENNES", "MONTREUIL", "NOGENT", "MANDE" };

                var villeCheck = listVille.Any(o => Ville.ToUpper().Contains(o));

                return villeCheck;
            }
        }
        
        public static string GetHeader()
        {
            return $"Ville; Prix; Surface; NbrChambre; NbrPiece; Date; MatchCriteria; WebSite; Comments; UrlDetail";
        }

        public override string ToString()
        {
            return $"Ville={Ville}, Prix={Prix}, Surface={Surface}, NbrChambre={NbrChambre}, NbrPiece={NbrPiece}, WebSite={WebSite}, Comments={Comments}, UrlDetail={UrlDetail}";
        }

        public string ToCsv()
        {
            return $"{Ville}; {Prix}; {Surface}; {NbrChambre}; {NbrPiece}; {Date}; {MatchCriteria}; {WebSite}; {Comments}; {UrlDetail}";
        }
    }
}
