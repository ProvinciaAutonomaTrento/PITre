using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.DocsPaWR;
using NttDataWA.UIManager;
using NttDataWA.Utils;

namespace NttDataWA.Popup
{
    public partial class HistoryPreserved : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try {
                if (!IsPostBack)
                {
                    this.InitializeLanguage();
                    this.InitializePage();
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void InitializeLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            
            string labelNotFound = string.Empty;
            string labelCaption = string.Empty;
            this.HistoryPreservedBtnChiudi.Text = Utils.Languages.GetLabelFromCode("HistoryPreservedBtnChiudi", language);

            if (Convert.ToString(Request.QueryString["typeObject"]).Equals("D"))
            {
                labelNotFound = "HistroyPreservedDocLblNotFound";
                labelCaption = "HistroyPreservedDocLblCaption";
            }
            else if (Convert.ToString(Request.QueryString["typeObject"]).Equals("F"))
            {
                labelNotFound = "HistroyPreservedPrjLblNotFound";
                labelCaption = "HistroyPreservedPrjLblCaption";
            }

            this.HistroyPreservedLblNotFound.Text = Utils.Languages.GetLabelFromCode(labelNotFound, language);
            this.HistroyPreservedLblCaption.Text = Utils.Languages.GetLabelFromCode(labelCaption, language);
        }

        private void InitializePage()
        {
            if (Request.QueryString["typeObject"] != null && Request.QueryString["typeObject"] != string.Empty)
            {
                string typeObject = Request.QueryString["typeObject"].ToString();
                Session["TypeObject"] = typeObject;

                CreateDataGridHistory();
            }
        }

        private void CreateDataGridHistory()
        {
            DocsPaWR.DettItemsConservazione[] dettItemsCons = null;

            if (Session["TypeObject"].Equals("D"))
            {
                dettItemsCons = DocumentManager.getStoriaConsDoc(DocumentManager.getSelectedRecord().systemId);
            }
            else if (Session["TypeObject"].Equals("F"))
            {
                dettItemsCons = ProjectManager.getStoriaConsFasc(ProjectManager.getProjectInSession().systemID);
            }

            if (dettItemsCons != null && dettItemsCons.Length > 0)
            {
                string language = UIManager.UserManager.GetUserLanguage();
                var filtered_data = (from f in dettItemsCons
                                     select new
                                     {
                                         ID_Istanza = GetID(f.IdConservazione),
                                         Descrizione = GetDescrizione(f.Descrizione),
                                         Data_Conservazione = GetRegistro(f.Data_riversamento),
                                         Utente = GetUtente(f.UserId),
                                         Collocazione = GetCollocazione(f.CollocazioneFisica),
                                         Tipo_Conservazione = GetConservazione(f.tipo_cons),
                                         Num_Doc_In_Fasc = GetNumDoc(f.num_docInFasc),
                                         IdSel = f.id_profile_trasm,
                                         Tooltip0 = Utils.Languages.GetLabelFromCode("HistoryPreservedTooltipCell0", language),
                                         Tooltip1 = Utils.Languages.GetLabelFromCode("HistoryPreservedTooltipCell1", language),
                                         Tooltip2 = Utils.Languages.GetLabelFromCode("HistoryPreservedTooltipCell2", language),
                                         Tooltip3 = Utils.Languages.GetLabelFromCode("HistoryPreservedTooltipCell3", language),
                                         Tooltip4 = Utils.Languages.GetLabelFromCode("HistoryPreservedTooltipCell4", language),
                                         Tooltip5 = Utils.Languages.GetLabelFromCode("HistoryPreservedTooltipCell5", language),
                                         Tooltip6 = Utils.Languages.GetLabelFromCode("HistoryPreservedTooltipCell6", language)
                                     }).ToArray();

                this.GridViewHistory.DataSource = filtered_data;
                this.GridViewHistory.DataBind();

                this.GridViewHistory.HeaderRow.Cells[0].Text = Utils.Languages.GetLabelFromCode("HistoryPreservedHeaderGrid0", language);
                this.GridViewHistory.HeaderRow.Cells[1].Text = Utils.Languages.GetLabelFromCode("HistoryPreservedHeaderGrid1", language);
                this.GridViewHistory.HeaderRow.Cells[2].Text = Utils.Languages.GetLabelFromCode("HistoryPreservedHeaderGrid2", language);
                this.GridViewHistory.HeaderRow.Cells[3].Text = Utils.Languages.GetLabelFromCode("HistoryPreservedHeaderGrid3", language);
                this.GridViewHistory.HeaderRow.Cells[4].Text = Utils.Languages.GetLabelFromCode("HistoryPreservedHeaderGrid4", language);
                this.GridViewHistory.HeaderRow.Cells[5].Text = Utils.Languages.GetLabelFromCode("HistoryPreservedHeaderGrid5", language);
                this.GridViewHistory.HeaderRow.Cells[7].Text = Utils.Languages.GetLabelFromCode("HistoryPreservedHeaderGrid7", language);

                if (Session["TypeObject"].Equals("D"))
                {
                    this.GridViewHistory.Columns[6].Visible = false;
                }
                else
                {
                    this.GridViewHistory.Columns[6].Visible = true;
                    this.GridViewHistory.HeaderRow.Cells[6].Text = Utils.Languages.GetLabelFromCode("HistoryPreservedHeaderGrid6", language);
                }

                this.GridViewHistory.Visible = true;
            }
            else
            {
                this.HistroyPreservedPnlNotFound.Visible = true;
            }
        }


        protected string GetID(string valID)
        {
            string result = valID;
            return result;
        }

        protected string GetDescrizione(string valDescrizione)
        {
            string result = valDescrizione;
            return result;
        }

        protected string GetRegistro(string valRegistro)
        {
            string result = valRegistro;
            return result;
        }

        protected string GetUtente(string valUtente)
        {
            string result = valUtente;
            return result;
        }

        protected string GetCollocazione(string valCollocazione)
        {
            string result = valCollocazione;
            return result;
        }

        protected string GetConservazione(string valConservazione)
        {
            string result = string.Empty;
            DocsPaWR.TipoIstanzaConservazione[] cons = UIManager.PreservetionManager.GetTipologieIstanzeConservazione();
                for (int t = 0; t < cons.Length; t++)
                {
                    string tipo_cons = valConservazione;
                    if (tipo_cons.Equals(cons[t].Codice))
                    {
                        result = cons[t].Descrizione;
                    }
                }           
            return result;
        }

        protected string GetNumDoc(string valNumDoc)
        {
            string result = valNumDoc;
            return result;
        }
        
        protected void HistoryPreservedBtnChiudi_Click(object sender, EventArgs e)
        {
            try {
                ScriptManager.RegisterClientScriptBlock(this.UpPnlButtons, this.UpPnlButtons.GetType(), "closeAJM", "parent.closeAjaxModal('HistoryPreserved','');", true);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void RowSelected_RowCommand(object sender, GridViewRowEventArgs e)
        {
            try {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    string idProfile_trasm = ((Label)e.Row.FindControl("LblIdSel")).Text;
                    string language = UIManager.UserManager.GetUserLanguage();

                    if (!string.IsNullOrEmpty(idProfile_trasm))
                    {
                        DocsPaWR.SchedaDocumento schedaDoc = DocumentManager.getDocumentDetails(this, idProfile_trasm, "");
                        DocsPaWR.InfoDocumento doc = DocumentManager.getInfoDocumento(schedaDoc);

                        if (doc.tipoProto.ToUpper().Equals("R"))
                        {
                            //Tipologia documento non visualizzabile
                            ((Label)e.Row.FindControl("Comunication")).Text = Utils.Languages.GetLabelFromCode("HistoryPreservedDocNotView", language);
                        }
                        else
                        {
                            // Verifica se l'utente ha i diritti per accedere al documento
                            //int retValue = DocumentManager.verificaACL("D", idProfile_trasm, UserManager.getInfoUtente(), out errorMessage);
                            //if (retValue == 0 || retValue == 1)
                            //{
                            //    string script = ("<script>alert('" + errorMessage + "');</script>");
                            //    Response.Write(script);
                            //}
                            //else
                            //{

                            //Rendo il record cliccabile
                            e.Row.Attributes["onclick"] = "$('#grid_rowindex').val('" + ((Label)e.Row.FindControl("LblIdSel")).Text + "'); __doPostBack('UpGridCorr', ''); return false;";
                            e.Row.Attributes.Add("onmouseover", "this.style.cursor='pointer'");
                            //}
                        }
                    }
                    else
                    {
                        //Non esiste alcuna trasmissione associata al documento
                        ((Label)e.Row.FindControl("Comunication")).Text = Utils.Languages.GetLabelFromCode("HistoryPreservedDocNotTrasm", language);
                    }
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void SetSelectedRow(object sender, EventArgs e)
        {
            try {
                string idProfile_trasm = grid_rowindex.Text;

                if (!string.IsNullOrEmpty(idProfile_trasm))
                {
                    SchedaDocumento doc = DocumentManager.getDocumentDetails(this, idProfile_trasm, idProfile_trasm);
                    DocumentManager.setSelectedRecord(doc);
                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SelectedOk", "parent.closeAjaxModal('HistoryPreserved','');", true);
                    Response.Write("<html><body><script type=\"text/javascript\">parent.closeAjaxModal('HistoryPreserved', 'up');</script></body></html>");
                    Response.End();
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }
    }
}