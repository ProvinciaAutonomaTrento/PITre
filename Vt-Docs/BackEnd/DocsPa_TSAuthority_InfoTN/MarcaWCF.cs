using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Channels;
using System.ServiceModel;
using System.ServiceModel.Description;
using log4net;
using System.Xml;
using System.ServiceModel.Dispatcher;

using System.Security.Cryptography;
using DocsPaVO.areaConservazione;

namespace DocsPa_TSAuthority_InfoTN
{
    public class MarcaWCF
    {
        private static ILog logger = LogManager.GetLogger(typeof(MarcaWCF));
        /// <summary>
        /// Metodo per generare il proxy WCF senza dover configurare alcunchè nel web.config
        /// </summary>
        /// <param name="endPoindAddress"> l'url del webservice tibco</param>
        /// <returns>Il client generato</returns>
        public static  MarcaturaTemporale.MarcaturaTemporalePortTypeClient createClient(string endPoindAddress)
        {
            logger.Debug("INIZIO");
            MarcaturaTemporale.MarcaturaTemporalePortTypeClient channel = null;
            try
            {
                var binding = new CustomBinding();
                binding.Name = "MarcaturaTemporalePortTypeEndpoint2BindingMIO";
                XmlDictionaryReaderQuotas readquota = new XmlDictionaryReaderQuotas
                {
                    MaxStringContentLength = 100000000,
                    MaxArrayLength = 100000000,
                    MaxBytesPerRead = 524288
                };

                MtomMessageEncodingBindingElement mtomEconding = new MtomMessageEncodingBindingElement
                {
                    MaxReadPoolSize = 640,
                    MaxWritePoolSize = 160,
                    MessageVersion = MessageVersion.Soap12,
                    MaxBufferSize = 2147483647,
                };

                TextMessageEncodingBindingElement textEncoding = new TextMessageEncodingBindingElement
                {
                    MessageVersion = MessageVersion.Soap11,
                    MaxReadPoolSize = 640,
                    MaxWritePoolSize = 160

                };
                mtomEconding.ReaderQuotas.MaxArrayLength = 100000000;
                mtomEconding.ReaderQuotas.MaxBytesPerRead = 524288;
                mtomEconding.ReaderQuotas.MaxStringContentLength = 5242880;

                textEncoding.ReaderQuotas.MaxArrayLength = 100000000;
                textEncoding.ReaderQuotas.MaxBytesPerRead = 524288;
                textEncoding.ReaderQuotas.MaxStringContentLength = 5242880;

                HttpsTransportBindingElement httpsTransposport = new HttpsTransportBindingElement
                {
                    ManualAddressing = false,
                    MaxBufferPoolSize = 524288,
                    MaxBufferSize = 2147483647,
                    MaxReceivedMessageSize = 2147483647,
                    AllowCookies = false,
                    AuthenticationScheme = System.Net.AuthenticationSchemes.Anonymous,
                    BypassProxyOnLocal = false,
                    HostNameComparisonMode = System.ServiceModel.HostNameComparisonMode.StrongWildcard,
                    KeepAliveEnabled = true,
                    ProxyAuthenticationScheme = System.Net.AuthenticationSchemes.Anonymous,
                    Realm = "",
                    TransferMode = System.ServiceModel.TransferMode.Buffered,
                    UnsafeConnectionNtlmAuthentication = false,
                    UseDefaultWebProxy = false,
                    RequireClientCertificate = true

                };

                //binding.Elements.Add(mtomEconding);
                binding.Elements.Add(textEncoding);
                binding.Elements.Add(httpsTransposport);
                EndpointAddress epAddre = new EndpointAddress(endPoindAddress);
                ContractDescription cd = new ContractDescription("MarcaturaTemporale.MarcaturaTemporalePortType");
                channel = new MarcaturaTemporale.MarcaturaTemporalePortTypeClient(binding, epAddre);
                string certFindData = "client.marca";
                channel.ClientCredentials.ClientCertificate.SetCertificate(System.Security.Cryptography.X509Certificates.StoreLocation.LocalMachine, System.Security.Cryptography.X509Certificates.StoreName.My, System.Security.Cryptography.X509Certificates.X509FindType.FindBySubjectName, certFindData);
                channel.Endpoint.Behaviors.Add(new MaxFaultSizeBehavior(2147483647));
            }
            catch (Exception e)
            {
                //Console.WriteLine("createClient error :" + e.Message + e.StackTrace);
                logger.Error("createClient error :" + e.Message + e.StackTrace);
            }
            return channel;
        }

        /// <summary>
        /// Funzione di generazione della marca temporale light, invio al servizio tibco solo l'hash a SHA256 del file da marcare. 
        /// </summary>
        /// <param name="inMarca">Struttura contente le informazioni per la generazione della marca</param>
        /// <param name="svcUrl">Url del webservice tibco della marca temporale</param>
        /// <returns></returns>
        public OutputResponseMarca getMarcaByHash(InputMarca inMarca, string svcUrl)
        {
            logger.DebugFormat("INIZIO {0}  - {1}", inMarca.file_p7m, svcUrl);
            byte[] fileContent = String_To_Bytes(inMarca.file_p7m); //System.IO.File.ReadAllBytes(inMarca.file_p7m);
            SHA256Managed sha256 = new SHA256Managed();
            byte[] hash = sha256.ComputeHash(fileContent);

            string hexHash = BitConverter.ToString(hash);
            hexHash = hexHash.Replace("-", "");

            OutputResponseMarca retval = new OutputResponseMarca();
            retval.TSA = new TSARFC2253();
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls | System.Net.SecurityProtocolType.Ssl3;
            MarcaturaTemporale.MarcaturaTemporalePortTypeClient client = createClient(svcUrl);
            try
            {
                MarcaturaTemporale.MarcaType mt = client.MarcaTemporaleDaHash(hexHash);

                retval.marca = Convert.ToBase64String(mt.marca);
                retval.sernum = mt.serialNumber.ToString();
                retval.TSA.TSARFC2253Name = mt.timestampAuthority;
                retval.fhash = hexHash;
                retval.docm_date = mt.dataOraMarca.ToString();
                retval.esito = "OK";
                logger.DebugFormat("Generazione Marca tramite Hash: [{1}]  MarcaBase64 len:{3} Val [{0}] - OK  DATATA {2}", retval.marca, hexHash , retval.docm_date.ToString(),mt.marca.Length );
            }

            catch (FaultException<MarcaturaTemporale.FaultType> f)
            {
                logger.Error(String.Format("Errore {0} code {1}", f.Detail.userMessage, f.Code));
                //Console.WriteLine(String.Format("Errore {0} code {1}", f.Detail.userMessage, f.Code));
                retval.descrizioneErrore = f.Detail.userMessage;
                retval.esito = "KO";
            }
            catch (Exception e)
            {
                logger.Error(String.Format("Errore {0} code {1}", e.Message, e.Data));
                // Console.WriteLine(String.Format("Errore {0} code {1}", pe.Message, pe.Data));
                retval.descrizioneErrore = e.Message;
                retval.esito = "KO";
            }

            return retval;
        }


        /// <summary>
        /// Funzione di generazione della marca temporale. 
        /// </summary>
        /// <param name="inMarca">Struttura contente le informazioni per la generazione della marca</param>
        /// <param name="svcUrl">Url del webservice tibco della marca temporale</param>
        /// <returns></returns>
        public OutputResponseMarca getMarcaByFile(InputMarca inMarca, string svcUrl)
        {
            logger.DebugFormat("INIZIO {0}  - {1}", inMarca.file_p7m, svcUrl);
            byte[] fileContent = String_To_Bytes(inMarca.file_p7m);  //System.IO.File.ReadAllBytes(inMarca.file_p7m);
           
            OutputResponseMarca retval = new OutputResponseMarca();
            retval.TSA = new TSARFC2253();
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls | System.Net.SecurityProtocolType.Ssl3;
            MarcaturaTemporale.MarcaturaTemporalePortTypeClient client = createClient(svcUrl);

            try
            {
                MarcaturaTemporale.MarcaType mt = client.EmissioneMarcaTemporale(fileContent);
                retval.marca = Convert.ToBase64String(mt.marca);
                retval.sernum = mt.serialNumber.ToString();
                retval.TSA.TSARFC2253Name = mt.timestampAuthority;
                retval.docm_date = mt.dataOraMarca.ToString();
                retval.esito = "OK";
                logger.DebugFormat("Generazione Marca per file  MarcaBase64 len:{2} Val [{0}] DATATA {1}", retval.marca, retval.docm_date.ToString(),mt.marca.Length);


                //Per la generazione del fhash.. ma non è richiesto per il funzionamento
                SHA256Managed sha256 = new SHA256Managed();
                byte[] hash = sha256.ComputeHash(fileContent);

                string hexHash = BitConverter.ToString(hash);
                hexHash = hexHash.Replace("-", "");

                retval.fhash = hexHash;

            }
            catch (FaultException<MarcaturaTemporale.FaultType> f)
            {
                logger.Error(String.Format("Errore {0} code {1}", f.Detail.userMessage, f.Code));
                //Console.WriteLine(String.Format("Errore {0} code {1}", f.Detail.userMessage, f.Code));
                retval.descrizioneErrore = f.Detail.userMessage;
                retval.esito = "KO";
            }
            catch (Exception e)
            {
                logger.Error(String.Format("Errore {0} code {1}", e.Message, e.Data));
                // Console.WriteLine(String.Format("Errore {0} code {1}", pe.Message, pe.Data));
                retval.descrizioneErrore = e.Message;
                retval.esito = "KO";
            }


            return retval;
        }


        /// <summary>
        /// Funzione di verifica della marca temporale
        /// </summary>
        /// <param name="base64Marca">Marca temporale il formato stringa Base64</param>
        /// <param name="base64File">File marcato temporalmente in formato stringa Base64</param>
        /// <param name="svcUrl">Url del webservice tibco della marca temporale</param>
        /// <returns></returns>
        public OutputResponseMarca verificaTimeStamp(string base64Marca , string base64File ,string svcUrl)
        {
            logger.DebugFormat("INIZIO {0}", svcUrl);
            OutputResponseMarca retval = new OutputResponseMarca();
            retval.TSA = new TSARFC2253();
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls | System.Net.SecurityProtocolType.Ssl3;
            MarcaturaTemporale.MarcaturaTemporalePortTypeClient client = createClient(svcUrl);
            try
            {
                byte[] marca = Convert.FromBase64String(base64Marca);
                byte[] file = Convert.FromBase64String(base64File);

                Console.WriteLine("marca " + marca.Length+ " file " + file.Length );
                
                MarcaturaTemporale.MarcaType mt = client.VerificaMarcatura(marca, file);
                retval.marca = Convert.ToBase64String(mt.marca);
                retval.sernum = mt.serialNumber.ToString();
                retval.TSA.TSARFC2253Name = mt.timestampAuthority;
                retval.docm_date = mt.dataOraMarca.ToString();
                retval.esito = "OK";
                logger.DebugFormat ("Verifica Marca OK  DATATA {1}", retval.docm_date.ToString ());
            }
            catch (FaultException<MarcaturaTemporale.FaultType> f)
            {
                logger.Error(String.Format("Errore {0} code {1}", f.Detail.userMessage, f.Code));
                Console.WriteLine(String.Format("Errore {0} code {1}", f.Detail.userMessage, f.Code));
                retval.descrizioneErrore = f.Detail.userMessage;
                retval.esito = "KO";
            }
            catch (Exception e)
            {
                logger.Error(String.Format("Errore {0} code {1}", e.Message, e.Data));
                Console.WriteLine(String.Format("Errore {0} code {1}", e.Message, e.Data));
                retval.descrizioneErrore = e.Message;
                retval.esito = "KO";
            }
            return retval;

        }

        /// <summary>
        /// Funzione di verifica delle marche disponibili
        /// </summary>
        /// <param name="svcUrl">Url del webservice tibco della marca temporale</param>
        /// <returns>Stringa con lo status del cosumo delle marche separate da pipe</returns>
        public string getMarcheDisponibili(string svcUrl)
        {
            string retval = string.Empty;
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls | System.Net.SecurityProtocolType.Ssl3;
            MarcaturaTemporale.MarcaturaTemporalePortTypeClient client = createClient(svcUrl);
            try
            {
                MarcaturaTemporale.DisponibilitaType dm = client.VerificaDisponibilitaMarche(/*"KITMT01"*/ null);
                retval = String.Format("Disponibili :{0}|Consumate:{1}", dm.marcheDisponibili, dm.marcheConsumate);
                logger.Debug(retval);
            }

            catch (FaultException<MarcaturaTemporale.FaultType> f)
            {
                logger.Error(String.Format("Errore {0} code {1}", f.Detail.userMessage, f.Code));
                //Console.WriteLine(String.Format("Errore {0} code {1}", f.Detail.userMessage, f.Code));

            }

            catch (ProtocolException pe)
            {
                logger.Error(String.Format("Errore {0} code {1}", pe.Message, pe.Data));
                //Console.WriteLine(String.Format("Errore {0} code {1}", pe.Message, pe.Data));
            }
            catch (Exception e)
            {
                logger.Error(String.Format("Errore {0} code {1}", e.Message, e.Data));
               // Console.WriteLine(String.Format("Errore {0} code {1}", pe.Message, pe.Data));
            }


              return retval;
        }

        #region utility
        /// <summary>
        /// Convert hex string to byte_array
        /// </summary>
        /// <param name="strInput"></param>
        /// <returns></returns>
        protected byte[] String_To_Bytes(string strInput)
        {
            // i variable used to hold position in string
            int i = 0;
            // x variable used to hold byte array element position
            int x = 0;
            // allocate byte array based on half of string length
            byte[] bytes = new byte[(strInput.Length) / 2];
            // loop through the string - 2 bytes at a time converting
            //  it to decimal equivalent and store in byte array
            while (strInput.Length > i + 1)
            {
                long lngDecimal = Convert.ToInt32(strInput.Substring(i, 2), 16);
                bytes[x] = Convert.ToByte(lngDecimal);
                i = i + 2;
                ++x;
            }
            // return the finished byte array of decimal values
            return bytes;
        }
        #endregion  
    }
    #region MaxFaultSizeBehavior
    public class MaxFaultSizeBehavior : IEndpointBehavior
    {
        private int _size;

        public MaxFaultSizeBehavior(int size)
        {
            _size = size;
        }

        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            clientRuntime.MaxFaultSize = _size;
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
        }

        public void Validate(ServiceEndpoint endpoint)
        {
        }
    }
    #endregion

}
