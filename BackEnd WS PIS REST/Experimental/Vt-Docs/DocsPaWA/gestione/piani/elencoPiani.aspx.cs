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
using gestione.piani;
using DocsPAWA.DocsPaWR;

namespace DocsPAWA.gestione.piani
{
    public partial class elencoPiani : System.Web.UI.Page
    {
        protected System.Web.UI.WebControls.ImageButton btn_stampa;
        protected System.Web.UI.WebControls.DropDownList ddl_registro;
        protected System.Web.UI.WebControls.Label lbl_initDataScadenza;
        protected DocsPaWebCtrlLibrary.DateMask txt_initDataScadenza;
        protected System.Web.UI.WebControls.Label lbl_fineDataScadenza;
        protected DocsPaWebCtrlLibrary.DateMask txt_fineDataScadenza;
        DocsPAWA.DocsPaWR.Utente utente = new DocsPAWA.DocsPaWR.Utente();
	
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                txt_initDataScadenza.Text = "";
                utente = (DocsPAWA.DocsPaWR.Utente)Session["userData"];
                if (utente != null)
                {
                    this.DO_SetControlFromDocsPA(utente);
                }
            }
        }

        #region Web Form Designer generated code
        override protected void OnInit(EventArgs e)
        {
            InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.ddl_registro.SelectedIndexChanged += new System.EventHandler(this.ddl_registro_SelectedIndexChanged);
            this.btn_stampa.Click += new System.Web.UI.ImageClickEventHandler(this.btn_stampa_Click);
            this.Load += new System.EventHandler(this.Page_Load);
        }
        #endregion

        private void DO_SetControlFromDocsPA(DocsPAWA.DocsPaWR.Utente utente)
        {
            try
            {
                //Dobbiamo usare il registro dell'utente 
                ddl_registro.Items.Clear();
                ddl_registro.Items.Add("");
                for (int i = 0; i < utente.ruoli.Length; i++)
                {
                    DocsPAWA.DocsPaWR.Ruolo ruolo = (DocsPAWA.DocsPaWR.Ruolo)utente.ruoli[i];
                    for (int j = 0; j < ruolo.registri.Length; j++)
                    {
                        DocsPAWA.DocsPaWR.Registro reg = (DocsPAWA.DocsPaWR.Registro)ruolo.registri[j];
                        if (!DO_VerifyList(ddl_registro, reg.systemId))
                        {
                            ddl_registro.Items.Add(new ListItem(reg.descrizione, reg.systemId));
                        }
                    }
                }
                if(ddl_registro.Items.Count >= 2)
					ddl_registro.SelectedIndex = 1;
               
            }
            catch (Exception ex)
            {
                DocsPAWA.ErrorManager.redirectToErrorPage(this, ex);
            }
        }

        private bool DO_VerifyList(DropDownList list, string system_id)
        {
            bool result = false;
            for (int i = 0; i < list.Items.Count; i++)
            {
                if (system_id == list.Items[i].Value)
                {
                    result = true;
                    return result;
                }
            }
            return result;
        }

        private void btn_stampa_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            DocsPAWA.DocsPaWR.FileDocumento fileRep = new DocsPAWA.DocsPaWR.FileDocumento();
            try
            {
                string dataInizio = "";
                string dataFine = "";

                #region controllo che i campi data non siano vuoti
                if (this.txt_initDataScadenza.Text.Equals("") || this.txt_fineDataScadenza.Text.Equals(""))
                {
                    Response.Write("<script>alert('Attenzione inserire il periodo di riferimento.');</script>");
                    string s = "<SCRIPT language='javascript'>document.getElementById('" + txt_initDataScadenza.ID + "').focus();</SCRIPT>";
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "focus", s);
                    do_openinRightFrame(this, "whitepage.htm");
                    return;
                }

                if (!this.txt_initDataScadenza.Text.Equals(""))
                {
                    //controllo validità di data iniziale
                    if (!DocsPAWA.Utils.isDate(this.txt_initDataScadenza.Text))
                    {
                        Response.Write("<script>alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');</script>");
                        string s = "<SCRIPT language='javascript'>document.getElementById('" + txt_initDataScadenza.ID + "').focus();</SCRIPT>";
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "focus", s);
                        do_openinRightFrame(this, "whitepage.htm");
                        return;
                    }
                }
                if (!this.txt_fineDataScadenza.Text.Equals(""))
                {
                    //controllo validità di data finale
                    if (!DocsPAWA.Utils.isDate(this.txt_fineDataScadenza.Text))
                    {
                        Response.Write("<script>alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');</script>");
                        string s = "<SCRIPT language='javascript'>document.getElementById('" + txt_fineDataScadenza.ID + "').focus();</SCRIPT>";
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "focus", s);
                        do_openinRightFrame(this, "whitepage.htm");
                        return;
                    }
                }
                if (DocsPAWA.Utils.verificaIntervalloDate(txt_initDataScadenza.Text, txt_fineDataScadenza.Text))
                {
                    //controllo periodo data
                    Response.Write("<script>alert('Verificare intervallo date !');</script>");
                    string s = "<SCRIPT language='javascript'>document.getElementById('" + txt_initDataScadenza.ID + "').focus();</SCRIPT>";
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "focus", s);
                    do_openinRightFrame(this, "whitepage.htm");
                    return;
                }
                #endregion

                dataInizio = this.txt_initDataScadenza.Text;
                dataFine = this.txt_fineDataScadenza.Text;
                int id_reg = Convert.ToInt32(ddl_registro.SelectedValue);
                fileRep = XlsReport.CreaReportPianiRientro(this, id_reg, ddl_registro.SelectedItem.Text, dataInizio, dataFine);
                if (fileRep != null)
                {
                    DocsPAWA.exportDati.exportDatiSessionManager session = new DocsPAWA.exportDati.exportDatiSessionManager();
                    session.SetSessionExportFile(fileRep);
                }
            }
            catch (Exception ex)
            {
                DocsPAWA.ErrorManager.redirectToErrorPage(this, ex);
            }
            if (fileRep != null)
            {
                if (fileRep.content != null && fileRep.content.Length > 0)
                {
                    this.executeJS("<SCRIPT>OpenFile();</SCRIPT>");
                    do_openinRightFrame(this, "whitepage.htm");
                }
                else
                    this.executeJS("<SCRIPT>alert('Impossibile generare il file xls');</SCRIPT>");
            }
            else
            {
                do_alert(this, "Non ci sono dati per il Rapporto selezionato");
                do_openinRightFrame(this, "whitepage.htm");
            }
        }

        /// <summary>
        /// Esegue JS
        /// </summary>
        /// <param name="key"></param>
        private void executeJS(string key)
        {
            if (!this.Page.IsStartupScriptRegistered("theJS"))
                this.Page.RegisterStartupScript("theJS", key);
        }

        public static void do_openinRightFrame(Page page, string url)
        {
            string funct_dx2 = "top.principale.frames[1].location='" + url + "'";
            page.Response.Write("<script> " + funct_dx2 + "</script>");
        }

        public static void do_alert(Page page, string message)
        {
            message = message.Replace("'", "\\'");
            message = message.Trim();
            page.Response.Write("<script>alert('" + message + "');</script>");
        } 


        private void ddl_registro_SelectedIndexChanged(object sender, System.EventArgs e)
        {
        }
    }
}
