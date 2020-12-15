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
using System.IO;

namespace DocsPAWA
{
    public partial class importFascicoli : System.Web.UI.Page
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
            //Controllo che sia un file Excel
            if (uploadFile.Value != "")
            {
                if (uploadFile.Value != null)
                {
                    string[] path = uploadFile.Value.Split('.');
                    if (path.Length != 0 && path[path.Length - 1] != "xls")
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), "selezioneNonValida_2", "alert('I files validi sono solo quelli con estensione *.xls');", true);
                        return;
                    }
                }
            }

            //Inizio importazione
            HttpPostedFile p = uploadFile.PostedFile;
            Stream fs = p.InputStream;
            byte[] dati = new byte[fs.Length];
            fs.Read(dati, 0, (int)fs.Length);
            fs.Close();

            wws.Timeout = System.Threading.Timeout.Infinite;
            if (wws.importFascicoliDocumenti(dati))
            {
                lbl_errore.Text = "Importazione avvenuta con successo.";
                btn_log.Visible = true;
            }
            else
            {
                lbl_errore.Text = "Problemi nell\'importazione controllare il file di log.";
                btn_log.Visible = true;
            }
        }

        protected void bnt_log_Click(object sender, EventArgs e)
        {
            txt_log.Text = "";
            ArrayList fileLog = new ArrayList(wws.getLogImportFascicoli());
            foreach (string sOutput in fileLog)
                txt_log.Text += sOutput + "\n";
            txt_log.ReadOnly = true;

            pnl_log.Visible = true;
            btn_log.Visible = true;
        }       
    }
}
