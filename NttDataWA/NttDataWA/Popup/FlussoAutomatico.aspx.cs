using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.UIManager;
using NttDataWA.DocsPaWR;

namespace NttDataWA.Popup
{
    public partial class FlussoAutomatico : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                InitializeLanguage();
                InitializePage();
            }
        }

        private void InitializeLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.FlussoAutomaticoClose.Text = Utils.Languages.GetLabelFromCode("FlussoAutomaticoClose", language);
            this.LblNoFlussoAutomatico.Text = Utils.Languages.GetLabelFromCode("LblNoFlussoAutomatico", language);
            this.lblIdProcesso.Text = Utils.Languages.GetLabelFromCode("FlussoAutomaticoLblIdProcesso", language);
        }

        private void InitializePage()
        {
            List<DocsPaWR.Flusso> flusso = DocumentManager.GetListaFlussoDocumento(DocumentManager.getSelectedRecord());
            if (flusso != null && flusso.Count > 0)
            {
                this.lblIdProcessoCode.Text = flusso[0].ID_PROCESSO;
                this.pnlNoFlussoAutomatico.Visible = false;
                this.pnlIdProcesso.Visible = true;
                this.GridFlussoAutomatico_Bind(flusso);
            }
            else
            {
                this.pnlNoFlussoAutomatico.Visible = true;
                this.pnlIdProcesso.Visible= false;
            }
        }

        private void GridFlussoAutomatico_Bind(List<DocsPaWR.Flusso> flusso)
        {
            this.GridFlussoAutomatico.DataSource = flusso;
            this.GridFlussoAutomatico.DataBind();
        }

        protected string GetLabelNumProto(DocsPaWR.Flusso flusso)
        {
            if (!string.IsNullOrEmpty(flusso.INFO_DOCUMENTO.NUM_PROTO))
                return "<span style=\"color:Red; font-weight:bold;\">" + flusso.INFO_DOCUMENTO.NUM_PROTO + "</span>";
            else
                return "<span style=\"color:Black; font-weight:bold;\">" + flusso.INFO_DOCUMENTO.ID_PROFILE + "</span>"; 
        }

        protected void FlussoAutomaticoClose_Click(object sender, EventArgs e)
        {
            try
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);
                ScriptManager.RegisterStartupScript(this, this.GetType(), "closeMask", "if (parent.fra_main) {parent.fra_main.closeAjaxModal('FlussoAutomatico', '');} else {parent.closeAjaxModal('FlussoAutomatico', '', parent);};", true);
            }
            catch (Exception ex)
            {

            }
        }

        protected void GridFlussoAutomatico_RowDataBound(object sender, EventArgs e)
        {
            try
            {
               
            }
            catch (Exception ex)
            {

            }
        }
        protected void GridFlussoAutomatico_PreRender(object sender, EventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {

            }
        }

        protected void GridFlussoAutomatico_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            GridViewRow row = ((e.CommandSource as Control).Parent.Parent as GridViewRow);
            switch (e.CommandName)
            {
                case "ViewDocument":
                    string idProfile = (row.FindControl("LblIdProfile") as Label).Text;
                    if (idProfile.Equals(DocumentManager.getSelectedRecord().docNumber))
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "closeMask", "if (parent.fra_main) {parent.fra_main.closeAjaxModal('FlussoAutomatico', '');} else {parent.closeAjaxModal('FlussoAutomatico', '', parent);};", true);
                    }
                    else
                    {
                        SchedaDocumento schedaDocumento = UIManager.DocumentManager.getDocumentDetails(this.Page, idProfile, idProfile);
                        DocumentManager.setSelectedRecord(schedaDocumento);
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "closeMask", "if (parent.fra_main) {parent.fra_main.closeAjaxModal('FlussoAutomatico', 'up');} else {parent.closeAjaxModal('FlussoAutomatico', 'up', parent);};", true);
                    }
                    return;
            }
        }

        protected void GridFlussoAutomatico_ItemCreated(object sender, GridViewRowEventArgs e)
        {
            try
            {

            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }
    }
}