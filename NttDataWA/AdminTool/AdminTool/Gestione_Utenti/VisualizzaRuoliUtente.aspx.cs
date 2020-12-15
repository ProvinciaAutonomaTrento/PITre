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

namespace Amministrazione.Gestione_Utenti
{
	/// <summary>
	/// Summary description for VisualizzaRuoliUtente.
	/// </summary>
	public class VisualizzaRuoliUtente : System.Web.UI.Page
	{
		#region WebControls e variabili
		protected System.Web.UI.WebControls.DataGrid dg_ruoli;
		protected System.Web.UI.WebControls.Label lbl_testa;
		protected System.Web.UI.WebControls.Button btn_salvaPref;
		protected System.Web.UI.HtmlControls.HtmlInputHidden hd_idPeople;
		protected System.Web.UI.WebControls.Button btn_chiudi;
		//-----------------------------------------------------------------------
		private DataSet dataSet = new DataSet();
		#endregion

		#region Page Load
		private void Page_Load(object sender, System.EventArgs e)
		{
			Response.Expires = -1;

			//----- CONTROLLO DELL'UTENTE AMMINISTRATORE CONNESSO -----------
			if(Session.IsNewSession)
			{
				Response.Redirect("../Exit.aspx?FROM=EXPIRED");
			}
			
			AmmUtils.WebServiceLink  ws=new AmmUtils.WebServiceLink();
			if(!ws.CheckSession(Session.SessionID))
			{
				Response.Redirect("../Exit.aspx?FROM=ABORT");
			}
			// ---------------------------------------------------------------			
			
			if (!IsPostBack)
			{
				try
				{
					this.btn_chiudi.Attributes.Add("onclick","javascript: window.close();");
					this.lbl_testa.Text = "Ruoli dell'utente: " + Server.UrlDecode(Request.QueryString["nome"].ToString().Replace("|@ap@|","'"));
					this.hd_idPeople.Value = Request.QueryString["idPeople"].ToString();
					this.Inizialize();					
				}
				catch
				{
					
				}				
			} 		
		}

		private void Inizialize()
		{
			try
			{
				Manager.UtentiManager theManager = new Amministrazione.Manager.UtentiManager();
				theManager.RuoliUtente(Request.QueryString["idPeople"].ToString());

				if(theManager.getRuoliUtente() != null && theManager.getRuoliUtente().Count > 0)
				{
					this.LoadRuoliUtente(theManager.getRuoliUtente());
				}
			}
			catch
			{
				this.AlertJS("Attenzione, si è verificato un errore");
			}
		}

		#endregion

		#region datagrid Ruoli
		private void IniDataset()
		{
			//DataSet contenente la lista dei ruoli
			dataSet = new DataSet();
			dataSet.Tables.Add("Ruoli");

			DataColumn dc = new DataColumn("IDGRUPPO");
			dataSet.Tables["Ruoli"].Columns.Add(dc);

			dc = new DataColumn("CODICE");
			dataSet.Tables["Ruoli"].Columns.Add(dc);

			dc = new DataColumn("DESCRIZIONE");
			dataSet.Tables["Ruoli"].Columns.Add(dc);

			dc = new DataColumn("PREFERITO");
			dataSet.Tables["Ruoli"].Columns.Add(dc);
		}


		private void LoadRuoliUtente(ArrayList listaRuoli)
		{
			this.IniDataset();			

			try
			{
				DataRow row;
				foreach(SAAdminTool.DocsPaWR.OrgRuolo ruolo in listaRuoli)
				{
					row = dataSet.Tables["Ruoli"].NewRow();
					row["IDGRUPPO"] = ruolo.IDGruppo;
					row["CODICE"] = ruolo.Codice;
					row["DESCRIZIONE"] = ruolo.Descrizione;				
					row["PREFERITO"] = ruolo.DiRiferimento;
				
					dataSet.Tables["Ruoli"].Rows.Add(row);
				}
			
				DataView dv = dataSet.Tables["Ruoli"].DefaultView;
				dv.Sort = "DESCRIZIONE ASC";
				this.dg_ruoli.DataSource = dv;
				this.dg_ruoli.DataBind();		
			}
			catch
			{
				this.AlertJS("Attenzione, si è verificato un errore");
			}
		}

		private void dg_ruoli_PreRender(object sender, System.EventArgs e)
		{            
            RadioButton rd;            
            string numObj=string.Empty;

			foreach(DataGridItem item in this.dg_ruoli.Items)
			{                
				rd = (RadioButton)item.FindControl("RBpref");
                if(rd!=null)
                {
                    if((item.ItemIndex + 2) < 10)
                        numObj = "0" + Convert.ToString(item.ItemIndex + 2);
                    else
                        numObj = Convert.ToString(item.ItemIndex + 2);

                    //rd.Attributes.Add("onClick", "javascript:SelectChange('" + rd.UniqueID + "');");
                    rd.Attributes.Add("onClick", "javascript:SelectChange('dg_ruoli$ctl"+numObj+"$ruolo_pref');");                 
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
			this.dg_ruoli.PreRender += new System.EventHandler(this.dg_ruoli_PreRender);
			this.btn_salvaPref.Click += new System.EventHandler(this.btn_salvaPref_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		#region Ruolo preferito

		private void btn_salvaPref_Click(object sender, System.EventArgs e)
		{
			Manager.UtentiManager theManager = new Amministrazione.Manager.UtentiManager();
            SAAdminTool.DocsPaWR.EsitoOperazione esito = new SAAdminTool.DocsPaWR.EsitoOperazione();

			string idGruppo = GetIDGruppoSelezionato();

			if(idGruppo!=null && idGruppo != string.Empty)
			{
				theManager.ImpostaRuoloPreferito(this.hd_idPeople.Value,idGruppo);
				
				esito = theManager.getEsitoOperazione();

				if(esito.Codice.Equals(1))
				{
					this.AlertJS("Attenzione, " + esito.Descrizione);
				}
			}
		}

		private string GetIDGruppoSelezionato()
		{
			string idGruppo = string.Empty;

			foreach(DataGridItem item in this.dg_ruoli.Items)
			{
				RadioButton rd = (RadioButton)item.FindControl("RBpref");
				if ((rd!=null) && rd.Checked == true)
				{		
					idGruppo = item.Cells[0].Text;					
					break;
				}
			}		

			return idGruppo;
		}
		#endregion

		#region Gestione errori

		private void AlertJS(string msg)
		{
			if(!this.Page.IsStartupScriptRegistered("alertJavaScript"))
			{					
				string scriptString = "<SCRIPT>alert('" + msg.Replace("'","\\'") + "');</SCRIPT>";				
				this.Page.RegisterStartupScript("alertJavaScript", scriptString);
			}	
		}

		#endregion
	}
}
