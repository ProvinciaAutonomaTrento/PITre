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
	/// Summary description for salvaModTrasm.
	/// </summary>
    public class salvaModTrasm : DocsPAWA.CssPage
	{
		protected System.Web.UI.WebControls.RadioButtonList rbl_share;
		protected System.Web.UI.WebControls.Button btn_salva;
		protected System.Web.UI.WebControls.Button btn_annulla;
		protected System.Web.UI.WebControls.Label lbl_messaggio;
		protected System.Web.UI.WebControls.TextBox txt_nomeModello;
		protected DocsPAWA.DocsPaWR.ModelloTrasmissione modello;
		protected DocsPAWA.DocsPaWR.DocsPaWebService wws;

		private void Page_Load(object sender, System.EventArgs e)
		{
			// Put user code to initialize the page here
			wws = new DocsPAWA.DocsPaWR.DocsPaWebService();
			lbl_messaggio.Visible = false;
			SetFocus(txt_nomeModello);
			
			if(Session["Modello"] != null)
			{
				modello = (DocsPAWA.DocsPaWR.ModelloTrasmissione) Session["Modello"];				
			}
			else
			{
				RegisterStartupScript("Errore","<script>alert('Errore nella lettura del modello! Riprovare!');</script>");
				RegisterStartupScript("chiudi","<script>window.close();</script>");
			}

			if (!IsPostBack)
			{
				rbl_share.Items[0].Text = rbl_share.Items[0].Text.Replace("@usr@",UserManager.getUtente(this).descrizione);
				rbl_share.Items[1].Text = rbl_share.Items[1].Text.Replace("@grp@",UserManager.getRuolo(this).descrizione);
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
			this.btn_salva.Click += new System.EventHandler(this.btn_salva_Click);
			this.btn_annulla.Click += new System.EventHandler(this.btn_annulla_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		#region SetFocus
		private void SetFocus(System.Web.UI.Control ctrl)
		{
			string s = "<SCRIPT language='javascript'>document.getElementById('" + ctrl.ID + "').focus() </SCRIPT>";
			RegisterStartupScript("focus", s);
		}
		#endregion SetFocus	

		private void btn_annulla_Click(object sender, System.EventArgs e)
		{
			Session.Remove("Modello");
			RegisterStartupScript("chiudi","<script>window.close();</script>");
		}

		private void btn_salva_Click(object sender, System.EventArgs e)
		{
            try
            {
                //Controllo che i campi obbligatori siano stati compilati
                if (txt_nomeModello.Text == "")
                {
                    lbl_messaggio.Text = "Inserire i campi obbligatori !";
                    lbl_messaggio.Visible = true;
                    return;
                }
                
                modello.NOME = txt_nomeModello.Text;
                if (rbl_share.Items[0].Selected)
                    for (int k = 0; k < modello.MITTENTE.Length; k++)
                    {
                        modello.MITTENTE[k].ID_CORR_GLOBALI = 0;
                    }
                else
                    modello.ID_PEOPLE = "";

                modello.CODICE = "MT_" + wws.getModelloSystemId();

                DocsPaWR.InfoUtente infoUtente = UserManager.getInfoUtente(this);                
                wws.salvaModello(modello, infoUtente);
                Session.Remove("Modello");
                RegisterStartupScript("chiudi", "<script>window.close();</script>");
            }
            catch
            {
                RegisterStartupScript("avviso", "<script>alert('Errore nella creazione del modello!'); window.close();</script>");
            }
		}
	}
}
