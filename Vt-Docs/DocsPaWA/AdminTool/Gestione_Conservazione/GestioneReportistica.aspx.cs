using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DocsPAWA.SiteNavigation;
using DocsPAWA.DocsPaWR;

namespace DocsPAWA.AdminTool.Gestione_Conservazione
{
    public partial class GestioneReportistica : System.Web.UI.Page
    {

        private DocsPaWR.DocsPaWebService _wsInstance = new DocsPAWA.DocsPaWR.DocsPaWebService();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.FetchData();
            }
        }

        protected void btnSalva_Click(object sender, EventArgs e)
        {

            if (this.Validate())
            {
                DocsPaWR.Mailbox mailBox = new Mailbox();
                mailBox.MailStruttura = this.txt_mail_struttura.Text;
                mailBox.Server = this.txt_smtp.Text;
                mailBox.Username = this.txt_username.Text;
                mailBox.Password = this.txt_password.Text;
                mailBox.Port = this.txt_port.Text;
                mailBox.UseSSL = this.chk_ssl.Checked;
                mailBox.From = this.txt_from.Text;
                mailBox.MailPolicy = this.txt_mail_policy.Text;
                bool result = this._wsInstance.SetMailStruttura(this.IdAmministrazione.ToString(), mailBox);
                string msg = string.Empty;

                if (result)
                {
                    msg = "Configurazione salvata con successo";
                }
                else
                {
                    msg = "Errore nel salvataggio della configurazione";
                }

                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "save_result", "alert('" + msg + "');", true);
            }
            this.pnlRecipients.Update();
            this.pnlMailbox.Update();
        }

        protected void FetchData()
        {
            DocsPaWR.Mailbox mailParams = this._wsInstance.GetMailStruttura(this.IdAmministrazione.ToString());

            if (mailParams != null)
            {
                this.txt_mail_struttura.Text = mailParams.MailStruttura;
                this.txt_smtp.Text = mailParams.Server;
                this.txt_username.Text = mailParams.Username;
                this.txt_password.Text = mailParams.Password;
                this.txt_port.Text = mailParams.Port;
                this.txt_from.Text = mailParams.From;
                this.txt_mail_policy.Text = mailParams.MailPolicy;
                this.chk_ssl.Checked = mailParams.UseSSL;
            }
            else
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "save_result", "alert('Si è verificato un errore nel recupero dei parametri di configurazione');", true);
                this.btn_save.Enabled = false;
            }
        }

        protected int IdAmministrazione
        {
            get
            {
                string idAmministrazione = AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "3");

                return Convert.ToInt32(idAmministrazione);
            }
        }

        protected bool Validate()
        {
            string message = string.Empty;
            // Validazione parametri casella
            if (string.IsNullOrEmpty(this.txt_smtp.Text))
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "save_result", "alert('Il campo \"Server SMTP\" è obbligatorio');", true);
                return false;
            }

            int i;
            

            if (!Int32.TryParse(this.txt_port.Text, out i))
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "save_result", "alert('Il valore inserito nel campo \"Porta\" non è valido');", true);
                return false;
            }

            if (this.txt_password.Text != this.txt_conf_pass.Text)
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "save_result", "alert('Il valori inseriti nei campi \"Password\" e \"Conferma\" non coincidono');", true);
            }

            return true;
        }
    }
}