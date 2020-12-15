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
using System.IO;
using DocsPaIntegration.ObjectTypes.Attributes;
using log4net;

namespace DocsPaIntegrationImpl.Implementations
{
    [IntegrationAdapterAttribute("WSIntegrationAdapter","WS integration adapter","L'adapter interagisce con un webservice (il cui URL è configurato da parametro) che ha un WSDL predefinito","1.0",true)]
    public class WSIntegrationAdapter : GeneralIntegrationAdapter
    {
        private ILog logger = LogManager.GetLogger(typeof(WSIntegrationAdapter));

        [IntegrationStringType("WS Url", true)]
        private string WSUrl
        {
            get;
            set;
        }

        [IntegrationStringType("Username", false)]
        private string Username
        {
            get;
            set;
        }

        [IntegrationStringType("Password", false)]
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

        [IntegrationImageType("Icon", false,MaxSize=10)]
        private FileType Image
        {
            get;
            set;
        }

        protected override void InitParticular()
        {
            if (NetworkAuth && string.IsNullOrEmpty(Username))
            {
                throw new DocsPaIntegration.Config.ConfigurationException(ConfigurationExceptionCode.VALIDATION_ERROR, "Specificare la Username se si vuole Network auth");
            }
        }

        private ExternalWSSoapClient GetWSClient()
        {
            logger.Info("BEGIN");
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
            logger.Debug("building client...");
            ExternalWSSoapClient res = new ExternalWSSoapClient(binding, new EndpointAddress(WSUrl));
            if (NetworkAuth)
            {
                binding.Security.Mode = BasicHttpSecurityMode.TransportCredentialOnly;
                binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
                logger.Debug("adding network credentials...");
                res.ClientCredentials.UserName.UserName = this.Username;
                res.ClientCredentials.UserName.Password = this.Password;
            }
            logger.Info("END");
            return res;
        }

        public override DocsPaIntegration.Search.SearchOutputRow PuntualSearch(DocsPaIntegration.Search.PuntualSearchInfo puntualSearchInfo)
        {
            try
            {
                ExternalWSSoapClient soap = GetWSClient();
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
            catch (SearchException se)
            {
                throw se;
            }
            catch (Exception e)
            {
                logger.Error("Exception: " + e);
                throw new SearchException(SearchExceptionCode.SERVICE_UNAVAILABLE);
            }
        }

        public override DocsPaIntegration.Search.SearchOutput Search(DocsPaIntegration.Search.SearchInfo searchInfo)
        {
            try
            {
                List<SearchOutputRow> rows = new List<SearchOutputRow>();
                ExternalWSSoapClient soap = GetWSClient();
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
                logger.Error("Exception: " + e);
                throw new SearchException(SearchExceptionCode.SERVICE_UNAVAILABLE);
            }
        }


        public override Bitmap Icon
        {
            get {
                if (this.Image != null)
                {
                    MemoryStream ms=new MemoryStream(this.Image.Content);
                    Bitmap bm = new Bitmap(ms);
                    return bm;
                }
                else
                {
                    return Resources.ws_adapter_icon;
                }
            }

        }

        public override bool HasIcon
        {
            get
            {
                return true;
            }
        }
    }
}
