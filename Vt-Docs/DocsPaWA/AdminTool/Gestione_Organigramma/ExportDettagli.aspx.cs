using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DocsPAWA.SiteNavigation;

namespace DocsPAWA.AdminTool.Gestione_Organigramma
{
    public partial class ExportDettagli : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                InitializePage();
            }
        }

        private void InitializePage()
        {
            string exportType = this.Request.QueryString["ExportType"];
            if (string.IsNullOrEmpty(exportType))
            {
                if (!string.IsNullOrEmpty(this.Request.QueryString["idRuoloTitolare"]) &&
                    !string.IsNullOrEmpty(this.Request.QueryString["idUtenteTitolare"]))
                {
                    this.lbl_type_descr.Text = "Processi e istanze di processi in cui l'utente è coinvolto nel ruolo selezionato";
                }
                else if (!string.IsNullOrEmpty(this.Request.QueryString["idUtenteTitolare"]))
                {
                    this.lbl_type_descr.Text = "Processi e istanze di processi in cui l'utente è coinvolto";
                }
                else if (!string.IsNullOrEmpty(this.Request.QueryString["idRuoloTitolare"]))
                {
                    this.lbl_type_descr.Text = "Processi e istanze di processi in cui il ruolo è coinvolto";
                }
            }
            else
            {
                switch(exportType)
                {
                    case "TRASM_UTENTE":
                        this.lbl_type_descr.Text = "Trasmissioni pendenti";
                        break;
                }
            }
        }

        protected void btnConferma_Click(object sender, EventArgs e)
        {
            try
            {
                string exportType = this.Request.QueryString["ExportType"];
                DocsPAWA.DocsPaWR.FileDocumento fileDoc = new DocsPAWA.DocsPaWR.FileDocumento();
                // selezione formato
                string formato = this.ddl_format.SelectedValue;
                if (string.IsNullOrEmpty(exportType))
                {
                    string idRuoloTitolare = !string.IsNullOrEmpty(this.Request.QueryString["idRuoloTitolare"]) ? this.Request.QueryString["idRuoloTitolare"].ToString() : string.Empty;
                    string idUtenteTitolare = !string.IsNullOrEmpty(this.Request.QueryString["idUtenteTitolare"]) ? this.Request.QueryString["idUtenteTitolare"].ToString() : string.Empty;
                    AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
                    fileDoc = ws.GetReportProcessiFirma(idRuoloTitolare, idUtenteTitolare, formato);
                }
                else
                {
                    switch (exportType)
                    {
                        case "TRASM_UTENTE":
                            string idPeople = this.Request.QueryString["idPeople"];
                            string idCorrGlobali = this.Request.QueryString["idCorrGlobali"];
                            AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
                            fileDoc = ws.GetReportTrasmissioniPendentiUtente(idPeople, idCorrGlobali, formato);
                            break;
                    }
                }
                if (fileDoc != null)
                {
                    // Inizializzazione del call context
                    if (CallContextStack.CurrentContext == null)
                        CallContextStack.CurrentContext = new CallContext("ReportingContext");
                    CallContextStack.CurrentContext.ContextState["documentFile"] = fileDoc;

                    this.reportContent.Attributes["src"] = "../../popup/Reporting/ReportContent.aspx";
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "exportFailed", "alert('Si è verificato un errore nel processo di esportazione.');", true);
            }

        }
    }
}