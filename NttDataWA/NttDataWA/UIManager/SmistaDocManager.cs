using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;
using NttDataWA.DocsPaWR;
using NttDataWA.Utils;

namespace NttDataWA.UIManager
{
    public class SmistaDocManager
    {
        private static DocsPaWR.DocsPaWebService docsPaWS = ProxyManager.GetWS();

        #region Session
        public NotificationsFilters Filters
        {
            get
            {
                if (HttpContext.Current.Session["Filters"] != null)
                {
                    return HttpContext.Current.Session["Filters"] as NotificationsFilters;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                HttpContext.Current.Session["Filters"] = value;
            }
        }

        public const string CURRENT_FILTERS_SESSION_KEY = "SmistaDocFilters";


        /// <summary>
        /// Rimozione filtri in sessione
        /// </summary>
        public static void RemoveCurrentFilters()
        {
            if (HttpContext.Current.Session[CURRENT_FILTERS_SESSION_KEY] != null)
                HttpContext.Current.Session.Remove(CURRENT_FILTERS_SESSION_KEY);

            RemoveCurrentFilters();
        }

        /// <summary>
        /// Impostazione / reperimento da sessione del filtro correntemente impostato sulle trasmissioni
        /// </summary>
        public static DocsPaWR.FiltroRicerca[] CurrentFilters
        {
            get
            {
                return HttpContext.Current.Session[CURRENT_FILTERS_SESSION_KEY] as DocsPaWR.FiltroRicerca[];
            }
            set
            {
                if (value == null)
                {
                    HttpContext.Current.Session.Remove(CURRENT_FILTERS_SESSION_KEY);
                    //HttpContext.Current.Session.Remove(FiltriRicercaTrasmissioni.CURRENT_UI_FILTERS_SESSION_KEY);
                }
                else
                {
                    HttpContext.Current.Session[CURRENT_FILTERS_SESSION_KEY] = value;
                }
            }
        }

        #endregion

        #region Declaration

        private int _currentDocumentIndex = 1;
        private DocsPaWR.DocumentoSmistamento _currentDocument = null;

        private ArrayList _datiDocumentiTrasmessi = null;

        private DocsPaWR.Ruolo _ruolo = null;
        private DocsPaWR.Utente _utente = null;
        private DocsPaWR.InfoUtente _infoUtente = null;

        private DocsPaWR.MittenteSmistamento _mittenteSmistamento = null;

        private DocsPaWR.UOSmistamento _uoAppartenenza = null;
        private DocsPaWR.UOSmistamento[] _uoInferiori = null;


        #endregion

        #region Public

        public SmistaDocManager()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="docNumber"></param>
        public SmistaDocManager(DocsPaWR.Ruolo ruolo,
                                DocsPaWR.Utente utente,
                                DocsPaWR.InfoUtente infoUtente,
                                string docNumber)
        {
            this.Initialize(ruolo, utente, infoUtente, docNumber);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="ruolo"></param>
        /// <param name="utente"></param>
        /// <param name="infoUtente"></param>
        public SmistaDocManager(DocsPaWR.Ruolo ruolo,
                                DocsPaWR.Utente utente,
                                DocsPaWR.InfoUtente infoUtente)
        {
            this.Initialize(ruolo, utente, infoUtente, string.Empty);
        }


        private void Initialize(DocsPaWR.Ruolo ruolo,
                                DocsPaWR.Utente utente,
                                DocsPaWR.InfoUtente infoUtente,
                                string docNumber)
        {
            this._ruolo = ruolo;
            this._utente = utente;
            this._infoUtente = infoUtente;

            this.FillMittenteSmistamento();

            //if (docNumber != null && docNumber.Equals(string.Empty))
            //{
            // Se sono stati impostati i filtri in todolist sulle trasmissioni,
            // vengono applicati anche nel reperimento dei dati dello smistamento
            //DocsPaWR.FiltroRicerca[] filtriRicerca = this.GetFilters();

            //if (CurrentFilters != null)
            //    filtriRicerca = CurrentFilters;
            this.FillArrayDocumentiTrasmessi(null);
            //}
            //else
            //{
            //    DocsPaWR.DatiTrasmissioneDocumento datiDoc = new DocsPaWR.DatiTrasmissioneDocumento();
            //    datiDoc.IDDocumento = docNumber;

            //    this._datiDocumentiTrasmessi = new ArrayList();
            //    this._datiDocumentiTrasmessi.Add(datiDoc);
            //    datiDoc = null;
            //}

            // posizionamento sul primo documento in lista
            if (this.GetDocumentCount() > 0)
                this.MoveFirstDocument();

            // lettura UO (destinazione)
            this.FillUO();
        }

        private void BuildFiltersSearchNotifications()
        {
            //Filters = new NotificationsFilters();
            //Filters.TYPE_DOCUMENT = this.rblFilterDocumentType.SelectedValue.Equals("ALL") ? string.Empty : this.rblFilterDocumentType.SelectedValue;
            //Filters.DOCUMENT_ACQUIRED = this.cbDocumentAcquired.Selected;
            //Filters.DOCUMENT_SIGNED = this.cbDocumentSigned.Selected;
            //Filters.DOCUMENT_UNSIGNED = this.cbDocumentNotSigned.Selected;
            //Filters.PENDING = this.cbWaitingAcceptance.Checked;
            //Filters.DATE_EVENT_FROM = this.txt_dateEventFrom.Text;
            //Filters.DATE_EVENT_TO = this.PlaceHolderDateEventTo.Visible ? this.txt_dateEventTo.Text : "";
            //Filters.DATE_EXPIRE_FROM = this.txt_expirationDateFrom.Text;
            //Filters.DATE_EXPIRE_TO = this.PlaceHolderExpirationDateTo.Visible ? this.txt_expirationDateTo.Text : "";
            //Filters.AUTHOR_DESCRIPTION = this.txtDescrizioneCreatore.Text;
            //Filters.AUTHOR_SYSTEM_ID = this.txtCodiceCreatore.Text;
            //Filters.AUTHOR_TYPE = this.rblOwnerType.SelectedValue;
            //Filters.OBJECT = this.TxtObject.Text;
            //Filters.TYPE_FILE_ACQUIRED = this.ddlTypeFileAcquired.SelectedValue;

            // Reperimento filtri impostati dall'utente
            DocsPaWR.FiltroRicerca[] filters = this.GetFilters();

            if (filters != null)
            {
                int countValoriOk = 0;
                foreach (DocsPaWR.FiltroRicerca filtro in filters)
                {
                    if (filtro.argomento == DocsPaWR.FiltriTrasmissioneNascosti.ELEMENTI_NON_VISTI.ToString() && !string.IsNullOrEmpty(filtro.valore))
                        countValoriOk++;
                    if (filtro.argomento == DocsPaWR.FiltriTrasmissioneNascosti.TIPO_OGGETTO.ToString() && !string.IsNullOrEmpty(filtro.valore))
                        countValoriOk++;
                    if (filtro.argomento == DocsPaWR.FiltriTrasmissioneNascosti.DOCUMENTI_ACQUISITI.ToString() && !string.IsNullOrEmpty(filtro.valore))
                        countValoriOk++;
                    if (filtro.argomento == DocsPaWR.FiltriTrasmissioneNascosti.DOCUMENTI_FIRMATI.ToString() && !string.IsNullOrEmpty(filtro.valore))
                        countValoriOk++;
                }
                // Impostazione filtri impostati in sessione
                CurrentFilters = filters;


            }
        }

        public DocsPaWR.DocumentoSmistamento GetCurrentDocument(bool content)
        {
            if (this._currentDocument == null || content)
                this._currentDocument = this.GetDocumentoTrasmesso(content);
            return this._currentDocument;
        }

        public DocsPaWR.UOSmistamento GetUOAppartenenza()
        {
            return this._uoAppartenenza;
        }
        public void SetUOAppartenenza(DocsPaWR.UOSmistamento uoApp)
        {
            this._uoAppartenenza = uoApp;

        }

        public DocsPaWR.UOSmistamento[] GetUOInferiori()
        {
            return this._uoInferiori;
        }
        public void SetUOInferiori(DocsPaWR.UOSmistamento[] uoInf)
        {
            this._uoInferiori = uoInf;
        }

        /// <summary>
        /// Reperimento dello stato della trasmissione correntemente relativa al documento 
        /// correntemente visualizzato nello smistamento
        /// </summary>
        /// <returns></returns>
        public DocsPaWR.StatoTrasmissioneUtente GetStatoTrasmissioneCorrente()
        {
            // Reperimento id della trasmissione utente
            string idTrasmissioneUtente = this.GetIdTrasmissioneUtente(this.GetCurrentDocumentPosition() - 1);

            return TrasmManager.getStatoTrasmissioneUtente(idTrasmissioneUtente);
        }

        public void RemoveDocument()
        {
            this._datiDocumentiTrasmessi.RemoveAt(this._currentDocumentIndex - 1);
        }

        public int GetCurrentDocumentPosition()
        {
            return this._currentDocumentIndex;
        }

        public int GetDocumentCount()
        {
            //return this._idDocumentiTrasmessi.Count;
            return this._datiDocumentiTrasmessi.Count;
        }

        public bool MoveFirstDocument()
        {
            bool retValue = false;

            if (this._currentDocumentIndex >= 1)
            {
                this._currentDocument = null;
                this._currentDocumentIndex = 1;
                retValue = true;
            }

            return retValue;
        }

        public bool MoveLastDocument()
        {
            bool retValue = false;

            if (this._currentDocumentIndex < this.GetDocumentCount())
            {
                this._currentDocument = null;
                this._currentDocumentIndex = this.GetDocumentCount();
                retValue = true;
            }

            return retValue;
        }

        public bool MoveNextDocument()
        {
            bool retValue = false;

            if (this._currentDocumentIndex < this.GetDocumentCount())
            {
                this._currentDocument = null;
                this._currentDocumentIndex++;
                retValue = true;
            }

            return retValue;
        }

        public bool MovePreviousDocument()
        {
            bool retValue = false;

            if (this._currentDocumentIndex > 1)
            {
                this._currentDocument = null;
                this._currentDocumentIndex--;
                retValue = true;
            }

            return retValue;
        }

        public bool MoveAbsoluteDocument(int documentIndex)
        {
            bool retValue = false;

            if (documentIndex >= 1 &&
                documentIndex <= this._datiDocumentiTrasmessi.Count)
            {
                this._currentDocument = null;
                this._currentDocumentIndex = documentIndex;
                retValue = true;
            }

            return retValue;
        }

        public bool IsTrasmissioneConWorkflow(int documentIndex)
        {
            bool retValue = false;

            if (this._datiDocumentiTrasmessi.Count > 0)
                retValue = ((DocsPaWR.DatiTrasmissioneDocumento)this._datiDocumentiTrasmessi[documentIndex]).TrasmissioneConWorkflow;

            return retValue;
        }

        public string GetIdTrasmissione(int documentIndex)
        {
            string retValue = string.Empty;

            if (this._datiDocumentiTrasmessi.Count > 0)
                retValue = ((DocsPaWR.DatiTrasmissioneDocumento)this._datiDocumentiTrasmessi[documentIndex]).IDTrasmissione;

            return retValue;
        }

        public string GetIdTrasmissioneSingola(int documentIndex)
        {
            string retValue = string.Empty;

            if (this._datiDocumentiTrasmessi.Count > 0)
                retValue = ((DocsPaWR.DatiTrasmissioneDocumento)this._datiDocumentiTrasmessi[documentIndex]).IDTrasmissioneSingola;

            return retValue;
        }

        public string GetIdTrasmissioneUtente(int documentIndex)
        {
            string retValue = string.Empty;

            if (this._datiDocumentiTrasmessi.Count > 0)
                retValue = ((DocsPaWR.DatiTrasmissioneDocumento)this._datiDocumentiTrasmessi[documentIndex]).IDTrasmissioneUtente;


            return retValue;
        }

        /// <summary>
        /// Prende le note generali associate alla trasmissione del documento corrente.
        /// </summary>
        /// <param name="documentIndex"></param>
        /// <returns></returns>
        public string GetNoteGenerali(int documentIndex)
        {
            string retValue = string.Empty;

            if (this._datiDocumentiTrasmessi.Count > 0)
                retValue = ((DocsPaWR.DatiTrasmissioneDocumento)this._datiDocumentiTrasmessi[documentIndex]).NoteGenerali;

            return retValue;
        }

        public string GetNoteIndividuali(int documentIndex)
        {
            string retValue = string.Empty;

            if (this._datiDocumentiTrasmessi.Count > 0)
                retValue = ((DocsPaWR.DatiTrasmissioneDocumento)this._datiDocumentiTrasmessi[documentIndex]).NoteIndividualiTrasmSingola;

            return retValue;
        }

        public string GetDescRagioneTrasm(int documentIndex)
        {
            string retValue = string.Empty;

            if (this._datiDocumentiTrasmessi.Count > 0)
                retValue = ((DocsPaWR.DatiTrasmissioneDocumento)this._datiDocumentiTrasmessi[documentIndex]).DescRagioneTrasmissione;

            return retValue;
        }

        public bool RifiutaDoc(string notaRifiuto)
        {
            string IDTrasmUtente = GetIdTrasmissioneUtente(GetCurrentDocumentPosition() - 1);
            string idTrasmissione = ((DocsPaWR.DatiTrasmissioneDocumento)this._datiDocumentiTrasmessi[GetCurrentDocumentPosition() - 1]).IDTrasmissione;
            string idPeople = this._infoUtente.idPeople;
            return RifiutaDocumento(notaRifiuto, IDTrasmUtente, idTrasmissione, idPeople);
        }

        public bool ScartaDoc()
        {
            //string IDTrasmUtente = this.GetIdTrasmissioneUtente(GetCurrentDocumentPosition()-1);						
            //bool trasmConWorkflow = ((DocsPAWA.DocsPaWR.DatiTrasmissioneDocumento) this._datiDocumentiTrasmessi[GetCurrentDocumentPosition()-1]).TrasmissioneConWorkflow;
            //string idPeople = this._infoUtente.idPeople;
            //string idTrasmissione = ((DocsPAWA.DocsPaWR.DatiTrasmissioneDocumento)this._datiDocumentiTrasmessi[GetCurrentDocumentPosition() - 1]).IDTrasmissione;
            //return ScartaDocumento(IDTrasmUtente,trasmConWorkflow, idPeople, idTrasmissione);

            //new
            //nuovi parametri per la chiamata alla nuova procedura
            string idOggetto = ((DocsPaWR.DatiTrasmissioneDocumento)this._datiDocumentiTrasmessi[GetCurrentDocumentPosition() - 1]).IDDocumento;
            string tipoOggetto = "D";
            string idTrasmissione = ((DocsPaWR.DatiTrasmissioneDocumento)this._datiDocumentiTrasmessi[GetCurrentDocumentPosition() - 1]).IDTrasmissione;
            string idTrasmsingola = ((DocsPaWR.DatiTrasmissioneDocumento)this._datiDocumentiTrasmessi[GetCurrentDocumentPosition() - 1]).IDTrasmissioneSingola;

            DocsPaWR.RuoloSmistamento ruolo = new DocsPaWR.RuoloSmistamento();
            ruolo.ID = this._ruolo.systemId;


            return ScartaDocumentoSP(this._infoUtente, idOggetto, tipoOggetto, idTrasmissione, idTrasmsingola, this._mittenteSmistamento, ruolo);
        }


        public DocsPaWR.EsitoSmistamentoDocumento[] SmistaDocumento()
        {
            DocsPaWR.DocsPaWebService ws = new DocsPaWR.DocsPaWebService();
            //Andrea - Evita lancio eccezione per lo scadere del timeout
            //ws.Timeout = System.Threading.Timeout.Infinite;
            //End Andrea
            return ws.SmistaDocumento(
                                this._mittenteSmistamento,
                                this._infoUtente,
                                this._currentDocument,
                                (DocsPaWR.DatiTrasmissioneDocumento)this._datiDocumentiTrasmessi[this.GetCurrentDocumentPosition() - 1],
                                this._uoAppartenenza,
                                this._uoInferiori,
                                utils.getHttpFullPath());
        }

        public void setNoteGenarali(string value)
        {
            ((DocsPaWR.DatiTrasmissioneDocumento)this._datiDocumentiTrasmessi[this.GetCurrentDocumentPosition() - 1]).NoteGenerali = value;
        }

        /// <summary>
        /// verifica l'esistenza delle ragioni di trasmissione: COMPETENZA e CONOSCENZA
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <returns>TRUE: esistono le ragioni di trasmissione per lo smistamento; FALSE: non esistono</returns>
        public bool CheckExistRagTrasm(DocsPaWR.InfoUtente infoUtente)
        {
            return this.verificaRagTrasmSmistamento(infoUtente);
        }

        public DocsPaWR.RagioneTrasmissione[] GetListaRagioniSmistamento(DocsPaWR.InfoUtente infoUtente)
        {
            return SmistamentoGetListaRagioni(infoUtente);
        }

        public bool IsEnabledNavigazioneUO()
        {
            return (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["SMISTA_NAVIGA_UO"]) && System.Configuration.ConfigurationManager.AppSettings["SMISTA_NAVIGA_UO"].Equals("1"));
        }

        public void FillUOInf_NavigaUO(string idUO,
                                        DocsPaWR.Ruolo ruolo,
                                        DocsPaWR.Utente utente,
                                        DocsPaWR.InfoUtente infoUtente)
        {
            FillMittenteSmistamentoNavigaUO(ruolo, utente, infoUtente);
            // FillUOInf(idUO);
        }

        public void FillCurrentUO_NavigaUO(string idUO,
                                        DocsPaWR.Ruolo ruolo,
                                        DocsPaWR.Utente utente,
                                        DocsPaWR.InfoUtente infoUtente)
        {
            FillMittenteSmistamentoNavigaUO(ruolo, utente, infoUtente);
            //DocsPaWR.DocsPaWebService ws = new DocsPAWA.DocsPaWR.DocsPaWebService();
            //if (this._uoAppartenenza == null || (this._uoAppartenenza !=null 
            //    &&  !this._uoAppartenenza.ID.Equals(idUO)))
            //questo if serve altrimenti ricaricando dal server si perde le eventuali
            //selezioni su ruoli e/o utenti
            //   {
            //     this._uoAppartenenza = ws.GetUOAppartenenza(idUO, this._mittenteSmistamento, true);
            // }
        }

        public ArrayList ListaIDUOParent(string idUO)
        {
            OrganigrammaManager ammMng = new OrganigrammaManager();
            ammMng.ListaIDParentRicerca(idUO, "U");
            ArrayList lista = new ArrayList();
            Object[] list = ammMng.getListaIDParentRicerca();
            for (int i = 0; i < list.Length; i++)
            {
                lista.Add(list[i]);
            }
            return lista;
        }

        public void FillDestinatariDefault(DocsPaWR.Ruolo ruolo,
                                DocsPaWR.Utente utente,
                                DocsPaWR.InfoUtente infoUtente)
        {
            this._ruolo = ruolo;
            this._utente = utente;
            this._infoUtente = infoUtente;
            this.FillMittenteSmistamento();
            this.FillUO();
        }

        public bool ExistUoInf(string idUO,
                                DocsPaWR.Ruolo ruolo,
                                DocsPaWR.Utente utente,
                                DocsPaWR.InfoUtente infoUtente)
        {
            FillMittenteSmistamentoNavigaUO(ruolo, utente, infoUtente);
            return this.existUOInf(idUO);
        }

        #endregion

        #region Private

        /// <summary>
        /// Carica array con tutte le system_id delle trasmissioni
        /// </summary>
        /// <param name="filtriRicerca"></param>
        private void FillArrayDocumentiTrasmessi(DocsPaWR.FiltroRicerca[] filtriRicerca)
        {

            DocsPaWR.DatiTrasmissioneDocumento[] list = null;
            this._datiDocumentiTrasmessi = new ArrayList();

            //if (filtriRicerca != null)
            //    list = docsPaWS.GetListDocumentiTrasmessiNotifyFilters(this._mittenteSmistamento, filtriRicerca);
            //else
            //{
            //list = docsPaWS.GetListDocumentiTrasmessiNotify(this._mittenteSmistamento);

            if (NotificationManager.ListNotifyFiltered != null && NotificationManager.ListNotifyFiltered.Count > 0)
            {
                List<Notification> notifications = NotificationManager.ListNotifyFiltered.Where(e => NotificationManager.EventType.TRASM == NotificationManager.GetTypeEvent(e.TYPE_EVENT) && e.COLOR.Equals(NotificationManager.Color.BLUE) && !string.IsNullOrEmpty(e.ID_OBJECT) && e.DOMAINOBJECT == FiltersNotifications.DOCUMENT).ToList();

                if (notifications != null && notifications.Count > 0)
                {
                    list = docsPaWS.GetListDocumentiTrasmessiNotify(notifications.ToArray<Notification>(), this._mittenteSmistamento);
                    //DatiTrasmissioneDocumento transmData = new DatiTrasmissioneDocumento();
                    //foreach (Notification temp in notifications)
                    //{
                    //    transmData = new DatiTrasmissioneDocumento();
                    //    transmData.IDDocumento = temp.ID_OBJECT;
                    //    transmData.IDTrasmissioneSingola = temp.ID_SPECIALIZED_OBJECT;
                    //    transmData.DescRagioneTrasmissione = string.Empty;
                    //    transmData.
                    //}
                    this._datiDocumentiTrasmessi = new ArrayList(list);
                }

            
                
            }


            //if (NotificationManager.EventType.TRASM == NotificationManager.GetTypeEvent(a.TYPE_EVENT))
            //{

            //}

           
        }

        /// <summary>
        /// Info del singolo documento trasmesso
        /// </summary>
        /// <returns></returns>
        private DocsPaWR.DocumentoSmistamento GetDocumentoTrasmesso(bool content)
        {
            DocsPaWR.DocumentoSmistamento retValue = null;
            if (this._datiDocumentiTrasmessi.Count > 0)
            {
                string idDocumentoTrasmesso = ((DocsPaWR.DatiTrasmissioneDocumento)this._datiDocumentiTrasmessi[this._currentDocumentIndex - 1]).IDDocumento;

                if (PdfConverterInfo.ShowDocumentAsPdfFormat)
                {
                    // Conversione pdf inline del documento
                    bool pdfConverted;
                    retValue = docsPaWS.GetDocumentoSmistamentoAsPdf(idDocumentoTrasmesso, this._infoUtente, content, out pdfConverted);
                }
                else
                {
                   // retValue = docsPaWS.GetDocumentoSmistamento(idDocumentoTrasmesso, this._infoUtente, content);
                    retValue = docsPaWS.GetDocumentoSmistamento(idDocumentoTrasmesso, this._infoUtente, false);
                }
            }

            return retValue;
        }

        /// <summary>
        /// Aggiornamento stato dell'oggetto
        /// </summary>
        public void RefreshUO()
        {
            this.FillUO();
        }

        /// <summary>
        /// Lettura Unità Organizzative (destinazione)
        /// </summary>
        private void FillUO()
        {
            FillUOApp();
            FillUOInf(this._uoAppartenenza.ID);
            this._uoAppartenenza.UoInferiori = this._uoInferiori;
            this._uoAppartenenza.Selezionata = true;
            //if (this._uoInferiori!=null && this._uoInferiori.Length > 0)
            //{
            //    foreach (DocsPaWR.UOSmistamento uo in this._uoInferiori)
            //    {
            //        creaGerarchia(uo);
            //    }
            //}             
        }

        private void FillUOApp()
        {
            DocsPaWR.DocsPaWebService ws = new DocsPaWR.DocsPaWebService();
            this._uoAppartenenza = ws.GetUOAppartenenza(this._infoUtente.idCorrGlobali, this._mittenteSmistamento, false);
        }

        public List<string> GetIdDestinatariTrasmDocInUo(string idUo, string docnumber)
        {
            DocsPaWR.DocsPaWebService ws = new DocsPaWR.DocsPaWebService();
            try
            {
                return ws.GetIdDestinatariTrasmDocInUo(idUo, docnumber).ToList();
            }
            catch(Exception e)
            {
                return null;
            }
        }

        private void FillUOInf(string idUO)
        {
            DocsPaWR.DocsPaWebService ws = new DocsPaWR.DocsPaWebService();
            this._uoInferiori = ws.GetUOInferiori(idUO, this._mittenteSmistamento);
        }

        private bool RifiutaDocumento(string notaRifiuto, string IDTrasmUtente, string idTrasmissione, string idPeople)
        {
            bool retValue = false;
            DocsPaWR.DocsPaWebService ws = new DocsPaWR.DocsPaWebService();
            if (retValue = ws.RifiutaDocumento(notaRifiuto, IDTrasmUtente, idTrasmissione, idPeople, RoleManager.GetRoleInSession(), UserManager.GetInfoUser()))
            {
                // creazione oggetto TrasmissioneUtente per la gestione del ritorno al mitt della trasm. rifiutata
                this.FillMittenteSmistamento();

                DocsPaWR.TrasmissioneUtente objTrasmUt = new DocsPaWR.TrasmissioneUtente();
                objTrasmUt.systemId = IDTrasmUtente;

                DocsPaWR.Utente objUtente = new DocsPaWR.Utente();
                objTrasmUt.utente = objUtente;
                objTrasmUt.utente.idPeople = this._mittenteSmistamento.IDPeople;
                objTrasmUt.utente.idAmministrazione = this._mittenteSmistamento.IDAmministrazione;

                DocsPaWR.Ruolo objRuolo = new DocsPaWR.Ruolo();
                objRuolo.systemId = this._mittenteSmistamento.IDCorrGlobaleRuolo;
                objTrasmUt.utente.ruoli = new DocsPaWR.Ruolo[1];
                objTrasmUt.utente.ruoli[0] = objRuolo;

                DocsPaWR.InfoUtente infoUtente = UserManager.GetInfoUser();

                // ws.RitornaAlMittTrasmUt(objTrasmUt, infoUtente);
            }
            return retValue;
        }

        private bool ScartaDocumentoSP(DocsPaWR.InfoUtente infoUtente, string idOggetto, string tipoOggetto, string idTrasmissione, string idTrasmSingola, DocsPaWR.MittenteSmistamento mittente, DocsPaWR.RuoloSmistamento ruolo)
        {
            bool retValue = false;
            DocsPaWR.DocsPaWebService ws = new DocsPaWR.DocsPaWebService();
            retValue = ws.ScartaDocumento(infoUtente, idOggetto, tipoOggetto, idTrasmissione, idTrasmSingola, mittente, ruolo);
            return retValue;
        }

        /// <summary>
        /// Creazione oggetto MittenteSmistamento
        /// </summary>
        private void FillMittenteSmistamento()
        {
            this._mittenteSmistamento = new DocsPaWR.MittenteSmistamento();
            this._mittenteSmistamento.IDPeople = this._infoUtente.idPeople;
            this._mittenteSmistamento.IDAmministrazione = this._infoUtente.idAmministrazione;

            string[] registriApp = new string[this._ruolo.registri.Length];
            for (int i = 0; i < this._ruolo.registri.Length; i++)
            {
                registriApp[i] = this._ruolo.registri[i].systemId;
            }
            this._mittenteSmistamento.RegistriAppartenenza = registriApp;
            registriApp = null;


            this._mittenteSmistamento.EMail = this._utente.email;
            this._mittenteSmistamento.IDCorrGlobaleRuolo = this._ruolo.systemId;
            this._mittenteSmistamento.IDGroup = this._ruolo.idGruppo;
            this._mittenteSmistamento.LivelloRuolo = this._ruolo.livello;

        }

        private void FillMittenteSmistamentoNavigaUO(DocsPaWR.Ruolo ruolo,
                                                        DocsPaWR.Utente utente,
                                                        DocsPaWR.InfoUtente infoUtente)
        {
            this._mittenteSmistamento = new DocsPaWR.MittenteSmistamento();
            this._mittenteSmistamento.IDPeople = infoUtente.idPeople;
            this._mittenteSmistamento.IDAmministrazione = infoUtente.idAmministrazione;

            string[] registriApp = new string[ruolo.registri.Length];
            for (int i = 0; i < ruolo.registri.Length; i++)
            {
                registriApp[i] = ruolo.registri[i].systemId;
            }
            this._mittenteSmistamento.RegistriAppartenenza = registriApp;
            registriApp = null;

            this._mittenteSmistamento.EMail = utente.email;
            this._mittenteSmistamento.IDCorrGlobaleRuolo = ruolo.systemId;
            this._mittenteSmistamento.IDGroup = ruolo.idGruppo;
            this._mittenteSmistamento.LivelloRuolo = ruolo.livello;
        }

        /// <summary>
        /// verifica l'esistenza delle ragioni di trasmissione: COMPETENZA e CONOSCENZA
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <returns>TRUE: esistono le ragioni di trasmissione per lo smistamento; FALSE: non esistono</returns>
        private bool verificaRagTrasmSmistamento(DocsPaWR.InfoUtente infoUtente)
        {
            bool retValue = false;

            this._infoUtente = infoUtente;
            DocsPaWR.DocsPaWebService ws = new DocsPaWR.DocsPaWebService();

            retValue = ws.VerificaRagTrasmSmista(this._infoUtente.idAmministrazione);

            return retValue;
        }

        private DocsPaWR.RagioneTrasmissione[] SmistamentoGetListaRagioni(DocsPaWR.InfoUtente infoUtente)
        {
            DocsPaWR.RagioneTrasmissione[] listaRagSmista;

            this._infoUtente = infoUtente;
            DocsPaWR.DocsPaWebService ws = new DocsPaWR.DocsPaWebService();

            listaRagSmista = ws.SmistamentoGetRagioniTrasmissione(this._infoUtente.idAmministrazione);

            return listaRagSmista;
        }

        private bool existUOInf(string idUO)
        {
            DocsPaWR.DocsPaWebService ws = new DocsPaWR.DocsPaWebService();
            return ws.SmistamentoExistUOInf(idUO, this._mittenteSmistamento);
        }

        private void creaGerarchia(DocsPaWR.UOSmistamento UoInferiore)
        {
            DocsPaWR.DocsPaWebService ws = new DocsPaWR.DocsPaWebService();
            DocsPaWR.UOSmistamento[] UoInf = ws.GetUOInferiori(UoInferiore.ID, this._mittenteSmistamento);
            UoInferiore.UoInferiori = UoInf;
            if (UoInferiore.UoInferiori != null && UoInferiore.UoInferiori.Length > 0)
            {
                foreach (DocsPaWR.UOSmistamento uo in UoInferiore.UoInferiori)
                {
                    creaGerarchia(uo);
                }
            }
        }
        #endregion

        #region Gestione filtri

        /// <summary>
        /// Creazione oggetti filtro
        /// </summary>
        /// <returns></returns>
        public DocsPaWR.FiltroRicerca[] GetFilters()
        {
            ArrayList filterItems = new ArrayList();
            bool result = true;
            if (this.Filters == null) result = false;
            if (result)
            {
                // Filtri per mittente trasmissione
                this.AddFilterMittente(filterItems);

                // Filtri per oggetto
                this.AddFilterOggettoDocumento(filterItems);

                // Filtri per data trasmissione
                result = this.AddFilterDataTrasmissioneDocumento(filterItems);

                //Filtri per trasmissioni in attesa di accettazione
                this.AddFilterTrasmissioniAccettate(filterItems);

                //Filtro tipoDocumento
                this.AddFilterTipoDocumento(filterItems);

                // Filtri per documenti acquisiti
                this.AddFilterDocumentiAcquisiti(filterItems);

                // Filtri per documenti firmati
                this.AddFilterDocumentiFirmati(filterItems);

                //filtri per tipo file acquisito
                this.AddFilterTipoFileAcquisito(filterItems);

                if (result)
                    // Filtri per data scadenza trasmissione
                    result = this.AddFilterDataScadenzaTrasmissione(filterItems);
            }

            if (result)
                return (DocsPaWR.FiltroRicerca[])filterItems.ToArray(typeof(DocsPaWR.FiltroRicerca));
            else
                return null;
        }

        /// <summary>
        /// Creazione oggetti di filtro per oggetto documento
        /// </summary>
        /// <param name="filterItems"></param>
        private void AddFilterTipoDocumento(ArrayList filterItems)
        {
            DocsPaWR.FiltroRicerca filterItem = new DocsPaWR.FiltroRicerca();
            filterItem.argomento = DocsPaWR.FiltriDocumento.TIPO.ToString();
            filterItem.valore = this.Filters.TYPE_DOCUMENT; //this.rbListTipoDocumento.SelectedValue;
            filterItems.Add(filterItem);
            filterItem = null;
        }

        /// <summary>
        /// Creazione oggetti di filtro per data trasmissione documento
        /// </summary>
        /// <param name="filterItems"></param>
        private bool AddFilterDataTrasmissioneDocumento(ArrayList filterItems)
        {
            bool result = true;
            bool rangeFilterInterval = false; // = (this.cboTypeDataTrasmissione.SelectedValue == RANGE_FILTER_TYPE_INTERVAL);
            if (this.Filters.DATE_EVENT_TO != "" && this.Filters.DATE_EVENT_FROM != "") rangeFilterInterval = true;

            FiltroRicerca filterItem = null;

            if (this.Filters.DATE_EVENT_FROM != "")  //this.GetCalendarControl("txtInitDataTrasmissione").txt_Data.Text.Length > 0)
            {
                filterItem = new DocsPaWR.FiltroRicerca();

                if (rangeFilterInterval)
                    filterItem.argomento = DocsPaWR.FiltriTrasmissioneNascosti.TRASMISSIONE_SUCCESSIVA_AL.ToString();
                else
                    filterItem.argomento = DocsPaWR.FiltriTrasmissioneNascosti.TRASMISSIONE_IL.ToString();

                filterItem.valore = this.Filters.DATE_EVENT_FROM; //this.GetCalendarControl("txtInitDataTrasmissione").txt_Data.Text.Trim();
                filterItems.Add(filterItem);
                filterItem = null;
            }

            if (rangeFilterInterval && this.Filters.DATE_EVENT_TO != "") //this.GetCalendarControl("txtEndDataTrasmissione").txt_Data.Text.Length > 0)
            {
                filterItem = new DocsPaWR.FiltroRicerca();
                filterItem.argomento = DocsPaWR.FiltriTrasmissioneNascosti.TRASMISSIONE_PRECEDENTE_IL.ToString();
                filterItem.valore = this.Filters.DATE_EVENT_TO; //this.GetCalendarControl("txtEndDataTrasmissione").txt_Data.Text.Trim();
                filterItems.Add(filterItem);
                filterItem = null;
            }

            //if (this.cboTypeDataTrasmissione.SelectedIndex > 0 && !this.GetCalendarControl("txtInitDataTrasmissione").txt_Data.Text.Equals("") && !this.GetCalendarControl("txtEndDataTrasmissione").txt_Data.Text.Equals(""))
            if (rangeFilterInterval)
            {
                if (NttDataWA.Utils.utils.verificaIntervalloDate(this.Filters.DATE_EVENT_FROM, this.Filters.DATE_EVENT_TO)) //this.GetCalendarControl("txtInitDataTrasmissione").txt_Data.Text, this.GetCalendarControl("txtEndDataTrasmissione").txt_Data.Text))
                {
                    result = false;
                }
            }
            return result;
        }

        /// <summary>
        /// Creazione oggetti di filtro per data scadenza trasmissione documento
        /// </summary>
        /// <param name="filterItems"></param>
        private bool AddFilterDataScadenzaTrasmissione(ArrayList filterItems)
        {
            bool result = true;
            bool rangeFilterInterval = false; // = (this.cboTypeDataTrasmissione.SelectedValue == RANGE_FILTER_TYPE_INTERVAL);
            if (this.Filters.DATE_EXPIRE_TO != "" && this.Filters.DATE_EXPIRE_FROM != "") rangeFilterInterval = true;

            FiltroRicerca filterItem = null;

            if (this.Filters.DATE_EXPIRE_FROM != "")  //this.GetCalendarControl("txtInitDataTrasmissione").txt_Data.Text.Length > 0)
            {
                filterItem = new DocsPaWR.FiltroRicerca();

                if (rangeFilterInterval)
                    filterItem.argomento = DocsPaWR.FiltriTrasmissione.SCADENZA_SUCCESSIVA_AL.ToString();
                else
                    filterItem.argomento = DocsPaWR.FiltriTrasmissione.SCADENZA_IL.ToString();

                filterItem.valore = this.Filters.DATE_EXPIRE_FROM; //this.GetCalendarControl("txtInitDataTrasmissione").txt_Data.Text.Trim();
                filterItems.Add(filterItem);
                filterItem = null;
            }

            if (rangeFilterInterval && this.Filters.DATE_EXPIRE_TO != "") //this.GetCalendarControl("txtEndDataTrasmissione").txt_Data.Text.Length > 0)
            {
                filterItem = new DocsPaWR.FiltroRicerca();
                filterItem.argomento = DocsPaWR.FiltriTrasmissione.SCADENZA_PRECEDENTE_IL.ToString();
                filterItem.valore = this.Filters.DATE_EXPIRE_TO; //this.GetCalendarControl("txtEndDataTrasmissione").txt_Data.Text.Trim();
                filterItems.Add(filterItem);
                filterItem = null;
            }

            //if (this.cboTypeDataTrasmissione.SelectedIndex > 0 && !this.GetCalendarControl("txtInitDataTrasmissione").txt_Data.Text.Equals("") && !this.GetCalendarControl("txtEndDataTrasmissione").txt_Data.Text.Equals(""))
            if (rangeFilterInterval)
            {
                if (NttDataWA.Utils.utils.verificaIntervalloDate(this.Filters.DATE_EXPIRE_FROM, this.Filters.DATE_EXPIRE_TO)) //this.GetCalendarControl("txtInitDataTrasmissione").txt_Data.Text, this.GetCalendarControl("txtEndDataTrasmissione").txt_Data.Text))
                {
                    result = false;
                }
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="codRubrica"></param>
        /// <param name="tipoCorr"></param>
        /// <returns></returns>
        private string GetArgomento(bool codRubrica, string tipoCorr)
        {
            string argomento = string.Empty;

            switch (tipoCorr)
            {
                case "R":
                    if (codRubrica)
                        argomento = DocsPaWR.FiltriTrasmissioneNascosti.COD_RUBR_MITT_RUOLO.ToString();
                    else
                        argomento = DocsPaWR.FiltriTrasmissioneNascosti.MITTENTE_RUOLO.ToString();
                    break;

                case "P":
                    if (codRubrica)
                        argomento = DocsPaWR.FiltriTrasmissioneNascosti.COD_RUBR_MITT_UTENTE.ToString();
                    else
                        argomento = DocsPaWR.FiltriTrasmissioneNascosti.MITTENTE_UTENTE.ToString();
                    break;

                case "U":
                    if (codRubrica)
                        argomento = DocsPaWR.FiltriTrasmissioneNascosti.ID_UO_MITT.ToString();
                    else
                        argomento = DocsPaWR.FiltriTrasmissioneNascosti.MITTENTE_UO.ToString();
                    break;
            }

            return argomento;
        }

        /// <summary>
        /// Creazione filtro su mittente trasmissione
        /// </summary>
        /// <param name="filterItem"></param>
        private void AddFilterMittente(ArrayList filterItem)
        {
            FiltroRicerca filter = null;

            //if (!string.IsNullOrEmpty(this.txtCodiceUtenteMittente.Text))
            if (!string.IsNullOrEmpty(this.Filters.AUTHOR_SYSTEM_ID))
            {
                filter = new FiltroRicerca();
                filter.argomento = this.GetArgomento(true, this.Filters.AUTHOR_TYPE); //this.optListTipiMittente.SelectedValue);
                //if (!this.Filters.AUTHOR_TYPE.Equals("U"))
                filter.valore = this.Filters.AUTHOR_SYSTEM_ID; // this.txtCodiceUtenteMittente.Text.Trim();
                //else
                //    filter.valore = this.Filters.AUTHOR_SYSTEM_ID;  //this.txtSystemIdUtenteMittente.Value.Trim();
            }
            else if (!string.IsNullOrEmpty(this.Filters.AUTHOR_DESCRIPTION)) //this.txtDescrizioneUtenteMittente.Text))
            {
                filter = new DocsPaWR.FiltroRicerca();
                filter.argomento = this.GetArgomento(false, this.Filters.AUTHOR_TYPE); //this.optListTipiMittente.SelectedValue);
                filter.valore = this.Filters.AUTHOR_DESCRIPTION; //this.txtDescrizioneUtenteMittente.Text.Trim();
            }

            if (filter != null)
                filterItem.Add(filter);
        }

        /// <summary>
        /// Creazione oggetti di filtro per documenti acquisiti
        /// </summary>
        /// <param name="filterItems"></param>
        private void AddFilterDocumentiAcquisiti(ArrayList filterItems)
        {
            //if (this.chkAcquisiti.Checked)
            if (this.Filters.DOCUMENT_ACQUIRED)
            {
                DocsPaWR.FiltroRicerca filterItem = new DocsPaWR.FiltroRicerca();
                filterItem.argomento = DocsPaWR.FiltriTrasmissioneNascosti.DOCUMENTI_ACQUISITI.ToString();
                filterItem.valore = "1";
                filterItems.Add(filterItem);
                filterItem = null;
            }
        }

        /// <summary>
        /// Creazione oggetti di filtro per documenti firmati
        /// </summary>
        /// <param name="filterItems"></param>
        private void AddFilterDocumentiFirmati(ArrayList filterItems)
        {
            //if (this.chkFirmati.Checked)
            if (this.Filters.DOCUMENT_SIGNED)
            {
                if (!this.Filters.DOCUMENT_UNSIGNED) //.cb_nonFirmato.Checked)
                {
                    DocsPaWR.FiltroRicerca filterItem = new DocsPaWR.FiltroRicerca();
                    filterItem.argomento = DocsPaWR.FiltriTrasmissioneNascosti.DOCUMENTI_FIRMATI.ToString();
                    filterItem.valore = "1";
                    filterItems.Add(filterItem);
                    filterItem = null;
                }
                else //se sono entrambi selezionati cerco i documenti che abbiano un file acquisito, siano essi firmati o meno.
                {
                    DocsPaWR.FiltroRicerca filterItem = new DocsPaWR.FiltroRicerca();
                    filterItem.argomento = DocsPaWR.FiltriTrasmissioneNascosti.DOCUMENTI_FIRMATI.ToString();
                    filterItem.valore = "2";
                    filterItems.Add(filterItem);
                    filterItem = null;
                }
            }
            else
            {
                if (this.Filters.DOCUMENT_UNSIGNED)  //this.cb_nonFirmato.Checked)
                {
                    DocsPaWR.FiltroRicerca filterItem = new DocsPaWR.FiltroRicerca();
                    filterItem.argomento = DocsPaWR.FiltriTrasmissioneNascosti.DOCUMENTI_FIRMATI.ToString();
                    filterItem.valore = "0";
                    filterItems.Add(filterItem);
                    filterItem = null;
                }
            }
        }

        //Creazione oggetti di filtro per tipo file acquisito

        private void AddFilterTipoFileAcquisito(ArrayList filterItems)
        {
            if (this.Filters.TYPE_FILE_ACQUIRED != "")  //this.ddl_tipoFileAcquisiti.SelectedIndex > 0)
            {
                DocsPaWR.FiltroRicerca filterItem = new DocsPaWR.FiltroRicerca();
                filterItem.argomento = DocsPaWR.FiltriDocumento.TIPO_FILE_ACQUISITO.ToString();
                filterItem.valore = this.Filters.TYPE_FILE_ACQUIRED;  //this.ddl_tipoFileAcquisiti.SelectedItem.Value;
                filterItems.Add(filterItem);
                filterItem = null;
            }
        }

        /// <summary>
        /// Creazione oggetti di filtro per trasmissioni in attesa di accettazione
        /// </summary>
        /// <param name="filterItems"></param>
        private void AddFilterTrasmissioniAccettate(ArrayList filterItems)
        {
            //if (this.chkAccettazione.Checked)
            if (this.Filters.PENDING == true)
            {
                //Session.Add("TrasmNonAccettate", "T");
                HttpContext.Current.Session.Add("TrasmNonAccettate", "T");
                DocsPaWR.FiltroRicerca filterItem = new DocsPaWR.FiltroRicerca();
                filterItem.argomento = DocsPaWR.FiltriTrasmissioneNascosti.TRASMISSIONI_ACCETTATE.ToString();
                filterItem.valore = "1";
                filterItems.Add(filterItem);
                filterItem = null;
            }
        }


        ///// <summary>
        ///// Creazione oggetti di filtro per oggetto trasmissione
        ///// </summary>
        ///// <param name="filterItems"></param>
        //private void AddFilterTipoOggettoTrasmissione(ArrayList filterItems)
        //{
        //    //if (!string.IsNullOrEmpty(this.TipoOggettoTrasmissione))
        //    //{
        //    DocsPaWR.FiltroRicerca filterItem = new DocsPAWA.DocsPaWR.FiltroRicerca();
        //    filterItem.argomento = DocsPaWR.FiltriTrasmissioneNascosti.TIPO_OGGETTO.ToString();
        //    if (this.chkDocumenti.Checked && !this.chkFasciscoli.Checked)
        //        filterItem.valore = "D";
        //    if (this.chkFasciscoli.Checked && !this.chkDocumenti.Checked)
        //        filterItem.valore = "F";
        //    if (this.chkFasciscoli.Checked && this.chkDocumenti.Checked)
        //        filterItem.valore = "T";
        //    filterItems.Add(filterItem);
        //    filterItem = null;

        //    //}
        //}

        /// <summary>
        /// Creazione oggetti di filtro per oggetto documento
        /// </summary>
        /// <param name="filterItems"></param>
        private void AddFilterOggettoDocumento(ArrayList filterItems)
        {
            if (!string.IsNullOrEmpty(this.Filters.OBJECT)) //this.txtOggetto.Text))
            {
                DocsPaWR.FiltroRicerca filterItem = new DocsPaWR.FiltroRicerca();

                filterItem.argomento = DocsPaWR.FiltriTrasmissioneNascosti.OGGETTO_DOCUMENTO_TRASMESSO.ToString();
                filterItem.valore = this.Filters.OBJECT; //this.txtOggetto.Text.Trim();
                filterItems.Add(filterItem);
                filterItem = null;

            }
        }

        #endregion

        public DocsPaWR.MittenteSmistamento getMittenteSmistamento()
        {
            return this._mittenteSmistamento;
        }

        public string getMittenteTrasmissione(int index)
        {
            string mittTrasm = ((DocsPaWR.DatiTrasmissioneDocumento)this._datiDocumentiTrasmessi[index]).MittenteTrasmissione;
            if (!string.IsNullOrEmpty(mittTrasm))
                return mittTrasm;
            else
                return "---";
        }
    }
}
