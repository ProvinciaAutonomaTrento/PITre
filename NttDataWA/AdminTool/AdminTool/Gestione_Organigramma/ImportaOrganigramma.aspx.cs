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

namespace SAAdminTool.AdminTool.Gestione_Organigramma
{
    public partial class ImportaOrganigramma : System.Web.UI.Page
    {
        protected SAAdminTool.DocsPaWR.DocsPaWebService wws = new SAAdminTool.DocsPaWR.DocsPaWebService();
        
        protected void Page_Load(object sender, EventArgs e)
        {
            Session.Add("importOrganigramma", "importOrganigramma");
        }

        protected void btn_importa_Click(object sender, EventArgs e)
        {
            // Impostazione del timeout del web service
            this.wws.Timeout = System.Threading.Timeout.Infinite;

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

            SAAdminTool.AdminTool.Manager.SessionManager sessionManager = new SAAdminTool.AdminTool.Manager.SessionManager();

            if (wws.importOrganigramma(sessionManager.getUserAmmSession(),dati))
            {
                ClientScript.RegisterStartupScript(this.GetType(), "importazioneAvvenuta", "alert('Importazione avvenuta con successo.');", true);
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "importazioneNonAvvenuta", "alert('Problemi nell\\'importazione controllare il file di log.');", true);
            }            

            //Abilito il pulsante di log
            btn_log.Visible = true;
        }

        protected void btn_annulla_Click(object sender, EventArgs e)
        {
            //Session.Remove("importOrganigramma");
            ClientScript.RegisterStartupScript(this.GetType(), "chiudeFinestra", "window.close();", true);
        }

        protected void btn_log_Click(object sender, EventArgs e)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "apriLog", "apriLog();", true);
        }
    }
}
