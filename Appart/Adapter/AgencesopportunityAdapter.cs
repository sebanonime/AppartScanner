using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appart.Adapter
{
    public class AgencesopportunityAdapter : DefaultAdapter
    {
        public AgencesopportunityAdapter(AgenceConfig agenceConfig) : base(agenceConfig)
        {
        }


        public override void SetAppartInfo(Appartement appart, string webPage, string url)
        {
            base.SetAppartInfo(appart, webPage, url);

            // http://www.agencesopportunity.com/annonce,vente-
            //maison -viry-chatillon,6029,
            var villeTemp = url.Replace("http://www.agencesopportunity.com/annonce,vente-", "");
            var splitTemp = villeTemp.Split(',');
            if (splitTemp.Length > 1)
            {
                var ville = splitTemp[0];
                splitTemp = ville.Split('-');
                if (splitTemp.Length > 1)
                {
                    appart.Ville = ProcessVilleOrCodePostal(ville.Replace(splitTemp[0] + "-", ""));
                }
            }
        }
    }
}
