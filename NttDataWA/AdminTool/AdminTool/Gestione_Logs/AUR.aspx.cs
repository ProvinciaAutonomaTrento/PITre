using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SAAdminTool.DocsPaWR;

namespace SAAdminTool.AdminTool.Gestione_Logs
{
    public partial class AUR : System.Web.UI.Page
    {
        #region Property

        /// <summary>
        /// List all assertions
        /// </summary>
        private List<DocsPaWR.Assertion> Assertions
        {
            get
            {
                if (HttpContext.Current.Session["AssertionsList"] == null)
                    return new List<DocsPaWR.Assertion>();
                else
                    return HttpContext.Current.Session["AssertionsList"] as List<DocsPaWR.Assertion>;
            }

            set
            {
                HttpContext.Current.Session["AssertionsList"] = value;
            }
        }

        private List<string> ListResultAur
        {
            get
            {
                if (HttpContext.Current.Session["ListResultAur"] == null)
                    return new List<string>();
                else
                    return HttpContext.Current.Session["ListResultAur"] as List<string>;
            }

            set
            {
                HttpContext.Current.Session["ListResultAur"] = value;
            }
        }

        private string ID_ADMINISTRATION
        {
            get
            {
                if (HttpContext.Current.Session["IdAdministrationAUR"] == null)
                    return string.Empty;
                else
                    return HttpContext.Current.Session["IdAdministrationAUR"].ToString();
            }

            set
            {
                HttpContext.Current.Session["IdAdministrationAUR"] = value;
            }
        }

        private string CODE_ADMINISTRATION
        {
            get
            {
                if (HttpContext.Current.Session["CodeAdministrationAUR"] == null)
                    return string.Empty;
                else
                    return HttpContext.Current.Session["CodeAdministrationAUR"].ToString();
            }

            set
            {
                HttpContext.Current.Session["CodeAdministrationAUR"] = value;
            }
        }

        private string MODE
        {
            get
            {
                return HttpContext.Current.Session["ModePanelAssertion"].ToString();
            }

            set
            {
                HttpContext.Current.Session["ModePanelAssertion"] = value;
            }
        }

        /// <summary>
        /// System id delle asserzioni che sono state selezionate, tramite checkbox, per la rimozione
        /// </summary>
        private List<string> ListSystemIdAssertionToRemove
        {
            get
            {
                if (HttpContext.Current.Session["ListSystemIdAssertionToRemove"] != null)
                    return HttpContext.Current.Session["ListSystemIdAssertionToRemove"] as List<string>;
                else
                    return null;
            }
            set
            {
                HttpContext.Current.Session["ListSystemIdAssertionToRemove"] = value;
            }
        }

         #endregion

        #region Remove property

        private void RemovePropertyInSession()
        {
            HttpContext.Current.Session.Remove("ListResultAur");
            HttpContext.Current.Session.Remove("ListSystemIdAssertionToRemove");
        }

        #endregion

        #region Const

        private const string TYPE_AUR_ADMINISTRATION = "AMM";
        private const string TYPE_AUR_RF = "RF";
        private const string TYPE_AUR_ROLETYPE = "TR";
        private const string TYPE_AUR_ROLE = "R";
        private const string TYPE_AUR_UO = "UO";
        private const string INFORMATION = "I";
        private const string OPERATIONAL = "O";
        private const string UPDATE_PANEL_GRID_VIEW = "UpdatePanelGridView";
        private const string REMOVE_SELECTED_ASSERTION  = "RemoveSelectedAssertion";
        private const string UPDATE_PANEL_RESULT = "upPnlResult";
        private const string BTN_CERCA = "btnCerca";
        private const string NO_NOTIFICABLE = "NN";
        private const string OBLIGATORY = "OBB";
        private const string UPDATE_PANEL_CODICE_DESCRIZIONE = "UpPnlCodiceDescrizione";
        private const string UPDATE_GRD_AGGREGATES = "updateGrdAggregates";
        private const string SELECTED_ASSERTION_IN_GRID_VIEW = "selectedAssertionInGridView";
        #endregion

        private SAAdminTool.DocsPaWR.DocsPaWebService ws = new SAAdminTool.DocsPaWR.DocsPaWebService();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                

                //leggo l'id dell'amministrazione corrente
                string[] amministrazione = ((string)Session["AMMDATASET"]).Split('@');
                string codiceAmministrazione = amministrazione[0];
                string idAmm = ws.getIdAmmByCod(codiceAmministrazione);
                CODE_ADMINISTRATION = codiceAmministrazione;
                ID_ADMINISTRATION = idAmm;
                //popolo la drop down list con i tipi evento configurabili
                BuildDdlTypeConfigurableEvents();
                //Popolo la griglia delle asserzioni
                Assertions = ws.GetListAssertion(ID_ADMINISTRATION).ToList();
                //Popolo la griglia delle asserzioni
                Initialize();
                BuildGrdAssertions();
                //Popolo il pannello dell'asserzione
                if (this.GrdAsserzioni != null && this.GrdAsserzioni.Rows.Count > 0)
                    MODE = "Modify";
                else
                {
                    MODE = "Hidden";
                }
                BuildPanelAssertion();
            }
            else
            {
                if (this.Request.Form["__EVENTTARGET"] != null && this.Request.Form["__EVENTTARGET"].Equals(UPDATE_PANEL_GRID_VIEW))
                {
                    if (this.Request.Form["__EVENTARGUMENT"] != null && (this.Request.Form["__EVENTARGUMENT"].Equals(REMOVE_SELECTED_ASSERTION)))
                    {
                        RemoveAssertion();
                        return;
                    }
                    if (this.Request.Form["__EVENTARGUMENT"] != null && (this.Request.Form["__EVENTARGUMENT"].Equals(SELECTED_ASSERTION_IN_GRID_VIEW)))
                    {
                        //Popolo la griglia delle asserzioni
                        BuildGrdAssertions();

                        if (GrdAssertionResult.Rows != null && GrdAssertionResult.Rows.Count > 0)
                        {
                            GrdAssertionResult.DataSource = null;
                            GrdAssertionResult.DataBind();
                            this.grdAssertionResult_rowindex.Value = "0";
                            this.txt_codice.Text = string.Empty;
                            this.txt_descrizione.Text = string.Empty;
                        }
                        //Popolo il pannello dell'asserzione
                        if (!MODE.Equals("New"))
                            BuildPanelAssertion();
                        this.UpdatePanelGridView.Update();
                        this.UpPnlCodiceDescrizione.Update();
                        return;
                    }
                }
                if (this.Request.Form["__EVENTTARGET"] != null && this.Request.Form["__EVENTTARGET"].Equals(UPDATE_PANEL_RESULT))
                {
                    if (this.Request.Form["__EVENTARGUMENT"] != null && (this.Request.Form["__EVENTARGUMENT"].Equals(BTN_CERCA)))
                    {
                        Search();
                        return;
                    }
                    if (this.Request.Form["__EVENTARGUMENT"] != null && (this.Request.Form["__EVENTARGUMENT"].Equals(UPDATE_GRD_AGGREGATES)))
                    {
                        GrdAssertionResult.DataSource = BuildObjectAggregatorRole(ListResultAur);
                        GrdAssertionResult.DataBind();
                        this.GrdAssertionResult.SelectedIndex = Convert.ToInt32(this.grdAssertionResult_rowindex.Value);
                        HighlightSelectedRow(GrdAssertionResult);
                        upPnlResult.Update();
                        return;
                    }
                }
                if (this.Request.Form["__EVENTTARGET"] != null && this.Request.Form["__EVENTTARGET"].Equals(UPDATE_PANEL_CODICE_DESCRIZIONE))
                {
                    if (GrdAssertionResult.Rows != null && GrdAssertionResult.Rows.Count > 0)
                    {
                        GrdAssertionResult.DataSource = null;
                        GrdAssertionResult.DataBind();
                        txt_codice.Text = string.Empty;
                        txt_descrizione.Text = string.Empty;
                        this.grdAssertionResult_rowindex.Value = "0";
                    }
                    if (!ddlAur.SelectedItem.Value.Equals(TYPE_AUR_ADMINISTRATION))
                    {
                        pnlCodiceDescrizione.Attributes["style"] = "display:block";
                        txt_codice.Text = string.Empty;
                        txt_descrizione.Text = string.Empty;
                        //pnlCodiceDescrizione.Visible = true;
                    }
                    else
                    {
                        pnlCodiceDescrizione.Attributes["style"] = "display:none";
                        txt_codice.Text = string.Empty;
                        txt_descrizione.Text = string.Empty;
                        //pnlCodiceDescrizione.Visible = false;
                    }
                    return;
                }
                /*
                //Popolo la griglia delle asserzioni
                BuildGrdAssertions();
                this.UpdatePanelGridView.Update();
                //Popolo il pannello dell'asserzione
                if(!MODE.Equals("New"))
                    BuildPanelAssertion();*/
            }
        }

        private void Initialize()
        {
            RemovePropertyInSession();
            this.grid_rowindex.Value = "0";
            this.grdAssertionResult_rowindex.Value = "0";
            ddlAur.Attributes.Add("onChange", "__doPostBack('UpPnlCodiceDescrizione');return false;");
            
        }

        #region grid Assertions

        private void BuildGrdAssertions()
        {
            GrdAsserzioni.DataSource = Assertions;
            this.GrdAsserzioni.SelectedIndex = Convert.ToInt32(this.grid_rowindex.Value);
            GrdAsserzioni.DataBind();
            HighlightSelectedRow(this.GrdAsserzioni);
            if (GrdAsserzioni.Rows == null || GrdAsserzioni.Rows.Count == 0)
            {
                this.panelLegend.Visible = false;
                this.lblListAssertions.Text = "Non sono presenti asserzioni";
                MODE = "Hidden";
                this.UpPanelLabelNoGrid.Update();
                this.UpPnlLegend.Update();
            }
            else
            {
                if (GrdAsserzioni.Rows.Count == 1)
                {
                    this.panelLegend.Visible = true;
                    this.lblListAssertions.Text = "Lista Asserzioni";
                    this.UpPanelLabelNoGrid.Update();
                    this.UpPnlLegend.Update();
                }
                if (ListSystemIdAssertionToRemove != null && ListSystemIdAssertionToRemove.Count > 0)
                { 
                    foreach(GridViewRow row in GrdAsserzioni.Rows)
                    {
                        (row.FindControl("cbElimina") as CheckBox).Checked = (from systemIdAssertion in ListSystemIdAssertionToRemove
                                                                              where systemIdAssertion.Equals((row.FindControl("lblSystemId") as Label).Text)
                                                                              select systemIdAssertion).Count() > 0;
                    }
                }
                MODE = "Modify";
            }
        }
        
        /// <summary>
        /// Build grid assertions
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void GrdAsserzioni_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                for (int i = 0; i < e.Row.Cells.Count-1; i++)
                {
                    e.Row.Cells[i].Attributes["onClick"] = "document.getElementById('grid_rowindex').value = '" + e.Row.RowIndex.ToString() + "';SetDdlAur();__doPostBack('UpdatePanelGridView','selectedAssertionInGridView');return false;";
                }
                Assertion assertion = e.Row.DataItem as Assertion;
                if (assertion.CONFIG_TYPE_EVENT.ToUpper().Equals(NO_NOTIFICABLE))
                {
                    e.Row.Style.Add("color", "Red");
                }
                else if (assertion.CONFIG_TYPE_EVENT.ToUpper().Equals(OBLIGATORY))
                {
                    e.Row.Style.Add("color", "Gray");
                }
                //(e.Row.FindControl("cbElimina") as CheckBox).Attributes["OnClick"] = "document.getElementById('rowIndexChecked').value ='" + e.Row.RowIndex.ToString() + "';__doPostBack('UpdatePanel');return false;";
                (e.Row.FindControl("cbElimina") as CheckBox).Attributes["class"] =  e.Row.RowIndex.ToString();
            }
        }

        /// <summary>
        /// Selected row
        /// </summary>
        protected void HighlightSelectedRow(GridView grd)
        {
            if (grd.Rows.Count > 0 && grd.SelectedRow != null)
            {
                GridViewRow gvRow = grd.SelectedRow;
                foreach (GridViewRow GVR in grd.Rows)
                {
                    if (GVR == gvRow)
                    {
                        GVR.CssClass = "bg_grigioS";
                    }
                    else
                    {
                        GVR.CssClass = "bg_grigioA";
                    }
                }
            }
        }

        public string GetTypeEvent(DocsPaWR.Assertion assertion)
        {
            return assertion.DESC_TYPE_EVENT;
        }

        public string GetDescAur(DocsPaWR.Assertion assertion)
        {
            return assertion.DESC_AUR;
        }

        public string GetTypeAur(DocsPaWR.Assertion assertion)
        {
            switch (assertion.TYPE_AUR)
            {
                case TYPE_AUR_ADMINISTRATION:
                    return "Amministrazione";

                case TYPE_AUR_RF:
                    return "RF";

                case TYPE_AUR_ROLE:
                    return "Ruolo";

                case TYPE_AUR_ROLETYPE:
                    return "Tipo ruolo";

                case TYPE_AUR_UO:
                    return "UO";

                default:
                    return string.Empty;
            }
        }

        public string GetNotificationType(DocsPaWR.Assertion assertion)
        {
            if (assertion.TYPE_NOTIFY == Convert.ToChar(OPERATIONAL))
                return "Operativa";
            else if (assertion.TYPE_NOTIFY == Convert.ToChar(INFORMATION))
                return "Informativa";
            else
                return string.Empty;
        }

        public string GetInExercise(DocsPaWR.Assertion assertion)
        {
            if (assertion.IS_EXERCISE)
                return "SI";
            else
                return "NO";
        }

        public string GetSystemId(DocsPaWR.Assertion assertion)
        {
            return assertion.SYSTEM_ID.ToString();
        }

        private bool CheckValue()
        {
            if (ddlTypeEvent.SelectedItem == null || ddlTypeEvent.SelectedItem.Value.Equals(string.Empty))
            {
                string s = "<script language='javascript'>alert('Inserire un tipo evento valido.');</script>";
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "errorTypeEvent", s, false);
                return false;
            }
            if (!ddlAur.SelectedItem.Value.Equals(TYPE_AUR_ADMINISTRATION) &&
                (GrdAssertionResult.Rows == null ||
                GrdAssertionResult.Rows.Count == 0 ||
                GrdAssertionResult.SelectedRow == null) &&
                (!MODE.Equals("Modify")))
            {
                string s = "<script language='javascript'>alert('Selezionare un aggregatore di ruolo valido.');</script>";
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "errorTypeEvent", s, false);
                return false;
            }
            //se siamo in modifica e viene modificato il tipo aggregato di ruolo ma non viene modificato 
            //l'aggregato visualizzo il msg di errore ed interrompo la modifica
            if (MODE.Equals("Modify") &&
                (GrdAssertionResult.Rows == null || GrdAssertionResult.Rows.Count < 1) &&
                (!ddlAur.SelectedItem.Text.Equals(
                    (this.GrdAsserzioni.Rows[this.GrdAsserzioni.SelectedIndex].FindControl("lblArg") as Label).Text)) &&
                (!ddlAur.SelectedValue.Equals(TYPE_AUR_ADMINISTRATION)))
            {
                string s = "<script language='javascript'>alert('Attenzione!Non è stato possibile modificare l\\'asserzione. Compilare correttamente tutti i campi.');</script>";
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "errorModifyAur", s, false);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Insert assertions
        /// </summary>
        private void InsertAssertion()
        {
            if (CheckValue()) // se supero i controlli sui valori immessi nel pannello asserzione, preparo l'oggetto asserzione da inserire
            {
                Assertion newAssertion = new Assertion();
                newAssertion.ID_TYPE_EVENT = Convert.ToInt64(ddlTypeEvent.SelectedItem.Value);
                newAssertion.DESC_TYPE_EVENT = ddlTypeEvent.SelectedItem.Text;
                newAssertion.TYPE_AUR = ddlAur.SelectedItem.Value;
                if(newAssertion.TYPE_AUR.Equals(TYPE_AUR_ADMINISTRATION))
                {
                    newAssertion.ID_AUR = Convert.ToInt64(ID_ADMINISTRATION);
                    newAssertion.DESC_AUR = CODE_ADMINISTRATION;
                }
                else
                {
                    newAssertion.ID_AUR = Convert.ToInt64((GrdAssertionResult.SelectedRow.FindControl("lblSystemIdAggregator") as Label).Text);
                    newAssertion.DESC_AUR = (GrdAssertionResult.SelectedRow.FindControl("lblCode") as Label).Text + "(" +
                        (GrdAssertionResult.SelectedRow.FindControl("lblDescription") as Label).Text + ")";
                }

                newAssertion.TYPE_NOTIFY = Convert.ToChar(rbTipologiaNotifica.SelectedItem.Value);
                newAssertion.IS_EXERCISE = cbInEsercizio.Checked;
                newAssertion.ID_AMM = Convert.ToInt64(ID_ADMINISTRATION);
                //chiamo il servizio di insert
                //0 - inserimento riuscito con successo
                //1 - l'asserzione è già presente
                //-1 - si è verificato un errore durante l'inserimento dell'asserzione.
                int valRet = ws.InsertAssertionEvent(newAssertion);
                if (valRet == 0)
                { 
                    //recupero le asserzioni dal backend
                    Assertions = ws.GetListAssertion(ID_ADMINISTRATION).ToList();
                    //aggiorno la griglia delle asserzioni
                    this.grid_rowindex.Value = "0";
                    BuildGrdAssertions();
                    MODE = "Modify";
                    BuildPanelAssertion();
                    GrdAssertionResult.DataSource = null;
                    GrdAssertionResult.DataBind();
                    this.grdAssertionResult_rowindex.Value = "0";
                    pnlAssertion.Update();
                    UpdatePanelGridView.Update();
                }
                else if (valRet == -1)
                {
                    string s = "<script language='javascript'>alert('Si è verificato un errore durante l\\'inserimento dell\\'asserzione.');</script>";
                    ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "errorInsertAssertion", s, false);
                }
                else if (valRet == 1)
                {
                    string s = "<script language='javascript'>alert('l\\'asserzione è già presente.');</script>";
                    ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "duplicateAssertion", s, false);
                }
            }
        }

        /// <summary>
        /// Modify assertion
        /// </summary>
        private void ModifyAssertion()
        {
            Assertion assertion = new Assertion();
            if (CheckValue())
            {
                assertion.SYSTEM_ID = Convert.ToInt64((this.GrdAsserzioni.Rows[this.GrdAsserzioni.SelectedIndex].FindControl("lblSystemId") as Label).Text);
                assertion.ID_TYPE_EVENT = Convert.ToInt64(ddlTypeEvent.SelectedItem.Value);
                assertion.DESC_TYPE_EVENT = ddlTypeEvent.SelectedItem.Text;
                assertion.ID_AMM = Convert.ToInt64(ID_ADMINISTRATION);
                if (GrdAssertionResult.Rows == null || GrdAssertionResult.Rows.Count < 1)
                {
                    if(ddlAur.SelectedItem.Value.Equals(TYPE_AUR_ADMINISTRATION))
                    {
                        assertion.DESC_AUR = CODE_ADMINISTRATION;
                        assertion.ID_AUR = Convert.ToInt64(ID_ADMINISTRATION);
                    }
                    else
                    {
                        assertion.DESC_AUR = (this.GrdAsserzioni.Rows[this.GrdAsserzioni.SelectedIndex].FindControl("lblAur") as Label).Text;
                        assertion.ID_AUR = (from a in Assertions
                                        where a.SYSTEM_ID.ToString().Equals(assertion.SYSTEM_ID.ToString())
                                        select a.ID_AUR).FirstOrDefault();
                    }
                }
                else
                {
                    assertion.DESC_AUR = (this.GrdAssertionResult.Rows[this.GrdAssertionResult.SelectedIndex].FindControl("lblCode") as Label).Text + 
                        "(" + (this.GrdAssertionResult.Rows[this.GrdAssertionResult.SelectedIndex].FindControl("lblDescription") as Label).Text + ")";
                    assertion.ID_AUR = Convert.ToInt64((this.GrdAssertionResult.Rows[this.GrdAssertionResult.SelectedIndex].FindControl("lblSystemIdAggregator") as Label).Text);
                }
                assertion.IS_EXERCISE = cbInEsercizio.Checked;
                assertion.TYPE_AUR = ddlAur.SelectedValue;
                assertion.TYPE_NOTIFY = Convert.ToChar(rbTipologiaNotifica.SelectedValue);

                bool isPresent = (from a in Assertions
                                     where (a.ID_TYPE_EVENT.Equals(assertion.ID_TYPE_EVENT)
                                     && a.ID_AUR.Equals(assertion.ID_AUR)
                                     && a.TYPE_AUR.Equals(assertion.TYPE_AUR)
                                     && a.ID_AMM.Equals(assertion.ID_AMM) && !a.SYSTEM_ID.ToString().Equals(assertion.SYSTEM_ID.ToString()))
                                     select a).Count() > 0;
                if (isPresent)
                {
                    string s = "<script language='javascript'>alert('L\\'asserzione è già presente.');</script>";
                    ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "duplicateAssertion", s, false);
                    return;
                }
                //service modify
                int res = ws.UpdateAssertionEvent(assertion);
                if (res == 0)
                {
                    //recupero le asserzioni dal backend
                    Assertions = ws.GetListAssertion(ID_ADMINISTRATION).ToList();
                    //aggiorno la griglia delle asserzioni
                    BuildGrdAssertions();
                    MODE = "Modify";
                    BuildPanelAssertion();
                    GrdAssertionResult.DataSource = null;
                    GrdAssertionResult.DataBind();
                    this.grdAssertionResult_rowindex.Value = "0";
                    pnlAssertion.Update();
                    UpdatePanelGridView.Update();
                }
                else if (res == -1)
                {
                    string s = "<script language='javascript'>alert('Si è verificato un problema durante la modifica dell\\'asserzione selezionata');</script>";
                    ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "errorUpdateAssertion", s, false);
                }
                //else if (res == 1)
                //{
                //    string s = "<script language='javascript'>alert('l\\'asserzione è già presente.');</script>";
                //    ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "duplicateAssertion", s, false);
                //}
            }
        }

        #endregion

        #region Assertion

        private void BuildPanelAssertion()
        {
            if(MODE.Equals("Hidden"))
            {
                //pnlAssertion.Visible = false;
                this.panelAssertion.Attributes["style"] = "display:none";
            }

            else if (MODE.Equals("Modify") && this.GrdAsserzioni != null && this.GrdAsserzioni.Rows.Count > 0) // se è presente almeno un elemento sono in modalità modifica 
            {
                string systemId = (this.GrdAsserzioni.Rows[this.GrdAsserzioni.SelectedIndex].FindControl("lblSystemId") as Label).Text;
                Assertion assertion = (from a in Assertions where a.SYSTEM_ID.ToString().Equals(systemId) select a).FirstOrDefault();

                if (assertion != null)
                {
                    if (ddlTypeEvent.Items != null && ddlTypeEvent.Items.Count > 0)
                    {
                        ddlTypeEvent.ClearSelection();
                        foreach (ListItem li in ddlTypeEvent.Items)
                        {
                            if (li.Value.Equals(assertion.ID_TYPE_EVENT.ToString()))
                            {
                                li.Selected = true;
                                break;
                            }
                        }
                        if (ddlTypeEvent.SelectedItem == null)
                        {
                            ddlTypeEvent.SelectedIndex = 0;
                        }

                        ddlAur.ClearSelection();
                        foreach (ListItem li in ddlAur.Items)
                        {
                            if (li.Value.Equals(assertion.TYPE_AUR))
                            {
                                li.Selected = true;
                            }
                        }

                        if (!assertion.TYPE_AUR.Equals(TYPE_AUR_ADMINISTRATION))
                        {
                            this.pnlCodiceDescrizione.Attributes["style"] = "display:block";
                            string [] tmp = assertion.DESC_AUR.Split(new string[]{"("}, 2, StringSplitOptions.None);
                            txt_codice.Text = tmp[0];
                            txt_descrizione.Text = tmp[1].Replace(")", string.Empty);
                        }
                        else
                        {
                            this.pnlCodiceDescrizione.Attributes["style"] = "display:none";
                        }
                        rbTipologiaNotifica.ClearSelection();
                        foreach (ListItem li in rbTipologiaNotifica.Items)
                        {
                            if (li.Value.Equals(assertion.TYPE_NOTIFY.ToString()))
                            {
                                li.Selected = true;
                            }
                        }

                        if (assertion.IS_EXERCISE)
                        {
                            cbInEsercizio.Checked = true;
                        }
                        else
                        {
                            cbInEsercizio.Checked = false;
                        }
                        btnModificaInserisci.Text = "Modifica";
                    }
                    else
                    {
                        ddlTypeEvent.Enabled = false;
                        ddlAur.Enabled = false;
                        pnlCodiceDescrizione.Attributes["style"] = "display:none";
                        rbTipologiaNotifica.Enabled = false;
                        cbInEsercizio.Checked = false;
                        cbInEsercizio.Enabled = false;
                        btnModificaInserisci.Text = "Modifica";
                        btnModificaInserisci.Enabled = false;
                    }
                }
                
            }
            else if (MODE.Equals("New"))
            {
                //pnlAssertion.Visible = true;
                this.panelAssertion.Attributes["style"] = "display:block";
                if (ddlTypeEvent.Items != null && ddlTypeEvent.Items.Count > 0)
                {
                    ddlTypeEvent.ClearSelection();
                    ddlTypeEvent.Items[0].Selected = true;
                    ddlAur.ClearSelection();
                    ddlAur.Items[0].Selected = true;
                    pnlCodiceDescrizione.Attributes["style"] = "display:none";
                    rbTipologiaNotifica.ClearSelection();
                    rbTipologiaNotifica.Items[0].Selected = true;
                    cbInEsercizio.Checked = false;
                    btnModificaInserisci.Text = "Inserisci";
                }
                else
                {
                    ddlTypeEvent.Enabled = false;
                    ddlAur.Enabled = false;
                    pnlCodiceDescrizione.Attributes["style"] = "display:none";
                    //pnlCodiceDescrizione.Visible = false;
                    rbTipologiaNotifica.Enabled = false;
                    cbInEsercizio.Checked = false;
                    cbInEsercizio.Enabled = false;
                    btnModificaInserisci.Enabled = false;
                    btnModificaInserisci.Text = "Inserisci";
                }
            }
            this.pnlAssertion.Update();
        }


        private void BuildDdlTypeConfigurableEvents()
        {
            List<string> listTypeEventConf = ws.GetTypeConfigurableEvents(ID_ADMINISTRATION).ToList();
            if (listTypeEventConf != null && listTypeEventConf.Count > 0)
            {
                ddlTypeEvent.Items.Clear();
                foreach (string eventType in listTypeEventConf)
                {
                    string[] tmp = eventType.Split(new string[] { "#" }, 2, StringSplitOptions.None);
                    ddlTypeEvent.Items.Add(new ListItem() { Value = tmp[0].Trim(), Text = tmp[1].Trim() });
                }
                pnlAssertion.Update();
            }
        }

        protected void GrdAsserzioni_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            this.GrdAsserzioni.PageIndex = e.NewPageIndex;
            this.grid_rowindex.Value = "0";
            BuildGrdAssertions();
            this.UpdatePanelGridView.Update();
        }

        #endregion

        #region grid assertion result

        protected void GrdAssertionResult_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                for (int i = 0; i < e.Row.Cells.Count; i++)
                {

                    e.Row.Cells[i].Attributes["onClick"] = "document.getElementById('grdAssertionResult_rowindex').value = '" + Convert.ToInt32(e.Row.RowIndex.ToString()) + "';__doPostBack('upPnlResult','updateGrdAggregates');return false;";
                }
            }
        }

        protected void GrdAssertionResult_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            this.GrdAssertionResult.PageIndex = e.NewPageIndex;
            this.grdAssertionResult_rowindex.Value = "0";
            GrdAssertionResult.DataSource = BuildObjectAggregatorRole(ListResultAur);
            GrdAssertionResult.DataBind();
            this.GrdAssertionResult.SelectedIndex = Convert.ToInt32(this.grdAssertionResult_rowindex.Value);
            HighlightSelectedRow(GrdAssertionResult);
            upPnlResult.Update();
        }


        /// <summary>
        /// Search aggregates role
        /// </summary>
        private void Search()
        {
            this.panelLabelNoResult.Visible = false;
            if (string.IsNullOrEmpty(this.txt_codice.Text) && string.IsNullOrEmpty(this.txt_descrizione.Text))
            {
                string s = "<script language='javascript'>alert('Attenzione! Per poter effettuare la ricerca è necessario compilare i campi codice e/o descrizione');</script>";
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "WarningSearch", s, false);
            }
            else
            {
                string typeAur = ddlAur.SelectedItem.Value;
                string code = txt_codice.Text;
                string description = txt_descrizione.Text;
                ListResultAur = ws.SearchAur(typeAur, code, description, ID_ADMINISTRATION).ToList();
                if(ListResultAur != null && ListResultAur.Count > 0)
                {
                    this.upPnlResult.Visible = true;
                    GrdAssertionResult.DataSource = BuildObjectAggregatorRole(ListResultAur);
                    GrdAssertionResult.DataBind();
                    this.GrdAssertionResult.SelectedIndex = Convert.ToInt32(this.grdAssertionResult_rowindex.Value);
                    HighlightSelectedRow(GrdAssertionResult);
                    upPnlResult.Update();
                }
                else
                {
                    GrdAssertionResult.DataSource = null;
                    GrdAssertionResult.DataBind();
                    this.panelLabelNoResult.Visible = true;
                }
            }
            this.upPnlResult.Update();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        private List<AggregatorRole>  BuildObjectAggregatorRole(List<string> list)
        {
            List<AggregatorRole> listAggregatorRole = new List<AggregatorRole>();
            foreach (string aggr in list)
            {
                string[] tmp = aggr.Split(new string[] { "#" }, 3, StringSplitOptions.None);
                listAggregatorRole.Add(new AggregatorRole()
                {
                    SYSTEM_ID = tmp[0].Trim(),
                    CODE = tmp[1].Trim(),
                    DESCRIPTION = tmp[2].Trim()
                }); 
            }
            return listAggregatorRole;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public string GetSystemIdAggregatorRole(AggregatorRole aggregator)
        {
            return aggregator.SYSTEM_ID;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public string GetCodeAggregatorRole(AggregatorRole aggregator)
        {
            return aggregator.CODE;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public string GetDescriptionAggregatorRole(AggregatorRole aggregator)
        {
            return aggregator.DESCRIPTION;
        }
        #endregion

        #region buttons

        protected void btnNuovo_Click(Object sender, EventArgs e)
        {
            MODE = "New";
            BuildPanelAssertion();
        }

        private void RemoveAssertion()
        {
            if (ListSystemIdAssertionToRemove != null && ListSystemIdAssertionToRemove.Count > 0)
            {
                List<Assertion> listAssertionToRemove = (from assertion in Assertions where ListSystemIdAssertionToRemove.Contains(assertion.SYSTEM_ID.ToString()) select assertion).ToList();
                if (ws.RemoveAssertions(listAssertionToRemove.ToArray()))
                {
                    Assertions = (from assertion in Assertions where !ListSystemIdAssertionToRemove.Contains(assertion.SYSTEM_ID.ToString()) select assertion).ToList();
                    this.grid_rowindex.Value = "0"; 
                    BuildGrdAssertions();
                    BuildPanelAssertion();
                    HttpContext.Current.Session.Remove("ListSystemIdAssertionToRemove");
                }
            }
        }

        protected void btnModificaInserisci_Click(Object sender, EventArgs e)
        {
            if(MODE.Equals("New"))
            {
                InsertAssertion();
            }
            else if(MODE.Equals("Modify"))
            {
                ModifyAssertion();
            }
        }

        protected void cbElimina_OnCheckedChanged(Object sender, EventArgs e)
        {
            CheckBox c = (sender as CheckBox);
            string rowIndexChecked = c.Attributes["class"];
            GridViewRow row = GrdAsserzioni.Rows[Convert.ToInt32(rowIndexChecked)];
            string systemIdAssertion = (row.FindControl("lblSystemId") as Label).Text;
            if ((row.FindControl("cbElimina") as CheckBox).Checked)
            {
                if (ListSystemIdAssertionToRemove == null)
                {
                    ListSystemIdAssertionToRemove = new List<string>();
                }
                ListSystemIdAssertionToRemove.Add(systemIdAssertion);
            }
            else
            {
                ListSystemIdAssertionToRemove.Remove(systemIdAssertion);
            }
        }
        
        #endregion
    }


    /// <summary>
    /// This class implements the concept of the role of aggregator
    /// </summary>
    public class AggregatorRole
    {
        private string _systemId;
        private string _code;
        private string _description;

        public string SYSTEM_ID
        {
            get
            {
                return _systemId;
            }

            set
            {
                _systemId = value;
            }
        }

        public string CODE
        {
            get 
            {
                return _code;
            }
            
            set
            {
                _code = value;
            }
        }

        public string DESCRIPTION
        {
            get
            {
                return _description;
            }

            set
            {
                _description = value;
            }
        }
    }
}
