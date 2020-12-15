using System;
using System.Collections;

namespace DocsPAWA.smistaDoc
{
	/// <summary>
	/// Summary description for SmistaDocManager.
	/// </summary>
	public class SmistaDocManager
	{
		#region Declaration

		private int _currentDocumentIndex=1;
		private DocsPAWA.DocsPaWR.DocumentoSmistamento _currentDocument=null;

		private ArrayList _datiDocumentiTrasmessi=null;

		private DocsPAWA.DocsPaWR.Ruolo _ruolo=null;
		private DocsPAWA.DocsPaWR.Utente _utente=null;
		private DocsPAWA.DocsPaWR.InfoUtente _infoUtente=null;

		private DocsPAWA.DocsPaWR.MittenteSmistamento _mittenteSmistamento=null;
		
		private DocsPAWA.DocsPaWR.UOSmistamento _uoAppartenenza=null;
		private DocsPAWA.DocsPaWR.UOSmistamento[] _uoInferiori=null;
       

		#endregion

		#region Public

		public SmistaDocManager()
		{

		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="docNumber"></param>
		public SmistaDocManager(DocsPAWA.DocsPaWR.Ruolo ruolo,
								DocsPaWR.Utente utente,
								DocsPaWR.InfoUtente infoUtente,
								string docNumber)
		{			
			this.Initialize(ruolo,utente,infoUtente,docNumber);
		}
	

		/// <summary>
		/// 
		/// </summary>
		/// <param name="ruolo"></param>
		/// <param name="utente"></param>
		/// <param name="infoUtente"></param>
		public SmistaDocManager(DocsPAWA.DocsPaWR.Ruolo ruolo,
								DocsPaWR.Utente utente,
								DocsPaWR.InfoUtente infoUtente)
		{
			this.Initialize(ruolo,utente,infoUtente,string.Empty);
		}


		private void Initialize(DocsPAWA.DocsPaWR.Ruolo ruolo,
								DocsPaWR.Utente utente,
								DocsPaWR.InfoUtente infoUtente,
								string docNumber)
		{
			this._ruolo=ruolo;
			this._utente=utente;
			this._infoUtente=infoUtente;
            
			this.FillMittenteSmistamento();

			if (docNumber!=null && docNumber.Equals(string.Empty))
			{
				// caricamento dei system_id di tutte le trasmissioni ricevute dall'utente

                // Se sono stati impostati i filtri in todolist sulle trasmissioni,
                // vengono applicati anche nel reperimento dei dati dello smistamento

                DocsPaWR.FiltroRicerca[] filtriRicerca = null;
                if (ricercaTrasm.DialogFiltriRicercaTrasmissioni.CurrentFilters != null)
                    filtriRicerca = ricercaTrasm.DialogFiltriRicercaTrasmissioni.CurrentFilters;
				this.FillArrayDocumentiTrasmessi(filtriRicerca);
			}
			else
			{
				DocsPaWR.DatiTrasmissioneDocumento datiDoc=new DocsPAWA.DocsPaWR.DatiTrasmissioneDocumento();
				datiDoc.IDDocumento=docNumber;

				this._datiDocumentiTrasmessi=new ArrayList();
				this._datiDocumentiTrasmessi.Add(datiDoc);
				datiDoc=null;
			}

			// posizionamento sul primo documento in lista
			if (this.GetDocumentCount()>0)
				this.MoveFirstDocument();

			// lettura UO (destinazione)
			this.FillUO();
		}

		public DocsPAWA.DocsPaWR.DocumentoSmistamento GetCurrentDocument(bool content)
		{
			if (this._currentDocument==null || content)
				this._currentDocument=this.GetDocumentoTrasmesso(content);
			return this._currentDocument;
		}

		public DocsPAWA.DocsPaWR.UOSmistamento GetUOAppartenenza()
		{
			return this._uoAppartenenza;
		}
        public void SetUOAppartenenza(DocsPAWA.DocsPaWR.UOSmistamento uoApp)
        {
            this._uoAppartenenza=uoApp;
            
        }

		public DocsPAWA.DocsPaWR.UOSmistamento[] GetUOInferiori()
		{
			return this._uoInferiori;
		}
        public void SetUOInferiori(DocsPAWA.DocsPaWR.UOSmistamento[] uoInf)
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

//			this._idDocumentiTrasmessi.RemoveAt(this._currentDocumentIndex - 1);
//
//			this._tipoDocumentiTrasmessi.RemoveAt(this._currentDocumentIndex - 1);
//			this._idTrasmissione.RemoveAt(this._currentDocumentIndex - 1);
//			this._idTrasmissioneSingola.RemoveAt(this._currentDocumentIndex - 1);
//			this._idTrasmissioneUtente.RemoveAt(this._currentDocumentIndex - 1);
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
			bool retValue=false;

			if (this._currentDocumentIndex>=1)
			{
				this._currentDocument=null;
				this._currentDocumentIndex=1;
				retValue=true;
			}

			return retValue;
		}

		public bool MoveLastDocument()
		{
			bool retValue=false;

			if (this._currentDocumentIndex < this.GetDocumentCount())
			{
				this._currentDocument=null;
				this._currentDocumentIndex=this.GetDocumentCount();
				retValue=true;
			}

			return retValue;
		}

		public bool MoveNextDocument()
		{
			bool retValue=false;

			if (this._currentDocumentIndex<this.GetDocumentCount())
			{
				this._currentDocument=null;
				this._currentDocumentIndex++;
				retValue=true;
			}

			return retValue;
		}

		public bool MovePreviousDocument()
		{
			bool retValue=false;

			if (this._currentDocumentIndex>1)
			{
				this._currentDocument=null;
				this._currentDocumentIndex--;
				retValue=true;
			}

			return retValue;
		}

		public bool MoveAbsoluteDocument(int documentIndex)
		{
			bool retValue=false;

			if (documentIndex >= 1 && 
				documentIndex <= this._datiDocumentiTrasmessi.Count)
			{
				this._currentDocument=null;
				this._currentDocumentIndex=documentIndex;
				retValue=true;
			}

			return retValue;
		}

		public bool IsTrasmissioneConWorkflow(int documentIndex)
		{
			bool retValue=false;

			if(this._datiDocumentiTrasmessi.Count>0)
				retValue=((DocsPAWA.DocsPaWR.DatiTrasmissioneDocumento) this._datiDocumentiTrasmessi[documentIndex]).TrasmissioneConWorkflow;
				
			return retValue;
		}

		public string GetIdTrasmissione(int documentIndex)
		{
			string retValue = string.Empty;

			if(this._datiDocumentiTrasmessi.Count>0)
				retValue=((DocsPAWA.DocsPaWR.DatiTrasmissioneDocumento) this._datiDocumentiTrasmessi[documentIndex]).IDTrasmissione;

			return retValue;
		}

		public string GetIdTrasmissioneSingola(int documentIndex)
		{
			string retValue = string.Empty;

			if(this._datiDocumentiTrasmessi.Count>0)
				retValue=((DocsPAWA.DocsPaWR.DatiTrasmissioneDocumento) this._datiDocumentiTrasmessi[documentIndex]).IDTrasmissioneSingola;

			return retValue;
		}

		public string GetIdTrasmissioneUtente(int documentIndex)
		{
			string retValue = string.Empty;

			if(this._datiDocumentiTrasmessi.Count>0)
				retValue=((DocsPAWA.DocsPaWR.DatiTrasmissioneDocumento) this._datiDocumentiTrasmessi[documentIndex]).IDTrasmissioneUtente;


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
                retValue = ((DocsPAWA.DocsPaWR.DatiTrasmissioneDocumento)this._datiDocumentiTrasmessi[documentIndex]).NoteGenerali;

            return retValue;
        }

        public string GetNoteIndividuali(int documentIndex)
        {
            string retValue = string.Empty;

            if (this._datiDocumentiTrasmessi.Count > 0)
                retValue = ((DocsPAWA.DocsPaWR.DatiTrasmissioneDocumento)this._datiDocumentiTrasmessi[documentIndex]).NoteIndividualiTrasmSingola;

            return retValue;
        }

        public string GetDescRagioneTrasm(int documentIndex)
        {
            string retValue = string.Empty;

            if (this._datiDocumentiTrasmessi.Count > 0)
                retValue = ((DocsPAWA.DocsPaWR.DatiTrasmissioneDocumento)this._datiDocumentiTrasmessi[documentIndex]).DescRagioneTrasmissione;

            return retValue;
        }

		public bool RifiutaDoc(string notaRifiuto)
		{
			string IDTrasmUtente = GetIdTrasmissioneUtente(GetCurrentDocumentPosition()-1);
            string idTrasmissione = ((DocsPAWA.DocsPaWR.DatiTrasmissioneDocumento)this._datiDocumentiTrasmessi[GetCurrentDocumentPosition() - 1]).IDTrasmissione;
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
            string idOggetto = ((DocsPAWA.DocsPaWR.DatiTrasmissioneDocumento)this._datiDocumentiTrasmessi[GetCurrentDocumentPosition() - 1]).IDDocumento;
            string tipoOggetto = "D";
            string idTrasmissione = ((DocsPAWA.DocsPaWR.DatiTrasmissioneDocumento)this._datiDocumentiTrasmessi[GetCurrentDocumentPosition() - 1]).IDTrasmissione;
            string idTrasmsingola = ((DocsPAWA.DocsPaWR.DatiTrasmissioneDocumento)this._datiDocumentiTrasmessi[GetCurrentDocumentPosition() - 1]).IDTrasmissioneSingola;

            DocsPaWR.RuoloSmistamento ruolo = new DocsPAWA.DocsPaWR.RuoloSmistamento();
            ruolo.ID = this._ruolo.systemId;


            return ScartaDocumentoSP(this._infoUtente, idOggetto, tipoOggetto, idTrasmissione, idTrasmsingola, this._mittenteSmistamento, ruolo);
        }


        public DocsPAWA.DocsPaWR.EsitoSmistamentoDocumento[] SmistaDocumento()
		{
			DocsPaWR.DocsPaWebService ws=new DocsPAWA.DocsPaWR.DocsPaWebService();
            //Andrea - Evita lancio eccezione per lo scadere del timeout
            //ws.Timeout = System.Threading.Timeout.Infinite;
            //End Andrea
            return ws.SmistaDocumento(
                                this._mittenteSmistamento,
                                this._infoUtente,
                                this._currentDocument,
                                (DocsPAWA.DocsPaWR.DatiTrasmissioneDocumento)this._datiDocumentiTrasmessi[this.GetCurrentDocumentPosition() - 1],
                                this._uoAppartenenza,
                                this._uoInferiori,
                                DocsPAWA.Utils.getHttpFullPath());
		}

        public void setNoteGenarali(string value)
        {
            ((DocsPAWA.DocsPaWR.DatiTrasmissioneDocumento)this._datiDocumentiTrasmessi[this.GetCurrentDocumentPosition() - 1]).NoteGenerali = value;
        }

		/// <summary>
		/// verifica l'esistenza delle ragioni di trasmissione: COMPETENZA e CONOSCENZA
		/// </summary>
		/// <param name="infoUtente"></param>
		/// <returns>TRUE: esistono le ragioni di trasmissione per lo smistamento; FALSE: non esistono</returns>
		public bool CheckExistRagTrasm(DocsPAWA.DocsPaWR.InfoUtente infoUtente)
		{
			return this.verificaRagTrasmSmistamento(infoUtente);
		}

        public DocsPaWR.RagioneTrasmissione[] GetListaRagioniSmistamento(DocsPAWA.DocsPaWR.InfoUtente infoUtente)
        {
            return SmistamentoGetListaRagioni(infoUtente);
        }

        public bool IsEnabledNavigazioneUO()
        {
            return (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["SMISTA_NAVIGA_UO"]) && System.Configuration.ConfigurationManager.AppSettings["SMISTA_NAVIGA_UO"].Equals("1"));
        }

        public void FillUOInf_NavigaUO(string idUO, 
                                        DocsPAWA.DocsPaWR.Ruolo ruolo,
                                        DocsPaWR.Utente utente,
                                        DocsPaWR.InfoUtente infoUtente)
        {            
            FillMittenteSmistamentoNavigaUO(ruolo,utente,infoUtente);
           // FillUOInf(idUO);
        }

        public void FillCurrentUO_NavigaUO(string idUO, 
                                        DocsPAWA.DocsPaWR.Ruolo ruolo,
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
            Amministrazione.Manager.OrganigrammaManager ammMng = new Amministrazione.Manager.OrganigrammaManager();
            ammMng.ListaIDParentRicerca(idUO, "U");
            ArrayList lista = ammMng.getListaIDParentRicerca();
            return lista;
        }

        public void FillDestinatariDefault(DocsPAWA.DocsPaWR.Ruolo ruolo,
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
                                DocsPAWA.DocsPaWR.Ruolo ruolo,
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
			DocsPaWR.DocsPaWebService ws=new DocsPAWA.DocsPaWR.DocsPaWebService();

            DocsPaWR.DatiTrasmissioneDocumento[] list = null;
            
            if (filtriRicerca!=null)
                list = ws.GetListDocumentiTrasmessiFilters(this._mittenteSmistamento, filtriRicerca);
            else
                list = ws.GetListDocumentiTrasmessi(this._mittenteSmistamento);
		
			this._datiDocumentiTrasmessi=new ArrayList(list);
			list=null;

			ws=null;
		}

		/// <summary>
		/// Info del singolo documento trasmesso
		/// </summary>
		/// <returns></returns>
		private DocsPAWA.DocsPaWR.DocumentoSmistamento GetDocumentoTrasmesso(bool content)
		{
            DocsPaWR.DocumentoSmistamento retValue = null;
            if (this._datiDocumentiTrasmessi.Count > 0)
            {
                string idDocumentoTrasmesso = ((DocsPAWA.DocsPaWR.DatiTrasmissioneDocumento)this._datiDocumentiTrasmessi[this._currentDocumentIndex - 1]).IDDocumento;

                DocsPaWR.DocsPaWebService ws = new DocsPAWA.DocsPaWR.DocsPaWebService();

                if (PdfConverterInfo.ShowDocumentAsPdfFormat)
                {
                    // Conversione pdf inline del documento
                    bool pdfConverted;
                    retValue = ws.GetDocumentoSmistamentoAsPdf(idDocumentoTrasmesso, this._infoUtente, content, out pdfConverted);
                }
                else
                {
                    retValue = ws.GetDocumentoSmistamento(idDocumentoTrasmesso, this._infoUtente, content);
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
            DocsPaWR.DocsPaWebService ws = new DocsPAWA.DocsPaWR.DocsPaWebService();
            this._uoAppartenenza = ws.GetUOAppartenenza(this._infoUtente.idCorrGlobali, this._mittenteSmistamento, false);
        }

        private void FillUOInf(string idUO)
        {
            DocsPaWR.DocsPaWebService ws = new DocsPAWA.DocsPaWR.DocsPaWebService();
            this._uoInferiori = ws.GetUOInferiori(idUO, this._mittenteSmistamento);			
        }

		private bool RifiutaDocumento(string notaRifiuto, string IDTrasmUtente, string idTrasmissione, string idPeople)
		{
			bool retValue = false;
			DocsPaWR.DocsPaWebService ws=new DocsPAWA.DocsPaWR.DocsPaWebService();
            if (ws.RifiutaDocumento(notaRifiuto, IDTrasmUtente, idTrasmissione, idPeople, UserManager.getRuolo(), UserManager.getInfoUtente()))
			{
				// creazione oggetto TrasmissioneUtente per la gestione del ritorno al mitt della trasm. rifiutata
				this.FillMittenteSmistamento();

				DocsPaWR.TrasmissioneUtente objTrasmUt = new DocsPAWA.DocsPaWR.TrasmissioneUtente();				
				objTrasmUt.systemId = IDTrasmUtente;

				DocsPaWR.Utente objUtente = new DocsPAWA.DocsPaWR.Utente();
				objTrasmUt.utente = objUtente;
				objTrasmUt.utente.idPeople = this._mittenteSmistamento.IDPeople;
				objTrasmUt.utente.idAmministrazione = this._mittenteSmistamento.IDAmministrazione;
				
				DocsPaWR.Ruolo objRuolo = new DocsPAWA.DocsPaWR.Ruolo();
				objRuolo.systemId=this._mittenteSmistamento.IDCorrGlobaleRuolo;
				objTrasmUt.utente.ruoli=new DocsPAWA.DocsPaWR.Ruolo[1];
				objTrasmUt.utente.ruoli[0]=objRuolo;

                DocsPAWA.DocsPaWR.InfoUtente infoUtente = UserManager.getInfoUtente();

                retValue = ws.RitornaAlMittTrasmUt(objTrasmUt, infoUtente);
			}
			return retValue;
		}

        //private bool ScartaDocumento(string IDTrasmUtente, bool trasmConWorkflow, string idPeople, string idTrasmissione)
        //{
        //    bool retValue = false;
        //    DocsPaWR.DocsPaWebService ws=new DocsPAWA.DocsPaWR.DocsPaWebService();
        //    retValue = ws.ScartaDocumento(IDTrasmUtente, trasmConWorkflow, idPeople, idTrasmissione);
        //    return retValue;
        //}

        private bool ScartaDocumentoSP(DocsPaWR.InfoUtente infoUtente, string idOggetto, string tipoOggetto, string idTrasmissione, string idTrasmSingola, DocsPaWR.MittenteSmistamento mittente, DocsPaWR.RuoloSmistamento ruolo)
        {
            bool retValue = false;
            DocsPaWR.DocsPaWebService ws = new DocsPAWA.DocsPaWR.DocsPaWebService();
            retValue = ws.ScartaDocumento(infoUtente, idOggetto, tipoOggetto, idTrasmissione, idTrasmSingola, mittente, ruolo);
            return retValue;
        }

		/// <summary>
		/// Creazione oggetto MittenteSmistamento
		/// </summary>
		private void FillMittenteSmistamento()
		{
			this._mittenteSmistamento=new DocsPAWA.DocsPaWR.MittenteSmistamento();
			this._mittenteSmistamento.IDPeople=this._infoUtente.idPeople;
			this._mittenteSmistamento.IDAmministrazione=this._infoUtente.idAmministrazione;

			string[] registriApp=new string[this._ruolo.registri.Length];
			for (int i=0;i<this._ruolo.registri.Length;i++)
			{
				registriApp[i]=this._ruolo.registri[i].systemId;
			}
			this._mittenteSmistamento.RegistriAppartenenza=registriApp;
			registriApp=null;

            
			this._mittenteSmistamento.EMail=this._utente.email;
			this._mittenteSmistamento.IDCorrGlobaleRuolo=this._ruolo.systemId;
			this._mittenteSmistamento.IDGroup=this._ruolo.idGruppo;
			this._mittenteSmistamento.LivelloRuolo=this._ruolo.livello;		

		}

        private void FillMittenteSmistamentoNavigaUO(DocsPAWA.DocsPaWR.Ruolo ruolo,
                                                        DocsPaWR.Utente utente,
                                                        DocsPaWR.InfoUtente infoUtente)
        {
            this._mittenteSmistamento = new DocsPAWA.DocsPaWR.MittenteSmistamento();
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
		private bool verificaRagTrasmSmistamento(DocsPAWA.DocsPaWR.InfoUtente infoUtente)
		{
			bool retValue = false;

			this._infoUtente=infoUtente;	
			DocsPaWR.DocsPaWebService ws = new DocsPAWA.DocsPaWR.DocsPaWebService();

			retValue = ws.VerificaRagTrasmSmista(this._infoUtente.idAmministrazione);
			
			return retValue;
		}

        private DocsPaWR.RagioneTrasmissione[] SmistamentoGetListaRagioni(DocsPAWA.DocsPaWR.InfoUtente infoUtente)
		{
            DocsPaWR.RagioneTrasmissione[] listaRagSmista;

			this._infoUtente=infoUtente;	
			DocsPaWR.DocsPaWebService ws = new DocsPAWA.DocsPaWR.DocsPaWebService();

            listaRagSmista = ws.SmistamentoGetRagioniTrasmissione(this._infoUtente.idAmministrazione);

            return listaRagSmista;
		}

        private bool existUOInf(string idUO)
        {
            DocsPaWR.DocsPaWebService ws = new DocsPAWA.DocsPaWR.DocsPaWebService();
            return ws.SmistamentoExistUOInf(idUO, this._mittenteSmistamento);
        }

        private void creaGerarchia(DocsPaWR.UOSmistamento UoInferiore)
        {
            DocsPaWR.DocsPaWebService ws = new DocsPAWA.DocsPaWR.DocsPaWebService();
            DocsPaWR.UOSmistamento[] UoInf = ws.GetUOInferiori(UoInferiore.ID, this._mittenteSmistamento);
            UoInferiore.UoInferiori = UoInf;
            if (UoInferiore.UoInferiori!=null && UoInferiore.UoInferiori.Length > 0)
            {
                foreach (DocsPaWR.UOSmistamento uo in UoInferiore.UoInferiori)
                {
                    creaGerarchia(uo);
                }
            }
        }
		#endregion

        public DocsPAWA.DocsPaWR.MittenteSmistamento getMittenteSmistamento()
        {
            return this._mittenteSmistamento;
        }

        public string getMittenteTrasmissione(int index)
        {
            string mittTrasm = ((DocsPAWA.DocsPaWR.DatiTrasmissioneDocumento)this._datiDocumentiTrasmessi[index]).MittenteTrasmissione;
            if (!string.IsNullOrEmpty(mittTrasm))
                return mittTrasm;
            else
                return "---";
        }
	}
}
