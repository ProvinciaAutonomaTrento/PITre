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

namespace DocsPAWA.AdminTool.Gestione_Utenti
{
    public partial class ImportaUtenti : System.Web.UI.Page
    {
        protected int utInseriti = 0;
        protected int utAggiornati = 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Expires = -1;

            if (Session["ImportazioneAvvenuta"] == null)
            {
                lbl_avviso.Text = "";
                btn_log.Visible = false;
            }
        }

        protected void btn_annulla_Click(object sender, EventArgs e)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "chiudeFinestra", "window.close();",true);            
        }

        protected void btn_importa_Click(object sender, EventArgs e)
        {
            //Controllo la selezione file
            if (uploadFile.Value == "" || uploadFile.Value == null)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "selezioneNonValida_1", "alert('Selezionare un file valido.');", true);
                return;
            }

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
            bool esitoImport = true;

            DocsPAWA.DocsPaWR.DocsPaWebService ws = new DocsPAWA.DocsPaWR.DocsPaWebService();
            string[] amministrazione = ((string)Session["AMMDATASET"]).Split('@');
            string codiceAmm = amministrazione[0];
            bool update = false;
            
            if (CheckBox1.Checked)
                update = true;
            ws.Timeout = System.Threading.Timeout.Infinite;

            DocsPAWA.AdminTool.Manager.SessionManager sessionManager = new DocsPAWA.AdminTool.Manager.SessionManager();

            esitoImport = ws.importaUtenti(sessionManager.getUserAmmSession(), dati, "importUtenti.xls", codiceAmm, update, ref utInseriti, ref utAggiornati);

            if (!esitoImport)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "errore", "alert('ERRORE : controllare che il modello del file sia corretto.');", true);
                lbl_avviso.Text = "ERRORE nell'importazione !";
                btn_log.Visible = true;
                return;
            }

            //ClientScript.RegisterStartupScript(this.GetType(), "importazioneAvvenuta", "alert('Importazione completata correttamente.');", true);
            //Inserisco questa variabile in sessione per poter discriminare dalla pagina chiamante
            //se aggiornare o meno il datagrid degli utenti
            Session.Add("ImportazioneAvvenuta", "ImportazioneAvvenuta");

            lbl_avviso.Text = "Utenti Inseriti : " + utInseriti + " Utenti Aggiornati : " + utAggiornati;
            btn_log.Visible = true;
        }

        protected void btn_log_Click(object sender, EventArgs e)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "apriLog", "apriLog();", true);                
        }
    }
}
