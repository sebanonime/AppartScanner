using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appart.Adapter
{
    public class AdapterWithSearchBetween : DefaultAdapter
    {
        public AdapterWithSearchBetween(AgenceConfig agenceConfig) : base(agenceConfig)
        {
        }

        public override string GetDetailUrl(ref string webPage)
        {
            
            return webPage.SearchBetween(this.agenceConfig.DetailUrlBegin, this.agenceConfig.DetailUrlEnd, out webPage).CleanText().CleanUrl();
        }
    }
}
