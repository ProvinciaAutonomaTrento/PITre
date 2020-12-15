using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocsPaIntegration.Config;
using DocsPaIntegration.Attributes;
using DocsPaIntegration.Search;
using DocsPaIntegration.ObjectTypes;
using System.Drawing;
using DocsPaIntegrationImpl.Properties;
using System.ServiceModel.Channels;
using System.Configuration;
using System.ServiceModel.Configuration;
using System.ServiceModel;
using System.Xml;
using System.ServiceModel.Security;
using DocsPaIntegrationImpl.ExternalWR;
using DocsPaIntegration;
using DocsPaIntegration.ObjectTypes.Attributes;
using DocsPaIntegrationImpl.VO;

namespace DocsPaIntegrationImpl.Implementations
{
    [IntegrationAdapterAttribute("SAPIntegrationAdapter","SAP integration adapter","L'adapter interagisce con un webservice (il cui URL è configurato da parametro) che si interfaccia con il sistema SAP","1.0",true)]
    public class SAPIntegrationAdapter : GeneralIntegrationAdapter
    {
        [IntegrationStringType("WS Url",false)]
        private string WSUrl
        {
            get;
            set;
        }

        [IntegrationStringType("Username", true)]
        private string Username
        {
            get;
            set;
        }

        [IntegrationStringType("Password", true)]
        private string Password
        {
            get;
            set;
        }

        [IntegrationBooleanType("Network auth", false)]
        private bool NetworkAuth
        {
            get;
            set;
        }


        protected override void InitParticular()
        {
            
        }

        private ExternalWSSoapClient getWSClient()
        {
            BasicHttpBinding binding = new BasicHttpBinding();
            binding.AllowCookies = false;
            binding.BypassProxyOnLocal = false;
            binding.CloseTimeout = new TimeSpan(0, 1, 0);
            binding.HostNameComparisonMode = HostNameComparisonMode.StrongWildcard;
            binding.MaxBufferPoolSize = 524288;
            binding.MaxBufferSize = 65536;
            binding.MaxReceivedMessageSize = 65536;
            binding.MessageEncoding = WSMessageEncoding.Text;
            binding.Name="ExternalWSSoap";
            binding.OpenTimeout= new TimeSpan(0, 1, 0);
            binding.ReceiveTimeout = new TimeSpan(0, 1, 0);
            XmlDictionaryReaderQuotas readQuot = new XmlDictionaryReaderQuotas();
            readQuot.MaxArrayLength = 16384;
            readQuot.MaxBytesPerRead = 4096;
            readQuot.MaxDepth = 32;
            readQuot.MaxNameTableCharCount = 16384;
            readQuot.MaxStringContentLength = 8192;
            binding.ReaderQuotas = readQuot;
            binding.Security.Mode = BasicHttpSecurityMode.None;
            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;
            binding.Security.Transport.ProxyCredentialType = HttpProxyCredentialType.None;
            binding.Security.Transport.Realm = "";
            binding.Security.Message.ClientCredentialType = BasicHttpMessageCredentialType.UserName;
            binding.Security.Message.AlgorithmSuite = SecurityAlgorithmSuite.Default;
            ExternalWSSoapClient res = new ExternalWSSoapClient(binding, new EndpointAddress(WSUrl));
            return res;
        }

        public override DocsPaIntegration.Search.SearchOutputRow PuntualSearch(DocsPaIntegration.Search.PuntualSearchInfo puntualSearchInfo)
        {
            try
            {
                ExternalWSSoapClient soap = getWSClient();
                PuntualSearchInfoWS search = new PuntualSearchInfoWS();
                search.Codice = puntualSearchInfo.Codice;
                PuntualSearchOutputWS outputWS = soap.PuntualSearch(search);
                if (outputWS.Code == SearchOutputCode.KO)
                {
                    throw new SearchException(SearchExceptionCode.SERVER_ERROR, outputWS.ErrorMessage);
                }
                SearchOutputRow res = new SearchOutputRow();
                if (outputWS.Row != null)
                {
                    res.Codice = outputWS.Row.Codice;
                    res.Descrizione = outputWS.Row.Descrizione;
                }
                return res;
            }
            catch (Exception e)
            {
                throw new SearchException(SearchExceptionCode.SERVICE_UNAVAILABLE);
            }
        }

        public override DocsPaIntegration.Search.SearchOutput Search(DocsPaIntegration.Search.SearchInfo searchInfo)
        {
            try
            {
                List<SearchOutputRow> rows = new List<SearchOutputRow>();
                ExternalWSSoapClient soap = getWSClient();
                SearchInfoWS search = new SearchInfoWS();
                search.Descrizione = searchInfo.Descrizione;
                search.Codice = searchInfo.Codice;
                search.PageSize = searchInfo.PageSize;
                search.RequestedPage = searchInfo.RequestedPage;
                SearchOutputWS outputWS = soap.Search(search);
                if (outputWS.Code == SearchOutputCode.KO)
                {
                    throw new SearchException(SearchExceptionCode.SERVER_ERROR, outputWS.ErrorMessage);
                }
                for (int i = 0; i < outputWS.Rows.Length; i++)
                {
                    SearchOutputRow row = new SearchOutputRow();
                    row.Codice = outputWS.Rows[i].Codice;
                    row.Descrizione = outputWS.Rows[i].Descrizione;
                    rows.Add(row);
                }
                SearchOutput res = new SearchOutput(rows, outputWS.NumTotResults);
                return res;
            }
            catch (SearchException se)
            {
                throw se;
            }
            catch (Exception e)
            {
                throw new SearchException(SearchExceptionCode.SERVICE_UNAVAILABLE);
            }
        }


        public override Bitmap Icon
        {
            get {
                return Resources.sap_adapter_icon;
            }

        }

        public override bool HasIcon
        {
            get
            {
                return true;
            }
        }

        [IntegrationAdapterOperationAttribute("Pippo")]
        public TestOutput Pippo(TestInput input)
        {
            return null;
        }
    }
}
