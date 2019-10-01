using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.ServiceModel.Channels;
using System.Xml.XPath;
using System.Xml;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Xml.Linq;
using System.Web;
using System.Security.Cryptography.X509Certificates;
using log4net;

namespace WcfRouting
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "ServiceRouter" in code, svc and config file together.
    public class ServiceRouter : IServiceRouter
    {
        private ILog logger = LogManager.GetLogger(typeof(ServiceRouter));

        private void SetClientCredential(ClientCredentials credentials)
        {
            if (ConfigurationMgr.useSecurity)
            {
                credentials.ClientCertificate.SetCertificate(StoreLocation.LocalMachine,
                    StoreName.My,
                    X509FindType.FindBySubjectDistinguishedName,
                    ConfigurationMgr.clientCer);
                credentials.ServiceCertificate.SetDefaultCertificate(StoreLocation.LocalMachine,
                    StoreName.My,
                    X509FindType.FindBySubjectDistinguishedName,
                    ConfigurationMgr.serverCer);
            }
        }

        public Message MessageRoute(Message value)
        {
            logger.Debug("START");
            try
            {
                MessageVersion clientMsgVersion = value.Version;
                logger.DebugFormat("Azione: {0}", value.Headers.Action);
                string endpointAddressUrl = CreateEndpoint(value);
                if (string.IsNullOrEmpty(endpointAddressUrl))
                {
                    logger.Error("Endpoint url nullo o vuoto");
                    throw new Exception("Endpoint url nullo o vuoto");
                }
                logger.DebugFormat("Endpoint puntato: {0}", endpointAddressUrl);
                Binding bind = CreateBindingByConfig();
                Uri u = new Uri(endpointAddressUrl);
                EndpointAddress endpointAddress = new EndpointAddress(u, EndpointIdentity.CreateDnsIdentity(ConfigurationMgr.dnsIdentity));

                ChannelFactory<IServiceRouter> ch = new ChannelFactory<IServiceRouter>(bind, endpointAddress);
                SetClientCredential(ch.Credentials);
                IServiceRouter router = ch.CreateChannel();
                Message toSend = TransformInputMessage(value, EnvelopeVersion.Soap12);
                logger.Debug("Creato il messaggio da inviare");
                Message toReply = router.MessageRoute(toSend);
                logger.Debug("Ricevuto il messaggio di risposta");
                ((IChannel)router).Close();
                ch.Close();
                logger.Debug("END");
                return TransformOutputMessage(toReply, clientMsgVersion);
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Errore nel metodo MessageRoute: {0}", ex.Message);
                logger.Error(ex.ToString());
                throw new FaultException(ex.Message);
            }

        }

        private Binding CreateBindingByConfig()
        {
            Binding bind = null;
            if (ConfigurationMgr.useSecurity) bind = new WSHttpBinding(ConfigurationMgr.bindingPerPis);
            else bind = new BasicHttpBinding(ConfigurationMgr.bindingPerPis);
            return bind;
        }

        private string CreateEndpoint(Message value)
        {
            bool useLocalFile = false;
            bool UseAppMapping = false;
            String ApplicationName = null;
            string nameSpace = null;
            WebOperationContext Current = WebOperationContext.Current;
            OperationContext opcont = OperationContext.Current;
            //ricavo il namesapce

            // Modifica Lembo 24-04-2013: ricavo il nome interfaccia dall'action senza estrarre il name space
            //nameSpace = GetNameSpaceFromMessage(opcont, nameSpace);
            //if (string.IsNullOrEmpty(nameSpace))
            //{
            //    nameSpace = "http://nttdata.com/2012/Pi3";
            //}
            if (Current.IncomingRequest.Headers != null) ApplicationName = Current.IncomingRequest.Headers["APPLICATION_NAME"];

            //Costruzione dinamica del nome dell'endpoint del servizio da invocare
            string endpointConfigurationName = ConfigurationMgr.endpointConfigurationName;
            string UseLocalFileRouting_string = ConfigurationMgr.UseLocalFileRouting_string;
            string UseAppMapping_string = ConfigurationMgr.UseAppMapping_string;

            Boolean.TryParse(UseLocalFileRouting_string, out useLocalFile);
            Boolean.TryParse(UseAppMapping_string, out UseAppMapping);
            if (string.IsNullOrEmpty(ApplicationName) && UseAppMapping)
            {
                logger.Error("Header APPLICATION_NAME mancante");
                throw new Exception("Header APPLICATION_NAME mancante");
            }
            else if (!string.IsNullOrEmpty(ApplicationName) && UseAppMapping)
            {
                logger.DebugFormat("Codice applicazione: {0}", ApplicationName);
            }
            //Recupero il nome dell'interfaccia
            // Modifica Lembo 24-04-2013: ricavo il nome dell'interfaccia direttamente dall'action
            //string svcIface = value.Headers.Action.Replace(nameSpace, "");
            //var serviceInterface = new Uri(new Uri("http://tempuri"), svcIface).Segments[1].Replace("/", "");

            Uri action = new Uri(value.Headers.Action);
            var serviceInterface = action.Segments[action.Segments.Length - 2].Replace("/", "");

            endpointConfigurationName = endpointConfigurationName + serviceInterface;
            string serviceName = serviceInterface.Substring(1) + ".svc";
            string uriRoot = GetUriRoot(value, useLocalFile, UseAppMapping, ApplicationName);
            if (string.IsNullOrEmpty(uriRoot))
            {
                if (UseAppMapping)
                {
                    logger.Error("Codice APPLICATION_NAME inserito non valido per il codice amministrazione inserito");
                    throw new Exception("Codice APPLICATION_NAME inserito non valido per il codice amministrazione inserito");
                }
                else
                {
                    logger.Error("Amministrazione non configurata nel file locale");
                    throw new Exception("Amministrazione non configurata nel file locale");
                }

            }
            //endpoint Address Url
            string endpointAddressUrl = uriRoot + serviceName;

            return endpointAddressUrl;
        }


        private string GetUriRoot(Message value, bool useLocalFile, bool UseAppMapping, String ApplicationName)
        {
            string uriRoot = string.Empty;
            string codeAdm = GetCodeAdmFromMessage(value);


            if (string.IsNullOrEmpty(codeAdm))
                throw new Exception("Codice Amministrazione non trovato.");
            else
            {
                try
                {
                    if (useLocalFile)   // usa il file locale
                    {
                        if (!UseAppMapping)
                        {
                            uriRoot = LocalManager.PISRoutingManager.GetEndpoint(codeAdm);
                            if (string.IsNullOrEmpty(uriRoot))
                            {
                                logger.Error("Amministrazione non configurata nel file locale");
                                throw new Exception("Amministrazione non configurata nel file locale");
                            }
                        }
                        else
                        {
                            String[] EndPointAndApps = LocalManager.PISRoutingManager.GetEndpointAndApps(codeAdm);


                            uriRoot = EndPointAndApps.FirstOrDefault();
                            if (string.IsNullOrEmpty(uriRoot))
                            {
                                logger.Error("Amministrazione non configurata nel file locale");
                                throw new Exception("Amministrazione non configurata nel file locale");
                            }

                            List<String> Apps = EndPointAndApps.Skip(1).ToList();

                            if (!Apps.Contains(ApplicationName))
                            {
                                uriRoot = null;
                            }

                        }
                    }
                    else // usa il servizio WCF remoto
                    {
                        //if (!UseAppMapping)
                        {

                            PISRoutingServiceReference.PISRoutingServiceClient client = new PISRoutingServiceReference.PISRoutingServiceClient();
                            uriRoot = client.GetEndpoint(codeAdm);
                        }
                        //else 
                        {
                            PISRoutingServiceReference.PISRoutingServiceClient client = new PISRoutingServiceReference.PISRoutingServiceClient();
                            String[] EndPointAndApps = client.GetEndpointAndApps(codeAdm);
                            uriRoot = EndPointAndApps.FirstOrDefault();
                            List<String> Apps = EndPointAndApps.Skip(1).ToList();

                            if (!Apps.Contains(ApplicationName))
                            {
                                uriRoot = null;
                            }
                        }

                    }
                }
                catch (Exception e)
                {
                    throw new Exception("Errore: " + e.Message);
                }
            }
            return uriRoot;
        }

        private string GetNameSpaceFromMessage(OperationContext opcont, string nameSpace)
        {
            string sdoc = opcont.RequestContext.RequestMessage.ToString();
            XDocument z = XDocument.Parse(sdoc);
            var result = z.Root.Attributes().
                    Where(a => a.IsNamespaceDeclaration).
                    GroupBy(a => a.Name.Namespace == XNamespace.None ? String.Empty : a.Name.LocalName,
                            a => XNamespace.Get(a.Value)).
                    ToDictionary(g => g.Key,
                                 g => g.First());

            //prendo il namespace dei pis
            if (result.ContainsKey("pi3"))
                nameSpace = result["pi3"].NamespaceName;
            return nameSpace;
        }


        // A che serve? 
        //private    System.ServiceModel.Channels.Binding CreateBinding(string uriRoot)
        //{

        //    System.ServiceModel.BasicHttpBinding bind = null;
        //    if ((new Uri(uriRoot).Scheme.ToLowerInvariant().Equals("https")))
        //    {
        //        bind = new BasicHttpBinding("BasicHttpBinding_IServiceTarget_httpsClient");
        //        return bind;
        //    }
        //    else
        //    {
        //        return new WSHttpBinding("WsHttpCertText");
        //            //new BasicHttpBinding("BasicHttpBinding_IServiceTarget");
        //    }

        //   // return bind;
        //}


        private string GetCodeAdmFromMessage(Message message)
        {
            string retVal = string.Empty;
            string tagName = string.Empty;

            //Modifica Lembo 24-04-2013: estrazione del CodeAdm tramite query XPath
            //XmlDocument doc = new XmlDocument();
            //doc.LoadXml(message.ToString());

            ////Recupero tagName che contiene CodeAdm
            //foreach (XmlNode el in doc.GetElementsByTagName("*"))
            //{
            //    if (el.Name != null && el.Name.Contains("CodeAdm"))
            //        tagName = el.Name;
            //}

            //if (!string.IsNullOrEmpty(tagName))
            //{
            //    XmlNodeList xlst = doc.GetElementsByTagName(tagName);
            //    if (xlst.Count > 0)
            //        retVal = xlst[0].InnerText;
            //}
            XDocument z = XDocument.Parse(message.ToString());
            XPathNavigator xnav = z.CreateNavigator();
            XmlNamespaceManager manager = new XmlNamespaceManager(xnav.NameTable);
            manager.AddNamespace("pi3", "http://nttdata.com/2012/Pi3");
            manager.AddNamespace("vtd", "http://schemas.datacontract.org/2004/07/VtDocsWS.Services");
            string strExpression = "//pi3:request/vtd:CodeAdm"; //aggiungendo questo si recupera il codice app /vtd:UserName";
            XPathNavigator node = xnav.SelectSingleNode(strExpression, manager);
            retVal = node.InnerXml;

            logger.DebugFormat("Codice amministrazione: {0}", retVal);
            return retVal;
            //throw new NotImplementedException();
        }


        /// <summary>
        /// Trasforma il messaggio da inviare al servizio PIS target in modo compatibile con il binding utilizzato
        /// Il messaggio in ingresso arriva da un basic http binding (soap 1.1) il metodo lascia il messaggio in soap 1.1 o lo modific ain modo compatibile a 
        /// soap1.2 con wshttp
        /// </summary>
        /// <param name="value">Messaggio da trasformare</param>
        /// <returns>il messaggio da restituire</returns>

        private Message TransformInputMessage(Message value, System.ServiceModel.EnvelopeVersion vers)
        {
            Message toSend;
            if (vers == System.ServiceModel.EnvelopeVersion.Soap11)
            {
                toSend = value;
            }
            else
            {
                //Trasforma il messaggio da inviare al servizion target in modo compatibile con SOAP 1.2 e wsa 1.0
                toSend = Message.CreateMessage(MessageVersion.Soap12WSAddressing10, value.Headers.Action, value.GetReaderAtBodyContents());
            }


            return toSend;
        }


        private Message TransformOutputMessage(Message value, MessageVersion clientMsgVersion)
        {
            Message toSend;
            if (value.Version == clientMsgVersion)
            {
                toSend = value;
            }
            else
            {
                if (value.IsFault)
                {
                    MessageFault fault = MessageFault.CreateFault(value, 100000);

                    MessageFault fault2 = MessageFault.CreateFault(fault.Code, fault.Reason);

                    toSend = Message.CreateMessage(
                      clientMsgVersion, //versione
                      fault2,
                      value.Headers.Action);

                }
                else
                {

                    //trasforma il messaggio d restituire in una versione compatibile con il chiamante originale
                    toSend = Message.CreateMessage(clientMsgVersion, value.Headers.Action, value.GetReaderAtBodyContents());
                }
            }
            return toSend;
        }

    }
}
