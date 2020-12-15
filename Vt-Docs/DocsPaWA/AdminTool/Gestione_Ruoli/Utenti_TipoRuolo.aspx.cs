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

namespace DocsPAWA.AdminTool.Gestione_Ruoli
{
	/// <summary>
	/// Summary description for Utenti_TipoRuolo.
	/// </summary>
	public class Utenti_TipoRuolo : System.Web.UI.Page
	{
		protected System.Web.UI.WebControls.DataGrid dg_utenti;
		protected System.Web.UI.WebControls.Label lbl_info;
		//-----------------------------------------------------
		protected DataSet dsUtenti;
	
		private void Page_Load(object sender, System.EventArgs e)
		{	
			this.Response.Expires=-1;

			if(!IsPostBack)
			{
				if((Request.QueryString["codAmm"].ToString()!=null && Request.QueryString["codAmm"].ToString()!=string.Empty) ||
					(Request.QueryString["codRuolo"].ToString()!=null && Request.QueryString["codRuolo"].ToString()!=string.Empty))
				{
					this.lbl_info.Text = "Utenti presenti nei ruoli in UO con tipologia: " + Request.QueryString["descRuolo"].ToString().Replace("'","\\'");
					this.Inizialize();
				}
			}			
		}

		/// <summary>
		/// 
		/// </summary>
		private void Inizialize()
		{
			try
			{
				string codRuolo = Request.QueryString["codRuolo"].ToString();
				string codAmm = Request.QueryString["codAmm"].ToString();

				DocsPAWA.AdminTool.Manager.TipiRuoloManager manager = new DocsPAWA.AdminTool.Manager.TipiRuoloManager();
				manager.ListaTipoRuoloUtenti(codRuolo,codAmm);
				ArrayList lista = manager.getListaTipoRuoloUtenti();

				if(lista!=null && lista.Count>0)
				{
					this.LoadLista(lista);
				}
				else
				{
					if(!this.Page.IsStartupScriptRegistered("alertJavaScript"))
					{					
						string scriptString = "<SCRIPT>alert('Attenzione, nessun utente presente!');window.close();</SCRIPT>";				
						this.Page.RegisterStartupScript("alertJavaScript", scriptString);
					}
				}
				
			}
			catch
			{
				this.lbl_info.Text = "Attenzione! si è verificato un errore nella generazione della lista degli utenti";
				this.dg_utenti.Visible = false;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="lista"></param>
		private void LoadLista(ArrayList lista)
		{
			this.InizializeDataSet();

			DataRow row;
			foreach(DocsPAWA.DocsPaWR.OrgRuolo ruolo in lista)
			{								
				if(ruolo.Utenti!=null && ruolo.Utenti.Length>0)
				{
					row = dsUtenti.Tables["LISTA"].NewRow();
					row["utente"] = this.LoadUtenti(ruolo);
					row["ruolo"] = ruolo.Descrizione;
					dsUtenti.Tables["LISTA"].Rows.Add(row);
				}								
			}

			DataView dv = dsUtenti.Tables["LISTA"].DefaultView;
			dv.Sort = "ruolo ASC";			
			dg_utenti.DataSource = dv;
			dg_utenti.DataBind();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="ruolo"></param>
		/// <returns></returns>
		private string LoadUtenti(DocsPAWA.DocsPaWR.OrgRuolo ruolo)
		{
			string listaUtenti = string.Empty;
		
			foreach(DocsPAWA.DocsPaWR.OrgUtente utente in ruolo.Utenti)
			{
				listaUtenti += utente.Cognome + " " + utente.Nome + "<br>";
			}

			return listaUtenti;
		}

		/// <summary>
		/// 
		/// </summary>
		private void InizializeDataSet()
		{
			dsUtenti = new DataSet();
			dsUtenti.Tables.Add("LISTA");
			
			DataColumn dc;			
			dc = new DataColumn("ruolo");
			dsUtenti.Tables["LISTA"].Columns.Add(dc);
			dc = new DataColumn("utente");
			dsUtenti.Tables["LISTA"].Columns.Add(dc);			
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

		}
		#endregion
	}
}
