using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SAAdminTool.DocsPaWR;
using SAAdminTool.utils;

namespace SAAdminTool.popup
{
    /// <summary>
    /// Pagina per la visualizzazione della storia delle storicizzazione subite da un ruolo
    /// </summary>
    public partial class RoleHistory : System.Web.UI.Page
    {
        /// <summary>
        /// Script per l'apertura della pagina con la storia di un ruolo
        /// </summary>
        /// <param name="idCorrGlobRole"></param>
        /// <param name="highlightRole">Id del ruolo da evidenziare</param>
        public static String GetScriptToOpenWindow(String idCorrGlobRole, String highlightRole)
        {

            return String.Format(
                "window.showModalDialog('{0}/popup/RoleHistory.aspx?idCorr={1}&highlight={2}', '', 'dialogWidth:588px;dialogHeight:350px; resizable: no;status:no;scroll:yes;help:no;close:no;center:yes;');",
                Utils.getHttpFullPath(), idCorrGlobRole, highlightRole);

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Response.Expires = -1;

                // Recupero della storia del ruolo
                RoleHistoryResponse response = UserManager.GetRoleHistory(new RoleHistoryRequest()
                    {
                        IdCorrGlobRole = Request["idCorr"]
                    });

                // Impostazione delle informazioni necessarie per l'eventuale generazione del report
                ReportingUtils.PrintRequest = new PrintReportObjectTransformationRequest()
                    {
                        ContextName = "GestioneRuolo",
                        ReportKey = "StoriaRuolo",
                        DataObject = response.RoleHistoryItems
                    };

                // Impostazione del link per l'apertura della pagina del report
                this.btnEsporta.OnClientClick = ReportingUtils.GetOpenReportPageScript(false);



                this.dgHistory.DataSource = response.RoleHistoryItems;
                this.dgHistory.DataBind();

            }
            catch (Exception ex)
            {
                this.ClientScript.RegisterStartupScript(this.GetType(), "Alert", "alert('Si è verificato un errore durante la ricostruzione della storia del ruolo.');self.close();", true);

            }
        }

        protected String FormatDate(DateTime dateTime)
        {
            return dateTime.ToString("dddd, dd MMMM yyyy");
        }

        protected String GetRoleDescription(RoleHistoryItem roleHistoryItem)
        {
            if (!String.IsNullOrEmpty(Request["highlight"]) && 
                (roleHistoryItem.HistoryAction == AdmittedHistoryAction.S || roleHistoryItem.HistoryAction == AdmittedHistoryAction.C) &&
                roleHistoryItem.RoleCorrGlobId == Request["highlight"])
                return String.Format("<span style=\"color:Red\"><strong>{0}</strong></span>", roleHistoryItem.RoleDescription);
            else
                return roleHistoryItem.RoleDescription;

        }

        protected void btnClose_Click(object sender, EventArgs e)
        {
            ReportingUtils.PrintRequest = null;
        }
    }
}
