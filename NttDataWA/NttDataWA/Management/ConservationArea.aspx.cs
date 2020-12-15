using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.UIManager;
using NttDataWA.DocsPaWR;
using NttDatalLibrary;

namespace NttDataWA.Management
{
    public partial class ConservationArea : System.Web.UI.Page
    {
        #region Properties

        private List<InfoConservazione> InstancesConservation
        {
            get
            {
                if (HttpContext.Current.Session["InstancesConservation"] != null)
                    return (List<InfoConservazione>)HttpContext.Current.Session["InstancesConservation"];
                else
                    return null;
            }
            set
            {
                HttpContext.Current.Session["InstancesConservation"] = value;
            }
        }

        private List<TipoIstanzaConservazione> TypeInstanceConservation
        {
            get
            {
                if (HttpContext.Current.Session["TypeInstanceConservation"] != null)
                    return (List<TipoIstanzaConservazione>)HttpContext.Current.Session["TypeInstanceConservation"];
                else
                    return null;
            }
            set
            {
                HttpContext.Current.Session["TypeInstanceConservation"] = value;
            }
        }

        private InfoConservazione InstanceConservation
        {
            get
            {
                if (HttpContext.Current.Session["InstanceConservazione"] != null)
                    return (InfoConservazione)HttpContext.Current.Session["InstanceConservazione"];
                else
                    return null;
            }
            set
            {
                HttpContext.Current.Session["InstanceConservazione"] = value;
            }
        }

        #region Remove Property

        private void RemoveInstancesConcervation()
        {
            HttpContext.Current.Session.Remove("InstancesConservation");
        }

        private void RemovePropertiesZoom()
        {
            HttpContext.Current.Session.Remove("isZoom");
            HttpContext.Current.Session.Remove("selectedRecord");
            FileManager.removeSelectedFile();
        }

        private void RemovePropertiesViewDetails()
        {
            HttpContext.Current.Session.Remove("InfoConservazione");
            HttpContext.Current.Session.Remove("ItemsConservation");
            HttpContext.Current.Session.Remove("InstanceConservazione");
        }

        #endregion

        #endregion

        #region Const

        private const string REMOVE_INSTANCE = "RemoveInstance";
        private const string VIEW_DETAILS_INSTANCE = "ViewDetailsInstance";
        private const string UP_PANEL_GRID_INSTANCES_CONSERVATION = "UpPanelGridInstancesConservation";
        private const string CLOSE_ZOOM = "closeZoom";
        private const string UP_PANEL_BUTTONS = "UpPnlButtons";
        private const string CLOSE_POPUP_VIEW_DETAILS_INSTANCE_CONSERVATION = "closePopupViewDetailsInstanceConservation";

        #endregion

        #region Standard Method
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ClearSession();
                InitializeLanguage();
                InitializeContent();
            }
            else
            {
                if (this.Request.Form["__EVENTTARGET"] != null && this.Request.Form["__EVENTTARGET"].Equals(UP_PANEL_GRID_INSTANCES_CONSERVATION))
                {
                    HighlightSelectedRow();
                    this.UpPanelGridInstancesConservation.Update();
                    return;
                }
                ReadRetValueFromPopup();
            }
        }

        private void ReadRetValueFromPopup()
        {
            if (this.Request.Form["__EVENTTARGET"] != null && this.Request.Form["__EVENTTARGET"].Equals(UP_PANEL_BUTTONS))
            {

                if (this.Request.Form["__EVENTARGUMENT"] != null && (this.Request.Form["__EVENTARGUMENT"].Equals(CLOSE_ZOOM)))
                {
                    RemovePropertiesZoom();
                    return;
                }
                if (this.Request.Form["__EVENTARGUMENT"] != null && (this.Request.Form["__EVENTARGUMENT"].Equals(CLOSE_POPUP_VIEW_DETAILS_INSTANCE_CONSERVATION)))
                {
                    RemovePropertiesViewDetails();
                    LoadInstancesConservations();
                    GrdInstancesConservation_Bind();
                    this.UpPanelGridInstancesConservation.Update();
                    return;
                }
            }

            if (!string.IsNullOrEmpty(this.HiddenRemoveInstanceConservation.Value))
            {
                string idInstance = (this.GrdInstancesConservation.SelectedRow.FindControl("instanceId") as Label).Text;
                DocumentManager.eliminaDaAreaConservazione(null, null, idInstance, true, "");
                InstancesConservation.Remove((from instanceConservation in InstancesConservation
                                              where instanceConservation.SystemID.Equals(idInstance)
                                              select instanceConservation).FirstOrDefault());
                this.grid_rowindex.Value = "0";
                GrdInstancesConservation_Bind();
                this.UpPanelGridInstancesConservation.Update();
                this.HiddenRemoveInstanceConservation.Value = string.Empty;
                return;
            }

        }

        private void InitializeLanguage()
        {
            string language = UserManager.GetUserLanguage();
            this.ConservationAreaLblListInstancesOfConservation.Text = Utils.Languages.GetLabelFromCode("ConservationAreaLblListInstancesOfConservation", language);
            this.cbInstancesClose.Text = Utils.Languages.GetLabelFromCode("ConservationAreaInstancesClose", language);
            this.cbInstancesAutomatic.Text = Utils.Languages.GetLabelFromCode("ConservationAreaInstancesAutomatic", language);
            this.cbInstancesManuals.Text = Utils.Languages.GetLabelFromCode("ConservationAreaInstancesManuals", language);
            this.ConservationAreaSearch.Text = Utils.Languages.GetLabelFromCode("ConservationAreaSearch", language);
            this.ViewDetailsInstanceConservation.Title = Utils.Languages.GetLabelFromCode("ViewDetailsInstanceConservationTitle", language);
            this.DocumentViewer.Title = Utils.Languages.GetLabelFromCode("TitleDocumentViewerPopup", language);
            
        }

        private void InitializeContent()
        {
            LoadTypeInstanceConservation();
            LoadInstancesConservations();
            this.grid_rowindex.Value = "0";
            GrdInstancesConservation_Bind();
        }

        private void ClearSession()
        {
            RemoveInstancesConcervation();
        }

        protected void LoadTypeInstanceConservation()
        {
            TypeInstanceConservation = DocumentManager.GetTipologieIstanzeConservazione().ToList();
        }

        private void LoadInstancesConservations()
        {
            string filtro = this.Filtra();
            InstancesConservation = DocumentManager.getListaConservazioneByFiltro(filtro).ToList();
        }

        #endregion

        #region Event

        protected void ConservationAreaSearch_Click(object sender, EventArgs e)
        {
            string filtro = this.Filtra();
            InstancesConservation = DocumentManager.getListaConservazioneByFiltro(filtro).ToList();
            this.grid_rowindex.Value = "0";
            GrdInstancesConservation_Bind();
            HighlightSelectedRow();
            this.UpPanelGridInstancesConservation.Update();
        }

        #endregion

        #region Management Grid

        private void GrdInstancesConservation_Bind()
        {
            this.GrdInstancesConservation.DataSource = InstancesConservation;
            this.GrdInstancesConservation.DataBind();
            if (this.GrdInstancesConservation != null && this.GrdInstancesConservation.Rows.Count > 0 &&
                !string.IsNullOrEmpty(grid_rowindex.Value) && int.Parse(this.grid_rowindex.Value) != -1)
            {
                HighlightSelectedRow();
            }
        }

        protected void GrdInstancesConservation_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {

                    for (int i = 0; i < e.Row.Cells.Count - 2; i++)
                    {
                        e.Row.Cells[i].Attributes["onClick"] = "$('#grid_rowindex').val('" + e.Row.RowIndex.ToString() + "');__doPostBack('UpPanelGridInstancesConservation');return false;";
                    }

                    InfoConservazione info = e.Row.DataItem as InfoConservazione;
                    SetLabelStateConservation(e.Row, info);
                    SetLabelCheckResult(e.Row, info);
                    if (info.StatoConservazione != "N" && info.StatoConservazione != "Nuova" && info.StatoConservazione != "R" && info.StatoConservazione != "Respinta" && info.StatoConservazione != "B")
                    {
                        (e.Row.FindControl("ConservationAreaRemoveInstance") as CustomImageButton).Enabled = false;
                    }
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void GrdInstancesConservation_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                this.GrdInstancesConservation.PageIndex = e.NewPageIndex;
                this.grid_rowindex.Value = "0";
                GrdInstancesConservation_Bind();
                this.UpPanelGridInstancesConservation.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void GrdInstancesConservation_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            string idInstance = string.Empty;
            switch (e.CommandName)
            {
                case REMOVE_INSTANCE:
                    idInstance = (((e.CommandSource as CustomImageButton).Parent.Parent as GridViewRow).FindControl("instanceId") as Label).Text;
                    try
                    {
                        this.grid_rowindex.Value = ((e.CommandSource as CustomImageButton).Parent.Parent as GridViewRow).RowIndex.ToString();
                        HighlightSelectedRow();
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxConfirmModal", "ajaxConfirmModal('ConfirmRemoveInstanceNotification', 'HiddenRemoveInstanceConservation', '');", true);
                        return;
                    }
                    catch (System.Exception ex)
                    {
                        UIManager.AdministrationManager.DiagnosticError(ex);
                        return;
                    }
                case VIEW_DETAILS_INSTANCE:
                    idInstance = (((e.CommandSource as CustomImageButton).Parent.Parent as GridViewRow).FindControl("instanceId") as Label).Text;
                    this.grid_rowindex.Value = ((e.CommandSource as CustomImageButton).Parent.Parent as GridViewRow).RowIndex.ToString();
                    HighlightSelectedRow();
                    this.UpPanelGridInstancesConservation.Update();
                    InstanceConservation = (from instanceConservation in InstancesConservation
                                            where instanceConservation.SystemID.Equals(idInstance)
                                            select instanceConservation).FirstOrDefault();
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxModalPopupViewDetailsInstanceConservation", "ajaxModalPopupViewDetailsInstanceConservation();", true);
                    break;
            }
        }

        protected void HighlightSelectedRow()
        {
            this.GrdInstancesConservation.SelectedIndex = int.Parse(this.grid_rowindex.Value);
            if (this.GrdInstancesConservation.Rows.Count > 0 && this.GrdInstancesConservation.SelectedRow != null)
            {
                GridViewRow gvRow = this.GrdInstancesConservation.SelectedRow;
                foreach (GridViewRow GVR in this.GrdInstancesConservation.Rows)
                {
                    if (GVR == gvRow)
                    {
                        GVR.CssClass += " selectedrow";
                    }
                    else
                    {
                        GVR.CssClass = GVR.CssClass.Replace(" selectedrow", "");
                    }
                }
            }
        }

        protected string GetLabelInstanceId(InfoConservazione info)
        {
            string result = info.SystemID;
            if (info.StatoConservazione == "N" || info.StatoConservazione == "Nuova")
            {
                string predefinita = info.predefinita.ToString();
                if (!string.IsNullOrEmpty(predefinita) && predefinita.Equals("True"))
                {
                    result = info.SystemID + "<strong>*</strong>";
                }
            }
            return result;
        }

        /// <summary>
        /// Associa a statoConservazione la stringa corrispondente
        /// </summary>
        /// <param name="row"></param>
        /// <param name="state"></param>
        protected void SetLabelStateConservation(GridViewRow row, InfoConservazione info)
        {
            if (info.StatoConservazione != "N" && info.StatoConservazione != "Nuova" && info.StatoConservazione != "R" && info.StatoConservazione != "Respinta")
            {
                //((System.Web.UI.WebControls.ImageButton)this.gv_istanzeCons.Rows[i].Cells[12].Controls[1]).OnClientClick = "return showModalDialogEliminaIstanza();";
            }
            if (info.StatoConservazione == "N" || info.StatoConservazione == "Nuova")
            {
                ((Label)row.FindControl("conservationState")).Text = "Nuova";
                string predefinita = info.predefinita.ToString();
                if (!string.IsNullOrEmpty(predefinita) && predefinita.Equals("True"))
                {
                    //string idTemp = ((Label)this.gv_istanzeCons.Rows[i].FindControl("lbl_idIstanza")).Text.ToString();
                    //((Label)this.gv_istanzeCons.Rows[i].FindControl("lbl_idIstanzaVis")).Text = idTemp + "<strong>*</strong>";
                }
            }
            if (info.StatoConservazione == "I" || info.StatoConservazione == "Inviata")
            {
                ((Label)row.FindControl("conservationState")).Text = "Inviata";
            }
            if (info.StatoConservazione == "R" || info.StatoConservazione == "Rifiutata")
            {
                ((Label)row.FindControl("conservationState")).Text = "Rifiutata";
                ((Label)row.FindControl("conservationState")).Attributes.Add("style", "color:Red;font-weight:bold;");
            }
            if (info.StatoConservazione == "L" || info.StatoConservazione == "In lavorazione" || info.StatoConservazione == "E")
            {
                ((Label)row.FindControl("conservationState")).Text = "In lavorazione";
            }
            if (info.StatoConservazione == "F" || info.StatoConservazione == "Firmata")
            {
                ((Label)row.FindControl("conservationState")).Text = "Firmata";
            }
            if (info.StatoConservazione == "C" || info.StatoConservazione == "Chiusa")
            {
                ((Label)row.FindControl("conservationState")).Text = "Chiusa";
            }
            if (info.StatoConservazione == "V" || info.StatoConservazione == "Conservata")
            {
                ((Label)row.FindControl("conservationState")).Text = "Conservata";
            }
            if (info.StatoConservazione == "T" || info.StatoConservazione == "In Transizione")
            {
                ((Label)row.FindControl("conservationState")).Text = "In Transizione";
            }
            if (info.StatoConservazione == "Q" || info.StatoConservazione == "In fase di verifica")
            {
                ((Label)row.FindControl("conservationState")).Text = "In fase di verifica";
            }
            if (info.StatoConservazione == "A" || info.StatoConservazione == "Verifica formati in corso")
            {
                ((Label)row.FindControl("conservationState")).Text = "In verifica";
            }
            if (info.StatoConservazione == "B" || info.StatoConservazione == "Verificata")
            {
                ((Label)row.FindControl("conservationState")).Text = "Verificata";
            }
            if (info.StatoConservazione == "Y" || info.StatoConservazione == "In conversione")
            {
                ((Label)row.FindControl("conservationState")).Text = "In conversione";
            }
            if (info.StatoConservazione == "Z" || info.StatoConservazione == "In Errore di conversione formati")
            {
                ((Label)row.FindControl("conservationState")).Text = "Errore Conversione";
            }
        }

        /// <summary>
        /// Associa l?esito della verifica alla riga corrispondente
        /// </summary>
        /// <param name="row"></param>
        /// <param name="state"></param>
        protected void SetLabelCheckResult(GridViewRow row, InfoConservazione info)
        {
            switch (info.esitoVerifica)
            {
                //NON EFFETTUATA
                case 0:
                    ((Label)row.FindControl("checkResult")).Text = "Non Effettuata";
                    break;
                //SUCCESSO
                case 1:
                    ((Label)row.FindControl("checkResult")).Text = "Successo";
                    break;
                //CONVERTIBILE DIRETTAMENTE DALL'UTENTE
                case 2:
                    ((Label)row.FindControl("checkResult")).Text = "Convertibile";
                    break;
                //NON CONVERTIBILE DIRETTAMENTE DALL'UTENTE
                case 3:
                    ((Label)row.FindControl("checkResult")).Text = "Non Convertibile direttamente";
                    break;
                //FALLITA
                case 4:
                    ((Label)row.FindControl("checkResult")).Text = "Fallita";
                    break;
                
                case 5:
                    ((Label)row.FindControl("checkResult")).Text = "Errore";

                    break;
               
            }


        }

        protected string GetLabelTypeConservation(InfoConservazione info)
        {
            return (from type in TypeInstanceConservation where type.Codice.Equals(info.TipoConservazione) select type.Descrizione).FirstOrDefault();
        }

        #endregion

        #region Utils

        protected string Filtra()
        {
            string query = string.Empty;

            query = " WHERE ID_PEOPLE=" + UserManager.GetInfoUser().idPeople + " AND ID_RUOLO_IN_UO=" + UserManager.GetInfoUser().idCorrGlobali;

            bool filterStart = false;

            if (this.cbInstancesClose.Checked)
            {
                if (!filterStart)
                {
                    query += " AND (CHA_STATO ='C' ";
                    filterStart = true;
                }
                else
                {
                    query += " OR CHA_STATO ='C' ";
                }
            }
            if (this.cbInstancesManuals.Checked)
            {
                if (!filterStart)
                {
                    query += " AND (ID_POLICY is null ";
                    filterStart = true;
                }
                else
                {

                    query += " OR ID_POLICY is null ";
                }
            }
            if (this.cbInstancesAutomatic.Checked)
            {
                if (!filterStart)
                {
                    query += " AND (ID_POLICY is not null ";
                    filterStart = true;
                }
                else
                {
                    query += " OR ID_POLICY is not null ";
                }
            }

            if (filterStart)
            {
                query += ")";
            }

            return query;
        }

        #endregion
    }
}