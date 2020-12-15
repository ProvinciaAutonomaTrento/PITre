using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.DocsPaWR;
using NttDataWA.UIManager;
using NttDataWA.Utils;
using NttDatalLibrary;
//using log4net;

namespace NttDataWA.Popup
{
    public partial class AddressBook : System.Web.UI.Page
    {
        //private ILog logger = LogManager.GetLogger(typeof(ObjectManager));

        private const string IMG_BLANK = "../Images/Icons/blank_icon.png";
        private const string IMG_AT_SELECT = "../Images/Icons/addressBook_bg_btn_a_click.png";
        private const string IMG_AT_UNSELECT = "../Images/Icons/addressBook_bg_btn_a.png";
        private const string IMG_CC_SELECT = "../Images/Icons/addressBook_bg_btn_cc_click.png";
        private const string IMG_CC_UNSELECT = "../Images/Icons/addressBook_bg_btn_cc.png";
        private const string IMG_FROM_SELECT = "../Images/Icons/addressBook_bg_btn_da_click.png";
        private const string IMG_FROM_UNSELECT = "../Images/Icons/addressBook_bg_btn_da.png";
        private const string IMG_ALL_A_ADD = "../Images/Icons/addressBook_supA.png";
        private const string IMG_ALL_DA_ADD = "../Images/Icons/addressBook_supDA.png";

        public bool multipleSelection;
        private bool ccTableVisible;

        private string language;
        private string lblNumFound;
        private string destinationList_atText;
        private string destinationList_fromText;
        private string destinationList_selectText;

        private int atRowForPage = 4;
        private int ccRowForPage = 4;
        private int foundRowForPage = 8;

        private int rowForPage = 100000;//int.Parse(InitConfigurationKeys.GetValue(UserManager.GetUserInSession().idAmministrazione, "RUBRICA_RIGHE_PER_PAGINA"));
        //private int rowForPage = int.Parse(InitConfigurationKeys.GetValue("0", "RUBRICA_RIGHE_PER_PAGINA"));

        public class CorrespondentDetail
        {
            public string SystemID { get; set; }
            public string Tipo { get; set; }
            public string ImgTipo { get; set; }
            public string CodiceRubrica { get; set; }
            public string Descrizione { get; set; }
            public string Canale { get; set; }
            public string Rubrica { get; set; }
            public string At { get; set; }
            public string Cc { get; set; }
            public bool isRubricaComune { get; set; }
            public string EI_Type { get; set; }
            public bool Enabled { get; set; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                this.language = UserManager.GetUserLanguage();
                this.setVariableText();
                this.setFormContent();

                if (!IsPostBack)
                {
                    this.AaTableVisible = true;
                    this.InitializePage();
                    if (this.AddressBookDDLTypeAOO.Enabled)
                        loadRfData();

                    //Laura 19 Marzo
                    select_tipoIE();

                    this.ClearAllGrid();
                    this.TxtDescription.Focus();
                }
                else
                {
                    if (!string.IsNullOrEmpty(this.AddressBook_Details.ReturnValue))
                    {
                        if (this.AddressBook_Details.ReturnValue.IndexOf("del") >= 0)
                        {
                            string item_systemId = this.AddressBook_Details.ReturnValue.Split('|')[1];

                            if (existCorrespondentInGrid(item_systemId))
                            {
                                bool existsEmptyRow = false;
                                LitAddressBookAt.Text = deleteElementInGridView(item_systemId, GrdAtSelection, atRowForPage, out existsEmptyRow).ToString() + " " + this.lblNumFound;

                                if (existsEmptyRow) refreshImageStateInGrid(GrdAtSelection);
                                UpPnlGridAt.Update();
                            }
                        }

                        this.AddressBookSearch_Click(null, null);
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('AddressBook_Details','');", true);
                    }

                    if (!string.IsNullOrEmpty(this.AddressBook_New.ReturnValue))
                    {
                        this.addAtCorrespondent(this.AddressBook_New.ReturnValue);
                        LitAddressBookResult.Text = "1 " + this.lblNumFound;
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('AddressBook_New','');", true);
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

        private void setVariableText()
        {
            this.lblNumFound = Utils.Languages.GetLabelFromCode("AddressBookLblNumFound", language);
            this.destinationList_atText = Utils.Languages.GetLabelFromCode("AddressBookLitA", language);
            this.destinationList_fromText = Utils.Languages.GetLabelFromCode("AddressBookLitDa", language);
            this.destinationList_selectText = Utils.Languages.GetLabelFromCode("AddressBookLitIn", language);
        }

        private bool treeVisible(){
            return this.liAddressBookLinkOrg.Visible;
        }

        private void CreateEmptyRow(GridView gridObject, int totalRow)
        {
            List<CorrespondentDetail> CorrespondentsFound = new List<CorrespondentDetail>();

            for (int i = 0; i <= totalRow; i++)
            {
                CorrespondentDetail newElement = new CorrespondentDetail();
                newElement.Descrizione = "";
                newElement.Tipo = "";
                newElement.ImgTipo = IMG_BLANK;
                newElement.At = IMG_BLANK;
                newElement.Cc = IMG_BLANK;
                newElement.Canale = "";
                newElement.Rubrica = "";
                newElement.SystemID = "-1";
                newElement.isRubricaComune = false;
                newElement.CodiceRubrica = "-1";
                newElement.EI_Type = "";
                newElement.Enabled = true;

                CorrespondentsFound.Add(newElement);
            }

            gridObject.DataSource = CorrespondentsFound;
            gridObject.DataBind();

            if (gridObject.Equals(GrdAddressBookResult))
            {
                foreach (GridViewRow item in GrdAddressBookResult.Rows)
                {
                    Image imgDetail_inRow = item.FindControl("imgDetail") as Image;
                    if (imgDetail_inRow != null) imgDetail_inRow.Visible = false;
                    Image imgA_inRow = item.FindControl("imgA") as Image;
                    if (imgA_inRow != null) imgA_inRow.Visible = false;
                    Image imgCc_inRow = item.FindControl("imgCc") as Image;
                    if (imgCc_inRow != null) imgCc_inRow.Visible = false;
                }
            }
            else
            {
                foreach (GridViewRow item in GrdAddressBookResult.Rows)
                {
                    Image imgDeleteA_inRow = item.FindControl("imgDeleteA") as Image;
                    if (imgDeleteA_inRow != null) imgDeleteA_inRow.Visible = false;
                    Image imgDeleteCc_inRow = item.FindControl("imgDeleteCc") as Image;
                    if (imgDeleteCc_inRow != null) imgDeleteCc_inRow.Visible = false;
                }
            }
        }

        private List<CorrespondentDetail> AddEmptyRow(List<CorrespondentDetail> listForAdd, int totalRowInList, out bool existsEmptyRow)
        {
            if (listForAdd == null) listForAdd = new List<CorrespondentDetail>();

            List<CorrespondentDetail> temp_listForAdd = new List<CorrespondentDetail>(listForAdd);

            if (listForAdd.Count <= totalRowInList)
            {
                while (temp_listForAdd.Count() <= totalRowInList)
                {
                    CorrespondentDetail newElement = new CorrespondentDetail();
                    newElement.Descrizione = "";
                    newElement.Tipo = "";
                    newElement.ImgTipo = IMG_BLANK;
                    newElement.At = IMG_BLANK;
                    newElement.Cc = IMG_BLANK;
                    newElement.Canale = "";
                    newElement.Rubrica = "";
                    newElement.SystemID = "-1";
                    newElement.isRubricaComune = false;
                    newElement.CodiceRubrica = "-1";
                    newElement.EI_Type = "";
                    newElement.Enabled = true;

                    temp_listForAdd.Add(newElement);
                }
                existsEmptyRow = true;
            }
            else
            {
                existsEmptyRow = false;
            }

            return temp_listForAdd;
        }

        private void RefreshScript()
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshTipsy", "tooltipTipsy();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshSelect", "refreshSelect();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "InitDragNDrop", "InitDragNDrop();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ResizeTables", "resizeTable();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);
        }

        private void InitializePage()
        {
            this.FromInstanceAccess = false;
            this.FromNewProject = false;
            
            if (Request.QueryString["popupid"] == "ManageAddressBook")
                this.CallType = RubricaCallType.CALLTYPE_MANAGE;

            if (Request.QueryString["rt"] != null && Request.QueryString["rt"] == "serachDocPopup")
            {
                this.FromInstanceAccess = true;
            }
            if (Request.QueryString["from"] != null && Request.QueryString["from"] == "newProject")
            {
                this.FromNewProject = true;
            }
            this.IsOrganigramMode = false;
            //this.IsTotalOrganigram = false;
            ConfigurazioniRubricaComune configSetting = CommonAddressBook.Configurations.GetConfigurations(UIManager.UserManager.GetInfoUser());
            this.EnableCommonAddressBook = configSetting.GestioneAbilitata;

            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.LISTE_DISTRIBUZIONE.ToString()]) && System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.LISTE_DISTRIBUZIONE.ToString()].Equals("1"))
            {
                this.EnableDistributionLists = true;
            }

            this.InitializeLanguage();
            this.InitializeFilters();

            if (this.ChkListType.Items.FindByValue("AddressBookChkRF") != null)
            {
                if (AdministrationManager.IsEnableRF(UserManager.GetInfoUser().idAmministrazione))
                {
                    this.ChkListType.Items.RemoveAt(4);
                }
            }

            if (!string.IsNullOrEmpty(Request.QueryString["popupid"]) && Request.QueryString["popupid"] == "ManageAddressBook")
                this.AddressBookSave.Enabled = false;

            HttpContext.Current.Session.Remove("AddressBook.corrFilter");
            this.AddressBookExportSearch.Enabled = false;
        }

        private void InitializeFilters()
        {
            //KEY ABILITAZIONE_FLAG_RUBRICA
            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.ABILITAZIONE_FLAG_RUBRICA.ToString()]))
            {
                char[] separator = { '|' };
                String[] prefissi = System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.ABILITAZIONE_FLAG_RUBRICA.ToString()].Split(separator);
                for (int i = 0; i < prefissi.Length; i++)
                {
                    if (this.AddressBookChkOfficies.Enabled)
                    {
                        this.AddressBookChkOfficies.Selected = (prefissi[0].ToUpper().Equals("Y"));
                    }
                    if (this.AddressBookChkRoles.Enabled)
                    {
                        this.AddressBookChkRoles.Selected = (prefissi[2].ToUpper().Equals("Y"));
                    }
                    if (this.AddressBookChkUsers.Enabled)
                    {
                        this.AddressBookChkUsers.Selected = (prefissi[1].ToUpper().Equals("Y"));
                    }
                    //caso per le liste: se sono abilitate in amministrazione bisogna consentirne o meno
                    //l'abilitazione del flag da web.config
                    if (this.AddressBookChkLists.Enabled)
                    {
                        if (prefissi.Length > 3 && prefissi[3] != null)
                        {
                            this.AddressBookChkLists.Selected = (prefissi[3].ToUpper().Equals("Y"));
                        }
                    }
                }
            }
            else if (HttpContext.Current.Session["AddressBook.EnableOnly"] == null)
            { // come era prima
                this.AddressBookChkOfficies.Selected = true;
                this.AddressBookChkRoles.Selected = true;
                this.AddressBookChkUsers.Selected = true;
            }

            if (this.CallType != null)
            {

                this.debug_callType(this.CallType);

                this.AddressBookImport.Visible = false;
                this.AddressBookExport.Visible = false;
                this.AddressBookExportSearch.Visible = false;
                this.AddressBookDownloadTemplate.Visible = false;

                #region switch case
                switch (this.CallType)
                {
                    case RubricaCallType.CALLTYPE_PROTO_IN:
                    case RubricaCallType.CALLTYPE_PROTO_IN_INT:
                        //ibtnMoveToCC.Visible = false;
                        //dgCC.Visible = false;
                        //lblCC.Visible = false;
                        //lblA.Text = "Da";
                        //dgCorr.AllowMultipleSelection = false;
                        //tvOrganigramma.SelectorType = TreeViewSelectorType.RadioButton;
                        //dgA.PageSize = 1;
                        //tc.VAlign = "top";
                        this.RblTypeCorrespondent.Items.FindByValue("E").Selected = true;
                        this.RblTypeCorrespondent.Enabled = true;
                        //if (!this.IsPostBack)
                        //    this.ddlIE.SelectedValue = "E";
                        //cbSelAll.Visible = false;
                        //handle_proto_giallo();
                        //cbRuoli.Visible = true;
                        //cbUtenti.Visible = true;
                        //cbComandi.Visible = true;

                        this.AddressBookChkLists.Enabled = false;

                        this.AddressBookChkRF.Enabled = false;

                        this.AddressBookChkCommonAddressBook.Visible = this.AddressBookChkCommonAddressBook.Checked = this.EnableCommonAddressBook;
                        break;

                    case RubricaCallType.CALLTYPE_PROTO_INT_MITT:
                        this.RblTypeCorrespondent.Items.FindByValue("I").Selected = true;
                        this.RblTypeCorrespondent.Enabled = false;
                        //    //lblCC.Visible = false;
                        //    //lblA.Text = "Da";
                        //    //dgCorr.AllowMultipleSelection = false;
                        //    //tvOrganigramma.SelectorType = TreeViewSelectorType.RadioButton;
                        //    //dgA.PageSize = 1;
                        //    //tc.VAlign = "top";
                        //    this.RblTypeCorrespondent.Enabled = false;
                        //    //this.RblTypeCorrespondent.Enabled =  false;
                        //    //cbSelAll.Visible = false;
                        //    //handle_proto_giallo();
                        //    cbRuoli.Visible = true;
                        //    cbUtenti.Visible = true;
                        //    cbComandi.Visible = true;

                        this.AddressBookChkLists.Enabled = false;
                        this.AddressBookDDLTypeAOO.Enabled = false;

                        //    //disabilito il pulsante Nuovo Corrispondente per i mittenti del protocollo interno
                        this.AddressBookNew.Enabled = false;

                        this.AddressBookChkRF.Enabled = false;

                        //    //cb_rf.Checked = false;
                        this.AddressBookChkCommonAddressBook.Visible = this.AddressBookChkCommonAddressBook.Checked = false;
                        break;

                    case RubricaCallType.CALLTYPE_VIS_UTENTE:
                        this.RblTypeCorrespondent.Items.FindByValue("I").Selected = true;
                        this.RblTypeCorrespondent.Enabled = false;
                        this.ChkListType.Items.FindByValue("P").Selected = true;
                        this.ChkListType.Enabled = false;
                        this.AddressBookChkOfficies.Enabled = false;
                        this.AddressBookNew.Enabled = false;
                        this.AddressBookChkRoles.Enabled = false;
                        this.AddressBookChkRoles.Selected = false;
                        this.AddressBookChkOfficies.Selected = false;
                        this.AddressBookNew.Enabled = false;
                        this.AddressBookChkLists.Enabled = false;
                        this.AddressBookNew.Enabled = false;
                        this.AddressBookChkRF.Enabled = false;
                        this.AddressBookDDLTypeAOO.Enabled = false;
                        this.AddressBookChkCommonAddressBook.Visible = this.AddressBookChkCommonAddressBook.Checked = false;
                        this.liAddressBookLinkOrg.Visible = false;
                        this.AddressBookLinkOrg.Visible = false;
                        break;

                    case RubricaCallType.CALLTYPE_VIS_RUOLO:
                        this.RblTypeCorrespondent.Items.FindByValue("I").Selected = true;
                        this.RblTypeCorrespondent.Enabled = false;
                        this.ChkListType.Items.FindByValue("R").Selected = true;
                        this.ChkListType.Enabled = false;
                        this.AddressBookChkOfficies.Enabled = false;
                        this.AddressBookNew.Enabled = false;
                        this.AddressBookChkUsers.Enabled = false;
                        this.AddressBookChkUsers.Selected = false;
                        this.AddressBookNew.Enabled = false;
                        this.AddressBookChkLists.Enabled = false;
                        this.AddressBookChkOfficies.Selected = false;
                        this.AddressBookNew.Enabled = false;
                        this.AddressBookChkRF.Enabled = false;
                        this.AddressBookDDLTypeAOO.Enabled = false;
                        this.AddressBookChkCommonAddressBook.Visible = this.AddressBookChkCommonAddressBook.Checked = false;
                        this.liAddressBookLinkOrg.Visible = false;
                        this.AddressBookLinkOrg.Visible = false;
                        break;

                    case RubricaCallType.CALLTYPE_PROTO_OUT_MITT:
                    case RubricaCallType.CALLTYPE_PROTO_OUT_MITT_SEMPLIFICATO:
                        //    ibtnMoveToCC.Visible = false;
                        //    dgCC.Visible = false;
                        //    lblCC.Visible = false;
                        //    lblA.Text = "Da";
                        //    dgCorr.AllowMultipleSelection = false;
                        //    tvOrganigramma.SelectorType = TreeViewSelectorType.RadioButton;
                        //    dgA.PageSize = 1;
                        //    tc.VAlign = "top";
                        this.RblTypeCorrespondent.Items.FindByValue("I").Selected = true;
                        this.RblTypeCorrespondent.Enabled = false;
                        this.AddressBookChkLists.Enabled = false;
                        //    handle_proto_giallo();
                        //    cbRuoli.Visible = true;
                        //    cbUtenti.Visible = true;
                        //    cbComandi.Visible = true;
                        //    cbListeDist.Checked = false;
                        //    //disabilito il pulsante Nuovo Corrispondente per i mittenti del protocollo in uscita
                        //          this.AddressBookNew.Enabled = false;
                        //    this.lbl_registro.Visible = false;
                        this.AddressBookDDLTypeAOO.Enabled = false;

                        this.AddressBookChkRF.Enabled = true;
                        //    //cb_rf.Checked = false;
                        this.AddressBookChkCommonAddressBook.Visible = this.AddressBookChkCommonAddressBook.Checked = false;
                        this.AddressBookNew.Enabled = false;
                        break;

                    case RubricaCallType.CALLTYPE_DEST_FOR_SEARCH_MODELLI:
                        //    ibtnMoveToCC.Visible = false;
                        //    dgCC.Visible = false;
                        //    lblCC.Visible = false;
                        //    lblA.Text = "A";
                        //    dgCorr.AllowMultipleSelection = false;
                        //    tvOrganigramma.SelectorType = TreeViewSelectorType.RadioButton;
                        //    dgA.PageSize = 1;
                        //    tc.VAlign = "top";
                        this.RblTypeCorrespondent.Items.FindByValue("I").Selected = true;
                        this.RblTypeCorrespondent.Enabled = false;

                        this.AddressBookChkLists.Enabled = false;

                        this.AddressBookChkOfficies.Enabled = true;
                        this.AddressBookChkRoles.Enabled = true;
                        this.AddressBookChkUsers.Enabled = true;

                        this.AddressBookChkOfficies.Selected = false;
                        this.AddressBookChkRoles.Selected = true;
                        this.AddressBookChkUsers.Selected = false;

                        //    handle_proto_giallo();
                        //    if (!IsPostBack)
                        //    {
                        //        cbRuoli.Visible = true;
                        //        cbRuoli.Enabled = true;
                        //        cbUtenti.Visible = true;
                        //        cbUtenti.Enabled = true;
                        //        cbUtenti.Checked = false;
                        //        cbComandi.Visible = true;
                        //        cbComandi.Enabled = true;
                        //        cbComandi.Checked = false;
                        //    }
                        //    cbListeDist.Checked = false;
                        //    //disabilito il pulsante Nuovo Corrispondente per i mittenti del protocollo in uscita
                        this.AddressBookNew.Enabled = false;
                        //    this.lbl_registro.Visible = false;
                        this.AddressBookDDLTypeAOO.Enabled = false;

                        this.AddressBookChkRF.Enabled = false;
                        //    //cb_rf.Checked = false;
                        break;

                    case RubricaCallType.CALLTYPE_RICERCA_MITTDEST:
                    case RubricaCallType.CALLTYPE_RICERCA_MITTINTERMEDIO:
                    case RubricaCallType.CALLTYPE_RICERCA_ESTESA:
                    case RubricaCallType.CALLTYPE_RICERCA_COMPLETAMENTO:
                    case RubricaCallType.CALLTYPE_RICERCA_DOCUMENTI:
                        //    ibtnMoveToCC.Visible = false;
                        //    dgCC.Visible = false;
                        //    lblCC.Visible = false;
                        //    lblA.Text = "Da";
                        //    dgCorr.AllowMultipleSelection = false;
                        //    tvOrganigramma.SelectorType = TreeViewSelectorType.RadioButton;
                        //    dgA.PageSize = 1;
                        //    tc.VAlign = "top";
                        this.RblTypeCorrespondent.Items.FindByValue("IE").Selected = true;
                        this.RblTypeCorrespondent.Enabled = false;

                        this.AddressBookChkLists.Enabled = false;
                        //    handle_proto_giallo();
                        //    cbListeDist.Checked = false;
                        //    //disabilito il pulsante Nuovo Corrispondente nelle ricerche dei documenti
                        this.AddressBookNew.Enabled = false;
                        //    this.ddlIE.SelectedValue = "IE";

                        this.AddressBookChkRF.Enabled = false;
                        //    //cb_rf.Checked = false;
                        break;

                    case RubricaCallType.CALLTYPE_RICERCA_DOCUMENTI_CORR_INT:
                        //    ibtnMoveToCC.Visible = false;
                        //    dgCC.Visible = false;
                        //    lblCC.Visible = false;
                        //    lblA.Text = "Da";
                        //    dgCorr.AllowMultipleSelection = false;
                        //    tvOrganigramma.SelectorType = TreeViewSelectorType.RadioButton;
                        //    dgA.PageSize = 1;
                        //    tc.VAlign = "top";
                        this.RblTypeCorrespondent.Items.FindByValue("I").Selected = true;
                        this.RblTypeCorrespondent.Enabled = false;

                        this.AddressBookChkLists.Enabled = false;

                        //    handle_proto_giallo();
                        //    cbListeDist.Checked = false;

                        this.AddressBookChkRF.Enabled = false;

                        //    //cb_rf.Checked = false;
                        break;

                    case RubricaCallType.CALLTYPE_PROTO_OUT:
                    case RubricaCallType.CALLTYPE_PROTO_USCITA_SEMPLIFICATO:
                        this.RblTypeCorrespondent.Items.FindByValue("IE").Selected = true;
                        this.RblTypeCorrespondent.Enabled = true;
                        this.ChkListType.Items.FindByValue("U").Selected = true;
                        this.ChkListType.Items.FindByValue("R").Selected = true;
                        this.ChkListType.Items.FindByValue("P").Selected = true;
                        this.ChkListType.Items.FindByValue("L").Selected = true;
                        this.ChkListType.Items.FindByValue("F").Selected = false;
                        this.AddressBookChkCommonAddressBook.Visible = this.AddressBookChkCommonAddressBook.Checked = this.EnableCommonAddressBook;
                        //    ibtnMoveToCC.Visible = true;
                        //    dgCC.Visible = true;
                        //    lblCC.Visible = true;
                        //    lblA.Text = "A";
                        //    lblCC.Text = "CC";
                        //    dgCorr.AllowMultipleSelection = true;
                        //    tvOrganigramma.SelectorType = TreeViewSelectorType.CheckBox;
                        //    this.RblTypeCorrespondent.Enabled =  true;
                        //    cb_rf.Visible = true;
                        //    //cb_rf.Checked = true;
                        //    cbListeDist.Visible = true;
                        //    handle_proto_giallo();
                        break;

                    case RubricaCallType.CALLTYPE_PROTO_INT_DEST:
                        //    ibtnMoveToCC.Visible = true;
                        //    dgCC.Visible = true;
                        //    lblCC.Visible = true;
                        //    lblA.Text = "A";
                        //    lblCC.Text = "CC";
                        //    dgCorr.AllowMultipleSelection = true;
                        //    tvOrganigramma.SelectorType = TreeViewSelectorType.CheckBox;
                        if (this.ChkListType.Items.FindByValue("U").Enabled)
                            this.ChkListType.Items.FindByValue("U").Selected = true;
                        if (this.ChkListType.Items.FindByValue("R").Enabled)
                            this.ChkListType.Items.FindByValue("R").Selected = true;
                        if (this.ChkListType.Items.FindByValue("P").Enabled)
                            this.ChkListType.Items.FindByValue("P").Selected = true;
                        if (this.ChkListType.Items.FindByValue("L").Enabled)
                            this.ChkListType.Items.FindByValue("L").Selected = true;
                        this.RblTypeCorrespondent.Items.FindByValue("I").Selected = true;
                        this.RblTypeCorrespondent.Enabled = false;
                        //    //						lblIE.Visible = false;
                        //    cbListeDist.Visible = true;
                        //    //cbListeDist.Checked = true;
                        //    //disabilito nuovo corrisp per i destinatari interni
                        this.AddressBookNew.Enabled = false;
                        //    handle_proto_giallo();

                        this.AddressBookChkRF.Enabled = true;

                        this.AddressBookDDLTypeAOO.Enabled = false;

                        //    //cb_rf.Checked = false;
                        this.AddressBookChkCommonAddressBook.Visible = this.AddressBookChkCommonAddressBook.Checked = false;
                        break;

                    case RubricaCallType.CALLTYPE_TRASM_INF:
                    case RubricaCallType.CALLTYPE_TRASM_SUP:
                    case RubricaCallType.CALLTYPE_TRASM_ALL:
                    case RubricaCallType.CALLTYPE_TRASM_PARILIVELLO:
                        //    ibtnMoveToCC.Visible = false;
                        //    dgCC.Visible = false;
                        //    lblCC.Visible = false;
                        //    lblA.Text = "A";
                        //    dgCorr.AllowMultipleSelection = true;
                        //    tvOrganigramma.SelectorType = TreeViewSelectorType.CheckBox;
                        this.ChkListType.Items.FindByValue("U").Selected = true;
                        this.ChkListType.Items.FindByValue("R").Selected = true;
                        this.ChkListType.Items.FindByValue("P").Selected = true;
                        this.ChkListType.Items.FindByValue("L").Selected = true;
                        this.RblTypeCorrespondent.Items.FindByValue("I").Selected = true;
                        this.RblTypeCorrespondent.Enabled = false;
                        //    //						lblIE.Visible = false;
                        //    //disabilito nuovo corrisp in Nuova Trasmissione 
                        this.AddressBookNew.Enabled = false;
                        //    cbListeDist.Visible = true;
                        //    cb_rf.Visible = true;

                        this.AddressBookDDLTypeAOO.Enabled = false;
                        this.AddressBookNew.Enabled = false;

                        this.AddressBookChkRF.Enabled = true;

                        this.AddressBookChkCommonAddressBook.Visible = this.AddressBookChkCommonAddressBook.Checked = false;
                        break;

                    case RubricaCallType.CALLTYPE_MANAGE:
                        this.AddressBookDownloadTemplate.Visible = true;
                        this.AddressBookExportSearch.Visible = true;
                        if (!UserManager.IsAuthorizedFunctions("EXPORT_RUBRICA"))
                            this.AddressBookExport.Visible = false;
                        else
                            this.AddressBookExport.Visible = true;

                        if (!UserManager.IsAuthorizedFunctions("IMPORT_RUBRICA"))
                            this.AddressBookImport.Visible = false;
                        else
                            this.AddressBookImport.Visible = true;
                        //    //this.btnEsporta.Visible = true;
                        //    //this.btnImporta.Visible = true;
                        //    ibtnMoveToCC.Visible = false;
                        //    ibtnMoveToA.Visible = false;
                        //    dgCC.Visible = false;
                        //    dgA.Visible = false;
                        //    lblCC.Visible = false;
                        //    lblA.Visible = false;
                        //    dgCorr.ShowSelectors = false;
                        //    tvOrganigramma.SelectorType = TreeViewSelectorType.None;
                        this.RblTypeCorrespondent.Items.FindByValue("IE").Selected = true;
                        this.RblTypeCorrespondent.Enabled = false;
                        this.liAddressBookLinkOrg.Visible = false;
                        this.multipleSelection = false;
                        this.UpPnlGridAt.Visible = false;

                        this.GrdCctSelection.Visible = this.ccTableVisible = AddressBookLitCc.Visible = this.LitAddressBookCc.Visible = false;
                        this.GrdAtSelection.Visible = this.AddressBookLitA.Visible = false;
                        this.AaTableVisible = false;

                        //    if (!this.IsPostBack)
                        //    {
                        //        this.ddlIE.SelectedValue = "E";
                        //        this.ddlIE.Items[ddlIE.SelectedIndex].Text = this.ddlIE.Items[ddlIE.SelectedIndex].Text + " Amm.ne";
                        //    }

                        this.AddressBookChkLists.Enabled = true;

                        //    cbListeDist.Checked = false;
                        //    tsMode.Tabs.RemoveAt(1);

                        this.AddressBookChkRF.Enabled = true;

                        this.AddressBookChkRoles.Enabled = true;
                        this.AddressBookChkUsers.Enabled = true;
                        this.AddressBookChkOfficies.Enabled = true;

                        this.AddressBookChkOfficies.Selected = true;
                        this.AddressBookChkUsers.Selected = true;
                        this.AddressBookChkRoles.Selected = true;

                        //    //cb_rf.Checked = false;
                        this.AddressBookChkCommonAddressBook.Visible = this.AddressBookChkCommonAddressBook.Checked = this.EnableCommonAddressBook;
                        break;

                    case RubricaCallType.CALLTYPE_UFFREF_PROTO:
                    case RubricaCallType.CALLTYPE_GESTFASC_LOCFISICA:
                    case RubricaCallType.CALLTYPE_GESTFASC_UFFREF:
                    case RubricaCallType.CALLTYPE_EDITFASC_LOCFISICA:
                    case RubricaCallType.CALLTYPE_EDITFASC_UFFREF:
                    case RubricaCallType.CALLTYPE_NEWFASC_LOCFISICA:
                    case RubricaCallType.CALLTYPE_NEWFASC_UFFREF:
                    case RubricaCallType.CALLTYPE_RICERCA_UFFREF:
                    case RubricaCallType.CALLTYPE_FILTRIRICFASC_UFFREF:
                    case RubricaCallType.CALLTYPE_FILTRIRICFASC_LOCFIS:

                        this.RblTypeCorrespondent.Items.FindByValue("I").Selected = true;
                        this.RblTypeCorrespondent.Enabled = false;

                        this.AddressBookChkLists.Enabled = false;

                        this.AddressBookNew.Enabled = false;

                        this.AddressBookChkRF.Enabled = false;
                        this.AddressBookChkOfficies.Selected = false;
                        this.AddressBookChkOfficies.Selected = true;
                        this.AddressBookChkRoles.Selected = false;
                        this.AddressBookChkUsers.Selected = false;
                        this.AddressBookChkOfficies.Enabled = false;
                        this.AddressBookChkRoles.Enabled = false;
                        this.AddressBookChkUsers.Enabled = false;
                        this.AddressBookChkCommonAddressBook.Visible = false;
                        this.AddressBookDDLTypeAOO.Enabled = false;
                        break;
                    case RubricaCallType.CALLTYPE_STAMPA_REG_UO:
                        this.RblTypeCorrespondent.Items.FindByValue("I").Selected = true;
                        this.RblTypeCorrespondent.Enabled = false;

                        this.AddressBookChkLists.Enabled = false;

                        this.AddressBookNew.Enabled = false;

                        this.AddressBookChkRF.Enabled = false;
                        this.AddressBookChkOfficies.Selected = false;
                        this.AddressBookChkOfficies.Selected = true;
                        this.AddressBookChkRoles.Selected = false;
                        this.AddressBookChkUsers.Selected = false;
                        this.AddressBookChkOfficies.Enabled = false;
                        this.AddressBookChkRoles.Enabled = false;
                        this.AddressBookChkUsers.Enabled = false;
                        this.AddressBookChkCommonAddressBook.Visible = false;
                        this.AddressBookDDLTypeAOO.Enabled = false;
                        this.liAddressBookLinkOrg.Visible = false;
                        this.AddressBookLinkOrg.Visible = false;
                        break;
                    case RubricaCallType.CALLTYPE_PROTO_INGRESSO:


                        this.AddressBookChkLists.Enabled = false;

                        //    handle_proto_giallo();
                        //    cbListeDist.Checked = false;

                        this.AddressBookChkRF.Enabled = false;

                        //    //cb_rf.Checked = false;
                        break;
                    //IACOZZILLI GIORDANO:
                    //Aggiungo la nostra nuova call_type per customizzzare la form.
                    //dopo il collaudo ne creerò una mia.

                    case RubricaCallType.CALLTYPE_DEP_OSITO:
                        this.AddressBookTitleType.Visible = false;
                        this.ChkListType.Visible = false;
                        this.RblTypeCorrespondent.Visible = false;
                        this.AddressBookLblCity.Visible = false;
                        this.TxtCity.Visible = false;
                        this.AddressBookLblCountry.Visible = false;
                        this.TxtCountry.Visible = false;
                        this.AddressBookLblMail.Visible = false;
                        this.TxtMail.Visible = false;
                        this.AddressBookLblNIN.Visible = false;
                        this.TxtNIN.Visible = false;
                        this.AddressBookLblPIVA.Visible = false;
                        this.TxtPIVA.Visible = false;
                        this.AddressBookLinkOrg.Visible = false;
                        this.AddressBookNew.Visible = false;
                        this.RblTypeCorrespondent.Items.FindByValue("IE").Selected = true;
                        this.RblTypeCorrespondent.Enabled = false;
                        this.AddressBookChkRF.Enabled = false;
                        this.AddressBookChkCommonAddressBook.Visible = this.AddressBookChkCommonAddressBook.Checked = this.EnableCommonAddressBook;

                        break;

                    case RubricaCallType.CALLTYPE_OWNER_AUTHOR:
                        this.AddressBookNew.Enabled = false;
                        this.AddressBookChkLists.Enabled = false;
                        this.RblTypeCorrespondent.Items.FindByValue("IE").Selected = true;
                        this.RblTypeCorrespondent.Enabled = false;
                        this.AddressBookChkRF.Enabled = false;
                        this.AddressBookChkCommonAddressBook.Visible = this.AddressBookChkCommonAddressBook.Checked = this.EnableCommonAddressBook;

                        this.liAddressBookLinkOrg.Visible = false;
                        this.AddressBookLinkOrg.Visible = false;

                        break;

                    case RubricaCallType.CALLTYPE_RICERCA_TRASM:
                    case RubricaCallType.CALLTYPE_RICERCA_TRASM_TODOLIST:
                    case RubricaCallType.CALLTYPE_RICERCA_CREATOR:
                    case RubricaCallType.CALLTYPE_RICERCA_UO_RUOLI_SOTTOPOSTI:
                    case RubricaCallType.CALLTYPE_TUTTI_RUOLI:
                    case RubricaCallType.CALLTYPE_TUTTE_UO:
                        //    ibtnMoveToCC.Visible = false;
                        //    dgCC.Visible = false;
                        //    lblCC.Visible = false;
                        //    //disabilito nuovo corrisp in ricerca trasmissioni
                        this.AddressBookNew.Enabled = false;
                        //    lblA.Text = "Da";
                        //    dgCorr.AllowMultipleSelection = false;
                        //    tvOrganigramma.SelectorType = TreeViewSelectorType.RadioButton;
                        //    dgA.PageSize = 1;
                        //    tc.VAlign = "top";

                        this.AddressBookChkLists.Enabled = false;

                        //    cbComandi.Visible = true;
                        //    cbRuoli.Visible = false;
                        //    cbUtenti.Visible = false;
                        //    cbComandi.Visible = false;
                        this.RblTypeCorrespondent.Items.FindByValue("IE").Selected = true;
                        this.RblTypeCorrespondent.Enabled = false;
                        //    tsMode.Tabs.RemoveAt(1);
                        //    tsMode.ClearEvents();
                        //    cbListeDist.Checked = false;

                        this.AddressBookChkRF.Enabled = false;

                        //    //cb_rf.Checked = false;
                        this.AddressBookChkCommonAddressBook.Visible = this.AddressBookChkCommonAddressBook.Checked = this.EnableCommonAddressBook;
                        break;

                    case RubricaCallType.CALLTYPE_LISTE_DISTRIBUZIONE:
                        //    cbListeDist.Checked = false;
                        //    cbListeDist.Visible = false;
                        //    lblCC.Visible = false;
                        //    ibtnMoveToCC.Visible = false;
                        //    phCC.Visible = false;
                        //    lblA.Text = "A";
                        //    btnNuovo.Enabled = false;
                        //    if (Session["AMMDATASET"] != null)
                        this.RblTypeCorrespondent.Items.FindByValue("IE").Selected = true;
                        this.RblTypeCorrespondent.Enabled = true;
                        //    else
                        //    {
                        //        this.RblTypeCorrespondent.Enabled =  true;
                        //        this.ddl_rf.Visible = false;
                        //        this.lbl_registro.Visible = false;
                        //    }
                        this.AddressBookChkCommonAddressBook.Visible = this.AddressBookChkCommonAddressBook.Checked = this.EnableCommonAddressBook;

                        this.AddressBookChkLists.Enabled = false;

                        this.AddressBookChkRF.Enabled = false;

                        break;

                    case RubricaCallType.CALLTYPE_MITT_MODELLO_TRASM:
                        //    cbComandi.Checked = false;
                        //    cbComandi.Enabled = false;
                        //    cbRuoli.Checked = true;
                        //    cbRuoli.Enabled = false;
                        //    cbUtenti.Checked = false;
                        //    cbUtenti.Enabled = false;
                        //    cbListeDist.Checked = false;
                        //    cbListeDist.Visible = false;
                        //    lblCC.Visible = false;
                        //    ibtnMoveToCC.Visible = false;
                        //    phCC.Visible = false;
                        this.RblTypeCorrespondent.Items.FindByValue("IE").Selected = true;
                        this.RblTypeCorrespondent.Enabled = false;
                        //    btnNuovo.Enabled = false;
                        //    dgCorr.AllowMultipleSelection = true;
                        //    tvOrganigramma.SelectorType = TreeViewSelectorType.RadioButton;
                        //    //dgA.PageSize = 1;
                        //    tc.VAlign = "top";

                        this.AddressBookChkLists.Enabled = false;

                        this.AddressBookChkRF.Enabled = false;
                        //    //cb_rf.Checked = false;
                        //    lblA.Text = "DA";
                        break;

                    case RubricaCallType.CALLTYPE_REPLACE_ROLE:
                    case RubricaCallType.CALLTYPE_FIND_ROLE:
                        //    cbComandi.Checked = false;
                        //    cbComandi.Enabled = false;
                        //    cbRuoli.Checked = true;
                        //    cbRuoli.Enabled = false;
                        //    cbUtenti.Checked = false;
                        //    cbUtenti.Enabled = false;
                        //    cbListeDist.Checked = false;
                        //    cbListeDist.Visible = false;
                        //    lblCC.Visible = false;
                        //    ibtnMoveToCC.Visible = false;
                        //    phCC.Visible = false;
                        this.RblTypeCorrespondent.Items.FindByValue("IE").Selected = true;
                        this.RblTypeCorrespondent.Enabled = false;
                        //    btnNuovo.Enabled = false;
                        //    dgCorr.AllowMultipleSelection = false;
                        //    tvOrganigramma.SelectorType = TreeViewSelectorType.RadioButton;
                        //    //dgA.PageSize = 1;
                        //    tc.VAlign = "top";

                        this.AddressBookChkLists.Enabled = false;

                        this.AddressBookChkRF.Enabled = false;

                        this.AddressBookChkCommonAddressBook.Visible = false;

                        this.AddressBookChkOfficies.Selected = false;
                        this.AddressBookChkOfficies.Enabled = false;

                        this.AddressBookChkUsers.Enabled = false;
                        this.AddressBookChkUsers.Selected = false;

                        this.AddressBookChkRoles.Enabled = false;
                        this.AddressBookChkRoles.Selected = true;

                        this.AddressBookNew.Enabled = false;

                        this.liAddressBookLinkOrg.Visible = false;
                        this.AddressBookLinkOrg.Visible = false;

                        //    //cb_rf.Checked = false;
                        //    lblA.Text = "DA";
                        break;

                    case RubricaCallType.CALLTYPE_RUOLO_REG_NOMAIL:
                    case RubricaCallType.CALLTYPE_RUOLO_RESP_REG:
                    case RubricaCallType.CALLTYPE_RUOLO_RESP_REPERTORI:
                        //    cbComandi.Checked = false;
                        //    cbComandi.Enabled = false;
                        //    cbRuoli.Checked = true;
                        //    cbRuoli.Enabled = false;
                        //    cbUtenti.Checked = false;
                        //    cbUtenti.Enabled = false;
                        //    cbListeDist.Checked = false;
                        //    cbListeDist.Visible = false;
                        //    lblCC.Visible = false;
                        //    ibtnMoveToCC.Visible = false;
                        //    phCC.Visible = false;
                        this.RblTypeCorrespondent.Items.FindByValue("IE").Selected = true;
                        this.RblTypeCorrespondent.Enabled = false;
                        //    dgCorr.AllowMultipleSelection = false;
                        //    tvOrganigramma.SelectorType = TreeViewSelectorType.RadioButton;
                        //    dgA.PageSize = 1;
                        //    tc.VAlign = "top";

                        this.AddressBookChkLists.Enabled = false;

                        this.AddressBookChkRF.Enabled = false;

                        //    //cb_rf.Checked = false;
                        //    lblA.Text = "DA";
                        break;

                    case RubricaCallType.CALLTYPE_UTENTE_REG_NOMAIL:
                        //    cbComandi.Checked = false;
                        //    cbComandi.Enabled = false;
                        //    cbRuoli.Checked = false;
                        //    cbRuoli.Enabled = false;
                        //    cbUtenti.Checked = true;
                        //    cbUtenti.Enabled = false;
                        //    cbListeDist.Checked = false;
                        //    cbListeDist.Visible = false;
                        //    lblCC.Visible = false;
                        //    ibtnMoveToCC.Visible = false;
                        //    phCC.Visible = false;
                        this.RblTypeCorrespondent.Items.FindByValue("IE").Selected = true;
                        this.RblTypeCorrespondent.Enabled = false;
                        //    dgCorr.AllowMultipleSelection = false;
                        //    tvOrganigramma.SelectorType = TreeViewSelectorType.RadioButton;
                        //    dgA.PageSize = 1;
                        //    tc.VAlign = "top";

                        this.AddressBookChkLists.Enabled = false;

                        this.AddressBookChkRF.Enabled = false;

                        //    //cb_rf.Checked = false;
                        //    lblA.Text = "DA";
                        break;

                    case RubricaCallType.CALLTYPE_MODELLI_TRASM_INF:
                    case RubricaCallType.CALLTYPE_MODELLI_TRASM_SUP:
                    case RubricaCallType.CALLTYPE_MODELLI_TRASM_ALL:
                    case RubricaCallType.CALLTYPE_MODELLI_TRASM_PARILIVELLO:
                        //    //						cbComandi.Checked = false;
                        //    //						cbComandi.Enabled = false;
                        //    //						cbRuoli.Checked = true;
                        //    //						cbRuoli.Enabled = true;
                        //    //						cbUtenti.Checked = true;
                        //    //						cbUtenti.Enabled = true;
                        //    cbListeDist.Visible = true;
                        //    lblCC.Visible = false;
                        //    ibtnMoveToCC.Visible = false;
                        //    phCC.Visible = false;
                        this.RblTypeCorrespondent.Items.FindByValue("IE").Selected = true;
                        this.RblTypeCorrespondent.Enabled = false;
                        //    btnNuovo.Enabled = false;
                        //    dgCorr.AllowMultipleSelection = true;
                        //    tvOrganigramma.SelectorType = TreeViewSelectorType.CheckBox;
                        //    //						dgA.PageSize = 1;
                        //    tc.VAlign = "top";

                        this.AddressBookChkLists.Enabled = false;

                        this.AddressBookChkRF.Enabled = false;

                        break;

                    case RubricaCallType.CALLTYPE_DEST_MODELLO_TRASM:
                        //    cbListeDist.Checked = true;
                        //    cbListeDist.Visible = true;
                        //    lblCC.Visible = false;
                        //    ibtnMoveToCC.Visible = false;
                        //    phCC.Visible = false;
                        this.RblTypeCorrespondent.Items.FindByValue("IE").Selected = true;
                        this.RblTypeCorrespondent.Enabled = false;
                        //    btnNuovo.Enabled = false;
                        //    dgCorr.AllowMultipleSelection = true;
                        //    tvOrganigramma.SelectorType = TreeViewSelectorType.CheckBox;
                        //    tc.VAlign = "top";

                        this.AddressBookChkLists.Enabled = false;

                        this.AddressBookChkRF.Enabled = false;

                        //    //cb_rf.Checked = false;
                        //    lblA.Text = "A";
                        this.AddressBookChkCommonAddressBook.Visible = this.AddressBookChkCommonAddressBook.Checked = this.EnableCommonAddressBook;
                        break;

                    case RubricaCallType.CALLTYPE_CORR_INT:
                        //    cbListeDist.Checked = false;
                        //    cbListeDist.Visible = false;
                        //    lblCC.Visible = false;
                        //    ibtnMoveToCC.Visible = false;
                        //    phCC.Visible = false;
                        this.RblTypeCorrespondent.Items.FindByValue("I").Selected = true;
                        this.RblTypeCorrespondent.Enabled = false;
                        //    dgCorr.AllowMultipleSelection = false;
                        //    tvOrganigramma.SelectorType = TreeViewSelectorType.CheckBox;
                        //    dgA.PageSize = 1;
                        //    tc.VAlign = "top";

                        this.AddressBookChkLists.Enabled = false;

                        this.AddressBookChkRF.Enabled = false;

                        //    //cb_rf.Checked = false;
                        this.AddressBookChkCommonAddressBook.Visible = this.AddressBookChkCommonAddressBook.Checked = false;
                        break;

                    case RubricaCallType.CALLTYPE_CORR_EST:
                    case RubricaCallType.CALLTYPE_CORR_EST_CON_DISABILITATI:
                        //    cbListeDist.Checked = false;
                        //    cbListeDist.Visible = false;
                        //    lblCC.Visible = false;
                        //    ibtnMoveToCC.Visible = false;
                        //    phCC.Visible = false;
                        this.RblTypeCorrespondent.Items.FindByValue("E").Selected = true;
                        this.RblTypeCorrespondent.Enabled = false;
                        //    dgCorr.AllowMultipleSelection = false;
                        //    tvOrganigramma.SelectorType = TreeViewSelectorType.CheckBox;
                        //    dgA.PageSize = 1;
                        //    tc.VAlign = "top";

                        this.AddressBookChkLists.Enabled = false;

                        this.AddressBookChkRF.Enabled = false;

                        //    //cb_rf.Checked = false;
                        this.AddressBookChkCommonAddressBook.Visible = this.AddressBookChkCommonAddressBook.Checked = this.EnableCommonAddressBook;
                        break;

                    case RubricaCallType.CALLTYPE_CORR_INT_EST:
                    case RubricaCallType.CALLTYPE_CORR_INT_EST_CON_DISABILITATI:
                    case RubricaCallType.CALLTYPE_CORR_INT_CON_DISABILITATI:
                        //    cbListeDist.Checked = false;
                        //    cbListeDist.Visible = false;
                        //    lblCC.Visible = false;
                        //    ibtnMoveToCC.Visible = false;
                        //    phCC.Visible = false;
                        this.RblTypeCorrespondent.Items.FindByValue("IE").Selected = true;
                        this.RblTypeCorrespondent.Enabled = false;
                        //    dgCorr.AllowMultipleSelection = false;
                        //    tvOrganigramma.SelectorType = TreeViewSelectorType.CheckBox;
                        //    dgA.PageSize = 1;
                        //    tc.VAlign = "top";

                        this.AddressBookChkLists.Enabled = false;

                        this.AddressBookChkRF.Enabled = false;

                        //    //cb_rf.Checked = false;
                        this.AddressBookChkCommonAddressBook.Visible = this.AddressBookChkCommonAddressBook.Checked = this.EnableCommonAddressBook;
                        break;

                    case RubricaCallType.CALLTYPE_CORR_NO_FILTRI:
                    case RubricaCallType.CALLTYPE_RICERCA_CORR_NON_STORICIZZATO:
                        //    cbListeDist.Checked = false;
                        //    cbListeDist.Visible = false;
                        //    lblCC.Visible = false;
                        //    ibtnMoveToCC.Visible = false;
                        //    phCC.Visible = false;
                        //    dgCorr.AllowMultipleSelection = false;
                        //    tvOrganigramma.SelectorType = TreeViewSelectorType.CheckBox;
                        //    dgA.PageSize = 1;
                        //    tc.VAlign = "top";

                        this.AddressBookChkLists.Enabled = false;

                        this.AddressBookChkRF.Enabled = false;

                        //    //cb_rf.Checked = false;
                        this.AddressBookChkCommonAddressBook.Visible = this.AddressBookChkCommonAddressBook.Checked = this.EnableCommonAddressBook;
                        break;
                    case RubricaCallType.CALLTYPE_MITT_MULTIPLI:
                    case RubricaCallType.CALLTYPE_MITT_MULTIPLI_SEMPLIFICATO:
                        //    lblA.Text = "Da";
                        //    ibtnMoveToCC.Visible = false;
                        //    dgCC.Visible = false;
                        //    lblCC.Visible = false;
                        //    dgCorr.AllowMultipleSelection = true;
                        //    tvOrganigramma.SelectorType = TreeViewSelectorType.CheckBox;
                        //    this.RblTypeCorrespondent.Enabled =  true;

                        this.AddressBookChkRF.Enabled = true;
                        this.RblTypeCorrespondent.Items.FindByValue("E").Selected = true;
                        this.AddressBookChkCommonAddressBook.Visible = this.AddressBookChkCommonAddressBook.Checked = this.EnableCommonAddressBook;
                        //    //handle_proto_giallo();
                        break;

                    case RubricaCallType.CALLTYPE_CORR_INT_NO_UO:
                        //    cbListeDist.Checked = false;
                        //    cbListeDist.Visible = false;
                        //    lblCC.Visible = false;
                        //    ibtnMoveToCC.Visible = false;
                        //    phCC.Visible = false;
                        this.RblTypeCorrespondent.Items.FindByValue("I").Selected = true;
                        this.RblTypeCorrespondent.Enabled = false;
                        //    cbComandi.Visible = false;
                        //    dgCorr.AllowMultipleSelection = false;
                        //    tvOrganigramma.SelectorType = TreeViewSelectorType.CheckBox;
                        //    dgA.PageSize = 1;
                        //    tc.VAlign = "top";

                        this.AddressBookChkLists.Enabled = false;

                        this.AddressBookChkRF.Enabled = false;

                        this.AddressBookChkOfficies.Enabled = false;

                        this.AddressBookChkOfficies.Selected = false;

                        this.AddressBookDDLTypeAOO.Enabled = false;

                        this.AddressBookNew.Enabled = false;

                        //    //cb_rf.Checked = false;
                        this.AddressBookChkCommonAddressBook.Visible = this.AddressBookChkCommonAddressBook.Checked = false;
                        break;

                    case RubricaCallType.CALLTYPE_RICERCA_TRASM_SOTTOPOSTO:

                        this.AddressBookChkUsers.Enabled = false;
                        this.AddressBookChkUsers.Selected = false;

                        this.AddressBookChkRoles.Enabled = false;
                        this.AddressBookChkRoles.Selected = true;

                        this.AddressBookChkOfficies.Enabled = false;
                        this.AddressBookChkOfficies.Selected = false;

                        this.AddressBookDDLTypeAOO.Enabled = false;

                        this.AddressBookChkRF.Selected = false;
                        this.AddressBookChkRF.Enabled = false;

                        this.AddressBookChkLists.Selected = false;
                        this.AddressBookChkLists.Enabled = false;

                        this.AddressBookChkCommonAddressBook.Checked = false;
                        this.AddressBookChkCommonAddressBook.Visible = false;

                        this.AddressBookNew.Enabled = false;

                        this.RblTypeCorrespondent.Enabled = false;

                        this.RblTypeCorrespondent.SelectedValue = "I";

                        this.liAddressBookLinkOrg.Visible = false;
                        break;
                }
                #endregion
            }
        }

        private void setFormContent()
        {
            if (HttpContext.Current.Session["AddressBook.from"] != null)
            {
                switch (HttpContext.Current.Session["AddressBook.from"].ToString())
                {
                    case "M_D_T_R":
                    case "M_D_F_R":
                    case "DISTRIBUTIONLIST":
                        this.multipleSelection = true;
                        this.GrdCctSelection.Visible = this.ccTableVisible = AddressBookLitCc.Visible = this.LitAddressBookCc.Visible = false;
                        this.AddressBookLitA.Text = destinationList_atText;
                        break;
                    case "G_M_T_I":
                        this.multipleSelection = true;
                        this.GrdCctSelection.Visible = this.ccTableVisible = AddressBookLitCc.Visible = this.LitAddressBookCc.Visible = false;
                        this.AddressBookLitA.Text = destinationList_atText;
                        this.AddressBookChkCommonAddressBook.Visible = false;
                        break;
                    case "D_R_X_S":
                        this.multipleSelection = false;
                        this.GrdCctSelection.Visible = this.ccTableVisible = AddressBookLitCc.Visible = this.LitAddressBookCc.Visible = false;
                        this.AddressBookLitA.Text = string.Empty;
                        break;
                    case "F_X_X_S":
                    case "F_X_X_S_4":
                    case "F_X_X_S_5":
                    case "F_X_X_S_2":
                    case "F_X_X_S_3":
                    case "M_D_T_M":
                        this.multipleSelection = false;
                        this.GrdCctSelection.Visible = this.ccTableVisible = AddressBookLitCc.Visible = this.LitAddressBookCc.Visible = false;
                        this.AddressBookLitA.Text = string.Empty;
                        if (HttpContext.Current.Session["AddressBook.from"].ToString() == "F_X_X_S_3")
                        {
                            this.AddressBookChkCommonAddressBook.Visible = false;
                        }
                        if (HttpContext.Current.Session["AddressBook.EnableOnly"] != null)
                        {
                            foreach (ListItem item in this.ChkListType.Items)
                            {
                                item.Enabled = false;
                                item.Selected = false;
                            }
                            this.ChkListType.Items.FindByValue(HttpContext.Current.Session["AddressBook.EnableOnly"].ToString()).Enabled = true;
                            this.ChkListType.Items.FindByValue(HttpContext.Current.Session["AddressBook.EnableOnly"].ToString()).Selected = true;
                        }
                        this.AddressBookChkCommonAddressBook.Visible = false;
                        this.AddressBookLitA.Text = destinationList_fromText;
                        break;
                    case "D_A_S_S":
                    case "D_I_S_S":
                    case "D_P_S_S":
                    case "CUSTOM":
                        this.AddressBookLitA.Text = destinationList_atText;
                        this.multipleSelection = false;
                        this.GrdCctSelection.Visible = this.ccTableVisible = AddressBookLitCc.Visible = this.LitAddressBookCc.Visible = false;
                        break;
                    case "COLLOCATION":
                    //Laura 19 Marzo
                    case "SP_CREATOR":
                        this.multipleSelection = false;
                        this.GrdCctSelection.Visible = this.ccTableVisible = AddressBookLitCc.Visible = this.LitAddressBookCc.Visible = false;
                        this.AddressBookLitA.Text = destinationList_fromText;
                        break;
                    case "T_N_R_S":
                        this.multipleSelection = true;
                        this.GrdCctSelection.Visible = this.ccTableVisible = AddressBookLitCc.Visible = this.LitAddressBookCc.Visible = false;
                        this.AddressBookLitA.Text = destinationList_atText;
                        break;
                    case "V_U_R_S":
                    case "V_R_R_S":
                    case "T_S_R_S":
                    case "T_S_R_S_2":
                    case "T_S_R_S_3":
                        this.multipleSelection = false;
                        this.GrdCctSelection.Visible = this.ccTableVisible = AddressBookLitCc.Visible = this.LitAddressBookCc.Visible = false;
                        this.AddressBookLitA.Text = destinationList_selectText;
                        if (HttpContext.Current.Session["AddressBook.EnableOnly"] != null)
                        {
                            foreach (ListItem item in this.ChkListType.Items)
                            {
                                item.Enabled = false;
                                item.Selected = false;
                            }
                            this.ChkListType.Items.FindByValue(HttpContext.Current.Session["AddressBook.EnableOnly"].ToString()).Enabled = true;
                            this.ChkListType.Items.FindByValue(HttpContext.Current.Session["AddressBook.EnableOnly"].ToString()).Selected = true;
                        }
                        break;

                    case "D_A_S_M":
                    case "D_I_S_M":
                    case "D_P_S_M":
                        this.multipleSelection = true;
                        this.GrdCctSelection.Visible = this.ccTableVisible = AddressBookLitCc.Visible = this.LitAddressBookCc.Visible = false;
                        this.AddressBookLitA.Text = destinationList_fromText;
                        break;

                    case "D_A_R_S":
                    case "D_I_R_S":
                    case "D_P_R_S":
                        this.multipleSelection = true;
                        this.GrdCctSelection.Visible = this.ccTableVisible = AddressBookLitCc.Visible = this.LitAddressBookCc.Visible = false;
                        this.AddressBookLitA.Text = destinationList_atText;
                        break;

                    case "D_A_R_M":
                    case "D_I_R_M":
                    case "D_P_R_M":
                        this.multipleSelection = true;
                        this.GrdCctSelection.Visible = this.ccTableVisible = AddressBookLitCc.Visible = this.LitAddressBookCc.Visible = true;
                        this.AddressBookLitA.Text = destinationList_atText;
                        break;
                    case "STAMPAREPORTUO":
                        this.multipleSelection = false;
                        this.GrdCctSelection.Visible = this.ccTableVisible = AddressBookLitCc.Visible = this.LitAddressBookCc.Visible = false;
                        this.AddressBookLitA.Text = destinationList_atText;
                        break;
                    case "VISIBILITY_SIGNATURE_PROCESS":
                    case "FILTER_VISIBILITY_SIGNATURE_PROCESS":
                    case "COPIA_PROCESSO":
                        this.multipleSelection = true;
                        this.GrdCctSelection.Visible = this.ccTableVisible = AddressBookLitCc.Visible = this.LitAddressBookCc.Visible = false;
                        this.AddressBookLitA.Text = destinationList_atText;
                        if (HttpContext.Current.Session["AddressBook.EnableOnly"] != null)
                        {
                            foreach (ListItem item in this.ChkListType.Items)
                            {
                                item.Enabled = false;
                                item.Selected = false;
                            }
                            this.ChkListType.Items.FindByValue(HttpContext.Current.Session["AddressBook.EnableOnly"].ToString()).Enabled = true;
                            this.ChkListType.Items.FindByValue(HttpContext.Current.Session["AddressBook.EnableOnly"].ToString()).Selected = true;
                        }
                        break;

                    case "SIGNATURE_PROCESS":
                    case "START_SIGNATURE_PROCESS":
                    case "FILTER_SIGNATURE_PROCESS_ROLE":
                    case "FILTER_SIGNATURE_PROCESS_USER":
                    case "FORMAZIONE":
                        this.multipleSelection = false;
                        this.GrdCctSelection.Visible = this.ccTableVisible = AddressBookLitCc.Visible = this.LitAddressBookCc.Visible = false;
                        this.AddressBookLitA.Text = destinationList_atText;
                        if (HttpContext.Current.Session["AddressBook.EnableOnly"] != null)
                        {
                            foreach (ListItem item in this.ChkListType.Items)
                            {
                                item.Enabled = false;
                                item.Selected = false;
                            }
                            this.ChkListType.Items.FindByValue(HttpContext.Current.Session["AddressBook.EnableOnly"].ToString()).Enabled = true;
                            this.ChkListType.Items.FindByValue(HttpContext.Current.Session["AddressBook.EnableOnly"].ToString()).Selected = true;
                        }
                        break;
                }

                switch (HttpContext.Current.Session["AddressBook.from"].ToString())
                {
                    case "D_A_S_S":
                    case "D_I_S_S":
                    case "D_P_S_S":
                    case "D_A_S_M":
                    case "D_I_S_M":
                    case "D_P_S_M":
                        this.AddressBookLitA.Text = destinationList_fromText;
                        break;
                }
            }
            else
            {
                this.multipleSelection = true;
                this.GrdCctSelection.Visible = this.ccTableVisible = AddressBookLitCc.Visible = this.LitAddressBookCc.Visible = true;
            }
        }

        protected void InitializeLanguage()
        {
            this.AddressBookLitCc.Text = Utils.Languages.GetLabelFromCode("AddressBookLitCc", language);
            this.AddressBookSearch.Text = Utils.Languages.GetLabelFromCode("AddressBookSearch", language);
            this.AddressBookSave.Text = Utils.Languages.GetLabelFromCode("AddressBookSave", language);
            this.AddressBookNew.Text = Utils.Languages.GetLabelFromCode("AddressBookNew", language);
            this.AddressBookImport.Text = Utils.Languages.GetLabelFromCode("AddressBookImport", language);
            this.AddressBookExport.Text = Utils.Languages.GetLabelFromCode("AddressBookExport", language);
            this.AddressBookBtnClose.Text = Utils.Languages.GetLabelFromCode("AddressBookBtnClose", language);
            this.AddressBookChkOfficies.Text = Utils.Languages.GetLabelFromCode("AddressBookChkOfficies", language);
            this.AddressBookChkRoles.Text = Utils.Languages.GetLabelFromCode("AddressBookChkRoles", language);
            this.AddressBookChkUsers.Text = Utils.Languages.GetLabelFromCode("AddressBookChkUsers", language);
            this.AddressBookChkLists.Text = Utils.Languages.GetLabelFromCode("AddressBookChkLists", language);
            this.AddressBookChkRF.Text = Utils.Languages.GetLabelFromCode("AddressBookChkRF", language);
            this.AddressBookRadioTypeAll.Text = Utils.Languages.GetLabelFromCode("AddressBookRadioTypeAll", language);
            this.AddressBookRadioExternal.Text = Utils.Languages.GetLabelFromCode("AddressBookRadioExternal", language);
            this.AddressBookRadioInternal.Text = Utils.Languages.GetLabelFromCode("AddressBookRadioInternal", language);
            this.AddressBookBtnInitialize.AlternateText = Utils.Languages.GetLabelFromCode("AddressBookBtnInitialize", language);
            this.AddressBookBtnInitialize.ToolTip = Utils.Languages.GetLabelFromCode("AddressBookBtnInitialize", language);
            this.AddressBookTitleType.Text = Utils.Languages.GetLabelFromCode("AddressBookTitleType", language);
            this.AddressBookLitAddress.Text = Utils.Languages.GetLabelFromCode("AddressBookLitAddress", language);
            this.AddressBookLblCode.Text = Utils.Languages.GetLabelFromCode("AddressBookLblCode", language);
            this.AddressBookDDLTypeAOO.Attributes.Add("data-placeholder", Utils.Languages.GetLabelFromCode("AddressBookDDLTypeAOO", language));
            this.AddressBookLblDescription.Text = Utils.Languages.GetLabelFromCode("AddressBookLblDescription", language);
            this.AddressBookLblCity.Text = Utils.Languages.GetLabelFromCode("AddressBookLblCity", language);
            this.AddressBookLblCountry.Text = Utils.Languages.GetLabelFromCode("AddressBookLblCountry", language);
            this.AddressBookLblMail.Text = Utils.Languages.GetLabelFromCode("AddressBookLblMail", language);
            this.AddressBookLblNIN.Text = Utils.Languages.GetLabelFromCode("AddressBookLblNIN", language);
            this.AddressBookLinkList.Text = Utils.Languages.GetLabelFromCode("AddressBookLinkList", language);
            this.AddressBookLinkOrg.Text = Utils.Languages.GetLabelFromCode("AddressBookLinkOrg", language);
            this.AddressBookChkCommonAddressBook.Text = Utils.Languages.GetLabelFromCode("AddressBookChkCommonAddressBook", language);
            this.AddressBookLblPIVA.Text = Utils.Languages.GetLabelFromCode("AddressBookLblPIVA", language);

            this.GrdAddressBookResult.Columns[1].HeaderText = Utils.Languages.GetLabelFromCode("AddressBookResult_Type", language);
            this.GrdAddressBookResult.Columns[2].HeaderText = Utils.Languages.GetLabelFromCode("AddressBookResult_Description", language);
            this.GrdAddressBookResult.Columns[3].HeaderText = Utils.Languages.GetLabelFromCode("AddressBookResult_Canal", language);
            this.GrdAddressBookResult.Columns[4].HeaderText = Utils.Languages.GetLabelFromCode("AddressBookResult_Book", language);

            this.GrdAtSelection.Columns[1].HeaderText = Utils.Languages.GetLabelFromCode("AddressBookResult_Type", language);
            this.GrdAtSelection.Columns[2].HeaderText = Utils.Languages.GetLabelFromCode("AddressBookResult_Description", language);
            this.GrdAtSelection.Columns[3].HeaderText = Utils.Languages.GetLabelFromCode("AddressBookResult_Canal", language);
            this.GrdAtSelection.Columns[4].HeaderText = Utils.Languages.GetLabelFromCode("AddressBookResult_Book", language);

            this.GrdCctSelection.Columns[1].HeaderText = Utils.Languages.GetLabelFromCode("AddressBookResult_Type", language);
            this.GrdCctSelection.Columns[2].HeaderText = Utils.Languages.GetLabelFromCode("AddressBookResult_Description", language);
            this.GrdCctSelection.Columns[3].HeaderText = Utils.Languages.GetLabelFromCode("AddressBookResult_Canal", language);
            this.GrdCctSelection.Columns[4].HeaderText = Utils.Languages.GetLabelFromCode("AddressBookResult_Book", language);

            this.AddressBookOrgTitle.Text = Utils.Languages.GetLabelFromCode("AddressBookOrgTitle", language);
            this.AddressBook_Details.Title = Utils.Languages.GetLabelFromCode("AddressBook_Details", language);
            this.AddressBook_New.Title = Utils.Languages.GetLabelFromCode("AddressBook_New", language);
            this.ExportDati.Title = utils.FormatJs(Utils.Languages.GetLabelFromCode("AddressBookExportDatiTitleAddressbook", language));
            this.ExportSearch.Title = utils.FormatJs(Utils.Languages.GetLabelFromCode("AddressBookExportSearchTitleAddressbook", language));
            this.ImportDati.Title = utils.FormatJs(Utils.Languages.GetLabelFromCode("AddressBookImportDatiTitle", language));
            this.AddressBookDownloadTemplate.Text = Utils.Languages.GetLabelFromCode("AddressBookDownloadTemplate", language);
            this.AddressBookExportSearch.Text = Utils.Languages.GetLabelFromCode("AddressBookExportSearch", language);
        }

        protected string GetLabelDetails()
        {
            return Utils.Languages.GetLabelFromCode("AddressBookGetLabelDetails", this.language);
        }

        protected string GetLabelDetailsRemove()
        {
            return Utils.Languages.GetLabelFromCode("AddressBookGetLabelRemove", this.language);
        }

        protected string GetLabelDetailsRemoveAll()
        {
            return Utils.Languages.GetLabelFromCode("AddressBookGetLabelRemoveAll", this.language);
        }

        protected string GetLabelDetailsAddAll()
        {
            return Utils.Languages.GetLabelFromCode("AddressBookGetLabelAddAll", this.language);
        }

        protected string GetLabelDetailsImgType()
        {
            return Utils.Languages.GetLabelFromCode("AddressBookGetLabelImgType", this.language);
        }

        public string GetLabelDetailsAddCC(string id)
        {
            string retVal = string.Empty;

            if (id.Equals(IMG_CC_UNSELECT))
            {
                retVal = Utils.Languages.GetLabelFromCode("AddressBookGetLabelAddCC", this.language);
            }

            if (id.Equals(IMG_CC_SELECT))
            {
                retVal = Utils.Languages.GetLabelFromCode("AddressBookGetLabelRemoveCC", this.language);
            }

            return retVal;
        }

        protected string GetImgDetailsAALL()
        {
            string result = string.Empty;
            if (HttpContext.Current.Session["AddressBook.from"] != null)
            {
                switch (HttpContext.Current.Session["AddressBook.from"].ToString())
                {
                    case "D_A_S_S":
                    case "D_I_S_S":
                    case "D_P_S_S":
                    case "D_A_S_M":
                    case "D_I_S_M":
                    case "D_P_S_M":
                    case "SP_CREATOR":
                    case "F_X_X_S":
                    case "F_X_X_S_5":
                    case "F_X_X_S_4" :
                        result = IMG_ALL_DA_ADD;
                        break;
                    default:
                        result = IMG_ALL_A_ADD;
                        break;
                }
            }
            return result;
        }

        public bool GetEnableImg(string img)
        {
            bool retVal = false;
            string imgUrl1 = "../Images/Icons/uo_icon_2.png";
            string imgUrl2 = "../Images/Icons/user_icon2.png";
            string imgUrl3 = "../Images/Icons/role_icon_2.png";
            if (img.Equals(imgUrl1) || img.Equals(imgUrl2) || img.Equals(imgUrl3))
            {
                retVal = true;
            }
            return retVal;
        }

        public string GetLabelDetailsAddA(string id)
        {
            string retVal = string.Empty;

            if (id.Equals(IMG_AT_UNSELECT))
            {
                retVal = Utils.Languages.GetLabelFromCode("AddressBookGetLabelAdd", this.language);
            }

            if (id.Equals(IMG_AT_SELECT))
            {
                retVal = Utils.Languages.GetLabelFromCode("AddressBookGetLabelRemoveA", this.language);
            }

            return retVal;
        }

        protected void AddressBookBtnClose_Click(object sender, EventArgs e)
        {
            try
            {
                resetAllSessionElement();
                string parent = string.Empty;
                if (this.AddDoc || this.FromMassive || this.ManagementTransmission || this.FromInstanceAccess || this.FromNewProject)
                {
                    parent = ",parent";
                }
                if (this.CallType == RubricaCallType.CALLTYPE_MANAGE)
                {
                    ScriptManager.RegisterClientScriptBlock(this.UpPnlButtons, this.UpPnlButtons.GetType(), "closeAJM", "parent.closeAjaxModal('ManageAddressBook',''" + parent + ");", true);
                }
                else if (HttpContext.Current.Session["AddressBook.from"].ToString().Equals("START_SIGNATURE_PROCESS"))
                {
                    ScriptManager.RegisterClientScriptBlock(this.UpPnlButtons, this.UpPnlButtons.GetType(), "closeAJM", "parent.closeAjaxModal('AddressBookFromPopup',''" + parent + ");", true);
                }
                else if (HttpContext.Current.Session["AddressBook.from"].ToString().Equals("FORMAZIONE"))
                {
                    ScriptManager.RegisterClientScriptBlock(this.UpPnlButtons, this.UpPnlButtons.GetType(), "closeAJM", "parent.closeAjaxModal('FormazioneAddressBook',''" + parent + ");", true);

                }
                else
                {
                    ScriptManager.RegisterClientScriptBlock(this.UpPnlButtons, this.UpPnlButtons.GetType(), "closeAJM", "parent.closeAjaxModal('AddressBook',''" + parent + ");", true);
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void AddressBookBtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                HttpContext.Current.Session["AddressBook.corrsFound"] = null;
                string parent = string.Empty;
                if (this.AddDoc || this.FromMassive || this.ManagementTransmission || this.FromInstanceAccess || FromNewProject)
                {
                    parent = ",parent";
                }
                if (HttpContext.Current.Session["AddressBook.from"].ToString().Equals("START_SIGNATURE_PROCESS"))
                {
                    ScriptManager.RegisterClientScriptBlock(this.UpPnlButtons, this.UpPnlButtons.GetType(), "closeAJM", "parent.closeAjaxModal('AddressBookFromPopup','up'" + parent + ");", true);
                }
                if (HttpContext.Current.Session["AddressBook.from"].ToString().Equals("FORMAZIONE"))
                {
                    ScriptManager.RegisterClientScriptBlock(this.UpPnlButtons, this.UpPnlButtons.GetType(), "closeAJM", "parent.closeAjaxModal('FormazioneAddressBook','up'" + parent + ");", true);
                }
                else
                {
                    ScriptManager.RegisterClientScriptBlock(this.UpPnlButtons, this.UpPnlButtons.GetType(), "closeAJM", "parent.closeAjaxModal('AddressBook','up'" + parent + ");", true);
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private void resetAllSessionElement()
        {
            HttpContext.Current.Session["AddressBook.corrsFound"] = null;
            HttpContext.Current.Session["AddressBook.At"] = null;
            HttpContext.Current.Session["AddressBook.Cc"] = null;
        }

        protected void AddressBookLinkList_Click(object sender, EventArgs e)
        {
            try
            {
                this.AddressBookBtnInitialize.Enabled = true;
                this.IsOrganigramMode = false;
                this.IsTotalOrganigram = true;
                if (this.UpPnlTreeResult.Visible)
                {
                    AddressBookTreeView.Nodes.Clear();
                    if (!string.IsNullOrEmpty(this.hdnOldSelectedCorrType.Value))
                    {
                        this.RblTypeCorrespondent.SelectedValue = this.hdnOldSelectedCorrType.Value;
                        this.RblTypeCorrespondent.Enabled = true;
                    }
                    this.showTreeView(false);

                    this.liAddressBookLinkList.Attributes.Remove("class");
                    this.liAddressBookLinkList.Attributes.Add("class", "addressTab");
                    this.liAddressBookLinkOrg.Attributes.Remove("class");
                    this.liAddressBookLinkOrg.Attributes.Add("class", "otherAddressTab");
                    this.UpTypeResult.Update();
                    this.AddressBookSearch.Enabled = true;
                    this.AddressBookSearch.Focus();
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "functionKeyPress", "<script>$(function() {$('.defaultAction').keypress(function(e) {if(e.which == 13) {e.preventDefault();$('#AddressBookSearch').click();}});});</script>", false);
                    this.UpPnlButtons.Update();
                }

                this.UpdatePanelTop.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void AddressBookLinkOrg_Click(object sender, EventArgs e)
        {
            try
            {
                //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "<script>reallowOp();</script>", false);
                this.IsOrganigramMode = true;
                this.AddressBookBtnInitialize.Enabled = false;

                if (!this.UpPnlTreeResult.Visible)
                {

                    if (this.RblTypeCorrespondent.Enabled)
                    {
                        this.hdnOldSelectedCorrType.Value = this.RblTypeCorrespondent.SelectedValue;
                        this.RblTypeCorrespondent.SelectedValue = "I";
                        this.RblTypeCorrespondent.Enabled = false;
                        UpdatePanelTop.Update();
                    }

                    if (this.RblTypeCorrespondent.SelectedValue == "I")
                    {
                        this.liAddressBookLinkList.Attributes.Remove("class");
                        this.liAddressBookLinkList.Attributes.Add("class", "otherAddressTab");
                        this.liAddressBookLinkOrg.Attributes.Remove("class");
                        this.liAddressBookLinkOrg.Attributes.Add("class", "addressTab");
                        this.UpTypeResult.Update();

                        //this.AddressBookSearch.Enabled = false;
                        this.UpPnlButtons.Update();

                        this.loadTreeView();
                        this.showTreeView(true);
                    }

                    if (string.IsNullOrEmpty(this.RblTypeCorrespondent.SelectedValue) || (!this.RblTypeCorrespondent.Enabled && this.RblTypeCorrespondent.SelectedValue != "I"))
                    {
                        string msg = "WarningOrgAddressBook";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", "\\'") + "', 'warning');} else {parent.ajaxDialogModal('" + msg.Replace("'", "\\'") + "', 'warning');}", true);
                    }
                }
                UpdatePanelTop.Update();
                this.AddressBookSearch.Enabled = true;

                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "functionKeyPress", "<script>$(function() {$('.defaultAction').keypress(function(e) {if(e.which == 13) {e.preventDefault();$('#AddressBookSearch').click();}});});</script>", false);
                this.UpPnlButtons.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private void showTreeView(bool show)
        {
            this.PnlTreeView.Visible = show;
            this.UpPnlTreeResult.Visible = show;
            this.UpPnlTreeResult.Update();

            this.UpPnlGridResult.Visible = !show;
            this.UpPnlGridResult.Update();

            this.UpdatePanelDxContent.Update();
        }

        protected void viewInTreeView_Click(object sender, EventArgs e)
        {
            try
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);
                List<CorrespondentDetail> correspondentsFound = (List<CorrespondentDetail>)HttpContext.Current.Session["AddressBook.corrsFound"];
                string item_systemId = ((CustomImageButton)sender).Attributes["RowValue"];
                int i = correspondentsFound.FindIndex(o => o.SystemID == item_systemId);
                if (i != -1)
                {
                    CorrespondentDetail corrForLoading = correspondentsFound.ElementAt(i);
                    if (!corrForLoading.isRubricaComune && corrForLoading.EI_Type == "I")
                    {
                        if (this.RblTypeCorrespondent.Enabled)
                        {
                            this.hdnOldSelectedCorrType.Value = this.RblTypeCorrespondent.SelectedValue;
                            this.RblTypeCorrespondent.SelectedValue = "I";
                            this.RblTypeCorrespondent.Enabled = false;
                            UpdatePanelTop.Update();
                        }
                        //if (this.RblTypeCorrespondent.SelectedValue == "I")
                        //{
                            this.liAddressBookLinkList.Attributes.Remove("class");
                            this.liAddressBookLinkList.Attributes.Add("class", "otherAddressTab");
                            this.liAddressBookLinkOrg.Attributes.Remove("class");
                            this.liAddressBookLinkOrg.Attributes.Add("class", "addressTab");
                            this.UpTypeResult.Update();

                            //this.AddressBookSearch.Enabled = false;
                            this.UpPnlButtons.Update();


                            this.showTreeView(this.loadTreeView(corrForLoading));

                        //}
                    }
                    UpdatePanelTop.Update();
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "functionKeyPress", "<script>$(function() {$('.defaultAction').keypress(function(e) {if(e.which == 13) {e.preventDefault();$('#AddressBookSearch').click();}});});</script>", false);
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void ExpandeTreeView(object sender, TreeNodeEventArgs e)
        {
            try
            {
                List<CorrespondentDetail> treeNodeElementInSession = (List<CorrespondentDetail>)HttpContext.Current.Session["AddressBook.nodeElementFound"];

                if (this.IsTotalOrganigram)
                {
                    SmistamentoRubrica smistamentoRubrica = (SmistamentoRubrica)HttpContext.Current.Session["SmistamentoRubrica"];
                    DocsPaWR.InfoUtente infoUt = UIManager.UserManager.GetInfoUser();

                    if (smistamentoRubrica != null)
                    {
                        OrganizationChartTreeNode selectedNode = (OrganizationChartTreeNode)e.Node;
                        Corrispondente corr = UIManager.AddressBookManager.GetCorrespondentBySystemId(selectedNode.SystemID);
                        ElementoRubrica elem = new ElementoRubrica();

                        elem.systemId = corr.systemId;
                        elem.descrizione = corr.descrizione;
                        elem.codice = corr.codiceRubrica;
                        elem.interno = (corr.tipoIE.Equals("I"));

                        DocsPaWR.ElementoRubrica next_child = null;

                        int insertPosition = treeNodeElementInSession.FindIndex(o => o.SystemID == selectedNode.SystemID);
                        this.addChildren(infoUt, selectedNode, elem, next_child, smistamentoRubrica.calltype, smistamentoRubrica, treeNodeElementInSession, insertPosition);

                        if (selectedNode.ChildNodes.Count > 0)
                        {
                            foreach (OrganizationChartTreeNode dummyNode in selectedNode.ChildNodes)
                            {
                                if (dummyNode.NodeData == "__DUMMY_NODE__")
                                {
                                    selectedNode.ChildNodes.Remove(dummyNode);
                                    break;
                                }
                            }
                        }

                        UpdateGridAtCcButtonTreeView(treeNodeElementInSession);
                    }
                }
                else
                {
                    List<OrganizationChartTreeNode> allNodeCollapsed = ExpandedTreeNodeToList(AddressBookTreeView.Nodes);

                    foreach (OrganizationChartTreeNode collapsedNode in allNodeCollapsed)
                    {
                        List<int> indexList = listIndexFound(treeNodeElementInSession, collapsedNode.SystemID);
                        if (indexList.Count > 0)
                        {
                            foreach (int i in indexList)
                            {
                                grdAtCcTreeNode.Rows[i].Visible = true;
                            }
                        }
                    }

                    UpPnlTreeResult.Update();
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void CollapseTreeView(object sender, TreeNodeEventArgs e)
        {
            try
            {
                List<CorrespondentDetail> treeNodeElementInSession = (List<CorrespondentDetail>)HttpContext.Current.Session["AddressBook.nodeElementFound"];
                OrganizationChartTreeNode selectedNode = (OrganizationChartTreeNode)e.Node;

                if (this.IsTotalOrganigram)
                {
                    foreach (OrganizationChartTreeNode tempNode in selectedNode.ChildNodes)
                    {
                        this.RemoveNodeAndChildren(tempNode, treeNodeElementInSession);
                    }

                    if (selectedNode.ChildNodes.Count > 0)
                    {
                        int totalChildNodes = selectedNode.ChildNodes.Count - 1;
                        for (int i = totalChildNodes; i >= 0; i--)
                        {
                            selectedNode.ChildNodes.RemoveAt(i);
                        }
                    }

                    OrganizationChartTreeNode dummy = new OrganizationChartTreeNode();
                    dummy.Text = "Caricamento in corso...";
                    dummy.NodeData = "__DUMMY_NODE__";
                    dummy.Expanded = false;
                    selectedNode.ChildNodes.Add(dummy);

                    UpdateGridAtCcButtonTreeView(treeNodeElementInSession);
                }
                else
                {
                    List<OrganizationChartTreeNode> allNodeCollapsed = CollapsedTreeNodeToList(selectedNode.ChildNodes);

                    foreach (OrganizationChartTreeNode collapsedNode in allNodeCollapsed)
                    {
                        grdAtCcTreeNode.Rows[collapsedNode.listPosition].Visible = false;
                    }

                    UpPnlTreeResult.Update();
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private List<OrganizationChartTreeNode> ExpandedTreeNodeToList(TreeNodeCollection childrenOfNode, List<OrganizationChartTreeNode> tempList = null)
        {
            if (tempList == null) tempList = new List<OrganizationChartTreeNode>();

            try
            {
                foreach (OrganizationChartTreeNode nodeFound in childrenOfNode)
                {
                    if (nodeFound.Expanded.Value)
                        tempList.Add(nodeFound);

                    if (nodeFound.ChildNodes.Count > 0)
                        tempList = ExpandedTreeNodeToList(nodeFound.ChildNodes, tempList);
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }

            return tempList;
        }

        private List<OrganizationChartTreeNode> CollapsedTreeNodeToList(TreeNodeCollection childrenOfNode, List<OrganizationChartTreeNode> tempList = null)
        {
            if (tempList == null) tempList = new List<OrganizationChartTreeNode>();

            foreach (OrganizationChartTreeNode nodeFound in childrenOfNode)
            {
                tempList.Add(nodeFound);
                tempList = CollapsedTreeNodeToList(nodeFound.ChildNodes, tempList);
            }

            return tempList;
        }

        private void RemoveNodeAndChildren(OrganizationChartTreeNode deletingNode, List<CorrespondentDetail> treeNodeElementInSession)
        {
            foreach (OrganizationChartTreeNode tempNode in deletingNode.ChildNodes)
            {
                RemoveNodeAndChildren(tempNode, treeNodeElementInSession);
            }

            int deletePosition = treeNodeElementInSession.FindIndex(o => o.SystemID == deletingNode.SystemID);
            if (deletePosition != -1)
            {
                treeNodeElementInSession.RemoveAt(deletePosition);
            }

            if (deletingNode.ChildNodes.Count > 0)
            {
                int totalChildNodes = deletingNode.ChildNodes.Count - 1;
                for (int i = totalChildNodes; i >= 0; i--)
                {
                    deletingNode.ChildNodes.RemoveAt(i);
                }
            }
        }

        private bool loadTreeView(CorrespondentDetail rootItem = null)
        {
            bool result = true;
            try
            {

                this.IsOrganigramMode = true;
                DocsPaWR.InfoUtente infoUt = UIManager.UserManager.GetInfoUser();
                DocsPaWR.ParametriRicercaRubrica qco = new DocsPaWR.ParametriRicercaRubrica();
                List<CorrespondentDetail> treeNodeElementInSession = new List<CorrespondentDetail>();
                SmistamentoRubrica smistamentoRubrica = (SmistamentoRubrica)HttpContext.Current.Session["SmistamentoRubrica"];
                List<CorrespondentDetail> correspondentsAtSelectedInSession = (List<CorrespondentDetail>)Session["AddressBook.At"];
                List<CorrespondentDetail> correspondentsCcSelectedInSession = (List<CorrespondentDetail>)Session["AddressBook.Cc"];

                ElementoRubrica[] ers = null;

                if (this.RblTypeCorrespondent.SelectedValue == "IE")
                {
                    if (this.CallType == RubricaCallType.CALLTYPE_PROTO_INT_DEST
                        || this.CallType == RubricaCallType.CALLTYPE_PROTO_OUT
                        || this.CallType == RubricaCallType.CALLTYPE_PROTO_USCITA_SEMPLIFICATO
                        || this.CallType == RubricaCallType.CALLTYPE_ORGANIGRAMMA)
                    {
                        qco.calltype = RubricaCallType.CALLTYPE_ORGANIGRAMMA;
                    }
                    else
                    {
                        if (this.CallType == RubricaCallType.CALLTYPE_NEWFASC_LOCFISICA
                            || this.CallType == RubricaCallType.CALLTYPE_NEWFASC_UFFREF
                            || this.CallType == RubricaCallType.CALLTYPE_FILTRIRICFASC_UFFREF
                            || this.CallType == RubricaCallType.CALLTYPE_FILTRIRICFASC_LOCFIS
                            || this.CallType == RubricaCallType.CALLTYPE_EDITFASC_LOCFISICA
                            || this.CallType == RubricaCallType.CALLTYPE_EDITFASC_UFFREF
                            || this.CallType == RubricaCallType.CALLTYPE_GESTFASC_LOCFISICA
                            || this.CallType == RubricaCallType.CALLTYPE_GESTFASC_UFFREF
                            || (this.CallType == RubricaCallType.CALLTYPE_TRASM_ALL
                            || this.CallType == RubricaCallType.CALLTYPE_TRASM_INF
                            || this.CallType == RubricaCallType.CALLTYPE_TRASM_SUP
                            || this.CallType == RubricaCallType.CALLTYPE_TRASM_PARILIVELLO
                            && UserManager.getRegistroSelezionato(this) == null)
                            )
                        {
                            qco.calltype = RubricaCallType.CALLTYPE_ORGANIGRAMMA_TOTALE;
                        }
                        else
                        {
                            qco.calltype = RubricaCallType.CALLTYPE_ORGANIGRAMMA_INTERNO;
                        }
                    }

                }
                else
                {
                    if (this.CallType == RubricaCallType.CALLTYPE_PROTO_INT_DEST
                        || this.CallType == RubricaCallType.CALLTYPE_PROTO_OUT
                        || this.CallType == RubricaCallType.CALLTYPE_PROTO_USCITA_SEMPLIFICATO
                        || this.CallType == RubricaCallType.CALLTYPE_ORGANIGRAMMA_TOTALE_PROTOCOLLO)
                    {
                        qco.calltype = RubricaCallType.CALLTYPE_ORGANIGRAMMA_TOTALE_PROTOCOLLO;
                    }
                    else
                    {

                        if (this.CallType == RubricaCallType.CALLTYPE_PROTO_OUT_MITT
                            || this.CallType == RubricaCallType.CALLTYPE_PROTO_OUT_MITT_SEMPLIFICATO
                            || this.CallType == RubricaCallType.CALLTYPE_PROTO_INT_MITT
                            || this.CallType == RubricaCallType.CALLTYPE_ORGANIGRAMMA_INTERNO
                            || this.CallType == RubricaCallType.CALLTYPE_DEST_FOR_SEARCH_MODELLI
                            || this.CallType == RubricaCallType.CALLTYPE_OWNER_AUTHOR)
                        {
                            qco.calltype = RubricaCallType.CALLTYPE_ORGANIGRAMMA_INTERNO;
                        }
                        else
                        {
                            qco.calltype = RubricaCallType.CALLTYPE_ORGANIGRAMMA_TOTALE;
                        }
                    }
                }

                //SmistamentoRubrica smistamentoRubrica = (SmistamentoRubrica) HttpContext.Current.Session["SmistamentoRubrica"];
                if (smistamentoRubrica == null)
                {
                    smistamentoRubrica = new SmistamentoRubrica();
                    HttpContext.Current.Session["SmistamentoRubrica"] = smistamentoRubrica;
                }
                //callType
                smistamentoRubrica.calltype = qco.calltype;

                //InfoUtente
                smistamentoRubrica.infoUt = infoUt;

                //Ruolo Protocollatore
                DocsPaWR.Ruolo role = UIManager.RoleManager.GetRoleInSession();
                if (role != null)
                {
                    smistamentoRubrica.ruoloProt = role;
                }

                if (UserManager.getRegistroSelezionato(this) != null)
                {
                    //Registro corrente
                    smistamentoRubrica.idRegistro = UserManager.getRegistroSelezionato(this).systemId;
                }
                smistamentoRubrica.daFiltrareSmistamento = "0";
                if (this.CallType == RubricaCallType.CALLTYPE_PROTO_INT_DEST
                    || this.CallType == RubricaCallType.CALLTYPE_PROTO_OUT
                    || this.CallType == RubricaCallType.CALLTYPE_PROTO_USCITA_SEMPLIFICATO)
                {
                    smistamentoRubrica.daFiltrareSmistamento = "1";
                }

                if (rootItem != null)
                {
                    this.IsTotalOrganigram = false;

                    UserManager.setQueryRubricaCaller(ref qco);
                    qco.parent = rootItem.CodiceRubrica;
                    qco.doUo = true;
                    qco.doRuoli = true;
                    qco.doUtenti = true;
                    qco.tipoIE = rootItem.EI_Type != "I" ? AddressbookTipoUtente.ESTERNO : AddressbookTipoUtente.INTERNO;

                    ElementoRubrica er1 = UserManager.getElementoRubrica(this, (Server.UrlDecode(rootItem.CodiceRubrica).Replace("|@ap@|", "'")));
                    string id_amm = infoUt.idAmministrazione;
                    ers = UserManager.getGerarchiaElemento(infoUt, (er1.codice),
                        er1.interno ? DocsPaWR.AddressbookTipoUtente.INTERNO : DocsPaWR.AddressbookTipoUtente.ESTERNO,
                        this, smistamentoRubrica);
                    if (ers == null || ers.Length == 0)
                    {
                        string msg = "WarningErrorOrganizationChart";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", "\\'") + "', 'warning');} else {parent.ajaxDialogModal('" + msg.Replace("'", "\\'") + "', 'warning');}", true);
                        return false;
                    }
                }
                else
                {
                    ers = UserManager.getRootItems(AddressbookTipoUtente.INTERNO, infoUt, smistamentoRubrica);
                }

                select_item_types(ref qco);
                UserManager.check_children_existence(infoUt, ref ers, qco.doUo, qco.doRuoli, qco.doUtenti);

                OrganizationChartTreeNode preNd = null;
                bool newRoot = true;
                int livello = -1;
                for (int n = 0; n < ers.Length; n++)
                {
                    DocsPaWR.ElementoRubrica er = ers[n];

                    CorrespondentDetail nc = new CorrespondentDetail();
                    OrganizationChartTreeNode nd = new OrganizationChartTreeNode();

                    string startSpecialStyle = "";
                    string endSpecialStyle = "";
                    if (er.disabledTrasm) { startSpecialStyle = "<span class=\"red\">"; endSpecialStyle = "</span>"; }
                    if (er.disabled) { startSpecialStyle = "<span class=\"redStrike\">"; endSpecialStyle = "</span>"; }
                    nc.Enabled = (!(er.disabledTrasm || er.disabled));
                    nc.SystemID = nd.SystemID = er.systemId;
                    nd.Descrizione = nd.Text = startSpecialStyle + er.descrizione + endSpecialStyle + "<em style='visibility: hidden;'>" + n + "</em>";
                    nc.Descrizione = er.descrizione + " (" + er.codice + ")";
                    nc.CodiceRubrica = er.codice;
                    if (nc.Enabled) nd.Text = "<span class=\"draggable clickable SystemID_" + er.systemId + " treeview\">" + nd.Text + "</span>";
                    nd.NodeData = n.ToString();//er.codice;
                    nd.Expanded = true;
                    nc.Tipo = er.tipo;
                    nc.EI_Type = (er.interno ? "I" : "E");
                    nc.ImgTipo = nd.ImageUrl = getElementType(er.tipo, nc.EI_Type);
                    nd.At = nc.At = (correspondentsAtSelectedInSession != null ? (correspondentsAtSelectedInSession.FindIndex(z => z.SystemID == er.systemId) == -1 ? IMG_AT_UNSELECT : IMG_AT_SELECT) : IMG_AT_UNSELECT);
                    nd.Cc = nc.Cc = (correspondentsCcSelectedInSession != null ? (correspondentsCcSelectedInSession.FindIndex(y => y.SystemID == er.systemId) == -1 ? IMG_CC_UNSELECT : IMG_CC_SELECT) : IMG_CC_UNSELECT);
                    nc.Canale = er.canale;
                    nc.Rubrica = getAddressBookType(er);
                    nc.isRubricaComune = er.isRubricaComune;
                    nd.listPosition = n;

                    treeNodeElementInSession.Add(nc);

                    if (rootItem != null)
                    {
                        if (newRoot)
                        {
                            preNd = nd;
                            AddressBookTreeView.Nodes.Add(preNd);
                            livello++;
                            newRoot = false;
                        }
                        else
                            preNd = nodeToAddInTreeView(nd, er, rootItem, livello, out newRoot);

                        if (newRoot && rootItem.Tipo.Equals("R"))
                        {
                            this.addChildrenElement(ref preNd, rootItem.Tipo, treeNodeElementInSession, infoUt, n);
                        }

                        if (n == (ers.Length - 1) && rootItem.Tipo.Equals("U"))
                        {
                            this.addChildrenElement(ref nd, rootItem.Tipo, treeNodeElementInSession, infoUt, n);
                        }
                    }
                    else
                    {
                        this.AddressBookTreeView.Nodes.Add(nd);
                        DocsPaWR.ElementoRubrica next_child = (n < (ers.Length - 1)) ? ers[n + 1] : null;
                        this.addChildren(infoUt, nd, er, next_child, qco.calltype, smistamentoRubrica, treeNodeElementInSession, -1);
                    }
                }

                HttpContext.Current.Session["AddressBook.nodeElementFound"] = treeNodeElementInSession;
                this.UpdateGridAtCcButtonTreeView(treeNodeElementInSession);
                result = true;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
            return result;
        }

        public string spanEnable(bool t)
        {
            if (t) return "draggable";
            return string.Empty;
        }

        protected string GetImgNoDetails(string img)
        {
            string imgUrl = string.Empty;
            if (!string.IsNullOrEmpty(img))
            {
                imgUrl = "../Images/Icons/";

                switch (img.ToUpper())
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
            }

            return imgUrl;
        }

        protected bool GetVisibleImgNoDetails(string img)
        {
            bool result = false;
            if (!string.IsNullOrEmpty(img))
            {
                result = true;
            }

            return result;
        }

        protected string GetImgDetailsAddA(string img)
        {
            string result = string.Empty;
            if (HttpContext.Current.Session["AddressBook.from"] != null)
            {
                switch (HttpContext.Current.Session["AddressBook.from"].ToString())
                {
                    case "D_A_S_S":
                    case "D_I_S_S":
                    case "D_P_S_S":
                    case "D_A_S_M":
                    case "D_I_S_M":
                    case "D_P_S_M":
                    case "SP_CREATOR":
                    case "F_X_X_S":
                    case "F_X_X_S_4" :
                    case "F_X_X_S_5":
                        if (img.Equals(IMG_AT_SELECT))
                        {
                            result = IMG_FROM_SELECT;
                        }
                        if (img.Equals(IMG_AT_UNSELECT))
                        {
                            result = IMG_FROM_UNSELECT;
                        }
                        break;
                    default:
                        result = img;
                        break;
                }
            }
            return result;
        }

        protected string GetImgDetailsAddAHover(string img)
        {
            string result = string.Empty;
            if (HttpContext.Current.Session["AddressBook.from"] != null)
            {
                switch (HttpContext.Current.Session["AddressBook.from"].ToString())
                {
                    case "D_A_S_S":
                    case "D_I_S_S":
                    case "D_P_S_S":
                    case "D_A_S_M":
                    case "D_I_S_M":
                    case "D_P_S_M":
                    case "SP_CREATOR":
                    case "F_X_X_S":
                    case "F_X_X_S_4" :
                    case "F_X_X_S_5":
                        if (img.Equals(IMG_AT_SELECT))
                        {
                            result = IMG_FROM_UNSELECT;
                        }
                        if (img.Equals(IMG_AT_UNSELECT))
                        {
                            result = IMG_FROM_SELECT;
                        }
                        break;
                    default:
                        if (img.Equals(IMG_AT_SELECT))
                        {
                            result = IMG_AT_UNSELECT;
                        }
                        if (img.Equals(IMG_AT_UNSELECT))
                        {
                            result = IMG_AT_SELECT;
                        }
                        break;
                }
            }
            return result;
        }

        private void addChildrenElement(ref OrganizationChartTreeNode preNd, string preNdType, List<CorrespondentDetail> treeNodeElementInSession, DocsPaWR.InfoUtente infoUt, int n)
        {
            try
            {
                List<CorrespondentDetail> correspondentsAtSelectedInSession = (List<CorrespondentDetail>)Session["AddressBook.At"];
                List<CorrespondentDetail> correspondentsCcSelectedInSession = (List<CorrespondentDetail>)Session["AddressBook.Cc"];

                DocsPaWR.ElementoRubrica[] lastNodeList = UserManager.getChildrenElemento(preNd.SystemID, preNdType, infoUt);
                if (lastNodeList != null && lastNodeList.Count() > 0)
                {
                    for (int f = 0; f < lastNodeList.Length; f++)
                    {
                        DocsPaWR.ElementoRubrica elem = lastNodeList[f];
                        CorrespondentDetail nc = new CorrespondentDetail();
                        OrganizationChartTreeNode nd = new OrganizationChartTreeNode();

                        string startSpecialStyle = "";
                        string endSpecialStyle = "";
                        if (elem.disabledTrasm) { startSpecialStyle = "<span class=\"red\">"; endSpecialStyle = "</span>"; }
                        if (elem.disabled) { startSpecialStyle = "<span class=\"redStrike\">"; endSpecialStyle = "</span>"; }
                        nc.Enabled = (!(elem.disabledTrasm || elem.disabled));
                        nc.SystemID = nd.SystemID = elem.systemId;
                        nd.Text = nd.Descrizione = elem.descrizione + "<em style='visibility: hidden;'>" + f + "</em>";
                        nc.Descrizione = startSpecialStyle + elem.descrizione + endSpecialStyle + " (" + elem.codice + ")";
                        nc.CodiceRubrica = nd.NodeData = elem.codice;
                        if (nc.Enabled) nd.Text = "<span class=\"draggable clickable SystemID_" + elem.systemId + " treeview\">" + nd.Text + "</span>";
                        nd.Expanded = true;
                        nc.Tipo = elem.tipo;
                        nc.EI_Type = (elem.interno ? "I" : "E");
                        nc.ImgTipo = nd.ImageUrl = getElementType(elem.tipo, nc.EI_Type);
                        nd.At = nc.At = (correspondentsAtSelectedInSession != null ? (correspondentsAtSelectedInSession.FindIndex(z => z.SystemID == elem.systemId) == -1 ? IMG_AT_UNSELECT : IMG_AT_SELECT) : IMG_AT_UNSELECT);
                        nd.Cc = nc.Cc = (correspondentsCcSelectedInSession != null ? (correspondentsCcSelectedInSession.FindIndex(y => y.SystemID == elem.systemId) == -1 ? IMG_CC_UNSELECT : IMG_CC_SELECT) : IMG_CC_UNSELECT);
                        nc.Canale = elem.canale;
                        nc.Rubrica = getAddressBookType(elem);
                        nc.isRubricaComune = elem.isRubricaComune;
                        nd.listPosition = n;
                        n++;
                        treeNodeElementInSession.Add(nc);

                        preNd.ChildNodes.Add(nd);
                    }
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private OrganizationChartTreeNode nodeToAddInTreeView(OrganizationChartTreeNode nodeToAdd, ElementoRubrica elemToAdd, CorrespondentDetail correspondentFrom, int livello, out bool newRoot)
        {
            OrganizationChartTreeNode returnNode = null;
            bool tempNewRoot = false;

            switch (correspondentFrom.Tipo)
            {
                case "U":
                    returnNode = iterChildens((OrganizationChartTreeNode)this.AddressBookTreeView.Nodes[livello]);
                    returnNode.ChildNodes.Add(nodeToAdd);
                    break;

                case "R":
                    if (elemToAdd.tipo.Equals("R"))
                    {
                        OrganizationChartTreeNode tempNode = iterChildens((OrganizationChartTreeNode)this.AddressBookTreeView.Nodes[livello]);
                        tempNode.ChildNodes.Add(nodeToAdd);
                        returnNode = nodeToAdd;
                        tempNewRoot = true;
                    }
                    else
                    {
                        returnNode = iterChildens((OrganizationChartTreeNode)this.AddressBookTreeView.Nodes[livello]);
                        returnNode.ChildNodes.Add(nodeToAdd);
                    }

                    break;

                case "P":
                    returnNode = iterChildens((OrganizationChartTreeNode)this.AddressBookTreeView.Nodes[livello]);
                    returnNode.ChildNodes.Add(nodeToAdd);

                    if (elemToAdd.tipo.Equals("P") && elemToAdd.codice.Equals(correspondentFrom.CodiceRubrica))
                    {
                        tempNewRoot = true;
                    }
                    else
                    {
                        tempNewRoot = false;
                    }
                    break;
            }
            newRoot = tempNewRoot;
            return returnNode;
        }

        private OrganizationChartTreeNode iterChildens(OrganizationChartTreeNode nodeToRoot)
        {
            if (nodeToRoot.ChildNodes.Count == 0)
            {
                return nodeToRoot;
            }
            else
            {
                return iterChildens((OrganizationChartTreeNode)nodeToRoot.ChildNodes[0]);
            }
        }

        private void addChildren(DocsPaWR.InfoUtente infoUt, OrganizationChartTreeNode root, DocsPaWR.ElementoRubrica er, DocsPaWR.ElementoRubrica next_child, DocsPaWR.RubricaCallType calltype, DocsPaWR.SmistamentoRubrica smistamentoRubrica, List<CorrespondentDetail> treeNodeElementInSession, int insertPosition)
        {
            try
            {
                List<CorrespondentDetail> correspondentsAtSelectedInSession = (List<CorrespondentDetail>)Session["AddressBook.At"];
                List<CorrespondentDetail> correspondentsCcSelectedInSession = (List<CorrespondentDetail>)Session["AddressBook.Cc"];

                DocsPaWR.ParametriRicercaRubrica qco = new DocsPaWR.ParametriRicercaRubrica();
                qco.calltype = calltype;
                UserManager.setQueryRubricaCaller(ref qco);
                qco.parent = er.codice;
                qco.tipoIE = er.interno ? DocsPaWR.AddressbookTipoUtente.INTERNO : DocsPaWR.AddressbookTipoUtente.ESTERNO;
                select_item_types(ref qco);

                //qco.doListe = false;
                DocsPaWR.ElementoRubrica[] children = UserManager.getElementiRubrica(this.Page, qco, smistamentoRubrica);
                rearrange_children(er, ref children);
                if (children != null && children.Length > 0)
                {
                    UserManager.check_children_existence(infoUt, ref children, qco.doUo, qco.doRuoli, qco.doUtenti);
                    int n = 0;
                    foreach (DocsPaWR.ElementoRubrica e in children)
                    {
                        if ((next_child == null) || (e.codice != next_child.codice))
                        {
                            OrganizationChartTreeNode nd = new OrganizationChartTreeNode();
                            CorrespondentDetail nc = new CorrespondentDetail();

                            string startSpecialStyle = "";
                            string endSpecialStyle = "";
                            if (e.disabledTrasm) { startSpecialStyle = "<span class=\"red\">"; endSpecialStyle = "</span>"; }
                            if (e.disabled) { startSpecialStyle = "<span class=\"redStrike\">"; endSpecialStyle = "</span>"; }
                            nc.Enabled = (!(e.disabledTrasm || e.disabled));
                            nc.SystemID = nd.SystemID = e.systemId;
                            nd.Text = nd.Descrizione = startSpecialStyle + e.descrizione + endSpecialStyle + "<em style='visibility: hidden;'>" + n + "</em>";
                            if (nc.Enabled) nd.Text = "<span class=\"clickable draggable SystemID_" + e.systemId + " treeview\">" + nd.Text + "</span>";
                            nc.Descrizione = e.descrizione + " (" + e.codice + ")";
                            nc.CodiceRubrica = nd.NodeData = e.codice;
                            nc.Tipo = e.tipo;
                            nc.EI_Type = (e.interno ? "I" : "E");
                            nc.ImgTipo = nd.ImageUrl = getElementType(e.tipo, nc.EI_Type);
                            nc.At = nd.At = (correspondentsAtSelectedInSession != null ? (correspondentsAtSelectedInSession.FindIndex(z => z.SystemID == e.systemId) == -1 ? IMG_AT_UNSELECT : IMG_AT_SELECT) : IMG_AT_UNSELECT);
                            nc.Cc = nd.Cc = (correspondentsCcSelectedInSession != null ? (correspondentsCcSelectedInSession.FindIndex(y => y.SystemID == e.systemId) == -1 ? IMG_CC_UNSELECT : IMG_CC_SELECT) : IMG_CC_UNSELECT);
                            nc.Canale = e.canale;
                            nc.Rubrica = getAddressBookType(e);
                            nc.isRubricaComune = e.isRubricaComune;
                            nd.SelectAction = TreeNodeSelectAction.None;
                            nd.Expanded = false;
                            nd.Target = "_self";

                            n++;

                            if (e.tipo != "P")
                            {
                                OrganizationChartTreeNode dummy = new OrganizationChartTreeNode();
                                dummy.Text = "Caricamento in corso...";
                                dummy.NodeData = "__DUMMY_NODE__";
                                dummy.Expanded = false;
                                nd.ChildNodes.Add(dummy);
                            }

                            root.ChildNodes.Add(nd);

                            if (insertPosition != -1)
                            {
                                treeNodeElementInSession.Insert(insertPosition + 1, nc);
                                insertPosition++;
                            }
                            else
                                treeNodeElementInSession.Add(nc);
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private void rearrange_children(ElementoRubrica parent, ref ElementoRubrica[] children)
        {
            if (parent.tipo != "U")
                return;

            List<ElementoRubrica> nc_1 = new List<ElementoRubrica>();
            for (int i = 0; i < children.Length; i++)
            {
                if (children[i].tipo == "R" || children[i].tipo == "U")
                {
                    nc_1.Add(children[i]);
                }
            }

            ElementoRubrica[] nc = new ElementoRubrica[nc_1.Count];
            nc_1.CopyTo(nc);

            children = nc;
        }

        protected void AddressBookSearch_Click(object sender, EventArgs e)
        {
            try
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);
                if (this.UpPnlTreeResult.Visible)
                {
                    this.AddressBookBtnInitialize.Enabled = true;
                    this.IsOrganigramMode = false;
                    this.IsTotalOrganigram = true;
                    AddressBookTreeView.Nodes.Clear();
                    if (!string.IsNullOrEmpty(this.hdnOldSelectedCorrType.Value))
                    {
                        this.RblTypeCorrespondent.SelectedValue = this.hdnOldSelectedCorrType.Value;
                        this.RblTypeCorrespondent.Enabled = true;
                    }
                    this.showTreeView(false);

                    this.liAddressBookLinkList.Attributes.Remove("class");
                    this.liAddressBookLinkList.Attributes.Add("class", "addressTab");
                    this.liAddressBookLinkOrg.Attributes.Remove("class");
                    this.liAddressBookLinkOrg.Attributes.Add("class", "otherAddressTab");
                    this.UpTypeResult.Update();
                    //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "<script>$(function() {$('.defaultAction').keypress(function(e) {if(e.which == 13) {e.preventDefault();$('#AddressBookSearch').click();}});});</script>", false);
                }
                //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "<script>reallowOp();</script>", false);
                this.Search();
                this.RefreshScript();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private void Search()
        {
            try
            {
                int firstRow = 0;

                DocsPaWR.ParametriRicercaRubrica qco = new DocsPaWR.ParametriRicercaRubrica();
                qco.calltype = this.CallType;
                if (this.RblTypeCorrespondent.SelectedValue == "E") // se è selezionato Esterni
                {
                    if (qco.calltype == RubricaCallType.CALLTYPE_PROTO_OUT
                        || qco.calltype == RubricaCallType.CALLTYPE_PROTO_USCITA_SEMPLIFICATO)
                    {
                        qco.calltype = RubricaCallType.CALLTYPE_PROTO_OUT_ESTERNI;
                    }
                }

                UserManager.setQueryRubricaCaller(ref qco);

                if (qco.calltype == RubricaCallType.CALLTYPE_PROTO_OUT_ESTERNI
                        || qco.calltype == RubricaCallType.CALLTYPE_RICERCA_CORR_NON_STORICIZZATO
                        || ((qco.calltype == RubricaCallType.CALLTYPE_PROTO_IN
                            || qco.calltype == RubricaCallType.CALLTYPE_PROTO_INGRESSO
                            || qco.calltype == RubricaCallType.CALLTYPE_PROTO_IN_INT
                            || qco.calltype == RubricaCallType.CALLTYPE_MITT_MULTIPLI
                            || qco.calltype == RubricaCallType.CALLTYPE_MITT_MULTIPLI_SEMPLIFICATO)
                            && (this.RblTypeCorrespondent.SelectedValue == "E" || this.RblTypeCorrespondent.SelectedValue == "IE")
                            || ((qco.calltype == RubricaCallType.CALLTYPE_PROTO_OUT
                                || qco.calltype == RubricaCallType.CALLTYPE_PROTO_USCITA_SEMPLIFICATO)
                                && this.RblTypeCorrespondent.SelectedValue == "IE")
                        )
                    || qco.calltype== RubricaCallType.CALLTYPE_LISTE_DISTRIBUZIONE)
                {
                    char[] sep = { '_' };
                    if (!string.IsNullOrEmpty(this.AddressBookDDLTypeAOO.SelectedValue))
                    {
                        string[] currentSelection = this.AddressBookDDLTypeAOO.SelectedItem.Value.Split(sep);
                        string condRegistro = string.Empty;

                        if (currentSelection != null)
                        {
                            if (currentSelection[1] != string.Empty)
                            {
                                condRegistro = currentSelection[0];
                            }
                            else
                            {
                                condRegistro = getCondizioneRegistro(this.AddressBookDDLTypeAOO);
                            }
                        }
                        qco.caller.filtroRegistroPerRicerca = condRegistro;
                    }
                }

                if (qco.calltype == DocsPaWR.RubricaCallType.CALLTYPE_MANAGE
                    || qco.calltype == DocsPaWR.RubricaCallType.CALLTYPE_RICERCA_ESTESA
                    || qco.calltype == DocsPaWR.RubricaCallType.CALLTYPE_RICERCA_MITTDEST
                    || qco.calltype == DocsPaWR.RubricaCallType.CALLTYPE_RICERCA_MITTINTERMEDIO
                    || qco.calltype == DocsPaWR.RubricaCallType.CALLTYPE_RICERCA_COMPLETAMENTO
                    //|| qco.calltype == DocsPaWR.RubricaCallType.CALLTYPE_LISTE_DISTRIBUZIONE
                    || qco.calltype == DocsPaWR.RubricaCallType.CALLTYPE_CORR_EST
                    || qco.calltype == DocsPaWR.RubricaCallType.CALLTYPE_CORR_EST_CON_DISABILITATI
                    || qco.calltype == DocsPaWR.RubricaCallType.CALLTYPE_CORR_INT
                    || qco.calltype == DocsPaWR.RubricaCallType.CALLTYPE_CORR_INT_EST
                    || qco.calltype == DocsPaWR.RubricaCallType.CALLTYPE_CORR_INT_EST_CON_DISABILITATI
                    || qco.calltype == DocsPaWR.RubricaCallType.CALLTYPE_CORR_INT_CON_DISABILITATI
                    || qco.calltype == DocsPaWR.RubricaCallType.CALLTYPE_CORR_NO_FILTRI
                    || qco.calltype == DocsPaWR.RubricaCallType.CALLTYPE_CORR_INT_NO_UO
                    || qco.calltype == RubricaCallType.CALLTYPE_RICERCA_CORR_NON_STORICIZZATO
                    || qco.calltype == RubricaCallType.CALLTYPE_VIS_RUOLO
                    || qco.calltype == RubricaCallType.CALLTYPE_VIS_UTENTE
                        )
                {
                    
                    if (!string.IsNullOrEmpty(this.AddressBookDDLTypeAOO.SelectedValue))
                    {
                        char[] sep = { '_' };
                        string[] currentSelection = this.AddressBookDDLTypeAOO.SelectedItem.Value.Split(sep);
                        string condRegistro = string.Empty;

                        if (currentSelection != null)
                        {
                            if (currentSelection[1] != string.Empty)
                            {
                                condRegistro = currentSelection[0];
                            }
                            else
                            {
                                condRegistro = getCondizioneRegistro(this.AddressBookDDLTypeAOO);
                            }
                        }
                        qco.caller.filtroRegistroPerRicerca = condRegistro;
                    }
                    //nuova gestione: devo cercare in tutti i registri e RF visibili al ruolo
                    else if (qco.caller.IdRuolo != null)
                    {
                        DocsPaWR.Registro[] regRuolo = UserManager.getListaRegistriWithRF(qco.caller.IdRuolo, "", "");
                        //regRuolo = UserManager.getListaRegistriWithRF(qco.caller.IdRuolo, "", "");
                        string filtroRegistro = "";
                        for (int i = 0; i < regRuolo.Length; i++)
                        {
                            filtroRegistro = filtroRegistro + regRuolo[i].systemId;
                            if (i < regRuolo.Length - 1)
                            {
                                filtroRegistro = filtroRegistro + " , ";
                            }
                        }
                        qco.caller.filtroRegistroPerRicerca = filtroRegistro;
                        if (qco.calltype == RubricaCallType.CALLTYPE_LISTE_DISTRIBUZIONE && !string.IsNullOrEmpty(filtroRegistro))
                            qco.caller.IdRegistro = filtroRegistro;
                    }
                    else
                    {
                        string idAmm = UIManager.UserManager.GetInfoUser().idAmministrazione;
                        string infoAmm = AdministrationManager.AmmGetInfoAmmCorrente(idAmm).Codice;
                        DocsPaWR.OrgRegistro[] regRuoloOrg = RegistryManager.getRegistriByCodAmm(infoAmm, "0");
                        string filtroRegistro = string.Empty;
                        for (int i = 0; i < regRuoloOrg.Length; i++)
                        {
                            filtroRegistro = filtroRegistro + regRuoloOrg[i].IDRegistro;
                            if (i < regRuoloOrg.Length - 1)
                            {
                                filtroRegistro = filtroRegistro + " , ";
                            }
                        }
                        qco.caller.filtroRegistroPerRicerca = filtroRegistro;
                        if (qco.calltype == RubricaCallType.CALLTYPE_LISTE_DISTRIBUZIONE && !string.IsNullOrEmpty(filtroRegistro))
                            qco.caller.IdRegistro = filtroRegistro;
                    }

                }

                qco.parent = string.Empty;
                qco.citta = this.TxtCity.Text;
                qco.codice = this.TxtCode.Text;
                qco.descrizione = this.TxtDescription.Text;
                qco.localita = this.TxtCountry.Text;
                qco.tipoIE = select_tipoIE();
                //old qco.calltype = (DocsPAWA.DocsPaWR.RubricaCallType) Convert.ToInt32 (calltype);
                //qco.ObjectType = objtype;
                qco.doRubricaComune = (this.EnableCommonAddressBook && this.AddressBookChkCommonAddressBook.Checked);
                qco.email = this.TxtMail.Text;

                //Aggiungo i parametri di ricerca per codice fiscale e partita iva
                qco.codiceFiscale = this.TxtNIN.Text;
                qco.partitaIva = this.TxtPIVA.Text;
                this.select_item_types(ref qco);

                int totalFound;
                DocsPaWR.ElementoRubrica[] corrSearch = UserManager.getElementiRubrica(this, qco, firstRow, this.rowForPage, out totalFound);
                LitAddressBookResult.Text = totalFound.ToString() + " " + this.lblNumFound;

                List<CorrespondentDetail> CorrespondentsFound = new List<CorrespondentDetail>();

                if (corrSearch != null && corrSearch.Length != 0)
                {
                    this.AddressBookExportSearch.Enabled = true;
                    HttpContext.Current.Session["firstRow"] = 1;
                    List<CorrespondentDetail> listaCorrAtInSession = (List<CorrespondentDetail>)HttpContext.Current.Session["AddressBook.At"];
                    List<CorrespondentDetail> listaCorrCcInSession = (List<CorrespondentDetail>)HttpContext.Current.Session["AddressBook.Cc"];

                    HttpContext.Current.Session["AddressBook.corrFilter"] = qco;

                    foreach (ElementoRubrica element in corrSearch)
                    {
                        Boolean enabled = true;
                        string startSpecialStyle = "";
                        string endSpecialStyle = "";
                        if (element.disabledTrasm) { startSpecialStyle = "<span class=\"red\">"; endSpecialStyle = "</span>"; }
                        if (element.disabled) { startSpecialStyle = "<span class=\"redStrike\">"; endSpecialStyle = "</span>"; enabled = false; }

                        CorrespondentDetail newElement = new CorrespondentDetail();
                        newElement.SystemID = (element.isRubricaComune && element.rubricaComune != null ? "RC_" + element.rubricaComune.IdRubricaComune.ToString() : element.systemId);
                        newElement.Descrizione = startSpecialStyle + element.descrizione + endSpecialStyle + " (" + element.codice + ")";
                        if(element.tipo.Equals("L"))
                        {
                            string language = UserManager.GetUserLanguage();
                            newElement.Descrizione = string.IsNullOrEmpty(element.idPeopleLista) && string.IsNullOrEmpty(element.idGruppoLista) ? newElement.Descrizione + " - [" + Utils.Languages.GetLabelFromCode("AddressBookListCentralized", language) + "]" :
                                (string.IsNullOrEmpty(element.idGruppoLista) ? newElement.Descrizione + " - [" + Utils.Languages.GetLabelFromCode("AddressBookListAdministrableByUser", language) +"]" : newElement.Descrizione + " - [" + Utils.Languages.GetLabelFromCode("AddressBookListAdministrableByRole", language) + "]");
                        }
                        newElement.CodiceRubrica = element.codice;
                        newElement.Tipo = element.tipo;
                        newElement.EI_Type = (element.interno ? "I" : "E");
                        newElement.ImgTipo = getElementType(element.tipo, newElement.EI_Type, enabled);
                        newElement.At = getImgAtSearch(element.systemId, listaCorrAtInSession);
                        newElement.Cc = (ccTableVisible ? getImgCcSearch(element.systemId, listaCorrCcInSession) : IMG_BLANK);
                        newElement.Canale = element.canale;
                        newElement.Rubrica = getAddressBookType(element);
                        newElement.isRubricaComune = element.isRubricaComune;
                        //Permetto la selezione anche dei disabilitati
                        //newElement.Enabled = (!(element.disabledTrasm || element.disabled));
                        newElement.Enabled = true;
                        CorrespondentsFound.Add(newElement);
                    }
                }
                else
                {
                    this.AddressBookExportSearch.Enabled = false;
                    //RegisterStartupScript("noresults", "<script language=\"javascript\">var ddlIE = document.getElementById('ddlIE'); if(ddlIE!=null && (ddlIE.selectedIndex!=1 && ddlIE.selectedIndex!=2)) {var ddl = document.getElementById('ddl_rf'); if(ddl!=null){ddl.style.visibility = 'hidden'; var label = document.getElementById('lbl_registro'); if(label!=null){label.style.visibility = 'hidden';}}}; alert(\"I parametri di ricerca impostati non hanno prodotto alcun risultato\");</script>");
                }

                this.UpdateGridCorrespondentFound(CorrespondentsFound);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected string GetChannelCorrespondent(bool commonAddress, string code, string channel)
        {
            string result = string.Empty;
            bool common = commonAddress;
            //if (!common)
            //{
            result = channel;
            //}
            //else
            //{
            //    Corrispondente corr = AddressBookManager.getCorrispondenteByCodRubricaRubricaComune(code);
            //    if (corr != null && corr.canalePref != null && !string.IsNullOrEmpty(corr.canalePref.descrizione))
            //    {
            //        result = corr.canalePref.descrizione;
            //    }
            //}
            return result;
        }

        protected string GetChannelPref(ElementoRubrica el)
        {
            string result = string.Empty;
            //if (!string.IsNullOrEmpty(el.canale))
            //{
            result = el.canale;
            //}
            //else
            //{
            //    if (el.isRubricaComune)
            //    {
            //        Corrispondente corr = UIManager.AddressBookManager.getCorrispondenteByCodRubricaRubricaComune(el.codice);
            //        if (corr.canalePref != null && !string.IsNullOrEmpty(corr.canalePref.descrizione))
            //        {
            //            result = corr.canalePref.descrizione;
            //        }
            //    }
            //}

            return result;
        }

        private string getImgAtSearch(string element_systemId, List<CorrespondentDetail> listaCorrInSession)
        {
            string strImg = IMG_AT_UNSELECT;

            if (listaCorrInSession != null)
            {
                if (listaCorrInSession.Where(x => x.SystemID == element_systemId).ToArray().Count() > 0)
                    strImg = IMG_AT_SELECT;
            }

            return strImg;
        }

        private string getImgCcSearch(string element_systemId, List<CorrespondentDetail> listaCorrInSession)
        {
            string strImg = IMG_CC_UNSELECT;

            if (listaCorrInSession != null)
            {
                if (listaCorrInSession.Where(x => x.SystemID == element_systemId).ToArray().Count() > 0)
                    strImg = IMG_CC_SELECT;
            }

            return strImg;
        }

        private string getElementType(string etype, string ie, Boolean enabled = true)
        {
            string imgUrl = "../Images/Icons/";

            switch (etype.ToUpper())
            {
                case "U":
                    if ((!string.IsNullOrEmpty(ie) && ie.Equals("I") && !this.IsOrganigramMode) && treeVisible() && enabled)
                    {
                        imgUrl += "uo_icon_2.png";
                    }
                    else
                    {
                        imgUrl += "uo_icon.png";
                    }
                    break;
                case "P":
                    if ((!string.IsNullOrEmpty(ie) && ie.Equals("I") && !this.IsOrganigramMode) && treeVisible() && enabled)
                    {
                        imgUrl += "user_icon2.png";
                    }
                    else
                    {
                        imgUrl += "user_icon.png";
                    }
                    break;
                case "R":
                    if ((!string.IsNullOrEmpty(ie) && ie.Equals("I") && !this.IsOrganigramMode) && treeVisible() && enabled)
                    {
                        imgUrl += "role_icon_2.png";
                    }
                    else
                    {
                        imgUrl += "role2_icon.png";
                    }
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

        private string getAddressBookType(ElementoRubrica er)
        {
            string codRegTemp = er.codiceRegistro;
            if (er.isRubricaComune == true || er.isDisbledAndRC)
            {
                codRegTemp = "RC";
            }
            else
            {
                if (string.IsNullOrEmpty(codRegTemp))
                {
                    if (er.interno)
                    {
                        codRegTemp = string.Empty;
                    }
                    else
                    {
                        codRegTemp = er.tipo != "L" ? "TUTTI" : string.Empty;
                    }
                }
            }

            return codRegTemp;
        }

        private string getAddressBookType(Corrispondente er)
        {
            string codRegTemp = string.Empty;
            if (!string.IsNullOrEmpty(er.idRegistro)) codRegTemp = RegistryManager.getRegistroBySistemId(er.idRegistro).codRegistro;

            if (er.inRubricaComune)
            {
                codRegTemp = "RC";
            }
            else
            {
                if (string.IsNullOrEmpty(codRegTemp))
                {
                    if (er.tipoIE == "I")
                    {
                        codRegTemp = string.Empty;
                    }
                    else
                    {
                        codRegTemp = "TUTTI";
                    }
                }
            }

            return codRegTemp;
        }

        private void UpdateGridCorrespondentFound(List<CorrespondentDetail> CorrespondentsFound, bool fromSearch = true)
        {
            bool existsEmptyRow = false;
            if (fromSearch) HttpContext.Current.Session["AddressBook.corrsFound"] = CorrespondentsFound;
            if (CorrespondentsFound.Count > 0)
            {
                List<CorrespondentDetail> correspondentsList;

                if (CorrespondentsFound.Count < foundRowForPage)
                {
                    correspondentsList = new List<CorrespondentDetail>(CorrespondentsFound);
                    correspondentsList = AddEmptyRow(correspondentsList, foundRowForPage, out existsEmptyRow);
                }
                else
                {
                    correspondentsList = CorrespondentsFound;
                }

                GrdAddressBookResult.DataSource = correspondentsList;
                if (fromSearch) GrdAddressBookResult.PageIndex = 0;
                GrdAddressBookResult.DataBind();
                if (existsEmptyRow) refreshImageStateInGrid(GrdAddressBookResult);
                if (!ccTableVisible) hideImgCC();
                if (!this.AaTableVisible)
                {
                    GrdAddressBookResult.Columns[5].Visible = false;
                }
            }
            else
                this.CreateEmptyRow(GrdAddressBookResult, foundRowForPage);

            UpPnlGridResult.Update();
        }

        private void UpdateGridAtCcButtonTreeView(List<CorrespondentDetail> TreeCorrespondentFound)
        {
            if (TreeCorrespondentFound.Count > 0)
            {
                grdAtCcTreeNode.DataSource = TreeCorrespondentFound;
                grdAtCcTreeNode.DataBind();
                if (!ccTableVisible) hideImgCC(false);
                if (!this.AaTableVisible)
                {
                    GrdAddressBookResult.Columns[5].Visible = false;
                }
            }

            UpPnlTreeResult.Update();
        }

        private void hideImgCC(bool forGridView = true)
        {
            //foreach (GridViewRow item in GrdAddressBookResult.Rows)
            //{
            //    Image imgCc_inRow = item.FindControl("imgCc") as Image;
            //    if (imgCc_inRow != null) imgCc_inRow.Visible = false;
            //}
            if (forGridView)
                GrdAddressBookResult.Columns[6].Visible = false;
            else
                grdAtCcTreeNode.Columns[1].Visible = false;

        }

        private void ClearAllGrid()
        {
            //resetAllSessionElement();
            this.CreateEmptyRow(GrdAddressBookResult, foundRowForPage);
            this.CreateEmptyRow(GrdAtSelection, atRowForPage);
            this.CreateEmptyRow(GrdCctSelection, ccRowForPage);
            refreshImageStateInGrid(GrdAtSelection);
            refreshImageStateInGrid(GrdCctSelection);
            if (!ccTableVisible) hideImgCC();

            if (!this.AaTableVisible)
            {
                GrdAddressBookResult.Columns[5].Visible = false;
            }

            this.UpPnlTreeResult.Visible = false;
            UpPnlTreeResult.Update();
            UpPnlGridResult.Update();
            UpPnlGridAt.Update();
            UpPnlGridCc.Update();
        }

        private void select_item_types(ref DocsPaWR.ParametriRicercaRubrica qco)
        {
            switch (this.CallType)
            {
                case RubricaCallType.CALLTYPE_UFFREF_PROTO:
                case RubricaCallType.CALLTYPE_GESTFASC_LOCFISICA:
                case RubricaCallType.CALLTYPE_GESTFASC_UFFREF:
                case RubricaCallType.CALLTYPE_EDITFASC_LOCFISICA:
                case RubricaCallType.CALLTYPE_EDITFASC_UFFREF:
                case RubricaCallType.CALLTYPE_NEWFASC_LOCFISICA:
                case RubricaCallType.CALLTYPE_NEWFASC_UFFREF:
                case RubricaCallType.CALLTYPE_RICERCA_UFFREF:
                case RubricaCallType.CALLTYPE_FILTRIRICFASC_UFFREF:
                case RubricaCallType.CALLTYPE_FILTRIRICFASC_LOCFIS:
                case RubricaCallType.CALLTYPE_STAMPA_REG_UO:
                    qco.doUo = true;
                    qco.doRuoli = false;
                    qco.doUtenti = false;
                    break;

                case RubricaCallType.CALLTYPE_RICERCA_TRASM:
                case RubricaCallType.CALLTYPE_RICERCA_TRASM_TODOLIST:

                case RubricaCallType.CALLTYPE_RICERCA_UO_RUOLI_SOTTOPOSTI:
                case RubricaCallType.CALLTYPE_TUTTI_RUOLI:
                case RubricaCallType.CALLTYPE_TUTTE_UO:
                    //qco.doUo = (Request.QueryString["tipo_corr"] == "U");
                    //qco.doRuoli = (Request.QueryString["tipo_corr"] == "R");
                    //qco.doUtenti = (Request.QueryString["tipo_corr"] == "P");
                    break;

                case RubricaCallType.CALLTYPE_REPLACE_ROLE:
                case RubricaCallType.CALLTYPE_FIND_ROLE:
                    qco.doUo = this.ChkListType.Items.FindByValue("U").Selected;
                    qco.doRuoli = this.ChkListType.Items.FindByValue("R").Selected;
                    qco.doUtenti = this.ChkListType.Items.FindByValue("P").Selected;
                    break;

                //Laura 19 Marzo
                case RubricaCallType.CALLTYPE_OWNER_AUTHOR:
                //IACOZZILLI GIORDANO:
                //NUOVO CALLTYPE ARCHIVE:
                case RubricaCallType.CALLTYPE_DEP_OSITO:
                case RubricaCallType.CALLTYPE_RICERCA_CREATOR:
                    qco.doUo = this.ChkListType.Items.FindByValue("U").Selected;
                    qco.doRuoli = this.ChkListType.Items.FindByValue("R").Selected;
                    qco.doUtenti = this.ChkListType.Items.FindByValue("P").Selected;
                    break;

                case RubricaCallType.CALLTYPE_ORGANIGRAMMA:
                case RubricaCallType.CALLTYPE_ORGANIGRAMMA_TOTALE_PROTOCOLLO:
                case RubricaCallType.CALLTYPE_ORGANIGRAMMA_INTERNO:
                    qco.doUo = true;
                    qco.doRuoli = true;
                    qco.doUtenti = true;
                    qco.doListe = false;
                    break;

                case RubricaCallType.CALLTYPE_CORR_INT_NO_UO:
                    qco.doUo = false;
                    qco.doListe = false;
                    qco.doRuoli = this.AddressBookChkRoles.Selected;
                    qco.doUtenti = this.AddressBookChkRoles.Selected;
                    break;

                case RubricaCallType.CALLTYPE_VIS_UTENTE:
                case RubricaCallType.CALLTYPE_VIS_RUOLO:
                    qco.doUo = false;
                    qco.doListe = false;
                    qco.doRuoli = this.AddressBookChkRoles.Selected;
                    qco.doUtenti = this.AddressBookChkUsers.Selected;
                    break;

                case RubricaCallType.CALLTYPE_RICERCA_TRASM_SOTTOPOSTO:
                    qco.doUo = false;
                    qco.doListe = false;
                    qco.doRuoli = true;
                    qco.doUtenti = false;
                    break;

                default:
                    qco.doUo = this.AddressBookChkOfficies.Selected;
                    qco.doRuoli = this.AddressBookChkRoles.Selected;
                    qco.doUtenti = this.AddressBookChkUsers.Selected;
                    qco.doListe = (this.AddressBookChkLists.Enabled && AddressBookChkLists.Selected);
                    qco.doRF = (this.AddressBookChkRF.Enabled && AddressBookChkRF.Selected);
                    break;
            }
        }

        private AddressbookTipoUtente select_tipoIE()
        {

            RubricaCallType ct = this.CallType;

            if (!this.RblTypeCorrespondent.Enabled)
            {
                if (ct == RubricaCallType.CALLTYPE_TRASM_ALL ||
                    ct == RubricaCallType.CALLTYPE_TRASM_INF ||
                    ct == RubricaCallType.CALLTYPE_PROTO_OUT_MITT ||
                    ct == RubricaCallType.CALLTYPE_PROTO_OUT_MITT_SEMPLIFICATO ||
                    ct == RubricaCallType.CALLTYPE_TRASM_SUP ||
                    ct == RubricaCallType.CALLTYPE_TRASM_PARILIVELLO ||
                    ct == RubricaCallType.CALLTYPE_UFFREF_PROTO ||
                    ct == RubricaCallType.CALLTYPE_STAMPA_REG_UO ||
                    ct == RubricaCallType.CALLTYPE_GESTFASC_LOCFISICA ||
                    ct == RubricaCallType.CALLTYPE_GESTFASC_UFFREF ||
                    ct == RubricaCallType.CALLTYPE_EDITFASC_LOCFISICA ||
                    ct == RubricaCallType.CALLTYPE_EDITFASC_UFFREF ||
                    ct == RubricaCallType.CALLTYPE_NEWFASC_LOCFISICA ||
                    ct == RubricaCallType.CALLTYPE_NEWFASC_UFFREF ||
                    ct == RubricaCallType.CALLTYPE_RICERCA_UFFREF ||
                    ct == RubricaCallType.CALLTYPE_PROTO_INT_MITT ||
                    ct == RubricaCallType.CALLTYPE_PROTO_INT_DEST ||
                    ct == RubricaCallType.CALLTYPE_FILTRIRICFASC_UFFREF ||
                    ct == RubricaCallType.CALLTYPE_FILTRIRICFASC_LOCFIS ||
                    ct == RubricaCallType.CALLTYPE_RICERCA_TRASM ||
                    ct == RubricaCallType.CALLTYPE_RICERCA_TRASM_TODOLIST ||
                    ct == RubricaCallType.CALLTYPE_RICERCA_DOCUMENTI_CORR_INT ||
                    ct == RubricaCallType.CALLTYPE_MITT_MODELLO_TRASM ||
                    ct == RubricaCallType.CALLTYPE_MODELLI_TRASM_ALL ||
                    ct == RubricaCallType.CALLTYPE_MODELLI_TRASM_INF ||
                    ct == RubricaCallType.CALLTYPE_MODELLI_TRASM_SUP ||
                    ct == RubricaCallType.CALLTYPE_MODELLI_TRASM_PARILIVELLO ||
                    ct == RubricaCallType.CALLTYPE_DEST_MODELLO_TRASM ||
                    ct == RubricaCallType.CALLTYPE_ORGANIGRAMMA ||
                    ct == RubricaCallType.CALLTYPE_ORGANIGRAMMA_TOTALE_PROTOCOLLO ||
                    ct == RubricaCallType.CALLTYPE_ORGANIGRAMMA_TOTALE ||
                    ct == RubricaCallType.CALLTYPE_RUOLO_REG_NOMAIL ||
                    ct == RubricaCallType.CALLTYPE_RUOLO_RESP_REG ||
                    ct == RubricaCallType.CALLTYPE_RUOLO_RESP_REPERTORI ||
                    ct == RubricaCallType.CALLTYPE_UTENTE_REG_NOMAIL ||
                    ct == RubricaCallType.CALLTYPE_RICERCA_CREATOR ||
                    ct == RubricaCallType.CALLTYPE_OWNER_AUTHOR ||
                    //IACOZZILLI GIORDANO:
                    //NUOVO CALLTYPE ARCHIVE:
                    ct == RubricaCallType.CALLTYPE_DEP_OSITO ||
                    ct == RubricaCallType.CALLTYPE_RICERCA_TRASM_SOTTOPOSTO ||
                    ct == RubricaCallType.CALLTYPE_RICERCA_UO_RUOLI_SOTTOPOSTI ||
                    ct == RubricaCallType.CALLTYPE_TUTTI_RUOLI ||
                    ct == RubricaCallType.CALLTYPE_TUTTE_UO ||
                    ct == RubricaCallType.CALLTYPE_REPLACE_ROLE ||
                    ct == RubricaCallType.CALLTYPE_FIND_ROLE ||
                    ct == RubricaCallType.CALLTYPE_DEST_FOR_SEARCH_MODELLI
                    )
                {
                    if (ct == RubricaCallType.CALLTYPE_ORGANIGRAMMA ||
                        ct == RubricaCallType.CALLTYPE_ORGANIGRAMMA_TOTALE_PROTOCOLLO ||
                        ct == RubricaCallType.CALLTYPE_ORGANIGRAMMA_INTERNO
                        || ct == RubricaCallType.CALLTYPE_ORGANIGRAMMA_TOTALE)
                    {

                        return AddressbookTipoUtente.INTERNO;
                    }
                    else
                    {
                        this.RblTypeCorrespondent.SelectedValue = "I";
                        return AddressbookTipoUtente.INTERNO;
                    }
                }
                else
                {
                    if (ct == RubricaCallType.CALLTYPE_MANAGE)
                    {
                        this.RblTypeCorrespondent.SelectedValue = "E";
                        return AddressbookTipoUtente.ESTERNO;
                    }

                    if (ct == RubricaCallType.CALLTYPE_ORGANIGRAMMA_INTERNO)
                    {
                        return AddressbookTipoUtente.INTERNO;
                    }
                }

                if (ct == RubricaCallType.CALLTYPE_CORR_INT || ct == RubricaCallType.CALLTYPE_CORR_INT_NO_UO || ct == RubricaCallType.CALLTYPE_VIS_UTENTE || ct == RubricaCallType.CALLTYPE_VIS_RUOLO)
                {
                    this.RblTypeCorrespondent.SelectedValue = "I";
                    return AddressbookTipoUtente.INTERNO;
                }

                if (ct == RubricaCallType.CALLTYPE_CORR_EST || ct == RubricaCallType.CALLTYPE_CORR_EST_CON_DISABILITATI)
                {
                    this.RblTypeCorrespondent.SelectedValue = "E";
                    return AddressbookTipoUtente.ESTERNO;
                }
            }
            else
            {
                switch (this.RblTypeCorrespondent.SelectedValue)
                {
                    case "I":
                        return AddressbookTipoUtente.INTERNO;

                    case "E":
                        return AddressbookTipoUtente.ESTERNO;

                    case "IE":
                        {
                            if (ct == RubricaCallType.CALLTYPE_ORGANIGRAMMA
                                || ct == RubricaCallType.CALLTYPE_ORGANIGRAMMA_TOTALE_PROTOCOLLO
                                || ct == RubricaCallType.CALLTYPE_ORGANIGRAMMA_INTERNO
                                || ct == RubricaCallType.CALLTYPE_ORGANIGRAMMA_TOTALE)
                            {
                                //se ho cliccato su organigramma, nel caso di interni/Esterni
                                //devo restituire tutto l'organigramma interno escludendo i
                                //corrispondenti esterni all'amministrazione.
                                return AddressbookTipoUtente.INTERNO;

                            }
                            else
                            {
                                return AddressbookTipoUtente.GLOBALE;
                            }
                        }
                }
            }

            this.RblTypeCorrespondent.SelectedValue = "IE";
            return AddressbookTipoUtente.GLOBALE;
        }

        private string getCondizioneRegistro(DropDownList ddl_rf)
        {
            string retValue = string.Empty;
            foreach (ListItem item in ddl_rf.Items)
            {
                if (item.Value != "")
                {
                    char[] sep = { '_' };
                    string[] currentSelection = item.Value.Split(sep);

                    retValue += " " + currentSelection[0] + ",";
                }
            }
            if (retValue.EndsWith(","))
                retValue = retValue.Remove(retValue.LastIndexOf(","));
            return retValue;
        }

        private void loadRfData()
        {

            DocsPaWR.Registro[] userRegistri = null;
            DocsPaWR.Registro[] listaTotale = null;
            Registro registro = null;

            registro = UIManager.RegistryManager.GetRegistryInSession();

            if (registro == null)
            {
                registro = UIManager.RoleManager.GetRoleInSession().registri[0];
                listaTotale = UserManager.getListaRegistriWithRF(this, "1", UIManager.RoleManager.GetRoleInSession().registri[0].systemId);
            }
            else
            {
                listaTotale = UserManager.getListaRegistriWithRF(this, "1", registro.systemId);
            }

            if (listaTotale != null && listaTotale.Length > 0)
            {
                userRegistri = new Registro[listaTotale.Length + 1];
                userRegistri[0] = registro;
                listaTotale.CopyTo(userRegistri, 1);
                AddressBookDDLTypeAOO.Enabled = true;
            }
            else
            {
                userRegistri = new Registro[1];
                userRegistri[0] = registro;
                AddressBookDDLTypeAOO.Enabled = false;
            }

            string strText = "";
            for (short iff = 0; iff < 3; iff++)
            {
                strText += " ";
            }

            for (int i = 0; i < userRegistri.Length; i++)
            {
                string testo = userRegistri[i].codRegistro;
                if (userRegistri[i].chaRF == "1")
                {
                    testo = strText + testo;
                }

                AddressBookDDLTypeAOO.Items.Add(testo);

                AddressBookDDLTypeAOO.Items[i].Value = userRegistri[i].systemId + "_" + userRegistri[i].rfDisabled + "_" + userRegistri[i].idAOOCollegata;
            }
        }

        protected void AddressBook_OnRowCreated(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Pager)
            {
                //e.Row.Attributes.Add("onclick", "disallowOp('Content2')");

                TableRow tRow = e.Row.Controls[0].Controls[0].Controls[0] as TableRow;
                foreach (TableCell tCell in tRow.Cells)
                {
                    Control ctrl = tCell.Controls[0];
                    if (ctrl is LinkButton)
                    {
                        LinkButton lb = (LinkButton)ctrl;
                        lb.Attributes.Add("onclick", "disallowOp('Content2')");
                    }
                }
            }
        }

        protected void changPageGrdFound_click(object sender, GridViewPageEventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);

            List<CorrespondentDetail> correspondentsFound = (List<CorrespondentDetail>)HttpContext.Current.Session["AddressBook.corrsFound"];
            this.GrdAddressBookResult.PageIndex = e.NewPageIndex;
            GrdAddressBookResult.DataSource = correspondentsFound;
            GrdAddressBookResult.DataBind();
            if (!ccTableVisible) hideImgCC();
            if (!this.AaTableVisible)
            {
                GrdAddressBookResult.Columns[5].Visible = false;
            }
            UpPnlGridResult.Update();
        }

        protected void changPageGrdAt_click(object sender, GridViewPageEventArgs e)
        {
            try
            {
                List<CorrespondentDetail> correspondentsAt = (List<CorrespondentDetail>)HttpContext.Current.Session["AddressBook.At"];
                this.GrdAtSelection.PageIndex = e.NewPageIndex;
                GrdAtSelection.DataSource = correspondentsAt;
                GrdAtSelection.DataBind();
                UpPnlGridAt.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void changPageGrdCc_click(object sender, GridViewPageEventArgs e)
        {
            try
            {
                List<CorrespondentDetail> correspondentsCc = (List<CorrespondentDetail>)HttpContext.Current.Session["AddressBook.Cc"];
                this.GrdCctSelection.PageIndex = e.NewPageIndex;
                GrdCctSelection.DataSource = correspondentsCc;
                GrdCctSelection.DataBind();
                UpPnlGridCc.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private void addAllElement(List<CorrespondentDetail> addList, List<CorrespondentDetail> fromList, List<CorrespondentDetail> removeList, bool isAt)
        {
            if (fromList != null && fromList.Count() > 0)
            {
                for (int i = 0; i < fromList.Count(); i++)
                {
                    CorrespondentDetail corr = fromList[i];

                    corr.At = (isAt ? IMG_AT_SELECT : IMG_AT_UNSELECT);
                    corr.Cc = (isAt ? IMG_CC_UNSELECT : IMG_CC_SELECT);

                    int atIndex = -1;
                    if (addList == null)
                        addList = new List<CorrespondentDetail>();
                    else
                        atIndex = addList.FindIndex(o => o.SystemID == corr.SystemID);

                    if (atIndex == -1)
                    {
                        addList.Add(corr);
                    }

                    if (removeList != null)
                    {
                        int ccIndex = removeList.FindIndex(o => o.SystemID == corr.SystemID);
                        if (ccIndex != -1)
                        {
                            removeList.RemoveAt(ccIndex);
                        }
                    }
                }

                //HttpContext.Current.Session["AddressBook.corrsFound"] = tempFromList;
                HttpContext.Current.Session["AddressBook.At"] = (isAt ? addList : removeList);
                HttpContext.Current.Session["AddressBook.Cc"] = (isAt ? removeList : addList);

                bool existsEmptyRowFrom = false;
                List<CorrespondentDetail> fromListUpdated = AddEmptyRow(fromList, foundRowForPage, out existsEmptyRowFrom);

                bool existsEmptyRow1 = false;
                List<CorrespondentDetail> addListUpdated = AddEmptyRow(addList, atRowForPage, out existsEmptyRow1);

                bool existsEmptyRow2 = false;
                List<CorrespondentDetail> removeListUpdated = AddEmptyRow(removeList, ccRowForPage, out existsEmptyRow2);

                GrdAddressBookResult.DataSource = fromListUpdated;
                GrdAddressBookResult.DataBind();
                if (existsEmptyRowFrom) refreshImageStateInGrid(GrdAddressBookResult);

                if (isAt)
                {
                    LitAddressBookAt.Text = (addList != null ? addList.Count().ToString() : "0") + " " + this.lblNumFound;
                    GrdAtSelection.DataSource = addListUpdated;
                    GrdAtSelection.DataBind();
                    if (existsEmptyRow1) refreshImageStateInGrid(GrdAtSelection);

                    if (ccTableVisible)
                    {
                        LitAddressBookCc.Text = (removeList != null ? removeList.Count().ToString() : "0") + " " + this.lblNumFound;
                        GrdCctSelection.DataSource = removeListUpdated;
                        GrdCctSelection.DataBind();
                        if (existsEmptyRow2) refreshImageStateInGrid(GrdCctSelection);
                    }
                }
                else
                {
                    LitAddressBookCc.Text = (addList != null ? addList.Count().ToString() : "0") + " " + this.lblNumFound;
                    GrdCctSelection.DataSource = addListUpdated;
                    GrdCctSelection.DataBind();
                    if (existsEmptyRow1) refreshImageStateInGrid(GrdCctSelection);

                    LitAddressBookAt.Text = (removeList != null ? removeList.Count().ToString() : "0") + " " + this.lblNumFound;
                    GrdAtSelection.DataSource = removeListUpdated;
                    GrdAtSelection.DataBind();
                    if (existsEmptyRow2) refreshImageStateInGrid(GrdAtSelection);
                }

                if (!ccTableVisible) hideImgCC();
                if (!this.AaTableVisible)
                {
                    GrdAddressBookResult.Columns[5].Visible = false;
                }
            }
        }

        private void addListOfElement(List<CorrespondentDetail> addList, string[] idList, List<CorrespondentDetail> removeList, bool isAt, string sessionFrom = "AddressBook.corrsFound")
        {
            List<CorrespondentDetail> listaCorrFound = (List<CorrespondentDetail>)HttpContext.Current.Session[sessionFrom];

            if (idList != null && idList.Count() > 0)
            {
                //List<CorrespondentDetail> fromList = (from s in idList join e in listaCorrFound on s equals e.SystemID select e).ToList();

                foreach (string systemID in idList)
                {
                    List<int> IndexArray = (from a in listaCorrFound.Select((item, i) => new { obj = item, index = i }) where a.obj.SystemID.Equals(systemID) select a.index).ToList();
                    foreach (int i in IndexArray)
                    {
                        listaCorrFound[i].At = (isAt ? IMG_AT_SELECT : IMG_AT_UNSELECT);
                        listaCorrFound[i].Cc = (isAt ? IMG_CC_UNSELECT : IMG_CC_SELECT);
                    }

                    int atIndex = -1;
                    if (addList == null)
                        addList = new List<CorrespondentDetail>();
                    else
                    {
                        if (multipleSelection)
                            atIndex = addList.FindIndex(o => o.SystemID == systemID);
                        else
                        {
                            CorrespondentDetail tempCorr = addList.ElementAt(0);
                            int d = listaCorrFound.FindIndex(v => v.SystemID == tempCorr.SystemID);
                            CorrespondentDetail oldcorr = listaCorrFound[d];

                            oldcorr.At = (isAt ? IMG_AT_SELECT : IMG_AT_UNSELECT);
                            oldcorr.Cc = (isAt ? IMG_CC_UNSELECT : IMG_CC_SELECT);
                            addList.RemoveAt(0);
                        }
                    }

                    int j = listaCorrFound.FindIndex(v => v.SystemID == systemID);
                    CorrespondentDetail corr = listaCorrFound[j];

                    if (atIndex == -1)
                    {
                        addList.Add(corr);
                    }

                    if (removeList != null && removeList.Count > 0)
                    {
                        int ccIndex = removeList.FindIndex(o => o.SystemID == corr.SystemID);
                        if (ccIndex != -1)
                        {
                            removeList.RemoveAt(ccIndex);
                        }
                    }
                }

                HttpContext.Current.Session["AddressBook.At"] = (isAt ? addList : removeList);
                HttpContext.Current.Session["AddressBook.Cc"] = (isAt ? removeList : addList);

                bool existsEmptyRowFrom = false;
                List<CorrespondentDetail> fromListUpdated = AddEmptyRow(listaCorrFound, foundRowForPage, out existsEmptyRowFrom);

                bool existsEmptyRow1 = false;
                List<CorrespondentDetail> addListUpdated = AddEmptyRow(addList, atRowForPage, out existsEmptyRow1);

                bool existsEmptyRow2 = false;
                List<CorrespondentDetail> removeListUpdated = AddEmptyRow(removeList, ccRowForPage, out existsEmptyRow2);

                if (!this.IsOrganigramMode)
                {
                    GrdAddressBookResult.DataSource = fromListUpdated;
                    GrdAddressBookResult.DataBind();
                    if (existsEmptyRowFrom) refreshImageStateInGrid(GrdAddressBookResult);
                }
                else
                    UpdateGridAtCcButtonTreeView(listaCorrFound);

                if (isAt)
                {
                    LitAddressBookAt.Text = (addList != null ? addList.Count().ToString() : "0") + " " + this.lblNumFound;
                    GrdAtSelection.DataSource = addListUpdated;
                    GrdAtSelection.DataBind();
                    if (existsEmptyRow1) refreshImageStateInGrid(GrdAtSelection);

                    if (ccTableVisible)
                    {
                        LitAddressBookCc.Text = (removeList != null ? removeList.Count().ToString() : "0") + " " + this.lblNumFound;
                        GrdCctSelection.DataSource = removeListUpdated;
                        GrdCctSelection.DataBind();
                        if (existsEmptyRow2) refreshImageStateInGrid(GrdCctSelection);
                    }
                }
                else
                {
                    LitAddressBookCc.Text = (addList != null ? addList.Count().ToString() : "0") + " " + this.lblNumFound;
                    GrdCctSelection.DataSource = addListUpdated;
                    GrdCctSelection.DataBind();
                    if (existsEmptyRow1) refreshImageStateInGrid(GrdCctSelection);

                    LitAddressBookAt.Text = (removeList != null ? removeList.Count().ToString() : "0") + " " + this.lblNumFound;
                    GrdAtSelection.DataSource = removeListUpdated;
                    GrdAtSelection.DataBind();
                    if (existsEmptyRow2) refreshImageStateInGrid(GrdAtSelection);
                }

                if (!ccTableVisible) hideImgCC();
                if (!this.AaTableVisible)
                {
                    GrdAddressBookResult.Columns[5].Visible = false;
                }
            }
        }

        protected void refreshSelectedCorrType(object sender, EventArgs e)
        {
            try
            {
                RadioButtonList obj = (RadioButtonList)sender;
                this.hdnOldSelectedCorrType.Value = obj.SelectedValue;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void addAllAt_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);

                if (multipleSelection)
                {
                    List<CorrespondentDetail> correspondentsAtSelectedInSession = (List<CorrespondentDetail>)Session["AddressBook.At"];
                    List<CorrespondentDetail> correspondentsCcSelectedInSession = (List<CorrespondentDetail>)Session["AddressBook.Cc"];
                    List<CorrespondentDetail> listaCorrFound = (List<CorrespondentDetail>)HttpContext.Current.Session["AddressBook.corrsFound"];

                    this.addAllElement(correspondentsAtSelectedInSession, listaCorrFound, correspondentsCcSelectedInSession, true);

                    UpPnlGridResult.Update();
                    UpPnlGridAt.Update();
                    if (ccTableVisible) UpPnlGridCc.Update();
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void addAllCc_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);
                if (multipleSelection)
                {
                    if (ccTableVisible)
                    {
                        List<CorrespondentDetail> correspondentsAtSelectedInSession = (List<CorrespondentDetail>)Session["AddressBook.At"];
                        List<CorrespondentDetail> correspondentsCcSelectedInSession = (List<CorrespondentDetail>)Session["AddressBook.Cc"];
                        List<CorrespondentDetail> listaCorrFound = (List<CorrespondentDetail>)HttpContext.Current.Session["AddressBook.corrsFound"];

                        this.addAllElement(correspondentsCcSelectedInSession, listaCorrFound, correspondentsAtSelectedInSession, false);

                        UpPnlGridResult.Update();
                        UpPnlGridAt.Update();
                        UpPnlGridCc.Update();
                    }
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void addAtCorrespondent(string item_systemId)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);

            List<CorrespondentDetail> correspondentsFoundInSession = new List<CorrespondentDetail>();
            Corrispondente corr = AddressBookManager.GetCorrespondentBySystemId(item_systemId);

            CorrespondentDetail newCorrespondent = new CorrespondentDetail();
            newCorrespondent.SystemID = corr.systemId;
            newCorrespondent.Descrizione = corr.descrizione + " (" + corr.codiceRubrica + ")";
            newCorrespondent.CodiceRubrica = corr.codiceRubrica;
            newCorrespondent.Tipo = corr.tipoCorrispondente;
            newCorrespondent.EI_Type = corr.tipoIE;
            newCorrespondent.ImgTipo = getElementType(corr.tipoCorrispondente, newCorrespondent.EI_Type);
            newCorrespondent.At = IMG_AT_SELECT;
            newCorrespondent.Cc = IMG_CC_UNSELECT;
            newCorrespondent.Canale = corr.canalePref.descrizione;
            newCorrespondent.Rubrica = getAddressBookType(corr);
            newCorrespondent.isRubricaComune = corr.inRubricaComune;
            newCorrespondent.Enabled = (!(corr.disabledTrasm || corr.disabledTrasm));

            correspondentsFoundInSession.Add(newCorrespondent);
            Session["AddressBook.corrsFound"] = correspondentsFoundInSession;

            bool existsEmptyRow = false;
            LitAddressBookAt.Text = addElementInGridView(item_systemId, GrdAtSelection, atRowForPage, out existsEmptyRow).ToString() + " " + this.lblNumFound;
            if (existsEmptyRow) refreshImageStateInGrid(GrdAtSelection);
            UpPnlGridAt.Update();
        }

        protected void btnInsertMultipleAtCorrespondents_Click(object sender, EventArgs e)
        {
            try
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);

                string[] ids = new string[0];
                if (!string.IsNullOrEmpty(this.hdnMultipleAtCorrespondents.Value))
                {
                    string temp = this.hdnMultipleAtCorrespondents.Value.Replace("SystemID_", "");
                    if (temp.IndexOf("|") > 0)
                        ids = temp.Split('|');
                    else
                        ids = new string[1] { temp };
                }

                List<CorrespondentDetail> correspondentsAtSelectedInSession = (List<CorrespondentDetail>)Session["AddressBook.At"];
                List<CorrespondentDetail> correspondentsCcSelectedInSession = (List<CorrespondentDetail>)Session["AddressBook.Cc"];

                string sessionFrom = "AddressBook.corrsFound";
                if (!string.IsNullOrEmpty(this.hdnCallingFromTreeview.Value))
                    sessionFrom = "AddressBook.nodeElementFound";

                if (!this.multipleSelection)
                {
                    string item_systemId = ids[0];
                    if (!existCorrespondentInGrid(item_systemId))
                    {
                        bool existsEmptyRow = false;
                        LitAddressBookAt.Text = addElementInGridView(item_systemId, GrdAtSelection, atRowForPage, out existsEmptyRow, sessionFrom).ToString() + " " + this.lblNumFound;
                        if (existsEmptyRow) refreshImageStateInGrid(GrdAtSelection);
                    }
                    else
                    {
                        bool existsEmptyRow = false;
                        LitAddressBookAt.Text = deleteElementInGridView(item_systemId, GrdAtSelection, ccRowForPage, out existsEmptyRow, sessionFrom).ToString() + " " + this.lblNumFound;
                        if (existsEmptyRow) refreshImageStateInGrid(GrdAtSelection);
                    }
                }
                else
                {
                    this.addListOfElement(correspondentsAtSelectedInSession, ids, correspondentsCcSelectedInSession, true, sessionFrom);
                }

                this.hdnMultipleAtCorrespondents.Value = string.Empty;
                this.UpPnlGridResult.Update();
                this.UpPnlTreeResult.Update();
                this.UpPnlGridAt.Update();
                this.UpPnlGridCc.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void btnInsertMultipleCcCorrespondents_Click(object sender, EventArgs e)
        {
            try
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);

                string[] ids = new string[0];
                if (!string.IsNullOrEmpty(this.hdnMultipleCcCorrespondents.Value))
                {
                    string temp = this.hdnMultipleCcCorrespondents.Value.Replace("SystemID_", "");
                    if (temp.IndexOf("|") > 0)
                        ids = temp.Split('|');
                    else
                        ids = new string[1] { temp };
                }

                List<CorrespondentDetail> correspondentsAtSelectedInSession = (List<CorrespondentDetail>)Session["AddressBook.At"];
                List<CorrespondentDetail> correspondentsCcSelectedInSession = (List<CorrespondentDetail>)Session["AddressBook.Cc"];

                string sessionFrom = "AddressBook.corrsFound";
                if (!string.IsNullOrEmpty(this.hdnCallingFromTreeview.Value))
                    sessionFrom = "AddressBook.nodeElementFound";

                this.addListOfElement(correspondentsCcSelectedInSession, ids, correspondentsAtSelectedInSession, false, sessionFrom);

                this.hdnMultipleCcCorrespondents.Value = string.Empty;
                this.UpPnlGridResult.Update();
                this.UpPnlTreeResult.Update();
                this.UpPnlGridAt.Update();
                this.UpPnlGridCc.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void addAtCorrespondent_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);

                string item_systemId = ((CustomImageButton)sender).Attributes["RowValue"];

                this.removeCorrespondentSelectedInCc(item_systemId);

                if (!existCorrespondentInGrid(item_systemId))
                {
                    bool existsEmptyRow = false;
                    LitAddressBookAt.Text = addElementInGridView(item_systemId, GrdAtSelection, atRowForPage, out existsEmptyRow).ToString() + " " + this.lblNumFound;
                    if (existsEmptyRow) refreshImageStateInGrid(GrdAtSelection);
                    UpPnlGridAt.Update();
                }
                else
                {
                    bool existsEmptyRow = false;
                    LitAddressBookAt.Text = deleteElementInGridView(item_systemId, GrdAtSelection, ccRowForPage, out existsEmptyRow).ToString() + " " + this.lblNumFound;
                    if (existsEmptyRow) refreshImageStateInGrid(GrdAtSelection);
                    UpPnlGridAt.Update();
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void addAtNodeCorrespondent_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);

                string item_systemId = ((CustomImageButton)sender).Attributes["RowValue"];

                this.removeCorrespondentSelectedInCc(item_systemId, "AddressBook.nodeElementFound");

                if (!existCorrespondentInGrid(item_systemId))
                {
                    bool existsEmptyRow = false;
                    LitAddressBookAt.Text = addElementInGridView(item_systemId, GrdAtSelection, atRowForPage, out existsEmptyRow, "AddressBook.nodeElementFound").ToString() + " " + this.lblNumFound;
                    if (existsEmptyRow) refreshImageStateInGrid(GrdAtSelection);
                    UpPnlGridAt.Update();
                }
                else
                {
                    bool existsEmptyRow = false;
                    LitAddressBookAt.Text = deleteElementInGridView(item_systemId, GrdAtSelection, ccRowForPage, out existsEmptyRow, "AddressBook.nodeElementFound").ToString() + " " + this.lblNumFound;
                    if (existsEmptyRow) refreshImageStateInGrid(GrdAtSelection);
                    UpPnlGridAt.Update();
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void removeAtCorrespondent_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);

                bool fromTreeView = this.UpPnlTreeResult.Visible;
                string item_systemId = ((CustomImageButton)sender).Attributes["RowValue"];

                if (existCorrespondentInGrid(item_systemId))
                {
                    bool existsEmptyRow = false;
                    if (fromTreeView)
                        LitAddressBookAt.Text = deleteElementInGridView(item_systemId, GrdAtSelection, atRowForPage, out existsEmptyRow, "AddressBook.nodeElementFound").ToString() + " " + this.lblNumFound;
                    else
                        LitAddressBookAt.Text = deleteElementInGridView(item_systemId, GrdAtSelection, atRowForPage, out existsEmptyRow).ToString() + " " + this.lblNumFound;

                    if (existsEmptyRow) refreshImageStateInGrid(GrdAtSelection);
                    UpPnlGridAt.Update();
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void addCcCorrespondent_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);

                string item_systemId = ((CustomImageButton)sender).Attributes["RowValue"];

                this.removeCorrespondentSelectedInAt(item_systemId);

                if (!existCorrespondentInGrid(item_systemId))
                {
                    bool existsEmptyRow = false;
                    LitAddressBookCc.Text = addElementInGridView(item_systemId, GrdCctSelection, ccRowForPage, out existsEmptyRow).ToString() + " " + this.lblNumFound;
                    if (existsEmptyRow) refreshImageStateInGrid(GrdCctSelection);
                    UpPnlGridCc.Update();
                }
                else
                {
                    bool existsEmptyRow = false;
                    LitAddressBookCc.Text = deleteElementInGridView(item_systemId, GrdCctSelection, ccRowForPage, out existsEmptyRow).ToString() + " " + this.lblNumFound;
                    if (existsEmptyRow) refreshImageStateInGrid(GrdCctSelection);
                    UpPnlGridCc.Update();
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void addCcNodeCorrespondent_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);

                string item_systemId = ((CustomImageButton)sender).Attributes["RowValue"];

                this.removeCorrespondentSelectedInAt(item_systemId, "AddressBook.nodeElementFound");

                if (!existCorrespondentInGrid(item_systemId))
                {
                    bool existsEmptyRow = false;
                    LitAddressBookCc.Text = addElementInGridView(item_systemId, GrdCctSelection, ccRowForPage, out existsEmptyRow, "AddressBook.nodeElementFound").ToString() + " " + this.lblNumFound;
                    if (existsEmptyRow) refreshImageStateInGrid(GrdCctSelection);
                    UpPnlGridCc.Update();
                }
                else
                {
                    bool existsEmptyRow = false;
                    LitAddressBookCc.Text = deleteElementInGridView(item_systemId, GrdCctSelection, ccRowForPage, out existsEmptyRow, "AddressBook.nodeElementFound").ToString() + " " + this.lblNumFound;
                    if (existsEmptyRow) refreshImageStateInGrid(GrdCctSelection);
                    UpPnlGridCc.Update();
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void removeCcCorrespondent_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);
                bool fromTreeView = this.UpPnlTreeResult.Visible;
                string item_systemId = ((CustomImageButton)sender).Attributes["RowValue"];

                if (existCorrespondentInGrid(item_systemId))
                {
                    bool existsEmptyRow = false;
                    if (fromTreeView)
                        LitAddressBookCc.Text = deleteElementInGridView(item_systemId, GrdCctSelection, ccRowForPage, out existsEmptyRow, "AddressBook.nodeElementFound").ToString() + " " + this.lblNumFound;
                    else
                        LitAddressBookCc.Text = deleteElementInGridView(item_systemId, GrdCctSelection, ccRowForPage, out existsEmptyRow).ToString() + " " + this.lblNumFound;

                    if (existsEmptyRow) refreshImageStateInGrid(GrdCctSelection);
                    UpPnlGridCc.Update();
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private static bool ExcludeBlank(CorrespondentDetail cor)
        {

            if (cor.SystemID == "-1")
            {
                return false;
            }
            {
                return true;
            }

        }

        protected void imgDeleteAllAt_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);

                List<CorrespondentDetail> correspondentsAtSelectedInSession = (List<CorrespondentDetail>)Session["AddressBook.At"];
                string sessionFrom = (this.IsOrganigramMode ? "AddressBook.nodeElementFound" : "AddressBook.corrsFound");
                List<CorrespondentDetail> listaCorrFound = (List<CorrespondentDetail>)HttpContext.Current.Session[sessionFrom];

                foreach (CorrespondentDetail corr in correspondentsAtSelectedInSession)
                {
                    List<int> indexList = listIndexFound(listaCorrFound, corr.SystemID);

                    if (indexList.Count > 0)
                    {
                        foreach (int i in indexList)
                        {
                            listaCorrFound[i].At = IMG_AT_UNSELECT;
                        }
                    }
                }

                correspondentsAtSelectedInSession.RemoveAll(ExcludeBlank);

                bool existsEmptyRow;
                List<CorrespondentDetail> correspondentsAtSelectedInSessionUpdated = AddEmptyRow(correspondentsAtSelectedInSession, atRowForPage, out existsEmptyRow);
                GrdAtSelection.DataSource = correspondentsAtSelectedInSessionUpdated;
                GrdAtSelection.DataBind();
                if (existsEmptyRow) refreshImageStateInGrid(GrdAtSelection);
                LitAddressBookAt.Text = (correspondentsAtSelectedInSession != null ? correspondentsAtSelectedInSession.Count().ToString() : "0") + " " + this.lblNumFound;

                if (!this.IsOrganigramMode)
                {
                    List<CorrespondentDetail> listaCorrFoundUpdated = AddEmptyRow(listaCorrFound, foundRowForPage, out existsEmptyRow);
                    GrdAddressBookResult.DataSource = listaCorrFoundUpdated;
                    GrdAddressBookResult.DataBind();
                    if (existsEmptyRow) refreshImageStateInGrid(GrdAddressBookResult);

                    UpPnlGridResult.Update();
                }
                else
                {
                    UpdateGridAtCcButtonTreeView(listaCorrFound);
                }
                UpPnlGridAt.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void imgDeleteAllCc_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);

                List<CorrespondentDetail> correspondentsCcSelectedInSession = (List<CorrespondentDetail>)Session["AddressBook.Cc"];
                string sessionFrom = (this.IsOrganigramMode ? "AddressBook.nodeElementFound" : "AddressBook.corrsFound");
                List<CorrespondentDetail> listaCorrFound = (List<CorrespondentDetail>)HttpContext.Current.Session[sessionFrom];

                foreach (CorrespondentDetail corr in correspondentsCcSelectedInSession)
                {
                    List<int> indexList = listIndexFound(listaCorrFound, corr.SystemID);

                    if (indexList.Count > 0)
                    {
                        foreach (int i in indexList)
                        {
                            listaCorrFound[i].Cc = IMG_CC_UNSELECT;
                        }
                    }
                }

                correspondentsCcSelectedInSession.RemoveAll(ExcludeBlank);

                bool existsEmptyRow;
                List<CorrespondentDetail> correspondentsCcSelectedInSessionUpdated = AddEmptyRow(correspondentsCcSelectedInSession, ccRowForPage, out existsEmptyRow);
                GrdCctSelection.DataSource = correspondentsCcSelectedInSessionUpdated;
                GrdCctSelection.DataBind();
                if (existsEmptyRow) refreshImageStateInGrid(GrdCctSelection);
                LitAddressBookCc.Text = (correspondentsCcSelectedInSession != null ? correspondentsCcSelectedInSession.Count().ToString() : "0") + " " + this.lblNumFound;

                if (!this.IsOrganigramMode)
                {
                    List<CorrespondentDetail> listaCorrFoundUpdated = AddEmptyRow(listaCorrFound, foundRowForPage, out existsEmptyRow);
                    GrdAddressBookResult.DataSource = listaCorrFoundUpdated;
                    GrdAddressBookResult.DataBind();
                    if (existsEmptyRow) refreshImageStateInGrid(GrdAddressBookResult);

                    UpPnlGridResult.Update();
                }
                else
                {
                    UpdateGridAtCcButtonTreeView(listaCorrFound);
                }

                UpPnlGridCc.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private int addElementInGridView(string item_systemId, GridView gridForAdd, int totalRow, out bool existsEmptyRow, string sessionFrom = "AddressBook.corrsFound")
        {
            existsEmptyRow = false;
            string sessionName = (gridForAdd.Equals(GrdAtSelection) ? "AddressBook.At" : "AddressBook.Cc");
            List<CorrespondentDetail> correspondentsSelectedInSession = (List<CorrespondentDetail>)Session[sessionName];
            string id_correspondentForExchange = "";

            if (correspondentsSelectedInSession == null) correspondentsSelectedInSession = new List<CorrespondentDetail>();

            if (!multipleSelection && correspondentsSelectedInSession.Count > 0)
            {
                CorrespondentDetail correspondentForExchange = correspondentsSelectedInSession[0];
                id_correspondentForExchange = correspondentForExchange.SystemID;
            }

            List<CorrespondentDetail> listaCorrInSession = (List<CorrespondentDetail>)HttpContext.Current.Session[sessionFrom];

            CorrespondentDetail selectedElement = new CorrespondentDetail();

            List<int> indexList = listIndexFound(listaCorrInSession, item_systemId);

            if (indexList.Count > 0)
            {
                foreach (int i in indexList)
                {
                    selectedElement = listaCorrInSession.ElementAt(i);

                    if (sessionName == "AddressBook.At")
                    {
                        selectedElement.At = IMG_AT_SELECT;
                        selectedElement.Cc = IMG_CC_UNSELECT;
                    }
                    else
                    {
                        selectedElement.Cc = IMG_CC_SELECT;
                        selectedElement.At = IMG_AT_UNSELECT;
                    }

                    listaCorrInSession[i] = selectedElement;
                }

                if (!string.IsNullOrEmpty(id_correspondentForExchange))
                {
                    //CorrespondentDetail[] exchangeElementArray = listaCorrInSession.Where(x => x.SystemID == id_correspondentForExchange).ToArray();
                    List<int> exchangeElementIndexArray = (from a in listaCorrInSession.Select((item, i) => new { obj = item, index = i }) where a.obj.SystemID.Equals(id_correspondentForExchange) select a.index).ToList();
                    if (exchangeElementIndexArray.Count() > 0)
                    {
                        foreach (int index in exchangeElementIndexArray)
                        {
                            if (sessionName == "AddressBook.At")
                                listaCorrInSession[index].At = IMG_AT_UNSELECT;
                            else
                                listaCorrInSession[index].At = IMG_CC_UNSELECT;
                        }
                    }
                    correspondentsSelectedInSession.RemoveAt(0);
                }

                if (!this.IsOrganigramMode)
                    UpdateGridCorrespondentFound(listaCorrInSession, false);
                else
                    UpdateGridAtCcButtonTreeView(listaCorrInSession);

                correspondentsSelectedInSession.Add(selectedElement);
                Session[sessionName] = correspondentsSelectedInSession;

                List<CorrespondentDetail> correspondentsList = AddEmptyRow(correspondentsSelectedInSession, totalRow, out existsEmptyRow);

                gridForAdd.DataSource = correspondentsList;
                gridForAdd.DataBind();
            }

            return correspondentsSelectedInSession.Count();
        }

        private List<int> listIndexFound(List<CorrespondentDetail> listaCorrInSession, string item_systemId)
        {
            List<int> tempLista = new List<int>();

            for (int i = 0; i < listaCorrInSession.Count; i++)
            {
                if (listaCorrInSession[i].SystemID == item_systemId)
                {
                    tempLista.Add(i);
                }
            }

            return tempLista;
        }

        private int deleteElementInGridView(string item_systemId, GridView gridForAdd, int totalRow, out bool existsEmptyRow, string sessionFrom = "AddressBook.corrsFound")
        {
            int retVal = 0;
            existsEmptyRow = false;
            string sessionName = (gridForAdd.Equals(GrdAtSelection) ? "AddressBook.At" : "AddressBook.Cc");
            List<CorrespondentDetail> correspondentsSelectedInSession = (List<CorrespondentDetail>)Session[sessionName];

            if (correspondentsSelectedInSession != null)
            {
                correspondentsSelectedInSession.Remove(correspondentsSelectedInSession.Where(x => x.SystemID == item_systemId).ToArray()[0]);
                Session[sessionName] = correspondentsSelectedInSession;

                List<CorrespondentDetail> listaCorrInSession = (List<CorrespondentDetail>)HttpContext.Current.Session[sessionFrom];

                List<int> indexList = listIndexFound(listaCorrInSession, item_systemId);

                if (indexList.Count > 0)
                {
                    foreach (int i in indexList)
                    {
                        if (sessionName == "AddressBook.At")
                            listaCorrInSession[i].At = IMG_AT_UNSELECT;
                        else
                            listaCorrInSession[i].Cc = IMG_CC_UNSELECT;
                    }

                    if (!this.IsOrganigramMode)
                        UpdateGridCorrespondentFound(listaCorrInSession, false);
                    else
                        UpdateGridAtCcButtonTreeView(listaCorrInSession);
                }

                List<CorrespondentDetail> correspondentsList = AddEmptyRow(correspondentsSelectedInSession, totalRow, out existsEmptyRow);

                gridForAdd.DataSource = correspondentsList;
                gridForAdd.DataBind();

                retVal = correspondentsSelectedInSession.Count();
            }

            return retVal;
        }

        private bool existCorrespondentInGrid(string elementSystemId)
        {
            bool retVal = false;

            List<CorrespondentDetail> correspondentsSelectedInSession = (List<CorrespondentDetail>)HttpContext.Current.Session["AddressBook.At"];
            if (correspondentsSelectedInSession != null && correspondentsSelectedInSession.FindIndex(o => o.SystemID == elementSystemId) != -1)
            {
                retVal = true;
            }
            else
            {
                correspondentsSelectedInSession = (List<CorrespondentDetail>)HttpContext.Current.Session["AddressBook.Cc"];
                if (correspondentsSelectedInSession != null && correspondentsSelectedInSession.FindIndex(o => o.SystemID == elementSystemId) != -1)
                    retVal = true;
            }

            return retVal;
        }

        private void removeCorrespondentSelectedInCc(string elementSystemId, string sessionFrom = "AddressBook.corrsFound")
        {
            List<CorrespondentDetail> correspondentsSelectedInSession = (List<CorrespondentDetail>)HttpContext.Current.Session["AddressBook.Cc"];
            if (correspondentsSelectedInSession != null)
            {
                int index = correspondentsSelectedInSession.FindIndex(o => o.SystemID == elementSystemId);
                if (index != -1)
                {
                    correspondentsSelectedInSession.RemoveAt(index);
                    List<CorrespondentDetail> listaCorrInSession = (List<CorrespondentDetail>)HttpContext.Current.Session[sessionFrom];
                    CorrespondentDetail selectedElement = listaCorrInSession.Where(x => x.SystemID == elementSystemId).ToArray()[0];
                    selectedElement.Cc = IMG_CC_UNSELECT;

                    bool existsEmptyRow;
                    LitAddressBookCc.Text = correspondentsSelectedInSession.Count() + " " + this.lblNumFound;
                    correspondentsSelectedInSession = AddEmptyRow(correspondentsSelectedInSession, ccRowForPage, out existsEmptyRow);
                    GrdCctSelection.DataSource = correspondentsSelectedInSession;
                    GrdCctSelection.DataBind();
                    if (existsEmptyRow) refreshImageStateInGrid(GrdCctSelection);
                    UpPnlGridCc.Update();
                }
            }
        }

        private void removeCorrespondentSelectedInAt(string elementSystemId, string sessionFrom = "AddressBook.corrsFound")
        {
            List<CorrespondentDetail> correspondentsSelectedInSession = (List<CorrespondentDetail>)HttpContext.Current.Session["AddressBook.At"];
            if (correspondentsSelectedInSession != null)
            {
                int index = correspondentsSelectedInSession.FindIndex(o => o.SystemID == elementSystemId);
                if (index != -1)
                {
                    correspondentsSelectedInSession.RemoveAt(index);
                    List<CorrespondentDetail> listaCorrInSession = (List<CorrespondentDetail>)HttpContext.Current.Session[sessionFrom];
                    CorrespondentDetail selectedElement = listaCorrInSession.Where(x => x.SystemID == elementSystemId).ToArray()[0];
                    selectedElement.At = IMG_AT_UNSELECT;

                    bool existsEmptyRow;
                    LitAddressBookAt.Text = correspondentsSelectedInSession.Count() + " " + this.lblNumFound;
                    correspondentsSelectedInSession = AddEmptyRow(correspondentsSelectedInSession, atRowForPage, out existsEmptyRow);
                    GrdAtSelection.DataSource = correspondentsSelectedInSession;
                    GrdAtSelection.DataBind();
                    if (existsEmptyRow) refreshImageStateInGrid(GrdAtSelection);
                    UpPnlGridAt.Update();
                }
            }
        }

        private void refreshImageStateInGrid(GridView gridForUpdate)
        {
            foreach (GridViewRow item in gridForUpdate.Rows)
            {
                HiddenField item_systemId = item.FindControl("trasmId") as HiddenField;
                if (item_systemId != null && item_systemId.Value == "-1")
                {
                    Image imgDetail_inRow = item.FindControl("imgDetail") as Image;
                    if (imgDetail_inRow != null) imgDetail_inRow.Visible = false;
                    Image imgA_inRow = item.FindControl("imgA") as Image;
                    if (imgA_inRow != null) imgA_inRow.Visible = false;
                    Image imgCc_inRow = item.FindControl("imgCc") as Image;
                    if (imgCc_inRow != null) imgCc_inRow.Visible = false;
                    Image imgDeleteA_inRow = item.FindControl("imgDeleteA") as Image;
                    if (imgDeleteA_inRow != null) imgDeleteA_inRow.Visible = false;
                    Image imgDeleteCc_inRow = item.FindControl("imgDeleteCc") as Image;
                    if (imgDeleteCc_inRow != null) imgDeleteCc_inRow.Visible = false;
                }
            }
        }

        protected void AddressBookBtnInitialize_Click(object sender, EventArgs e)
        {
            try
            {
                this.InitializeFilters();
                this.ClearFields();
                this.UpdatePanelTop.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void ClearFields()
        {
            if (this.AddressBookDDLTypeAOO.SelectedIndex >= 0)
            {
                this.AddressBookDDLTypeAOO.SelectedIndex = 0;
            }
            this.TxtCode.Text = string.Empty;
            this.TxtDescription.Text = string.Empty;
            this.TxtCity.Text = string.Empty;
            this.TxtCountry.Text = string.Empty;
            this.TxtMail.Text = string.Empty;
            this.TxtNIN.Text = string.Empty;
            this.TxtPIVA.Text = string.Empty;
        }

        protected void imgDetail_Click(object sender, EventArgs e)
        {
            try
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);

                DocsPaWR.Corrispondente corr = null;

                CustomImageButton img = (CustomImageButton)sender;
                string systemId = img.Attributes["RowIndex"];
                string addressCode = img.Attributes["CodiceRubrica"];

                bool isTypeListOrRf = (from row in GrdAddressBookResult.Rows.Cast<GridViewRow>()
                                       where (row.FindControl("imgDetail") as CustomImageButton).Attributes["RowIndex"].Equals(systemId)
                                       select (row.FindControl("imgType") as CustomImageButton).ImageUrl.Contains("users_list_icon.png")
                                                || (row.FindControl("imgType") as CustomImageButton).ImageUrl.Contains("rf_icon.png")).FirstOrDefault();

                ArrayList lsCorr = new ArrayList();
                if (this.EnableDistributionLists && isTypeListOrRf)
                {
                    string idAmm = UIManager.UserManager.GetInfoUser().idAmministrazione;
                    lsCorr = UIManager.AddressBookManager.getCorrispondentiByCodLista(addressCode, idAmm);
                    if (lsCorr.Count != 0)
                    {
                        corr = new DocsPaWR.Corrispondente();
                        corr.codiceRubrica = addressCode;
                        corr.descrizione = UIManager.AddressBookManager.getNomeLista(addressCode, idAmm);
                        corr.tipoCorrispondente = "L";
                    }
                    else
                    {
                        lsCorr = UIManager.AddressBookManager.getCorrispondentiByCodRF(addressCode);
                        if (lsCorr.Count != 0)
                        {
                            corr = new DocsPaWR.Corrispondente();
                            corr.codiceRubrica = addressCode;
                            corr.descrizione = UIManager.AddressBookManager.getNomeRF(addressCode);
                            corr.tipoCorrispondente = "F";
                        }
                        else
                        {
                            corr = UIManager.AddressBookManager.GetCorrespondentBySystemId(systemId);
                            if (corr == null || string.IsNullOrEmpty(corr.systemId))
                            {
                                corr = UIManager.AddressBookManager.getCorrispondenteByCodRubricaRubricaComune(addressCode);
                            }

                        }
                    }
                }
                else
                {
                    corr = UIManager.AddressBookManager.GetCorrespondentBySystemId(systemId);
                    if (corr == null || string.IsNullOrEmpty(corr.systemId))
                    {
                        corr = UIManager.AddressBookManager.getCorrispondenteByCodRubricaRubricaComune(addressCode);
                    }
                }

                Session["AddressBook_details_corr"] = corr;
                ScriptManager.RegisterStartupScript(this, this.GetType(), "AddressBook_Details", "ajaxModalPopupAddressBook_Details();", true);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void AddressBookNew_Click(object sender, EventArgs e)
        {
            try
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "AddressBook_New", "ajaxModalPopupAddressBook_New();", true);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void AddressBookDownloadTemplate_Click(object sender, EventArgs e)
        {
            //Response.Redirect("../ExportDati/exportCorrispondenti.xls");
            Response.Redirect("../ExportDati/DownloadCorrispondenti.aspx");
        }

        #region PropertySession

        private bool AaTableVisible
        {
            get
            {
                if (HttpContext.Current.Session["aaTableVisible"] != null)
                {
                    return (bool)HttpContext.Current.Session["aaTableVisible"];
                }
                else
                {
                    return false;
                }
            }
            set
            {
                HttpContext.Current.Session["aaTableVisible"] = value;
            }
        }

        private bool AddDoc
        {
            get
            {
                if (HttpContext.Current.Session["AddDocInProject"] != null)
                {
                    return (bool)HttpContext.Current.Session["AddDocInProject"];
                }
                else
                {
                    return false;
                }
            }
        }
        private bool FromMassive
        {
            get
            {
                if (HttpContext.Current.Session["FromMassive"] != null)
                {
                    return (bool)HttpContext.Current.Session["FromMassive"];
                }
                else
                {
                    return false;
                }
            }
        }

        private bool ManagementTransmission
        {
            get
            {
                if (HttpContext.Current.Session["fromFindModelli"] != null)
                {
                    return (bool)HttpContext.Current.Session["fromFindModelli"];
                }
                else
                {
                    return false;
                }
            }
        }

        private bool FromInstanceAccess
        {
            get
            {
                if (HttpContext.Current.Session["fromInstanceAccess"] != null)
                {
                    return (bool)HttpContext.Current.Session["fromInstanceAccess"];
                }
                else
                {
                    return false;
                }
            }
            set
            {
                HttpContext.Current.Session["fromInstanceAccess"] = value;
            }
        }

        private bool FromNewProject
        {
            get
            {
                if (HttpContext.Current.Session["FromNewProject"] != null)
                {
                    return (bool)HttpContext.Current.Session["FromNewProject"];
                }
                else
                {
                    return false;
                }
            }
            set
            {
                HttpContext.Current.Session["FromNewProject"] = value;
            }
        }

        private bool EnableCommonAddressBook
        {
            get
            {
                if (HttpContext.Current.Session["enableCommonAddressBook"] != null)
                    return bool.Parse(HttpContext.Current.Session["enableCommonAddressBook"].ToString());
                else return false;
            }
            set
            {
                HttpContext.Current.Session["enableCommonAddressBook"] = value;
            }

        }

        public RubricaCallType CallType
        {
            get
            {
                if (HttpContext.Current.Session["callType"] != null)
                    return (RubricaCallType)HttpContext.Current.Session["callType"];
                else return RubricaCallType.CALLTYPE_PROTO_IN;
            }
            set
            {
                HttpContext.Current.Session["callType"] = value;
            }

        }

        private bool IsOrganigramMode
        {
            get
            {
                if (HttpContext.Current.Session["isOrganigramMode"] != null)
                    return bool.Parse(HttpContext.Current.Session["isOrganigramMode"].ToString());
                else return false;
            }
            set
            {
                HttpContext.Current.Session["isOrganigramMode"] = value;
            }

        }

        private bool IsTotalOrganigram
        {
            get
            {
                if (HttpContext.Current.Session["IsTotalOrganigram"] != null)
                    return bool.Parse(HttpContext.Current.Session["IsTotalOrganigram"].ToString());
                else return true;
            }
            set
            {
                HttpContext.Current.Session["IsTotalOrganigram"] = value;
            }

        }

        private bool EnableDistributionLists
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["enableDistributionLists"] != null)
                {
                    result = bool.Parse(HttpContext.Current.Session["enableDistributionLists"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["enableDistributionLists"] = value;
            }
        }

        public string IdCustomObjectCustomCorrespondent
        {
            get
            {
                string result = string.Empty;
                if (HttpContext.Current.Session["idCustomObjectCustomCorrespondent"] != null)
                {
                    result = HttpContext.Current.Session["idCustomObjectCustomCorrespondent"] as string;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["idCustomObjectCustomCorrespondent"] = value;
            }
        }

        #endregion

        #region debug
        private void debug_callType(RubricaCallType calty)
        {
            switch (calty)
            {
                case RubricaCallType.CALLTYPE_PROTO_IN:
                    this.lbldebug_CallType.Text = "CALLTYPE_PROTO_IN"; break;
                case RubricaCallType.CALLTYPE_PROTO_IN_INT:
                    this.lbldebug_CallType.Text = "CALLTYPE_PROTO_IN_INT"; break;
                case RubricaCallType.CALLTYPE_PROTO_OUT:
                    this.lbldebug_CallType.Text = "CALLTYPE_PROTO_OUT"; break;
                case RubricaCallType.CALLTYPE_TRASM_INF:
                    this.lbldebug_CallType.Text = "CALLTYPE_TRASM_INF"; break;
                case RubricaCallType.CALLTYPE_TRASM_SUP:
                    this.lbldebug_CallType.Text = "CALLTYPE_TRASM_SUP"; break;
                case RubricaCallType.CALLTYPE_TRASM_ALL:
                    this.lbldebug_CallType.Text = "CALLTYPE_TRASM_ALL"; break;
                case RubricaCallType.CALLTYPE_MANAGE:
                    this.lbldebug_CallType.Text = "CALLTYPE_MANAGE"; break;
                case RubricaCallType.CALLTYPE_PROTO_OUT_MITT:
                    this.lbldebug_CallType.Text = "CALLTYPE_PROTO_OUT_MITT"; break;
                case RubricaCallType.CALLTYPE_PROTO_INT_MITT:
                    this.lbldebug_CallType.Text = "CALLTYPE_PROTO_INT_MITT"; break;
                case RubricaCallType.CALLTYPE_PROTO_INGRESSO:
                    this.lbldebug_CallType.Text = "CALLTYPE_PROTO_INGRESSO"; break;
                case RubricaCallType.CALLTYPE_UFFREF_PROTO:
                    this.lbldebug_CallType.Text = "CALLTYPE_UFFREF_PROTO"; break;
                case RubricaCallType.CALLTYPE_GESTFASC_LOCFISICA:
                    this.lbldebug_CallType.Text = "CALLTYPE_GESTFASC_LOCFISICA"; break;
                case RubricaCallType.CALLTYPE_GESTFASC_UFFREF:
                    this.lbldebug_CallType.Text = "CALLTYPE_GESTFASC_UFFREF"; break;
                case RubricaCallType.CALLTYPE_EDITFASC_LOCFISICA:
                    this.lbldebug_CallType.Text = "CALLTYPE_EDITFASC_LOCFISICA"; break;
                case RubricaCallType.CALLTYPE_EDITFASC_UFFREF:
                    this.lbldebug_CallType.Text = "CALLTYPE_EDITFASC_UFFREF"; break;
                case RubricaCallType.CALLTYPE_NEWFASC_LOCFISICA:
                    this.lbldebug_CallType.Text = "CALLTYPE_NEWFASC_LOCFISICA"; break;
                case RubricaCallType.CALLTYPE_NEWFASC_UFFREF:
                    this.lbldebug_CallType.Text = "CALLTYPE_NEWFASC_UFFREF"; break;
                case RubricaCallType.CALLTYPE_RICERCA_MITTDEST:
                    this.lbldebug_CallType.Text = "CALLTYPE_RICERCA_MITTDEST"; break;
                case RubricaCallType.CALLTYPE_RICERCA_UFFREF:
                    this.lbldebug_CallType.Text = "CALLTYPE_RICERCA_UFFREF"; break;
                case RubricaCallType.CALLTYPE_RICERCA_MITTINTERMEDIO:
                    this.lbldebug_CallType.Text = "CALLTYPE_RICERCA_MITTINTERMEDIO"; break;
                case RubricaCallType.CALLTYPE_RICERCA_ESTESA:
                    this.lbldebug_CallType.Text = "CALLTYPE_RICERCA_ESTESA"; break;
                case RubricaCallType.CALLTYPE_RICERCA_COMPLETAMENTO:
                    this.lbldebug_CallType.Text = "CALLTYPE_RICERCA_COMPLETAMENTO"; break;
                case RubricaCallType.CALLTYPE_RICERCA_TRASM:
                    this.lbldebug_CallType.Text = "CALLTYPE_RICERCA_TRASM"; break;
                case RubricaCallType.CALLTYPE_PROTO_INT_DEST:
                    this.lbldebug_CallType.Text = "CALLTYPE_PROTO_INT_DEST"; break;
                case RubricaCallType.CALLTYPE_FILTRIRICFASC_UFFREF:
                    this.lbldebug_CallType.Text = "CALLTYPE_FILTRIRICFASC_UFFREF"; break;
                case RubricaCallType.CALLTYPE_FILTRIRICFASC_LOCFIS:
                    this.lbldebug_CallType.Text = "CALLTYPE_FILTRIRICFASC_LOCFIS"; break;
                case RubricaCallType.CALLTYPE_RICERCA_DOCUMENTI:
                    this.lbldebug_CallType.Text = "CALLTYPE_RICERCA_DOCUMENTI"; break;
                case RubricaCallType.CALLTYPE_RICERCA_DOCUMENTI_CORR_INT:
                    this.lbldebug_CallType.Text = "CALLTYPE_RICERCA_DOCUMENTI_CORR_INT"; break;
                case RubricaCallType.CALLTYPE_LISTE_DISTRIBUZIONE:
                    this.lbldebug_CallType.Text = "CALLTYPE_LISTE_DISTRIBUZIONE"; break;
                case RubricaCallType.CALLTYPE_TRASM_PARILIVELLO:
                    this.lbldebug_CallType.Text = "CALLTYPE_TRASM_PARILIVELLO"; break;
                case RubricaCallType.CALLTYPE_MITT_MODELLO_TRASM:
                    this.lbldebug_CallType.Text = "CALLTYPE_MITT_MODELLO_TRASM"; break;
                case RubricaCallType.CALLTYPE_MODELLI_TRASM_ALL:
                    this.lbldebug_CallType.Text = "CALLTYPE_MODELLI_TRASM_ALL"; break;
                case RubricaCallType.CALLTYPE_MODELLI_TRASM_INF:
                    this.lbldebug_CallType.Text = "CALLTYPE_MODELLI_TRASM_INF"; break;
                case RubricaCallType.CALLTYPE_MODELLI_TRASM_SUP:
                    this.lbldebug_CallType.Text = "CALLTYPE_MODELLI_TRASM_SUP"; break;
                case RubricaCallType.CALLTYPE_MODELLI_TRASM_PARILIVELLO:
                    this.lbldebug_CallType.Text = "CALLTYPE_MODELLI_TRASM_PARILIVELLO"; break;
                case RubricaCallType.CALLTYPE_DEST_MODELLO_TRASM:
                    this.lbldebug_CallType.Text = "CALLTYPE_DEST_MODELLO_TRASM"; break;
                case RubricaCallType.CALLTYPE_ORGANIGRAMMA:
                    this.lbldebug_CallType.Text = "CALLTYPE_ORGANIGRAMMA"; break;
                case RubricaCallType.CALLTYPE_ORGANIGRAMMA_TOTALE_PROTOCOLLO:
                    this.lbldebug_CallType.Text = "CALLTYPE_ORGANIGRAMMA_TOTALE_PROTOCOLLO"; break;
                case RubricaCallType.CALLTYPE_STAMPA_REG_UO:
                    this.lbldebug_CallType.Text = "CALLTYPE_STAMPA_REG_UO"; break;
                case RubricaCallType.CALLTYPE_ORGANIGRAMMA_TOTALE:
                    this.lbldebug_CallType.Text = "CALLTYPE_ORGANIGRAMMA_TOTALE"; break;
                case RubricaCallType.CALLTYPE_ORGANIGRAMMA_INTERNO:
                    this.lbldebug_CallType.Text = "CALLTYPE_ORGANIGRAMMA_INTERNO"; break;
                case RubricaCallType.CALLTYPE_RICERCA_TRASM_TODOLIST:
                    this.lbldebug_CallType.Text = "CALLTYPE_RICERCA_TRASM_TODOLIST"; break;
                case RubricaCallType.CALLTYPE_RUOLO_REG_NOMAIL:
                    this.lbldebug_CallType.Text = "CALLTYPE_RUOLO_REG_NOMAIL"; break;
                case RubricaCallType.CALLTYPE_UTENTE_REG_NOMAIL:
                    this.lbldebug_CallType.Text = "CALLTYPE_UTENTE_REG_NOMAIL"; break;
                case RubricaCallType.CALLTYPE_PROTO_USCITA_SEMPLIFICATO:
                    this.lbldebug_CallType.Text = "CALLTYPE_PROTO_USCITA_SEMPLIFICATO"; break;
                case RubricaCallType.CALLTYPE_PROTO_OUT_MITT_SEMPLIFICATO:
                    this.lbldebug_CallType.Text = "CALLTYPE_PROTO_OUT_MITT_SEMPLIFICATO"; break;
                case RubricaCallType.CALLTYPE_CORR_INT:
                    this.lbldebug_CallType.Text = "CALLTYPE_CORR_INT"; break;
                case RubricaCallType.CALLTYPE_CORR_EST:
                    this.lbldebug_CallType.Text = "CALLTYPE_CORR_EST"; break;
                case RubricaCallType.CALLTYPE_CORR_EST_CON_DISABILITATI:
                    this.lbldebug_CallType.Text = "CALLTYPE_CORR_EST_CON_DISABILITATI"; break;
                case RubricaCallType.CALLTYPE_CORR_INT_EST:
                    this.lbldebug_CallType.Text = "CALLTYPE_CORR_INT_EST"; break;
                case RubricaCallType.CALLTYPE_CORR_INT_EST_CON_DISABILITATI:
                    this.lbldebug_CallType.Text = "CALLTYPE_CORR_INT_EST_CON_DISABILITATI"; break;
                case RubricaCallType.CALLTYPE_CORR_INT_CON_DISABILITATI:
                    this.lbldebug_CallType.Text = "CALLTYPE_CORR_INT_CON_DISABILITATI"; break;
                case RubricaCallType.CALLTYPE_CORR_NO_FILTRI:
                    this.lbldebug_CallType.Text = "CALLTYPE_CORR_NO_FILTRI"; break;
                case RubricaCallType.CALLTYPE_RICERCA_CREATOR:
                    this.lbldebug_CallType.Text = "CALLTYPE_RICERCA_CREATOR"; break;
                case RubricaCallType.CALLTYPE_ESTERNI_AMM:
                    this.lbldebug_CallType.Text = "CALLTYPE_ESTERNI_AMM"; break;
                case RubricaCallType.CALLTYPE_PROTO_OUT_ESTERNI:
                    this.lbldebug_CallType.Text = "CALLTYPE_PROTO_OUT_ESTERNI"; break;
                case RubricaCallType.CALLTYPE_PROTO_IN_ESTERNI:
                    this.lbldebug_CallType.Text = "CALLTYPE_PROTO_IN_ESTERNI"; break;
                case RubricaCallType.CALLTYPE_RUOLO_RESP_REG:
                    this.lbldebug_CallType.Text = "CALLTYPE_RUOLO_RESP_REG"; break;
                case RubricaCallType.CALLTYPE_RICERCA_TRASM_SOTTOPOSTO:
                    this.lbldebug_CallType.Text = "CALLTYPE_RICERCA_TRASM_SOTTOPOSTO"; break;
                case RubricaCallType.CALLTYPE_MITT_MULTIPLI:
                    this.lbldebug_CallType.Text = "CALLTYPE_MITT_MULTIPLI"; break;
                case RubricaCallType.CALLTYPE_MITT_MULTIPLI_SEMPLIFICATO:
                    this.lbldebug_CallType.Text = "CALLTYPE_MITT_MULTIPLI_SEMPLIFICATO"; break;
                case RubricaCallType.CALLTYPE_RICERCA_UO_RUOLI_SOTTOPOSTI:
                    this.lbldebug_CallType.Text = "CALLTYPE_RICERCA_UO_RUOLI_SOTTOPOSTI"; break;
                case RubricaCallType.CALLTYPE_TUTTI_RUOLI:
                    this.lbldebug_CallType.Text = "CALLTYPE_TUTTI_RUOLI"; break;
                case RubricaCallType.CALLTYPE_TUTTE_UO:
                    this.lbldebug_CallType.Text = "CALLTYPE_TUTTE_UO"; break;
                case RubricaCallType.CALLTYPE_CORR_INT_NO_UO:
                    this.lbldebug_CallType.Text = "CALLTYPE_CORR_INT_NO_UO"; break;
                case RubricaCallType.CALLTYPE_REPLACE_ROLE:
                    this.lbldebug_CallType.Text = "CALLTYPE_REPLACE_ROLE"; break;
                case RubricaCallType.CALLTYPE_DEST_FOR_SEARCH_MODELLI:
                    this.lbldebug_CallType.Text = "CALLTYPE_DEST_FOR_SEARCH_MODELLI"; break;
                case RubricaCallType.CALLTYPE_FIND_ROLE:
                    this.lbldebug_CallType.Text = "CALLTYPE_FIND_ROLE"; break;
                //IACOZZILLI GIORDANO:
                //NUOVO CALLTYPE ARCHIVE:
                case RubricaCallType.CALLTYPE_DEP_OSITO:
                    this.lbldebug_CallType.Text = "CALLTYPE_DEP_OSITO"; break;
                case RubricaCallType.CALLTYPE_OWNER_AUTHOR:
                    this.lbldebug_CallType.Text = "CALLTYPE_OWNER_AUTHOR"; break;
                case RubricaCallType.CALLTYPE_RUOLO_RESP_REPERTORI:
                    this.lbldebug_CallType.Text = "CALLTYPE_RUOLO_RESP_REPERTORI"; break;
                case RubricaCallType.CALLTYPE_RICERCA_CORRISPONDENTE:
                    this.lbldebug_CallType.Text = "CALLTYPE_RICERCA_CORRISPONDENTE"; break;
                case RubricaCallType.CALLTYPE_RICERCA_CORR_NON_STORICIZZATO:
                    this.lbldebug_CallType.Text = "CALLTYPE_RICERCA_CORR_NON_STORICIZZATO"; break;
            }

        }
        #endregion
    }
}
