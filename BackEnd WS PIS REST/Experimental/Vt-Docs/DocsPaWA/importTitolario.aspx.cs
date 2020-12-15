using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace DocsPAWA
{
    public partial class importTitolario : System.Web.UI.Page
    {
        protected DocsPaWR.DocsPaWebService wws = new DocsPAWA.DocsPaWR.DocsPaWebService();

        protected void Page_Load(object sender, EventArgs e)
        {
            lbl_errore.Text = "";
            btn_log.Visible = false;
            pnl_log.Visible = false;
        }

        protected void btn_import_Click(object sender, EventArgs e)
        {
            //Verifica dati obbligatori e loro correttezza
            if (txt_Codice.Text == "" || txt_Descrizione.Text == "")
            {
                lbl_errore.Text = "Inserire i dati obbligatori contrassegnati da un asterisco.";
                return;
            }
            wws.Timeout = System.Threading.Timeout.Infinite;

            DocsPAWA.AdminTool.Manager.SessionManager sessionManager = new DocsPAWA.AdminTool.Manager.SessionManager();

            bool result = wws.modificaTemplateExcelTitolario(txt_Codice.Text, txt_Descrizione.Text, "importTitolario.xls", sessionManager.getUserAmmSession());

            if (result)
                lbl_errore.Text = "Importazione avvenuta con successo.";
            else
                lbl_errore.Text = "Problemi durante l'importazione, consultare il file di log.";

            btn_log.Visible = true;

        }

        protected void bnt_log_Click(object sender, EventArgs e)
        {
            txt_log.Text = "";
            ArrayList fileLog = new ArrayList(wws.getLogImportTitolario());
            foreach (string sOutput in fileLog)
                txt_log.Text += sOutput + "\n";
            txt_log.ReadOnly = true;

            pnl_log.Visible = true;
            btn_log.Visible = true;
        }
    }
}
