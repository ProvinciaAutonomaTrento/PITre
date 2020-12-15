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
using SAAdminTool;

namespace Amministrazione.Gestione_Utenti
{
	/// <summary>
	/// Summary description for VisualizzaRegistriUtente.
	/// </summary>
	public class VisualizzaRegistriUtente : System.Web.UI.Page
	{
		protected System.Web.UI.WebControls.DataGrid dg_registri;
		protected System.Web.UI.WebControls.Button btn_mod_registri;
		protected System.Web.UI.WebControls.Label titolo;
		protected System.Web.UI.WebControls.Button btn_chiudi;
		protected DataSet dsRegistri;
	
		private void Page_Load(object sender, System.EventArgs e)
		{
			Response.Expires = -1;

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

				if(!IsPostBack)
				{
					this.btn_chiudi.Attributes.Add("onclick","window.close();");
					this.Inizialize();
				}
			}
			catch
			{
				this.GUI("Errore");
			}
		}

		private void Inizialize()
		{
			string idCorrGlob = this.Request.QueryString["idCorrGlob"];
			string idAmm = this.Request.QueryString["idAmm"];
			
			this.GetListaRegistri(idAmm,idCorrGlob);
		}

		private void GetListaRegistri(string idAmm, string idCorrGlob)
		{
			try
			{	
				Manager.UtentiManager theManager = new Amministrazione.Manager.UtentiManager();				
				theManager.RegistriUtente(idCorrGlob,idAmm);	

				if(theManager.getRegistriUtente()!=null && theManager.getRegistriUtente().Count>0)
				{					
					InitializeDataSetRegistri();
					DataRow row;
					foreach(SAAdminTool.DocsPaWR.OrgRegistro registro in theManager.getRegistriUtente())
					{
						row = dsRegistri.Tables[0].NewRow();
						row["IDRegistro"] = registro.IDRegistro;
						row["Codice"] = registro.Codice;
						row["Descrizione"] = registro.Descrizione;
						row["IDAmministrazione"] = idAmm;
						row["IDCorrGlob"] = idCorrGlob;
						if(registro.Associato != null && registro.Associato != String.Empty)
						{
							row["Sel"] = "true";
						}
						else
						{
							row["Sel"] = "false";
						}
						dsRegistri.Tables["REGISTRI"].Rows.Add(row);
					}

					DataView dv = dsRegistri.Tables["REGISTRI"].DefaultView;
					dv.Sort = "Descrizione ASC";
					this.dg_registri.DataSource = dv;
					this.dg_registri.DataBind();

					this.GUI("Ok");
				}
				else
				{
					this.GUI("NoDataFound");
				}
			}
			catch
			{
				this.GUI("Errore");
			}
		}

		private void InitializeDataSetRegistri()
		{
			dsRegistri = new DataSet();
			DataColumn dc;
			dsRegistri.Tables.Add("REGISTRI");
			dc = new DataColumn("IDRegistro");
			dsRegistri.Tables["REGISTRI"].Columns.Add(dc);
			dc = new DataColumn("Codice");
			dsRegistri.Tables["REGISTRI"].Columns.Add(dc);
			dc = new DataColumn("Descrizione");
			dsRegistri.Tables["REGISTRI"].Columns.Add(dc);
			dc = new DataColumn("IDAmministrazione");
			dsRegistri.Tables["REGISTRI"].Columns.Add(dc);
			dc = new DataColumn("IDCorrGlob");
			dsRegistri.Tables["REGISTRI"].Columns.Add(dc);
			dc = new DataColumn("Sel");
			dsRegistri.Tables["REGISTRI"].Columns.Add(dc);
		}

		private void GUI(string key)
		{
			switch(key)
			{
				case "Errore":
					this.titolo.Text = "Si è verificato un errore!";
					this.dg_registri.Visible=false;
					this.btn_mod_registri.Visible=false;
					this.btn_chiudi.Visible=true;
					break;
				case "NoDataFound":
					this.titolo.Text = "Nessun registro disponibile!";
					this.dg_registri.Visible=false;
					this.btn_mod_registri.Visible=false;
					this.btn_chiudi.Visible=true;
					break;
				case "Ok":
					this.titolo.Text = "Registri disponibili:";
					this.dg_registri.Visible=true;
					this.btn_mod_registri.Visible=true;
					this.btn_chiudi.Visible=true;
					break;
			}
		}

		private void btn_mod_registri_Click(object sender, System.EventArgs e)
		{
			this.InserimentoRegistri();
		}
		
		private void InserimentoRegistri()
		{
			string idCorrGlob = string.Empty;
			try
			{
                idCorrGlob = this.Request.QueryString["idCorrGlob"];

				if(this.dg_registri.Items.Count > 0)
				{					
					CheckBox spunta;
					SAAdminTool.DocsPaWR.OrgRegistro registro = null;
					ArrayList listaRegistriSelezionati = new ArrayList();

					for(int i=0; i<this.dg_registri.Items.Count; i++)
					{
						spunta = (CheckBox) dg_registri.Items[i].Cells[5].FindControl("Chk_registri");	

						if(spunta.Checked)
						{
							registro = new SAAdminTool.DocsPaWR.OrgRegistro();
							registro.IDRegistro = dg_registri.Items[i].Cells[0].Text;							
							listaRegistriSelezionati.Add(registro);					
							registro=null;
							idCorrGlob = dg_registri.Items[i].Cells[4].Text;
						}
					}

					if(listaRegistriSelezionati != null && listaRegistriSelezionati.Count > 0)
					{
						SAAdminTool.DocsPaWR.OrgRegistro[] registri=new SAAdminTool.DocsPaWR.OrgRegistro[listaRegistriSelezionati.Count];
						listaRegistriSelezionati.CopyTo(registri);
						listaRegistriSelezionati=null;
												
						Manager.UtentiManager theManager = new Amministrazione.Manager.UtentiManager();				
						theManager.InsRegistriUtente(registri,idCorrGlob);	

						SAAdminTool.DocsPaWR.EsitoOperazione esito = new SAAdminTool.DocsPaWR.EsitoOperazione();
						esito = theManager.getEsitoOperazione();

						if(esito.Codice.Equals(0))		
						{
							if(!this.Page.IsStartupScriptRegistered("closeJavaScript"))
							{					
								string scriptString = "<SCRIPT>window.close();</SCRIPT>";				
								this.Page.RegisterStartupScript("closeJavaScript", scriptString);
							}	
						}
						else
						{
							if(!this.Page.IsStartupScriptRegistered("alertJavaScript"))
							{					
								string scriptString = "<SCRIPT>alert('Attenzione, " + esito.Descrizione.Replace("'","''") + "');</SCRIPT>";				
								this.Page.RegisterStartupScript("alertJavaScript", scriptString);
							}	
						}

						esito = null;
					}
					else
					{
                        //gestione cancellazione dati

                        Manager.UtentiManager theManager = new Amministrazione.Manager.UtentiManager();
                        theManager.EliminaRegistriUtente(idCorrGlob);

                        SAAdminTool.DocsPaWR.EsitoOperazione esito = new SAAdminTool.DocsPaWR.EsitoOperazione();
                        esito = theManager.getEsitoOperazione();

                        if (esito.Codice.Equals(0))
                        {
                            if (!this.Page.IsStartupScriptRegistered("execJavaScript"))
                            {
                                string scriptString = "<SCRIPT>window.returnValue = 'noAmmTitolario'; window.close();</SCRIPT>";
                                this.Page.RegisterStartupScript("execJavaScript", scriptString);
                            }
                        }
                        else
                        {
                            if (!this.Page.IsStartupScriptRegistered("alertJavaScript"))
                            {
                                string scriptString = "<SCRIPT>alert('Attenzione, " + esito.Descrizione.Replace("'", "''") + "');</SCRIPT>";
                                this.Page.RegisterStartupScript("alertJavaScript", scriptString);
                            }
                        }

                        esito = null;

                        //vecchio sistema
                        //if(!this.Page.IsStartupScriptRegistered("execJavaScript"))
                        //{					
                        //    string scriptString = "<SCRIPT>window.returnValue = 'noAmmTitolario'; window.close();</SCRIPT>";				
                        //    this.Page.RegisterStartupScript("execJavaScript", scriptString);
                        //}	
					}
				}
			}
			catch
			{				
				this.GUI("Errore");
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
			this.btn_mod_registri.Click += new System.EventHandler(this.btn_mod_registri_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion		
	}
}
