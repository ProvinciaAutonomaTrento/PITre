using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Configuration;
using DocsPaVO.areaConservazione;
using DocsPaVO.utente;
using DocsPaConservazione;
using ProspettiRiepilogativi;
//using Microsoft.Web.Services3;
//using Microsoft.Web.Services3.Design;
//using Microsoft.Web.Services3.Diagnostics.Configuration;
using System.IO;
using log4net;
using System.Xml.Serialization;
using DocsPaVO.fascicolazione;
using DocsPaVO.ProfilazioneDinamicaLite;
using System.Collections.Generic;
using DocsPaVO.ricerche;
using DocsPaVO.Grid;
using System.Net;
using DocsPaVO.documento;
using DocsPaVO.filtri;

namespace DocsPaWS
{
    /// <summary>
    /// Summary description for DocsPaConservazioneWS
    /// </summary>
    [WebService(Namespace = "http://localhost")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class DocsPaConservazioneWS : System.Web.Services.WebService
    {
        private static ILog logger = LogManager.GetLogger(typeof(DocsPaConservazioneWS));
        protected static string path;
        public static string Path { get { return path; } }

        /// <summary>
        /// </summary>
        public DocsPaConservazioneWS()
        {
            path = this.Server.MapPath("");
            //InitializeComponent();
        }

        /// <summary>
        /// Azione di avvio dell'istanza di conservazione
        /// </summary>
        /// <param name="systemID"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        [WebMethod()]
        public void MettiInLavorazioneAsync(InfoUtente infoUtente, string idConservazione, string noteIstanza, int copieSupportiRimovibili)
        {
            //nuova gestione dei messaggi di errore **********************************
            esitoCons esito = new esitoCons();
            bool result = false;
            esito.esito = result;

            //L'assegnazione di questo path serve solo nel caso in cui non sia configurato o ci sia un errore
            //nel path scritto nel web.config
            //modifica Lembo 16-11-2012: uso il metodo di DocsPaConsManager per prelevare il root path.
            //string root_path = @"C:\Sviluppo\docspa.etnoteam.it\DocsPA30\Conservazione";
            //if (ConfigurationManager.AppSettings["CONSERVAZIONE_ROOT_PATH"] != null)
            //{
            //    root_path = ConfigurationManager.AppSettings["CONSERVAZIONE_ROOT_PATH"].ToString();
            //}

            string root_path = DocsPaConsManager.getConservazioneRootPath();
            if (string.IsNullOrEmpty(root_path)) root_path = @"C:\Sviluppo\docspa.etnoteam.it\DocsPA30\Conservazione";

            ////prefisso del file Xml se non fosse configurato
            //string XmlName = "CONSERVAZIONE_";
            //if (ConfigurationManager.AppSettings["XML_PREFIX_NAME"] != null)
            //{
            //    XmlName = ConfigurationManager.AppSettings["XML_PREFIX_NAME"].ToString();
            //}
            
            string ReadmePathWS = Server.MapPath("Readme");
            string ReadmePath = ConfigurationManager.AppSettings["CONSERVAZIONE_README_PATH"];

            DocsPaVO.Logger.CodAzione.Esito logResponse = DocsPaVO.Logger.CodAzione.Esito.OK;

            BusinessLogic.UserLog.UserLog.WriteLog(
                  infoUtente,
                 "ACCETTAZIONE_ISTANZA",
                 idConservazione,
                 String.Format("Accettazione e passaggio in lavorazione dell’istanza {0} ", idConservazione),
                 logResponse);

            //creo un oggetto di tipo registroCons per inserire l'operazione  "Accettazione e passaggio in lavorazione" sul registro
            DocsPaVO.Conservazione.RegistroCons regCons = new DocsPaVO.Conservazione.RegistroCons();
            regCons.idAmm = infoUtente.idAmministrazione;
            regCons.idIstanza = idConservazione;
            regCons.tipoOggetto = "I";
            regCons.tipoAzione = "";
            regCons.userId = infoUtente.userId;
            regCons.codAzione = "ACCETTAZIONE_ISTANZA";
            regCons.descAzione = "Accettazione e passaggio in lavorazione dell’istanza " + idConservazione;
            regCons.esito = "1";
            RegistroConservazione rc = new RegistroConservazione();
            rc.inserimentoInRegistroCons(regCons, infoUtente);

            FileManager.MettinInLavorazioneAsync(root_path, string.Empty, idConservazione, ReadmePath, ReadmePathWS, infoUtente, noteIstanza, copieSupportiRimovibili);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idConservazione"></param>
        /// <returns></returns>
        [WebMethod()]
        public bool IsInPreparazioneAsync( string idConservazione)
        {
            return FileManager.IsInPreparazioneAsync(idConservazione);
        }

        /// <summary>
        /// Avvia la creazione della folder della relativa istanza di conservazione passata come parametro
        /// </summary>
        /// <param name="systemID"></param>
        /// <returns></returns>
        [WebMethod]
        public esitoCons avviaConservazione(string systemID, InfoUtente infoUtente)
        {
             //nuova gestione dei messaggi di errore **********************************
            esitoCons esito = new esitoCons();
            bool result = false;
            esito.esito = result;
            
            //Se l'oggetto infoUtente non è valorizzato non eseguo alcuna operazione e restituisco false
            if (infoUtente == null)
            {
                logger.Debug("InfoUtente nullo o non valido.");
                //nuova gestione dei messaggi di errore **********************************
                esito.messaggio = "InfoUtente nullo o non valido.";
                esito.esito = false;
                return esito;
            }

            //L'assegnazione di questo path serve solo nel caso in cui non sia configurato o ci sia un errore
            //nel path scritto nel web.config
            //modifica Lembo 16-11-2012: uso il metodo di DocsPaConsManager per prelevare il root path.
            //string root_path = @"C:\Sviluppo\docspa.etnoteam.it\DocsPA30\Conservazione";
            //if (ConfigurationManager.AppSettings["CONSERVAZIONE_ROOT_PATH"] != null)
            //{
            //    root_path = ConfigurationManager.AppSettings["CONSERVAZIONE_ROOT_PATH"].ToString();
            //}

            string root_path = DocsPaConsManager.getConservazioneRootPath();
            if (string.IsNullOrEmpty(root_path)) root_path = @"C:\Sviluppo\docspa.etnoteam.it\DocsPA30\Conservazione";

            ////prefisso del file Xml se non fosse configurato
            //string XmlName = "CONSERVAZIONE_";
            //if (ConfigurationManager.AppSettings["XML_PREFIX_NAME"] != null)
            //{
            //    XmlName = ConfigurationManager.AppSettings["XML_PREFIX_NAME"].ToString();
            //}
            string ReadmePathWS = Server.MapPath("Readme");
            string ReadmePath = ConfigurationManager.AppSettings["CONSERVAZIONE_README_PATH"];
            FileManager fm = new FileManager();
            //nuova gestione dei messaggi di errore **********************************
            esito = fm.MettinInLavorazione(root_path, string.Empty, systemID, ReadmePath, ReadmePathWS, infoUtente);

            
          
          return esito;            
        }

        /// <summary>
        /// Restituisce tutte le istanze di conservazione in stato inviato
        /// </summary>
        /// <returns></returns>
        [WebMethod]
        public InfoConservazione[] getAllInfoConservazione(InfoUtente infoUtente)
        {
            //Se l'oggetto infoUtente non è valorizzato non eseguo alcuna operazione e restituisco false
            if (infoUtente == null)
            {
                logger.Debug("InfoUtente nullo o non valido.");
                return null;
            }

            DocsPaConsManager consManager = new DocsPaConsManager();
            return consManager.getInfoConservazione("");
        }

        /// <summary>
        /// Restituisce l'istanza di conservazione il cui id è passato come parametro
        /// </summary>
        /// <param name="systemID"></param>
        /// <returns></returns>
        [WebMethod]
        public InfoConservazione getInfoConservazione(string systemID, InfoUtente infoUtente)
        {
            //Se l'oggetto infoUtente non è valorizzato non eseguo alcuna operazione e restituisco false
            if (infoUtente == null)
            {
                logger.Debug("InfoUtente nullo o non valido.");
                return null;
            }
            
            DocsPaConsManager consManager = new DocsPaConsManager();
            InfoConservazione[] infoConsResult = consManager.getInfoConservazione(systemID);
            return infoConsResult[0];
        }

        /// <summary>
        /// Restituisce gli items associati alla specifica istanza di conservazione passata come parametro
        /// </summary>
        /// <param name="IdConservazione"></param>
        /// <returns></returns>
        [WebMethod]
        public ItemsConservazione[] getItemsConservazione(string IdConservazione, InfoUtente infoUtente)
        {
            //Se l'oggetto infoUtente non è valorizzato non eseguo alcuna operazione e restituisco false
            if (infoUtente == null)
            {
                logger.Debug("InfoUtente nullo o non valido.");
                return null;
            }

            DocsPaConsManager consManager = new DocsPaConsManager();
            return consManager.getItemsConservazioneById(IdConservazione, infoUtente);
        }

        [WebMethod]
        public ItemsConservazione[] getItemsConservazioneWithSecurity(string IdConservazione, InfoUtente infoUtente, string idGruppo)
        {
            if (string.IsNullOrEmpty(idGruppo)) 
            {
                logger.Debug("idGruppo nullo o non valido.");
                return null;
            }

            //Se l'oggetto infoUtente non è valorizzato non eseguo alcuna operazione e restituisco false
            if (infoUtente == null)
            {
                logger.Debug("InfoUtente nullo o non valido.");
                return null;
            }

            DocsPaConsManager consManager = new DocsPaConsManager();
            return consManager.getItemsConservazioneByIdWithSecurity(IdConservazione, infoUtente, idGruppo);
        }

        /// <summary>
        /// Validazione di un'istanza di conservazione
        /// </summary>
        /// <param name="idConservazione"></param>
        /// <returns></returns>
        [WebMethod()]
        public AreaConservazioneValidationResult validateIstanzaConservazione(string idConservazione, InfoUtente infoUtente)
        {
            //Se l'oggetto infoUtente non è valorizzato non eseguo alcuna operazione e restituisco false
            if (infoUtente == null)
            {
                logger.Debug("InfoUtente nullo o non valido.");
                return null;
            }

            DocsPaConsManager consManager = new DocsPaConsManager();
            return consManager.validateIstanzaConservazione(idConservazione);
        }

        /// <summary>
        /// Questo metodo restituisce le istanze di conservazione in base al filtro passato in input
        /// </summary>
        /// <param name="filtro">filtro di ricerca comprensivo della clausola where</param>
        /// <returns></returns>
        [WebMethod]
        public InfoConservazione[] RicercaInfoConservazione(string filtro, InfoUtente infoUtente)
        {
            //Se l'oggetto infoUtente non è valorizzato non eseguo alcuna operazione e restituisco false
            if (infoUtente == null)
            {
                logger.Debug("InfoUtente nullo o non valido.");
                return null;
            }

            DocsPaConsManager consManager = new DocsPaConsManager();
            return consManager.RicercaInfoConservazione(filtro);
        }


        # region metodi aggiunti per ricerca e firma stampe registro di conservazione

        [WebMethod]
        public StampaConservazione[] RicercaStampaConservazione(FiltroRicerca[] filters, InfoUtente infoUtente)
        {
            
            //Se l'oggetto infoUtente non è valorizzato non eseguo alcuna operazione e restituisco false
            if (infoUtente == null)
            {
                logger.Debug("Infoutente nullo o non valido.");
                return null;
            }
            
            //ricavo la lista delle stampe da visualizzare
            DocsPaConsManager consManager = new DocsPaConsManager();
            StampaConservazione[] elencoStampe = consManager.RicercaStampaConservazione(filters);
            
            return elencoStampe;

        }
        
        # endregion


        /// <summary>
        /// Questo metodo restituisce l'elenco dei supporti in base al filtro passato in input
        /// </summary>
        /// <param name="filtro">filtro di ricerca privo di clausola where</param>
        /// <returns></returns>
        [WebMethod]
        public InfoSupporto[] RicercaInfoSupporto(string filtro, InfoUtente infoUtente)
        {
            //Se l'oggetto infoUtente non è valorizzato non eseguo alcuna operazione e restituisco false
            if (infoUtente == null)
            {
                logger.Debug("InfoUtente nullo o non valido.");
                return null;
            }
            
            DocsPaConsManager consManager = new DocsPaConsManager();
            return consManager.RicercaInfoSupporto(filtro);
        }
        
        /// <summary>
        /// Questo metodo inserisce i dati nel DB di info supporto in base ai valori passati come input
        /// </summary>
        /// <param name="values">comprendono la stringa SQL priva solo della parola chiave INSERT e nome tabella</param>
        /// <returns></returns>
        [WebMethod]
        public bool InsertInfoSupporto(string values, InfoUtente infoUtente)
        {
            //Se l'oggetto infoUtente non è valorizzato non eseguo alcuna operazione e restituisco false
            if (infoUtente == null)
            {
                logger.Debug("InfoUtente nullo o non valido.");
                return false;
            }

            DocsPaConsManager consManager = new DocsPaConsManager();
            return consManager.InsertInfoSupporto(values);
        }
        
        /// <summary>
        /// Aggiorna info supporto a partire dalla parola chiave SET
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        [WebMethod]
        public bool UpdateInfoSupporto(string values, InfoUtente infoUtente)
        {
            //Se l'oggetto infoUtente non è valorizzato non eseguo alcuna operazione e restituisco false
            if (infoUtente == null)
            {
                logger.Debug("InfoUtente nullo o non valido.");
                return false;
            }
            
            DocsPaConsManager consManager = new DocsPaConsManager();
            return consManager.UpdateInfoSupporto(values);
        }

        /// <summary>
        /// Aggiorna info conservazione a partire dalla parola chiave SET
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        [WebMethod]
        public bool UpdateInfoConservazione(string values, InfoUtente infoUtente)
        {
            //Se l'oggetto infoUtente non è valorizzato non eseguo alcuna operazione e restituisco false
            if (infoUtente == null)
            {
                logger.Debug("InfoUtente nullo o non valido.");
                return false;
            }

            DocsPaConsManager consManager = new DocsPaConsManager();
            return consManager.UpdateInfoConservazione(values);
        }

        /// <summary>
        /// Aggiorna o inserisce in info supporto per mezzo della store procedure
        /// </summary>
        /// <param name="copia"></param>
        /// <param name="collFisica"></param>
        /// <param name="dataUltimaVer"></param>
        /// <param name="dataEliminazione"></param>
        /// <param name="esitoUltimaVer"></param>
        /// <param name="numeroVer"></param>
        /// <param name="dataProxVer"></param>
        /// <param name="dataAppoMarca"></param>
        /// <param name="dataScadMarca"></param>
        /// <param name="marca"></param>
        /// <param name="idCons"></param>
        /// <param name="tipoSupp"></param>
        /// <param name="stato"></param>
        /// <param name="note"></param>
        /// <param name="query"></param>
        /// <param name="idSupp"></param>
        /// <returns></returns>
        [WebMethod]
        public int SetDpaSupporto(string copia, string collFisica, string dataUltimaVer, string dataEliminazione, string esitoUltimaVer, string numeroVer, string dataProxVer, string dataAppoMarca, string dataScadMarca, string marca, string idCons, string tipoSupp, string stato, string note, string query, string idSupp, string percVerifica, InfoUtente infoUtente, string progressivoMarca, out int newId)
        {
            newId = 0;
            //Se l'oggetto infoUtente non è valorizzato non eseguo alcuna operazione e restituisco false
            if (infoUtente == null)
            {
                logger.Debug("InfoUtente nullo o non valido.");
                return -1;
            }

            DocsPaConsManager consManager = new DocsPaConsManager();
            return consManager.SetDpaSupporto(copia, collFisica, dataUltimaVer, dataEliminazione, esitoUltimaVer, numeroVer, dataProxVer, dataAppoMarca, dataScadMarca, marca, idCons, tipoSupp, stato, note, query, idSupp, percVerifica, progressivoMarca, out newId);
        }

        #region upload_download
        /// <summary>
        /// Download del file xml da firmare
        /// </summary>
        /// <param name="systemID"></param>
        /// <returns></returns>
        [WebMethod]
        public byte[] downloadSignedXml(string systemID, InfoUtente infoUtente)
        {
            byte[] result = null;

            //Se l'oggetto infoUtente non è valorizzato non eseguo alcuna operazione e restituisco false
            if (infoUtente == null)
            {
                logger.Debug("InfoUtente nullo o non valido.");
                return null;
            }

            FileManager fm = new FileManager();
            result = fm.downloadSignXml(systemID, infoUtente);

            return result;
        }

        ///// <summary>
        ///// Upload del file xml firmato
        ///// </summary>
        ///// <param name="systemID">id dell'istanza di conservazione</param>
        ///// <param name="signedContent"></param>
        ///// <returns></returns>
        //[WebMethod]
        //public string uploadSignedXml(string systemID, byte[] signedContent, DocsPaVO.utente.InfoUtente utente)
        //{
        //    return uploadSignedXml(systemID, signedContent, utente, false);
        //}

        /// <summary>
        /// Upload del file xml firmato
        /// </summary>
        /// <param name="systemID">id dell'istanza di conservazione</param>
        /// <param name="signedContent"></param>
        /// <param name="fromApplet">Se true proviene da applet, se false da smartclient</param>
        /// <returns></returns>
        [WebMethod]
        public string uploadSignedXml(string systemID, byte[] signedContent, DocsPaVO.utente.InfoUtente utente, bool fromApplet)
        {
           
            string result = "-1";

            //Se l'oggetto infoUtente non è valorizzato non eseguo alcuna operazione e restituisco false
            if (utente == null)
            {
                logger.Debug("InfoUtente nullo o non valido.");
                return "-1";
            }

            if (!fromApplet)
            {
                //Normalizzazione del content in base64
                System.Text.ASCIIEncoding ae = new System.Text.ASCIIEncoding();
                string base64content = ae.GetString(signedContent);
                signedContent = Convert.FromBase64String(base64content);
            }

            FileManager fm = new FileManager();

            #region Timestamp
            string enableTSA = "0";
            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["ENABLE_TIMESTAMP"]))
            {
                enableTSA = ConfigurationManager.AppSettings["ENABLE_TIMESTAMP"].ToString();
            }
            string base64TSR = string.Empty;

            if (enableTSA == "1")
            {
                try
                {
                    TimestampManager tm = new TimestampManager();
                    //marcatura.OutputResponseMarca outMarca = new DocsPaWS.marcatura.OutputResponseMarca();
                    OutputResponseMarca outMarca = new OutputResponseMarca();
                    //marcatura.InputMarca inputMarca = new DocsPaWS.marcatura.InputMarca();
                    InputMarca inputMarca = new InputMarca();
                    //marcatura.marcatura Marca = new DocsPaWS.marcatura.marcatura();
                    DocsPaMarcaturaWS Marca = new DocsPaMarcaturaWS();
                    inputMarca.applicazione = ConfigurationManager.AppSettings["APP_CHIAMANTE_TIMESTAMP"];
                    inputMarca.riferimento = utente.userId;
                    //inputMarca.riferimento = "MASSARI";
                    //Recupero la conversione in stringa Hex del file Xml firmato
                    if (signedContent != null)
                    {
                        if (signedContent.Length > 0)
                        {
                            inputMarca.file_p7m = tm.getSignedXmlHex(signedContent);
                        }
                        else
                        {
                            result = "-1";
                        }
                    }
                    else
                    {
                        result = "-1";
                    }

                    //outMarca = Marca.getTSR(inputMarca);
                    outMarca = Marca.getTSR(inputMarca, utente);
                    base64TSR = outMarca.marca;

                    //Nel caso in cui non si ottenga un contenuto valido per generare il file TSR restituisco 0
                    if (string.IsNullOrEmpty(base64TSR))
                    {
                        result = "0";
                        writeAppLog(utente, systemID, "MARCA_ISTANZA", "Apposizione della marca temporale all’indice di conservazione dell’istanza " + systemID, false);

                        // Modifica scrittura Registro conservazione per generazione Marca 
                        DocsPaVO.Conservazione.RegistroCons regConsMarca = new DocsPaVO.Conservazione.RegistroCons();
                        regConsMarca.idAmm = utente.idAmministrazione;
                        regConsMarca.idIstanza = systemID;
                        regConsMarca.tipoOggetto = "I";
                        regConsMarca.tipoAzione = "";
                        regConsMarca.userId = utente.userId;
                        regConsMarca.codAzione = "MARCA_ISTANZA";
                        regConsMarca.descAzione = "Apposizione della marca temporale all’indice di conservazione dell’istanza  " + systemID;
                        regConsMarca.esito = "0";
                        RegistroConservazione rcMarca = new RegistroConservazione();
                        rcMarca.inserimentoInRegistroCons(regConsMarca, utente);


                    }
                    else
                    {
                        //Scrivo solo nel Debugger l'esito dell'operazione di Timestamping e l'eventuale descrizione errore
                        bool appo = tm.updateTimeStamp(outMarca.marca, outMarca.docm, outMarca.dsm, systemID, "");
                        logger.Debug(outMarca.esito + " : " + outMarca.descrizioneErrore + " Aggiornamento dati marca su DB: " + appo.ToString());
                        writeAppLog(utente, systemID, "MARCA_ISTANZA", "Apposizione della marca temporale all’indice di conservazione dell’istanza " + systemID, true);
                        result = "1";

                        // Modifica scrittura Registro conservazione per generazione Marca 
                        DocsPaVO.Conservazione.RegistroCons regConsMarca = new DocsPaVO.Conservazione.RegistroCons();
                        regConsMarca.idAmm = utente.idAmministrazione;
                        regConsMarca.idIstanza = systemID;
                        regConsMarca.tipoOggetto = "I";
                        regConsMarca.tipoAzione = "";
                        regConsMarca.userId = utente.userId;
                        regConsMarca.codAzione = "MARCA_ISTANZA";
                        regConsMarca.descAzione = "Apposizione della marca temporale all’indice di conservazione dell’istanza " + systemID;
                        regConsMarca.esito = "1";
                        RegistroConservazione rcMarca = new RegistroConservazione();
                        rcMarca.inserimentoInRegistroCons(regConsMarca, utente);

                    }

                }
                catch (Exception eTm)
                {
                    logger.Debug("Errore nel reperimento della marca temporale: " + eTm);
                    result = "0";
                }
            }
            #endregion

            bool check = fm.uploadSignXml(systemID, signedContent, base64TSR, "", utente);
            if (check)
            {
                writeAppLog(utente, systemID, "FIRMA_ISTANZA", "Generazione file firmato OK per l'istanza :" + systemID, true);
                result = "1";

                // Modifica Registro di Conservazione per la Firma Istanza
                DocsPaVO.Conservazione.RegistroCons regCons = new DocsPaVO.Conservazione.RegistroCons();
                regCons.idAmm = utente.idAmministrazione;
                regCons.idIstanza = systemID;
                regCons.tipoOggetto = "I";
                regCons.tipoAzione = "";
                regCons.userId = utente.userId;
                regCons.codAzione = "FIRMA_ISTANZA";
                regCons.descAzione = "Generazione file firmato OK per l'istanza: " + systemID;
                regCons.esito = "1";
                RegistroConservazione rc = new RegistroConservazione();
                rc.inserimentoInRegistroCons(regCons, utente);


                // Istanza di conservazione in stato firmata, inizio del task dei verifica integrità del supporto remoto per il passaggio allo stato "Conservata"
                // 26_10_2012 questa qui la spostiamo dopo la verifica di leggibilità.
                // this.IniziaVerificaSupportoRemoto(utente, systemID);


                DocsPaConsManager.UpdateStatoIstanzaConservazione(systemID, DocsPaConservazione.StatoIstanza.FIRMATA);

            }
            else
            {
                writeAppLog(utente, systemID, "FIRMA_ISTANZA", "Errore durante la generazione file firmato per l'istanza :" + systemID, false);

                //Modifica per Registro in Conservazione Passaggio a stato "FIRMATA"
                DocsPaVO.Conservazione.RegistroCons regCons2 = new DocsPaVO.Conservazione.RegistroCons();
                regCons2.idAmm = utente.idAmministrazione;
                regCons2.idIstanza = systemID;
                regCons2.tipoOggetto = "I";
                regCons2.tipoAzione = "";
                regCons2.userId = utente.userId;
                regCons2.codAzione = "FIRMA_ISTANZA";
                regCons2.descAzione = "Generazione file firmato KO per l'istanza: " + systemID;
                regCons2.esito = "0";
                RegistroConservazione rc2 = new RegistroConservazione();
                rc2.inserimentoInRegistroCons(regCons2, utente);


            }

            return result;
        }

        /// <summary>
        /// Contatta il servizio per rigenerare la marca temporale
        /// </summary>
        /// <param name="systemID"></param>
        /// <param name="utente"></param>
        /// <returns></returns>
        [WebMethod]
        public bool getTSR(string systemID, DocsPaVO.utente.InfoUtente utente, string progressivoMarca, string idProfileTrasmissione)
        {
            bool result = false;
             
            DocsPaConservazione.DocsPaConsManager dpaCM = new DocsPaConsManager ();
            bool estemporanea = dpaCM.IsConservazioneInterna(systemID);

            //Se l'oggetto infoUtente non è valorizzato non eseguo alcuna operazione e restituisco false
            if (utente == null)
            {
                logger.Debug("InfoUtente nullo o non valido.");
                return false;
            }

            ////L'assegnazione di questo path serve solo nel caso in cui non sia configurato o ci sia un errore
            ////nel path scritto nel web.config
            //string root_path = @"C:\Sviluppo\docspa.etnoteam.it\DocsPA30\Conservazione";
            //if (ConfigurationManager.AppSettings["CONSERVAZIONE_ROOT_PATH"] != null)
            //{
            //    root_path = ConfigurationManager.AppSettings["CONSERVAZIONE_ROOT_PATH"].ToString();
            //}

            
            ////Leggo il file P7M per rigenerare la marca
            //string rootXml = System.IO.Path.Combine(root_path, BusinessLogic.Amministrazione.AmministraManager.AmmGetInfoAmmCorrente(utente.idAmministrazione).Codice);
            //rootXml = System.IO.Path.Combine(rootXml, systemID);

            ////nuova struttura directory!!!!!!!!!!!
            //rootXml = System.IO.Path.Combine(rootXml, "chiusura");


            byte[] signedContent = null;
            if (!estemporanea)
            {
                try
                {
                    signedContent =  this.getFileFromStore(utente, systemID, @"\Chiusura\file_chiusura.XML.P7M",false); //non sul locale che tanto è per la rigenerazione
                    if (signedContent == null)
                    {
                        logger.Debug("Errore nella lettura del file P7M ritorno null");
                        return false;
                    }
                }
                catch (Exception eSign)
                {
                    logger.Debug("Errore nella lettura del file P7M: " + eSign.Message);
                    return false;
                }
            }
            else
            {
                signedContent = dpaCM.getUniSincroP7MFromDB(systemID);
            }

            FileManager fm = new FileManager();

            #region Timestamp
            string enableTSA = "0";
            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["ENABLE_TIMESTAMP"]))
            {
                enableTSA = ConfigurationManager.AppSettings["ENABLE_TIMESTAMP"].ToString();
            }
            string base64TSR = string.Empty;
            if (enableTSA == "1")
            {
                try
                {
                    TimestampManager tm = new TimestampManager();
                    //marcatura.OutputResponseMarca outMarca = new DocsPaWS.marcatura.OutputResponseMarca();
                    OutputResponseMarca outMarca = new OutputResponseMarca();
                    //marcatura.InputMarca inputMarca = new DocsPaWS.marcatura.InputMarca();
                    InputMarca inputMarca = new InputMarca();
                    //marcatura.marcatura Marca = new DocsPaWS.marcatura.marcatura();
                    DocsPaMarcaturaWS Marca = new DocsPaMarcaturaWS();
                    inputMarca.applicazione = ConfigurationManager.AppSettings["APP_CHIAMANTE_TIMESTAMP"];
                    inputMarca.riferimento = utente.userId;
                    //inputMarca.riferimento = "MASSARI";
                    //Recupero la conversione in stringa Hex del file Xml firmato
                    inputMarca.file_p7m = tm.getSignedXmlHex(signedContent);
                    //outMarca = Marca.getTSR(inputMarca);
                    outMarca = Marca.getTSR(inputMarca, utente);
                    base64TSR = outMarca.marca;
                    bool appo = tm.updateTimeStamp(outMarca.marca, outMarca.docm, outMarca.dsm, systemID, progressivoMarca);
                    logger.Debug(outMarca.esito + " : " + outMarca.descrizioneErrore + " Aggiornamento dati marca su DB: " + appo.ToString());
                }
                catch (Exception eTm)
                {
                    logger.Debug("Errore nel reperimento della marca temporale: " + eTm);
                    
                }
            }
            #endregion
            if (!estemporanea)
            {
                result = fm.rigeneraMarca(string.Empty, systemID, signedContent, base64TSR, progressivoMarca, utente);

                if (result && !string.IsNullOrEmpty(progressivoMarca))
                {
                    //creo il file tsr come allegato del documento principale associato alla trasmissione
                    DocsPaConsManager consManager = new DocsPaConsManager();
                    bool resultAll = consManager.createAllegato(idProfileTrasmissione, progressivoMarca, systemID, utente);
                    result = resultAll;
                }

            }
            else
            {
                dpaCM.updateUniSincroTSRInDB(systemID, base64TSR);
                result = true;
            }
            if (result)
                writeAppLog(utente, systemID, "MARCA_ISTANZA", "Rigenerazione marca OK", true);
            else
                writeAppLog(utente, systemID, "MARCA_ISTANZA", "Errore nel reperimento della marca temporale", false);

            return result;
        }

        /// <summary>
        /// Download del file zip dell'intera istanza di conservazione
        /// </summary>
        /// <param name="systemID"></param>
        /// <returns></returns>
        [WebMethod]
        public void createZipFile(string systemID, InfoUtente infoUtente)
        {
            //Se l'oggetto infoUtente non è valorizzato non eseguo alcuna operazione e restituisco false
            if (infoUtente == null)
            {
                logger.Debug("InfoUtente nullo o non valido.");
                return;
            }

            FileManager fm = new FileManager();
            fm.createZipFile(systemID);
        }
        #endregion


        /// <summary>
        /// Invio dell'istanza su storage remoto
        /// </summary>
        /// <param name="systemID"></param>
        /// <param name="infoUtente"></param>
        [WebMethod]
        public bool SubmitToRemoteFolder(string systemID, InfoUtente infoUtente)
        {
            //Se l'oggetto infoUtente non è valorizzato non eseguo alcuna operazione e restituisco false
            if (infoUtente == null)
            {
                logger.Debug("InfoUtente nullo o non valido.");
                return false;
            }

            FileManager fm = new FileManager();
            return fm.SubmitToRemoteFolder(systemID);
        }
        

        /// <summary>
        /// Metodo che trasmette la notifica dell'avvenuta conservazione
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idIstanzaCons"></param>
        /// <param name="idPeopleDest"></param>
        /// <returns></returns>
        [WebMethod]
        public bool trasmettiNotifica(DocsPaVO.utente.InfoUtente infoUtente, string idIstanzaCons, string idPeopleDest)
        {
            //L'assegnazione di questo path serve solo nel caso in cui non sia configurato o ci sia un errore
            //nel path scritto nel web.config
            //modifica Lembo 16-11-2012: uso il metodo di DocsPaConsManager per prelevare il root path.
            //string root_path = @"C:\Sviluppo\docspa.etnoteam.it\DocsPA30\Conservazione";
            //if (ConfigurationManager.AppSettings["CONSERVAZIONE_ROOT_PATH"] != null)
            //{
            //    root_path = ConfigurationManager.AppSettings["CONSERVAZIONE_ROOT_PATH"].ToString();
            //}

            string root_path = DocsPaConsManager.getConservazioneRootPath();
            if (string.IsNullOrEmpty(root_path)) root_path = @"C:\Sviluppo\docspa.etnoteam.it\DocsPA30\Conservazione";

            
            DocsPaConsManager consManager = new DocsPaConsManager();
            root_path = System.IO.Path.Combine(root_path, infoUtente.idAmministrazione);

            bool retval = consManager.TrasmettiNotifica(infoUtente, idIstanzaCons);

            return retval;
        }

        [WebMethod]
        public bool trasmettiNotificaRifiuto(DocsPaVO.utente.InfoUtente infoUtente, string idIstanzaCons, string noteRifiuto)
        {
            DocsPaConsManager consManager = new DocsPaConsManager();
            bool retval;
            retval =    consManager.TrasmettiNotificaRifiuto(infoUtente, idIstanzaCons, noteRifiuto);


            if (retval)
            {
                writeAppLog(infoUtente, idIstanzaCons, "RIFIUTO_ISTANZA", "Rifiuto istanza ID " + idIstanzaCons + ": " + noteRifiuto, true);

                //MODIFICA GESTIONE REGISTRO DI CONSERVAZIONE
                //creo un oggetto di tipo registroCons per inserire l'operazione  "Rifiuto istanza ID" sul registro
                DocsPaVO.Conservazione.RegistroCons regCons = new DocsPaVO.Conservazione.RegistroCons();
                regCons.idAmm = infoUtente.idAmministrazione;
                regCons.idIstanza = idIstanzaCons;
                regCons.tipoOggetto = "I"; //GM 13-09-2013 modifica tipo oggetto
                regCons.tipoAzione = "";
                regCons.userId = infoUtente.userId;
                regCons.codAzione = "RIFIUTO_ISTANZA";
                regCons.descAzione = "Rifiuto Istanza " + idIstanzaCons + " - Note rifiuto: "+ noteRifiuto;
                regCons.esito = "1";
                RegistroConservazione rc = new RegistroConservazione();
                rc.inserimentoInRegistroCons(regCons, infoUtente);


            }
            else
            {
                //MODIFICA GESTIONE REGISTRO DI CONSERVAZIONE
                //creo un oggetto di tipo registroCons per inserire l'operazione  "Rifiuto istanza ID" sul registro
                DocsPaVO.Conservazione.RegistroCons regCons = new DocsPaVO.Conservazione.RegistroCons();
                regCons.idAmm = infoUtente.idAmministrazione;
                regCons.idIstanza = idIstanzaCons;
                regCons.tipoOggetto = "D";
                regCons.tipoAzione = "";
                regCons.userId = infoUtente.userId;
                regCons.codAzione = "RIFIUTO_ISTANZA";
                regCons.descAzione = "Rifiuto Istanza" + idIstanzaCons + "Note rifiuto: " + noteRifiuto;
                regCons.esito = "0";
                RegistroConservazione rc = new RegistroConservazione();
                rc.inserimentoInRegistroCons(regCons, infoUtente);

                writeAppLog(infoUtente, idIstanzaCons, "RIFIUTO_ISTANZA", "Errore nel Rifiuto istanza ID " + idIstanzaCons + ": " + noteRifiuto, false);
            }
        
            return retval;

        }
        [WebMethod]
        public string getDbType()
        {
            string result = string.Empty;
            if (ConfigurationManager.AppSettings["DBType"] != null)
            {
                result = ConfigurationManager.AppSettings["DBType"].ToString();
            }
            return result;
        }

        [WebMethod]
        public string consXmlPrefixName()
        {
            string xmlPrefixName = string.Empty;
            try
            {
                xmlPrefixName = System.Configuration.ConfigurationManager.AppSettings["XML_PREFIX_NAME"];
                return xmlPrefixName;
            }
            catch(Exception e)
            {
                logger.Debug("Errore nel servizio di conservazione (metodo consXmlPrefixName):" + e.Message);
                return xmlPrefixName;
            }
        }

        [WebMethod]
        public string calcolaImprontaFile(DocsPaVO.utente.InfoUtente utente, byte[] content,bool sha256)
        {
            string impronta = string.Empty;            
            try
            {
                //DocsPaVO.documento.SchedaDocumento sch = BusinessLogic.Documenti.DocManager.getDettaglio(utente, null, docNumber);
                //DocsPaVO.documento.SchedaDocumento sch = BusinessLogic.Documenti.DocManager.getDettaglioNoSecurity(utente, docNumber);
                //if (sch.documenti != null && sch.documenti[0] != null)
                //{
                //    DocsPaVO.documento.FileRequest fr = (DocsPaVO.documento.FileRequest)sch.documenti[0];                   
                //    DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
                //    DocsPaVO.documento.FileDocumento fileDoc = BusinessLogic.Documenti.FileManager.getFile(fr);
                if (sha256)
                {
                    impronta = DocsPaUtils.Security.CryptographyManager.CalcolaImpronta256(content);
                }
                else
                {
                    impronta = DocsPaUtils.Security.CryptographyManager.CalcolaImpronta(content);
                }
                   // doc.GetImpronta(out impronta, fr.versionId, fr.docNumber);
                //}
                //else
                //{
                //    impronta = string.Empty;
                //}
            }
            catch (Exception ex)
            {
                logger.Debug("Errore nel servizio di conservazione (metodo calcolaImprontaFile):" + ex.Message);
                impronta = "";
            }
            return impronta;
        }

        [WebMethod]
        public int verificaFirma(string path, DocsPaVO.utente.InfoUtente utente, string idIstanza)
        {
            //DocsPaConsManager consManager = new DocsPaConsManager();
            //string docNumberTrasm = consManager.getDocNumberTrasmissione(idConservazione);
            try
            {
                //DocsPaVO.documento.SchedaDocumento sch = BusinessLogic.Documenti.DocManager.getDettaglioNoSecurity(utente, docNumberTrasm);
                //ArrayList allegati = sch.allegati;
                //DocsPaVO.documento.FileRequest fileP7m = new DocsPaVO.documento.FileRequest();
                //for (int i = 0; i < allegati.Count; i++)
                //{
                //    if (((DocsPaVO.documento.Allegato)sch.allegati[i]).firmato == "1")
                //        fileP7m = (DocsPaVO.documento.FileRequest)sch.allegati[i];
                //}
                BusinessLogic.Documenti.DigitalSignature.VerifySignature vs = new BusinessLogic.Documenti.DigitalSignature.VerifySignature();
                DocsPaVO.documento.VerifySignatureResult verifica = vs.Verify(path);
                DocsPaVO.documento.PKCS7Document[] doc = verifica.PKCS7Documents;
                DocsPaVO.documento.SignerInfo[] infoSign = doc[0].SignersInfo;
                DocsPaVO.documento.CertificateInfo infoCert = infoSign[0].CertificateInfo;

                if (infoCert.RevocationStatus==0)
                    writeAppLog(utente, idIstanza, "VERIFICA_VALIDITA_FIRMA", "Firma OK: ", true);
                else
                    writeAppLog(utente, idIstanza, "VERIFICA_VALIDITA_FIRMA", "Errore nella verifica della firma", false);

      
                return infoCert.RevocationStatus;
            }
            catch (Exception ex)
            {
                logger.Debug("Errore nel servizio di conservazione (metodo verificaFirma):" + ex.Message);
                return -1;
            }
        }

        [WebMethod]
        public int verificaMarcaM7M(string path, DocsPaVO.utente.InfoUtente utente, string idIstanza)
        {
            //DocsPaConsManager consManager = new DocsPaConsManager();
            //string docNumberTrasm = consManager.getDocNumberTrasmissione(idConservazione);
            try
            {
                //DocsPaVO.documento.SchedaDocumento sch = BusinessLogic.Documenti.DocManager.getDettaglioNoSecurity(utente, docNumberTrasm);
                //ArrayList allegati = sch.allegati;
                //DocsPaVO.documento.FileRequest fileP7m = new DocsPaVO.documento.FileRequest();
                //for (int i = 0; i < allegati.Count; i++)
                //{
                //    if (((DocsPaVO.documento.Allegato)sch.allegati[i]).firmato == "1")
                //        fileP7m = (DocsPaVO.documento.FileRequest)sch.allegati[i];
                //}
                BusinessLogic.Documenti.DigitalSignature.VerifySignature vs = new BusinessLogic.Documenti.DigitalSignature.VerifySignature();
                DocsPaVO.documento.VerifySignatureResult verifica = vs.VerifyM7M(path);
                DocsPaVO.documento.PKCS7Document[] doc = verifica.PKCS7Documents;
                DocsPaVO.documento.SignerInfo[] infoSign = doc[0].SignersInfo;
                DocsPaVO.documento.CertificateInfo infoCert = infoSign[0].CertificateInfo;

                if (infoCert.RevocationStatus == 0)
                    writeAppLog(utente, idIstanza, "VERIFICA_VALIDITA_MARCA", "Marca OK: ", true);
                else
                    writeAppLog(utente, idIstanza, "VERIFICA_VALIDITA_MARCA", "Errore nella verifica della marca", false);


                return infoCert.RevocationStatus;
            }
            catch (Exception ex)
            {
                logger.Debug("Errore nel servizio di conservazione (metodo verificaFirma):" + ex.Message);
                return -1;
            }
        }


        [WebMethod]
        public string verifyMarca(byte[] fileTSR, DocsPaVO.utente.InfoUtente utente, string idIstanza)
        {
            string result = string.Empty;
            string esito = string.Empty;
            bool esi= false;
            try
            {
                DocsPaVO.areaConservazione.OutputResponseMarca marca = new OutputResponseMarca();
                BusinessLogic.Documenti.DigitalSignature.VerifyTimeStamp verifica = new BusinessLogic.Documenti.DigitalSignature.VerifyTimeStamp();
                if (verifica != null)
                {
                    marca = verifica.Verify(fileTSR);
                    esito = marca.esito;
                    if (esito == "KO" && marca.descrizioneErrore != "marca temporale scaduta")
                        result = "Verifica della marca fallita";
                    else
                        result = marca.descrizioneErrore;

                    if (esito == "OK")
                        esi = true;
                }
                else
                {
                    result = "File TSR mancante oppure corrotto!";
                }
            }
            catch (Exception e)
            {
                logger.Debug("Errore nel servizio di conservazione (metodo verifyMarca):" + e.Message);
                result = "KO";
                esi = false;
            }

            if (esi)
                writeAppLog(utente, idIstanza, "MARCA_ISTANZA", "Marca OK: ", true);
            else
                writeAppLog(utente, idIstanza, "MARCA_ISTANZA", "Errore nella verifica della marca", false);

      
            return result;
        }

        [WebMethod]
        public bool insertConsVerifica(DocsPaVO.utente.InfoUtente utente, string idSupporto, string idIstanza, string note, string percentuale, string num_ver, string esito, string tipoVerifica)
        {
            bool result = false;
            try
            {
                DocsPaConsManager consManager= new DocsPaConsManager();
                result= consManager.insertVerificaSupporto(idSupporto, idIstanza, note, percentuale, num_ver, esito, tipoVerifica);
            }
            catch (Exception e)
            {
                logger.Debug("Errore nel servizio di conservazione (metodo insertConsVerifica):" + e.Message);
            }
            return result;
        }

        [WebMethod]
        public InfoSupporto[] conservazioneGetReportVerifiche(string idConservazione, string idSupporto, DocsPaVO.utente.InfoUtente infoUtente)
        {
            if (infoUtente == null)
            {
                logger.Debug("InfoUtente nullo o non valido.");
                return null;
            }            
            try
            {             
                DocsPaConsManager consManager = new DocsPaConsManager();
                return consManager.getReportVerificheSupporto(idConservazione, idSupporto);
            }
            catch (Exception ex)
            {
                logger.Debug("Errore nel servizio di conservaizone - metodo: conservazioneGetReportVerifiche" + ex.Message);
                return null;
            }
        }

        [WebMethod]
        public void deleteDirectoryIstanzaCons(string idConservazione, DocsPaVO.utente.InfoUtente infoUtente)
        {
            if (infoUtente == null)
            {
                logger.Debug("InfoUtente nullo o non valido.");
                return;
            }
            //modifica Lembo 16-11-2012: uso il metodo di DocsPaConsManager per prelevare il root path.
            //string root_path = @"C:\Sviluppo\docspa.etnoteam.it\DocsPA30\Conservazione";
            //if (ConfigurationManager.AppSettings["CONSERVAZIONE_ROOT_PATH"] != null)
            //{
            //    root_path = ConfigurationManager.AppSettings["CONSERVAZIONE_ROOT_PATH"].ToString();
            //}

            string root_path = DocsPaConsManager.getConservazioneRootPath();
            if (string.IsNullOrEmpty(root_path)) root_path = @"C:\Sviluppo\docspa.etnoteam.it\DocsPA30\Conservazione";


            root_path = System.IO.Path.Combine(root_path, infoUtente.idAmministrazione);
            string directory = System.IO.Path.Combine(root_path, idConservazione);
            if (Directory.Exists(directory))
                Directory.Delete(directory, true);            
        }

        /// <summary>
        /// Task di verifica della valididità di firma e marca per un'istanza di conservazione
        /// </summary>
        /// <param name="idConservazione"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        [WebMethod()]
        public AreaConservazioneValidationResult ValidaFileFirmati(string idConservazione, DocsPaVO.utente.InfoUtente infoUtente, out int totale, out int valid, out int invalid)
        {
            AreaConservazioneValidationResult result = new AreaConservazioneValidationResult();

            totale = 0;
            valid = 0;
            invalid = 0;

            try
            {
                DocsPaConservazione.DocsPaConsManager consManager = new DocsPaConsManager();

                result = consManager.ValidaFileFirmati(idConservazione, infoUtente, out totale, out valid, out invalid);
                if (result.Items.Length > 0)
                {
                    writeAppLog(infoUtente, idConservazione, "VERIFICA_VALIDITA_FIRMA", String.Format("Esecuzione della verifica di validità dei file firmati per l’istanza {0}", idConservazione), false);


                }
                else
                {
                    writeAppLog(infoUtente, idConservazione, "VERIFICA_VALIDITA_FIRMA", "Esecuzione della verifica di validità dei file firmati per l’istanza " + idConservazione, true);
                    
                }

            }
            catch (Exception ex)
            {
                result = new AreaConservazioneValidationResult();
                result.IsValid = false;
                result.ErrorMessage = ex.Message;
                writeAppLog(infoUtente, idConservazione, "VERIFICA_VALIDITA_FIRMA", "Problema nel verificare la firma", false);
            }
            
            return result;
        }

        /// <summary>
        /// Task di verifica della valididità  marca per un'istanza di conservazione
        /// </summary>
        /// <param name="idConservazione"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        [WebMethod()]
        public AreaConservazioneValidationResult ValidaFormatoFile(string idConservazione, DocsPaVO.utente.InfoUtente infoUtente)
        {
            AreaConservazioneValidationResult result = new AreaConservazioneValidationResult();

            try
            {
                DocsPaConservazione.DocsPaConsManager consManager = new DocsPaConsManager();

                result = consManager.ValidaFormatoFile(idConservazione, infoUtente);

                if (result.Items.Length > 0)
                {
                    writeAppLog(infoUtente, idConservazione, "VERIFICA_INT_CONTENUTO_FILE", "I file presenti NON sono del formato accettato", false);
                }
                else
                {
                    writeAppLog(infoUtente, idConservazione, "VERIFICA_INT_CONTENUTO_FILE", "I file presenti sono del formato accettato", true);
                }
            }
            catch (Exception ex)
            {
                result = new AreaConservazioneValidationResult();
                result.IsValid = false;
                result.ErrorMessage = ex.Message;
                writeAppLog(infoUtente, idConservazione, "VERIFICA_INT_CONTENUTO_FILE", "Problema nel verificare il formato file", false);
  
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idConservazione"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        [WebMethod()]
        public DocsPaVO.areaConservazione.ItemsConservazione[] getItemsConservazioneWithContentValidation(
                                        string idConservazione,
                                        DocsPaVO.utente.InfoUtente infoUtente)
        {
            DocsPaConservazione.DocsPaConsManager consManager = new DocsPaConsManager();

            return consManager.getItemsConservazioneById(idConservazione, infoUtente, true);
        }


        /// <summary>
        /// Task di verifica della valididità  marca per un'istanza di conservazione
        /// </summary>
        /// <param name="idConservazione"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        [WebMethod()]
        public AreaConservazioneValidationResult ValidaFileMarcati(string idConservazione, DocsPaVO.utente.InfoUtente infoUtente, out int totale, out int valid, out int invalid)
        {
            AreaConservazioneValidationResult result = new AreaConservazioneValidationResult();

            totale = 0;
            valid = 0;
            invalid = 0;

            try
            {
                DocsPaConservazione.DocsPaConsManager consManager = new DocsPaConsManager();
                string testo_esito;
                result = consManager.ValidaFileMarcati(idConservazione, infoUtente, out totale, out valid, out invalid);
                //GM 13-09-2013
                //inserisco nel registro l'informazione relativa alla verifica
                //anche se non ci sono file marcati all'interno dell'istanza
                if (valid == 0 && invalid == 0)
                {
                    testo_esito = "Esecuzione della verifica di validità dei file marcati per l’istanza ";
                    writeAppLog(infoUtente, idConservazione, "VERIFICA_VALIDITA_MARCA", testo_esito + idConservazione, true);

                    DocsPaVO.Conservazione.RegistroCons regCons = new DocsPaVO.Conservazione.RegistroCons();
                    regCons.idAmm = infoUtente.idAmministrazione;
                    regCons.idIstanza = idConservazione;
                    regCons.tipoOggetto = "I";
                    regCons.tipoAzione = "";
                    regCons.userId = infoUtente.userId;
                    regCons.codAzione = "VERIFICA_VALIDITA_MARCA";
                    regCons.descAzione = "Nessun file marcato presente in istanza ID " + idConservazione;
                    regCons.esito = "1";
                    RegistroConservazione rc = new RegistroConservazione();
                    rc.inserimentoInRegistroCons(regCons, infoUtente);

                }
                else
                {
                    if (result.Items.Length == 0)
                    {
                        testo_esito = "Esecuzione della verifica di validità dei file marcati per l’istanza ";
                        writeAppLog(infoUtente, idConservazione, "VERIFICA_VALIDITA_MARCA", testo_esito + idConservazione, true);

                        DocsPaVO.Conservazione.RegistroCons regCons = new DocsPaVO.Conservazione.RegistroCons();
                        regCons.idAmm = infoUtente.idAmministrazione;
                        regCons.idIstanza = idConservazione;
                        regCons.tipoOggetto = "I";
                        regCons.tipoAzione = "";
                        regCons.userId = infoUtente.userId;
                        regCons.codAzione = "VERIFICA_VALIDITA_MARCA";
                        regCons.descAzione = "Esecuzione della verifica di validità dei file marcati in istanza ID " + idConservazione;
                        regCons.esito = "1";
                        RegistroConservazione rc = new RegistroConservazione();
                        rc.inserimentoInRegistroCons(regCons, infoUtente);


                    }
                    else
                    {
                        testo_esito = "Esecuzione della verifica di validità dei file marcati per l’istanza ";
                        writeAppLog(infoUtente, idConservazione, "VERIFICA_VALIDITA_MARCA", testo_esito + idConservazione, false);

                        DocsPaVO.Conservazione.RegistroCons regCons = new DocsPaVO.Conservazione.RegistroCons();
                        regCons.idAmm = infoUtente.idAmministrazione;
                        regCons.idIstanza = idConservazione;
                        regCons.tipoOggetto = "I";
                        regCons.tipoAzione = "";
                        regCons.userId = infoUtente.userId;
                        regCons.codAzione = "VERIFICA_VALIDITA_MARCA";
                        regCons.descAzione = "Esecuzione della verifica di validità dei file marcati in istanza ID " + idConservazione;
                        regCons.esito = "0";
                        RegistroConservazione rc = new RegistroConservazione();
                        rc.inserimentoInRegistroCons(regCons, infoUtente);


                    }

                }



            }
            catch (Exception ex)
            {
                result = new AreaConservazioneValidationResult();
                result.IsValid = false;
                result.ErrorMessage = ex.Message;
                writeAppLog(infoUtente, idConservazione, "VERIFICA_VALIDITA_MARCA", "Problema nel verificare la marca", false);

            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idConservazione"></param>
        [WebMethod()]
        public void IniziaVerificaSupportoRemoto(DocsPaVO.utente.InfoUtente infoUtente, string idConservazione)
        {
            DocsPaConservazione.VerificaSupportoRemoto.IniziaVerifica(infoUtente, idConservazione);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idConservazione"></param>
        /// <returns></returns>
        [WebMethod()]
        public DocsPaConservazione.StatoVerificaSupporto GetStatoVerificaSupportoRemoto(DocsPaVO.utente.InfoUtente infoUtente, string idConservazione)
        {
            return DocsPaConservazione.VerificaSupportoRemoto.GetStatoVerifica(infoUtente, idConservazione);
        }
  

        private bool writeAppLog(DocsPaVO.utente.InfoUtente infoUtente,string idConservazione, string operazione,string description, bool success)
        {
            DocsPaVO.Logger.CodAzione.Esito logResponse = DocsPaVO.Logger.CodAzione.Esito.KO;
            if (success)
                logResponse= DocsPaVO.Logger.CodAzione.Esito.OK;


            bool retval = BusinessLogic.UserLog.UserLog.WriteLog(infoUtente,
                    operazione,
                    idConservazione, 
                    description,
                    logResponse);

            return retval ;
            
        }

        [WebMethod]
        public DocsPaVO.areaConservazione.TipoSupporto[] GetTipiSupporto()
        {
            DocsPaVO.areaConservazione.TipoSupporto[] tipoSupp = null;
            try
            {
                DocsPaConservazione.DocsPaConsManager consManager = new DocsPaConservazione.DocsPaConsManager();
                tipoSupp = consManager.getListaTipoSupporto();
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaConservazioneWS.asmx  - metodo: getListaTipoSupporto", e);
            }
            return tipoSupp;
        }

       

        [WebMethod()]
        [XmlInclude(typeof(DocsPaVO.Conservazione.Policy))]
        public DocsPaVO.Conservazione.Policy[] GetListaPolicy(int idAmm, string tipo)
        {
            try
            {
                DocsPaVO.Conservazione.Policy[] result = null;

                result = BusinessLogic.Conservazione.Policy.PolicyManager.GetListaPolicy(idAmm, tipo);

                return result;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaConservazioneWS.asmx  - metodo: GetListaPolicy", e);

                throw e;
            }
        }

        [WebMethod]
        public bool ValidateIstanzaConservazioneConPolicy(string idPolicy, string idConservazione, DocsPaVO.utente.InfoUtente infoUtente)
        {
            bool result = false;
            try
            {
                DocsPaConservazione.DocsPaConsManager consManager = new DocsPaConservazione.DocsPaConsManager();
                result = consManager.ValidateIstanzaConservazioneConPolicy(idPolicy, idConservazione, infoUtente);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaConservazioneWS.asmx  - metodo: ValidateIstanzaConservazioneConPolicy", e);
            }
            return result;
        }

        [WebMethod]
        public bool DeleteValidateIstanzaConservazioneConPolicy(string idPolicy, string idConservazione, DocsPaVO.utente.InfoUtente infoUtente)
        {
            bool result = false;
            try
            {
                DocsPaConservazione.DocsPaConsManager consManager = new DocsPaConservazione.DocsPaConsManager();
                result = consManager.DeleteValidateIstanzaConservazioneConPolicy(idPolicy, idConservazione, infoUtente);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaConservazioneWS.asmx  - metodo: ValidateIstanzaConservazioneConPolicy", e);
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idSupporto"></param>
        /// <param name="collocazione"></param>
        /// <param name="note"></param>
        [WebMethod()]
        public void RegistraSupportoRimovibile(DocsPaVO.utente.InfoUtente infoUtente, string idIstanza, string idSupporto, string collocazione, string note)
        {
            DocsPaConservazione.DocsPaConsManager consManager = new DocsPaConservazione.DocsPaConsManager();

            consManager.RegistraSupportoRimovibile(infoUtente, idIstanza, idSupporto, collocazione, note);
        }

        /// <summary>
        /// Registrazione dell'esito della verifica di integrità di un supporto rimovibile registrato
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idIstanza"></param>
        /// <param name="idSupporto"></param>
        /// <param name="esitoVerifica"></param>
        /// <param name="dataProssimaVerifica"></param>
        [WebMethod()]
        public void RegistraEsitoVerificaSupportoRegistrato(
                                    DocsPaVO.utente.InfoUtente infoUtente,
                                    string idIstanza,
                                    string idSupporto,
                                    bool esitoVerifica,
                                    string percentualeVerifica,
                                    string dataProssimaVerifica,
                                    string noteDiVerifica,
                                    string tipoVerifica)
        {
            DocsPaConservazione.DocsPaConsManager consManager = new DocsPaConservazione.DocsPaConsManager();

            consManager.RegistraEsitoVerificaSupportoRegistrato(infoUtente, idIstanza, idSupporto, esitoVerifica, percentualeVerifica, dataProssimaVerifica, noteDiVerifica, tipoVerifica);
        }

        /// <summary>
        /// Reperimento degli stati di un supporto
        /// </summary>
        /// <returns></returns>
        [WebMethod()]
        public DocsPaConservazione.StatoSupporto[] GetStatiSupporto()
        {
            return DocsPaConservazione.StatoSupporto.Stati;
        }

        /// <summary>
        /// Reperimento dei tipi di istanze conservazione
        /// </summary>
        /// <returns></returns>
        [WebMethod()]
        public DocsPaConservazione.TipoIstanzaConservazione[] GetTipiIstanza()
        {
            return TipoIstanzaConservazione.Tipi;
        }

        /// <summary>
        /// Reperimento dei tipi di istanze conservazione
        /// </summary>
        /// <returns></returns>
        [WebMethod()]
        public DocsPaConservazione.StatoIstanza[] GetStatiIstanza()
        {
            return DocsPaConservazione.StatoIstanza.Stati;
        }

        /// <summary>
        /// Reperimento dei contatori delle istanze di conservazione per l'amministrazione
        /// </summary>
        /// <param name="infoUtente"></param>
        [WebMethod()]
        public DocsPaConservazione.Contatori GetContatori(DocsPaVO.utente.InfoUtente infoUtente)
        {
            return DocsPaConservazione.Contatori.Get(infoUtente);
        }

        /// <summary>
        /// Reperimento della policy con cui è stata eventualmente creata l'istanza di conservazione
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idIstanza"></param>
        /// <returns></returns>
        [WebMethod()]
        public DocsPaVO.Conservazione.Policy GetPolicyIstanza(DocsPaVO.utente.InfoUtente infoUtente, string idIstanza)
        {
            DocsPaConservazione.DocsPaConsManager consManager = new DocsPaConsManager();
            
            return consManager.GetPolicyByIdIstanzaConservazione(infoUtente, idIstanza);
        }       

        [WebMethod()]
        [XmlInclude(typeof(DocsPaVO.utente.Registro))]
        public DocsPaVO.utente.Registro[] GetRfByIdAmm(int idAmministrazione, string tipo)
        {
            try
            {
                DocsPaVO.utente.Registro[] result = null;

                result = BusinessLogic.Amministrazione.RegistroManager.GetRfByIdAmm(idAmministrazione, tipo);

                return result;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaConservazioneWS.asmx  - metodo: GetRfByIdAmm", e);

                throw e;
            }
        }

        [WebMethod()]
        [XmlInclude(typeof(DocsPaVO.fascicolazione.Fascicolo))]
        public DocsPaVO.fascicolazione.Fascicolo[] GetFascicoloDaCodiceNoSecurity(string codiceFasc, string idAmm, string idTitolario, bool soloGenerali, bool isRicFasc)
        {
            try
            {
                Fascicolo[] result = null;

                //result = BusinessLogic.Fascicoli.FascicoloManager.GetFascicoloDaCodiceNoSecurity(codiceFasc, idAmm, idTitolario, soloGenerali);
                result = BusinessLogic.Fascicoli.FascicoloManager.GetFascicoloDaCodiceNoSecurityConservazione(codiceFasc, idAmm, idTitolario, soloGenerali, isRicFasc);

                return result;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaConservazioneWS.asmx  - metodo: GetFascicoloDaCodiceNoSecurity", e);

                throw e;
            }
        }

        /// <summary>
        /// Get Fascicolo By Id
        /// </summary>
        /// <param name="idFascicolo"></param>
        /// <returns></returns>
        [WebMethod()]
        [XmlInclude(typeof(DocsPaVO.fascicolazione.Fascicolo))]
        public DocsPaVO.fascicolazione.Fascicolo GetFascicoloById(string idFascicolo)
        {
            try
            {
                Fascicolo result = null;

                result = BusinessLogic.Fascicoli.FascicoloManager.getFascicoloByIdNoSecurity(idFascicolo);

                return result;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaConservazioneWS.asmx  - metodo: getFascicoloByIdNoSecurity", e);

                throw e;
            }
        }
        /// <summary>
        /// recupera la folder del fascicolo
        /// </summary>
        /// <param name="idPeople"></param>
        /// <param name="idGruppo"></param>
        /// <param name="fascicolo"></param>
        /// <returns></returns>
        [WebMethod]
        public DocsPaVO.fascicolazione.Folder FascicolazioneGetFolder(string idPeople, string idGruppo, DocsPaVO.fascicolazione.Fascicolo fascicolo)
        {
            DocsPaVO.fascicolazione.Folder objFolder = null;

            try
            {
                //gestione tracer
#if TRACE_WS
				DocsPaUtils.LogsManagement.PerformaceTracer pt = new DocsPaUtils.LogsManagement.PerformaceTracer("TRACE_WS");
#endif
                objFolder = BusinessLogic.Fascicoli.FolderManager.getFolder(idPeople, idGruppo, fascicolo);
#if TRACE_WS
				pt.WriteLogTracer("FascicolazioneGetFolder");
#endif
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: FascicolazioneGetFolder", e);
                objFolder = null;
            }

            return objFolder;
        }

        [WebMethod()]
        [XmlInclude(typeof(DocsPaVO.ProfilazioneDinamicaLite.TemplateLite))]
        public DocsPaVO.ProfilazioneDinamicaLite.TemplateLite[] GetTypeDocumentsWithDiagramByIdAmm(int idAmministrazione, string type)
        {
            try
            {
                TemplateLite[] result = null;
                if (type.Equals("D"))
                {
                    result = BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.getListTemplatesLite(idAmministrazione.ToString());
                }
                else
                {
                    result = BusinessLogic.ProfilazioneDinamica.ProfilazioneFascicoli.getListTemplatesLite(idAmministrazione.ToString());
                }
                return result;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaConservazioneWS.asmx  - metodo: GetTypeDocumentsWithDiagramByIdAmm", e);

                throw e;
            }
        }

        [WebMethod]
        public virtual ArrayList getListaExtFileAcquisiti(string idamm)
        {
            ArrayList fileAcquisiti = new ArrayList();
            try
            {
                fileAcquisiti = BusinessLogic.Documenti.DocManager.getListaExtFileAcquisiti(idamm);
                return fileAcquisiti;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaConservazioneWS.asmx  - metodo: getListaExtFileAcquisiti", e);
                return null;
            }
        }

        /// <summary>
        /// Nuova funzione di ricerca documenti
        /// </summary>
        [WebMethod]
        [XmlInclude(typeof(DocsPaVO.Grids.SearchObjectField))]
        [XmlInclude(typeof(DocsPaVO.Grid.Field))]
        [XmlInclude(typeof(DocsPaVO.filtri.FiltroRicerca))]
        [return: XmlArray()]
        [return: XmlArrayItem(typeof(DocsPaVO.Grids.SearchObject))]
        public virtual ArrayList DocumentoGetQueryDocumentoPagingCustom(InfoUtente infoUtente, DocsPaVO.filtri.FiltroRicerca[][] queryList, int numPage, bool security, int pageSize, out int numTotPage, out int nRec, bool getIdProfilesList, bool gridPersonalization, bool export, Field[] visibleFieldsTemplate, String[] documentsSystemId, out List<SearchResultInfo> idProfileList)
        {
            List<SearchResultInfo> toSet = new List<SearchResultInfo>();
            ArrayList objListaInfoDocumenti = null;
            numTotPage = 0;
            nRec = 0;

            try
            {
                objListaInfoDocumenti = BusinessLogic.Documenti.InfoDocManager.getQueryPagingCustom(infoUtente, queryList, numPage, pageSize, security, export, gridPersonalization, visibleFieldsTemplate, documentsSystemId, out numTotPage, out nRec, getIdProfilesList, out toSet);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaConservazioneWS.asmx  - metodo: DocumentoGetQueryDocumentoPagingCustom", e);

                objListaInfoDocumenti = null;
            }
            idProfileList = toSet;
            return objListaInfoDocumenti;
        }

        [WebMethod]
        [return: XmlArray()]
        [return: XmlArrayItem(typeof(DocsPaVO.amministrazione.OrgTitolario))]
        public virtual ArrayList getTitolariUtilizzabili(string idAmministrazione)
        {
            ArrayList titolari = new ArrayList();
            try
            {
                titolari = BusinessLogic.Amministrazione.TitolarioManager.getTitolariUtilizzabili(idAmministrazione);
                return titolari;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaConservazioneWS.asmx  - metodo: getTitolariUtilizzabili", e);
                return titolari;
            }
        }

        [WebMethod]
        [XmlInclude(typeof(DocsPaVO.Grids.SearchObjectField))]
        [XmlInclude(typeof(DocsPaVO.Grid.Field))]
        [return: XmlArray()]
        [return: XmlArrayItem(typeof(DocsPaVO.Grids.SearchObject))]
        public virtual ArrayList FascicolazioneGetListaFascicoliPagingCustom(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.fascicolazione.Classificazione classificazione, DocsPaVO.utente.Registro registro, DocsPaVO.filtri.FiltroRicerca[] listaFiltri, bool enableUfficioRef, bool enableProfilazione, bool childs, int numPage, out int numTotPage, out int nRec, int pageSize, bool getSystemIdList, out List<SearchResultInfo> idProjectList, byte[] excelDati, bool showGridPersonalization, bool export, Field[] visibleFieldsTemplate, String[] documentsSystemId, bool security)
        {
            ArrayList result = null;
            nRec = 0;
            numTotPage = 0;

            // Lista dei system id dei fascicoli restituiti dalla ricerca
            List<SearchResultInfo> idProjects = null;

            string serverPath = System.Configuration.ConfigurationManager.AppSettings["LOG_PATH"];
            serverPath = serverPath.Replace("%DATA", DateTime.Now.ToString("yyyyMMdd"));

            try
            {
                result = BusinessLogic.Fascicoli.FascicoloManager.getListaFascicoliPagingCustom(infoUtente, classificazione, registro, listaFiltri, enableUfficioRef, enableProfilazione, childs, out numTotPage, out  nRec, numPage, pageSize, getSystemIdList, out idProjects, excelDati, serverPath, showGridPersonalization, export, visibleFieldsTemplate, documentsSystemId, security);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaConservazioneWS.asmx  - metodo: FascicolazioneGetListaFascicoliPagingCustom", e);
                result = null;
            }

            // Assegnazione della lista dei system id dei fascicoli
            idProjectList = idProjects;

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idIstanza"></param>
        /// <returns></returns>
        [WebMethod()]
        public bool IsIstanzaConservazioneInterna(DocsPaVO.utente.InfoUtente infoUtente, string idIstanza)
        {
            DocsPaConservazione.DocsPaConsManager consManager = new DocsPaConsManager();
            
            return consManager.IsConservazioneInterna(idIstanza);
        }

        /// <summary>
        /// Reperimento delle notifiche di conservazione
        /// </summary>
        /// <returns></returns>
        [WebMethod()]
        public DocsPaConservazione.InfoNotifica[] GetNotifiche(string idAmm)
        {
            //MEV CS 1.5 - notifiche per verifiche leggibilità
            //return DocsPaConservazione.Notifiche.GetNotifiche(idAmm);
            return DocsPaConservazione.Notifiche.GetNotificheTotali(idAmm);

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idIstanza"></param>
        /// <param name="dataIniziale"></param>
        /// <param name="dataFinale"></param>
        /// <param name="utente"></param>
        /// <param name="azione"></param>
        /// <param name="esito"></param>
        /// <returns></returns>
        [WebMethod()]
        public LogConservazione[] GetLogs(DocsPaVO.utente.InfoUtente infoUtente,
                                              string idIstanza,
                                              string dataIniziale,
                                              string dataFinale,
                                              string utente, 
                                              string azione, 
                                              string esito)
        {
            return DocsPaConservazione.LogConservazione.GetLogs(infoUtente, idIstanza, dataIniziale, dataFinale, utente, azione, esito);
        }

        [WebMethod()]
        public LogConservazione[] GetListAzioniLog()
        {
            LogConservazione[] retValue = new DocsPaConsManager().GetAzioniLog();
            return retValue;

        }


        [WebMethod()]
        public virtual DocsPaVO.amministrazione.InfoAmministrazione AmmGetInfoAmmCorrente(string idAmm)
        {
            DocsPaVO.amministrazione.InfoAmministrazione retValue = null;
            try
            {
                retValue = BusinessLogic.Amministrazione.AmministraManager.AmmGetInfoAmmCorrente(idAmm);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: AmmGetInfoAmmCorrente - ", e);
            }
            return retValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idDocumento"></param>
        /// <param name="indiceAllegato"></param>
        /// <returns></returns>
        [WebMethod()]
        public DocsPaVO.documento.FileDocumento GetFileDocumentoNotifica(
                                DocsPaVO.utente.InfoUtente infoUtente,
                                string idDocumento, 
                                int indiceAllegato)
        {
            DocsPaVO.documento.FileDocumento file = null;

            DocsPaVO.documento.SchedaDocumento documento = BusinessLogic.Documenti.DocManager.getDettaglioNoSecurity(infoUtente, idDocumento);

            if (indiceAllegato == 0)
            {
                // Reperimento documento principale
                DocsPaVO.documento.FileRequest fileRequest = (DocsPaVO.documento.FileRequest)documento.documenti[0];

                file = BusinessLogic.Documenti.FileManager.getFile(fileRequest, infoUtente);
            }
            else
            {
                DocsPaVO.documento.FileRequest fileRequest = (DocsPaVO.documento.FileRequest)documento.allegati[0];

                file = BusinessLogic.Documenti.FileManager.getFile(fileRequest, infoUtente);
            }

            return file;
        }

        [WebMethod()]
        public DocsPaVO.documento.FileDocumento GetFileDocumentoFirmato(DocsPaVO.utente.InfoUtente infoUtente, string idDocumento, int indiceAllegato)
        {

            DocsPaVO.documento.FileDocumento file = null;
            DocsPaVO.documento.SchedaDocumento documento = BusinessLogic.Documenti.DocManager.getDettaglioNoSecurity(infoUtente, idDocumento);

            if (indiceAllegato == 0)
            {
                DocsPaVO.documento.FileRequest fileRequest = (DocsPaVO.documento.FileRequest)documento.documenti[0];
                file = BusinessLogic.Documenti.FileManager.getFileFirmato(fileRequest, infoUtente, false);
            }
            else
            {
                DocsPaVO.documento.FileRequest fileRequest = (DocsPaVO.documento.FileRequest)documento.allegati[0];
                file = BusinessLogic.Documenti.FileManager.getFileFirmato(fileRequest, infoUtente, false);
            }

            return file;
        }

        [WebMethod]
        public virtual DocsPaVO.ProfilazioneDinamica.Templates getTemplateFascById(string idTemplate)
        {
            try
            {
                return BusinessLogic.ProfilazioneDinamica.ProfilazioneFascicoli.getTemplateFascById(idTemplate);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaConservazioneWS.asmx  - metodo: getTemplateFascById", e);
                return null;
            }
        }

        [WebMethod]
        public virtual DocsPaVO.ProfilazioneDinamica.Templates getTemplateFascCampiComuniById(DocsPaVO.utente.InfoUtente infoUtente, string idTemplate)
        {
            try
            {
                SetUserId(infoUtente);
                return BusinessLogic.ProfilazioneDinamica.ProfilazioneFascicoli.getTemplateFascCampiComuniById(infoUtente, idTemplate);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaConservazioneWS.asmx  - metodo: getTemplateFascCampiComuniById", e);
                return null;
            }
        }

        private void SetUserId(InfoUtente infoUtente)
        {
            if (infoUtente != null) SetUserId(infoUtente.userId);
        }

        private void SetUserId(string userId)
        {
            if (!string.IsNullOrEmpty(userId)) LogicalThreadContext.Properties["userId"] = userId.ToUpper();
        }

        [WebMethod]
        public virtual DocsPaVO.ProfilazioneDinamica.Templates getTemplateById(string idTemplate)
        {
            try
            {
                return BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.getTemplateById(idTemplate);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaConservazioneWS.asmx  - metodo: getTemplateById", e);
                return null;
            }
        }

        [WebMethod]
        public virtual DocsPaVO.ProfilazioneDinamica.Templates getTemplateCampiComuniById(DocsPaVO.utente.InfoUtente infoUtente, string idTemplate)
        {
            try
            {
                SetUserId(infoUtente);
                return BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.getTemplateCampiComuniById(infoUtente, idTemplate);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaConservazioneWS.asmx  - metodo: getTemplateCampiComuniById", e);
                return null;
            }
        }

        [WebMethod]
        [XmlInclude(typeof(DocsPaVO.utente.Corrispondente))]
        public virtual DocsPaVO.utente.Corrispondente AddressbookGetCorrispondenteBySystemId(string system_id)
        {
            DocsPaVO.utente.Corrispondente corr = null;

            try
            {
                logger.Debug("WS > AddressbookGetCorrispondenteBySystemId");
                corr = BusinessLogic.Utenti.UserManager.getCorrispondenteBySystemID(system_id);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaConservazioneWS.asmx  - metodo: AddressbookGetCorrispondenteBySystemId - ", e);
            }
            return corr;
        }

        [WebMethod]
        [XmlInclude(typeof(DocsPaVO.utente.Corrispondente))]
        public virtual DocsPaVO.utente.Corrispondente AddressbookGetCorrispondenteByCodRubrica(string codice, DocsPaVO.utente.InfoUtente u, string condRegistri)
        {
            SetUserId(u);
            DocsPaVO.utente.Corrispondente corr = null;

            try
            {
                logger.Debug("WS > AddressbookGetCorrispondenteByCodRubrica");
                corr = BusinessLogic.Utenti.UserManager.getCorrispondenteByCodRubrica(codice, u, condRegistri, false);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaConservazioneWS.asmx  - metodo: AddressbookGetCorrispondenteByCodRubrica - ", e);
            }
            return corr;
        }

        [WebMethod]
        [XmlInclude(typeof(DocsPaVO.rubrica.ElementoRubrica))]
        public virtual DocsPaVO.rubrica.ElementoRubrica rubricaGetElementoRubrica(string cod, DocsPaVO.utente.InfoUtente u, DocsPaVO.rubrica.SmistamentoRubrica smistamentoRubrica, string condRegistri)
        {
            SetUserId(u);
            try
            {
                logger.Debug("WS > rubricaGetElementoRubrica");
                BusinessLogic.Rubrica.DPA3_RubricaSearchAgent ccs = new BusinessLogic.Rubrica.DPA3_RubricaSearchAgent(u);
                return ccs.SearchSingle(cod, smistamentoRubrica, condRegistri);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaConservazioneWS.asmx  - metodo: rubricaGetElementoRubrica - ", e);
                return null;
            }
        }

        /// <summary>
        /// Ritorna un singolo elemento rubrica, sulla base della systemId passata in ingresso.
        /// </summary>
        /// <param name="systemId">systemId dell'elemento rubrica che sarà ritornato</param>
        /// <param name="u">infoUtente</param>
        /// <returns></returns>
        [WebMethod]
        [XmlInclude(typeof(DocsPaVO.rubrica.ElementoRubrica))]
        public virtual DocsPaVO.rubrica.ElementoRubrica rubricaGetElementoRubricaSimpleBySystemId(string systemId, DocsPaVO.utente.InfoUtente u)
        {
            SetUserId(u);
            try
            {
                logger.Debug("WS > rubricaGetElementoRubricaSimpleBySystemId");
                BusinessLogic.Rubrica.DPA3_RubricaSearchAgent ccs = new BusinessLogic.Rubrica.DPA3_RubricaSearchAgent(u);
                return ccs.SearchSingleSimpleBySystemId(systemId);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaConservazioneWS.asmx  - metodo: rubricaGetElementoRubricaSimpleBySystemId - ", e);
                return null;
            }
        }

        /// <summary>
        /// Metodo che restituisce vero se un'istanza di Conservazione può essere passata in lavorazione.
        /// </summary>
        /// <param name="idConservazione">Id dell'istanza</param>
        /// <returns>Vero se l'istanza è abilitata, falso altrimenti.</returns>
        [WebMethod]
        public virtual bool abilitaLavorazione(string idConservazione)
        {
            try
            {
                logger.Debug("WS > abilitaLavorazione");
                bool retval = false;
                DocsPaConsManager dpcm = new DocsPaConsManager();
                int mask = dpcm.getValidationMask(idConservazione);
                int filtro = (int)InfoConservazione.EsitoValidazioneMask.FirmaValida | (int)InfoConservazione.EsitoValidazioneMask.MarcaValida | (int)InfoConservazione.EsitoValidazioneMask.FormatoValido | (int) InfoConservazione.EsitoValidazioneMask.DimensioneValida;
                mask &= filtro; //23
                if (mask == filtro) retval = true;
                return retval;
            }catch(Exception e){
                logger.Debug("Errore in DocsPaConservazioneWS.asmx  - metodo: abilitaLavorazione - ", e);
                return false;
            }
        }

        /// <summary>
        /// Metodo che restituisce gli id di tutte le istanze nuove e non verificate ne automaticamente ne manualmente.
        /// </summary>
        /// <returns></returns>
        [WebMethod]
        public virtual string[] getIstanzeNonVerificate(string idAmministrazione)
        {
            try
            {
                logger.Debug("WS > getIstanzeNonVerificate");
                DocsPaConsManager dpcm = new DocsPaConsManager();
                return dpcm.getIstanzeNonVerificate(idAmministrazione);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaConservazioneWS.asmx  - metodo: getIstanzeNonVerificate - ", e);
                return null;
            }
        }

        /// <summary>
        /// Metodo che verifica che sia le policy di un'istanza di conservazione siano state validate
        /// </summary>
        /// <param name="idConservazione">Id dell'istanza</param>
        /// <returns>Vero se le policy sono validate</returns>
        [WebMethod]
        public virtual bool policyVerificata(string idConservazione)
        {     
            
            try
            {
                logger.Debug("WS > policyVerificata");
                DocsPaConservazione.DocsPaConsManager dpcm = new DocsPaConsManager();
                return dpcm.policyVerificata(idConservazione);

            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaConservazioneWS.asmx  - metodo: policyVerificata - ", e);
                return false;
            }
        }

        /// <summary>
        /// Metodo che restituisce la validation Mask dell'istanza.
        /// </summary>
        /// <returns></returns>

        [WebMethod]
        public virtual int getValidationMask(string idConservazione)
        {
            int mask = 0;
            try
            {
                logger.Debug("WS > getValidationMask");

                DocsPaConsManager dpcm = new DocsPaConsManager();
                mask = dpcm.getValidationMask(idConservazione);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaConservazioneWS.asmx  - metodo: getValidationMask - ", e);
                return 0;


            }
            return mask;
        }

        /// <summary>
        /// Metodo per salvare il registro controlli
        /// </summary>
        /// <param name="idConservazione"></param>
        /// <param name="docnumber"></param>
        /// <param name="infoUtente"></param>
        /// <param name="tipoOperazione"></param>
        /// <param name="esito"></param>
        /// <param name="verificati"></param>
        /// <param name="validi"></param>
        /// <param name="invalidi"></param>
        [WebMethod]
        public virtual void inserimentoInRegistroControlli(string idConservazione, string docnumber, InfoUtente infoUtente, string tipoOperazione, bool esito, int verificati, int validi, int invalidi)
        {
            new DocsPaConsManager().inserimentoInRegistroControlli(idConservazione, docnumber, infoUtente, tipoOperazione, esito, verificati, validi, invalidi);
        }

        /// <summary>
        /// Metodo per salvare il registro di conservazione
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="DocsPaVO.Conservazione.RegistroCons"></param>
        /// <param name="infoUtente"></param>
        /// 
        [WebMethod]
        public virtual void inserimentoInRegistroCons(DocsPaVO.Conservazione.RegistroCons regCons, InfoUtente infoUtente)
        {
            RegistroConservazione rc = new RegistroConservazione();
            rc.inserimentoInRegistroCons(regCons, infoUtente);
        }


        /// <summary>
        /// Metodo per salvare alcune operazioni su dpa_log
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name=WebMethodName></param>
        /// <param name="idConservazione"></param>
        /// <param name="descAzione"></param>
        /// <param name="codAzione"></param>

        /// 
        [WebMethod]
        public virtual void inserimentoInDpaLog(DocsPaVO.utente.InfoUtente infoUtente, string WebMethodName, string ID_Oggetto, string Var_desc_Oggetto, DocsPaVO.Logger.CodAzione.Esito Cha_Esito)
        {
            BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, WebMethodName, ID_Oggetto,
            Var_desc_Oggetto, Cha_Esito);
        }


        /// <summary>
        /// Ricava un file dallo storage remoto, specificando l'istanza
        /// </summary>
        /// <param name="infoUtente">utente loggato</param>
        /// <param name="idConservazione">id conservazione</param>
        /// <param name="pathFile"> path relativo del file in conservazione</param>
        /// <returns>bytearray del file</returns>
        [WebMethod]
        public virtual byte[] getFileFromStore(InfoUtente infoUtente, string idConservazione, string pathFile, bool localStore)
        {
            return new DocsPaConservazione.FileManager().getFileFromStore(idConservazione, pathFile,localStore );
            
        }


        /// <summary>
        /// Ricava l'hash di un file dallo storage remoto, specificando l'istanza
        /// </summary>
        /// <param name="infoUtente">utente loggato</param>
        /// <param name="idConservazione">id conservazione</param>
        /// <param name="pathFile"> path relativo del file in conservazione</param>
        /// <returns>bytearray del file</returns>
        [WebMethod]
        public virtual string getFileHashFromStore(InfoUtente infoUtente, string idConservazione, string pathFile, bool localStore)
        {
            return new DocsPaConservazione.FileManager().getFileHashFromStore(idConservazione, pathFile,localStore);

        }

       
        /// <summary>
        /// Metodo per modificare la validation mask in seguito alla verifica della leggibilità
        /// </summary>
        /// <param name="idConservazione">id dell'istanza di conservazione</param>
        /// <param name="passed">vero se i file sono leggibili</param>
        /// <param name="infoUtente">l'infoUtente</param>
        /// <returns>esito dell'operazione di update in db</returns>
        [WebMethod]
        public virtual bool esitoLeggibilita(InfoUtente infoUtente, string idConservazione, bool passed)
        {

            logger.Debug("BEGIN");
            //Per rendere asincrona l'operazione di spostamento dei file Impostare a true 
            //Questa operazione è da fare pure su IniziaVerificaSupporto in VerificaSupportoRemoto.cs
            bool operazioneAsincrona = true;
            bool retval = false;
            string esito ="0";
            DocsPaVO.Logger.CodAzione.Esito codAzione;
            string descrizione;

            InfoConservazione info = new DocsPaConservazione.DocsPaConsManager().getInfoIstanza(idConservazione);
            DocsPaConsManager dpcm = new DocsPaConsManager();
            retval = dpcm.esitoLeggibilita(idConservazione, passed);
            if (info.StatoConservazione == "F")
            {
                if (passed)
                {
                    if (operazioneAsincrona)
                    {
                        // Metto l'istanza in uno stato intermedio di transizione
                        DocsPaConsManager.UpdateStatoIstanzaConservazione(idConservazione, DocsPaConservazione.StatoIstanza.IN_TRANSIZIONE);
                        this.IniziaVerificaSupportoRemoto(infoUtente, idConservazione);
                        retval = true;
                    }
                    else
                    {
                        //Spostiamo i file sullo storage remoto..
                        createZipFile(idConservazione, infoUtente);
                        if (SubmitToRemoteFolder(idConservazione, infoUtente))
                        {
                            this.IniziaVerificaSupportoRemoto(infoUtente, idConservazione);
                        }
                        else
                        {
                            retval = false;

                        }
                    }
                }
            }


            if (passed)
            {
                descrizione = "Esecuzione della verifica di leggibilità dei documenti dell'istanza " + idConservazione;
                esito = "1";
                codAzione = DocsPaVO.Logger.CodAzione.Esito.OK;
            }
            else
            {
                descrizione = "Esecuzione della verifica di leggibilità dei documenti dell'istanza " + idConservazione;
                esito = "0";
                codAzione = DocsPaVO.Logger.CodAzione.Esito.KO;

            }

            BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "LEGGIBILITA",
            idConservazione, descrizione, codAzione);


            DocsPaVO.Conservazione.RegistroCons regCons = new DocsPaVO.Conservazione.RegistroCons();
            regCons.idAmm = infoUtente.idAmministrazione;
            regCons.idIstanza = idConservazione;            
            regCons.tipoOggetto = "I";
            regCons.tipoAzione = "";
            regCons.userId = infoUtente.userId;
            regCons.codAzione = "LEGGIBILITA";
            regCons.descAzione = descrizione;
            regCons.esito = esito;
            RegistroConservazione rc = new RegistroConservazione();
            rc.inserimentoInRegistroCons(regCons, infoUtente);

            logger.Debug("END");


            return retval;



        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="idConservazione"></param>
        /// <param name="pathFile"></param>
        /// <returns></returns>
        [WebMethod]
        public FileDocumento sbustaFileFirmato(string idConservazione, string pathFile,bool localStore)
        {
            FileDocumento fileDoc = new FileDocumento();
            fileDoc.content = new DocsPaConservazione.FileManager().getFileFromStore(idConservazione, pathFile,localStore);
            fileDoc.name = System.IO.Path.GetFileName(pathFile);
            BusinessLogic.Documenti.FileManager.VerifyFileSignature(fileDoc, null);

            return fileDoc;
        }


        /// <summary>
        /// ritorna se i supporti esterni sono rimovibili o meno
        /// </summary>
        /// <returns></returns>
        [WebMethod]
        public bool supportiRimovibiliVerificabili()
        {
            //bool retval= false;
            //string setting;
            ////prima provo nel WEB_CONFIG
            //if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["CONSERVAZIONE_VERIFICA_SUPPORTI_REMOVIBILI"]))
            //{
            //    setting = ConfigurationManager.AppSettings["CONSERVAZIONE_VERIFICA_SUPPORTI_REMOVIBILI"].ToString();
                
            //}
            //else  //poi neldb
            //{
            //    //per ora è globale
            //    setting = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_CONSERVAZIONE_VERIFICA_SUPPORTI_REMOVIBILI");
            //}
            //if (setting.Equals("1"))
            //    setting = "true";
            //Boolean.TryParse(setting, out retval);
            //return retval;
            return DocsPaConsManager.supportiRimovibiliVerificabili();
        }

        [WebMethod]
        public string httpStorageRemoteUrlAddress()
        {
            return DocsPaConsManager.httpStorageRemoteUrlAddress();
        }

        /// <summary>
        /// Metodo per il controllo della dimensione dell'istanza. 
        /// </summary>
        /// <param name="idConservazione">Il system id dell'istanza</param>
        /// <returns>vero se le dimensioni sono nei limiti stabiliti.</returns>
        [WebMethod]
        public bool ctrlDimensioniIstanza(string idConservazione)
        {
            DocsPaConsManager dpcm = new DocsPaConsManager();
            return dpcm.ctrlDimensioniIstanza(idConservazione);
        }

        /// <summary>
        /// Metodo per il controllo della dimensione dell'istanza. 
        /// </summary>
        /// <param name="idConservazione">Il system id dell'istanza</param>
        ///  <param name="infoUtente">utente che effettua la verifica</param>
        /// <returns>vero se le dimensioni sono nei limiti stabiliti.</returns>
        [WebMethod]
        public bool ctrlDimensioniIstanzaUt(string idConservazione, InfoUtente infoUtente)
        {
            DocsPaConsManager dpcm = new DocsPaConsManager();
            return dpcm.ctrlDimensioniIstanza(idConservazione, infoUtente);
        }
        /// <summary>
        /// Metodo per prelevare i valori della massima dimensione di un istanza, per la visualizzazione all'interno del messaggio di errore.
        /// </summary>
        /// <param name="idConservazione"></param>
        /// <returns></returns>
        [WebMethod]
        public string getMaxDimensioniIstanza(string idConservazione)
        {
            DocsPaConsManager dpcm = new DocsPaConsManager();
            return dpcm.getMaxDimensioniIstanza(idConservazione);
        }

        /// <summary>
        /// Nel caso un istanza non sia ancora stata verificata automaticamente, e non abbia una policy assegnata, questo metodo
        /// modifica la validation mask in maniera che non risulti l'errore di policy non valida (essendo assente, il comportamento 
        /// sarebbe errato).
        /// </summary>
        /// <param name="idConservazione"></param>
        [WebMethod]
        public void setPolicyVerificataLite(string idConservazione)
        {
            DocsPaConsManager dpcm = new DocsPaConsManager();
            dpcm.setPolicyVerificataLite(idConservazione);
        }


        [WebMethod]
        public string getSegnatura_ID_Doc(string idProfile)
        {
            string segnaturaOrId = string.Empty;
            try
            {
                DocsPaDB.Query_DocsPAWS.Conservazione cons = new DocsPaDB.Query_DocsPAWS.Conservazione();
                segnaturaOrId = cons.getSegnatura_Id(idProfile);
            }
            catch (Exception ex)
            {
                logger.Debug("Errore nella ricerca della segnatura_Id doc :" + ex.Message);
                segnaturaOrId = "";
            }
            return segnaturaOrId;
        }

        #region REPORT CONSERVAZIONE

        /// <summary>
        /// report della conservazione
        /// </summary>
        /// <param name="filtriReport"></param>
        /// <param name="tipoRep"></param>
        [WebMethod]
        public FileDocumento createReportConservazione(DocsPaVO.filtri.FiltroRicerca[] filtriReport, string tipoRep, string tipoFile,string titoloReport,string reportKey,string contextName, InfoUtente infoUt)
        {
            FileDocumento fd = new FileDocumento();
            DocsPaVO.Report.ReportTypeEnum rt = new DocsPaVO.Report.ReportTypeEnum();
            if (!string.IsNullOrEmpty(tipoRep) && tipoRep.Equals("PDF"))
                rt = DocsPaVO.Report.ReportTypeEnum.PDF;
            else
                rt = DocsPaVO.Report.ReportTypeEnum.Excel;

            string admin = string.Empty;
            DocsPaDB.Query_Utils.Utils obj = new DocsPaDB.Query_Utils.Utils();
            admin = obj.getDescAmm(infoUt.idAmministrazione);
            string Sottotitolo = "Amministrazione: " + admin
                + Environment.NewLine + "Data generazione report : " + DateTime.Now.ToString();

            List<FiltroRicerca> filtri = new List<FiltroRicerca>();

            for (int i = 0; i < filtriReport.Length; i++)
            {
                filtri.Add(filtriReport[i]);
            }

            String Lista = GetListaFiltri(filtri);

            


            fd = BusinessLogic.Reporting.ReportGeneratorCommand.GetReport(
               new DocsPaVO.Report.PrintReportRequest()
               {
                   UserInfo = infoUt,
                   SearchFilters = filtri,
                   Title= titoloReport,
                   SubTitle = Sottotitolo,
                   ReportType = rt,
                   ReportKey = reportKey,
                   ContextName =contextName,
                   AdditionalInformation = String.Format(Lista)
               }).Document;

            return fd;
        }

        private String GetListaFiltri(List<FiltroRicerca> searchFilters)
        {

            // funzione implementata per mostrare nel report da lista dei filtri
            // utilizzati per la ricerca.
            String ListaFiltri = "Lista dei filtri di ricerca: " + Environment.NewLine;

            foreach (FiltroRicerca f in searchFilters) 
            {
                switch (f.argomento)
                {
                    case "DATA_INVIO_IL":

                        ListaFiltri += "Data invio in conservazione : " + f.valore.ToString() + Environment.NewLine;

                        break;

                    case "DATA_INVIO_DAL":

                        ListaFiltri += "Data invio in conservazione dal : " + f.valore.ToString() + Environment.NewLine;

                        break;

                    case "DATA_INVIO_AL":

                        ListaFiltri += "Data invio in conservazione al : " + f.valore.ToString() + Environment.NewLine;

                        break;

                    case "DATA_INVIO_TODAY":

                        ListaFiltri += "Data invio in conservazione : oggi " + Environment.NewLine;

                        break;

                    case "DATA_INVIO_SC":

                        ListaFiltri += "Data invio in conservazione : settimana corrente " + Environment.NewLine;

                        break;

                    case "DATA_INVIO_MC":

                        ListaFiltri += "Data invio in conservazione : mese corrente " + Environment.NewLine;

                        break;

                    case "DATA_CHIUSURA_IL":

                        ListaFiltri += "Data chiusura conservazione il : " + f.valore.ToString() + Environment.NewLine;

                        break;

                    case "DATA_CHIUSURA_DAL":

                        ListaFiltri += "Data chiusura conservazione dal : " + f.valore.ToString() + Environment.NewLine;

                        break;

                    case "DATA_CHIUSURA_AL":

                        ListaFiltri += "Data chiusura conservazione al : " + f.valore.ToString() + Environment.NewLine;

                        break;

                    case "DATA_CHIUSURA_TODAY":

                        ListaFiltri += "Data chiusura conservazione : oggi " + Environment.NewLine;

                        break;

                    case "DATA_CHIUSURA_SC":

                        ListaFiltri += "Data chiusura conservazione : settimana corrente " + Environment.NewLine;

                        break;

                    case "DATA_CHIUSURA_MC":

                        ListaFiltri += "Data chiusura conservazione : mese corrente " + Environment.NewLine;

                        break;


                    case "DATA_RIFIUTO_IL":

                        ListaFiltri += "Data rifiuto conservazione il : " + f.valore.ToString() + Environment.NewLine;

                        break;

                    case "DATA_RIFIUTO_DAL":

                        ListaFiltri += "Data rifiuto conservazione dal : " + f.valore.ToString() + Environment.NewLine;

                        break;

                    case "DATA_RIFIUTO_AL":

                        ListaFiltri += "Data rifiuto conservazione al : " + f.valore.ToString() + Environment.NewLine;

                        break;
                    case "DATA_RIFIUTO_TODAY":

                        ListaFiltri += "Data rifiuto conservazione : oggi " + Environment.NewLine;

                        break;
                    case "DATA_RIFIUTO_SC":

                        ListaFiltri += "Data rifiuto conservazione : settimana corrente" + Environment.NewLine;

                        break;
                    case "DATA_RIFIUTO_MC":

                        ListaFiltri += "Data rifiuto conservazione : mese corrente" + Environment.NewLine;

                        break;
                    case "ID_ISTANZA":

                        ListaFiltri += "Id istanza di conservazione : " + f.valore.ToString() + Environment.NewLine;

                        break;
                    case "ID_ISTANZA_DAL":

                        ListaFiltri += "Id istanza di conservazione dal : " + f.valore.ToString() + Environment.NewLine;

                        break;
                    case "ID_ISTANZA_AL":

                        ListaFiltri += "Id istanza di conservazione al : " + f.valore.ToString() + Environment.NewLine;

                        break;
                    case "ID_SUPPORTO":

                        ListaFiltri += "Id supporto : " + f.valore.ToString() + Environment.NewLine;

                        break;
                    case "ID_SUPPORTO_DAL":

                        ListaFiltri += "Id supporto dal : " + f.valore.ToString() + Environment.NewLine;

                        break;
                    case "ID_SUPPORTO_AL":

                        ListaFiltri += "Id supporto al : " + f.valore.ToString() + Environment.NewLine;

                        break;
                    case "DATA_GENERAZIONE_IL":

                        ListaFiltri += "Data generazione supporto il : " + f.valore.ToString() + Environment.NewLine;

                        break;

                    case "DATA_GENERAZIONE_DAL":

                        ListaFiltri += "Data generazione supporto dal : " + f.valore.ToString() + Environment.NewLine;

                        break;
                    case "DATA_GENERAZIONE_AL":

                        ListaFiltri += "Data generazione supporto al : " + f.valore.ToString() + Environment.NewLine;

                        break;
                    case "DATA_GENERAZIONE_TODAY":

                        ListaFiltri += "Data generazione supporto : oggi " + Environment.NewLine;

                        break;
                    case "DATA_GENERAZIONE_SC":

                        ListaFiltri += "Data generazione supporto : settimana corrente " + Environment.NewLine;

                        break;
                    case "DATA_GENERAZIONE_MC":

                        ListaFiltri += "Data generazione supporto : mese corrente " + Environment.NewLine;

                        break;
                    case "DATA_ES_VERIFICA_IL":

                        ListaFiltri += "Esecuzione verifica il : " + f.valore.ToString() + Environment.NewLine;

                        break;
                    case "DATA_ES_VERIFICA_DAL":

                        ListaFiltri += "Esecuzione verifica dal : " + f.valore.ToString() + Environment.NewLine;

                        break;
                    case "DATA_ES_VERIFICA_AL":

                        ListaFiltri += "Esecuzione verifica al : " + f.valore.ToString() + Environment.NewLine;

                        break;
                    case "DATA_ES_VERIFICA_TODAY":

                        ListaFiltri += "Esecuzione verifica : oggi " + Environment.NewLine;

                        break;
                    case "DATA_ES_VERIFICA_SC":

                        ListaFiltri += "Esecuzione verifica : settimana corrente " + Environment.NewLine;

                        break;
                    case "DATA_ES_VERIFICA_MC":

                        ListaFiltri += "Esecuzione verifica : mese corrente " + Environment.NewLine;

                        break;
                    case "TIPO_DI_VERIFICA":

                        if (f.valore == "L")
                        {
                            ListaFiltri += "Tipo di verifica : leggibilità " + Environment.NewLine;
                        }

                        else if (f.valore == "U")
                        {
                            ListaFiltri += "Tipo di verifica : unificata " + Environment.NewLine;
                        }

                        else if (f.valore == "I")
                        {
                            ListaFiltri += "Tipo di verifica : integrità " + Environment.NewLine;
                        }
                        break;

                    case "ESITO_VERIFICA":

                        if (f.valore == "1")
                        {
                            ListaFiltri += "Esito verifica : positivo " + Environment.NewLine;
                        }

                        else if (f.valore == "0")
                        {
                            ListaFiltri += "Esito verifica : negativo " + Environment.NewLine;
                        }
                        break;

                    case "PROTOCOLLO":

                        ListaFiltri += "Protocollo numero : " + f.valore.ToString() + Environment.NewLine;

                        break;
                    case "PROTOCOLLO_DAL":

                        ListaFiltri += "Protocollo dal : " + f.valore.ToString() + Environment.NewLine;

                        break;
                    case "PROTOCOLLO_AL":

                        ListaFiltri += "Protocollo al : " + f.valore.ToString() + Environment.NewLine;

                        break;
                    case "DOCUMENTO":

                        ListaFiltri += "Documento numero : " + f.valore.ToString() + Environment.NewLine;

                        break;
                    case "DOCUMENTO_DAL":

                        ListaFiltri += "Documento dal : " + f.valore.ToString() + Environment.NewLine;

                        break;
                    case "DOCUMENTO_AL":

                        ListaFiltri += "Documento al : " + f.valore.ToString() + Environment.NewLine;

                        break;

                }

            }

            return ListaFiltri;
        }


        #endregion

        /// <summary>
        /// Restituisce la lista delle verifiche alle policy fallite decodificando la mask
        /// </summary>
        /// <param name="maskPolicy"></param>
        /// <returns></returns>
        [WebMethod]
        public string GetListNonConfPolicy(string maskPolicy)
        {
            string result = string.Empty;
            try
            {
                DocsPaConsManager manager = new DocsPaConsManager();
                result = manager.GetListaNonConfPolicy(maskPolicy);
            }
            catch (Exception ex)
            {
                logger.Debug("Errore nel reperimento delle verifiche alle policy fallite :" + ex.Message);
            }

            return result;
        }

        #region MEV CS 1.4 - Esibizione

        [WebMethod()]
        public InfoEsibizione[] GetInfoEsibizione(InfoUtente infoUtente, ArrayList filters)
        {
            try
            {
                DocsPaConsManager manager = new DocsPaConsManager();
                return manager.GetInfoEsibizione(infoUtente, filters);
            }
            catch (Exception ex)
            {
                logger.Debug("Errore in DocsPaConservazioneWS.asmx - Metodo GetInfoEsibizione - ", ex);
                return null;
            }
        }

        [WebMethod()]
        public ItemsEsibizione[] GetItemsEsibizione(InfoUtente infoUtente, string idEsibizione)
        {

            try
            {
                DocsPaConsManager manager = new DocsPaConsManager();
                return manager.GetItemsEsibizione(infoUtente, idEsibizione);
            }
            catch (Exception ex)
            {
                logger.Debug("Errore in DocsPaConservazioneWS.asmx - Metodo GetItemsEsibizione - ", ex);
                return null;
            }
        }

        [WebMethod()]
        public bool RemoveItemsEsibizione(InfoUtente infoUtente, string idDocumento)
        {
            try
            {
                DocsPaConsManager manager = new DocsPaConsManager();
                return manager.RemoveItemsEsibizione(infoUtente, idDocumento);
            }
            catch (Exception ex)
            {
                logger.Debug("Errore in DocsPaConservazioneWS.asmx - Metodo RemoveItemsEsibizione - ", ex);
                return false;
            }

        }

        [WebMethod()]
        public bool RemoveIstanzaEsibizione(InfoUtente infoUtente, string idEsibizione)
        {
            try
            {
                DocsPaConsManager manager = new DocsPaConsManager();
                return manager.RemoveIstanzaEsibizione(infoUtente, idEsibizione);
            }
            catch (Exception ex)
            {
                logger.Debug("Errore in DocsPaConservazioneWS.asmx - Metodo RemoveIstanzaEsibizione - ", ex);
                return false;
            }
        }

        [WebMethod()]
        public bool SaveFieldsEsibizione(InfoUtente infoUtente, string idEsibizione, string descrizione, string note)
        {
            try
            {
                DocsPaConsManager manager = new DocsPaConsManager();
                return manager.SaveIstanzaEsibizioneFields(infoUtente, idEsibizione, descrizione, note, string.Empty);
            }
            catch (Exception ex)
            {
                logger.Debug("Errore in DocsPaConservazioneWS - Metodo SaveFieldsEsibizione - ", ex);
                return false;
            }
        }

        [WebMethod()]
        public bool RichiediCertificazioneIstanzaEsibizione(InfoUtente infoUtente, string idEsibizione, string descrizione, string note)
        {
            try
            {
                DocsPaConsManager manager = new DocsPaConsManager();
                return manager.RichiediCertificazioneIstanzaEsibizione(infoUtente, idEsibizione, descrizione, note);
            }
            catch (Exception ex)
            {
                logger.Debug("Errore in DocsPaConservazioneWS - Metodo RichiediCertificazioneIstanzaEsibizione - ", ex);
                return false;
            }
        }

        [WebMethod()]
        public bool MarcaCertificazioneIstanzaEsibizione(string systemID, InfoUtente utente, byte[] signedContent)
        {
            bool result = false;

            string enableTSA = "0";
            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["ENABLE_TIMESTAMP"]))
            {
                enableTSA = ConfigurationManager.AppSettings["ENABLE_TIMESTAMP"].ToString();
            }
            string base64TSR = string.Empty;

            if (enableTSA == "1")
            {
                try
                {
                    TimestampManager tm = new TimestampManager();
                    //marcatura.OutputResponseMarca outMarca = new DocsPaWS.marcatura.OutputResponseMarca();
                    OutputResponseMarca outMarca = new OutputResponseMarca();
                    //marcatura.InputMarca inputMarca = new DocsPaWS.marcatura.InputMarca();
                    InputMarca inputMarca = new InputMarca();
                    //marcatura.marcatura Marca = new DocsPaWS.marcatura.marcatura();
                    DocsPaMarcaturaWS Marca = new DocsPaMarcaturaWS();
                    inputMarca.applicazione = ConfigurationManager.AppSettings["APP_CHIAMANTE_TIMESTAMP"];
                    inputMarca.riferimento = utente.userId;
                    //inputMarca.riferimento = "MASSARI";
                    //Recupero la conversione in stringa Hex del file Xml firmato
                    if (signedContent != null)
                    {
                        if (signedContent.Length > 0)
                        {
                            inputMarca.file_p7m = tm.getSignedXmlHex(signedContent);
                        }
                        else
                        {
                            result = false;
                        }
                    }

                    //outMarca = Marca.getTSR(inputMarca);
                    outMarca = Marca.getTSR(inputMarca, utente);
                    base64TSR = outMarca.marca;

                    //Nel caso in cui non si ottenga un contenuto valido per generare il file TSR restituisco 0
                    if (string.IsNullOrEmpty(base64TSR))
                    {
                        logger.Debug("base64TSR Empty");
                        result = false;
                    }
                    else
                    {
                        //Scrivo solo nel Debugger l'esito dell'operazione di Timestamping e l'eventuale descrizione errore
                        //bool appo = tm.updateTimeStamp(outMarca.marca, outMarca.docm, outMarca.dsm, systemID, "");
                        //logger.Debug(outMarca.esito + " : " + outMarca.descrizioneErrore + " Aggiornamento dati marca su DB: " + appo.ToString());
                        result = true;
                    }
                }
                catch(Exception ex)
                {
                    logger.Debug("Errore nel reperimento della marca temporale: " + ex);
                    result = false;
                }

                //salvo la marca
                string root_path = DocsPaConsManager.getEsibizioneRootPath();
                //se non è definito uso il rootpath della conservazione
                if (string.IsNullOrEmpty(root_path))
                    root_path = DocsPaConsManager.getConservazioneRootPath();

                if (string.IsNullOrEmpty(root_path)) root_path = @"C:\Sviluppo\docspa.etnoteam.it\DocsPA30\Conservazione";

                logger.Debug("RootPath : " + root_path);

                EsibizioneFileManager manager = new EsibizioneFileManager();
                if (!manager.saveMarcaCertificazione(root_path, systemID, base64TSR, utente.idAmministrazione))
                    result = false;

            }

            return result;
        }


        [WebMethod()]
        public bool UpdateCertificazioneIstanzaEsibizione(InfoUtente infoUtente, string idEsibizione)
        {
            try
            {
                DocsPaConsManager manager = new DocsPaConsManager();
                bool esito = manager.UpdateCertificazioneIstanzaEsibizione(infoUtente, idEsibizione);
                if (esito)
                {
                    ChiudiIstanzaEsibizioneAsync(infoUtente, idEsibizione, string.Empty, string.Empty);
                    return true;
                }
                else
                {
                    logger.Debug("Errore in DocsPaConservazioneWS.asmx - Metodo UpdateCertificazioneIstanzaEsibizione");
                    return false;
                }
                
            }
            catch (Exception ex)
            {
                logger.Debug("Errore in DocsPaConservazioneWS.asmx - Metodo UpdateCertificazioneIstanzaEsibizione - ", ex);
                return false;
            }

        }


        [WebMethod()]
        public bool RifiutaCertificazioneIstanzaEsibizione(InfoUtente infoUtente, string idEsibizione, string note)
        {
            try
            {
                DocsPaConsManager manager = new DocsPaConsManager();
                return manager.RifiutaCertificazioneIstanzaEsibizione(infoUtente, idEsibizione, note);
            }
            catch (Exception ex)
            {
                logger.Debug("Errore in DocsPaConservazioneWS.asmx - Metodo RifiutaCertificazioneIStanzaEsibizione - ", ex);
                return false;
            }


        }

        /// <summary>
        /// Metodo per eseguire la riabilitazione di un'istanza di esibizione rifiutata
        /// </summary>
        /// <param name="infoUtente">Proprietario dell'istanza</param>
        /// <param name="idEsibizione">ID dell'istanza rifiutata</param>
        /// <returns>Nuovo ID dell'istanza riabilitata</returns>
        [WebMethod()]
        public string RiabilitaIstanzaEsibizione(InfoUtente infoUtente, string idEsibizione)
        {
            try
            {
                DocsPaConsManager manager = new DocsPaConsManager();
                return manager.RiabilitaIstanzaEsibizione(infoUtente, idEsibizione);
            }
            catch (Exception ex)
            {
                logger.Debug("Errore in DocsPaConservazioneWS.asmx - Metodo RiabilitaIstanzaEsibizione - ", ex);
                return string.Empty;
            }

        }

        [WebMethod()]
        public void ChiudiIstanzaEsibizioneAsync(InfoUtente infoUtente, string idEsibizione, string descrizione, string note)
        {

            //metto l'istanza in stato "IN TRANSIZIONE"
            bool esito = new DocsPaConsManager().SaveIstanzaEsibizioneFields(infoUtente, idEsibizione, descrizione, note, "T");

            string root_path = DocsPaConsManager.getEsibizioneRootPath();
            //se non è definito uso il rootpath della conservazione
            if (string.IsNullOrEmpty(root_path))
                root_path = DocsPaConsManager.getConservazioneRootPath();

            if (string.IsNullOrEmpty(root_path)) root_path = @"C:\Sviluppo\docspa.etnoteam.it\DocsPA30\Conservazione";

            //store locale o remoto
            bool localStore = this.isLocalStore();

            EsibizioneFileManager.ChiudiIstanzaEsibizioneAsync(root_path, idEsibizione, infoUtente, localStore);

        }

        [WebMethod()]
        public string GetEsibizioneDownloadUrl(InfoUtente infoUtente, string idEsibizione)
        {
            DocsPaConsManager manager = new DocsPaConsManager();
            return manager.GetEsibizioneDownloadUrl(infoUtente, idEsibizione);
        }

        #endregion



        // Nuovi WebMethod per il calcolo delle utenze abilitate al CS e all'Esibizione
        #region Calcola Profili Utente CS / Esibizione
        
        /// <summary>
        /// Metodo per prelevare il valore del campo abilitato Centro Servizi
        /// </summary>
        /// <param name="idPeople"></param>
        /// <param name="idAmm"></param>
        /// <returns></returns>
        [WebMethod]
        public string GetAbilitatoCentroServizi(string idPeople, string idAmm)
        {
            string AbilitatoCS = string.Empty;
            try
            {
                logger.Debug("WS > GetAbilitatoCentroServizi");
                AbilitatoCS = "1";

                DocsPaConsManager manager = new DocsPaConsManager();
                AbilitatoCS = manager.GetUtenteAbilitatoCS(idPeople, idAmm);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaConservazioneWS.asmx  - metodo: GetAbilitatoCentroServizi - ", e);
                AbilitatoCS = "0";
            }

            return AbilitatoCS;
        }

        /// <summary>
        /// Metodo per prelevare il valore del campo abilitato Esibizione
        /// </summary>
        /// <param name="idPeople"></param>
        /// <param name="idAmm"></param>
        /// <returns></returns>
        [WebMethod]
        public string GetAbilitatoEsibizione(string idPeople, string idAmm)
        {
            string AbilitatoE = string.Empty;
            try
            {
                logger.Debug("WS > GetAbilitatoEsibizione");
                AbilitatoE = "1";

                DocsPaConsManager manager = new DocsPaConsManager();
                AbilitatoE = manager.GetUtenteAbilitatoEsibizione(idPeople, idAmm);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaConservazioneWS.asmx  - metodo: GetAbilitatoEsibizione - ", e);
                AbilitatoE = "0";
            }

            return AbilitatoE;
        }

        #endregion

        /// <summary>
        /// Reperimento dei contatori delle istanze di Esibizione per l'amministrazione
        /// </summary>
        /// <param name="infoUtente"></param>
        [WebMethod()]
        public DocsPaConservazione.ContatoriEsibizione GetContatoriEsibizione(DocsPaVO.utente.InfoUtente infoUtente, string idGruppo)
        {
            return DocsPaConservazione.ContatoriEsibizione.Get(infoUtente, idGruppo);
        }

        [WebMethod()]
        public DocsPaConservazione.ContatoriEsibizione GetContatoriEsibizioneConservazione(DocsPaVO.utente.InfoUtente infoUtente)
        {
            return DocsPaConservazione.ContatoriEsibizione.GetConservazione(infoUtente);
        }

        //
        // Mev Cs 1.4 - Esibizione
        // Metodo per l'inserimento di documenti e fascicoli in area esibizione.
        /// <summary>
        /// Metodo per inserire documenti / fascicoli in area esibizione (DPA_AREA_ESIBIZIONE/DPA_ITEMS_ESIBIZIONE)
        /// </summary>
        /// <param name="idProfile"></param>
        /// <param name="idProject"></param>
        /// <param name="docNumber"></param>
        /// <param name="infoUtente"></param>
        /// <param name="tipoOggetto">D: documento, F: Fascicolo</param>
        /// <returns></returns>
        [WebMethod()]
        public virtual string CreateAndAddDocInAreaEsibizione(string idProfile, string idProject, string docNumber, DocsPaVO.utente.InfoUtente infoUtente, string tipoOggetto, string idConservazione, out DocsPaVO.documento.SchedaDocumento sd)
        {
            SetUserId(infoUtente);
            string result = String.Empty;
            try
            {
                result = BusinessLogic.Documenti.areaConservazioneManager.CreateAndAddDocInAreaEsibizione(infoUtente, idProfile, idProject, docNumber, tipoOggetto, idConservazione, out sd);
                BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCUMENTOESIBIZIONE", idProfile, string.Format("{0}{1}", "Invio in esibizione del doc: ", idProfile), DocsPaVO.Logger.CodAzione.Esito.OK);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaConservazioneWS.asmx  - metodo: CreateAndAddDocInAreaEsibizione", e);
                sd = null;
                result = "-1";
            }
            return result;
        }

        /// <summary>
        /// Serializza i metadati del documento passato in input
        /// </summary>
        /// <param name="schDoc"></param>
        /// <param name="systemID">systemId dell'item esibizione</param>
        /// <returns></returns>
        [WebMethod()]
        public int SerializeSchedaEsib(DocsPaVO.documento.SchedaDocumento schDoc, string systemID)
        {
            int size_xml = 0;
            try
            {
                DocsPaConservazione.InfoDocXml DocXml = new DocsPaConservazione.InfoDocXml();
                size_xml = System.Convert.ToInt32(DocXml.serializeSchedaEsib(schDoc, systemID));
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaConservazioneWS.asmx  - metodo: SerializeSchedaEsib", e);
            }
            return size_xml;
        }

        /// <summary>
        /// Aggiorna la dimensione dell'item di esibizione
        /// </summary>
        /// <param name="sysId"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        [WebMethod()]
        public virtual bool UpdateSizeItemEsib(string sysId, int size)
        {
            bool result = true;
            try
            {
                BusinessLogic.Documenti.areaConservazioneManager.updateSizeItemEsib(sysId, size);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaConservazioneWS.asmx  - metodo: UpdateSizeItemEsib", e);
                result = false;
            }
            return result;
        }

        /// <summary>
        /// Aggiorna le informazioni item esibizione
        /// </summary>
        /// <param name="tipoFile"></param>
        /// <param name="numAllegati"></param>
        /// <param name="systemId"></param>
        /// <returns></returns>
        [WebMethod()]
        public virtual bool updateItemsEsib(string tipoFile, string numAllegati, string systemId)
        {
            bool result = true;
            try
            {
                BusinessLogic.Documenti.areaConservazioneManager.updateItemsEsib(tipoFile, numAllegati, systemId);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaConservazioneWS.asmx  - metodo: updateItemsEsib", e);
                result = false;
            }
            return result;
        }

        /// <summary>
        /// Verifica la presenza di un item esibizione
        /// </summary>
        /// <param name="id_profile"></param>
        /// <param name="id_project"></param>
        /// <param name="type"></param>
        /// <param name="infoUtente"></param>
        /// <param name="idIstanzaConservazione"></param>
        /// <returns></returns>
        [WebMethod()]
        public virtual bool checkItemEsibizionePresenteInIstanzaEsibizione(string id_profile, string id_project, string type, DocsPaVO.utente.InfoUtente infoUtente, string idIstanzaConservazione)
        {
            bool result = false;
            try
            {
                result = BusinessLogic.Documenti.areaConservazioneManager.checkItemEsibizionePresenteInIstanzaEsibizione(id_profile, id_project, type, infoUtente, idIstanzaConservazione);
            }
            catch (Exception e)
            {
                result = false;
            }
            return result;
        }

        /// <summary>
        /// indica se i file sono memorizzati sullo storage remoto o in locale
        /// </summary>
        /// <returns></returns>
        [WebMethod()]
        public virtual bool isLocalStore()
        {
            bool localStore = true;
            string valoreChiaveDB = string.Empty;
            string valoreChiaveWebConfig = string.Empty;

            // Recupero valore chiave di DB
            try
            {
                valoreChiaveDB = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_CONSERVAZIONE_REMOTE_STORAGE_URL");
            }
            catch (Exception e)
            {
                valoreChiaveDB = string.Empty;
            }

            // Recupero Valore chiave di webConfig
            try
            {
                valoreChiaveWebConfig = System.Configuration.ConfigurationManager.AppSettings["CONSERVAZIONE_REMOTE_STORAGE_URL"].ToString();
            }
            catch (Exception ex) 
            {
                valoreChiaveWebConfig = string.Empty;
            }

            if (!string.IsNullOrEmpty(valoreChiaveWebConfig) || !string.IsNullOrEmpty(valoreChiaveDB))
            {
                localStore = false;
            }

            return localStore;
        }

        [WebMethod()]
        public string GetIdCorrGlobaliEsibizione(string idGruppo)
        {
            string retVal = string.Empty;
            try
            {
                DocsPaConsManager manager = new DocsPaConsManager();
                retVal = manager.GetIdCorrGlobaliEsibizione(idGruppo);
            }
            catch (Exception ex)
            {
                logger.Debug("Errore in DocsPaConservazioneWS.asmx - Metodo GetIdCorrGlobaliEsibizione - " + ex.Message);
            }

            return retVal;
        }
        // End Mev Cs 1.4 - Esibizione
        //

        /// <summary>
        /// Verifica la presenza di un item esibizione
        /// </summary>
        /// <param name="idConservazione"></param>
        /// <param name="idSupporto"></param>       
        /// <param name="infoUtente"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        [WebMethod()]
        public bool rigeneraIstanza(string idConservazione, string idSupporto, DocsPaVO.utente.InfoUtente infoUtente, out string  message)
        {
            message = "";
            bool result = false;
            try
            {
                //string message = "";
                DocsPaConservazione.DocsPaConsManager docsPaConsM = new DocsPaConsManager();
                result = docsPaConsM.processoRigeneraIstanza(idConservazione, idSupporto, infoUtente, out message, HttpContext.Current);
            }
            catch (Exception e)
            {
                result = false;
            }
            return result;
        }

        [WebMethod()]
        public bool isIstanzaRigenerata(string idConservazione, DocsPaVO.utente.InfoUtente infoUtente)
        {
            
            bool result = false;
            try
            {
                //string message = "";
                DocsPaConservazione.DocsPaConsManager docsPaConsM = new DocsPaConsManager();
                result = docsPaConsM.isIstanzaRigenerata(idConservazione);
            }
            catch (Exception e)
            {
                result = false;
            }
            return result;
        }

        // MEV CS 1.5 - Alert Conservazione

        /// <summary>
        /// Metodo per il recupero da DB di chiavi di configurazione
        /// </summary>
        /// <param name="idAmm">id dell'amministrazione, 0=chiave globale</param>
        /// <param name="key">chiave da reperire</param>
        /// <returns></returns>
        [WebMethod()]
        public string GetChiaveConfigurazione(string idAmm, string key)
        {

            string retVal = string.Empty;
            if (string.IsNullOrEmpty(idAmm))
                idAmm = "0";

            //cerco la chiave nel db
            try
            {
                retVal = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(idAmm, key);
            }
            catch (Exception ex)
            {
                retVal = string.Empty;
            }

            return retVal;
        }

        [WebMethod()]
        public bool IsAlertConservazioneAttivo(string idAmm, string codice)
        {
            try
            {
                AlertManager manager = new AlertManager();
                return manager.isAlertAttivo(idAmm, codice);
            }
            catch (Exception ex)
            {
                logger.Debug("Errore in DocsPaConservazioneWS.asmx - Metodo: IsAlertConservazioneAttivo - ", ex);
                return false;
            }

        }

        /// <summary>
        /// Metodo per il recupero dei parametri di configurazione di un dato alert di conservazione
        /// </summary>
        /// <param name="idAmm"></param>
        /// <param name="codice"></param>
        /// <returns></returns>
        [WebMethod()]
        public string GetParametriAlertConservazione(string idAmm, string codice)
        {
            try
            {
                AlertManager manager = new AlertManager();
                return manager.getParametriAlert(idAmm, codice);
            }
            catch (Exception ex)
            {
                logger.Debug("Errore in DocsPaConservazioneWS.asmx - Metodo: GetParametriAlertConservazione - ", ex);
                return string.Empty;
            }

        }

        /// <summary>
        /// Metodo per l'invio degli alert della conservazione.
        /// Per gli alert con contatore, viene prima eseguita una verifica sul raggiungimento
        /// delle condizioni richieste per l'invio.
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="codice"></param>
        /// <param name="idIstanza"></param>
        /// <param name="idSupporto"></param>
        [WebMethod()]
        public void InvioAlertAsync(InfoUtente infoUtente, string codice, string idIstanza, string idSupporto)
        {
            try
            {
                AlertManager.InvioAlertAsync(infoUtente, codice, idIstanza, idSupporto);
                //AlertManager manager = new AlertManager();
                //manager.InvioAlert(infoUtente, codice, idIstanza, idSupporto);
                
            }
            catch (Exception ex)
            {
                logger.Debug("Errore in DocsPaConservazioneWS.asmx - Metodo: InvioAlertAsync - ", ex);
            }
        }

        /// <summary>
        /// Registrazione dell'esito della verifica di leggibilità di un supporto rimovibile registrato
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idIstanza"></param>
        /// <param name="idSupporto"></param>
        /// <param name="esitoVerifica"></param>
        /// <param name="dataProssimaVerifica"></param>
        [WebMethod()]
        public void RegistraEsitoVerificaLeggibilitaSupportoRegistrato(
                                    DocsPaVO.utente.InfoUtente infoUtente,
                                    string idIstanza,
                                    string idSupporto,
                                    bool esitoVerifica,
                                    string percentualeVerifica,
                                    string dataProssimaVerifica,
                                    string noteDiVerifica,
                                    string tipoVerifica)
        {
            DocsPaConservazione.DocsPaConsManager consManager = new DocsPaConservazione.DocsPaConsManager();

            consManager.RegistraEsitoVerificaLeggibilitaSupportoRegistrato(infoUtente, idIstanza, idSupporto, esitoVerifica, percentualeVerifica, dataProssimaVerifica, noteDiVerifica, tipoVerifica);
        }

        /// <summary>
        /// Determina se una verifica di leggibilità viene eseguita prima dei termini previsti
        /// </summary>
        /// <param name="idConservazione"></param>
        /// <param name="idSupporto"></param>
        /// <param name="idAmm"></param>
        /// <returns></returns>
        [WebMethod()]
        public bool IsVerificaLeggibilitaAnticipata(string idConservazione, string idSupporto, string idAmm)
        {
            AlertManager manager = new AlertManager();
            return manager.isVerificaLeggibilitaAnticipata(idConservazione, idSupporto, idAmm);
        }

        // End MEV CS 1.5 - Alert Conservazione
    }
}
