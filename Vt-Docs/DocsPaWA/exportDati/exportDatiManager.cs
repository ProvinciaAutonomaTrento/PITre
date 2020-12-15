using System;
using System.Collections;
using System.Data;
using System.Web.UI;
using System.Xml;
using System.Xml.Xsl;
using System.Xml.XPath;
using System.Collections.Generic;
using DocsPAWA.utils;
using DocsPAWA.DocsPaWR;
using System.Linq;
using DocsPAWA.SiteNavigation;

namespace DocsPAWA.exportDati
{
	/// <summary>
	/// Summary description for exportDatiManager.
	/// </summary>
	public class exportDatiManager
	{		
		#region VAR
		private string _oggettoExport = string.Empty;
		private string _tipologiaExport = string.Empty;		
		private string _titolo = string.Empty;

        private DocsPAWA.DocsPaWR.Folder _folder = null;
        private string codFasc = "";

		private DocsPAWA.DocsPaWR.FiltroRicerca[][] _lstFiltri = null;
		private DocsPAWA.DocsPaWR.FiltroRicerca[] _lstFiltriTrasm = null;
        private DocsPAWA.DocsPaWR.FullTextSearchContext _context = null;
		private DocsPAWA.DocsPaWR.Ruolo _userRuolo = null;
		private DocsPAWA.DocsPaWR.InfoUtente _userInfo = null;
		private DocsPAWA.DocsPaWR.Utente _user = null;
		private DocsPAWA.DocsPaWR.FascicolazioneClassificazione _classificazione = null;
		private DocsPAWA.DocsPaWR.Registro _userReg = null;
        private string _docOrFasc = "";
        private ArrayList _campiSelezionati = null;

        // Lista dei system id degli oggetti selezionati
        String[] _objSystemId;
		
		DocsPaWR.FileDocumento _file = new DocsPAWA.DocsPaWR.FileDocumento();
		#endregion

		#region COSTRUTTORI
		public exportDatiManager()
		{

		}

		public exportDatiManager(
			string oggettoExport, 
			string tipologiaExport,
			string titolo,
			DocsPaWR.InfoUtente userInfo,
			DocsPaWR.Ruolo userRuolo,
            DocsPaWR.FullTextSearchContext context,
            string docOrFasc,
            String[] objSystemId)
		{
			this._oggettoExport   = oggettoExport;   // doc o fasc o trasm.....
			this._tipologiaExport = tipologiaExport; // PDF o XLS
			this._titolo		  = titolo;
			this._userInfo		  = userInfo;
			this._userRuolo		  = userRuolo;
            this._context         = context;
            this._docOrFasc       = docOrFasc;
            this._objSystemId = objSystemId;
		}

        public exportDatiManager(
            string oggettoExport,
            string tipologiaExport,
            string titolo,
            DocsPaWR.InfoUtente userInfo,
            DocsPaWR.Ruolo userRuolo,
            DocsPaWR.FullTextSearchContext context,
            string docOrFasc,
            ArrayList campiSelezionati,
            String[] objSystemId)
        {
            this._oggettoExport = oggettoExport;   // doc o fasc o trasm.....
            this._tipologiaExport = tipologiaExport; // PDF o XLS
            this._titolo = titolo;
            this._userInfo = userInfo;
            this._userRuolo = userRuolo;
            this._context = context;
            this._docOrFasc = docOrFasc;
            this._campiSelezionati = campiSelezionati;

            // Impostazione della lista dei system id degli oggetti selezionati
            this._objSystemId =objSystemId;
        }
		#endregion

		#region EXPORT
        public DocsPAWA.DocsPaWR.FileDocumento Export()
		{
			this.setData();
			return _file;
		}

		private void setData()
		{
			switch(this._oggettoExport)
			{
				case "doc":
                    this.setDataDoc(this._objSystemId);
					break;
                case "docInfasc":
                    this.setDataDocInFasc(this._objSystemId);
                    break;
				case "fasc":
					this.setDataFasc(this._objSystemId);
					break;
				case "trasm":				
					this.setDataTrasm();
					break;		
		        case "toDoList":
                    this.setDataToDoList();
                    break;
                case "archivioCartaceo":
                    this.setDataArchivioCartaceo();
                    break;
                case "docInCest":
                    this.setDataDocInCest();
                    break;
                case "rubrica":
                    this.setDataRubrica();
                    break;
                }
		}
		#endregion
		
		#region FILTRI
		/// <summary>
		/// Reperisce i filtri impostati dall'utente per i documenti
		/// </summary>
		private void getFiltriDoc()
		{
			if(DocumentManager.getFiltroRicDoc(null)!=null)
				this._lstFiltri = DocumentManager.getFiltroRicDoc(null);				
			else 
				this._lstFiltri = DocumentManager.getMemoriaFiltriRicDoc(null);

            if (this._lstFiltri == null)
            {
                this._lstFiltri = SearchFilters;
            }
		}

		/// <summary>
		/// Reperisce i filtri impostati dall'utente per i fascicoli
		/// </summary>
		private void getFiltriFasc()
		{			
			if(FascicoliManager.getFiltroRicFasc(null)!=null)
				this._lstFiltri = FascicoliManager.getFiltroRicFasc(null);				
			else 
				this._lstFiltri = FascicoliManager.getMemoriaFiltriRicFasc(null);

            if (this._lstFiltri == null)
            {
                this._lstFiltri = SearchFilters;
            }
		}

		/// <summary>
		/// Reperisce i filtri impostati dall'utente per le trasmissioni
		/// </summary>
		private void getFiltriTrasm()
		{
			this._lstFiltriTrasm = DocumentManager.getFiltroRicTrasm(null);

            if (this._lstFiltri == null)
            {
                this._lstFiltri = SearchFilters;
            }
		}

        /// <summary>
        ///  Reperisce il codice del fascicolo
        /// </summary>
        private void getFascDocumenti()
        {
            this._folder = FascicoliManager.getFolderSelezionato();
            //this._fascicolo = FascicoliManager.getFascicoloSelezionato(null);
            DocsPAWA.DocsPaWR.Fascicolo fascicolo = null;
            fascicolo = FascicoliManager.getFascicoloSelezionato(null);
            this.codFasc = fascicolo.codice;
        }
		#endregion

        #region Archivio cartaceo

        /// <summary>
        /// 
        /// </summary>
        private void setDataArchivioCartaceo()
        {
            DocsPaWR.FiltroRicerca[] filtri = FascicolazioneCartacea.SessionManager.Filtri;

            if (filtri != null && filtri.Length > 0)
            {
                DocsPaWR.ExportDataFormatEnum format = DocsPaWR.ExportDataFormatEnum.Pdf;
                if (this._tipologiaExport.ToUpper().Equals("XLS"))
                    format = DocsPAWA.DocsPaWR.ExportDataFormatEnum.Excel;

                DocsPaWR.DocsPaWebService ws = new DocsPAWA.DocsPaWR.DocsPaWebService();
                this._file = ws.FascCartaceaExportListFilters(UserManager.getInfoUtente(), format, filtri, this._titolo);

                if (this._file != null)
                {
                    exportDatiSessionManager session = new exportDatiSessionManager();
                    session.SetSessionExportFile(this._file);
                }
            }
        }

        #endregion

        #region DOCUMENTI

        /// <summary>
		/// 
		/// </summary>
        private void setDataDoc(String[] documentsSystemId)
		{
            this.getFiltriDoc();
            AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();


            Field[] visibleArray = null;

            if (this._campiSelezionati!=null)
            {
                List<Field> visibleFieldsTemplate;
                visibleFieldsTemplate = new List<Field>();

                if (GridManager.SelectedGrid == null)
                {
                    GridManager.SelectedGrid = GridManager.GetStandardGridForUser(GridTypeEnumeration.Document);
                }

                foreach (CampoSelezionato tempCamp in this._campiSelezionati)
                {
                    Field d = (Field)GridManager.SelectedGrid.Fields.Where(f => f.FieldId.Equals(tempCamp.fieldID) && f.CustomObjectId>0).FirstOrDefault();
                    if (d != null)
                    {
                        visibleFieldsTemplate.Add(d);
                    }
                    else
                    {
                        if (!GridManager.IsRoleEnabledToUseGrids() && !tempCamp.campoStandard.Equals("1"))
                        {
                            d = new Field();
                            d.FieldId = tempCamp.fieldID;
                            d.CustomObjectId = Convert.ToInt32(tempCamp.campoStandard);
                            d.OriginalLabel = tempCamp.nomeCampo;
                            d.Label = tempCamp.nomeCampo;
                            d.Width = 100;
                            visibleFieldsTemplate.Add(d);
                        }
                    }
                }

                if (visibleFieldsTemplate != null && visibleFieldsTemplate.Count > 0)
                {
                    visibleArray = visibleFieldsTemplate.ToArray();
                }
            }

            this._file = ws.ExportDocCustom(this._userInfo, this._lstFiltri, this._tipologiaExport, this._titolo, this._context, this._campiSelezionati, documentsSystemId, visibleArray);
         
            ws = null;
            if (this._file != null)
            {
                exportDatiSessionManager session = new exportDatiSessionManager();
                session.SetSessionExportFile(this._file);
            }
		}

        /// <summary>
        /// Lista dei filtri di ricerca
        /// </summary>
        private FiltroRicerca[][] SearchFilters
        {
            get
            {
                if (CallContextStack.CurrentContext != null)
                {
                    return CallContextStack.CurrentContext.ContextState["SearchFilters"] as FiltroRicerca[][];
                }
                else
                {
                    return null;
                }   
            }

            set
            {
                CallContextStack.CurrentContext.ContextState["SearchFilters"] = value;
            }
        }

        /// <summary>
        /// 
        ///</summary>
        ///<param name="selectedDocumentsId">Array con gli id dei documenti da esportare</param>
        private void setDataDocInFasc(String[] selectedDocumentsId)
        {
            this.getFascDocumenti();

            // Reperimento filtri di ricerca correntemente immessi
            DocsPaWR.FiltroRicerca[][] currentFilters = ricercaDoc.FiltriRicercaDocumenti.CurrentFilterSessionStorage.GetCurrentFilter();

            DocsPaWR.FiltroRicerca[][] orderFilters = ricercaDoc.FiltriRicercaDocumenti.CurrentFilterSessionOrderFilter.GetCurrentFilter();


            AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();

            Field[] visibleArray = null;

            if (this._campiSelezionati != null)
            {
                List<Field> visibleFieldsTemplate;
                visibleFieldsTemplate = new List<Field>();

                foreach (CampoSelezionato tempCamp in this._campiSelezionati)
                {
                    Field d = (Field)GridManager.SelectedGrid.Fields.Where(f => f.FieldId.Equals(tempCamp.fieldID) && f.CustomObjectId > 0).FirstOrDefault();
                    if (d != null)
                    {
                        visibleFieldsTemplate.Add(d);
                    }
                    else
                    {
                        if (!GridManager.IsRoleEnabledToUseGrids() && !tempCamp.campoStandard.Equals("1"))
                        {
                            d = new Field();
                            d.FieldId = tempCamp.fieldID;
                            d.CustomObjectId = Convert.ToInt32(tempCamp.campoStandard);
                            d.OriginalLabel = tempCamp.nomeCampo;
                            d.Label = tempCamp.nomeCampo;
                            d.Width = 100;
                            visibleFieldsTemplate.Add(d);
                        }
                    }
                }

                if (visibleFieldsTemplate != null && visibleFieldsTemplate.Count > 0)
                {
                    visibleArray = visibleFieldsTemplate.ToArray();
                }
            }

            this._file = ws.ExportDocInFascCustom(this._userInfo, this._folder, this.codFasc, this._tipologiaExport, this._titolo, currentFilters, this._campiSelezionati, selectedDocumentsId, visibleArray, orderFilters);

            ws = null;

            if (this._file != null)
            {
                exportDatiSessionManager session = new exportDatiSessionManager();
                session.SetSessionExportFile(this._file);
            }
        }

        private void setDataDocInCest()
        {
            // Reperimento filtri di ricerca correntemente immessi
            DocsPaWR.FiltroRicerca[][] currentFilters = ricercaDoc.FiltriRicercaDocumenti.CurrentFilterSessionStorage.GetCurrentFilter();
            AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
            this._file = ws.ExportDocInCest(this._userInfo, this._tipologiaExport, this._titolo, currentFilters, this._campiSelezionati);
            ws = null;
            if (this._file != null)
            {
                exportDatiSessionManager session = new exportDatiSessionManager();
                session.SetSessionExportFile(this._file);
            }
        }

        private static bool cercaStampeRegistro(DocsPAWA.DocsPaWR.FiltroRicerca[][] objQueryList)
        {
            for (int i = 0; i < objQueryList.Length; i++)
            {
                for (int j = 0; j < objQueryList[i].Length; j++)
                {
                    DocsPAWA.DocsPaWR.FiltroRicerca f = objQueryList[i][j];
                    if (f.argomento.Equals("TIPO") && f.valore.Equals("R"))
                        return true;
                    if (f.argomento.Equals("STAMPA_REG") && f.valore.Equals("true"))
                        return true;
                }
            }
            return false;
        }
		#endregion

		#region FASCICOLI

		/// <summary>
		/// 
		/// </summary>
        /// <param name="objSystemId">Lista dei system id dei fascicoli selezionati</param>
        private void setDataFasc(String[] objSystemId)
		{
            this.getFiltriFasc();

            this._classificazione = FascicoliManager.getClassificazioneSelezionata(null);

            this._userReg = DocsPAWA.UserManager.getRegistroSelezionato(null);

            bool enableUfficioRef = (DocsPAWA.ConfigSettings.getKey(DocsPAWA.ConfigSettings.KeysENUM.ENABLE_UFFICIO_REF) != null
                && DocsPAWA.ConfigSettings.getKey(DocsPAWA.ConfigSettings.KeysENUM.ENABLE_UFFICIO_REF).Equals("1"));

            bool enableChilds = DocsPAWA.FascicoliManager.getAllClassValue(null);

            bool enableProfilazione = false;
            if (System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamicaFasc"] != null && System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamicaFasc"] == "1")
                enableProfilazione = true;

            AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();

            Field[] visibleArray = null;

            if (this._campiSelezionati != null)
            {
                List<Field> visibleFieldsTemplate;
                visibleFieldsTemplate = new List<Field>();

                foreach (CampoSelezionato tempCamp in this._campiSelezionati)
                {
                    Field d = (Field)GridManager.SelectedGrid.Fields.Where(f => f.FieldId.Equals(tempCamp.fieldID) && f.CustomObjectId > 0).FirstOrDefault();
                    if (d != null)
                    {
                        visibleFieldsTemplate.Add(d);
                    }
                    else
                    {
                        if (!GridManager.IsRoleEnabledToUseGrids() && !tempCamp.campoStandard.Equals("1"))
                        {
                            d = new Field();
                            d.FieldId = tempCamp.fieldID;
                            d.CustomObjectId = Convert.ToInt32(tempCamp.campoStandard);
                            d.OriginalLabel = tempCamp.nomeCampo;
                            d.Label = tempCamp.nomeCampo;
                            d.Width = 100;
                            visibleFieldsTemplate.Add(d);
                        }
                    }
                }

                if (visibleFieldsTemplate != null && visibleFieldsTemplate.Count > 0)
                {
                    visibleArray = visibleFieldsTemplate.ToArray();
                }
            }

            this._file = ws.ExportFascCustom(this._userInfo, this._userReg, enableUfficioRef, enableProfilazione, enableChilds, this._classificazione, this._lstFiltri, this._tipologiaExport, this._titolo, this._campiSelezionati, objSystemId, visibleArray,true);

            ws = null;
            if (this._file != null)
            {
                exportDatiSessionManager session = new exportDatiSessionManager();
                session.SetSessionExportFile(this._file);
            }
		}

		#endregion
		
		#region TRASMISSIONI

		/// <summary>
		/// 
		/// </summary>
		private void setDataTrasm()
		{
			string tipoRicerca = (string) System.Web.HttpContext.Current.Session["tiporic"]; 
			if(tipoRicerca!=null && (tipoRicerca.Equals("R") || tipoRicerca.Equals("E")))
			{
				this.getFiltriTrasm();
		
				this._user = UserManager.getUtente();

				DocsPaWR.TrasmissioneOggettoTrasm oggettoTrasm = new DocsPAWA.DocsPaWR.TrasmissioneOggettoTrasm();

				AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
				this._file = ws.ExportTrasm(oggettoTrasm,tipoRicerca,this._user,this._userRuolo,this._lstFiltriTrasm,this._tipologiaExport,this._titolo, this._campiSelezionati, UserManager.getInfoUtente());
				ws = null;

				if(this._file!=null)
				{
					exportDatiSessionManager session = new exportDatiSessionManager();
					session.SetSessionExportFile(this._file);
				}
			}
		}

        private void setDataToDoList()
        {
            this.getFiltriTrasm();
            DocsPaWR.InfoUtente infoUtente = UserManager.getInfoUtente();
                
            DocsPAWA.DocsPaWR.Registro[] reg = UserManager.getRuolo().registri;
            string registri = "0";
            if (reg.Length > 0)
            {
                foreach (DocsPAWA.DocsPaWR.Registro registro in reg)
                {
                    registri = registri + "," + registro.systemId;
                }

            }

            AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
            this._file = ws.ExportToDoList(infoUtente, this._docOrFasc, this._lstFiltriTrasm, registri, this._tipologiaExport, this._titolo, this._campiSelezionati, this._objSystemId);
            ws = null;

            if (this._file != null)
            {
                exportDatiSessionManager session = new exportDatiSessionManager();
                session.SetSessionExportFile(this._file);
            }
        }

		#endregion

        #region RUBRICA
        private void setDataRubrica()
        {
            //this.getFiltriDoc();
            bool store = true;
            string registri = "";
             AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
            this._file = ws.ExportRubrica(this._userInfo, store, registri);
            ws = null;
            if (this._file != null)
            {
                exportDatiSessionManager session = new exportDatiSessionManager();
                session.SetSessionExportFile(this._file);
            }
        }
        #endregion

    }
}
