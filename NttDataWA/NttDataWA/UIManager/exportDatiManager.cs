using System;
using System.Collections;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Xml;
using System.Xml.Xsl;
using System.Xml.XPath;
using System.Collections.Generic;
using NttDataWA.DocsPaWR;
using NttDataWA.Utils;


namespace NttDataWA.UIManager
{
	/// <summary>
	/// Summary description for exportDatiManager.
	/// </summary>
	public class exportDatiManager
    {
        #region constant

        private const string DOC = "doc";
        private const string DOC_IN_FASC = "docInfasc";
        private const string FASC = "fasc";
        private const string TRASM = "trasm";
        private const string NOTIFY = "notify";
        private const string ARCHIVIO_CARTACEO = "archivioCartaceo";
        private const string DOC_IN_CESTINO = "docInCest";
        private const string RUBRICA = "rubrica";
        private const string SEARCH_ADDRESS_BOOK = "searchAddressBook";
        private const string VISIBILITA_PROCESSI_FIRMA = "exportVisibilita";

        #endregion

        #region VAR

        private string _oggettoExport = string.Empty;
		private string _tipologiaExport = string.Empty;		
		private string _titolo = string.Empty;

        private Folder _folder = null;
        private string codFasc = "";

		private FiltroRicerca[][] _lstFiltri = null;
		private FiltroRicerca[] _lstFiltriTrasm = null;
        private FullTextSearchContext _context = null;
		private Ruolo _userRuolo = null;
		private InfoUtente _userInfo = null;
		private Utente _user = null;
		private FascicolazioneClassificazione _classificazione = null;
		private Registro _userReg = null;
        private string _docOrFasc = "";
        private ArrayList _campiSelezionati = null;

        // Lista dei system id degli oggetti selezionati
        String[] _objSystemId;
		
		DocsPaWR.FileDocumento _file = new FileDocumento();
        private static DocsPaWR.DocsPaWebService docsPaWS = ProxyManager.GetWS();

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

        public FileDocumento Export()
		{
			this.SetData();
			return _file;
		}

		private void SetData()
		{
			switch(this._oggettoExport)
			{
				case DOC:
                    this.SetDataDoc(this._objSystemId);
					break;
                case DOC_IN_FASC:
                    this.SetDataDocInFasc(this._objSystemId);
                    break;
				case FASC:
					this.SetDataFasc(this._objSystemId);
					break;
				case TRASM:				
					this.SetDataTrasm();
					break;		
		        case NOTIFY:
                    this.SetDataNotify();
                    break;
                case ARCHIVIO_CARTACEO:
                    this.SetDataArchivioCartaceo();
                    break;
                case DOC_IN_CESTINO:
                    this.SetDataDocInCest();
                    break;
                case RUBRICA:
                    this.SetDataRubrica();
                    break;
                case SEARCH_ADDRESS_BOOK:
                    this.SetDataSearchAddressBook();
                    break;
                case VISIBILITA_PROCESSI_FIRMA:
                    this.SetDataVisibilitaProcessiFirma();
                    break;
                }
		}

		#endregion
		
		#region FILTRI
		/// <summary>
		/// Reperisce i filtri impostati dall'utente per i documenti
		/// </summary>
		private void GetFiltriDoc()
		{
			if(DocumentManager.getFiltroRicDoc()!=null)
				this._lstFiltri = DocumentManager.getFiltroRicDoc();				
			else 
				this._lstFiltri = DocumentManager.getMemoriaFiltriRicDoc();

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
			if(ProjectManager.getFiltroRicFasc()!=null)
                this._lstFiltri = ProjectManager.getFiltroRicFasc();				
			else
                this._lstFiltri = ProjectManager.getMemoriaFiltriRicFasc();

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
			this._lstFiltriTrasm = DocumentManager.getFiltroRicTrasm();
            if (this._lstFiltriTrasm == null)
                this._lstFiltriTrasm = this.SearchFilters[0];

            if (this._lstFiltri == null)
            {
                this._lstFiltri = SearchFilters;
            }
		}

        /// <summary>
        ///  Reperisce il codice del fascicolo
        /// </summary>
        private void GetFascDocumenti()
        {
            this._folder = UIManager.ProjectManager.getProjectInSession().folderSelezionato;
            Fascicolo fascicolo = null;
            fascicolo = ProjectManager.getProjectInSession();
            this.codFasc = fascicolo.codice;
        }
		#endregion

        #region Archivio cartaceo

        /// <summary>
        /// 
        /// </summary>
        private void SetDataArchivioCartaceo()
        {
            //DocsPaWR.FiltroRicerca[] filtri = FascicolazioneCartacea.SessionManager.Filtri;
            DocsPaWR.FiltroRicerca[] filtri = null;

            if (filtri != null && filtri.Length > 0)
            {
                DocsPaWR.ExportDataFormatEnum format = DocsPaWR.ExportDataFormatEnum.Pdf;
                if (this._tipologiaExport.ToUpper().Equals("XLS"))
                    format = ExportDataFormatEnum.Excel;

                DocsPaWR.DocsPaWebService ws = new DocsPaWebService();
                this._file = ws.FascCartaceaExportListFilters(UserManager.GetInfoUser(), format, filtri, this._titolo);

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
        private void SetDataDoc(String[] documentsSystemId)
		{
            this.GetFiltriDoc();

            Field[] visibleArray = null;

            if (this._campiSelezionati!=null)
            {
                List<Field> visibleFieldsTemplate;
                visibleFieldsTemplate = new List<Field>();

                if (GridManager.SelectedGrid == null)
                {
                    GridManager.SelectedGrid = GridManager.GetStandardGridForUser(GridTypeEnumeration.Document, UserManager.GetInfoUser());
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

            if (this._campiSelezionati == null)
                this._campiSelezionati = new ArrayList();

            this._file = docsPaWS.ExportDocCustom(this._userInfo, this._lstFiltri, this._tipologiaExport, this._titolo, this._context, this._campiSelezionati.ToArray(), documentsSystemId, GridManager.SelectedGrid, GridManager.IsRoleEnabledToUseGrids(), visibleArray);
         
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
                return HttpContext.Current.Session["filtroRicerca"] as FiltroRicerca[][];
            }

            set
            {
                HttpContext.Current.Session["filtroRicerca"] = value;
            }
        }

        /// <summary>
        /// 
        ///</summary>
        ///<param name="selectedDocumentsId">Array con gli id dei documenti da esportare</param>
        private void SetDataDocInFasc(String[] selectedDocumentsId)
        {
            this.GetFascDocumenti();

            // Reperimento filtri di ricerca correntemente immessi
            //DocsPaWR.FiltroRicerca[][] currentFilters = ricercaDoc.FiltriRicercaDocumenti.CurrentFilterSessionStorage.GetCurrentFilter();
            //DocsPaWR.FiltroRicerca[][] orderFilters = ricercaDoc.FiltriRicercaDocumenti.CurrentFilterSessionOrderFilter.GetCurrentFilter();
            DocsPaWR.FiltroRicerca[][] currentFilters = null;
            DocsPaWR.FiltroRicerca[][] orderFilters = UIManager.GridManager.GetFiltriOrderRicerca(GridManager.SelectedGrid);

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

            if (this._campiSelezionati == null)
                this._campiSelezionati = new ArrayList();
            this._file = docsPaWS.ExportRicercaDocInFascCustom(this._folder, this.codFasc, this._tipologiaExport, this._titolo, currentFilters, this._userInfo, this._campiSelezionati.ToArray(), selectedDocumentsId, GridManager.SelectedGrid, GridManager.IsRoleEnabledToUseGrids(), visibleArray, orderFilters);

            if (this._file != null)
            {
                exportDatiSessionManager session = new exportDatiSessionManager();
                session.SetSessionExportFile(this._file);
            }
        }

        private void SetDataDocInCest()
        {
            // Reperimento filtri di ricerca correntemente immessi
            //DocsPaWR.FiltroRicerca[][] currentFilters = ricercaDoc.FiltriRicercaDocumenti.CurrentFilterSessionStorage.GetCurrentFilter();
            DocsPaWR.FiltroRicerca[][] currentFilters = null;

            if (this._campiSelezionati == null)
                this._campiSelezionati = new ArrayList();

            this._file = docsPaWS.ExportRicercaDocInCest(this._userInfo, this._tipologiaExport, this._titolo, currentFilters, this._campiSelezionati.ToArray());

            if (this._file != null)
            {
                exportDatiSessionManager session = new exportDatiSessionManager();
                session.SetSessionExportFile(this._file);
            }
        }

        private static bool CercaStampeRegistro(FiltroRicerca[][] objQueryList)
        {
            for (int i = 0; i < objQueryList.Length; i++)
            {
                for (int j = 0; j < objQueryList[i].Length; j++)
                {
                    FiltroRicerca f = objQueryList[i][j];
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
        private void SetDataFasc(String[] objSystemId)
		{
            this.getFiltriFasc();

            this._classificazione = ProjectManager.getClassificazioneSelezionata(null);

            this._userReg = RoleManager.GetRoleInSession().registri[0];

            bool enableUfficioRef = (ConfigSettings.getKey(ConfigSettings.KeysENUM.ENABLE_UFFICIO_REF) != null
                && ConfigSettings.getKey(ConfigSettings.KeysENUM.ENABLE_UFFICIO_REF).Equals("1"));

            bool enableChilds = ProjectManager.getAllClassValue(null);

            bool enableProfilazione = false;
            if (System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamicaFasc"] != null && System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamicaFasc"] == "1")
                enableProfilazione = true;

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

            if (this._campiSelezionati == null)
                this._campiSelezionati = new ArrayList();

            this._file = docsPaWS.ExportRicercaFascCustom(this._userInfo, this._userReg, enableUfficioRef, enableProfilazione, enableChilds, this._classificazione, this._lstFiltri, this._tipologiaExport, this._titolo, this._campiSelezionati.ToArray(), objSystemId, GridManager.SelectedGrid, GridManager.IsRoleEnabledToUseGrids(), visibleArray, true);

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
		private void SetDataTrasm()
		{
            string tipoRicerca = (string)System.Web.HttpContext.Current.Session["typeSearch"]; 
			if(tipoRicerca!=null && (tipoRicerca.Equals("R") || tipoRicerca.Equals("E")))
			{
				this.getFiltriTrasm();
		
				this._user = UserManager.GetUserInSession();

				DocsPaWR.TrasmissioneOggettoTrasm oggettoTrasm = new DocsPaWR.TrasmissioneOggettoTrasm();

                if (this._campiSelezionati == null)
                    this._campiSelezionati = new ArrayList();

                this._file = docsPaWS.ExportRicercaTrasm(oggettoTrasm, tipoRicerca, this._user, this._userRuolo, this._lstFiltriTrasm, this._tipologiaExport, this._titolo, UserManager.GetInfoUser(), this._campiSelezionati.ToArray());

				if(this._file!=null)
				{
					exportDatiSessionManager session = new exportDatiSessionManager();
					session.SetSessionExportFile(this._file);
				}
			}
		}

		#endregion

        #region RUBRICA
        private void SetDataRubrica()
        {
            bool store = true;
            string registri = "";

            docsPaWS.Timeout = System.Threading.Timeout.Infinite;
            //this._file = docsPaWS.ExportRubricaWithTitle(this._userInfo, store, registri, this._titolo);
            this._file = docsPaWS.ExportRubricaWithTitleNew(this._userInfo, store, registri, this._titolo, this._tipologiaExport);

            if (this._file != null)
            {
                exportDatiSessionManager session = new exportDatiSessionManager();
                session.SetSessionExportFile(this._file);
            }
        }

        private void SetDataSearchAddressBook()
        {
            bool store = true;
            DocsPaWR.ParametriRicercaRubrica qco = (DocsPaWR.ParametriRicercaRubrica)HttpContext.Current.Session["AddressBook.corrFilter"];
            //this._file = docsPaWS.ExportSearchAddressBook(this._userInfo, store, qco);
            this._file = docsPaWS.ExportSearchAddressBookNew(this._userInfo, store, qco, this._titolo, this._tipologiaExport);
            if (this._file != null)
            {
                exportDatiSessionManager session = new exportDatiSessionManager();
                session.SetSessionExportFile(this._file);
            }
        }

        #endregion

        #region NOFICATION CENTER

        private void SetDataNotify()
        {
            DocsPaWR.InfoUtente infoUtente = UserManager.GetInfoUser();

            if (this._campiSelezionati == null)
                this._campiSelezionati = new ArrayList();

            this._file = docsPaWS.ExportNotificationCenter(infoUtente, this._tipologiaExport, this._titolo, this._campiSelezionati.ToArray(), NotificationManager.ListNotifyFiltered.ToArray());

            if (this._file != null)
            {
                exportDatiSessionManager session = new exportDatiSessionManager();
                session.SetSessionExportFile(this._file);
            }
        }

        #endregion

        #region VISIBILITA PROCESSI FIRMA

        private void SetDataVisibilitaProcessiFirma()
        {
            DocsPaWR.InfoUtente infoUtente = UserManager.GetInfoUser();
            List<ProcessoFirma> listaProcessiFirma;
            if (this.ProcessoDiFirmaSelected != null && !string.IsNullOrEmpty(this.ProcessoDiFirmaSelected.idProcesso))
            {
                listaProcessiFirma = new List<ProcessoFirma>() { ProcessoDiFirmaSelected };
            }
            else
            {
                listaProcessiFirma = ListaProcessiDiFirma;
            }
            if (this._campiSelezionati == null)
                this._campiSelezionati = new ArrayList();

            this._file = docsPaWS.ExportVisibilitaProcessiFirma(infoUtente, this._tipologiaExport, this._titolo, this._campiSelezionati.ToArray(), listaProcessiFirma.ToArray());

            if (this._file != null)
            {
                exportDatiSessionManager session = new exportDatiSessionManager();
                session.SetSessionExportFile(this._file);
            }
        }

        private ProcessoFirma ProcessoDiFirmaSelected
        {
            get
            {
                if (HttpContext.Current.Session["ProcessoDiFirmaSelected"] != null)
                    return (ProcessoFirma)HttpContext.Current.Session["ProcessoDiFirmaSelected"];
                else
                    return null;
            }
        }

        private List<ProcessoFirma> ListaProcessiDiFirma
        {
            get
            {
                if (HttpContext.Current.Session["ListaProcessiDiFirma"] != null)
                    return (List<ProcessoFirma>)HttpContext.Current.Session["ListaProcessiDiFirma"];
                else
                    return null;
            }
            set
            {
                HttpContext.Current.Session["ListaProcessiDiFirma"] = value;
            }
        }

        #endregion
    }
}
