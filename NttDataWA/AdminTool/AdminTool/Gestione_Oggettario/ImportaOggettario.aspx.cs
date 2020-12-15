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

namespace SAAdminTool.AdminTool.Gestione_Oggettario
{
    public partial class ImportaOggettario : System.Web.UI.Page
    {
        protected int oggInseriti = 0;
        protected int oggAggiornati = 0;
        protected int oggErrati = 0;
        //protected System.Web.UI.WebControls.CheckBox ckb_update;

        protected void Page_Load(object sender, EventArgs e)
        {
            //Response.Expires = -1;
            Session["AdminBookmark"] = "ImportOggettario";

            string idAmm = AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "3");
            if (!IsPostBack)
            {
                this.lbl_position.Text = "&nbsp;&bull;&nbsp;Amministrazione: " + AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "1");
            }
            if (Session["ImportazioneAvvenuta"] == null)
            {
                lbl_avviso.Text = "";
                btn_log.Visible = false;
            }
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

            SAAdminTool.DocsPaWR.DocsPaWebService ws = new SAAdminTool.DocsPaWR.DocsPaWebService();
            string[] amministrazione = ((string)Session["AMMDATASET"]).Split('@');
            string codiceAmm = amministrazione[0];

            ws.Timeout = System.Threading.Timeout.Infinite;

            SAAdminTool.AdminTool.Manager.SessionManager sessionManager = new SAAdminTool.AdminTool.Manager.SessionManager();

            esitoImport = ws.importaOggettario(dati, "importOggettario.xls", codiceAmm, this.ckb_update.Checked, ref oggInseriti, ref oggAggiornati, ref oggErrati);

            if (!esitoImport)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "errore", "alert('ERRORE : controllare che il modello del file sia corretto.');", true);
                lbl_avviso.Text = "ERRORE nell'importazione !";
                btn_log.Visible = true;
                return;
            }

            //ClientScript.RegisterStartupScript(this.GetType(), "importazioneAvvenuta", "alert('Importazione completata correttamente.');", true);

            Session.Add("ImportazioneAvvenuta", "ImportazioneAvvenuta");

            lbl_avviso.Text = "Oggetti Inseriti : " + oggInseriti + " Oggetti Aggiornati : " + oggAggiornati + " Oggetti Errati : " + oggErrati;
            btn_log.Visible = true;
        }

        protected void btn_log_Click(object sender, EventArgs e)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "apriLog", "apriLog();", true);                
        }
    }
}
