using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.IO;

namespace DocsPAWA.popup
{
    public partial class ImportCorrispondenti : DocsPAWA.CssPage
    {
        protected DocsPAWA.UserControls.AppTitleProvider appTitleProvider;
        protected System.Web.UI.WebControls.Label lbl_avviso;
        protected System.Web.UI.HtmlControls.HtmlInputFile uploadFile;
        protected System.Web.UI.WebControls.Button btn_importa;
        protected System.Web.UI.WebControls.Button btn_annulla;
        protected System.Web.UI.WebControls.Button btn_log;

        protected string listeDistr = System.Configuration.ConfigurationManager.AppSettings["LISTE_DISTRIBUZIONE"];
        // di default si suppone che le liste sono disabilitate
        protected int flagListe = 0;
        protected int corrInseriti = 0;
        protected int corrAggiornati = 0;
        protected int corrRimossi = 0;
        protected int corrNonInseriti = 0;
        protected int corrNonAggiornati = 0;
        protected int corrNonRimossi = 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Expires = -1;
            if (!IsPostBack)
            {
                lbl_avviso.Text = "";
                btn_log.Visible = false;
            }
        }

        protected void btn_annulla_Click(object sender, EventArgs e)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "chiudeFinestra", "window.close();", true);
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
            ws.Timeout = System.Threading.Timeout.Infinite;
            switch (listeDistr)
            {
                case "1": // Liste di distribuzione abilitate
                    flagListe = 1;
                    break;
                case "0": // Liste di distribuzione disabilitate							
                    flagListe = 0;
                    break;
            }

            esitoImport = ws.ImportaRubrica(UserManager.getInfoUtente(), dati, flagListe, "importRubrica.xls", ref corrInseriti, ref corrAggiornati, ref corrRimossi, ref corrNonInseriti, ref corrNonAggiornati, ref corrNonRimossi);
            if (!esitoImport)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "errore", "alert('ERRORE : controllare che il modello del file sia corretto.');", true);
                lbl_avviso.Text = "ERRORE nell'importazione !";
                btn_log.Visible = true;
                return;
            }

            lbl_avviso.Text = String.Format("Corrispondenti Inseriti: {0}<br />Corrispondenti Aggiornati: {1}<br />Corrispondenti Rimossi: {2}<br />Corrispondenti NON Inseriti: {3}<br />Corrispondenti NON Aggiornati: {4}<br />Corrispondenti NON Rimossi: {5}",
                corrInseriti, corrAggiornati, corrRimossi, corrNonInseriti, corrNonAggiornati, corrNonRimossi);
            btn_log.Visible = true;
        }

        protected void btn_log_Click(object sender, EventArgs e)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "apriLog", "apriLog();", true);
        }
    }
}
