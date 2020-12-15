using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DocsPAWA.AdminTool.Gestione_Funzioni
{
    public partial class ExportDettagli : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
                string tipoReport = Request.QueryString["tipo"].ToString();
                this.lbl_cod.Text = Request.QueryString["id"].ToString();
                this.lbl_type.Text = tipoReport;
                switch (tipoReport)
                {
                    case "MACRO_FUNZ":
                        this.lbl_type_descr.Text = "Funzione e ruoli/utenti associati al tipo funzione.";
                        this.lbl_funzione.Text = "Tipo funzione selezionato";
                        this.lbl_description.Text = this.lbl_cod.Text + " - " + ws.GetTipoFunzioneByCod(this.lbl_cod.Text.Trim(), false).Descrizione;
                        break;

                    case "MICRO_FUNZ":
                        this.lbl_type_descr.Text = "Tipi funzione e ruoli/utenti associati alla funzione.";
                        this.lbl_funzione.Text = "Funzione selezionata";
                        this.lbl_description.Text = this.lbl_cod.Text + " - " + ws.GetFunzioneAnagraficaReport(this.lbl_cod.Text.Trim()).Descrizione;
                        break;
                }
            }
        }

        protected void btn_conferma_Click(object sender, EventArgs e)
        {
            try
            {
                // selezione tipo report
                string reportType = this.lbl_type.Text.Trim();

                // selezione formato
                string formato = this.ddl_format.SelectedValue;

                // reperimento dati amministrazione
                string idAmm = AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "3");

                DocsPAWA.DocsPaWR.FileDocumento fileDoc = new DocsPAWA.DocsPaWR.FileDocumento();

                AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
                fileDoc = ws.GetReportFunzioni(reportType, formato, this.lbl_cod.Text.Trim(), idAmm);

                if (fileDoc != null)
                {

                    Response.ContentType = "application/vnd.ms-excel";
                    Response.AddHeader("Content-Disposition", "attachment; filename=" + fileDoc.name);
                    Response.BinaryWrite(fileDoc.content);
                    Response.Flush();
                    Response.End();
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "exportFailed", "alert('Si è verificato un errore nel processo di esportazione.');", true);
            }
 
        }
    }
}