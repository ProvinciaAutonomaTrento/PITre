using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using RESTServices.Model.Domain;
using RESTServices.Model.Responses;
using RESTServices.Model.Requests;
using log4net;
using System.Collections;

namespace RESTServices.Manager
{
    public class TransmissionsManager
    {
        private static ILog logger = LogManager.GetLogger(typeof(TransmissionsManager));

        public static async Task<TransmissionResponse> executeTransmDocModel(string token, ExecuteTransmDocModelRequest request)
        {
            logger.Info("begin executeTransmDocModel");
            TransmissionResponse response = new TransmissionResponse();
            try
            {
                #region controllo Token
                if (string.IsNullOrWhiteSpace(token))
                {
                    logger.Error("Missing AuthToken Header, manca il token di autenticazione");
                    throw new Exception("Missing AuthToken Header");
                }
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();

                string authInfoString = Utils.Decrypt(token.Substring(4));
                string[] authInfoArray = authInfoString.Split('|');

                utente = BusinessLogic.Utenti.UserManager.getUtente(authInfoArray[5], authInfoArray[4]);
                ruolo = BusinessLogic.Utenti.UserManager.getRuoloById(authInfoArray[0]);
                infoUtente = new DocsPaVO.utente.InfoUtente(utente, ruolo);
                infoUtente.codWorkingApplication = authInfoArray[8]; infoUtente.dst = authInfoArray[3];

                #endregion
                bool controlloPerUtDiSistema = true;

                if (string.IsNullOrEmpty(request.DocumentId))
                {
                    throw new RestException("REQUIRED_ID");
                }


                if (string.IsNullOrEmpty(request.IdModel))
                {
                    throw new RestException("REQUIRED_ID_MODEL");
                }



                DocsPaVO.documento.SchedaDocumento documento = new DocsPaVO.documento.SchedaDocumento();
                DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione model = new DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione();

                try
                {
                    documento = BusinessLogic.Documenti.DocManager.getDettaglio(infoUtente, request.DocumentId, request.DocumentId);

                }
                catch (Exception ex)
                {
                    throw new RestException("DOCUMENT_NOT_FOUND");
                }

                if (documento != null)
                {
                    try
                    {
                        // Modifica Lembo 10-01-2013: Controllo che il modello sia disponibile per il ruolo dell'utente.
                        // Se il modello non è associato al ruolo, non è possibile effettuare la trasmissione, e restituisce modello non trovato.
                        DocsPaVO.utente.Registro[] registri = null;
                        ArrayList registriArr = BusinessLogic.Utenti.RegistriManager.getListaRegistriRfRuolo(ruolo.systemId, "0", string.Empty);
                        if (registriArr != null && registriArr.Count > 0)
                        {
                            int i = 0;
                            registri = new DocsPaVO.utente.Registro[registriArr.Count];
                            foreach (DocsPaVO.utente.Registro reg in registriArr)
                            {
                                registri[i] = reg;
                                i++;
                            }
                        }
                        ArrayList modelli = BusinessLogic.Trasmissioni.ModelliTrasmissioni.getModelliPerTrasmLite(infoUtente.idAmministrazione, registri, infoUtente.idPeople, infoUtente.idCorrGlobali, string.Empty, string.Empty, string.Empty, "D", string.Empty, infoUtente.idGruppo, false, string.Empty);
                        ArrayList modelliFasc = BusinessLogic.Trasmissioni.ModelliTrasmissioni.getModelliPerTrasmLite(infoUtente.idAmministrazione, registri, infoUtente.idPeople, infoUtente.idCorrGlobali, string.Empty, string.Empty, string.Empty, "F", string.Empty, infoUtente.idGruppo, false, string.Empty);
                        foreach (DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione mod in modelliFasc)
                        {
                            modelli.Add(mod);
                        }

                        string idModello = request.IdModel;
                        foreach (DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione mod in modelli)
                        {
                            if (mod.SYSTEM_ID.ToString().Equals(idModello))
                            {
                                model = BusinessLogic.Trasmissioni.ModelliTrasmissioni.getModelloByID(infoUtente.idAmministrazione, idModello);
                                if (!string.IsNullOrEmpty(infoUtente.diSistema) && infoUtente.diSistema == "1")
                                {
                                    foreach (DocsPaVO.Modelli_Trasmissioni.RagioneDest rdest in model.RAGIONI_DESTINATARI)
                                    {
                                        if (rdest.RAGIONE != "COMPETENZA_SIST_ESTERNI") { controlloPerUtDiSistema = false; break; }
                                    }
                                }
                                break;
                            }
                        }
                        //model = BusinessLogic.Trasmissioni.ModelliTrasmissioni.getModelloByIDSoloConNotifica(infoUtente.idAmministrazione, request.IdModel);

                    }
                    catch (Exception ex)
                    {
                        throw new RestException("DOCUMENT_NOT_FOUND");
                    }
                }
                if (!controlloPerUtDiSistema)
                {
                    throw new Exception("Errore: il modello di trasmissione utilizza una ragione non valida per i sistemi esterni. Ragione disponibile: COMPETENZA_SIST_ESTERNI.");
                }

                bool result = false;

                // Modifica Lembo 10-01-2013: Cambiamento della clausola if: il model non è mai null.
                //if (model != null)
                if (model.SYSTEM_ID != 0)
                {
                    string pathFE = "";
                    if (System.Configuration.ConfigurationManager.AppSettings["URL_PATH_IS"] != null)
                        pathFE = System.Configuration.ConfigurationManager.AppSettings["URL_PATH_IS"].ToString();

                    result = BusinessLogic.Trasmissioni.TrasmManager.TransmissionExecuteDocTransmFromModelCodeSoloConNotifica(infoUtente, pathFE, documento, model.CODICE, ruolo, out model);
                }
                else
                {
                    throw new RestException("TRANSMISSION_MODEL_NOT_FOUND");
                }

                if (!result)
                {
                    throw new Exception("APPLICATION_ERROR");
                }
                response.Code = TransmissionResponseCode.OK;
                response.TransmMessage = "Effettuata trasmissione con modello";
                logger.Info("end executeTransmDocModel");
            }
            catch (RestException pisEx)
            {
                logger.ErrorFormat("Eccezione executeTransmDocModel: {0}, {1}", pisEx.ErrorCode, pisEx.Description);
                response = new TransmissionResponse();
                response.Code = TransmissionResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                logger.Error("eccezione executeTransmDocModel: " + e);
                response = new TransmissionResponse();
                response.Code = TransmissionResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }
            return response;
        }

        public static async Task<TransmissionResponse> executeTransmPrjModel(string token, ExecuteTransmPrjModelRequest request)
        {
            logger.Info("begin executeTransmPrjModel");
            TransmissionResponse response = new TransmissionResponse();
            try
            {
                #region controllo Token
                if (string.IsNullOrWhiteSpace(token))
                {
                    logger.Error("Missing AuthToken Header, manca il token di autenticazione");
                    throw new Exception("Missing AuthToken Header");
                }
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();

                string authInfoString = Utils.Decrypt(token.Substring(4));
                string[] authInfoArray = authInfoString.Split('|');

                utente = BusinessLogic.Utenti.UserManager.getUtente(authInfoArray[5], authInfoArray[4]);
                ruolo = BusinessLogic.Utenti.UserManager.getRuoloById(authInfoArray[0]);
                infoUtente = new DocsPaVO.utente.InfoUtente(utente, ruolo);
                infoUtente.codWorkingApplication = authInfoArray[8]; infoUtente.dst = authInfoArray[3];

                #endregion
                bool controlloPerUtDiSistema = true;

                if (string.IsNullOrEmpty(request.IdProject))
                {
                    throw new RestException("REQUIRED_ID");
                }


                if (string.IsNullOrEmpty(request.IdModel))
                {
                    throw new RestException("REQUIRED_ID_MODEL");
                }



                DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione model = new DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione();
                DocsPaVO.fascicolazione.Fascicolo fascicolo = new DocsPaVO.fascicolazione.Fascicolo();

                if (!string.IsNullOrEmpty(request.IdProject))
                {
                    fascicolo = BusinessLogic.Fascicoli.FascicoloManager.getFascicoloById(request.IdProject, infoUtente);
                }
                else
                {
                    throw new RestException("PROJECT_NOT_FOUND");
                }

                if (fascicolo != null)
                {
                    try
                    {
                        model = BusinessLogic.Trasmissioni.ModelliTrasmissioni.getModelloByIDSoloConNotifica(infoUtente.idAmministrazione, request.IdModel);
                        if (!string.IsNullOrEmpty(infoUtente.diSistema) && infoUtente.diSistema == "1")
                        {
                            foreach (DocsPaVO.Modelli_Trasmissioni.RagioneDest rdest in model.RAGIONI_DESTINATARI)
                            {
                                if (rdest.RAGIONE != "COMPETENZA_SIST_ESTERNI") { controlloPerUtDiSistema = false; break; }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new RestException("PROJECT_NOT_FOUND");
                    }
                }
                if (!controlloPerUtDiSistema)
                {
                    throw new Exception("Errore: il modello di trasmissione utilizza una ragione non valida per i sistemi esterni. Ragione disponibile: COMPETENZA_SIST_ESTERNI.");
                }
                bool result = false;

                if (model != null)
                {
                    result = BusinessLogic.Trasmissioni.TrasmManager.TrasmissioneExecuteTrasmFascDaModelloSoloConNotifica(string.Empty, fascicolo, model, infoUtente);
                }
                else
                {
                    throw new RestException("TRANSMISSION_MODEL_NOT_FOUND");
                }

                if (!result)
                {
                    throw new Exception("APPLICATION_ERROR");
                }
                response.Code = TransmissionResponseCode.OK;
                response.TransmMessage = "Effettuata trasmissione con modello";


                logger.Info("end executeTransmPrjModel");
            }
            catch (RestException pisEx)
            {
                logger.ErrorFormat("Eccezione executeTransmPrjModel: {0}, {1}", pisEx.ErrorCode, pisEx.Description);
                response = new TransmissionResponse();
                response.Code = TransmissionResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                logger.Error("eccezione executeTransmPrjModel: " + e);
                response = new TransmissionResponse();
                response.Code = TransmissionResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }
            return response;
        }

        public static async Task<TransmissionResponse> executeTransmissionDocument(string token, ExecuteTransmissionDocumentRequest request)
        {
            logger.Info("begin executeTransmissionDocument");
            TransmissionResponse response = new TransmissionResponse();
            try
            {
                #region controllo Token
                if (string.IsNullOrWhiteSpace(token))
                {
                    logger.Error("Missing AuthToken Header, manca il token di autenticazione");
                    throw new Exception("Missing AuthToken Header");
                }
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();

                string authInfoString = Utils.Decrypt(token.Substring(4));
                string[] authInfoArray = authInfoString.Split('|');

                utente = BusinessLogic.Utenti.UserManager.getUtente(authInfoArray[5], authInfoArray[4]);
                ruolo = BusinessLogic.Utenti.UserManager.getRuoloById(authInfoArray[0]);
                infoUtente = new DocsPaVO.utente.InfoUtente(utente, ruolo);
                infoUtente.codWorkingApplication = authInfoArray[8]; infoUtente.dst = authInfoArray[3];

                #endregion
                if (string.IsNullOrEmpty(request.IdDocument))
                {
                    throw new RestException("REQUIRED_ID");
                }


                if (request.Receiver == null)
                {
                    //Destinatario non trovato
                    throw new RestException("REQUIRED_CORRESPONDENT");
                }
                if (string.IsNullOrEmpty(request.Receiver.Code) && string.IsNullOrEmpty(request.Receiver.Id))
                {
                    throw new RestException("REQUIRED_CODE_OR_ID_RECEIVER");
                }
                DocsPaVO.utente.Corrispondente corr = null;
                try
                {
                    if (!string.IsNullOrEmpty(request.Receiver.Id))
                    {
                        corr = BusinessLogic.Utenti.UserManager.getCorrispondenteBySystemID(request.Receiver.Id);
                    }
                    else if (!string.IsNullOrEmpty(request.Receiver.Code))
                    {
                        //corr = BusinessLogic.Utenti.UserManager.getCorrispondenteByCodRubrica(request.Receiver.Code, infoUtente);
                        corr = BusinessLogic.Utenti.UserManager.getCorrispondenteByCodRubrica(request.Receiver.Code, DocsPaVO.addressbook.TipoUtente.INTERNO, infoUtente);
                    }
                    if (corr == null)
                    {
                        throw new RestException("CORRESPONDENT_NOT_FOUND");
                    }

                }
                catch (Exception e)
                {
                    throw new RestException("CORRESPONDENT_NOT_FOUND");
                }

                // La ragione di trasmissione
                DocsPaVO.trasmissione.RagioneTrasmissione ragione = null;
                if (request.TransmissionReason == null || string.IsNullOrEmpty(request.TransmissionReason))
                {
                    throw new RestException("REQUIRED_TRANSMISSION_REASON");
                }
                else if ((!string.IsNullOrEmpty(infoUtente.diSistema)) && (infoUtente.diSistema == "1") && (request.TransmissionReason != "COMPETENZA_SIST_ESTERNI"))
                {
                    throw new Exception("Ragione di trasmissione non valida per un sistema esterno. Ragione disponibile: COMPETENZA_SIST_ESTERNI");
                }
                try
                {
                    ragione = BusinessLogic.Trasmissioni.RagioniManager.getRagioneByCodice(infoUtente.idAmministrazione, request.TransmissionReason.ToUpper());
                    if (ragione == null)
                        throw new RestException("TRANSMISSION_REASON_NOT_FOUND");
                }
                catch (Exception e)
                {
                    throw new RestException("TRANSMISSION_REASON_NOT_FOUND");
                }

                DocsPaVO.documento.SchedaDocumento documento = null;

                try
                {
                    documento = BusinessLogic.Documenti.DocManager.getDettaglio(infoUtente, request.IdDocument, request.IdDocument);

                }
                catch (Exception ex)
                {
                    throw new RestException("DOCUMENT_NOT_FOUND");
                }
                if (documento == null)
                {
                    throw new RestException("DOCUMENT_NOT_FOUND");
                }
                string trasmType = "S";
                if (!string.IsNullOrEmpty(request.TransmissionType) && request.TransmissionType == "T")
                {
                    trasmType = "T";
                }
                #region Cessione diritti
                DocsPaVO.utente.Utente utentePropCessione = null;
                DocsPaVO.utente.InfoUtente infoUtentePropCessione = null;
                DocsPaVO.utente.Ruolo ruoloPropCessione = null;
                if (ragione != null && !string.IsNullOrEmpty(ragione.prevedeCessione) && ragione.prevedeCessione != "N")
                {
                    ragione.cessioneImpostata = true;
                    // proprietario del documento
                    if (documento != null && documento.accessRights == "255")
                    {

                        if (corr.tipoCorrispondente != "R")
                        {
                            throw new Exception("Per la cessione del diritto di proprietà, il destinatario deve essere un ruolo");
                        }
                        if (request == null || request.Receiver == null || string.IsNullOrEmpty(request.Receiver.Note))
                        {

                            //throw new Exception("Per la cessione del diritto di proprietà, è necessario indicare lo userid del nuovo proprietario nel campo Receiver.Note");
                            // Selezione del primo utente nel ruolo
                            ruoloPropCessione = BusinessLogic.Utenti.UserManager.getRuoloByCodice(corr.codiceRubrica);
                            if (ruoloPropCessione != null)
                            {
                                List<DocsPaVO.utente.UserMinimalInfo> userList = BusinessLogic.Utenti.UserManager.GetUsersInRoleMinimalInfo(ruoloPropCessione.idGruppo);
                                if (userList != null && userList.Count > 0)
                                {
                                    utentePropCessione = BusinessLogic.Utenti.UserManager.getUtenteById(userList[0].SystemId);
                                    infoUtentePropCessione = new DocsPaVO.utente.InfoUtente(utentePropCessione, ruoloPropCessione);
                                }
                            }
                        }
                        else
                        {
                            utentePropCessione = BusinessLogic.Utenti.UserManager.getUtente(request.Receiver.Note, infoUtente.idAmministrazione);
                            if (utentePropCessione != null)
                            {
                                DocsPaVO.utente.InfoUtente infoUtenteDaCercare = new DocsPaVO.utente.InfoUtente(utentePropCessione, Utils.GetRuoloPreferito(utentePropCessione.idPeople));
                                ArrayList arrayRuoli = BusinessLogic.Utenti.UserManager.getRuoliUtente(infoUtenteDaCercare.idPeople);
                                bool ruoloPresente = false;
                                if (arrayRuoli != null && arrayRuoli.Count > 0)
                                {
                                    foreach (DocsPaVO.utente.Ruolo rol in arrayRuoli)
                                    {
                                        if (rol.codiceRubrica == corr.codiceRubrica)
                                        {
                                            ruoloPresente = true;
                                            ruoloPropCessione = rol;
                                            break;
                                        }
                                    }
                                }
                                if (!ruoloPresente)
                                {
                                    throw new Exception("L'utente non è presente nel ruolo");
                                }

                            }
                            else
                            {
                                throw new RestException("USER_NO_EXIST");
                            }
                        }
                    }
                    else
                    {
                        if (corr.tipoCorrispondente != "R")
                        {
                            throw new Exception("Per la cessione del diritto, il destinatario deve essere un ruolo");
                        }
                        if (ragione.prevedeCessione == "W")
                        {
                            if (!(request != null && request.Receiver != null && !string.IsNullOrEmpty(request.Receiver.Note)
                                && request.Receiver.Note.ToUpper() == "CEDI"))
                                ragione.cessioneImpostata = false;
                        }
                    }
                }
                #endregion


                try
                {
                    //creazione oggetto trasmissione e popolamento campi.
                    DocsPaVO.trasmissione.Trasmissione transmission = new DocsPaVO.trasmissione.Trasmissione();
                    // Creazione dell'oggetto trasmissione
                    transmission = new DocsPaVO.trasmissione.Trasmissione();

                    // Impostazione dei parametri della trasmissione
                    // Note generali
                    // transmission.noteGenerali = ragione.note;

                    // Tipo oggetto
                    transmission.tipoOggetto = DocsPaVO.trasmissione.TipoOggetto.DOCUMENTO;

                    // Informazioni sul documento
                    transmission.infoDocumento = BusinessLogic.Documenti.DocManager.getInfoDocumento(documento);

                    // Utente mittente della trasmissione
                    transmission.utente = BusinessLogic.Utenti.UserManager.getUtente(infoUtente.idPeople);

                    // Ruolo mittente
                    // Se inserito in ingresso il CodeRole utilizzo il coderole (WIP)
                    transmission.ruolo = BusinessLogic.Utenti.UserManager.getRuolo(infoUtente.idCorrGlobali);
                    string generaNotifica = "1";
                    if (request.Notify != null && !request.Notify)
                    {
                        transmission.NO_NOTIFY = "1";
                        generaNotifica = "0";
                    }
                    else
                    {
                        transmission.NO_NOTIFY = "0";
                        generaNotifica = "1";
                    }
                    // Modifica per invio in mail dell'url del frontend
                    string urlfrontend = "";
                    if (System.Configuration.ConfigurationManager.AppSettings["URL_PATH_IS"] != null)
                        urlfrontend = System.Configuration.ConfigurationManager.AppSettings["URL_PATH_IS"].ToString();
                    // Cessione diritti
                    if (ragione != null && !string.IsNullOrEmpty(ragione.prevedeCessione) && ragione.prevedeCessione != "N" && ragione.cessioneImpostata)
                    {
                        transmission.cessione = new DocsPaVO.documento.CessioneDocumento();
                        transmission.cessione.idPeople = infoUtente.idPeople;
                        transmission.cessione.idRuolo = infoUtente.idGruppo;
                        transmission.cessione.docCeduto = true;
                        transmission.cessione.userId = infoUtente.userId;
                        if (utentePropCessione != null && ruoloPropCessione != null)
                        {
                            transmission.cessione.idPeopleNewPropr = utentePropCessione.idPeople;
                            transmission.cessione.idRuoloNewPropr = ruoloPropCessione.idGruppo;
                        }
                        if (ragione.cessioneImpostata && !string.IsNullOrEmpty(ragione.mantieniScrittura) && ragione.mantieniScrittura == "1")
                        {
                            transmission.mantieniScrittura = true;
                        }
                        // End MEV Cessione Diritti

                        if (ragione.cessioneImpostata && !string.IsNullOrEmpty(ragione.mantieniLettura) && ragione.mantieniLettura == "1")
                        {
                            transmission.mantieniLettura = true;
                        }

                    }
                    transmission = BusinessLogic.Trasmissioni.TrasmManager.addTrasmissioneSingola(
                        transmission, corr, ragione, "", trasmType);
                    // il tipo trasm è S
                    // Gestione notifiche in caso cessione proprietario
                    if (ragione != null && !string.IsNullOrEmpty(ragione.prevedeCessione) && ragione.prevedeCessione != "N" && utentePropCessione != null && ruoloPropCessione != null)
                    {
                        foreach (DocsPaVO.trasmissione.TrasmissioneSingola trasmS in transmission.trasmissioniSingole)
                        {
                            foreach (DocsPaVO.trasmissione.TrasmissioneUtente trasmU in trasmS.trasmissioneUtente)
                            {
                                if (trasmU.utente.idPeople == utentePropCessione.idPeople)
                                    trasmU.daNotificare = true;
                                else
                                    trasmU.daNotificare = false;
                            }
                        }

                    }

                    DocsPaVO.trasmissione.Trasmissione result = BusinessLogic.Trasmissioni.ExecTrasmManager.saveExecuteTrasmMethod(urlfrontend, transmission);
                    response.Code = TransmissionResponseCode.OK;
                    string desc = string.Empty;
                    // LOG per documento
                    if (result.infoDocumento != null && !string.IsNullOrEmpty(result.infoDocumento.idProfile))
                    {
                        foreach (DocsPaVO.trasmissione.TrasmissioneSingola single in result.trasmissioniSingole)
                        {
                            string method = "TRASM_DOC_" + single.ragione.descrizione.ToUpper().Replace(" ", "_");
                            if (result.infoDocumento.segnatura == null)
                                desc = "Trasmesso Documento : " + result.infoDocumento.docNumber.ToString();
                            else
                                desc = "Trasmesso Documento : " + result.infoDocumento.segnatura.ToString();

                            BusinessLogic.UserLog.UserLog.WriteLog(result.utente.userId, result.utente.idPeople, result.ruolo.idGruppo, result.utente.idAmministrazione, method, result.infoDocumento.docNumber, desc, DocsPaVO.Logger.CodAzione.Esito.OK, infoUtente.delegato, generaNotifica, single.systemId);
                        }
                    }
                    // LOG per fascicolo
                    if (result.infoFascicolo != null && !string.IsNullOrEmpty(result.infoFascicolo.idFascicolo))
                    {
                        foreach (DocsPaVO.trasmissione.TrasmissioneSingola single in result.trasmissioniSingole)
                        {
                            string method = "TRASM_FOLDER_" + single.ragione.descrizione.ToUpper().Replace(" ", "_");
                            desc = "Trasmesso Fascicolo ID: " + result.infoFascicolo.idFascicolo.ToString();
                            BusinessLogic.UserLog.UserLog.WriteLog(result.utente.userId, result.utente.idPeople, result.ruolo.idGruppo, result.utente.idAmministrazione, method, result.infoFascicolo.idFascicolo, desc, DocsPaVO.Logger.CodAzione.Esito.OK, infoUtente.delegato, generaNotifica, single.systemId);
                        }
                    }


                }
                catch (Exception ex)
                {
                    response.ErrorMessage = ex.Message;
                    response.Code = TransmissionResponseCode.SYSTEM_ERROR;
                }

                response.TransmMessage = "Trasmissione effettuata";

                logger.Info("end executeTransmissionDocument");
            }
            catch (RestException pisEx)
            {
                logger.ErrorFormat("Eccezione executeTransmissionDocument: {0}, {1}", pisEx.ErrorCode, pisEx.Description);
                response = new TransmissionResponse();
                response.Code = TransmissionResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                logger.Error("eccezione executeTransmissionDocument: " + e);
                response = new TransmissionResponse();
                response.Code = TransmissionResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }
            return response;
        }

        public static async Task<TransmissionResponse> executeTransmissionProject(string token, ExecuteTransmissionProjectRequest request)
        {
            logger.Info("begin executeTransmissionProject");
            TransmissionResponse response = new TransmissionResponse();
            try
            {
                #region controllo Token
                if (string.IsNullOrWhiteSpace(token))
                {
                    logger.Error("Missing AuthToken Header, manca il token di autenticazione");
                    throw new Exception("Missing AuthToken Header");
                }
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();

                string authInfoString = Utils.Decrypt(token.Substring(4));
                string[] authInfoArray = authInfoString.Split('|');

                utente = BusinessLogic.Utenti.UserManager.getUtente(authInfoArray[5], authInfoArray[4]);
                ruolo = BusinessLogic.Utenti.UserManager.getRuoloById(authInfoArray[0]);
                infoUtente = new DocsPaVO.utente.InfoUtente(utente, ruolo);
                infoUtente.codWorkingApplication = authInfoArray[8]; infoUtente.dst = authInfoArray[3];

                #endregion

                if (string.IsNullOrEmpty(request.IdProject))
                {
                    throw new RestException("REQUIRED_ID");
                }


                if (request.Receiver == null)
                {
                    //Destinatario non trovato
                    throw new RestException("REQUIRED_CORRESPONDENT");
                }
                if (string.IsNullOrEmpty(request.Receiver.Code) && string.IsNullOrEmpty(request.Receiver.Id))
                {
                    throw new RestException("REQUIRED_CODE_OR_ID_RECEIVER");
                }
                DocsPaVO.utente.Corrispondente corr = null;
                try
                {
                    if (!string.IsNullOrEmpty(request.Receiver.Id))
                    {
                        corr = BusinessLogic.Utenti.UserManager.getCorrispondenteBySystemID(request.Receiver.Id);
                    }
                    else if (!string.IsNullOrEmpty(request.Receiver.Code))
                    {
                        corr = BusinessLogic.Utenti.UserManager.getCorrispondenteByCodRubrica(request.Receiver.Code, infoUtente);
                    }
                    if (corr == null)
                    {
                        throw new RestException("CORRESPONDENT_NOT_FOUND");
                    }

                }
                catch (Exception e)
                {
                    throw new RestException("CORRESPONDENT_NOT_FOUND");
                }

                // La ragione di trasmissione
                DocsPaVO.trasmissione.RagioneTrasmissione ragione = null;
                if (request.TransmissionReason == null || string.IsNullOrEmpty(request.TransmissionReason))
                {
                    throw new RestException("REQUIRED_TRANSMISSION_REASON");
                }
                else if (infoUtente.diSistema == "1" && request.TransmissionReason != "COMPETENZA_SIST_ESTERNI")
                {
                    throw new Exception("Ragione di trasmissione non valida per un sistema esterno. Ragione disponibile: COMPENTENZA_SIST_ESTERNI");
                }
                try
                {
                    ragione = BusinessLogic.Trasmissioni.RagioniManager.getRagioneByCodice(infoUtente.idAmministrazione, request.TransmissionReason.ToUpper());
                    if (ragione == null)
                        throw new RestException("TRANSMISSION_REASON_NOT_FOUND");
                }
                catch (Exception e)
                {
                    throw new RestException("TRANSMISSION_REASON_NOT_FOUND");
                }

                DocsPaVO.fascicolazione.Fascicolo fascicolo = null;

                if (!string.IsNullOrEmpty(request.IdProject))
                {
                    fascicolo = BusinessLogic.Fascicoli.FascicoloManager.getFascicoloById(request.IdProject, infoUtente);
                }
                else
                {
                    throw new RestException("REQUIRED_ID_PROJECT");
                }

                if (fascicolo == null)
                {
                    throw new RestException("PROJECT_NOT_FOUND");

                }
                string trasmType = "S";
                if (!string.IsNullOrEmpty(request.TransmissionType) && request.TransmissionType == "T")
                {
                    trasmType = "T";
                }
                #region Cessione diritti
                DocsPaVO.utente.Utente utentePropCessione = null;
                DocsPaVO.utente.InfoUtente infoUtentePropCessione = null;
                DocsPaVO.utente.Ruolo ruoloPropCessione = null;
                if (ragione != null && !string.IsNullOrEmpty(ragione.prevedeCessione) && ragione.prevedeCessione != "N")
                {
                    ragione.cessioneImpostata = true;
                    // proprietario del documento
                    if (fascicolo != null && fascicolo.accessRights == "255")
                    {

                        if (corr.tipoCorrispondente != "R")
                        {
                            throw new Exception("Per la cessione del diritto di proprietà, il destinatario deve essere un ruolo");
                        }
                        if (request == null || request.Receiver == null || string.IsNullOrEmpty(request.Receiver.Note))
                        {

                            //throw new Exception("Per la cessione del diritto di proprietà, è necessario indicare lo userid del nuovo proprietario nel campo Receiver.Note");
                            // Selezione del primo utente nel ruolo
                            ruoloPropCessione = BusinessLogic.Utenti.UserManager.getRuoloByCodice(corr.codiceRubrica);
                            if (ruoloPropCessione != null)
                            {
                                List<DocsPaVO.utente.UserMinimalInfo> userList = BusinessLogic.Utenti.UserManager.GetUsersInRoleMinimalInfo(ruoloPropCessione.idGruppo);
                                if (userList != null && userList.Count > 0)
                                {
                                    utentePropCessione = BusinessLogic.Utenti.UserManager.getUtenteById(userList[0].SystemId);
                                    infoUtentePropCessione = new DocsPaVO.utente.InfoUtente(utentePropCessione, ruoloPropCessione);
                                }
                            }
                        }
                        else
                        {
                            utentePropCessione = BusinessLogic.Utenti.UserManager.getUtente(request.Receiver.Note, infoUtente.idAmministrazione);
                            if (utentePropCessione != null)
                            {
                                DocsPaVO.utente.InfoUtente infoUtenteDaCercare = new DocsPaVO.utente.InfoUtente(utentePropCessione, Utils.GetRuoloPreferito(utentePropCessione.idPeople));
                                ArrayList arrayRuoli = BusinessLogic.Utenti.UserManager.getRuoliUtente(infoUtenteDaCercare.idPeople);
                                bool ruoloPresente = false;
                                if (arrayRuoli != null && arrayRuoli.Count > 0)
                                {
                                    foreach (DocsPaVO.utente.Ruolo rol in arrayRuoli)
                                    {
                                        if (rol.codiceRubrica == corr.codiceRubrica)
                                        {
                                            ruoloPresente = true;
                                            ruoloPropCessione = rol;
                                            break;
                                        }
                                    }
                                }
                                if (!ruoloPresente)
                                {
                                    throw new Exception("L'utente non è presente nel ruolo");
                                }

                            }
                            else
                            {
                                throw new RestException("USER_NO_EXIST");
                            }
                        }
                    }
                    else
                    {
                        if (corr.tipoCorrispondente != "R")
                        {
                            throw new Exception("Per la cessione del diritto, il destinatario deve essere un ruolo");
                        }
                        if (ragione.prevedeCessione == "W")
                        {
                            if (!(request != null && request.Receiver != null && !string.IsNullOrEmpty(request.Receiver.Note)
                                && request.Receiver.Note.ToUpper() == "CEDI"))
                                ragione.cessioneImpostata = false;
                        }
                    }
                }
                #endregion


                try
                {
                    //creazione oggetto trasmissione e popolamento campi.
                    DocsPaVO.trasmissione.Trasmissione transmission = new DocsPaVO.trasmissione.Trasmissione();
                    // Creazione dell'oggetto trasmissione
                    transmission = new DocsPaVO.trasmissione.Trasmissione();

                    // Impostazione dei parametri della trasmissione
                    // Note generali
                    // transmission.noteGenerali = ragione.note;

                    // Tipo oggetto
                    transmission.tipoOggetto = DocsPaVO.trasmissione.TipoOggetto.FASCICOLO;

                    // Informazioni sul documento
                    transmission.infoFascicolo = new DocsPaVO.fascicolazione.InfoFascicolo(fascicolo);
                    // Utente mittente della trasmissione
                    transmission.utente = BusinessLogic.Utenti.UserManager.getUtente(infoUtente.idPeople);

                    // Ruolo mittente
                    // Se inserito in ingresso il CodeRole utilizzo il coderole (WIP)
                    transmission.ruolo = BusinessLogic.Utenti.UserManager.getRuolo(infoUtente.idCorrGlobali);
                    string generaNotifica = "1";
                    if (request.Notify != null && !request.Notify)
                    {
                        transmission.NO_NOTIFY = "1";
                        generaNotifica = "0";
                    }
                    else
                    {
                        transmission.NO_NOTIFY = "0";
                        generaNotifica = "1";
                    }
                    // Modifica per invio in mail dell'url del frontend
                    string urlfrontend = "";
                    if (System.Configuration.ConfigurationManager.AppSettings["URL_PATH_IS"] != null)
                        urlfrontend = System.Configuration.ConfigurationManager.AppSettings["URL_PATH_IS"].ToString();
                    // Cessione diritti
                    if (ragione != null && !string.IsNullOrEmpty(ragione.prevedeCessione) && ragione.prevedeCessione != "N" && ragione.cessioneImpostata)
                    {
                        transmission.cessione = new DocsPaVO.documento.CessioneDocumento();
                        
                        transmission.cessione.idPeople = infoUtente.idPeople;
                        transmission.cessione.idRuolo = infoUtente.idGruppo;
                        transmission.cessione.docCeduto = true;
                        transmission.cessione.userId = infoUtente.userId;
                        if (utentePropCessione != null && ruoloPropCessione != null)
                        {
                            transmission.cessione.idPeopleNewPropr = utentePropCessione.idPeople;
                            transmission.cessione.idRuoloNewPropr = ruoloPropCessione.idGruppo;
                        }
                        if (ragione.cessioneImpostata && !string.IsNullOrEmpty(ragione.mantieniScrittura) && ragione.mantieniScrittura == "1")
                        {
                            transmission.mantieniScrittura = true;
                        }
                        // End MEV Cessione Diritti

                        if (ragione.cessioneImpostata && !string.IsNullOrEmpty(ragione.mantieniLettura) && ragione.mantieniLettura == "1")
                        {
                            transmission.mantieniLettura = true;
                        }

                    }
                    transmission = BusinessLogic.Trasmissioni.TrasmManager.addTrasmissioneSingola(
                        transmission, corr, ragione, "", trasmType);
                    // il tipo trasm è S
                    // Gestione notifiche in caso cessione proprietario
                    if (ragione != null && !string.IsNullOrEmpty(ragione.prevedeCessione) && ragione.prevedeCessione != "N" && utentePropCessione != null && ruoloPropCessione != null)
                    {
                        foreach (DocsPaVO.trasmissione.TrasmissioneSingola trasmS in transmission.trasmissioniSingole)
                        {
                            foreach (DocsPaVO.trasmissione.TrasmissioneUtente trasmU in trasmS.trasmissioneUtente)
                            {
                                if (trasmU.utente.idPeople == utentePropCessione.idPeople)
                                    trasmU.daNotificare = true;
                                else
                                    trasmU.daNotificare = false;
                            }
                        }

                    }
                    DocsPaVO.trasmissione.Trasmissione result = BusinessLogic.Trasmissioni.ExecTrasmManager.saveExecuteTrasmMethod(urlfrontend, transmission);
                    response.Code = TransmissionResponseCode.OK;

                    //Log per Centro notifiche
                    string desc = string.Empty;
                    if (result != null)
                    {
                        // LOG per documento
                        if (result.infoDocumento != null && !string.IsNullOrEmpty(result.infoDocumento.idProfile))
                        {
                            foreach (DocsPaVO.trasmissione.TrasmissioneSingola single in result.trasmissioniSingole)
                            {
                                string method = "TRASM_DOC_" + single.ragione.descrizione.ToUpper().Replace(" ", "_");
                                if (result.infoDocumento.segnatura == null)
                                    desc = "Trasmesso Documento : " + result.infoDocumento.docNumber.ToString();
                                else
                                    desc = "Trasmesso Documento : " + result.infoDocumento.segnatura.ToString();

                                BusinessLogic.UserLog.UserLog.WriteLog(result.utente.userId, result.utente.idPeople, result.ruolo.idGruppo, result.utente.idAmministrazione, method, result.infoDocumento.docNumber, desc, DocsPaVO.Logger.CodAzione.Esito.OK, infoUtente.delegato, generaNotifica, single.systemId);
                            }
                        }
                        // LOG per fascicolo
                        if (result.infoFascicolo != null && !string.IsNullOrEmpty(result.infoFascicolo.idFascicolo))
                        {
                            foreach (DocsPaVO.trasmissione.TrasmissioneSingola single in result.trasmissioniSingole)
                            {
                                string method = "TRASM_FOLDER_" + single.ragione.descrizione.ToUpper().Replace(" ", "_");
                                desc = "Trasmesso Fascicolo ID: " + result.infoFascicolo.idFascicolo.ToString();
                                BusinessLogic.UserLog.UserLog.WriteLog(result.utente.userId, result.utente.idPeople, result.ruolo.idGruppo, result.utente.idAmministrazione, method, result.infoFascicolo.idFascicolo, desc, DocsPaVO.Logger.CodAzione.Esito.OK, infoUtente.delegato, generaNotifica, single.systemId);
                            }
                        }

                    }




                }
                catch (Exception ex)
                {
                    response.ErrorMessage = ex.Message;
                    response.Code = TransmissionResponseCode.SYSTEM_ERROR;
                }
                response.TransmMessage = "Trasmissione effettuata";

                logger.Info("end executeTransmissionProject");
            }
            catch (RestException pisEx)
            {
                logger.ErrorFormat("Eccezione executeTransmissionProject: {0}, {1}", pisEx.ErrorCode, pisEx.Description);
                response = new TransmissionResponse();
                response.Code = TransmissionResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                logger.Error("eccezione executeTransmissionProject: " + e);
                response = new TransmissionResponse();
                response.Code = TransmissionResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }
            return response;
        }

        public static async Task<GetTransmModelResponse> getTransmissionModel(string token, string idModel, string codeModel)
        {
            logger.Info("begin getTransmissionModel");
            GetTransmModelResponse response = new GetTransmModelResponse();
            try
            {
                #region controllo Token
                if (string.IsNullOrWhiteSpace(token))
                {
                    logger.Error("Missing AuthToken Header, manca il token di autenticazione");
                    throw new Exception("Missing AuthToken Header");
                }
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();

                string authInfoString = Utils.Decrypt(token.Substring(4));
                string[] authInfoArray = authInfoString.Split('|');

                utente = BusinessLogic.Utenti.UserManager.getUtente(authInfoArray[5], authInfoArray[4]);
                ruolo = BusinessLogic.Utenti.UserManager.getRuoloById(authInfoArray[0]);
                infoUtente = new DocsPaVO.utente.InfoUtente(utente, ruolo);
                infoUtente.codWorkingApplication = authInfoArray[8]; infoUtente.dst = authInfoArray[3];

                #endregion
                DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione modello = null;

                TransmissionModel trasmResponse = null;

                if (string.IsNullOrEmpty(idModel) && string.IsNullOrEmpty(codeModel))
                {
                    //Inserire un valore
                    throw new RestException("REQUIRED_TM_ID_OR_CODE");
                }

                if (!string.IsNullOrEmpty(idModel) && !string.IsNullOrEmpty(codeModel))
                {
                    //Inserire solo un valore
                    throw new RestException("REQUIRED_ONLY_TM_ID_OR_CODE");
                }

                ArrayList modelli = new ArrayList();
                ArrayList modelliFasc = new ArrayList();
                DocsPaVO.utente.Registro[] registri = null;
                string idModello = string.Empty;

                ArrayList registriArr = BusinessLogic.Utenti.RegistriManager.getListaRegistriRfRuolo(ruolo.systemId, "0", string.Empty);
                if (registriArr != null && registriArr.Count > 0)
                {
                    int i = 0;
                    registri = new DocsPaVO.utente.Registro[registriArr.Count];
                    foreach (DocsPaVO.utente.Registro reg in registriArr)
                    {
                        registri[i] = reg;
                        i++;
                    }
                }

                if (!string.IsNullOrEmpty(idModel))
                {

                    modelli = BusinessLogic.Trasmissioni.ModelliTrasmissioni.getModelliPerTrasmLite(infoUtente.idAmministrazione, registri, infoUtente.idPeople, infoUtente.idCorrGlobali, string.Empty, string.Empty, string.Empty, "D", string.Empty, infoUtente.idGruppo, false, string.Empty);
                    modelliFasc = BusinessLogic.Trasmissioni.ModelliTrasmissioni.getModelliPerTrasmLite(infoUtente.idAmministrazione, registri, infoUtente.idPeople, infoUtente.idCorrGlobali, string.Empty, string.Empty, string.Empty, "F", string.Empty, infoUtente.idGruppo, false, string.Empty);
                    foreach (DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione mod in modelliFasc)
                    {
                        modelli.Add(mod);
                    }

                    idModello = idModel;
                    foreach (DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione mod in modelli)
                    {
                        if (mod.SYSTEM_ID.ToString().Equals(idModello))
                        {
                            modello = BusinessLogic.Trasmissioni.ModelliTrasmissioni.getModelloByID(infoUtente.idAmministrazione, idModel);
                            break;
                        }
                    }


                }
                else
                {
                    //Tolgo MT_ e ho l'id
                    modelli = BusinessLogic.Trasmissioni.ModelliTrasmissioni.getModelliPerTrasmLite(infoUtente.idAmministrazione, registri, infoUtente.idPeople, infoUtente.idCorrGlobali, string.Empty, string.Empty, string.Empty, "D", string.Empty, infoUtente.idGruppo, false, string.Empty);
                    modelliFasc = BusinessLogic.Trasmissioni.ModelliTrasmissioni.getModelliPerTrasmLite(infoUtente.idAmministrazione, registri, infoUtente.idPeople, infoUtente.idCorrGlobali, string.Empty, string.Empty, string.Empty, "F", string.Empty, infoUtente.idGruppo, false, string.Empty);
                    foreach (DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione mod in modelliFasc)
                    {
                        modelli.Add(mod);
                    }

                    idModello = codeModel.Substring(3);
                    foreach (DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione mod in modelli)
                    {
                        if (mod.SYSTEM_ID.ToString().Equals(idModello))
                        {
                            modello = BusinessLogic.Trasmissioni.ModelliTrasmissioni.getModelloByID(infoUtente.idAmministrazione, idModello);
                            break;
                        }
                    }
                }

                if (modello != null)
                {
                    trasmResponse = Utils.GetModel(modello);
                    response.TransmissionModel = trasmResponse;
                    response.Code = GetTransmModelResponseCode.OK;
                }
                else
                {
                    //modello di trasmissione non trovato
                    throw new RestException("TRANSMISSION_MODEL_NOT_FOUND");
                }



                logger.Info("end getTransmissionModel");
            }
            catch (RestException pisEx)
            {
                logger.ErrorFormat("Eccezione getTransmissionModel: {0}, {1}", pisEx.ErrorCode, pisEx.Description);
                response = new GetTransmModelResponse();
                response.Code = GetTransmModelResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                logger.Error("eccezione getTransmissionModel: " + e);
                response = new GetTransmModelResponse();
                response.Code = GetTransmModelResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }
            return response;
        }

        public static async Task<GetTransmModelsResponse> getTransmissionModels(string token, GetTransmModelsRequest request)
        {
            logger.Info("begin getTransmissionModels");
            GetTransmModelsResponse response = new GetTransmModelsResponse();
            try
            {
                #region controllo Token
                if (string.IsNullOrWhiteSpace(token))
                {
                    logger.Error("Missing AuthToken Header, manca il token di autenticazione");
                    throw new Exception("Missing AuthToken Header");
                }
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();

                string authInfoString = Utils.Decrypt(token.Substring(4));
                string[] authInfoArray = authInfoString.Split('|');

                utente = BusinessLogic.Utenti.UserManager.getUtente(authInfoArray[5], authInfoArray[4]);
                ruolo = BusinessLogic.Utenti.UserManager.getRuoloById(authInfoArray[0]);
                infoUtente = new DocsPaVO.utente.InfoUtente(utente, ruolo);
                infoUtente.codWorkingApplication = authInfoArray[8]; infoUtente.dst = authInfoArray[3];

                #endregion
                string tipo = string.Empty;
                DocsPaVO.utente.Registro[] registri = null;
                ArrayList modelli = new ArrayList();
                TransmissionModel[] responseModels = null;

                if (!string.IsNullOrEmpty(request.Type) && (request.Type.ToUpper().Equals("D") || request.Type.ToUpper().Equals("F")))
                {
                    tipo = request.Type;
                }
                else
                {
                    //Tipo non trovato
                    throw new RestException("TRANSMISSION_MODEL_TYPE");
                }

                if (request.Registers != null && request.Registers.Length > 0)
                {
                    int y = 0;
                    registri = new DocsPaVO.utente.Registro[request.Registers.Length];
                    foreach (Register regTemp in request.Registers)
                    {
                        DocsPaVO.utente.Registro reg = BusinessLogic.Utenti.RegistriManager.getRegistroByCode(regTemp.Code);
                        if (reg != null)
                        {
                            registri[y] = reg;
                            y++;
                        }
                        else
                        {
                            //Registro mancante
                            throw new RestException("REGISTER_NOT_FOUND");
                        }
                    }
                }
                else
                {
                    //Registro mancante
                    throw new RestException("REQUIRED_REGISTER");
                }

                modelli = BusinessLogic.Trasmissioni.ModelliTrasmissioni.getModelliPerTrasmLite(infoUtente.idAmministrazione, registri, infoUtente.idPeople, infoUtente.idCorrGlobali, string.Empty, string.Empty, string.Empty, tipo, string.Empty, infoUtente.idGruppo, false, string.Empty);

                if (modelli != null && modelli.Count > 0)
                {
                    int y = 0;
                    responseModels = new TransmissionModel[modelli.Count];
                    foreach (DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione mod in modelli)
                    {
                        responseModels[y] = Utils.GetModel(mod);
                        y++;
                    }
                    response.TransmissionModels = responseModels;
                    response.Code = GetTransmModelsResponseCode.OK;
                }
                else
                {
                    //Modelli di trasmisssione non trovati
                    throw new RestException("TRANSMISSION_MODELS_NOT_FOUND");
                }



                logger.Info("end getTransmissionModels");
            }
            catch (RestException pisEx)
            {
                logger.ErrorFormat("Eccezione getTransmissionModels: {0}, {1}", pisEx.ErrorCode, pisEx.Description);
                response = new GetTransmModelsResponse();
                response.Code = GetTransmModelsResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                logger.Error("eccezione getTransmissionModels: " + e);
                response = new GetTransmModelsResponse();
                response.Code = GetTransmModelsResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }
            return response;
        }

        public static async Task<MessageResponse> giveUpRights(string token, string rightToKeep, string idObject)
        {
            logger.Info("begin giveUpRights");
            MessageResponse response = new MessageResponse();
            try
            {
                #region controllo Token
                if (string.IsNullOrWhiteSpace(token))
                {
                    logger.Error("Missing AuthToken Header, manca il token di autenticazione");
                    throw new Exception("Missing AuthToken Header");
                }
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();

                string authInfoString = Utils.Decrypt(token.Substring(4));
                string[] authInfoArray = authInfoString.Split('|');

                utente = BusinessLogic.Utenti.UserManager.getUtente(authInfoArray[5], authInfoArray[4]);
                ruolo = BusinessLogic.Utenti.UserManager.getRuoloById(authInfoArray[0]);
                infoUtente = new DocsPaVO.utente.InfoUtente(utente, ruolo);
                infoUtente.codWorkingApplication = authInfoArray[8]; infoUtente.dst = authInfoArray[3];

                #endregion
                int idOggetto;

                if (string.IsNullOrEmpty(rightToKeep))
                {
                    throw new RestException("MISSING_PARAMETER");
                }
                else if (rightToKeep.ToUpper() != "WRITE" && rightToKeep.ToUpper() != "READ" && rightToKeep.ToUpper() != "NONE")
                {
                    throw new Exception("Wrong value for parameter RightToKeep. Accepted values: WRITE, READ, NONE.");
                }
                if (string.IsNullOrEmpty(idObject))
                {
                    throw new RestException("MISSING_PARAMETER");
                }
                else if (!Int32.TryParse(idObject, out idOggetto))
                {
                    throw new Exception("IdObject must be an integer");
                }
                DocsPaVO.amministrazione.SistemaEsterno sysExt = BusinessLogic.Amministrazione.SistemiEsterni.getSistemaEsternoByUserid(infoUtente.idAmministrazione, infoUtente.userId);
                if (sysExt == null)
                {
                    throw new Exception("Metodo non disponibile per l'utente: l'utente non è un sistema esterno");
                }
                bool trovato = false;
                string accessRightsUtente, idGruppoTrasmUtente, TipoDirittoUtente;
                string accessRightsRuolo = string.Empty, idGruppoTrasmRuolo = string.Empty, TipoDirittoRuolo = string.Empty;
                trovato = BusinessLogic.Trasmissioni.ExecTrasmManager.SelectSecurity(out accessRightsUtente, out idGruppoTrasmUtente, out TipoDirittoUtente, idObject, utente.idPeople, null);
                if (trovato && !string.IsNullOrEmpty(accessRightsUtente) && !string.IsNullOrEmpty(TipoDirittoUtente))
                    trovato = BusinessLogic.Trasmissioni.ExecTrasmManager.SelectSecurity(out accessRightsRuolo, out idGruppoTrasmRuolo, out TipoDirittoRuolo, idObject, ruolo.idGruppo, null);
                if (trovato && !string.IsNullOrEmpty(accessRightsUtente) && !string.IsNullOrEmpty(TipoDirittoUtente) && !string.IsNullOrEmpty(accessRightsRuolo) && !string.IsNullOrEmpty(TipoDirittoRuolo))
                {
                    if (accessRightsUtente == "0" || TipoDirittoUtente == "P" || accessRightsRuolo == "255" || TipoDirittoRuolo == "P")
                    {
                        throw new Exception("Errore: proprietà non ceduta. Impossibile cedere i diritti");
                    }
                    string messDiritti = "";
                    int ARToMantain = 0;
                    switch (rightToKeep.ToUpper())
                    {
                        case "WRITE":
                            ARToMantain = 63;
                            messDiritti = "scrittura";
                            break;
                        case "READ":
                            ARToMantain = 45;
                            messDiritti = "lettura";
                            break;
                        case "NONE":
                            ARToMantain = 0;
                            break;
                    }
                    if (ARToMantain > Int32.Parse(accessRightsUtente) || ARToMantain > Int32.Parse(accessRightsRuolo))
                    {
                        throw new Exception("Errore: diritti da mantenere superiori a quelli effettivamente posseduti");
                    }
                    else if (ARToMantain == Int32.Parse(accessRightsUtente) || ARToMantain == Int32.Parse(accessRightsRuolo))
                    {
                        response.ResultMessage = string.Format("Nessuna modifica effettuata. Mantenuti i diritti di {0} in precedenza posseduti", messDiritti);
                        response.Code = MessageResponseCode.OK;
                    }
                    else
                    {
                        if (ARToMantain > 0)
                        {
                            trovato = BusinessLogic.Amministrazione.SistemiEsterni.AggiornaDirittiSistemaEsterno(idObject, ruolo.idGruppo, utente.idPeople, TipoDirittoUtente, TipoDirittoUtente, accessRightsUtente, "45", ARToMantain.ToString());
                            if (trovato)
                            {
                                response.ResultMessage = string.Format("Modifica dei diritti avvenuta con successo: ora il sistema esterno {0} ha diritti di {1} sull'oggetto di id {2}", sysExt.CodiceApplicazione, messDiritti, idObject);
                                response.Code = MessageResponseCode.OK;
                            }
                            else
                            {
                                throw new Exception("Errore durante la modifica dei diritti");
                            }
                        }
                        else
                        {
                            trovato = BusinessLogic.Amministrazione.SistemiEsterni.CessioneTotaleDirittiSysExt(idObject, utente.idPeople, ruolo.idGruppo, accessRightsUtente);
                            if (trovato)
                            {
                                response.ResultMessage = string.Format("Modifica dei diritti avvenuta con successo: ora il sistema esterno {0} non ha diritti sull'oggetto di id {1}", sysExt.CodiceApplicazione, idObject);
                                response.Code = MessageResponseCode.OK;
                            }
                            else
                            {
                                throw new Exception("Errore durante la modifica dei diritti");
                            }
                        }
                    }

                }
                else
                {
                    throw new Exception("Il sistema esterno non ha alcun diritto sull'oggetto corrispondente all'id inserito " + idObject);
                }

                logger.Info("end giveUpRights");
            }
            catch (RestException pisEx)
            {
                logger.ErrorFormat("Eccezione giveUpRights: {0}, {1}", pisEx.ErrorCode, pisEx.Description);
                response = new MessageResponse();
                response.Code = MessageResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                logger.Error("eccezione giveUpRights: " + e);
                response = new MessageResponse();
                response.Code = MessageResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }
            return response;
        }
    }
}