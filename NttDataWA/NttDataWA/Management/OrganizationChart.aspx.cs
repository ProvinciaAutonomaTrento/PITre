using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using NttDataWA.UIManager;
using NttDataWA.DocsPaWR;
using System.Collections;
using NttDataWA.UserControls;
using NttDatalLibrary;
using NttDataWA.Utils;


namespace NttDataWA.Management
{

    public class UOSort : IComparer
    {
        public UOSort() : base() { }

        int IComparer.Compare(object x, object y)
        {
            DocsPaWR.OrgUO UoX = (DocsPaWR.OrgUO)x;
            DocsPaWR.OrgUO UoY = (DocsPaWR.OrgUO)y;
            return UoX.Descrizione.CompareTo(UoY.Descrizione);
        }
    }

    public partial class OrganizationChart : System.Web.UI.Page
    {

        #region Fields

        private DocsPaWR.OrgRegistro[] _RFList;
        private DocsPaWR.OrgRuolo[] _ruoliConRFselezionato;
        private bool _ricercaRFimpostata = false;

        public static string componentType = Constans.TYPE_SMARTCLIENT;

        #endregion

        #region Properties

        private bool RFEnabled
        {
            get
            {
                bool value = false;
                if (HttpContext.Current.Session["RFEnabled"] != null && (bool)HttpContext.Current.Session["RFEnabled"])
                    value = true;
                return value;
            }
            set
            {
                HttpContext.Current.Session["RFEnabled"] = value;
            }
        }

        private string idAmm
        {
            get
            {
                return HttpContext.Current.Session["idAmm"] as string;
            }
            set
            {
                HttpContext.Current.Session["idAmm"] = value;
            }
        }

        private bool isOrdinamentoAbilitato
        {
            get
            {
                bool value = false;
                if (HttpContext.Current.Session["isOrdinamentoAbilitato"] != null)
                    value = (bool)HttpContext.Current.Session["isOrdinamentoAbilitato"];
                return value;
            }
            set
            {
                HttpContext.Current.Session["isOrdinamentoAbilitato"] = value;
            }
        }

        public static string httpFullPath
        {
            get
            {
                return utils.getHttpFullPath();
            }
        }

        private string Codice
        {
            get
            {
                return HttpContext.Current.Session["OrganizationChart_Code"] as string;
            }
            set
            {
                HttpContext.Current.Session["OrganizationChart_Code"] = value;
            }
        }

        private string Descrizione
        {
            get
            {
                return HttpContext.Current.Session["OrganizationChart_Description"] as string;
            }
            set
            {
                HttpContext.Current.Session["OrganizationChart_Description"] = value;
            }
        }

        private string IdAmm
        {
            get
            {
                return HttpContext.Current.Session["OrganizationChart_IdAmm"] as string;
            }
            set
            {
                HttpContext.Current.Session["OrganizationChart_IdAmm"] = value;
            }
        }

        private string Tipo
        {
            get
            {
                return HttpContext.Current.Session["OrganizationChart_Type"] as string;
            }
            set
            {
                HttpContext.Current.Session["OrganizationChart_Type"] = value;
            }
        }

        private bool SearchHistoricized
        {
            get
            {
                bool retValue = false;
                if (HttpContext.Current.Session["OrganizationChart_SearchHistoricized"] != null)
                    retValue = (bool)HttpContext.Current.Session["OrganizationChart_SearchHistoricized"];
                return retValue;
            }
            set
            {
                HttpContext.Current.Session["OrganizationChart_SearchHistoricized"] = value;
            }
        }

        private DataSet dsRisultato
        {
            get
            {
                return HttpContext.Current.Session["OrganizationChart_dsRisultato"] as DataSet;
            }
            set
            {
                HttpContext.Current.Session["OrganizationChart_dsRisultato"] = value;
            }
        }

        #endregion

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                componentType = UserManager.getComponentType(Request.UserAgent);
                this.InitApplet();

                if (!IsPostBack)
                {
                    this.InitializePage();
                    this.VisualizzaNodoRoot();
                }
                else
                {
                    this.ReadRetValueFromPopup();
                }
                
                this.RefreshScript();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void ddlSearchIn_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.plcCodeName.Visible = true;
            this.plcRF.Visible = false;

            switch (this.ddlSearchIn.SelectedValue) {
                case "U":
                    this.lblName.Text = this.GetLabel("OrganizationChartLblNameUO");
                    break;
                case "R":
                    this.lblName.Text = this.GetLabel("OrganizationChartLblNameRole");
                    break;
                case "PN":
                    this.lblName.Text = this.GetLabel("OrganizationChartLblNameUser");
                    break;
                case "PC":
                    this.lblName.Text = this.GetLabel("OrganizationChartLblSurnameUser");
                    break;
                case "RF":
                    this.fillComboBoxRF();
                    this.plcCodeName.Visible = false;
                    this.plcRF.Visible = true;
                    this.treeViewUO.Nodes.Clear();
                    this.UpPnlTree.Update();
                    break;
            }

            this.UpPnlFilters.Update();
        }

        protected void treeViewUO_TreeNodeExpanded(object sender, TreeNodeEventArgs e)
        {
            DocsPaWR.OrgUO currentUO;
            DocsPaWR.OrgRuolo currentRole;

            myTreeNode2 TreeNodo = (myTreeNode2)e.Node;
            TreeNodo.Expanded = true;

            if (TreeNodo.ChildNodes.Count > 0)
                TreeNodo.ChildNodes.Clear();

            switch (TreeNodo.TipoNodo)
            {
                case "U":

                    currentUO = new DocsPaWR.OrgUO();
                    currentUO.IDCorrGlobale = TreeNodo.ID;
                    currentUO.IDAmministrazione = this.idAmm;
                    currentUO.Ruoli = TreeNodo.RuoliUO;
                    currentUO.SottoUo = TreeNodo.SottoUO;
                    currentUO.Livello = TreeNodo.Livello;

                    if (this.ddlView.SelectedIndex >= 1 && Convert.ToInt32(currentUO.Ruoli) > 0)
                        this.RuoliUO(currentUO, TreeNodo);

                    if (Convert.ToInt32(currentUO.SottoUo) > 0)
                        this.SottoUO(currentUO, TreeNodo);

                    break;

                // Gabriele Melini
                // bug gestione ruoli organigramma
                case "R":

                    currentRole = new DocsPaWR.OrgRuolo();

                    OrganigrammaManager manager = new OrganigrammaManager();
                    currentRole = manager.GetRole(TreeNodo.ID);
                    manager.ListaUtenti(currentRole.IDGruppo);

                    if (manager.getListaUtenti() != null && manager.getListaUtenti().Count > 0)
                    {
                        DocsPaWR.OrgUtente[] utenti = (DocsPaWR.OrgUtente[])manager.getListaUtenti().ToArray(typeof(DocsPaWR.OrgUtente));
                        currentRole.Utenti = utenti;

                        if (this.ddlView.SelectedValue == "3" && currentRole.Utenti.Length > 0)
                            this.UtentiRuolo(TreeNodo, currentRole);
                    }

                    break;
            }
        }

        protected void ddlView_SelectedIndexChanged(object sender, EventArgs e)
        {
            // gestione RF
            if (this.verificaStatoRicercaRF())
            {
                switch (this.ddlView.SelectedIndex)
                {
                    case 0:
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "dialog", "ajaxDialogModal('ErrorOrganizationChartSearchRF', 'error', '');", true);

                        this.ddlView.SelectedIndex = 1;
                        return;

                    case 1:
                        this.performFindRF(false);
                        return;

                    case 2:
                        this.performFindRF(true);
                        return;
                }
            }

            if (this.treeViewUO.Nodes.Count > 0)
                if (this.hd_lastReturnValueModal.Value != null && this.hd_lastReturnValueModal.Value != string.Empty && this.hd_lastReturnValueModal.Value != "undefined")
                    this.VisualizzaNodoRicercato(this.hd_lastReturnValueModal.Value);

            this.UpPnlTree.Update();
        }

		/// <summary>
		/// Imposta come Root la UO selezionata
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
        protected void BtnRoot_Click(object sender, EventArgs e)
        {
			this.ddlSearchIn.SelectedIndex = 0;
			this.txtCode.Text = "";
			this.txtName.Text = "";
			this.VisualizzaUOSelezionata();

            this.UpPnlButtons.Update();
            this.UpPnlFilters.Update();
            this.UpPnlTree.Update();
        }

        /// <summary>
        /// Visualizza l'organigramma partendo dalla UO principale (livello 0)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void BtnStart_Click(object sender, EventArgs e)
        {
            this.restoreSearchDefault();
            this.ddlView.SelectedIndex = 0;
            this.txtCode.Text = "";
            this.txtName.Text = "";
            this.VisualizzaNodoRoot();

            this.UpPnlButtons.Update();
            this.UpPnlFilters.Update();
            this.UpPnlTree.Update();
        }

        protected void BtnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                XmlDocument xmlDoc = this.exportToXML();

                if (xmlDoc != null && (xmlDoc.InnerXml != string.Empty || xmlDoc.InnerXml != ""))
                {
                    DocsPaWR.FileDocumento filePdf = new DocsPaWR.FileDocumento();
                    OrganigrammaManager manager = new OrganigrammaManager();

                    manager.StampaOrganigramma(xmlDoc);

                    filePdf = manager.getFilePDF();

                    if (filePdf != null && filePdf.content.Length > 0)
                    {
                        manager.setSessionFilePDF(filePdf);
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "stampa", "stampa('" + utils.FormatJs(componentType) + "');", true);
                    }
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "dialog", "ajaxDialogModal('ErrorCustom', 'error', '', '" + utils.FormatJs(ex.Message) + "');", true);
            }
        }

        protected void BtnSearch_Click(object sender, EventArgs e)
        {
            if (this.ddlSearchIn.SelectedValue != "RF")
            {
                this.IdAmm = this.idAmm;
                this.Tipo = this.ddlSearchIn.SelectedValue;
                this.Codice = this.txtCode.Text;
                this.Descrizione = this.txtName.Text;
                HttpContext.Current.Session["OrganizationChart_AlreadyClosed"] = null;

                this.PerformSearch();
            }
            else
            {
                try
                {
                    this.performFindRF(false);
                }
                catch (Exception ex)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "dialog", "ajaxDialogModal('ErrorOrganizationChartSearchRF', 'error', '');", true);
                }
            }
        }

        #endregion

        #region Methods

      
        private void InitApplet()
        {
            if (componentType == Constans.TYPE_APPLET)
                this.plcApplet.Visible = true;
            else
            {
                Control ShellWrapper = Page.LoadControl("../ActivexWrappers/ShellWrapper.ascx");
                this.plcActiveX.Controls.Add(ShellWrapper);

                Control AdoStreamWrapper = Page.LoadControl("../ActivexWrappers/AdoStreamWrapper.ascx");
                this.plcActiveX.Controls.Add(AdoStreamWrapper);

                Control FsoWrapper = Page.LoadControl("../ActivexWrappers/FsoWrapper.ascx");
                this.plcActiveX.Controls.Add(FsoWrapper);

                this.plcActiveX.Visible = true;
            }
        }

        protected void InitializePage()
        {
            this.InitializeLanguage();
            this.LoadKeys();
            this.idAmm = UserManager.GetInfoUser().idAmministrazione;
            this.LoadDdlView();
            this.LoadDdlSearchIn();
            this.verificaAbilitazioneRF();
        }

        private void LoadKeys()
        {
            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.ORDINAMENTO_ORGANIGRAMMA.ToString()]) && System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.ORDINAMENTO_ORGANIGRAMMA.ToString()].Equals("1"))
                this.isOrdinamentoAbilitato = true;
        }

        protected void LoadDdlView()
        {
            this.ddlView.Items.Add(new ListItem(this.GetLabel("OrganizationChartItemViewUO"), "1"));
            this.ddlView.Items.Add(new ListItem(this.GetLabel("OrganizationChartItemViewUORoles"), "2"));
            this.ddlView.Items.Add(new ListItem(this.GetLabel("OrganizationChartItemViewUORolesUsers"), "3"));
        }

        protected void LoadDdlSearchIn()
        {
            this.ddlSearchIn.Items.Add(new ListItem(this.GetLabel("OrganizationChartItemSearchInUO"), "U"));
            this.ddlSearchIn.Items.Add(new ListItem(this.GetLabel("OrganizationChartItemSearchInRole"), "R"));
            this.ddlSearchIn.Items.Add(new ListItem(this.GetLabel("OrganizationChartItemSearchInName"), "PN"));
            this.ddlSearchIn.Items.Add(new ListItem(this.GetLabel("OrganizationChartItemSearchInSurname"), "PC"));
        }

        protected void ReadRetValueFromPopup()
        {
            if (!string.IsNullOrEmpty(this.OrganizationChartSearchResult.ReturnValue))
            {
                this.hd_returnValueModal.Value = this.OrganizationChartSearchResult.ReturnValue;
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('OrganizationChartSearchResult','')", true);
                this.VisualizzaNodoRicercato(this.hd_returnValueModal.Value);
                this.UpPnlTree.Update();
                this.UpContainer.Update();
                return;
            }
        }

        protected void RefreshScript()
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshSelect", "refreshSelect();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "functionKeyPress", "<script>$(function() {$('#contentStandard1Column input, #contentStandard1Column textarea').keypress(function(e) {if(e.which == 13) {e.preventDefault();$('.defaultAction').click();}});});</script>", false);
        }

        protected void InitializeLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.OrganizationChartSearchResult.Title = Utils.Languages.GetLabelFromCode("ManageOrganizationChartPageTitle", language);
            this.pageTitle.Text = Utils.Languages.GetLabelFromCode("ManageOrganizationChartTitle", language);
            this.BtnSearch.Text = Utils.Languages.GetLabelFromCode("OrganizationChartBtnSearch", language);
            this.BtnStart.Text = Utils.Languages.GetLabelFromCode("OrganizationChartBtnStart", language);
            this.BtnRoot.Text = Utils.Languages.GetLabelFromCode("OrganizationChartBtnRoot", language);
            this.BtnPrint.Text = Utils.Languages.GetLabelFromCode("OrganizationChartBtnPrint", language);
            this.lblView.Text = Utils.Languages.GetLabelFromCode("OrganizationChartLblView", language);
            this.lblSearchIn.Text = Utils.Languages.GetLabelFromCode("OrganizationChartLbllblSearchIn", language);
            this.lblCode.Text = Utils.Languages.GetLabelFromCode("OrganizationChartLblCode", language);
            this.lblName.Text = Utils.Languages.GetLabelFromCode("OrganizationChartLblNameUO", language);
            this.lblRF.Text = Utils.Languages.GetLabelFromCode("OrganizationChartLblRF", language);
        }

        protected string GetLabel(string id)
        {
            string language = UIManager.UserManager.GetUserLanguage();
            return Utils.Languages.GetLabelFromCode(id, language);
        }

        /// <summary>
        /// verifica l'abilitazione agli RF e imposta la voce RF nella combo di tipo ricerca
        /// </summary>   
        private void verificaAbilitazioneRF()
        {
            try
            {
                if (this.isEnabledRF())
                {
                    this.getRF();

                    if (this._RFList.Length > 0)
                        this.addRFRicercaTipo();
                }
            }
            catch
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "dialog", "ajaxDialogModal('ErrorOrganizationChartRF', 'error', '');", true);
            }
        }

        private bool isEnabledRF()
        {
            this.RFEnabled = AdministrationManager.IsEnableRF(UserManager.GetInfoUser().idAmministrazione);
            return this.RFEnabled;
        }

        /// <summary>
        /// Reperisce la lista di tutti gli RF disponibile e abilitati
        /// </summary>
        private void getRF()
        {
            this._RFList = RegistryManager.AmmGetRegistri();
        }

        private void addRFRicercaTipo()
        {
            this.ddlSearchIn.Items.Add(new ListItem(this.GetLabel("OrganizationChartItemSearchInRF"), "RF"));
        }

        private void fillComboBoxRF()
        {
            this.getRF();

            if (this._RFList.Length > 0)
            {
                ListItem item;
                this.ddlRF.Items.Clear();

                foreach (DocsPaWR.OrgRegistro currentRF in this._RFList)
                {
                    item = new ListItem();
                    item.Text = currentRF.Descrizione;
                    item.Value = currentRF.IDRegistro;

                    this.ddlRF.Items.Add(item);
                }
            }
        }

        /// <summary>
        /// Visualizza come nodo Root la UO di livello 0
        /// </summary>
        private void VisualizzaNodoRoot()
        {
            string idCorrGlob = string.Empty;
            OrganigrammaManager manager = new OrganigrammaManager();
            manager.ListaUOLivelloZero(this.idAmm);
            ArrayList lista = manager.getListaUO();

            if (lista != null && lista.Count > 0)
            {
                foreach (DocsPaWR.OrgUO uo in lista)
                    idCorrGlob = uo.IDCorrGlobale;

                this.VisualizzaNodoRicercato(idCorrGlob + "_0_U");
            }
        }

        /// <summary>
        /// Visualizza nella Treeview il dato ricercato
        /// </summary>
        /// <param name="returnValue"></param>
        private void VisualizzaNodoRicercato(string returnValue)
        {
            try
            {
                myTreeNode2 treenode;

                this.hd_returnValueModal.Value = "";
                this.hd_lastReturnValueModal.Value = returnValue;

                /*
                    possibili valori di ritorno:
					
                        idCorrGlobUO_idParentUO_U					=	ricerca traUO
                        idCorrGlobRuolo_idCorrGlobUO_R				=	ricerca tra ruoli
                        idCorrGlobPersona_idCorrGlobRuolo_<PN>/<PC>	=	ricerca tra utenti
                */

                string[] appo = returnValue.Split('_');
                string idCorrGlobale = appo[0];
                string idParent = appo[1];
                string tipo = appo[2];

                switch (tipo)
                {
                    case "R":
                        idCorrGlobale = idParent;
                        break;

                    case "PN":
                    case "PC":
                        idCorrGlobale = this.GetUOPadre(idParent, tipo);
                        break;
                }

                DocsPaWR.OrgUO currentUO = this.GetDatiUOCorrente(idCorrGlobale);
                if (currentUO != null)
                {
                    // diventa la ROOT della treeview
                    treenode = new myTreeNode2();
                    treenode = this.SetRootTreeview(currentUO);

                    this.SetRootTreeviewExpanded();
                }
            }
            catch
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "dialog", "ajaxDialogModal('ErrorOrganizationChartSystemError', 'error', '');", true);
            }
        }

        /// <summary>
        /// Ritorna la UO padre
        /// </summary>
        /// <param name="idCorrGlobRuolo"></param>
        /// <param name="tipo"></param>
        /// <returns></returns>
        private string GetUOPadre(string idCorrGlobRuolo, string tipo)
        {
            string idCorrGlob = string.Empty;

            OrganigrammaManager manager = new OrganigrammaManager();
            manager.ListaIDParentRicerca(idCorrGlobRuolo, tipo);
            if (manager.getListaIDParentRicerca() != null && manager.getListaIDParentRicerca().Length > 0)
            {
                object[] lista = manager.getListaIDParentRicerca();
                idCorrGlob = lista[1].ToString();
            }
            return idCorrGlob;
        }

        /// <summary>
        /// Reperimento dati della UO passata come parametro
        /// </summary>
        /// <param name="idCorrGlobale"></param>
        /// <returns></returns>
        private DocsPaWR.OrgUO GetDatiUOCorrente(string idCorrGlobale)
        {
            DocsPaWR.OrgUO currentUO;

            OrganigrammaManager manager = new OrganigrammaManager();
            manager.DatiUOCorrente(idCorrGlobale);

            currentUO = manager.getDatiUO();

            return currentUO;
        }

        /// <summary>
        /// Imposta la Root nella Treeview
        /// </summary>
        /// <param name="currentUO"></param>
        /// <returns></returns>
        private myTreeNode2 SetRootTreeview(DocsPaWR.OrgUO currentUO)
        {
            this.treeViewUO.Nodes.Clear();

            myTreeNode2 treenode = new myTreeNode2();

            treenode.ID = currentUO.IDCorrGlobale;
            treenode.Text = currentUO.Codice + " - " + currentUO.Descrizione;
            treenode.TipoNodo = "U";
            treenode.RuoliUO = currentUO.Ruoli;
            treenode.SottoUO = currentUO.SottoUo;
            treenode.Livello = currentUO.Livello;
            treenode.ImageUrl = this.getElementType(treenode.TipoNodo);

            treeViewUO.Nodes.Add(treenode);

            return treenode;
        }

        private string getElementType(string etype)
        {
            string imgUrl = "../Images/Icons/";

            switch (etype.ToUpper())
            {
                case "U":
                    imgUrl += "uo_icon.png";
                    break;
                case "P":
                    imgUrl += "user_icon.png";
                    break;
                case "R":
                    imgUrl += "role2_icon.png";
                    break;
                case "F":
                    imgUrl += "rf_icon.png";
                    break;
                case "L":
                    imgUrl += "users_list_icon.png";
                    break;
                default:
                    imgUrl += "unknow_icon.png";
                    break;
            }

            return imgUrl;
        }

        /// <summary>
        /// Espande i figli del nodo Root
        /// </summary>
        private void SetRootTreeviewExpanded()
        {
            TreeNodeEventArgs e = new TreeNodeEventArgs(this.treeViewUO.Nodes[0]);
            this.treeViewUO_TreeNodeExpanded(null, e);
        }

        /// <summary>
        /// Visualizza i ruoli di una UO
        /// </summary>
        /// <param name="currentUO"></param>
        /// <param name="indice"></param>
        private void RuoliUO(DocsPaWR.OrgUO currentUO, myTreeNode2 TreeNodo)
        {
            myTreeNode2 nodoRuoli;

            OrganigrammaManager manager = new OrganigrammaManager();
            manager.ListaRuoliUO(currentUO.IDCorrGlobale);
            if (manager.getListaRuoliUO() != null && manager.getListaRuoliUO().Count > 0)
            {
                TreeNodo.Expanded = true;
                if (UIManager.UserManager.IsAuthorizedFunctions("DO_VIS_LIV_ORG"))
                {
                    OrgTipoRuolo[] lista = manager.getListaTipiRuolo();
                    foreach (DocsPaWR.OrgRuolo ruolo in manager.getListaRuoliUO())
                    {
                        // filtro per nuova gestione RF
                        if (this.visualizzaRuolo_filtroRF(ruolo.IDCorrGlobale))
                        {
                            //Modifica per visualizzare il codice del tipo ruolo
                            manager.ListaTipiRuolo(ruolo.IDAmministrazione);
                            OrgTipoRuolo[] elencoTipiRuolo = (OrgTipoRuolo[])manager.getListaTipiRuolo();
                            OrgTipoRuolo tipoRuolo = elencoTipiRuolo.Where(oggetto => oggetto.IDTipoRuolo.ToUpper().Equals(ruolo.IDTipoRuolo.ToUpper())).FirstOrDefault();


                            nodoRuoli = new myTreeNode2();
                            nodoRuoli.ID = ruolo.IDCorrGlobale;
                            nodoRuoli.Text = ruolo.CodiceRubrica + " - [" + tipoRuolo.Livello + "] - " + ruolo.Descrizione;
                            nodoRuoli.TipoNodo = "R";
                            nodoRuoli.ImageUrl = this.getElementType(nodoRuoli.TipoNodo);
                            nodoRuoli.Expanded = true;

                            TreeNodo.ChildNodes.Add(nodoRuoli);

                            if (this.ddlView.SelectedValue == "3" && ruolo.Utenti.Length > 0)
                                this.UtentiRuolo(nodoRuoli, ruolo);
                        }
                    }
                }
                else
                {
                    foreach (DocsPaWR.OrgRuolo ruolo in manager.getListaRuoliUO())
                    {
                        // filtro per nuova gestione RF
                        if (this.visualizzaRuolo_filtroRF(ruolo.IDCorrGlobale))
                        {

                            nodoRuoli = new myTreeNode2();
                            nodoRuoli.ID = ruolo.IDCorrGlobale;
                            nodoRuoli.Text = ruolo.CodiceRubrica + " - " + ruolo.Descrizione;
                            nodoRuoli.TipoNodo = "R";
                            nodoRuoli.ImageUrl = this.getElementType(nodoRuoli.TipoNodo);
                            nodoRuoli.Expanded = true;

                            TreeNodo.ChildNodes.Add(nodoRuoli);

                            if (this.ddlView.SelectedValue == "3" && ruolo.Utenti.Length > 0)
                                this.UtentiRuolo(nodoRuoli, ruolo);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Imposta l'aggiunta del ruolo nella UO. Regole: 1) sempre, quando non è ricerca per RF; 2) quando si ricerca per un RF ed il ruolo appartiene allo stesso RF 
        /// </summary>
        /// <param name="idRuolo">ID della tabella DPA_CORR_GLOBALI</param>
        /// <returns>True o False</returns>
        private bool visualizzaRuolo_filtroRF(string idRuolo)
        {
            bool retValue = true;
            if (this.RFEnabled && this.ddlSearchIn.SelectedValue=="RF")
                retValue = this.esitoRicercaRuoloConRF(idRuolo);

            return retValue;
        }

        /// <summary>
        /// Visualizza gli utenti di un ruolo
        /// </summary>
        /// <param name="nodoRuoli"></param>
        /// <param name="ruolo"></param>
        private void UtentiRuolo(myTreeNode2 nodoRuoli, DocsPaWR.OrgRuolo ruolo)
        {
            myTreeNode2 nodoUtenti;

            nodoRuoli.Expanded = true;

            foreach (DocsPaWR.OrgUtente utente in ruolo.Utenti)
            {
                nodoUtenti = new myTreeNode2();

                nodoUtenti.ID = utente.IDCorrGlobale;
                nodoUtenti.TipoNodo = "P";
                nodoUtenti.Text = utente.CodiceRubrica + " - " + utente.Cognome + " " + utente.Nome;
                nodoUtenti.ImageUrl = this.getElementType(nodoUtenti.TipoNodo);
                nodoUtenti.Expanded = true;

                nodoRuoli.ChildNodes.Add(nodoUtenti);
            }
        }

        /// <summary>
        /// Filtro su lista dei ruoli con un RF dato
        /// </summary>
        /// <param name="idRuolo"></param>
        /// <returns></returns>
        private bool esitoRicercaRuoloConRF(string idRuolo)
        {
            bool retValue = false;

            if (this._ruoliConRFselezionato != null)
            {
                foreach (DocsPaWR.OrgRuolo currentRuolo in this._ruoliConRFselezionato)
                {
                    if (currentRuolo.IDCorrGlobale.Equals(idRuolo))
                    {
                        retValue = true;
                        break;
                    }
                }
            }

            return retValue;
        }

        /// <summary>
        /// Visualizza le sotto UO
        /// </summary>
        /// <param name="currentUO"></param>
        /// <param name="indice"></param>
        /// <returns></returns>
        private bool SottoUO(DocsPaWR.OrgUO currentUO, myTreeNode2 TreeNodo)
        {
            bool retValue = true;

            myTreeNode2 TreeNodoFiglio;
            myTreeNode2 nodoUO;

            ArrayList lista = new ArrayList();

            int livello = Convert.ToInt32(currentUO.Livello) + 1;

            OrganigrammaManager manager = new OrganigrammaManager();
            manager.ListaUO(currentUO.IDCorrGlobale, Convert.ToString(livello), currentUO.IDAmministrazione);
            lista = manager.getListaUO();

            if (lista != null && lista.Count > 0)
            {
                if (!this.isOrdinamentoAbilitato)
                    lista.Sort(new UOSort());
                if (UIManager.UserManager.IsAuthorizedFunctions("DO_VIS_LIV_ORG"))
                {
                    foreach (DocsPaWR.OrgUO sottoUO in lista)
                    {
                        nodoUO = new myTreeNode2();

                        nodoUO.ID = sottoUO.IDCorrGlobale;
                        nodoUO.TipoNodo = "U";
                        nodoUO.Text = sottoUO.CodiceRubrica + " - [" + sottoUO.Livello + "] - " + sottoUO.Descrizione;
                        nodoUO.ImageUrl = this.getElementType(nodoUO.TipoNodo);
                        nodoUO.Expanded = false;

                        TreeNodo.ChildNodes.Add(nodoUO);

                        nodoUO.RuoliUO = sottoUO.Ruoli;
                        nodoUO.SottoUO = sottoUO.SottoUo;
                        nodoUO.Livello = sottoUO.Livello;

                        if ((int.Parse(this.ddlView.SelectedValue) > 1 && Convert.ToInt32(sottoUO.Ruoli) > 0) || Convert.ToInt32(sottoUO.SottoUo) > 0)
                        {
                            TreeNodoFiglio = new myTreeNode2();
                            TreeNodoFiglio.Text = "<font color='#ff0000'>&nbsp;loading...</font>";
                            nodoUO.ChildNodes.Add(TreeNodoFiglio);
                        }
                    }
                }
                else
                {
                    foreach (DocsPaWR.OrgUO sottoUO in lista)
                    {
                        nodoUO = new myTreeNode2();

                        nodoUO.ID = sottoUO.IDCorrGlobale;
                        nodoUO.TipoNodo = "U";
                        nodoUO.Text = sottoUO.CodiceRubrica + " - " + sottoUO.Descrizione;
                        nodoUO.ImageUrl = this.getElementType(nodoUO.TipoNodo);
                        nodoUO.Expanded = false;

                        TreeNodo.ChildNodes.Add(nodoUO);

                        nodoUO.RuoliUO = sottoUO.Ruoli;
                        nodoUO.SottoUO = sottoUO.SottoUo;
                        nodoUO.Livello = sottoUO.Livello;

                        if ((int.Parse(this.ddlView.SelectedValue) > 1 && Convert.ToInt32(sottoUO.Ruoli) > 0) || Convert.ToInt32(sottoUO.SottoUo) > 0)
                        {
                            TreeNodoFiglio = new myTreeNode2();
                            TreeNodoFiglio.Text = "<font color='#ff0000'>&nbsp;loading...</font>";
                            nodoUO.ChildNodes.Add(TreeNodoFiglio);
                        }
                    }
                }
            }
            else
                retValue = false;

            return retValue;
        }

        /// <summary>
        /// Verifica se l'utente ha selezionato il filtro di ricerca per RF
        /// </summary>
        /// <returns></returns>
        private bool verificaStatoRicercaRF()
        {
            bool retValue = false;
            if (this.ddlSearchIn.Items[this.ddlSearchIn.SelectedIndex].Value.Equals("RF"))
                retValue = true;
            return retValue;
        }

        /// <summary>
        /// Ricerca per RF selezionato
        /// </summary>
        /// <param name="ricercaAncheUtenti">True o False: indica se la ricerca deve visualizzare anche gli utenti dei ruoli</param> 
        private void performFindRF(bool ricercaAncheUtenti)
        {
            this._ricercaRFimpostata = true;

            this.getRuoliRFSelezionato();

            if (this._ruoliConRFselezionato.Length > 0)
            {
                this.treeViewUO.Nodes.Clear();
                this.VisualizzaNodoRoot();

                if (ricercaAncheUtenti)
                {
                    this.ddlView.SelectedIndex = 2;
                }
                else
                {
                    if (this.ddlView.SelectedIndex == 0)
                        this.ddlView.SelectedIndex = 1;
                }

                this.EspandeOrgDallaRoot();
                this.pulisceNodiOrfaniRF();
                this.UpPnlTree.Update();
                this.UpContainer.Update();
            }
            else
                ScriptManager.RegisterStartupScript(this, this.GetType(), "dialog", "ajaxDialogModal('ErrorOrganizationChartRFCustom', 'error', '', '"+utils.FormatJs(this.ddlRF.SelectedItem.Text)+"');", true);
        }

        /// <summary>
        /// Reperisce la lista dei ruoli con l'RF selezionato
        /// </summary>
        private void getRuoliRFSelezionato()
        {
            this._ruoliConRFselezionato = OrganigrammaManager.AmmGetListaRuoliAOO(this.ddlRF.SelectedValue);
        }

        /// <summary>
        /// Espande tutto l'organigramma dalla Root
        /// </summary>
        private void EspandeOrgDallaRoot()
        {
            if (this.treeViewUO.Nodes.Count > 0)
            {
                myTreeNode2 TreeNodo = (myTreeNode2)this.treeViewUO.Nodes[0];
                this.ricorsioneCercaFigli(TreeNodo);
            }
        }

        /// <summary>
        /// Ricorsione sui nodi figli della treeview
        /// </summary>
        /// <param name="nodo"></param>
        private void ricorsioneCercaFigli(myTreeNode2 nodo)
        {
            if (nodo.TipoNodo.Equals("U"))
            {
                this.executeExpand(nodo);

                foreach (myTreeNode2 currentNode in nodo.ChildNodes)
                    this.ricorsioneCercaFigli(currentNode);
            }
        }

        /// <summary>
        /// Esegue l'espansione del nodo
        /// </summary>
        /// <param name="indice"></param>
        private void executeExpand(myTreeNode2 TreeNodo)
        {
            DocsPaWR.OrgUO currentUO;

            TreeNodo.Expanded = true;

            if (TreeNodo.ChildNodes.Count > 0)
                TreeNodo.ChildNodes.Clear();

            switch (TreeNodo.TipoNodo)
            {
                case "U":

                    currentUO = new DocsPaWR.OrgUO();
                    currentUO.IDCorrGlobale = TreeNodo.ID;
                    currentUO.IDAmministrazione = this.idAmm;
                    currentUO.Ruoli = TreeNodo.RuoliUO;
                    currentUO.SottoUo = TreeNodo.SottoUO;
                    currentUO.Livello = TreeNodo.Livello;

                    if (this.ddlView.SelectedIndex >= 1 && Convert.ToInt32(currentUO.Ruoli) > 0)
                        this.RuoliUO(currentUO, TreeNodo);

                    if (Convert.ToInt32(currentUO.SottoUo) > 0)
                        this.SottoUO(currentUO, TreeNodo);

                    break;
            }
        }

        /// <summary>
        /// Elimina i nodi di tipo UO sulla TreeView che non presentano ruoli associati all'RF selezionato
        /// </summary>
        private void pulisceNodiOrfaniRF()
        {
            if (this.treeViewUO.Nodes.Count > 0)
            {
                myTreeNode2 TreeNodo = (myTreeNode2)treeViewUO.Nodes[0];

                this.ricorsioneCercaFigliOrfaniRF(TreeNodo);
            }
        }

        /// <summary>
        /// Ricorsione del metodo "pulisceNodiOrfaniRF"
        /// </summary>
        /// <param name="nodo"></param>
        private void ricorsioneCercaFigliOrfaniRF(myTreeNode2 nodo)
        {
            if (nodo.TipoNodo.Equals("U"))
            {
                if (nodo.ChildNodes.Count > 0)
                {
                    try
                    {
                        foreach (myTreeNode2 currentNode in nodo.ChildNodes)
                        {
                            if (currentNode.ChildNodes.Count == 0 && currentNode.TipoNodo.Equals("U"))
                                this.treeViewUO.Nodes.Remove(currentNode);
                            else
                                this.ricorsioneCercaFigliOrfaniRF(currentNode);
                        }
                    }
                    catch
                    {
                        this.pulisceNodiOrfaniRF();
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Visualizza partendo dalla UO selezionata in Treeview
        /// </summary>
        private void VisualizzaUOSelezionata()
        {
            try
            {
                if (this.treeViewUO.SelectedNode!=null)
                {
                    myTreeNode2 TreeNodo;
                    TreeNodo = (myTreeNode2)this.treeViewUO.SelectedNode;

                    if (TreeNodo.TipoNodo.Equals("U")) // solo se è una UO
                    {
                        DocsPaWR.OrgUO currentUO = this.GetDatiUOCorrente(TreeNodo.ID);
                        if (currentUO != null)
                        {
                            // diventa la ROOT della treeview
                            myTreeNode2 treenode = new myTreeNode2();
                            treenode = this.SetRootTreeview(currentUO);

                            this.SetRootTreeviewExpanded();

                            this.hd_lastReturnValueModal.Value = currentUO.IDCorrGlobale + "_" + currentUO.IDParent + "_U";
                        }
                    }
                }
            }
            catch
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "dialog", "ajaxDialogModal('ErrorOrganizationChartSystemError', 'error', '');", true);
            }
        }

        /// <summary>
        /// re-imposta la GUI di default per la tipologia di ricerca
        /// </summary>
        private void restoreSearchDefault()
        {
            this.lblCode.Text = this.GetLabel("OrganizationChartLblCode");
            this.lblName.Text = this.GetLabel("OrganizationChartLblNameUO");

            this.txtCode.Visible = true;
            this.txtName.Visible = true;
            this.ddlRF.Visible = false;

            this._ricercaRFimpostata = false;
        }

        /// <summary>
        /// Converte i dati visualizzati nella Treeview in un file XML
        /// </summary>
        /// <returns></returns>
        private XmlDocument exportToXML()
        {
            XmlDocument xmldoc = new XmlDocument();

            if (this.treeViewUO.Nodes.Count > 0)
            {
                myTreeNode2 nodo = (myTreeNode2)treeViewUO.Nodes[0];

                //gestione RF
                string titoloPDF = string.Empty;
                if (this.verificaStatoRicercaRF())
                    titoloPDF = "Composizione RF: " + this.ddlRF.Items[this.ddlRF.SelectedIndex].Text;
                else
                    titoloPDF = "Organigramma";

                XmlNode organigramma = xmldoc.AppendChild(xmldoc.CreateElement("ORGANIGRAMMA"));

                XmlAttribute attrRoot = xmldoc.CreateAttribute("title");
                attrRoot.InnerText = titoloPDF;
                organigramma.Attributes.Append(attrRoot);

                XmlElement record = xmldoc.CreateElement("RECORD");
                record.SetAttribute("tipo", nodo.TipoNodo);
                record.SetAttribute("desc", nodo.Text);
                xmldoc.DocumentElement.AppendChild(record);

                this.addElement(nodo, xmldoc, 1);
            }

            return xmldoc;
        }

        /// <summary>
        /// Ricorsione
        /// </summary>
        /// <param name="nodo"></param>
        /// <param name="xmldoc"></param>
        /// <param name="indentazionePadre"></param>
        private void addElement(myTreeNode2 nodo, XmlDocument xmldoc, int indentazionePadre)
        {
            string prefisso = string.Empty;

            if ((bool)nodo.Expanded && nodo.ChildNodes.Count > 0)
            {
                indentazionePadre += 1;

                TreeNodeCollection nodi = nodo.ChildNodes;
                foreach (myTreeNode2 n in nodi)
                {
                    XmlElement record = xmldoc.CreateElement("RECORD");
                    record.SetAttribute("tipo", n.TipoNodo);
                    switch (n.TipoNodo)
                    {
                        case "U":
                            prefisso = "[UO] ";
                            break;
                        case "R":
                            prefisso = "[R] ";
                            break;
                        case "P":
                            prefisso = "[U] ";
                            break;
                    }
                    record.SetAttribute("desc", this.addIndentation(indentazionePadre) + prefisso + n.Text);
                    xmldoc.DocumentElement.AppendChild(record);

                    this.addElement(n, xmldoc, indentazionePadre);
                }
            }
        }

        /// <summary>
        /// Imposta sull'XML l'indentazione dei dati
        /// </summary>
        /// <param name="indentazionePadre"></param>
        /// <returns></returns>
        private string addIndentation(int indentazionePadre)
        {
            string indent = string.Empty;

            string fixPlus = "+ ";

            if (indentazionePadre.Equals(2))
                indent = fixPlus;
            else if (indentazionePadre.Equals(3))
                indent = "--" + fixPlus;
            else if (indentazionePadre > 3)
            {
                for (int n = 1; n <= (indentazionePadre); n++)
                {
                    indent += "-";
                }
                indent += fixPlus;
            }

            return indent;
        }

        private void PerformSearch()
        {
            int num_trovati = 0;
            string returnValue = string.Empty;
            string codice = this.Codice.Trim();
            string descrizione = this.Descrizione.Trim();
            string idAmm = this.IdAmm.Trim();
            string tipo = this.Tipo.Trim();
            bool searchHistoricized = this.SearchHistoricized;

            if (!string.IsNullOrEmpty(idAmm))
            {
                OrganigrammaManager theManager = new OrganigrammaManager();
                theManager.RicercaInOrg(tipo, codice, descrizione, idAmm, searchHistoricized, false);
                if (theManager.getRisultatoRicerca() != null && theManager.getRisultatoRicerca().Count > 0)
                {
                    this.InitializeDataSetRisultato();

                    DataRow row;
                    foreach (DocsPaWR.OrgRisultatoRicerca risultato in theManager.getRisultatoRicerca())
                    {
                        returnValue = risultato.IDCorrGlob + "_" + risultato.IDParent + "_" + tipo;

                        row = dsRisultato.Tables[0].NewRow();
                        row["IDCorrGlob"] = risultato.IDCorrGlob;
                        row["Codice"] = "<a href=\"javascript:void(0)\" onclick=\"parent.closeAjaxModal('OrganizationChartSearchResult', '" + returnValue + "');\">" + risultato.Codice + "</a>";
                        row["Descrizione"] = "<a href=\"javascript:void(0)\" onclick=\"parent.closeAjaxModal('OrganizationChartSearchResult', '" + returnValue + "');\">" + risultato.Descrizione + "</a>";
                        row["IDParent"] = risultato.IDParent;
                        row["DescParent"] = risultato.DescParent;

                        dsRisultato.Tables["LISTA_RICERCA"].Rows.Add(row);

                        num_trovati++;
                    }

                    if (num_trovati > 1)
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "popup", "ajaxModalPopupOrganizationChartSearchResult();", true);
                    }
                    else
                    {
                        this.VisualizzaNodoRicercato(returnValue);
                        this.UpPnlTree.Update();
                        this.UpContainer.Update();
                    }
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "dialog", "ajaxDialogModal('InfoOrganizationChartSearchResultNone', 'info', '');", true);
                }
            }
        }

        private void InitializeDataSetRisultato()
        {
            dsRisultato = new DataSet();
            DataColumn dc;
            dsRisultato.Tables.Add("LISTA_RICERCA");
            dc = new DataColumn("IDCorrGlob");
            dsRisultato.Tables["LISTA_RICERCA"].Columns.Add(dc);
            dc = new DataColumn("Codice");
            dsRisultato.Tables["LISTA_RICERCA"].Columns.Add(dc);
            dc = new DataColumn("Descrizione");
            dsRisultato.Tables["LISTA_RICERCA"].Columns.Add(dc);
            dc = new DataColumn("IDParent");
            dsRisultato.Tables["LISTA_RICERCA"].Columns.Add(dc);
            dc = new DataColumn("DescParent");
            dsRisultato.Tables["LISTA_RICERCA"].Columns.Add(dc);
        }

        #endregion

    }
}