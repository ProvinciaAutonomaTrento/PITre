using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace DocsPAWA.popup
{
	/// <summary>
	/// Summary description for visualizzaLog.
	/// </summary>
    public class sceltaRFSegnatura : DocsPAWA.CssPage
	{
        protected System.Web.UI.WebControls.Label lbl_doc_rf;
        protected System.Web.UI.WebControls.Label lb_title;
        protected System.Web.UI.WebControls.Label lblMailAddress;

        protected System.Web.UI.WebControls.Button Btn_ok;
        protected System.Web.UI.WebControls.Button Btn_annulla;
        protected System.Web.UI.WebControls.DropDownList ddl_regRF;
        protected System.Web.UI.WebControls.DropDownList ddlCaselle;
		private void Page_Load(object sender, System.EventArgs e)
		{
            if (!IsPostBack)
            {
                ddl_regRF.SelectedIndexChanged += new EventHandler(ddl_regRF_IndexChanged);
                // l'utente può selezionare registro o RF da utilizzare
                this.lbl_doc_rf.Visible = true;

                string codice = Request.QueryString["codice"].ToString();
                if (!string.IsNullOrEmpty(codice))
                {
                    switch (codice.ToUpper())
                    {
                        case "SEGNATURA":
                            this.lbl_doc_rf.Text = "Selezionare il registro/RF da visualizzare nella segnatura";
                            this.lb_title.Text = "Scelta RF per la Segnatura";
                            (this.FindControl("divCaselle") as HtmlGenericControl).Visible = false;
                            break;

                        case "RICEV":
                        case "RICEVUTA":
                            this.lbl_doc_rf.Text = "Selezionare il registro/RF da utilizzare per l'invio della ricevuta di ritorno";
                            this.lb_title.Text = "Scelta RF per l'invio della ricevuta";
                            (this.FindControl("divCaselle") as HtmlGenericControl).Visible = true;
                            break;
                    }
                }
                CaricaComboRegistri(this.ddl_regRF, codice);
            }
        }

        private void CaricaComboRegistri(DropDownList ddl, string codice)
        {
            DocsPaWR.SchedaDocumento schedaDocumento = DocumentManager.getDocumentoSelezionato();
            //verifico se il registro ha RF associati
            DocsPaWR.Registro[] listaRF = UserManager.getListaRegistriWithRF(this, "1", schedaDocumento.registro.systemId);
            if (listaRF != null && listaRF.Length > 0)
            {
                if (listaRF.Length == 1)
                {
                    ListItem li = new ListItem();
                    li.Value = listaRF[0].systemId;
                    li.Text = listaRF[0].codRegistro + " - " + listaRF[0].descrizione;
                    this.ddl_regRF.Items.Add(li);
                    ListItem lit2 = new ListItem();
                    lit2.Value = schedaDocumento.registro.systemId;
                    lit2.Text = schedaDocumento.registro.codRegistro + " - " + schedaDocumento.registro.descrizione;
                    this.ddl_regRF.Items.Add(lit2);
                }
                else
                {
                    ListItem lit = new ListItem();
                    lit.Value = "";
                    lit.Text = " ";
                    this.ddl_regRF.Items.Add(lit);
                    ListItem lit2 = new ListItem();
                    lit2.Value = schedaDocumento.registro.systemId;
                    lit2.Text = schedaDocumento.registro.codRegistro + " - " + schedaDocumento.registro.descrizione;
                    this.ddl_regRF.Items.Add(lit2);

                    foreach (DocsPaWR.Registro regis in listaRF)
                    {
                        if (codice.ToUpper().Equals("RICEVUTA"))
                        {
                            if (!regis.invioRicevutaManuale.ToUpper().Equals("1"))
                            {
                                ListItem li = new ListItem();
                                li.Value = regis.systemId;
                                li.Text = regis.codRegistro + " - " + regis.descrizione;
                                this.ddl_regRF.Items.Add(li);
                            }
                        }
                        else
                        {
                            ListItem li = new ListItem();
                            li.Value = regis.systemId;
                            li.Text = regis.codRegistro + " - " + regis.descrizione;
                            this.ddl_regRF.Items.Add(li);
                        }
                    }
                }
                if (codice != null && codice.Equals("RICEVUTA") && (!string.IsNullOrEmpty(ddl_regRF.SelectedValue)))
                {
                    CaricaComboCaselle(ddl_regRF.SelectedValue);
                }
            }
            else
            {
                this.ddl_regRF.Visible = false;
                this.ddlCaselle.Visible = false;
            }
        }

        private void CaricaComboCaselle(string idRegistro)
        {
            DocsPaWR.CasellaRegistro [] caselle = DocsPAWA.utils.MultiCasellaManager.GetMailRegistro(idRegistro);
            if (caselle != null && caselle.Length > 0)
            {
                foreach (DocsPaWR.CasellaRegistro c in caselle)
                { 
                    System.Text.StringBuilder formatString = new System.Text.StringBuilder();
                    if (c.Principale.Equals("1"))
                        formatString.Append("* ");
                    formatString.Append(c.EmailRegistro);
                    if (!string.IsNullOrEmpty(c.Note))
                        formatString.Append(" - " + c.Note);
                    ddlCaselle.Items.Add(new ListItem { Text = formatString.ToString(), Value = c.EmailRegistro });
                    if (c.Principale.Equals("1"))
                        ddlCaselle.SelectedValue = c.EmailRegistro;
                }
            }
        }

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.Load += new System.EventHandler(this.Page_Load);
            this.Btn_ok.Click += new System.EventHandler(this.Btn_ok_Click);
            this.Btn_annulla.Click += new EventHandler(Btn_annulla_Click);
		}

		#endregion

        private void Btn_ok_Click(object sender, System.EventArgs e)
        {
            string codice = Request.QueryString["codice"].ToString();
            if (this.ddl_regRF.Visible)
            {
                if (this.ddl_regRF.SelectedValue.Equals(""))
                {
                    Response.Write("<script>alert('Selezionare un registro o un RF come mittente');</script>");
                }
                else
                {
                    //salvo in dpa_ass_doc_mail_interop le informazioni sull'rf e la casella da utilizzare per l'invio della ricevuta ed eventualmente della notifica di annullamento
                    DataSet ds = DocsPAWA.utils.MultiCasellaManager.GetAssDocAddress(DocumentManager.getDocumentoSelezionato().docNumber);
                    if (ds != null && ds.Tables["ass_doc_rf"].Rows.Count > 0)
                    {
                        foreach (DataRow row in ds.Tables["ass_doc_rf"].Rows)
                        {
                            if (!codice.Equals("RICEVUTA") && (!ddl_regRF.SelectedValue.Equals(row["registro"])))
                            {
                                string mailAddress = DocsPAWA.utils.MultiCasellaManager.GetMailPrincipaleRegistro(ddl_regRF.SelectedValue);
                                DocsPAWA.utils.MultiCasellaManager.UpdateAssDocAddress(DocumentManager.getDocumentoSelezionato().docNumber,
                                    ddl_regRF.SelectedValue, mailAddress);
                            }
                            else
                            {
                                DocsPAWA.utils.MultiCasellaManager.UpdateAssDocAddress(DocumentManager.getDocumentoSelezionato().docNumber,
                                    ddl_regRF.SelectedValue, ddlCaselle.SelectedValue);
                            }

                        }
                    }
                    Session.Add("ifChooseRf", true);
                    Response.Write("<script>window.returnValue='OK^" + this.ddl_regRF.SelectedItem.Value + "^" + this.ddl_regRF.SelectedItem.Text.Replace("'", "\\'") + "'; window.close();</script>");
                }
            }
            else
            {
                Session.Add("ifChooseRf", true);
                Response.Write("<script>window.returnValue='OK^OK'; window.close();</script>");
            }
        }

        private void Btn_annulla_Click(object sender, EventArgs e)
        {
            Response.Write("<script>window.returnValue='KO^KO'; window.close();</script>");
        }

        protected void ddl_regRF_IndexChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(ddl_regRF.SelectedValue))
            {
                ddlCaselle.Items.Clear();
            }
            else
            {
                CaricaComboCaselle(ddl_regRF.SelectedValue);
            }
        }
    }
}
