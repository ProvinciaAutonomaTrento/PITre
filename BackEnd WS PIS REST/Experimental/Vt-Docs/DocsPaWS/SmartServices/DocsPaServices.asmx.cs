using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Serialization;
using System.Xml;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DocsPaVO.ricerche;
using log4net;

namespace DocsPaWS.SmartServices
{
    /// <summary>
    /// Summary description for DocsPaServices
    /// </summary>
    [XmlInclude(typeof(DocsPaVO.utente.RaggruppamentoFunzionale))]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [WebService(Namespace = "http://tempuri.org")]
    [ToolboxItem(false)]
    public class DocsPaServices : System.Web.Services.WebService
    {
        private static ILog logger = LogManager.GetLogger(typeof(DocsPaServices));
        #region Base Services
        [WebMethod]
        public virtual DocsPaVO.utente.Utente Login(DocsPaVO.utente.UserLogin userLogin)
        {
            ServiceLoadController.LoadControllerManager.CheckInterval("SmartServices", "Login");
            DocsPaVO.utente.Utente utente = null;
            string idWebSession = string.Empty;
            string ipAddress = string.Empty;
            DocsPaVO.utente.UserLogin.LoginResult loginResult = DocsPaVO.utente.UserLogin.LoginResult.OK;
            try
            {
                utente = BusinessLogic.Utenti.Login.loginMethod(userLogin, out loginResult, true, idWebSession, out ipAddress);
            }
            catch (Exception e)
            {
                logger.Debug("Errore durante la Login.", e);
            }

            return utente;
        }

        [WebMethod]
        public virtual bool Logoff(DocsPaVO.utente.Utente utente)
        {
            try
            {
                utente = BusinessLogic.Utenti.UserManager.getUtente(utente.idPeople);
                BusinessLogic.Utenti.Login.logoff(utente.userId, utente.idAmministrazione, utente.sessionID, utente.dst);
                BusinessLogic.UserLog.UserLog.WriteLog(utente.userId, null, null, utente.idAmministrazione, "LOGOFF", null, null, DocsPaVO.Logger.CodAzione.Esito.OK, null);
                return true;
            }
            catch (Exception e)
            {
                logger.Debug("Errore durante il Logoff.", e);
                return false;
            }
        }

        [WebMethod]
        public virtual string GetToken(DocsPaVO.utente.Utente utente, DocsPaVO.utente.Ruolo ruolo)
        {
            ServiceLoadController.LoadControllerManager.CheckInterval("SmartServices", "GetToken");

            //Controllo Correttezza Ruolo
            bool okRuolo = false;
            foreach (DocsPaVO.utente.Ruolo rl in utente.ruoli)
            {
                if (rl.idGruppo == ruolo.idGruppo)
                    okRuolo = true;
            }

            if (okRuolo)
            {
                string tokenDiAutenticazione = null;
                try
                {
                    string clearToken = string.Empty;
                    clearToken += ruolo.systemId + "|";
                    clearToken += utente.idPeople + "|";
                    clearToken += ruolo.idGruppo + "|";
                    clearToken += utente.dst + "|";
                    clearToken += utente.idAmministrazione + "|";
                    clearToken += utente.userId + "|";
                    clearToken += utente.sede + "|";
                    clearToken += utente.urlWA;

                    tokenDiAutenticazione = Utils.Encrypt(clearToken);
                }
                catch (Exception e)
                {
                    logger.Debug("Errore durante il GetInfoUtente.", e);
                }

                return tokenDiAutenticazione;
            }
            else
            {
                logger.Debug("L'utente : " + utente.descrizione + " non appartiene al ruolo : " + ruolo.descrizione);
                return null;
            }
        }
        #endregion Base Services

        #region Documento
        [WebMethod]
        public virtual DocsPaVO.documento.SchedaDocumento DocumentoProtocolla(string tokenDiAutenticazione, DocsPaVO.documento.SchedaDocumento schedaDocumento, out DocsPaVO.documento.ResultProtocollazione risultatoProtocollazione)
        {
            logger.Info("BEGIN");
            ServiceLoadController.LoadControllerManager.CheckInterval("SmartServices", "DocumentoProtocolla");

            DocsPaVO.documento.SchedaDocumento result = null;
            string VarDescOggetto = string.Empty;
            string WebMethodName;
            risultatoProtocollazione = DocsPaVO.documento.ResultProtocollazione.OK;

            if (schedaDocumento.systemId != null)
                WebMethodName = "DOCUMENTOPROTOCOLLA2";
            else
                WebMethodName = "DOCUMENTOPROTOCOLLA";

            DocsPaVO.utente.InfoUtente infoUtente = Utils.getInfoUtenteFromToken(tokenDiAutenticazione);
            DocsPaVO.utente.Ruolo ruolo = Utils.getRuoloFromInfoUtente(infoUtente);

            try
            {
                result = BusinessLogic.Documenti.ProtoManager.protocolla(schedaDocumento, ruolo, infoUtente, out risultatoProtocollazione);
                result = schedaDocumento;
                if (result != null)
                {
                    if (schedaDocumento.protocollo != null)
                        VarDescOggetto = string.Format("{0}{1} / {2}{3}", "N.ro Doc.: ", schedaDocumento.docNumber, "Segnatura: ", schedaDocumento.protocollo.segnatura);
                    else
                        VarDescOggetto = string.Format("{0}{1}", "N.ro Doc.: ", schedaDocumento.docNumber);

                    BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, WebMethodName, schedaDocumento.systemId, VarDescOggetto, DocsPaVO.Logger.CodAzione.Esito.OK);
                }
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaServices.asmx  - metodo: DocumentoProtocolla", e);
                if (risultatoProtocollazione != DocsPaVO.documento.ResultProtocollazione.DOCUMENTO_GIA_PROTOCOLLATO)
                {
                    risultatoProtocollazione = DocsPaVO.documento.ResultProtocollazione.APPLICATION_ERROR;
                    result = null;
                }
                else
                {
                    risultatoProtocollazione = DocsPaVO.documento.ResultProtocollazione.DOCUMENTO_GIA_PROTOCOLLATO;
                    result = BusinessLogic.Documenti.DocManager.getDettaglio(infoUtente, schedaDocumento.systemId, schedaDocumento.docNumber);
                }

                BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, WebMethodName, null, VarDescOggetto, DocsPaVO.Logger.CodAzione.Esito.KO);
            }
            logger.Info("END");
            return result;
        }

        [WebMethod]
        public virtual DocsPaVO.documento.SchedaDocumento DocumentoProtocollaClassFasc(string tokenDiAutenticazione, DocsPaVO.documento.SchedaDocumento schedaDocumento, string codFascicolo, out DocsPaVO.documento.ResultProtocollazione risultatoProtocollazione)
        {
            ServiceLoadController.LoadControllerManager.CheckInterval("SmartServices", "DocumentoProtocollaClassFasc");
            DocsPaVO.documento.SchedaDocumento result = null;
            string VarDescOggetto = string.Empty;
            string WebMethodName;
            risultatoProtocollazione = DocsPaVO.documento.ResultProtocollazione.OK;
            DocsPaVO.fascicolazione.Fascicolo fasc = null;
            bool fascicolato = false;




            if (schedaDocumento.systemId != null)
                WebMethodName = "DOCUMENTOPROTOCOLLA2";
            else
                WebMethodName = "DOCUMENTOPROTOCOLLA";

            DocsPaVO.utente.InfoUtente infoUtente = Utils.getInfoUtenteFromToken(tokenDiAutenticazione);
            DocsPaVO.utente.Ruolo ruolo = Utils.getRuoloFromInfoUtente(infoUtente);

            try
            {

                fasc = BusinessLogic.Fascicoli.FascicoloManager.getFascicoloDaCodice(infoUtente, codFascicolo, schedaDocumento.registro, false, false);
                if (fasc == null)
                {
                    risultatoProtocollazione = DocsPaVO.documento.ResultProtocollazione.FASCICOLO_NON_TROVATO;
                    throw new Exception("Attenzione, non è stato possibile trovare il fascicolo o perchè non esiste o perchè non si possiedono i diritti per poterlo visualizzare");
                }



                result = BusinessLogic.Documenti.ProtoManager.protocolla(schedaDocumento, ruolo, infoUtente, out risultatoProtocollazione);
                result = schedaDocumento;
                if (result != null)
                {
                    if (schedaDocumento.protocollo != null)
                        VarDescOggetto = string.Format("{0}{1} / {2}{3}", "N.ro Doc.: ", schedaDocumento.docNumber, "Segnatura: ", schedaDocumento.protocollo.segnatura);
                    else
                        VarDescOggetto = string.Format("{0}{1}", "N.ro Doc.: ", schedaDocumento.docNumber);

                    BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, WebMethodName, schedaDocumento.systemId, VarDescOggetto, DocsPaVO.Logger.CodAzione.Esito.OK);
                }
                if (result != null && fasc != null && !string.IsNullOrEmpty(fasc.systemID))
                {
                    string msg = string.Empty;
                    fascicolato = BusinessLogic.Fascicoli.FascicoloManager.addDocFascicolo(infoUtente, schedaDocumento.systemId, fasc.systemID, true, out msg);
                    if (!fascicolato)
                    {
                        risultatoProtocollazione = DocsPaVO.documento.ResultProtocollazione.ERRORE_DURANTE_LA_FASCICOLAZIONE;
                        throw new Exception("Non è stato possibile fascicolare il documento. riprovare più tardi.");
                    }
                    else
                    {
                        BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "FASCICOLOADDDOC", fasc.systemID, "Inserimento doc " + schedaDocumento.systemId + " in fascicolo: " + fasc.systemID, DocsPaVO.Logger.CodAzione.Esito.OK);
                        BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCADDINFASC", schedaDocumento.systemId, "Inserimento doc " + schedaDocumento.systemId + " in fascicolo: " + fasc.systemID, DocsPaVO.Logger.CodAzione.Esito.OK);

                    }
                }

            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaServices.asmx  - metodo: DocumentoProtocolla", e);
                if (risultatoProtocollazione != DocsPaVO.documento.ResultProtocollazione.DOCUMENTO_GIA_PROTOCOLLATO &&
                    risultatoProtocollazione != DocsPaVO.documento.ResultProtocollazione.ERRORE_DURANTE_LA_FASCICOLAZIONE
                    && risultatoProtocollazione != DocsPaVO.documento.ResultProtocollazione.FASCICOLO_NON_TROVATO)
                {
                    risultatoProtocollazione = DocsPaVO.documento.ResultProtocollazione.APPLICATION_ERROR;
                    result = null;
                }
                else
                {
                    risultatoProtocollazione = DocsPaVO.documento.ResultProtocollazione.DOCUMENTO_GIA_PROTOCOLLATO;
                    result = BusinessLogic.Documenti.DocManager.getDettaglio(infoUtente, schedaDocumento.systemId, schedaDocumento.docNumber);
                }

                BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, WebMethodName, null, VarDescOggetto, DocsPaVO.Logger.CodAzione.Esito.KO);
            }

            return result;
        }



        [WebMethod]
        public virtual DocsPaVO.documento.SchedaDocumento DocumentoAddDocGrigia(string tokenDiAutenticazione, DocsPaVO.documento.SchedaDocumento schedaDocumento)
        {
            logger.Info("BEGIN");
            ServiceLoadController.LoadControllerManager.CheckInterval("SmartServices", "DocumentoAddDocGrigia");
            DocsPaVO.documento.SchedaDocumento result = null;
            try
            {
                DocsPaVO.utente.InfoUtente infoUtente = Utils.getInfoUtenteFromToken(tokenDiAutenticazione);
                DocsPaVO.utente.Ruolo ruolo = Utils.getRuoloFromInfoUtente(infoUtente);
                result = BusinessLogic.Documenti.DocSave.addDocGrigia(schedaDocumento, infoUtente, ruolo);
                BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCUMENTOADDDOCGRIGIA", schedaDocumento.systemId, string.Format("{0} {1}", "N.ro Doc.: ", schedaDocumento.docNumber), DocsPaVO.Logger.CodAzione.Esito.OK);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaServices.asmx  - metodo: DocumentoAddDocGrigia", e);
                result = null;
            }
            logger.Info("END");
            return result;
        }

        [WebMethod]
        public virtual DocsPaVO.documento.FileRequest DocumentoPutFile(string tokenDiAutenticazione, DocsPaVO.documento.FileRequest fileRequest, DocsPaVO.documento.FileDocumento fileDocument)
        {
            ServiceLoadController.LoadControllerManager.CheckInterval("SmartServices", "DocumentoPutFile");
            string isFirmato = " ";
            DocsPaVO.documento.FileRequest result = null;
            string errore = string.Empty;
            try
            {
                DocsPaVO.utente.InfoUtente infoUtente = Utils.getInfoUtenteFromToken(tokenDiAutenticazione);
                result = BusinessLogic.Documenti.FileManager.putFile(fileRequest, fileDocument, infoUtente);
                if (result != null)
                {

                    if (DocsPaUtils.Functions.Functions.isDocumentoFirmato(result.fileName)) //se l'estensione è p7m
                    {
                        isFirmato = " Firmato ";
                    }
                    BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCUMENTOPUTFILE", fileRequest.docNumber, string.Format("{0} {1}", "File" + isFirmato + "N.ro:", fileRequest.docNumber), DocsPaVO.Logger.CodAzione.Esito.OK);
                }
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaServices.asmx  - metodo: DocumentoPutFile", e);

                if (result != null)
                {
                    if (DocsPaUtils.Functions.Functions.isDocumentoFirmato(result.fileName)) //se l'estensione è p7m
                    {
                        isFirmato = " Firmato ";
                    }
                }
            }

            return result;
        }

        [WebMethod]
        [return: XmlArray()]
        [return: XmlArrayItem(typeof(DocsPaVO.documento.InfoDocumento))]
        public virtual ArrayList DocumentoGetQueryDocumento(string tokenDiAutenticazione, DocsPaVO.filtri.FiltroRicerca[][] queryList)
        {
            ServiceLoadController.LoadControllerManager.CheckInterval("SmartServices", "DocumentoGetQueryDocumento");
            ArrayList objListaInfoDocumenti = null;

            try
            {
                DocsPaVO.utente.InfoUtente infoUtente = Utils.getInfoUtenteFromToken(tokenDiAutenticazione);
                objListaInfoDocumenti = BusinessLogic.Documenti.InfoDocManager.getQuery(infoUtente.idGruppo, infoUtente.idPeople, setFiltriRicercaDoc(queryList));
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaServices.asmx  - metodo: DocumentoGetQueryDocumento", e);
                objListaInfoDocumenti = null;
            }

            return objListaInfoDocumenti;
        }

        [WebMethod]
        public virtual DocsPaVO.documento.SchedaDocumento DocumentoGetDettaglioDocumento(string tokenDiAutenticazione, string idProfile, string docNumber)
        {
            logger.Info("BEGIN");
            ServiceLoadController.LoadControllerManager.CheckInterval("SmartServices", "DocumentoGetDettaglioDocumento");
            DocsPaVO.documento.SchedaDocumento schedaDocumento = null;
            string VarDescOggetto = string.Empty;

            try
            {
                DocsPaVO.utente.InfoUtente infoUtente = Utils.getInfoUtenteFromToken(tokenDiAutenticazione);
                schedaDocumento = BusinessLogic.Documenti.DocManager.getDettaglio(infoUtente, idProfile, docNumber);
                if (schedaDocumento != null)
                {
                    if (schedaDocumento.protocollo != null)
                        VarDescOggetto = string.Format("{0}{1} / {2}{3}", "N.ro Doc.:", schedaDocumento.docNumber, "Segnatura: ", schedaDocumento.protocollo.segnatura);
                    else
                        VarDescOggetto = string.Format("{0}{1}", "N.ro Doc.:", schedaDocumento.docNumber);

                    BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCUMENTOGETDETTAGLIODOCUMENTO", schedaDocumento.systemId, VarDescOggetto, DocsPaVO.Logger.CodAzione.Esito.OK);
                }
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaServices.asmx  - metodo: DocumentoGetDettaglioDocumento", e);
                schedaDocumento = null;
            }
            logger.Info("END");
            return schedaDocumento;
        }

        [WebMethod]
        public virtual DocsPaVO.documento.FileDocumento DocumentoGetFile(string tokenDiAutenticazione, DocsPaVO.documento.FileRequest fileRequest)
        {
            ServiceLoadController.LoadControllerManager.CheckInterval("SmartServices", "DocumentoGetFile");
            DocsPaVO.documento.FileDocumento fileDocumento = null;

            try
            {
                DocsPaVO.utente.InfoUtente infoUtente = Utils.getInfoUtenteFromToken(tokenDiAutenticazione);
                fileDocumento = BusinessLogic.Documenti.FileManager.getFile(fileRequest, infoUtente);
                BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCUMENTOGETFILE", fileRequest.docNumber, string.Format("{0} {1} {2} {3}", "Visualizzazione Doc. nr.", fileRequest.docNumber, "Ver.", fileRequest.version), DocsPaVO.Logger.CodAzione.Esito.OK);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaServices.asmx  - metodo: DocumentoGetFile", e);
                fileDocumento = null;
            }

            return fileDocumento;
        }

        [WebMethod]
        public virtual DocsPaVO.documento.FileRequest DocumentoAggiungiVersione(string tokenDiAutenticazione, DocsPaVO.documento.FileRequest fileRequest)
        {
            ServiceLoadController.LoadControllerManager.CheckInterval("SmartServices", "DocumentoAggiungiVersione");
            DocsPaVO.documento.FileRequest fileReq = null;

            try
            {
                DocsPaVO.utente.InfoUtente infoUtente = Utils.getInfoUtenteFromToken(tokenDiAutenticazione);
                fileReq = BusinessLogic.Documenti.VersioniManager.addVersion(fileRequest, infoUtente, false);
                if (fileReq != null)
                    BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCUMENTOAGGIUNGIVERSIONE", fileReq.docNumber, string.Format("{0}{1}{2}{3}", "Aggiunta al N.ro Doc.: ", fileReq.docNumber, " la Ver. ", fileReq.version), DocsPaVO.Logger.CodAzione.Esito.OK);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaServices.asmx  - metodo: DocumentoAggiungiVersione", e);
                fileReq = null;
            }

            return fileReq;
        }

        [WebMethod]
        public virtual DocsPaVO.documento.Allegato DocumentoAggiungiAllegato(string tokenDiAutenticazione, DocsPaVO.documento.Allegato allegato)
        {
            ServiceLoadController.LoadControllerManager.CheckInterval("SmartServices", "DocumentoAggiungiAllegato");
            DocsPaVO.documento.Allegato res = null;

            try
            {
                DocsPaVO.utente.InfoUtente infoUtente = Utils.getInfoUtenteFromToken(tokenDiAutenticazione);
                res = BusinessLogic.Documenti.AllegatiManager.aggiungiAllegato(infoUtente, allegato);
                if (res != null)
                {
                    BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCUMENTOAGGIUNGIALLEGATO", allegato.docNumber, string.Format("{0}{1}{2}{3}", "Aggiunto al N.ro Doc.: ", res.docNumber, " il N.ro Allegato: ", res.versionLabel), DocsPaVO.Logger.CodAzione.Esito.OK);
                    BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCNEWALLEGATO", allegato.docNumber, string.Format("{0}{1}{2}{3}", "Aggiunto al N.ro Doc.: ", res.docNumber, " il N.ro Allegato: ", res.versionLabel), DocsPaVO.Logger.CodAzione.Esito.OK);
                }
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaServices.asmx  - metodo: DocumentoAggiungiAllegato", e);
                res = null;
            }

            return res;
        }

        [WebMethod]
        public virtual DocsPaVO.documento.SchedaDocumento DocumentoSaveDocumento(string tokenDiAutenticazione, DocsPaVO.documento.SchedaDocumento schedaDocumento, bool enableUffRef, out bool daAggiornareUffRef)
        {
            logger.Info("BEGIN");
            ServiceLoadController.LoadControllerManager.CheckInterval("SmartServices", "DocumentoSaveDocumento");
            DocsPaVO.documento.SchedaDocumento result = null;
            daAggiornareUffRef = false;
            try
            {
                bool docModOggetto = schedaDocumento.oggetto.daAggiornare;
                bool docModMitt = false;
                bool docModDest = false;
                bool docModDestCC = false;
                DocsPaVO.utente.InfoUtente infoUtente = Utils.getInfoUtenteFromToken(tokenDiAutenticazione);
                DocsPaVO.utente.Ruolo ruolo = Utils.getRuoloFromInfoUtente(infoUtente);


                switch (schedaDocumento.tipoProto)
                {
                    case "A":
                        if (((DocsPaVO.documento.ProtocolloEntrata)schedaDocumento.protocollo).daAggiornareMittente)
                            docModMitt = true;
                        break;

                    case "P":
                        if (((DocsPaVO.documento.ProtocolloUscita)schedaDocumento.protocollo).daAggiornareMittente)
                            docModMitt = true;
                        if (((DocsPaVO.documento.ProtocolloUscita)schedaDocumento.protocollo).daAggiornareDestinatari)
                            docModDest = true;
                        if (((DocsPaVO.documento.ProtocolloUscita)schedaDocumento.protocollo).daAggiornareDestinatariConoscenza)
                            docModDest = true;
                        break;

                    case "I":
                        if (((DocsPaVO.documento.ProtocolloInterno)schedaDocumento.protocollo).daAggiornareMittente)
                            docModMitt = true;
                        if (((DocsPaVO.documento.ProtocolloInterno)schedaDocumento.protocollo).daAggiornareDestinatari)
                            docModDest = true;
                        if (((DocsPaVO.documento.ProtocolloInterno)schedaDocumento.protocollo).daAggiornareDestinatariConoscenza)
                            docModDest = true;
                        break;
                }

                result = BusinessLogic.Documenti.DocSave.save(infoUtente, schedaDocumento, enableUffRef, out daAggiornareUffRef, ruolo);

                if (result != null)
                {
                    if (docModOggetto || docModDest || docModMitt || docModDestCC)
                    {
                        if (docModOggetto)
                            if (schedaDocumento.tipoProto != "G")
                                BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCUMENTOMODOGGETTO", schedaDocumento.systemId, "Modifica oggetto del Protocollo " + schedaDocumento.protocollo.segnatura, DocsPaVO.Logger.CodAzione.Esito.OK);
                            else
                                BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCUMENTOMODOGGETTO", schedaDocumento.systemId, "Modifica oggetto del Documento " + result.docNumber, DocsPaVO.Logger.CodAzione.Esito.OK);
                        if (docModDest)
                            BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCUMENTOMODMITTDEST", schedaDocumento.systemId, "Modifica Destinatari sul Protocollo " + schedaDocumento.protocollo.segnatura, DocsPaVO.Logger.CodAzione.Esito.OK);
                        if (docModDestCC)
                            BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCUMENTOMODMITTDEST", schedaDocumento.systemId, "Modifica Destinatari CC sul Protocollo " + schedaDocumento.protocollo.segnatura, DocsPaVO.Logger.CodAzione.Esito.OK);
                        if (docModMitt)
                            BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCUMENTOMODMITTDEST", schedaDocumento.systemId, "Modifica Mittente sul Protocollo " + schedaDocumento.protocollo.segnatura, DocsPaVO.Logger.CodAzione.Esito.OK);
                    }
                    else
                    {
                        if (schedaDocumento.tipoProto != "G")
                            BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCUMENTOSAVEDOCUMENTO", schedaDocumento.systemId, string.Format("{0} {1}", "Aggiornamento Protocollo Numero ", schedaDocumento.protocollo.segnatura), DocsPaVO.Logger.CodAzione.Esito.OK);
                        else
                            BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCUMENTOSAVEDOCUMENTO", schedaDocumento.systemId, string.Format("{0} {1}", "Aggiornamento Documento Numero ", result.docNumber), DocsPaVO.Logger.CodAzione.Esito.OK);
                    }
                }
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaServices.asmx  - metodo: DocumentoSaveDocumento", e);
                result = null;
            }
            logger.Info("END");
            return result;
        }

        [WebMethod]
        public virtual DocsPaVO.documento.FileDocumento DocumentoAggiungiEtichettaPDF(string tokenDiAutenticazione, DocsPaVO.documento.SchedaDocumento schedaDocumento, DocsPaVO.documento.labelPdf position)
        {
            ServiceLoadController.LoadControllerManager.CheckInterval("SmartServices", "DocumentoAggiungiEtichettaPDF");
            DocsPaVO.documento.FileDocumento resultFileDocumento = null;
            try
            {
                DocsPaVO.utente.InfoUtente infoUtente = Utils.getInfoUtenteFromToken(tokenDiAutenticazione);
                string xmlPath = this.Server.MapPath("XML/labelPdf.xml");
                DocsPaVO.documento.FileRequest fileRequest = (DocsPaVO.documento.FileRequest)schedaDocumento.documenti[0];
                resultFileDocumento = BusinessLogic.Documenti.FileManager.getFileConSegnatura(fileRequest, schedaDocumento, infoUtente, xmlPath, position,false);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaServices.asmx  - metodo: DocumentoAggiungiEtichettaPDF", e);
                resultFileDocumento = null;
            }
            return resultFileDocumento;
        }
        #endregion Documento

        #region Fascicolo
        [WebMethod]
        [return: XmlArray()]
        [return: XmlArrayItem(typeof(DocsPaVO.fascicolazione.Classificazione))]
        public virtual ArrayList FascicolazioneGetClassificazione(string tokenDiAutenticazione, DocsPaVO.utente.Registro registro, string codiceClassifica, bool getFigli, string idTitolario)
        {
            ServiceLoadController.LoadControllerManager.CheckInterval("SmartServices", "FascicolazioneGetClassificazione");
            ArrayList result = null;

            try
            {
                DocsPaVO.utente.InfoUtente infoUtente = Utils.getInfoUtenteFromToken(tokenDiAutenticazione);
                result = BusinessLogic.Fascicoli.TitolarioManager.getTitolario2(infoUtente.idAmministrazione, infoUtente.idGruppo, infoUtente.idPeople, registro, codiceClassifica, getFigli, idTitolario);
                return result;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaServices.asmx  - metodo: FascicolazioneGetClassificazione", e);
                result = null;
            }

            return result;
        }

        [WebMethod]
        public virtual string FascicolazioneGetFascNumRif(string tokenDiAutenticazione, string idTitolario, string idRegistro)
        {
            ServiceLoadController.LoadControllerManager.CheckInterval("SmartServices", "FascicolazioneGetFascNumRif");
            string numFascicolo = "";
            try
            {
                DocsPaVO.utente.InfoUtente infoUtente = Utils.getInfoUtenteFromToken(tokenDiAutenticazione);
                numFascicolo = BusinessLogic.Fascicoli.FascicoloManager.getFascNumRif(idTitolario, idRegistro);
                return numFascicolo;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaServices.asmx  - metodo: FascicolazioneGetFascNumRif", e);
                return null;
            }
        }

        [WebMethod]
        public virtual DocsPaVO.fascicolazione.Fascicolo FascicolazioneNewFascicolo(string tokenDiAutenticazione, DocsPaVO.fascicolazione.Classificazione classificazione, DocsPaVO.fascicolazione.Fascicolo fascicolo, bool enableUffRef, out DocsPaVO.fascicolazione.ResultCreazioneFascicolo resultCreazione)
        {
            ServiceLoadController.LoadControllerManager.CheckInterval("SmartServices", "FascicolazioneNewFascicolo");
            DocsPaVO.fascicolazione.Fascicolo objFascicolo = null;
            resultCreazione = DocsPaVO.fascicolazione.ResultCreazioneFascicolo.OK;
            try
            {
                DocsPaVO.utente.InfoUtente infoUtente = Utils.getInfoUtenteFromToken(tokenDiAutenticazione);
                DocsPaVO.utente.Ruolo ruolo = Utils.getRuoloFromInfoUtente(infoUtente);
                objFascicolo = BusinessLogic.Fascicoli.FascicoloManager.newFascicolo(classificazione, fascicolo, infoUtente, ruolo, enableUffRef, out resultCreazione);
                if (objFascicolo != null)
                    BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "FASCICOLAZIONENEWFASCICOLO", objFascicolo.systemID, string.Format("{0} {1}", "Cod. Fascicolo:", objFascicolo.codice), DocsPaVO.Logger.CodAzione.Esito.OK);
            }
            catch (Exception e)
            {
                resultCreazione = DocsPaVO.fascicolazione.ResultCreazioneFascicolo.GENERIC_ERROR;
                logger.Debug("Errore in DocsPaServices.asmx  - metodo: FascicolazioneNewFascicolo", e);
                objFascicolo = null;
            }

            return objFascicolo;
        }

        [WebMethod]
        public virtual DocsPaVO.fascicolazione.Folder FascicolazioneGetFolder(string tokenDiAutenticazione, DocsPaVO.fascicolazione.Fascicolo fascicolo)
        {
            ServiceLoadController.LoadControllerManager.CheckInterval("SmartServices", "FascicolazioneGetFolder");
            DocsPaVO.fascicolazione.Folder objFolder = null;

            try
            {
                DocsPaVO.utente.InfoUtente infoUtente = Utils.getInfoUtenteFromToken(tokenDiAutenticazione);
                objFolder = BusinessLogic.Fascicoli.FolderManager.getFolder(infoUtente.idPeople, infoUtente.idGruppo, fascicolo);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaServices.asmx  - metodo: FascicolazioneGetFolder", e);
                objFolder = null;
            }

            return objFolder;
        }

        [WebMethod]
        [return: XmlArray()]
        [return: XmlArrayItem(typeof(DocsPaVO.documento.InfoDocumento))]
        public virtual ArrayList FascicolazioneGetDocumenti(string tokenDiAutenticazione, DocsPaVO.fascicolazione.Folder folder)
        {
            ServiceLoadController.LoadControllerManager.CheckInterval("SmartServices", "FascicolazioneGetDocumenti");
            ArrayList objListaDocumenti = null;

            try
            {
                DocsPaVO.utente.InfoUtente infoUtente = Utils.getInfoUtenteFromToken(tokenDiAutenticazione);
                objListaDocumenti = BusinessLogic.Fascicoli.FolderManager.getDocumenti(infoUtente.idGruppo, infoUtente.idPeople, folder);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaServices.asmx  - metodo: FascicolazioneGetDocumenti", e);
                objListaDocumenti = null;
            }

            return objListaDocumenti;
        }

        [WebMethod]
        [return: XmlArray()]
        [return: XmlArrayItem(typeof(DocsPaVO.fascicolazione.Fascicolo))]
        public virtual ArrayList FascicolazioneGetListaFascicoliDaCodice(string tokenDiAutenticazione, string codiceFascicolo, DocsPaVO.utente.Registro registro, bool enableUffRef, bool enableProfilazione)
        {
            ServiceLoadController.LoadControllerManager.CheckInterval("SmartServices", "FascicolazioneGetListaFascicoliDaCodice");
            ArrayList result = null;

            try
            {
                DocsPaVO.utente.InfoUtente infoUtente = Utils.getInfoUtenteFromToken(tokenDiAutenticazione);
                result = BusinessLogic.Fascicoli.FascicoloManager.getListaFascicoliDaCodice(infoUtente, codiceFascicolo, registro, enableUffRef, enableProfilazione, "R");
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaServices.asmx  - metodo: FascicolazioneGetListaFascicoliDaCodice", e);
                result = null;
            }

            return result;
        }



        [WebMethod]
        public virtual bool FascicolazioneAddDocFolder(string tokenDiAutenticazione, string idProfile, string idFolder, out bool outValue)
        {
            ServiceLoadController.LoadControllerManager.CheckInterval("SmartServices", "FascicolazioneAddDocFolder");
            bool result = true;
            outValue = false;
            string msg = string.Empty;

            try
            {
                DocsPaVO.utente.InfoUtente infoUtente = Utils.getInfoUtenteFromToken(tokenDiAutenticazione);
                result = BusinessLogic.Fascicoli.FolderManager.addDocFolder(infoUtente, idProfile, idFolder, false, out msg);
                BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "FOLDERADDDOC", idFolder, "Inserimento doc " + idProfile + " in folder: " + idFolder, DocsPaVO.Logger.CodAzione.Esito.OK);
                BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCADDINFOLDER", idProfile, "Inserimento doc " + idProfile + " in folder: " + idFolder, DocsPaVO.Logger.CodAzione.Esito.OK);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaServices.asmx  - metodo: FascicolazioneAddDocFolder", e);
                result = false;
            }

            return result;
        }

        /// <summary> 
        /// </summary> 
        [WebMethod]
        [return: XmlArray()]
        [return: XmlArrayItem(typeof(DocsPaVO.fascicolazione.Fascicolo))]
        public virtual ArrayList FascicolazioneGetFascicoliDaDoc(string tokenDiAutenticazione, string idProfile)
        {
            ServiceLoadController.LoadControllerManager.CheckInterval("SmartServices", "FascicolazioneAddDocFolder");

            ArrayList result = null;

            try
            {
                //gestione tracer 
                #if TRACE_WS 
                    DocsPaUtils.LogsManagement.PerformaceTracer pt = new DocsPaUtils.LogsManagement.PerformaceTracer("TRACE_WS");
                #endif

                DocsPaVO.utente.InfoUtente infoUtente = Utils.getInfoUtenteFromToken(tokenDiAutenticazione);
                
                if (idProfile != null)
                {
                    result = BusinessLogic.Fascicoli.FascicoloManager.getFascicoliDaDoc(infoUtente, idProfile);
                    #if TRACE_WS 
                        pt.WriteLogTracer("FascicolazioneGetFascicoliDaDoc"); 
                    #endif
                }
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaSmartServices.asmx  - metodo: FascicolazioneGetFascicoliDaDoc", e);
                result = null;
            }

            return result;
        }

        [WebMethod]
        [return: XmlArray()]
        [return: XmlArrayItem(typeof(DocsPaVO.fascicolazione.Fascicolo))]
        public virtual ArrayList FascicolazioneGetListaFascicoli(string tokenDiAutenticazione, DocsPaVO.fascicolazione.Classificazione classificazione, DocsPaVO.filtri.FiltroRicerca[] listaFiltri, bool enableUfficioRef, bool enableProfilazione, bool childs)
        {
            ArrayList result = null;
            try
            {
                DocsPaVO.utente.InfoUtente infoUtente = Utils.getInfoUtenteFromToken(tokenDiAutenticazione);
                result = BusinessLogic.Fascicoli.FascicoloManager.getListaFascicoli(infoUtente, classificazione, listaFiltri, enableUfficioRef, enableProfilazione, childs, null, null, String.Empty);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaServices.asmx  - metodo: FascicolazioneGetListaFascicoli", e);
                result = null;
            }

            return result;
        }

        #endregion Fascicolo

        #region Rubrica
        [WebMethod]
        public virtual DocsPaVO.utente.Corrispondente RubricaGetCorrispondenteByCodRubricaIE(string tokenDiAutenticazione, string codice, DocsPaVO.addressbook.TipoUtente tipoIE)
        {
            ServiceLoadController.LoadControllerManager.CheckInterval("SmartServices", "RubricaGetCorrispondenteByCodRubricaIE");

            DocsPaVO.utente.Corrispondente corr = null;
            try
            {
                DocsPaVO.utente.InfoUtente infoUtente = Utils.getInfoUtenteFromToken(tokenDiAutenticazione);
                corr = BusinessLogic.Utenti.UserManager.getCorrispondenteByCodRubrica(codice, tipoIE, infoUtente);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaServices.asmx  - metodo: RubricaGetCorrispondenteByCodRubricaIE - ", e);
            }
            return corr;
        }

        [WebMethod]
        [XmlInclude(typeof(DocsPaVO.rubrica.ElementoRubrica))]
        [return: XmlArray()]
        [return: XmlArrayItem(typeof(DocsPaVO.rubrica.ElementoRubrica))]
        public virtual ArrayList RubricaGetElementiRubrica(string tokenDiAutenticazione, DocsPaVO.rubrica.ParametriRicercaRubrica qc, DocsPaVO.rubrica.SmistamentoRubrica smistamentoRubrica)
        {
            ServiceLoadController.LoadControllerManager.CheckInterval("SmartServices", "RubricaGetElementiRubrica");

            ArrayList objElementiRubrica = null;

            try
            {
               // qc.caller.filtroRegistroPerRicerca = string.Empty;
                DocsPaVO.utente.InfoUtente infoUtente = Utils.getInfoUtenteFromToken(tokenDiAutenticazione);
                BusinessLogic.Rubrica.DPA3_RubricaSearchAgent ccs = new BusinessLogic.Rubrica.DPA3_RubricaSearchAgent(infoUtente);
                if (qc.caller != null && qc.caller.IdUtente == null)
                    qc.caller.IdUtente = infoUtente.idPeople;
                objElementiRubrica = ccs.Search(qc, smistamentoRubrica);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaServices.asmx  - metodo: RubricaGetElementiRubrica - ", e);
            }
            return objElementiRubrica;
        }

        [WebMethod]
        public virtual bool RubricaInsCorrisondenteEsterno(string tokenDiAutenticazione, DocsPaVO.utente.Corrispondente corrispondente, DocsPaVO.utente.Registro registro)
        {
            ServiceLoadController.LoadControllerManager.CheckInterval("SmartServices", "RubricaInsCorrisondenteEsterno");

            bool result = true;
            DocsPaVO.utente.Corrispondente resultCorr = null;
            DocsPaVO.utente.Corrispondente newCorr = null;
            string idRegistro = string.Empty;

            try
            {
                DocsPaVO.utente.InfoUtente infoUtente = Utils.getInfoUtenteFromToken(tokenDiAutenticazione);
                if (registro != null)
                    idRegistro = registro.systemId;

                switch (corrispondente.tipoCorrispondente)
                {
                    case "U":
                        newCorr = new DocsPaVO.utente.UnitaOrganizzativa();
                        break;
                    case "P":
                        newCorr = new DocsPaVO.utente.Utente();
                        ((DocsPaVO.utente.Utente)newCorr).cognome = corrispondente.cognome;
                        ((DocsPaVO.utente.Utente)newCorr).nome = corrispondente.nome;
                        break;
                }

                newCorr.codiceCorrispondente = corrispondente.codiceCorrispondente;
                newCorr.codiceRubrica = corrispondente.codiceRubrica;
                newCorr.descrizione = corrispondente.descrizione;
                newCorr.tipoCorrispondente = corrispondente.tipoCorrispondente;
                newCorr.email = corrispondente.email;
                newCorr.idAmministrazione = infoUtente.idAmministrazione;
                newCorr.idRegistro = idRegistro;
                //dati canale
                DocsPaVO.utente.Canale canale = new DocsPaVO.utente.Canale();
                if (corrispondente.canalePref != null
                    && !string.IsNullOrEmpty(corrispondente.canalePref.systemId))
                {
                    newCorr.canalePref = corrispondente.canalePref;
                }
                else
                { //default è lettera
                    DocsPaDB.Query_DocsPAWS.Utenti u=new DocsPaDB.Query_DocsPAWS.Utenti();

                    newCorr.canalePref = new DocsPaVO.utente.Canale();
                     newCorr.canalePref.systemId=    u.GetSystemIDCanale();
                         
                }
                

                //Dettagli corrispondente
                DocsPaVO.addressbook.DettagliCorrispondente dettagli = new DocsPaVO.addressbook.DettagliCorrispondente();
                dettagli.Corrispondente.AddCorrispondenteRow(
                    corrispondente.indirizzo,
                    corrispondente.citta,
                    corrispondente.cap,
                    corrispondente.prov,
                    corrispondente.nazionalita,
                    corrispondente.telefono1,
                    corrispondente.telefono2,
                    corrispondente.fax,
                    corrispondente.codfisc,
                    corrispondente.note,
                    corrispondente.localita,
                    corrispondente.luogoDINascita,
                    corrispondente.dataNascita,
                    corrispondente.titolo,
                    corrispondente.partitaiva//,
                    //corrispondente.codiceIpa
                    );
                newCorr.info = dettagli;
                newCorr.dettagli = true;

                //Inserimento corrispondente
                resultCorr = BusinessLogic.Utenti.addressBookManager.insertCorrispondente(newCorr, null);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaServices.asmx  - metodo: RubricaInsCorrisondenteEsterno - ", e);
            }

            if (resultCorr != null && resultCorr.errore == null)
                return true;
            else
                return false;
        }

        [WebMethod]
        public virtual DocsPaVO.utente.Corrispondente RubricaGetCorrispondenteBySystemId(string tokenDiAutenticazione, string system_id)
        {
            ServiceLoadController.LoadControllerManager.CheckInterval("SmartServices", "RubricaGetCorrispondenteBySystemId");

            DocsPaVO.utente.Corrispondente corr = null;
            try
            {
                DocsPaVO.utente.InfoUtente infoUtente = Utils.getInfoUtenteFromToken(tokenDiAutenticazione);
                corr = BusinessLogic.Utenti.UserManager.getCorrispondenteBySystemID(system_id);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaServices.asmx  - metodo: RubricaGetCorrispondenteBySystemId - ", e);
            }
            return corr;
        }

        [WebMethod]
        public virtual string RubricaGetCodiceCorrispondente(string tokenDiAutenticazione, string docnumber, string idOggettoCustom, string idTemplate)
        {
            ServiceLoadController.LoadControllerManager.CheckInterval("SmartServices", "RubricaGetCodiceCorrispondente");


            string codiceCorrispondente = string.Empty;

            try
            {
                DocsPaVO.utente.InfoUtente infoUtente = Utils.getInfoUtenteFromToken(tokenDiAutenticazione);
                if (infoUtente != null)
                {
                    codiceCorrispondente = BusinessLogic.Utenti.UserManager.getCodiceCorrispondente(docnumber, idOggettoCustom, idTemplate);
                }
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaService.asmx - metodo: RubricaGetCodiceCorrispondente - ", e);
            }
            return codiceCorrispondente;
        }

        [WebMethod]
        public virtual bool RubricaModCorrispondenteEsterno(string tokenDiAutenticazione, DocsPaVO.utente.DatiModificaCorr datiModificaCorr, out string message)
        {
            bool result = false;
            message = String.Empty;

            try
            {
                DocsPaVO.utente.InfoUtente infoUtente = Utils.getInfoUtenteFromToken(tokenDiAutenticazione);
                DocsPaVO.utente.Corrispondente corr = BusinessLogic.Utenti.UserManager.getCorrispondenteBySystemID(datiModificaCorr.idCorrGlobali);

                if (infoUtente != null && corr != null && corr.tipoIE == "E")
                {
                    if (corr.canalePref != null && !String.IsNullOrEmpty(corr.canalePref.systemId))
                        datiModificaCorr.idCanalePref = corr.canalePref.systemId;
                    else
                        datiModificaCorr.idCanalePref = "0";
                    result = BusinessLogic.Utenti.UserManager.ModifyCorrispondenteEsterno(datiModificaCorr, infoUtente, out message);
                }
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaService.asmx - metodo: RubricaModCorrispondenteEsterno - ", e);
            }

            return result;
        }       

        /// <summary>
        /// web method che restituisce un corrispondente estreno ricercandolo dalla mail 
        /// </summary>
        /// <param name="tokenDiAutenticazione"></param>
        /// <param name="email"></param>
        /// <param name="registro"></param>
        /// <returns>restituisce un  DocsPaVO.utente.Corrispondente o null se il processo è andato a buon fine, null con messaggio di errore diverso da string
        /// se non è andato a buon fine</returns>
        [WebMethod]
        private DocsPaVO.utente.Corrispondente RubricaGetMittenteEsternoByEmail(string tokenDiAutenticazione, string email, DocsPaVO.utente.Registro registro, out string errorMessage)
        {
            errorMessage = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(email)
                    && !string.IsNullOrEmpty(tokenDiAutenticazione)
                    && registro != null)
                {
                    DocsPaVO.utente.InfoUtente infoUtente = Utils.getInfoUtenteFromToken(tokenDiAutenticazione);
                    if (infoUtente != null)
                    {
                        return BusinessLogic.Utenti.UserManager.getCorrispondenteByEmail(email, infoUtente, registro);
                    }
                }
            }
            catch (Exception e)
            {
                errorMessage = "errore durante la ricerca del mittente";
                logger.Debug("errore nel web service getMittenteEsternoByMail - errore: " + e.Message);
            }

            return null;
        }

        private static DocsPaVO.utente.Ruolo getRuoloFromInfoUtente(DocsPaVO.utente.InfoUtente infoUtente)
        {
            try
            {
                ArrayList ruoli = BusinessLogic.Utenti.UserManager.getRuoliUtente(infoUtente.idPeople);

                foreach (DocsPaVO.utente.Ruolo ruolo in ruoli)
                {
                    if (ruolo.systemId == infoUtente.idCorrGlobali)
                        return ruolo;
                }

                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }


        #endregion Rubrica

        #region Registro
        [WebMethod]
        [return: XmlArray()]
        [return: XmlArrayItem(typeof(DocsPaVO.utente.Registro))]
        public virtual ArrayList RegistroGetRegistriRuolo(string tokenDiAutenticazione)
        {
            ServiceLoadController.LoadControllerManager.CheckInterval("SmartServices", "RegistroGetRegistriRuolo");

            ArrayList registri = null;
            try
            {
                DocsPaVO.utente.InfoUtente infoUtente = Utils.getInfoUtenteFromToken(tokenDiAutenticazione);
                DocsPaVO.utente.Ruolo ruolo = Utils.getRuoloFromInfoUtente(infoUtente);

                registri = BusinessLogic.Utenti.RegistriManager.getRegistriRuolo(ruolo.systemId);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaServices.asmx  - metodo: RegistroGetRegistriRuolo", e);
            }

            return registri;
        }
        #endregion Registro

        #region Titolario
        [WebMethod]
        [return: XmlArray()]
        [return: XmlArrayItem(typeof(DocsPaVO.amministrazione.OrgTitolario))]
        public virtual ArrayList TitolarioGetTitolariUtilizzabili(string tokenDiAutenticazione)
        {
            ServiceLoadController.LoadControllerManager.CheckInterval("SmartServices", "TitolarioGetTitolariUtilizzabili");

            ArrayList titolari = new ArrayList();
            try
            {
                DocsPaVO.utente.InfoUtente infoUtente = Utils.getInfoUtenteFromToken(tokenDiAutenticazione);
                titolari = BusinessLogic.Amministrazione.TitolarioManager.getTitolariUtilizzabili(infoUtente.idAmministrazione);
                return titolari;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaServices.asmx  - metodo: TitolarioGetTitolariUtilizzabili", e);
                return titolari;
            }
        }
        #endregion Titolario

        #region Trasmissione
        [WebMethod]
        public virtual DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione TrasmissioneGetModelloByID(string tokenDiAutenticazione, string idModello)
        {
            ServiceLoadController.LoadControllerManager.CheckInterval("SmartServices", "TrasmissioneGetModelloByID");

            try
            {
                DocsPaVO.utente.InfoUtente infoUtente = Utils.getInfoUtenteFromToken(tokenDiAutenticazione);
                return BusinessLogic.Trasmissioni.ModelliTrasmissioni.getModelloByID(infoUtente.idAmministrazione, idModello);

            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaServices.asmx  - metodo: TrasmissioneGetModelloByID", e);
                return null;
            }
        }


        [WebMethod]
        [return: XmlArray()]
        [return: XmlArrayItem(typeof(DocsPaVO.utente.Utente))]
        public virtual ArrayList GetListaUtentiByIdRuolo(string idRuolo)
        {
            ServiceLoadController.LoadControllerManager.CheckInterval("SmartServices", "GetListaUtentiByIdRuolo");
            ArrayList retValue = new ArrayList();

            try
            {
                retValue = BusinessLogic.Utenti.UserManager.getListaUtentiByIdRuolo(idRuolo);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: GetListaUtentiByIdRuolo - ", e);
            }

            return retValue;
        }

        /// <summary>
        /// trasmissione dei documenti con eliminazione da todoList
        /// </summary>
        /// <param name="tokenDiAutenticazione"></param>
        /// <param name="serverPath"></param>
        /// <param name="scheda"></param>
        /// <param name="modello"></param>
        /// <returns></returns>
        [WebMethod(Description = "trasmissione dei documenti con eliminazione da todoList")]
        public virtual bool TrasmissioneExecuteTrasmDocDaModello(string tokenDiAutenticazione, string serverPath, DocsPaVO.documento.SchedaDocumento scheda, DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione modello)
        {
            ServiceLoadController.LoadControllerManager.CheckInterval("SmartServices", "TrasmissioneExecuteTrasmDocDaModello");
            string desc = string.Empty;
            bool retval = false;
            DocsPaVO.trasmissione.Trasmissione trasmissione = new DocsPaVO.trasmissione.Trasmissione();
            try
            {
                DocsPaVO.utente.InfoUtente infoUtente = Utils.getInfoUtenteFromToken(tokenDiAutenticazione);



                //Parametri della trasmissione
                trasmissione.noteGenerali = modello.VAR_NOTE_GENERALI;

                trasmissione.tipoOggetto = DocsPaVO.trasmissione.TipoOggetto.DOCUMENTO;
                //trasmissione.infoDocumento = DocumentManager.getInfoDocumento(scheda);
                trasmissione.infoDocumento = BusinessLogic.Documenti.DocManager.getInfoDocumento(scheda);


                trasmissione.utente = BusinessLogic.Utenti.UserManager.getUtente(infoUtente.idPeople);
                trasmissione.ruolo = BusinessLogic.Utenti.UserManager.getRuolo(infoUtente.idCorrGlobali);


                //Parametri delle trasmissioni singole
                for (int i = 0; i < modello.RAGIONI_DESTINATARI.Count; i++)
                {
                    DocsPaVO.Modelli_Trasmissioni.RagioneDest ragDest = (DocsPaVO.Modelli_Trasmissioni.RagioneDest)modello.RAGIONI_DESTINATARI[i];
                    ArrayList destinatari = new ArrayList(ragDest.DESTINATARI);
                    for (int j = 0; j < destinatari.Count; j++)
                    {
                        DocsPaVO.Modelli_Trasmissioni.MittDest mittDest = (DocsPaVO.Modelli_Trasmissioni.MittDest)destinatari[j];

                        DocsPaVO.utente.Corrispondente corr = BusinessLogic.Utenti.UserManager.getCorrispondenteByCodRubrica(mittDest.VAR_COD_RUBRICA, infoUtente);

                        DocsPaVO.trasmissione.RagioneTrasmissione ragione = BusinessLogic.Trasmissioni.QueryTrasmManager.getRagioneById(mittDest.ID_RAGIONE.ToString());
                        trasmissione = BusinessLogic.Trasmissioni.TrasmManager.addTrasmissioneSingola(trasmissione, corr, ragione, mittDest.VAR_NOTE_SING, mittDest.CHA_TIPO_TRASM);
                    }
                }
                if (infoUtente.delegato != null)
                    trasmissione.delegato = ((DocsPaVO.utente.InfoUtente)(infoUtente.delegato)).idPeople;


                //se è posto ad 1 elimina da todoList se è a 0 no
                trasmissione.NO_NOTIFY = modello.NO_NOTIFY;

                trasmissione = BusinessLogic.Trasmissioni.ExecTrasmManager.saveExecuteTrasmMethod(serverPath, trasmissione);
                string notify = "1";
                if (trasmissione.NO_NOTIFY != null && trasmissione.NO_NOTIFY.Equals("1"))
                {
                    notify = "0";
                }
                else
                {
                    notify = "1";
                }
                if (trasmissione != null)
                {
                    // LOG per documento
                    if (trasmissione.infoDocumento != null && !string.IsNullOrEmpty(trasmissione.infoDocumento.idProfile))
                    {
                        foreach (DocsPaVO.trasmissione.TrasmissioneSingola single in trasmissione.trasmissioniSingole)
                        {
                            string method = "TRASM_DOC_" + single.ragione.descrizione.ToUpper().Replace(" ", "_");
                            if (trasmissione.infoDocumento.segnatura == null)
                                desc = "Trasmesso Documento : " + trasmissione.infoDocumento.docNumber.ToString();
                            else
                                desc = "Trasmesso Documento : " + trasmissione.infoDocumento.segnatura.ToString();

                            BusinessLogic.UserLog.UserLog.WriteLog(trasmissione.utente.userId, trasmissione.utente.idPeople, trasmissione.ruolo.idGruppo, trasmissione.utente.idAmministrazione, method, trasmissione.infoDocumento.docNumber, desc, DocsPaVO.Logger.CodAzione.Esito.OK, infoUtente.delegato, notify, single.systemId);
                        }
                    }
                    // LOG per fascicolo
                    if (trasmissione.infoFascicolo != null && !string.IsNullOrEmpty(trasmissione.infoFascicolo.idFascicolo))
                    {
                        foreach (DocsPaVO.trasmissione.TrasmissioneSingola single in trasmissione.trasmissioniSingole)
                        {
                            string method = "TRASM_FOLDER_" + single.ragione.descrizione.ToUpper().Replace(" ", "_");
                            desc = "Trasmesso Fascicolo ID: " + trasmissione.infoFascicolo.idFascicolo.ToString();
                            BusinessLogic.UserLog.UserLog.WriteLog(trasmissione.utente.userId, trasmissione.utente.idPeople, trasmissione.ruolo.idGruppo, trasmissione.utente.idAmministrazione, method, trasmissione.infoFascicolo.idFascicolo, desc, DocsPaVO.Logger.CodAzione.Esito.OK, infoUtente.delegato, notify, single.systemId);
                        }
                    }

                }

                if (trasmissione != null)
                    retval = true;
            }
            catch (Exception e)
            {
                // LOG per documento
                if (trasmissione.infoDocumento != null && !string.IsNullOrEmpty(trasmissione.infoDocumento.idProfile))
                {
                    if (trasmissione.infoDocumento.segnatura == null)
                        desc = "Trasmesso Documento : " + trasmissione.infoDocumento.docNumber.ToString();
                    else
                        desc = "Trasmesso Documento : " + trasmissione.infoDocumento.segnatura.ToString();
                    BusinessLogic.UserLog.UserLog.WriteLog(trasmissione.utente.userId, trasmissione.utente.idPeople, trasmissione.ruolo.idGruppo, trasmissione.utente.idAmministrazione, "DOCUMENTOTRASMESSO", trasmissione.infoDocumento.docNumber, desc, DocsPaVO.Logger.CodAzione.Esito.KO, null);
                }
                logger.Debug("errore nel web method TrasmissioneExecuteTrasmDocDaModello - errore: " + e.Message);
                retval = false;
            }

            return retval;
        }


        [WebMethod]
        [return: XmlArray()]
        [return: XmlArrayItem(typeof(DocsPaVO.trasmissione.Trasmissione))]
        public virtual ArrayList trasmGetDettaglioTrasmissione(string tokenDiAutenticazione, DocsPaVO.trasmissione.infoToDoList todo)
        {
            ServiceLoadController.LoadControllerManager.CheckInterval("SmartServices", "trasmGetDettaglioTrasmissione");

            try
            {
                //DocsPaVO.utente.Utente ut, DocsPaVO.utente.Ruolo role,
                DocsPaVO.utente.InfoUtente infoUtente = Utils.getInfoUtenteFromToken(tokenDiAutenticazione);

                DocsPaVO.utente.Ruolo role = Utils.getRuoloFromInfoUtente(infoUtente);
                DocsPaVO.utente.Utente ut = new DocsPaVO.utente.Utente();
                ut.idPeople = infoUtente.idPeople;
                DocsPaVO.trasmissione.OggettoTrasm oggettoTrasm = new DocsPaVO.trasmissione.OggettoTrasm();

                if (!string.IsNullOrEmpty(todo.sysIdDoc))
                {
                    using (DocsPaDB.Query_DocsPAWS.Documenti dbDoc = new DocsPaDB.Query_DocsPAWS.Documenti())
                        oggettoTrasm.infoDocumento = dbDoc.GetInfoDocumento(role.idGruppo, infoUtente.idPeople, todo.sysIdDoc, false);

                    oggettoTrasm.infoFascicolo = null;
                }
                else if (!string.IsNullOrEmpty(todo.sysIdFasc))
                {
                    oggettoTrasm.infoDocumento = null;
                    oggettoTrasm.infoFascicolo = new DocsPaVO.fascicolazione.InfoFascicolo { idFascicolo = todo.sysIdFasc };
                }

                return BusinessLogic.Trasmissioni.QueryTrasmManager.getQueryDettaglioTrasmMethod(oggettoTrasm, ut, role, null, todo.sysIdTrasm);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: trasmGetDettaglioTrasmissione", e);
                throw e;
            }
        }
        /// <summary>
        /// trasmissione di fascicoli con eliminazione da todoList
        /// </summary>
        /// <param name="tokenDiAutenticazione"></param>
        /// <param name="serverPath"></param>
        /// <param name="fascicolo"></param>
        /// <param name="modello"></param>
        /// <returns></returns>
        [WebMethod(Description = "trasmissione di fascicoli con eliminazione da todoList")]
        public virtual bool TrasmissioneExecuteTrasmFascDaModello(string tokenDiAutenticazione, string serverPath, DocsPaVO.fascicolazione.Fascicolo fascicolo, DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione modello)
        {
            ServiceLoadController.LoadControllerManager.CheckInterval("SmartServices", "TrasmissioneExecuteTrasmFascDaModello");

            bool retval = false;
            string desc = string.Empty;
            DocsPaVO.trasmissione.Trasmissione trasmissione = new DocsPaVO.trasmissione.Trasmissione();
            try
            {
                DocsPaVO.utente.InfoUtente infoUtente = Utils.getInfoUtenteFromToken(tokenDiAutenticazione);



                //Parametri della trasmissione
                trasmissione.noteGenerali = modello.VAR_NOTE_GENERALI;

                trasmissione.tipoOggetto = DocsPaVO.trasmissione.TipoOggetto.FASCICOLO;

                DocsPaVO.fascicolazione.Fascicolo fasc = BusinessLogic.Fascicoli.FascicoloManager.getFascicoloById(fascicolo.systemID, infoUtente);
                trasmissione.infoFascicolo = new DocsPaVO.fascicolazione.InfoFascicolo(fasc);

                trasmissione.utente = BusinessLogic.Utenti.UserManager.getUtente(infoUtente.idPeople);
                trasmissione.ruolo = BusinessLogic.Utenti.UserManager.getRuolo(infoUtente.idCorrGlobali);


                //Parametri delle trasmissioni singole
                for (int i = 0; i < modello.RAGIONI_DESTINATARI.Count; i++)
                {
                    DocsPaVO.Modelli_Trasmissioni.RagioneDest ragDest = (DocsPaVO.Modelli_Trasmissioni.RagioneDest)modello.RAGIONI_DESTINATARI[i];
                    ArrayList destinatari = new ArrayList(ragDest.DESTINATARI);
                    for (int j = 0; j < destinatari.Count; j++)
                    {
                        DocsPaVO.Modelli_Trasmissioni.MittDest mittDest = (DocsPaVO.Modelli_Trasmissioni.MittDest)destinatari[j];

                        DocsPaVO.utente.Corrispondente corr = BusinessLogic.Utenti.UserManager.getCorrispondenteByCodRubrica(mittDest.VAR_COD_RUBRICA, infoUtente);

                        DocsPaVO.trasmissione.RagioneTrasmissione ragione = BusinessLogic.Trasmissioni.QueryTrasmManager.getRagioneById(mittDest.ID_RAGIONE.ToString());
                        trasmissione = BusinessLogic.Trasmissioni.TrasmManager.addTrasmissioneSingola(trasmissione, corr, ragione, mittDest.VAR_NOTE_SING, mittDest.CHA_TIPO_TRASM);
                    }
                }
                if (infoUtente.delegato != null)
                    trasmissione.delegato = ((DocsPaVO.utente.InfoUtente)(infoUtente.delegato)).idPeople;

                //se è posto ad 1 elimina da todoList se è a 0 no
                trasmissione.NO_NOTIFY = modello.NO_NOTIFY;

                trasmissione = BusinessLogic.Trasmissioni.ExecTrasmManager.saveExecuteTrasmMethod(serverPath, trasmissione);
                if (trasmissione != null)
                    retval = true;

                string notify = "1";
                if (trasmissione.NO_NOTIFY != null && trasmissione.NO_NOTIFY.Equals("1"))
                {
                    notify = "0";
                }
                else
                {
                    notify = "1";
                }
                if (trasmissione != null)
                {
                    // LOG per documento
                    if (trasmissione.infoDocumento != null && !string.IsNullOrEmpty(trasmissione.infoDocumento.idProfile))
                    {
                        foreach (DocsPaVO.trasmissione.TrasmissioneSingola single in trasmissione.trasmissioniSingole)
                        {
                            string method = "TRASM_DOC_" + single.ragione.descrizione.ToUpper().Replace(" ", "_");
                            if (trasmissione.infoDocumento.segnatura == null)
                                desc = "Trasmesso Documento : " + trasmissione.infoDocumento.docNumber.ToString();
                            else
                                desc = "Trasmesso Documento : " + trasmissione.infoDocumento.segnatura.ToString();

                            BusinessLogic.UserLog.UserLog.WriteLog(trasmissione.utente.userId, trasmissione.utente.idPeople, trasmissione.ruolo.idGruppo, trasmissione.utente.idAmministrazione, method, trasmissione.infoDocumento.docNumber, desc, DocsPaVO.Logger.CodAzione.Esito.OK, infoUtente.delegato, notify, single.systemId);
                        }
                    }
                    // LOG per fascicolo
                    if (trasmissione.infoFascicolo != null && !string.IsNullOrEmpty(trasmissione.infoFascicolo.idFascicolo))
                    {
                        foreach (DocsPaVO.trasmissione.TrasmissioneSingola single in trasmissione.trasmissioniSingole)
                        {
                            string method = "TRASM_FOLDER_" + single.ragione.descrizione.ToUpper().Replace(" ", "_");
                            desc = "Trasmesso Fascicolo ID: " + trasmissione.infoFascicolo.idFascicolo.ToString();
                            BusinessLogic.UserLog.UserLog.WriteLog(trasmissione.utente.userId, trasmissione.utente.idPeople, trasmissione.ruolo.idGruppo, trasmissione.utente.idAmministrazione, method, trasmissione.infoFascicolo.idFascicolo, desc, DocsPaVO.Logger.CodAzione.Esito.OK, infoUtente.delegato, notify, single.systemId);
                        }
                    }

                }
            }
            catch (Exception e)
            {

                // LOG per documento
                if (trasmissione.infoDocumento != null && !string.IsNullOrEmpty(trasmissione.infoDocumento.idProfile))
                {
                    if (trasmissione.infoDocumento.segnatura == null)
                        desc = "Trasmesso Documento : " + trasmissione.infoDocumento.docNumber.ToString();
                    else
                        desc = "Trasmesso Documento : " + trasmissione.infoDocumento.segnatura.ToString();
                    BusinessLogic.UserLog.UserLog.WriteLog(trasmissione.utente.userId, trasmissione.utente.idPeople, trasmissione.ruolo.idGruppo, trasmissione.utente.idAmministrazione, "DOCUMENTOTRASMESSO", trasmissione.infoDocumento.docNumber, desc, DocsPaVO.Logger.CodAzione.Esito.KO, null);
                }
                logger.Debug("errore nel web method TrasmissioneExecuteTrasmDocDaModello - errore: " + e.Message);
                retval = false;
            }

            return retval;
        }





        [WebMethod]
        public virtual ArrayList TrasmissioneGetModelliPerTrasm(string tokenDiAutenticazione, DocsPaVO.utente.Registro[] registri, string idTipoDoc, string idDiagramma, string idStato, string cha_tipo_oggetto)
        {
            ServiceLoadController.LoadControllerManager.CheckInterval("SmartServices", "TrasmissioneGetModelliPerTrasm");

            try
            {
                DocsPaVO.utente.InfoUtente infoUtente = Utils.getInfoUtenteFromToken(tokenDiAutenticazione);
                return BusinessLogic.Trasmissioni.ModelliTrasmissioni.getModelliPerTrasm(infoUtente.idAmministrazione, registri, infoUtente.idPeople, infoUtente.idCorrGlobali, idTipoDoc, idDiagramma, idStato, cha_tipo_oggetto, false);

            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaServices.asmx  - metodo: TrasmissioneGetModelliPerTrasm", e);
                return null;
            }
        }



        [WebMethod]
        [return: XmlArray()]
        [return: XmlArrayItem(typeof(DocsPaVO.trasmissione.infoToDoList))]
        public virtual ArrayList getMyTodoList(string tokenDiAutenticazione)
        {
            ServiceLoadController.LoadControllerManager.CheckInterval("SmartServices", "getMyTodoList");
            ArrayList array = new ArrayList();
            try
            {
                DocsPaVO.utente.InfoUtente infoUtente = Utils.getInfoUtenteFromToken(tokenDiAutenticazione);
                BusinessLogic.Trasmissioni.TrasmManager trasmManager = new BusinessLogic.Trasmissioni.TrasmManager();
                array = trasmManager.getMyNewTodoListMigrazione(infoUtente);
            }
            catch (Exception e)
            {
                logger.Debug("errore durante il reperimento della todo list durante la migrazione - " + e.Message);
            }
            return array;
        }


        /// <summary>
        /// </summary>
        [WebMethod]
        public virtual DocsPaVO.trasmissione.Trasmissione TrasmissioneExecuteTrasm(string path, DocsPaVO.trasmissione.Trasmissione trasmissione)
        {
            ServiceLoadController.LoadControllerManager.CheckInterval("SmartServices", "TrasmissioneExecuteTrasm");

            DocsPaVO.trasmissione.Trasmissione result = null;
            string desc = string.Empty;

            try
            {
                //gestione tracer
#if TRACE_WS
				DocsPaUtils.LogsManagement.PerformaceTracer pt = new DocsPaUtils.LogsManagement.PerformaceTracer("TRACE_WS");
#endif
                result = BusinessLogic.Trasmissioni.ExecTrasmManager.executeTrasmMethod(path, trasmissione);

                if (result != null)
                {
                    // LOG per documento
                    if (result.infoDocumento != null)
                    {
                        if (result.infoDocumento.segnatura == null)
                            desc = "Trasmesso Documento ID: " + result.infoDocumento.docNumber.ToString();
                        else
                            desc = "Trasmesso Documento ID: " + result.infoDocumento.segnatura.ToString();

                        //BusinessLogic.UserLog.UserLog.getInstance().WriteLog(result.utente.userId, result.utente.idPeople, result.ruolo.idGruppo, result.utente.idAmministrazione, "DOCUMENTOTRASMESSO", result.infoDocumento.docNumber, desc, DocsPaVO.Logger.CodAzione.Esito.OK);
                    }

                    // LOG per fascicolo
                    if (result.infoFascicolo != null)
                    {
                        desc = "Trasmesso Fascicolo: " + result.infoFascicolo.codice;
                        //BusinessLogic.UserLog.UserLog.getInstance().WriteLog(result.utente.userId, result.utente.idPeople, result.ruolo.idGruppo, result.utente.idAmministrazione, "FASCICOLOTRASMESSO", result.infoFascicolo.idFascicolo, desc, DocsPaVO.Logger.CodAzione.Esito.OK);
                    }
                }

#if TRACE_WS
				pt.WriteLogTracer("TrasmissioneExecuteTrasm");
#endif
            }
            catch (Exception e)
            {
                // LOG per documento
                if (trasmissione.infoDocumento != null)
                {
                    if (trasmissione.infoDocumento.segnatura == null)
                        desc = "Trasmesso Documento ID: " + trasmissione.infoDocumento.docNumber.ToString();
                    else
                        desc = "Trasmesso Documento ID: " + trasmissione.infoDocumento.segnatura.ToString();

                    //BusinessLogic.UserLog.UserLog.getInstance().WriteLog(trasmissione.utente.userId, trasmissione.utente.idPeople, trasmissione.ruolo.idGruppo, trasmissione.utente.idAmministrazione, "DOCUMENTOTRASMESSO", trasmissione.infoDocumento.docNumber, desc, DocsPaVO.Logger.CodAzione.Esito.KO);
                }

                // LOG per fascicolo
                if (trasmissione.infoFascicolo != null)
                {
                    desc = "Trasmesso Fascicolo: " + trasmissione.infoFascicolo.codice;
                    //BusinessLogic.UserLog.UserLog.getInstance().WriteLog(trasmissione.utente.userId, trasmissione.utente.idPeople, trasmissione.ruolo.idGruppo, trasmissione.utente.idAmministrazione, "FASCICOLOTRASMESSO", trasmissione.infoFascicolo.idFascicolo, desc, DocsPaVO.Logger.CodAzione.Esito.KO);
                }

                logger.Debug("Errore in DocsPaWS.asmx  - metodo: TrasmissioneExecuteTrasm - ", e);
                result = null;
            }
            return result;
        }

        [WebMethod]
        public virtual DocsPaVO.trasmissione.Trasmissione TrasmissioneSaveExecuteTrasm(string path, DocsPaVO.trasmissione.Trasmissione trasmissione)
        {
            ServiceLoadController.LoadControllerManager.CheckInterval("SmartServices", "TrasmissioneSaveExecuteTrasm");


            string desc = string.Empty;

            try
            {
                trasmissione = BusinessLogic.Trasmissioni.ExecTrasmManager.saveExecuteTrasmMethod(path, trasmissione);

                string notify = "1";
                if (trasmissione.NO_NOTIFY != null && trasmissione.NO_NOTIFY.Equals("1"))
                {
                    notify = "0";
                }
                else
                {
                    notify = "1";
                }
                if (trasmissione != null)
                {
                    // LOG per documento
                    if (trasmissione.infoDocumento != null && !string.IsNullOrEmpty(trasmissione.infoDocumento.idProfile))
                    {
                        foreach (DocsPaVO.trasmissione.TrasmissioneSingola single in trasmissione.trasmissioniSingole)
                        {
                            string method = "TRASM_DOC_" + single.ragione.descrizione.ToUpper().Replace(" ", "_");
                            if (trasmissione.infoDocumento.segnatura == null)
                                desc = "Trasmesso Documento : " + trasmissione.infoDocumento.docNumber.ToString();
                            else
                                desc = "Trasmesso Documento : " + trasmissione.infoDocumento.segnatura.ToString();

                            BusinessLogic.UserLog.UserLog.WriteLog(trasmissione.utente.userId, trasmissione.utente.idPeople, trasmissione.ruolo.idGruppo, trasmissione.utente.idAmministrazione, method, trasmissione.infoDocumento.docNumber, desc, DocsPaVO.Logger.CodAzione.Esito.OK, null, notify, single.systemId);
                        }
                    }
                    // LOG per fascicolo
                    if (trasmissione.infoFascicolo != null && !string.IsNullOrEmpty(trasmissione.infoFascicolo.idFascicolo))
                    {
                        foreach (DocsPaVO.trasmissione.TrasmissioneSingola single in trasmissione.trasmissioniSingole)
                        {
                            string method = "TRASM_FOLDER_" + single.ragione.descrizione.ToUpper().Replace(" ", "_");
                            desc = "Trasmesso Fascicolo ID: " + trasmissione.infoFascicolo.idFascicolo.ToString();
                            BusinessLogic.UserLog.UserLog.WriteLog(trasmissione.utente.userId, trasmissione.utente.idPeople, trasmissione.ruolo.idGruppo, trasmissione.utente.idAmministrazione, method, trasmissione.infoFascicolo.idFascicolo, desc, DocsPaVO.Logger.CodAzione.Esito.OK, null, notify, single.systemId);
                        }
                    }

                }
            }
            catch (Exception e)
            {
                // LOG per documento
                if (trasmissione.infoDocumento != null && !string.IsNullOrEmpty(trasmissione.infoDocumento.idProfile))
                {
                    if (trasmissione.infoDocumento.segnatura == null)
                        desc = "Trasmesso Documento : " + trasmissione.infoDocumento.docNumber.ToString();
                    else
                        desc = "Trasmesso Documento : " + trasmissione.infoDocumento.segnatura.ToString();
                    BusinessLogic.UserLog.UserLog.WriteLog(trasmissione.utente.userId, trasmissione.utente.idPeople, trasmissione.ruolo.idGruppo, trasmissione.utente.idAmministrazione, "DOCUMENTOTRASMESSO", trasmissione.infoDocumento.docNumber, desc, DocsPaVO.Logger.CodAzione.Esito.KO, null);

                }
                logger.Debug("Errore in SmartServices  - metodo: TrasmissioneSaveExecuteTrasm - ", e);
                trasmissione = null;
            }
            return trasmissione;
        }


        [WebMethod]
        public virtual bool executeAccRifMethod(DocsPaVO.trasmissione.TrasmissioneUtente trasmissioneUtente, string idTrasmissione, DocsPaVO.utente.Ruolo ruolo, DocsPaVO.utente.InfoUtente infoUtente, out string err)
        {
            ServiceLoadController.LoadControllerManager.CheckInterval("SmartServices", "executeAccRifMethod");
            bool retval = false;
            err = string.Empty;
            string mode;
            string idObj;
            try
            {
                retval = BusinessLogic.Trasmissioni.ExecTrasmManager.executeAccRifMethod(trasmissioneUtente, idTrasmissione, ruolo, infoUtente, out err, out mode, out idObj);
            }
            catch (Exception e)
            {
                logger.Debug("Errore nel webmethod executeAccRifMethod - errore: " + e.Message);
            }

            return retval;
        }


        #endregion Trasmissione


        #region Tipologia Documenti
        [WebMethod]
        public virtual DocsPaVO.ProfilazioneDinamica.Templates TipologiaGetTipologia(string tokenDiAutenticazione, string tipoAtto)
        {
            ServiceLoadController.LoadControllerManager.CheckInterval("SmartServices", "TipologiaGetTipologia");

            try
            {
              
                DocsPaVO.utente.InfoUtente infoUtente = Utils.getInfoUtenteFromToken(tokenDiAutenticazione);
                return BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.getTemplatePerRicerca(infoUtente.idAmministrazione, tipoAtto);

            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaServices.asmx  - metodo: TipologiaGetTipologia", e);
                return null;
            }
        }
        #endregion Tipologia Documenti

        #region Tipologia Fascicoli
        [WebMethod]
        public virtual DocsPaVO.ProfilazioneDinamica.Templates TipologiaGetTipologiaFasc(string tokenDiAutenticazione, string tipoFasc)
        {
            ServiceLoadController.LoadControllerManager.CheckInterval("SmartServices", "TipologiaGetTipologiaFasc");

            try
            {

                DocsPaVO.utente.InfoUtente infoUtente = Utils.getInfoUtenteFromToken(tokenDiAutenticazione);
                return BusinessLogic.ProfilazioneDinamica.ProfilazioneFascicoli.getTemplatePerRicerca(infoUtente.idAmministrazione, tipoFasc);

            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaServices.asmx  - metodo: TipologiaGetTipologia", e);
                return null;
            }
        }
        #endregion Tipologia Fascicoli

        #region Diagrammi Di Stato
        [WebMethod]
        public virtual DocsPaVO.DiagrammaStato.DiagrammaStato DiagrammaGetDiagrammaByIdTipoDoc(string tokenDiAutenticazione, string idTipoDoc)
        {
            ServiceLoadController.LoadControllerManager.CheckInterval("SmartServices", "DiagrammaGetDiagrammaByIdTipoDoc");
            try
            {
                DocsPaVO.utente.InfoUtente infoUtente = Utils.getInfoUtenteFromToken(tokenDiAutenticazione);
                return BusinessLogic.DiagrammiStato.DiagrammiStato.getDgByIdTipoDoc(idTipoDoc, infoUtente.idAmministrazione);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaServices.asmx  - metodo: DiagrammaGetDiagrammaByIdTipoDoc", e);
                return null;
            }
        }

        [WebMethod]
        public virtual string DiagrammaGetValoreFiltroStato(string tokenDiAutenticazione, string idStato)
        {
            ServiceLoadController.LoadControllerManager.CheckInterval("SmartServices", "DiagrammaGetValoreFiltroStato");
            try
            {
                DocsPaVO.utente.InfoUtente infoUtente = Utils.getInfoUtenteFromToken(tokenDiAutenticazione);
                return " AND (DPA_DIAGRAMMI.DOC_NUMBER = A.DOCNUMBER AND DPA_DIAGRAMMI.ID_STATO = " + idStato + ") ";
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaServices.asmx  - metodo: DiagrammaGetValoreFiltroStato", e);
                return null;
            }
        }
        #endregion Diagrammi Di Stato

        #region Utils
        //Metodo che imposta i filtri ricerca Documenti
        //A partire dal TIPO imposta correttamente le nuove proprietà dei filtri per le versioni dalla 3.10 in poi
        private DocsPaVO.filtri.FiltroRicerca[][] setFiltriRicercaDoc(DocsPaVO.filtri.FiltroRicerca[][] queryList)
        {
            DocsPaVO.filtri.FiltroRicerca[][] listaFiltri = queryList;
            DocsPaVO.filtri.FiltroRicerca fV;

            if (queryList.Length > 0)
            {
                for (int i = 0; i < queryList[0].Length; i++)
                {
                    DocsPaVO.filtri.FiltroRicerca f = queryList[0][i];
                    if (f.argomento.ToUpper().Equals("TIPO"))
                    {
                        switch (f.valore)
                        {
                            case "A":
                                for (int j = 0; j < queryList[0].Length; j++)
                                {
                                    if (queryList[0][j].argomento.ToUpper().Equals("PROT_ARRIVO"))
                                    {
                                        queryList[0][j].valore = "true";
                                        listaFiltri = queryList;
                                        return listaFiltri;
                                    }
                                }

                                fV = new DocsPaVO.filtri.FiltroRicerca();
                                fV.argomento = DocsPaVO.filtri.ricerca.listaArgomenti.PROT_ARRIVO.ToString();
                                fV.valore = "true";

                                listaFiltri[0] = addToArrayFiltroRicerca(listaFiltri[0], fV);
                                return listaFiltri;
                                break;

                            case "P":
                                for (int j = 0; j < queryList[0].Length; j++)
                                {
                                    if (queryList[0][j].argomento.ToUpper().Equals("PROT_PARTENZA"))
                                    {
                                        queryList[0][j].valore = "true";
                                        listaFiltri = queryList;
                                        return listaFiltri;
                                    }
                                }

                                fV = new DocsPaVO.filtri.FiltroRicerca();
                                fV.argomento = DocsPaVO.filtri.ricerca.listaArgomenti.PROT_PARTENZA.ToString();
                                fV.valore = "true";

                                listaFiltri[0] = addToArrayFiltroRicerca(listaFiltri[0], fV);
                                return listaFiltri;
                                break;

                            case "I":
                                for (int j = 0; j < queryList[0].Length; j++)
                                {
                                    if (queryList[0][j].argomento.ToUpper().Equals("PROT_INTERNO"))
                                    {
                                        queryList[0][j].valore = "true";
                                        listaFiltri = queryList;
                                        return listaFiltri;
                                    }
                                }

                                fV = new DocsPaVO.filtri.FiltroRicerca();
                                fV.argomento = DocsPaVO.filtri.ricerca.listaArgomenti.PROT_INTERNO.ToString();
                                fV.valore = "true";

                                listaFiltri[0] = addToArrayFiltroRicerca(listaFiltri[0], fV);
                                return listaFiltri;
                                break;

                            case "G":
                                for (int j = 0; j < queryList[0].Length; j++)
                                {
                                    if (queryList[0][j].argomento.ToUpper().Equals("GRIGIO"))
                                    {
                                        queryList[0][j].valore = "true";
                                        listaFiltri = queryList;
                                        return listaFiltri;
                                    }
                                }

                                fV = new DocsPaVO.filtri.FiltroRicerca();
                                fV.argomento = DocsPaVO.filtri.ricerca.listaArgomenti.GRIGIO.ToString();
                                fV.valore = "true";

                                listaFiltri[0] = addToArrayFiltroRicerca(listaFiltri[0], fV);
                                return listaFiltri;
                                break;

                            case "T":
                                //Protocollo Arrivo
                                bool existFilter = false;
                                for (int j = 0; j < queryList[0].Length; j++)
                                {
                                    if (queryList[0][j].argomento.ToUpper().Equals("PROT_ARRIVO"))
                                    {
                                        queryList[0][j].valore = "true";
                                        listaFiltri = queryList;
                                        existFilter = true;
                                    }
                                }
                                if (!existFilter)
                                {
                                    fV = new DocsPaVO.filtri.FiltroRicerca();
                                    fV.argomento = DocsPaVO.filtri.ricerca.listaArgomenti.PROT_ARRIVO.ToString();
                                    fV.valore = "true";

                                    listaFiltri[0] = addToArrayFiltroRicerca(listaFiltri[0], fV);
                                }

                                //Protocollo Partenza
                                existFilter = false;
                                for (int j = 0; j < queryList[0].Length; j++)
                                {
                                    if (queryList[0][j].argomento.ToUpper().Equals("PROT_PARTENZA"))
                                    {
                                        queryList[0][j].valore = "true";
                                        listaFiltri = queryList;
                                        existFilter = true;
                                    }
                                }
                                if (!existFilter)
                                {
                                    fV = new DocsPaVO.filtri.FiltroRicerca();
                                    fV.argomento = DocsPaVO.filtri.ricerca.listaArgomenti.PROT_PARTENZA.ToString();
                                    fV.valore = "true";

                                    listaFiltri[0] = addToArrayFiltroRicerca(listaFiltri[0], fV);
                                    existFilter = false;
                                }

                                //Protocollo Interno
                                existFilter = false;
                                for (int j = 0; j < queryList[0].Length; j++)
                                {
                                    if (queryList[0][j].argomento.ToUpper().Equals("PROT_INTERNO"))
                                    {
                                        queryList[0][j].valore = "true";
                                        listaFiltri = queryList;
                                        existFilter = true;
                                    }
                                }
                                if (!existFilter)
                                {
                                    fV = new DocsPaVO.filtri.FiltroRicerca();
                                    fV.argomento = DocsPaVO.filtri.ricerca.listaArgomenti.PROT_INTERNO.ToString();
                                    fV.valore = "true";

                                    listaFiltri[0] = addToArrayFiltroRicerca(listaFiltri[0], fV);
                                    existFilter = false;
                                }

                                break;
                        }
                    }
                }
            }

            return listaFiltri;

        }

        private DocsPaVO.filtri.FiltroRicerca[] addToArrayFiltroRicerca(DocsPaVO.filtri.FiltroRicerca[] array, DocsPaVO.filtri.FiltroRicerca nuovoElemento)
        {
            DocsPaVO.filtri.FiltroRicerca[] nuovaLista;
            if (array != null)
            {
                int len = array.Length;
                nuovaLista = new DocsPaVO.filtri.FiltroRicerca[len + 1];
                array.CopyTo(nuovaLista, 0);
                nuovaLista[len] = nuovoElemento;
                return nuovaLista;
            }
            else
            {
                nuovaLista = new DocsPaVO.filtri.FiltroRicerca[1];
                nuovaLista[0] = nuovoElemento;
                return nuovaLista;
            }
        }

        #endregion Utils

        #region ricerca
        /// <summary>
        /// RICERCA DEI DOCUMENTI PER SEGNATURA O PER IDDOCUMENTO
        /// </summary>
        /// <param name="user"> USER UTENTE</param>
        /// <param name="password">PASSWORD UTENTE</param>
        /// <param name="Ruolo">DESCRIZIONE DEL RUOLO DELL'UTENTE</param>
        /// <param name="segnatura">SEGNATURA DEL DOCUMENTO NB. E' UNA STRINGA VUOTA SE E' VALORIZZATO ID DOCUMENTO</param>
        /// <param name="idDocumento">ID DEL DOCUEMNTO NB. E' UNA STRINGA VUOTA SE E' VALORIZZATO LA SEGNATURA </param>
        /// <param name="documentoProtocollato">TRUE SE è UN PROTOCOLLO; FALSE SE è UN DOCUMENTO NON PROTOCOLLATO</param>
        /// <param name="dettagliCorrispondente">VALE "M" PER I DETTAGLI DEL MITTENTE, "D" PER I DETTAGLI DEL/I DESTINATARIO, "T" PER I DETTAGLI DEL MITTENTE E DEL/I DESTINATARIO, "N" SE NON SI VUOLE NESSUN DETTAGLIO </param>
        /// <param name="messaggioErrore">MESSAGGIO DI ERRORE</param>
        /// <returns>RITORNA UNA SCHEDA DOCUMENTO DIVERSA DA NULL SE IL DOCUEMNTO è STATO TROVATO ALTRIMENTI NULL</returns>
        [WebMethod]
        public DocsPaVO.documento.SchedaDocumento ricercaDocumento(string user, string password, string Ruolo, string segnatura, string idDocumento, bool documentoProtocollato, string dettagliCorrispondente, out string messaggioErrore)
        {
            ServiceLoadController.LoadControllerManager.CheckInterval("SmartServices", "ricercaDocumento");

            messaggioErrore = string.Empty;

            DocsPaVO.utente.UserLogin userLogin = new DocsPaVO.utente.UserLogin();
            userLogin.Password = password;
            userLogin.UserName = user;

            string idWebSession = string.Empty;
            string ipAddress = string.Empty;
            string token = string.Empty;

            DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();

            //login
            DocsPaVO.utente.UserLogin.LoginResult loginResult = new DocsPaVO.utente.UserLogin.LoginResult();
            DocsPaVO.utente.Utente utente = BusinessLogic.Utenti.Login.loginMethod(userLogin, out loginResult, true, idWebSession, out ipAddress);

            if (utente == null)
            {
                logger.Debug("l'utente indicato non esiste");
                messaggioErrore = "l'utente indicato non esiste";
                throw new Exception("l'utente indicato non esiste");
            }
            //controllo dell'esistenza del ruolo
            for (int i = 0; i < utente.ruoli.Count; i++)
                if (((DocsPaVO.utente.Ruolo)utente.ruoli[i]).descrizione.ToUpper().Equals(Ruolo.ToUpper()))
                    ruolo = ((DocsPaVO.utente.Ruolo)utente.ruoli[i]);

            bool okRuolo = false;

            if (ruolo != null)
            {
                foreach (DocsPaVO.utente.Ruolo rl in utente.ruoli)
                {
                    if (rl.idGruppo == ruolo.idGruppo)
                        okRuolo = true;
                }
            }

            if (okRuolo)
            {

                string clearToken = string.Empty;
                clearToken += ruolo.systemId + "|";
                clearToken += utente.idPeople + "|";
                clearToken += ruolo.idGruppo + "|";
                clearToken += utente.dst + "|";
                clearToken += utente.idAmministrazione + "|";
                clearToken += utente.userId + "|";
                clearToken += utente.sede + "|";
                clearToken += utente.urlWA;

                token = Utils.Encrypt(clearToken);

            }
            else
            {
                logger.Debug("non è stato trovato il ruolo indicato");
                messaggioErrore = "non è stato trovato il ruolo indicato";
                throw new Exception("non è stato trovato il ruolo indicato");
            }

            if (string.IsNullOrEmpty(token))
            {
                logger.Debug("l'accoppiata utente ruolo non esiste");
                messaggioErrore = "l'accoppiata utente ruolo non esiste";
                throw new Exception("l'accoppiata utente ruolo non esiste");
            }


            DocsPaVO.utente.InfoUtente infoUtente = Utils.getInfoUtenteFromToken(token);

            //RICERCA DEL DOCUMENTO
            DocsPaVO.documento.SchedaDocumento schedaDocumento = null;
            if (documentoProtocollato)
            {

                if (!string.IsNullOrEmpty(segnatura)
                    && documentoProtocollato)
                    schedaDocumento = BusinessLogic.Documenti.ProtoManager.ricercaScheda(segnatura, infoUtente);
                else
                    messaggioErrore = "Errore nell'inserimento della segnatura";
            }
            else
            {
                //docuemento non protocollato
                if (!string.IsNullOrEmpty(idDocumento)
                    && !documentoProtocollato)
                {
                    DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
                    schedaDocumento = doc.GetSchedaDocumentoByID(infoUtente, idDocumento);
                }
                else
                    messaggioErrore = "idDocumento non presente";

            }
            if (schedaDocumento != null)
            {
                logger.Debug("documento Trovato!");
                messaggioErrore = "documento Trovato!";

                DocsPaVO.addressbook.QueryCorrispondente qco = null;
                switch (schedaDocumento.tipoProto)
                {
                    case "A":
                        DocsPaVO.documento.ProtocolloEntrata pr = (DocsPaVO.documento.ProtocolloEntrata)schedaDocumento.protocollo;
                        if (dettagliCorrispondente.ToUpper().Equals("M") || dettagliCorrispondente.ToUpper().Equals("T"))
                        {
                            qco = this.getQueryCorrispondente(pr.mittente.systemId, infoUtente.idAmministrazione, pr.mittente.idRegistro);


                            if (pr.mittente.tipoIE.ToUpper().Equals("I"))
                            {
                                qco.tipoUtente = DocsPaVO.addressbook.TipoUtente.INTERNO;
                                pr.mittente = BusinessLogic.Utenti.UserManager.getCorrispondenteCompletoBySystemId(pr.mittente.systemId, DocsPaVO.addressbook.TipoUtente.INTERNO, infoUtente);
                            }
                            else
                            {
                                qco.tipoUtente = DocsPaVO.addressbook.TipoUtente.ESTERNO;
                                pr.mittente = BusinessLogic.Utenti.UserManager.getCorrispondenteCompletoBySystemId(pr.mittente.systemId, DocsPaVO.addressbook.TipoUtente.ESTERNO, infoUtente);
                            }

                            pr.mittente = this.getMergeCorrispondente(pr.mittente, qco);
                        }
                        schedaDocumento.protocollo = pr;
                        break;

                    case "P":
                        DocsPaVO.documento.ProtocolloUscita pr1 = (DocsPaVO.documento.ProtocolloUscita)schedaDocumento.protocollo;
                        qco = this.getQueryCorrispondente(pr1.mittente.systemId, infoUtente.idAmministrazione, pr1.mittente.idRegistro);

                        if (dettagliCorrispondente.ToUpper().Equals("M") || dettagliCorrispondente.ToUpper().Equals("T"))
                        {
                            if (pr1.mittente.tipoIE.ToUpper().Equals("I"))
                            {
                                qco.tipoUtente = DocsPaVO.addressbook.TipoUtente.INTERNO;
                                pr1.mittente = BusinessLogic.Utenti.UserManager.getCorrispondenteCompletoBySystemId(pr1.mittente.systemId, DocsPaVO.addressbook.TipoUtente.INTERNO, infoUtente);
                            }
                            else
                            {
                                qco.tipoUtente = DocsPaVO.addressbook.TipoUtente.ESTERNO;
                                pr1.mittente = BusinessLogic.Utenti.UserManager.getCorrispondenteCompletoBySystemId(pr1.mittente.systemId, DocsPaVO.addressbook.TipoUtente.ESTERNO, infoUtente);
                            }
                            pr1.mittente = this.getMergeCorrispondente(pr1.mittente, qco);
                        }

                        if (dettagliCorrispondente.ToUpper().Equals("D") || dettagliCorrispondente.ToUpper().Equals("T"))
                        {
                            if (pr1.destinatari != null && pr1.destinatari.Count > 0)
                            {
                                for (int i = 0; i < pr1.destinatari.Count; i++)
                                {
                                    qco = this.getQueryCorrispondente(((DocsPaVO.utente.Corrispondente)pr1.destinatari[i]).systemId, infoUtente.idAmministrazione, ((DocsPaVO.utente.Corrispondente)pr1.destinatari[i]).idRegistro);

                                    if (((DocsPaVO.utente.Corrispondente)pr1.destinatari[i]).tipoIE.ToUpper().Equals("I"))
                                    {
                                        qco.tipoUtente = DocsPaVO.addressbook.TipoUtente.INTERNO;
                                        pr1.destinatari[i] = BusinessLogic.Utenti.UserManager.getCorrispondenteCompletoBySystemId(((DocsPaVO.utente.Corrispondente)pr1.destinatari[i]).systemId, DocsPaVO.addressbook.TipoUtente.INTERNO, infoUtente);
                                    }
                                    else
                                    {
                                        qco.tipoUtente = DocsPaVO.addressbook.TipoUtente.ESTERNO;
                                        pr1.destinatari[i] = BusinessLogic.Utenti.UserManager.getCorrispondenteCompletoBySystemId(((DocsPaVO.utente.Corrispondente)pr1.destinatari[i]).systemId, DocsPaVO.addressbook.TipoUtente.ESTERNO, infoUtente);

                                    }

                                    pr1.destinatari[i] = this.getMergeCorrispondente((DocsPaVO.utente.Corrispondente)pr1.destinatari[i], qco);
                                }
                            }
                        }

                        schedaDocumento.protocollo = pr1;
                        break;
                    case "I":
                        DocsPaVO.documento.ProtocolloInterno pr2 = (DocsPaVO.documento.ProtocolloInterno)schedaDocumento.protocollo;
                        qco = this.getQueryCorrispondente(pr2.mittente.systemId, infoUtente.idAmministrazione, pr2.mittente.idRegistro);


                        if (dettagliCorrispondente.ToUpper().Equals("M") || dettagliCorrispondente.ToUpper().Equals("T"))
                        {
                            if (pr2.mittente.tipoIE.ToUpper().Equals("I"))
                            {
                                qco.tipoUtente = DocsPaVO.addressbook.TipoUtente.INTERNO;
                                pr2.mittente = BusinessLogic.Utenti.UserManager.getCorrispondenteCompletoBySystemId(pr2.mittente.systemId, DocsPaVO.addressbook.TipoUtente.INTERNO, infoUtente);

                            }
                            else
                            {
                                qco.tipoUtente = DocsPaVO.addressbook.TipoUtente.ESTERNO;
                                pr2.mittente = BusinessLogic.Utenti.UserManager.getCorrispondenteCompletoBySystemId(pr2.mittente.systemId, DocsPaVO.addressbook.TipoUtente.ESTERNO, infoUtente);

                            }
                            pr2.mittente = this.getMergeCorrispondente(pr2.mittente, qco);
                        }

                        if (dettagliCorrispondente.ToUpper().Equals("D") || dettagliCorrispondente.ToUpper().Equals("T"))
                        {
                            if (pr2.destinatari != null && pr2.destinatari.Count > 0)
                            {
                                for (int i = 0; i < pr2.destinatari.Count; i++)
                                {
                                    qco = this.getQueryCorrispondente(((DocsPaVO.utente.Corrispondente)pr2.destinatari[i]).systemId, infoUtente.idAmministrazione, ((DocsPaVO.utente.Corrispondente)pr2.destinatari[i]).idRegistro);

                                    if (((DocsPaVO.utente.Corrispondente)pr2.destinatari[i]).tipoIE.ToUpper().Equals("I"))
                                    {
                                        qco.tipoUtente = DocsPaVO.addressbook.TipoUtente.INTERNO;
                                        pr2.destinatari[i] = BusinessLogic.Utenti.UserManager.getCorrispondenteCompletoBySystemId(((DocsPaVO.utente.Corrispondente)pr2.destinatari[i]).systemId, DocsPaVO.addressbook.TipoUtente.INTERNO, infoUtente);

                                    }
                                    else
                                    {
                                        qco.tipoUtente = DocsPaVO.addressbook.TipoUtente.ESTERNO;
                                        pr2.destinatari[i] = BusinessLogic.Utenti.UserManager.getCorrispondenteCompletoBySystemId(((DocsPaVO.utente.Corrispondente)pr2.destinatari[i]).systemId, DocsPaVO.addressbook.TipoUtente.ESTERNO, infoUtente);

                                    }
                                    pr2.destinatari[i] = this.getMergeCorrispondente((DocsPaVO.utente.Corrispondente)pr2.destinatari[i], qco);
                                }
                            }
                        }

                        schedaDocumento.protocollo = pr2;
                        break;
                }

                //ritorno della scheda documento
                return schedaDocumento;
            }

            return null;
        }



        private DocsPaVO.addressbook.QueryCorrispondente getQueryCorrispondente(string systemId, string idAmministrazione, string idRegistro)
        {
            if (!string.IsNullOrEmpty(systemId) &&
                !string.IsNullOrEmpty(idAmministrazione))
            {
                DocsPaVO.addressbook.QueryCorrispondente qco = new DocsPaVO.addressbook.QueryCorrispondente();
                ArrayList al = new ArrayList();
                al.Add(idRegistro);
                qco.systemId = systemId;
                qco.getChildren = false;
                qco.idAmministrazione = idAmministrazione;
                qco.idRegistri = al;
                qco.fineValidita = true;

                return qco;
            }

            return null;

        }

        private DocsPaVO.utente.Corrispondente getMergeCorrispondente(DocsPaVO.utente.Corrispondente corr, DocsPaVO.addressbook.QueryCorrispondente qc)
        {
            if (corr != null && qc != null)
            {

                DataSet dataSet = new DataSet();

                DocsPaVO.addressbook.DettagliCorrispondente dettagliCorr = new DocsPaVO.addressbook.DettagliCorrispondente();

                DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();
                utenti.DettagliCorrispondenti(out dataSet, qc);


                if (dataSet.Tables["DETTAGLI"].Rows.Count > 0)
                {
                    DataRow dettagliRow = dataSet.Tables["DETTAGLI"].Rows[0];

                    corr.cap = dettagliRow["VAR_CAP"].ToString();
                    corr.citta = dettagliRow["VAR_CITTA"].ToString();
                    corr.codfisc = dettagliRow["VAR_COD_FISCALE"].ToString();
                    corr.indirizzo = dettagliRow["VAR_INDIRIZZO"].ToString();
                    corr.prov = dettagliRow["VAR_PROVINCIA"].ToString();
                    corr.nazionalita = dettagliRow["VAR_NAZIONE"].ToString();
                    corr.telefono1 = dettagliRow["VAR_TELEFONO"].ToString();
                    corr.telefono2 = dettagliRow["VAR_TELEFONO2"].ToString();
                    corr.fax = dettagliRow["VAR_FAX"].ToString();
                    corr.note = dettagliRow["VAR_NOTE"].ToString();
                }
            }

            return corr;
        }

        /// <summary>
        /// Ricerca dei documenti con esportazione dei campi profilati
        /// </summary>
        /// <param name="tokenDiAutenticazione">Token di autenticazione</param>
        /// <param name="options">parametri di ricerca</param>
        /// <returns></returns>
        [WebMethod(Description = "Ricerca dei documenti con esportazione dei campi profilati")]
        [XmlInclude(typeof(Proprieta))]
        public SearchResponse SearchDocuments(string tokenDiAutenticazione, SearchOptions options)
        {

            ServiceLoadController.LoadControllerManager.CheckInterval("SmartServices", "SearchDocuments");
            try
            {
              
                SearchResponse response = new SearchResponse();

                List<OggettoProfile> data = new List<OggettoProfile>();

                DocsPaVO.utente.InfoUtente infoUtente = Utils.getInfoUtenteFromToken(tokenDiAutenticazione);

                // Mapping criteri di filtro standard
                List<DocsPaVO.filtri.FiltroRicerca> filtriRicercaStandard = new List<DocsPaVO.filtri.FiltroRicerca>();

                if (options.SearchProperties != null)
                {
                    foreach (Proprieta p in options.SearchProperties.Where(e => !e.ProprietaCampiProfilati).ToArray())
                    {
                        DocsPaVO.filtri.FiltroRicerca filter = new DocsPaVO.filtri.FiltroRicerca();
                        filter.argomento = p.Nome;
                        filter.valore = p.Valore;
                        filtriRicercaStandard.Add(filter);
                    }
                }

                // Verifica la presenza del parametro obbligatorio TIPO
                if (filtriRicercaStandard.Count(e => e.argomento == DocsPaVO.filtri.ricerca.listaArgomenti.TIPO.ToString()) == 0)
                {
                    DocsPaVO.filtri.FiltroRicerca filter = new DocsPaVO.filtri.FiltroRicerca();
                    filter.argomento = DocsPaVO.filtri.ricerca.listaArgomenti.TIPO.ToString();
                    filter.valore = "T";
                    filtriRicercaStandard.Add(filter);
                }

                DocsPaVO.RicercaLite.CampiProfilati filtriRicercaCampiProfilati = null;

                // Mapping criteri di filtro template profilazione
                if (!string.IsNullOrEmpty(options.NomeDocumentoCampiProfilati))
                {
                    filtriRicercaCampiProfilati = new DocsPaVO.RicercaLite.CampiProfilati();
                    filtriRicercaCampiProfilati.nomeDocumento = options.NomeDocumentoCampiProfilati;

                    List<DocsPaVO.RicercaLite.CampoProfilatoAvanzata> list = new List<DocsPaVO.RicercaLite.CampoProfilatoAvanzata>();

                    if (options.SearchProperties != null)
                    {
                        foreach (Proprieta p in options.SearchProperties.Where(e => e.ProprietaCampiProfilati).ToArray())
                        {
                            DocsPaVO.RicercaLite.CampoProfilatoAvanzata itm = new DocsPaVO.RicercaLite.CampoProfilatoAvanzata();
                            itm.nomeCampo = p.Nome;
                            itm.valoreCampo = p.Valore.ToString();
                            list.Add(itm);

                            DocsPaVO.RicercaLite.CampoProfilatoAvanzata itm2 = null;

                            if (!string.IsNullOrEmpty(p.Valore2))
                            {
                                itm2 = new DocsPaVO.RicercaLite.CampoProfilatoAvanzata();
                                itm2.nomeCampo = p.Nome;
                                itm2.valoreCampo = p.Valore2;
                                itm2.IsIntervalloA = 1;
                                list.Add(itm2);
                            }

                            if (itm2 != null)
                                itm.IsIntervalloDa = 1;
                        }
                    }

                    filtriRicercaCampiProfilati.campiProfilati = list.ToArray();
                }

                DocsPaVO.filtri.FiltroRicerca[][] filtriRicerca = BusinessLogic.ProfilazioneAvanzata.ProfilazioneAvanzata.ricercaProfilazioneAvanzata(infoUtente, filtriRicercaStandard.ToArray(), filtriRicercaCampiProfilati);

                if (filtriRicerca != null)
                {
                    int numTotPage, nRec;
                    List<SearchResultInfo> idProfileList;

                    int numPage = 1;
                    int pageSize = Int32.MaxValue;

                    if (options.PagineNelContesto != null)
                    {
                        numPage = options.PagineNelContesto.NumeroDiPagineRichieste;
                        pageSize = options.PagineNelContesto.OggettiPerPagina;
                    }

                ArrayList infoDocumento = BusinessLogic.Documenti.InfoDocManager.getQueryPaging(
                                                    infoUtente.idGruppo,
                                                    infoUtente.idPeople,
                                                    setFiltriRicercaDoc(filtriRicerca),
                                                    false,
                                                    numPage,
                                                    pageSize,
                                                    true,
                                                    out numTotPage,
                                                    out nRec,
                                                    false,
                                                    out idProfileList,
                                                    true
                                                );

                    if (options.PagineNelContesto != null)
                    {
                        options.PagineNelContesto.TotaleNumeroOggettiRicercati = nRec;
                        options.PagineNelContesto.NumeroTotaleDiPagine = numTotPage;
                    }

                    List<DocsPaVO.RicercaLite.Profilo> listaDocumenti = BusinessLogic.ProfilazioneAvanzata.ProfilazioneAvanzata.getProfiloAvanzato(infoUtente, infoDocumento);

                    foreach (DocsPaVO.RicercaLite.Profilo doc in listaDocumenti)
                    {



                        // Nuovo documento
                        OggettoProfile profile = new OggettoProfile();
                        profile.Id = doc.idProfile;
                        profile.Description = doc.oggetto;
                        //profile.ObjectTemplateName = options.ObjectTemplateName;

                        List<Proprieta> props = new List<Proprieta>();
                        // Iterazione sui fields dell'oggetto documento
                        foreach (System.Reflection.FieldInfo field in doc.GetType().GetFields())
                        {
                            if (field.Name.ToLower() != "campiprofilati".ToLower())
                            {
                                Proprieta prop = new Proprieta();


                                prop.Nome = field.Name;

                                if (field.GetType() == typeof(Boolean))
                                    prop.Tipo = PropertyTypesEnum.Boolean;
                                else if (field.GetType() == typeof(DateTime))
                                    prop.Tipo = PropertyTypesEnum.Date;
                                else if (field.GetType() == typeof(Int32) ||
                                            field.GetType() == typeof(Int16) ||
                                            field.GetType() == typeof(Int64) ||
                                            field.GetType() == typeof(Decimal))
                                    prop.Tipo = PropertyTypesEnum.Numeric;
                                else
                                    prop.Tipo = PropertyTypesEnum.String;

                                object fieldValue = field.GetValue(doc);
                                if (fieldValue != null)
                                    prop.Valore = fieldValue.ToString();
                                props.Add(prop);
                            }
                        }






                        // Iterazione sulle properties dell'oggetto documento
                        foreach (System.Reflection.PropertyInfo property in doc.GetType().GetProperties())
                        {
                            if (property.Name.ToLower() != "campiprofilati".ToLower())
                            {
                                Proprieta prop = new Proprieta();

                                prop.Nome = property.Name;

                                if (property.GetType() == typeof(Boolean))
                                    prop.Tipo = PropertyTypesEnum.Boolean;
                                else if (property.GetType() == typeof(DateTime))
                                    prop.Tipo = PropertyTypesEnum.Date;
                                else if (property.GetType() == typeof(Int32) ||
                                            property.GetType() == typeof(Int16) ||
                                            property.GetType() == typeof(Int64) ||
                                            property.GetType() == typeof(Decimal))
                                    prop.Tipo = PropertyTypesEnum.Numeric;
                                else
                                    prop.Tipo = PropertyTypesEnum.String;

                                object fieldValue = property.GetValue(doc, null);
                                if (fieldValue != null)
                                    prop.Valore = fieldValue.ToString();

                                props.Add(prop);
                            }
                        }

                        bool ok = false;

                        foreach (DocsPaVO.documento.InfoDocumento.CampoProfiloInfoDocumento ca in doc.CampiProfilati)
                        {
                            ok = true;
                            Proprieta prop = new Proprieta();
                            prop.Nome = ca.NomeCampo;

                            if (!string.IsNullOrEmpty(ca.ValoreCampo))
                                prop.Valore = ca.ValoreCampo;
                            else
                                prop.Valore = string.Empty;

                            prop.Valore2 = string.Empty;
                            prop.ProprietaCampiProfilati = true;
                            prop.Tipo = PropertyTypesEnum.String;
                            props.Add(prop);
                        }

                        if (ok)
                            profile.NomeDocumentoProfilazioneDinamica = options.NomeDocumentoCampiProfilati;

                        profile.Properties = props.ToArray();

                        data.Add(profile);
                    }

                    if (options.PagineNelContesto != null)
                        response.Paginazione = options.PagineNelContesto;

                    response.Objects = data.ToArray();
                }

                return response;
            }
            catch (Exception ex)
            {
                throw DocsPaUtils.Exceptions.SoapExceptionFactory.Create(ex);
            }
        }
        #endregion

        #region Migrazione Massiva
        [WebMethod()]
        public virtual bool CheckMittDestTrasm(string codice_ruolo, string id_modello, string tipo)
        {
            bool res = false;
            try
            {
                DocsPaDB.Query_DocsPAWS.MigrazioneMassivaDocFascTDL migrazione = new DocsPaDB.Query_DocsPAWS.MigrazioneMassivaDocFascTDL();
                res = migrazione.CheckMittDestTrasm(codice_ruolo, id_modello, tipo);
            }
            catch (Exception e)
            {
                return false;
            }

            return res;
        }

        [WebMethod()]
        public virtual string GetCodiceFasc(string systemId)
        {
            string codice = string.Empty;
            try
            {
                DocsPaDB.Query_DocsPAWS.MigrazioneMassivaDocFascTDL migrazione = new DocsPaDB.Query_DocsPAWS.MigrazioneMassivaDocFascTDL();
                codice = migrazione.GetCodiceFasc(systemId);
            }
            catch (Exception e)
            {
            }

            return codice;
        }

        [WebMethod()]
        public virtual bool SetNoteGenerali(string systemId, string note)
        {
            string codice = string.Empty;
            try
            {
                DocsPaDB.Query_DocsPAWS.MigrazioneMassivaDocFascTDL migrazione = new DocsPaDB.Query_DocsPAWS.MigrazioneMassivaDocFascTDL();
                return migrazione.SetNoteGenerali(systemId, note);
            }
            catch (Exception e)
            {
                return false;
            }
        }

        [WebMethod()]
        public virtual string[] GetSystemIdDocumentiRconRisalita(string codice_ruolo)
        {
            ServiceLoadController.LoadControllerManager.CheckInterval("SmartServices", "GetSystemIdDocumentiRconRisalita");
            string[] systemId = null;
            try
            {
                DocsPaDB.Query_DocsPAWS.MigrazioneMassivaDocFascTDL migrazione = new DocsPaDB.Query_DocsPAWS.MigrazioneMassivaDocFascTDL();
                systemId = migrazione.GetSystemIdDocumentiRconRisalita(codice_ruolo);
            }
            catch (Exception e)
            {
            }

            return systemId;
        }

        [WebMethod()]
        public virtual string[] GetSystemIdDocumentiRsenzaRisalita(string codice_ruolo)
        {
            ServiceLoadController.LoadControllerManager.CheckInterval("SmartServices", "GetSystemIdDocumentiRsenzaRisalita");
            string[] systemId = null;
            try
            {
                DocsPaDB.Query_DocsPAWS.MigrazioneMassivaDocFascTDL migrazione = new DocsPaDB.Query_DocsPAWS.MigrazioneMassivaDocFascTDL();
                systemId = migrazione.GetSystemIdDocumentiRsenzaRisalita(codice_ruolo);
            }
            catch (Exception e)
            {
            }

            return systemId;
        }

        [WebMethod()]
        public virtual string[] GetSystemIdDocumentiRWsenzaRisalita(string codice_ruolo)
        {
            ServiceLoadController.LoadControllerManager.CheckInterval("SmartServices", "GetSystemIdDocumentiRWsenzaRisalita");
            string[] systemId = null;
            try
            {
                DocsPaDB.Query_DocsPAWS.MigrazioneMassivaDocFascTDL migrazione = new DocsPaDB.Query_DocsPAWS.MigrazioneMassivaDocFascTDL();
                systemId = migrazione.GetSystemIdDocumentiRWsenzaRisalita(codice_ruolo);
            }
            catch (Exception e)
            {
            }

            return systemId;
        }


        [WebMethod()]
        public virtual string[] GetSystemIdDocumentiRWconRisalita(string codice_ruolo)
        {
            ServiceLoadController.LoadControllerManager.CheckInterval("SmartServices", "GetSystemIdDocumentiRWconRisalita");
            string[] systemId = null;
            try
            {
                DocsPaDB.Query_DocsPAWS.MigrazioneMassivaDocFascTDL migrazione = new DocsPaDB.Query_DocsPAWS.MigrazioneMassivaDocFascTDL();
                systemId = migrazione.GetSystemIdDocumentiRWconRisalita(codice_ruolo);
            }
            catch (Exception e)
            {
            }

            return systemId;
        }


        [WebMethod()]
        public virtual string[] GetSystemIdFascicoliRconRisalita(string codice_ruolo)
        {
            ServiceLoadController.LoadControllerManager.CheckInterval("SmartServices", "GetSystemIdFascicoliRconRisalita");
            string[] systemId = null;
            try
            {
                DocsPaDB.Query_DocsPAWS.MigrazioneMassivaDocFascTDL migrazione = new DocsPaDB.Query_DocsPAWS.MigrazioneMassivaDocFascTDL();
                systemId = migrazione.GetSystemIdFascicoliRconRisalita(codice_ruolo);
            }
            catch (Exception e)
            {
            }

            return systemId;
        }


        [WebMethod()]
        public virtual string[] GetSystemIdFascicoliRsenzaRisalita(string codice_ruolo)
        {
            ServiceLoadController.LoadControllerManager.CheckInterval("SmartServices", "GetSystemIdFascicoliRsenzaRisalita");
            string[] systemId = null;
            try
            {
                DocsPaDB.Query_DocsPAWS.MigrazioneMassivaDocFascTDL migrazione = new DocsPaDB.Query_DocsPAWS.MigrazioneMassivaDocFascTDL();
                systemId = migrazione.GetSystemIdFascicoliRsenzaRisalita(codice_ruolo);
            }
            catch (Exception e)
            {

                DocsPaDB.Query_DocsPAWS.MigrazioneMassivaDocFascTDL migrazione = new DocsPaDB.Query_DocsPAWS.MigrazioneMassivaDocFascTDL();
                systemId = migrazione.GetSystemIdFascicoliRsenzaRisalita(codice_ruolo);
            }

            return systemId;
        }


        [WebMethod()]
        public virtual string[] GetSystemIdFascicoliRWsenzaRisalita(string codice_ruolo)
        {
            ServiceLoadController.LoadControllerManager.CheckInterval("SmartServices", "GetSystemIdFascicoliRWsenzaRisalita");

            string[] systemId = null;
            try
            {

                DocsPaDB.Query_DocsPAWS.MigrazioneMassivaDocFascTDL migrazione = new DocsPaDB.Query_DocsPAWS.MigrazioneMassivaDocFascTDL();
                systemId = migrazione.GetSystemIdFascicoliRWsenzaRisalita(codice_ruolo);
            }
            catch (Exception e)
            {
            }

            return systemId;
        }


        [WebMethod()]
        public virtual string[] GetSystemIdFascicoliRWconRisalita(string codice_ruolo)
        {
            ServiceLoadController.LoadControllerManager.CheckInterval("SmartServices", "GetSystemIdFascicoliRWconRisalita");
            string[] systemId = null;
            try
            {
                DocsPaDB.Query_DocsPAWS.MigrazioneMassivaDocFascTDL migrazione = new DocsPaDB.Query_DocsPAWS.MigrazioneMassivaDocFascTDL();
                systemId = migrazione.GetSystemIdFascicoliRWconRisalita(codice_ruolo);
            }
            catch (Exception e)
            {
            }

            return systemId;
        }


        /// <returns></returns>
        [WebMethod]
        public virtual bool SetDataVistaSP(DocsPaVO.utente.InfoUtente infoutente, string docNumber, string docOrFasc)
        {
            ServiceLoadController.LoadControllerManager.CheckInterval("SmartServices", "SetDataVistaSP");

            return BusinessLogic.Documenti.DocManager.setDataVistaSP(infoutente, docNumber, docOrFasc);
        }

        [WebMethod]
        [return: XmlArray()]
        [return: XmlArrayItem(typeof(DocsPaVO.trasmissione.Trasmissione))]
        public virtual ArrayList trasmGetDettaglioTrasmissioneByInfoutente(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.trasmissione.infoToDoList todo)
        {
            try
            {
                //DocsPaVO.utente.Utente ut, DocsPaVO.utente.Ruolo role,
                //                DocsPaVO.utente.InfoUtente infoUtente = Utils.getInfoUtenteFromToken(tokenDiAutenticazione);

                DocsPaVO.utente.Ruolo role = Utils.getRuoloFromInfoUtente(infoUtente);
                DocsPaVO.utente.Utente ut = new DocsPaVO.utente.Utente();
                ut.idPeople = infoUtente.idPeople;
                DocsPaVO.trasmissione.OggettoTrasm oggettoTrasm = new DocsPaVO.trasmissione.OggettoTrasm();

                if (!string.IsNullOrEmpty(todo.sysIdDoc))
                {
                    using (DocsPaDB.Query_DocsPAWS.Documenti dbDoc = new DocsPaDB.Query_DocsPAWS.Documenti())
                        oggettoTrasm.infoDocumento = dbDoc.GetInfoDocumento(role.idGruppo, infoUtente.idPeople, todo.sysIdDoc, false);

                    oggettoTrasm.infoFascicolo = null;
                }
                else if (!string.IsNullOrEmpty(todo.sysIdFasc))
                {
                    oggettoTrasm.infoDocumento = null;
                    oggettoTrasm.infoFascicolo = new DocsPaVO.fascicolazione.InfoFascicolo { idFascicolo = todo.sysIdFasc };
                }

                return BusinessLogic.Trasmissioni.QueryTrasmManager.getQueryDettaglioTrasmMethod(oggettoTrasm, ut, role, null, todo.sysIdTrasm);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: trasmGetDettaglioTrasmissione", e);
                throw e;
            }
        }
        #endregion

        #region classi interne per la ricerca documenti campi profilati
        /// <summary>
        /// Tipologie di dato
        /// </summary>
        public enum PropertyTypesEnum
        {
            String,
            Numeric,
            Date,
            Boolean,
        }

        /// <summary>
        /// Dati di una proprietà di un oggetto
        /// </summary>
        [Serializable()]
        public class Proprieta
        {
            /// <summary>
            /// 
            /// </summary>
            public Proprieta()
            {
                // Per default, proprietà di tipo string
                this.Tipo = PropertyTypesEnum.String;
            }

            /// <summary>
            /// Nome della proprietà
            /// </summary>
            public string Nome
            {
                get;
                set;
            }

            /// <summary>
            /// Tipo dato proprietà
            /// </summary>
            public PropertyTypesEnum Tipo
            {
                get;
                set;
            }

            /// <summary>
            /// Valore proprietà
            /// </summary>
            public string Valore
            {
                get;
                set;
            }

            /// <summary>
            /// Valore proprietà 2
            /// </summary>
            public string Valore2
            {
                get;
                set;
            }

            /// <summary>
            /// Indica se la proprietà si riferisce ad un template oggetto 
            /// </summary>
            public bool ProprietaCampiProfilati
            {
                get;
                set;
            }
        }

        /// <summary>
        /// Opzioni di ricerca
        /// </summary>
        [Serializable()]
        public class SearchOptions
        {
            /// <summary>
            /// 
            /// </summary>
            public PagingContext PagineNelContesto
            {
                get;
                set;
            }

            /// <summary>
            /// Nome template per l'oggetto da ricercare
            /// </summary>
            public string NomeDocumentoCampiProfilati
            {
                get;
                set;
            }

            /// <summary>
            /// Array contenente le proprietà di ricerca
            /// </summary>
            public Proprieta[] SearchProperties
            {
                get;
                set;
            }
        }

        /// <summary>
        /// Dati di profilo di un oggetto nel sistema
        /// </summary>
        [Serializable()]
        public class OggettoProfile
        {
            /// <summary>
            /// 
            /// </summary>
            public OggettoProfile()
            {
                this.Properties = new Proprieta[0];
            }

            /// <summary>
            /// Rappresenta l'id univoco dell'oggetto
            /// </summary>
            public string Id
            {
                get;
                set;
            }

            /// <summary>
            /// Descrizione testuale dell'oggetto
            /// </summary>
            public string Description
            {
                get;
                set;
            }

            /// <summary>
            /// Tipologia oggetto
            /// </summary>
            public string NomeDocumentoProfilazioneDinamica
            {
                get;
                set;
            }

            /// <summary>
            /// Proprietà estese di un oggetto
            /// </summary>
            public Proprieta[] Properties
            {
                get;
                set;
            }
        }

        /// <summary>
        /// Criteri di paginazione per le ricerche
        /// </summary>
        [Serializable()]
        public class PagingContext
        {
            /// <summary>
            /// Indica il numero di pagina richiesto 
            /// </summary>
            public int NumeroDiPagineRichieste
            {
                get;
                set;
            }

            /// <summary>
            /// Indica il numero di oggetti da includere nella ricerca per ciascuna pagina 
            /// </summary>
            public int OggettiPerPagina
            {
                get;
                set;
            }

            /// <summary>
            /// Indica il numero totale delle pagine restituite dalla ricerca 
            /// </summary>
            /// <remarks>
            /// Il dato è disponibile solamente come risultato della ricerca effettuata 
            /// </remarks>
            public int NumeroTotaleDiPagine
            {
                get;
                set;
            }

            /// <summary>
            /// Indica il numero totale di oggetti (non paginati) restituiti dalla ricerca 
            /// </summary>
            /// <remarks>
            /// Il dato è disponibile solamente come risultato della ricerca effettuata 
            /// </remarks>
            public int TotaleNumeroOggettiRicercati
            {
                get;
                set;
            }
        }


        /// <summary>
        /// Risultato ricerca
        /// </summary>
        [Serializable()]
        public class SearchResponse
        {
            /// <summary>
            /// 
            /// </summary>
            public PagingContext Paginazione
            {
                get;
                set;
            }

            /// <summary>
            /// Oggetti restituiti dalla ricerca
            /// </summary>
            public OggettoProfile[] Objects
            {
                get;
                set;
            }
        }

        #endregion

    }   
}
