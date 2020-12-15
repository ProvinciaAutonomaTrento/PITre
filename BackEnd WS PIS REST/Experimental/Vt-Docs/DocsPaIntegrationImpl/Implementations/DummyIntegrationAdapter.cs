using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocsPaIntegration.Attributes;
using DocsPaIntegration.Search;
using System.Drawing;
using DocsPaIntegration;
using DocsPaIntegration.ObjectTypes;

namespace DocsPaIntegrationImpl.Implementations
{
    [IntegrationAdapterAttribute("DummyIntegrationAdapter", "Dummy integration adapter", "L'adapter è solo un dummy",false)]
    public class DummyIntegrationAdapter : GeneralIntegrationAdapter
    {
        protected override void InitParticular()
        {

        }

        public override SearchOutputRow PuntualSearch(PuntualSearchInfo puntualSearchInfo)
        {
            throw new SearchException(SearchExceptionCode.SERVICE_UNAVAILABLE);
        }

        public override SearchOutput Search(SearchInfo searchInfo)
        {
            throw new SearchException(SearchExceptionCode.SERVICE_UNAVAILABLE);
        }

        public override Bitmap Icon
        {
            get
            {
                return null;
            }

        }

        public override bool HasIcon
        {
            get
            {
                return false;
            }
        }
    }
}
