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

namespace DocsPAWA.organigramma
{
	/// <summary>
	/// Summary description for RisultatoRicercaOrg.
	/// </summary>
	public class RisultatoRicercaOrg : DocsPAWA.CssPage
	{
		#region WebControls e variabili

		protected System.Web.UI.WebControls.DataGrid dg_listaRicerca;
		protected System.Web.UI.WebControls.Panel pnl_risultato;
		protected System.Web.UI.WebControls.Label lbl_ContaRec;
		protected System.Web.UI.WebControls.Label lbl_percorso;
		//----------------------------------------------------------------------
		protected DataSet dsRisultato;
	
		#endregion

		#region Page Load
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Page_Load(object sender, System.EventArgs e)
		{
			Response.Cache.SetCacheability (HttpCacheability.NoCache);
			Response.Expires = -1;
			//Response.Buffer = false;
			
			if(!IsPostBack)
			{
				try
				{
					this.Inizialize();
				}
				catch
				{

				}
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
			this.dg_listaRicerca.PreRender += new System.EventHandler(this.dg_listaRicerca_PreRender);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		private void Inizialize()
		{			
			int num_trovati = 0;
			string returnValue = string.Empty;
			string codice = string.Empty;
			string descrizione = string.Empty;
			string idAmm = string.Empty;
			string tipo = string.Empty;
            bool searchHistoricized = false;

			if(Request.QueryString["cod"] != null && Request.QueryString["cod"] != "")
				codice = Request.QueryString["cod"].Trim();

			if(Request.QueryString["desc"] != null && Request.QueryString["desc"] != "")
				descrizione = Request.QueryString["desc"].Trim();

            if (!String.IsNullOrEmpty(Request["searchHistoricized"]))
                Boolean.TryParse(Request["searchHistoricized"], out searchHistoricized);

			if(Request.QueryString["idAmm"] != null && Request.QueryString["idAmm"] != "")
			{
				idAmm = Request.QueryString["idAmm"];
				tipo = Request.QueryString["tipo"];

				this.lbl_percorso.Text = "Risultato ricerca per: " + codice + " " + descrizione + "";

				Amministrazione.Manager.OrganigrammaManager theManager = new Amministrazione.Manager.OrganigrammaManager();
				
				theManager.RicercaInOrg(tipo,codice,descrizione,idAmm, searchHistoricized, false);

				if(theManager.getRisultatoRicerca()!=null && theManager.getRisultatoRicerca().Count>0)
				{					
					this.lbl_ContaRec.Text = "Risultato: " + theManager.getRisultatoRicerca().Count.ToString() + " records.";
					this.pnl_risultato.Visible=true;					

					this.SetHeaderDG(tipo);

					this.InitializeDataSetRisultato();

					DataRow row;
					foreach(DocsPAWA.DocsPaWR.OrgRisultatoRicerca risultato in theManager.getRisultatoRicerca())
					{
						returnValue = risultato.IDCorrGlob + "_" + risultato.IDParent + "_" + tipo;

						row = dsRisultato.Tables[0].NewRow();
						row["IDCorrGlob"] = risultato.IDCorrGlob;												
						row["Codice"] = "<a href=\"javascript:void(0)\" onclick=\"window.returnValue='" + returnValue + "'; window.close();\">" + risultato.Codice + "</a>";	
						row["Descrizione"] = "<a href=\"javascript:void(0)\" onclick=\"window.returnValue='" + returnValue + "'; window.close();\">" + risultato.Descrizione + "</a>";
						row["IDParent"] = risultato.IDParent;
						row["DescParent"] = risultato.DescParent;
					
						dsRisultato.Tables["LISTA_RICERCA"].Rows.Add(row);

						num_trovati++;
					}

					if(num_trovati>1)
					{
						DataView dv = dsRisultato.Tables["LISTA_RICERCA"].DefaultView;
						dv.Sort = "Descrizione ASC";
						this.dg_listaRicerca.DataSource = dv;
						this.dg_listaRicerca.DataBind();										
					}
					else
					{
						this.executeJS("<SCRIPT>window.returnValue='" + returnValue + "'; window.close();</SCRIPT>");
					}					
				}
				else
				{
					//this.pnl_risultato.Visible=false;
					//this.lbl_ContaRec.Text = "Nessun dato trovato.";
					this.executeJS("<SCRIPT>alert('Nessun dato trovato');window.close();</SCRIPT>");
				}
			}						
		}

		private void InitializeDataSetRisultato()
		{
			dsRisultato = new DataSet();
			DataColumn dc;
			dsRisultato.Tables.Add("LISTA_RICERCA");
			dc = new DataColumn("IDCorrGlob");
			dsRisultato.Tables["LISTA_RICERCA"].Columns.Add(dc);
			dc = new DataColumn("Codice");
			dsRisultato.Tables["LISTA_RICERCA"].Columns.Add(dc);
			dc = new DataColumn("Descrizione");
			dsRisultato.Tables["LISTA_RICERCA"].Columns.Add(dc);
			dc = new DataColumn("IDParent");
			dsRisultato.Tables["LISTA_RICERCA"].Columns.Add(dc);
			dc = new DataColumn("DescParent");
			dsRisultato.Tables["LISTA_RICERCA"].Columns.Add(dc);
		}

		private void SetHeaderDG(string tipo)
		{
			switch(tipo)
			{
				case "U":
					this.dg_listaRicerca.Columns[2].HeaderText = "Unita' organizzativa";
					this.dg_listaRicerca.Columns[4].HeaderText = "Unita' org. superiore";
					break;
				case "R":
					this.dg_listaRicerca.Columns[2].HeaderText = "Ruolo";
					this.dg_listaRicerca.Columns[4].HeaderText = "Unita' org. di appartenenza";
					break;
				case "PN":
					this.dg_listaRicerca.Columns[2].HeaderText = "Nomi utente";
					this.dg_listaRicerca.Columns[4].HeaderText = "Ruolo di appartenenza";
					break;
				case "PC":
					this.dg_listaRicerca.Columns[2].HeaderText = "Cognomi utente";
					this.dg_listaRicerca.Columns[4].HeaderText = "Ruolo di appartenenza";
					break;						
			}
		}

		private void dg_listaRicerca_PreRender(object sender, System.EventArgs e)
		{
			for(int i=0;i<this.dg_listaRicerca.Items.Count;i++)
			{

				if(this.dg_listaRicerca.Items[i].ItemIndex>=0)
				{     
					switch(this.dg_listaRicerca.Items[i].ItemType.ToString().Trim())
					{
						case "Item":						
							this.dg_listaRicerca.Items[i].Attributes.Add("onmouseover","this.className='bg_grigioS'");
							this.dg_listaRicerca.Items[i].Attributes.Add("onmouseout","this.className='bg_grigioN'");
							break;
						
						case "AlternatingItem":                             						
							this.dg_listaRicerca.Items[i].Attributes.Add("onmouseover","this.className='bg_grigioS'");
							this.dg_listaRicerca.Items[i].Attributes.Add("onmouseout","this.className='bg_grigioA'");
							break;						                                 
					}
				}
			}
		}

		private void executeJS(string key)
		{
			if(!this.Page.IsStartupScriptRegistered("theJS"))			
				this.Page.RegisterStartupScript("theJS", key);
		}
	}
}
