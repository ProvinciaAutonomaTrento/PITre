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
using System.IO;
using System.Xml;
using Microsoft.Web.UI.WebControls;

namespace Amministrazione.Gestione_Logs
{
	/// <summary>
	/// Summary description for TipiFunzione.
	/// </summary>
	public class QueryLogAmm : System.Web.UI.Page
	{
		#region WebControls e variabili
		protected System.Web.UI.WebControls.Label lbl_tit;
		protected System.Web.UI.WebControls.Button btn_cerca;
        protected SAAdminTool.AdminTool.Getione_Logs.LogCalendar txt_data_da;
        protected SAAdminTool.AdminTool.Getione_Logs.LogCalendar txt_data_a;
        //protected System.Web.UI.WebControls.TextBox txt_data_da;
		//protected System.Web.UI.WebControls.TextBox txt_data_a;
		protected System.Web.UI.WebControls.TextBox txt_user;
		protected System.Web.UI.WebControls.DropDownList ddl_oggetto;
		protected System.Web.UI.WebControls.DropDownList ddl_azione;
		protected System.Web.UI.WebControls.Panel pnl_log;		
		protected System.Web.UI.WebControls.Label lbl_position;
		protected System.Web.UI.WebControls.DataGrid dg_Log;
		protected System.Web.UI.WebControls.DropDownList ddl_esito;
		protected DataSet dataSet;
        protected System.Web.UI.WebControls.CheckBox ckb_Log;
        protected System.Web.UI.WebControls.CheckBox ckb_Storico;
        protected System.Web.UI.WebControls.ImageButton btn_stampa;
        protected System.Web.UI.WebControls.ImageButton eliminaDataDa;
        protected System.Web.UI.WebControls.ImageButton eliminaDataA;
        protected string codAmm;
        #endregion

		#region Page_Load
		private void Page_Load(object sender, System.EventArgs e)
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

			if (!IsPostBack)
			{
                this.btn_stampa.Attributes.Add("onclick", "StampaRisultatoRicerca();");

                lbl_position.Text = "&nbsp;&bull;&nbsp;Amministrazione: " + AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "1");
				lbl_tit.Text		= "Filtri di ricerca:";
				//txt_data_a.Text		= DateTime.Now.Day.ToString() + "/" + DateTime.Now.Month.ToString() + "/" + DateTime.Now.Year.ToString();

				caricaDataSet();				
			}
            this.codAmm = AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "0");
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
			this.btn_cerca.Click += new System.EventHandler(this.btn_cerca_Click);
			this.ddl_oggetto.SelectedIndexChanged += new System.EventHandler(this.ddl_oggetto_SelectedIndexChanged);
            this.eliminaDataA.Click += new ImageClickEventHandler(cancelDataA);
            this.eliminaDataDa.Click += new ImageClickEventHandler(cancelDataDA);
            this.Load += new System.EventHandler(this.Page_Load);
		}
		#endregion

		#region Tasto Ricerca

		/// <summary>
		/// Tasto Ricerca
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_cerca_Click(object sender, System.EventArgs e)
		{			
			string query_data_a	 = null;
			string query_data_da = null;
			string query_user	 = null;
			string query_oggetto = null;
			string query_azione	 = null;
			string query_esito	 = null;
			string codAmm		 = AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"],"0");
			DateTime myDateTime;
            Hashtable filtriLog = new Hashtable();

			pnl_log.Visible = false;

			// controlli sui filtri

			// date --------------------------------------------------------------------------------------------
			// data da:
            this.GetCalendarControl("txt_data_da").txt_Data.Text = this.GetCalendarControl("txt_data_da").txt_Data.Text.Trim();
            this.GetCalendarControl("txt_data_da").txt_Data.Text = this.GetCalendarControl("txt_data_da").txt_Data.Text.TrimEnd();
            this.GetCalendarControl("txt_data_da").txt_Data.Text = this.GetCalendarControl("txt_data_da").txt_Data.Text.TrimStart();

            if (this.GetCalendarControl("txt_data_da").txt_Data.Text != "")
            {
				try
				{
					//Convert.ToDateTime(txt_data_da.Text);
                    myDateTime = DateTime.Parse(this.GetCalendarControl("txt_data_da").txt_Data.Text);
                    lbl_tit.Text = "Filtri di ricerca:";
                    filtriLog.Add("DATA_DA", this.GetCalendarControl("txt_data_da").txt_Data.Text);
                }
				catch
				{
					lbl_tit.Text = "Filtri di ricerca: <b><font color='#ff0000'>ATTENZIONE! campo 'Data da:' errato!</font></b>";
					return;
				}
			}

			// data a: -----------------------------------------------------------------------------------------
            this.GetCalendarControl("txt_data_a").txt_Data.Text = this.GetCalendarControl("txt_data_a").txt_Data.Text.Trim();
            this.GetCalendarControl("txt_data_a").txt_Data.Text = this.GetCalendarControl("txt_data_a").txt_Data.Text.TrimEnd();
            this.GetCalendarControl("txt_data_a").txt_Data.Text = this.GetCalendarControl("txt_data_a").txt_Data.Text.TrimStart();

            if (this.GetCalendarControl("txt_data_a").txt_Data.Text != "")
            {
				try
				{
					//Convert.ToDateTime(txt_data_a.Text);
                    myDateTime = DateTime.Parse(this.GetCalendarControl("txt_data_a").txt_Data.Text);
                    lbl_tit.Text = "Filtri di ricerca:";
                    filtriLog.Add("DATA_A", this.GetCalendarControl("txt_data_a").txt_Data.Text);
                }
				catch
				{
					lbl_tit.Text = "Filtri di ricerca: <b><font color='#ff0000'>ATTENZIONE! campo 'Data a:' errato!</font></b>";
					return;
				}
			}

			// date corrette ed accettabili -------------------------------------------------------------------
            if (this.GetCalendarControl("txt_data_da").txt_Data.Text != "" && this.GetCalendarControl("txt_data_a").txt_Data.Text != "")
            {
                if (Convert.ToDateTime(this.GetCalendarControl("txt_data_da").txt_Data.Text) > Convert.ToDateTime(this.GetCalendarControl("txt_data_a").txt_Data.Text))
                {
					lbl_tit.Text = "Filtri di ricerca: <b><font color='#ff0000'>ATTENZIONE! campi 'Data' non accettabili!</font></b>";
					return;
				}
			}

			// userid ---------------------------------------------------------------------------------------
			txt_user.Text = txt_user.Text.Trim();
			txt_user.Text = txt_user.Text.TrimEnd();
			txt_user.Text = txt_user.Text.TrimStart();
            if (!txt_user.Text.Equals(""))
            {
                filtriLog.Add("USER", txt_user.Text);
            }

			// controllo valori filtri ----------------------------------------------------------------------
            if (this.GetCalendarControl("txt_data_da").txt_Data.Text != "")
            {
                query_data_da = this.GetCalendarControl("txt_data_da").txt_Data.Text;
            }
            if (this.GetCalendarControl("txt_data_a").txt_Data.Text != "")
            {
                query_data_a = this.GetCalendarControl("txt_data_a").txt_Data.Text;
            }
            if (txt_user.Text != "")
			{
				query_user = txt_user.Text;
			}
			if (ddl_oggetto.SelectedItem.Value != "null") 
			{
				query_oggetto	= ddl_oggetto.SelectedItem.Value;
                filtriLog.Add("OGGETTO", query_oggetto);

				if(ddl_azione.SelectedItem.Value != "null")
				{					
					query_azione = ddl_azione.SelectedItem.Value;
                    filtriLog.Add("AZIONE", query_azione);
                }
			}	
			if (ddl_esito.SelectedItem.Value != "null") 
			{
				query_esito		= ddl_esito.SelectedItem.Value;
                filtriLog.Add("ESITO", query_esito);
            }

            if (!ckb_Log.Checked && !ckb_Storico.Checked)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Selezionare almeno una tabella da cui fare le ricerche tra Log e Storico.');", true);
                return;
            }
            int query_table = 0;
            if (ckb_Log.Checked)
                query_table += 1;
            if (ckb_Storico.Checked)
                query_table += 2;
            //MEV CONS 1.3
            //Filtro per nascondere i log conservazione se è abilitata la chiave corrispondente
            if (this.DisableAmmGestCons())
                query_table += 10;

            filtriLog.Add("TABELLE", query_table);
            Session.Add("listaFiltriLog", filtriLog);

            try
			{
				// chiama WS -----------------------------------------------------------------------------------
				AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
				string xmlStream = ws.GetXmlLogFiltrato(query_data_da,query_data_a,query_user,query_oggetto,query_azione,codAmm,query_esito, "Amministrazione",query_table);

				if(xmlStream != null || xmlStream != "")
				{
					XmlDocument doc = new XmlDocument();
					doc.LoadXml(xmlStream);

					DataRow row; 

					XmlNode listaAzioni = doc.SelectSingleNode("NewDataSet");
					if(listaAzioni != null && listaAzioni.ChildNodes.Count > 0)
					{
						IniDataSetQuery();
						foreach (XmlNode azione in listaAzioni.ChildNodes)
						{
							//carica il dataset
							row = dataSet.Tables["QUERY"].NewRow();		

							row["utente"] = azione.ChildNodes[0].InnerText;
							row["data"]		= Convert.ToDateTime(azione.ChildNodes[1].InnerText);
							row["oggetto"]		= azione.ChildNodes[2].InnerText;
							row["descrizione"] = azione.ChildNodes[3].InnerText;
							row["operazione"]	= azione.ChildNodes[4].InnerText;

							if(azione.ChildNodes[5] != null) 
							{
								if(azione.ChildNodes[5].InnerText == "1")
								{
									row["esito"]		= "<font color=#008000>OK</font>";
								}
								else
								{
									row["esito"]		= "<font color=#ff0000><b>KO</b></font>";
								}
							}
							else
								row["esito"]		= "<font color=#0000ff><b>????</b></font>";

							dataSet.Tables["QUERY"].Rows.Add(row);						
						}
						dg_Log.DataSource = dataSet.Tables["QUERY"];
						dg_Log.DataBind();
						pnl_log.Visible = true;
						lbl_tit.Text = "Filtri di ricerca:";
					}
					else
					{
						lbl_tit.Text = "Filtri di ricerca: <b><font color='#ff0000'>Nessun dato trovato!</font></b>";
						pnl_log.Visible = false;
					}
				}
			}
			catch
			{
				lbl_tit.Text = "Filtri di ricerca: <b><font color='#ff0000'>ATTENZIONE! errore nella ricerca!</font></b>";
				pnl_log.Visible = false;
				btn_cerca.Visible = false;
			}
		}
		#endregion

        private void cancelDataA(object sender, System.EventArgs e)
        {
            this.GetCalendarControl("txt_data_a").txt_Data.Text = "";
        }

        private void cancelDataDA(object sender, System.EventArgs e)
        {
            this.GetCalendarControl("txt_data_da").txt_Data.Text = "";
        }

        private SAAdminTool.AdminTool.Getione_Logs.LogCalendar GetCalendarControl(string controlId)
        {
            return (SAAdminTool.AdminTool.Getione_Logs.LogCalendar)this.FindControl(controlId);
        }

		#region combo-box OGGETTO
		/// <summary>
		/// carica Azioni
		/// </summary>
		/// <param name="filtro">filtro</param>
		/// <returns>collection</returns>
		public ICollection caricaAzioni(string filtro)
		{

			DataTable dt = new DataTable();
			DataRow dr;

			dataSet = new DataSet();
			dataSet = (DataSet)ViewState["LOG"];

			dt.Columns.Add(new DataColumn("codice", typeof(string)));
			dt.Columns.Add(new DataColumn("oggetto", typeof(string)));
			dt.Columns.Add(new DataColumn("descrizione", typeof(string)));			

			// aggiunge la prima riga con "*"
			dr = dt.NewRow();
			dr[0] = "null";		// campo codice
			dr[1] = filtro;		// campo oggetto
			dr[2] = "*";		// campo descrizione

			dt.Rows.Add(dr);

			foreach(DataRow riga in dataSet.Tables["LOG"].Rows)
			{				
				dr = dt.NewRow();
				dr[0] = riga[0].ToString(); // campo codice
				dr[1] = riga[1].ToString(); // campo oggetto
				dr[2] = riga[2].ToString(); // campo descrizione

				dt.Rows.Add(dr);
			}

			DataView dv		= dt.DefaultView;
			dv.RowFilter	= "oggetto = '" + filtro + "'";			
			dv.Sort			= "descrizione ASC";

			return dv;
		}

		/// <summary>
		/// carica Combobox Oggetto
		/// </summary>
		public void caricaComboOggetto()
		{		
			if(ViewState["LOG"] != null)
			{
				dataSet = new DataSet();
				dataSet = (DataSet)ViewState["LOG"];

				DataTable dt = IniDataSetAppo(dataSet);

				foreach(DataRow riga in dataSet.Tables["LOG"].Rows)
				{
					DataRow[] righe = dt.Select("oggetto='" + riga[1] + "'");
					if(righe.Length == 0)
					{
						dt.ImportRow(riga);
					}
				}

				DataView dv = dataSet.Tables["APPO"].DefaultView;
				dv.Sort= "oggetto asc";			
			
				foreach(DataRow riga in  dv.Table.Rows)
				{		
					ListItem item = new ListItem(riga[1].ToString(), riga[1].ToString());
                    if (!(this.DisableAmmGestCons() && riga[1].ToString() == "CONSERVAZIONE"))
					    ddl_oggetto.Items.Add(item);				
				}
			}
		}

        //MEV CONS 1.3
        /// <summary>
        /// Funzione per la gestione dell'abilitazione/disabilitazione della visualizzazione
        /// dei log conservazione
        /// </summary>
        /// <returns></returns>
        protected bool DisableAmmGestCons()
        {
            bool result = false;

            string PGU_FE_DISABLE_AMM_GEST_CONS_Value = string.Empty;
            PGU_FE_DISABLE_AMM_GEST_CONS_Value = SAAdminTool.utils.InitConfigurationKeys.GetValue("0", "PGU_FE_DISABLE_AMM_GEST_CONS");
            result = ((PGU_FE_DISABLE_AMM_GEST_CONS_Value.Equals("0") || string.IsNullOrEmpty(PGU_FE_DISABLE_AMM_GEST_CONS_Value)) ? false : true);

            return result;

        }

		/// <summary>
		/// onChange ddl_oggetto
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ddl_oggetto_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (ddl_oggetto.SelectedItem.Value != "null") 
			{
				pnl_log.Visible = false;

				ddl_azione.DataSource = caricaAzioni(ddl_oggetto.SelectedItem.Value);
				ddl_azione.DataTextField	= "descrizione";
				ddl_azione.DataValueField	= "codice";
				ddl_azione.DataBind();
			}
			else
			{
				ddl_azione.Items.Clear();
			}
		}

		#endregion

		#region inizializza dataset

		/// <summary>
		/// Inizializza il dataset
		/// </summary>
		private DataTable IniDataSetAppo(DataSet dataSet)
		{
			dataSet.Tables.Add("APPO");

			DataColumn dc = new DataColumn("codice");
			dataSet.Tables["APPO"].Columns.Add(dc);

			dc = new DataColumn("oggetto");			
			dataSet.Tables["APPO"].Columns.Add(dc);

			dc = new DataColumn("descrizione");			
			dataSet.Tables["APPO"].Columns.Add(dc);		
	
			return dataSet.Tables["APPO"];
	}

		/// <summary>
		/// Inizializza il dataset
		/// </summary>
		private void IniDataSet()
		{
			dataSet = new DataSet();

			dataSet.Tables.Add("LOG");

			DataColumn dc = new DataColumn("codice");
			dataSet.Tables["LOG"].Columns.Add(dc);

			dc = new DataColumn("oggetto");			
			dataSet.Tables["LOG"].Columns.Add(dc);

			dc = new DataColumn("descrizione");			
			dataSet.Tables["LOG"].Columns.Add(dc);			
		}

		/// <summary>
		/// Inizializza il dataset
		/// </summary>
		private void IniDataSetQuery() 
		{
			dataSet = new DataSet();

			dataSet.Tables.Add("QUERY");

			DataColumn dc = new DataColumn("utente");
			dataSet.Tables["QUERY"].Columns.Add(dc);

			dc = new DataColumn("data");			
			dataSet.Tables["QUERY"].Columns.Add(dc);

			dc = new DataColumn("oggetto");			
			dataSet.Tables["QUERY"].Columns.Add(dc);	
		
			dc = new DataColumn("descrizione");			
			dataSet.Tables["QUERY"].Columns.Add(dc);

			dc = new DataColumn("operazione");			
			dataSet.Tables["QUERY"].Columns.Add(dc);

			dc = new DataColumn("esito");			
			dataSet.Tables["QUERY"].Columns.Add(dc);
		}

		/// <summary>
		/// carica DataSet
		/// </summary>
		public void caricaDataSet()
		{
			try
			{
				IniDataSet();

				XmlDocument xmlDoc = new XmlDocument();	
				DataRow row; 

				AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
				string xmlStream = ws.GetXmlLogAmm(AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"],"0"));

				if(xmlStream != null && xmlStream != "")
				{
					XmlDocument doc = new XmlDocument();
					doc.LoadXml(xmlStream);

					//XmlNode listaAzioni = doc.SelectSingleNode("AMMINISTRAZIONE");
					XmlNode listaAzioni = doc.SelectSingleNode("NewDataSet");
					if(listaAzioni.ChildNodes.Count > 0)
					{					
						foreach (XmlNode azione in listaAzioni.ChildNodes)
						{
							//carica il dataset
							row = dataSet.Tables["LOG"].NewRow();	
							row["codice"] = azione.ChildNodes[0].InnerText;
							row["oggetto"] = azione.ChildNodes[2].InnerText;
							row["descrizione"] = azione.ChildNodes[1].InnerText;							
						
							dataSet.Tables["LOG"].Rows.Add(row);						
						}

						ViewState["LOG"] = dataSet;
					}

					caricaComboOggetto();
				}	
				else
				{
					lbl_tit.Text = "ATTENZIONE! nessun record dei log presente in tabella!";
					btn_cerca.Visible = false;
				}
			}
			catch
			{
				lbl_tit.Text = "ATTENZIONE! errore nel caricamento del file XML dei log!";
				btn_cerca.Visible = false;
			}
		}
		#endregion		
	}
}
