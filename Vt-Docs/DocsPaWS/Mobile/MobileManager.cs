using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DocsPaVO.Mobile.Responses;
using DocsPaVO.Mobile.Requests;
using DocsPaVO.utente;
using DocsPaVO.documento;
using DocsPaVO.Mobile;
using DocsPaVO.filtri;
using DocsPaVO.filtri.trasmissione;
using DocsPaVO.fascicolazione;
using DocsPaVO.trasmissione;
using DocsPaDB.Query_DocsPAWS;
using DocsPaVO.Deleghe;
using DocsPaVO.amministrazione;
using DocsPaWS.Mobile.PdfToImage;
using DocsPaVO.ricerche;
using DocsPaVO.Modelli_Trasmissioni;
using DocsPaVO.addressbook;
using DocsPaWS.Mobile.Builders;
using DocsPaVO.Note;
using DocsPaWS.Mobile.Decorators;
using DocsPaWS.Mobile.Ricerca;
using log4net;
using DocsPaVO.Smistamento;
using DocsPaVO.Grids;
using System.Globalization;

namespace DocsPaWS.Mobile
{
    public class MobileManager
    {
        private static ILog logger = LogManager.GetLogger(typeof(MobileManager));

        private static Dictionary<TipoDelega, string> _tipoDelegaMap;
        private static string getTipoDelega(TipoDelega input)
        {
            if (_tipoDelegaMap == null)
            {
                _tipoDelegaMap = new Dictionary<TipoDelega, string>();
                _tipoDelegaMap.Add(TipoDelega.ASSEGNATA, "assegnate");
                _tipoDelegaMap.Add(TipoDelega.ESERCIZIO, "esercizio");
                _tipoDelegaMap.Add(TipoDelega.RICEVUTA, "ricevute");
                _tipoDelegaMap.Add(TipoDelega.TUTTI, "tutte");
            }
            return _tipoDelegaMap[input];
        }

        private static Dictionary<StatoDelega, string> _statoDelegaMap;
        private static string getStatoDelega(StatoDelega input)
        {
            if (_statoDelegaMap == null)
            {
                _statoDelegaMap = new Dictionary<StatoDelega, string>();
                _statoDelegaMap.Add(StatoDelega.ATTIVA, "A");
                _statoDelegaMap.Add(StatoDelega.IMPOSTATA, "I");
                _statoDelegaMap.Add(StatoDelega.SCADUTA, "S");
                _statoDelegaMap.Add(StatoDelega.TUTTI, "T");
            }
            return _statoDelegaMap[input];
        }

        public static LoginResponse login(LoginRequest loginRequest)
        {
            logger.Info("begin");
            DocsPaVO.utente.UserLogin.LoginResult loginResult = DocsPaVO.utente.UserLogin.LoginResult.OK;
            string ipAddress = "";
            try
            {
                logger.Debug("ricerca user con userName " + loginRequest.UserName);
                Utente utente = BusinessLogic.Utenti.Login.loginMethod(loginRequest.UserLogin, out loginResult, true,
                        null, out ipAddress);
                logger.Debug("user found");
                return new LoginResponse(utente, loginResult);
            }
            catch (Exception ex)
            {
                logger.Error("eccezione: " + ex);
                return LoginResponse.ErrorResponse;
            }
        }

        public static LogoutResponse logout(LogoutRequest request)
        {
            logger.Info("BEGIN");
            try
            {
                UserInfo ui = request.UserInfo;
                BusinessLogic.Utenti.Login.logoff(ui.UserId, ui.IdAmministrazione, ui.Dst);
                LogoutResponse res = new LogoutResponse(LogoutResponseCode.OK);
                logger.Info("END");
                return res;
            }
            catch (Exception e)
            {
                logger.Info("END");
                return LogoutResponse.ErrorResponse;
            }
        }

        public static ToDoListResponse getTodoList(ToDoListRequest request)
        {
            try
            {
                BusinessLogic.Trasmissioni.TrasmManager trasmManager = new BusinessLogic.Trasmissioni.TrasmManager();
                int totalRecordCount, totalTrasmNonViste;
                ToDoListResponse res = new ToDoListResponse();
                res.Elements = new List<ToDoListElement>();
                InfoUtente infoUtente = request.UserInfo.InfoUtente;
                infoUtente.idGruppo = request.IdGruppo;
                infoUtente.idCorrGlobali = request.IdCorrGlobali;
                if (string.IsNullOrEmpty(request.ParentFolderId))
                {
                    FiltroRicerca[] filtri = new FiltroRicerca[3];
                    filtri[0] = new FiltroRicerca();
                    filtri[0].argomento = listaArgomentiNascosti.TODO_LIST.ToString();
                    filtri[1] = new FiltroRicerca();
                    filtri[1].argomento = listaArgomentiNascosti.NO_CERCA_INFERIORI.ToString();
                    filtri[2] = new FiltroRicerca();
                    filtri[2].argomento = listaArgomentiNascosti.ELEMENTI_NON_VISTI.ToString();
                    filtri[2].valore = "0";
                    IList temp = trasmManager.getMyNewTodoList(infoUtente, request.Registri, filtri, request.RequestedPage, request.PageSize, out totalRecordCount, out totalTrasmNonViste);
                    foreach (Object obj in temp)
                    {
                        infoToDoList tdl = (infoToDoList)obj;
                        //DocsPaVO.trasmissione.Trasmissione trasm = BusinessLogic.Trasmissioni.TrasmManager.CreateObjTrasmissioneByID(tdl.sysIdTrasm);
                        //res.Elements.Add(ToDoListElement.buildInstance(tdl, trasm, request.UserInfo));
                        res.Elements.Add(ToDoListElement.buildInstance(tdl, null, request.UserInfo));
                    }
                    res.TotalRecordCount = totalRecordCount;
                }
                else
                {
                    logger.Debug("contenuto del folder con id " + request.ParentFolderId);
                    FascicoloContentDecorator<ToDoListElement> fcDecorator = new FascicoloContentDecorator<ToDoListElement>(request.UserInfo.IdPeople, request.IdGruppo, request.ParentFolderId, request.FascId, ToDoListElement.buildInstance, ToDoListElement.buildInstance);
                    PaginatorDecorator<ToDoListElement> pag = new PaginatorDecorator<ToDoListElement>(request.RequestedPage, request.PageSize, fcDecorator);
                    NoteDecorator note = new NoteDecorator(infoUtente, pag);
                    res.Elements = note.execute();
                    res.TotalRecordCount = pag.TotalResultCount;
                }
                logger.Info("end");
                return res;
            }
            catch (Exception e)
            {
                logger.Error("eccezione: " + e);
                return ToDoListResponse.ErrorResponse;
            }
        }

        public static ToDoListResponse getNotify(ToDoListRequest request)
        {
            try
            {
                BusinessLogic.Trasmissioni.TrasmManager trasmManager = new BusinessLogic.Trasmissioni.TrasmManager();
                int totalRecordCount;
                ToDoListResponse res = new ToDoListResponse();
                res.Elements = new List<ToDoListElement>();
                InfoUtente infoUtente = request.UserInfo.InfoUtente;
                infoUtente.idGruppo = request.IdGruppo;
                infoUtente.idCorrGlobali = request.IdCorrGlobali;
                if (string.IsNullOrEmpty(request.ParentFolderId))
                {
                    List<DocsPaVO.Notification.Notification> listNotificatione = BusinessLogic.ServiceNotifications.Notification.ReadNotificationsMobile(infoUtente.idPeople, infoUtente.idGruppo, request.RequestedPage, request.PageSize, out totalRecordCount);
                    foreach (DocsPaVO.Notification.Notification notification in listNotificatione)
                    {
                        string idTrasmissione = string.Empty;
                        if (!string.IsNullOrEmpty(notification.ID_SPECIALIZED_OBJECT))
                            idTrasmissione = BusinessLogic.Trasmissioni.TrasmManager.GetIdTrasmissioneByIdTrasmSingola(notification.ID_SPECIALIZED_OBJECT);
                        res.Elements.Add(ToDoListElement.buildInstance(notification, idTrasmissione));
                    }
                    res.TotalRecordCount = totalRecordCount;
                }
                else
                {
                    logger.Debug("contenuto del folder con id " + request.ParentFolderId);
                    FascicoloContentDecorator<ToDoListElement> fcDecorator = new FascicoloContentDecorator<ToDoListElement>(request.UserInfo.IdPeople, request.IdGruppo, request.ParentFolderId, request.FascId, ToDoListElement.buildInstance, ToDoListElement.buildInstance);
                    PaginatorDecorator<ToDoListElement> pag = new PaginatorDecorator<ToDoListElement>(request.RequestedPage, request.PageSize, fcDecorator);
                    NoteDecorator note = new NoteDecorator(infoUtente, pag);
                    res.Elements = note.execute();
                    res.TotalRecordCount = pag.TotalResultCount;
                }
                logger.Info("end");
                return res;
            }
            catch (Exception e)
            {
                logger.Error("eccezione: " + e);
                return ToDoListResponse.ErrorResponse;
            }
        }

        #region CheckPage

        public static bool checkConnection(out string message)
        {
            bool result = true; // Presume successo
            message = null;

            try
            {
                result = BusinessLogic.Amministrazione.UtenteManager.Checkconnection();
            }
            catch (Exception ex)
            {
                message = ex.Message + " " + ex.StackTrace;
                result = false;
            }

            return result;
        }

        #endregion

        #region Documento

        public static bool rimuoviNotifica(string idEvento)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.NotificationDB notification = new NotificationDB();
                return notification.CheckNotification(idEvento);
            }
            catch (Exception e)
            {
                logger.Error("eccezione: " + e);
                return false;
            }
        }

        public static GetDocInfoResponse getDocInfo(GetDocInfoRequest request)
        {
            try
            {
                logger.Info("begin");
                GetDocInfoResponse resp = new GetDocInfoResponse();
                InfoUtente infoUtente = request.UserInfo.InfoUtente;
                infoUtente.idGruppo = request.IdGruppo;
                infoUtente.idCorrGlobali = request.IdCorrGlobali;
                if (!string.IsNullOrEmpty(request.IdTrasm))
                {
                    DocsPaVO.trasmissione.Trasmissione trasm = BusinessLogic.Trasmissioni.TrasmManager.CreateObjTrasmissioneByID(request.IdTrasm);
                    logger.Debug("data invio: " + trasm.dataInvio);
                    Utente delegato=null;
                    // Gabriele Melini 17-02-2014
                    //if (trasm.delegato != null)
                    if(!string.IsNullOrEmpty(trasm.delegato))
                    {
                        try
                        {
                            delegato = BusinessLogic.Utenti.UserManager.getUtenteById(trasm.delegato);
                        }
                        catch (Exception e1)
                        {
                            delegato = null;
                        }
                    }
                    resp.TrasmInfo = TrasmInfo.buildInstance(trasm, request.UserInfo,delegato);
                }
                List<BaseInfoDoc> infos = BusinessLogic.Documenti.DocManager.GetBaseInfoForDocument(request.IdDoc, null, null);
                
                if (infos.Count>0)
                    resp.Allegati = new List<DocInfo>();

                foreach (BaseInfoDoc info in infos)
                        resp.Allegati.Add(generateDocInfo(resp, infoUtente, info));

                resp.DocInfo = resp.Allegati.FirstOrDefault();
                if (string.IsNullOrEmpty(request.IdTrasm) && !string.IsNullOrEmpty(request.IdEvento) && Cfg_SET_DATA_VISTA_GRD != "2")
                {
                    DocsPaDB.Query_DocsPAWS.NotificationDB notification = new NotificationDB();
                    notification.CheckNotification(request.IdEvento);
                }
                logger.Info("end ");
                return resp;
            }
            catch (Exception e)
            {
                logger.Error("eccezione: " + e);
                return GetDocInfoResponse.ErrorResponse;
            }

        }

        private static string Cfg_SET_DATA_VISTA_GRD
        {
            get
            {
                string eme = System.Configuration.ConfigurationManager.AppSettings["SET_DATA_VISTA_GRD"];
                return (eme != null) ? eme : "0";
            }
        }


        private static DocInfo generateDocInfo(GetDocInfoResponse resp, InfoUtente infoUtente, BaseInfoDoc doc)
        {
            SchedaDocumento schedaDoc = BusinessLogic.Mobile.DocManager.getDettaglioMobile(infoUtente, doc.IdProfile, doc.DocNumber);
            IEnumerable<Fascicolo> fascicoli = BusinessLogic.Fascicoli.FascicoloManager.getFascicoliDaDoc(infoUtente, doc.IdProfile).Cast<Fascicolo>();
            //logger.Debug(String.Format("SLOG Data1 [{0}]  Data2[{1}]  TipoProto[{2}]  daproto[{3}], isegnatura[{4}] ", schedaDoc.dataCreazione.ToString(), schedaDoc.protocollo.dataProtocollazione.ToString(), schedaDoc.tipoProto, schedaDoc.protocollo.daProtocollare.ToString(), schedaDoc.protocollo.segnatura));
            return DocInfo.buildInstance(schedaDoc, doc.OriginalFileName , fascicoli, doc.HaveFile, doc.HaveFile && doc.FileName.ToUpper().Contains(".PDF"));
        }

        public static GetFileResponse getFile(GetFileRequest request)
        {
            try
            {
                logger.Info("begin");
                GetFileResponse resp = new GetFileResponse();
                InfoUtente infoUtente = request.UserInfo.InfoUtente;
                infoUtente.idGruppo = request.IdGruppo;
                infoUtente.idCorrGlobali = request.IdCorrGlobali;
                /*
                List<BaseInfoDoc> infos = BusinessLogic.Documenti.DocManager.GetBaseInfoForDocument(request.IdDoc, null, null);
                BaseInfoDoc doc = infos[0];
                FileRequest req = new FileRequest();
                req.versionId = doc.VersionId;
                req.docNumber = doc.IdProfile;
                req.path = doc.Path;
                req.fileName = doc.FileName;
                */


                List<BaseInfoDoc> infos = BusinessLogic.Documenti.DocManager.GetBaseInfoForDocument(request.IdDoc, null, null);
                BaseInfoDoc doc = infos[0];
                bool sbusta = true;
                if (doc.FileName.ToUpper().EndsWith(".P7M") ||
                    doc.FileName.ToUpper().EndsWith(".TSD"))
                {
                    sbusta = false;
                }

                DocsPaVO.documento.SchedaDocumento schedaDocumento = BusinessLogic.Documenti.DocManager.getDettaglioNoSecurity(infoUtente, request.IdDoc);
                DocsPaVO.documento.FileRequest fileRequest = (DocsPaVO.documento.FileRequest)schedaDocumento.documenti[0];

                //BusinessLogic.Documenti.DocManager.getDettaglio(infoUtente, request.IdDoc,null);
                DocsPaVO.documento.FileDocumento fileDocumento=null;
                if (sbusta)
                     fileDocumento = BusinessLogic.Documenti.FileManager.getFile(fileRequest, request.UserInfo.InfoUtente);
                else
                    fileDocumento = BusinessLogic.Documenti.FileManager.getFileFirmato (fileRequest, request.UserInfo.InfoUtente,false);

                resp.File = DocsPaVO.Mobile.FileInfo.buildInstance(fileDocumento);
                logger.Info("end");
                return resp;
            }
            catch (Exception e)
            {
                logger.Error("eccezione: " + e);
                return GetFileResponse.ErrorResponse;
            }
        }

        public static GetPreviewResponse getPreview(GetPreviewRequest request)
        {
            GetPreviewResponse resp = new GetPreviewResponse();
            try
            {
                logger.Debug("begin");
                InfoUtente infoUtente = request.UserInfo.InfoUtente;
                infoUtente.idGruppo = request.IdGruppo;
                infoUtente.idCorrGlobali = request.IdCorrGlobali;
                /*
                List<BaseInfoDoc> infos = BusinessLogic.Documenti.DocManager.GetBaseInfoForDocument(request.IdDoc, null, null);
                BaseInfoDoc doc = infos[0];
                FileRequest req = new FileRequest();
                req.versionId = doc.VersionId;
                req.docNumber = doc.IdProfile;
                req.path = doc.Path;
                req.fileName = doc.FileName;
                 * */
                DocsPaVO.documento.SchedaDocumento schedaDocumento;
                DocsPaVO.documento.FileRequest fileRequest;
                DocsPaVO.documento.FileDocumento fileDocumento;

/*
                schedaDocumento = BusinessLogic.Documenti.DocManager.getDettaglioNoSecurity(infoUtente, request.IdDoc);
                fileRequest = (DocsPaVO.documento.FileRequest)schedaDocumento.documenti[0];
                fileDocumento = BusinessLogic.Documenti.FileManager.getFile(fileRequest, request.UserInfo.InfoUtente);
                if (!fileDocumento.name.ToUpper().EndsWith("PDF"))
                {
                    resp.Code = GetPreviewResponseCode.NO_PREVIEW_FOR_EXTENSION;
                }
                else
                {
                    PDFConvertWrapper pdfConvWrapper = PDFConvertWrapper.GetInstance();
                    FileInfo fileInfo = new FileInfo();
                    fileInfo.Content = pdfConvWrapper.Convert(fileDocumento.content, request.RequestedPage, request.DimX, request.DimY);
                    if (fileInfo.Content == null)
                    {
                        resp.Code = GetPreviewResponseCode.SYSTEM_ERROR;
                    }
                    else
                    {
                        resp.Code = GetPreviewResponseCode.OK;
                        fileInfo.ContentType = "image/jpeg";
                        fileInfo.EstensioneFile = "jpg";
                        resp.File = fileInfo;
                    }
                }
*/


                List<BaseInfoDoc> infos = BusinessLogic.Documenti.DocManager.GetBaseInfoForDocument(request.IdDoc, null, null);
                string filename =infos[0].FileName.ToUpper();
                if (!infos[0].HaveFile)
                {
                    resp.Code = GetPreviewResponseCode.PAGE_NOT_FOUND;
                }
                else if (!infos[0].FileName.ToUpper().Contains(".PDF"))
                {
                    resp.Code = GetPreviewResponseCode.NO_PREVIEW_FOR_EXTENSION;
                }
                else {
                    schedaDocumento = BusinessLogic.Documenti.DocManager.getDettaglioNoSecurity(infoUtente, request.IdDoc);
                    fileRequest = (DocsPaVO.documento.FileRequest)schedaDocumento.documenti[0];
                    fileDocumento = BusinessLogic.Documenti.FileManager.getFile(fileRequest, request.UserInfo.InfoUtente);

                    PDFConvertWrapper pdfConvWrapper = PDFConvertWrapper.GetInstance();
                    FileInfo fileInfo = new FileInfo();
                    fileInfo.Content = pdfConvWrapper.Convert(fileDocumento.content, request.RequestedPage, request.DimX, request.DimY);
                    if (fileInfo.Content == null)
                    {
                        resp.Code = GetPreviewResponseCode.SYSTEM_ERROR;
                    }
                    else
                    {
                        resp.Code = GetPreviewResponseCode.OK;
                        fileInfo.ContentType = "image/jpeg";
                        fileInfo.EstensioneFile = "jpg";
                        resp.File = fileInfo;
                    }
                }



                logger.Info("end");
                return resp;
            }
            catch (PDFConvertException pe)
            {
                if (pe.Code == PDFConvertExceptionCode.PAGE_NOT_FOUND)
                {
                    resp.Code = GetPreviewResponseCode.PAGE_NOT_FOUND;
                    return resp;
                }
                else
                {
                    return GetPreviewResponse.ErrorResponse;
                }
            }
            catch (Exception e)
            {
                logger.Error("eccezione: " + e);
                return GetPreviewResponse.ErrorResponse;
            }

        }

        public static ADLActionResponse ADLAction(ADLActionRequest request)
        {
            logger.Debug("begin");
            ADLActionResponse response = new ADLActionResponse { Code = AddToADLResponseCode.SYSTEM_ERROR };
            string idProfile = request.DocInfo.IdDoc;
            string tipoProto = request.DocInfo.TipoProto;
            string idRegistro = request.IdCorrGlobali;
            Fascicolo fasc = null;
            InfoUtente infoUtente = request.UserInfo.InfoUtente;
            infoUtente.idCorrGlobali = request.IdCorrGlobali;
            infoUtente.idGruppo = request.IdGruppo;
            if (tipoProto.Equals ("fasc"))
            {   // si tratta di un fascicolo
                fasc = BusinessLogic.Fascicoli.FascicoloManager.getFascicoloById(request.DocInfo.IdDoc, infoUtente);
                idProfile = null;
                tipoProto = null;
            }

            try
            {
                if (request.AdlAction == ADLActionRequest.ADLActions.ADD)
                    BusinessLogic.Documenti.areaLavoroManager.execAddLavoroMethod(idProfile, tipoProto, idRegistro, infoUtente, fasc);
                else
                    BusinessLogic.Documenti.areaLavoroManager.cancellaAreaLavoro(request.UserInfo.IdPeople,idRegistro, idProfile,  fasc);

                response.Code = AddToADLResponseCode.OK;
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Errore in addToAdl, {0} {1}", e.Message, e.StackTrace);
                response.Code = AddToADLResponseCode.SYSTEM_ERROR;
            }
            return response;
        }

        #endregion
        #region Fascicolo
        public static GetFascInfoResponse getFascInfo(GetFascInfoRequest request)
        {
            try
            {
                logger.Info("begin");
                GetFascInfoResponse resp = new GetFascInfoResponse();
                InfoUtente infoUtente = request.UserInfo.InfoUtente;
                infoUtente.idGruppo = request.IdGruppo;
                infoUtente.idCorrGlobali = request.IdCorrGlobali;
                if (!string.IsNullOrEmpty(request.IdTrasm))
                {
                    DocsPaVO.trasmissione.Trasmissione trasm = BusinessLogic.Trasmissioni.TrasmManager.CreateObjTrasmissioneByID(request.IdTrasm);
                    Utente delegato = null;
                    if (trasm.delegato != null)
                    {
                        try
                        {
                            delegato = BusinessLogic.Utenti.UserManager.getUtenteById(trasm.delegato);
                        }
                        catch (Exception e1)
                        {
                            delegato = null;
                        }
                    }
                    resp.TrasmInfo = TrasmInfo.buildInstance(trasm, request.UserInfo,delegato);
                }
                Fascicolo fasc = BusinessLogic.Fascicoli.FascicoloManager.getFascicoloById(request.IdFasc, infoUtente);
                Fascicoli fascicoli = new Fascicoli();
                fascicoli.SetDataVistaSP(infoUtente, request.IdFasc, "F");
                resp.FascInfo = FascInfo.buildInstance(fasc);
                resp.Code = GetFascInfoResponseCode.OK;
                logger.Info("end");
                return resp;
            }
            catch (Exception e)
            {
                logger.Error("eccezione: " + e);
                return GetFascInfoResponse.ErrorResponse;
            }
        }
        #endregion
        #region Ricerca
        public static RicercaResponse ricerca(RicercaRequest request)
        {
            try
            {
                logger.Info("begin");
                RicercaResponse resp = new RicercaResponse();
                RicercaStrategy.GetStrategy(request).buildResponse(request, resp);
                resp.Code = RicercaResponseCode.OK;
                logger.Info("end");
                return resp;
            }
            catch (Exception e)
            {
                logger.Error("eccezione: " + e);
                return RicercaResponse.ErrorResponse;
            }
        }

        public static GetRicSalvateResponse getRicercheSalvate(GetRicSalvateRequest request)
        {
            try
            {
                logger.Info("begin");
                GetRicSalvateResponse resp = new GetRicSalvateResponse();
                resp.Risultati = new List<RicercaSalvata>();
                SearchItem[] ricSalvDoc = BusinessLogic.Documenti.InfoDocManager.GetSearchList(Int32.Parse(request.UserInfo.IdPeople), Int32.Parse(request.IdGruppo), null, false, "D");
                foreach (SearchItem temp in ricSalvDoc)
                {
                    resp.Risultati.Add(new RicercaSalvata("" + temp.system_id, temp.descrizione, RicercaSalvataType.RIC_DOCUMENTO));
                }
                SearchItem[] ricSalvFasc = BusinessLogic.Documenti.InfoDocManager.GetSearchList(Int32.Parse(request.UserInfo.IdPeople), Int32.Parse(request.IdGruppo), null, false, "F");
                foreach (SearchItem temp in ricSalvFasc)
                {
                    resp.Risultati.Add(new RicercaSalvata("" + temp.system_id, temp.descrizione, RicercaSalvataType.RIC_FASCICOLO));
                }
                resp.Code = GetRicSalvateResponseCode.OK;
                logger.Info("end");
                return resp;
            }
            catch (Exception e)
            {
                logger.Error("eccezione: " + e);
                return GetRicSalvateResponse.ErrorResponse;
            }
        }
        #endregion
        #region Deleghe
        public static DelegheResponse getListaDeleghe(DelegheRequest request)
        {
            logger.Debug("Start");
            DelegheResponse resp = new DelegheResponse();
            try
            {
                int numDeleghe = 0;
                string tipoDelega = getTipoDelega(request.TipoDelega);
                string statoDelega = getStatoDelega(request.StatoDelega);
                SearchDelegaInfo sdInfo = new SearchDelegaInfo();
                sdInfo.StatoDelega = statoDelega;
                sdInfo.TipoDelega = tipoDelega;
                SearchPagingContext pagingContext = new SearchPagingContext();
                pagingContext.Page = 1;
                pagingContext.PageSize = 1000;
                List<InfoDelega> deleghe = BusinessLogic.Deleghe.DelegheManager.searchDeleghe(request.UserInfo.InfoUtente, sdInfo, pagingContext);
                resp.Elements = new List<Delega>();
                foreach (InfoDelega temp in deleghe)
                {
                    logger.DebugFormat("Delega: decorr: {0} -> scad :{1} codDel {2}", temp.dataDecorrenza, temp.dataScadenza, temp.codiceDelegante);
                    if ("0".Equals(temp.id_ruolo_delegante))
                    {
                        resp.Elements.Add(Delega.buildInstance(temp, null));
                    }
                    else
                    {
                        Ruolo ruolo = BusinessLogic.Utenti.UserManager.getRuoloById(temp.id_ruolo_delegante);
                        resp.Elements.Add(Delega.buildInstance(temp, ruolo));
                    }
                }
                resp.TotalRecordCount = deleghe.Count;
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Errore {0} {1}", e.Message, e.StackTrace);
                resp = DelegheResponse.ErrorResponse;
                resp.TotalRecordCount = 0;
            }
            return resp;
        }

        public static CountDelegheAttiveResponse getCountDelegheAttive(CountDelegheAttiveRequest request)
        {
            CountDelegheAttiveResponse resp = new CountDelegheAttiveResponse();
            resp.NumDeleghe = BusinessLogic.Deleghe.DelegheManager.checkDelegaAttiva(request.UserInfo.InfoUtente);
            return resp;
        }

        public static ListaModelliDelegaResponse getListaModelliDelega(ListaModelliDelegaRequest request)
        {
            ListaModelliDelegaResponse resp = new ListaModelliDelegaResponse();
            try
            {
                resp.Modelli = new List<ModelloDelegaInfo>();
                List<ModelloDelega> tempList = BusinessLogic.Deleghe.ModelliDelegaManager.getModelliDelegaByStato(request.UserInfo.InfoUtente, StatoModelloDelega.VALIDO);
                foreach (ModelloDelega temp in tempList)
                {
                    resp.Modelli.Add(ModelloDelegaInfo.buildInstance(temp));
                }
                resp.Code = ListaModelliDelegaResponseCode.OK;
            }
            catch (Exception e)
            {
                resp = ListaModelliDelegaResponse.ErrorResponse;
            }
            return resp;
        }

        public static CreaDelegaResponse creaDelega(CreaDelegaRequest request)
        {
            try
            {
                logger.Info("begin");
                Delega input = request.Delega;
                CreaDelegaResponse resp = new CreaDelegaResponse();
                InfoDelega delega = new InfoDelega();
                if (string.IsNullOrEmpty(input.IdRuoloDelegante) || input.IdRuoloDelegante=="Tutti")
                {
                    logger.Debug("crea delega per tutti i ruoli");
                    delega.id_ruolo_delegante = "0";
                    delega.cod_ruolo_delegante = "TUTTI";
                }
                else
                {
                    logger.Debug("crea delega per il ruolo con id " + input.IdRuoloDelegante);
                    delega.id_ruolo_delegante = input.IdRuoloDelegante;
                    Ruolo ruoloDelegante = BusinessLogic.Utenti.UserManager.getRuoloById(input.IdRuoloDelegante);
                    logger.Debug("codice ruolo delegante: " + ruoloDelegante.codice);
                    delega.cod_ruolo_delegante = ruoloDelegante.codice;
                }
                delega.id_utente_delegante = request.UserInfo.IdPeople;
                logger.Debug("ricerca delegante...");
                Corrispondente delegante = BusinessLogic.Utenti.UserManager.getCorrispondenteByIdPeople(request.UserInfo.IdPeople, TipoUtente.INTERNO, request.UserInfo.InfoUtente);
                logger.Debug("delegante: " + delegante.codiceRubrica);
                delega.cod_utente_delegante = delegante.codiceRubrica;
                //ricerca delegato
                logger.Debug("ricerca delegato");
                Corrispondente delegato = BusinessLogic.Utenti.UserManager.getCorrispondenteByIdPeople(request.Delega.IdDelegato, TipoUtente.INTERNO, request.UserInfo.InfoUtente);
                logger.Debug("delegato: " + delegante.codiceRubrica);
                delega.cod_utente_delegato = delegato.codiceRubrica;
                //ricerca ruoli delegato
                ArrayList ruoliDelegato = BusinessLogic.Utenti.UserManager.getRuoliUtente(input.IdDelegato);
                ruoliDelegato.Sort(new RuoliComparer());
                delega.cod_ruolo_delegato = ((Ruolo)ruoliDelegato[0]).codice;
                delega.id_ruolo_delegato = ((Ruolo)ruoliDelegato[0]).systemId;
                delega.id_uo_delegato = ((Ruolo)ruoliDelegato[0]).uo.systemId;
                delega.id_utente_delegato = input.IdDelegato;
                delega.dataDecorrenza = buildDateString(input.DataDecorrenza).Replace('.', ':');
                if (input.DataScadenza >= input.DataDecorrenza) delega.dataScadenza = buildDateString(input.DataScadenza, true).Replace('.', ':');
                logger.Debug("dataDecorrenza: " + delega.dataDecorrenza + " dataScadenza: " + delega.dataScadenza);

                if (BusinessLogic.Deleghe.DelegheManager.verificaUnicaDelega(request.UserInfo.InfoUtente, delega))
                {
                    bool result = BusinessLogic.Deleghe.DelegheManager.creaNuovaDelega(request.UserInfo.InfoUtente, delega);
                    if (result == false)
                    {
                        resp.Code = CreaDelegaResponseCode.NOT_CREATED;
                    }
                    else
                    {
                        resp.Code = CreaDelegaResponseCode.OK;
                    }
                }
                else
                {
                    resp.Code = CreaDelegaResponseCode.OVERLAPPING_PERIODS;
                }
                return resp;
            }
            catch (Exception e)
            {
                return CreaDelegaResponse.ErrorResponse;
            }
        }

        private static string buildDateString(DateTime data)
        {
            return MobileManager.buildDateString(data, false);
        }
        
        private static string buildDateString(DateTime data, bool isEnding)
        {
            if (isEnding)
            {
                return data.ToShortDateString() + " 23.59.59";
            }
            else
            {
                string ora = "" + data.Hour;
                if (ora.Length == 1) ora = "0" + ora;
                return data.ToShortDateString() + " " + ora + ".00.00";
            }
        }

        public static CreaDelegaDaModelloResponse creaDelegaDaModello(CreaDelegaDaModelloRequest request)
        {
            try
            {
                CreaDelegaDaModelloResponse resp = new CreaDelegaDaModelloResponse();
                InfoDelega delega = BusinessLogic.Deleghe.ModelliDelegaManager.buildDelegaFromModello(request.UserInfo.InfoUtente, request.IdModelloDelega, request.DataInizio, request.DataFine);
                bool result = BusinessLogic.Deleghe.DelegheManager.creaNuovaDelega(request.UserInfo.InfoUtente, delega);
                if (result == false)
                {
                    resp.Code = CreaDelegaDaModelloResponseCode.NOT_CREATED;
                }
                else
                {
                    resp.Code = CreaDelegaDaModelloResponseCode.OK;
                }
                return resp;
            }
            catch (Exception e)
            {
                return CreaDelegaDaModelloResponse.ErrorResponse;
            }
        }

        public static AccettaDelegaResponse accettaDelega(AccettaDelegaRequest request)
        {
            logger.Debug("Start");
            try
            {
                UserLogin userLogin = new UserLogin();
                userLogin.UserName = request.Delega.CodiceDelegante;
                userLogin.Password = "";
                userLogin.IdAmministrazione = request.UserInfo.IdAmministrazione;
                userLogin.IPAddress = request.IpAddress;
                Utente utente = null;
                DelegaInfo delega = request.Delega;
                DocsPaVO.utente.UserLogin.LoginResult loginResult = new DocsPaVO.utente.UserLogin.LoginResult();
                logger.DebugFormat("chiamo accettaDelega sessID {0}  delega.Id {1} idRuoloDelegante{2}", request.SessionId, delega.Id, delega.IdRuoloDelegante);
                utente = BusinessLogic.Deleghe.DelegheManager.esercitaDelega(request.UserInfo.InfoUtente, userLogin, request.SessionId, delega.Id, delega.IdRuoloDelegante, out loginResult);
                if (loginResult != DocsPaVO.utente.UserLogin.LoginResult.OK)
                {
                    return AccettaDelegaResponse.buildErrorInstance(loginResult);
                }
                UserInfo userInfo = UserInfo.buildInstance(utente);
                AccettaDelegaResponse resp = new AccettaDelegaResponse();
                resp.Code = AccettaDelegaResponseCode.OK;
                resp.UserInfo = userInfo;
                InfoDelega temp = BusinessLogic.Deleghe.DelegheManager.getDelegaById(delega.Id);
                Ruolo ruolo = BusinessLogic.Utenti.UserManager.getRuoloById(temp.id_ruolo_delegante);
                if (temp != null) resp.DelegaAccettata = Delega.buildInstance(temp, ruolo);
                return resp;
            }
            catch (Exception e)
            {
                return AccettaDelegaResponse.ErrorResponse;
            }
        }

        public static DismettiDelegaResponse dismettiDelega(DismettiDelegaRequest request)
        {
            try
            {
                DismettiDelegaResponse resp = new DismettiDelegaResponse();
                bool dismetti = BusinessLogic.Deleghe.DelegheManager.dismettiDelega(request.UserInfo.InfoUtente, request.IdDelegante);
                if (dismetti)
                {
                    ArrayList ruoli = BusinessLogic.Utenti.UserManager.getRuoliUtente(request.UserInfo.IdPeople);
                    List<RuoloInfo> ruoloInfoLst = new List<RuoloInfo>();

                    foreach (Ruolo r in ruoli)
                        ruoloInfoLst.Add(RuoloInfo.buildInstance(r));
                    resp.UserInfo = request.UserInfo;
                    resp.UserInfo.Ruoli = ruoloInfoLst;
                    resp.Code = DismettiDelegaResponseCode.OK;
                }
                else
                {
                    resp.Code = DismettiDelegaResponseCode.OPERATION_FAILED;
                }

                return resp;
            }
            catch (Exception e)
            {
                return DismettiDelegaResponse.ErrorResponse;
            }
        }

        public static RevocaDelegheResponse revocaDeleghe(RevocaDelegheRequest request)
        {
            try
            {
                RevocaDelegheResponse res = new RevocaDelegheResponse();
                string msg = string.Empty;
                InfoDelega[] deleghe = request.Deleghe.Select<Delega, InfoDelega>(e => e.InfoDelega).ToArray();
                logger.Debug("chiamata metodo businesslogic...");
                bool operResult = BusinessLogic.Deleghe.DelegheManager.revocaDelega(request.UserInfo.InfoUtente, deleghe, out msg);
                logger.Debug("codice ottenuto: " + operResult + " messaggio: " + msg);
                if (operResult)
                {
                    res.Code = RevocaDelegheResponseCode.OK;
                }
                else
                {
                    res.Code = RevocaDelegheResponseCode.OPERATION_FAILED;
                    res.Error = msg;
                }
                return res;
            }
            catch (Exception e)
            {
                logger.Error("Eccezione: " + e);
                return RevocaDelegheResponse.ErrorResponse;
            }
        }

        public static ListaTipiRuoloResponse getListaRuoli(ListaTipiRuoloRequest request)
        {
            try
            {
                ListaTipiRuoloResponse resp = new ListaTipiRuoloResponse();
                ArrayList list = BusinessLogic.Amministrazione.OrganigrammaManager.GetListTipiRuolo(request.IdAmministrazione);
                resp.TipiRuolo = new List<TipoRuoloInfo>();
                foreach (OrgTipoRuolo temp in list)
                {
                    resp.TipiRuolo.Add(TipoRuoloInfo.buildInstance(temp));
                }
                resp.Code = ListaRuoliResponseCode.OK;
                return resp;
            }
            catch (Exception e)
            {
                return ListaTipiRuoloResponse.ErrorResponse;
            }
        }

        public static ListaUtentiResponse getListaUtenti(ListaUtentiRequest request)
        {
            try
            {
                ListaUtentiResponse resp = new ListaUtentiResponse();
                resp.Utenti = new List<UserInfo>();
                ArrayList ruoli = BusinessLogic.Amministrazione.TipiRuoloManager.ListTipoRuoloUtenti(request.CodiceTipoRuolo, request.IdAmministrazione);
                foreach (OrgRuolo temp in ruoli)
                {
                    foreach (OrgUtente utente in temp.Utenti)
                    {
                        resp.Utenti.Add(UserInfo.buildInstance(utente));
                    }
                }
                resp.Code = ListaUtentiResponseCode.OK;
                return resp;
            }
            catch (Exception e)
            {
                return ListaUtentiResponse.ErrorResponse;
            }

        }

        public static RicercaUtentiResponse ricercaUtenti(RicercaUtentiRequest request)
        {
            RicercaUtentiResponse res = new RicercaUtentiResponse();
            List<String> idRegistri = (from a in request.Ruolo.Registri select a.SystemId).ToList();
            String idRegistro = idRegistri.FirstOrDefault();

            res.Risultati = BusinessLogic.Mobile.RubricaManager.GetListaUtentiInterni(request.Descrizione.ToUpper(), request.UserInfo.InfoUtente, request.NumMaxResults, idRegistro);
            return res;
        }
        #endregion
        #region Trasmissioni

        public static AccettaRifiutaTrasmResponse setDataVistaSP_TV(AccettaRifiutaTrasmRequest request)
        {
            logger.Info("begin MobileManager.setDataVistaSP_TV");
            try
            {
                InfoUtente infoUser = request.UserInfo.InfoUtente;
                infoUser.idGruppo = request.IdGruppo;
                DocsPaVO.trasmissione.Trasmissione trasm = BusinessLogic.Trasmissioni.TrasmManager.CreateObjTrasmissioneByID(request.IdTrasmissione);
                bool result = false;
                if(trasm.infoDocumento != null && !string.IsNullOrEmpty(trasm.infoDocumento.docNumber))
                {
                    result = BusinessLogic.Documenti.DocManager.setDataVistaSP_TV(infoUser, trasm.infoDocumento.docNumber, "D", trasm.infoDocumento.idRegistro); ;
                }
                else
                {
                    result = BusinessLogic.Documenti.DocManager.setDataVistaSP_TV(infoUser, trasm.infoFascicolo.idFascicolo, "F", string.Empty);
                } 
                AccettaRifiutaTrasmResponse resp = new AccettaRifiutaTrasmResponse();
                if (result)
                {
                    resp.Code = AccettaRifiutaTrasmResponseCode.OK;
                }
                else
                {
                    resp.Code = AccettaRifiutaTrasmResponseCode.BL_ERROR;
                }
                logger.Info("end MobileManager.setDataVistaSP_TV");
                return resp;
            }
            catch (Exception e)
            {
                logger.Error("eccezione MobileManager.setDataVistaSP_TV: " + e);
                return AccettaRifiutaTrasmResponse.ErrorResponse;
            }
        }

        public static AccettaRifiutaTrasmResponse accettaRifiutaTrasm(AccettaRifiutaTrasmRequest request)
        {
            logger.Info("begin MobileManager.accettaRifiutaTrasm");
            try
            {
                TrasmissioneUtente tu = new TrasmissioneUtente();
                tu.utente = request.UserInfo.Utente;
                if (!string.IsNullOrEmpty(request.IdTrasmissioneUtente))
                {
                    logger.Debug("trasmissione utente con id " + request.IdTrasmissioneUtente);
                    tu.systemId = request.IdTrasmissioneUtente;
                }
                else
                {
                    logger.Debug("ricerca trasmissione utente, idTrasm: " + request.IdTrasmissione);
                    
                    DocsPaVO.trasmissione.Trasmissione trasm = BusinessLogic.Trasmissioni.TrasmManager.CreateObjTrasmissioneByID(request.IdTrasmissione);
                    foreach (TrasmissioneSingola temp in trasm.trasmissioniSingole)
                    {
                        foreach (TrasmissioneUtente tempUt in temp.trasmissioneUtente)
                        {
                            if (request.UserInfo.UserId.Equals(tempUt.utente.userId))
                            {
                                if (
                                    (temp.tipoDest == TipoDestinatario.RUOLO && int.Parse(temp.corrispondenteInterno.systemId) - 1 == int.Parse(request.IdGruppo))
                                    || temp.tipoDest != TipoDestinatario.RUOLO
                                )
                                {
                                    tu.systemId = tempUt.systemId;
                                    logger.Debug("idTrasmUtente: " + tu.systemId);
                                    if (!String.IsNullOrEmpty(tempUt.dataAccettata) || !String.IsNullOrEmpty(tempUt.dataRifiutata))
                                    {   //è Già stata accettata o rifiutata.. 
                                        return new AccettaRifiutaTrasmResponse { Code = AccettaRifiutaTrasmResponseCode.BL_ERROR, Errore = "La trasmissione è già stata accettata o rifiutata" };
                                    }
                                }
                            }
                        }
                    }
                }
                if (request.Action == AccettaRifiutaAction.ACCETTA)
                {
                    tu.noteAccettazione = request.Note;
                    tu.tipoRisposta = TipoRisposta.ACCETTAZIONE;
                }
                else
                {
                    tu.noteRifiuto = request.Note;
                    tu.tipoRisposta = TipoRisposta.RIFIUTO;
                }
                tu.dataVista = Convert.ToString(DateTime.Now);
                Utente utente = request.UserInfo.Utente;
                Ruolo ruolo = new Ruolo();
                ruolo.idGruppo = request.IdGruppo;
                string errore;
                string mode;
                string idObj;
                bool result = BusinessLogic.Trasmissioni.ExecTrasmManager.executeAccRifMethod(tu, request.IdTrasmissione, ruolo, request.UserInfo.InfoUtente, out errore, out mode, out idObj);
                AccettaRifiutaTrasmResponse resp = new AccettaRifiutaTrasmResponse();
                if (result)
                {
                    resp.Code = AccettaRifiutaTrasmResponseCode.OK;
                }
                else
                {
                    resp.Code = AccettaRifiutaTrasmResponseCode.BL_ERROR;
                    resp.Errore = errore;
                }
                logger.Info("end MobileManager.accettaRifiutaTrasm");
                return resp;
            }
            catch (Exception e)
            {
                logger.Error("eccezione MobileManager.accettaRifiutaTrasm: " + e);
                return AccettaRifiutaTrasmResponse.ErrorResponse;
            }
        }

        public static ListaModelliTrasmResponse getListaModelliTrasm(ListaModelliTrasmRequest request)
        {
            logger.Info("begin");
            try
            {
                ListaModelliTrasmResponse resp = new ListaModelliTrasmResponse();
                Utente ut = request.UserInfo.Utente;
                InfoUtente iu = request.UserInfo.InfoUtente;
                iu.idCorrGlobali = request.IdCorrGlobali;
                ArrayList listaModelli = BusinessLogic.Trasmissioni.ModelliTrasmissioni.getModelliUtente(ut, iu, null);
                resp.Code = ListaModelliTrasmResponseCode.OK;
                resp.Modelli = new List<ModelloTrasm>();
                string tipo = "D";
                if (request.TrasmFasc) tipo = "F";
                foreach (ModelloTrasmissione temp in listaModelli)
                {
                    if (tipo.Equals(temp.CHA_TIPO_OGGETTO)) resp.Modelli.Add(ModelloTrasm.buildInstance(temp));
                }
                logger.Info("end");
                //PassTrough
                return resp;
            }
            catch (Exception e)
            {
                logger.Error("eccezione: " + e);
                return ListaModelliTrasmResponse.ErrorResponse;
            }
        }

        public static EseguiTrasmResponse eseguiTrasm(EseguiTrasmRequest request)
        {
            try
            {
                logger.Info("begin");
                EseguiTrasmResponse response = new EseguiTrasmResponse();
                response.Code = EseguiTrasmResponseCode.OK;
                ModelloTrasmissione modello = BusinessLogic.Trasmissioni.ModelliTrasmissioni.getModelloByID(request.UserInfo.IdAmministrazione, request.IdModelloTrasm);
                Ruolo ruolo = BusinessLogic.Utenti.UserManager.getRuoloById(request.IdCorrGlobali);
                string id = (!string.IsNullOrEmpty(request.IdDoc)) ? request.IdDoc : request.IdFasc;
                bool isDoc = !string.IsNullOrEmpty(request.IdDoc);
                InfoUtente infoUtente = request.UserInfo.InfoUtente;
                infoUtente.idGruppo = ruolo.idGruppo;
                infoUtente.idCorrGlobali = ruolo.systemId;
                TrasmBuilder builder = new TrasmBuilder(modello, id, infoUtente, ruolo, isDoc);
                DocsPaVO.trasmissione.Trasmissione trasm = builder.Trasmissione;
                if (!string.IsNullOrEmpty(request.Note))
                {
                    trasm.noteGenerali = request.Note;
                }

                //trasm = BusinessLogic.Trasmissioni.TrasmManager.saveTrasmMethod(trasm);
                // Gabriele Melini 03-10-2014
                // il path da inserire nella mail dev'essere quello del frontend, non quello del mobile
                // lo recupero dalla chiave di BE URL_PATH_IS
                string path = string.Empty;
                
                if (System.Configuration.ConfigurationManager.AppSettings["URL_PATH_IS"] != null)
                    path = System.Configuration.ConfigurationManager.AppSettings["URL_PATH_IS"].ToString();
                else
                    path = request.Path;

                logger.Info("path : " + path);
                trasm = BusinessLogic.Trasmissioni.ExecTrasmManager.saveExecuteTrasmMethod(path, trasm);

                string notify = "1";
                if (trasm.NO_NOTIFY != null && trasm.NO_NOTIFY.Equals("1"))
                {
                    notify = "0";
                }
                else
                {
                    notify = "1";
                }
                if (trasm != null)
                {
                    string desc = string.Empty;
                    // LOG per documento
                    if (trasm.infoDocumento != null && !string.IsNullOrEmpty(trasm.infoDocumento.idProfile))
                    {
                        foreach (DocsPaVO.trasmissione.TrasmissioneSingola single in trasm.trasmissioniSingole)
                        {
                            string method = "TRASM_DOC_" + single.ragione.descrizione.ToUpper().Replace(" ", "_");
                            if (trasm.infoDocumento.segnatura == null)
                                desc = "Trasmesso Documento : " + trasm.infoDocumento.docNumber.ToString();
                            else
                                desc = "Trasmesso Documento : " + trasm.infoDocumento.segnatura.ToString();

                            BusinessLogic.UserLog.UserLog.WriteLog(trasm.utente.userId, trasm.utente.idPeople, trasm.ruolo.idGruppo, trasm.utente.idAmministrazione, method, trasm.infoDocumento.docNumber, desc, DocsPaVO.Logger.CodAzione.Esito.OK, infoUtente.delegato, notify, single.systemId);
                        }
                    }
                }
                //BusinessLogic.Trasmissioni.ExecTrasmManager.executeTrasmMethod(path, trasm);
                //BusinessLogic.Trasmissioni.ExecTrasmManager.executeTrasmMethod(request.Path, trasm);

                logger.Info("end");
                return response;
            }
            catch (Exception e)
            {
                logger.Error("eccezione: " + e);
                return EseguiTrasmResponse.ErrorResponse;
            }
        }

        #endregion

        #region Smistamento

        public static RicercaSmistamentoResponse ricercaSmistamento(RicercaSmistamentoRequest request)
        {
            try
            {
                logger.Info("begin");
                RicercaSmistamentoResponse res = new RicercaSmistamentoResponse();

                List<String> idRegistri = (from a in request.Ruolo.Registri select a.SystemId).ToList();
                String idRegistro = idRegistri.FirstOrDefault();
                List<RicercaSmistamentoElement> elements = BusinessLogic.Mobile.RubricaManager.GetListaElementiInterni(request.Descrizione, request.UserInfo.InfoUtente, request.NumMaxResults, request.numMaxResultsForCategory, idRegistro);
                
                res.Elements = elements;
                res.Code = RicercaSmistamentoResponseCode.OK;

                return res;
            }
            catch (Exception e)
            {
                logger.Error("Exception: " + e);
                return RicercaSmistamentoResponse.ErrorResponse;
            }
        }

        /// <summary>
        /// MEV SMISTAMENTO
        /// Metodo per l'esecuzione della ricerca ajax.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static RicercaSmistamentoResponse GetListaCorrispondentiVeloce(RicercaSmistamentoRequest request)
        {
            try
            {
                logger.Info("begin");
                logger.Info("descrizione: " + request.Descrizione);
                logger.Info("ragione: " + request.Ragione);
                RicercaSmistamentoResponse res = new RicercaSmistamentoResponse();

                List<String> idRegistri = (from a in request.Ruolo.Registri select a.SystemId).ToList();
                String idRegistro = idRegistri.FirstOrDefault();

                string IdRuolo = request.Ruolo.Id;

                List<RicercaSmistamentoElement> elements = BusinessLogic.Mobile.RubricaManager.GetListaCorrispondentiVeloce(request.Descrizione, request.UserInfo.InfoUtente, idRegistro, IdRuolo, request.Ragione, request.NumMaxResults);
                res.Elements = elements;
                res.Code = RicercaSmistamentoResponseCode.OK;

                return res;
            }
            catch (Exception ex)
            {
                logger.Error("Exception: " + ex);
                return RicercaSmistamentoResponse.ErrorResponse;
            }

        }

        //public static RicercaSmistamentoResponse AggiungiElementoRicerca(RicercaSmistamentoRequest request)
        //{
        //    try
        //    {
        //        logger.Info("begin");
        //        RicercaSmistamentoResponse res = new RicercaSmistamentoResponse();

        //        List<String> idRegistri = (from a in request.Ruolo.Registri select a.SystemId).ToList();
        //        String idRegistro = idRegistri.FirstOrDefault();

        //        List<RicercaSmistamentoElement> elements = new List<RicercaSmistamentoElement>();
        //        res.Elements = elements;
        //        res.Code = RicercaSmistamentoResponseCode.OK;

        //        return res;
        //    }
        //    catch (Exception e)
        //    {
        //        logger.Error("Exception: " + e);
        //        return RicercaSmistamentoResponse.ErrorResponse;
        //    }
        //}

        public static GetSmistamentoTreeResponse getSmistamentoTree(GetSmistamentoTreeRequest request)
        {
            try
            {
                logger.Info("begin");
                GetSmistamentoTreeResponse res = new GetSmistamentoTreeResponse();
                MittenteSmistamento mittente = new MittenteSmistamento();
                mittente.IDAmministrazione = request.UserInfo.IdAmministrazione;
                mittente.IDCorrGlobaleRuolo = null;
                mittente.IDGroup = request.Ruolo.IdGruppo;
                mittente.IDPeople = request.UserInfo.IdPeople;
                mittente.LivelloRuolo = request.Ruolo.Livello;
                foreach (RegistroInfo temp in request.Ruolo.Registri)
                {
                    mittente.RegistriAppartenenza.Add(temp.SystemId);
                }

                UOSmistamento uoAppartenenza = BusinessLogic.SmistamentoDocumenti.SmistamentoManager.GetUOAppartenenza(request.Ruolo.Id, mittente, false);
                SmistamentoTree tree = new SmistamentoTree();

                UOSmistamento uoAttuale = new UOSmistamento();

                if (!string.IsNullOrEmpty(request.IdUO) && !request.IdUO.Equals(request.Ruolo.IdUO))
                {
                    uoAttuale = BusinessLogic.SmistamentoDocumenti.SmistamentoManager.GetUOAppartenenza(request.IdUO, mittente, true);
                }

               

                // MEV SMISTAMENTO
                // nuovo codice per navigazione UO inf/sup
                // è utilizzata la stessa logica della versione desktop
                List<UOSmistamento> uoInferiori = null;
                if (!string.IsNullOrEmpty(request.IdUO))
                {
                    ArrayList uoInf = BusinessLogic.SmistamentoDocumenti.SmistamentoManager.GetUOInferiori(request.IdUO, mittente);
                    if(uoInf != null)
                        uoInferiori = uoInf.Cast<UOSmistamento>().ToList();
                }
                else
                {
                    ArrayList uoInf = BusinessLogic.SmistamentoDocumenti.SmistamentoManager.GetUOInferiori(request.Ruolo.IdUO, mittente);
                    if(uoInf != null)
                        uoInferiori = uoInf.Cast<UOSmistamento>().ToList();
                }


                tree.AltreUO = new List<SmistamentoUONode>();
                if (uoInferiori != null)
                {
                    foreach (UOSmistamento uo in uoInferiori)
                    {
                        tree.AltreUO.Add(SmistamentoUONode.BuildInstance(uo, request.UserInfo, request.Ruolo.Id));
                    }
                }

                #region OLD CODE
                // Codice per avere le UO inferiori e superiori
                //-------------   

                //List<UOSmistamento> UOs = new List<UOSmistamento>();
                //foreach (RegistroInfo temp in request.Ruolo.Registri)
                //    UOs.AddRange(BusinessLogic.SmistamentoDocumenti.SmistamentoManager.GetListUOSmistamento(temp.SystemId, mittente).OfType<UOSmistamento>());

                //tree.AltreUO = new List<SmistamentoUONode>();
                //foreach (UOSmistamento us in UOs)
                //    tree.AltreUO.Add(SmistamentoUONode.BuildInstance(us, request.UserInfo, request.Ruolo.Id));

                //--------------
                #endregion

                if (!string.IsNullOrEmpty(request.IdUO) && !request.IdUO.Equals(request.Ruolo.IdUO))
                {
                    tree.UOAppartenenza = SmistamentoUONode.BuildInstance(uoAttuale, request.UserInfo, request.Ruolo.Id);
                    tree.idParent = BusinessLogic.Amministrazione.OrganigrammaManager.AmmListaIDParentRicerca(request.IdUO, "U")[1].ToString();
                }
                else
                {
                    tree.UOAppartenenza = SmistamentoUONode.BuildInstance(uoAppartenenza, request.UserInfo, request.Ruolo.Id);
                    tree.idParent = string.Empty;
                }
                
                //tree.UOAppartenenza = SmistamentoUONode.BuildInstance(uoAppartenenza, request.UserInfo, request.Ruolo.Id);
                res.Code = GetSmistamentoTreeResponseCode.OK;
                res.Element = tree;
                return res;
            }
            catch (Exception e)
            {
                logger.Error("Exception: " + e);
                return GetSmistamentoTreeResponse.ErrorResponse;
            }
        }

        public static GetSmistamentoElementsResponse getSmistamentoElements(GetSmistamentoElementsRequest request)
        {
            try
            {
                logger.Info("begin");
                GetSmistamentoElementsResponse res = new GetSmistamentoElementsResponse();
                res.Code = GetSmistamentoElementsResponseCode.OK;
                BusinessLogic.Trasmissioni.TrasmManager trasmManager = new BusinessLogic.Trasmissioni.TrasmManager();
                int totalRecordCount, totalTrasmNonViste;
                res.Elements = new List<SmistamentoElement>();
                InfoUtente infoUtente = request.UserInfo.InfoUtente;
                infoUtente.idGruppo = request.IdGruppo;
                infoUtente.idCorrGlobali = request.IdCorrGlobali;
                FiltroRicerca[] filtri = new FiltroRicerca[4];
                filtri[0] = new FiltroRicerca();
                filtri[0].argomento = listaArgomentiNascosti.TODO_LIST.ToString();
                filtri[1] = new FiltroRicerca();
                filtri[1].argomento = listaArgomentiNascosti.NO_CERCA_INFERIORI.ToString();
                filtri[2] = new FiltroRicerca();
                filtri[2].argomento = listaArgomentiNascosti.ELEMENTI_NON_VISTI.ToString();
                filtri[2].valore = "0";
                filtri[3] = new FiltroRicerca();
                filtri[3].argomento = listaArgomentiNascosti.TIPO_OGGETTO.ToString();
                filtri[3].valore = "D";
                IEnumerable<infoToDoList> temp = trasmManager.getMyNewTodoList(infoUtente, request.Registri, filtri, request.RequestedPage, request.PageSize, out totalRecordCount, out totalTrasmNonViste).OfType<infoToDoList>();
                foreach (infoToDoList tdl in temp)
                {
                    DocsPaVO.trasmissione.Trasmissione trasm = BusinessLogic.Trasmissioni.TrasmManager.CreateObjTrasmissioneByID(tdl.sysIdTrasm);
                    res.Elements.Add(SmistamentoElement.BuildInstance(tdl, trasm, request.UserInfo));
                }
                res.TotalRecordCount = totalRecordCount;
                logger.Info("end");
                return res;
            }
            catch (Exception e)
            {
                logger.Error("Exception: " + e);
                return GetSmistamentoElementsResponse.ErrorResponse;
            }
        }

        public static EseguiSmistamentoResponse eseguiSmistamento(EseguiSmistamentoRequest request)
        {

            try
            {
                logger.Info("begin");
                // MEV SMISTAMENTO - DEBUG
                foreach (EseguiSmistamentoElement el in request.Elements)
                {
                    logger.Info(string.Format("Destinatario - IdUtente: {0}, IdRuolo: {1}, IdUO: {2}, comp: {3}, conosc: {4}, note: {5}, isRicerca: {6}", el.IdUtente, el.IdRuolo, el.IdUO, el.Competenza, el.Conoscenza, el.NoteIndividuali, el.isRicerca));
                    logger.Info(string.Format("Mittente: IdUtente {0} IdGruppo {1} ", request.UserInfo.InfoUtente.idPeople, request.UserInfo.InfoUtente.idGruppo));
                    logger.Info(string.Format("RuoloInfo: {0}", request.Ruolo.IdGruppo));
                }
                // Gabriele Melini 13-02-2014 
                // check se idgruppo=null in infoutente
                // causa vari schianti nell'invio delle notifiche
                InfoUtente infoUt = request.UserInfo.InfoUtente;
                if (string.IsNullOrEmpty(infoUt.idGruppo))
                {
                    infoUt.idGruppo = request.Ruolo.IdGruppo;
                }

                EseguiSmistamentoResponse res = new EseguiSmistamentoResponse();

                //Creo il mittente
                MittenteSmistamento mittente = new MittenteSmistamento();
                mittente.IDAmministrazione = request.UserInfo.IdAmministrazione;
                mittente.IDPeople = request.UserInfo.IdPeople;

                foreach (RegistroInfo temp in request.Ruolo.Registri)
                {
                    mittente.RegistriAppartenenza.Add(temp.SystemId);
                }

                mittente.EMail = BusinessLogic.Utenti.UserManager.getUtenteById(request.UserInfo.IdPeople).email;
                mittente.IDCorrGlobaleRuolo = request.Ruolo.Id;
                mittente.IDGroup = request.Ruolo.IdGruppo;
                mittente.LivelloRuolo = request.Ruolo.Livello;

                #region OLD CODE

                //Creo uo smista  e uoInferiori

                /*
                DocsPaVO.Smistamento.UOSmistamento uoSmista = null;
                DocsPaVO.Smistamento.UOSmistamento[] uoInferiori = null;
                
                uoSmista = BusinessLogic.SmistamentoDocumenti.SmistamentoManager.GetUOAppartenenza(request.Ruolo.Id, mittente, false);
                uoSmista.UoInferiori = BusinessLogic.SmistamentoDocumenti.SmistamentoManager.GetUOInferiori(uoSmista.ID, mittente);
                if (uoSmista.UoInferiori != null)
                {
                    uoInferiori = uoSmista.UoInferiori.Cast<DocsPaVO.Smistamento.UOSmistamento>().ToArray();
                }
                */

                #endregion

                if (string.IsNullOrEmpty(request.NoteGenerali))
                    request.NoteGenerali = String.Empty;


                //Dati trasmissione documento
                DatiTrasmissioneDocumento dtd = new DatiTrasmissioneDocumento();
                dtd.IDDocumento = request.IdDocumento;
                dtd.IDTrasmissione = request.IdTrasmissione;
                dtd.IDTrasmissioneUtente = request.IdTrasmissioneUtente;
                dtd.IDTrasmissioneSingola = request.IdTrasmissioneSingola;
                dtd.NoteGenerali = request.NoteGenerali;

                #region OLD CODE

                //foreach (EseguiSmistamentoElement se in request.Elements)
                //{
                //    se.fillUOSmistamento(uoSmista);
                //}

                //DocumentoSmistamento doc = BusinessLogic.SmistamentoDocumenti.SmistamentoManager.GetDocumentoSmistamento(request.IdDocumento, request.UserInfo.InfoUtente, false);

                //List<DocsPaVO.Smistamento.EsitoSmistamentoDocumento> retvalSmistaDocumento = null;
                //retvalSmistaDocumento = BusinessLogic.SmistamentoDocumenti.SmistamentoManager.SmistaDocumento(
                //     mittente,
                //     request.UserInfo.InfoUtente,
                //     doc,
                //     dtd,
                //     uoSmista,
                //     uoInferiori,
                //     null).Cast<DocsPaVO.Smistamento.EsitoSmistamentoDocumento>().ToList();

                #endregion

                // ---------- MEV SMISTAMENTO ----------
                // nuovo codice per gestire smistamento a destinatari da ricerca ajax
                //DocumentoSmistamento doc = BusinessLogic.SmistamentoDocumenti.SmistamentoManager.GetDocumentoSmistamento(request.IdDocumento, request.UserInfo.InfoUtente, false);
                DocumentoSmistamento doc = BusinessLogic.SmistamentoDocumenti.SmistamentoManager.GetDocumentoSmistamento(request.IdDocumento, infoUt, false);

                // individuo le UO a cui smistare
                List<DocsPaVO.Smistamento.UOSmistamento> targetUOs = new List<UOSmistamento>();
                //List<string> idUOs = (from a in request.Elements where a.isRicerca select a.IdUO).Distinct().ToList();
                List<string> idUOs = (from a in request.Elements select a.IdUO).Distinct().ToList();
        
                foreach (string id in idUOs)
                {
                    DocsPaVO.Smistamento.UOSmistamento newUO = new UOSmistamento();
                    newUO = BusinessLogic.SmistamentoDocumenti.SmistamentoManager.GetUOAppartenenza(id, mittente, true);
                    targetUOs.Add(newUO);
                }

                #region OLD CODE
                /*
                foreach (EseguiSmistamentoElement se in request.Elements)
                {
                    // il destinatario proviene dalla ricerca ajax
                    if (se.isRicerca)   
                    {
                        DocsPaVO.Smistamento.UOSmistamento newUO = new UOSmistamento();
                        newUO = BusinessLogic.SmistamentoDocumenti.SmistamentoManager.GetUOAppartenenza(se.IdUO, mittente, true);
                        if(!targetUOs.Contains(newUO))
                            targetUOs.Add(newUO);
                    }
                    // il destinatario proviene dalla vista principale
                    else
                    {
                        if (!targetUOs.Contains(uoSmista))
                            targetUOs.Add(uoSmista);
                    }
                }
                */
                #endregion

                // imposto i destinatari 
                foreach(UOSmistamento uo in targetUOs)
                {
                    List<EseguiSmistamentoElement> targetElements = (from a in request.Elements where a.IdUO.Equals(uo.ID) select a).ToList();
                    targetElements.ForEach(e => e.fillUOSmistamento(uo));
                }

                List<DocsPaVO.Smistamento.EsitoSmistamentoDocumento> retvalSmistaDocumento = new List<EsitoSmistamentoDocumento>();

                // smisto a tutte le uo selezionate
                foreach (UOSmistamento uo in targetUOs)
                {
                    uo.UoInferiori = BusinessLogic.SmistamentoDocumenti.SmistamentoManager.GetUOInferiori(uo.ID, mittente);
                    DocsPaVO.Smistamento.UOSmistamento[] uoInf = null; 
                    if(uo.UoInferiori != null)
                        uoInf = uo.UoInferiori.Cast<UOSmistamento>().ToArray();

                    List<DocsPaVO.Smistamento.EsitoSmistamentoDocumento> uoResult = null;
                    //uoResult = BusinessLogic.SmistamentoDocumenti.SmistamentoManager.SmistaDocumento(mittente, request.UserInfo.InfoUtente, doc, dtd, uo, uoInf, null).Cast<DocsPaVO.Smistamento.EsitoSmistamentoDocumento>().ToList();

                    // Gabriele Melini 03-10-2014
                    // il path da inserire nella mail dev'essere quello del frontend, non quello del mobile
                    // lo recupero dalla chiave di BE URL_PATH_IS
                    string path = string.Empty;
                    if (System.Configuration.ConfigurationManager.AppSettings["URL_PATH_IS"] != null)
                        path = System.Configuration.ConfigurationManager.AppSettings["URL_PATH_IS"].ToString();

                    //uoResult = BusinessLogic.SmistamentoDocumenti.SmistamentoManager.SmistaDocumento(mittente, infoUt, doc, dtd, uo, uoInf, null).Cast<DocsPaVO.Smistamento.EsitoSmistamentoDocumento>().ToList();
                    uoResult = BusinessLogic.SmistamentoDocumenti.SmistamentoManager.SmistaDocumento(mittente, infoUt, doc, dtd, uo, uoInf, path).Cast<DocsPaVO.Smistamento.EsitoSmistamentoDocumento>().ToList();
                    retvalSmistaDocumento.AddRange(uoResult);
                }
                // ---------- fine MEV SMISTAMENTO ----------


                if (retvalSmistaDocumento.Count > 0)
                    if (retvalSmistaDocumento.FirstOrDefault().CodiceEsitoSmistamento == -1)
                        res.Code = EseguiSmistamentoResponseCode.SYSTEM_ERROR;

                logger.Info("end: " + res.Code.ToString());
                return res;
            }
            catch (Exception e)
            {
                logger.Error("Exception: " + e);
                return EseguiSmistamentoResponse.ErrorResponse;
            }


        }


        //Vecchio codice dello smistamento.. confrontare e capire..
        
        //public static EseguiSmistamentoResponse eseguiSmistamentoOLD(EseguiSmistamentoRequest request)
        //{
        //    try
        //    {
        //        logger.Info("begin");
        //        EseguiSmistamentoResponse res = new EseguiSmistamentoResponse();
        //        MittenteSmistamento mittente = new MittenteSmistamento();
        //        mittente.IDAmministrazione = request.UserInfo.IdAmministrazione;
        //        mittente.IDCorrGlobaleRuolo = request.Ruolo.Id;
        //        mittente.IDGroup = request.Ruolo.IdGruppo;
        //        mittente.IDPeople = request.UserInfo.IdPeople;
        //        mittente.LivelloRuolo = request.Ruolo.Livello;

        //        foreach (RegistroInfo temp in request.Ruolo.Registri)
        //        {
        //            mittente.RegistriAppartenenza.Add(temp.SystemId);
        //        }
        //        DocumentoSmistamento doc = BusinessLogic.SmistamentoDocumenti.SmistamentoManager.GetDocumentoSmistamento(request.IdDocumento, request.UserInfo.InfoUtente, false);
        //        //id delle uo a cui si sta facendo smistamento
        //        List<string> idsUO = (from a in request.Elements select a.IdUO).Distinct().ToList();
        //        //tutte le UO che l'utente può vedere
        //        List<UOSmistamento> UOs = new List<UOSmistamento>();

        //        foreach (RegistroInfo temp in request.Ruolo.Registri)
        //        {
        //            UOs.AddRange(BusinessLogic.SmistamentoDocumenti.SmistamentoManager.GetListUOSmistamento(temp.SystemId, mittente).OfType<UOSmistamento>());
        //        }

        //        UOSmistamento uoAppartenenza = BusinessLogic.SmistamentoDocumenti.SmistamentoManager.GetUOAppartenenza(request.Ruolo.IdUO, mittente, true);


        //        //uo target dello smistamento
        //        List<UOSmistamento> targetUOs = (from a in UOs where idsUO.Contains(a.ID) select a).ToList();
        //        foreach (UOSmistamento temp in targetUOs)
        //        {
        //            List<EseguiSmistamentoElement> targetUOelements = (from a in request.Elements where a.IdUO.Equals(temp.ID) select a).ToList();
        //            fillUO(temp, targetUOelements);

        //        }

        //        List<DocsPaVO.Smistamento.EsitoSmistamentoDocumento> retvalSmistaDocumento = null;

        //        if (string.IsNullOrEmpty(request.NoteGenerali))
        //            request.NoteGenerali = String.Empty;

        //        //Dati trasmissione documento
        //        DatiTrasmissioneDocumento dtd = new DatiTrasmissioneDocumento();
        //        dtd.IDDocumento = request.IdDocumento;
        //        dtd.IDTrasmissione = request.IdTrasmissione;
        //        dtd.IDTrasmissioneUtente = request.IdTrasmissioneUtente;
        //        dtd.IDTrasmissioneSingola = request.IdTrasmissioneSingola;
        //        dtd.NoteGenerali = request.NoteGenerali;


        //        //Aggiunti 
        //        dtd.TrasmissioneConWorkflow = true;
        //        // targetUOs.FirstOrDefault().Selezionata = true;

        //        retvalSmistaDocumento = BusinessLogic.SmistamentoDocumenti.SmistamentoManager.SmistaDocumento(mittente, request.UserInfo.InfoUtente, doc, dtd, targetUOs.FirstOrDefault(), null, null).Cast<DocsPaVO.Smistamento.EsitoSmistamentoDocumento>().ToList();


        //        if (retvalSmistaDocumento.Count > 0)
        //            if (retvalSmistaDocumento.FirstOrDefault().CodiceEsitoSmistamento == -1)
        //                res.Code = EseguiSmistamentoResponseCode.SYSTEM_ERROR;

        //        logger.Info("end: " + res.Code.ToString());
        //        return res;
        //    }
        //    catch (Exception e)
        //    {
        //        logger.Error("Exception: " + e);
        //        return EseguiSmistamentoResponse.ErrorResponse;
        //    }
        //}
        

        private static void fillUO(UOSmistamento uo, List<EseguiSmistamentoElement> elements)
        {
            elements.ForEach(e => e.fillUOSmistamento(uo));
        }


        #endregion


        #region HSM Signature

        public static HSMSignResponse hsmSign(HSMSignRequest request)
        {
            try
            {
                logger.Info("begin");
                InfoUtente infoUtente = request.UserInfo.InfoUtente;
                infoUtente.idGruppo = request.IdGruppo;
                infoUtente.idCorrGlobali = request.IdCorrGlobali;
                HSMSignResponse res = new HSMSignResponse();
                DocsPaVO.documento.SchedaDocumento sd = null;
                FileRequest fr = null;
                // recupera il file request del doc
                sd = BusinessLogic.Documenti.DocManager.getDettaglio(infoUtente, request.IdDoc, request.IdDoc);
                fr = (FileRequest)sd.documenti[0];
                res.Code = ((BusinessLogic.Documenti.FileManager.HSM_Sign(infoUtente,
                                                                         fr,
                                                                         request.cofirma,
                                                                         request.timestamp,
                                                                         request.TipoFirma,
                                                                         request.AliasCertificato,
                                                                         request.DominioCertificato,
                                                                         request.OtpFirma,
                                                                         request.PinCertificato,
                                                                         request.ConvertPdf)
                                                                          ) ?
                                                                        HSMSignResponseCode.OK :
                                                                        HSMSignResponseCode.SYSTEM_ERROR);
                logger.Info("end: " + res.Code.ToString());
                return res;
            }
            catch (Exception e)
            {
                logger.Error("Exception: " + e);
                return HSMSignResponse.ErrorResponse;
            }
        }

        public static HSMSignResponse hsmRequestOTP(HSMSignRequest request)
        {
            try
            {
                logger.Info("begin");
                HSMSignResponse res = new HSMSignResponse();

                //aggiorna memento se diverso da quello in db
                //inizio
                InfoUtente infoUtente = request.UserInfo.InfoUtente;
                infoUtente.idGruppo = request.IdGruppo;
                infoUtente.idCorrGlobali = request.IdCorrGlobali;

                Memento infoMemento = new Memento();
                // recupera le info del memento
                string[] infoMementoResult = BusinessLogic.Utenti.UserManager.getMementoUtente(infoUtente);
                if (infoMementoResult.Length == 2)
                {
                    infoMemento.Dominio = infoMementoResult[0].ToUpper();
                    infoMemento.Alias = infoMementoResult[1].ToUpper();
                    if ((!infoMemento.Dominio.ToUpper().Equals(request.DominioCertificato) ||
                        (!infoMemento.Alias.ToUpper().Equals(request.AliasCertificato))))
                    {
                        BusinessLogic.Utenti.UserManager.setMementoUtente(infoUtente, request.DominioCertificato.ToUpper(), request.AliasCertificato.ToUpper());
                    }
                }
                if (infoMementoResult.Length == 1) //nessun memento definito
                { 
                    //update campo memento
                    BusinessLogic.Utenti.UserManager.setMementoUtente(infoUtente, request.DominioCertificato.ToUpper(), request.AliasCertificato.ToUpper());
                }

                //fine

                res.Code = (BusinessLogic.Documenti.FileManager.HSM_RequestOTP(request.AliasCertificato, request.DominioCertificato)) ?
                                                                        HSMSignResponseCode.OK :
                                                                        HSMSignResponseCode.SYSTEM_ERROR;
                logger.Info("end: " + res.Code.ToString());
                return res;

            }
            catch (Exception e)
            {
                logger.Error("Exception: " + e);
                return HSMSignResponse.ErrorResponse;
            }

        }

        public static HSMSignResponse hsmInfoSign(HSMSignRequest request)
        {
            try
            {
                logger.Info("begin");
                InfoUtente infoUtente = request.UserInfo.InfoUtente;
                infoUtente.idGruppo = request.IdGruppo;
                infoUtente.idCorrGlobali = request.IdCorrGlobali;
                HSMSignResponse res = new HSMSignResponse();

                InfoDocFirmato rootInfoDoc = new InfoDocFirmato();
                List<NodoFirma> listaNodoFirme = new List<NodoFirma>();

                DocsPaVO.documento.SchedaDocumento sd = null;

                FileRequest fr = null;
                // recupera il file request del doc
                sd = BusinessLogic.Documenti.DocManager.getDettaglio(infoUtente, request.IdDoc, request.IdDoc);
                fr = (FileRequest)sd.documenti[0];

                // Info documento 
                DocsPaVO.documento.FileDocumento signedDocument = BusinessLogic.Documenti.FileManager.getFile(fr, infoUtente);
                if (signedDocument != null && signedDocument.signatureResult != null)
                {
                    //Info Data Validità 
                    DateTime dtValiditaDocumento = GetDataRiferimentoValiditaDocumento(fr, infoUtente);
                    string dataValiditaDocumento = dtValiditaDocumento.ToString();
                    //Info verifica CRL 
                    string resultValiditaCRL = VerificaValiditaFirmaAllaData(fr, infoUtente, signedDocument, dtValiditaDocumento).ToString();

                    //info generali documento firmato
                    int documentIndex = 0;
                    bool isFirstItem = true;
                    //aggiunta nodo firme root 
                    NodoFirma rootFirma = new NodoFirma
                    {
                        Value = "",
                        Text = "",
                        DetailFirma = new InfoDetailFirma(),
                        ChildNodiFirma = new List<NodoFirma>()
                    };

                    List<NodoFirma> listPrec = new List<NodoFirma>();
                    foreach (PKCS7Document document in signedDocument.signatureResult.PKCS7Documents)
                    {
                        NodoFirma nodoFirma = GetNodesFirmeDigitali(document, documentIndex, null, signedDocument.signatureResult, dataValiditaDocumento,  resultValiditaCRL);
                        nodoFirma.DetailFirma = GetInfoDettaglioFirma(document);
                      //  nodoFirma.ChildNodiFirma = new List<NodoFirma>();
                        if (isFirstItem)
                        {
                            rootFirma.ChildNodiFirma.Add(nodoFirma);
                            isFirstItem = false;
                        }
                        else
                        {
                            listPrec.Add(nodoFirma);
                        }
                        listPrec = nodoFirma.ChildNodiFirma;
                        documentIndex++;
                    }
                    listaNodoFirme.Add(rootFirma);
                    rootInfoDoc.Firme = listaNodoFirme;
                    res.infoFirma = rootInfoDoc;
                    res.Code = HSMSignResponseCode.OK;
                }
                else
                {
                    res.Code = HSMSignResponseCode.KO;
                    throw new Exception("Il Documento non risulta firmato.");
                }
                logger.Info("end: " + res.Code.ToString());
                return res;
            }
            catch (Exception e)
            {
                logger.Error("Exception: " + e);
                return HSMSignResponse.ErrorResponse;
            }
        }
        private static NodoFirma GetNodesFirmeDigitali(PKCS7Document document, int documentIndex, string signerLevel, VerifySignatureResult signatureResult, string dataValiditaDocumento, string resultValiditaCRL)
        {
            NodoFirma nodoFirme = new NodoFirma();
            nodoFirme.Text = "Firme" + " (" + document.SignersInfo.Length + ")";
            nodoFirme.Value = string.Empty;
            List<NodoFirma> lstNodoFirmeChild = new List<NodoFirma>();
            int index = 0;
            foreach (SignerInfo info in document.SignersInfo)
            {
                NodoFirma signNode = new NodoFirma();
                List<NodoFirma> lstSignNodeChild = new List<NodoFirma>();

                signNode.Value = "sign&documentIndex=" + documentIndex.ToString() + "&index=" + index.ToString() + signerLevel;
                signNode.Text = (info.SubjectInfo.Cognome + " " + info.SubjectInfo.Nome + " " + document.SignatureType.ToString()).Trim();
                signNode.DetailFirma = GetInfoDettaglioFirma(info, document, signatureResult,  dataValiditaDocumento, resultValiditaCRL);
                if (info.SignatureTimeStampInfo != null)
                {
                    foreach (TSInfo ts in info.SignatureTimeStampInfo)
                    {
                        NodoFirma tsNode = new NodoFirma();
                        tsNode.Value = "timestamp&documentIndex=" + documentIndex.ToString() + "&index=" + index.ToString() + signerLevel;
                        string tsText = string.Empty;
                        tsNode.Text = tsText;
                        tsNode.DetailFirma = GetInfoDettaglioFirma(ts);
                        lstSignNodeChild.Add(tsNode);
                    }
                    signNode.ChildNodiFirma = lstSignNodeChild;
                }

                if (info.counterSignatures != null)
                {
                    int signLevels = 0;
                    foreach (SignerInfo csigner in info.counterSignatures)
                    {
                        List<SignerInfo> tmpLst = new List<SignerInfo>();
                        tmpLst.Add(csigner);
                        signerLevel = ":" + signLevels.ToString();

                        lstSignNodeChild.Add(GetNodesFirmeDigitali(tmpLst.ToArray(), documentIndex, signerLevel, document, signatureResult, dataValiditaDocumento,  resultValiditaCRL));
                        signLevels++;
                    }
                    signNode.ChildNodiFirma = lstSignNodeChild;
                }
                lstNodoFirmeChild.Add(signNode);
                index++;
            }
            nodoFirme.ChildNodiFirma = lstNodoFirmeChild;

            return nodoFirme;
        }
        private static NodoFirma GetNodesFirmeDigitali(SignerInfo[] signersInfo, int documentIndex, string signerLevel, PKCS7Document document, VerifySignatureResult signatureResult, string dataValiditaDocumento, string resultValiditaCRL)
        {
            NodoFirma rootNode = new NodoFirma();
            List<NodoFirma> lstNodoFirmeChild = new List<NodoFirma>();
            rootNode.Text = "Firme" + " (" + signersInfo.Length + ")";
            int index = 0;

            foreach (SignerInfo info in signersInfo)
            {
                NodoFirma signNode = new NodoFirma();
                List<NodoFirma> lstSignNodeChild = new List<NodoFirma>();
                signNode.Value = "sign&documentIndex=" + documentIndex.ToString() + "&index=" + index.ToString() + signerLevel;

                string nodeText = info.SubjectInfo.Cognome + " " + info.SubjectInfo.Nome;
                if (nodeText.Trim() == string.Empty) nodeText = "Non Presente";
                signNode.Text = nodeText;
                signNode.DetailFirma = GetInfoDettaglioFirma(info, document, signatureResult, dataValiditaDocumento, resultValiditaCRL);

                if (info.SignatureTimeStampInfo != null)
                {
                    foreach (TSInfo ts in info.SignatureTimeStampInfo)
                    {
                        NodoFirma tsNode = new NodoFirma();
                        tsNode.Value = "timestamp&documentIndex=" + documentIndex.ToString() + "&index=" + index.ToString() + signerLevel;
                        string tsText = "Marca Temporale";
                        tsNode.Text = tsText;
                        tsNode.DetailFirma = GetInfoDettaglioFirma(ts);
                        lstSignNodeChild.Add(tsNode);
                    }
                    signNode.ChildNodiFirma = lstSignNodeChild;
                }

                if (info.counterSignatures != null)
                {
                    int signLevels = 0;
                    foreach (SignerInfo csigner in info.counterSignatures)
                    {
                        List<SignerInfo> tmpLst = new List<SignerInfo>();
                        tmpLst.Add(csigner);
                        signerLevel = ":" + signLevels.ToString();
                        lstSignNodeChild.Add(GetNodesFirmeDigitali(tmpLst.ToArray(), documentIndex, signerLevel, document, signatureResult,dataValiditaDocumento,  resultValiditaCRL));
                        signLevels++;
                    }
                    signNode.ChildNodiFirma = lstSignNodeChild;
                }

                lstNodoFirmeChild.Add(signNode);
                signNode = null;

                index++;
            }
            rootNode.ChildNodiFirma = lstNodoFirmeChild;
            return rootNode;
        }
        private static InfoDetailFirma GetInfoDettaglioFirma(PKCS7Document document)
        {
            InfoDetailFirma infoDetailFirma = new InfoDetailFirma();
             
            //Nome File
            infoDetailFirma.NomeFile = document.DocumentFileName;
            if (!document.SignAlgorithm.Contains("PADES"))
            {
                infoDetailFirma.NomeFile = string.Format("{0}.P7M (Nome file P7M)", infoDetailFirma.NomeFile);
            }
            //Livello
            infoDetailFirma.Livello = document.Level.ToString();
            //Algoritmo di Firma documento
            infoDetailFirma.InfoFirmaAlgoritmo = document.SignAlgorithm;

            return infoDetailFirma;
        }
        private static InfoDetailFirma GetInfoDettaglioFirma(SignerInfo info, PKCS7Document document, VerifySignatureResult signatureResult, string dataValiditaDocumento, string  resultValiditaCRL)
        {
            InfoDetailFirma infoDetailFirma = new InfoDetailFirma();

            #region Dati Controllo

            //////Stato della firma
            ////if (signatureResult.StatusCode == 0)
            ////    infoDetailFirma.VerificaStatoFirma = "Valido";
            ////else if (signatureResult.StatusCode == -5)
            ////    infoDetailFirma.VerificaStatoFirma = "Non Conforme CADES (idaasigningcertificateV2 non trovato nella firma)";
            ////else
            ////    infoDetailFirma.VerificaStatoFirma = "Invalido";

            ////if (signatureResult.ErrorMessages != null)
            ////    foreach (string msg in signatureResult.ErrorMessages)
            ////        infoDetailFirma.VerificaStatoFirma += msg + "<br/>";
            
            string VerifiCRLResult = VerifyCRLReturnValue(info.CertificateInfo,dataValiditaDocumento);
            switch (VerifiCRLResult)
            {
                case "VerifyCRLNotValid":
                    infoDetailFirma.VerificaStatoFirma = "Non Valido";
                    break;
                case "VerifyCRLValid":
                    infoDetailFirma.VerificaStatoFirma = "Valido";
                    break;
                case "VerifyCRLExpired":
                    infoDetailFirma.VerificaStatoFirma = "Verifica firma scaduta";
                    break;
                case "VerifyCRLRevoked":
                    infoDetailFirma.VerificaStatoFirma = "Verifica CRL revocata";
                    break;
            }

            //Stato del certificato
            infoDetailFirma.VerificaStatoCertificato = infoDetailFirma.VerificaStatoFirma; // info.CertificateInfo.RevocationStatusDescription;
            
            //Data Verifica Validità
            infoDetailFirma.VerificaDataValiditaDocumento = dataValiditaDocumento;
            //Verifica CRL
            infoDetailFirma.VerificaCRL = resultValiditaCRL;

            #endregion

            #region  Dati certificato

            CertificateInfo certInfo = info.CertificateInfo;
            //Ente Certificatore
            infoDetailFirma.CertificatoEnte = getEnteCertificatore(certInfo.IssuerName);
            //SN Certificato
            infoDetailFirma.CertificatoSN = certInfo.SerialNumber;
            //Data Validità
            infoDetailFirma.CertificatoValidoDal = certInfo.ValidFromDate.ToLongDateString();
            infoDetailFirma.CertificatoValidoAl = certInfo.ValidToDate.ToLongDateString();
            //Algoritmo Firma certificato
            infoDetailFirma.CertificatoAlgoritmo = certInfo.SignatureAlgorithm;
            //Firmatario
            infoDetailFirma.CertificatoFirmatario = string.Format("{0} {1}", info.SubjectInfo.Cognome, info.SubjectInfo.Nome);
            // Thumbprint certificato
            infoDetailFirma.CertificatoThumbprint = certInfo.ThumbPrint;

            #endregion

            #region Dati soggetto

            //Nome
            infoDetailFirma.SoggettoNome = info.SubjectInfo.Nome;
            //Cognome
            infoDetailFirma.SoggettoCognome = info.SubjectInfo.Cognome;
            //CodiceFiscale
            infoDetailFirma.SoggettoCodiceFiscale = info.SubjectInfo.CodiceFiscale;
            //Data Di Nascita
            infoDetailFirma.SoggettoDataNascita = info.SubjectInfo.DataDiNascita;
            //Organizzazione
            infoDetailFirma.SoggettoOrganizzazione = info.SubjectInfo.Organizzazione;
            //Ruolo
            infoDetailFirma.SoggettoRuolo = info.SubjectInfo.Ruolo;
            //Paese
            infoDetailFirma.SoggettoPaese = info.SubjectInfo.Country;
            //Id Titolare
            infoDetailFirma.SoggettoIdTitolare = info.SubjectInfo.CertId;

            #endregion

            #region Dati relativi all'algoritmo e firma digitale
            //Algoritmo di firma
            infoDetailFirma.InfoFirmaAlgoritmo = info.SignatureAlgorithm;
            //Impronta
            infoDetailFirma.InfoFirmaImpronta = document.SignHash;
            //Controfirmatario
            infoDetailFirma.InfoFirmaControfirmatario = info.isCountersigner ? "Vero" : "Falso";
            //Data di Firma
            if (info.SigningTime != DateTime.MinValue)
            {
                infoDetailFirma.InfoFirmaData = info.SigningTime.ToLocalTime().ToString();
            }

            #endregion

            return infoDetailFirma;
        }

        private static string VerifyCRLReturnValue(CertificateInfo certInfo, string dateVerificaCRL)
        {
            if ((certInfo.RevocationDate != null) && (certInfo.ValidToDate != null))
            {
                string label = string.Empty;
                DateTime date = Convert.ToDateTime(dateVerificaCRL);
                //SE LA DATA INSERITA è PRECEDENTE ALLA DATA DI INIZIO VALIDITà
                if (!verificaIntervalloDate(date.ToString(), certInfo.ValidFromDate.ToString()))
                {
                    label = "VerifyCRLNotValid";
                }
                else
                {
                    //SE IL CERTIFICATO HA UNA DATA DI REVOCA
                    if (!certInfo.RevocationDate.Equals(DateTime.MinValue))
                    {
                        if (verificaIntervalloDate(certInfo.RevocationDate.ToString(), date.ToString()))
                        {
                            label = verificaIntervalloDate(certInfo.ValidToDate.ToString(), date.ToString()) ? "VerifyCRLValid" : "VerifyCRLExpired";
                        }
                        else
                        {
                            label = verificaIntervalloDate(certInfo.ValidToDate.ToString(), date.ToString()) ? "VerifyCRLRevoked" : "VerifyCRLExpired";
                        }
                    }
                    else
                    {
                        label = verificaIntervalloDate(certInfo.ValidToDate.ToString(), date.ToString()) ? "VerifyCRLValid" : "VerifyCRLExpired";
                    }
                }
                return  label;
            }
            else
            {
                return  "VerifyCLRko";
            }
        }

        public static bool verificaIntervalloDate(string dataUno, string dataDue)
        {
            if (dataUno != "" && dataDue != "")
            {
                try
                {
                    dataUno = dataUno.Trim();
                    dataDue = dataDue.Trim();
                    CultureInfo ci = new CultureInfo("it-IT");
                    string[] formati = { "dd/MM/yyyy HH.mm.ss", "dd/MM/yyyy H.mm.ss", "dd/MM/yyyy HH:mm:ss", "dd/MM/yyyy H:mm:ss", "dd/MM/yyyy" };
                    DateTime d_Uno = DateTime.ParseExact(dataUno, formati, ci.DateTimeFormat, DateTimeStyles.AllowWhiteSpaces);
                    DateTime d_Due = DateTime.ParseExact(dataDue, formati, ci.DateTimeFormat, DateTimeStyles.AllowWhiteSpaces);

                    if (d_Uno >= d_Due)
                        return true;
                    else
                        return false;
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message);
                }
            }
            else
            {
                return false;
            }
        }
        private static InfoDetailFirma GetInfoDettaglioFirma(TSInfo tsfInfo)
        {
            InfoDetailFirma infoDetailFirma = new InfoDetailFirma();
            infoDetailFirma.TimeStampInfo = tsfInfo;
            return infoDetailFirma;

        }
        private static string getEnteCertificatore(string issuerName)
        {
            string enteCertCN = string.Empty;
            string enteCertO = string.Empty;
            string enteCertC = string.Empty;
            if (string.IsNullOrEmpty(issuerName)) return string.Empty; //no issuesName
            string[] issuerNamePars = issuerName.Split(',');
            // recupera CN
            if (issuerName.Contains("CN="))
                enteCertCN = issuerNamePars.Where(a => a.Contains("CN=")).SingleOrDefault().Split('=')[1];
            // recupera O
            if (issuerName.Contains("O="))
                enteCertO = string.Format("{0}{1}", (string.IsNullOrEmpty(enteCertCN) ? string.Empty : ", "), issuerNamePars.Where(a => a.Contains("O=")).SingleOrDefault().Split('=')[1]);
            if (issuerName.Contains("C="))
                enteCertC = string.Format(", {0}", issuerNamePars.Where(a => a.Contains("C=")).SingleOrDefault().Split('=')[1]);
            return string.Format("{0}{1}{2}", enteCertCN, enteCertO, enteCertC);
        }
        private static DateTime GetDataRiferimentoValiditaDocumento(FileRequest fr, InfoUtente iu)
        {
            return BusinessLogic.Documenti.FileManager.dataRiferimentoValitaDocumento(fr, iu);
        }

        private static DocsPaVO.Mobile.InfoDetailFirma.StatusCode VerificaValiditaFirmaAllaData(FileRequest fr, InfoUtente iu, DocsPaVO.documento.FileDocumento fileDocumento, DateTime dataRif)
        {
            DocsPaVO.Mobile.InfoDetailFirma.StatusCode statusCode = InfoDetailFirma.StatusCode.NOT_DEFINED;
            string risultato = "Non Definito";
            try
            {
                BusinessLogic.Documenti.FileManager.processFileInformationCRLUpdate(fr, iu, fileDocumento,dataRif);
                DocsPaVO.Logger.CodAzione.Esito esito = DocsPaVO.Logger.CodAzione.Esito.OK;
                if (fileDocumento.signatureResult.StatusCode == -100)
                {
                    statusCode  = InfoDetailFirma.StatusCode.ERROR_SERVICE_CRL;
                    esito = DocsPaVO.Logger.CodAzione.Esito.KO;
                    risultato = "Errore di connessione al servizio CRL";
                }

                if (fileDocumento.signatureResult.StatusCode != 0)
                {
                    statusCode = InfoDetailFirma.StatusCode.ERROR;
                    risultato = "Verifica con errori "; // +BusinessLogic.Documenti.FileManager.getExtCheckErrors(fileDocumento);
                }
                if (fileDocumento.signatureResult.StatusCode == 0)
                {
                    statusCode = InfoDetailFirma.StatusCode.VERIFY_OK;
                    risultato = "Verifica OK";
                }

                bool revokedPresent = BusinessLogic.Documenti.FileManager.revokedCertArePresent(fileDocumento);
                if (revokedPresent)
                {
                    statusCode = InfoDetailFirma.StatusCode.REVOKED_CERT;
                    risultato = "Sono presenti Certificati Revocati";
                }

                BusinessLogic.UserLog.UserLog.WriteLog(iu, "DOCUMENTOVERIFICACRL", fr.docNumber, string.Format("{0} {1} {2} {3} {4}  Esito:{5}", "Verifica CRL alla data", dataRif.ToLongDateString(), fr.docNumber, "Ver.", fr.version, risultato), esito);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: VerificaValiditaFirma", e);
            }
            return statusCode;
        }

        public static HSMSignResponse hsmVerifySign(HSMSignRequest request)
        {
            try
            {
                logger.Info("begin");
                InfoUtente infoUtente = request.UserInfo.InfoUtente;
                infoUtente.idGruppo = request.IdGruppo;
                infoUtente.idCorrGlobali = request.IdCorrGlobali;
                HSMSignResponse res = new HSMSignResponse();
                DocsPaVO.documento.SchedaDocumento sd = null;
                FileRequest fr = null;
                // recupera il file request del doc
                sd = BusinessLogic.Documenti.DocManager.getDettaglio(infoUtente, request.IdDoc, request.IdDoc);
                fr = (FileRequest)sd.documenti[0];
                DocsPaVO.documento.FileDocumento signedDocument = BusinessLogic.Documenti.FileManager.getFile(fr, infoUtente);
                res.Code = (signedDocument != null && signedDocument.signatureResult != null) ? HSMSignResponseCode.OK : HSMSignResponseCode.KO;
                logger.Info("end: " + res.Code.ToString());
                return res;
            }
            catch (Exception e)
            {
                logger.Error("Exception: " + e);
                return HSMSignResponse.ErrorResponse;
            }
        }

        public static HSMSignResponse hsmGetMementoForUser(HSMSignRequest request)
        {
            try
            {
                logger.Info("begin");
                InfoUtente infoUtente = request.UserInfo.InfoUtente;
                infoUtente.idGruppo = request.IdGruppo;
                infoUtente.idCorrGlobali = request.IdCorrGlobali;
                HSMSignResponse res = new HSMSignResponse();
                Memento infoMemento = new Memento();
                // recupera le info del memento
                string[] infoMementoResult =  BusinessLogic.Utenti.UserManager.getMementoUtente(infoUtente);
                if (infoMementoResult.Length == 2)
                {
                    infoMemento.Dominio =   infoMementoResult[0].ToUpper();
                    infoMemento.Alias =  infoMementoResult[1].ToUpper();
                }
                res.memento = infoMemento;
                res.Code = HSMSignResponseCode.OK;
                logger.Info("end: " + res.Code.ToString());
                return res;
            }
            catch (Exception e)
            {
                logger.Error("Exception: " + e);
                return HSMSignResponse.ErrorResponse;
            }
        }

        /// <summary>
        /// Verifica se il ruolo è abilitato alla funzione di richiesta OTP
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public static bool isAllowedOTP(RuoloInfo info)
        {
            bool result = false;
            try
            {
                Ruolo role = BusinessLogic.Utenti.UserManager.getRuolo(info.Id);
                if (role != null)
                {
                    result = (from function in (Funzione[])role.funzioni.ToArray(typeof(Funzione)) where function.codice.ToUpper().Equals("TO_GET_OTP") select function.systemId).Count() > 0 ? true : false;
                }
            }
            catch (Exception ex)
            {
                logger.Error("Exception: " + ex);
            }
            return result;

        }

        #endregion




        //Faillace 12_12_2012 codice ADL commentato
        //Era l'implmentazione per ricavare l'ADL mettendo dei filtri statici
        //è stata invece integrata nelle search strategies
        #region ADL
        #region CODICE ADL COMMENTATO
        /*
        //Creo i filtri di ricerca per i fascicoli
        private static FiltroRicerca[] creaFiltriADLFasc(InfoUtente infoUt, Ruolo ruolo, List<OrgTitolario> titolari)
        {
            List<FiltroRicerca> frList = new List<FiltroRicerca>();

            string listaTitolari = string.Empty;
            foreach (OrgTitolario tit in titolari)
                listaTitolari += "," + tit.ID.ToString();

            listaTitolari = listaTitolari.Substring(1);

            frList.Add(new FiltroRicerca
            {
                argomento = "INCLUDI_FASCICOLI_FIGLI",
                valore = "N",
                searchTextOptions = SearchTextOptionsEnum.WholeWord,
                listaFiltriTrasmissione = DocsPaVO.filtri.trasmissione.listaArgomenti.NOTE_GENERALI,
                listaFiltriModelliTrasmissione = DocsPaVO.filtri.trasmissione.listaArgomentiModelliTrasmissione.CODICE_MODELLO,
                listaFiltriTrasmissioneNascosti = DocsPaVO.filtri.trasmissione.listaArgomentiNascosti.TIPO_OGGETTO,
                listaFiltriDocumento = DocsPaVO.filtri.ricerca.listaArgomenti.TIPO,
                listaFiltriFascicolo = DocsPaVO.filtri.fascicolazione.listaArgomenti.APERTURA_IL,
                listaFiltriStampaRegistro = DocsPaVO.filtri.stampaRegistro.listaArgomenti.REGISTRO
            });


            frList.Add(new FiltroRicerca
            {
                argomento = "ID_TITOLARIO",
                valore = listaTitolari,// "7067503,96163",
                searchTextOptions = SearchTextOptionsEnum.WholeWord,
                listaFiltriTrasmissione = DocsPaVO.filtri.trasmissione.listaArgomenti.NOTE_GENERALI,
                listaFiltriModelliTrasmissione = DocsPaVO.filtri.trasmissione.listaArgomentiModelliTrasmissione.CODICE_MODELLO,
                listaFiltriTrasmissioneNascosti = DocsPaVO.filtri.trasmissione.listaArgomentiNascosti.TIPO_OGGETTO,
                listaFiltriDocumento = DocsPaVO.filtri.ricerca.listaArgomenti.TIPO,
                listaFiltriFascicolo = DocsPaVO.filtri.fascicolazione.listaArgomenti.APERTURA_IL,
                listaFiltriStampaRegistro = DocsPaVO.filtri.stampaRegistro.listaArgomenti.REGISTRO
            });

            frList.Add(new FiltroRicerca
            {
                argomento = "DOC_IN_FASC_ADL",
                valore = String.Format("{0}@{1}", infoUt.idPeople, ruolo.systemId),
                searchTextOptions = SearchTextOptionsEnum.WholeWord,
                listaFiltriTrasmissione = DocsPaVO.filtri.trasmissione.listaArgomenti.NOTE_GENERALI,
                listaFiltriModelliTrasmissione = DocsPaVO.filtri.trasmissione.listaArgomentiModelliTrasmissione.CODICE_MODELLO,
                listaFiltriTrasmissioneNascosti = DocsPaVO.filtri.trasmissione.listaArgomentiNascosti.TIPO_OGGETTO,
                listaFiltriDocumento = DocsPaVO.filtri.ricerca.listaArgomenti.TIPO,
                listaFiltriFascicolo = DocsPaVO.filtri.fascicolazione.listaArgomenti.APERTURA_IL,
                listaFiltriStampaRegistro = DocsPaVO.filtri.stampaRegistro.listaArgomenti.REGISTRO
            });

            frList.Add(new FiltroRicerca
            {
                argomento = "SOTTOFASCICOLO",
                valore = "",
                searchTextOptions = SearchTextOptionsEnum.WholeWord,
                listaFiltriTrasmissione = DocsPaVO.filtri.trasmissione.listaArgomenti.NOTE_GENERALI,
                listaFiltriModelliTrasmissione = DocsPaVO.filtri.trasmissione.listaArgomentiModelliTrasmissione.CODICE_MODELLO,
                listaFiltriTrasmissioneNascosti = DocsPaVO.filtri.trasmissione.listaArgomentiNascosti.TIPO_OGGETTO,
                listaFiltriDocumento = DocsPaVO.filtri.ricerca.listaArgomenti.TIPO,
                listaFiltriFascicolo = DocsPaVO.filtri.fascicolazione.listaArgomenti.APERTURA_IL,
                listaFiltriStampaRegistro = DocsPaVO.filtri.stampaRegistro.listaArgomenti.REGISTRO
            });

            frList.Add(new FiltroRicerca
            {
                argomento = "EXTEND_TO_HISTORICIZED_OWNER",
                valore = "False",
                searchTextOptions = SearchTextOptionsEnum.WholeWord,
                listaFiltriTrasmissione = DocsPaVO.filtri.trasmissione.listaArgomenti.NOTE_GENERALI,
                listaFiltriModelliTrasmissione = DocsPaVO.filtri.trasmissione.listaArgomentiModelliTrasmissione.CODICE_MODELLO,
                listaFiltriTrasmissioneNascosti = DocsPaVO.filtri.trasmissione.listaArgomentiNascosti.TIPO_OGGETTO,
                listaFiltriDocumento = DocsPaVO.filtri.ricerca.listaArgomenti.TIPO,
                listaFiltriFascicolo = DocsPaVO.filtri.fascicolazione.listaArgomenti.APERTURA_IL,
                listaFiltriStampaRegistro = DocsPaVO.filtri.stampaRegistro.listaArgomenti.REGISTRO
            });

            frList.Add(new FiltroRicerca
            {
                argomento = "EXTEND_TO_HISTORICIZED_AUTHOR",
                valore = "False",
                searchTextOptions = SearchTextOptionsEnum.WholeWord,
                listaFiltriTrasmissione = DocsPaVO.filtri.trasmissione.listaArgomenti.NOTE_GENERALI,
                listaFiltriModelliTrasmissione = DocsPaVO.filtri.trasmissione.listaArgomentiModelliTrasmissione.CODICE_MODELLO,
                listaFiltriTrasmissioneNascosti = DocsPaVO.filtri.trasmissione.listaArgomentiNascosti.TIPO_OGGETTO,
                listaFiltriDocumento = DocsPaVO.filtri.ricerca.listaArgomenti.TIPO,
                listaFiltriFascicolo = DocsPaVO.filtri.fascicolazione.listaArgomenti.APERTURA_IL,
                listaFiltriStampaRegistro = DocsPaVO.filtri.stampaRegistro.listaArgomenti.REGISTRO
            });

            frList.Add(new FiltroRicerca
            {
                argomento = "CORR_TYPE_OWNER",
                valore = "R",
                searchTextOptions = SearchTextOptionsEnum.WholeWord,
                listaFiltriTrasmissione = DocsPaVO.filtri.trasmissione.listaArgomenti.NOTE_GENERALI,
                listaFiltriModelliTrasmissione = DocsPaVO.filtri.trasmissione.listaArgomentiModelliTrasmissione.CODICE_MODELLO,
                listaFiltriTrasmissioneNascosti = DocsPaVO.filtri.trasmissione.listaArgomentiNascosti.TIPO_OGGETTO,
                listaFiltriDocumento = DocsPaVO.filtri.ricerca.listaArgomenti.TIPO,
                listaFiltriFascicolo = DocsPaVO.filtri.fascicolazione.listaArgomenti.APERTURA_IL,
                listaFiltriStampaRegistro = DocsPaVO.filtri.stampaRegistro.listaArgomenti.REGISTRO
            });

            frList.Add(new FiltroRicerca
            {
                argomento = "CORR_TYPE_AUTHOR",
                valore = "R",
                searchTextOptions = SearchTextOptionsEnum.WholeWord,
                listaFiltriTrasmissione = DocsPaVO.filtri.trasmissione.listaArgomenti.NOTE_GENERALI,
                listaFiltriModelliTrasmissione = DocsPaVO.filtri.trasmissione.listaArgomentiModelliTrasmissione.CODICE_MODELLO,
                listaFiltriTrasmissioneNascosti = DocsPaVO.filtri.trasmissione.listaArgomentiNascosti.TIPO_OGGETTO,
                listaFiltriDocumento = DocsPaVO.filtri.ricerca.listaArgomenti.TIPO,
                listaFiltriFascicolo = DocsPaVO.filtri.fascicolazione.listaArgomenti.APERTURA_IL,
                listaFiltriStampaRegistro = DocsPaVO.filtri.stampaRegistro.listaArgomenti.REGISTRO
            });

            frList.Add(new FiltroRicerca
            {
                argomento = "ORACLE_FIELD_FOR_ORDER",
                valore = "A.DTA_CREAZIONE",
                searchTextOptions = SearchTextOptionsEnum.WholeWord,
                listaFiltriTrasmissione = DocsPaVO.filtri.trasmissione.listaArgomenti.NOTE_GENERALI,
                listaFiltriModelliTrasmissione = DocsPaVO.filtri.trasmissione.listaArgomentiModelliTrasmissione.CODICE_MODELLO,
                listaFiltriTrasmissioneNascosti = DocsPaVO.filtri.trasmissione.listaArgomentiNascosti.TIPO_OGGETTO,
                listaFiltriDocumento = DocsPaVO.filtri.ricerca.listaArgomenti.TIPO,
                listaFiltriFascicolo = DocsPaVO.filtri.fascicolazione.listaArgomenti.APERTURA_IL,
                listaFiltriStampaRegistro = DocsPaVO.filtri.stampaRegistro.listaArgomenti.REGISTRO,
                nomeCampo = "P20"
            });

            frList.Add(new FiltroRicerca
            {
                argomento = "SQL_FIELD_FOR_ORDER",
                valore = "A.DTA_CREAZIONE",
                searchTextOptions = SearchTextOptionsEnum.WholeWord,
                listaFiltriTrasmissione = DocsPaVO.filtri.trasmissione.listaArgomenti.NOTE_GENERALI,
                listaFiltriModelliTrasmissione = DocsPaVO.filtri.trasmissione.listaArgomentiModelliTrasmissione.CODICE_MODELLO,
                listaFiltriTrasmissioneNascosti = DocsPaVO.filtri.trasmissione.listaArgomentiNascosti.TIPO_OGGETTO,
                listaFiltriDocumento = DocsPaVO.filtri.ricerca.listaArgomenti.TIPO,
                listaFiltriFascicolo = DocsPaVO.filtri.fascicolazione.listaArgomenti.APERTURA_IL,
                listaFiltriStampaRegistro = DocsPaVO.filtri.stampaRegistro.listaArgomenti.REGISTRO,
                nomeCampo = "P20"
            });

            frList.Add(new FiltroRicerca
            {
                argomento = "ORDER_DIRECTION",
                valore = "DESC",
                searchTextOptions = SearchTextOptionsEnum.WholeWord,
                listaFiltriTrasmissione = DocsPaVO.filtri.trasmissione.listaArgomenti.NOTE_GENERALI,
                listaFiltriModelliTrasmissione = DocsPaVO.filtri.trasmissione.listaArgomentiModelliTrasmissione.CODICE_MODELLO,
                listaFiltriTrasmissioneNascosti = DocsPaVO.filtri.trasmissione.listaArgomentiNascosti.TIPO_OGGETTO,
                listaFiltriDocumento = DocsPaVO.filtri.ricerca.listaArgomenti.TIPO,
                listaFiltriFascicolo = DocsPaVO.filtri.fascicolazione.listaArgomenti.APERTURA_IL,
                listaFiltriStampaRegistro = DocsPaVO.filtri.stampaRegistro.listaArgomenti.REGISTRO,
                nomeCampo = "P20"
            });


            return frList.ToArray();
        }


        //Creo i filtri di ricerca per i documenti
        private static FiltroRicerca[] creaFiltriADLDoc(InfoUtente infoUt, Ruolo ruolo)
        {

            string listaRegistri = string.Empty;
            foreach (DocsPaVO.utente.Registro reg in ruolo.registri)
            {
                if (!reg.flag_pregresso)
                {
                    listaRegistri += "," + reg.systemId.ToString();
                }

            }

            listaRegistri = listaRegistri.Substring(1);

            List<FiltroRicerca> frList = new List<FiltroRicerca>();
            frList.Add(new FiltroRicerca
            {
                argomento = "PROT_ARRIVO",
                valore = "true",
                searchTextOptions = SearchTextOptionsEnum.WholeWord,
                listaFiltriTrasmissione = DocsPaVO.filtri.trasmissione.listaArgomenti.NOTE_GENERALI,
                listaFiltriModelliTrasmissione = DocsPaVO.filtri.trasmissione.listaArgomentiModelliTrasmissione.CODICE_MODELLO,
                listaFiltriTrasmissioneNascosti = DocsPaVO.filtri.trasmissione.listaArgomentiNascosti.TIPO_OGGETTO,
                listaFiltriDocumento = DocsPaVO.filtri.ricerca.listaArgomenti.TIPO,
                listaFiltriFascicolo = DocsPaVO.filtri.fascicolazione.listaArgomenti.APERTURA_IL,
                listaFiltriStampaRegistro = DocsPaVO.filtri.stampaRegistro.listaArgomenti.REGISTRO
            });


            frList.Add(new FiltroRicerca
            {
                argomento = "PROT_PARTENZA",
                valore = "true",
                searchTextOptions = SearchTextOptionsEnum.WholeWord,
                listaFiltriTrasmissione = DocsPaVO.filtri.trasmissione.listaArgomenti.NOTE_GENERALI,
                listaFiltriModelliTrasmissione = DocsPaVO.filtri.trasmissione.listaArgomentiModelliTrasmissione.CODICE_MODELLO,
                listaFiltriTrasmissioneNascosti = DocsPaVO.filtri.trasmissione.listaArgomentiNascosti.TIPO_OGGETTO,
                listaFiltriDocumento = DocsPaVO.filtri.ricerca.listaArgomenti.TIPO,
                listaFiltriFascicolo = DocsPaVO.filtri.fascicolazione.listaArgomenti.APERTURA_IL,
                listaFiltriStampaRegistro = DocsPaVO.filtri.stampaRegistro.listaArgomenti.REGISTRO
            });

            frList.Add(new FiltroRicerca
            {
                argomento = "PROT_INTERNO",
                valore = "true",
                searchTextOptions = SearchTextOptionsEnum.WholeWord,
                listaFiltriTrasmissione = DocsPaVO.filtri.trasmissione.listaArgomenti.NOTE_GENERALI,
                listaFiltriModelliTrasmissione = DocsPaVO.filtri.trasmissione.listaArgomentiModelliTrasmissione.CODICE_MODELLO,
                listaFiltriTrasmissioneNascosti = DocsPaVO.filtri.trasmissione.listaArgomentiNascosti.TIPO_OGGETTO,
                listaFiltriDocumento = DocsPaVO.filtri.ricerca.listaArgomenti.TIPO,
                listaFiltriFascicolo = DocsPaVO.filtri.fascicolazione.listaArgomenti.APERTURA_IL,
                listaFiltriStampaRegistro = DocsPaVO.filtri.stampaRegistro.listaArgomenti.REGISTRO
            });

            frList.Add(new FiltroRicerca
            {
                argomento = "STAMPA_REG",
                valore = "false",
                searchTextOptions = SearchTextOptionsEnum.WholeWord,
                listaFiltriTrasmissione = DocsPaVO.filtri.trasmissione.listaArgomenti.NOTE_GENERALI,
                listaFiltriModelliTrasmissione = DocsPaVO.filtri.trasmissione.listaArgomentiModelliTrasmissione.CODICE_MODELLO,
                listaFiltriTrasmissioneNascosti = DocsPaVO.filtri.trasmissione.listaArgomentiNascosti.TIPO_OGGETTO,
                listaFiltriDocumento = DocsPaVO.filtri.ricerca.listaArgomenti.TIPO,
                listaFiltriFascicolo = DocsPaVO.filtri.fascicolazione.listaArgomenti.APERTURA_IL,
                listaFiltriStampaRegistro = DocsPaVO.filtri.stampaRegistro.listaArgomenti.REGISTRO
            });

            frList.Add(new FiltroRicerca
            {
                argomento = "GRIGIO",
                valore = "true",
                searchTextOptions = SearchTextOptionsEnum.WholeWord,
                listaFiltriTrasmissione = DocsPaVO.filtri.trasmissione.listaArgomenti.NOTE_GENERALI,
                listaFiltriModelliTrasmissione = DocsPaVO.filtri.trasmissione.listaArgomentiModelliTrasmissione.CODICE_MODELLO,
                listaFiltriTrasmissioneNascosti = DocsPaVO.filtri.trasmissione.listaArgomentiNascosti.TIPO_OGGETTO,
                listaFiltriDocumento = DocsPaVO.filtri.ricerca.listaArgomenti.TIPO,
                listaFiltriFascicolo = DocsPaVO.filtri.fascicolazione.listaArgomenti.APERTURA_IL,
                listaFiltriStampaRegistro = DocsPaVO.filtri.stampaRegistro.listaArgomenti.REGISTRO
            });

            frList.Add(new FiltroRicerca
            {
                argomento = "PREDISPOSTO",
                valore = "false",
                searchTextOptions = SearchTextOptionsEnum.WholeWord,
                listaFiltriTrasmissione = DocsPaVO.filtri.trasmissione.listaArgomenti.NOTE_GENERALI,
                listaFiltriModelliTrasmissione = DocsPaVO.filtri.trasmissione.listaArgomentiModelliTrasmissione.CODICE_MODELLO,
                listaFiltriTrasmissioneNascosti = DocsPaVO.filtri.trasmissione.listaArgomentiNascosti.TIPO_OGGETTO,
                listaFiltriDocumento = DocsPaVO.filtri.ricerca.listaArgomenti.TIPO,
                listaFiltriFascicolo = DocsPaVO.filtri.fascicolazione.listaArgomenti.APERTURA_IL,
                listaFiltriStampaRegistro = DocsPaVO.filtri.stampaRegistro.listaArgomenti.REGISTRO
            });


            frList.Add(new FiltroRicerca
            {
                argomento = "TIPO",
                valore = "tipo",
                searchTextOptions = SearchTextOptionsEnum.WholeWord,
                listaFiltriTrasmissione = DocsPaVO.filtri.trasmissione.listaArgomenti.NOTE_GENERALI,
                listaFiltriModelliTrasmissione = DocsPaVO.filtri.trasmissione.listaArgomentiModelliTrasmissione.CODICE_MODELLO,
                listaFiltriTrasmissioneNascosti = DocsPaVO.filtri.trasmissione.listaArgomentiNascosti.TIPO_OGGETTO,
                listaFiltriDocumento = DocsPaVO.filtri.ricerca.listaArgomenti.TIPO,
                listaFiltriFascicolo = DocsPaVO.filtri.fascicolazione.listaArgomenti.APERTURA_IL,
                listaFiltriStampaRegistro = DocsPaVO.filtri.stampaRegistro.listaArgomenti.REGISTRO
            });


            //QUI!!!!
            frList.Add(new FiltroRicerca
            {
                argomento = DocsPaVO.filtri.ricerca.listaArgomenti.REGISTRO.ToString(),
                valore = listaRegistri, // "86107,8548943",
                searchTextOptions = SearchTextOptionsEnum.WholeWord,
                listaFiltriTrasmissione = DocsPaVO.filtri.trasmissione.listaArgomenti.NOTE_GENERALI,
                listaFiltriModelliTrasmissione = DocsPaVO.filtri.trasmissione.listaArgomentiModelliTrasmissione.CODICE_MODELLO,
                listaFiltriTrasmissioneNascosti = DocsPaVO.filtri.trasmissione.listaArgomentiNascosti.TIPO_OGGETTO,
                listaFiltriDocumento = DocsPaVO.filtri.ricerca.listaArgomenti.TIPO,
                listaFiltriFascicolo = DocsPaVO.filtri.fascicolazione.listaArgomenti.APERTURA_IL,
                listaFiltriStampaRegistro = DocsPaVO.filtri.stampaRegistro.listaArgomenti.REGISTRO
            });


            //"90249@347718",
            frList.Add(new FiltroRicerca
            {
                argomento = DocsPaVO.filtri.ricerca.listaArgomenti.DOC_IN_ADL.ToString(),
                valore = String.Format("{0}@{1}", infoUt.idPeople, ruolo.systemId),
                searchTextOptions = SearchTextOptionsEnum.WholeWord,
                listaFiltriTrasmissione = DocsPaVO.filtri.trasmissione.listaArgomenti.NOTE_GENERALI,
                listaFiltriModelliTrasmissione = DocsPaVO.filtri.trasmissione.listaArgomentiModelliTrasmissione.CODICE_MODELLO,
                listaFiltriTrasmissioneNascosti = DocsPaVO.filtri.trasmissione.listaArgomentiNascosti.TIPO_OGGETTO,
                listaFiltriDocumento = DocsPaVO.filtri.ricerca.listaArgomenti.TIPO,
                listaFiltriFascicolo = DocsPaVO.filtri.fascicolazione.listaArgomenti.APERTURA_IL,
                listaFiltriStampaRegistro = DocsPaVO.filtri.stampaRegistro.listaArgomenti.REGISTRO
            });


            frList.Add(new FiltroRicerca
            {
                argomento = "EXTEND_TO_HISTORICIZED_OWNER",
                valore = "false",
                searchTextOptions = SearchTextOptionsEnum.WholeWord,
                listaFiltriTrasmissione = DocsPaVO.filtri.trasmissione.listaArgomenti.NOTE_GENERALI,
                listaFiltriModelliTrasmissione = DocsPaVO.filtri.trasmissione.listaArgomentiModelliTrasmissione.CODICE_MODELLO,
                listaFiltriTrasmissioneNascosti = DocsPaVO.filtri.trasmissione.listaArgomentiNascosti.TIPO_OGGETTO,
                listaFiltriDocumento = DocsPaVO.filtri.ricerca.listaArgomenti.TIPO,
                listaFiltriFascicolo = DocsPaVO.filtri.fascicolazione.listaArgomenti.APERTURA_IL,
                listaFiltriStampaRegistro = DocsPaVO.filtri.stampaRegistro.listaArgomenti.REGISTRO
            });

            frList.Add(new FiltroRicerca
            {
                argomento = "EXTEND_TO_HISTORICIZED_AUTHOR",
                valore = "false",
                searchTextOptions = SearchTextOptionsEnum.WholeWord,
                listaFiltriTrasmissione = DocsPaVO.filtri.trasmissione.listaArgomenti.NOTE_GENERALI,
                listaFiltriModelliTrasmissione = DocsPaVO.filtri.trasmissione.listaArgomentiModelliTrasmissione.CODICE_MODELLO,
                listaFiltriTrasmissioneNascosti = DocsPaVO.filtri.trasmissione.listaArgomentiNascosti.TIPO_OGGETTO,
                listaFiltriDocumento = DocsPaVO.filtri.ricerca.listaArgomenti.TIPO,
                listaFiltriFascicolo = DocsPaVO.filtri.fascicolazione.listaArgomenti.APERTURA_IL,
                listaFiltriStampaRegistro = DocsPaVO.filtri.stampaRegistro.listaArgomenti.REGISTRO
            });

            frList.Add(new FiltroRicerca
            {
                argomento = "CORR_TYPE_OWNER",
                valore = "R",
                searchTextOptions = SearchTextOptionsEnum.WholeWord,
                listaFiltriTrasmissione = DocsPaVO.filtri.trasmissione.listaArgomenti.NOTE_GENERALI,
                listaFiltriModelliTrasmissione = DocsPaVO.filtri.trasmissione.listaArgomentiModelliTrasmissione.CODICE_MODELLO,
                listaFiltriTrasmissioneNascosti = DocsPaVO.filtri.trasmissione.listaArgomentiNascosti.TIPO_OGGETTO,
                listaFiltriDocumento = DocsPaVO.filtri.ricerca.listaArgomenti.TIPO,
                listaFiltriFascicolo = DocsPaVO.filtri.fascicolazione.listaArgomenti.APERTURA_IL,
                listaFiltriStampaRegistro = DocsPaVO.filtri.stampaRegistro.listaArgomenti.REGISTRO
            });

            frList.Add(new FiltroRicerca
            {
                argomento = "CORR_TYPE_AUTHOR",
                valore = "R",
                searchTextOptions = SearchTextOptionsEnum.WholeWord,
                listaFiltriTrasmissione = DocsPaVO.filtri.trasmissione.listaArgomenti.NOTE_GENERALI,
                listaFiltriModelliTrasmissione = DocsPaVO.filtri.trasmissione.listaArgomentiModelliTrasmissione.CODICE_MODELLO,
                listaFiltriTrasmissioneNascosti = DocsPaVO.filtri.trasmissione.listaArgomentiNascosti.TIPO_OGGETTO,
                listaFiltriDocumento = DocsPaVO.filtri.ricerca.listaArgomenti.TIPO,
                listaFiltriFascicolo = DocsPaVO.filtri.fascicolazione.listaArgomenti.APERTURA_IL,
                listaFiltriStampaRegistro = DocsPaVO.filtri.stampaRegistro.listaArgomenti.REGISTRO
            });


            frList.Add(new FiltroRicerca
            {
                argomento = "ANNO_PROTOCOLLO",
                valore = "",
                searchTextOptions = SearchTextOptionsEnum.WholeWord,
                listaFiltriTrasmissione = DocsPaVO.filtri.trasmissione.listaArgomenti.NOTE_GENERALI,
                listaFiltriModelliTrasmissione = DocsPaVO.filtri.trasmissione.listaArgomentiModelliTrasmissione.CODICE_MODELLO,
                listaFiltriTrasmissioneNascosti = DocsPaVO.filtri.trasmissione.listaArgomentiNascosti.TIPO_OGGETTO,
                listaFiltriDocumento = DocsPaVO.filtri.ricerca.listaArgomenti.TIPO,
                listaFiltriFascicolo = DocsPaVO.filtri.fascicolazione.listaArgomenti.APERTURA_IL,
                listaFiltriStampaRegistro = DocsPaVO.filtri.stampaRegistro.listaArgomenti.REGISTRO
            });


            frList.Add(new FiltroRicerca
            {
                argomento = "ORACLE_FIELD_FOR_ORDER",
                valore = "NVL (a.dta_proto, a.creation_time)",
                searchTextOptions = SearchTextOptionsEnum.WholeWord,
                listaFiltriTrasmissione = DocsPaVO.filtri.trasmissione.listaArgomenti.NOTE_GENERALI,
                listaFiltriModelliTrasmissione = DocsPaVO.filtri.trasmissione.listaArgomentiModelliTrasmissione.CODICE_MODELLO,
                listaFiltriTrasmissioneNascosti = DocsPaVO.filtri.trasmissione.listaArgomentiNascosti.TIPO_OGGETTO,
                listaFiltriDocumento = DocsPaVO.filtri.ricerca.listaArgomenti.TIPO,
                listaFiltriFascicolo = DocsPaVO.filtri.fascicolazione.listaArgomenti.APERTURA_IL,
                listaFiltriStampaRegistro = DocsPaVO.filtri.stampaRegistro.listaArgomenti.REGISTRO,
                nomeCampo = "D9"
            });

            frList.Add(new FiltroRicerca
            {
                argomento = "SQL_FIELD_FOR_ORDER",
                valore = "ISNULL (a.dta_proto, a.creation_time)",
                searchTextOptions = SearchTextOptionsEnum.WholeWord,
                listaFiltriTrasmissione = DocsPaVO.filtri.trasmissione.listaArgomenti.NOTE_GENERALI,
                listaFiltriModelliTrasmissione = DocsPaVO.filtri.trasmissione.listaArgomentiModelliTrasmissione.CODICE_MODELLO,
                listaFiltriTrasmissioneNascosti = DocsPaVO.filtri.trasmissione.listaArgomentiNascosti.TIPO_OGGETTO,
                listaFiltriDocumento = DocsPaVO.filtri.ricerca.listaArgomenti.TIPO,
                listaFiltriFascicolo = DocsPaVO.filtri.fascicolazione.listaArgomenti.APERTURA_IL,
                listaFiltriStampaRegistro = DocsPaVO.filtri.stampaRegistro.listaArgomenti.REGISTRO,
                nomeCampo = "D9"
            });

            frList.Add(new FiltroRicerca
            {
                argomento = "ORDER_DIRECTION",
                valore = "DESC",
                searchTextOptions = SearchTextOptionsEnum.WholeWord,
                listaFiltriTrasmissione = DocsPaVO.filtri.trasmissione.listaArgomenti.NOTE_GENERALI,
                listaFiltriModelliTrasmissione = DocsPaVO.filtri.trasmissione.listaArgomentiModelliTrasmissione.CODICE_MODELLO,
                listaFiltriTrasmissioneNascosti = DocsPaVO.filtri.trasmissione.listaArgomentiNascosti.TIPO_OGGETTO,
                listaFiltriDocumento = DocsPaVO.filtri.ricerca.listaArgomenti.TIPO,
                listaFiltriFascicolo = DocsPaVO.filtri.fascicolazione.listaArgomenti.APERTURA_IL,
                listaFiltriStampaRegistro = DocsPaVO.filtri.stampaRegistro.listaArgomenti.REGISTRO,
                nomeCampo = "D9"
            });

            frList.Add(new FiltroRicerca
            {
                argomento = "DA_PROTOCOLLARE",
                valore = "0",
                searchTextOptions = SearchTextOptionsEnum.WholeWord,
                listaFiltriTrasmissione = DocsPaVO.filtri.trasmissione.listaArgomenti.NOTE_GENERALI,
                listaFiltriModelliTrasmissione = DocsPaVO.filtri.trasmissione.listaArgomentiModelliTrasmissione.CODICE_MODELLO,
                listaFiltriTrasmissioneNascosti = DocsPaVO.filtri.trasmissione.listaArgomentiNascosti.TIPO_OGGETTO,
                listaFiltriDocumento = DocsPaVO.filtri.ricerca.listaArgomenti.TIPO,
                listaFiltriFascicolo = DocsPaVO.filtri.fascicolazione.listaArgomenti.APERTURA_IL,
                listaFiltriStampaRegistro = DocsPaVO.filtri.stampaRegistro.listaArgomenti.REGISTRO,
            });

            return frList.ToArray();
        }


        public static RicercaResponse getAdl(GetAdlRequest request)
        {
            UserInfo ui = request.UserInfo ;
            InfoUtente infoUtente =  request.UserInfo.InfoUtente;
            infoUtente.idGruppo = request.RuoloInfo.IdGruppo;
            //ui.Utente.ruoli = BusinessLogic.Utenti.UserManager.getRuoliUtente(ui.IdPeople);
            Ruolo ruolo= null;
            Registro registro = null;

            foreach (Ruolo r in BusinessLogic.Utenti.UserManager.getRuoliUtente(ui.IdPeople))
            {
                if (r.systemId == request.RuoloInfo.Id)
                {
                    ruolo= r;
                    foreach (Registro re in r.registri)
                    {
                        if (re.systemId == request.RegistroInfo.SystemId)
                        {
                            registro = re;
                        }
                    }
                }
            }

            List<OrgTitolario> titolari = BusinessLogic.Amministrazione.TitolarioManager.getTitolariUtilizzabili(infoUtente.idAmministrazione).Cast<OrgTitolario>().ToList();
            FiltroRicerca[][] qv;
             qv = new FiltroRicerca[1][];
             qv[0] = creaFiltriADLDoc(infoUtente, ruolo);
            List <SearchResultInfo> outList;
            int numTotPage,nRec;
            List<SearchObject> documenti = DocumentoGetQueryDocumentoPagingCustom(infoUtente, qv, 1, false, 100, out numTotPage, out nRec, false, false, false, null, out outList).Cast <SearchObject>().ToList();

            List<SearchResultInfo> idProjects = new List<SearchResultInfo>();

            List<SearchObject> fascicoli = BusinessLogic.Fascicoli.FascicoloManager.getListaFascicoliPagingCustom(
                infoUtente,
                null,
                registro,
                creaFiltriADLFasc(infoUtente, ruolo , titolari),
                false, true, false, out numTotPage, out  nRec, 1, 20, false, out idProjects, null, string.Empty, false, false, null, null, true).Cast <SearchObject>().ToList();


            List<RicercaElement> element = new List<RicercaElement>();

            foreach (SearchObject d in documenti)
            {
                string tipodoc = (from n in d.SearchObjectField where n.SearchObjectFieldID == "D3" select n.SearchObjectFieldValue).FirstOrDefault().ToString();
                string oggettodoc = (from n in d.SearchObjectField where n.SearchObjectFieldID == "D4" select n.SearchObjectFieldValue).FirstOrDefault().ToString();
                               
                RicercaElement r = RicercaElement.buildInstance(new InfoDocumento { idProfile = d.SearchObjectID, oggetto = oggettodoc, tipoProto = tipodoc});
                element.Add(r);
            }

            foreach (SearchObject f in fascicoli)
            {
                string descrizionefasc = (from n in f.SearchObjectField where n.SearchObjectFieldID == "P4" select n.SearchObjectFieldValue).FirstOrDefault().ToString();
                string notefasc = (from n in f.SearchObjectField where n.SearchObjectFieldID == "P8" select n.SearchObjectFieldValue).FirstOrDefault().ToString();

                List<DocsPaVO.Note.InfoNota> noteFascLst = new List<DocsPaVO.Note.InfoNota>();
                noteFascLst.Add(new DocsPaVO.Note.InfoNota { Testo = notefasc });

                RicercaElement r = RicercaElement.buildInstance(new Fascicolo { systemID = f.SearchObjectID, descrizione = descrizionefasc, noteFascicolo = noteFascLst.ToArray() });
                element.Add(r);
            }


            RicercaResponse retval = new RicercaResponse();
            retval.Risultati = element;
            retval.TotalRecordCount = element.Count();
            retval.Code = RicercaResponseCode.OK;
            
            return retval;
        }


        
        public static ArrayList DocumentoGetQueryDocumentoPagingCustom(InfoUtente infoUtente, DocsPaVO.filtri.FiltroRicerca[][] queryList, int numPage, bool security, int pageSize, out int numTotPage, out int nRec, bool getIdProfilesList, bool gridPersonalization, bool export, String[] documentsSystemId, out List<SearchResultInfo> idProfileList)
        {
            List<SearchResultInfo> toSet = new List<SearchResultInfo>();
            ArrayList objListaInfoDocumenti = null;
            numTotPage = 0;
            nRec = 0;

            try
            {
                objListaInfoDocumenti = BusinessLogic.Documenti.InfoDocManager.getQueryPagingCustom(infoUtente, queryList, numPage, pageSize, security, export, gridPersonalization, null, documentsSystemId, out numTotPage, out nRec, getIdProfilesList, out toSet);
            }
            catch (Exception e)
            {
               // logger.Debug("Errore in DocsPaWS.asmx  - metodo: DocumentoGetQueryDocumentoPagingCustom", e);

                objListaInfoDocumenti = null;
            }
            idProfileList = toSet;
            return objListaInfoDocumenti;
        }
        */
        #endregion

        #endregion

        #region LIBRO FIRMA

        public static LibroFirmaResponse GetLibroFirmaElements(LibroFirmaRequest request)
        {
            try
            {
                int totalRecordCount = 0;
                LibroFirmaResponse res = new LibroFirmaResponse();
                res.Elements = new List<LibroFirmaElement>();
                DocsPaVO.utente.InfoUtente infoUser = new InfoUtente();
                infoUser = request.UserInfo.InfoUtente;
                infoUser.idGruppo = request.IdGruppo;
                List<DocsPaVO.LibroFirma.ElementoInLibroFirma> elements = BusinessLogic.LibroFirma.LibroFirmaManager.GetElementiInLibroFirmaIntoPage(infoUser, request.PageSize, request.RequestedPage, request.Testo, request.TipoRicerca, out totalRecordCount);
                foreach (DocsPaVO.LibroFirma.ElementoInLibroFirma el in elements)
                {
                    res.Elements.Add(DocsPaVO.Mobile.LibroFirmaElement.BuildInstance(el));
                }
                res.TotalRecordCount = totalRecordCount;
                return res;
            }
            catch (Exception e)
            {
                logger.Error("eccezione: " + e);
                return LibroFirmaResponse.ErrorResponse;
            }
        }

        public static LibroFirmaResponse CambiaStatoElementoLibroFirma(LibroFirmaCambiaStatoRequest request)
        {
            LibroFirmaResponse res = new LibroFirmaResponse();
            string message = string.Empty;
            try
            { 
                DocsPaVO.LibroFirma.ElementoInLibroFirma elemento = new DocsPaVO.LibroFirma.ElementoInLibroFirma();
                elemento.IdElemento = request.elemento.IdElemento;
                elemento.DataAccettazione= request.elemento.DataAccettazione;
                elemento.StatoFirma= (DocsPaVO.LibroFirma.TipoStatoElemento)Enum.Parse(typeof( DocsPaVO.LibroFirma.TipoStatoElemento),request.elemento.StatoFirma);
                elemento.IdIstanzaProcesso = request.elemento.IdIstanzaProcesso;
                elemento.MotivoRespingimento = request.elemento.MotivoRespingimento;

                DocsPaVO.utente.InfoUtente infoUser = new InfoUtente();
                infoUser = request.UserInfo.InfoUtente;

                Ruolo ruolo = BusinessLogic.Utenti.UserManager.getRuoloById(request.IdorrGlobaliRuolo);

                BusinessLogic.LibroFirma.LibroFirmaManager.AggiornaStatoElementoInLibroFirma(elemento, request.NuovoStato, ruolo, infoUser, out message);
            }
            catch (Exception e)
            {
                logger.Error("eccezione: " + e);
                return LibroFirmaResponse.ErrorResponse;
            }
            return res;
        }

        public static LibroFirmaResponse FirmaSelezionatiElementiLf(LibroFirmaRequest request)
        {
            LibroFirmaResponse res = new LibroFirmaResponse();
            string message = string.Empty;
            try
            {
                DocsPaVO.utente.InfoUtente infoUser = new InfoUtente();
                infoUser = request.UserInfo.InfoUtente;
                infoUser.idGruppo = request.IdGruppo;

                //Devo estrarre la lista degli elementi da respingere
                List<DocsPaVO.LibroFirma.ElementoInLibroFirma> elements = BusinessLogic.LibroFirma.LibroFirmaManager.GetElementsInLibroFirmabByState(infoUser, DocsPaVO.LibroFirma.TipoStatoElemento.DA_FIRMARE);
                if (elements != null && elements.Count > 0)
                {
                    Ruolo ruolo = BusinessLogic.Utenti.UserManager.getRuoloByIdGruppo(request.IdGruppo);
                    infoUser.idCorrGlobali = ruolo.systemId;

                    List<FileRequest> fileReqToSign = new List<FileRequest>();
                    Allegato attach;
                    Documento doc;

                    #region AVANZAMENTO ITER
                    List<DocsPaVO.LibroFirma.ElementoInLibroFirma> listElementAdvancementProcess = (from i in elements
                                                                                                    where i.TipoFirma.Equals("DOC_STEP_OVER")
                                                                                select i).ToList();
                    if (listElementAdvancementProcess != null && listElementAdvancementProcess.Count > 0)
                    {
                        foreach (DocsPaVO.LibroFirma.ElementoInLibroFirma element in listElementAdvancementProcess)
                        {
                            if (string.IsNullOrEmpty(element.InfoDocumento.IdDocumentoPrincipale))
                            {
                                doc = new Documento()
                                {
                                    descrizione = element.InfoDocumento.Oggetto,
                                    docNumber = element.InfoDocumento.Docnumber,
                                    versionId = element.InfoDocumento.VersionId,
                                    version = element.InfoDocumento.NumVersione.ToString(),
                                    versionLabel = element.InfoDocumento.NumAllegato.ToString(),
                                    inLibroFirma = true

                                };
                                fileReqToSign.Add(doc as FileRequest);
                            }
                            else
                            {
                                attach = new Allegato()
                                {
                                    descrizione = element.InfoDocumento.Oggetto,
                                    docNumber = element.InfoDocumento.Docnumber,
                                    versionId = element.InfoDocumento.VersionId,
                                    version = element.InfoDocumento.NumVersione.ToString(),
                                    versionLabel = element.InfoDocumento.NumAllegato.ToString(),
                                    inLibroFirma = true

                                };
                                fileReqToSign.Add(attach as FileRequest);
                            }
                        }
                        bool isAdvancementProcess = true;
                        PutElectronicSignature(fileReqToSign, infoUser, isAdvancementProcess);
                    }
                    #endregion

                    #region SOTTOSCRIZIONE

                    fileReqToSign.Clear();
                     List<DocsPaVO.LibroFirma.ElementoInLibroFirma> listElementSubscription = (from i in elements
                                                                                               where i.TipoFirma.Equals("DOC_VERIFIED")
                                                                                    select i).ToList();
                     if (listElementSubscription != null && listElementSubscription.Count > 0)
                     {
                         foreach (DocsPaVO.LibroFirma.ElementoInLibroFirma element in listElementSubscription)
                         {
                             if (string.IsNullOrEmpty(element.InfoDocumento.IdDocumentoPrincipale))
                             {
                                 doc = new Documento()
                                 {
                                     descrizione = element.InfoDocumento.Oggetto,
                                     docNumber = element.InfoDocumento.Docnumber,
                                     versionId = element.InfoDocumento.VersionId,
                                     version = element.InfoDocumento.NumVersione.ToString(),
                                     versionLabel = element.InfoDocumento.NumAllegato.ToString(),
                                     inLibroFirma = true

                                 };
                                 fileReqToSign.Add(doc as FileRequest);
                             }
                             else
                             {
                                 attach = new Allegato()
                                 {
                                     descrizione = element.InfoDocumento.Oggetto,
                                     docNumber = element.InfoDocumento.Docnumber,
                                     versionId = element.InfoDocumento.VersionId,
                                     version = element.InfoDocumento.NumVersione.ToString(),
                                     versionLabel = element.InfoDocumento.NumAllegato.ToString(),
                                     inLibroFirma = true

                                 };
                                 fileReqToSign.Add(attach as FileRequest);
                             }
                         }

                         bool isAdvancementProcess = false;
                         PutElectronicSignature(fileReqToSign, infoUser, isAdvancementProcess);
                     }
                    #endregion

                }
                res = GetLibroFirmaElements(request);
            }
            catch (Exception e)
            {
                logger.Error("eccezione: " + e);
                return LibroFirmaResponse.ErrorResponse;
            }
            return res;
        }

        private static void PutElectronicSignature(List<DocsPaVO.documento.FileRequest> approvingFiles, DocsPaVO.utente.InfoUtente infoUtente, bool isAdvancementProcess)
        {
            string message;
            foreach (DocsPaVO.documento.FileRequest fileReq in approvingFiles)
            {
                message = string.Empty;
                if (BusinessLogic.LibroFirma.LibroFirmaManager.PutElectronicSignature(fileReq, infoUtente, isAdvancementProcess, out message))
                {
                    string method2 = "DOC_VERIFIED";
                    string description2 = "Il documento è stato firmato elettronicamente";
                    if (isAdvancementProcess)
                    {
                        description2 = "Eseguito passo avanzamento iter";
                        method2 = "DOC_STEP_OVER";
                    }
                    if (fileReq.inLibroFirma)
                    {
                        BusinessLogic.LibroFirma.LibroFirmaManager.AggiornaDataEsecuzioneElemento(fileReq.docNumber, DocsPaVO.LibroFirma.TipoStatoElemento.FIRMATO.ToString());
                        BusinessLogic.LibroFirma.LibroFirmaManager.SalvaStoricoIstanzaProcessoFirmaByDocnumber(fileReq.docNumber, description2, infoUtente);
                    }

                    BusinessLogic.UserLog.UserLog.WriteLog(infoUtente.userId, infoUtente.idPeople, infoUtente.idGruppo, infoUtente.idAmministrazione, method2, fileReq.docNumber,
                        description2, DocsPaVO.Logger.CodAzione.Esito.OK, (infoUtente != null && infoUtente.delegato != null ? infoUtente.delegato : null), "0");
                }
                else if (fileReq.inLibroFirma)
                {
                    BusinessLogic.LibroFirma.LibroFirmaManager.AggiornaErroreEsitoFirma(fileReq.docNumber, message);
                }
            }
        }

        public static LibroFirmaResponse RespingiSelezionatiElementiLf(LibroFirmaRequest request)
        {
            LibroFirmaResponse res = new LibroFirmaResponse();
            string message = string.Empty;
            bool result = false;
            try
            {
                DocsPaVO.utente.InfoUtente infoUser = new InfoUtente();
                infoUser = request.UserInfo.InfoUtente;
                infoUser.idGruppo = request.IdGruppo;

                //Devo estrarre la lista degli elementi da respingere
                List<DocsPaVO.LibroFirma.ElementoInLibroFirma> elements = BusinessLogic.LibroFirma.LibroFirmaManager.GetElementsInLibroFirmabByState(infoUser, DocsPaVO.LibroFirma.TipoStatoElemento.DA_RESPINGERE);
                if (elements != null && elements.Count > 0)
                {
                    Ruolo ruolo = BusinessLogic.Utenti.UserManager.getRuoloByIdGruppo(request.IdGruppo);

                    foreach (DocsPaVO.LibroFirma.ElementoInLibroFirma e in elements)
                    {
                        result = BusinessLogic.LibroFirma.LibroFirmaManager.RejectElementsSignatureProcess(e, ruolo, infoUser);

                        //Se è andato a buon fine ed è un allegato
                        if (result && (!string.IsNullOrEmpty(e.InfoDocumento.IdDocumentoPrincipale)) && (e.InfoDocumento.IdDocumentoPrincipale != e.InfoDocumento.Docnumber))
                        {
                            BusinessLogic.LibroFirma.LibroFirmaManager.StopPassoWait(e.InfoDocumento.IdDocumentoPrincipale, infoUser);
                        }
                    }
                }
                res = GetLibroFirmaElements(request);
            }
            catch (Exception e)
            {
                logger.Error("eccezione: " + e);
                return LibroFirmaResponse.ErrorResponse;
            }
            return res;
        }

        public static bool ExistsElementWithSignCades(UserInfo infoUser, RuoloInfo roleInfo)
        {
            bool result = false;
            try
            {
                DocsPaVO.utente.InfoUtente infoUtente = new InfoUtente();
                infoUtente = infoUser.InfoUtente;
                infoUtente.idGruppo = roleInfo.IdGruppo;
                string tipoFirma = "DOC_SIGNATURE";
                result = BusinessLogic.LibroFirma.LibroFirmaManager.ExistsElementWithTypeSign(tipoFirma, infoUtente);
            }
            catch (Exception ex)
            {
                logger.Error("Exception: " + ex);
            }
            return result;

        }

        public static bool ExistsElementWithSignPades(UserInfo infoUser, RuoloInfo roleInfo)
        {
            bool result = false;
            try
            {
                DocsPaVO.utente.InfoUtente infoUtente = new InfoUtente();
                infoUtente = infoUser.InfoUtente;
                infoUtente.idGruppo = roleInfo.IdGruppo;
                string tipoFirma = "DOC_SIGNATURE_P";
                result = BusinessLogic.LibroFirma.LibroFirmaManager.ExistsElementWithTypeSign(tipoFirma, infoUtente);
            }
            catch (Exception ex)
            {
                logger.Error("Exception: " + ex);
            }
            return result;

        }

        public static HSMSignResponse HsmMultiSign(HSMSignRequest request)
        {
            try
            {
                logger.Info("begin");
                InfoUtente infoUtente = request.UserInfo.InfoUtente;
                infoUtente.idGruppo = request.IdGruppo;
                infoUtente.idCorrGlobali = request.IdCorrGlobali;
                HSMSignResponse res = new HSMSignResponse();
                List<string> idDocuments = new List<string>();
                DocsPaVO.documento.SchedaDocumento sd = null;
                FileRequest fr = null;
                string method = "DOC_SIGNATURE";
                string description = "Il documento è stato firmato digitalmente HSM CADES";
                if (request.TipoFirma.ToString() == "PADES")
                {
                    method = "DOC_SIGNATURE_P";
                    description = "Il documento è stato firmato digitalmente HSM PADES";
                }

                idDocuments = BusinessLogic.LibroFirma.LibroFirmaManager.GetIdDocumentInLibroFirmabBySign(infoUtente, DocsPaVO.LibroFirma.TipoStatoElemento.DA_FIRMARE, method);
                foreach (string idDoc in idDocuments)
                {

                    // recupera il file request del doc
                    sd = BusinessLogic.Documenti.DocManager.getDettaglio(infoUtente, idDoc, idDoc);
                    fr = (FileRequest)sd.documenti[0];
                    res.Code = ((BusinessLogic.Documenti.FileManager.HSM_Sign(infoUtente,
                                                                             fr,
                                                                             request.cofirma,
                                                                             request.timestamp,
                                                                             request.TipoFirma,
                                                                             request.AliasCertificato,
                                                                             request.DominioCertificato,
                                                                             request.OtpFirma,
                                                                             request.PinCertificato,
                                                                             request.ConvertPdf)
                                                                              ) ?
                                                                            HSMSignResponseCode.OK :
                                                                            HSMSignResponseCode.SYSTEM_ERROR);
                    logger.Info("end: " + res.Code.ToString());
                    if (res.Code.ToString().Equals(HSMSignResponseCode.OK.ToString()))
                    {
                        if (fr.inLibroFirma)
                        {
                            BusinessLogic.LibroFirma.LibroFirmaManager.AggiornaDataEsecuzioneElemento(fr.docNumber, DocsPaVO.LibroFirma.TipoStatoElemento.FIRMATO.ToString());
                            BusinessLogic.LibroFirma.LibroFirmaManager.SalvaStoricoIstanzaProcessoFirmaByDocnumber(fr.docNumber, description, infoUtente);
                        }

                        BusinessLogic.UserLog.UserLog.WriteLog(infoUtente.userId, infoUtente.idPeople, infoUtente.idGruppo, infoUtente.idAmministrazione, method, fr.docNumber,
                         description, DocsPaVO.Logger.CodAzione.Esito.OK, (infoUtente != null && infoUtente.delegato != null ? infoUtente.delegato : null), "0");
                    }
                    else
                    {
                        if (fr.inLibroFirma)
                        {
                            string msg = "Errore durante la procedura di firma";
                            BusinessLogic.LibroFirma.LibroFirmaManager.AggiornaErroreEsitoFirma(fr.docNumber, msg);
                        }
                    }
                }
                return res;
            }
            catch (Exception e)
            {
                logger.Error("Exception: " + e);
                return HSMSignResponse.ErrorResponse;
            }
        }

        /// <summary>
        /// Verifica se il ruolo è abilitato alla funzione di richiesta OTP
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public static bool LibroFirmaIsAutorized(RuoloInfo info)
        {
            bool result = false;
            try
            {
                Ruolo role = BusinessLogic.Utenti.UserManager.getRuolo(info.Id);
                if (role != null)
                {
                    result = (from function in (Funzione[])role.funzioni.ToArray(typeof(Funzione)) where function.codice.ToUpper().Equals("DO_LIBRO_FIRMA") select function.systemId).Count() > 0 ? true : false;
                }
            }
            catch (Exception ex)
            {
                logger.Error("Exception: " + ex);
            }
            return result;

        }
        #endregion

    }

    public class RuoliComparer : IComparer
    {

        public int Compare(object x, object y)
        {
            int xId = Int32.Parse(((Ruolo)x).systemId);
            int yId = Int32.Parse(((Ruolo)y).systemId);
            return xId.CompareTo(yId);
        }
    }
}
