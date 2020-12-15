using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using log4net;
using System.Threading.Tasks;
using RESTServices.Model.Domain;
using RESTServices.Model.Responses;
using RESTServices.Model.Requests;

namespace RESTServices.Manager
{
    public class LibroFirmaManager
    {
        private static ILog logger = LogManager.GetLogger(typeof(LibroFirmaManager));

        public static async Task<GetFiltersResponse> getInstanceSearchFilters(string token)
        {
            logger.Info("begin getInstanceSearchFilters");
            GetFiltersResponse response = new GetFiltersResponse();
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
                List<Filter> listaFiltri = new List<Filter>();

                listaFiltri.Add(new Filter() { Name = "PROCESS_ID", Description = "ID del processo associato all'istanza di firma", Type = FilterTypeEnum.Number });
                listaFiltri.Add(new Filter() { Name = "DOC_NUMBER", Description = "ID del documento associato all'istanza di firma", Type = FilterTypeEnum.Number });
                listaFiltri.Add(new Filter() { Name = "NOTE", Description = "Stringa contenuta nelle note dell'istanza", Type = FilterTypeEnum.String });
                listaFiltri.Add(new Filter() { Name = "START_DATE", Description = "Data di avvio dell'istanza di firma", Type = FilterTypeEnum.Date });
                listaFiltri.Add(new Filter() { Name = "START_DATE_FROM", Description = "Data di avvio dell'istanza di firma successiva a", Type = FilterTypeEnum.Date });
                listaFiltri.Add(new Filter() { Name = "START_DATE_TO", Description = "Data di avvio dell'istanza di firma precedende il", Type = FilterTypeEnum.Date });
                listaFiltri.Add(new Filter() { Name = "START_NOTES", Description = "Stringa contenuta nelle note di avvio dell'istanza", Type = FilterTypeEnum.String });
                listaFiltri.Add(new Filter() { Name = "END_DATE", Description = "Data di conclusione dell'istanza di firma", Type = FilterTypeEnum.Date });
                listaFiltri.Add(new Filter() { Name = "END_DATE_FROM", Description = "Data di conclusione dell'istanza di firma successiva a", Type = FilterTypeEnum.Date });
                listaFiltri.Add(new Filter() { Name = "END_DATE_TO", Description = "Data di conclusione dell'istanza di firma precedende il", Type = FilterTypeEnum.Date });
                listaFiltri.Add(new Filter() { Name = "INTERRUPTION_DATE", Description = "Data di interruzione dell'istanza di firma", Type = FilterTypeEnum.Date });
                listaFiltri.Add(new Filter() { Name = "INTERRUPTION_DATE_FROM", Description = "Data di interruzione dell'istanza di firma successiva a", Type = FilterTypeEnum.Date });
                listaFiltri.Add(new Filter() { Name = "INTERRUPTION_DATE_TO", Description = "Data di interruzione dell'istanza di firma precedende il", Type = FilterTypeEnum.Date });
                listaFiltri.Add(new Filter() { Name = "REFUSAL_NOTE", Description = "Stringa contenuta nelle note di respingimento dell'istanza", Type = FilterTypeEnum.String });
                listaFiltri.Add(new Filter() { Name = "IN_EXECUTION", Description = "Processo di firma in esecuzione", Type = FilterTypeEnum.Bool });
                listaFiltri.Add(new Filter() { Name = "INTERRUPTED", Description = "Processo di firma interrotto", Type = FilterTypeEnum.Bool });
                listaFiltri.Add(new Filter() { Name = "ENDED", Description = "Processo di firma concluso", Type = FilterTypeEnum.Bool });
                listaFiltri.Add(new Filter() { Name = "TRUNCATED", Description = "Processo di firma troncato", Type = FilterTypeEnum.Bool });

                response.Filters = listaFiltri;

                logger.Info("end getInstanceSearchFilters");
            }
            catch (RestException pisEx)
            {
                logger.ErrorFormat("Eccezione getInstanceSearchFilters: {0}, {1}", pisEx.ErrorCode, pisEx.Description);
                response = new GetFiltersResponse();
                response.Code = GetFiltersResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                logger.Error("eccezione getInstanceSearchFilters: " + e);
                response = new GetFiltersResponse();
                response.Code = GetFiltersResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }
            return response;
        }

        public static async Task<GetSignatureProcessesResponse> getSignatureProcesses(string token)
        {
            logger.Info("begin GetSignatureProcesses");
            GetSignatureProcessesResponse response = new GetSignatureProcessesResponse();
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

                List<DocsPaVO.LibroFirma.ProcessoFirma> listaProc = BusinessLogic.LibroFirma.LibroFirmaManager.GetProcessesSignatureVisibleRole(infoUtente);
                if (listaProc != null && listaProc.Count > 0)
                {
                    response.Processes = new RESTServices.Model.Domain.SignBook.SignatureProcess[listaProc.Count];
                    RESTServices.Model.Domain.SignBook.SignatureProcess spD = null;
                    int i = 0;
                    foreach (DocsPaVO.LibroFirma.ProcessoFirma proc in listaProc)
                    {
                        spD = new RESTServices.Model.Domain.SignBook.SignatureProcess(proc);
                        response.Processes[i] = spD;
                        i++;
                    }

                    response.TotalProcessesNumber = listaProc.Count();
                }
                response.Code = GetSignatureProcessesResponseCode.OK;
                logger.Info("end GetSignatureProcesses");
            }
            catch (RestException pisEx)
            {
                logger.ErrorFormat("Eccezione GetSignatureProcesses: {0}, {1}", pisEx.ErrorCode, pisEx.Description);
                response = new GetSignatureProcessesResponse();
                response.Code = GetSignatureProcessesResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                logger.Error("eccezione GetSignatureProcesses: " + e);
                response = new GetSignatureProcessesResponse();
                response.Code = GetSignatureProcessesResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }
            return response;
        }

        public static async Task<GetSignatureProcessResponse> getSignatureProcess(string token, string idProcess)
        {
            logger.Info("begin getSignatureProcess");
            GetSignatureProcessResponse response = new GetSignatureProcessResponse();
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

                DocsPaVO.LibroFirma.ProcessoFirma proc = BusinessLogic.LibroFirma.LibroFirmaManager.GetProcessoDiFirmaById(idProcess, infoUtente);
                if (proc != null)
                {
                    response.SignatureProcess = new RESTServices.Model.Domain.SignBook.SignatureProcess(proc);
                }
                else { throw new Exception("Signature Process not found"); }

                response.Code = GetSignatureProcessResponseCode.OK;
                logger.Info("end getSignatureProcess");
            }
            catch (RestException pisEx)
            {
                logger.ErrorFormat("Eccezione GetSignatureProcesses: {0}, {1}", pisEx.ErrorCode, pisEx.Description);
                response = new GetSignatureProcessResponse();
                response.Code = GetSignatureProcessResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                logger.Error("eccezione GetSignatureProcesses: " + e);
                response = new GetSignatureProcessResponse();
                response.Code = GetSignatureProcessResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }
            return response;
        }

        public static async Task<GetSignProcessInstanceResponse> getSignProcessInstance(string token, string idProcessInstance)
        {
            logger.Info("begin getSignProcessInstance");
            GetSignProcessInstanceResponse response = new GetSignProcessInstanceResponse();
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

                DocsPaVO.LibroFirma.IstanzaProcessoDiFirma istpdf = BusinessLogic.LibroFirma.LibroFirmaManager.GetIstanzaProcessoDiFirmaByIdIstanzaProcesso(idProcessInstance, infoUtente);
                if (istpdf != null)
                {
                    response.ProcessInstance = new RESTServices.Model.Domain.SignBook.SignatureProcessInstance(istpdf);
                }
                else { throw new Exception("Signature Process Instance not found"); }
                
                response.Code = GetSignProcessInstanceResponseCode.OK;
                logger.Info("end getSignProcessInstance");
            }
            catch (RestException pisEx)
            {
                logger.ErrorFormat("Eccezione GetSignProcessInstancees: {0}, {1}", pisEx.ErrorCode, pisEx.Description);
                response = new GetSignProcessInstanceResponse();
                response.Code = GetSignProcessInstanceResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                logger.Error("eccezione GetSignProcessInstancees: " + e);
                response = new GetSignProcessInstanceResponse();
                response.Code = GetSignProcessInstanceResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }
            return response;
        }

        public static async Task<GetSignProcessInstancesResponse> searchSignProcessInstances(string token, SearchRequest request)
        {
            logger.Info("begin searchSignProcessInstances");
            GetSignProcessInstancesResponse response = new GetSignProcessInstancesResponse();
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

                List<DocsPaVO.LibroFirma.FiltroIstanzeProcessoFirma> filtri = new List<DocsPaVO.LibroFirma.FiltroIstanzeProcessoFirma>();
                DocsPaVO.LibroFirma.FiltroIstanzeProcessoFirma fTemp;
                List<DocsPaVO.LibroFirma.IstanzaProcessoDiFirma> istanze = new List<DocsPaVO.LibroFirma.IstanzaProcessoDiFirma>();
                int totnumpage = 0;
                if (request.Filters != null && request.Filters.Length > 0)
                {
                    foreach (Filter fPis in request.Filters)
                    {
                        fTemp = new DocsPaVO.LibroFirma.FiltroIstanzeProcessoFirma();
                        switch (fPis.Name)
                        {
                            case "PROCESS_ID":
                                fTemp.Argomento = "ID_PROCESSO";
                                fTemp.Valore = fPis.Value;
                                break;
                            case "DOC_NUMBER":
                                fTemp.Argomento = "DOCNUMBER";
                                fTemp.Valore = fPis.Value;
                                break;
                            case "NOTE":
                                fTemp.Argomento = "NOTE";
                                fTemp.Valore = fPis.Value;
                                break;
                            case "START_DATE":
                                fTemp.Argomento = "DATA_AVVIO_IL";
                                fTemp.Valore = fPis.Value;
                                break;
                            case "START_DATE_FROM":
                                fTemp.Argomento = "DATA_AVVIO_SUCCESSIVA_AL";
                                fTemp.Valore = fPis.Value;
                                break;
                            case "START_DATE_TO":
                                fTemp.Argomento = "DATA_AVVIO_PRECEDENTE_IL";
                                fTemp.Valore = fPis.Value;
                                break;
                            case "START_NOTES":
                                fTemp.Argomento = "NOTE_AVVIO";
                                fTemp.Valore = fPis.Value;
                                break;
                            case "END_DATE":
                                fTemp.Argomento = "DATA_CONCLUSIONE_IL";
                                fTemp.Valore = fPis.Value;
                                break;
                            case "END_DATE_FROM":
                                fTemp.Argomento = "DATA_CONCLUSIONE_SUCCESSIVA_AL";
                                fTemp.Valore = fPis.Value;
                                break;
                            case "END_DATE_TO":
                                fTemp.Argomento = "DATA_CONCLUSIONE_PRECEDENTE_IL";
                                fTemp.Valore = fPis.Value;
                                break;
                            case "INTERRUPTION_DATE":
                                fTemp.Argomento = "DATA_INTERRUZIONE_IL";
                                fTemp.Valore = fPis.Value;
                                break;
                            case "INTERRUPTION_DATE_FROM":
                                fTemp.Argomento = "DATA_INTERRUZIONE_SUCCESSIVA_AL";
                                fTemp.Valore = fPis.Value;
                                break;
                            case "INTERRUPTION_DATE_TO":
                                fTemp.Argomento = "DATA_INTERRUZIONE_PRECEDENTE_IL";
                                fTemp.Valore = fPis.Value;
                                break;
                            case "REFUSAL_NOTE":
                                fTemp.Argomento = "NOTE_RESPINGIMENTO";
                                fTemp.Valore = fPis.Value;
                                break;
                            case "IN_EXECUTION":
                                fTemp.Argomento = "STATO_IN_ESECUZIONE";
                                fTemp.Valore = fPis.Value;
                                break;
                            case "INTERRUPTED":
                                fTemp.Argomento = "STATO_INTERROTTO";
                                fTemp.Valore = fPis.Value;
                                break;
                            case "ENDED":
                                fTemp.Argomento = "STATO_CONCLUSO";
                                fTemp.Valore = fPis.Value;
                                break;
                            case "TRUNCATED":
                                fTemp.Argomento = "TRONCATO";
                                fTemp.Valore = fPis.Value;
                                break;

                        }
                        filtri.Add(fTemp);
                    }
                    int numpage = 1, numInPage = 20, nrec;
                    if (request.PageNumber != null && request.PageNumber > 1)
                        numpage = request.PageNumber;

                    if (request.ElementsInPage != null && request.ElementsInPage > 0) numInPage = request.ElementsInPage;


                    

                    //List<DocsPaVO.LibroFirma.IstanzaProcessoDiFirma> istanze = GetIstanzaProcessiDiFirmaByFilter(List<DocsPaVO.LibroFirma.FiltroIstanzeProcessoFirma> filtro, int numPage, int pageSize, out int numTotPage, out int nRec, DocsPaVO.utente.InfoUtente infoUtente);

                    istanze = BusinessLogic.LibroFirma.LibroFirmaManager.GetIstanzaProcessiDiFirmaByFilter(filtri, numpage, numInPage, out totnumpage, out nrec, infoUtente);

                }
                if (istanze != null && istanze.Count > 0)
                {
                    response.SignatureProcessInstances = new RESTServices.Model.Domain.SignBook.SignatureProcessInstance[istanze.Count];
                    int indice = 0;
                    response.TotalNumber = istanze.Count;

                    foreach (DocsPaVO.LibroFirma.IstanzaProcessoDiFirma iX in istanze)
                    {
                        response.SignatureProcessInstances[indice] = new RESTServices.Model.Domain.SignBook.SignatureProcessInstance(iX);
                        indice++;
                    }
                }
                response.Code = GetSignProcessInstancesResponseCode.OK;

                logger.Info("end searchSignProcessInstances");
            }
            catch (RestException pisEx)
            {
                logger.ErrorFormat("Eccezione searchSignProcessInstances: {0}, {1}", pisEx.ErrorCode, pisEx.Description);
                response = new GetSignProcessInstancesResponse();
                response.Code = GetSignProcessInstancesResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                logger.Error("eccezione searchSignProcessInstances: " + e);
                response = new GetSignProcessInstancesResponse();
                response.Code = GetSignProcessInstancesResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }
            return response;
        }

        public static async Task<MessageResponse> startSignatureProcess(string token, StartSignatureProcessRequest request)
        {
            logger.Info("begin startSignatureProcess");
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
                DocsPaVO.LibroFirma.ProcessoFirma procFirma;
                DocsPaVO.documento.FileRequest frequest;
                bool retBool = false;
                string idDocPrincipale = "", modalita = "A";
                DocsPaVO.documento.SchedaDocumento documento = new DocsPaVO.documento.SchedaDocumento();

                if (!string.IsNullOrEmpty(request.IdDocument))
                {
                    documento = BusinessLogic.Documenti.DocManager.getDettaglio(infoUtente, request.IdDocument, request.IdDocument);
                    if (documento != null)
                    {
                        frequest = (DocsPaVO.documento.FileRequest)documento.documenti[0];
                    }
                    else
                    {
                        throw new RestException("DOCUMENT_NOT_FOUND");
                    }
                }
                else
                {
                    throw new RestException("REQUIRED_ID");
                }
                if (string.IsNullOrEmpty(request.Note))
                {
                    throw new RestException("MISSING_PARAMETER");
                }
                if (string.IsNullOrEmpty(idDocPrincipale)) { }

                DocsPaVO.LibroFirma.ResultProcessoFirma resultAvvio;

                if (request.SignatureProcess != null)
                {
                    procFirma = Utils.GetProcessoFirmaFromDomain(request.SignatureProcess);
                }
                else
                {
                    throw new RestException("MISSING_PARAMETER");
                }

                retBool = BusinessLogic.LibroFirma.LibroFirmaManager.StartProcessoDiFirma(procFirma,
                    frequest,
                    infoUtente,
                    "A", request.Note, request.InterruptionGeneratesNote, request.EndGeneratesNote,
                    out resultAvvio);

                if (retBool)
                {
                    string method = "AVVIATO_PROCESSO_DI_FIRMA_DOCUMENTO";
                    string description = "Avviato processo di firma per la versione " + frequest.version;
                    response.ResultMessage = description;
                    if (frequest.GetType().Equals(typeof(DocsPaVO.documento.Allegato)))
                    {
                        method = "AVVIATO_PROCESSO_DI_FIRMA_ALLEGATO";
                    }

                    BusinessLogic.UserLog.UserLog.WriteLog(infoUtente.userId, infoUtente.idPeople, infoUtente.idGruppo, infoUtente.idAmministrazione, method, frequest.docNumber,
                        description, DocsPaVO.Logger.CodAzione.Esito.OK, (infoUtente != null && infoUtente.delegato != null ? infoUtente.delegato : null), "1");
                }

                switch (resultAvvio)
                {
                    case DocsPaVO.LibroFirma.ResultProcessoFirma.DOCUMENTO_BLOCCATO:
                        throw new Exception("DOCUMENTO BLOCCATO");
                        break;
                    case DocsPaVO.LibroFirma.ResultProcessoFirma.DOCUMENTO_CONSOLIDATO:
                        throw new Exception("DOCUMENTO CONSOLIDATO");
                        break;
                    case DocsPaVO.LibroFirma.ResultProcessoFirma.DOCUMENTO_GIA_IN_LIBRO_FIRMA:
                        throw new Exception("DOCUMENTO GIA IN LIBRO FIRMA");
                        break;
                    case DocsPaVO.LibroFirma.ResultProcessoFirma.EXISTING_PROCESS_NAME:
                        throw new Exception("ERRORE GENERICO");
                        break;
                    case DocsPaVO.LibroFirma.ResultProcessoFirma.FILE_NON_AMMESSO_ALLA_FIRMA:
                        throw new Exception("FILE NON AMMESSO ALLA FIRMA");
                        break;
                    case DocsPaVO.LibroFirma.ResultProcessoFirma.KO:
                        throw new Exception("ERRORE GENERICO");
                        break;
                }
                if (!retBool) throw new Exception("ERRORE GENERICO");
                response.Code = MessageResponseCode.OK;

                logger.Info("end startSignatureProcess");
            }
            catch (RestException pisEx)
            {
                logger.ErrorFormat("Eccezione startSignatureProcess: {0}, {1}", pisEx.ErrorCode, pisEx.Description);
                response = new MessageResponse();
                response.Code = MessageResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                logger.Error("eccezione startSignatureProcess: " + e);
                response = new MessageResponse();
                response.Code = MessageResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }
            return response;
        }

        public static async Task<MessageResponse> interruptSignatureProcess(string token, InterruptSignatureProcessRequest request)
        {
            logger.Info("begin interruptSignatureProcess");
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

                ruolo = BusinessLogic.Utenti.UserManager.getRuoloByIdGruppo(infoUtente.idGruppo);
                DocsPaVO.LibroFirma.IstanzaProcessoDiFirma istpdf = BusinessLogic.LibroFirma.LibroFirmaManager.GetIstanzaProcessoDiFirmaByIdIstanzaProcesso(request.IdSignProcessInstance, infoUtente);
                bool result = BusinessLogic.LibroFirma.LibroFirmaManager.InterruptionSignatureProcessByProponent(istpdf, request.InterruptionNote, ruolo, infoUtente);
                string idDocPrincipale = "";

                //Se è andato a buon fine ed è un allegato
                if (result && (istpdf.docAll.Equals("A")))
                {
                    DocsPaVO.documento.Allegato allX = new DocsPaVO.documento.Allegato();
                    allX.docNumber = istpdf.docNumber;
                    idDocPrincipale = BusinessLogic.Documenti.AllegatiManager.getIdDocumentoPrincipale(allX);
                    BusinessLogic.LibroFirma.LibroFirmaManager.StopPassoWait(idDocPrincipale, infoUtente);
                }
                response.ResultMessage = "Processo di firma interrotto.";
                response.Code = MessageResponseCode.OK;
                logger.Info("end interruptSignatureProcess");
            }
            catch (RestException pisEx)
            {
                logger.ErrorFormat("Eccezione interruptSignatureProcess: {0}, {1}", pisEx.ErrorCode, pisEx.Description);
                response = new MessageResponse();
                response.Code = MessageResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                logger.Error("eccezione interruptSignatureProcess: " + e);
                response = new MessageResponse();
                response.Code = MessageResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }
            return response;
        }

    }
}