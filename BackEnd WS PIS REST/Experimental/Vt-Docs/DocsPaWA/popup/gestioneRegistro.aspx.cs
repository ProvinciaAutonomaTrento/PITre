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
using System.Configuration;

namespace DocsPAWA.popup
{
	/// <summary>
	/// Summary description for gestioneregistro.
	/// </summary>
    public class gestioneregistro : DocsPAWA.CssPage
	{
		protected System.Web.UI.WebControls.Label LabelRegistro;
		protected System.Web.UI.WebControls.TextBox txtRegistro;
		protected System.Web.UI.WebControls.Label LabelDescrizione;
		protected System.Web.UI.WebControls.TextBox txtDescrizione;
		protected System.Web.UI.WebControls.Label LabelEmail;
		protected System.Web.UI.WebControls.TextBox txtEmail;
		protected System.Web.UI.WebControls.Label LabelDataapertura;
		protected System.Web.UI.WebControls.TextBox txtDataApertura;
		protected System.Web.UI.WebControls.Label LabelDataChiusura;
		protected System.Web.UI.WebControls.TextBox txtDataChiusura;
		protected System.Web.UI.WebControls.Label LabelProssimoProtocollo;
		protected System.Web.UI.WebControls.TextBox txtProssimoProtocollo;
		protected System.Web.UI.WebControls.Label LabelNota;
		protected DocsPAWA.DocsPaWR.Registro registro;
		protected System.Web.UI.WebControls.Button btn_ok;
		protected System.Web.UI.WebControls.Button btn_chiudi;
		protected System.Web.UI.WebControls.TextBox txtDataUltimoProt;
		protected System.Web.UI.WebControls.Label Label1;
		protected System.Web.UI.WebControls.Label Label2;
		protected string tipoGest;
	
		private void Page_Load(object sender, System.EventArgs e)
		{
			// Put user code to initialize the page here

			this.btn_chiudi.Attributes.Add("onclick" , "window.close()");
			tipoGest = Request.QueryString["tipo"];
			if (tipoGest != null)
			{
				if (tipoGest.Equals("M"))
				{
					registro = GestManager.getRegistroSel(this);
					if(registro != null)
					{
						setDettagli(registro);
					}
				}
				else
					if(tipoGest.Equals("I"))
				{
					registro = new DocsPAWA.DocsPaWR.Registro();
				}
			}
		}


		private void setDettagli(DocsPAWA.DocsPaWR.Registro registro)
		{
			if(!Page.IsPostBack)
			{
				this.txtRegistro.Text = registro.codRegistro;
				this.txtDataApertura.Text = registro.dataApertura;
				this.txtDataChiusura.Text = registro.dataChiusura;
				this.txtDescrizione.Text = registro.descrizione;
				this.txtEmail.Text = registro.email;
				this.txtProssimoProtocollo.Text = registro.ultimoNumeroProtocollo;
				this.txtDataUltimoProt.Text = registro.dataUltimoProtocollo;
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
			this.btn_ok.Click += new System.EventHandler(this.btn_ok_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion



		private void btn_ok_Click(object sender, System.EventArgs e)
		{
			if (registro != null)
			{
				if (tipoGest.Equals("M")) 
				{
					registro.descrizione = this.txtDescrizione.Text;
					registro.email = this.txtEmail.Text;
					if(!this.txtProssimoProtocollo.Text.Equals(""))
						registro.ultimoNumeroProtocollo = this.txtProssimoProtocollo.Text;
					
					GestManager.modificaRegistro(this, registro);
				} 
				else
				{
					registro.descrizione = this.txtDescrizione.Text;
					registro.dataApertura = this.txtDataApertura.Text;
					registro.dataChiusura = this.txtDataChiusura.Text;
					registro.codAmministrazione = UserManager.getInfoUtente(this).idAmministrazione;//ConfigurationManager.AppSettings["ID_AMMINISTRAZIONE"];
					//richiama il metodo che crea il nuovo registro
				
				}

				//richiama la funzione javascript che aggiorna il form chiamante
				string  funct = " window.open('../gestione/registro/regElenco.aspx','iFrame_elenco'); window.open('../gestione/registro/regDettagli.aspx','iFrame_dettagli'); ";
				funct = funct + " window.close(); ";
				Response.Write("<script> " + funct + "</script>");
			}
		}
	}
}
