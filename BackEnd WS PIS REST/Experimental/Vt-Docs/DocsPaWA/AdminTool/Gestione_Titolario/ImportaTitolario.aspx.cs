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

namespace DocsPAWA.AdminTool.Gestione_Titolario
{
    public partial class ImportaTitolario : System.Web.UI.Page
    {
        protected DocsPAWA.DocsPaWR.DocsPaWebService wws = new DocsPAWA.DocsPaWR.DocsPaWebService();
        
        protected void Page_Load(object sender, EventArgs e)
        {
            this.wws.Timeout = System.Threading.Timeout.Infinite;

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

            if (Session["titolarioSelezionato"] != null)
            {
                DocsPAWA.DocsPaWR.OrgTitolario titolario = (DocsPAWA.DocsPaWR.OrgTitolario) Session["titolarioSelezionato"];
            
                //Inizio importazione
                HttpPostedFile p = uploadFile.PostedFile;
                Stream fs = p.InputStream;
                byte[] dati = new byte[fs.Length];
                fs.Read(dati, 0, (int)fs.Length);
                fs.Close();

                DocsPAWA.AdminTool.Manager.SessionManager sessionManager = new DocsPAWA.AdminTool.Manager.SessionManager();

                if (wws.importTitolario(dati, titolario, sessionManager.getUserAmmSession()))
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "importazioneAvvenuta", "alert('Importazione avvenuta con successo.');", true);
                }
                else
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "importazioneNonAvvenuta", "alert('Problemi nell\\'importazione controllare il file di log.');", true);
                }
            }

            //Abilito il pulsante di log
            btn_log.Visible = true;
        }

        protected void btn_annulla_Click(object sender, EventArgs e)
        {
            //Session.Remove("titolarioSelezionato");
            ClientScript.RegisterStartupScript(this.GetType(), "chiudeFinestra", "window.close();", true);            
        }

        protected void btn_log_Click(object sender, EventArgs e)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "apriLog", "apriLog();", true);                
        }
    }
}
