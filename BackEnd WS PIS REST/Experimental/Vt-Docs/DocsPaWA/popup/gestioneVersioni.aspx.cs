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
	/// Summary description for gestioneVersioni.
	/// </summary>
    /// 
    public class gestioneVersioni : DocsPAWA.CssPage
	{
		#region WebControls e variabili
		protected System.Web.UI.WebControls.Label LabelCodice;
		protected System.Web.UI.WebControls.Label TextCodice;
		protected System.Web.UI.WebControls.Label LabelNote;
		protected System.Web.UI.WebControls.TextBox TextNote;
		protected System.Web.UI.WebControls.Button btn_ok;
		protected System.Web.UI.WebControls.Label Label;
		protected System.Web.UI.WebControls.Label descDoc;
        protected System.Web.UI.HtmlControls.HtmlInputControl clTesto;
		//---------------------------------------------------------------------
		protected DocsPAWA.DocsPaWR.Documento docSel;
		protected string txt_indexSel;
		protected int indexSel;		
		protected DocsPAWA.DocsPaWR.SchedaDocumento schedaDocumento;
        protected  int caratteriDisponibili = 200;
		#endregion 
	
		#region Page Load
		/// <summary>
		/// Page Load
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Page_Load(object sender, System.EventArgs e)
		{
			// Put user code to initialize the page here
			this.txt_indexSel = Request.QueryString["index"];
			schedaDocumento = (DocsPAWA.DocsPaWR.SchedaDocumento) DocumentManager.getDocumentoSelezionato(this);

			Utils.SetFocus(TextNote.ID,this);

			this.descDoc.Text = schedaDocumento.oggetto.descrizione;

			if(this.txt_indexSel != null)
			{
				indexSel = Int32.Parse(txt_indexSel);
				if(indexSel >=0)
				{
					//Reperisce la versione selezionata
					if (schedaDocumento != null)
					{
						docSel = schedaDocumento.documenti[indexSel];

						if (!this.Page.IsPostBack)
						{
							if (docSel!= null)
							{
								this.TextCodice.Text = docSel.version;
								this.TextNote.Text = docSel.descrizione;
								this.Label.Text = "Modifica versione";
							} 
							else
							{
								this.btn_ok.Enabled = false;
							}
						}
					}
				} 

			}
			else
			{
				this.LabelCodice.Visible = false;
				this.TextCodice.Visible = false;
			}

			//variabile utilizzata per impedire il ripetuto click del bottone ok
            if (!IsPostBack)
            {
                DocsPAWA.DocsPaWR.InfoUtente info = new DocsPAWA.DocsPaWR.InfoUtente();
                info = UserManager.getInfoUtente(this.Page);


                string valoreChiave = utils.InitConfigurationKeys.GetValue(info.idAmministrazione, "FE_MAX_LENGTH_NOTE");
                if (!string.IsNullOrEmpty(valoreChiave))
                Session.Remove("gestioneVersione");
                TextNote.MaxLength = caratteriDisponibili;
                clTesto.Value = caratteriDisponibili.ToString();
                TextNote.Attributes.Add("onKeyUp","calcTesto(this,'"+ caratteriDisponibili.ToString() +" ','NOTE',"+ clTesto.ClientID +")");
                TextNote.Attributes.Add("onchange", "calcTesto(this,'" + caratteriDisponibili.ToString() + " ','NOTE'," + clTesto.ClientID + ")");
            }
		}
		#endregion

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

		#region Inserimento o modifica

		/// <summary>
		/// gestione della versione
		/// </summary>
		private void gestioneVersione()
		{
			string  funct = " window.opener.top.principale.iFrame_sx.document.location='../documento/tabGestioneDoc.aspx?tab=versioni'; ";
			
			//controllo il tipo di operazione da effettuare	
			if (this.txt_indexSel != null && indexSel >= 0)
			{
				//salvo le nuove informazioni
				if (docSel != null)
				{
					docSel.descrizione = this.TextNote.Text;
					DocumentManager.modificaVersione(this, docSel);
					schedaDocumento.documenti[indexSel] = docSel;
					DocumentManager.setDocumentoSelezionato(this, schedaDocumento);
				}
			}
			else
			{
				//creo la nuova versione
				DocsPaWR.Documento fileReq = new DocsPAWA.DocsPaWR.Documento();
				fileReq.descrizione = this.TextNote.Text;
				fileReq.docNumber = schedaDocumento.docNumber;
				
				//parte relativa alla data arrivo
				fileReq.dataArrivo = schedaDocumento.documenti[schedaDocumento.documenti.Length-1].dataArrivo;
				
				fileReq = (DocsPAWA.DocsPaWR.Documento) DocumentManager.aggiungiVersione(this, fileReq, true, true);						
			} 
			
			funct += " window.close(); ";
			Response.Write("<script> " + funct + "</script>");
		}
		#endregion

		#region Pulsanti
		/// <summary>
		/// pulsante Conferma
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_ok_Click(object sender, System.EventArgs e)
		{
			string flag_btn = (string) Session["gestioneVersione"];
			if (flag_btn == null || flag_btn.Equals(""))
			{
                if (this.TextNote.Text.Trim().Length <= 200)
                {
                    //setto la variabile di Sessione
                    Session.Add("gestioneVersione", "N");
                    gestioneVersione();
                }
                else
                {
                    int strLen = this.TextNote.Text.Trim().Length;
                    string avviso = "Lunghezza NOTE eccessiva: " + strLen + " caratteri, max 200";
                    Page.RegisterStartupScript("", "<script>alert('" + avviso + "');if(document.getElementById('TextNote')!=null){document.getElementById('TextNote').value = document.getElementById('TextNote').value.substring(0,200);}</script>");
                   
                }
			}
			else
			{
				string  funct = " window.opener.top.principale.iFrame_sx.document.location='../documento/tabGestioneDoc.aspx?tab=versioni'; ";

				funct = funct + " window.close(); ";
				Response.Write("<script> " + funct + "</script>");
			}
		}

		/// <summary>
		/// pulsante Chiudi
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void butt_chiudi_Click(object sender, System.EventArgs e)
		{
			Response.Write("<script>window.close();</script>");	
		}
		#endregion
	}
}
