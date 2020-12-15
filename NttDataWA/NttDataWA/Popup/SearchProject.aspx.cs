using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.DocsPaWR;
using NttDataWA.UIManager;
using System.Data;
using NttDataWA.Utils;
using NttDatalLibrary;
using System.Web.UI.HtmlControls;
using System.Text;
using System.Collections;

namespace NttDataWA.Popup
{
    public partial class SearchProject : System.Web.UI.Page
    {

        #region Variabili
        private NttDataWA.DocsPaWR.FiltroRicerca[][] qV;
        private NttDataWA.DocsPaWR.FiltroRicerca[] fVList;
        private NttDataWA.DocsPaWR.FiltroRicerca fV1;
        protected NttDataWA.DocsPaWR.Fascicolo[] listaFascicoli;
        private Hashtable m_hashTableFascicoli;
        private Hashtable HT_fascicoliSelezionati;
        protected int nRec = 0;
        protected int numTotPage = 0;
        private int rowForPage = 100000;
        private DocsPaWR.Corrispondente SelectedCorr = null;

        #endregion

        #region Const

        private const string START_SPECIAL_STYLE = "<span class=\"redStrike\">";
        private const string END_SPECIAL_STYLE = "</span>";

        #endregion


        #region Properties

        private SchedaDocumento DocumentInWorking
        {
            get
            {
                SchedaDocumento result = null;
                if (HttpContext.Current.Session["document"] != null)
                {
                    result = HttpContext.Current.Session["document"] as SchedaDocumento;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["document"] = value;
            }
        }

        public List<InstanceAccessDocument> ListDocumentsAccess
        {
            get
            {
                List<InstanceAccessDocument> result = new List<InstanceAccessDocument>();
                if (HttpContext.Current.Session["listDocumentsAccess"] != null)
                {
                    result = HttpContext.Current.Session["listDocumentsAccess"] as List<InstanceAccessDocument>;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["listDocumentsAccess"] = value;
            }
        }

        private DocsPaWR.Registro Registry
        {
            get
            {
                DocsPaWR.Registro result = null;
                if (HttpContext.Current.Session["registry"] != null)
                {
                    result = HttpContext.Current.Session["registry"] as DocsPaWR.Registro;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["registry"] = value;
            }
        }

        private DocsPaWR.Fascicolo Project
        {
            get
            {
                Fascicolo result = null;
                if (HttpContext.Current.Session["searchproject"] != null)
                {
                    result = HttpContext.Current.Session["searchproject"] as Fascicolo;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["searchproject"] = value;
            }
        }

        private DocsPaWR.Fascicolo ProjectClass
        {
            get
            {
                Fascicolo result = null;
                if (HttpContext.Current.Session["projectClass"] != null)
                {
                    result = HttpContext.Current.Session["projectClass"] as Fascicolo;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["projectClass"] = value;
            }
        }

        protected GridViewRow RowSelected
        {
            get
            {
                return HttpContext.Current.Session["RowSelected"] as GridViewRow;
            }
            set
            {
                HttpContext.Current.Session["RowSelected"] = value;
            }
        }

        protected int currentPage
        {
            get
            {
                int result = 1;
                if (Session["currentPage"] != null) result = (int)Session["currentPage"];
                return result;
            }
            set
            {
                Session["currentPage"] = value;
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

        /// <summary>
        /// Number of result in page
        /// </summary>
        public int PageSize
        {
            get
            {
                int result = 20;
                if (HttpContext.Current.Session["pageSizeProject"] != null)
                {
                    result = int.Parse(HttpContext.Current.Session["pageSizeProject"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["pageSizeProject"] = value;
            }
        }
        #endregion


        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.Page.IsPostBack)
            {
                this.BtnSearch.Focus();
                Session["fileExcel"] = null;
                Session["datiExcel"] = null;
                this.Template = null;
                this.CallerSearchProject = "protocollo";
                if (Request.QueryString["caller"] != null)
                {
                    this.CallerSearchProject = Request.QueryString["caller"].ToString();
                }
                else
                {
                    if (HttpContext.Current.Session["SearchProject.from"] != null)
                    {
                        this.CallerSearchProject = HttpContext.Current.Session["SearchProject.from"].ToString();
                    }
                }
                this.InitLanguage();
                this.InitPage();
                this.InitializePageSize();
                this.PopulateDDLTitolario();
                this.PopulateDDLRegistry(RoleManager.GetRoleInSession());
                this.SetAjaxDescriptionProject();
                this.SetAjaxAddressBook();
                if (ProjectManager.getProjectInSessionForRicFasc() != null)
                {
                    TxtCodeProject.Text = ProjectManager.getProjectInSessionForRicFasc();
                    TxtCodeProject_OnTextChanged(new object(), new EventArgs());
                }
                ClearAllGrid();
                ProjectManager.setHashFascicoliSelezionati(this, null);
                ProjectManager.removeHashFascicoli(this);
                this.FolderFoundCount.Text = "0";
                this.LoadTypeProjects();
                //this.TxtCodeProject.Attributes.Add("onblur", "disallowOp('Content2');");
                //this.txtCodiceProprietario.Attributes.Add("onblur", "disallowOp('Content2');");

                VisibiltyRoleFunctions();

                //Emanuela 11/07/2014: la property in basso imposta readonly il campo corrispondente
                HttpContext.Current.Session["sCodiceReadOnly"] = null;
            }
            else
            {
                ReadRetValueFromPopup();
                if (this.CustomDocuments)
                {
                    this.PnlTypeDocument.Controls.Clear();
                    if (!string.IsNullOrEmpty(this.DocumentDdlTypeDocument.SelectedValue))
                    {
                        if (this.Template == null || !this.Template.SYSTEM_ID.Equals(this.DocumentDdlTypeDocument.SelectedValue))
                        {
                            this.Template = ProfilerProjectManager.getTemplateFascById(this.DocumentDdlTypeDocument.SelectedItem.Value);
                        }
                        if (this.CustomDocuments)
                        {
                            this.PopulateProfiledDocument();
                        }
                    }
                }

                if (Session["fileExcel"] != null)
                    this.pnlMainExcelFilter.Attributes["class"] = "shown";
            }

            if (this.CallerSearchProject == null || !this.CallerSearchProject.Equals("searchInstance"))
            {
                if (DocumentInWorking != null && DocumentInWorking.protocollo != null && !String.IsNullOrEmpty(DocumentInWorking.protocollo.segnatura))
                {
                    this.divObjSegn.Visible = true;
                    this.divDataProt.Visible = true;
                    this.LblSegnatura.InnerText = DocumentInWorking.protocollo.segnatura;
                    this.LblDataSegnatura.InnerText = DocumentInWorking.protocollo.dataProtocollazione;
                    this.LblOggetto.InnerText = DocumentInWorking.oggetto.descrizione;
                }
                else if (this.CallerSearchProject != null && !this.CallerSearchProject.Equals("search") && DocumentInWorking != null && !String.IsNullOrEmpty(DocumentInWorking.systemId))
                {
                    this.divObjSegn.Visible = true;
                    this.LblOggetto.InnerText = DocumentInWorking.oggetto.descrizione;
                    this.Segnatura.Visible = false;
                }
                else
                    this.divObjSegn.Visible = false;

            }
            else
            {
                this.divObjSegn.Visible = false;
                this.divDataProt.Visible = false;
            }


            HT_fascicoliSelezionati = ProjectManager.getHashFascicoliSelezionati(this);
            m_hashTableFascicoli = ProjectManager.getHashFascicoli(this);


            foreach (GridViewRow dgItem in GrdFascResult.Rows)
            {
                if (HT_fascicoliSelezionati == null)
                    HT_fascicoliSelezionati = ProjectManager.getHashFascicoliSelezionati(this);


                if (HT_fascicoliSelezionati != null)
                {
                    CheckBox checkBox = dgItem.FindControl("cbSel") as CheckBox;
                    string key = ((Label)dgItem.FindControl("lblFascKey")).Text;

                    if (key != null && checkBox != null)
                    {
                        if (checkBox.Checked)//se è spuntato lo inserisco
                        {
                            if (!HT_fascicoliSelezionati.ContainsKey(key))
                            {
                                HT_fascicoliSelezionati.Add(key, m_hashTableFascicoli[key]);
                            }
                        }
                        else //se non è selezionato vedo se è in hashtable, in caso lo rimuovo
                        {
                            if (HT_fascicoliSelezionati.ContainsKey(key))
                            {
                                HT_fascicoliSelezionati.Remove(key);
                            }
                        }
                    }
                }

            }
            if (HT_fascicoliSelezionati != null)
                ProjectManager.setHashFascicoliSelezionati(this, HT_fascicoliSelezionati);

            //Laura rimosso 17 aprile
            //Fascicolo Fasc = ProjectManager.getProjectInSession();
            //if (Fasc != null)
            //{
            //    DocsPaWR.Folder folder = ProjectManager.getFolder(this, Fasc);
            //    this.treenode_sel.Value = "root_" + folder.systemID;
            //}

            this.RefreshScript();
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);
        }

        protected void InitializePageSize()
        {
            string keyValue = Utils.InitConfigurationKeys.GetValue(UserManager.GetInfoUser().idAmministrazione, DBKeys.FE_PAGING_ROW_PROJECT.ToString());
            int tempValue = 0;
            if (!string.IsNullOrEmpty(keyValue))
            {
                tempValue = Convert.ToInt32(keyValue);
                if (tempValue >= 20 || tempValue <= 50)
                {
                    this.PageSize = tempValue;
                }
            }
        }

        private void InitPage()
        {
            if (UserManager.FunzioneEsistente("FILTRO_FASC_EXCEL") && (!string.IsNullOrEmpty(this.CallerSearchProject) && !this.CallerSearchProject.Equals("profilo") && !this.CallerSearchProject.Equals("protocollo")))
            {
                this.plcExcelFilter.Visible = true;

                if (this.CallerSearchProject == "classifica")
                {
                    this.pnlAdvFiltersCollapse.Attributes["class"] += " shown";
                    this.pnlMainExcelFilter.Attributes["class"] += " shown";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "pnlAdvFilters", "$('#pnlAdvFilters').css('display', 'block');", true);
                }
            }
            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.ProfilazioneDinamica.ToString()]) && System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.ProfilazioneDinamica.ToString()].Equals("1"))
            {
                this.CustomDocuments = true;
            }
        }

        protected void PopulateDDLRegistry(DocsPaWR.Ruolo role)
        {
            foreach (DocsPaWR.Registro reg in role.registri)
            {
                if (!reg.flag_pregresso)
                {
                    ListItem item = new ListItem();
                    item.Text = reg.codRegistro;
                    item.Value = reg.systemId;
                    this.DdlRegistries.Items.Add(item);
                }
            }

            if (this.DdlRegistries.Items.Count == 1)
            {
                this.plcRegistry.Visible = false;
                this.UpPnlRegistry.Update();
            }

            if (this.CallerSearchProject == "protocollo")
            {
                this.DdlRegistries.SelectedValue = UIManager.RegistryManager.GetRegistryInSession().systemId;
                this.DdlRegistries.Enabled = false;
                this.UpPnlRegistry.Update();
            }

        }

        private void LoadTypeProjects()
        {
            ArrayList listaTipiFasc = new ArrayList(ProfilerProjectManager.getTipoFascFromRuolo(UserManager.GetInfoUser().idAmministrazione, RoleManager.GetRoleInSession().idGruppo, "1"));
            ListItem item = new ListItem();
            item.Value = "";
            item.Text = "";
            if (this.DocumentDdlTypeDocument.Items.Count == 0)
            {
                this.DocumentDdlTypeDocument.Items.Add(item);
            }
            for (int i = 0; i < listaTipiFasc.Count; i++)
            {
                DocsPaWR.Templates templates = (DocsPaWR.Templates)listaTipiFasc[i];
                ListItem item_1 = new ListItem();
                item_1.Value = templates.SYSTEM_ID.ToString();
                item_1.Text = templates.DESCRIZIONE;

                //Christian - Ticket OC0000001490459 - Ricerca fascicoli: ripristino tipologia successivo a ordinamento tramite griglia.
                if (this.DocumentDdlTypeDocument.Items.FindByValue(templates.SYSTEM_ID.ToString()) == null)
                {
                    if (templates.IPER_FASC_DOC == "1")
                        this.DocumentDdlTypeDocument.Items.Insert(1, item_1);
                    else
                        this.DocumentDdlTypeDocument.Items.Add(item_1);
                }

            }
        }


        protected void ReadRetValueFromPopup()
        {
            if (!string.IsNullOrEmpty(this.OpenTitolario.ReturnValue))
            {
                if (this.ReturnValue.Split('#').Length > 1)
                {
                    this.TxtCodeProject.Text = this.ReturnValue.Split('#').First();
                    this.TxtDescriptionProject.Text = this.ReturnValue.Split('#').Last();
                    this.UpPnlProject.Update();
                    TxtCodeProject_OnTextChanged(new object(), new EventArgs());
                }
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('OpenTitolario','')", true);
            }

            if (!string.IsNullOrEmpty(this.HiddenRemoveNode.Value))
            {
                this.DeleteSubset();
                this.HiddenRemoveNode.Value = string.Empty;
            }

            if (!string.IsNullOrEmpty(this.SearchSubset.ReturnValue))
            {
                ProjectManager.setHashFascicoliSelezionati(this, null);
                BtnCollate.Enabled = false;

                //Laura Rimosso 17 Aprile--non veniva più evidenziato il fascicolo ricercato
                //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('SearchSubset','')", true);
            }

            if (!string.IsNullOrEmpty(this.AddressBook.ReturnValue))
            {
                ProjectManager.setHashFascicoliSelezionati(this, null);
                BtnCollate.Enabled = false;

                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('AddressBook','')", true);
            }
            if (!string.IsNullOrEmpty(this.HiddenPublicFolder.Value))
            {
                this.HiddenPublicFolder.Value = string.Empty;
                this.HiddenContinuaFascicolazioneInPubblico.Value = "up";
                m_hashTableFascicoli = ProjectManager.getHashFascicoli(this);
                HT_fascicoliSelezionati = ProjectManager.getHashFascicoliSelezionati(this);
                this.BtnCollate_Click(null, null);
            }
        }

        private void InitLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.BtnClose.Text = Utils.Languages.GetLabelFromCode("GenericBtnClose", language);
            this.BtnCollate.Text = Utils.Languages.GetLabelFromCode("SearchProjectBtnCollate", language);
            if (Request.QueryString["fromf"] == "Instance")
            {
                this.BtnCollate.Text = Utils.Languages.GetLabelFromCode("SearchProjectBtnCollateInstance", language);
            }
            this.BtnSearch.Text = Utils.Languages.GetLabelFromCode("SearchProjectBtnSearch", language);
            this.OpenTitolario.Title = Utils.Languages.GetLabelFromCode("TitleClassificationScheme", language);
            this.AddressBook.Title = Utils.Languages.GetLabelFromCode("AddressBookTitle", language);
            this.litStruttura.Text = Utils.Languages.GetLabelFromCode("projectTreeTitle", language);
            this.litTreeExpandAll.Text = Utils.Languages.GetLabelFromCode("projectTreeExpandAll", language);
            this.litTreeCollapseAll.Text = Utils.Languages.GetLabelFromCode("projectTreeCollapseAll", language);
            //this.ddl_dtaOpen.Items.Add(new ListItem(Utils.Languages.GetLabelFromCode("ProjectSearchOpenSingleValue", language), "0"));
            //this.ddl_dtaOpen.Items.Add(new ListItem(Utils.Languages.GetLabelFromCode("ProjectSearchOpenInterval", language), "1"));
            this.dtaOpen_opt0.Text = Utils.Languages.GetLabelFromCode("VisibilityOpt0", language);
            this.dtaOpen_opt1.Text = Utils.Languages.GetLabelFromCode("VisibilityOpt1", language);
            this.dtaOpen_opt2.Text = Utils.Languages.GetLabelFromCode("VisibilityOpt2", language);
            this.dtaOpen_opt3.Text = Utils.Languages.GetLabelFromCode("VisibilityOpt3", language);
            this.dtaOpen_opt4.Text = Utils.Languages.GetLabelFromCode("VisibilityOpt4", language);
            //this.ddl_dtaClosed.Items.Add(new ListItem(Utils.Languages.GetLabelFromCode("ProjectSearchClosedSingleValue", language), "0"));
            //this.ddl_dtaClosed.Items.Add(new ListItem(Utils.Languages.GetLabelFromCode("ProjectSearchClosedInterval", language), "1"));
            this.dtaClosed_opt0.Text = Utils.Languages.GetLabelFromCode("VisibilityOpt0", language);
            this.dtaClosed_opt1.Text = Utils.Languages.GetLabelFromCode("VisibilityOpt1", language);
            this.dtaClosed_opt2.Text = Utils.Languages.GetLabelFromCode("VisibilityOpt2", language);
            this.dtaClosed_opt3.Text = Utils.Languages.GetLabelFromCode("VisibilityOpt3", language);
            this.dtaClosed_opt4.Text = Utils.Languages.GetLabelFromCode("VisibilityOpt4", language);
            //this.ddl_dtaCreated.Items.Add(new ListItem(Utils.Languages.GetLabelFromCode("ProjectSearchCreatedSingleValue", language), "0"));
            //this.ddl_dtaCreated.Items.Add(new ListItem(Utils.Languages.GetLabelFromCode("ProjectSearchCreatedInterval", language), "1"));
            this.dtaCreate_opt0.Text = Utils.Languages.GetLabelFromCode("VisibilityOpt0", language);
            this.dtaCreate_opt1.Text = Utils.Languages.GetLabelFromCode("VisibilityOpt1", language);
            this.dtaCreate_opt2.Text = Utils.Languages.GetLabelFromCode("VisibilityOpt2", language);
            this.dtaCreate_opt3.Text = Utils.Languages.GetLabelFromCode("VisibilityOpt3", language);
            this.dtaCreate_opt4.Text = Utils.Languages.GetLabelFromCode("VisibilityOpt4", language);
            //this.GrdFascResult.Columns[0].HeaderText = Utils.Languages.GetLabelFromCode("FascRapidResult_Sel", language);
            //this.GrdFascResult.Columns[1].HeaderText = Utils.Languages.GetLabelFromCode("FascRapidResult_Sel", language);
            this.GrdFascResult.Columns[3].HeaderText = Utils.Languages.GetLabelFromCode("FascRapidResult_Code", language);
            this.GrdFascResult.Columns[4].HeaderText = Utils.Languages.GetLabelFromCode("FascRapidResult_Description", language);
            this.GrdFascResult.Columns[5].HeaderText = Utils.Languages.GetLabelFromCode("FascRapidResult_DtaOpen", language);
            this.litCollocation.Text = Utils.Languages.GetLabelFromCode("SearchProjectCollocation", language);
            this.lit_dtaCollocation.Text = Utils.Languages.GetLabelFromCode("SearchProjectDtaCollocation", language);
            this.litCollocationAddr.Text = Utils.Languages.GetLabelFromCode("SearchProjectCollocationAddr", language);
            this.dtaCollocation_opt0.Text = Utils.Languages.GetLabelFromCode("VisibilityOpt0", language);
            this.dtaCollocation_opt1.Text = Utils.Languages.GetLabelFromCode("VisibilityOpt1", language);
            this.dtaCollocation_opt2.Text = Utils.Languages.GetLabelFromCode("VisibilityOpt2", language);
            this.dtaCollocation_opt3.Text = Utils.Languages.GetLabelFromCode("VisibilityOpt3", language);
            this.dtaCollocation_opt4.Text = Utils.Languages.GetLabelFromCode("VisibilityOpt4", language);
            this.chkConservation.Text = Utils.Languages.GetLabelFromCode("SearchProjectConservationYes", language);
            this.chkConservationNo.Text = Utils.Languages.GetLabelFromCode("SearchProjectConservationNo", language);
            this.litViewAll.Text = Utils.Languages.GetLabelFromCode("SearchProjectViewAll", language);
            this.rbViewAllYes.Text = Utils.Languages.GetLabelFromCode("SearchProjectViewAllYes", language);
            this.rbViewAllNo.Text = Utils.Languages.GetLabelFromCode("SearchProjectViewAllNo", language);
            this.dtaExpire_opt0.Text = Utils.Languages.GetLabelFromCode("VisibilityOpt0", language);
            this.dtaExpire_opt1.Text = Utils.Languages.GetLabelFromCode("VisibilityOpt1", language);
            this.dtaExpire_opt2.Text = Utils.Languages.GetLabelFromCode("VisibilityOpt2", language);
            this.dtaExpire_opt3.Text = Utils.Languages.GetLabelFromCode("VisibilityOpt3", language);
            this.dtaExpire_opt4.Text = Utils.Languages.GetLabelFromCode("VisibilityOpt4", language);
            this.SearchDocumentLitTypology.Text = Utils.Languages.GetLabelFromCode("SearchProjectLitTypology", language);
            this.opt_StateConditionEquals.Text = Utils.Languages.GetLabelFromCode("SearchProjectStateConditionEquals", language);
            this.opt_StateConditionUnequals.Text = Utils.Languages.GetLabelFromCode("SearchProjectStateConditionUnequals", language);
            this.DocumentDdlStateDiagram.Attributes.Add("data-placeholder", Utils.Languages.GetLabelFromCode("DocumentDdlStateDiagram", language));
            this.lit_dtaCreate.Text = Utils.Languages.GetLabelFromCode("SearchProjectDtaCreate", language);
            this.lit_dtaOpen.Text = Utils.Languages.GetLabelFromCode("SearchProjectDtaOpen", language);
            this.lit_dtaClose.Text = Utils.Languages.GetLabelFromCode("SearchProjectDtaClose", language);
            this.litStatus.Text = Utils.Languages.GetLabelFromCode("SearchProjectStatus", language);
            this.ddlStatus.Attributes["data-placeholder"] = Utils.Languages.GetLabelFromCode("GenericChosenSelectOne", language);
            this.opt_statusA.Text = Utils.Languages.GetLabelFromCode("SearchProjectStatusOptA", language);
            this.opt_statusC.Text = Utils.Languages.GetLabelFromCode("SearchProjectStatusOptC", language);
            this.chkInADL.Text = Utils.Languages.GetLabelFromCode("AnswerSearchDocChkADL", language);
            this.ltrCode.Text = Utils.Languages.GetLabelFromCode("SearchProjectCodeCLa", language);
            this.LtrDescr.Text = Utils.Languages.GetLabelFromCode("projectLblDescrizione", language);
            this.ltrTipoFasc.Text = Utils.Languages.GetLabelFromCode("SearchProjectType", language);
            this.litSubset.Text = Utils.Languages.GetLabelFromCode("SearchProjectSubset", language);
            this.LitProprietario.Text = Utils.Languages.GetLabelFromCode("SearchProjectOwner", language);
            this.LitNote.Text = Utils.Languages.GetLabelFromCode("SearchProjectNotes", language);
            this.opt_typeG.Text = Utils.Languages.GetLabelFromCode("SearchProjectTypeOptG", language);
            this.opt_typeP.Text = Utils.Languages.GetLabelFromCode("SearchProjectTypeOptP", language);
            this.litNum.Text = Utils.Languages.GetLabelFromCode("SearchProjectNum", language);
            this.litYear.Text = Utils.Languages.GetLabelFromCode("SearchProjectYear", language);
            this.ltlCreator.Text = Utils.Languages.GetLabelFromCode("SearchProjectCreator", language);
            this.ltlDtaExpire.Text = Utils.Languages.GetLabelFromCode("SearchProjectDtaExpire", language);
            this.DocumentDdlTypeDocument.Attributes.Add("data-placeholder", Utils.Languages.GetLabelFromCode("ProjectDdlTypeProject", language));
            this.ddl_typeProject.Attributes["data-placeholder"] = Utils.Languages.GetLabelFromCode("GenericChosenSelectOne", language);
            this.DdlRegistries.Attributes["data-placeholder"] = Utils.Languages.GetLabelFromCode("GenericChosenSelectOne", language);
            this.ddl_attributo.Attributes.Add("data-placeholder", Utils.Languages.GetLabelFromCode("GenericChosenSelectOne", language));
            this.ddl_tipoFascExcel.Attributes.Add("data-placeholder", Utils.Languages.GetLabelFromCode("GenericChosenSelectOne", language));
            this.ddl_attrTipoFascExcel.Attributes.Add("data-placeholder", Utils.Languages.GetLabelFromCode("GenericChosenSelectOne", language));
            this.litRegistry.Text = Utils.Languages.GetLabelFromCode("SearchProjectRegistry", language);
            this.LitPopupSearchProjectFound.Text = Utils.Languages.GetLabelFromCode("LitPopupSearchProjectFound", language);
            this.LitPopupSearchProjectFound2.Text = Utils.Languages.GetLabelFromCode("LitPopupSearchProjectFound2", language);
            this.LitPopupSearchProjectFilters.Text = Utils.Languages.GetLabelFromCode("LitPopupSearchProjectFilters", language);
            this.LitSearchProjectPopupObject.Text = Utils.Languages.GetLabelFromCode("LitSearchProjectPopupObject", language);
            this.LitSearchProjectPopupSignature.Text = Utils.Languages.GetLabelFromCode("LitSearchProjectPopupSignature", language);
            this.LitSerachProkjectPopupDateSignature.Text = Utils.Languages.GetLabelFromCode("LitSerachProkjectPopupDateSignature", language);
            this.DocumentImgSenderAddressBook.AlternateText = Utils.Languages.GetLabelFromCode("DocumentImgCollocationAddressBook", language);
            this.DocumentImgSenderAddressBook.ToolTip = Utils.Languages.GetLabelFromCode("DocumentImgCollocationAddressBook", language);
            this.lblInit_DtaOpen.Text = Utils.Languages.GetLabelFromCode("VisibilityOneField", language);
            this.lblInit_DtaClosed.Text = Utils.Languages.GetLabelFromCode("VisibilityOneField", language);
            this.lblInit_DtaCreated.Text = Utils.Languages.GetLabelFromCode("VisibilityOneField", language);
            this.lbl_dtaCollocationFrom.Text = Utils.Languages.GetLabelFromCode("VisibilityOneField", language);
            this.DocumentImgCollocationAddressBook.AlternateText = Utils.Languages.GetLabelFromCode("DocumentImgCollocationAddressBook", language);
            this.DocumentImgCollocationAddressBook.ToolTip = Utils.Languages.GetLabelFromCode("DocumentImgCollocationAddressBook", language);
            this.ImgCreatoreAddressBook.AlternateText = Utils.Languages.GetLabelFromCode("DocumentImgCollocationAddressBook", language);
            this.ImgCreatoreAddressBook.ToolTip = Utils.Languages.GetLabelFromCode("DocumentImgCollocationAddressBook", language);
            this.lbl_dtaExpireFrom.Text = Utils.Languages.GetLabelFromCode("VisibilityOneField", language);
            this.optUO.Text = Utils.Languages.GetLabelFromCode("RblSearchProjectPopupoptUO", language);
            this.optRole.Text = Utils.Languages.GetLabelFromCode("RblSearchProjectPopupoptRole", language);
            this.optUser.Text = Utils.Languages.GetLabelFromCode("RblSearchProjectPopupoptPeople", language);
            this.litConfirmMoveFolder.Text = Utils.Languages.GetLabelFromCode("projectConfirmMoveFolder", language);

            this.optUOC.Text = Utils.Languages.GetLabelFromCode("RblSearchProjectPopupoptUO", language);
            this.optRoleC.Text = Utils.Languages.GetLabelFromCode("RblSearchProjectPopupoptRole", language);
            this.optUserC.Text = Utils.Languages.GetLabelFromCode("RblSearchProjectPopupoptPeople", language);

            if (!string.IsNullOrEmpty(this.CallerSearchProject) && (this.CallerSearchProject.Equals("custom") || this.CallerSearchProject.Equals("searchInstance")))
            {
                this.BtnCollate.Text = Utils.Languages.GetLabelFromCode("BtnCollateSearchProjectCustom", language);
            }

            this.litExcelFilter.Text = Utils.Languages.GetLabelFromCode("SearchProjectExcelFilterTitle", language);
            this.litExcelFilterFile.Text = Utils.Languages.GetLabelFromCode("SearchProjectExcelFilterFile", language);
            this.lbl_fileExcel.Text = Utils.Languages.GetLabelFromCode("SearchProjectExcelFilterNoFileUploaded", language);
            this.lbl_attributo.Text = Utils.Languages.GetLabelFromCode("SearchProjectExcelFilterAttribute", language);
            this.lbl_typeFasc.Text = Utils.Languages.GetLabelFromCode("SearchProjectExcelFilterTypeFasc", language);
            this.lbl_attrTypeFasc.Text = Utils.Languages.GetLabelFromCode("SearchProjectExcelFilterAttrTypeFasc", language);
            this.ddl_attributo.Items[1].Text = Utils.Languages.GetLabelFromCode("SearchProjectExcelFilterAttributeItem1", language);
            this.ddl_attributo.Items[2].Text = Utils.Languages.GetLabelFromCode("SearchProjectExcelFilterAttributeItem2", language);
            this.ddl_attributo.Items[3].Text = Utils.Languages.GetLabelFromCode("SearchProjectExcelFilterAttributeItem3", language);
            this.ddl_attributo.Items[4].Text = Utils.Languages.GetLabelFromCode("SearchProjectExcelFilterAttributeItem4", language);
            this.ddl_attributo.Items[5].Text = Utils.Languages.GetLabelFromCode("SearchProjectExcelFilterAttributeItem5", language);
            this.litTitolario.Text = Utils.Languages.GetLabelFromCode("SearchProjectTitolario", language);
            this.ddl_attributo.Attributes["data-placeholder"] = Utils.Languages.GetLabelFromCode("SearchProjectExcelFilterAttributeSelect", language);

            this.CreateNode.Title = Utils.Languages.GetLabelFromCode("ProjectDataentryFolderTitle", language);
            this.ModifyNode.Title = Utils.Languages.GetLabelFromCode("ProjectModifyFolderTitle", language);
            this.SearchSubset.Title = Utils.Languages.GetLabelFromCode("ProjectSearchSubsetTitle", language);

            this.LitPopupSearchProjectSearch.Text = Utils.Languages.GetLabelFromCode("LitPopupSearchProjectSearch", language);
            this.btnclassificationschema.ToolTip = Utils.Languages.GetLabelFromCode("LitPopupSearchProjectTit", language);
            this.optNoteAny.Text = Utils.Languages.GetLabelFromCode("SearchProjectNotesOptAny", language);
            this.optNoteAll.Text = Utils.Languages.GetLabelFromCode("SearchProjectNotesOptAll", language);
            this.optNoteRole.Text = Utils.Languages.GetLabelFromCode("SearchProjectNotesOptRole", language);
            this.optNoteRF.Text = Utils.Languages.GetLabelFromCode("SearchProjectNotesOptRF", language);
            this.optNotePersonal.Text = Utils.Languages.GetLabelFromCode("SearchProjectNotesOptPersonal", language);

            this.SearchProjectBtnDownload.Attributes["title"] = Utils.Languages.GetLabelFromCode("SearchProjectExcelFilterBtnDownload", language);
            this.UploadBtn.Text = Utils.Languages.GetLabelFromCode("SearchProjectExcelFilterBtnUpload", language);
        }

        private void RefreshScript()
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshTipsy", "tooltipTipsy();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshSelect", "refreshSelect();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshPicker", "DatePicker('" + UIManager.UserManager.GetLanguageData() + "');", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "JsTree", "JsTree();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "res", "resizeDiv();", true);
        }

        private void ClearAllGrid()
        {
            //resetAllSessionElement();
            this.CreateEmptyRow(GrdFascResult, 8);
            //this.UpPnlTreeResult.Visible = false;
            //UpPnlTreeResult.Update();
            //UpPnlGridResult.Update();
            //UpPnlGridAt.Update();
            //UpPnlGridCc.Update();
        }

        private void CreateEmptyRow(GridView gridObject, int totalRow)
        {
            ArrayList fascicoli = new ArrayList();

            for (int i = 0; i <= totalRow; i++)
            {
                RicercaFascicoliPerFascicolazioneRapida newElement = new RicercaFascicoliPerFascicolazioneRapida("", "", "", "", "", "", "", "");

                fascicoli.Add(newElement);
            }

            gridObject.DataSource = fascicoli;
            gridObject.DataBind();
            UpPnlGridResult.Update();

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

        public class RicercaFascicoliPerFascicolazioneRapida
        {

            private string tipo;
            private string codice;
            private string desc;
            private string apertura;
            private string chiusura;
            private string chiave;
            private string stato;
            private string registro;



            public RicercaFascicoliPerFascicolazioneRapida(string tipo, string codice, string desc, string apertura, string chiusura, string chiave, string stato, string registro)
            {

                //this.tipo = tipo;
                this.codice = codice;
                this.desc = desc;
                this.apertura = apertura;
                this.chiusura = chiusura;
                this.chiave = chiave;
                //this.stato = stato;
                //this.registro = registro;
            }


            public RicercaFascicoliPerFascicolazioneRapida()
            {

                //this.tipo = tipo;
                this.codice = "";
                this.desc = "";
                this.apertura = "";
                this.chiusura = "";
                this.chiave = "";
                //this.stato = stato;
                //this.registro = registro;
            }

            public string Tipo { get { return tipo; } }
            public string Codice { get { return codice; } }
            public string Descrizione { get { return desc; } }
            public string FascDataOpen { get { return apertura; } }
            public string FascDataClose { get { return chiusura; } }
            public string Chiave { get { return chiave; } }
            public string Stato { get { return stato; } }
            public string Registro { get { return registro; } }
        }

        protected void BtnSearch_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);
            currentPage = 1;
            if (!ricercaFascicoli())
            {

            }
            else
                LoadData(false, currentPage, nRec, numTotPage);
        }

        protected void BtnCollate_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);
            DocsPaWR.SearchObject fasc = null;
            DocsPaWR.Fascicolo prj = null;
            DocsPaWR.Folder selectedFolder = null;
            string returnValue = String.Empty;
            string key;
            bool grid = false;
            string codice = "";

            if (this.CallerSearchProject != null && (this.CallerSearchProject == "profilo" || this.CallerSearchProject == "protocollo"))
            {
                if (this.Project != null && (HttpContext.Current.Session["ReturnValuePopup"] == null || !HttpContext.Current.Session["ReturnValuePopup"].ToString().Contains("//")))
                {
                    returnValue = this.Project.codice + "#" + this.Project.descrizione;
                    HttpContext.Current.Session["ReturnValuePopup"] = returnValue;
                }

                bool avanza = this.verificaSelezione("profilo", out key, out grid);
                string msgDesc = "";
                if (avanza)
                {
                    //if (String.IsNullOrEmpty(DocumentInWorking.systemId))
                    //{
                    if (grid)
                    {
                        m_hashTableFascicoli = ProjectManager.getHashFascicoli(this);

                        fasc = (DocsPaWR.SearchObject)m_hashTableFascicoli[key];
                        prj = GetProjectFormSearchObject(fasc);
                        returnValue = prj.codice + "#" + prj.descrizione;
                        HttpContext.Current.Session["ReturnValuePopup"] = returnValue;
                    }
                    ProjectManager.setHashFascicoli(this, null);
                    ProjectManager.setHashFascicoliSelezionati(this, null);
                    this.CloseMask(true);
                    //}
                    //else
                    //{
                    //    string accessRight = DocumentManager.getAccessRightDocBySystemID(DocumentInWorking.systemId, UIManager.UserManager.GetInfoUser());
                    //    if (accessRight.Equals("20"))
                    //    {
                    //        //senza chiudere la maschera
                    //        msgDesc = "ResultProjectDocLocked";
                    //        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');}", true);
                    //        return;
                    //    }

                    //    //if (grid)
                    //    //{
                    //    //    //fasc = ProjectManager.getFascicoloById(this, systemId);
                    //    //    m_hashTableFascicoli = ProjectManager.getHashFascicoli(this);
                    //    //    fasc = (DocsPaWR.SearchObject)m_hashTableFascicoli[key];
                    //    //    prj = GetProjectFormSearchObject(fasc);
                    //    //    codice = prj.codice.Replace(@"\", @"\\");
                    //    //}
                    //    //else
                    //    //{
                    //    //    selectedFolder = ProjectManager.getFolder(this, key);
                    //    //    prj = ProjectManager.getFascicoloById(this, selectedFolder.idFascicolo);
                    //    //    codice = prj.codice.Replace(@"\", @"\\");
                    //    //}




                    //    bool control = true;

                    //    fasc = (DocsPaWR.SearchObject)m_hashTableFascicoli[key];
                    //    prj = GetProjectFormSearchObject(fasc);

                    //    bool isInFolder = false;

                    //    if (prj != null)
                    //    {
                    //        //se lo stato del fascicolo è chiuso non si deve inserire il documento. Non fa return, segnala l'errore alla fine
                    //        if (prj.stato == "C")
                    //        {
                    //            singleMessage = Languages.GetMessageFromCode("ResultProjectClosed", UIManager.UserManager.GetUserLanguage());
                    //            singleMessage = singleMessage.Replace("{1}", codice);
                    //            msgDesc += singleMessage;
                    //            control = false;
                    //        }
                    //        if (prj.accessRights != null && Convert.ToInt32(prj.accessRights) <= 45 && control)
                    //        {
                    //            // Non fa return, segnala l'errore alla fine
                    //            singleMessage = Languages.GetMessageFromCode("ResultProjectReadOnly", UIManager.UserManager.GetUserLanguage());
                    //            singleMessage = singleMessage.Replace("{1}", codice);
                    //            msgDesc += singleMessage;

                    //            control = false;
                    //        }

                    //        //Ricavo la folder dal fascicolo selezionato		
                    //        if (grid)
                    //            selectedFolder = ProjectManager.getFolder(this, prj);

                    //        bool inserted = false;

                    //        //if (selectedFolder != null && control)
                    //        //{
                    //        //    String message = String.Empty;
                    //        //    //Inserisco/Classifico il documento nel folder/sottoFascicolo selezionato
                    //        //    inserted = DocumentManager.addDocumentoInFolder(this, DocumentInWorking.systemId, selectedFolder.systemID, false, out isInFolder, out message);
                    //        //}

                    //        //dopo la classifica rimuovo la Folder Selezionata
                    //        //FascicoliManager.removeFolderSelezionato(this);

                    //        if (isInFolder && control) // SE il doc è già nella folder indicata
                    //        {
                    //            // Non fa return, segnala l'errore alla fine
                    //            singleMessage = Languages.GetMessageFromCode("ResultProjectDocAlreadyInFolder", UIManager.UserManager.GetUserLanguage());
                    //            singleMessage = singleMessage.Replace("{1}", codice);
                    //            msgDesc += singleMessage;
                    //            control = false;
                    //        }
                    //        else if (inserted)
                    //        {  // Non fa return, segnala l'avvenuto inserimento alla fine
                    //            singleMessage = Languages.GetMessageFromCode("ResultProjectAddDoc", UIManager.UserManager.GetUserLanguage());
                    //            //msgDesc = Languages.GetMessageFromCode("ResultSelectProject", UIManager.UserManager.GetUserLanguage());
                    //            singleMessage = singleMessage.Replace("{1}", codice);
                    //            msgDesc += singleMessage;
                    //            //ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');}", true);
                    //            //messaggio += "Documento classificato correttamente nel fascicolo " + codice + "!\\n";
                    //        }
                    //        else
                    //        {
                    //            // Non fa return, segnala alla fine------COMMENTARE TUTTO???
                    //            singleMessage = Languages.GetMessageFromCode("ResultProjectErrorAddDoc", UIManager.UserManager.GetUserLanguage());
                    //            singleMessage = singleMessage.Replace("{1}", codice);
                    //            msgDesc += singleMessage;
                    //            //ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');}", true);
                    //            //messaggio += "Errore in inserimento del documento nel fascicolo " + codice + "!\\n";
                    //        }

                    //        if (grid)
                    //        {
                    //            m_hashTableFascicoli = ProjectManager.getHashFascicoli(this);
                    //            returnValue = prj.codice + "#" + prj.descrizione;
                    //            HttpContext.Current.Session["ReturnValuePopup"] = returnValue;
                    //        }
                    //        ProjectManager.setHashFascicoli(this, null);
                    //        ProjectManager.setHashFascicoliSelezionati(this, null);
                    //        this.CloseMask(true);
                    //    }
                    //    else
                    //    {
                    //        // Non fa return, segnala l'errore alla fine
                    //        singleMessage = Languages.GetMessageFromCode("ResultProjectNotFound", UIManager.UserManager.GetUserLanguage());
                    //        singleMessage = singleMessage.Replace("{1}", codice);
                    //        msgDesc += singleMessage;
                    //        //   ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');}", true);
                    //        //messaggio += "Attenzione! codice fascicolo " + codice + " non presente!\\n";
                    //    }

                    //string errFormt = Server.UrlEncode(msg);
                    //HttpContext.Current.Session["ReturnValuePopup"] = msgDesc;//Server.UrlEncode(msgDesc);
                    //ProjectManager.setHashFascicoli(this, null);
                    //ProjectManager.setHashFascicoliSelezionati(this, null);
                    //this.CloseMask(true);
                    //}
                }
                else
                {
                    msgDesc = "ResultSelectProject";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');}", true);
                    return;
                }
            }
            else if (this.CallerSearchProject != null && this.CallerSearchProject == "classifica")
            {
                string msgDesc = "";
                string singleMessage = "";

                bool avanza = this.verificaSelezione("classifica", out key, out grid);
                if (avanza)
                {

                    //Controllo i diritti sul documento

                    string accessRight = DocumentManager.getAccessRightDocBySystemID(DocumentInWorking.systemId, UIManager.UserManager.GetInfoUser());


                    if (accessRight.Equals("20"))
                    {
                        //senza chiudere la maschera
                        msgDesc = "ResultProjectDocLocked";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');}", true);
                        return;
                    }


                    if (HT_fascicoliSelezionati != null)
                    {
                        if (HT_fascicoliSelezionati.Count == 0)
                        {
                            //senza chiudere la maschera
                            msgDesc = "ResultSelectProject";
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');}", true);
                            return;
                        }

                        if (HT_fascicoliSelezionati.Count > 0)
                        {
                            foreach (DictionaryEntry entry in HT_fascicoliSelezionati)
                            {
                                selectedFolder = null;
                                string systemId = (string)entry.Key;
                                if (grid)
                                {
                                    prj = ProjectManager.getFascicoloById(this, systemId);
                                    if (prj == null)
                                    {
                                        selectedFolder = ProjectManager.getFolder(this, systemId);
                                        prj = ProjectManager.getFascicoloById(this, selectedFolder.idFascicolo);
                                    }
                                    if (prj != null)
                                    {
                                        codice = prj.codice.Replace(@"\", @"\\");
                                    }
                                    if (prj != null && prj.pubblico && string.IsNullOrEmpty(this.HiddenContinuaFascicolazioneInPubblico.Value))
                                    {
                                        string msgConfirm = "WarningDocumentConfirmPublicFolder";
                                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxConfirmModal", "ajaxConfirmModal('" + msgConfirm.Replace("'", @"\'") + "', 'HiddenPublicFolder', '');", true);
                                        return;
                                    }
                                }
                                else
                                {
                                    selectedFolder = ProjectManager.getFolder(this, systemId);
                                    prj = ProjectManager.getFascicoloById(this, selectedFolder.idFascicolo);
                                    codice = prj.codice.Replace(@"\", @"\\");

                                    if (prj != null && prj.pubblico && string.IsNullOrEmpty(this.HiddenContinuaFascicolazioneInPubblico.Value))
                                    {
                                        string msgConfirm = "WarningDocumentConfirmPublicFolder";
                                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxConfirmModal", "ajaxConfirmModal('" + msgConfirm.Replace("'", @"\'") + "', 'HiddenPublicFolder', '');", true);
                                        return;
                                    }
                                }
                                bool control = true;


                                bool isInFolder = false;

                                if (prj != null)
                                {
                                    //se lo stato del fascicolo è chiuso non si deve inserire il documento. Non fa return, segnala l'errore alla fine
                                    if (prj.stato == "C")
                                    {
                                        singleMessage = Languages.GetMessageFromCode("ResultProjectClosed", UIManager.UserManager.GetUserLanguage());
                                        singleMessage = singleMessage.Replace("{1}", codice);
                                        msgDesc += singleMessage;
                                        control = false;
                                    }
                                    if (prj.accessRights != null && Convert.ToInt32(prj.accessRights) <= 45 && control)
                                    {
                                        // Non fa return, segnala l'errore alla fine
                                        singleMessage = Languages.GetMessageFromCode("ResultProjectReadOnly", UIManager.UserManager.GetUserLanguage());
                                        singleMessage = singleMessage.Replace("{1}", codice);
                                        msgDesc += singleMessage;

                                        control = false;
                                    }

                                    //Ricavo la folder dal fascicolo selezionato		
                                    if (grid && selectedFolder == null)
                                        selectedFolder = ProjectManager.getFolder(this, prj);

                                    bool inserted = false;

                                    if (selectedFolder != null && control)
                                    {
                                        String message = String.Empty;
                                        //Inserisco/Classifico il documento nel folder/sottoFascicolo selezionato
                                        inserted = DocumentManager.addDocumentoInFolder(this, DocumentInWorking.systemId, selectedFolder.systemID, false, out isInFolder, out message);
                                    }

                                    //dopo la classifica rimuovo la Folder Selezionata
                                    //FascicoliManager.removeFolderSelezionato(this);

                                    if (isInFolder && control) // SE il doc è già nella folder indicata
                                    {
                                        // Non fa return, segnala l'errore alla fine
                                        singleMessage = Languages.GetMessageFromCode("ResultProjectDocAlreadyInFolder", UIManager.UserManager.GetUserLanguage());
                                        singleMessage = singleMessage.Replace("{1}", codice);
                                        msgDesc += singleMessage;
                                        control = false;
                                    }
                                    else if (inserted)
                                    {  // Non fa return, segnala l'avvenuto inserimento alla fine
                                        singleMessage = Languages.GetMessageFromCode("ResultProjectAddDoc", UIManager.UserManager.GetUserLanguage());
                                        //msgDesc = Languages.GetMessageFromCode("ResultSelectProject", UIManager.UserManager.GetUserLanguage());
                                        singleMessage = singleMessage.Replace("{1}", codice);
                                        msgDesc += singleMessage;
                                        //ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');}", true);
                                        //messaggio += "Documento classificato correttamente nel fascicolo " + codice + "!\\n";
                                    }
                                    else
                                    {
                                        // Non fa return, segnala alla fine------COMMENTARE TUTTO???
                                        singleMessage = Languages.GetMessageFromCode("ResultProjectErrorAddDoc", UIManager.UserManager.GetUserLanguage());
                                        singleMessage = singleMessage.Replace("{1}", codice);
                                        msgDesc += singleMessage;
                                        //ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');}", true);
                                        //messaggio += "Errore in inserimento del documento nel fascicolo " + codice + "!\\n";
                                    }

                                }
                                else
                                {
                                    // Non fa return, segnala l'errore alla fine
                                    singleMessage = Languages.GetMessageFromCode("ResultProjectNotFound", UIManager.UserManager.GetUserLanguage());
                                    singleMessage = singleMessage.Replace("{1}", codice);
                                    msgDesc += singleMessage;
                                    //   ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');}", true);
                                    //messaggio += "Attenzione! codice fascicolo " + codice + " non presente!\\n";
                                }
                            }
                            //string errFormt = Server.UrlEncode(msg);
                            HttpContext.Current.Session["ReturnValuePopup"] = msgDesc;//Server.UrlEncode(msgDesc);
                            ProjectManager.setHashFascicoli(this, null);
                            ProjectManager.setHashFascicoliSelezionati(this, null);
                            this.HiddenContinuaFascicolazioneInPubblico.Value = string.Empty;
                            this.CloseMask(true);
                        }
                    }
                    else
                    {
                        msgDesc = "ResultSelectProject";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');}", true);
                        return;
                    }
                }
                else
                {
                    msgDesc = "ResultSelectProject";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');}", true);
                    return;
                }
            }
            else if (!string.IsNullOrEmpty(this.CallerSearchProject) && this.CallerSearchProject == "search")
            {
                bool avanza = this.verificaSelezione("search", out key, out grid);
                string msgDesc = "";
                if (avanza)
                {
                    if (grid)
                    {
                        m_hashTableFascicoli = ProjectManager.getHashFascicoli(this);

                        fasc = (DocsPaWR.SearchObject)m_hashTableFascicoli[key];
                        prj = GetProjectFormSearchObject(fasc);
                        returnValue = prj.codice + "#" + prj.descrizione;
                        HttpContext.Current.Session["ReturnValuePopup"] = returnValue;

                    }
                    ProjectManager.setHashFascicoli(this, null);
                    ProjectManager.setHashFascicoliSelezionati(this, null);
                    this.CloseMask(true);
                }
                else
                {
                    msgDesc = "ResultSelectProject";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');}", true);
                    return;
                }
            }
            else if (this.CallerSearchProject != null && this.CallerSearchProject == "custom")
            {
                bool avanza = this.verificaSelezione("profilo", out key, out grid);
                string msgDesc = "";
                if (avanza)
                {
                    m_hashTableFascicoli = ProjectManager.getHashFascicoli(this);
                    fasc = (DocsPaWR.SearchObject)m_hashTableFascicoli[key];
                    prj = GetProjectFormSearchObject(fasc);
                    HttpContext.Current.Session["LinkCustom.return"] = prj.systemID;
                    this.ClearSessionData();
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "closeMask", "if (parent.fra_main) {parent.fra_main.closeAjaxModal('SearchProjectCustom', '" + HttpContext.Current.Session["LinkCustom.type"].ToString() + "');} else {parent.closeAjaxModal('SearchProjectCustom', '" + HttpContext.Current.Session["LinkCustom.type"].ToString() + "');};", true);
                }
                else
                {
                    msgDesc = "ResultSelectProject";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');}", true);
                    return;
                }
            }
            else if (this.CallerSearchProject != null && this.CallerSearchProject == "searchInstance")
            {
                string msgDesc = string.Empty;
                string singleMessage = string.Empty;

                bool avanza = this.verificaSelezione("searchInstance", out key, out grid);
                if (avanza)
                {
                    if (HT_fascicoliSelezionati != null)
                    {
                        if (HT_fascicoliSelezionati.Count == 0)
                        {
                            //senza chiudere la maschera
                            msgDesc = "ResultSelectProject";
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');}", true);
                            return;
                        }

                        if (HT_fascicoliSelezionati.Count > 0)
                        {
                            this.ListDocumentsAccess = new List<InstanceAccessDocument>();
                            InstanceAccessDocument instAccDoc = new InstanceAccessDocument();
                            SearchResultInfo[] idProfilesList;
                            int pageNumbers = 0;
                            int recordNumber = 0;

                            foreach (DictionaryEntry entry in HT_fascicoliSelezionati)
                            {
                                string systemId = (string)entry.Key;
                                if (grid)
                                {
                                    prj = ProjectManager.getFascicoloById(this, systemId);
                                    prj.folderSelezionato = ProjectManager.getFolder(this, prj);

                                    SearchObject[] result = UIManager.ProjectManager.getListaDocumentiPagingCustom(prj.folderSelezionato, null, 1, out pageNumbers, out recordNumber, true, false, false, null, null, 20, UIManager.GridManager.GetFiltriOrderRicerca(GridManager.GetStandardGridForUser(GridTypeEnumeration.DocumentInProject,UIManager.UserManager.GetInfoUser())), out idProfilesList);

                                    if (result != null && result.Length > 0)
                                    {
                                        //Root principale
                                        foreach (SearchObject obj in result)
                                        {
                                            instAccDoc = new InstanceAccessDocument();
                                            instAccDoc.INFO_DOCUMENT = new InfoDocument();
                                            instAccDoc.INFO_DOCUMENT.DOCNUMBER = obj.SearchObjectID;
                                            instAccDoc.INFO_PROJECT = new InfoProject();
                                            instAccDoc.INFO_PROJECT.ID_PROJECT = prj.systemID;
                                            instAccDoc.ENABLE = true;
                                            SchedaDocumento docDetails = DocumentManager.getDocumentListVersions(this.Page, obj.SearchObjectID, obj.SearchObjectID);
                                            instAccDoc.TYPE_REQUEST = (docDetails.tipologiaAtto != null && HttpContext.Current.Session["TipologiaAttoIstanza"] != null &&
                                                docDetails.tipologiaAtto.descrizione.Equals((HttpContext.Current.Session["TipologiaAttoIstanza"] as Templates).DESCRIZIONE)) ? string.Empty : InstanceAccessManager.TipoRichiesta.COPIA_SEMPLICE;
                                            instAccDoc.INFO_DOCUMENT.DESCRIPTION_TIPOLOGIA_ATTO = docDetails.tipologiaAtto != null ? docDetails.tipologiaAtto.descrizione : string.Empty;
                                            instAccDoc.ATTACHMENTS = new InstanceAccessAttachments[docDetails.allegati.Length];
                                            if (docDetails.allegati != null && docDetails.allegati.Length > 0)
                                            {
                                                for (int i = 0; i < docDetails.allegati.Length; i++)
                                                {
                                                    instAccDoc.ATTACHMENTS[i] = new InstanceAccessAttachments();
                                                    instAccDoc.ATTACHMENTS[i].ID_ATTACH = docDetails.allegati[i].docNumber;
                                                    if (docDetails.allegati[i].TypeAttachment == 1 || docDetails.allegati[i].TypeAttachment == 4)
                                                    {
                                                        instAccDoc.ATTACHMENTS[i].ENABLE = true;
                                                    }
                                                    else
                                                    {
                                                        instAccDoc.ATTACHMENTS[i].ENABLE = false;
                                                    }
                                                }
                                            }
                                            this.ListDocumentsAccess.Add(instAccDoc);
                                        }
                                    }

                                    if (prj.folderSelezionato.childs.Length > 0)
                                    {
                                        for (int k = 0; k < prj.folderSelezionato.childs.Length; k++)
                                        {
                                            this.GetSubFolders(prj.folderSelezionato.childs[k]);
                                        }
                                    }

                                        //verifico se ci sono sotto fascicoli nel fascicolo
                                    //}
                                }
                            }
                            ProjectManager.setHashFascicoli(this, null);
                            ProjectManager.setHashFascicoliSelezionati(this, null);
                            this.CloseMask(true);
                            return;
                        }
                    }
                }
                else
                {
                    msgDesc = "ResultSelectProject";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');}", true);
                    return;
                }
            }
        }

        private void GetSubFolders(DocsPaWR.Folder folder)
        {
            bool isRoot = false;
            this.GetSubFolders(folder, isRoot);
        }

        private void GetSubFolders(DocsPaWR.Folder folder, bool isRoot)
        {
            try
            {
                InstanceAccessDocument instAccDoc = new InstanceAccessDocument();
                SearchResultInfo[] idProfilesList;
                int pageNumbers = 0;
                int recordNumber = 0;

                SearchObject[] result = UIManager.ProjectManager.getListaDocumentiPagingCustom(folder, null, 1, out pageNumbers, out recordNumber, true, false, false, null, null, 20, UIManager.GridManager.GetFiltriOrderRicerca(GridManager.GetStandardGridForUser(GridTypeEnumeration.DocumentInProject, UIManager.UserManager.GetInfoUser())), out idProfilesList);

                if (result != null && result.Length > 0)
                {
                    //Root principale
                    foreach (SearchObject obj in result)
                    {
                        instAccDoc = new InstanceAccessDocument();
                        instAccDoc.INFO_DOCUMENT = new InfoDocument();
                        instAccDoc.INFO_DOCUMENT.DOCNUMBER = obj.SearchObjectID;
                        instAccDoc.INFO_PROJECT = new InfoProject();
                        instAccDoc.INFO_PROJECT.ID_PROJECT = folder.systemID;
                        instAccDoc.INFO_PROJECT.ID_PARENT = folder.idFascicolo;
                        instAccDoc.ENABLE = true;
                        SchedaDocumento docDetails = DocumentManager.getDocumentListVersions(this.Page, obj.SearchObjectID, obj.SearchObjectID);
                        instAccDoc.TYPE_REQUEST = (docDetails.tipologiaAtto != null && HttpContext.Current.Session["TipologiaAttoIstanza"] != null &&
                                docDetails.tipologiaAtto.descrizione.Equals((HttpContext.Current.Session["TipologiaAttoIstanza"] as Templates).DESCRIZIONE)) ? string.Empty : InstanceAccessManager.TipoRichiesta.COPIA_SEMPLICE;
                        instAccDoc.INFO_DOCUMENT.DESCRIPTION_TIPOLOGIA_ATTO = docDetails.tipologiaAtto != null ? docDetails.tipologiaAtto.descrizione : string.Empty;
                        instAccDoc.ATTACHMENTS = new InstanceAccessAttachments[docDetails.allegati.Length];
                        if (docDetails.allegati != null && docDetails.allegati.Length > 0)
                        {
                            for (int i = 0; i < docDetails.allegati.Length; i++)
                            {
                                instAccDoc.ATTACHMENTS[i] = new InstanceAccessAttachments();
                                instAccDoc.ATTACHMENTS[i].ID_ATTACH = docDetails.allegati[i].docNumber;
                                if (docDetails.allegati[i].TypeAttachment == 1 || docDetails.allegati[i].TypeAttachment == 4)
                                {
                                    instAccDoc.ATTACHMENTS[i].ENABLE = true;
                                }
                                else
                                {
                                    instAccDoc.ATTACHMENTS[i].ENABLE = false;
                                }
                            }
                        }
                        this.ListDocumentsAccess.Add(instAccDoc);
                    }
                }

                if (folder.childs.Length > 0)
                {
                    for (int k = 0; k < folder.childs.Length; k++)
                    {
                        this.GetSubFolders(folder.childs[k]);
                    }
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        protected void BtnClose_Click(object sender, EventArgs e)
        {
            this.CloseMask(false);
        }

        protected void CloseMask(bool withReturnValue)
        {
            string returnValue = string.Empty;
            if (withReturnValue)
                returnValue = "true";
            else
                this.ClearSessionData();

            if (Request.QueryString["popupid"] == "SearchProjectMassive")
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "closeMask", "if (parent.fra_main) {parent.fra_main.closeAjaxModal('SearchProjectMassive', '" + returnValue + "');} else {parent.closeAjaxModal('SearchProjectMassive', '" + returnValue + "');};", true);
            }
            else
            {
                if (Request.QueryString["fromf"] == "Instance")
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "closeMask", "if (parent.fra_main) {parent.fra_main.closeAjaxModal('SearchProject', '" + returnValue + "');} else {parent.closeAjaxModal('SearchProject', 'up',parent);};", true);
                }
                else
                {
                    if (this.CallerSearchProject != null && this.CallerSearchProject == "searchInstance")
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "closeMask", "if (parent.fra_main) {parent.fra_main.closeAjaxModal('SearchDocumentsInProject', '" + returnValue + "');} else {parent.closeAjaxModal('SearchDocumentsInProject', '" + returnValue + "');};", true);
                    }
                    else if (this.CallerSearchProject != null && this.CallerSearchProject == "custom")
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "closeMask", "if (parent.fra_main) {parent.fra_main.closeAjaxModal('SearchProjectCustom', '" + returnValue + "');} else {parent.closeAjaxModal('SearchProjectCustom', '" + returnValue + "');};", true);
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "closeMask", "if (parent.fra_main) {parent.fra_main.closeAjaxModal('SearchProject', '" + returnValue + "');} else {parent.closeAjaxModal('SearchProject', '" + returnValue + "');};", true);
                    }
                }
            }
        }

        private void ClearSessionData()
        {
            this.Project = null;
            this.ProjectClass = null;
            HttpContext.Current.Session["ReturnValuePopup"] = null;
            ProjectManager.removeHashFascicoli(this);
            ProjectManager.setHashFascicoliSelezionati(this, null);
            //FascicoliManager.DO_RemoveFlagLF();
            //FascicoliManager.DO_RemoveLocazioneFisica();
            //RemoveListaFascicoli(page);
        }

        protected void TxtCodeProject_OnTextChanged(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);
            System.Type tipo = sender.GetType();
            //if (sender.GetType() == typeof(NttDatalLibrary.CustomTextArea))
            //    ProjectManager.setProjectInSessionForRicFasc(this.TxtCodeProject.Text);
            //codeChanged = true;
            if (!string.IsNullOrEmpty(this.TxtCodeProject.Text))
            {
                SchedaDocumento documentoSelezionato = UIManager.DocumentManager.getSelectedRecord();

                if (this.Registry == null)
                {
                    this.SearchProjectNoRegistro();
                }
                else
                {
                    this.SearchProjectRegistro();
                }
            }
            else
            {
                this.TxtCodeProject.Text = string.Empty;
                this.TxtDescriptionProject.Text = string.Empty;
                this.IdProject.Value = string.Empty;
                //this.Project = null;
                this.ProjectClass = null;

            }

            this.UpPnlProject.Update();
        }

        protected void SearchProjectNoRegistro()
        {

            this.TxtDescriptionProject.Text = string.Empty;
            if (string.IsNullOrEmpty(this.TxtCodeProject.Text))
            {
                this.TxtDescriptionProject.Text = string.Empty;
                return;
            }
            //su DocProfilo devo cercare senza condizione sul registro.
            //Basta che il fascicolo sia visibile al ruolo loggato


            DocsPaWR.Fascicolo[] listaFasc = getFascicolo(this.Registry);
            string codClassifica = string.Empty;
            if (listaFasc != null)
            {
                if (listaFasc.Length > 0)
                {
                    //caso 1: al codice digitato corrisponde un solo fascicolo
                    if (listaFasc.Length == 1)
                    {
                        this.TxtDescriptionProject.Text = listaFasc[0].descrizione;
                        //metto il fascicolo in sessione
                        if (listaFasc[0].tipo.Equals("G"))
                        {
                            codClassifica = listaFasc[0].codice;
                        }
                        else
                        {
                            this.TxtDescriptionProject.Text = string.Empty;
                            this.TxtCodeProject.Text = string.Empty;
                            this.ProjectClass = null;

                            string msg = "WarningDocumentCodFileNoFound";

                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                        }
                        this.ProjectClass = listaFasc[0];
                        //IL SECONDO PARAMETRO INDICA SE IL NODO è PRESENTE SU PIU REGISTRI
                        //this.imgFasc.Attributes.Add("onclick","ApriRicercaFascicoli('"+codClassifica+"', 'N');");
                    }
                    else
                    {
                        codClassifica = this.TxtCodeProject.Text;
                        if (listaFasc[0].tipo.Equals("G"))
                        {
                            //codClassifica = codClassifica;
                        }
                        else
                        {
                            //se il fascicolo è procedimentale, ricerco la classifica a cui appartiene
                            //DocsPaWR.FascicolazioneClassifica[] gerClassifica = ProjectManager.getGerarchia(this, listaFasc[0].idClassificazione, UserManager.GetUserInSession().idAmministrazione);
                            //string codiceGerarchia = gerClassifica[gerClassifica.Length - 1].codice;
                            //codClassifica = codiceGerarchia;
                            //string msg = @"Attenzione, codice fascicolo non presente.";
                            this.TxtDescriptionProject.Text = string.Empty;
                            this.TxtCodeProject.Text = string.Empty;
                            this.ProjectClass = null;

                            string msg = "WarningDocumentCodFileNoFound";

                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);

                        }
                        //Page.RegisterStartupScript("openListaFasc","<SCRIPT>ApriSceltaFascicolo();</SCRIPT>");
                        //Session.Add("hasRegistriNodi",hasRegistriNodi);

                        //IL SECONDO PARAMETRO INDICA SE IL NODO è PRESENTE SU PIU REGISTRI
                        //this.imgFasc.Attributes.Add("onclick","ApriRicercaFascicoli('"+codClassifica+"', 'Y');");

                        //Da Fare
                        //RegisterStartupScript("openModale", "<script>ApriRicercaFascicoli2('" + codClassifica + "', 'Y')</script>");                            

                        return;
                    }
                }
                else
                {
                    //caso 0: al codice digitato non corrisponde alcun fascicolo
                    if (listaFasc.Length == 0)
                    {

                        //string msg = @"Attenzione, codice fascicolo non presente.";
                        string msg = "WarningDocumentCodFileNoFound";

                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);



                        this.TxtDescriptionProject.Text = string.Empty;
                        this.TxtCodeProject.Text = string.Empty;
                        this.ProjectClass = null;
                    }
                    //IL SECONDO PARAMETRO INDICA SE IL NODO è PRESENTE SU PIU REGISTRI
                    //this.imgFasc.Attributes.Add("onclick","ApriRicercaFascicoli('"+codClassifica+"', '');");
                }

            }
        }

        protected void SearchProjectRegistro()
        {
            this.TxtDescriptionProject.Text = string.Empty;
            string codClassifica = string.Empty;

            if (string.IsNullOrEmpty(this.TxtCodeProject.Text))
            {
                this.TxtDescriptionProject.Text = string.Empty;
                this.ProjectClass = null;
                return;
            }

            //FASCICOLAZIONE IN SOTTOFASCICOLI


            DocsPaWR.Fascicolo[] listaFasc = getFascicoli(this.Registry);
            // new code
            DocsPaWR.FascicolazioneClassificazione[] FascClass = ProjectManager.fascicolazioneGetTitolario2(this, this.TxtCodeProject.Text, false, getIdTitolario(this.TxtCodeProject.Text, listaFasc));

            if (FascClass != null && FascClass.Length > 0)
            {
                if (FascClass.Length != 0)
                {
                    this.ProjectClass = ProjectManager.getClassificazioneById(FascClass[0].systemID);
                    this.TxtDescriptionProject.Text = this.ProjectClass.descrizione;
                    if (this.ProjectClass.tipo.Equals("G"))
                    {
                        codClassifica = listaFasc[0].codice;
                    }
                    else
                    {
                        codClassifica = FascClass[0].codice;

                    }
                }
                else
                {
                    return;
                }
            }
            else
            {
                //caso 0: al codice digitato non corrisponde alcun fascicolo
                if (FascClass.Length == 0)
                {

                    //string msg = @"Attenzione, codice fascicolo non presente.";
                    string msg = "WarningDocumentCodFileNoFound";

                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);

                    this.TxtDescriptionProject.Text = string.Empty;
                    this.TxtCodeProject.Text = string.Empty;
                    this.ProjectClass = null;
                }
            }

            #region OLD CODE
            //if (listaFasc != null)
            //{
            //    if (listaFasc.Length > 0)
            //    {
            //        //caso 1: al codice digitato corrisponde un solo fascicolo
            //        if (listaFasc.Length == 1)
            //        {
            //            this.ProjectClass = listaFasc[0];
            //            this.TxtDescriptionProject.Text = listaFasc[0].descrizione;
            //            if (listaFasc[0].tipo.Equals("G"))
            //            {
            //                codClassifica = listaFasc[0].codice;
            //            }
            //            else
            //            {
            //                //se il fascicolo è procedimentale, ricerco la classifica a cui appartiene
            //                //DocsPaWR.FascicolazioneClassifica[] gerClassifica = ProjectManager.getGerarchia(this, listaFasc[0].idClassificazione, UserManager.GetUserInSession().idAmministrazione);
            //                //string codiceGerarchia = gerClassifica[gerClassifica.Length - 1].codice;
            //                //codClassifica = codiceGerarchia;
            //                string msg = "WarningDocumentCodFileGeneral";

            //                this.TxtDescriptionProject.Text = string.Empty;
            //                this.TxtCodeProject.Text = string.Empty;
            //                this.ProjectClass = null;

            //                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
            //            }
            //            //ProjectManager.setFascicoloSelezionatoFascRapida(this, listaFasc[0]);
            //        }
            //        else
            //        {
            //            ////caso 2: al codice digitato corrispondono piu fascicoli
            //            //codClassifica = this.TxtCodeProject.Text;
            //            //if (listaFasc[0].tipo.Equals("G"))
            //            //{
            //            //    //codClassifica = codClassifica;
            //            //}
            //            //else
            //            //{
            //            //    //se il fascicolo è procedimentale, ricerco la classifica a cui appartiene
            //            //    DocsPaWR.FascicolazioneClassifica[] gerClassifica = ProjectManager.getGerarchia(this, listaFasc[0].idClassificazione, UserManager.GetUserInSession().idAmministrazione);
            //            //    string codiceGerarchia = gerClassifica[gerClassifica.Length - 1].codice;
            //            //    codClassifica = codiceGerarchia;
            //            //}

            //            ////Da Fare
            //            //RegisterStartupScript("openModale", "<script>ApriRicercaFascicoli('" + codClassifica + "', 'Y')</script>");
            //            return;
            //        }
            //    }
            //    else
            //    {
            //        //caso 0: al codice digitato non corrisponde alcun fascicolo
            //        if (listaFasc.Length == 0)
            //        {

            //            //string msg = @"Attenzione, codice fascicolo non presente.";
            //            string msg = "WarningDocumentCodFileNoFound";

            //            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);

            //            this.TxtDescriptionProject.Text = string.Empty;
            //            this.TxtCodeProject.Text = string.Empty;
            //            this.ProjectClass = null;
            //        }
            //    }
            //}
            #endregion

        }


        private string getIdTitolario(string codClassificazione, DocsPaWR.Fascicolo[] listaFasc)
        {
            if (!string.IsNullOrEmpty(codClassificazione))
            {
                //DocsPaWR.Fascicolo[] listaFasc = getFascicolo(UserManager.getRegistroSelezionato(this), codClassificazione);

                //In questo caso il metodo "GetFigliClassifica2" funzionerebbe male
                //per questo viene restituti l'idTitolario dell'unico fascicolo risolto
                if (ddlTitolario.SelectedItem != null && ddlTitolario.SelectedItem.Text == "Tutti i titolari" && listaFasc != null && listaFasc.Length == 1)
                {
                    DocsPaWR.Fascicolo fasc = (DocsPaWR.Fascicolo)listaFasc[0];
                    return fasc.idTitolario;
                }
            }

            //In tutti gli altri casi è sufficiente restituire il value degli item della
            //ddl_Titolario in quanto formati secondo le specifiche di uno o piu' titolari
            return ddlTitolario.SelectedValue;
        }

        private DocsPaWR.Fascicolo[] getFascicolo(DocsPaWR.Registro registro)
        {
            DocsPaWR.Fascicolo[] listaFasc = null;
            if (!string.IsNullOrEmpty(this.TxtCodeProject.Text))
            {
                string codiceFascicolo = TxtCodeProject.Text;
                listaFasc = ProjectManager.getListaFascicoliDaCodice(this, codiceFascicolo, registro, "I");
            }
            if (listaFasc != null)
            {
                return listaFasc;
            }
            else
            {
                return null;
            }
        }

        private DocsPaWR.Fascicolo[] getFascicoli(DocsPaWR.Registro registro)
        {
            DocsPaWR.Fascicolo[] listaFasc = null;
            if (!this.TxtCodeProject.Text.Equals(""))
            {
                string codiceFascicolo = TxtCodeProject.Text;
                listaFasc = ProjectManager.getListaFascicoliDaCodice(this, codiceFascicolo, registro, "I");
            }
            if (listaFasc != null)
            {
                return listaFasc;
            }
            else
            {
                return null;
            }
        }

        private DocsPaWR.Fascicolo getFolder(DocsPaWR.Registro registro, ref string codice, ref string descrizione)
        {
            DocsPaWR.Folder[] listaFolder = null;
            DocsPaWR.Fascicolo fasc = null;
            string separatore = "//";
            int posSep = this.TxtCodeProject.Text.IndexOf("//");
            if (this.TxtCodeProject.Text != string.Empty && posSep > -1)
            {

                string codiceFascicolo = TxtCodeProject.Text.Substring(0, posSep);
                string descrFolder = TxtCodeProject.Text.Substring(posSep + separatore.Length);

                listaFolder = ProjectManager.getListaFolderDaCodiceFascicolo(this, codiceFascicolo, descrFolder, registro);
                if (listaFolder != null && listaFolder.Length > 0)
                {
                    //calcolo fascicolazionerapida
                    InfoUtente infoUtente = UserManager.GetInfoUser();
                    fasc = ProjectManager.getFascicoloById(listaFolder[0].idFascicolo, infoUtente);

                    if (fasc != null)
                    {
                        //folder selezionato è l'ultimo
                        fasc.folderSelezionato = listaFolder[listaFolder.Length - 1];
                    }
                    codice = fasc.codice + separatore;
                    descrizione = fasc.descrizione + separatore;
                    for (int i = 0; i < listaFolder.Length; i++)
                    {
                        codice += listaFolder[i].descrizione + "/";
                        descrizione += listaFolder[i].descrizione + "/";
                    }
                    codice = codice.Substring(0, codice.Length - 1);
                    descrizione = descrizione.Substring(0, descrizione.Length - 1);

                }
            }
            if (fasc != null)
            {
                return fasc;
            }
            else
            {
                return null;
            }
        }

        protected void ddl_dtaOpen_SelectedIndexChanged(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);

            string language = UIManager.UserManager.GetUserLanguage();
            switch (this.ddl_dtaOpen.SelectedIndex)
            {
                case 0: //Valore singolo
                    this.txtInitDtaOpen.ReadOnly = false;
                    this.txtEndDtaOpen.Visible = false;
                    this.lblEnd_DtaOpen.Visible = false;
                    this.lblInit_DtaOpen.Text = Utils.Languages.GetLabelFromCode("VisibilityOneField", language);
                    break;
                case 1: //Intervallo
                    this.txtInitDtaOpen.ReadOnly = false;
                    this.txtEndDtaOpen.ReadOnly = false;
                    this.lblEnd_DtaOpen.Visible = true;
                    this.lblInit_DtaOpen.Visible = true;
                    this.txtEndDtaOpen.Visible = true;
                    this.lblInit_DtaOpen.Text = Utils.Languages.GetLabelFromCode("VisibilityFrom", language);
                    this.lblEnd_DtaOpen.Text = Utils.Languages.GetLabelFromCode("VisibilityTo", language);
                    break;
                case 2: //Oggi
                    this.lblEnd_DtaOpen.Visible = false;
                    this.txtEndDtaOpen.Visible = false;
                    this.txtInitDtaOpen.ReadOnly = true;
                    this.txtInitDtaOpen.Text = NttDataWA.Utils.dateformat.toDay();
                    break;
                case 3: //Settimana corrente
                    this.lblEnd_DtaOpen.Visible = true;
                    this.txtEndDtaOpen.Visible = true;
                    this.txtInitDtaOpen.Text = NttDataWA.Utils.dateformat.getFirstDayOfWeek();
                    this.txtEndDtaOpen.Text = NttDataWA.Utils.dateformat.getLastDayOfWeek();
                    this.txtEndDtaOpen.ReadOnly = true;
                    this.txtInitDtaOpen.ReadOnly = true;
                    this.lblInit_DtaOpen.Text = Utils.Languages.GetLabelFromCode("VisibilityFrom", language);
                    this.lblEnd_DtaOpen.Text = Utils.Languages.GetLabelFromCode("VisibilityTo", language);
                    break;
                case 4: //Mese corrente
                    this.lblEnd_DtaOpen.Visible = true;
                    this.txtEndDtaOpen.Visible = true;
                    this.txtInitDtaOpen.Text = NttDataWA.Utils.dateformat.getFirstDayOfMonth();
                    this.txtEndDtaOpen.Text = NttDataWA.Utils.dateformat.getLastDayOfMonth();
                    this.txtEndDtaOpen.ReadOnly = true;
                    this.txtInitDtaOpen.ReadOnly = true;
                    this.lblInit_DtaOpen.Text = Utils.Languages.GetLabelFromCode("VisibilityFrom", language);
                    this.lblEnd_DtaOpen.Text = Utils.Languages.GetLabelFromCode("VisibilityTo", language);
                    break;
            }



            //this.txtEndDtaOpen.Text = "";

            //if (this.ddl_dtaOpen.SelectedIndex == 0)
            //{
            //    this.pnlEndDtaOpen.Visible = false;
            //    this.lblInit_DtaOpen.Visible = false;
            //}
            //else
            //{
            //    this.pnlEndDtaOpen.Visible = true;
            //    this.lblInit_DtaOpen.Visible = true;
            //}
        }

        protected void ddl_dtaClosed_SelectedIndexChanged(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);

            string language = UIManager.UserManager.GetUserLanguage();
            switch (this.ddl_dtaClosed.SelectedIndex)
            {
                case 0: //Valore singolo
                    this.txtInitDtaClosed.ReadOnly = false;
                    this.txtEndDtaClosed.Visible = false;
                    this.lblEnd_DtaClosed.Visible = false;
                    this.lblInit_DtaClosed.Text = Utils.Languages.GetLabelFromCode("VisibilityOneField", language);
                    break;
                case 1: //Intervallo
                    this.txtInitDtaClosed.ReadOnly = false;
                    this.txtEndDtaClosed.ReadOnly = false;
                    this.lblEnd_DtaClosed.Visible = true;
                    this.lblInit_DtaClosed.Visible = true;
                    this.txtEndDtaClosed.Visible = true;
                    this.lblInit_DtaClosed.Text = Utils.Languages.GetLabelFromCode("VisibilityFrom", language);
                    this.lblEnd_DtaClosed.Text = Utils.Languages.GetLabelFromCode("VisibilityTo", language);
                    break;
                case 2: //Oggi
                    this.lblEnd_DtaClosed.Visible = false;
                    this.txtEndDtaClosed.Visible = false;
                    this.txtInitDtaClosed.ReadOnly = true;
                    this.txtInitDtaClosed.Text = NttDataWA.Utils.dateformat.toDay();
                    break;
                case 3: //Settimana corrente
                    this.lblEnd_DtaClosed.Visible = true;
                    this.txtEndDtaClosed.Visible = true;
                    this.txtInitDtaClosed.Text = NttDataWA.Utils.dateformat.getFirstDayOfWeek();
                    this.txtEndDtaClosed.Text = NttDataWA.Utils.dateformat.getLastDayOfWeek();
                    this.txtEndDtaClosed.ReadOnly = true;
                    this.txtInitDtaClosed.ReadOnly = true;
                    this.lblInit_DtaClosed.Text = Utils.Languages.GetLabelFromCode("VisibilityFrom", language);
                    this.lblEnd_DtaClosed.Text = Utils.Languages.GetLabelFromCode("VisibilityTo", language);
                    break;
                case 4: //Mese corrente
                    this.lblEnd_DtaClosed.Visible = true;
                    this.txtEndDtaClosed.Visible = true;
                    this.txtInitDtaClosed.Text = NttDataWA.Utils.dateformat.getFirstDayOfMonth();
                    this.txtEndDtaClosed.Text = NttDataWA.Utils.dateformat.getLastDayOfMonth();
                    this.txtEndDtaClosed.ReadOnly = true;
                    this.txtInitDtaClosed.ReadOnly = true;
                    this.lblInit_DtaClosed.Text = Utils.Languages.GetLabelFromCode("VisibilityFrom", language);
                    this.lblEnd_DtaClosed.Text = Utils.Languages.GetLabelFromCode("VisibilityTo", language);
                    break;
            }









            //this.txtEndDtaClosed.Text = "";

            //if (this.ddl_dtaClosed.SelectedIndex == 0)
            //{
            //    this.pnlEndDtaClosed.Visible = false;
            //    this.lblInit_DtaClosed.Visible = false;
            //}
            //else
            //{
            //    this.pnlEndDtaClosed.Visible = true;
            //    this.lblInit_DtaClosed.Visible = true;
            //}
        }

        protected void ddl_dtaCreated_SelectedIndexChanged(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);
            string language = UIManager.UserManager.GetUserLanguage();
            switch (this.ddl_dtaCreated.SelectedIndex)
            {
                case 0: //Valore singolo
                    this.txtInitDtaCreated.ReadOnly = false;
                    this.txtEndDtaCreated.Visible = false;
                    this.litEnd_DataCreated.Visible = false;
                    this.lblInit_DtaCreated.Text = Utils.Languages.GetLabelFromCode("VisibilityOneField", language);
                    break;
                case 1: //Intervallo
                    this.txtInitDtaCreated.ReadOnly = false;
                    this.txtEndDtaCreated.ReadOnly = false;
                    this.litEnd_DataCreated.Visible = true;
                    this.lblInit_DtaCreated.Visible = true;
                    this.txtEndDtaCreated.Visible = true;
                    this.lblInit_DtaCreated.Text = Utils.Languages.GetLabelFromCode("VisibilityFrom", language);
                    this.litEnd_DataCreated.Text = Utils.Languages.GetLabelFromCode("VisibilityTo", language);
                    break;
                case 2: //Oggi
                    this.litEnd_DataCreated.Visible = false;
                    this.txtEndDtaCreated.Visible = false;
                    this.txtInitDtaCreated.ReadOnly = true;
                    this.txtInitDtaCreated.Text = NttDataWA.Utils.dateformat.toDay();
                    break;
                case 3: //Settimana corrente
                    this.litEnd_DataCreated.Visible = true;
                    this.txtEndDtaCreated.Visible = true;
                    this.txtInitDtaCreated.Text = NttDataWA.Utils.dateformat.getFirstDayOfWeek();
                    this.txtEndDtaCreated.Text = NttDataWA.Utils.dateformat.getLastDayOfWeek();
                    this.txtEndDtaCreated.ReadOnly = true;
                    this.txtInitDtaCreated.ReadOnly = true;
                    this.lblInit_DtaCreated.Text = Utils.Languages.GetLabelFromCode("VisibilityFrom", language);
                    this.litEnd_DataCreated.Text = Utils.Languages.GetLabelFromCode("VisibilityTo", language);
                    break;
                case 4: //Mese corrente
                    this.litEnd_DataCreated.Visible = true;
                    this.txtEndDtaCreated.Visible = true;
                    this.txtInitDtaCreated.Text = NttDataWA.Utils.dateformat.getFirstDayOfMonth();
                    this.txtEndDtaCreated.Text = NttDataWA.Utils.dateformat.getLastDayOfMonth();
                    this.txtEndDtaCreated.ReadOnly = true;
                    this.txtInitDtaCreated.ReadOnly = true;
                    this.lblInit_DtaCreated.Text = Utils.Languages.GetLabelFromCode("VisibilityFrom", language);
                    this.litEnd_DataCreated.Text = Utils.Languages.GetLabelFromCode("VisibilityTo", language);
                    break;
            }


            //this.txtInitDtaCreated.Text = "";

            //if (this.ddl_dtaCreated.SelectedIndex == 0)
            //{
            //    this.pnlEndDtaCreated.Visible = false;
            //    this.lblInit_DtaCreated.Visible = false;
            //}
            //else
            //{
            //    this.pnlEndDtaCreated.Visible = true;
            //    this.lblInit_DtaCreated.Visible = true;
            //}
        }

        protected void ddl_dtaExpire_SelectedIndexChanged(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);

            string language = UIManager.UserManager.GetUserLanguage();
            switch (this.ddl_dtaExpire.SelectedIndex)
            {
                case 0: //Valore singolo
                    this.dtaExpire_TxtFrom.ReadOnly = false;
                    this.dtaExpire_TxtTo.Visible = false;
                    this.lbl_dtaExpireTo.Visible = false;
                    this.lbl_dtaExpireFrom.Text = Utils.Languages.GetLabelFromCode("VisibilityOneField", language);
                    break;
                case 1: //Intervallo
                    this.dtaExpire_TxtFrom.ReadOnly = false;
                    this.dtaExpire_TxtTo.ReadOnly = false;
                    this.lbl_dtaExpireTo.Visible = true;
                    this.lbl_dtaExpireFrom.Visible = true;
                    this.dtaExpire_TxtTo.Visible = true;
                    this.lbl_dtaExpireFrom.Text = Utils.Languages.GetLabelFromCode("VisibilityFrom", language);
                    this.lbl_dtaExpireTo.Text = Utils.Languages.GetLabelFromCode("VisibilityTo", language);
                    break;
                case 2: //Oggi
                    this.lbl_dtaExpireTo.Visible = false;
                    this.dtaExpire_TxtTo.Visible = false;
                    this.dtaExpire_TxtFrom.ReadOnly = true;
                    this.dtaExpire_TxtFrom.Text = NttDataWA.Utils.dateformat.toDay();
                    break;
                case 3: //Settimana corrente
                    this.lbl_dtaExpireTo.Visible = true;
                    this.dtaExpire_TxtTo.Visible = true;
                    this.dtaExpire_TxtFrom.Text = NttDataWA.Utils.dateformat.getFirstDayOfWeek();
                    this.dtaExpire_TxtTo.Text = NttDataWA.Utils.dateformat.getLastDayOfWeek();
                    this.dtaExpire_TxtTo.ReadOnly = true;
                    this.dtaExpire_TxtFrom.ReadOnly = true;
                    this.lbl_dtaExpireFrom.Text = Utils.Languages.GetLabelFromCode("VisibilityFrom", language);
                    this.lbl_dtaExpireTo.Text = Utils.Languages.GetLabelFromCode("VisibilityTo", language);
                    break;
                case 4: //Mese corrente
                    this.lbl_dtaExpireTo.Visible = true;
                    this.dtaExpire_TxtTo.Visible = true;
                    this.dtaExpire_TxtFrom.Text = NttDataWA.Utils.dateformat.getFirstDayOfMonth();
                    this.dtaExpire_TxtTo.Text = NttDataWA.Utils.dateformat.getLastDayOfMonth();
                    this.dtaExpire_TxtTo.ReadOnly = true;
                    this.dtaExpire_TxtFrom.ReadOnly = true;
                    this.lbl_dtaExpireFrom.Text = Utils.Languages.GetLabelFromCode("VisibilityFrom", language);
                    this.lbl_dtaExpireTo.Text = Utils.Languages.GetLabelFromCode("VisibilityTo", language);
                    break;
            }

            this.upPnlIntervals.Update();
        }



        private void eseguiRicerca()
        {
            string newUrl;
            if (!ricercaFascicoli())
            {
                return;
            }
        }


        private bool ricercaFascicoli()
        {
            bool result = true;
            try
            {
                //array contenitore degli array filtro di ricerca
                qV = new NttDataWA.DocsPaWR.FiltroRicerca[1][];
                qV[0] = new NttDataWA.DocsPaWR.FiltroRicerca[1];
                fVList = new NttDataWA.DocsPaWR.FiltroRicerca[0];

                #region Filtro diTitolario

                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriFascicolazione.ID_TITOLARIO.ToString();
                fV1.valore = this.ddlTitolario.SelectedValue;
                fVList = GridManager.AddToArrayFiltroRicerca(fVList, fV1);

                #endregion
                #region Filtro su codice classificazione fascicolo

                if (this.TxtCodeProject.Text != string.Empty)
                {
                    fV1 = new NttDataWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriFascicolazione.CODICE_CLASSIFICA.ToString();
                    fV1.valore = this.TxtCodeProject.Text;
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }

                #endregion
                #region  filtro sulla tipologia del fascicolo
                if (ddl_typeProject.SelectedIndex > 0)
                {
                    fV1 = new NttDataWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriFascicolazione.TIPO_FASCICOLO.ToString();
                    fV1.valore = ddl_typeProject.SelectedItem.Value;
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                #endregion
                #region  filtro sullo stato del fascicolo
                if (ddlStatus.SelectedIndex > 0)
                {
                    fV1 = new NttDataWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriFascicolazione.STATO.ToString();
                    fV1.valore = ddlStatus.SelectedItem.Value;
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);

                }
                #endregion
                #region numero fascicolo
                if (!this.TxtNumProject.Text.Equals(""))
                {
                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriFascicolazione.NUMERO_FASCICOLO.ToString();
                    fV1.valore = this.TxtNumProject.Text.ToString();
                    if (utils.isNumeric(fV1.valore))
                    {
                        fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('ErrorSearchProjectFilterNum', 'error', '');} else {parent.ajaxDialogModal('ErrorSearchProjectFilterNum', 'error', '');}; parent.$('" + this.TxtNumProject.ClientID + "').focus();", true);
                        return false;
                    }
                }
                #endregion
                #region anno fascicolo
                if (!this.TxtYear.Text.Equals(""))
                {
                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriFascicolazione.ANNO_FASCICOLO.ToString();
                    fV1.valore = this.TxtYear.Text.ToString();
                    if (utils.isNumeric(fV1.valore))
                    {
                        fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('ErrorSearchProjectFilterYear', 'error', '');} else {parent.ajaxDialogModal('ErrorSearchProjectFilterYear', 'error', '');}; parent.$('" + this.TxtYear.ClientID + "').focus();", true);
                        return false;
                    }
                }
                #endregion
                #region  filtro sulla data di apertura fascicolo
                if (this.ddl_dtaOpen.SelectedIndex == 2)
                {
                    fV1 = new NttDataWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriFascicolazione.APERTURA_IL.ToString();
                    fV1.valore = this.txtInitDtaOpen.Text;
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                if (this.ddl_dtaOpen.SelectedIndex == 3)
                {
                    // siamo nel caso di Settimana corrente
                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriFascicolazione.APERTURA_SC.ToString();
                    fV1.valore = "1";
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                if (this.ddl_dtaOpen.SelectedIndex == 4)
                {
                    // siamo nel caso di Mese corrente
                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriFascicolazione.APERTURA_MC.ToString();
                    fV1.valore = "1";
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                if (this.ddl_dtaOpen.SelectedIndex == 0)
                {//valore singolo carico DATA_APERTURA
                    if (!String.IsNullOrEmpty(this.txtInitDtaOpen.Text))
                    {
                        if (!utils.isDate(this.txtInitDtaOpen.Text))
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('ErrorSearchProjectFilterDate', 'error', '');} else {parent.ajaxDialogModal('ErrorSearchProjectFilterDate', 'error', '');};", true);
                            return false;
                        }

                        fV1 = new NttDataWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriFascicolazione.APERTURA_IL.ToString();
                        fV1.valore = this.txtInitDtaOpen.Text;
                        fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                }
                if (this.ddl_dtaOpen.SelectedIndex == 1)
                {//valore singolo carico DATA_APERTURA_DAL - DATA_APERTURA_AL
                    if (!String.IsNullOrEmpty(this.txtInitDtaOpen.Text))
                    {
                        if (!utils.isDate(this.txtInitDtaOpen.Text))
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('ErrorSearchProjectFilterDate', 'error', '');} else {parent.ajaxDialogModal('ErrorSearchProjectFilterDate', 'error', '');};", true);
                            return false;
                        }

                        fV1 = new NttDataWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriFascicolazione.APERTURA_SUCCESSIVA_AL.ToString();
                        fV1.valore = this.txtInitDtaOpen.Text;
                        fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                    if (!this.txtEndDtaOpen.Text.Equals(""))
                    {
                        if (!utils.isDate(this.txtEndDtaOpen.Text))
                        {
                            Response.Write("<script>alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');</script>");
                            Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
                            return false;
                        }
                        fV1 = new NttDataWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriFascicolazione.APERTURA_PRECEDENTE_IL.ToString();
                        fV1.valore = this.txtEndDtaOpen.Text;
                        fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                }
                #endregion
                #region  filtro sulla data chiusura di un fascicolo
                if (this.ddl_dtaClosed.SelectedIndex == 2)
                {
                    fV1 = new DocsPaWR.FiltroRicerca();
                    //fV1.argomento = DocsPaWR.FiltriFascicolazione.CHIUSURA_TODAY.ToString();
                    //fV1.valore = "1";
                    fV1.argomento = DocsPaWR.FiltriFascicolazione.CHIUSURA_IL.ToString();
                    fV1.valore = this.txtInitDtaClosed.Text;

                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                if (this.ddl_dtaClosed.SelectedIndex == 3)
                {
                    // siamo nel caso di Settimana corrente
                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriFascicolazione.CHIUSURA_SC.ToString();
                    fV1.valore = "1";
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                if (this.ddl_dtaClosed.SelectedIndex == 4)
                {
                    // siamo nel caso di Mese corrente
                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriFascicolazione.CHIUSURA_MC.ToString();
                    fV1.valore = "1";
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                if (this.ddl_dtaClosed.SelectedIndex == 0)
                {//valore singolo carico DATA_CHIUSURA
                    if (!String.IsNullOrEmpty(this.txtInitDtaClosed.Text))
                    {
                        if (!utils.isDate(this.txtInitDtaClosed.Text))
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('ErrorSearchProjectFilterDate', 'error', '');} else {parent.ajaxDialogModal('ErrorSearchProjectFilterDate', 'error', '');};", true);
                            return false;
                        }

                        fV1 = new NttDataWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriFascicolazione.CHIUSURA_IL.ToString();
                        fV1.valore = this.txtInitDtaClosed.Text;
                        fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                }
                if (this.ddl_dtaClosed.SelectedIndex == 1)
                {//valore singolo carico DATA_CHIUSURA_DAL - DATA_CHIUSURA_AL
                    if (!String.IsNullOrEmpty(this.txtInitDtaClosed.Text))
                    {
                        if (!utils.isDate(this.txtInitDtaClosed.Text))
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('ErrorSearchProjectFilterDate', 'error', '');} else {parent.ajaxDialogModal('ErrorSearchProjectFilterDate', 'error', '');};", true);
                            return false;
                        }

                        fV1 = new NttDataWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriFascicolazione.CHIUSURA_SUCCESSIVA_AL.ToString();
                        fV1.valore = this.txtInitDtaClosed.Text;
                        fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                    if (!String.IsNullOrEmpty(this.txtInitDtaClosed.Text))
                    {
                        if (!utils.isDate(this.txtInitDtaClosed.Text))
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('ErrorSearchProjectFilterDate', 'error', '');} else {parent.ajaxDialogModal('ErrorSearchProjectFilterDate', 'error', '');};", true);
                            return false;
                        }
                        fV1 = new NttDataWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriFascicolazione.CHIUSURA_PRECEDENTE_IL.ToString();
                        fV1.valore = this.txtInitDtaClosed.Text;
                        fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                }
                #endregion
                #region  filtro sulla data creazione di un fascicolo
                if (this.ddl_dtaCreated.SelectedIndex == 2)
                {
                    fV1 = new DocsPaWR.FiltroRicerca();
                    //fV1.argomento = DocsPaWR.FiltriFascicolazione.CREAZIONE_TODAY.ToString();
                    //fV1.valore = "1";
                    fV1.argomento = DocsPaWR.FiltriFascicolazione.CREAZIONE_IL.ToString();
                    fV1.valore = this.txtInitDtaCreated.Text;

                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                if (this.ddl_dtaCreated.SelectedIndex == 3)
                {
                    // siamo nel caso di Settimana corrente
                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriFascicolazione.CREAZIONE_SC.ToString();
                    fV1.valore = "1";
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                if (this.ddl_dtaCreated.SelectedIndex == 4)
                {
                    // siamo nel caso di Mese corrente
                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriFascicolazione.CREAZIONE_MC.ToString();
                    fV1.valore = "1";
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                if (this.ddl_dtaCreated.SelectedIndex == 0)
                {//valore singolo carico DATA_CREAZIONE
                    if (!String.IsNullOrEmpty(this.txtInitDtaCreated.Text))
                    {
                        if (!utils.isDate(this.txtInitDtaCreated.Text))
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('ErrorSearchProjectFilterDate', 'error', '');} else {parent.ajaxDialogModal('ErrorSearchProjectFilterDate', 'error', '');};", true);
                            return false;
                        }

                        fV1 = new NttDataWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriFascicolazione.CREAZIONE_IL.ToString();
                        fV1.valore = this.txtInitDtaCreated.Text;
                        fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                }
                if (this.ddl_dtaCreated.SelectedIndex == 1)
                {//valore singolo carico DATA_CREAZIONE_DAL - DATA_CREAZIONE_AL
                    if (!String.IsNullOrEmpty(this.txtInitDtaCreated.Text))
                    {
                        if (!utils.isDate(this.txtInitDtaCreated.Text))
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('ErrorSearchProjectFilterDate', 'error', '');} else {parent.ajaxDialogModal('ErrorSearchProjectFilterDate', 'error', '');};", true);
                            return false;
                        }

                        fV1 = new NttDataWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriFascicolazione.CREAZIONE_SUCCESSIVA_AL.ToString();
                        fV1.valore = this.txtInitDtaCreated.Text;
                        fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                    if (!String.IsNullOrEmpty(this.txtEndDtaCreated.Text))
                    {
                        if (!utils.isDate(this.txtEndDtaCreated.Text))
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('ErrorSearchProjectFilterDate', 'error', '');} else {parent.ajaxDialogModal('ErrorSearchProjectFilterDate', 'error', '');};", true);
                            return false;
                        }
                        fV1 = new NttDataWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriFascicolazione.CREAZIONE_PRECEDENTE_IL.ToString();
                        fV1.valore = this.txtEndDtaCreated.Text;
                        fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                }
                #endregion
                #region  filtro sulla data scadenza di un fascicolo
                if (this.ddl_dtaExpire.SelectedIndex == 2)
                {
                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriFascicolazione.SCADENZA_IL.ToString();
                    fV1.valore = this.dtaExpire_TxtFrom.Text;
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);

                }
                if (this.ddl_dtaExpire.SelectedIndex == 3)
                {
                    // siamo nel caso di Settimana corrente
                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriFascicolazione.SCADENZA_SC.ToString();
                    fV1.valore = "1";
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                if (this.ddl_dtaExpire.SelectedIndex == 4)
                {
                    // siamo nel caso di Mese corrente
                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriFascicolazione.SCADENZA_MC.ToString();
                    fV1.valore = "1";
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                if (this.ddl_dtaExpire.SelectedIndex == 0)
                {//valore singolo carico DATA_SCADENZA
                    if (!String.IsNullOrEmpty(dtaExpire_TxtFrom.Text))
                    {
                        if (!utils.isDate(this.dtaExpire_TxtFrom.Text))
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('ErrorSearchProjectFilterDate', 'error', '');} else {parent.ajaxDialogModal('ErrorSearchProjectFilterDate', 'error', '');};", true);
                            return false;
                        }

                        fV1 = new DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriFascicolazione.SCADENZA_IL.ToString();
                        fV1.valore = this.dtaExpire_TxtFrom.Text;
                        fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                }
                if (this.ddl_dtaExpire.SelectedIndex == 1)
                {//valore singolo carico DATA_SCADENZA_DAL - DATA_SCADENZA_AL
                    if (!String.IsNullOrEmpty(this.dtaExpire_TxtFrom.Text))
                    {
                        if (!utils.isDate(this.dtaExpire_TxtFrom.Text))
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('ErrorSearchProjectFilterDate', 'error', '');} else {parent.ajaxDialogModal('ErrorSearchProjectFilterDate', 'error', '');};", true);
                            return false;
                        }

                        fV1 = new DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriFascicolazione.SCADENZA_SUCCESSIVA_AL.ToString();
                        fV1.valore = this.dtaExpire_TxtFrom.Text;
                        fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                    if (!String.IsNullOrEmpty(dtaExpire_TxtTo.Text))
                    {
                        if (!utils.isDate(this.dtaExpire_TxtTo.Text))
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('ErrorSearchProjectFilterDate', 'error', '');} else {parent.ajaxDialogModal('ErrorSearchProjectFilterDate', 'error', '');};", true);
                            return false;
                        }
                        fV1 = new DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriFascicolazione.SCADENZA_PRECEDENTE_IL.ToString();
                        fV1.valore = this.dtaExpire_TxtTo.Text;
                        fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                }
                #endregion
                #region descrizione
                if (!this.txt_Description.Text.Equals(""))
                {

                    fV1 = new NttDataWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriFascicolazione.TITOLO.ToString();
                    fV1.valore = this.txt_Description.Text.ToString();
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);

                }
                #endregion
                #region data Locazione Fisica
                if (this.ddl_dtaCollocation.SelectedIndex == 2)
                {
                    fV1 = new DocsPaWR.FiltroRicerca();
                    //fV1.argomento = DocsPaWR.FiltriFascicolazione.DATA_LF_TODAY.ToString();
                    //fV1.valore = "1";
                    fV1.argomento = DocsPaWR.FiltriFascicolazione.DATA_LF_IL.ToString();
                    fV1.valore = this.dtaCollocation_TxtFrom.Text;

                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                if (this.ddl_dtaCollocation.SelectedIndex == 3)
                {
                    // siamo nel caso di Settimana corrente
                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriFascicolazione.DATA_LF_SC.ToString();
                    fV1.valore = "1";
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                if (this.ddl_dtaCollocation.SelectedIndex == 4)
                {
                    // siamo nel caso di Mese corrente
                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriFascicolazione.DATA_LF_MC.ToString();
                    fV1.valore = "1";
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                if (this.ddl_dtaCollocation.SelectedIndex == 0)
                {//valore singolo carico DATA_LF
                    if (!String.IsNullOrEmpty(this.dtaCollocation_TxtFrom.Text))
                    {
                        if (!utils.isDate(this.dtaCollocation_TxtFrom.Text))
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('ErrorSearchProjectFilterDate', 'error', '');} else {parent.ajaxDialogModal('ErrorSearchProjectFilterDate', 'error', '');};", true);
                            return false;
                        }

                        fV1 = new DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriFascicolazione.DATA_LF_IL.ToString();
                        fV1.valore = this.dtaCollocation_TxtFrom.Text;
                        fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                }
                if (this.ddl_dtaCollocation.SelectedIndex == 1)
                {//valore singolo carico DATA_LF_DAL - DATA_LF_AL
                    if (!String.IsNullOrEmpty(this.dtaCollocation_TxtFrom.Text))
                    {
                        if (!utils.isDate(this.dtaCollocation_TxtFrom.Text))
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('ErrorSearchProjectFilterDate', 'error', '');} else {parent.ajaxDialogModal('ErrorSearchProjectFilterDate', 'error', '');};", true);
                            return false;
                        }

                        fV1 = new DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriFascicolazione.DATA_LF_SUCCESSIVA_AL.ToString();
                        fV1.valore = this.dtaCollocation_TxtFrom.Text;
                        fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                    if (!String.IsNullOrEmpty(this.dtaCollocation_TxtTo.Text))
                    {
                        if (!utils.isDate(this.dtaCollocation_TxtTo.Text))
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('ErrorSearchProjectFilterDate', 'error', '');} else {parent.ajaxDialogModal('ErrorSearchProjectFilterDate', 'error', '');};", true);
                            return false;
                        }
                        fV1 = new DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriFascicolazione.DATA_LF_PRECEDENTE_IL.ToString();
                        fV1.valore = this.dtaCollocation_TxtTo.Text;
                        fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                }
                if (this.idCollocationAddr != null && this.idCollocationAddr.Value != "")
                {
                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriFascicolazione.ID_UO_LF.ToString();
                    string IdUoLF = this.idCollocationAddr.Value;
                    fV1.valore = IdUoLF;
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                #endregion
                #region filtro ufficio referente fascicolo
                //if (enableUfficioRef)
                //{
                //    //if (this.txt_desc_uffRef.Text!=null && !this.txt_desc_uffRef.Text.Equals(""))
                //    if (this.txt_cod_UffRef.Text != null && !this.txt_cod_UffRef.Text.Equals(""))
                //    {
                //        if (this.hd_systemIdUffRef != null && !this.hd_systemIdUffRef.Value.Equals(""))
                //        {
                //            fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                //            fV1.argomento = DocsPaWR.FiltriFascicolazione.ID_UO_REF.ToString();
                //            fV1.valore = this.hd_systemIdUffRef.Value;
                //            fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                //        }
                //        else
                //        {
                //            if (!IsStartupScriptRegistered("alert"))
                //            {
                //                Page.RegisterStartupScript("", "<script>alert('Codice rubrica non valido per l\\'Ufficio referente!');</script>");
                //                string s = "<SCRIPT language='javascript'>document.getElementById('" + txt_cod_UffRef.ID + "').focus() </SCRIPT>";
                //                RegisterStartupScript("focus", s);
                //            }

                //            return false;
                //        }

                //        //						else//old perchè uff ref è solo interno
                //        //						{
                //        //							fV1.argomento=DocsPaWR.FiltriFascicolazione.DESC_UO_REF.ToString();
                //        //							fV1.valore = DocsPaUtils.Functions.Functions.ReplaceApexes(this.txt_desc_uffRef.Text);
                //        //						}
                //    }

                //}
                #endregion
                #region Note Fascicolo
                //                if (!this.txtNote.Text.Equals(""))
                if (!this.TxtNoteProject.Text.Equals(""))
                {

                    fV1 = new NttDataWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriFascicolazione.VAR_NOTE.ToString();
                    string[] rf;
                    string rfsel = "0";
                    if (Session["RFNote"] != null && !string.IsNullOrEmpty(Session["RFNote"].ToString()))
                    {
                        rf = Session["RFNote"].ToString().Split('^');
                        rfsel = rf[1];
                    }
                    //                    fV1.valore = this.txtNote.Text.ToString();
                    fV1.valore = this.TxtNoteProject.Text.ToString() + "@-@" + rblFilterNote.SelectedValue + "@-@" + rfsel;// rn_note.TipoRicerca + "@-@" + rfsel;
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);

                }
                #endregion
                #region Mostra tutti i fascicoli

                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriFascicolazione.INCLUDI_FASCICOLI_FIGLI.ToString();
                fV1.valore = this.rbViewAllYes.Checked ? "S" : "N";
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);

                #endregion
                #region filtro tipologia fascicolo e profilazione dinamica

                if (System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamicaFasc"] != null && System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamicaFasc"] == "1")
                {
                    if (!string.IsNullOrEmpty(this.DocumentDdlTypeDocument.SelectedValue))
                    {
                        this.SaveTemplateProject();
                        fV1 = new DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriFascicolazione.PROFILAZIONE_DINAMICA.ToString();
                        fV1.template = this.Template;
                        fV1.valore = "Profilazione Dinamica";
                        fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                        //fV1 = new DocsPaWR.FiltroRicerca();
                        //fV1.template = this.Template;
                        //fV1.valore = "Profilazione Dinamica";
                        //fV1 = (DocsPaWR.FiltroRicerca)Session["filtroProfDinamica"];
                        //fVList = utils.addToArrayFiltroRicerca(fVList, fV1);

                        fV1 = new DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriFascicolazione.TIPOLOGIA_FASCICOLO.ToString();
                        fV1.valore = this.DocumentDdlTypeDocument.SelectedItem.Value;
                        fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                }

                #endregion
                #region filtro DIAGRAMMI DI STATO
                if (DocumentDdlStateDiagram.Visible && DocumentDdlStateDiagram.SelectedIndex != 0)
                {
                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriFascicolazione.DIAGRAMMA_STATO_FASC.ToString();
                    //string cond = " AND (DPA_DIAGRAMMI.ID_PROJECT = A.SYSTEM_ID AND DPA_DIAGRAMMI.ID_STATO = " + ddl_statiDoc.SelectedValue + ") ";
                    //fV1.valore = cond;
                    fV1.nomeCampo = ddlStateCondition.SelectedValue;
                    fV1.valore = DocumentDdlStateDiagram.SelectedValue;
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                    //string cond = " AND (DPA_DIAGRAMMI.ID_PROJECT = A.SYSTEM_ID AND DPA_DIAGRAMMI.ID_STATO = " + ddlStateCondition.SelectedValue + ") ";
                }
                #endregion filtro DIAGRAMMI DI STATO
                #region filtro RICERCA IN AREA LAVORO

                if ((chkInADL.Checked))
                {
                    fV1 = new NttDataWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriFascicolazione.DOC_IN_FASC_ADL.ToString();
                    fV1.valore = UserManager.GetInfoUser().idPeople.ToString() + "@" + UserManager.GetSelectedRole().systemId.ToString();
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                #endregion
                #region filtro CONSERVAZIONE
                if (this.chkConservation.Checked)
                {
                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriFascicolazione.CONSERVAZIONE.ToString();
                    fV1.valore = "1";
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                if (this.chkConservationNo.Checked)
                {
                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriFascicolazione.CONSERVAZIONE.ToString();
                    fV1.valore = "0";
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                #endregion
                #region FILTRO SUI SOTTOFASCICOLI

                fV1 = new NttDataWA.DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriFascicolazione.SOTTOFASCICOLO.ToString();
                fV1.valore = this.txt_subProject.Text;
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);

                #endregion

                #region Filtri Proprietario

                fV1 = new NttDataWA.DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriFascicolazione.CORR_TYPE_OWNER.ToString();
                fV1.valore = this.rblOwnerType.SelectedValue;
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);

                if (this.rblOwnerType.SelectedValue == "U")
                {
                    if (!this.txtCodiceProprietario.Text.Equals(""))
                    {
                        #region filtro su ID_UO_PROPRIETARIO
                        fV1 = new NttDataWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriFascicolazione.ID_OWNER.ToString();
                        fV1.valore = this.idProprietario.Value;
                        fVList = utils.addToArrayFiltroRicerca(fVList, fV1);

                        fV1 = new DocsPaWR.FiltroRicerca();
                        fV1.argomento = "EXTEND_TO_HISTORICIZED_AUTHOR";
                        fV1.valore = "true";
                        fVList = GridManager.AddToArrayFiltroRicerca(fVList, fV1);

                        #endregion
                    }
                    else
                    {
                        if (!this.txtDescrizioneProprietario.Text.Equals(""))
                        {
                            #region filtro su DESC_UO_CREATORE
                            fV1 = new NttDataWA.DocsPaWR.FiltroRicerca();
                            //cambiare con proprietario
                            fV1.argomento = DocsPaWR.FiltriFascicolazione.DESC_UO_CREATORE.ToString();
                            fV1.valore = this.txtDescrizioneProprietario.Text;
                            fVList = utils.addToArrayFiltroRicerca(fVList, fV1);


                            #endregion
                        }
                    }
                    #region filtro su UO sottoposte
                    //if (this.cbx_UOsotto.Checked)
                    //{
                    //    fV1 = new DocsPaWR.FiltroRicerca();
                    //    fV1.argomento = DocsPaWR.FiltriFascicolazione.UO_SOTTOPOSTE.ToString();
                    //    fV1.valore = "1";
                    //    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    //}
                    #endregion
                }
                else
                {
                    if (this.rblOwnerType.SelectedValue == "R")
                    {
                        if (!this.txtCodiceProprietario.Text.Equals(""))
                        {
                            #region filtro su ID_RUOLO_PROPRIETARIO
                            fV1 = new NttDataWA.DocsPaWR.FiltroRicerca();
                            fV1.argomento = DocsPaWR.FiltriFascicolazione.ID_OWNER.ToString();
                            fV1.valore = this.idProprietario.Value;
                            fVList = utils.addToArrayFiltroRicerca(fVList, fV1);

                            #endregion
                        }
                        else
                        {
                            if (!this.txtDescrizioneProprietario.Text.Equals(""))
                            {
                                #region filtro su DESC_RUOLO_CREATORE
                                fV1 = new NttDataWA.DocsPaWR.FiltroRicerca();
                                fV1.argomento = DocsPaWR.FiltriFascicolazione.DESC_RUOLO_CREATORE.ToString();
                                fV1.valore = this.txtDescrizioneProprietario.Text;
                                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                                #endregion
                            }
                        }
                    }
                    else
                    {
                        if (this.rblOwnerType.SelectedValue == "P")
                        {
                            if (!this.txtCodiceProprietario.Text.Equals(""))
                            {
                                #region filtro su ID_PEOPLE_PROPRIETARIO
                                fV1 = new NttDataWA.DocsPaWR.FiltroRicerca();
                                fV1.argomento = DocsPaWR.FiltriFascicolazione.ID_OWNER.ToString();
                                fV1.valore = this.idProprietario.Value;
                                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);

                                #endregion
                            }
                            else
                            {
                                if (!this.txtDescrizioneProprietario.Text.Equals(""))
                                {
                                    #region filtro su DESC_PEOPLE_CREATORE
                                    fV1 = new NttDataWA.DocsPaWR.FiltroRicerca();
                                    fV1.argomento = DocsPaWR.FiltriFascicolazione.DESC_PEOPLE_CREATORE.ToString();
                                    fV1.valore = this.txtDescrizioneProprietario.Text;
                                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                                    #endregion
                                }
                            }
                        }
                    }
                }


                #endregion


                #region Filtri Creatore

                fV1 = new NttDataWA.DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriFascicolazione.CORR_TYPE_OWNER.ToString();
                fV1.valore = this.rblOwnerType.SelectedValue;
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);

                if (this.rblOwnerTypeCreator.SelectedValue == "U")
                {
                    if (!this.txtCodiceCreatore.Text.Equals(""))
                    {
                        #region filtro su ID_UO_CREATORE
                        fV1 = new NttDataWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriFascicolazione.ID_UO_CREATORE.ToString();
                        fV1.valore = this.idProprietario.Value;
                        fVList = utils.addToArrayFiltroRicerca(fVList, fV1);

                        #endregion
                    }
                    else
                    {
                        if (!this.txtDescrizioneCreatore.Text.Equals(""))
                        {
                            #region filtro su DESC_UO_CREATORE
                            fV1 = new NttDataWA.DocsPaWR.FiltroRicerca();
                            fV1.argomento = DocsPaWR.FiltriFascicolazione.DESC_UO_CREATORE.ToString();
                            fV1.valore = this.txtDescrizioneProprietario.Text;
                            fVList = utils.addToArrayFiltroRicerca(fVList, fV1);


                            #endregion
                        }
                    }
                    #region filtro su UO sottoposte
                    //if (this.cbx_UOsotto.Checked)
                    //{
                    //    fV1 = new DocsPaWR.FiltroRicerca();
                    //    fV1.argomento = DocsPaWR.FiltriFascicolazione.UO_SOTTOPOSTE.ToString();
                    //    fV1.valore = "1";
                    //    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    //}
                    #endregion
                }
                else
                {
                    if (this.rblOwnerTypeCreator.SelectedValue == "R")
                    {
                        if (!this.txtCodiceCreatore.Text.Equals(""))
                        {
                            #region filtro su ID_RUOLO_CREATORE
                            fV1 = new NttDataWA.DocsPaWR.FiltroRicerca();
                            fV1.argomento = DocsPaWR.FiltriFascicolazione.ID_RUOLO_CREATORE.ToString();
                            fV1.valore = this.idProprietario.Value;
                            fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                            #endregion
                        }
                        else
                        {
                            if (!this.txtDescrizioneCreatore.Text.Equals(""))
                            {
                                #region filtro su DESC_RUOLO_CREATORE
                                fV1 = new NttDataWA.DocsPaWR.FiltroRicerca();
                                fV1.argomento = DocsPaWR.FiltriFascicolazione.DESC_RUOLO_CREATORE.ToString();
                                fV1.valore = this.txtDescrizioneProprietario.Text;
                                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                                #endregion
                            }
                        }
                    }
                    else
                    {
                        if (this.rblOwnerTypeCreator.SelectedValue == "P")
                        {
                            if (!this.txtCodiceCreatore.Text.Equals(""))
                            {
                                #region filtro su ID_PEOPLE_CREATORE
                                fV1 = new NttDataWA.DocsPaWR.FiltroRicerca();
                                fV1.argomento = DocsPaWR.FiltriFascicolazione.ID_PEOPLE_CREATORE.ToString();
                                fV1.valore = this.idProprietario.Value;
                                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                                #endregion
                            }
                            else
                            {
                                if (!this.txtDescrizioneCreatore.Text.Equals(""))
                                {
                                    #region filtro su DESC_PEOPLE_CREATORE
                                    fV1 = new NttDataWA.DocsPaWR.FiltroRicerca();
                                    fV1.argomento = DocsPaWR.FiltriFascicolazione.DESC_PEOPLE_CREATORE.ToString();
                                    fV1.valore = this.txtDescrizioneProprietario.Text;
                                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                                    #endregion
                                }
                            }
                        }
                    }
                }


                #endregion
                #region Odinamento contatore no griglie custum
                ////Se non ho le griglie custom
                //if (!GridManager.IsRoleEnabledToUseGrids())
                //{
                //    if (ddl_tipoFasc.SelectedItem != null && !string.IsNullOrEmpty(ddl_tipoFasc.SelectedItem.Text))
                //    {
                //        DocsPAWA.DocsPaWR.Templates template = ProfilazioneFascManager.getTemplateFascById(ddl_tipoFasc.SelectedItem.Value, this);
                //        if (template != null)
                //        {
                //            OggettoCustom customObjectTemp = new OggettoCustom();
                //            customObjectTemp = template.ELENCO_OGGETTI.Where(
                //            r => r.TIPO.DESCRIZIONE_TIPO.ToUpper() == "CONTATORE" && r.DA_VISUALIZZARE_RICERCA == "1").
                //            FirstOrDefault();
                //            if (customObjectTemp != null && ddlOrder != null && ddlOrder.SelectedValue != null && ddlOrder.SelectedValue.Equals("-2"))
                //            {
                //                fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                //                fV1.argomento = DocsPaWR.FiltriDocumento.CONTATORE_GRIGLIE_NO_CUSTOM.ToString();
                //                fV1.valore = customObjectTemp.TIPO_CONTATORE;
                //                fV1.nomeCampo = template.SYSTEM_ID.ToString();
                //                fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);

                //                // Creazione di un filtro per la profilazione
                //                fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                //                fV1.argomento = FiltriDocumento.PROFILATION_FIELD_FOR_ORDER.ToString();
                //                fV1.valore = customObjectTemp.SYSTEM_ID.ToString();
                //                fV1.nomeCampo = customObjectTemp.DESCRIZIONE;
                //                fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                //            }
                //        }


                //    }
                //}

                #endregion


                #region filtro Deposito
                //if (!this.rbl_TrasfDep.SelectedItem.Value.Equals("T"))
                //{
                //    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                //    fV1.argomento = DocsPaWR.FiltriFascicolazione.DEPOSITO.ToString();
                //    fV1.valore = this.rbl_TrasfDep.SelectedItem.Value;
                //    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                //}
                #endregion

                #region Ordinamento

                //// Reperimento del filtro da utilizzare per la griglia
                //List<FiltroRicerca> filterList = GridManager.GetOrderFilterForProject(this.ddlOrder.SelectedValue, this.ddlOrderDirection.SelectedValue);

                //// Se la lista è valorizzata vengono aggiunti i filtri
                //if (filterList != null)
                //{
                //    foreach (FiltroRicerca filter in filterList)
                //    {
                //        fVList = Utils.addToArrayFiltroRicerca(fVList, filter);

                //        if (filter.argomento.Equals("ORDER_DIRECTION") && GridManager.IsRoleEnabledToUseGrids())
                //        {
                //            OrderDirectionEnum tempEnum = OrderDirectionEnum.Desc;
                //            if (filter.valore.ToUpper().Equals("ASC"))
                //            {
                //                tempEnum = OrderDirectionEnum.Asc;
                //            }
                //            else
                //            {
                //                tempEnum = OrderDirectionEnum.Desc;
                //            }

                //            if (GridManager.SelectedGrid.OrderDirection != tempEnum)
                //            {
                //                GridManager.SelectedGrid.OrderDirection = tempEnum;
                //                GridManager.SelectedGrid.GridId = string.Empty;
                //            }
                //        }
                //        if (filter.argomento.Equals("ORACLE_FIELD_FOR_ORDER") && GridManager.IsRoleEnabledToUseGrids())
                //        {
                //            Field d = (Field)GridManager.SelectedGrid.Fields.Where(e => e.FieldId.ToUpper().Equals((filter.nomeCampo).ToUpper())).FirstOrDefault();
                //            if ((GridManager.SelectedGrid.FieldForOrder == null && d != null) || GridManager.SelectedGrid.FieldForOrder.FieldId != d.FieldId)
                //            {
                //                GridManager.SelectedGrid.FieldForOrder = d;
                //                GridManager.SelectedGrid.GridId = string.Empty;
                //            }
                //        }
                //    }
                //}

                #endregion

                #region Visibilità Tipica / Atipica
                //if (pnl_visiblitaFasc.Visible)
                //{
                //    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                //    fV1.argomento = DocsPaWR.FiltriDocumento.VISIBILITA_T_A.ToString();
                //    fV1.valore = rbl_visibilita.SelectedValue;
                //    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                //}
                #endregion Visibilità Tipica / Atipica

                #region  filtro su elenco valori file excel
                if (Session["fileExcel"] != null && Session["datiExcel"] != null)
                {
                    if (this.ddl_attributo.SelectedIndex == 0)
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('WarningSearchProjectUploadXlsAttributeNone', 'warning', '');", true);
                        return false;

                    }
                    else
                    {
                        fV1 = new FiltroRicerca();
                        fV1.argomento = FiltriFascicolazione.FILE_EXCEL.ToString();
                        string[] nomeFile = Session["fileExcel"].ToString().Split('\\');
                        fV1.valore = nomeFile[nomeFile.Length - 1]; ;

                        fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                        if (this.ddl_attributo.SelectedIndex != 5)
                        {
                            fV1 = new FiltroRicerca();
                            fV1.argomento = FiltriFascicolazione.ATTRIBUTO_EXCEL.ToString();
                            fV1.valore = this.ddl_attributo.SelectedValue;
                            fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                        }
                        else
                        {
                            fV1 = new FiltroRicerca();
                            fV1.argomento = FiltriFascicolazione.ATTRIBUTO_EXCEL.ToString();
                            fV1.valore = "TIPOLOGIA&" + this.ddl_tipoFascExcel.SelectedItem.Text + "&" + this.ddl_tipoFascExcel.SelectedItem.Value + "&" + this.ddl_attrTipoFascExcel.SelectedItem.Text + "&" + this.ddl_attrTipoFascExcel.SelectedItem.Value;
                            fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                        }

                        //Session.Remove("fileExcel");
                    }
                }
                else
                {
                    if (this.ddl_attributo.SelectedIndex > 0)
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('WarningSearchProjectUploadXlsIncorrect', 'warning', '');", true);
                        return false;

                    }
                }
                #endregion

                /* ABBATANGELI GIANLUIGI */
                /// <summary>
                /// Creazione oggetti di filtro per oggetto documento
                /// </summary>
                /// <param name="filterItems"></param>

                if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["FILTRO_APPLICAZIONE"]))
                {
                    fV1 = new NttDataWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.COD_EXT_APP.ToString();
                    fV1.valore = (System.Configuration.ConfigurationManager.AppSettings["FILTRO_APPLICAZIONE"]);//rbl_visibilita.SelectedValue;
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }

                qV[0] = fVList;

                ProjectManager.setFiltroRicFasc(this, qV);
                return result;
            }
            catch (System.Exception es)
            {
                ErrorManager.redirect(this, es);
                result = false;
                return result;
            }
        }


        private void SaveSearchFilters(DocsPaWR.FiltroRicerca[][] filters)
        {
            //NttDataWA.ricercaDoc.SchedaRicerca schedaRicerca = new NttDataWA.ricercaDoc.SchedaRicerca(KEY_SCHEDA_RICERCA);
            //schedaRicerca.FiltriRicerca = filters;
            //Session[this.SchedaRicercaSessionKey] = schedaRicerca;
        }

        private void LoadData(bool updateGrid, int numPage, int nRec, int numTotPage)
        {
            string codClass = "";
            //ricavo il parametro della classificazione selezionata dalla pagina del protocollo dalla sessione
            if (this.ProjectClass != null && !string.IsNullOrEmpty(this.ProjectClass.systemID))
                codClass = this.ProjectClass.codice;



            //if (codeChanged)
            //{
            //    codClass = "";
            //}

            DocsPaWR.FascicolazioneClassificazione classificazione = null;
            DocsPaWR.FiltroRicerca[][] filtroRicerca = null;

            InfoUtente infoUtente = UserManager.GetInfoUser();
            DocsPaWR.Registro regSel = null;

            SearchObject[] fascList = null;

            bool showChilds = false;

            showChilds = rbViewAllYes.Checked;

            if (codClass != null)
            {
                if (codClass != "")
                {
                    //Non è possibile al momento scegliere se visualizzare o meno i figli dei sottofascicoli. Vedere nella versione precedente
                    //cosa accade mettente showChilds a true; 

                    string mostraTutti = rbViewAllYes.Checked.ToString();
                    switch (mostraTutti)
                    {
                        case "S":
                            {
                                showChilds = true;
                                break;
                            }
                        case "N":
                            {
                                showChilds = false;
                                break;
                            }
                    }



                    // vuol dire che l'utente su docProfilo ha specificato un fascicolo

                    // - se è GENERALE, il codClass è il codice del nodo di titolario a cui è associato
                    // - se è PROCEDIMENTALE il codClass coincide con il codice del nodo di titolario
                    // sotto il quale il fascicolo è stato creato

                    if (this.CallerSearchProject != null && this.CallerSearchProject == "profilo")
                    {

                        string idRegistroNodoTit = string.Empty;
                        Fascicolo fasc = this.ProjectClass;
                        if (fasc != null)
                            idRegistroNodoTit = fasc.idRegistroNodoTit;
                        if (idRegistroNodoTit != null && idRegistroNodoTit != String.Empty)
                        {
                            regSel = UserManager.getRegistroBySistemId(this, idRegistroNodoTit);
                        }
                        else
                        {
                            regSel = UserManager.getRegistroBySistemId(this, DdlRegistries.SelectedItem.Value);
                        }
                    }
                    else
                    {
                        regSel = UIManager.RegistryManager.GetRegistryInSession();
                    }


                    DocsPaWR.FascicolazioneClassificazione[] titolario = new DocsPaWR.FascicolazioneClassificazione[1];

                    

                    //Commentato, non abbiamo la dll dei titolari
                    if (this.plcTitolario.Visible)
                        titolario = ProjectManager.fascicolazioneGetTitolario2(this, codClass, regSel, false, ddlTitolario.SelectedValue);
                    else
                        titolario = ProjectManager.fascicolazioneGetTitolario2(this, codClass, regSel, false, UIManager.ClassificationSchemeManager.getTitolarioAttivo(infoUtente.idAmministrazione).ID);

                    if (titolario == null || titolario.Length < 1)
                    {
                        //return;
                        //Response.Write("<script>window.alert('Attenzione, si è verificato un errore\\ndurante l'operazione di ricerca'); window.close();</script>");
                    }
                    if (titolario.Length > 0
                        && titolario[0] != null)
                        classificazione = titolario[0];


                    //FascicoliManager.setClassificazioneSelezionata(this, classificazione);
                    filtroRicerca = ProjectManager.getFiltriRicFasc(this);


                    // Lista dei system id dei fascicoli. Non utilizzato
                    SearchResultInfo[] idProjects = null;



                    if (this.CallerSearchProject != null && this.CallerSearchProject == "profilo")
                    {

                        fascList = ProjectManager.getListaFascicoliPagingCustom(classificazione, regSel, filtroRicerca[0], showChilds, numPage, out  numTotPage, out nRec, rowForPage, false, out idProjects, Session["DatiExcel"] as byte[], false, false, null, null, true);
                        //listaFascicoli = ProjectManager.getListaFascicoliPaging(this, classificazione, regSel, filtroRicerca[0], showChilds, numPage, out numTotPage, out nRec, rowForPage, false, out idProjects, null);
                    }
                    if (this.CallerSearchProject != null && this.CallerSearchProject == "protocollo")
                    {
                        fascList = ProjectManager.getListaFascicoliPagingCustom(classificazione, regSel, filtroRicerca[0], showChilds, numPage, out  numTotPage, out nRec, rowForPage, false, out idProjects, Session["DatiExcel"] as byte[], false, false, null, null, true);
                        //listaFascicoli = ProjectManager.getListaFascicoliPaging(this, classificazione, null, filtroRicerca[0], showChilds, numPage, out numTotPage, out nRec, rowForPage, false, out idProjects, null);
                    }
                    if (this.CallerSearchProject != null && this.CallerSearchProject == "classifica")
                    {
                        fascList = ProjectManager.getListaFascicoliPagingCustom(classificazione, regSel, filtroRicerca[0], showChilds, numPage, out  numTotPage, out nRec, rowForPage, false, out idProjects, Session["DatiExcel"] as byte[], false, false, null, null, true);
                        //listaFascicoli = ProjectManager.getListaFascicoliPaging(this, classificazione, null, filtroRicerca[0], showChilds, numPage, out numTotPage, out nRec, rowForPage, false, out idProjects, null);
                    }
                    if (this.CallerSearchProject != null && this.CallerSearchProject == "searchInstance")
                    {
                        fascList = ProjectManager.getListaFascicoliPagingCustom(classificazione, regSel, filtroRicerca[0], showChilds, numPage, out  numTotPage, out nRec, rowForPage, false, out idProjects, Session["DatiExcel"] as byte[], false, false, null, null, true);
                        //listaFascicoli = ProjectManager.getListaFascicoliPaging(this, classificazione, null, filtroRicerca[0], showChilds, numPage, out numTotPage, out nRec, rowForPage, false, out idProjects, null);
                    }
                    if (this.CallerSearchProject != null && this.CallerSearchProject == "search")
                    {
                        fascList = ProjectManager.getListaFascicoliPagingCustom(classificazione, regSel, filtroRicerca[0], showChilds, numPage, out  numTotPage, out nRec, rowForPage, false, out idProjects, Session["DatiExcel"] as byte[], false, false, null, null, true);
                        //listaFascicoli = ProjectManager.getListaFascicoliPaging(this, classificazione, null, filtroRicerca[0], showChilds, numPage, out numTotPage, out nRec, rowForPage, false, out idProjects, null);
                    }

                }
                else
                {
                    // CASO IN CUI l'utente non ha digitato alcun codice fascicolo nella pagina chiamante,
                    // quindi la ricerca dei fascicoli verrà effettuata su tutto il titolario
                    //Si stanno cercando i fascicoli nell'intero titolario, allora
                    //prendo,				
                    //nel protocollo: il registro su cui si sta protocollando
                    //nel profilo: il regitro selezionato dall'utente nella combo

                    regSel = UIManager.RegistryManager.GetRegistryInSession();

                    classificazione = null;

                    //verifico se è stato specificato un codice di titolario nella pagina searchproject altrimenti codClass rimane vuoto
                    if (this.ProjectClass != null)
                        codClass = this.ProjectClass.codice;


                    if (!string.IsNullOrEmpty(codClass))
                    {
                        DocsPaWR.FascicolazioneClassificazione[] titolario = ProjectManager.fascicolazioneGetTitolario2(this, codClass, regSel, false, UIManager.ClassificationSchemeManager.getTitolarioAttivo(infoUtente.idAmministrazione).ID);
                        classificazione = titolario[0];
                    }

                    //showChilds = true;
                    ProjectManager.setClassificazioneSelezionata(this, classificazione);
                    ProjectManager.setProjectInSessionForRicFasc(codClass);
                    filtroRicerca = ProjectManager.getFiltriRicFasc(this);

                    SearchResultInfo[] idProjects = null;
                    // listaFascicoli = ProjectManager.getListaFascicoliPaging(this, classificazione, regSel, filtroRicerca[0], showChilds, numPage, out numTotPage, out nRec, rowForPage, false, out idProjects, null);
                    fascList = ProjectManager.getListaFascicoliPagingCustom(classificazione, regSel, filtroRicerca[0], showChilds, numPage, out  numTotPage, out nRec, rowForPage, false, out idProjects, Session["DatiExcel"] as byte[], false, false, null, null, true);
                }

                bool outOfMaxRowSearchable;

                outOfMaxRowSearchable = (numTotPage == -2);
                if (outOfMaxRowSearchable)
                {
                    string valoreChiaveDB = Utils.InitConfigurationKeys.GetValue("0", DBKeys.MAX_ROW_SEARCHABLE.ToString());
                    if (valoreChiaveDB.Length == 0)
                    {
                        valoreChiaveDB = Utils.InitConfigurationKeys.GetValue("0", DBKeys.MAX_ROW_SEARCHABLE.ToString());
                    }
                    string msgDesc = "WarningSearchRecordNumber";
                    string msgCenter = Utils.Languages.GetMessageFromCode("WarningSearchRecordNumber2", UIManager.UserManager.GetUserLanguage());
                    string customError = nRec + " " + msgCenter + " " + valoreChiaveDB;
                    string errFormt = Server.UrlEncode(customError);
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + utils.FormatJs(msgDesc) + "', 'warning', '', '" + utils.FormatJs(customError) + "');} else {parent.ajaxDialogModal('" + utils.FormatJs(msgDesc) + "', 'warning', '', '" + utils.FormatJs(customError) + "');}; ", true);
                    nRec = 0;
                    return;
                }

            }

            if (fascList != null && fascList.Length > 0)
            {
                this.GrdFascResult.PageIndex = currentPage - 1;
                //this.GrdFascResult.DataSource = listaFascicoli;
                this.BindGrid(fascList);

                //aggiornamento label count fascicoli
                this.FolderFoundCount.Text = nRec.ToString();
                this.UpTypeResult.Update();

                this.GrdFascResult.Visible = true;
                this.upnlStruttura.Visible = true;
                //this.docsInFolderCount.Text = "0";
            }
            else
            {
                this.FolderFoundCount.Text = "0";
                this.UpTypeResult.Update();

                ClearAllGrid();
                //this.GrdFascResult.Visible = false;                              
                UpTypeResult.Visible = true;
            }
            this.lbl_countRecord.Visible = true;
            this.FolderFoundCount.Visible = true;


            // Filippo: modificato l'uso della variabile in sessione
            // con una usata solo all'interno della pagina searchProject.aspx
            // per evitare malfunzionamenti in altre pagine collegate
            // ad es. la fascicolazione massiva dei documenti in progetto
            // UIManager.ProjectManager.setProjectInSession(null);
          //  this.Project = null;

            this.PlcStructur.Visible = false;
            this.upnlStruttura.Update();
            this.treenode_sel = null;
            //appoggio il risultato in sessione
            //setto la HASHTABLE in sessione e la lista fascicoli
            ProjectManager.setHashFascicoli(this, m_hashTableFascicoli);
            ProjectManager.setHashFascicoliSelezionati(this, null);
            //RicercaFascicoliSessionMng.SetListaFascicoli(this, listaFascicoli);
        }


        protected void cbSel_CheckedChanged(object sender, EventArgs e)
        {
            BtnCollate.Enabled = true;

            if (HT_fascicoliSelezionati == null)
            {
                HT_fascicoliSelezionati = new Hashtable(GrdFascResult.Rows.Count);
            }

            m_hashTableFascicoli = ProjectManager.getHashFascicoli(this);
            string idfascicolo = this.RowSelected != null ? ((Label)RowSelected.FindControl("lblFascKey")).Text : string.Empty;
            foreach (GridViewRow dgItem in this.GrdFascResult.Rows)
            {
                string key = ((Label)dgItem.FindControl("lblFascKey")).Text;
                CheckBox cb = dgItem.FindControl("cbSel") as CheckBox;
                if (cb.Checked && !HT_fascicoliSelezionati.ContainsKey(key))
                {
                    HT_fascicoliSelezionati.Add(key, m_hashTableFascicoli[key]);
                    
                }
                else if (this.RowSelected != null && cb.Checked && key.Equals(idfascicolo))
                {
                    this.GrdFascResult.SelectedIndex = -1;
                    this.RowSelected = this.GrdFascResult.SelectedRow;

                    string folderId = this.treenode_sel.Value.Replace("node_", "").Replace("root_", "");
                    if (!string.IsNullOrEmpty(folderId))
                    {
                        HT_fascicoliSelezionati.Remove(folderId);
                        this.treenode_sel.Value = string.Empty;
                    }

                    this.PlcStructur.Visible = false;
                    this.upnlStruttura.Update();
                    this.UpPnlGridResult.Update();
                }
            }

            ProjectManager.setHashFascicoliSelezionati(this, HT_fascicoliSelezionati);
        }

        protected void rbSel_CheckedChanged(object sender, EventArgs e)
        {
            BtnCollate.Enabled = true;
            this.PlcStructur.Visible = false;
            this.upnlStruttura.Update();
            HT_fascicoliSelezionati = new Hashtable();

            RadioButton check = sender as System.Web.UI.WebControls.RadioButton;

            string key = ((Label)(sender as System.Web.UI.WebControls.RadioButton).Parent.FindControl("lblFascKey")).Text;


            if (check.Checked && !HT_fascicoliSelezionati.ContainsKey(key))
            {
                HT_fascicoliSelezionati.Add(key, m_hashTableFascicoli[key]);
                ProjectManager.setHashFascicoliSelezionati(this, HT_fascicoliSelezionati);
            }
            else
            {
                if (HT_fascicoliSelezionati.ContainsKey(key))
                {
                    HT_fascicoliSelezionati.Remove(key);
                }
            }

            this.GrdFascResult.SelectedIndex = -1;
            this.RowSelected = this.GrdFascResult.SelectedRow;

            this.UpPnlGridResult.Update();

        }



        public void BindGrid(DocsPaWR.SearchObject[] listaFasc)
        {
            DocsPaWR.Fascicolo currentFasc;
            m_hashTableFascicoli = new Hashtable(listaFasc.Length);
            if (listaFasc != null && listaFasc.Length > 0)
            {
                //Costruisco il datagrid
                ArrayList Dg_elem = new ArrayList();

                for (int i = 0; i < listaFasc.Length; i++)
                {
                    //la chiave della HashTable è la systemId del fascicolo
                    if (!m_hashTableFascicoli.ContainsKey(listaFasc[i].SearchObjectID))
                    {
                        m_hashTableFascicoli.Add(listaFasc[i].SearchObjectID, listaFasc[i]);

                        currentFasc = new Fascicolo();

                        currentFasc = this.GetProjectFormSearchObject(listaFasc[i]);

                        //((DocsPaWR.Fascicolo)listaFasc[i]);

                        //string dtaApertura = "";
                        //string dtaChiusura = "";

                        //if (currentFasc.apertura != null && currentFasc.apertura.Length > 0)
                        //    dtaApertura = currentFasc.apertura.Substring(0, 10);

                        //if (currentFasc.chiusura != null && currentFasc.chiusura.Length > 0)
                        //    dtaChiusura = currentFasc.chiusura.Substring(0, 10);

                        ////dati registro associato al nodo di titolario
                        //string idRegistro = currentFasc.idRegistroNodoTit; // systemId del registro
                        //string codiceRegistro = currentFasc.codiceRegistroNodoTit; //codice del Registro
                        //
                        //if (idRegistro != null && idRegistro == String.Empty)//se il fascicolo è associato a un TITOLARIO con idREGISTRO = NULL
                        //    codiceRegistro = "TUTTI";

                        string dtaApertura = currentFasc.apertura;
                        string dtaChiusura = currentFasc.chiusura;
                        string tipoFasc = string.Empty;
                        string descFasc = currentFasc.descrizione;
                        string stato = string.Empty;
                        string chiave = listaFasc[i].SearchObjectID;
                        string codFasc = currentFasc.codice;
                        string codiceRegistro = string.Empty;

                        Dg_elem.Add(new RicercaFascicoliPerFascicolazioneRapida(tipoFasc, codFasc, descFasc, dtaApertura, dtaChiusura, chiave, stato, codiceRegistro));
                    }

                }

                if (Dg_elem.Count < GrdFascResult.PageSize)
                {
                    int i = Dg_elem.Count;
                    while (i < GrdFascResult.PageSize)
                    {
                        Dg_elem.Add(new RicercaFascicoliPerFascicolazioneRapida());
                        i++;
                    }
                }

                this.GrdFascResult.SelectedIndex = -1;
                this.GrdFascResult.DataSource = Dg_elem;
                HttpContext.Current.Session["SearchProject.listaFascicoliFound"] = Dg_elem;
                this.GrdFascResult.DataBind();

                for (int i = 0; i < this.GrdFascResult.Rows.Count; i++)
                {
                    if (this.CallerSearchProject != null && (this.CallerSearchProject == "classifica" || this.CallerSearchProject == "searchInstance"))
                    {
                        GrdFascResult.Rows[i].Cells[2].Visible = true;
                        GrdFascResult.Rows[i].Cells[1].Visible = false;
                        GrdFascResult.Rows[i].Cells[0].Visible = false;
                    }
                    else
                    {
                        GrdFascResult.Rows[i].Cells[1].Visible = true;
                        GrdFascResult.Rows[i].Cells[2].Visible = false;
                        GrdFascResult.Rows[i].Cells[0].Visible = false;
                    }
                }

                if (this.CallerSearchProject != null && (this.CallerSearchProject == "classifica" || this.CallerSearchProject == "searchInstance"))
                {
                    GrdFascResult.Columns[2].Visible = true;
                    GrdFascResult.Columns[0].Visible = false;
                }
                else
                {
                    GrdFascResult.Columns[1].Visible = true;
                    GrdFascResult.Columns[0].Visible = false;
                }

                UpPnlGridResult.Update();
            }
        }

        private Fascicolo GetProjectFormSearchObject(SearchObject obj)
        {
            Fascicolo prj = new Fascicolo();
            prj.apertura = obj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("P5")).FirstOrDefault().SearchObjectFieldValue;
            prj.chiusura = obj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("P6")).FirstOrDefault().SearchObjectFieldValue;
            prj.descrizione = obj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("P4")).FirstOrDefault().SearchObjectFieldValue;
            prj.codice = obj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("P3")).FirstOrDefault().SearchObjectFieldValue;
            prj.systemID = obj.SearchObjectID;
            return prj;

        }

        protected void changPageGrdFound_click(object sender, GridViewPageEventArgs e)
        {
            this.rowIndex.Value = string.Empty;
            this.RowSelected = null;
            ArrayList listaFascicoli = (ArrayList)HttpContext.Current.Session["SearchProject.listaFascicoliFound"];
            this.GrdFascResult.PageIndex = e.NewPageIndex;
            this.GrdFascResult.DataSource = listaFascicoli;
            this.GrdFascResult.DataBind();
            this.UpPnlGridResult.Update();

            this.PlcStructur.Visible = false;
            this.upnlStruttura.Update();
        }


        private void DataGrid_ItemDataBound(object sender, GridViewRowEventArgs e)
        {
        }

        public string GetLabel(string id)
        {
            string language = UIManager.UserManager.GetUserLanguage();
            return Utils.Languages.GetLabelFromCode(id, language);
        }


        private bool verificaSelezione(string caller, out string key, out bool grid)
        {
            grid = false;
            bool verificaSelezione = false;
            key = "";
            switch (caller)
            {
                case "profilo":
                case "protocollo":
                case "search":
                case "custom":
                    foreach (GridViewRow dgItem in this.GrdFascResult.Rows)
                    {
                        RadioButton optFasc = dgItem.FindControl("rbSel") as RadioButton;
                        if ((optFasc != null) && optFasc.Checked == true)
                        {
                            key = ((Label)dgItem.FindControl("lblFascKey")).Text;
                            verificaSelezione = true;
                            grid = true;
                            break;
                        }
                    }

                    if (!verificaSelezione)
                    {
                        string folderId = this.treenode_sel.Value.Replace("node_", "").Replace("root_", "");
                        if (!string.IsNullOrEmpty(folderId))
                        {
                            key = folderId;
                            verificaSelezione = true;
                            grid = false;
                        }
                    }
                    break;
                case "classifica":
                case "searchInstance":
                    foreach (GridViewRow dgItem in this.GrdFascResult.Rows)
                    {

                        //modificare con controllo nell'hash
                        CheckBox optFasc = dgItem.FindControl("cbSel") as CheckBox;
                        if ((optFasc != null) && optFasc.Checked == true)
                        {
                            key += ((Label)dgItem.FindControl("lblFascKey")).Text + ";";
                            verificaSelezione = true;
                            grid = true;
                            //break;
                        }
                    }

                    if (key.Length > 0)
                        key.Substring(0, key.Length - 1);

                    if (!verificaSelezione)
                    {
                        string folderId = this.treenode_sel.Value.Replace("node_", "").Replace("root_", "");
                        if (!string.IsNullOrEmpty(folderId))
                        {
                            key = folderId;
                            verificaSelezione = true;
                            grid = false;
                        }
                    }
                    break;
            }
            return verificaSelezione;
        }


        private void closePage(string _ParametroDiRitorno)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "closeMask", "if (parent.fra_main) {parent.fra_main.closeAjaxModal('SearchProject','" + _ParametroDiRitorno + "');} else {parent.closeAjaxModal('SearchProject', '" + _ParametroDiRitorno + "');};", true);
            Response.End();
        }


        protected void GrdFascResult_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void GrdFascResult_RowCommand(object sender, GridViewCommandEventArgs e)
        {

        }

        protected void GrdFascResult_RowDataBound(object sender, GridViewRowEventArgs e)
        {

        }



        protected void SetAjaxDescriptionProject()
        {
            string dataUser = UIManager.RoleManager.GetRoleInSession().idGruppo;
            if (this.Registry == null)
            {
                this.Registry = UIManager.RoleManager.GetRoleInSession().registri[0];
            }
            dataUser = dataUser + "-" + this.Registry.systemId;
            if (UIManager.ClassificationSchemeManager.getTitolarioAttivo(UIManager.UserManager.GetInfoUser().idAmministrazione) != null)
            {
                RapidSenderDescriptionProject.ContextKey = dataUser + "-" + UIManager.UserManager.GetUserInSession().idAmministrazione + "-" + UIManager.ClassificationSchemeManager.getTitolarioAttivo(UIManager.UserManager.GetInfoUser().idAmministrazione).ID + "-" + UIManager.UserManager.GetUserInSession().idPeople + "-" + UIManager.UserManager.GetUserInSession().systemId;
            }
        }


        protected void SetAjaxAddressBook()
        {
            this.RapidSenderDescriptionProprietario.MinimumPrefixLength = this.AjaxAddressBookMinPrefixLenght;

            string dataUser = UIManager.RoleManager.GetRoleInSession().systemId;

            if (this.Registry == null)
            {
                this.Registry = RegistryManager.GetRegistryInSession();
            }
            dataUser = dataUser + "-" + this.Registry.systemId;

            string callType = string.Empty;

            // Mittente su protocollo in ingresso
            callType = "CALLTYPE_PROTO_IN";
            RapidSenderDescriptionProprietario.ContextKey = dataUser + "-" + UIManager.UserManager.GetUserInSession().idAmministrazione + "-" + callType;

            callType = "CALLTYPE_OWNER_AUTHOR";
            this.RapidCreatore.ContextKey = dataUser + "-" + UIManager.UserManager.GetUserInSession().idAmministrazione + "-" + callType;

            callType = "CALLTYPE_GESTFASC_LOCFISICA";
            this.RapidCollocazione.ContextKey = dataUser + "-" + UIManager.UserManager.GetUserInSession().idAmministrazione + "-" + callType;
        }


        protected void GrdFascResult_PreRender(object sender, EventArgs e)
        {
            m_hashTableFascicoli = ProjectManager.getHashFascicoli(this);
            HT_fascicoliSelezionati = ProjectManager.getHashFascicoliSelezionati(this);
            bool alternateRow = false;

            for (int i = 0; i < GrdFascResult.Rows.Count; i++)
            {
                this.GrdFascResult.Rows[i].CssClass = "NormalRow";
                if (alternateRow) this.GrdFascResult.Rows[i].CssClass = "AltRow";

                alternateRow = !alternateRow;

                this.GrdFascResult.Rows[i].Attributes.Remove("class");

                if (GrdFascResult.Rows[i].DataItemIndex >= 0)
                {
                    if (!string.IsNullOrEmpty(((Label)GrdFascResult.Rows[i].FindControl("lblFascKey")).Text))
                    {
                        string dataChiusura = ((Literal)GrdFascResult.Rows[i].FindControl("LitFascDataClose")).Text;
                        RadioButton rb = (RadioButton)GrdFascResult.Rows[i].FindControl("rbSel") as RadioButton;
                        CheckBox cb = (CheckBox)GrdFascResult.Rows[i].FindControl("cbSel") as CheckBox;
                        string key = ((Label)GrdFascResult.Rows[i].FindControl("lblFascKey")).Text;

                        if (!String.IsNullOrEmpty(dataChiusura))
                        {
                            cb.Enabled = false;
                            rb.Enabled = false;
                        }
                        else
                        {
                            if (HT_fascicoliSelezionati != null && HT_fascicoliSelezionati.ContainsKey(key))
                            {
                                cb.Checked = true;
                                rb.Checked = true;
                            }
                            else
                            {
                                cb.Checked = false;
                                rb.Checked = false;
                            }
                        }

                        if (this.CallerSearchProject == null || this.CallerSearchProject != "searchInstance")
                        {
                            string jsOnClick = "disallowOp('Content2'); $('#rowIndex').val('" + GrdFascResult.Rows[i].RowIndex.ToString() + "'); $('#btnDetails').click();";
                            GrdFascResult.Rows[i].Cells[3].Attributes["onclick"] = jsOnClick;
                            GrdFascResult.Rows[i].Cells[4].Attributes["onclick"] = jsOnClick;
                            GrdFascResult.Rows[i].Cells[5].Attributes["onclick"] = jsOnClick;
                        }
                    }
                    else
                    {
                        RadioButton rb = (RadioButton)GrdFascResult.Rows[i].FindControl("rbSel") as RadioButton;
                        CheckBox cb = (CheckBox)GrdFascResult.Rows[i].FindControl("cbSel") as CheckBox;
                        rb.Visible = false;
                        cb.Visible = false;
                    }
                }

                if (this.RowSelected != null)
                {
                    if (!string.IsNullOrEmpty(this.rowIndex.Value))
                    {
                        if (this.GrdFascResult.Rows[i].RowIndex == Int32.Parse(this.rowIndex.Value))
                        {
                            this.GrdFascResult.Rows[i].Attributes.Remove("class");
                            this.GrdFascResult.Rows[i].CssClass = "selectedrow";
                        }
                    }
                }
            }

            if (this.CallerSearchProject != null && (this.CallerSearchProject == "classifica" || this.CallerSearchProject == "searchInstance"))
            {
                CheckBox chkBox = new CheckBox();
                chkBox.ID = "cb_selectall";
                chkBox.Attributes["onclick"] = "$('#BtnHiddenSelectAll').click();";
                if (HT_fascicoliSelezionati != null && m_hashTableFascicoli.Count == HT_fascicoliSelezionati.Count)
                {
                    chkBox.Checked = true;
                    Session["selectAll"] = true;
                }
                else
                {
                    chkBox.Checked = false;
                    Session["selectAll"] = null;
                }
                GrdFascResult.HeaderRow.Cells[2].Controls.Clear();
                GrdFascResult.HeaderRow.Cells[2].Controls.Add(chkBox);
            }
        }

        protected void BtnHiddenSelectAll_Click(object sender, EventArgs e)
        {
            this.BtnCollate.Enabled = true;
            if (Session["selectAll"] == null)
            {
                m_hashTableFascicoli = ProjectManager.getHashFascicoli(this);
                ProjectManager.setHashFascicoliSelezionati(this, m_hashTableFascicoli);
            }
            else
            {
                ProjectManager.setHashFascicoliSelezionati(this, null);
            }
            this.GrdFascResult_PreRender(null, null);
            this.UpPnlGridResult.Update();
        }

        protected void GridDocuments_Details(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);

            if (!string.IsNullOrEmpty(this.rowIndex.Value))
            {
                int selRow = int.Parse(this.rowIndex.Value);
                if (selRow == this.GrdFascResult.SelectedIndex) selRow = -1;

                if (selRow != -1)
                {
                    this.GrdFascResult.SelectedIndex = selRow;
                    this.RowSelected = this.GrdFascResult.SelectedRow;
                    string idfascicolo = ((Label)RowSelected.FindControl("lblFascKey")).Text;
                    Fascicolo fascicolo = UIManager.ProjectManager.getFascicoloById(idfascicolo, UIManager.UserManager.GetInfoUser());

                    // Filippo: modificato l'uso della variabile in sessione
                    // con una usata solo all'interno della pagina searchProject.aspx
                    // per evitare malfunzionamenti in altre pagine collegate
                    // ad es. la fascicolazione massiva dei documenti in progetto
                    //UIManager.ProjectManager.setProjectInSession(fascicolo);
                    this.Project = fascicolo;
                    for (int i = 0; i < GrdFascResult.Rows.Count; i++)
                    {
                        if (GrdFascResult.Rows[i].DataItemIndex >= 0)
                        {
                            RadioButton rb = (RadioButton)GrdFascResult.Rows[i].FindControl("rbSel") as RadioButton;
                            CheckBox cb = (CheckBox)GrdFascResult.Rows[i].FindControl("cbSel") as CheckBox;
                            rb.Checked = false;
                            cb.Checked = false;
                            BtnCollate.Enabled = false;
                            if (HT_fascicoliSelezionati != null && HT_fascicoliSelezionati.Count > 0)
                            {
                                string idTemp = ((Label)GrdFascResult.Rows[i].FindControl("lblFascKey")).Text;
                                if (HT_fascicoliSelezionati.ContainsKey(idTemp))
                                {
                                    HT_fascicoliSelezionati.Remove(idTemp);
                                }
                            }
                        }
                    }

                    this.UpPnlGridResult.Update();


                    //this.docsInFolderCount.Text = "0";
                    //Ricerca del numero di documenti presenti nel fascicolo. Il metodo attuale fa ritornare anche i documenti. A me serve soltanto il totale.
                    //InfoFascicolo fasc = ProjectManager.getInfoFascicoloDaFascicolo(fascicolo);  
                    Folder fold = ProjectManager.getFolder(this, fascicolo);
                    int numDoc = ProjectManager.getCountDocumentiInFolder(fold);

                    //this.docsInFolderCount.Text = numDoc.ToString();
                    if (!string.IsNullOrEmpty(fascicolo.chiusura) || fascicolo.HasStrutturaTemplate)
                    {
                        ImgFolderAdd.Enabled = false;
                        ImgFolderModify.Enabled = false;
                        ImgFolderRemove.Enabled = false;
                    }
                    else
                    {
                        ImgFolderAdd.Enabled = true;
                        ImgFolderModify.Enabled = true;
                        ImgFolderRemove.Enabled = true;
                    }

                    upPnlStruttura.Visible = true;
                    upnlStruttura.Visible = true;
                    upnlStruttura.Update();
                    PlcStructur.Visible = true;
                    upPnlStruttura.Update();
                }
            }
        }

        protected void BtnChangeSelectedFolder_Click(object sender, EventArgs e)
        {
            string codice = string.Empty;
            string descrizione = string.Empty;
            bool isRootFolder = false;
            string returnValue = "";
            Fascicolo project = this.Project;
            string folderId = this.treenode_sel.Value.Replace("node_", "").Replace("root_", "");
            if (project.stato != "C")
            {

                if (!string.IsNullOrEmpty(folderId))
                {
                    if (this.CallerSearchProject != null && (this.CallerSearchProject == "profilo" || this.CallerSearchProject == "protocollo" || this.CallerSearchProject == "search"))
                    {
                        Folder folder = ProjectManager.getFolder(this, folderId);
                        folder = ProjectManager.getFolder(this, folder);


                        project.folderSelezionato = folder;

                        // Filippo: modificato l'uso della variabile in sessione
                        // con una usata solo all'interno della pagina searchProject.aspx
                        // per evitare malfunzionamenti in altre pagine collegate
                        // ad es. la fascicolazione massiva dei documenti in progetto
                        //UIManager.ProjectManager.setProjectInSession(project);
                        this.Project = project;

                        if (project.codice == folder.descrizione)
                            returnValue = project.codice + "#" + project.descrizione;
                        else
                        {
                            this.CalcolaFascicolazioneRapida(folder, ref codice, ref descrizione, ref isRootFolder, project.codice);
                            returnValue = project.codice + "//" + codice.Substring(0, codice.Length - 2);
                        }
                        HttpContext.Current.Session["ReturnValuePopup"] = returnValue;
                    }


                    if (this.CallerSearchProject != null && (this.CallerSearchProject == "classifica" || this.CallerSearchProject == "searchInstance"))
                    {
                        if (HT_fascicoliSelezionati == null)
                        {
                            HT_fascicoliSelezionati = new Hashtable(1);
                        }

                        m_hashTableFascicoli = ProjectManager.getHashFascicoli(this);

                        if (!HT_fascicoliSelezionati.ContainsKey(folderId))
                        {
                            HT_fascicoliSelezionati.Add(folderId, m_hashTableFascicoli[folderId]);
                        }
                    }

                    //this.docsInFolderCount.Text = "0";
                    //Ricerca del numero di documenti presenti nel fascicolo. Il metodo attuale fa ritornare anche i documenti. A me serve soltanto il totale.
                    //InfoFascicolo fasc = ProjectManager.getInfoFascicoloDaFascicolo(fascicolo);  
                    Folder fold = ProjectManager.getFolder(folderId, UserManager.GetInfoUser());
                    int numDoc = ProjectManager.getCountDocumentiInFolder(fold);

                    //this.docsInFolderCount.Text = numDoc.ToString();
                    upPnlStruttura.Update();
                    ProjectManager.setHashFascicoliSelezionati(this, HT_fascicoliSelezionati);
                    BtnCollate.Enabled = true;
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(folderId))
                {
                    //this.docsInFolderCount.Text = "0";
                    //Ricerca del numero di documenti presenti nel fascicolo. Il metodo attuale fa ritornare anche i documenti. A me serve soltanto il totale.
                    //InfoFascicolo fasc = ProjectManager.getInfoFascicoloDaFascicolo(fascicolo);  
                    Folder fold = ProjectManager.getFolder(folderId, UserManager.GetInfoUser());
                    int numDoc = ProjectManager.getCountDocumentiInFolder(fold);

                    //this.docsInFolderCount.Text = numDoc.ToString();
                    upPnlStruttura.Update();
                }
            }
        }


        private void CalcolaFascicolazioneRapida(Folder folder, ref string codice, ref string descrizione, ref bool isRootFolder, string codFascicolo)
        {
            Folder parent = null;

            if (folder.descrizione.Equals(codFascicolo))
                isRootFolder = true;
            else
            {
                string valorechiave = InitConfigurationKeys.GetValue("0", "FE_PROJECT_LEVEL");
                int pos = folder.descrizione.IndexOf(" - ");
                
                if (!string.IsNullOrEmpty(valorechiave) && valorechiave.Equals("1") && pos > 0)
                {
                    codice = folder.descrizione.Substring(pos + 3, folder.descrizione.Length - pos - 3) + "//" + codice;
                    descrizione = folder.descrizione.Substring(pos + 3, folder.descrizione.Length - pos - 3) + "//" + descrizione;
                    parent = ProjectManager.getFolder(this, folder.idParent);
                    parent = ProjectManager.getFolder(this, parent);
                }
                else
                {
                    codice = folder.descrizione + "//" + codice;
                    descrizione = folder.descrizione + "//" + descrizione;
                    parent = ProjectManager.getFolder(this, folder.idParent);
                    try
                    {
                        parent = ProjectManager.getFolder(this, parent);
                    }
                    catch
                    {
                    }
                }
            }
            if (!isRootFolder)
                CalcolaFascicolazioneRapida(parent, ref codice, ref descrizione, ref isRootFolder, codFascicolo);
        }


        //public static void ClearSessionData(Page page)
        //{

        //    //ProjectManager.removeFiltroRicFascNew(page);

        //    ////ProjectManager.DO_RemoveFlagLF();
        //    ////ProjectManager.DO_RemoveLocazioneFisica();
        //    //RemoveListaFascicoli(page);
        //    ProjectManager.removeHashFascicoli(page);
        //}

        public static void ClearDatiRicerca(Page page)
        {

            //ProjectManager.removeFiltroRicFascNew(page);

            //RemoveListaFascicoli(page);
            ProjectManager.removeHashFascicoli(page);
        }




        //private void setDescCorrispondente(string codiceRubrica, bool fineValidita)
        //{
        //    string msg = "Codice rubrica non esistente";
        //    Corrispondente corr = null;
        //    try
        //    {
        //        corr = UserManager.GetCorrispondenteInterno(this, codiceRubrica, fineValidita);

        //        if ((corr != null && corr.descrizione != "") && corr.GetType().Equals(typeof(UnitaOrganizzativa)))
        //        {
        //            txtDescrizioneCreatore.Text = corr.descrizione;
        //            this.txtSystemIdCreatore.Value = corr.systemId;
        //            //FascicoliManager.DO_SetIdUO_LF(corr.systemId);
        //            //DocsPaVO.LocazioneFisica.LocazioneFisica LF =
        //            //    new DocsPaVO.LocazioneFisica.LocazioneFisica(corr.systemId, corr.descrizione,
        //            //    this.GetCalendarControl("txt_dta_LF_DA").txt_Data.Text, codiceRubrica);
        //            //FascicoliManager.DO_SetLocazioneFisica(LF);
        //        }
        //        else
        //        {
        //            //RegisterStartupScript("alert", "<script language='javascript'>alert('" + msg + "');</script>");
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorManager.redirect(this, ex);
        //    }
        //}

        protected void TxtCode_OnTextChanged(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);
            CustomTextArea caller = sender as CustomTextArea;

            String corrCode = this.txtCodiceProprietario.Text;
            DocsPaWR.Corrispondente corr = null;


            if (!String.IsNullOrEmpty(corrCode))
            {
                RubricaCallType calltype = GetCallType(caller.ID);
                // Reperimento oggetto corrispondente dal codice immesso dall'utente
                corr = UIManager.AddressBookManager.getCorrispondenteRubrica(corrCode, calltype);
                // corr = this.GetCorrispondenteDaCodice(corrCode);

                if (corr == null)
                {
                    this.Page.ClientScript.RegisterStartupScript(
                        this.GetType(),
                        "CodiceRubricaNonTrovato",
                        "alert('Codice rubrica non trovato');",
                        true);
                    this.SelectedCorr = null;
                }
                else
                {
                    this.SelectedCorr = corr;

                    if (corr.GetType().Equals(typeof(DocsPaWR.Utente)))
                        this.rblOwnerType.SelectedValue = "P";
                    else if (corr.GetType().Equals(typeof(DocsPaWR.Ruolo)))
                        this.rblOwnerType.SelectedValue = "R";
                    else if (corr.GetType().Equals(typeof(DocsPaWR.UnitaOrganizzativa)))
                        this.rblOwnerType.SelectedValue = "U";

                    //this.UpPnlProject.Update();
                }
                this.txtCodiceProprietario.Focus();

                this.FillDatiCorrispondenteDaRubrica();
            }
            else
            {
                this.txtCodiceProprietario.Text = "";
                this.txtDescrizioneProprietario.Text = "";

                SelectedCorr = null;

            }
        }

        private DocsPaWR.Corrispondente GetCorrispondenteDaCodice(String corrCode)
        {
            DocsPaWR.Corrispondente retValue = null;

            if (!String.IsNullOrEmpty(corrCode))
                retValue = AddressBookManager.getCorrispondenteByCodRubrica(corrCode, true);

            return retValue;
        }

        private void FillDatiCorrispondenteDaRubrica()
        {
            DocsPaWR.Corrispondente selectedCorr = this.SelectedCorr;
            if (selectedCorr != null)
            {
                this.txtCodiceProprietario.Text = selectedCorr.codiceRubrica;
                this.txtDescrizioneProprietario.Text = selectedCorr.descrizione;
                this.idProprietario.Value = selectedCorr.systemId;


                switch (selectedCorr.tipoCorrispondente)
                {
                    case "P":
                        this.rblOwnerType.SelectedValue = "P";
                        break;
                    case "R":
                        this.rblOwnerType.SelectedValue = "R";
                        break;
                    case "U":
                        this.rblOwnerType.SelectedValue = "U";
                        break;
                    default:
                        this.rblOwnerType.SelectedValue = "U";
                        break;
                }
                selectedCorr = null;
            }
            else
            {
                this.txtCodiceProprietario.Text = string.Empty;
                this.txtDescrizioneProprietario.Text = string.Empty;
                this.UpdatePanel1.Update();
            }
            this.UpdatePanel1.Update();

        }

        protected void DocumentImgSenderAddressBook_Click(object sender, EventArgs e)
        {

            this.CallType = RubricaCallType.CALLTYPE_RICERCA_CREATOR;

            HttpContext.Current.Session["AddressBook.from"] = "SP_CREATOR";// + this.RblTypeProtocol.SelectedValue + "_S_S";

            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "UpdatePanel1", "ajaxModalPopupAddressBook();", true);
        }


        protected void btnAddressBookPostback_Click(object sender, EventArgs e)
        {
            List<NttDataWA.Popup.AddressBook.CorrespondentDetail> atList = (List<NttDataWA.Popup.AddressBook.CorrespondentDetail>)HttpContext.Current.Session["AddressBook.At"];
            string addressBookCallFrom = HttpContext.Current.Session["AddressBook.from"].ToString();
            if (atList != null && atList.Count > 0)
            {
                NttDataWA.Popup.AddressBook.CorrespondentDetail corrInSess = atList[0];
                if (atList != null && atList.Count > 0)
                {
                    if (addressBookCallFrom == "COLLOCATION")
                    {
                        this.txtCodiceCollocazione.Text = corrInSess.CodiceRubrica;
                        this.txtDescrizioneCollocazione.Text = corrInSess.Descrizione;
                        this.idCollocationAddr.Value = corrInSess.SystemID;
                        this.upPnlCollocationAddr.Update();
                    }
                    else if (addressBookCallFrom == "F_X_X_S")
                    {
                        this.txtCodiceCreatore.Text = corrInSess.CodiceRubrica;
                        this.txtDescrizioneCreatore.Text = corrInSess.Descrizione;
                        this.idCreatore.Value = corrInSess.SystemID;
                        this.upPnlCreatore.Update();
                    }
                    else if (addressBookCallFrom == "SP_CREATOR")
                    {

                        if (corrInSess != null)
                        {
                            txtCodiceProprietario.Text = corrInSess.CodiceRubrica;
                            TxtCode_OnTextChanged(txtCodiceProprietario, new EventArgs());
                        }
                    }
                    //Laura 12 Aprile
                    else if (addressBookCallFrom == "CUSTOM")
                    {

                        if (HttpContext.Current.Session["idCustomObjectCustomCorrespondent"] != null)
                        {
                            if (HttpContext.Current.Session["idCustomObjectCustomCorrespondent"].ToString() != "")
                            {
                                UserControls.CorrespondentCustom userCorr = (UserControls.CorrespondentCustom)this.PnlTypeDocument.FindControl(HttpContext.Current.Session["idCustomObjectCustomCorrespondent"].ToString());
                                userCorr.TxtCodeCorrespondentCustom = corrInSess.CodiceRubrica;
                                userCorr.TxtDescriptionCorrespondentCustom = corrInSess.Descrizione.Replace(START_SPECIAL_STYLE, "").Replace(END_SPECIAL_STYLE, "");
                                userCorr.IdCorrespondentCustom = corrInSess.SystemID;
                            }
                        }
                        this.UpPnlTypeDocument.Update();
                    }
                }
            }
            HttpContext.Current.Session["AddressBook.At"] = null;
            HttpContext.Current.Session["AddressBook.Cc"] = null;
        }


        protected void btnAddressBookPostback2_Click(object sender, EventArgs e)
        {
            List<NttDataWA.Popup.AddressBook.CorrespondentDetail> atList = (List<NttDataWA.Popup.AddressBook.CorrespondentDetail>)HttpContext.Current.Session["AddressBook.At"];
            string addressBookCallFrom = HttpContext.Current.Session["AddressBook.from"].ToString();


            if (atList != null && atList.Count > 0)
            {
                NttDataWA.Popup.AddressBook.CorrespondentDetail corrInSess = atList[0];
                //gestione collocazione
                if (addressBookCallFrom == "COLLOCATION")
                {
                    this.txtCodiceCollocazione.Text = corrInSess.CodiceRubrica;
                    this.txtDescrizioneCollocazione.Text = corrInSess.Descrizione;
                    this.idCollocationAddr.Value = corrInSess.SystemID;
                    this.upPnlCollocationAddr.Update();
                }
                else
                {
                    this.txtCodiceCreatore.Text = corrInSess.CodiceRubrica;
                    this.txtDescrizioneCreatore.Text = corrInSess.Descrizione;
                    this.idCreatore.Value = corrInSess.SystemID;
                    this.upPnlCreatore.Update();
                }
            }

            HttpContext.Current.Session["AddressBook.At"] = null;
            HttpContext.Current.Session["AddressBook.Cc"] = null;
        }


        protected void btnclassificationschema_Click(object sender, ImageClickEventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "OpenTitolario", "ajaxModalPopupOpenTitolario();", true);
        }

        protected void DocumentImgCollocationAddressBook_Click(object sender, ImageClickEventArgs e)
        {
            this.CallType = RubricaCallType.CALLTYPE_EDITFASC_LOCFISICA;


            HttpContext.Current.Session["AddressBook.from"] = "COLLOCATION";

            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "upPnlCollocationAddr", "ajaxModalPopupAddressBook();", true);
        }

        protected void ddl_dtaCollocation_SelectedIndexChanged(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);

            string language = UIManager.UserManager.GetUserLanguage();
            switch (this.ddl_dtaCollocation.SelectedIndex)
            {
                case 0: //Valore singolo
                    this.dtaCollocation_TxtFrom.ReadOnly = false;
                    this.dtaCollocation_TxtTo.Visible = false;
                    this.lbl_dtaCollocationTo.Visible = false;
                    this.lbl_dtaCollocationFrom.Text = Utils.Languages.GetLabelFromCode("VisibilityOneField", language);
                    break;
                case 1: //Intervallo
                    this.dtaCollocation_TxtFrom.ReadOnly = false;
                    this.dtaCollocation_TxtTo.ReadOnly = false;
                    this.lbl_dtaCollocationTo.Visible = true;
                    this.lbl_dtaCollocationFrom.Visible = true;
                    this.dtaCollocation_TxtTo.Visible = true;
                    this.lbl_dtaCollocationFrom.Text = Utils.Languages.GetLabelFromCode("VisibilityFrom", language);
                    this.lbl_dtaCollocationTo.Text = Utils.Languages.GetLabelFromCode("VisibilityTo", language);
                    break;
                case 2: //Oggi
                    this.lbl_dtaCollocationTo.Visible = false;
                    this.dtaCollocation_TxtTo.Visible = false;
                    this.dtaCollocation_TxtFrom.ReadOnly = true;
                    this.dtaCollocation_TxtFrom.Text = NttDataWA.Utils.dateformat.toDay();
                    break;
                case 3: //Settimana corrente
                    this.lbl_dtaCollocationTo.Visible = true;
                    this.dtaCollocation_TxtTo.Visible = true;
                    this.dtaCollocation_TxtFrom.Text = NttDataWA.Utils.dateformat.getFirstDayOfWeek();
                    this.dtaCollocation_TxtTo.Text = NttDataWA.Utils.dateformat.getLastDayOfWeek();
                    this.dtaCollocation_TxtTo.ReadOnly = true;
                    this.dtaCollocation_TxtFrom.ReadOnly = true;
                    this.lbl_dtaCollocationFrom.Text = Utils.Languages.GetLabelFromCode("VisibilityFrom", language);
                    this.lbl_dtaCollocationTo.Text = Utils.Languages.GetLabelFromCode("VisibilityTo", language);
                    break;
                case 4: //Mese corrente
                    this.lbl_dtaCollocationTo.Visible = true;
                    this.dtaCollocation_TxtTo.Visible = true;
                    this.dtaCollocation_TxtFrom.Text = NttDataWA.Utils.dateformat.getFirstDayOfMonth();
                    this.dtaCollocation_TxtTo.Text = NttDataWA.Utils.dateformat.getLastDayOfMonth();
                    this.dtaCollocation_TxtTo.ReadOnly = true;
                    this.dtaCollocation_TxtFrom.ReadOnly = true;
                    this.lbl_dtaCollocationFrom.Text = Utils.Languages.GetLabelFromCode("VisibilityFrom", language);
                    this.lbl_dtaCollocationTo.Text = Utils.Languages.GetLabelFromCode("VisibilityTo", language);
                    break;
            }

            //this.upPnlCollocation.Update();
        }




        protected void TxtCodeColl_OnTextChanged(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);

            CustomTextArea caller = sender as CustomTextArea;
            string codeAddressBook = caller.Text;

            if (!string.IsNullOrEmpty(codeAddressBook))
            {
                this.SearchCorrespondent(codeAddressBook, caller.ID);
            }
            else
            {
                switch (caller.ID)
                {
                    case "txtCodiceCreatore":
                        this.txtCodiceCreatore.Text = string.Empty;
                        this.txtDescrizioneCreatore.Text = string.Empty;
                        this.idCreatore.Value = string.Empty;
                        this.upPnlCreatore.Update();
                        break;
                    case "txtCodiceCollocazione":
                        this.txtCodiceCollocazione.Text = string.Empty;
                        this.txtDescrizioneCollocazione.Text = string.Empty;
                        this.idCollocationAddr.Value = string.Empty;
                        this.upPnlCollocationAddr.Update();
                        break;
                }
            }
        }

        protected void SearchCorrespondent(string addressCode, string idControl)
        {
            DocsPaWR.Corrispondente corr = UIManager.AddressBookManager.getCorrispondenteRubrica(addressCode, this.CallType);
            if (corr == null)
            {
                switch (idControl)
                {
                    case "txtCodiceCreatore":
                        this.txtCodiceCreatore.Text = string.Empty;
                        this.txtDescrizioneCreatore.Text = string.Empty;
                        this.idCreatore.Value = string.Empty;
                        this.upPnlCreatore.Update();
                        break;
                    case "txtCodiceCollocazione":
                        this.txtCodiceCollocazione.Text = string.Empty;
                        this.txtDescrizioneCollocazione.Text = string.Empty;
                        this.idCollocationAddr.Value = string.Empty;
                        this.upPnlCollocationAddr.Update();
                        break;
                }

                string msg = "ErrorTransmissionCorrespondentNotFound";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
            }
            else
            {
                switch (idControl)
                {
                    case "txtCodiceCreatore":
                        this.txtCodiceCreatore.Text = corr.codiceRubrica;
                        this.txtDescrizioneCreatore.Text = corr.descrizione;
                        this.idCreatore.Value = corr.systemId;
                        this.rblOwnerTypeCreator.SelectedIndex = -1;
                        this.rblOwnerTypeCreator.Items.FindByValue(corr.tipoCorrispondente).Selected = true;
                        this.upPnlCreatore.Update();
                        break;
                    case "txtCodiceCollocazione":
                        this.txtCodiceCollocazione.Text = corr.codiceRubrica;
                        this.txtDescrizioneCollocazione.Text = corr.descrizione;
                        this.idCollocationAddr.Value = corr.systemId;
                        this.upPnlCollocationAddr.Update();
                        break;
                }
            }
        }

        protected void chkConservation_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox check = (CheckBox)sender;
            if (check.Checked)
            {
                if (check.ID == "chkConservation")
                {
                    this.chkConservationNo.Checked = false;
                }
                else
                {
                    this.chkConservation.Checked = false;
                }

                this.UpPnlConservation.Update();
            }
        }

        protected void rbViewAllYes_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton radio = (RadioButton)sender;
            if (radio.ID == "rbViewAllYes")
            {
                this.rbViewAllNo.Checked = false;
            }
            else
            {
                this.rbViewAllYes.Checked = false;
            }

            this.UpPnlViewAll.Update();
        }

        //protected void dtaExpire_TxtFrom_TextChanged(object sender, EventArgs e)
        //{

        //}



        protected void DocumentDdlTypeDocument_SelectedIndexChanged(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);
            if (!string.IsNullOrEmpty(this.DocumentDdlTypeDocument.SelectedValue))
            {
                if (this.CustomDocuments)
                {
                    this.Template = ProfilerProjectManager.getTemplateFascById(this.DocumentDdlTypeDocument.SelectedItem.Value);
                    if (this.Template != null)
                    {
                        if (this.EnableStateDiagram)
                        {
                            this.DocumentDdlStateDiagram.ClearSelection();

                            //Verifico se esiste un diagramma di stato associato al tipo di documento
                            //Modifica per la visualizzazione solo degli stati per cui esistono documenti in essi
                            //DiagrammaStato dia = DiagrammiManager.getDgByIdTipoFasc(this.DocumentDdlTypeDocument.SelectedValue, UserManager.GetInfoUser().idAmministrazione);
                            //string idDiagramma = dia.SYSTEM_ID.ToString();
                            string idDiagramma = DiagrammiManager.getDiagrammaAssociatoFasc(this.DocumentDdlTypeDocument.SelectedValue).ToString();
                            if (!string.IsNullOrEmpty(idDiagramma) && !idDiagramma.Equals("0"))
                            {
                                this.PnlStateDiagram.Visible = true;

                                //Inizializzazione comboBox
                                this.DocumentDdlStateDiagram.Items.Clear();
                                ListItem itemEmpty = new ListItem();
                                this.DocumentDdlStateDiagram.Items.Add(itemEmpty);

                                DocsPaWR.Stato[] statiDg = DiagrammiManager.getStatiPerRicerca(idDiagramma, "F");
                                foreach (Stato st in statiDg)
                                {
                                    ListItem item = new ListItem(st.DESCRIZIONE, Convert.ToString(st.SYSTEM_ID));
                                    this.DocumentDdlStateDiagram.Items.Add(item);
                                }

                                this.ddlStateCondition.Visible = true;
                                this.PnlStateDiagram.Visible = true;
                            }
                            else
                            {
                                this.ddlStateCondition.Visible = false;
                                this.PnlStateDiagram.Visible = false;
                            }
                        }
                    }
                }
            }
            else
            {
                this.Template = null;
                this.PnlTypeDocument.Controls.Clear();
                if (this.EnableStateDiagram)
                {
                    this.DocumentDdlStateDiagram.ClearSelection();
                    this.PnlStateDiagram.Visible = false;
                    this.ddlStateCondition.Visible = false;
                }
            }

            this.UpPnlTypeDocument.Update();
        }


        private void SaveTemplateProject()
        {
            int result = 0;
            for (int i = 0; i < this.Template.ELENCO_OGGETTI.Length; i++)
            {
                DocsPaWR.OggettoCustom oggettoCustom = (DocsPaWR.OggettoCustom)this.Template.ELENCO_OGGETTI[i];
                if (controllaCampi(oggettoCustom, oggettoCustom.SYSTEM_ID.ToString()))
                {
                    result++;
                }
                //if (oggettoCustom.TIPO.DESCRIZIONE_TIPO.Equals("Data"))
                //{
                //    try
                //    {
                //        UserControls.Calendar dataDa = (UserControls.Calendar)panel_Contenuto.FindControl("da_" + oggettoCustom.SYSTEM_ID.ToString());
                //        //if (dataDa.txt_Data.Text != null && dataDa.txt_Data.Text != "")
                //        if (dataDa.Text != null && dataDa.Text != "")
                //        {
                //            //DateTime dataAppoggio = Convert.ToDateTime(dataDa.txt_Data.Text);
                //            DateTime dataAppoggio = Convert.ToDateTime(dataDa.Text);
                //        }
                //        UserControls.Calendar dataA = (UserControls.Calendar)panel_Contenuto.FindControl("a_" + oggettoCustom.SYSTEM_ID.ToString());
                //        //if (dataA.txt_Data.Text != null && dataA.txt_Data.Text != "")
                //        if (dataA.Text != null && dataA.Text != "")
                //        {
                //            //DateTime dataAppoggio = Convert.ToDateTime(dataA.txt_Data.Text);
                //            DateTime dataAppoggio = Convert.ToDateTime(dataA.Text);
                //        }
                //    }
                //    catch (Exception)
                //    {
                //        Label_Avviso.Text = "Inserire valori validi per il campo data !";
                //        Label_Avviso.Visible = true;
                //        return;
                //    }
                //}
            }
            //if (result == this.Template.ELENCO_OGGETTI.Length)
            //{
            //    Label_Avviso.Text = "Inserire almeno un criterio di ricerca !";
            //    Label_Avviso.Visible = true;
            //    return;
            //}
        }


        //new code
        private bool controllaCampi(DocsPaWR.OggettoCustom oggettoCustom, string idOggetto)
        {
            //In questo metodo, oltre al controllo si salvano i valori dei campi inseriti 
            //dall'utente nel template in sessione. Solo successivamente, quanto verra' salvato 
            //il documento i suddetti valori verranno riportai nel Db vedi metodo "btn_salva_Click" della "docProfilo.aspx"

            //Label_Avviso.Visible = false;
            switch (oggettoCustom.TIPO.DESCRIZIONE_TIPO)
            {
                case "CampoDiTesto":
                    CustomTextArea textBox = (CustomTextArea)PnlTypeDocument.FindControl(idOggetto);
                    if (textBox != null)
                    {
                        if (string.IsNullOrEmpty(textBox.Text))
                        {
                            //SetFocus(textBox);
                            oggettoCustom.VALORE_DATABASE = textBox.Text;
                            return true;
                        }
                        oggettoCustom.VALORE_DATABASE = textBox.Text;
                    }
                    break;
                case "CasellaDiSelezione":
                    CheckBoxList checkBox = (CheckBoxList)PnlTypeDocument.FindControl(idOggetto);
                    if (checkBox != null)
                    {
                        if (checkBox.SelectedIndex == -1)
                        {
                            //SetFocus(checkBox);
                            for (int i = 0; i < oggettoCustom.VALORI_SELEZIONATI.Length; i++)
                                oggettoCustom.VALORI_SELEZIONATI[i] = null;

                            return true;
                        }

                        oggettoCustom.VALORI_SELEZIONATI = new string[checkBox.Items.Count];
                        oggettoCustom.VALORE_DATABASE = "";
                        for (int i = 0; i < checkBox.Items.Count; i++)
                        {
                            if (checkBox.Items[i].Selected)
                            {
                                oggettoCustom.VALORI_SELEZIONATI[i] = checkBox.Items[i].Text;
                            }
                        }
                    }
                    break;
                case "MenuATendina":
                    DropDownList dropDwonList = (DropDownList)PnlTypeDocument.FindControl(idOggetto);
                    if (dropDwonList != null)
                    {
                        if (dropDwonList.SelectedItem.Text.Equals(""))
                        {
                            //SetFocus(dropDwonList);
                            oggettoCustom.VALORE_DATABASE = "";
                            return true;
                        }
                        oggettoCustom.VALORE_DATABASE = dropDwonList.SelectedItem.Text;
                    }
                    break;
                case "SelezioneEsclusiva":
                    RadioButtonList radioButtonList = (RadioButtonList)PnlTypeDocument.FindControl(idOggetto);
                    if (radioButtonList != null)
                    {
                        if (oggettoCustom.VALORE_DATABASE == "-1" || radioButtonList.SelectedIndex == -1 || radioButtonList.SelectedValue == "-1")
                        {
                            //SetFocus(radioButtonList);
                            oggettoCustom.VALORE_DATABASE = "";
                            return true;
                        }
                        oggettoCustom.VALORE_DATABASE = radioButtonList.SelectedItem.Text;
                    }
                    break;
                case "Data":
                    UserControls.Calendar dataDa = (UserControls.Calendar)PnlTypeDocument.FindControl("da_" + oggettoCustom.SYSTEM_ID.ToString());
                    UserControls.Calendar dataA = (UserControls.Calendar)PnlTypeDocument.FindControl("a_" + oggettoCustom.SYSTEM_ID.ToString());
                    if (dataDa == null)
                    {
                        oggettoCustom.VALORE_DATABASE = "";
                        return true;
                    }
                    else
                    {
                        if (dataDa.Text.Equals("") && (dataA == null || dataA.Text.Equals("")))
                        {
                            //SetFocus(dataDa.txt_Data);
                            oggettoCustom.VALORE_DATABASE = "";
                            return true;
                        }
                        else if (dataDa.Text.Equals("") && dataA.Text != "")
                        {
                            //SetFocus(dataDa.txt_Data);
                            oggettoCustom.VALORE_DATABASE = "";
                            return true;
                        }
                        else if (dataDa.Text != "" && (dataA == null || dataA.Text == ""))
                        {
                            //oggettoCustom.VALORE_DATABASE = dataDa.txt_Data.Text;
                            oggettoCustom.VALORE_DATABASE = dataDa.Text;
                        }
                        else if (dataDa.Text != "" && dataA.Text != "")
                        {
                            //oggettoCustom.VALORE_DATABASE = dataDa.txt_Data.Text + "@" + dataA.txt_Data.Text;
                            oggettoCustom.VALORE_DATABASE = dataDa.Text + "@" + dataA.Text;
                        }
                    }
                    break;
                case "Contatore":
                    CustomTextArea contatoreDa = (CustomTextArea)PnlTypeDocument.FindControl("da_" + idOggetto);
                    CustomTextArea contatoreA = (CustomTextArea)PnlTypeDocument.FindControl("a_" + idOggetto);
                    //Controllo la valorizzazione di campi ed eventualmente notifico gli errori
                    switch (oggettoCustom.TIPO_CONTATORE)
                    {
                        case "T":
                            if (contatoreDa.Text.Equals("") && contatoreA.Text.Equals(""))
                            {
                                //SetFocus(contatoreDa);
                                oggettoCustom.VALORE_DATABASE = "";
                                return true;
                            }
                            break;
                        case "A":
                            DropDownList ddlAoo = (DropDownList)PnlTypeDocument.FindControl(oggettoCustom.SYSTEM_ID.ToString() + "_menu");
                            if (contatoreDa.Text.Equals("") && ddlAoo.SelectedValue.Equals(""))
                            {
                                //SetFocus(contatoreDa);
                                oggettoCustom.VALORE_DATABASE = "";
                                return true;
                            }

                            if (contatoreDa.Text.Equals("") && contatoreA.Text.Equals(""))
                                oggettoCustom.VALORE_DATABASE = "";
                            break;
                        case "R":
                            DropDownList ddlRf = (DropDownList)PnlTypeDocument.FindControl(oggettoCustom.SYSTEM_ID.ToString() + "_menu");
                            if (contatoreDa.Text.Equals("") && ddlRf.SelectedValue.Equals(""))
                            {
                                //SetFocus(contatoreDa);
                                oggettoCustom.VALORE_DATABASE = "";
                                return true;
                            }

                            if (contatoreDa.Text.Equals("") && contatoreA.Text.Equals(""))
                                oggettoCustom.VALORE_DATABASE = "";
                            break;
                    }

                    if (contatoreDa.Text.Equals("") && contatoreA.Text != "")
                    {
                        //SetFocus(contatoreDa);
                        oggettoCustom.VALORE_DATABASE = "";
                        return true;
                    }

                    try
                    {
                        if (contatoreDa.Text != null && contatoreDa.Text != "")
                            Convert.ToInt32(contatoreDa.Text);
                        if (contatoreA.Text != null && contatoreA.Text != "")
                            Convert.ToInt32(contatoreA.Text);
                    }
                    catch (Exception ex)
                    {
                        //SetFocus(contatoreDa);
                        oggettoCustom.VALORE_DATABASE = "";
                        return true;
                    }


                    //I campi sono valorizzati correttamente procedo
                    if (contatoreDa.Text != "" && contatoreA.Text != "")
                        oggettoCustom.VALORE_DATABASE = contatoreDa.Text + "@" + contatoreA.Text;

                    if (contatoreDa.Text != "" && contatoreA.Text == "")
                        oggettoCustom.VALORE_DATABASE = contatoreDa.Text;

                    switch (oggettoCustom.TIPO_CONTATORE)
                    {
                        case "A":
                            DropDownList ddlAoo = (DropDownList)PnlTypeDocument.FindControl(oggettoCustom.SYSTEM_ID.ToString() + "_menu");
                            oggettoCustom.ID_AOO_RF = ddlAoo.SelectedValue;
                            break;
                        case "R":
                            DropDownList ddlRf = (DropDownList)PnlTypeDocument.FindControl(oggettoCustom.SYSTEM_ID.ToString() + "_menu");
                            oggettoCustom.ID_AOO_RF = ddlRf.SelectedValue;
                            break;
                    }
                    break;
                case "Corrispondente":
                    UserControls.CorrespondentCustom corr = (UserControls.CorrespondentCustom)PnlTypeDocument.FindControl(oggettoCustom.SYSTEM_ID.ToString());
                    DocsPaWR.Corrispondente corrispondente = new DocsPaWR.Corrispondente();

                    if (corr != null)
                    {
                        // 1 - Ambedue i campi del corrispondente non sono valorizzati
                        if (string.IsNullOrEmpty(corr.TxtCodeCorrespondentCustom) && string.IsNullOrEmpty(corr.TxtDescriptionCorrespondentCustom))
                        {
                            oggettoCustom.VALORE_DATABASE = string.Empty;
                            return true;
                        }
                        // 2 - E' stato valorizzato solo il campo descrizione del corrispondente
                        if (string.IsNullOrEmpty(corr.TxtCodeCorrespondentCustom) && !string.IsNullOrEmpty(corr.TxtDescriptionCorrespondentCustom))
                        {
                            oggettoCustom.VALORE_DATABASE = corr.TxtDescriptionCorrespondentCustom;
                        }
                        // 3 - E' valorizzato il campo codice del corrispondente
                        if (!string.IsNullOrEmpty(corr.TxtCodeCorrespondentCustom))
                        {
                            //Cerco il corrispondente
                            if (!string.IsNullOrEmpty(corr.IdCorrespondentCustom))
                                corrispondente = UIManager.AddressBookManager.getCorrispondenteBySystemIDDisabled(corr.IdCorrespondentCustom);
                            else
                                corrispondente = UIManager.AddressBookManager.getCorrispondenteByCodRubrica(corr.TxtCodeCorrespondentCustom, false);

                            //corrispondente = UserManager.getCorrispondenteByCodRubrica(this, corr.CODICE_TEXT);
                            // 3.1 - Corrispondente trovato per codice
                            if (corrispondente != null)
                            {
                                oggettoCustom.VALORE_DATABASE = corrispondente.systemId;
                                oggettoCustom.ESTENDI_STORICIZZATI = corr.ChkStoryCustomCorrespondentCustom;
                            }
                            // 3.2 - Corrispondente non trovato per codice
                            else
                            {
                                // 3.2.1 - Campo descrizione non valorizzato
                                if (string.IsNullOrEmpty(corr.TxtDescriptionCorrespondentCustom))
                                {
                                    oggettoCustom.VALORE_DATABASE = string.Empty;
                                    return true;
                                }
                                // 3.2.2 - Campo descrizione valorizzato
                                else
                                    oggettoCustom.VALORE_DATABASE = corr.TxtDescriptionCorrespondentCustom;
                            }
                        }
                    }
                    break;
                case "ContatoreSottocontatore":
                    //TextBox contatoreSDa = (TextBox)PnlTypeDocument.FindControl("da_" + idOggetto);
                    //TextBox contatoreSA = (TextBox)PnlTypeDocument.FindControl("a_" + idOggetto);
                    //TextBox sottocontatoreDa = (TextBox)PnlTypeDocument.FindControl("da_sottocontatore_" + idOggetto);
                    //TextBox sottocontatoreA = (TextBox)PnlTypeDocument.FindControl("a_sottocontatore_" + idOggetto);
                    //UserControls.Calendar dataSottocontatoreDa = (UserControls.Calendar)PnlTypeDocument.FindControl("da_dataSottocontatore_" + oggettoCustom.SYSTEM_ID.ToString());
                    //UserControls.Calendar dataSottocontatoreA = (UserControls.Calendar)PnlTypeDocument.FindControl("a_dataSottocontatore_" + oggettoCustom.SYSTEM_ID.ToString());

                    ////Controllo la valorizzazione di campi ed eventualmente notifico gli errori
                    //switch (oggettoCustom.TIPO_CONTATORE)
                    //{
                    //    case "T":
                    //        if (contatoreSDa.Text.Equals("") && contatoreSA.Text.Equals("") &&
                    //            sottocontatoreDa.Text.Equals("") && sottocontatoreA.Text.Equals("") &&
                    //            dataSottocontatoreDa.Text.Equals("") && dataSottocontatoreA.Text.Equals("")
                    //            )
                    //        {
                    //            //SetFocus(contatoreDa);
                    //            oggettoCustom.VALORE_DATABASE = "";
                    //            oggettoCustom.VALORE_SOTTOCONTATORE = "";
                    //            oggettoCustom.DATA_INSERIMENTO = "";
                    //            return true;
                    //        }
                    //        break;
                    //    case "A":
                    //        DropDownList ddlAoo = (DropDownList)PnlTypeDocument.FindControl(oggettoCustom.SYSTEM_ID.ToString() + "_menu");
                    //        if (contatoreSDa.Text.Equals("") && sottocontatoreDa.Text.Equals("") && ddlAoo.SelectedValue.Equals(""))
                    //        {
                    //            //SetFocus(contatoreDa);
                    //            oggettoCustom.VALORE_DATABASE = "";
                    //            oggettoCustom.VALORE_SOTTOCONTATORE = "";
                    //            return true;
                    //        }

                    //        if (contatoreSDa.Text.Equals("") && contatoreSA.Text.Equals(""))
                    //            oggettoCustom.VALORE_DATABASE = "";

                    //        if (sottocontatoreDa.Text.Equals("") && sottocontatoreA.Text.Equals(""))
                    //            oggettoCustom.VALORE_SOTTOCONTATORE = "";

                    //        if (dataSottocontatoreDa.Text.Equals("") && dataSottocontatoreA.Text.Equals(""))
                    //            oggettoCustom.DATA_INSERIMENTO = "";
                    //        break;
                    //    case "R":
                    //        DropDownList ddlRf = (DropDownList)PnlTypeDocument.FindControl(oggettoCustom.SYSTEM_ID.ToString() + "_menu");
                    //        if (contatoreSDa.Text.Equals("") && sottocontatoreDa.Text.Equals("") && ddlRf.SelectedValue.Equals(""))
                    //        {
                    //            //SetFocus(contatoreDa);
                    //            oggettoCustom.VALORE_DATABASE = "";
                    //            oggettoCustom.VALORE_SOTTOCONTATORE = "";
                    //            return true;
                    //        }

                    //        if (contatoreSDa.Text.Equals("") && contatoreSA.Text.Equals(""))
                    //            oggettoCustom.VALORE_DATABASE = "";

                    //        if (sottocontatoreDa.Text.Equals("") && sottocontatoreA.Text.Equals(""))
                    //            oggettoCustom.VALORE_SOTTOCONTATORE = "";

                    //        if (dataSottocontatoreDa.Text.Equals("") && dataSottocontatoreA.Text.Equals(""))
                    //            oggettoCustom.DATA_INSERIMENTO = "";
                    //        break;
                    //}

                    //if (contatoreSDa.Text.Equals("") && contatoreSA.Text.Equals("") &&
                    //    sottocontatoreDa.Text.Equals("") && sottocontatoreA.Text.Equals("") &&
                    //    dataSottocontatoreDa.Text.Equals("") && dataSottocontatoreA.Text.Equals("")
                    //    )
                    //{
                    //    //SetFocus(contatoreDa);
                    //    oggettoCustom.VALORE_DATABASE = "";
                    //    oggettoCustom.VALORE_SOTTOCONTATORE = "";
                    //    oggettoCustom.DATA_INSERIMENTO = "";
                    //    return true;
                    //}

                    //if (contatoreSDa.Text != null && contatoreSDa.Text != "")
                    //    Convert.ToInt32(contatoreSDa.Text);
                    //if (contatoreSA.Text != null && contatoreSA.Text != "")
                    //    Convert.ToInt32(contatoreSA.Text);
                    //if (sottocontatoreDa.Text != null && sottocontatoreDa.Text != "")
                    //    Convert.ToInt32(sottocontatoreDa.Text);
                    //if (sottocontatoreA.Text != null && sottocontatoreA.Text != "")
                    //    Convert.ToInt32(sottocontatoreA.Text);


                    ////I campi sono valorizzati correttamente procedo
                    //if (contatoreSDa.Text != "" && contatoreSA.Text != "")
                    //    oggettoCustom.VALORE_DATABASE = contatoreSDa.Text + "@" + contatoreSA.Text;

                    //if (contatoreSDa.Text != "" && contatoreSA.Text == "")
                    //    oggettoCustom.VALORE_DATABASE = contatoreSDa.Text;

                    //if (sottocontatoreDa.Text != "" && sottocontatoreA.Text != "")
                    //    oggettoCustom.VALORE_SOTTOCONTATORE = sottocontatoreDa.Text + "@" + sottocontatoreA.Text;

                    //if (sottocontatoreDa.Text != "" && sottocontatoreA.Text == "")
                    //    oggettoCustom.VALORE_SOTTOCONTATORE = sottocontatoreDa.Text;

                    //if (dataSottocontatoreDa.Text != "" && dataSottocontatoreA.Text != "")
                    //    oggettoCustom.DATA_INSERIMENTO = dataSottocontatoreDa.Text + "@" + dataSottocontatoreA.Text;

                    //if (dataSottocontatoreDa.Text != "" && dataSottocontatoreA.Text == "")
                    //    oggettoCustom.DATA_INSERIMENTO = dataSottocontatoreDa.Text;

                    //switch (oggettoCustom.TIPO_CONTATORE)
                    //{
                    //    case "A":
                    //        DropDownList ddlAoo = (DropDownList)PnlTypeDocument.FindControl(oggettoCustom.SYSTEM_ID.ToString() + "_menu");
                    //        oggettoCustom.ID_AOO_RF = ddlAoo.SelectedValue;
                    //        break;
                    //    case "R":
                    //        DropDownList ddlRf = (DropDownList)PnlTypeDocument.FindControl(oggettoCustom.SYSTEM_ID.ToString() + "_menu");
                    //        oggettoCustom.ID_AOO_RF = ddlRf.SelectedValue;
                    //        break;
                    //}
                    break;


            }
            return false;
        }

        //oldcode
        //private bool controllaCampi(DocsPaWR.OggettoCustom oggettoCustom, string idOggetto)
        //{
        //    //In questo metodo, oltre al controllo si salvano i valori dei campi inseriti 
        //    //dall'utente nel template in sessione. Solo successivamente, quanto verra' salvato 
        //    //il documento i suddetti valori verranno riportai nel Db vedi metodo "btn_salva_Click" della "docProfilo.aspx"

        //    //Label_Avviso.Visible = false;
        //    switch (oggettoCustom.TIPO.DESCRIZIONE_TIPO)
        //    {
        //        case "CampoDiTesto":
        //            CustomTextArea textBox = (CustomTextArea)PnlTypeDocument.FindControl(idOggetto);
        //            if (textBox.Text.Equals(""))
        //            {
        //                //SetFocus(textBox);
        //                oggettoCustom.VALORE_DATABASE = textBox.Text;
        //                return true;
        //            }
        //            oggettoCustom.VALORE_DATABASE = textBox.Text;
        //            break;
        //        case "CasellaDiSelezione":
        //            CheckBoxList checkBox = (CheckBoxList)PnlTypeDocument.FindControl(idOggetto);
        //            if (checkBox.SelectedIndex == -1)
        //            {
        //                //SetFocus(checkBox);
        //                for (int i = 0; i < oggettoCustom.VALORI_SELEZIONATI.Length; i++)
        //                    oggettoCustom.VALORI_SELEZIONATI[i] = null;

        //                return true;
        //            }

        //            oggettoCustom.VALORI_SELEZIONATI = new string[checkBox.Items.Count];
        //            oggettoCustom.VALORE_DATABASE = "";
        //            for (int i = 0; i < checkBox.Items.Count; i++)
        //            {
        //                if (checkBox.Items[i].Selected)
        //                {
        //                    oggettoCustom.VALORI_SELEZIONATI[i] = checkBox.Items[i].Text;
        //                }
        //            }
        //            break;
        //        case "MenuATendina":
        //            DropDownList dropDwonList = (DropDownList)PnlTypeDocument.FindControl(idOggetto);
        //            if (dropDwonList.SelectedItem.Text.Equals(""))
        //            {
        //                //SetFocus(dropDwonList);
        //                oggettoCustom.VALORE_DATABASE = "";
        //                return true;
        //            }
        //            oggettoCustom.VALORE_DATABASE = dropDwonList.SelectedItem.Text;
        //            break;
        //        case "SelezioneEsclusiva":
        //            RadioButtonList radioButtonList = (RadioButtonList)PnlTypeDocument.FindControl(idOggetto);
        //            if (oggettoCustom.VALORE_DATABASE == "-1" || radioButtonList.SelectedIndex == -1 || radioButtonList.SelectedValue == "-1")
        //            {
        //                //SetFocus(radioButtonList);
        //                oggettoCustom.VALORE_DATABASE = "";
        //                return true;
        //            }
        //            oggettoCustom.VALORE_DATABASE = radioButtonList.SelectedItem.Text;
        //            break;
        //        case "Data":
        //            UserControls.Calendar dataDa = (UserControls.Calendar)PnlTypeDocument.FindControl("da_" + oggettoCustom.SYSTEM_ID.ToString());
        //            UserControls.Calendar dataA = (UserControls.Calendar)PnlTypeDocument.FindControl("a_" + oggettoCustom.SYSTEM_ID.ToString());

        //            if (dataDa.Text.Equals("") && dataA.Text.Equals(""))
        //            {
        //                //SetFocus(dataDa.txt_Data);
        //                oggettoCustom.VALORE_DATABASE = "";
        //                return true;
        //            }

        //            if (dataDa.Text.Equals("") && dataA.Text != "")
        //            {
        //                //SetFocus(dataDa.txt_Data);
        //                oggettoCustom.VALORE_DATABASE = "";
        //                return true;
        //            }

        //            if (dataDa.Text != "" && dataA.Text != "")
        //                //oggettoCustom.VALORE_DATABASE = dataDa.txt_Data.Text + "@" + dataA.txt_Data.Text;
        //                oggettoCustom.VALORE_DATABASE = dataDa.Text + "@" + dataA.Text;

        //            if (dataDa.Text != "" && dataA.Text == "")
        //                //oggettoCustom.VALORE_DATABASE = dataDa.txt_Data.Text;
        //                oggettoCustom.VALORE_DATABASE = dataDa.Text;
        //            break;
        //        case "Contatore":
        //            CustomTextArea contatoreDa = (CustomTextArea)PnlTypeDocument.FindControl("da_" + idOggetto);
        //            CustomTextArea contatoreA = (CustomTextArea)PnlTypeDocument.FindControl("a_" + idOggetto);
        //            //Controllo la valorizzazione di campi ed eventualmente notifico gli errori
        //            switch (oggettoCustom.TIPO_CONTATORE)
        //            {
        //                case "T":
        //                    if (contatoreDa.Text.Equals("") && contatoreA.Text.Equals(""))
        //                    {
        //                        //SetFocus(contatoreDa);
        //                        oggettoCustom.VALORE_DATABASE = "";
        //                        return true;
        //                    }
        //                    break;
        //                case "A":
        //                    DropDownList ddlAoo = (DropDownList)PnlTypeDocument.FindControl(oggettoCustom.SYSTEM_ID.ToString() + "_menu");
        //                    if (contatoreDa.Text.Equals("") && ddlAoo.SelectedValue.Equals(""))
        //                    {
        //                        //SetFocus(contatoreDa);
        //                        oggettoCustom.VALORE_DATABASE = "";
        //                        return true;
        //                    }

        //                    if (contatoreDa.Text.Equals("") && contatoreA.Text.Equals(""))
        //                        oggettoCustom.VALORE_DATABASE = "";
        //                    break;
        //                case "R":
        //                    DropDownList ddlRf = (DropDownList)PnlTypeDocument.FindControl(oggettoCustom.SYSTEM_ID.ToString() + "_menu");
        //                    if (contatoreDa.Text.Equals("") && ddlRf.SelectedValue.Equals(""))
        //                    {
        //                        //SetFocus(contatoreDa);
        //                        oggettoCustom.VALORE_DATABASE = "";
        //                        return true;
        //                    }

        //                    if (contatoreDa.Text.Equals("") && contatoreA.Text.Equals(""))
        //                        oggettoCustom.VALORE_DATABASE = "";
        //                    break;
        //            }

        //            if (contatoreDa.Text.Equals("") && contatoreA.Text != "")
        //            {
        //                //SetFocus(contatoreDa);
        //                oggettoCustom.VALORE_DATABASE = "";
        //                return true;
        //            }

        //            try
        //            {
        //                if (contatoreDa.Text != null && contatoreDa.Text != "")
        //                    Convert.ToInt32(contatoreDa.Text);
        //                if (contatoreA.Text != null && contatoreA.Text != "")
        //                    Convert.ToInt32(contatoreA.Text);
        //            }
        //            catch (Exception ex)
        //            {
        //                //SetFocus(contatoreDa);
        //                oggettoCustom.VALORE_DATABASE = "";
        //                return true;
        //            }


        //            //I campi sono valorizzati correttamente procedo
        //            if (contatoreDa.Text != "" && contatoreA.Text != "")
        //                oggettoCustom.VALORE_DATABASE = contatoreDa.Text + "@" + contatoreA.Text;

        //            if (contatoreDa.Text != "" && contatoreA.Text == "")
        //                oggettoCustom.VALORE_DATABASE = contatoreDa.Text;

        //            switch (oggettoCustom.TIPO_CONTATORE)
        //            {
        //                case "A":
        //                    DropDownList ddlAoo = (DropDownList)PnlTypeDocument.FindControl(oggettoCustom.SYSTEM_ID.ToString() + "_menu");
        //                    oggettoCustom.ID_AOO_RF = ddlAoo.SelectedValue;
        //                    break;
        //                case "R":
        //                    DropDownList ddlRf = (DropDownList)PnlTypeDocument.FindControl(oggettoCustom.SYSTEM_ID.ToString() + "_menu");
        //                    oggettoCustom.ID_AOO_RF = ddlRf.SelectedValue;
        //                    break;
        //            }
        //            break;
        //        case "Corrispondente":
        //            UserControls.CorrespondentCustom corr = (UserControls.CorrespondentCustom)PnlTypeDocument.FindControl(oggettoCustom.SYSTEM_ID.ToString());
        //            DocsPaWR.Corrispondente corrispondente = new DocsPaWR.Corrispondente();

        //            if (corr != null)
        //            {
        //                // 1 - Ambedue i campi del corrispondente non sono valorizzati
        //                if (string.IsNullOrEmpty(corr.TxtCodeCorrespondentCustom) && string.IsNullOrEmpty(corr.TxtDescriptionCorrespondentCustom))
        //                {
        //                    oggettoCustom.VALORE_DATABASE = string.Empty;
        //                    return true;
        //                }
        //                // 2 - E' stato valorizzato solo il campo descrizione del corrispondente
        //                if (string.IsNullOrEmpty(corr.TxtCodeCorrespondentCustom) && !string.IsNullOrEmpty(corr.TxtDescriptionCorrespondentCustom))
        //                {
        //                    oggettoCustom.VALORE_DATABASE = corr.TxtDescriptionCorrespondentCustom;

        //                }
        //                // 3 - E' valorizzato il campo codice del corrispondente
        //                if (!string.IsNullOrEmpty(corr.TxtCodeCorrespondentCustom))
        //                {
        //                    //Cerco il corrispondente
        //                    if (!string.IsNullOrEmpty(corr.IdCorrespondentCustom))
        //                        corrispondente = UIManager.AddressBookManager.getCorrispondenteBySystemIDDisabled(corr.IdCorrespondentCustom);
        //                    else
        //                        corrispondente = UIManager.AddressBookManager.getCorrispondenteByCodRubrica(corr.TxtCodeCorrespondentCustom, false);

        //                    //corrispondente = UserManager.getCorrispondenteByCodRubrica(this, corr.CODICE_TEXT);
        //                    // 3.1 - Corrispondente trovato per codice
        //                    if (corrispondente != null)
        //                    {
        //                        oggettoCustom.VALORE_DATABASE = corrispondente.systemId;
        //                        oggettoCustom.ESTENDI_STORICIZZATI = corr.ChkStoryCustomCorrespondentCustom;
        //                    }
        //                    // 3.2 - Corrispondente non trovato per codice
        //                    else
        //                    {
        //                        // 3.2.1 - Campo descrizione non valorizzato
        //                        if (string.IsNullOrEmpty(corr.TxtDescriptionCorrespondentCustom))
        //                        {
        //                            oggettoCustom.VALORE_DATABASE = string.Empty;
        //                            return true;
        //                        }
        //                        // 3.2.2 - Campo descrizione valorizzato
        //                        else
        //                            oggettoCustom.VALORE_DATABASE = corr.TxtDescriptionCorrespondentCustom;
        //                    }
        //                }
        //            }
        //            break;
        //        case "ContatoreSottocontatore":
        //            //TextBox contatoreSDa = (TextBox)PnlTypeDocument.FindControl("da_" + idOggetto);
        //            //TextBox contatoreSA = (TextBox)PnlTypeDocument.FindControl("a_" + idOggetto);
        //            //TextBox sottocontatoreDa = (TextBox)PnlTypeDocument.FindControl("da_sottocontatore_" + idOggetto);
        //            //TextBox sottocontatoreA = (TextBox)PnlTypeDocument.FindControl("a_sottocontatore_" + idOggetto);
        //            //UserControls.Calendar dataSottocontatoreDa = (UserControls.Calendar)PnlTypeDocument.FindControl("da_dataSottocontatore_" + oggettoCustom.SYSTEM_ID.ToString());
        //            //UserControls.Calendar dataSottocontatoreA = (UserControls.Calendar)PnlTypeDocument.FindControl("a_dataSottocontatore_" + oggettoCustom.SYSTEM_ID.ToString());

        //            ////Controllo la valorizzazione di campi ed eventualmente notifico gli errori
        //            //switch (oggettoCustom.TIPO_CONTATORE)
        //            //{
        //            //    case "T":
        //            //        if (contatoreSDa.Text.Equals("") && contatoreSA.Text.Equals("") &&
        //            //            sottocontatoreDa.Text.Equals("") && sottocontatoreA.Text.Equals("") &&
        //            //            dataSottocontatoreDa.Text.Equals("") && dataSottocontatoreA.Text.Equals("")
        //            //            )
        //            //        {
        //            //            //SetFocus(contatoreDa);
        //            //            oggettoCustom.VALORE_DATABASE = "";
        //            //            oggettoCustom.VALORE_SOTTOCONTATORE = "";
        //            //            oggettoCustom.DATA_INSERIMENTO = "";
        //            //            return true;
        //            //        }
        //            //        break;
        //            //    case "A":
        //            //        DropDownList ddlAoo = (DropDownList)PnlTypeDocument.FindControl(oggettoCustom.SYSTEM_ID.ToString() + "_menu");
        //            //        if (contatoreSDa.Text.Equals("") && sottocontatoreDa.Text.Equals("") && ddlAoo.SelectedValue.Equals(""))
        //            //        {
        //            //            //SetFocus(contatoreDa);
        //            //            oggettoCustom.VALORE_DATABASE = "";
        //            //            oggettoCustom.VALORE_SOTTOCONTATORE = "";
        //            //            return true;
        //            //        }

        //            //        if (contatoreSDa.Text.Equals("") && contatoreSA.Text.Equals(""))
        //            //            oggettoCustom.VALORE_DATABASE = "";

        //            //        if (sottocontatoreDa.Text.Equals("") && sottocontatoreA.Text.Equals(""))
        //            //            oggettoCustom.VALORE_SOTTOCONTATORE = "";

        //            //        if (dataSottocontatoreDa.Text.Equals("") && dataSottocontatoreA.Text.Equals(""))
        //            //            oggettoCustom.DATA_INSERIMENTO = "";
        //            //        break;
        //            //    case "R":
        //            //        DropDownList ddlRf = (DropDownList)PnlTypeDocument.FindControl(oggettoCustom.SYSTEM_ID.ToString() + "_menu");
        //            //        if (contatoreSDa.Text.Equals("") && sottocontatoreDa.Text.Equals("") && ddlRf.SelectedValue.Equals(""))
        //            //        {
        //            //            //SetFocus(contatoreDa);
        //            //            oggettoCustom.VALORE_DATABASE = "";
        //            //            oggettoCustom.VALORE_SOTTOCONTATORE = "";
        //            //            return true;
        //            //        }

        //            //        if (contatoreSDa.Text.Equals("") && contatoreSA.Text.Equals(""))
        //            //            oggettoCustom.VALORE_DATABASE = "";

        //            //        if (sottocontatoreDa.Text.Equals("") && sottocontatoreA.Text.Equals(""))
        //            //            oggettoCustom.VALORE_SOTTOCONTATORE = "";

        //            //        if (dataSottocontatoreDa.Text.Equals("") && dataSottocontatoreA.Text.Equals(""))
        //            //            oggettoCustom.DATA_INSERIMENTO = "";
        //            //        break;
        //            //}

        //            //if (contatoreSDa.Text.Equals("") && contatoreSA.Text.Equals("") &&
        //            //    sottocontatoreDa.Text.Equals("") && sottocontatoreA.Text.Equals("") &&
        //            //    dataSottocontatoreDa.Text.Equals("") && dataSottocontatoreA.Text.Equals("")
        //            //    )
        //            //{
        //            //    //SetFocus(contatoreDa);
        //            //    oggettoCustom.VALORE_DATABASE = "";
        //            //    oggettoCustom.VALORE_SOTTOCONTATORE = "";
        //            //    oggettoCustom.DATA_INSERIMENTO = "";
        //            //    return true;
        //            //}

        //            //if (contatoreSDa.Text != null && contatoreSDa.Text != "")
        //            //    Convert.ToInt32(contatoreSDa.Text);
        //            //if (contatoreSA.Text != null && contatoreSA.Text != "")
        //            //    Convert.ToInt32(contatoreSA.Text);
        //            //if (sottocontatoreDa.Text != null && sottocontatoreDa.Text != "")
        //            //    Convert.ToInt32(sottocontatoreDa.Text);
        //            //if (sottocontatoreA.Text != null && sottocontatoreA.Text != "")
        //            //    Convert.ToInt32(sottocontatoreA.Text);


        //            ////I campi sono valorizzati correttamente procedo
        //            //if (contatoreSDa.Text != "" && contatoreSA.Text != "")
        //            //    oggettoCustom.VALORE_DATABASE = contatoreSDa.Text + "@" + contatoreSA.Text;

        //            //if (contatoreSDa.Text != "" && contatoreSA.Text == "")
        //            //    oggettoCustom.VALORE_DATABASE = contatoreSDa.Text;

        //            //if (sottocontatoreDa.Text != "" && sottocontatoreA.Text != "")
        //            //    oggettoCustom.VALORE_SOTTOCONTATORE = sottocontatoreDa.Text + "@" + sottocontatoreA.Text;

        //            //if (sottocontatoreDa.Text != "" && sottocontatoreA.Text == "")
        //            //    oggettoCustom.VALORE_SOTTOCONTATORE = sottocontatoreDa.Text;

        //            //if (dataSottocontatoreDa.Text != "" && dataSottocontatoreA.Text != "")
        //            //    oggettoCustom.DATA_INSERIMENTO = dataSottocontatoreDa.Text + "@" + dataSottocontatoreA.Text;

        //            //if (dataSottocontatoreDa.Text != "" && dataSottocontatoreA.Text == "")
        //            //    oggettoCustom.DATA_INSERIMENTO = dataSottocontatoreDa.Text;

        //            //switch (oggettoCustom.TIPO_CONTATORE)
        //            //{
        //            //    case "A":
        //            //        DropDownList ddlAoo = (DropDownList)PnlTypeDocument.FindControl(oggettoCustom.SYSTEM_ID.ToString() + "_menu");
        //            //        oggettoCustom.ID_AOO_RF = ddlAoo.SelectedValue;
        //            //        break;
        //            //    case "R":
        //            //        DropDownList ddlRf = (DropDownList)PnlTypeDocument.FindControl(oggettoCustom.SYSTEM_ID.ToString() + "_menu");
        //            //        oggettoCustom.ID_AOO_RF = ddlRf.SelectedValue;
        //            //        break;
        //            //}
        //            break;


        //    }
        //    return false;
        //}

        //protected void rbViewAllNo_CheckedChanged(object sender, EventArgs e)
        //{

        //}

        //protected void chkConservationNo_CheckedChanged(object sender, EventArgs e)
        //{

        //}

        private bool CustomDocuments
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["customDocuments"] != null)
                {
                    result = bool.Parse(HttpContext.Current.Session["customDocuments"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["customDocuments"] = value;
            }
        }

        private bool EnableStateDiagram
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["enableStateDiagram"] != null)
                {
                    result = bool.Parse(HttpContext.Current.Session["enableStateDiagram"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["enableStateDiagram"] = value;
            }
        }


        private DocsPaWR.Templates Template
        {
            get
            {
                Templates result = null;
                if (HttpContext.Current.Session["templateRc"] != null)
                {
                    result = HttpContext.Current.Session["templateRc"] as Templates;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["templateRc"] = value;
            }
        }

        protected void ImgCreatoreAddressBook_Click(object sender, ImageClickEventArgs e)
        {
            this.CallType = RubricaCallType.CALLTYPE_OWNER_AUTHOR;
            HttpContext.Current.Session["AddressBook.from"] = "F_X_X_S";
            HttpContext.Current.Session["AddressBook.EnableOnly"] = this.rblOwnerTypeCreator.SelectedValue;
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "AddressBook", "ajaxModalPopupAddressBook();", true);
        }


        protected void PopulateProfiledDocument()
        {
            this.PnlTypeDocument.Controls.Clear();
            this.inserisciComponenti(false);
        }

        private void inserisciComponenti(bool readOnly)
        {
            RemovePropertySearchCorrespondentIntExtWithDisabled();
            ArrayList dirittiCampiRuolo = ProfilerProjectManager.getDirittiCampiTipologiaFasc(UIManager.RoleManager.GetRoleInSession().idGruppo, this.Template.SYSTEM_ID.ToString());

            for (int i = 0, index = 0; i < this.Template.ELENCO_OGGETTI.Length; i++)
            {
                DocsPaWR.OggettoCustom oggettoCustom = (DocsPaWR.OggettoCustom)this.Template.ELENCO_OGGETTI[i];

                ProfilerProjectManager.addNoRightsCustomObject(dirittiCampiRuolo, oggettoCustom);

                switch (oggettoCustom.TIPO.DESCRIZIONE_TIPO)
                {
                    case "CampoDiTesto":
                        this.inserisciCampoDiTesto(oggettoCustom, readOnly, index++, dirittiCampiRuolo);
                        break;
                    case "CasellaDiSelezione":
                        this.inserisciCasellaDiSelezione(oggettoCustom, readOnly, index++, dirittiCampiRuolo);
                        break;
                    case "MenuATendina":
                        this.inserisciMenuATendina(oggettoCustom, readOnly, index++, dirittiCampiRuolo);
                        break;
                    case "SelezioneEsclusiva":
                        this.inserisciSelezioneEsclusiva(oggettoCustom, readOnly, index++, dirittiCampiRuolo);
                        break;
                    case "Contatore":
                        this.inserisciContatore(oggettoCustom, readOnly, dirittiCampiRuolo);
                        break;
                    case "Data":
                        this.inserisciData(oggettoCustom, readOnly, index++, dirittiCampiRuolo);
                        break;
                    case "Corrispondente":
                        SearchCorrespondentIntExtWithDisabled = true;
                        this.inserisciCorrispondente(oggettoCustom, readOnly, index++, dirittiCampiRuolo);
                        break;
                    case "Link":
                        this.inserisciLink(oggettoCustom, readOnly, dirittiCampiRuolo);
                        break;
                    case "ContatoreSottocontatore":
                        this.inserisciContatoreSottocontatore(oggettoCustom, readOnly, dirittiCampiRuolo);
                        break;
                    case "Separatore":
                        this.inserisciCampoSeparatore(oggettoCustom);
                        break;
                }
            }
        }

        private void inserisciLink(DocsPaWR.OggettoCustom oggettoCustom, bool readOnly, ArrayList dirittiCampiRuolo)
        {
            if (string.IsNullOrEmpty(oggettoCustom.DESCRIZIONE))
            {
                return;
            }

            UserControls.LinkDocFasc link = (UserControls.LinkDocFasc)this.LoadControl("../UserControls/LinkDocFasc.ascx");
            link.EnableViewState = true;
            link.ID = oggettoCustom.SYSTEM_ID.ToString();
            link.IsEsterno = (oggettoCustom.TIPO_LINK.Equals("ESTERNO"));
            link.IsFascicolo = ("FASCICOLO".Equals(oggettoCustom.TIPO_OBJ_LINK));

            link.TxtEtiLinkDocOrFasc = oggettoCustom.DESCRIZIONE;

            //Verifico i diritti del ruolo sul campo
            this.impostaDirittiRuoloSulCampo(link.TxtEtiLinkDocOrFasc, link, oggettoCustom, this.Template, dirittiCampiRuolo);

            link.Value = oggettoCustom.VALORE_DATABASE;

            if (link.Visible)
            {
                this.PnlTypeDocument.Controls.Add(link);
            }
        }

        private void HandleInternalDoc(string idDoc)
        {
            //InfoDocumento infoDoc = DocumentManager.GetInfoDocumento(idDoc, null, this);
            //if (infoDoc == null)
            //{
            //    Response.Write("<script language='javascript'>alert('Non si possiedono i diritti per la visualizzazione')</script>");
            //    return;
            //}
            //string errorMessage = "";
            //int result = DocumentManager.verificaACL("D", infoDoc.idProfile, UserManager.getInfoUtente(), out errorMessage);
            //if (result != 2)
            //{
            //    Response.Write("<script language='javascript'>alert('Non si possiedono i diritti per la visualizzazione')</script>");
            //}
            //else
            //{
            //    DocumentManager.setRisultatoRicerca(this, infoDoc);
            //    Response.Write("<script language='javascript'> top.principale.document.location = '../documento/gestionedoc.aspx?tab=protocollo&forceNewContext=true';</script>");
            //}
        }


        private void HandleInternalFasc(string idFasc)
        {
            //Fascicolo fasc = FascicoliManager.getFascicoloById(this, idFasc);
            //if (fasc == null)
            //{
            //    Response.Write("<script language='javascript'>alert('Non si possiedono i diritti per la visualizzazione')</script>");
            //    return;
            //}
            //string errorMessage = "";
            //int result = DocumentManager.verificaACL("F", fasc.systemID, UserManager.getInfoUtente(), out errorMessage);
            //if (result != 2)
            //{
            //    Response.Write("<script language='javascript'>alert('Non si possiedono i diritti per la visualizzazione')</script>");
            //}
            //else
            //{
            //    FascicoliManager.setFascicoloSelezionato(this, fasc);
            //    string newUrl = "../fascicolo/gestioneFasc.aspx?tab=documenti&forceNewContext=true";
            //    Response.Write("<script language='javascript'>top.principale.document.location='" + newUrl + "';</script>");
            //}
        }

        private void inserisciData(DocsPaWR.OggettoCustom oggettoCustom, bool readOnly, int index, ArrayList dirittiCampiRuolo)
        {
            //Per il momento questo tipo di campo è stato implementato con tre semplici textBox
            //Sarebbe opportuno creare un oggetto personalizzato, che espone le stesse funzionalità
            //della textBox, ma che mi permette di gestire la data con i tre campi separati.
            DocsPaWR.StoricoProfilatiOldValue dataOldOb = new StoricoProfilatiOldValue();
            if (string.IsNullOrEmpty(oggettoCustom.DESCRIZIONE))
            {
                return;
            }
            Label etichettaData = new Label();
            etichettaData.EnableViewState = true;


            etichettaData.Text = oggettoCustom.DESCRIZIONE;

            etichettaData.CssClass = "weight";

            UserControls.Calendar data = (UserControls.Calendar)this.LoadControl("../UserControls/Calendar.ascx");
            data.EnableViewState = true;
            data.ID = "da_" + oggettoCustom.SYSTEM_ID.ToString();
            data.VisibleTimeMode = ProfilerDocManager.getVisibleTimeMode(oggettoCustom);
            data.SetEnableTimeMode();

            UserControls.Calendar data2 = (UserControls.Calendar)this.LoadControl("../UserControls/Calendar.ascx");
            data2.EnableViewState = true;
            data2.ID = "a_" + oggettoCustom.SYSTEM_ID.ToString();
            data2.VisibleTimeMode = ProfilerDocManager.getVisibleTimeMode(oggettoCustom);
            data2.SetEnableTimeMode();

            //Verifico i diritti del ruolo sul campo
            this.impostaDirittiRuoloSulCampo(etichettaData, data, oggettoCustom, this.Template, dirittiCampiRuolo);

            Panel divRowDesc = new Panel();
            divRowDesc.CssClass = "row";
            divRowDesc.EnableViewState = true;

            Panel divColDesc = new Panel();
            divColDesc.CssClass = "col";
            divColDesc.EnableViewState = true;

            HtmlGenericControl parDesc = new HtmlGenericControl("p");
            parDesc.Controls.Add(etichettaData);
            parDesc.EnableViewState = true;

            divColDesc.Controls.Add(parDesc);
            divRowDesc.Controls.Add(divColDesc);

            if (etichettaData.Visible)
            {
                this.PnlTypeDocument.Controls.Add(divRowDesc);
            }

            Label etichettaDataFrom = new Label();
            etichettaDataFrom.EnableViewState = true;
            etichettaDataFrom.Text = "Da";

            HtmlGenericControl parDescFrom = new HtmlGenericControl("p");
            parDescFrom.Controls.Add(etichettaDataFrom);
            parDescFrom.EnableViewState = true;

            Panel divRowValueFrom = new Panel();
            divRowValueFrom.CssClass = "row";
            divRowValueFrom.EnableViewState = true;

            Panel divColValueFrom = new Panel();
            divColValueFrom.CssClass = "col";
            divColValueFrom.EnableViewState = true;

            divColValueFrom.Controls.Add(parDescFrom);
            divRowValueFrom.Controls.Add(divColValueFrom);

            if (data.Visible)
            {
                this.PnlTypeDocument.Controls.Add(divRowValueFrom);
            }

            Panel divRowValue = new Panel();
            divRowValue.CssClass = "row";
            divRowValue.EnableViewState = true;

            Panel divColValue = new Panel();
            divColValue.CssClass = "col";
            divColValue.EnableViewState = true;

            divColValue.Controls.Add(data);
            divRowValue.Controls.Add(divColValue);

            if (data.Visible)
            {
                this.PnlTypeDocument.Controls.Add(divRowValue);
            }

            //////
            Label etichettaDataTo = new Label();
            etichettaDataTo.EnableViewState = true;
            etichettaDataTo.Text = "A";

            Panel divRowValueTo = new Panel();
            divRowValueTo.CssClass = "row";
            divRowValueTo.EnableViewState = true;

            Panel divColValueTo = new Panel();
            divColValueTo.CssClass = "col";
            divColValueTo.EnableViewState = true;

            HtmlGenericControl parDescTo = new HtmlGenericControl("p");
            parDescTo.Controls.Add(etichettaDataTo);
            parDescTo.EnableViewState = true;

            divColValueTo.Controls.Add(parDescTo);
            divRowValueTo.Controls.Add(divColValueTo);

            if (data.Visible)
            {
                this.PnlTypeDocument.Controls.Add(divRowValueTo);
            }

            Panel divRowValue2 = new Panel();
            divRowValue2.CssClass = "row";
            divRowValue2.EnableViewState = true;


            Panel divColValue2 = new Panel();
            divColValue2.CssClass = "col";
            divColValue2.EnableViewState = true;

            divColValue2.Controls.Add(data2);
            divRowValue2.Controls.Add(divColValue2);

            if (data.Visible)
            {
                this.PnlTypeDocument.Controls.Add(divRowValue2);
            }

            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshPicker", "DatePicker('" + UIManager.UserManager.GetLanguageData() + "');", true);
        }

        private void inserisciMenuATendina(DocsPaWR.OggettoCustom oggettoCustom, bool readOnly, int index, ArrayList dirittiCampiRuolo)
        {
            if (string.IsNullOrEmpty(oggettoCustom.DESCRIZIONE))
            {
                return;
            }

            DocsPaWR.StoricoProfilatiOldValue menuOldObj = new StoricoProfilatiOldValue();
            Label etichettaMenuATendina = new Label();
            etichettaMenuATendina.EnableViewState = true;
            etichettaMenuATendina.Text = oggettoCustom.DESCRIZIONE;

            etichettaMenuATendina.CssClass = "weight";

            int maxLenght = 0;
            DropDownList menuATendina = new DropDownList();
            menuATendina.EnableViewState = true;
            menuATendina.ID = oggettoCustom.SYSTEM_ID.ToString();
            //int valoreDiDefault = -1;
            for (int i = 0; i < oggettoCustom.ELENCO_VALORI.Length; i++)
            {
                DocsPaWR.ValoreOggetto valoreOggetto = ((DocsPaWR.ValoreOggetto)(oggettoCustom.ELENCO_VALORI[i]));
                //Valori disabilitati/abilitati
                //if (valoreOggetto.ABILITATO == 1 || (valoreOggetto.ABILITATO == 0 && valoreOggetto.VALORE == oggettoCustom.VALORE_DATABASE))
                //{
                //    //Nel caso il valore è disabilitato ma selezionato lo rendo disponibile solo fino al salvataggio del documento 
                //    if (valoreOggetto.ABILITATO == 0 && valoreOggetto.VALORE == oggettoCustom.VALORE_DATABASE)
                //        valoreOggetto.ABILITATO = 1;

                    menuATendina.Items.Add(new ListItem(valoreOggetto.VALORE, valoreOggetto.VALORE));
                    //Valore di default
                    //if (valoreOggetto.VALORE_DI_DEFAULT.Equals("SI"))
                    //{
                    //    valoreDiDefault = i;
                    //}

                    if (maxLenght < valoreOggetto.VALORE.Length)
                    {
                        maxLenght = valoreOggetto.VALORE.Length;
                    }
              //  }
            }
            menuATendina.CssClass = "chzn-select-deselect";
            string language = UIManager.UserManager.GetUserLanguage();
            menuATendina.Attributes.Add("data-placeholder", Utils.Languages.GetLabelFromCode("SelectProflierMenu", language));
            menuATendina.Width = maxLenght + 250;

            //if (valoreDiDefault != -1)
            //{
            //    menuATendina.SelectedIndex = valoreDiDefault;
            //}
            //if (!(valoreDiDefault != -1 && oggettoCustom.CAMPO_OBBLIGATORIO.Equals("SI")))
            //{
                menuATendina.Items.Insert(0, "");
            //}
            if (!oggettoCustom.VALORE_DATABASE.Equals(""))
            {
                menuATendina.SelectedIndex = this.impostaSelezioneMenuATendina(oggettoCustom.VALORE_DATABASE, menuATendina);
            }

            Panel divRowDesc = new Panel();
            divRowDesc.CssClass = "row";
            divRowDesc.EnableViewState = true;

            Panel divColDesc = new Panel();
            divColDesc.CssClass = "col";

            HtmlGenericControl parDesc = new HtmlGenericControl("p");
            parDesc.Controls.Add(etichettaMenuATendina);
            parDesc.EnableViewState = true;



            Panel divRowValue = new Panel();
            divRowValue.CssClass = "row";
            divRowValue.EnableViewState = true;

            Panel divColValue = new Panel();
            divColValue.CssClass = "col_full";
            divColValue.EnableViewState = true;

            //Verifico i diritti del ruolo sul campo
            this.impostaDirittiRuoloSulCampo(etichettaMenuATendina, menuATendina, oggettoCustom, this.Template, dirittiCampiRuolo);

            if (etichettaMenuATendina.Visible)
            {
                divColDesc.Controls.Add(parDesc);
                divRowDesc.Controls.Add(divColDesc);
                this.PnlTypeDocument.Controls.Add(divRowDesc);
            }


            if (menuATendina.Visible)
            {
                divColValue.Controls.Add(menuATendina);
                divRowValue.Controls.Add(divColValue);
                this.PnlTypeDocument.Controls.Add(divRowValue);
            }
        }


        private int impostaSelezioneMenuATendina(string valore, DropDownList ddl)
        {
            for (int i = 0; i < ddl.Items.Count; i++)
            {
                if (ddl.Items[i].Text == valore)
                    return i;
            }
            return 0;
        }
        private void inserisciSelezioneEsclusiva(DocsPaWR.OggettoCustom oggettoCustom, bool readOnly, int index, ArrayList dirittiCampiRuolo)
        {
            DocsPaWR.StoricoProfilatiOldValue selezEsclOldObj = new StoricoProfilatiOldValue();
            if (string.IsNullOrEmpty(oggettoCustom.DESCRIZIONE))
            {
                return;
            }
            Label etichettaSelezioneEsclusiva = new Label();
            etichettaSelezioneEsclusiva.EnableViewState = true;
            CustomImageButton cancella_selezioneEsclusiva = new CustomImageButton();
            string language = UIManager.UserManager.GetUserLanguage();
            cancella_selezioneEsclusiva.AlternateText = Utils.Languages.GetLabelFromCode("LinkDocFascBtn_Reset", language);
            cancella_selezioneEsclusiva.ToolTip = Utils.Languages.GetLabelFromCode("LinkDocFascBtn_Reset", language);
            cancella_selezioneEsclusiva.EnableViewState = true;


            etichettaSelezioneEsclusiva.Text = oggettoCustom.DESCRIZIONE;


            cancella_selezioneEsclusiva.ID = "_" + oggettoCustom.SYSTEM_ID.ToString();
            cancella_selezioneEsclusiva.ImageUrl = "../Images/Icons/clean_field_custom.png";
            cancella_selezioneEsclusiva.OnMouseOutImage = "../Images/Icons/clean_field_custom.png";
            cancella_selezioneEsclusiva.OnMouseOverImage = "../Images/Icons/clean_field_custom_hover.png";
            cancella_selezioneEsclusiva.ImageUrlDisabled = "../Images/Icons/clean_field_custom_disabled.png";
            cancella_selezioneEsclusiva.CssClass = "clickable";
            cancella_selezioneEsclusiva.Click += cancella_selezioneEsclusiva_Click;
            etichettaSelezioneEsclusiva.CssClass = "weight";

            RadioButtonList selezioneEsclusiva = new RadioButtonList();
            selezioneEsclusiva.EnableViewState = true;
            selezioneEsclusiva.ID = oggettoCustom.SYSTEM_ID.ToString();
            //int valoreDiDefault = -1;
            for (int i = 0; i < oggettoCustom.ELENCO_VALORI.Length; i++)
            {
                DocsPaWR.ValoreOggetto valoreOggetto = ((DocsPaWR.ValoreOggetto)(oggettoCustom.ELENCO_VALORI[i]));
                //Valori disabilitati/abilitati
                if (valoreOggetto.ABILITATO == 1 || (valoreOggetto.ABILITATO == 0 && valoreOggetto.VALORE == oggettoCustom.VALORE_DATABASE))
                {
                    //Nel caso il valore è disabilitato ma selezionato lo rendo disponibile solo fino al salvataggio del documento 
                    if (valoreOggetto.ABILITATO == 0 && valoreOggetto.VALORE == oggettoCustom.VALORE_DATABASE)
                        valoreOggetto.ABILITATO = 1;

                    selezioneEsclusiva.Items.Add(new ListItem(valoreOggetto.VALORE, valoreOggetto.VALORE));
                    //Valore di default
                    //if (valoreOggetto.VALORE_DI_DEFAULT.Equals("SI"))
                    //{
                    //    valoreDiDefault = i;
                    //}
                }
            }

            if (oggettoCustom.ORIZZONTALE_VERTICALE.Equals("Orizzontale"))
            {
                selezioneEsclusiva.RepeatDirection = RepeatDirection.Horizontal;
            }
            else
            {
                selezioneEsclusiva.RepeatDirection = RepeatDirection.Vertical;
            }
            //if (valoreDiDefault != -1)
            //{
            //    selezioneEsclusiva.SelectedIndex = valoreDiDefault;
            //}
            if (!string.IsNullOrEmpty(oggettoCustom.VALORE_DATABASE))
            {
                selezioneEsclusiva.SelectedIndex = impostaSelezioneEsclusiva(oggettoCustom.VALORE_DATABASE, selezioneEsclusiva);
            }

            Panel divRowDesc = new Panel();
            divRowDesc.CssClass = "row";
            divRowDesc.EnableViewState = true;

            Panel divColDesc = new Panel();
            divColDesc.CssClass = "col";
            divColDesc.EnableViewState = true;

            Panel divColImage = new Panel();
            divColImage.CssClass = "col-right-no-margin";
            divColImage.EnableViewState = true;

            divColImage.Controls.Add(cancella_selezioneEsclusiva);

            HtmlGenericControl parDesc = new HtmlGenericControl("p");
            parDesc.Controls.Add(etichettaSelezioneEsclusiva);
            parDesc.EnableViewState = true;

            divColDesc.Controls.Add(parDesc);

            divRowDesc.Controls.Add(divColDesc);
            divRowDesc.Controls.Add(divColImage);


            Panel divRowValue = new Panel();
            divRowValue.CssClass = "row";
            divRowValue.EnableViewState = true;

            Panel divColValue = new Panel();
            divColValue.CssClass = "col_full";
            divColValue.EnableViewState = true;

            divColValue.Controls.Add(selezioneEsclusiva);
            divRowValue.Controls.Add(divColValue);



            //Verifico i diritti del ruolo sul campo
            this.impostaDirittiRuoloSelezioneEsclusiva(etichettaSelezioneEsclusiva, selezioneEsclusiva, cancella_selezioneEsclusiva, oggettoCustom, this.Template, dirittiCampiRuolo);

            if (etichettaSelezioneEsclusiva.Visible)
            {
                this.PnlTypeDocument.Controls.Add(divRowDesc);
            }

            if (selezioneEsclusiva.Visible)
            {
                this.PnlTypeDocument.Controls.Add(divRowValue);
            }
        }


        private int impostaSelezioneEsclusiva(string valore, RadioButtonList rbl)
        {
            for (int i = 0; i < rbl.Items.Count; i++)
            {
                if (rbl.Items[i].Text == valore)
                    return i;
            }
            return 0;
        }

        protected void cancella_selezioneEsclusiva_Click(object sender, EventArgs e)
        {
            string idOggetto = (((CustomImageButton)sender).ID).Substring(1);
            ((RadioButtonList)this.PnlTypeDocument.FindControl(idOggetto)).SelectedIndex = -1;
            ((RadioButtonList)this.PnlTypeDocument.FindControl(idOggetto)).EnableViewState = true;
            for (int i = 0; i < this.Template.ELENCO_OGGETTI.Length; i++)
            {
                if (((DocsPaWR.OggettoCustom)this.Template.ELENCO_OGGETTI[i]).SYSTEM_ID.ToString().Equals(idOggetto))
                {
                    ((DocsPaWR.OggettoCustom)this.Template.ELENCO_OGGETTI[i]).VALORE_DATABASE = "";
                }
            }

        }

        public void impostaDirittiRuoloSelezioneEsclusiva(System.Object etichetta, System.Object campo, System.Object button, DocsPaWR.OggettoCustom oggettoCustom, DocsPaWR.Templates template, ArrayList dirittiCampiRuolo)
        {
            foreach (DocsPaWR.AssDocFascRuoli assDocFascRuoli in dirittiCampiRuolo)
            {
                if (assDocFascRuoli.ID_OGGETTO_CUSTOM == oggettoCustom.SYSTEM_ID.ToString())
                {
                    if ((assDocFascRuoli != null && assDocFascRuoli.INS_MOD_OGG_CUSTOM == "0" && assDocFascRuoli.VIS_OGG_CUSTOM == "1") || template.IN_ESERCIZIO.ToUpper().Equals("NO"))
                    {
                        ((System.Web.UI.WebControls.RadioButtonList)campo).Enabled = false;
                        ((CustomImageButton)button).Visible = false;
                        oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
                    }
                    if (assDocFascRuoli != null && assDocFascRuoli.INS_MOD_OGG_CUSTOM == "0" && assDocFascRuoli.VIS_OGG_CUSTOM == "0")
                    {
                        ((System.Web.UI.WebControls.Label)etichetta).Visible = false;
                        ((System.Web.UI.WebControls.RadioButtonList)campo).Visible = false;
                        ((System.Web.UI.WebControls.ImageButton)button).Visible = false;
                        oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
                    }
                }
            }
        }

        private void inserisciCasellaDiSelezione(DocsPaWR.OggettoCustom oggettoCustom, bool readOnly, int index, ArrayList dirittiCampiRuolo)
        {
            if (string.IsNullOrEmpty(oggettoCustom.DESCRIZIONE))
            {
                return;
            }
            DocsPaWR.StoricoProfilatiOldValue casellaSelOldObj = new StoricoProfilatiOldValue();
            Label etichettaCasellaSelezione = new Label();
            etichettaCasellaSelezione.EnableViewState = true;

            //if (oggettoCustom.CAMPO_OBBLIGATORIO.Equals("SI"))
            //{
            //    etichettaCasellaSelezione.Text = oggettoCustom.DESCRIZIONE + " *";
            //}
            //else
            //{
            etichettaCasellaSelezione.Text = oggettoCustom.DESCRIZIONE;
            //}

            etichettaCasellaSelezione.Width = Unit.Percentage(100);
            etichettaCasellaSelezione.CssClass = "weight";

            CheckBoxList casellaSelezione = new CheckBoxList();
            casellaSelezione.EnableViewState = true;
            casellaSelezione.ID = oggettoCustom.SYSTEM_ID.ToString();
            //int valoreDiDefault = -1;
            for (int i = 0; i < oggettoCustom.ELENCO_VALORI.Length; i++)
            {
                DocsPaWR.ValoreOggetto valoreElenco = ((ValoreOggetto)(oggettoCustom.ELENCO_VALORI[i]));
                if (i < oggettoCustom.VALORI_SELEZIONATI.Length)
                {
                    string valoreSelezionato = (string)(oggettoCustom.VALORI_SELEZIONATI[i]);
                    if (valoreElenco.ABILITATO == 1 || (valoreElenco.ABILITATO == 0 && !string.IsNullOrEmpty(valoreSelezionato)))
                    {
                        //Nel caso il valore è disabilitato ma selezionato lo rendo disponibile solo fino al salvataggio del documento 
                        if (valoreElenco.ABILITATO == 0 && !string.IsNullOrEmpty(valoreSelezionato))
                            valoreElenco.ABILITATO = 1;

                        casellaSelezione.Items.Add(new ListItem(valoreElenco.VALORE, valoreElenco.VALORE));
                        //Valore di default
                        //if (valoreElenco.VALORE_DI_DEFAULT.Equals("SI"))
                        //{
                        //    valoreDiDefault = i;
                        //}
                    }
                }
            }

            if (oggettoCustom.ORIZZONTALE_VERTICALE.Equals("Orizzontale"))
            {
                casellaSelezione.RepeatDirection = RepeatDirection.Horizontal;
            }
            else
            {
                casellaSelezione.RepeatDirection = RepeatDirection.Vertical;
            }
            //if (valoreDiDefault != -1)
            //{
            //    casellaSelezione.SelectedIndex = valoreDiDefault;
            //}

            if (oggettoCustom.VALORI_SELEZIONATI != null)
            {
                this.impostaSelezioneCaselleDiSelezione(oggettoCustom, casellaSelezione);
            }

            Panel divRowDesc = new Panel();
            divRowDesc.CssClass = "row";
            divRowDesc.EnableViewState = true;

            Panel divColDesc = new Panel();
            divColDesc.CssClass = "col";
            divColDesc.EnableViewState = true;

            HtmlGenericControl parDesc = new HtmlGenericControl("p");
            parDesc.Controls.Add(etichettaCasellaSelezione);
            parDesc.EnableViewState = true;



            Panel divRowValue = new Panel();
            divRowValue.CssClass = "row";
            divRowValue.EnableViewState = true;

            Panel divColValue = new Panel();
            divColValue.CssClass = "col_full";
            divColDesc.EnableViewState = true;



            //Verifico i diritti del ruolo sul campo
            this.impostaDirittiRuoloSulCampo(etichettaCasellaSelezione, casellaSelezione, oggettoCustom, this.Template, dirittiCampiRuolo);

            if (etichettaCasellaSelezione.Visible)
            {
                divColDesc.Controls.Add(parDesc);
                divRowDesc.Controls.Add(divColDesc);
                this.PnlTypeDocument.Controls.Add(divRowDesc);
            }

            if (casellaSelezione.Visible)
            {

                divColValue.Controls.Add(casellaSelezione);
                divRowValue.Controls.Add(divColValue);

                this.PnlTypeDocument.Controls.Add(divRowValue);
            }
        }

        private void impostaSelezioneCaselleDiSelezione(DocsPaWR.OggettoCustom objCustom, CheckBoxList cbl)
        {
            for (int i = 0; i < objCustom.VALORI_SELEZIONATI.Length; i++)
            {
                for (int j = 0; j < cbl.Items.Count; j++)
                {
                    if ((string)objCustom.VALORI_SELEZIONATI[i] == cbl.Items[j].Text)
                    {
                        cbl.Items[j].Selected = true;
                    }
                }
            }
        }
        private void inserisciContatore(DocsPaWR.OggettoCustom oggettoCustom, bool readOnly, ArrayList dirittiCampiRuolo)
        {
            bool paneldll = false;

            if (string.IsNullOrEmpty(oggettoCustom.DESCRIZIONE))
            {
                return;
            }

            Label etichettaContatore = new Label();
            etichettaContatore.EnableViewState = true;


            etichettaContatore.Text = oggettoCustom.DESCRIZIONE;

            etichettaContatore.CssClass = "weight";

            CustomTextArea contatoreDa = new CustomTextArea();
            contatoreDa.EnableViewState = true;
            contatoreDa.ID = "da_" + oggettoCustom.SYSTEM_ID.ToString();
            contatoreDa.CssClass = "txt_textdata";

            CustomTextArea contatoreA = new CustomTextArea();
            contatoreA.EnableViewState = true;
            contatoreA.ID = "a_" + oggettoCustom.SYSTEM_ID.ToString();
            contatoreA.CssClass = "txt_textdata";

            Panel divRowDesc = new Panel();
            divRowDesc.CssClass = "row";
            divRowDesc.EnableViewState = true;

            Panel divColDesc = new Panel();
            divColDesc.CssClass = "col";
            divColDesc.EnableViewState = true;

            HtmlGenericControl parDesc = new HtmlGenericControl("p");
            parDesc.Controls.Add(etichettaContatore);
            parDesc.EnableViewState = true;

            divColDesc.Controls.Add(parDesc);
            divRowDesc.Controls.Add(divColDesc);

            //Ruolo ruoloUtente = RoleManager.GetRoleInSession();
            //Registro[] registriRfVisibili = UserManager.getListaRegistriWithRF(ruoloUtente.systemId, string.Empty, string.Empty);
            Registro[] registriRfVisibili = RegistryManager.GetRegAndRFListInSession();
            //Le dropDownLsit delle AOO o RF e la checkbox per il contaDopo vanno considerati e visualizzati
            //solo nel caso di un contatore non valorizzato, altrimenti deve essere riporato solo il valore 
            //del contatore come da formato prescelto e in readOnly
            Label etichettaDDL = new Label();
            etichettaDDL.EnableViewState = true;
            etichettaDDL.Width = 50;
            DropDownList ddl = new DropDownList();
            ddl.EnableViewState = true;

            string language = UIManager.UserManager.GetUserLanguage();
            ddl.Attributes.Add("data-placeholder", Utils.Languages.GetLabelFromCode("SelectProflierMenu", language));

            ddl.Items.Add(new ListItem() { Text = "", Value = "" });

            Panel divRowDll = new Panel();
            divRowDll.CssClass = "row";
            divRowDll.EnableViewState = true;

            Panel divRowEtiDll = new Panel();
            divRowEtiDll.CssClass = "row";
            divRowEtiDll.EnableViewState = true;

            HtmlGenericControl parDll = new HtmlGenericControl("p");
            parDll.EnableViewState = true;

            Panel divColDllEti = new Panel();
            divColDllEti.CssClass = "col";
            divColDllEti.EnableViewState = true;

            Panel divColDll = new Panel();
            divColDll.CssClass = "col";
            divColDll.EnableViewState = true;


            if (!oggettoCustom.VALORE_DATABASE.Equals(""))
            {
                if (oggettoCustom.VALORE_DATABASE.IndexOf("@") != -1)
                {
                    string[] contatori = oggettoCustom.VALORE_DATABASE.Split('@');
                    contatoreDa.Text = contatori[0].ToString();
                    contatoreA.Text = contatori[1].ToString();
                }
                else
                {
                    contatoreDa.Text = oggettoCustom.VALORE_DATABASE.ToString();
                    contatoreA.Text = "";
                }
            }

            switch (oggettoCustom.TIPO_CONTATORE)
            {
                case "T":
                    break;
                case "A":
                    paneldll = true;
                    etichettaDDL.Text = "&nbsp;AOO&nbsp;";
                    etichettaDDL.CssClass = "weight";
                    ddl.ID = oggettoCustom.SYSTEM_ID.ToString() + "_menu";
                    ddl.CssClass = "chzn-select-deselect";
                    ddl.Width = 240;

                    //Distinguo se è un registro o un rf
                    for (int i = 0; i < registriRfVisibili.Length; i++)
                    {
                        ListItem item = new ListItem();
                        if (((Registro)registriRfVisibili[i]).chaRF == "0")
                        {
                            item.Value = ((Registro)registriRfVisibili[i]).systemId;
                            item.Text = ((Registro)registriRfVisibili[i]).codRegistro;
                            ddl.Items.Add(item);
                        }
                    }
                    ddl.SelectedValue = oggettoCustom.ID_AOO_RF;
                    parDll.Controls.Add(etichettaDDL);
                    divColDllEti.Controls.Add(parDll);
                    divRowEtiDll.Controls.Add(divColDllEti);

                    divColDll.Controls.Add(ddl);
                    divRowDll.Controls.Add(divColDll);
                    break;
                case "R":
                    paneldll = true;
                    etichettaDDL.Text = "&nbsp;RF&nbsp;";
                    etichettaDDL.CssClass = "weight";
                    ddl.ID = oggettoCustom.SYSTEM_ID.ToString() + "_menu";
                    ddl.CssClass = "chzn-select-deselect";
                    ddl.Width = 240;

                    //Distinguo se è un registro o un rf
                    for (int i = 0; i < registriRfVisibili.Length; i++)
                    {
                        ListItem item = new ListItem();
                        if (((Registro)registriRfVisibili[i]).chaRF == "1" && ((Registro)registriRfVisibili[i]).rfDisabled == "0")
                        {
                            item.Value = ((Registro)registriRfVisibili[i]).systemId;
                            item.Text = ((Registro)registriRfVisibili[i]).codRegistro;
                            ddl.Items.Add(item);
                        }
                    }
                    ddl.SelectedValue = oggettoCustom.ID_AOO_RF;

                    ddl.CssClass = "chzn-select-deselect";

                    parDll.Controls.Add(etichettaDDL);
                    divColDllEti.Controls.Add(parDll);
                    divRowEtiDll.Controls.Add(divColDllEti);

                    divColDll.Controls.Add(ddl);
                    divRowDll.Controls.Add(divColDll);

                    break;
            }

            if (etichettaContatore.Visible)
            {
                this.PnlTypeDocument.Controls.Add(divRowDesc);
            }

            if (!string.IsNullOrEmpty(oggettoCustom.VALORE_DATABASE))
            {
                if (oggettoCustom.VALORE_DATABASE.IndexOf("@") != -1)
                {
                    string[] contatori = oggettoCustom.VALORE_DATABASE.Split('@');
                    contatoreDa.Text = contatori[0].ToString();
                    contatoreA.Text = contatori[1].ToString();
                }
                else
                {
                    contatoreDa.Text = oggettoCustom.VALORE_DATABASE.ToString();
                    contatoreA.Text = "";
                }
            }

            Label etichettaContatoreDa = new Label();
            etichettaContatoreDa.EnableViewState = true;
            etichettaContatoreDa.Text = "Da";

            //////
            Label etichettaContatoreA = new Label();
            etichettaContatoreA.EnableViewState = true;
            etichettaContatoreA.Text = "A";

            Panel divColValueTo = new Panel();
            divColValueTo.CssClass = "col";
            divColValueTo.EnableViewState = true;

            Panel divRowValueFrom = new Panel();
            divRowValueFrom.CssClass = "row";
            divRowValueFrom.EnableViewState = true;

            Panel divCol1 = new Panel();
            divCol1.CssClass = "col";
            divCol1.EnableViewState = true;

            Panel divCol2 = new Panel();
            divCol2.CssClass = "col";
            divCol2.EnableViewState = true;

            Panel divCol3 = new Panel();
            divCol3.CssClass = "col";
            divCol3.EnableViewState = true;

            Panel divCol4 = new Panel();
            divCol4.CssClass = "col";
            divCol4.EnableViewState = true;

            divCol1.Controls.Add(etichettaContatoreDa);
            divCol2.Controls.Add(contatoreDa);
            divCol3.Controls.Add(etichettaContatoreA);
            divCol4.Controls.Add(contatoreA);
            divRowValueFrom.Controls.Add(divCol1);
            divRowValueFrom.Controls.Add(divCol2);
            divRowValueFrom.Controls.Add(divCol3);
            divRowValueFrom.Controls.Add(divCol4);

            impostaDirittiRuoloContatore(etichettaContatore, contatoreDa, contatoreA, etichettaContatoreDa, etichettaContatoreA, etichettaDDL, ddl, oggettoCustom, this.Template, dirittiCampiRuolo);

            if (paneldll)
            {
                this.PnlTypeDocument.Controls.Add(divRowEtiDll);
                this.PnlTypeDocument.Controls.Add(divRowDll);
            }

            if (contatoreDa.Visible)
            {
                this.PnlTypeDocument.Controls.Add(divRowValueFrom);
            }
        }

        public void impostaDirittiRuoloContatore(object etichettaContatore, object contatoreDa, object contatoreA, object etichettaContatoreDa, object etichettaContatoreA, object etichettaDDL, object ddl, DocsPaWR.OggettoCustom oggettoCustom, DocsPaWR.Templates template, ArrayList dirittiCampiRuolo)
        {
            foreach (DocsPaWR.AssDocFascRuoli assDocFascRuoli in dirittiCampiRuolo)
            {
                if (assDocFascRuoli.ID_OGGETTO_CUSTOM == oggettoCustom.SYSTEM_ID.ToString())
                {
                    if (assDocFascRuoli != null && assDocFascRuoli.VIS_OGG_CUSTOM == "0")
                    {
                        ((System.Web.UI.WebControls.Label)etichettaContatore).Visible = false;

                        ((System.Web.UI.WebControls.Label)etichettaContatoreDa).Visible = false;
                        ((CustomTextArea)contatoreDa).Visible = false;
                        ((System.Web.UI.WebControls.Label)etichettaContatoreA).Visible = false;
                        ((CustomTextArea)contatoreA).Visible = false;

                        ((System.Web.UI.WebControls.Label)etichettaDDL).Visible = false;
                        ((System.Web.UI.WebControls.DropDownList)ddl).Visible = false;
                    }
                }
            }
        }

        private void inserisciContatoreSottocontatore(DocsPaWR.OggettoCustom oggettoCustom, bool readOnly, ArrayList dirittiCampiRuolo)
        {
            bool paneldll = false;

            if (string.IsNullOrEmpty(oggettoCustom.DESCRIZIONE))
                return;

            Label etichettaContatoreSottocontatore = new Label();
            etichettaContatoreSottocontatore.EnableViewState = true;
            etichettaContatoreSottocontatore.Text = oggettoCustom.DESCRIZIONE;
            etichettaContatoreSottocontatore.CssClass = "weight";

            Panel divRowDesc = new Panel();
            divRowDesc.CssClass = "row";
            divRowDesc.EnableViewState = true;

            Panel divColDesc = new Panel();
            divColDesc.CssClass = "col";
            divColDesc.EnableViewState = true;

            HtmlGenericControl parDesc = new HtmlGenericControl("p");
            parDesc.Controls.Add(etichettaContatoreSottocontatore);
            parDesc.EnableViewState = true;

            divColDesc.Controls.Add(parDesc);
            divRowDesc.Controls.Add(divColDesc);

            //Ricerca contatore
            TextBox contatoreDa = new TextBox();
            contatoreDa.ID = "da_" + oggettoCustom.SYSTEM_ID.ToString();
            contatoreDa.Text = oggettoCustom.VALORE_DATABASE;
            contatoreDa.Width = 40;
            contatoreDa.CssClass = "comp_profilazione_anteprima";

            TextBox contatoreA = new TextBox();
            contatoreA.ID = "a_" + oggettoCustom.SYSTEM_ID.ToString();
            contatoreA.Text = oggettoCustom.VALORE_DATABASE;
            contatoreA.Width = 40;
            contatoreA.CssClass = "comp_profilazione_anteprima";

            TextBox sottocontatoreDa = new TextBox();
            sottocontatoreDa.ID = "da_sottocontatore_" + oggettoCustom.SYSTEM_ID.ToString();
            sottocontatoreDa.Text = oggettoCustom.VALORE_SOTTOCONTATORE;
            sottocontatoreDa.Width = 40;
            sottocontatoreDa.CssClass = "comp_profilazione_anteprima";

            TextBox sottocontatoreA = new TextBox();
            sottocontatoreA.ID = "a_sottocontatore_" + oggettoCustom.SYSTEM_ID.ToString();
            sottocontatoreA.Text = oggettoCustom.VALORE_SOTTOCONTATORE;
            sottocontatoreA.Width = 40;
            sottocontatoreA.CssClass = "comp_profilazione_anteprima";

            //UserControls.Calendar dataSottocontatoreDa = (UserControls.Calendar)this.LoadControl("../UserControls/Calendar.ascx");
            //dataSottocontatoreDa.fromUrl = "../UserControls/DialogCalendar.aspx";
            //dataSottocontatoreDa.CSS = "testo_grigio";
            //dataSottocontatoreDa.ID = "da_dataSottocontatore_" + oggettoCustom.SYSTEM_ID.ToString();
            //dataSottocontatoreDa.VisibleTimeMode = ProfilerDocManager.getVisibleTimeMode(oggettoCustom);

            //UserControls.Calendar dataSottocontatoreA = (UserControls.Calendar)this.LoadControl("../UserControls/Calendar.ascx");
            //dataSottocontatoreA.fromUrl = "../UserControls/DialogCalendar.aspx";
            //dataSottocontatoreA.CSS = "testo_grigio";
            //dataSottocontatoreA.ID = "a_dataSottocontatore_" + oggettoCustom.SYSTEM_ID.ToString();
            //dataSottocontatoreA.VisibleTimeMode = ProfilazioneDocManager.getVisibleTimeMode(oggettoCustom);

            if (!oggettoCustom.VALORE_DATABASE.Equals(""))
            {
                if (oggettoCustom.VALORE_DATABASE.IndexOf("@") != -1)
                {
                    string[] contatori = oggettoCustom.VALORE_DATABASE.Split('@');
                    contatoreDa.Text = contatori[0].ToString();
                    contatoreA.Text = contatori[1].ToString();
                }
                else
                {
                    contatoreDa.Text = oggettoCustom.VALORE_DATABASE.ToString();
                    contatoreA.Text = "";
                }
            }

            if (!oggettoCustom.VALORE_SOTTOCONTATORE.Equals(""))
            {
                if (oggettoCustom.VALORE_SOTTOCONTATORE.IndexOf("@") != -1)
                {
                    string[] contatori = oggettoCustom.VALORE_SOTTOCONTATORE.Split('@');
                    sottocontatoreDa.Text = contatori[0].ToString();
                    sottocontatoreA.Text = contatori[1].ToString();
                }
                else
                {
                    sottocontatoreDa.Text = oggettoCustom.VALORE_SOTTOCONTATORE.ToString();
                    sottocontatoreA.Text = "";
                }
            }

            //if (!oggettoCustom.DATA_INSERIMENTO.Equals(""))
            //{
            //    if (oggettoCustom.DATA_INSERIMENTO.IndexOf("@") != -1)
            //    {
            //        string[] date = oggettoCustom.DATA_INSERIMENTO.Split('@');
            //        dataSottocontatoreDa.Text = date[0].ToString();
            //        dataSottocontatoreA.Text = date[1].ToString();
            //    }
            //    else
            //    {
            //        dataSottocontatoreDa.Text = oggettoCustom.DATA_INSERIMENTO.ToString();
            //        dataSottocontatoreA.Text = "";
            //    }
            //}

            Label etichettaContatoreDa = new Label();
            etichettaContatoreDa.Text = "<br/>&nbsp;&nbsp;da&nbsp;";
            etichettaContatoreDa.Font.Size = FontUnit.Point(8);
            etichettaContatoreDa.Font.Bold = true;
            etichettaContatoreDa.Font.Name = "Verdana";
            Label etichettaContatoreA = new Label();
            etichettaContatoreA.Text = "&nbsp;a&nbsp;";
            etichettaContatoreA.Font.Size = FontUnit.Point(8);
            etichettaContatoreA.Font.Bold = true;
            etichettaContatoreA.Font.Name = "Verdana";

            Label etichettaSottocontatoreDa = new Label();
            etichettaSottocontatoreDa.Text = "<br/>&nbsp;&nbsp;da&nbsp;";
            etichettaSottocontatoreDa.Font.Size = FontUnit.Point(8);
            etichettaSottocontatoreDa.Font.Bold = true;
            etichettaSottocontatoreDa.Font.Name = "Verdana";
            Label etichettaSottocontatoreA = new Label();
            etichettaSottocontatoreA.Text = "&nbsp;a&nbsp;";
            etichettaSottocontatoreA.Font.Size = FontUnit.Point(8);
            etichettaSottocontatoreA.Font.Bold = true;
            etichettaSottocontatoreA.Font.Name = "Verdana";

            Label etichettaDataSottocontatoreDa = new Label();
            etichettaDataSottocontatoreDa.Text = "<br/>&nbsp;&nbsp;da&nbsp;";
            etichettaDataSottocontatoreDa.Font.Size = FontUnit.Point(8);
            etichettaDataSottocontatoreDa.Font.Bold = true;
            etichettaDataSottocontatoreDa.Font.Name = "Verdana";
            Label etichettaDataSottocontatoreA = new Label();
            etichettaDataSottocontatoreA.Text = "&nbsp;a&nbsp;";
            etichettaDataSottocontatoreA.Font.Size = FontUnit.Point(8);
            etichettaDataSottocontatoreA.Font.Bold = true;
            etichettaDataSottocontatoreA.Font.Name = "Verdana";

            //TableRow row = new TableRow();
            //TableCell cell_1 = new TableCell();
            //cell_1.Controls.Add(etichettaContatoreSottocontatore);
            //row.Cells.Add(cell_1);

            //TableCell cell_2 = new TableCell();
            //


            //Le dropDownLsit delle AOO o RF e la checkbox per il contaDopo vanno considerati e visualizzati
            //solo nel caso di un contatore non valorizzato, altrimenti deve essere riporato solo il valore 
            //del contatore come da formato prescelto e in readOnly
            Label etichettaDDL = new Label();
            etichettaDDL.EnableViewState = true;
            DropDownList ddl = new DropDownList();
            ddl.EnableViewState = true;

            string language = UIManager.UserManager.GetUserLanguage();
            ddl.Attributes.Add("data-placeholder", Utils.Languages.GetLabelFromCode("SelectProflierMenu", language));

            Panel divRowDll = new Panel();
            divRowDll.CssClass = "row";
            divRowDll.EnableViewState = true;

            Panel divRowEtiDll = new Panel();
            divRowEtiDll.CssClass = "row";
            divRowEtiDll.EnableViewState = true;

            HtmlGenericControl parDll = new HtmlGenericControl("p");
            parDll.EnableViewState = true;

            if (string.IsNullOrEmpty(oggettoCustom.VALORE_DATABASE))
            {
                if (oggettoCustom.VALORE_DATABASE.IndexOf("@") != -1)
                {
                    string[] contatori = oggettoCustom.VALORE_DATABASE.Split('@');
                    contatoreDa.Text = contatori[0].ToString();
                    contatoreA.Text = contatori[1].ToString();
                }
                else
                {
                    contatoreDa.Text = oggettoCustom.VALORE_DATABASE.ToString();
                    contatoreA.Text = "";
                }

                Ruolo ruoloUtente = RoleManager.GetRoleInSession();
                Registro[] registriRfVisibili = UIManager.RegistryManager.GetListRegistriesAndRF(ruoloUtente.systemId, string.Empty, string.Empty);

                Panel divColDllEti = new Panel();
                divColDllEti.CssClass = "col";
                divColDllEti.EnableViewState = true;

                Panel divColDll = new Panel();
                divColDll.CssClass = "col";
                divColDll.EnableViewState = true;

                switch (oggettoCustom.TIPO_CONTATORE)
                {
                    case "T":
                        break;
                    case "A":
                        paneldll = true;
                        etichettaDDL.Text = "&nbsp;AOO&nbsp;";
                        etichettaDDL.CssClass = "weight";
                        etichettaDDL.Width = 50;
                        ddl.ID = oggettoCustom.SYSTEM_ID.ToString() + "_menu";
                        ddl.CssClass = "chzn-select-deselect";
                        ddl.Width = 240;

                        //Distinguo se è un registro o un rf
                        for (int i = 0; i < registriRfVisibili.Length; i++)
                        {
                            ListItem item = new ListItem();
                            if (((Registro)registriRfVisibili[i]).chaRF == "0" && !((Registro)registriRfVisibili[i]).Sospeso)
                            {
                                item.Value = ((Registro)registriRfVisibili[i]).systemId;
                                item.Text = ((Registro)registriRfVisibili[i]).codRegistro;
                                ddl.Items.Add(item);
                            }
                        }
                        if (ddl.Items.Count > 1)
                        {
                            ListItem it = new ListItem();
                            it.Text = string.Empty;
                            it.Value = string.Empty;
                            ddl.Items.Add(it);
                            ddl.SelectedValue = string.Empty;
                        }
                        else
                            ddl.SelectedValue = oggettoCustom.ID_AOO_RF;

                        parDll.Controls.Add(etichettaDDL);
                        divColDllEti.Controls.Add(parDll);
                        divRowEtiDll.Controls.Add(divColDllEti);

                        divColDll.Controls.Add(ddl);
                        divRowDll.Controls.Add(divColDll);
                        break;
                    case "R":
                        paneldll = true;
                        etichettaDDL.Text = "&nbsp;RF&nbsp;";
                        etichettaDDL.CssClass = "weight";
                        etichettaDDL.Width = 50;
                        ddl.ID = oggettoCustom.SYSTEM_ID.ToString() + "_menu";
                        ddl.CssClass = "chzn-select-deselect";
                        ddl.Width = 240;

                        //Distinguo se è un registro o un rf
                        for (int i = 0; i < registriRfVisibili.Length; i++)
                        {
                            ListItem item = new ListItem();
                            if (((Registro)registriRfVisibili[i]).chaRF == "1" && ((Registro)registriRfVisibili[i]).rfDisabled == "0")
                            {
                                item.Value = ((Registro)registriRfVisibili[i]).systemId;
                                item.Text = ((Registro)registriRfVisibili[i]).codRegistro;
                                ddl.Items.Add(item);
                            }
                        }
                        if (ddl.Items.Count > 1)
                        {
                            ListItem it = new ListItem();
                            it.Value = string.Empty;
                            it.Text = string.Empty;
                            ddl.Items.Add(it);
                            ddl.SelectedValue = string.Empty;
                        }
                        else
                        {
                            ddl.SelectedValue = oggettoCustom.ID_AOO_RF;
                        }

                        parDll.Controls.Add(etichettaDDL);
                        divColDllEti.Controls.Add(parDll);
                        divRowEtiDll.Controls.Add(divColDllEti);

                        divColDll.Controls.Add(ddl);
                        divRowDll.Controls.Add(divColDll);
                        break;
                }
            }

            if (!oggettoCustom.VALORE_SOTTOCONTATORE.Equals(""))
            {
                if (oggettoCustom.VALORE_SOTTOCONTATORE.IndexOf("@") != -1)
                {
                    string[] contatori = oggettoCustom.VALORE_SOTTOCONTATORE.Split('@');
                    sottocontatoreDa.Text = contatori[0].ToString();
                    sottocontatoreA.Text = contatori[1].ToString();
                }
                else
                {
                    sottocontatoreDa.Text = oggettoCustom.VALORE_SOTTOCONTATORE.ToString();
                    sottocontatoreA.Text = "";
                }
            }

            //if (!oggettoCustom.DATA_INSERIMENTO.Equals(""))
            //{
            //    if (oggettoCustom.DATA_INSERIMENTO.IndexOf("@") != -1)
            //    {
            //        string[] date = oggettoCustom.DATA_INSERIMENTO.Split('@');
            //        dataSottocontatoreDa.Text = date[0].ToString();
            //        dataSottocontatoreA.Text = date[1].ToString();
            //    }
            //    else
            //    {
            //        dataSottocontatoreDa.Text = oggettoCustom.DATA_INSERIMENTO.ToString();
            //        dataSottocontatoreA.Text = "";
            //    }
            //}

            ////Imposto il contatore in funzione del formato
            //CustomTextArea contatore = new CustomTextArea();
            //CustomTextArea sottocontatore = new CustomTextArea();
            //CustomTextArea dataInserimentoSottocontatore = new CustomTextArea();
            //contatore.EnableViewState = true;
            //sottocontatore.EnableViewState = true;
            //dataInserimentoSottocontatore.EnableViewState = true;

            //contatore.ID = oggettoCustom.SYSTEM_ID.ToString();
            //sottocontatore.ID = oggettoCustom.SYSTEM_ID.ToString() + "_sottocontatore";
            //dataInserimentoSottocontatore.ID = oggettoCustom.SYSTEM_ID.ToString() + "_dataSottocontatore";
            //if (!string.IsNullOrEmpty(oggettoCustom.FORMATO_CONTATORE))
            //{
            //    contatore.Text = oggettoCustom.FORMATO_CONTATORE;
            //    sottocontatore.Text = oggettoCustom.FORMATO_CONTATORE;

            //    if (!string.IsNullOrEmpty(oggettoCustom.VALORE_DATABASE) && !string.IsNullOrEmpty(oggettoCustom.VALORE_SOTTOCONTATORE))
            //    {
            //        contatore.Text = contatore.Text.Replace("ANNO", oggettoCustom.ANNO);
            //        contatore.Text = contatore.Text.Replace("CONTATORE", oggettoCustom.VALORE_DATABASE);

            //        sottocontatore.Text = sottocontatore.Text.Replace("ANNO", oggettoCustom.ANNO);
            //        sottocontatore.Text = sottocontatore.Text.Replace("CONTATORE", oggettoCustom.VALORE_SOTTOCONTATORE);

            //        if (!string.IsNullOrEmpty(oggettoCustom.ID_AOO_RF) && oggettoCustom.ID_AOO_RF != "0")
            //        {
            //            Registro reg = UserManager.getRegistroBySistemId(this, oggettoCustom.ID_AOO_RF);
            //            if (reg != null)
            //            {
            //                contatore.Text = contatore.Text.Replace("RF", reg.codRegistro);
            //                contatore.Text = contatore.Text.Replace("AOO", reg.codRegistro);

            //                sottocontatore.Text = sottocontatore.Text.Replace("RF", reg.codRegistro);
            //                sottocontatore.Text = sottocontatore.Text.Replace("AOO", reg.codRegistro);
            //            }
            //        }
            //    }
            //    else
            //    {
            //        contatore.Text = contatore.Text.Replace("ANNO", "");
            //        contatore.Text = contatore.Text.Replace("CONTATORE", "");
            //        contatore.Text = contatore.Text.Replace("RF", "");
            //        contatore.Text = contatore.Text.Replace("AOO", "");

            //        sottocontatore.Text = sottocontatore.Text.Replace("ANNO", "");
            //        sottocontatore.Text = sottocontatore.Text.Replace("CONTATORE", "");
            //        sottocontatore.Text = sottocontatore.Text.Replace("RF", "");
            //        sottocontatore.Text = sottocontatore.Text.Replace("AOO", "");
            //    }
            //    //}
            //}
            //else
            //{
            //    contatore.Text = oggettoCustom.VALORE_DATABASE;
            //    sottocontatore.Text = oggettoCustom.VALORE_SOTTOCONTATORE;
            //}

            //Panel divRowCounter = new Panel();
            //divRowCounter.CssClass = "row";
            //divRowCounter.EnableViewState = true;

            //Panel divColCountCounter = new Panel();
            //divColCountCounter.CssClass = "col_full";
            //divColCountCounter.EnableViewState = true;
            //divColCountCounter.Controls.Add(contatore);
            //divColCountCounter.Controls.Add(sottocontatore);
            //divRowCounter.Controls.Add(divColCountCounter);

            //if (!string.IsNullOrEmpty(oggettoCustom.DATA_INSERIMENTO))
            //{
            //    dataInserimentoSottocontatore.Text = oggettoCustom.DATA_INSERIMENTO;

            //    Panel divColCountAbort = new Panel();
            //    divColCountAbort.CssClass = "col";
            //    divColCountAbort.EnableViewState = true;
            //    divColCountAbort.Controls.Add(dataInserimentoSottocontatore);
            //    divRowCounter.Controls.Add(divColCountAbort);
            //}

            //CheckBox cbContaDopo = new CheckBox();
            //cbContaDopo.EnableViewState = true;

            ////Verifico i diritti del ruolo sul campo
            //this.impostaDirittiRuoloContatoreSottocontatore(etichettaContatoreSottocontatore, contatore, sottocontatore, dataInserimentoSottocontatore, cbContaDopo, etichettaDDL, ddl, oggettoCustom, this.Template, dirittiCampiRuolo);

            //if (etichettaContatoreSottocontatore.Visible)
            //{
            //    this.PnlTypeDocument.Controls.Add(divRowDesc);
            //}

            //contatore.ReadOnly = true;
            //contatore.CssClass = "txt_input_half";
            //contatore.CssClassReadOnly = "txt_input_half_disabled";

            //sottocontatore.ReadOnly = true;
            //sottocontatore.CssClass = "txt_input_half";
            //sottocontatore.CssClassReadOnly = "txt_input_half_disabled";

            //dataInserimentoSottocontatore.ReadOnly = true;
            //dataInserimentoSottocontatore.CssClass = "txt_input_full";
            //dataInserimentoSottocontatore.CssClassReadOnly = "txt_input_full_disabled";
            //dataInserimentoSottocontatore.Visible = false;


            ////Inserisco il cb per il conta dopo
            //if (oggettoCustom.CONTA_DOPO == "1")
            //{
            //    cbContaDopo.ID = oggettoCustom.SYSTEM_ID.ToString() + "_contaDopo";
            //    cbContaDopo.Checked = oggettoCustom.CONTATORE_DA_FAR_SCATTARE;
            //    cbContaDopo.ToolTip = "Attiva / Disattiva incremento del contatore al salvataggio dei dati.";

            //    Panel divColCountAfter = new Panel();
            //    divColCountAfter.CssClass = "col";
            //    divColCountAfter.EnableViewState = true;
            //    divColCountAfter.Controls.Add(cbContaDopo);
            //    divRowDll.Controls.Add(divColCountAfter);
            //}

            //if (paneldll)
            //{
            //    this.PnlTypeDocument.Controls.Add(divRowEtiDll);
            //    this.PnlTypeDocument.Controls.Add(divRowDll);
            //}

            //if (contatore.Visible || sottocontatore.Visible)
            //{
            //    this.PnlTypeDocument.Controls.Add(divRowCounter);
            //}
        }

        private void inserisciCampoSeparatore(DocsPaWR.OggettoCustom oggettoCustom)
        {
            if (string.IsNullOrEmpty(oggettoCustom.DESCRIZIONE))
            {
                return;
            }

            Label etichettaCampoSeparatore = new Label();
            etichettaCampoSeparatore.CssClass = "weight";
            etichettaCampoSeparatore.EnableViewState = true;
            etichettaCampoSeparatore.Text = oggettoCustom.DESCRIZIONE.ToUpper();

            Panel divRowDesc = new Panel();
            divRowDesc.CssClass = "row";
            divRowDesc.EnableViewState = true;

            Panel divColDesc = new Panel();
            divColDesc.CssClass = "col_full_line";
            divColDesc.EnableViewState = true;

            HtmlGenericControl parDesc = new HtmlGenericControl("p");
            parDesc.Controls.Add(etichettaCampoSeparatore);
            parDesc.EnableViewState = true;

            divColDesc.Controls.Add(parDesc);
            divRowDesc.Controls.Add(divColDesc);

            if (etichettaCampoSeparatore.Visible)
            {
                this.PnlTypeDocument.Controls.Add(divRowDesc);
            }


        }

        private void inserisciCorrispondente(DocsPaWR.OggettoCustom oggettoCustom, bool readOnly, int index, ArrayList dirittiCampiRuolo)
        {
            if (string.IsNullOrEmpty(oggettoCustom.DESCRIZIONE))
            {
                return;
            }
            DocsPaWR.StoricoProfilatiOldValue corrOldOb = new StoricoProfilatiOldValue();

            UserControls.CorrespondentCustom corrispondente = (UserControls.CorrespondentCustom)this.LoadControl("../UserControls/CorrespondentCustom.ascx");
            corrispondente.EnableViewState = true;
            corrispondente.PageCaller = "Popup";

            corrispondente.TxtEtiCustomCorrespondent = oggettoCustom.DESCRIZIONE;

            corrispondente.TypeCorrespondentCustom = oggettoCustom.TIPO_RICERCA_CORR;
            corrispondente.ID = oggettoCustom.SYSTEM_ID.ToString();

            //Da amministrazione è stato impostato un ruolo di default per questo campo.
            if (!string.IsNullOrEmpty(oggettoCustom.ID_RUOLO_DEFAULT) && oggettoCustom.ID_RUOLO_DEFAULT != "0")
            {
                DocsPaWR.Ruolo ruolo = RoleManager.getRuoloById(oggettoCustom.ID_RUOLO_DEFAULT);
                if (ruolo != null)
                {
                    corrispondente.IdCorrespondentCustom = ruolo.systemId;
                    corrispondente.TxtCodeCorrespondentCustom = ruolo.codiceRubrica;
                    corrispondente.TxtDescriptionCorrespondentCustom = ruolo.descrizione;
                }
                oggettoCustom.ID_RUOLO_DEFAULT = "0";
            }

            //Il campo è valorizzato.
            if (!string.IsNullOrEmpty(oggettoCustom.VALORE_DATABASE))
            {
                DocsPaWR.Corrispondente corr_1 = AddressBookManager.getCorrispondenteBySystemIDDisabled(oggettoCustom.VALORE_DATABASE);
                if (corr_1 != null)
                {
                    corrispondente.IdCorrespondentCustom = corr_1.systemId;
                    corrispondente.TxtCodeCorrespondentCustom = corr_1.codiceRubrica.ToString();
                    corrispondente.TxtDescriptionCorrespondentCustom = corr_1.descrizione.ToString();
                    oggettoCustom.VALORE_DATABASE = corr_1.systemId;
                }
            }

            //Verifico i diritti del ruolo sul campo
            this.impostaDirittiRuoloSulCampo(corrispondente.TxtEtiCustomCorrespondent, corrispondente, oggettoCustom, this.Template, dirittiCampiRuolo);

            if (corrispondente.Visible)
            {
                this.PnlTypeDocument.Controls.Add(corrispondente);
            }

        }


        public void impostaDirittiRuoloSulCampo(System.Object etichetta, System.Object campo, DocsPaWR.OggettoCustom oggettoCustom, DocsPaWR.Templates template, ArrayList dirittiCampiRuolo)
        {
            foreach (DocsPaWR.AssDocFascRuoli assDocFascRuoli in dirittiCampiRuolo)
            {
                if (assDocFascRuoli.ID_OGGETTO_CUSTOM == oggettoCustom.SYSTEM_ID.ToString())
                {
                    switch (oggettoCustom.TIPO.DESCRIZIONE_TIPO)
                    {
                        case "CampoDiTesto":
                            if (assDocFascRuoli != null && assDocFascRuoli.VIS_OGG_CUSTOM == "0")
                            {
                                ((System.Web.UI.WebControls.Label)etichetta).Visible = false;
                                ((CustomTextArea)campo).Visible = false;
                                oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
                            }
                            break;
                        case "CasellaDiSelezione":
                            if (assDocFascRuoli != null && assDocFascRuoli.VIS_OGG_CUSTOM == "0")
                            {
                                ((System.Web.UI.WebControls.Label)etichetta).Visible = false;
                                ((System.Web.UI.WebControls.CheckBoxList)campo).Visible = false;
                                oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
                            }
                            break;
                        case "MenuATendina":
                            if (assDocFascRuoli != null && assDocFascRuoli.VIS_OGG_CUSTOM == "0")
                            {
                                ((System.Web.UI.WebControls.Label)etichetta).Visible = false;
                                ((System.Web.UI.WebControls.DropDownList)campo).Visible = false;
                                oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
                            }
                            break;
                        case "SelezioneEsclusiva":
                            //Per la selezione esclusiva è stato implementato un metodo a parte perchè gli oggetti in uso sono più di due
                            break;
                        case "Contatore":
                            //Per il contatore è stato implementato un metodo a parte perchè gli oggetti in uso sono più di due
                            break;
                        case "Data":
                            if (assDocFascRuoli != null && assDocFascRuoli.VIS_OGG_CUSTOM == "0")
                            {
                                ((System.Web.UI.WebControls.Label)etichetta).Visible = false;
                                ((UserControls.Calendar)campo).Visible = false;
                                ((UserControls.Calendar)campo).VisibleTimeMode = UserControls.Calendar.VisibleTimeModeEnum.Nothing;
                                oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
                            }
                            break;
                        case "Corrispondente":
                            if (assDocFascRuoli != null && assDocFascRuoli.VIS_OGG_CUSTOM == "0")
                            {
                                ((UserControls.CorrespondentCustom)campo).Visible = false;
                                oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
                            }
                            break;
                        case "Link":
                            if (assDocFascRuoli != null && assDocFascRuoli.VIS_OGG_CUSTOM == "0")
                            {
                                ((System.Web.UI.WebControls.Label)etichetta).Visible = false;
                                ((UserControls.LinkDocFasc)campo).Visible = false;
                                oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
                            }
                            break;
                        case "OggettoEsterno":
                            if (assDocFascRuoli != null && assDocFascRuoli.VIS_OGG_CUSTOM == "0")
                            {
                                ((System.Web.UI.WebControls.Label)etichetta).Visible = false;
                                ((UserControls.IntegrationAdapter)campo).Visible = false;
                                oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
                            }
                            break;
                    }
                }
            }
        }


        private void inserisciCampoDiTesto(DocsPaWR.OggettoCustom oggettoCustom, bool readOnly, int index, ArrayList dirittiCampiRuolo)
        {
            if (string.IsNullOrEmpty(oggettoCustom.DESCRIZIONE))
            {
                return;
            }

            Label etichettaCampoDiTesto = new Label();
            etichettaCampoDiTesto.EnableViewState = true;

            CustomTextArea txt_CampoDiTesto = new CustomTextArea();
            txt_CampoDiTesto.EnableViewState = true;

            if (oggettoCustom.MULTILINEA.Equals("SI"))
            {

                etichettaCampoDiTesto.Text = oggettoCustom.DESCRIZIONE;

                etichettaCampoDiTesto.CssClass = "weight";

                txt_CampoDiTesto.CssClass = "txt_textarea";
                txt_CampoDiTesto.CssClassReadOnly = "txt_textarea_disabled";

                if (string.IsNullOrEmpty(oggettoCustom.NUMERO_DI_LINEE))
                {
                    txt_CampoDiTesto.Height = 55;
                }
                else
                {
                    txt_CampoDiTesto.Rows = Convert.ToInt32(oggettoCustom.NUMERO_DI_LINEE);
                }

                if (string.IsNullOrEmpty(oggettoCustom.NUMERO_DI_CARATTERI))
                {
                    txt_CampoDiTesto.MaxLength = 150;
                }
                else
                {
                    txt_CampoDiTesto.MaxLength = Convert.ToInt32(oggettoCustom.NUMERO_DI_CARATTERI);
                }

                txt_CampoDiTesto.ID = oggettoCustom.SYSTEM_ID.ToString();
                txt_CampoDiTesto.Text = oggettoCustom.VALORE_DATABASE;
                txt_CampoDiTesto.TextMode = TextBoxMode.MultiLine;
            }
            else
            {

                etichettaCampoDiTesto.Text = oggettoCustom.DESCRIZIONE;

                etichettaCampoDiTesto.CssClass = "weight";

                if (!string.IsNullOrEmpty(oggettoCustom.NUMERO_DI_CARATTERI))
                {
                    //ATTENZIONE : La lunghezza della textBox non è speculare al numero massimo di
                    //caratteri che l'utente inserisce.
                    if (((Convert.ToInt32(oggettoCustom.NUMERO_DI_CARATTERI) * 6) <= 400))
                    {
                        txt_CampoDiTesto.Width = Convert.ToInt32(oggettoCustom.NUMERO_DI_CARATTERI) * 6;
                    }
                    txt_CampoDiTesto.MaxLength = Convert.ToInt32(oggettoCustom.NUMERO_DI_CARATTERI);
                }
                txt_CampoDiTesto.ID = oggettoCustom.SYSTEM_ID.ToString();
                txt_CampoDiTesto.Text = oggettoCustom.VALORE_DATABASE;
                txt_CampoDiTesto.CssClass = "txt_input_full";
                txt_CampoDiTesto.CssClassReadOnly = "txt_input_full_disabled";
                txt_CampoDiTesto.TextMode = TextBoxMode.SingleLine;


            }

            Panel divRowDesc = new Panel();
            divRowDesc.CssClass = "row";
            divRowDesc.EnableViewState = true;

            Panel divColDesc = new Panel();
            divColDesc.CssClass = "col";
            divColDesc.EnableViewState = true;

            Panel divRowValue = new Panel();
            divRowValue.CssClass = "row";
            divRowValue.EnableViewState = true;

            Panel divColValue = new Panel();
            divColValue.CssClass = "col_full";
            divColValue.EnableViewState = true;


            //Verifico i diritti del ruolo sul campo
            this.impostaDirittiRuoloSulCampo(etichettaCampoDiTesto, txt_CampoDiTesto, oggettoCustom, this.Template, dirittiCampiRuolo);

            if (etichettaCampoDiTesto.Visible)
            {
                HtmlGenericControl parDesc = new HtmlGenericControl("p");
                parDesc.Controls.Add(etichettaCampoDiTesto);
                parDesc.EnableViewState = true;
                divColDesc.Controls.Add(parDesc);
                divRowDesc.Controls.Add(divColDesc);
                this.PnlTypeDocument.Controls.Add(divRowDesc);
            }

            if (txt_CampoDiTesto.Visible)
            {
                divColValue.Controls.Add(txt_CampoDiTesto);
                divRowValue.Controls.Add(divColValue);
                this.PnlTypeDocument.Controls.Add(divRowValue);
            }
        }


        private int AjaxAddressBookMinPrefixLenght
        {
            get
            {
                int result = 3;
                if (HttpContext.Current.Session["ajaxAddressBookMinPrefixLenght"] != null)
                {
                    result = int.Parse(HttpContext.Current.Session["ajaxAddressBookMinPrefixLenght"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["ajaxAddressBookMinPrefixLenght"] = value;
            }
        }

        protected void DdlRegistries_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListItem itemSelect = (sender as DropDownList).SelectedItem;
            Registro RegSel = (from reg in RoleManager.GetRoleInSession().registri
                               where reg.systemId.Equals(itemSelect.Value) &&
                                   reg.codRegistro.Equals(itemSelect.Text.Trim())
                               select reg).FirstOrDefault();
            UIManager.RegistryManager.SetRegistryInSession(RegSel);
            this.Registry = RegSel;
            this.Project = null;
            this.ProjectClass = null;
            this.IdProject.Value = string.Empty;
            this.TxtCodeProject.Text = string.Empty;
            this.TxtDescriptionProject.Text = string.Empty;
            this.UpPnlProject.Update();
        }

        public string SelectedUniqueIdCustom
        {
            get
            {
                string result = string.Empty;
                if (HttpContext.Current.Session["UniqueIdCustom"] != null)
                {
                    result = HttpContext.Current.Session["UniqueIdCustom"].ToString();
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["UniqueIdCustom"] = value;
            }
        }

        private void PopulateDDLTitolario()
        {
            this.ddlTitolario.Items.Clear();
            ArrayList listaTitolari = ClassificationSchemeManager.getTitolariUtilizzabili();

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
                            this.ddlTitolario.Items.Add(it);
                            valueTutti += titolario.ID + ",";
                            break;
                        case DocsPaWR.OrgStatiTitolarioEnum.Chiuso:
                            it = new ListItem(titolario.Descrizione, titolario.ID);
                            this.ddlTitolario.Items.Add(it);
                            valueTutti += titolario.ID + ",";
                            break;
                    }
                }
                //Imposto la voce tutti i titolari
                valueTutti = valueTutti.Substring(0, valueTutti.Length - 1);
                if (valueTutti != string.Empty)
                {
                    if (valueTutti.IndexOf(',') == -1)
                        valueTutti = valueTutti + "," + valueTutti;

                    ListItem it = new ListItem("Tutti i titolari", valueTutti);
                    this.ddlTitolario.Items.Insert(0, it);
                }

                //se la chiave di db è a 1, seleziono di default il titolario attivo
                if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_CHECK_TITOLARIO_ATTIVO.ToString())) && Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_CHECK_TITOLARIO_ATTIVO.ToString()).Equals("1"))
                {
                    int indexTitAtt = 0;
                    foreach (DocsPaWR.OrgTitolario titolario in listaTitolari)
                    {
                        if (titolario.Stato == DocsPaWR.OrgStatiTitolarioEnum.Attivo)
                        {
                            this.ddlTitolario.SelectedIndex = ++indexTitAtt;
                            break;
                        }
                        indexTitAtt++;
                    }
                }
            }
            else
            {
                DocsPaWR.OrgTitolario titolario = (DocsPaWR.OrgTitolario)listaTitolari[0];
                if (titolario.Stato != DocsPaWR.OrgStatiTitolarioEnum.InDefinizione)
                {
                    ListItem it = new ListItem(titolario.Descrizione, titolario.ID);
                    this.ddlTitolario.Items.Add(it);
                }
                this.ddlTitolario.Enabled = false;
                this.plcTitolario.Visible = false;
                this.UpPnlTitolario.Update();
            }
        }

        private void RemovePropertySearchCorrespondentIntExtWithDisabled()
        {
            HttpContext.Current.Session.Remove("searchCorrespondentIntExtWithDisabled");
        }

        private bool SearchCorrespondentIntExtWithDisabled
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["searchCorrespondentIntExtWithDisabled"] != null)
                {
                    result = bool.Parse(HttpContext.Current.Session["searchCorrespondentIntExtWithDisabled"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["searchCorrespondentIntExtWithDisabled"] = value;
            }
        }

        private string CallerSearchProject
        {
            get
            {
                string result = string.Empty;
                if (HttpContext.Current.Session["callerSearchProject"] != null)
                {
                    result = (HttpContext.Current.Session["callerSearchProject"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["callerSearchProject"] = value;
            }
        }

        protected RubricaCallType GetCallType(string idControl)
        {
            RubricaCallType calltype = RubricaCallType.CALLTYPE_CORR_INT_NO_UO;
            if (idControl == "txt_codMit_E")
            {
                calltype = RubricaCallType.CALLTYPE_CORR_INT_EST;
            }

            if (idControl == "txtCodiceCreatore")
            {
                calltype = RubricaCallType.CALLTYPE_OWNER_AUTHOR;
            }

            return calltype;
        }

        private bool CheckIfRootFolder(DocsPaWR.Folder folder)
        {
            Fascicolo Fasc = ProjectManager.getProjectInSession();
            DocsPaWR.Folder rootFolder = ProjectManager.getFolder(this, Fasc);
            if (folder.systemID == rootFolder.systemID) return true;
            return false;
        }

        private void DeleteSubset()
        {
            if (!ProjectManager.CheckRevocationAcl())
            {
                try
                {
                    string nFasc = "";
                    Folder folder = Session["remove_node_folder"] as Folder;

                    if (this.CheckIfRootFolder(folder))
                    {
                        if (UIManager.ProjectManager.getProjectInSession().tipo.Equals("P"))
                        {
                            string msg = "WarningProjectImpossibleDeleteProcedural";
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('" + utils.FormatJs(msg) + "', 'warning', '', '" + utils.FormatJs(UIManager.ProjectManager.getProjectInSession().codice) + "');", true);
                        }
                        if (UIManager.ProjectManager.getProjectInSession().tipo.Equals("G"))
                        {
                            string msg = "WarningProjectImpossibleDeleteGeneral";
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('" + utils.FormatJs(msg) + "', 'warning', '', '" + utils.FormatJs(UIManager.ProjectManager.getProjectInSession().codice) + "');", true);
                        } return;
                    }
                    if (folder != null)
                    {
                        /* Se il folder selezionato ha figli (doc o sottocartelle) su cui HO visibilità 
                         * non deve essere rimosso. Dopo l'avviso all'utente, la procedura termina */
                        if (folder.childs.Length > 0)
                        {
                            string msg = "WarningProjectImpossibleDeleteHasChildren";
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('" + utils.FormatJs(msg) + "', 'warning', '');", true);
                        }
                        else
                        {
                            /* Se il folder selezionato ha figli (doc o sottocartelle) su cui NON HO 
                             * la visibilità non deve essere rimosso */
                            //CanRemoveFascicolo ritornerà un bool: true = posso rimuovere il folder, false altrimenti
                            if (!ProjectManager.CanRemoveFascicolo(this, folder.systemID, out nFasc))
                            {
                                if (nFasc.Equals("0") || nFasc.Equals(""))
                                {
                                    string msg = "WarningProjectImpossibleDeleteContainsDocuments";
                                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('" + utils.FormatJs(msg) + "', 'warning', '');", true);
                                }
                                else
                                {
                                    string msg = "WarningProjectImpossibleDeleteHasChildren";
                                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('" + utils.FormatJs(msg) + "', 'warning', '');", true);
                                }
                            }
                            else
                            {
                                ProjectManager.delFolder(this, folder);
                            }
                        }
                    }

                }
                catch (System.Exception ex)
                {
                    UIManager.AdministrationManager.DiagnosticError(ex);
                }
            }
        }

        private void VisibiltyRoleFunctions()
        {
            if (!UIManager.UserManager.IsAuthorizedFunctions("FASC_NEW_FOLDER"))
            {
                this.ImgFolderAdd.Visible = false;
            }

            if (!UIManager.UserManager.IsAuthorizedFunctions("FASC_MOD_FOLDER"))
            {
                this.ImgFolderModify.Visible = false;
            }

            if (!UIManager.UserManager.IsAuthorizedFunctions("FASC_DEL_FOLDER"))
            {
                this.ImgFolderRemove.Visible = false;
            }
        }

        #region Excel Upload

        protected void ddl_tipoFascExcel_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            this.pnl_attrTipoFascExcel.Visible = false;
            Templates template = ProfilerProjectManager.getAttributiTipoFasc(UserManager.GetInfoUser(), this.ddl_tipoFascExcel.SelectedValue);
            if (template != null)
            {
                this.ddl_attrTipoFascExcel.Items.Clear();
                this.pnl_attrTipoFascExcel.Visible = true;

                for (int i = 0; i < template.ELENCO_OGGETTI.Length; i++)
                {
                    ListItem item_1 = new ListItem();
                    item_1.Value = template.ELENCO_OGGETTI[i].SYSTEM_ID.ToString();
                    item_1.Text = template.ELENCO_OGGETTI[i].DESCRIZIONE;
                    this.ddl_attrTipoFascExcel.Items.Add(item_1);
                }
            }
        }

        protected void ddl_attributo_SelectedIndexChanged(object sender, System.EventArgs e)
        {

            this.pnl_tipoFascExcel.Visible = false;
            this.pnl_attrTipoFascExcel.Visible = false;
            if (this.ddl_attributo.SelectedIndex == 5 && this.DocumentDdlTypeDocument.Items.Count > 1)
            {
                this.pnl_tipoFascExcel.Visible = true;
                this.ddl_tipoFascExcel.Items.Clear();
                ArrayList listaTipiFasc = new ArrayList(ProfilerProjectManager.getTipoFascFromRuolo(UserManager.GetInfoUser().idAmministrazione, RoleManager.GetRoleInSession().idGruppo, "1"));
                ListItem item = new ListItem();
                item.Value = "";
                item.Text = "";
                this.ddl_tipoFascExcel.Items.Add(item);
                for (int i = 0; i < listaTipiFasc.Count; i++)
                {
                    DocsPaWR.Templates templates = (DocsPaWR.Templates)listaTipiFasc[i];
                    ListItem item_1 = new ListItem();
                    item_1.Value = templates.SYSTEM_ID.ToString();
                    item_1.Text = templates.DESCRIZIONE;
                    if (templates.IPER_FASC_DOC == "1")
                        this.ddl_tipoFascExcel.Items.Insert(1, item_1);
                    else
                        this.ddl_tipoFascExcel.Items.Add(item_1);
                }
            }
        }

        protected void UploadBtn_Click(object sender, EventArgs e)
        {
            // Verfica che il controllo uploadedFile contenga un file 
            if (this.uploadedFile.HasFile)
            {
                string fileName = Server.HtmlEncode(this.uploadedFile.FileName);
                string extension = System.IO.Path.GetExtension(fileName);

                if (extension == ".xls")
                {
                    Session.Add("fileExcel", uploadedFile.PostedFile.FileName);
                    HttpPostedFile p = uploadedFile.PostedFile;
                    System.IO.Stream fs = p.InputStream;
                    byte[] dati = new byte[fs.Length];
                    fs.Read(dati, 0, (int)fs.Length);
                    fs.Close();
                    Session.Add("DatiExcel", dati);
                    this.lbl_fileExcel.Text = this.GetLabel("SearchProjectExcelFilterFileUploaded");
                    this.btn_elimina_excel.Visible = true;
                    this.btn_elimina_excel.Attributes.Add("onclick", "return confirm('" + utils.FormatJs(this.GetLabel("SearchProjectExcelFilterDelConfirm")) + "');");
                    this.pnlMainExcelFilter.Attributes["class"] = "shown";
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('WarningSearchProjectUploadXlsIncorrect', 'warning', '');", true);
                }
            }


            this.pnlAdvFiltersCollapse.Attributes["class"] += " shown";
            this.pnlMainExcelFilter.Attributes["class"] += " shown";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "pnlAdvFilters", "$('#pnlAdvFilters').css('display', 'block');", true);
        }

        protected void btn_elimina_excel_Click(object sender, System.EventArgs e)
        {
            this.lbl_fileExcel.Text = this.GetLabel("SearchProjectExcelFilterNoFileUploaded");
            this.btn_elimina_excel.Visible = false;

            this.ddl_attributo.SelectedIndex = -1;

            this.ddl_tipoFascExcel.SelectedIndex = -1;
            this.pnl_tipoFascExcel.Visible = false;

            this.ddl_attrTipoFascExcel.SelectedIndex = -1;
            this.pnl_attrTipoFascExcel.Visible = false;

            Session.Remove("fileExcel");
            Session.Remove("DatiExcel");
        }

        #endregion

    }



}