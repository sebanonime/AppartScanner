using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appart.Adapter
{
    using Appart.Config;

    public class OrpiAdapter : DefaultAdapter
    {
        public OrpiAdapter(AgenceConfig agenceConfig)
            : base(agenceConfig)
        {
        }

        public override string GetDetailUrl(ref string webPage)
        {
            var startUrl = this.agenceConfig.DetailUrlBegin;
            var resu = webPage.SearchBetween(startUrl, @"""", out webPage).CleanUrl().CleanText();
            if (string.IsNullOrEmpty(resu))
            {
                return null;
            }

            return ("https://www.orpi.com/annonce-vente-" + resu).CleanUrl();
        }
    }
}
