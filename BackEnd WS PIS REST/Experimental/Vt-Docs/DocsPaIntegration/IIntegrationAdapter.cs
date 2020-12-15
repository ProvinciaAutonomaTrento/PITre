using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocsPaIntegration.Config;
using DocsPaIntegration.Search;
using System.Drawing;
using DocsPaIntegration.Operation;

namespace DocsPaIntegration
{
    public interface IIntegrationAdapter
    {

        void Init(ConfigurationInfo configurationInfo);

        IntegrationAdapterInfo AdapterInfo
        {
            get;
        }

        ConfigurationInfo ConfigurationInfo
        {
            get;
        }

        Bitmap Icon
        {
            get;
        }

        string IdLabel
        {
            get;
        }

        string DescriptionLabel
        {
            get;
        }

        SearchOutputRow PuntualSearch(PuntualSearchInfo puntualSearchInfo);

        SearchOutput Search(SearchInfo searchInfo);

        List<OperationInfo> Operations
        {
            get;
        }

    }
}
