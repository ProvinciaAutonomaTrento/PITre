using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.UIManager;
using System.Collections;
using NttDataWA.Utils;
using NttDataWA.DocsPaWR;
using System.Data;

namespace NttDataWA.Popup
{
    public partial class Object : System.Web.UI.Page
    {
        protected void RefreshScript()
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshSelect", "refreshSelect()", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "InitializeLengthCharacters",
                "$('#TxtCodObject').keydown(charsLeft('TxtDescObject','" + this.MaxLenghtObject + "','Descrizione oggetto'));\n" +
                "$('#TxtCodObject').change(charsLeft('TxtDescObject','" + this.MaxLenghtObject + "','Descrizione oggetto'));\n", true);
        }

        #region Property

        private Oggetto[] ObjectstList
        {
            get
            {
                if (HttpContext.Current.Session["objectsList"] != null)
                    return HttpContext.Current.Session["objectsList"] as Oggetto[];
                else
                    return null;
            }
            set
            {
                HttpContext.Current.Session["objectsList"] = value;
            }

        }

        private Oggetto[] ObjectstListAll
        {
            get
            {
                if (HttpContext.Current.Session["objectsListAll"] != null)
                    return HttpContext.Current.Session["objectsListAll"] as Oggetto[];
                else
                    return null;
            }
            set
            {
                HttpContext.Current.Session["objectsListAll"] = value;
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
       

        private Oggetto ObjectGrid
        {
            get
            {
                if (HttpContext.Current.Session["object"] != null)
                    return (Oggetto)HttpContext.Current.Session["object"];
                else return null;
            }
            set
            {
                HttpContext.Current.Session["object"] = value;
            }
        }

        private int SelectedRowIndex
        {
            get
            {
                if (HttpContext.Current.Session["SelectedRowIndex"] != null && (int)HttpContext.Current.Session["SelectedRowIndex"] != -1)
                    return (int)HttpContext.Current.Session["SelectedRowIndex"];
                else
                    return -1;
            }
            set
            {
                HttpContext.Current.Session["SelectedRowIndex"] = value;
            }
        }

        private Registro[] ObjectRegistryList
        {
            get
            {
                if (HttpContext.Current.Session["objectsListRegistry"] != null)
                    return HttpContext.Current.Session["objectsListRegistry"] as Registro[];
                else return null;
            }
            set
            {
                HttpContext.Current.Session["objectsListRegistry"] = value;
            }

        }

        private string ReturnValue
        {
            set
            {
                HttpContext.Current.Session["ReturnValuePopup"] = value;
            }
        }
        //readonly
        private string TypeDoc
        {
            get
            {
                if (string.IsNullOrEmpty(HttpContext.Current.Session["typeDoc"].ToString()))
                {
                    SchedaDocumento sch = UIManager.DocumentManager.getSelectedRecord();
                    if (sch != null && sch.protocollo != null && (!string.IsNullOrEmpty(sch.protocollo.segnatura)))
                        return "p";
                    else return "n";
                }
                else return HttpContext.Current.Session["typeDoc"].ToString();
            }
        }

        protected int MaxLenghtObject
        {
            get
            {
                if (HttpContext.Current.Session["maxLenghtObject"] != null)
                    return (int)HttpContext.Current.Session["maxLenghtObject"];
                else return 2000;
            }

            set
            {
                HttpContext.Current.Session["maxLenghtObject"] = value;
            }
        }

        private bool EnableCodeObject
        {
            get
            {
                if (HttpContext.Current.Session["enableCodeObject"] != null)
                    return (bool)HttpContext.Current.Session["enableCodeObject"];
                else return false;
            }
        }

        private bool Mod_Authorized
        {
            get
            {
                return UserManager.IsAuthorizedFunctions("DO_MODIFICA_OGGETTARIO");
            }
        }

        private bool Ins_Authorized
        {
            get
            {
                return UserManager.IsAuthorizedFunctions("DO_INSERISCI_OGGETTARIO");
            }
        }

        private bool Del_Authorized
        {
            get
            {
                return UserManager.IsAuthorizedFunctions("DO_CANCELLA_OGGETTARIO");
            }
        }
        #endregion

        #region Initialize
        /// <summary>
        /// Initialize page
        /// </summary>
        private void InitializesPage()
        {
            this.FromInstanceAccess = false;

            List<string> ListIdregistersRF = new List<string>();
            ListIdregistersRF = LoadRegistersRFResearch("A");
            //ObjectstListAll = ObjectManager.getListaOggetti(this, ListIdregistersRF, "");
            if (Request.QueryString["rt"] != null && Request.QueryString["rt"] == "serachDocPopup")
            {
                this.FromInstanceAccess = true;
            }
        }

        /// <summary>
        /// Initialize Controls
        /// </summary>
        private void InitializeControls()
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "InitializeLengthCharacters", 
                "$('#TxtCodObject').keydown(charsLeft('TxtDescObject','" + this.MaxLenghtObject + "','Descrizione oggetto'));\n"+
                "$('#TxtCodObject').change(charsLeft('TxtDescObject','" + this.MaxLenghtObject + "','Descrizione oggetto'));\n", true);
        }

        /// <summary>
        /// Initializes application labels
        /// </summary>
        private void InitializesLabel()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            ObjectLblRegistry.Text = Utils.Languages.GetLabelFromCode("ObjectLblRegistry", language);
            ObjectLblCodObject.Text = Utils.Languages.GetLabelFromCode("ObjectLblCodObject", language);
            ObjectLblVoiceObject.Text = Utils.Languages.GetLabelFromCode("ObjectLblVoiceObject", language);
            ObjectBtnSearch.Text = Utils.Languages.GetLabelFromCode("ObjectBtnSearch", language);
            ObjectBtnInsert.Text = Utils.Languages.GetLabelFromCode("ObjectBtnInsert", language);
            ObjectBtnUpdate.Text = Utils.Languages.GetLabelFromCode("ObjectBtnUpdate", language);
            ObjectBtnDelete.Text = Utils.Languages.GetLabelFromCode("ObjectBtnDelete", language);
            ObjectLblFindRis.Text = Utils.Languages.GetLabelFromCode("ObjectLblFindRis", language);
            ObjectBtnChiudi.Text = Utils.Languages.GetLabelFromCode("ObjectBtnChiudi", language);
            this.DocumentLitObjectChAv.Text = Utils.Languages.GetLabelFromCode("DocumentLitObjectChAv", language);
            this.ObjectBtnInitialize.ToolTip = Utils.Languages.GetLabelFromCode("DocumentObjectBtnInitialize", language);
            this.ObjectBtnInitialize.AlternateText = Utils.Languages.GetLabelFromCode("DocumentObjectBtnInitialize", language);
        }

        #endregion

        /// <summary>
        /// Delete Property in session
        /// </summary>
        private void DeleteProperty()
        {

            HttpContext.Current.Session.Remove("objectPageIndex");
            HttpContext.Current.Session.Remove("registrysel");
            HttpContext.Current.Session.Remove("object");
            HttpContext.Current.Session.Remove("SelectedRowIndex");
            HttpContext.Current.Session.Remove("objectsListRegistry");
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    InitializeControls();
                    this.InitializesLabel();
                    CheckPermissions();
                    this.LoadRegistryRF();
                    this.InitializesPage();
                }
                else
                {
                    if (((ScriptManager)Master.FindControl("ScriptManager1")).IsInAsyncPostBack)
                    {
                        this.RefreshScript();
                        switch (((ScriptManager)Master.FindControl("ScriptManager1")).AsyncPostBackSourceElementID)
                        {
                            case "UpdPnlGridSearch":
                                UpdateSelectedRow();
                                this.BtnOk.Enabled = true;
                                this.ObjectBtnDelete.Enabled = true;
                                this.UpPnlButtons.Update();
                                return;
                        }
                    }
                }

                if (GridSearchObject != null && GridSearchObject.SelectedIndex != -1)
                {
                    this.BtnOk.Enabled = true;
                    this.ObjectBtnDelete.Enabled = true;
                }

                else
                {
                    this.BtnOk.Enabled = false;
                    this.ObjectBtnDelete.Enabled = false;
                }

                if (!UserManager.IsAuthorizedFunctions("DO_PROT_OG_MODIFICA"))
                {
                    this.BtnOk.Visible = false;
                }
                //Emanuela 29-04-2014: Modifico la seguehnte riga di codice perchè, anche se la chiave non è attiva, in fase di creazione del protocollo
                //posso inserire dall'oggettario
                //if (DocumentManager.getSelectedRecord() != null && !string.IsNullOrEmpty(DocumentManager.getSelectedRecord().systemId) && DocumentManager.getSelectedRecord().tipoProto.Equals("G"))
                if (DocumentManager.getSelectedRecord() != null && (string.IsNullOrEmpty(DocumentManager.getSelectedRecord().systemId) || DocumentManager.getSelectedRecord().tipoProto.Equals("G")))
                {
                    this.BtnOk.Visible = true;
                }

                this.UpPnlButtons.Update();

            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// Checks whether the role is enabled to insert, edit, delete, and eventually hidden the buttons associated
        /// </summary>
        private void CheckPermissions()
        {
            //Check if the role has enabled the insert / modify / delete
            if (!this.Mod_Authorized)
                ObjectBtnUpdate.Visible = false;
            if (!this.Ins_Authorized)
                ObjectBtnInsert.Visible = false;
            if (!this.Del_Authorized)
                ObjectBtnDelete.Visible = false;
        }

        /// <summary>
        /// Load registry and RF
        /// </summary>
        private void LoadRegistryRF()
        {
            // if protocol or simplified protocol
            if (TypeDoc.Equals("p") || TypeDoc.Equals("psempl"))
            {
                DocsPaWR.Registro reg = null;
                if (TypeDoc.Equals("p"))
                {
                    //get protocol registry
                    reg = RegistryManager.GetRegistryInSession();
                }
                else
                {
                    //simplified protocol
                }

                this.DdlRegRf.Items.Add(new ListItem() { Text = reg.codRegistro, Value = string.Empty, Selected = true });
                //I take the RF associated with the registry protocol
                Registro[] listRF = RegistryManager.GetListRegistriesAndRF(UIManager.RoleManager.GetRoleInSession().systemId, "1", reg.systemId);
                if (listRF != null && listRF.Length > 0)
                {
                    /*I value della combo dei registri sono formati da terne separate dal carattere "_":
                     * nella prima posizione viene specificata la systemId dell'RF
                     * nella seconda posizione 0 per gli RF abilitati, 1 per gli RF non abilitati
                     * nella terza posizione viene specificato l'IdAooCollegata (solo per gli RF)
                     */
                    ListItem[] listItemRF = (from rf in listRF
                                             select new ListItem()
                                             {
                                                 Text = rf.codRegistro,
                                                 Value = rf.systemId + "_" + rf.rfDisabled + "_" + rf.idAOOCollegata
                                             }).ToArray();

                    if (listItemRF != null && listItemRF.Length > 0)
                    {
                        foreach (ListItem li in listItemRF)
                            li.Attributes.Add("style", "margin-left:10px;");
                        this.DdlRegRf.Items.AddRange(listItemRF);
                    }
                    Stack<Registro> stackRegistries = new Stack<Registro>(listRF);
                    stackRegistries.Push(reg);
                    this.ObjectRegistryList = stackRegistries.ToArray();
                }
                else
                {
                    this.ObjectRegistryList = new Registro[]{reg};
                }
            }
            else // case gray documents and search
            {
                this.DdlRegRf.Items.Add(new ListItem() { Text = "TUTTI", Value = string.Empty, Selected = true });
                Registro[] ListRegistriesAndRF = RegistryManager.GetListRegistriesAndRF(UIManager.RoleManager.GetRoleInSession().systemId, string.Empty, string.Empty);
                var PairRegRF = (from reg in ListRegistriesAndRF
                                 where reg.chaRF.Equals("0")
                                 select (new
                                 {
                                     IdReg = reg.systemId,
                                     CodReg = reg.codRegistro,
                                     ListItemRF =
                                         (from rf in ListRegistriesAndRF
                                          where rf.idAOOCollegata == reg.systemId
                                          select new ListItem() { Text = rf.codRegistro, Value = rf.systemId + "_" + rf.rfDisabled + "_" + rf.idAOOCollegata }).ToArray()
                                 }));
                if (PairRegRF != null)
                {
                    foreach (var RegAssRF in PairRegRF)
                    {
                        this.DdlRegRf.Items.Add(new ListItem() { Text = RegAssRF.CodReg, Value = RegAssRF.IdReg });
                        if ((RegAssRF.ListItemRF as ListItem[]) != null && (RegAssRF.ListItemRF as ListItem[]).Length > 0)
                        {
                            foreach (ListItem li in (RegAssRF.ListItemRF as ListItem[]))
                                li.Attributes.Add("style", "margin-left:10px;");
                            this.DdlRegRf.Items.AddRange(RegAssRF.ListItemRF as ListItem[]);
                        }
                    }
                }
                this.ObjectRegistryList = ListRegistriesAndRF;
            }
        }

        /// <summary>
        /// Search list Object
        /// </summary>
        private void Search()
        {
            #region get list registers and rf
            List<string> ListIdregistersRF = new List<string>();
            //the popup is call from proto or simplified proto
            if (this.TypeDoc.Equals("p") || this.TypeDoc.Equals("psempl"))
            {
                //if i select a registry
                if (this.DdlRegRf.SelectedValue.Equals(string.Empty))
                {
                    string IdRegistry = string.Empty;
                    if (this.TypeDoc.Equals("p")) // if protocol
                        IdRegistry = RegistryManager.GetRegistryInSession().systemId;
                    else if (this.TypeDoc.Equals("psempl")) // if simplified protocol
                        IdRegistry = "111111111";//regMng.GetRegistroCorrente().systemId;

                    //if i search in the registry protocol then I also try to register the associated rf
                    ListIdregistersRF = LoadRegistersRFResearch("R", RegistryManager.GetRegistryInSession().systemId);
                }
                else
                {
                    // I have specified a RF, so I try only to
                    ListIdregistersRF.Add(this.DdlRegRf.SelectedValue.Split('_').First());
                }
            }
            else //the popup is called by gray document or research
            {
                //the user has selected the field all
                if (this.DdlRegRf.SelectedValue.Equals(string.Empty))
                {
                    /*
                     The search is for:
                     * - all the objects associated with all registers visible to the role;
                     * - all the objects associated with RF visible to the role;
                     * - between all objects register with NULL.
                     */
                    ListIdregistersRF = LoadRegistersRFResearch("A");
                }
                else
                {
                    /* the user has selected a register or rf:
                     * - if you selected a registry then search for the object in the registry and in the rf associated with him
                     * - I have specified a RF, so I try only to
                     */
                    if (this.DdlRegRf.SelectedValue.Split('_').Length == 1) // case selected registry
                        ListIdregistersRF = LoadRegistersRFResearch("R", this.DdlRegRf.SelectedValue);
                    else
                        ListIdregistersRF.Add(this.DdlRegRf.SelectedValue.Split('_').First());
                }
            }
            #endregion

            #region get list object
            if (!string.IsNullOrEmpty(this.TxtCodObject.Text.Trim()) && this.EnableCodeObject)
            {
                //enable search with LIKE and add special characters $@
                this.ObjectstList = ObjectManager.getListaOggettiByCod(this, ListIdregistersRF, this.TxtDescObject.Text.Trim(), ("$@" + this.TxtCodObject.Text.Trim()));
            }
            else
            {
                this.ObjectstList = ObjectManager.getListaOggetti(this, ListIdregistersRF, this.TxtDescObject.Text.Trim());
            }

            if (this.ObjectstList.Length > 0)
            {
                this.ObjectLblFindRis.Visible = false;
                //Bind list to grid
                List<DocsPaWR.Oggetto> list = new List<Oggetto>(this.ObjectstList);
                this.GridSearchObject.Visible = true;
                list = (from og in list orderby og.descrizione ascending select og).ToList<Oggetto>();
                this.GridSearchObject.DataSource = list;

                // header
                string language = UIManager.UserManager.GetUserLanguage();
                this.GridSearchObject.Columns[1].HeaderText = Utils.Languages.GetLabelFromCode("ObjectCodeObject", language);
                this.GridSearchObject.Columns[2].HeaderText = Utils.Languages.GetLabelFromCode("ObjectListEntry", language);
                this.GridSearchObject.Columns[3].HeaderText = Utils.Languages.GetLabelFromCode("ObjectRegistry", language);

                this.GridSearchObject.DataBind();
                if (this.ObjectstList.Length == 1)
                {
                    this.GridSearchObject.SelectedIndex = 0;
                    this.BtnOk.Enabled = true;
                    this.ObjectBtnDelete.Enabled = true;
                }
                else
                {
                    this.GridSearchObject.SelectedIndex = -1;
                    this.BtnOk.Enabled = false;
                    this.ObjectBtnDelete.Enabled = false;
                }
            }
            else
            {
                this.ObjectLblFindRis.Visible = true;
                this.GridSearchObject.Visible = false;
                this.HighlightSelectedRow();
                this.BtnOk.Enabled = false;
                this.ObjectBtnDelete.Enabled = false;
            }
            this.UpPnlButtons.Update();

            #endregion
        }

        /// <summary>
        /// Returns a list of id registers and rf for research object
        /// </summary>
        /// <param name="systemId">Id registry</param>
        /// <returns>List</returns>
        private List<string> LoadRegistersRFResearch(string itemSelect, string systemId = "")
        {
            List<string> listIdRF = new List<string>();
            Stack<string> stackIdRf = new Stack<string>();
            switch (itemSelect)
            {
                case "R":
                    stackIdRf = (new Stack<string>(from r in this.DdlRegRf.Items.Cast<ListItem>()
                                 where r.Value.Split('_').Length > 1 && r.Value.Split('_').Last().Equals(systemId)
                                 select r.Value.Split('_').First()));
                    //I add the register of the stack
                    stackIdRf.Push(systemId);
                    listIdRF.AddRange(stackIdRf.ToArray());
                    break;
                case "A":
                    listIdRF = ((from r in this.DdlRegRf.Items.Cast<ListItem>() select r.Value.Split('_').First()).ToList());
                    break;
            }
            return listIdRF;
        }

        private enum oggettario
        {
            systemid = 0,
            codobject = 1,
            description = 2,
            registrycode = 3
        }

        private void HighlightSelectedRow()
        {
            if (this.GridSearchObject.Rows.Count > 0 && (int)this.SelectedRowIndex != -1)
            {
                GridViewRow gvRow = this.GridSearchObject.SelectedRow;
                foreach (GridViewRow GVR in this.GridSearchObject.Rows)
                {
                    if (GVR == gvRow)
                    {
                        GVR.CssClass += " selectedrow";
                        //DocumentManager.setSelectedAttachId(((HiddenField)GVR.Cells[4].FindControl("attachId")).Value);
                        //showDocumentAcquired();
                    }
                    else
                    {
                        GVR.CssClass = GVR.CssClass.Replace(" selectedrow", "");
                    }
                }
            }
        }

        #region Event Handler

        protected void btnInsert_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "<script>reallowOp();</script>", false);
            try
            {
                string errMsg = string.Empty;

                DocsPaWR.Oggetto oggetto = new DocsPaWR.Oggetto();
                DocsPaWR.Registro registro = new DocsPaWR.Registro();

                #region Validators
                //check on insert of the object
                if (string.IsNullOrEmpty(this.TxtDescObject.Text.Trim()))
                {
                    errMsg = "ErrorObjectNoObject";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ErrorObjectNoObject", "parent.ajaxDialogModal('" + errMsg + "', 'warning', 'ATTENZIONE');", true);
                    return;
                }

                //check over the length of the object (up to 2000 characters.)
                if (this.TxtDescObject.Text.Length > Convert.ToInt32(this.MaxLenghtObject))
                {
                    errMsg = "ErrorObjectLength";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ErrorObjectLength", "parent.ajaxDialogModal('" + errMsg + "', 'warning', 'ATTENZIONE');", true);
                    return;
                }
                #endregion

                //replace single quotes
                oggetto.descrizione = this.TxtDescObject.Text.Trim();// .Replace("'", "''"); Escape viene fatto dai WS
          

                //Adding the object code if there
                if (!string.IsNullOrEmpty(this.TxtCodObject.Text.Trim()))
                {
                    oggetto.codOggetto = this.TxtCodObject.Text.Trim(); // .Replace("'", "''"); Escape viene fatto dai WS
                }
                else
                {
                    oggetto.codOggetto = "";
                }

                if (this.DdlRegRf.SelectedItem.Value.Equals(""))
                {
                    if (TypeDoc.Equals("p")) // protocol
                    {
                        registro = RegistryManager.GetRegistryInSession(); //get protocol registry
                        registro.codice = this.DdlRegRf.SelectedItem.Text;
                    }
                    else if(TypeDoc.Equals("psempl"))
                    {
                         // simplified protocol
                         registro = new Registro();
                    }
                    else
                    {
                        registro = null;
                        errMsg = "ErrorRegisterNotSelected";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ErrorRegisterNotSelected", "parent.ajaxDialogModal('" + errMsg + "', 'warning', 'ATTENZIONE');", true);
                        return;
                    }
                }
                else
                {
                    registro.systemId = this.DdlRegRf.SelectedValue.Split('_').First();
                    registro.codice = this.DdlRegRf.SelectedItem.Text;
                }

                /*
                if (!string.IsNullOrEmpty(this.TxtCodObject.Text.Trim()))
                {
                    if ((from obj in ObjectstListAll where obj.codOggetto.Equals(this.TxtCodObject.Text.Trim()) && obj.codRegistro.Equals(registro.codice) select obj).Count() > 0)
                    {
                        errMsg = "ErrorObjectCodePresent";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ErrorObjectCodePresent", "parent.ajaxDialogModal('" + errMsg + "', 'warning', 'ATTENZIONE');", true);
                        return;
                    }
                }
                if ((from obj in ObjectstListAll where obj.descrizione.Equals(this.TxtDescObject.Text.Trim()) && obj.codRegistro.Equals(registro.codice) select obj).Count() > 0)
                {
                    errMsg = "ErrorObjectVoicePresent";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ErrorObjectVoicePresent", "parent.ajaxDialogModal('" + errMsg + "', 'warning', 'ATTENZIONE');", true);
                    return;
                }
                */
                oggetto = ObjectManager.AddObject(this, oggetto, registro, ref errMsg);

                if (oggetto != null)
                {
                    oggetto.codRegistro = registro.codice;
                    List<Oggetto> listObj = new List<Oggetto>();
                    listObj.Add(oggetto);
                    /*
                    List<Oggetto> l = ObjectstListAll.ToList<Oggetto>();
                    l.Add(oggetto);
                    ObjectstListAll = l.ToArray<Oggetto>();
                     * */
                    this.GridSearchObject.DataSource = listObj;
                    this.GridSearchObject.DataBind();
                    this.GridSearchObject.SelectedIndex = 0;
                    this.GridSearchObject.Visible = true;
                    this.BtnOk.Enabled = true;
                    this.ObjectBtnDelete.Enabled = true;
                    this.UpPnlButtons.Update();
                    this.UpdPnlGridSearch.Update();
                }
                else
                {
                    if (string.IsNullOrEmpty(this.TxtCodObject.Text.Trim()))
                    {
                        errMsg = "ErrorObjectCodeObjectPresent";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ErrorObjectCodeObjectPresent", "parent.ajaxDialogModal('" + errMsg + "', 'warning', 'ATTENZIONE');", true);
                    }
                    else
                    {
                        errMsg = "ErrorObjectPresent";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ErrorObjectPresent", "parent.ajaxDialogModal('" + errMsg + "', 'warning', 'ATTENZIONE');", true);
                    }
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "<script>reallowOp();</script>", false);
            string errMsg = string.Empty;
            DocsPaWR.Oggetto oggetto = new DocsPaWR.Oggetto();
            DocsPaWR.Registro registro = new DocsPaWR.Registro();
            DocsPaWR.Oggetto oggettoDoc = new DocsPaWR.Oggetto();

            #region controls
            //check on insert object

            bool result;

            if (this.GridSearchObject.SelectedIndex == -1)
            {
                errMsg = "ErrorObjectSelectRecordMod";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ErrorObjectSelectRecordMod", "parent.ajaxDialogModal('" + errMsg + "', 'warning', 'ATTENZIONE');", true);
                return;
            }

            if (string.IsNullOrEmpty(this.TxtDescObject.Text))
            {
                errMsg = "ErrorObjectNoObject";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ErrorObjectNoObject", "parent.ajaxDialogModal('" + errMsg + "', 'warning', 'ATTENZIONE');", true);
                return;
            }

            //check on the length of the object (up to 2000 characters.)
            if (this.TxtDescObject.Text.Length > 2000)
            {
                errMsg = "ErrorObjectLength";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ErrorObjectLength", "parent.ajaxDialogModal('" + errMsg + "', 'warning', 'ATTENZIONE');", true);
                return;
            }

            #endregion

            oggettoDoc.daAggiornare = true;

            int selRow = this.GridSearchObject.SelectedIndex;
            oggetto.descrizione = this.TxtDescObject.Text.Trim(); //.Replace("'", "''"); Escape gestito dai WS

            //Adding the object code if there
            if (!string.IsNullOrEmpty(this.TxtCodObject.Text.Trim()))
            {
                oggetto.codOggetto = this.TxtCodObject.Text.Trim(); //.Replace("'", "''"); Escape gestito dai WS
            }
            else
            {
                oggetto.codOggetto = string.Empty;
            }

            if (selRow > -1 && selRow < this.GridSearchObject.Rows.Count)
            {
                oggettoDoc.systemId = (GridSearchObject.Rows[selRow].Cells[(int)oggettario.systemid].FindControl("systemid") as Label).Text;
                oggettoDoc.descrizione = (GridSearchObject.Rows[selRow].Cells[(int)oggettario.description].FindControl("lblDescObject") as Label).Text;
                oggettoDoc.descrizione = oggettoDoc.descrizione; //.Replace("'", "''"); Escape gestito dai WS

                //recovery the registry of the selected object for editing
                Label lblRegCode = (Label)GridSearchObject.Rows[selRow].Cells[(int)oggettario.registrycode].FindControl("lblRegCode");
                registro.codice = lblRegCode.Text;
                for (int regNum = 0; regNum < this.ObjectRegistryList.Length; regNum++)
                {
                    if (this.ObjectRegistryList[regNum].codRegistro.ToLower().Equals(registro.codice.ToLower()))
                    {
                        registro.systemId = this.ObjectRegistryList[regNum].systemId;
                        break;
                    }
                }
                /*
                if (!string.IsNullOrEmpty(this.TxtCodObject.Text.Trim()))
                {
                    if ((from obj in ObjectstListAll where obj.codOggetto.Equals(this.TxtCodObject.Text.Trim()) && obj.codRegistro.Equals(registro.codice) && !obj.systemId.Equals(oggettoDoc.systemId) select obj).Count() > 0)
                    {
                        errMsg = "ErrorObjectCodePresent";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ErrorObjectCodePresent", "parent.ajaxDialogModal('" + errMsg + "', 'warning', 'ATTENZIONE');", true);
                        return;
                    }
                }
                if ((from obj in ObjectstListAll where obj.descrizione.Equals(this.TxtDescObject.Text.Trim()) && obj.codRegistro.Equals(registro.codice) && !obj.systemId.Equals(oggettoDoc.systemId) select obj).Count() > 0)
                {
                    errMsg = "ErrorObjectVoicePresent";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ErrorObjectVoicePresent", "parent.ajaxDialogModal('" + errMsg + "', 'warning', 'ATTENZIONE');", true);
                    return;
                }
                */
                if (string.IsNullOrEmpty(registro.systemId))
                {
                    registro = null;
                }
                oggettoDoc.codOggetto = (GridSearchObject.Rows[selRow].Cells[(int)oggettario.codobject].FindControl("lblCodObject") as Label).Text;

                try
                {
                    result = ObjectManager.DeleteObject(this, oggettoDoc);
                    if (result)
                    {
                        oggetto = ObjectManager.AddObject(this, oggetto, registro, ref errMsg);
                        if (!string.IsNullOrEmpty(errMsg) || oggetto == null)  //In the case of failure to enter recovery from deletion
                        {
                            oggetto = ObjectManager.AddObject(this, oggettoDoc, registro, ref errMsg);
                            errMsg = "ErrorObjectPresent";
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ErrorObjectPresent", "parent.ajaxDialogModal('" + errMsg + "', 'warning', 'ATTENZIONE');", true);
                            return;
                        }
                        if(this.ObjectstList != null && this.ObjectstList.Count() > 0 && (from o in ObjectstList where o.systemId.Equals(oggettoDoc.systemId) select o).FirstOrDefault() != null)
                        {
                            oggetto.codRegistro = registro != null ? registro.codice : string.Empty;
                            //List<int> index = (from a in ObjectstList.Select((item, i) => new { obj = item, index = i }) where a.obj.systemId.Equals(oggettoDoc.systemId) select a.index).ToList();
                            int index = ObjectstList.Select((item, i) => new { obj = item, index = i }).First(item => item.obj.systemId.Equals(oggettoDoc.systemId)).index;
                            ObjectstList[index] = oggetto;
                        }
                    }
                    else
                    {
                        errMsg = "ErrorObjectPresent";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ErrorObjectPresent", "parent.ajaxDialogModal('" + errMsg + "', 'warning', 'ATTENZIONE');", true);
                        return;
                    }
                }
                catch (Exception err)
                {
                    throw new Exception("ERROR: " + err.Message);
                }
            }
            this.GridSearchObject.EditIndex = -1;
            this.GridSearchObject.SelectedIndex = -1;
            this.BtnOk.Enabled = false;
            this.ObjectBtnDelete.Enabled = false;
            this.TxtCodObject.Text = string.Empty;
            this.TxtDescObject.Text = string.Empty;
            (GridSearchObject.Rows[selRow].Cells[(int)oggettario.systemid].FindControl("systemid") as Label).Text = oggetto.systemId;
            (GridSearchObject.Rows[selRow].Cells[(int)oggettario.codobject].FindControl("lblCodObject") as Label).Text = oggetto.codOggetto;
            (GridSearchObject.Rows[selRow].Cells[(int)oggettario.description].FindControl("lblDescObject") as Label).Text = oggetto.descrizione;
            this.UpdPnlGridSearch.Update();
            this.UpdPnlCodeObject.Update();
            this.UpPnlButtons.Update();
        }

        private void UpdateSelectedRow()
        {
            this.GridSearchObject.SelectedIndex = Convert.ToInt32(this.grid_rowindex.Value);
            HighlightSelectedRow();
            this.TxtCodObject.Text = (GridSearchObject.Rows[this.GridSearchObject.SelectedIndex].Cells[(int)oggettario.description].FindControl("lblCodObject") as Label).Text;
            this.TxtDescObject.Text = (GridSearchObject.Rows[this.GridSearchObject.SelectedIndex].Cells[(int)oggettario.description].FindControl("lblDescObject") as Label).Text;
            this.UpdPnlGridSearch.Update();
            this.UpdPnlCodeObject.Update();
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "<script>reallowOp();</script>", false);
            DocsPaWR.Oggetto oggetto = new DocsPaWR.Oggetto();
            DocsPaWR.Registro registro = new DocsPaWR.Registro();
            DocsPaWR.Registro registroDoc = new DocsPaWR.Registro();
            string msg = string.Empty;
            bool result;

            if (GridSearchObject.SelectedIndex == -1)
            {
                msg = "ErrorObjectSelRecDel";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ErrorObjectSelRecDel", 
                    "if (parent.fra_main) " + "{parent.fra_main.ajaxDialogModal('" + msg + "', 'warning', 'ATTENZIONE');} " +
                    "else {parent.ajaxDialogModal('" + msg + "', 'warning', 'ATTENZIONE');}", true);
                return;
            }
            int selRow = GridSearchObject.SelectedRow.RowIndex;
            oggetto.daAggiornare = true;
            oggetto.systemId = (GridSearchObject.Rows[selRow].Cells[(int)oggettario.systemid].FindControl("systemid") as Label).Text;
            oggetto.descrizione = (GridSearchObject.Rows[selRow].Cells[(int)oggettario.description].FindControl("lblDescObject") as Label).Text;

            if(!string.IsNullOrEmpty((GridSearchObject.Rows[selRow].Cells[(int)oggettario.codobject].FindControl("lblCodObject") as Label).Text))
                oggetto.codOggetto = (GridSearchObject.Rows[selRow].Cells[(int)oggettario.codobject].FindControl("lblCodObject") as Label).Text;

            result = ObjectManager.DeleteObject(this, oggetto);
            
            if (result)
            {
                this.ObjectstList = (from listObj in this.ObjectstList where (!string.IsNullOrEmpty(oggetto.systemId) &&
                                         (!listObj.systemId.Equals(oggetto.systemId))) select listObj).ToArray();
                //this.ObjectstListAll = (from listObj in this.ObjectstListAll where (!string.IsNullOrEmpty(oggetto.systemId) && (!listObj.systemId.Equals(oggetto.systemId)))
                //                     select listObj).ToArray();
                this.GridSearchObject.SelectedIndex = -1;
                this.GridSearchObject.PageIndex = 0;
                this.BtnOk.Enabled = false;
                this.ObjectBtnDelete.Enabled = false;
                this.TxtCodObject.Text = string.Empty;
                this.TxtDescObject.Text = string.Empty;
                this.GridSearchObject.DataSource = this.ObjectstList;
                this.GridSearchObject.DataBind();
                this.UpdPnlCodeObject.Update();
                this.UpdPnlGridSearch.Update();
                this.UpPnlButtons.Update();
            }
            else
            {
                msg = "ErrorObjectRemove";
                // if (parent.fra_main) {parent.fra_main.ajaxDialogModal();} else {top.ajaxDialogModal();}
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ErrorObjectRemove", "if (parent.fra_main) " +
                    "{parent.fra_main.ajaxDialogModal('" + msg + "', 'warning', 'ATTENZIONE');} " + 
                    "else {parent.ajaxDialogModal('" + msg + "', 'warning', 'ATTENZIONE');}",true);
            }
        }

        protected void ObjectBtnSearch_Click(object sender, EventArgs e)
        {
            Search();
            this.UpdPnlGridSearch.Update();
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "<script>reallowOp();</script>", false);
        }

        protected void btnOk_Click(object sender, EventArgs e)
        {
            string parent = string.Empty;
            if (this.AddDoc || this.FromInstanceAccess)
            {
                parent = ",parent";
            }
            string popupId = "Object";
            if (!string.IsNullOrEmpty(Request.QueryString["fromMainDocument"]))
                popupId = "ObjectFromMainDocument";
            if (!string.IsNullOrEmpty(this.TxtDescObject.Text))
            {
                this.ReturnValue = this.TxtDescObject.Text + "#" + (string.IsNullOrEmpty(this.TxtCodObject.Text) ? string.Empty : this.TxtCodObject.Text);
                ScriptManager.RegisterClientScriptBlock(this.UpPnlButtons, this.UpPnlButtons.GetType(), "closeAJM", "parent.closeAjaxModal('" + utils.FormatJs(popupId) + "','up'" + parent + ");", true);
            }
            else
                ScriptManager.RegisterClientScriptBlock(this.UpPnlButtons, this.UpPnlButtons.GetType(), "closeAJM", "parent.closeAjaxModal('" + utils.FormatJs(popupId) + "','');", true);
        }

        protected void ObjectBtnChiudi_Click(object sender, EventArgs e)
        {
            DeleteProperty();
            string parent = string.Empty;
            if (this.AddDoc || this.FromInstanceAccess)
            {
                parent = ",parent";
            }
            string popupId = "Object";
            if (!string.IsNullOrEmpty(Request.QueryString["fromMainDocument"]))
                popupId = "ObjectFromMainDocument";
            Response.Write("<html><body><script type=\"text/javascript\">parent.closeAjaxModal('" + utils.FormatJs(popupId) + "', ''" + parent + ");</script></body></html>");
            Response.End();
        }

        protected void gridSearchObject_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            this.GridSearchObject.PageIndex = e.NewPageIndex;
            if (!this.GridSearchObject.SelectedIndex.Equals(-1))
                this.GridSearchObject.SelectedIndex = 0;
            ObjectstList = (from og in ObjectstList orderby og.descrizione ascending select og).ToArray<Oggetto>();
            this.GridSearchObject.DataSource = this.ObjectstList;
            this.GridSearchObject.DataBind();
            this.UpdPnlGridSearch.Update();

        }

        protected void gridSearchObject_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Attributes["onclick"] = "$('#grid_rowindex').val('" + e.Row.RowIndex.ToString() + "'); __doPostBack('UpdPnlGridSearch', ''); return false;";
            }
        }

        protected void ObjectBtnInitialize_Click(object sender, EventArgs e)
        {
            this.DdlRegRf.SelectedIndex = -1;
            this.TxtCodObject.Text = string.Empty;
            TxtDescObject.Text = string.Empty;
            this.UpdPnlRegistry.Update();
            this.UpdPnlCodeObject.Update();
        }
        #endregion

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

    }
}
