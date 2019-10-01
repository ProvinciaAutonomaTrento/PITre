using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Text.RegularExpressions;
using System.Configuration;
using Microsoft.Web.Services2.Dime;
using log4net;

namespace DocsPaDocumentale_WSPIA.WsPiaServices
{
 public static  class  CallWebService
    {
     private static ILog logger = LogManager.GetLogger(typeof(CallWebService));
     static string urlWsPia = ConfigurationManager.AppSettings["WSPIA_WEBSERVICE_URL"].ToString();

        public static OutputData protocollaDocumento(InputData formData)
        {
            OutputData outProto = new OutputData();

            //<ServizioProtocollo><CodiceErrore>tipologia dell’errore</CodiceErrore><DescrizioneErrore>descrizione dell’errore</DescrizioneErrore></ServizioProtocollo><ServizioProtocollo><Segnatura>INPS.0064.22/06/2010.00022222122</Segnatura></ServizioProtocollo>
            string segnaturaXmlFormat = string.Empty;
           
            //recupero la segnatura in formato xml da Web Service
            //chiamata effettiva ai Web Services.
            
            try
            {
               
                WsPia.Service1 wspia = new WsPia.Service1();
                wspia.Url = urlWsPia;
                wspia.Timeout = System.Threading.Timeout.Infinite;
                logger.Debug("(WsPia) - Inizio Chiamata al WS protocolla di WsPia");
                logger.Debug("Param input codApp = " + formData.codApp);
                logger.Debug("Param input codAmm = " + formData.codAMM);
                logger.Debug("Param input codAOO = " + formData.codA00);
                logger.Debug("Param input codUtente = " + formData.codiceUtente);
                logger.Debug("Param input xml = " + formData.xml);
                segnaturaXmlFormat = wspia.protocolla(formData.codApp, formData.codAMM, formData.codA00, formData.codiceUtente, formData.xml);
                logger.Debug("(WsPia) - Fine Chiamata al WS protocolla di WsPia");
                logger.Debug("(WsPia) - Param output segnaturaXmlFormat = " + segnaturaXmlFormat);
                

                //estraggo la segnatura o il codice di errore a seconda dei casi
                if (segnaturaXmlFormat != null && segnaturaXmlFormat.Contains("Segnatura"))
                {
                    //estraggo la segnatura in formato stringa
                    string[] esitoArray1 = Regex.Split(segnaturaXmlFormat, "<Segnatura>");
                    string[] esitoArray2 = Regex.Split(esitoArray1[1], "</Segnatura>");
                    outProto.segnatura = esitoArray2[0];
                    logger.Debug("(WsPia) - Segnatura = " + outProto.segnatura);
                }
                else
                    if (segnaturaXmlFormat != null && segnaturaXmlFormat.Contains("CodiceErrore"))
                    {
                        //estraggo il codice di errore
                        string[] erroreCodeArray1 = Regex.Split(segnaturaXmlFormat, "<CodiceErrore>");
                        string[] erroreCodeArray2 = Regex.Split(erroreCodeArray1[1], "</CodiceErrore>");
                        outProto.codiceErrore = erroreCodeArray2[0];
                        logger.Debug("(WsPia) - Codice Errore = " + outProto.codiceErrore);

                        //estraggo la descrizione dell'errore
                        string[] erroreDescrArray1 = Regex.Split(segnaturaXmlFormat, "<DescrizioneErrore>");
                        string[] erroreDescrArray2 = Regex.Split(erroreDescrArray1[1], "</DescrizioneErrore>");
                        outProto.descrizioneErrore = erroreDescrArray2[0];
                        logger.Debug("(WsPia) - Descrizione Errore = " + outProto.descrizioneErrore);
                    }
                
            }
                 
            catch (Exception ex)
            {
                logger.Debug("(WsPia) - Errore nella chiamata al metodo protocolla dei WS di INPS " + ex);
            }
            
            logger.Debug("xml di input per il metodo protocolla di INPS: "+formData.xml);
            logger.Debug("xml di output del metodo protocolla di INPS: " + segnaturaXmlFormat);
            return outProto;
        }


        public static OutputData associaImmagine(InputData formData, string pathDelFile)
        {
            OutputData outProto = new OutputData();
            
            //<ServizioProtocollo><Esito>1</Esito></ServizioProtocollo><ServizioProtocollo><CodiceErrore>tipologia dell’errore</CodiceErrore><DescrizioneErrore>descrizione dell’errore</DescrizioneErrore></ServizioProtocollo>"<ServizioProtocollo><CodiceErrore>tipologia dell’errore</CodiceErrore><DescrizioneErrore>descrizione dell’errore</DescrizioneErrore></ServizioProtocollo>";
            string esitoXmlFormat = "<ServizioProtocollo><Esito>1</Esito></ServizioProtocollo>";// string.Empty;

            //recupero la segnatura in formato xml da Web Service
            //chiamata effettiva ai Web Services...
            try
            {
                
                WsPia.Service1 wspia = new WsPia.Service1();
                wspia.Url = urlWsPia;
                DimeAttachment dimeAttach = new DimeAttachment("image/gif", TypeFormat.MediaType, @pathDelFile);
                wspia.RequestSoapContext.Attachments.Add(dimeAttach);
                wspia.Timeout = System.Threading.Timeout.Infinite;
                
                esitoXmlFormat = wspia.associaImmagine(formData.codApp, formData.codAMM, formData.codA00, formData.codiceUtente, formData.segnatura, formData.xml);
               

            //estraggo l'esito o il codice di errore a seconda dei casi
            if (esitoXmlFormat != null && esitoXmlFormat.Contains("Esito"))
            {
                //estraggo la segnatura in formato stringa
                string[] esitoArray1 = Regex.Split(esitoXmlFormat, "<Esito>");
                string[] esitoArray2 = Regex.Split(esitoArray1[1], "</Esito>");
                outProto.esito = esitoArray2[0];
            }
            else
                if (esitoXmlFormat != null && esitoXmlFormat.Contains("CodiceErrore"))
                {
                    //estraggo il codice di errore
                    string[] erroreCodeArray1 = Regex.Split(esitoXmlFormat, "<CodiceErrore>");
                    string[] erroreCodeArray2 = Regex.Split(erroreCodeArray1[1], "</CodiceErrore>");
                    outProto.codiceErrore = erroreCodeArray2[0];

                    //estraggo la descrizione dell'errore
                    string[] erroreDescrArray1 = Regex.Split(esitoXmlFormat, "<DescrizioneErrore>");
                    string[] erroreDescrArray2 = Regex.Split(erroreDescrArray1[1], "</DescrizioneErrore>");
                    outProto.descrizioneErrore = erroreDescrArray2[0];
                }

            }
            catch (Exception ex)
            {
                logger.Debug("(WsPia) - Errore nella chiamata al metodo associaImmagine dei WS di INPS, è stata sollevata la seguente eccezione: " + ex);
            }
            
            logger.Debug("XML di input per il metodo associa immagine dei Ws di INPS:"+ formData.xml);
            logger.Debug("xml di output del metodo associaImmagine di INPS: " + esitoXmlFormat);
            return outProto;
        }

        public static OutputData associaAllegato(InputData inputData, string pathDelFile)
        {
            OutputData outProto = new OutputData();

            //<ServizioProtocollo><CodiceErrore>tipologia dell’errore</CodiceErrore><DescrizioneErrore>descrizione dell’errore</DescrizioneErrore></ServizioProtocollo><ServizioProtocollo><Segnatura>INPS.0064.22/06/2010.00022222122</Segnatura></ServizioProtocollo>
            string segnaturaXmlFormat = "<ServizioProtocollo><Segnatura>INPS.0064.22/06/2010.00022227772</Segnatura></ServizioProtocollo>";// string.Empty;

            //recupero la segnatura in formato xml da Web Service
            //chiamata effettiva ai Web Services.

            try
            {
                
                WsPia.Service1 wspia = new WsPia.Service1();
                wspia.Url = urlWsPia;
                DimeAttachment dimeAttach = new DimeAttachment("image/gif", TypeFormat.MediaType, @pathDelFile);
                wspia.RequestSoapContext.Attachments.Add(dimeAttach);
                wspia.Timeout = System.Threading.Timeout.Infinite;

                segnaturaXmlFormat = wspia.associaAllegato(inputData.codApp, inputData.codAMM, inputData.codA00, inputData.codiceUtente, inputData.segnatura, inputData.xml);
                

                //estraggo la segnatura o il codice di errore a seconda dei casi
                if (segnaturaXmlFormat != null && segnaturaXmlFormat.Contains("Segnatura"))
                {
                    //estraggo la segnatura in formato stringa
                    string[] esitoArray1 = Regex.Split(segnaturaXmlFormat, "<Segnatura>");
                    string[] esitoArray2 = Regex.Split(esitoArray1[1], "</Segnatura>");
                    outProto.segnatura = esitoArray2[0];
                }
                else
                    if (segnaturaXmlFormat != null && segnaturaXmlFormat.Contains("CodiceErrore"))
                    {
                        //estraggo il codice di errore
                        string[] erroreCodeArray1 = Regex.Split(segnaturaXmlFormat, "<CodiceErrore>");
                        string[] erroreCodeArray2 = Regex.Split(erroreCodeArray1[1], "</CodiceErrore>");
                        outProto.codiceErrore = erroreCodeArray2[0];

                        //estraggo la descrizione dell'errore
                        string[] erroreDescrArray1 = Regex.Split(segnaturaXmlFormat, "<DescrizioneErrore>");
                        string[] erroreDescrArray2 = Regex.Split(erroreDescrArray1[1], "</DescrizioneErrore>");
                        outProto.descrizioneErrore = erroreDescrArray2[0];
                    }

            }

            catch (Exception ex)
            {
                logger.Debug("(WsPia) - Errore nella chiamata al metodo associaAllegato dei WS di INPS " + ex);
            }

            logger.Debug("xml di input per il metodo associaAllegato di INPS: " + inputData.xml);
            logger.Debug("xml di output del metodo associaAllegato di INPS: " + segnaturaXmlFormat);
            return outProto;
        }
    }
}
