using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.UIManager;
using NttDataWA.DocsPaWR;
using NttDatalLibrary;
using System.Collections;

using System.Web.UI.HtmlControls;
using NttDataWA.Utils;

using System.Data;

using System.Text;



namespace NttDataWA.Popup
{
    public partial class TransferPolicy : System.Web.UI.Page
    {
        #region Fields

        //è un documento se isDoc è true, un fascicolo se isDoc è false
        protected bool isDoc
        {
            get
            {
                bool result = true;

                if (HttpContext.Current.Session["isDoc"] != null)
                {
                    result = bool.Parse(HttpContext.Current.Session["isDoc"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["isDoc"] = value;
            }
        }

        protected int Versamento_system_id
        {
            get
            {
                int result = 0;

                if (HttpContext.Current.Session["Versamento_system_id"] != null)
                {
                    result = int.Parse(HttpContext.Current.Session["Versamento_system_id"].ToString());
                }

                //mio id cablato
                //result = 56;

                return result;
            }
            set
            {
                HttpContext.Current.Session["Versamento_system_id"] = value;
            }
        }

        protected Dictionary<string, string> idRegAOO2DescAOO
        {
            get
            {
                Dictionary<string, string> result = new Dictionary<string, string>();

                if (HttpContext.Current.Session["idRegAOO2DescAOO"] != null)
                    result = HttpContext.Current.Session["idRegAOO2DescAOO"] as Dictionary<string, string>;
                else
                    HttpContext.Current.Session["idRegAOO2DescAOO"] = result;

                return result;
            }
            set
            {
                HttpContext.Current.Session["idRegAOO2DescAOO"] = value;
            }
        }

        protected Dictionary<string, DocsPaWR.ElementoRubrica> idUO2UO
        {
            get
            {
                Dictionary<string, DocsPaWR.ElementoRubrica> result = new Dictionary<string, DocsPaWR.ElementoRubrica>();

                if (HttpContext.Current.Session["idUO2UO"] != null)
                    result = HttpContext.Current.Session["idUO2UO"] as Dictionary<string, DocsPaWR.ElementoRubrica>;
                else
                    HttpContext.Current.Session["idUO2UO"] = result;

                return result;
            }
            set
            {
                HttpContext.Current.Session["idUO2UO"] = value;
            }
        }

        protected Dictionary<string, string> listaTipiDoc
        {
            get
            {
                Dictionary<string, string> result = new Dictionary<string, string>();

                if (HttpContext.Current.Session["listaTipiDoc"] != null)
                    result = HttpContext.Current.Session["listaTipiDoc"] as Dictionary<string, string>;
                else
                    HttpContext.Current.Session["listaTipiDoc"] = result;

                return result;
            }
            set
            {
                HttpContext.Current.Session["listaTipiDoc"] = value;
            }
        }

        protected Dictionary<string, string> listaTipiFasc
        {
            get
            {
                Dictionary<string, string> result = new Dictionary<string, string>();

                if (HttpContext.Current.Session["listaTipiFasc"] != null)
                    result = HttpContext.Current.Session["listaTipiFasc"] as Dictionary<string, string>;
                else
                    HttpContext.Current.Session["listaTipiFasc"] = result;

                return result;
            }
            set
            {
                HttpContext.Current.Session["listaTipiFasc"] = value;
            }
        }

        private string ReturnValue
        {
            get
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["ReturnValuePopup"].ToString()))
                    return HttpContext.Current.Session["ReturnValuePopup"].ToString();
                else
                    return string.Empty;
            }
        }

        //policy corrente originale in context
        protected ARCHIVE_TransferPolicy OriginalPolicyInContext
        {
            get
            {
                ARCHIVE_TransferPolicy result = null;
                if (HttpContext.Current.Session["OriginalPolicyInContext"] != null)
                {
                    result = HttpContext.Current.Session["OriginalPolicyInContext"] as ARCHIVE_TransferPolicy;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["OriginalPolicyInContext"] = value;
            }
        }

        //policy corrente con le modifiche
        protected ARCHIVE_TransferPolicy CurrentPolicyInContext
        {
            get
            {
                ARCHIVE_TransferPolicy result = null;
                if (HttpContext.Current.Session["CurrentPolicyInContext"] != null)
                {
                    result = HttpContext.Current.Session["CurrentPolicyInContext"] as ARCHIVE_TransferPolicy;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["CurrentPolicyInContext"] = value;
            }
        }

        //lista policy originale associate al versamento 
        protected List<ARCHIVE_TransferPolicy> TransferPolicyOriginalInContext
        {
            get
            {
                List<ARCHIVE_TransferPolicy> result = null;
                if (HttpContext.Current.Session["TransferPolicyOriginalInContext"] != null)
                {
                    result = HttpContext.Current.Session["TransferPolicyOriginalInContext"] as List<ARCHIVE_TransferPolicy>;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["TransferPolicyOriginalInContext"] = value;
            }
        }

        //lista policy corrente associate al versamento 
        protected List<ARCHIVE_TransferPolicy> TransferPolicyInContext
        {
            get
            {
                List<ARCHIVE_TransferPolicy> result = null;
                if (HttpContext.Current.Session["TransferPolicyInContext"] != null)
                {
                    result = HttpContext.Current.Session["TransferPolicyInContext"] as List<ARCHIVE_TransferPolicy>;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["TransferPolicyInContext"] = value;
            }
        }

        #region TransferPolicy_ProfileType

        //lista di profileType associate alla policy corrente
        protected List<ARCHIVE_TransferPolicy_ProfileType> CurrentProfileTypeInContext
        {
            get
            {
                List<ARCHIVE_TransferPolicy_ProfileType> result = new List<ARCHIVE_TransferPolicy_ProfileType>();
                if (HttpContext.Current.Session["CurrentProfileTypeInContext"] != null)
                {
                    result = HttpContext.Current.Session["CurrentProfileTypeInContext"] as List<ARCHIVE_TransferPolicy_ProfileType>;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["CurrentProfileTypeInContext"] = value;
            }

        }

        //lista di profileType associate alla policy Originale
        protected List<ARCHIVE_TransferPolicy_ProfileType> OriginalProfileTypeInContext
        {
            get
            {
                List<ARCHIVE_TransferPolicy_ProfileType> result = new List<ARCHIVE_TransferPolicy_ProfileType>();
                if (HttpContext.Current.Session["OriginalProfileTypeInContext"] != null)
                {
                    result = HttpContext.Current.Session["OriginalProfileTypeInContext"] as List<ARCHIVE_TransferPolicy_ProfileType>;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["OriginalProfileTypeInContext"] = value;
            }

        }

        #endregion

        protected List<ARCHIVE_Transfer> TransferInContext
        {
            get
            {
                List<ARCHIVE_Transfer> result = null;
                if (HttpContext.Current.Session["TransferInContext"] != null)
                {
                    result = HttpContext.Current.Session["TransferInContext"] as List<ARCHIVE_Transfer>;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["TransferInContext"] = value;
            }
        }


        protected List<ARCHIVE_View_Policy> TransferPolicyViewInContext
        {
            get
            {
                List<ARCHIVE_View_Policy> result = null;
                if (HttpContext.Current.Session["TransferPolicyViewInContext"] != null)
                {
                    result = HttpContext.Current.Session["TransferPolicyViewInContext"] as List<ARCHIVE_View_Policy>;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["TransferPolicyViewInContext"] = value;
            }
        }

        protected List<ARCHIVE_JOB_TransferPolicy> TransferPolicyJOBInContext
        {
            get
            {
                List<ARCHIVE_JOB_TransferPolicy> result = null;
                if (HttpContext.Current.Session["TransferPolicyJOBInContext"] != null)
                {
                    result = HttpContext.Current.Session["TransferPolicyJOBInContext"] as List<ARCHIVE_JOB_TransferPolicy>;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["TransferPolicyJOBInContext"] = value;
            }
        }

        #region GrdPolicy

        protected List<Int32> idPolicySelected
        {
            get
            {
                List<Int32> result = null;
                if (HttpContext.Current.Session["idPolicySelected"] != null)
                {
                    result = HttpContext.Current.Session["idPolicySelected"] as List<Int32>;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["idPolicySelected"] = value;
            }
        }

        protected bool selectedAll
        {
            get
            {
                bool result = false;

                if (HttpContext.Current.Session["selectedAll"] != null)
                {
                    result = bool.Parse(HttpContext.Current.Session["selectedAll"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["selectedAll"] = value;
            }
        }

        protected Int32 SelectedRow
        {
            get
            {
                Int32 result = -1;
                if (HttpContext.Current.Session["SelectedRow"] != null)
                {
                    result = int.Parse(HttpContext.Current.Session["SelectedRow"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["SelectedRow"] = value;
            }
        }

        protected Int32 SelectedPage
        {
            get
            {
                Int32 result = 1;
                if (HttpContext.Current.Session["SelectedPage"] != null)
                {
                    result = int.Parse(HttpContext.Current.Session["SelectedPage"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["SelectedPage"] = value;
            }
        }

        #endregion

        #endregion


        public enum PolicyState
        {
            NEW,
            MOD
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!this.IsPostBack)
                {
                    this.InitializePage();
                }
                else
                {
                    this.ReadRetValueFromPopup();

                    if (Session["PolicyState"] != null)
                    {
                        this.CallVisibilityByPolicyState(Session["PolicyState"]);
                    }
                }

                this.RefreshScript();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private void RefreshScript()
        {
            //fondamentali per refresciare gli script per l'aspetto grafico dopo l'avvio di un popup
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshSelect", "refreshSelect();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshTipsy", "tooltipTipsy();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshPicker", "DatePicker2();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "resizeTable", "resizeTable();", true);

        }

        private void InitializePage()
        {
            this.ClearSessionAndContextProperties();
            this.SetSessionProperties();

            this.CallVisibilityByPolicyState(Session["PolicyState"]);

            this.InitializeLanguage();
            this.VisibiltyRoleFunctions();
            this.PopulateddlPolicyTipologia();
            this.PopulateddlPolicyRegistri();
            this.PopulateddlPolicyTitolatio();
            this.PopulateddlPolicyUo();
            this.SetAjax();

            //controllo che l'id del versamento c'è e è diverso da 0
            if (this.Versamento_system_id != 0)
            {
                GetDataByPageState();

                //se è selezionata la policy corrente carico la pagina
                if (this.CurrentPolicyInContext != null)
                {
                    PopulatePageObject();
                }
            }
        }

        private void SetAjax()
        {
            string callType = "CALLTYPE_OWNER_AUTHOR";

            string dataUser = RoleManager.GetRoleInSession().systemId;
            dataUser = dataUser + "-" + this.ddlPolicyRegistri.SelectedValue;
            string contextKey = dataUser + "-" + UIManager.UserManager.GetUserInSession().idAmministrazione + "-" + callType;
            this.RapidCreatore.ContextKey = contextKey;

        }

        /// <summary>
        /// Caller Hub per il change della visibilità degli oggetti.
        /// </summary>
        /// <param name="p"></param>
        private void CallVisibilityByPolicyState(object p)
        {
            if (p == null)
                VisibilityByPolicyState((PolicyState)Enum.Parse(typeof(PolicyState), "NEW"));
            else
                VisibilityByPolicyState((PolicyState)Enum.Parse(typeof(PolicyState), p.ToString().ToUpper()));
        }

        private void ClearSessionAndContextProperties()
        {
            //SESSION
            Session["PolicyState"] = null;
            //UIManager.RegistryManager.SetRegistryInSession(null);
            //CONTEXT
            this.CurrentPolicyInContext = null;
            this.OriginalPolicyInContext = null;
            this.OriginalProfileTypeInContext = null;
            this.TransferPolicyOriginalInContext = null;
            //this.TransferInContext = null;
            //this.TransferPolicyInContext  = null;
            //this.TransferPolicyViewInContext = null;
            //this.TransferPolicyJOBInContext  = null;
            //this.idPolicySelected = null;
            //this.selectedAll = false;



        }

        private void SetSessionProperties()
        {
            //SESSION
            //metto il registry nella sessione perchè serve per il popup del titolario
            UIManager.RegistryManager.SetRegistryInSession(RoleManager.GetRoleInSession().registri[0]);
            Session["PolicyState"] = PolicyState.NEW.ToString();

            //CONTEXT
            //leggo l'id del versamento dalla sessione
            if (Session["Versamento_system_id"] != null)
            {
                int result;
                int.TryParse(Session["Versamento_system_id"].ToString(), out result);
                HttpContext.Current.Session["Versamento_system_id"] = result;
            }
            if (Session["PageState"] == null)
                Session["PageState"] = "MOD";

            this.isDoc = true;
            this.SelectedRow = -1;
            this.SelectedPage = 1;
        }

        private void PopulateddlPolicyRegistri()
        {

            //inizializzo gli ogetti
            this.idRegAOO2DescAOO = new Dictionary<string, string>();
            this.ddlPolicyRegistri.Items.Clear();

            //aggiungo riga vuota
            ListItem item = new ListItem(string.Empty, string.Empty);
            this.ddlPolicyRegistri.Items.Add(item);

            Registro[] lista = RoleManager.GetRoleInSession().registri;

            foreach (Registro reg in lista)
            {
                item = new ListItem();
                item.Text = reg.codRegistro;
                item.Value = reg.systemId;

                //popolo dictionary idRegistro -> descrizione registro
                this.idRegAOO2DescAOO.Add(reg.systemId, reg.descrizione);
                //popolo ddl reigstri 
                this.ddlPolicyRegistri.Items.Add(item);

            }
        }

        private void InitializeLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.OpenTitolario.Title = Utils.Languages.GetLabelFromCode("TitleClassificationScheme", language);
            this.btnPolicyNuovo.Text = Utils.Languages.GetLabelFromCode("btnPolicyNuovo", language);
            this.btnPolicyAvviaRicerca.Text = Utils.Languages.GetLabelFromCode("btnPolicyAvviaRicerca", language);
            this.btnPolicyRimuoviFiltro.Text = Utils.Languages.GetLabelFromCode("btnPolicyRimuoviFiltro", language);
            this.btnPolicyModifica.Text = Utils.Languages.GetLabelFromCode("btnPolicyModifica", language);
            this.btnPolicyElimina.Text = Utils.Languages.GetLabelFromCode("btnPolicyElimina", language);
            this.btnPolicyChiudi.Text = Utils.Languages.GetLabelFromCode("AddDocInProjectBtnClose", language);
            this.LitTitoloPolicy.Text = Utils.Languages.GetLabelFromCode("LitTitoloPolicy", language);
            this.LitIdVersamento.Text = Utils.Languages.GetLabelFromCode("LitIdVersamento", language);
            this.LitAnnoCreazione.Text = Utils.Languages.GetLabelFromCode("LitAnnoCreazione", language);
            this.LitAnnoProtocollazione.Text = Utils.Languages.GetLabelFromCode("LitAnnoProtocollazione", language);
            this.litPolicyId.Text = Utils.Languages.GetLabelFromCode("LitPolicyId", language);
            this.LitDescPolicy.Text = Utils.Languages.GetLabelFromCode("LitDescPolicy", language);
            this.LitDocFasc.Text = Utils.Languages.GetLabelFromCode("LitDocFasc", language);
            this.LitAOO.Text = Utils.Languages.GetLabelFromCode("LitAOO", language);
            this.LitUO.Text = Utils.Languages.GetLabelFromCode("LitUO", language);
            this.chkIncUO.Text = Utils.Languages.GetLabelFromCode("chkIncUO", language);
            this.LitPolicyTip.Text = Utils.Languages.GetLabelFromCode("LitPolicyTip", language);
            this.chkArrivo.Text = Utils.Languages.GetLabelFromCode("chkArrivo", language);
            this.chkPartenza.Text = Utils.Languages.GetLabelFromCode("chkPartenza", language);
            this.chkInt.Text = Utils.Languages.GetLabelFromCode("chkInt", language);
            this.chkNonProt.Text = Utils.Languages.GetLabelFromCode("chkNonProt", language);
            this.chkStampaRegProt.Text = Utils.Languages.GetLabelFromCode("chkStampaRegProt", language);
            this.chkStampaRep.Text = Utils.Languages.GetLabelFromCode("chkStampaRep", language);
            this.LitPolicyTipologia.Text = Utils.Languages.GetLabelFromCode("LitPolicyTipologia", language);
            this.LitPolicyTitolario.Text = Utils.Languages.GetLabelFromCode("LitPolicyTitolario", language);
            this.LitPolicyClassi.Text = Utils.Languages.GetLabelFromCode("LitPolicyClassi", language);
            this.ChkIncludeClass.Text = Utils.Languages.GetLabelFromCode("ChkIncludeClass", language);
            this.LitDescVers.Text = Utils.Languages.GetLabelFromCode("LitDescVers", language);
            this.btnPolicyAnalizza.Text = Utils.Languages.GetLabelFromCode("btnPolicyAnalizza", language);
            this.lbl_dtaCreateFrom.Text = Utils.Languages.GetLabelFromCode("VisibilityOneField", language);
            this.dtaCreate_opt0.Text = Utils.Languages.GetLabelFromCode("VisibilityOpt0", language);
            this.dtaCreate_opt1.Text = Utils.Languages.GetLabelFromCode("VisibilityOpt1", language);
            this.lbl_dtaProtFrom.Text = Utils.Languages.GetLabelFromCode("VisibilityOneField", language);
            this.dtaProt_opt0.Text = Utils.Languages.GetLabelFromCode("VisibilityOpt0", language);
            this.dtaProt_opt1.Text = Utils.Languages.GetLabelFromCode("VisibilityOpt1", language);
            this.ddlPolicyTipologia.Attributes.Add("data-placeholder", Utils.Languages.GetLabelFromCode("DocumentDdlTypeDocument", language));
            this.ddlPolicyRegistri.Attributes.Add("data-placeholder", Utils.Languages.GetLabelFromCode("PolicyDdlRegistry", language));
            this.ddlPolicyUo.Attributes.Add("data-placeholder", Utils.Languages.GetLabelFromCode("PolicyDdlUO", language));
            //this.LitGridPolicyVuota.Text = Utils.Languages.GetLabelFromCode("LitGridPolicyVuota", language);
            //this.LitGridPolicyVuota.Visible = false;

        }

        /// <summary>
        /// Set the objects visibility by page state and policy type
        /// Page state:
        /// enum PolicyState
        /// N= NEW  nuova policy
        /// M= MOD  modifica policy
        /// </summary>
        /// <param name="querystrig"></param>
        private void VisibilityByPolicyState(PolicyState querystrig)
        {

            //policy state
            switch (querystrig)
            {
                //Page state new
                case (PolicyState.NEW):
                    btnPolicyNuovo.Enabled = true;
                    btnPolicyAvviaRicerca.Enabled = true;
                    btnPolicyAnalizza.Enabled = true;
                    btnPolicyRimuoviFiltro.Enabled = true;
                    btnPolicyModifica.Enabled = false;
                    btnPolicyElimina.Enabled = true;

                    break;

                //Page state Modifiy
                case (PolicyState.MOD):
                    btnPolicyNuovo.Enabled = false;
                    btnPolicyAvviaRicerca.Enabled = true;
                    btnPolicyAnalizza.Enabled = true;
                    btnPolicyRimuoviFiltro.Enabled = true;
                    btnPolicyModifica.Enabled = true;
                    btnPolicyElimina.Enabled = true;

                    break;
            }

            //policy type

            PlcPolicyTip.Visible = this.isDoc;

            this.UpPnlPolicyMid.Update();
            this.upPnlButtons.Update();

        }

        /// <summary>
        /// Load data to populate ddl
        /// carica nella dropdownlist le tipoligie di documenti o fascicoli in base alla variabile isDoc
        /// </summary>
        /// <param name="querystrig"></param>
        private void PopulateddlPolicyTipologia()
        {

            Dictionary<string, string> lista;

            if (this.listaTipiDoc == null || this.listaTipiDoc.Keys.Count == 0 || this.listaTipiFasc == null || this.listaTipiFasc.Keys.Count == 0)
                this.GetListeTipologia();

            if (this.isDoc)
            {
                lista = this.listaTipiDoc;
            }
            else
            {
                lista = this.listaTipiFasc;
            }

            ListItem item;
            this.ddlPolicyTipologia.Items.Clear();
            this.ddlPolicyTipologia.Items.Add(new ListItem());
            if (lista != null && lista.Keys.Count > 0)
            {
                foreach (string id in lista.Keys.ToList())
                {
                    item = new ListItem();
                    item.Text = lista[id];
                    item.Value = id;
                    this.ddlPolicyTipologia.Items.Add(item);
                }
            }
        }

        /// <summary>
        /// carico le tipologie atto e fascicolo nel context
        /// </summary>
        private void GetListeTipologia()
        {
            DocsPaWR.TipologiaAtto[] listaTipologiaAtto = DocumentManager.ARCHIVE_GetListaTipologiaAtto(UserManager.GetUserInSession().idAmministrazione);
            DocsPaWR.Templates[] listaTipologiaFasc = ProjectManager.ARCHIVE_getTipoFasc(UserManager.GetUserInSession().idAmministrazione);
            this.listaTipiDoc = new Dictionary<string, string>();
            this.listaTipiFasc = new Dictionary<string, string>();
            foreach (TipologiaAtto ti in listaTipologiaAtto)
            {
                this.listaTipiDoc.Add(ti.systemId, ti.descrizione);
            }
            foreach (Templates te in listaTipologiaFasc)
            {
                this.listaTipiFasc.Add(te.SYSTEM_ID.ToString(), te.DESCRIZIONE);
            }

        }

        private void PopulateddlPolicyUo()
        {
            string dataUser = RoleManager.GetRoleInSession().systemId;
            dataUser = dataUser + "-" + this.ddlPolicyRegistri.SelectedValue;
            string contextKey = dataUser + "-" + UIManager.UserManager.GetUserInSession().idAmministrazione + "-" + "CALLTYPE_OWNER_AUTHOR";
            //CALL_TYPE_DEP_OSITO
            this.RapidCreatore.ContextKey = contextKey;
            //Prova ----------------------------------
            if (this.idUO2UO == null || this.idUO2UO.Count == 0)
            {
                this.idUO2UO = new Dictionary<string, ElementoRubrica>();
                ArrayList prova = UIManager.ArchiveManager.GetListaUOForTransferPolicy("", contextKey);

                //inizio test
                //Dictionary<string, ElementoRubrica> test= new Dictionary<string, ElementoRubrica>();

                ddlPolicyUo.Items.Add(new ListItem(string.Empty, null));
                foreach (NttDataWA.DocsPaWR.ElementoRubrica el in prova)
                {
                    if (!this.idUO2UO.ContainsKey(el.systemId))
                    {
                        this.idUO2UO.Add(el.systemId, el);
                        ddlPolicyUo.Items.Add(new ListItem(el.codice, el.systemId));
                    }
                }
            }
            else
            {
                ddlPolicyUo.Items.Add(new ListItem(string.Empty, null));
                foreach (NttDataWA.DocsPaWR.ElementoRubrica el in idUO2UO.Values)
                {
                    ddlPolicyUo.Items.Add(new ListItem(el.codice, el.systemId));
                }
            }
            //var testLinq = prova.ToArray().ToList().Select(x);
            //List<NttDataWA.DocsPaWR.ElementoRubrica> list=testLinq.Cast<List<NttDataWA.DocsPaWR.ElementoRubrica>>;



        }

        /// <summary>
        /// Get all date by Transfer systemId 
        /// </summary>
        private void GetDataByPageState()
        {
            //Faccio la get di tutta la pagina della policy, divisa in Aree:

            //Devo prendere la lista delle policy per popolare la grid
            this.TransferInContext = UIManager.ArchiveManager.GetARCHIVE_TransferBySystem_ID(this.Versamento_system_id);

            //Devo dividere gli oggetti utili per l'update:
            //ORIGINAL:
            this.TransferPolicyOriginalInContext = UIManager.ArchiveManager.GetARCHIVE_TransferPolicyByTransfer_ID(this.Versamento_system_id);

            //Devo dividere gli oggetti utili per l'update:
            //CORRENTE:
            this.TransferPolicyInContext = UIManager.ArchiveManager.GetARCHIVE_TransferPolicyByTransfer_ID(this.Versamento_system_id);

            //Valorizzo anche la vista delle policy associate a quel TranferID
            this.TransferPolicyViewInContext = UIManager.ArchiveManager.GetAllARCHIVE_View_Policy(this.Versamento_system_id);

            //Valorizzo la lista dei job (sia analisi che ricerca) per le policy del versamento attuale
            this.TransferPolicyJOBInContext = UIManager.ArchiveManager.GetARCHIVE_JOB_TransferPolicyByTransfer_ID(this.Versamento_system_id) != null ?
                UIManager.ArchiveManager.GetARCHIVE_JOB_TransferPolicyByTransfer_ID(this.Versamento_system_id) : new List<ARCHIVE_JOB_TransferPolicy>();

            //creazione pagina
            TxtIdVersamento.Text = TransferInContext[0].System_ID.ToString();
            TxtDescVers.Text = TransferInContext[0].Description;

            //lista policy selezionate, inizializzazione
            this.idPolicySelected = new List<Int32>();

            this.LoadDataInGrid();
        }

        /// <summary>
        /// Riempie la pagina con i dati della policy corrente
        /// </summary>
        private void PopulatePageObject()
        {
            //pulisco la pagina
            this.ClearPageObject();

            if (this.CurrentPolicyInContext != null)
            {
                this.TxtPolicyId.Text = this.CurrentPolicyInContext.System_ID.ToString();
                this.TxtDescPolicy.Text = this.CurrentPolicyInContext.Description;

                //policy type
                this.ddlDocFasc.SelectedItem.Selected = false;
                this.ddlDocFasc.Items.FindByValue(this.CurrentPolicyInContext.TransferPolicyType_ID.ToString()).Selected = true;
                this.ddlDocFasc_SelectedIndexChanged(null, new EventArgs());

                //titolario
                if (this.CurrentPolicyInContext.Titolario_ID != null)
                {
                    this.txtPolicyClassi.Text = this.CurrentPolicyInContext.ClasseTitolario != null ? this.CurrentPolicyInContext.ClasseTitolario : String.Empty;
                    this.ddlPolicyTitolatio.SelectedItem.Selected = false;
                    this.ddlPolicyTitolatio.Items.FindByValue(this.CurrentPolicyInContext.Titolario_ID.ToString()).Selected = true;
                }
                //includi sottolabero Titolario
                if (this.CurrentPolicyInContext.IncludiSottoalberoClasseTit != null && this.CurrentPolicyInContext.IncludiSottoalberoClasseTit == 1)
                    this.chkIncUO.Checked = true;

                //Registro/AOO
                if (this.CurrentPolicyInContext.Registro_ID != null)
                {
                    this.ddlPolicyRegistri.Items.FindByValue(this.CurrentPolicyInContext.Registro_ID.ToString()).Selected = true;
                    this.TxtAOODesc.Text = this.idRegAOO2DescAOO[this.CurrentPolicyInContext.Registro_ID.ToString()];
                }
                //Tipologia
                if (this.CurrentPolicyInContext.Tipologia_ID != null)
                {
                    this.ddlPolicyTipologia.Items.FindByValue(this.CurrentPolicyInContext.Tipologia_ID.ToString()).Selected = true;
                }

                //anno creazione e protocollazione/chiusura
                if (this.CurrentPolicyInContext.AnnoCreazioneA != null && this.CurrentPolicyInContext.AnnoCreazioneA != this.CurrentPolicyInContext.AnnoCreazioneDa)
                {
                    this.ddl_dtaCreate.Items.FindByValue("1").Selected = true;
                    this.ddl_dtaCreate_SelectedIndexChanged(null, new EventArgs());

                    this.dtaCreate_TxtTo.Text = !string.IsNullOrEmpty(this.CurrentPolicyInContext.AnnoCreazioneA.ToString()) ? this.CurrentPolicyInContext.AnnoCreazioneA.ToString() : string.Empty;
                    this.dtaCreate_TxtFrom.Text = !string.IsNullOrEmpty(this.CurrentPolicyInContext.AnnoCreazioneDa.ToString()) ? this.CurrentPolicyInContext.AnnoCreazioneDa.ToString() : string.Empty;

                }
                else
                {
                    this.ddl_dtaCreate.Items.FindByValue("0").Selected = true;
                    this.ddl_dtaCreate_SelectedIndexChanged(null, new EventArgs());
                    this.dtaCreate_TxtFrom.Text = !string.IsNullOrEmpty(this.CurrentPolicyInContext.AnnoCreazioneDa.ToString()) ? this.CurrentPolicyInContext.AnnoCreazioneDa.ToString() : string.Empty;

                }
                //chiusura/protocollazione
                if (this.isDoc) //se è un documento allora utilizzo anno di protocollazione
                {
                    if (this.CurrentPolicyInContext.AnnoProtocollazioneA != null && this.CurrentPolicyInContext.AnnoProtocollazioneA != this.CurrentPolicyInContext.AnnoProtocollazioneDa)
                    {
                        this.ddl_dtaProt.Items.FindByValue("1").Selected = true;
                        this.ddl_dtaProt_SelectedIndexChanged(null, new EventArgs());
                        this.dtaProt_TxtFrom.Text = !string.IsNullOrEmpty(this.CurrentPolicyInContext.AnnoProtocollazioneDa.ToString()) ? this.CurrentPolicyInContext.AnnoProtocollazioneDa.ToString() : string.Empty;
                        this.dtaProt_TxtTo.Text = !string.IsNullOrEmpty(this.CurrentPolicyInContext.AnnoProtocollazioneA.ToString()) ? this.CurrentPolicyInContext.AnnoProtocollazioneA.ToString() : string.Empty;
                        //this.dtaProt_TxtTo.Text = this.CurrentPolicyInContext.AnnoProtocollazioneA.ToString();
                    }
                    else
                    {
                        this.ddl_dtaProt.Items.FindByValue("0").Selected = true;
                        this.ddl_dtaProt_SelectedIndexChanged(null, new EventArgs());
                        this.dtaProt_TxtFrom.Text = !string.IsNullOrEmpty(this.CurrentPolicyInContext.AnnoProtocollazioneDa.ToString()) ? this.CurrentPolicyInContext.AnnoProtocollazioneDa.ToString() : string.Empty;
                    }
                }
                else  //altrimenti se è un fascicolo uso anno di chiusura
                {
                    if (this.CurrentPolicyInContext.AnnoChiusuraA != null && this.CurrentPolicyInContext.AnnoChiusuraA != this.CurrentPolicyInContext.AnnoChiusuraDa)
                    {
                        this.ddl_dtaProt.Items.FindByValue("1").Selected = true;
                        this.ddl_dtaProt_SelectedIndexChanged(null, new EventArgs());
                        this.dtaProt_TxtFrom.Text = !string.IsNullOrEmpty(this.CurrentPolicyInContext.AnnoChiusuraDa.ToString()) ? this.CurrentPolicyInContext.AnnoChiusuraDa.ToString() : string.Empty;
                        this.dtaProt_TxtTo.Text = !string.IsNullOrEmpty(this.CurrentPolicyInContext.AnnoChiusuraA.ToString()) ? this.CurrentPolicyInContext.AnnoChiusuraA.ToString() : string.Empty;
                        //this.dtaProt_TxtTo.Text = this.CurrentPolicyInContext.AnnoProtocollazioneA.ToString();
                    }
                    else
                    {
                        this.ddl_dtaProt.Items.FindByValue("0").Selected = true;
                        this.ddl_dtaProt_SelectedIndexChanged(null, new EventArgs());
                        this.dtaProt_TxtFrom.Text = !string.IsNullOrEmpty(this.CurrentPolicyInContext.AnnoChiusuraDa.ToString()) ? this.CurrentPolicyInContext.AnnoChiusuraDa.ToString() : string.Empty;
                    }

                }
                //UO
                this.FieldUO.Value = this.CurrentPolicyInContext.UO_ID == null ? this.CurrentPolicyInContext.UO_ID.ToString() : String.Empty;
                if (!string.IsNullOrEmpty(this.CurrentPolicyInContext.UO_ID.ToString()) && this.idUO2UO.ContainsKey(this.CurrentPolicyInContext.UO_ID.ToString()))
                {
                    this.ddlPolicyUo.SelectedItem.Selected = false;
                    this.ddlPolicyUo.Items.FindByValue(this.CurrentPolicyInContext.UO_ID.ToString()).Selected = true;
                    this.TxtUODesc.Text = this.idUO2UO[this.CurrentPolicyInContext.UO_ID.ToString()].descrizione;

                }

                //varie check box
                this.chkIncUO.Checked = CurrentPolicyInContext.IncludiSottoalberoUO != null ? CurrentPolicyInContext.IncludiSottoalberoUO == 1 : false;
                this.ChkIncludeClass.Checked = CurrentPolicyInContext.IncludiSottoalberoClasseTit != null ? CurrentPolicyInContext.IncludiSottoalberoClasseTit == 1 : false;
                if (this.CurrentProfileTypeInContext != null)
                {
                    this.chkArrivo.Checked = CurrentProfileTypeInContext.Any(x => x.ProfileType_ID ==
                        UIManager.ArchiveManager._dictionaryProfileType.Where(y => y.Value == "ARRIVO").Select(y => y.Key).FirstOrDefault());
                    this.chkPartenza.Checked = CurrentProfileTypeInContext.Any(x => x.ProfileType_ID ==
                        UIManager.ArchiveManager._dictionaryProfileType.Where(y => y.Value == "PARTENZA").Select(y => y.Key).FirstOrDefault());
                    this.chkInt.Checked = CurrentProfileTypeInContext.Any(x => x.ProfileType_ID ==
                        UIManager.ArchiveManager._dictionaryProfileType.Where(y => y.Value == "INTERNO").Select(y => y.Key).FirstOrDefault());
                    this.chkNonProt.Checked = CurrentProfileTypeInContext.Any(x => x.ProfileType_ID ==
                        UIManager.ArchiveManager._dictionaryProfileType.Where(y => y.Value == "NON PROTOCOLLATO").Select(y => y.Key).FirstOrDefault());
                    this.chkStampaRep.Checked = CurrentProfileTypeInContext.Any(x => x.ProfileType_ID ==
                        UIManager.ArchiveManager._dictionaryProfileType.Where(y => y.Value == "STAMPA REPERTORIO").Select(y => y.Key).FirstOrDefault());
                    this.chkStampaRegProt.Checked = CurrentProfileTypeInContext.Any(x => x.ProfileType_ID ==
                        UIManager.ArchiveManager._dictionaryProfileType.Where(y => y.Value == "STAMPA REGISTRO PROTOCOLLATO").Select(y => y.Key).FirstOrDefault());
                }

                //abilitazione dei cambi in funzione dello stato della policy

                if (this.CurrentPolicyInContext.TransferPolicyState_ID !=
                    UIManager.ArchiveManager._dictionaryTransferPolicyState.FirstOrDefault(x => x.Value == "RICERCA NON AVVIATA").Key ||
                    this.TransferPolicyJOBInContext.Where(x => x.TransferPolicy_ID == this.CurrentPolicyInContext.System_ID && x.JobType_ID == 1 && x.Executed == 0).ToList().Count > 0)
                {
                    this.dtaCreate_TxtFrom.Enabled = false;
                    this.dtaCreate_TxtTo.Enabled = false;
                    this.ddl_dtaCreate.Enabled = false;
                    this.dtaProt_TxtFrom.Enabled = false;
                    this.dtaProt_TxtTo.Enabled = false;
                    this.ddl_dtaProt.Enabled = false;

                    this.ddlPolicyRegistri.Enabled = false;
                    this.ddlPolicyTipologia.Enabled = false;
                    this.ddlDocFasc.Enabled = false;
                    this.ddlPolicyTitolatio.Enabled = false;
                    this.TxtAOODesc.Enabled = false;
                    this.TxtUODesc.Enabled = false;
                    //this.FieldUO.Enabled = false;
                    this.txtPolicyClassi.Enabled = false;
                    this.txtPolicyClassiDesc.Enabled = false;

                    this.btnclassificationschema.Enabled = false;
                    this.ddlPolicyUo.Enabled = false;

                    this.chkArrivo.Enabled = false;
                    this.chkPartenza.Enabled = false;
                    this.chkInt.Enabled = false;
                    this.chkStampaRegProt.Enabled = false;
                    this.chkStampaRep.Enabled = false;
                    this.chkNonProt.Enabled = false;

                    this.ChkIncludeClass.Enabled = false;
                    this.chkIncUO.Enabled = false;
                }
                else
                {
                    this.dtaCreate_TxtFrom.Enabled = true;
                    this.dtaCreate_TxtTo.Enabled = true;
                    this.ddl_dtaCreate.Enabled = true;
                    this.dtaProt_TxtFrom.Enabled = true;
                    this.dtaProt_TxtTo.Enabled = true;
                    this.ddl_dtaProt.Enabled = true;

                    this.ddlPolicyRegistri.Enabled = true;
                    this.ddlPolicyTipologia.Enabled = true;
                    this.ddlDocFasc.Enabled = true;
                    this.ddlPolicyTitolatio.Enabled = true;
                    this.TxtAOODesc.Enabled = true;
                    this.TxtUODesc.Enabled = true;
                    //this.FieldUO.Enabled = true;
                    this.txtPolicyClassi.Enabled = true;
                    this.txtPolicyClassiDesc.Enabled = true;

                    this.btnclassificationschema.Enabled = true;
                    this.ddlPolicyUo.Enabled = true;

                    this.chkArrivo.Enabled = true;
                    this.chkPartenza.Enabled = true;
                    this.chkInt.Enabled = true;
                    this.chkStampaRegProt.Enabled = true;
                    this.chkStampaRep.Enabled = true;
                    this.chkNonProt.Enabled = true;

                    this.ChkIncludeClass.Enabled = true;
                    this.chkIncUO.Enabled = true;

                }

            }
        }

        /// <summary>
        /// pulisce la pagina dai dati della policy precedentemente visualizzata e si ripristinano le abilitazioni degli oggetti.
        /// Ha senso fare questo perchè se non si effettua la pulizia si rischia di popolare
        /// dei campi, della policy corrente, non obbligatori, con quelli della policy precendete
        /// </summary>
        private void ClearPageObject()
        {
            this.ddlPolicyRegistri.SelectedItem.Selected = false;
            this.ddlPolicyTipologia.SelectedItem.Selected = false;
            this.ddl_dtaCreate.SelectedItem.Selected = false;
            this.ddl_dtaProt.SelectedItem.Selected = false;
            this.ddlPolicyUo.SelectedItem.Selected = false;
            this.ddlPolicyTitolatio.SelectedItem.Selected = false;

            this.TxtAOODesc.Text = String.Empty;
            this.TxtDescPolicy.Text = String.Empty;
            this.TxtPolicyId.Text = String.Empty;
            this.FieldUO.Value = String.Empty;
            this.TxtAOODesc.Text = String.Empty;
            this.txtPolicyClassi.Text = String.Empty;
            this.TxtUODesc.Text = String.Empty;
            this.txtPolicyClassiDesc.Text = string.Empty;
            this.chkIncUO.Checked = false;
            this.dtaCreate_TxtFrom.Text = String.Empty;
            this.dtaCreate_TxtTo.Text = String.Empty;
            this.dtaProt_TxtFrom.Text = String.Empty;
            this.dtaProt_TxtTo.Text = String.Empty;

            this.dtaCreate_TxtTo.Visible = false;
            this.dtaProt_TxtTo.Visible = false;
            this.lbl_dtaCreateTo.Visible = false;
            this.lbl_dtaProtTo.Visible = false;

            this.chkArrivo.Checked = false;
            this.chkPartenza.Checked = false;
            this.chkInt.Checked = false;
            this.chkStampaRegProt.Checked = false;
            this.chkStampaRep.Checked = false;
            this.chkNonProt.Checked = false;

            this.ChkIncludeClass.Checked = false;
            this.chkIncUO.Checked = false;

            this.dtaCreate_TxtFrom.Enabled = true;
            this.dtaCreate_TxtTo.Enabled = true;
            this.ddl_dtaCreate.Enabled = true;
            this.dtaProt_TxtFrom.Enabled = true;
            this.dtaProt_TxtTo.Enabled = true;
            this.ddl_dtaProt.Enabled = true;

            this.ddlPolicyRegistri.Enabled = true;
            this.ddlPolicyTipologia.Enabled = true;
            this.ddlDocFasc.Enabled = true;
            this.ddlPolicyUo.Enabled = true;
            this.ddlPolicyTitolatio.Enabled = true;
            this.TxtAOODesc.Enabled = true;
            this.TxtUODesc.Enabled = true;
            //this.FieldUO.Enabled = true;
            this.txtPolicyClassi.Enabled = true;
            this.txtPolicyClassiDesc.Enabled = true;

            this.btnclassificationschema.Enabled = true;

            this.chkArrivo.Enabled = true;
            this.chkPartenza.Enabled = true;
            this.chkInt.Enabled = true;
            this.chkStampaRegProt.Enabled = true;
            this.chkStampaRep.Enabled = true;
            this.chkNonProt.Enabled = true;

            this.ChkIncludeClass.Enabled = true;
            this.chkIncUO.Enabled = true;


        }

        /// <summary>
        /// aggiorna i dati nella griglia della policy 
        /// ricaricando la lista
        /// </summary>
        private void UpdateDataInGrid()
        {
            this.TransferPolicyViewInContext = UIManager.ArchiveManager.GetAllARCHIVE_View_Policy(this.Versamento_system_id);
            this.LoadDataInGrid();
        }

        /// <summary>
        /// carica i dati nella griglia della policy 
        /// </summary>
        private void LoadDataInGrid()
        {
            try
            {
                if (TransferPolicyViewInContext != null && TransferPolicyViewInContext.Count != 0)
                {
                    List<Int32> jobInRicerca = new List<int>();
                    List<Int32> jobInAnalisi = new List<int>();

                    if (this.TransferPolicyJOBInContext.Count > 0 && this.TransferPolicyJOBInContext != null)
                    {
                        jobInRicerca = this.TransferPolicyJOBInContext.Where(x => x.JobType_ID == 1 && x.Executed == 0).Select(x => x.TransferPolicy_ID).Distinct().ToList();
                        jobInAnalisi = this.TransferPolicyJOBInContext.Where(x => x.JobType_ID == 2 && x.Executed == 0).Select(x => x.TransferPolicy_ID).Distinct().ToList();
                    }
                    var query = (from o in TransferPolicyInContext
                                 join p in TransferPolicyViewInContext
                                      on o.System_ID equals p.Id_policy
                                 select new ARCHIVE_View_Policy
                                 {
                                     Id_policy = p.Id_policy,
                                     Descrizione = p.Descrizione,
                                     Totale_documenti = p.Totale_documenti,
                                     Totale_fascicoli = p.Totale_fascicoli,
                                     Num_documenti_copiati = p.Num_documenti_copiati,
                                     Num_documenti_trasferiti = p.Num_documenti_trasferiti,
                                     Stato = jobInRicerca.Contains(p.Id_policy) ? "RICERCA PROGRAMMATA" : (jobInAnalisi.Contains(p.Id_policy) ? "ANALISI PROGRAMMATA" : p.Stato)
                                     //TransferPolicyType_ID = o.TransferPolicyType_ID        //tipo di policy (inutilizzato)
                                 });

                    var rows = query.ToList();
                    while (rows.Count() % 9 != 0)
                    {
                        rows.Add(new ARCHIVE_View_Policy());
                    }

                    this.GrdPolicy.DataSource = rows.OrderByDescending(x => x.Id_policy).ToList();
                    //this.LitGridPolicyVuota.Visible = false;
                    this.GrdPolicy.DataBind();
                    this.UpPnlGridResult.Update();
                }
                else
                {
                    //this.LitGridPolicyVuota.Visible = true;
                    this.selectedAll = false;
                    List<ARCHIVE_View_Policy> rows = new List<ARCHIVE_View_Policy>();
                    while (rows.Count() < 9)
                    {
                        rows.Add(new ARCHIVE_View_Policy());
                    }
                    //Assegno un source vuoto per fare cmq il bind: policy
                    this.GrdPolicy.DataSource = rows;
                    this.GrdPolicy.DataBind();

                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }

        }


        private void VisibiltyRoleFunctions()
        {

        }

        private void PopulateddlPolicyTitolatio()
        {
            this.ddlPolicyTitolatio.Items.Clear();
            ArrayList listaTitolari = ClassificationSchemeManager.getTitolariUtilizzabili();
            this.ddlPolicyTitolatio.Items.Add(new ListItem("Tutti i titolari", ""));

            //Esistono dei titolari chiusi
            if (listaTitolari.Count > 1)
            {
                //Creo le voci della ddl dei titolari
                string valueTutti = string.Empty;
                foreach (DocsPaWR.OrgTitolario titolario in listaTitolari)
                {
                    ListItem it = null;
                    switch (titolario.Stato)
                    {
                        case DocsPaWR.OrgStatiTitolarioEnum.Attivo:
                            it = new ListItem(titolario.Descrizione, titolario.ID);
                            this.ddlPolicyTitolatio.Items.Add(it);
                            break;
                        case DocsPaWR.OrgStatiTitolarioEnum.Chiuso:
                            it = new ListItem(titolario.Descrizione, titolario.ID);
                            this.ddlPolicyTitolatio.Items.Add(it);
                            break;
                    }
                }

            }
            else
            {
                DocsPaWR.OrgTitolario titolario = (DocsPaWR.OrgTitolario)listaTitolari[0];
                if (titolario.Stato != DocsPaWR.OrgStatiTitolarioEnum.InDefinizione)
                {
                    ListItem it = new ListItem(titolario.Descrizione, titolario.ID);
                    this.ddlPolicyTitolatio.Items.Add(it);
                }
                this.ddlPolicyTitolatio.Enabled = false;

            }
        }


        private void ReadRetValueFromPopup()
        {
            //valore dal popup titolario
            if (!string.IsNullOrEmpty(this.OpenTitolario.ReturnValue))
            {
                if (this.ReturnValue.Split('#').Length > 1)
                {
                    this.txtPolicyClassi.Text = this.ReturnValue.Split('#').First();
                    this.txtPolicyClassiDesc.Text = this.ReturnValue.Split('#').Last();
                    this.UpPnlPolicyMid.Update();
                }
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('OpenTitolario','')", true);
            }

            //Elimina policy corrente
            if (!string.IsNullOrEmpty(this.HiddeConfirmDeleteTransferPolicy.Value))
            {
                this.HiddeConfirmDeleteTransferPolicy.Value = string.Empty;
                this.upPnlHiddeConfirmDeleteTransferPolicy.Update();

                if (this.TransferPolicyViewInContext != null && this.TransferPolicyInContext != null)
                {
                    List<Int32> listIdPolicyDaEliminare = new List<Int32>();

                    listIdPolicyDaEliminare = (from id in this.idPolicySelected
                                               join p in this.TransferPolicyInContext
                                                    on id equals p.System_ID
                                               where p.Enabled == 0
                                               select id).ToList();

                    bool result = false;

                    if (listIdPolicyDaEliminare.Count > 0)
                    {
                        result = UIManager.ArchiveManager.DeleteARCHIVE_TransferPolicyList(UIManager.ArchiveManager.GetSQLinStringFromListIdPolicy(listIdPolicyDaEliminare));

                        this.GetDataByPageState();
                        bool deletedAll = false;
                        if (selectedAll)
                            deletedAll = this.idPolicySelected.Count == listIdPolicyDaEliminare.Count;
                        if (this.selectedAll && deletedAll //se le policy sono tutte selezionate e sono state tutte cancellate
                            || (this.CurrentPolicyInContext != null && !this.TransferPolicyInContext.Contains(this.CurrentPolicyInContext))) //o la policy corrente è stata elimitata
                            this.btnPolicyRimuoviFiltro_Click(this, new EventArgs());

                        this.selectedAll = false;
                        this.UpdateDataInGrid();
                        this.UpPnlGridResult.Update();

                        string msgErrNewArchiveTransferPolicy;
                        if (!result)
                        {   //MESSAGGIO DI CONFERMA ELIMINAZIONE AVVENUTA
                            msgErrNewArchiveTransferPolicy = "msgErrDeletePolicy";
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('" + msgErrNewArchiveTransferPolicy.Replace("'", @"\'") + "', 'warning', '');", true);
                            return;
                        }
                        else
                        {   //MESSAGGIO DI ERRORE ELIMINAZIONE NON AVVENUTA
                            msgErrNewArchiveTransferPolicy = "msgConfirmDeletePolicy";
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('" + msgErrNewArchiveTransferPolicy.Replace("'", @"\'") + "', 'check', '');", true);
                            return;
                        }

                    }
                    else
                    {   //MESSAGGIO DI ERRORE NUSSUNA POLICY VALIDA ALLA CANCELLAZIONE
                        string msgErrNewArchiveTransferPolicy = "msgErrCheckDeletePolicy";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('" + msgErrNewArchiveTransferPolicy.Replace("'", @"\'") + "', 'warning', '');", true);
                        return;
                    }
                }
            }

            //Aggiorna policy corrente
            if (!string.IsNullOrEmpty(this.HiddeConfirmUpdateTransferPolicy.Value))
            {
                this.HiddeConfirmUpdateTransferPolicy.Value = string.Empty;

                bool result;

                if (CurrentPolicyInContext.TransferPolicyState_ID == UIManager.ArchiveManager._dictionaryTransferPolicyState.FirstOrDefault(x => x.Value == "RICERCA NON AVVIATA").Key ||
                    !this.TransferPolicyJOBInContext.Where(x => x.JobType_ID == 1).Select(x => x.TransferPolicy_ID).Contains(CurrentPolicyInContext.System_ID))
                {
                    int TransferPolicyType;
                    if (ddlDocFasc.SelectedItem.Value.Equals(UIManager.ArchiveManager._dictionaryTransferPolicyType.Where(x => x.Value == "DOCUMENTI").Select(x => x.Key).FirstOrDefault().ToString()))
                        TransferPolicyType =
                            UIManager.ArchiveManager._dictionaryTransferPolicyType.Where(x => x.Value == "DOCUMENTI").Select(x => x.Key).FirstOrDefault();
                    else
                        TransferPolicyType =
                            UIManager.ArchiveManager._dictionaryTransferPolicyType.Where(x => x.Value == "FASCICOLI").Select(x => x.Key).FirstOrDefault();
                    //OBBLIGATORI
                    this.CurrentPolicyInContext.Description = TxtDescPolicy.Text.Trim();
                    this.CurrentPolicyInContext.TransferPolicyType_ID = TransferPolicyType;
                    this.CurrentPolicyInContext.Tipologia_ID = !string.IsNullOrEmpty(this.ddlPolicyTipologia.SelectedItem.Value) ? int.Parse(this.ddlPolicyTipologia.SelectedItem.Value) : new int?();
                    this.CurrentPolicyInContext.Registro_ID = !string.IsNullOrEmpty(this.ddlPolicyRegistri.SelectedItem.Value) ? int.Parse(this.ddlPolicyRegistri.SelectedItem.Value) : new int?();
                    this.CurrentPolicyInContext.Titolario_ID = !string.IsNullOrEmpty(this.ddlPolicyTitolatio.SelectedItem.Value) ? int.Parse(this.ddlPolicyTitolatio.SelectedItem.Value) : new int?();
                    this.CurrentPolicyInContext.ClasseTitolario = !string.IsNullOrEmpty(this.txtPolicyClassi.Text) ? this.txtPolicyClassi.Text : String.Empty;

                    if (ddl_dtaCreate.SelectedItem.Value.Equals("0") || string.IsNullOrEmpty(dtaCreate_TxtTo.Text))
                    {
                        this.CurrentPolicyInContext.AnnoCreazioneA = !string.IsNullOrEmpty(dtaCreate_TxtFrom.Text) ? int.Parse(dtaCreate_TxtFrom.Text) : new int?();
                        this.CurrentPolicyInContext.AnnoCreazioneDa = !string.IsNullOrEmpty(dtaCreate_TxtFrom.Text) ? int.Parse(dtaCreate_TxtFrom.Text) : new int?();
                    }
                    else
                    {
                        this.CurrentPolicyInContext.AnnoCreazioneA = !string.IsNullOrEmpty(dtaCreate_TxtTo.Text) ? int.Parse(dtaCreate_TxtTo.Text) : new int?();
                        this.CurrentPolicyInContext.AnnoCreazioneDa = !string.IsNullOrEmpty(dtaCreate_TxtFrom.Text) ? int.Parse(dtaCreate_TxtFrom.Text) : new int?();
                    }

                    if (this.isDoc)
                    {   //se è un doc anno protocollazione
                        if (ddl_dtaProt.SelectedItem.Value.Equals("0") || string.IsNullOrEmpty(dtaProt_TxtTo.Text))
                        {
                            this.CurrentPolicyInContext.AnnoProtocollazioneA = !string.IsNullOrEmpty(dtaProt_TxtFrom.Text) ? int.Parse(dtaProt_TxtFrom.Text) : new int?();
                            this.CurrentPolicyInContext.AnnoProtocollazioneDa = !string.IsNullOrEmpty(dtaProt_TxtFrom.Text) ? int.Parse(dtaProt_TxtFrom.Text) : new int?();
                        }
                        else
                        {
                            this.CurrentPolicyInContext.AnnoProtocollazioneA = !string.IsNullOrEmpty(dtaProt_TxtTo.Text) ? int.Parse(dtaProt_TxtTo.Text) : new int?();
                            this.CurrentPolicyInContext.AnnoProtocollazioneDa = !string.IsNullOrEmpty(dtaProt_TxtFrom.Text) ? int.Parse(dtaProt_TxtFrom.Text) : new int?();
                        }
                        CurrentPolicyInContext.AnnoChiusuraA = CurrentPolicyInContext.AnnoChiusuraDa = null;
                    }
                    else
                    {   //se è un fascicolo anno chiusura
                        if (ddl_dtaProt.SelectedItem.Value.Equals("0") || string.IsNullOrEmpty(dtaProt_TxtTo.Text))
                        {
                            this.CurrentPolicyInContext.AnnoChiusuraA = !string.IsNullOrEmpty(dtaProt_TxtFrom.Text) ? int.Parse(dtaProt_TxtFrom.Text) : new int?();
                            this.CurrentPolicyInContext.AnnoChiusuraDa = !string.IsNullOrEmpty(dtaProt_TxtFrom.Text) ? int.Parse(dtaProt_TxtFrom.Text) : new int?();
                        }
                        else
                        {
                            this.CurrentPolicyInContext.AnnoChiusuraA = !string.IsNullOrEmpty(dtaProt_TxtTo.Text) ? int.Parse(dtaProt_TxtTo.Text) : new int?();
                            this.CurrentPolicyInContext.AnnoChiusuraDa = !string.IsNullOrEmpty(dtaProt_TxtFrom.Text) ? int.Parse(dtaProt_TxtFrom.Text) : new int?();
                        }
                        CurrentPolicyInContext.AnnoProtocollazioneA = CurrentPolicyInContext.AnnoProtocollazioneDa = null;
                    }
                    CurrentPolicyInContext.IncludiSottoalberoUO = this.chkIncUO.Checked ? 1 : 0;
                    CurrentPolicyInContext.IncludiSottoalberoClasseTit = this.ChkIncludeClass.Checked ? 1 : 0;

                    this.CurrentPolicyInContext.UO_ID = !string.IsNullOrEmpty(this.ddlPolicyUo.SelectedItem.Value) ? int.Parse(this.ddlPolicyUo.SelectedItem.Value) : new int?();

                    //Aggiorno la policy
                    if (this.isDoc)
                    {
                        result = UIManager.ArchiveManager.UpdateARCHIVE_TransferPolicy(CurrentPolicyInContext, this.chkArrivo.Checked, this.chkPartenza.Checked,
                            this.chkInt.Checked, this.chkNonProt.Checked, this.chkStampaRegProt.Checked, this.chkStampaRep.Checked);
                    }
                    else
                        result = UIManager.ArchiveManager.UpdateARCHIVE_TransferPolicy(CurrentPolicyInContext, false, false, false, false, false, false);
                }


                else
                {
                    this.CurrentPolicyInContext.Description = this.TxtDescPolicy.Text;
                    result = UIManager.ArchiveManager.UpdateARCHIVE_TransferPolicy(this.CurrentPolicyInContext);
                }


                this.ClearPageObject();
                this.GetDataByPageState();

                //ricarico la policy corrente
                int policyId = this.CurrentPolicyInContext.System_ID;
                this.CurrentPolicyInContext = (from x in this.TransferPolicyInContext
                                               where x.System_ID == policyId
                                               select x).Cast<DocsPaWR.ARCHIVE_TransferPolicy>().ToList().FirstOrDefault();

                this.OriginalPolicyInContext = (from x in this.TransferPolicyOriginalInContext
                                                where x.System_ID == policyId
                                                select x).Cast<DocsPaWR.ARCHIVE_TransferPolicy>().ToList().FirstOrDefault();

                this.CurrentProfileTypeInContext = UIManager.ArchiveManager.GetARCHIVE_TransferPolicy_ProfileTypeByTransferPolicy_ID(policyId);
                this.CurrentProfileTypeInContext = this.CurrentProfileTypeInContext != null ? this.CurrentProfileTypeInContext : new List<ARCHIVE_TransferPolicy_ProfileType>();

                this.OriginalProfileTypeInContext = UIManager.ArchiveManager.GetARCHIVE_TransferPolicy_ProfileTypeByTransferPolicy_ID(policyId);
                this.OriginalProfileTypeInContext = this.OriginalProfileTypeInContext != null ? this.OriginalProfileTypeInContext : new List<ARCHIVE_TransferPolicy_ProfileType>();

                this.PopulatePageObject();
                this.CallVisibilityByPolicyState(Session["PolicyState"]);

                string message;
                string messageType;
                if (result)
                {
                    message = "msgUpdatePolicySuccess";
                    messageType = "check";
                }
                else
                {
                    message = "msgUpdatePolicyFailed";
                    messageType = "warning";
                }
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('" + message.Replace("'", @"\'") + "','" + messageType + "' , '');", true);
            }


        }

        protected void FieldUO_onValueChenge(object sender, EventArgs e)
        {
            this.ddlPolicyUo.SelectedItem.Selected = false;

            if (!string.IsNullOrEmpty(FieldUO.Value) && this.idUO2UO.Where(x => x.Value.codice.Equals(FieldUO.Value)).Count() > 0)
                this.ddlPolicyUo.Items.FindByText(FieldUO.Value).Selected = true;
        }

        protected void ddl_dtaProt_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string language = UIManager.UserManager.GetUserLanguage();
                switch (this.ddl_dtaProt.SelectedIndex)
                {
                    case 0: //Valore singolo
                        //this.dtaProt_TxtFrom.ReadOnly = false;
                        this.dtaProt_TxtTo.Visible = false;
                        this.lbl_dtaProtTo.Visible = false;
                        this.lbl_dtaProtFrom.Text = Utils.Languages.GetLabelFromCode("VisibilityOneField", language);
                        break;
                    case 1: //Intervallo
                        //this.dtaProt_TxtFrom.ReadOnly = false;
                        //this.dtaProt_TxtTo.ReadOnly = false;
                        this.lbl_dtaProtTo.Visible = true;
                        this.lbl_dtaProtFrom.Visible = true;
                        this.dtaProt_TxtTo.Visible = true;
                        this.lbl_dtaProtFrom.Text = Utils.Languages.GetLabelFromCode("VisibilityFrom", language);
                        this.lbl_dtaProtTo.Text = Utils.Languages.GetLabelFromCode("VisibilityTo", language);
                        break;

                }

                this.upPnlIntervals.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void ddl_dtaCreate_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string language = UIManager.UserManager.GetUserLanguage();
                switch (this.ddl_dtaCreate.SelectedIndex)
                {
                    case 0: //Valore singolo
                        //this.dtaCreate_TxtFrom.ReadOnly = false;
                        this.dtaCreate_TxtTo.Visible = false;
                        this.lbl_dtaCreateTo.Visible = false;
                        this.lbl_dtaCreateFrom.Text = Utils.Languages.GetLabelFromCode("VisibilityOneField", language);
                        break;
                    case 1: //Intervallo
                        //this.dtaCreate_TxtFrom.ReadOnly = false;
                        //this.dtaCreate_TxtTo.ReadOnly = false;
                        this.lbl_dtaCreateTo.Visible = true;
                        this.lbl_dtaCreateFrom.Visible = true;
                        this.dtaCreate_TxtTo.Visible = true;
                        this.lbl_dtaCreateFrom.Text = Utils.Languages.GetLabelFromCode("VisibilityFrom", language);
                        this.lbl_dtaCreateTo.Text = Utils.Languages.GetLabelFromCode("VisibilityTo", language);
                        break;

                }
                //COMMENTO PERCHè NON CI SONO GLI UPDATEPANEL
                this.upPnlIntervals.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void ddlDocFasc_SelectedIndexChanged(object sender, EventArgs e)
        {
            string language = UIManager.UserManager.GetUserLanguage();
            string lit = string.Empty;
            if (ddlDocFasc.SelectedItem.Value.Equals(UIManager.ArchiveManager._dictionaryTransferPolicyType.Where(x => x.Value == "DOCUMENTI").Select(x => x.Key).FirstOrDefault().ToString()))
            {

                this.isDoc = true;
                lit = "LitAnnoProtocollazione";
            }
            else
            {
                this.isDoc = false;
                lit = "LitAnnoChiusura";
            }
            this.LitAnnoProtocollazione.Text = Utils.Languages.GetLabelFromCode(lit, language);
            PlcPolicyTip.Visible = this.isDoc;
            this.PopulateddlPolicyTipologia();
            this.UpPnlPolicyMid.Update();
        }

        protected void txtPolicyClassi_OnTextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(this.txtPolicyClassi.Text))
            {

                string codClassificazione = this.txtPolicyClassi.Text.ToString();

                this.cercaClassificazioneDaCodice(codClassificazione);
            }
            else
            {
                this.txtPolicyClassi.Text = string.Empty;
                this.txtPolicyClassiDesc.Text = string.Empty;
                //this.IdProject.Value = string.Empty;
                
            }

            this.UpPnlPolicyMid.Update();
        }

        private bool cercaClassificazioneDaCodice(string codClassificazione)
        {
            bool res = false;
            //DocsPaWR.Fascicolo[] listaFasc;
            if (!string.IsNullOrEmpty(codClassificazione))
            {
                //listaFasc = this.getFascicolo(this.Registry, codClassificazione);

                DocsPaWR.FascicolazioneClassificazione[] FascClass = ProjectManager.fascicolazioneGetTitolario2(this, codClassificazione,RegistryManager.getRegistroBySistemId(this.ddlPolicyRegistri.SelectedValue), false, this.ddlPolicyTitolatio.SelectedValue);
                if (FascClass != null && FascClass.Length != 0)
                {
                    FascicolazioneClassificazione Classification = FascClass[0];
                    this.txtPolicyClassi.Text = Classification.codice;
                    this.txtPolicyClassiDesc.Text = Classification.descrizione;
                    this.IdProject.Value = Classification.systemID;
                }
                else
                {
                    this.txtPolicyClassi.Text = string.Empty;
                    this.txtPolicyClassiDesc.Text = string.Empty;
                    this.IdProject.Value = string.Empty;
                }
            }

            return res;
        }

        #region checkbox
        protected void chkIncUO_CheckedChanged(object sender, EventArgs e)
        {

        }

        protected void chkArrivo_CheckedChanged(object sender, EventArgs e)
        {

        }


        protected void chkPartenza_CheckedChanged(object sender, EventArgs e)
        {

        }

        protected void chkInt_CheckedChanged(object sender, EventArgs e)
        {

        }

        protected void chkNonProt_CheckedChanged(object sender, EventArgs e)
        {

        }

        protected void chkStampaRegProt_CheckedChanged(object sender, EventArgs e)
        {

        }

        protected void chkStampaRep_CheckedChanged(object sender, EventArgs e)
        {

        }

        #endregion

        protected void ddlPolicyTipologia_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void ddlPolicyUO_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.TxtUODesc.Text = !string.IsNullOrEmpty(ddlPolicyUo.SelectedItem.Value) ? this.idUO2UO[ddlPolicyUo.SelectedItem.Value].descrizione : string.Empty;
        }

        protected void ddlPolicyRegistri_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ddlPolicyRegistri.SelectedItem.Value))
            {
                TxtAOODesc.Text = this.idRegAOO2DescAOO[ddlPolicyRegistri.SelectedItem.Value];
                UIManager.RegistryManager.SetRegistryInSession(RegistryManager.getRegistroBySistemId(ddlPolicyRegistri.SelectedItem.Value));
            }
            else
            {
                TxtAOODesc.Text = string.Empty;
            }
        }

        protected void ddlPolicyTitolatio_SelectedIndexChanged(object sender, EventArgs e)
        {

            UIManager.ClassificationSchemeManager.SetTitolarioInSession(UIManager.ClassificationSchemeManager.getTitolario(ddlPolicyTitolatio.SelectedValue));
        }

        protected void btnclassificationschema_Click(object sender, ImageClickEventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "OpenTitolario", "ajaxModalPopupOpenTitolario();", true);
        }


        #region eventi griglia
        //eventi della griglia


        protected void GrdPolicy_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            List<Int32> jobInRicerca = new List<int>();
            List<Int32> jobInAnalisi = new List<int>();

            if (this.TransferPolicyJOBInContext.Count > 0 && this.TransferPolicyJOBInContext != null)
            {
                jobInRicerca = this.TransferPolicyJOBInContext.Where(x => x.JobType_ID == 1 && x.Executed == 0).Select(x => x.TransferPolicy_ID).Distinct().ToList();
                jobInAnalisi = this.TransferPolicyJOBInContext.Where(x => x.JobType_ID == 2 && x.Executed == 0).Select(x => x.TransferPolicy_ID).Distinct().ToList();
            }
            var query = (from o in TransferPolicyInContext
                         join p in TransferPolicyViewInContext
                              on o.System_ID equals p.Id_policy
                         select new ARCHIVE_View_Policy
                         {
                             Id_policy = p.Id_policy,
                             Descrizione = p.Descrizione,
                             Totale_documenti = p.Totale_documenti,
                             Totale_fascicoli = p.Totale_fascicoli,
                             Num_documenti_copiati = p.Num_documenti_copiati,
                             Num_documenti_trasferiti = p.Num_documenti_trasferiti,
                             Stato = jobInRicerca.Contains(p.Id_policy) ? "RICERCA PROGRAMMATA" : (jobInAnalisi.Contains(p.Id_policy) ? "ANALISI PROGRAMMATA" : p.Stato)
                             //TransferPolicyType_ID = o.TransferPolicyType_ID        //tipo di policy
                         });

            var rows = query.ToList();
            while (rows.Count() % 9 != 0)
            {
                rows.Add(new ARCHIVE_View_Policy());
            }
            this.GrdPolicy.DataSource = rows.OrderByDescending(x => x.Id_policy).ToList();

            this.GrdPolicy.PageIndex = e.NewPageIndex;
            this.GrdPolicy.DataBind();

            if (e.NewPageIndex == this.SelectedPage)
                this.GrdPolicy.SelectedIndex = this.SelectedRow;
            else
                this.GrdPolicy.SelectedIndex = -1;

            this.UpPnlGridResult.Update();

        }

        protected void GrdPolicy_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            int[] cellClickable = new int[] { 1, 2, 3, 4, 5, 6, 7 };
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (!this.GrdPolicy.DataKeys[e.Row.RowIndex].Value.ToString().Equals("0"))
                {
                    foreach (int cell in cellClickable)
                    {
                        e.Row.Cells[cell].Attributes["onclick"] = this.Page.ClientScript.GetPostBackEventReference(this.GrdPolicy, "Select$" + e.Row.RowIndex.ToString());
                        e.Row.Cells[cell].ToolTip = "Cliccare per selezionare la policy";
                    }
                    //se lo stato della policy è ANALISI COMPLETATA allora abilito il bottone
                    //per l'analisi di impatto della relativa policy
                    if (this.TransferPolicyInContext.Where(x =>
                                 x.System_ID.ToString().Equals(this.GrdPolicy.DataKeys[e.Row.RowIndex].Value.ToString()))
                                            .Select(x => x.TransferPolicyState_ID).FirstOrDefault() == 5)
                    {
                        ((CustomImageButton)e.Row.FindControl("cImgBtnDettaglioPolicy")).Enabled = true;
                        ((CustomImageButton)e.Row.FindControl("cImgBtnDettaglioPolicy")).Visible = true;
                    }
                    
                }
                else
                {
                    ((CheckBox)e.Row.FindControl("ckbIncludiEscludi")).Visible = false;
                    ((CustomImageButton)e.Row.FindControl("cImgBtnDettaglioPolicy")).Visible = false;
                    ((Label)e.Row.FindControl("LblSystem_id_Policy")).Visible = false;
                    ((Label)e.Row.FindControl("LblDescription_Policy")).Visible = false;
                    ((Label)e.Row.FindControl("LblNumeroDocumenti")).Visible = false;
                    ((Label)e.Row.FindControl("LblNumeroFascicoli")).Visible = false;
                    ((Label)e.Row.FindControl("LblNEffettivi")).Visible = false;
                    ((Label)e.Row.FindControl("LblNCopiati")).Visible = false;
                    ((Label)e.Row.FindControl("LblStato")).Visible = false;
                    //porcata fatta perchè riesco a impostare l'altezza della riga in percentuale

                    e.Row.Height = Unit.Pixel(15);

                }

                if (this.idPolicySelected.Contains(int.Parse(this.GrdPolicy.DataKeys[e.Row.RowIndex].Value.ToString())))
                    ((CheckBox)e.Row.FindControl("ckbIncludiEscludi")).Checked = true;

            }
        }

        protected void GrdPolicy_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Determine the index of the selected row.
            int rowindex = GrdPolicy.SelectedIndex;
            Int32 _idPoli = 0;

            this.SelectedPage = GrdPolicy.PageIndex;
            this.SelectedRow = rowindex;

            if (rowindex >= 0)
            {
                _idPoli = int.Parse(this.GrdPolicy.DataKeys[rowindex].Value.ToString());


                this.CurrentPolicyInContext = (from x in this.TransferPolicyInContext
                                               where x.System_ID == _idPoli
                                               select x).Cast<DocsPaWR.ARCHIVE_TransferPolicy>().ToList().FirstOrDefault();

                this.OriginalPolicyInContext = (from x in this.TransferPolicyOriginalInContext
                                                where x.System_ID == _idPoli
                                                select x).Cast<DocsPaWR.ARCHIVE_TransferPolicy>().ToList().FirstOrDefault();

                this.CurrentProfileTypeInContext = UIManager.ArchiveManager.GetARCHIVE_TransferPolicy_ProfileTypeByTransferPolicy_ID(_idPoli);
                this.CurrentProfileTypeInContext = this.CurrentProfileTypeInContext != null ? this.CurrentProfileTypeInContext : new List<ARCHIVE_TransferPolicy_ProfileType>();

                this.OriginalProfileTypeInContext = UIManager.ArchiveManager.GetARCHIVE_TransferPolicy_ProfileTypeByTransferPolicy_ID(_idPoli);
                this.OriginalProfileTypeInContext = this.OriginalProfileTypeInContext != null ? this.OriginalProfileTypeInContext : new List<ARCHIVE_TransferPolicy_ProfileType>();

                Session["PolicyState"] = PolicyState.MOD.ToString();
                this.CallVisibilityByPolicyState(Session["PolicyState"]);
                this.PopulatePageObject();

            }
        }

        protected void GrdPolicy_PreRender(object sender, EventArgs e)
        {
            //header grid checkbox select all
            CheckBox c = ((CheckBox)this.GrdPolicy.HeaderRow.FindControl("cb_selectall"));
            c.Checked = this.selectedAll;

            if (this.GrdPolicy.Rows.Count > 0 && this.GrdPolicy.PageIndex == this.SelectedPage)
                this.GrdPolicy.SelectedIndex = this.SelectedRow;
            else
                this.GrdPolicy.SelectedIndex = -1;
        }

        protected void ckbIncludiEscludi_CheckedChanged(object sender, EventArgs e)
        {

            Int32 _idPoli = 0;
            CheckBox _check = (CheckBox)sender;

            //Get the row that contains this checkbox
            GridViewRow gridviewRow = (GridViewRow)_check.NamingContainer;

            //Get the rowindex
            int rowindex = gridviewRow.RowIndex;
            bool value = _check.Checked;
            _idPoli = int.Parse(this.GrdPolicy.DataKeys[rowindex].Value.ToString());
            if (value)
                this.idPolicySelected.Add(_idPoli);
            else
            {
                this.idPolicySelected.Remove(_idPoli);
                this.selectedAll = value;
            }
        }

       
        protected void addAll_Click(object sender, EventArgs e)
        {

            bool value = ((CheckBox)sender).Checked;
            this.idPolicySelected = new List<Int32>();
            foreach (GridViewRow dgItem in GrdPolicy.Rows)
            {
                CheckBox checkBox = dgItem.FindControl("ckbIncludiEscludi") as CheckBox;
                checkBox.Checked = value;
            }
            if (value)
                this.idPolicySelected.AddRange(this.TransferPolicyInContext.Select(x => x.System_ID).ToList());
            this.selectedAll = value;
            this.UpPnlGridResult.Update();

        }

        #endregion

        protected void cImgBtnDettaglioPolicy_Click(object sender, EventArgs e)
        {
            Int32 _idPoli = 0;
            CustomImageButton button = (CustomImageButton)sender;

            //Get the row that contains this checkbox
            GridViewRow gridviewRow = (GridViewRow)button.NamingContainer;

            //Get the rowindex
            int rowindex = gridviewRow.RowIndex;

            _idPoli = int.Parse(this.GrdPolicy.DataKeys[rowindex].Value.ToString());

            Session["SelectedPolicy"] = this.TransferPolicyInContext.Where(x => x.System_ID == _idPoli).FirstOrDefault();
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "TransferPolicyImpact", "ajaxModalPopupTransferPolicyImpact();", true);
        }

        protected void btnPolicyNuovo_Click(object sender, EventArgs e)
        {
            try
            {
                ////Controllo Campi
                if (string.IsNullOrEmpty(TxtDescPolicy.Text))
                {
                    string msgErrNewArchiveTransferPolicy = "msgErrNewArchiveTransferPolicy";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('" + msgErrNewArchiveTransferPolicy.Replace("'", @"\'") + "', 'warning', '');", true);
                    return;
                }
                else
                {
                    int enable = 0;  //Disabilitata
                    int TransferPolicyType;
                    //this.CurrentPolicyInContext
                    ARCHIVE_TransferPolicy policy = new ARCHIVE_TransferPolicy();

                    if (ddlDocFasc.SelectedItem.Value.Equals("1"))
                        TransferPolicyType = UIManager.ArchiveManager._dictionaryTransferPolicyType.Where(x => x.Value == "DOCUMENTI").Select(x => x.Key).FirstOrDefault();
                    else
                        TransferPolicyType = UIManager.ArchiveManager._dictionaryTransferPolicyType.Where(x => x.Value == "FASCICOLI").Select(x => x.Key).FirstOrDefault();
                    //OBBLIGATORI
                    policy.Description = TxtDescPolicy.Text.Trim();
                    policy.Transfer_ID = this.Versamento_system_id;
                    policy.Enabled = enable;
                    policy.TransferPolicyType_ID = TransferPolicyType;
                    policy.TransferPolicyState_ID = 1;

                    //Inserimento valori facoltativi 

                    policy.Tipologia_ID = !string.IsNullOrEmpty(this.ddlPolicyTipologia.SelectedItem.Value) ? int.Parse(this.ddlPolicyTipologia.SelectedItem.Value) : new int?();
                    policy.Registro_ID = !string.IsNullOrEmpty(this.ddlPolicyRegistri.SelectedItem.Value) ? int.Parse(this.ddlPolicyRegistri.SelectedItem.Value) : new int?();
                    policy.Titolario_ID = !string.IsNullOrEmpty(this.ddlPolicyTitolatio.SelectedItem.Value) ? int.Parse(this.ddlPolicyTitolatio.SelectedItem.Value) : new int?();
                    policy.ClasseTitolario = !string.IsNullOrEmpty(this.txtPolicyClassi.Text) ? this.txtPolicyClassi.Text : string.Empty;
                    //data creazione
                    if (ddl_dtaCreate.SelectedItem.Value.Equals("0"))
                    {
                        policy.AnnoCreazioneA = !string.IsNullOrEmpty(dtaCreate_TxtFrom.Text) ? int.Parse(dtaCreate_TxtFrom.Text) : new int?();
                        policy.AnnoCreazioneDa = !string.IsNullOrEmpty(dtaCreate_TxtFrom.Text) ? int.Parse(dtaCreate_TxtFrom.Text) : new int?();
                    }
                    else
                    {
                        policy.AnnoCreazioneA = !string.IsNullOrEmpty(dtaCreate_TxtTo.Text) ? int.Parse(dtaCreate_TxtTo.Text) : new int?();
                        policy.AnnoCreazioneDa = !string.IsNullOrEmpty(dtaCreate_TxtFrom.Text) ? int.Parse(dtaCreate_TxtFrom.Text) : new int?();
                    }
                    //data protocollazione/chiusura
                    if (this.isDoc)
                    {   //data protocollazione
                        if (ddl_dtaProt.SelectedItem.Value.Equals("0"))
                        {
                            policy.AnnoProtocollazioneA = !string.IsNullOrEmpty(dtaProt_TxtFrom.Text) ? int.Parse(dtaProt_TxtFrom.Text) : new int?();
                            policy.AnnoProtocollazioneDa = !string.IsNullOrEmpty(dtaProt_TxtFrom.Text) ? int.Parse(dtaProt_TxtFrom.Text) : new int?();
                        }
                        else
                        {
                            policy.AnnoProtocollazioneA = !string.IsNullOrEmpty(dtaProt_TxtTo.Text) ? int.Parse(dtaProt_TxtTo.Text) : new int?();
                            policy.AnnoProtocollazioneDa = !string.IsNullOrEmpty(dtaProt_TxtFrom.Text) ? int.Parse(dtaProt_TxtFrom.Text) : new int?();
                        }
                    }
                    else
                    {   //data chiusura
                        if (ddl_dtaProt.SelectedItem.Value.Equals("0"))
                        {
                            policy.AnnoChiusuraA = !string.IsNullOrEmpty(dtaProt_TxtFrom.Text) ? int.Parse(dtaProt_TxtFrom.Text) : new int?();
                            policy.AnnoChiusuraDa = !string.IsNullOrEmpty(dtaProt_TxtFrom.Text) ? int.Parse(dtaProt_TxtFrom.Text) : new int?();
                        }
                        else
                        {
                            policy.AnnoChiusuraA = !string.IsNullOrEmpty(dtaProt_TxtTo.Text) ? int.Parse(dtaProt_TxtTo.Text) : new int?();
                            policy.AnnoChiusuraDa = !string.IsNullOrEmpty(dtaProt_TxtFrom.Text) ? int.Parse(dtaProt_TxtFrom.Text) : new int?();
                        }

                    }
                    policy.IncludiSottoalberoUO = chkIncUO.Checked ? 1 : 0;
                    policy.IncludiSottoalberoClasseTit = ChkIncludeClass.Checked ? 1 : 0;
                    policy.UO_ID = !string.IsNullOrEmpty(ddlPolicyUo.SelectedItem.Value) ? int.Parse(ddlPolicyUo.SelectedItem.Value) : new int?();
                    int system_id;
                    //se è una policy documenti 
                    if (this.isDoc)
                    {
                        system_id = UIManager.ArchiveManager.InsertARCHIVE_TransferPolicy(policy, this.chkArrivo.Checked, this.chkPartenza.Checked,
                            this.chkInt.Checked, this.chkNonProt.Checked, this.chkStampaRegProt.Checked, this.chkStampaRep.Checked);
                    }
                    else
                        system_id = UIManager.ArchiveManager.InsertARCHIVE_TransferPolicy(policy);
                    //insert relazioni policy<-->profileType
                    //if (this.isDoc)
                    //{
                    //    if (this.chkArrivo.Checked)
                    //        UIManager.ArchiveManager.InsertARCHIVE_TransferPolicy_ProfileType(system_id,
                    //            UIManager.ArchiveManager._dictionaryProfileType.Where(x => x.Value == "ARRIVO").Select(x => x.Key).FirstOrDefault());
                    //    if (this.chkPartenza.Checked)
                    //        UIManager.ArchiveManager.InsertARCHIVE_TransferPolicy_ProfileType(system_id,
                    //            UIManager.ArchiveManager._dictionaryProfileType.Where(x => x.Value == "PARTENZA").Select(x => x.Key).FirstOrDefault());
                    //    if (this.chkInt.Checked)
                    //        UIManager.ArchiveManager.InsertARCHIVE_TransferPolicy_ProfileType(system_id,
                    //            UIManager.ArchiveManager._dictionaryProfileType.Where(x => x.Value == "INTERNO").Select(x => x.Key).FirstOrDefault());
                    //    if (this.chkNonProt.Checked)
                    //        UIManager.ArchiveManager.InsertARCHIVE_TransferPolicy_ProfileType(system_id,
                    //            UIManager.ArchiveManager._dictionaryProfileType.Where(x => x.Value == "NON PROTOCOLLATO").Select(x => x.Key).FirstOrDefault());
                    //    if (this.chkStampaRegProt.Checked)
                    //        UIManager.ArchiveManager.InsertARCHIVE_TransferPolicy_ProfileType(system_id,
                    //            UIManager.ArchiveManager._dictionaryProfileType.Where(x => x.Value == "STAMPA REGISTRO PROTOCOLLATO").Select(x => x.Key).FirstOrDefault());
                    //    if (this.chkStampaRep.Checked)
                    //        UIManager.ArchiveManager.InsertARCHIVE_TransferPolicy_ProfileType(system_id,
                    //            UIManager.ArchiveManager._dictionaryProfileType.Where(x => x.Value == "STAMPA REPERTORIO").Select(x => x.Key).FirstOrDefault());

                    //}
                    string msgResultCreatePolicy = string.Empty;
                    string messageType = string.Empty;
                    if (system_id > 0)
                    {
                        msgResultCreatePolicy = "msgResultCreatePolicyTrue";
                        messageType = "check";
                        this.GetDataByPageState();
                        this.CurrentPolicyInContext = this.TransferPolicyInContext.Where(p => p.System_ID == system_id).FirstOrDefault();
                        this.OriginalPolicyInContext = this.TransferPolicyOriginalInContext.Where(p => p.System_ID == system_id).FirstOrDefault();
                        this.PopulatePageObject();
                        Session["PolicyState"] = PolicyState.MOD.ToString();
                        this.CallVisibilityByPolicyState(Session["PolicyState"]);
                        this.UpPnlGridResult.Update();
                        this.UpPnlPolicyMid.Update();
                    }
                    else
                    {
                        msgResultCreatePolicy = "msgResultCreatePolicyFalse";
                        messageType = "warning";
                    }

                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('" + msgResultCreatePolicy.Replace("'", @"\'") + "', '" + messageType + "', '');", true);

                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }

        }

        protected void btnEseguiRicerca_Click(object sender, EventArgs e)
        {
            try
            {
                List<Int32> listIdPolicy = new List<Int32>();

                if (this.TransferPolicyViewInContext != null && this.TransferPolicyInContext != null && this.idPolicySelected.Count > 0)
                {

                    var listPolicyEffettive = (from id in this.idPolicySelected
                                               join p in this.TransferPolicyInContext
                                                    on id equals p.System_ID
                                               select p);

                    var listPolicyIdEffettive = (from p in listPolicyEffettive.ToList()
                                                 where p.TransferPolicyState_ID == UIManager.ArchiveManager._dictionaryTransferPolicyState.Where
                                                                               (x => x.Value == "RICERCA NON AVVIATA").Select(x => x.Key).FirstOrDefault()
                                                 select p.System_ID);

                    List<Int32> jobInRicerca = new List<int>();
                    List<Int32> listPolicyIdEffettiveNonSchedulate;

                    if (this.TransferPolicyJOBInContext.Count > 0 && this.TransferPolicyJOBInContext != null)
                    {
                        jobInRicerca = this.TransferPolicyJOBInContext.Where(x => x.JobType_ID == 1 && x.Executed == 0).Select(x => x.TransferPolicy_ID).Distinct().ToList();

                        listPolicyIdEffettiveNonSchedulate = (from p in listPolicyIdEffettive.ToList()
                                                              where !jobInRicerca.Contains(p)
                                                              select p).ToList();
                    }
                    else
                    {
                        listPolicyIdEffettiveNonSchedulate = listPolicyIdEffettive.ToList();
                    }
                    if (listPolicyIdEffettiveNonSchedulate.Count != 0)
                    {
                        UIManager.ArchiveManager.StartAsyncSearchForTransferPolicyListByPolicyId(
                            UIManager.ArchiveManager.GetSQLinStringFromListIdPolicy(listPolicyIdEffettive.ToList()));
                        //UIManager.ArchiveManager.StartSearchForTransferPolicyListByPolicyId(GetSQLinStringFromListIdPolicy(listPolicyIdEffettive.ToList()));
                        this.selectedAll = false;
                        this.GetDataByPageState();
                    }
                    else
                    {
                        string msgErrCheckArchiveTransferPolicy = "msgErrCheckArchiveTransferPolicy";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('" + msgErrCheckArchiveTransferPolicy.Replace("'", @"\'") + "', 'warning', '');", true);
                        return;
                    }
                }
                else
                {
                    string msgErrCheckArchiveTransferPolicy = "msgErrCheckArchiveTransferPolicy";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('" + msgErrCheckArchiveTransferPolicy.Replace("'", @"\'") + "', 'warning', '');", true);
                    return;
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void btnPolicyRimuoviFiltro_Click(object sender, EventArgs e)
        {
            Session["PolicyState"] = PolicyState.NEW.ToString();
            this.CurrentPolicyInContext = null;
            this.SelectedRow = -1;
            this.SelectedPage = 1;
            this.ClearPageObject();
            this.UpPnlPolicyMid.Update();
            this.UpPnlGridResult.Update();
            this.CallVisibilityByPolicyState(Session["PolicyState"]);

        }

        protected void btnPolicyAnalizza_Click(object sender, EventArgs e)
        {
            try
            {

                if (this.TransferPolicyViewInContext != null && this.TransferPolicyInContext != null && this.idPolicySelected.Count > 0)
                {

                    var listPolicyEffettive = (from id in this.idPolicySelected
                                               join p in this.TransferPolicyInContext
                                                    on id equals p.System_ID
                                               select p);

                    var listPolicyIdEffettive = (from p in listPolicyEffettive.ToList()
                                                 where p.TransferPolicyState_ID == UIManager.ArchiveManager._dictionaryTransferPolicyState.Where
                                                                               (x => x.Value == "RICERCA COMPLETATA").Select(x => x.Key).FirstOrDefault()
                                                 select p.System_ID);

                    List<Int32> jobInAnalisi = new List<int>();
                    List<Int32> listPolicyIdEffettiveNonSchedulate;

                    if (this.TransferPolicyJOBInContext.Count > 0 && this.TransferPolicyJOBInContext != null)
                    {
                        jobInAnalisi = this.TransferPolicyJOBInContext.Where(x => x.JobType_ID == 2 && x.Executed == 0).Select(x => x.TransferPolicy_ID).Distinct().ToList();

                        listPolicyIdEffettiveNonSchedulate = (from p in listPolicyIdEffettive.ToList()
                                                              where !jobInAnalisi.Contains(p)
                                                              select p).ToList();
                    }
                    else
                    {
                        listPolicyIdEffettiveNonSchedulate = listPolicyIdEffettive.ToList();
                    }

                    if (listPolicyIdEffettiveNonSchedulate.Count != 0)
                    {
                        UIManager.ArchiveManager.StartAsyncAnalysisForTransferPolicyListByPolicyId(
                            UIManager.ArchiveManager.GetSQLinStringFromListIdPolicy(listPolicyIdEffettive.ToList()));
                        //UIManager.ArchiveManager.StartAnalysisForTransferPolicyListByPolicyId(GetSQLinStringFromListIdPolicy(listPolicyIdEffettive.ToList()));
                        this.selectedAll = false;
                        this.GetDataByPageState();

                    }
                    else
                    {
                        string msgErrCheckArchiveTransferPolicy = "msgErrCheckArchiveTransferPolicy";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('" + msgErrCheckArchiveTransferPolicy.Replace("'", @"\'") + "', 'warning', '');", true);
                        //ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", "\\'") + "', 'warning');} else {parent.ajaxDialogModal('" + msg.Replace("'", "\\'") + "', 'warning');}", true);

                        return;
                    }

                }
                else
                {
                    string msgErrCheckArchiveTransferPolicy = "msgErrCheckArchiveTransferPolicy";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('" + msgErrCheckArchiveTransferPolicy.Replace("'", @"\'") + "', 'warning', '');", true);
                    //ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", "\\'") + "', 'warning');} else {parent.ajaxDialogModal('" + msg.Replace("'", "\\'") + "', 'warning');}", true);

                    return;
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void btnPolicyModifica_Click(object sender, EventArgs e)
        {
            try
            {
                if (CurrentPolicyInContext != null && Session["PolicyState"].ToString() == PolicyState.MOD.ToString())
                {
                    string msgConfirm = "msgbtnPolicyAggiorna_Click";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxConfirmModal", "ajaxConfirmModal('" + msgConfirm.Replace("'", @"\'") + "', 'HiddeConfirmUpdateTransferPolicy', 'Conferma');", true);
                    this.upPnlHiddeConfirmUpdateTransferPolicy.Update();
                }
                else
                {
                    string msgErrNewArchiveTransferPolicy = "msgErrCurrentPolicy";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('" + msgErrNewArchiveTransferPolicy.Replace("'", @"\'") + "', 'warning', '');", true);
                    return;
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void btnPolicyElimina_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.idPolicySelected.Count > 0)
                {
                    string msgConfirm = "msgbtnPolicyElimina_Click";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxConfirmModal", "ajaxConfirmModal('" + msgConfirm.Replace("'", @"\'") + "', 'HiddeConfirmDeleteTransferPolicy', 'Conferma');", true);
                    this.upPnlHiddeConfirmDeleteTransferPolicy.Update();
                }
                else
                {
                    string msgErrNewArchiveTransferPolicy = "msgErrCurrentPolicy";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('" + msgErrNewArchiveTransferPolicy.Replace("'", @"\'") + "', 'warning', '');", true);
                    return;
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void btnPolicyChiudi_Click(object sender, EventArgs e)
        {
            try
            {
                this.ClearSessionAndContextProperties();
                this.closePage();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private void closePage()
        {
            ScriptManager.RegisterClientScriptBlock(this.upPnlButtons, this.upPnlButtons.GetType(), "closeAJM", "closeAjaxModal('Policy','');", true);
        }

    }
}
