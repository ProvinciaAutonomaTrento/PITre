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

namespace DocsPAWA.AdminTool.Gestione_Organigramma
{
	/// <summary>
	/// Summary description for GestVisibilita.
	/// </summary>
	public class GestVisibilita : System.Web.UI.Page
	{
		#region WebControls e variabili
		protected System.Web.UI.WebControls.DataGrid dg_registri;
		protected System.Web.UI.WebControls.Button btn_esegui;
		protected System.Web.UI.WebControls.Button btn_chiudi;
		protected System.Web.UI.WebControls.Label lbl_intestazione;
		protected System.Web.UI.HtmlControls.HtmlInputHidden from;
        protected System.Web.UI.WebControls.CheckBox cb_atipicita;
	
		protected DataSet dsRegistri;
		#endregion

		#region Page Load
		/// <summary>
		/// Page Load
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Page_Load(object sender, System.EventArgs e)
		{
			try
			{
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

				this.Response.Expires = -1;				

				if(!IsPostBack)
				{
					try
					{					
						this.from.Value=this.Request.QueryString["from"].ToString();
						this.LoadDettagliRegistri();						
					}
					catch
					{						
						this.GUI("ResetAll");
					}
				}

                //Gestione atipicità 
                string[] amministrazione = ((string)Session["AMMDATASET"]).Split('@');
                string codiceAmministrazione = amministrazione[0];
                string idAmm = this.Request.QueryString["idAmm"].ToString();
                
                DocsPaWR.Utente ut = new DocsPAWA.DocsPaWR.Utente();
                ut.codiceAmm = codiceAmministrazione;
                ut.idAmministrazione = idAmm;
                ut.tipoIE = "I";
                Session.Add("userData", ut);

                if (Utils.GetAbilitazioneAtipicita())
                    this.cb_atipicita.Visible = true;
                else
                    this.cb_atipicita.Visible = false;
			}
			catch
			{				
				this.GUI("ResetAll");
			}
		}

		/// <summary>
		/// gestore visibilità oggetti sulla GUI
		/// </summary>
		/// <param name="from"></param>
		private void GUI(string from)
		{
			switch (from)
			{
				case "ResetAll": //........................ annulla tutto!
					this.lbl_intestazione.Text = "Attenzione! si è verificato un errore";
					this.dg_registri.Visible = false;
					this.btn_esegui.Visible = false;					
					break;
				case "End": //............................. procedura terminata!					
					this.dg_registri.Columns[6].Visible=false;
					this.dg_registri.Columns[7].Visible=true;
					this.btn_esegui.Visible=false;
					this.btn_chiudi.Enabled=true;
					this.btn_chiudi.Text = "Chiudi";
					break;
			}
		}
		#endregion

		#region datagrid Registri
		/// <summary>
		/// Caricamento della lista dei registri associati al ruolo
		/// </summary>
		private void LoadDettagliRegistri()
		{
			string idAmm = this.Request.QueryString["idAmm"].ToString();
			string idCorrGlobUO = this.Request.QueryString["idCorrGlobUO"].ToString();
			string idCorrGlobRuolo = this.Request.QueryString["idCorrGlobRuolo"].ToString();
			string idGruppo = this.Request.QueryString["idGruppo"].ToString();
		
			Amministrazione.Manager.OrganigrammaManager theManager = new Amministrazione.Manager.OrganigrammaManager();
			theManager.ListaRegistriAssRuolo(idAmm,idCorrGlobRuolo);			

			if(theManager.getListaRegistri()!=null && theManager.getListaRegistri().Count>0)
			{				
				InitializeDataSetRegistri();
				DataRow row;
				foreach(DocsPAWA.DocsPaWR.OrgRegistro registro in theManager.getListaRegistri())
				{
					row = dsRegistri.Tables["REGISTRI"].NewRow();

					row["IDRegistro"] = registro.IDRegistro;
					string testo = string.Empty;						
					testo = "<b>Codice: </b>" + registro.Codice + "<br>";
					testo += "<b>Descrizione: </b>" + registro.Descrizione + "<br>";
					testo += "<b>Registro associato al ruolo dal: </b>" + registro.data_inizio + "<br>";
					testo += "<b>Visibilità sui documenti aggiornata il: </b>" + registro.data_ass_visibilita;
					row["Descrizione"] = testo;
					row["IDAmministrazione"] = registro.IDAmministrazione;
					row["idCorrGlobUO"] = idCorrGlobUO;
					row["idCorrGlobRuolo"] = idCorrGlobRuolo;
					row["idGruppo"] = idGruppo;

					dsRegistri.Tables["REGISTRI"].Rows.Add(row);
				}

				DataView dv = dsRegistri.Tables["REGISTRI"].DefaultView;
				dv.Sort = "Descrizione";
				this.dg_registri.DataSource = dv;
				this.dg_registri.DataBind();
			}
			else
			{
				if(!this.Page.IsStartupScriptRegistered("alertJavaScript"))
				{					
					string scriptString = "<SCRIPT>alert('Attenzione, non ci sono registri associati al ruolo');self.close();</SCRIPT>";				
					this.Page.RegisterStartupScript("alertJavaScript", scriptString);
				}
			}
		}

		/// <summary>
		/// Inizializzazione dataset
		/// </summary>
		private void InitializeDataSetRegistri()
		{
			dsRegistri = new DataSet();
			DataColumn dc;
			dsRegistri.Tables.Add("REGISTRI");
			dc = new DataColumn("IDRegistro");
			dsRegistri.Tables["REGISTRI"].Columns.Add(dc);			
			dc = new DataColumn("Descrizione");
			dsRegistri.Tables["REGISTRI"].Columns.Add(dc);
			dc = new DataColumn("IDAmministrazione");
			dsRegistri.Tables["REGISTRI"].Columns.Add(dc);	
			dc = new DataColumn("idCorrGlobUO");
			dsRegistri.Tables["REGISTRI"].Columns.Add(dc);
			dc = new DataColumn("idCorrGlobRuolo");
			dsRegistri.Tables["REGISTRI"].Columns.Add(dc);
			dc = new DataColumn("idGruppo");
			dsRegistri.Tables["REGISTRI"].Columns.Add(dc);
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
			this.btn_esegui.Click += new System.EventHandler(this.btn_esegui_Click);
			this.btn_chiudi.Click += new System.EventHandler(this.btn_chiudi_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		#region tasti
		/// <summary>
		/// Tasto Annulla
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_chiudi_Click(object sender, System.EventArgs e)
		{
			string scriptString = string.Empty;

			if(!this.Page.IsStartupScriptRegistered("closeJavaScript"))
			{					
				if(this.from.Value.Equals("OR")) // proviene dalla pagina Organigramma
					scriptString = "<SCRIPT>self.close();</SCRIPT>";	
				else	// proviene da sposta ruolo tra UO
					scriptString = "<SCRIPT>window.returnValue = 'Y'; self.close();</SCRIPT>";

				this.Page.RegisterStartupScript("closeJavaScript", scriptString);
			}	
		}

		/// <summary>
		/// Tasto esecuzione procedura
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_esegui_Click(object sender, System.EventArgs e)
		{
			string IDRegistro = string.Empty;
			string IDAmministrazione = string.Empty;
			string idCorrGlobUO = string.Empty;
			string idCorrGlobRuolo = string.Empty;
			string idGruppo = string.Empty;
			string livelloRuolo = string.Empty;
			bool checkedExist = false;
			int countChecked = 0;

			CheckBox spunta;

			Amministrazione.Manager.OrganigrammaManager theManager = new Amministrazione.Manager.OrganigrammaManager();
			DocsPaWR.EsitoOperazione esito = new DocsPAWA.DocsPaWR.EsitoOperazione();

			try
			{			
				if(!this.VerificaUtConn(this.dg_registri.Items[0].Cells[2].Text))
				{
					for(int i=0; i<this.dg_registri.Items.Count; i++)
					{
						spunta = (CheckBox) dg_registri.Items[i].Cells[6].FindControl("Chk_registri");	

						if(spunta.Checked)
						{
							checkedExist = true;
							countChecked++;
	
							spunta.Enabled = false;
							spunta.Checked = false;

							IDRegistro = this.dg_registri.Items[i].Cells[0].Text;
							IDAmministrazione = this.dg_registri.Items[i].Cells[2].Text;
							idCorrGlobUO = this.dg_registri.Items[i].Cells[3].Text;
							idCorrGlobRuolo = this.dg_registri.Items[i].Cells[4].Text;
							idGruppo =  this.dg_registri.Items[i].Cells[5].Text;

							if(livelloRuolo.Equals(string.Empty))
								livelloRuolo = theManager.GetLivelloRuolo(idCorrGlobRuolo);

                            theManager.EstendeVisibRuolo(IDRegistro,idCorrGlobRuolo,idGruppo,idCorrGlobUO,IDAmministrazione,livelloRuolo, cb_atipicita.Checked);
                            esito = theManager.getEsitoOperazione();

							if(esito.Codice.Equals(0))		
							{
								this.dg_registri.Items[i].Cells[7].Text = "<font color=green><b>OK</b></font>";							
							}
							else
							{
								this.dg_registri.Items[i].Cells[7].Text = "<font color=red>" + esito.Descrizione + "</font>";								
							}
						}
						else
						{
							if(!spunta.Enabled)
								countChecked++;
						}
					}
				}					

				if(countChecked == this.dg_registri.Items.Count)
					this.btn_esegui.Visible = false;

				if(checkedExist)
				{
					this.btn_chiudi.Enabled=true;
					this.btn_chiudi.Text = "Chiudi";
				}
			}
			catch
			{				
				this.GUI("ResetAll");
			}
		}
		#endregion

		#region verifica utenti connessi
		/// <summary>
		/// 
		/// </summary>
		/// <param name="idAmm"></param>
		/// <returns></returns>
		private bool VerificaUtConn(string idAmm)
		{
			bool retValue = false;

			return retValue;
		}
		#endregion
	}
}
