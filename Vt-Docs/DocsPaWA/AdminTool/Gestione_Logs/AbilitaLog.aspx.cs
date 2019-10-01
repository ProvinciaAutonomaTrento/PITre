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
	public class AbilitaLog : System.Web.UI.Page
	{
		#region WebControls e variabili
		protected System.Web.UI.WebControls.DataGrid dg_AbilitaLog;		
		protected System.Web.UI.WebControls.Label lbl_position;
		protected System.Web.UI.WebControls.Label lbl_tit;
		protected System.Web.UI.WebControls.Button btn_modifica;
		protected System.Web.UI.WebControls.CheckBox chk_all;
		protected DataSet dataSet;
        private string idAmm;
		#endregion

		#region Page_Load

		/// <summary>
		/// Page_Load
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
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

            idAmm = AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "3");
			if (!IsPostBack)
			{				
				lbl_position.Text = "&nbsp;&bull;&nbsp;Amministrazione: " + AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"],"1");
				lbl_tit.Text = "Attivazione Log";

				chk_all.Attributes.Add("onclick","CheckAllDataGridCheckBoxes('Chk',document.forms[0].chk_all.checked)");
				
				LoadDataGrid();
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
			this.btn_modifica.Click += new System.EventHandler(this.btn_modifica_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		#region datagrid

		/// <summary>
		/// LoadDataGrid
		/// </summary>
		public void LoadDataGrid()
		{			
			XmlDocument xmlDoc = new XmlDocument();	
			DataRow row; 

			try
			{
				AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
				string xmlStream = ws.GetXmlLog(AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"],"0"));

				if(xmlStream != null && xmlStream != "")
				{
					XmlDocument doc = new XmlDocument();
					doc.LoadXml(xmlStream);

					//XmlNode listaAzioni = doc.SelectSingleNode("AMMINISTRAZIONE");
					XmlNode listaAzioni = doc.SelectSingleNode("NewDataSet");
					if(listaAzioni.ChildNodes.Count > 0)
					{
						IniDataSet();
						foreach (XmlNode azione in listaAzioni.ChildNodes)
						{


                                //carica il dataset
                                row = dataSet.Tables[0].NewRow();
                                row["descrizione"] = azione.ChildNodes[1].InnerText;
                                row["oggetto"] = azione.ChildNodes[2].InnerText;
                                if (azione.ChildNodes[4].InnerText == "1")
                                {
                                    row["attivo"] = "true";
                                }
                                else
                                {
                                    row["attivo"] = "false";
                                }
                                row["codice"] = azione.ChildNodes[0].InnerText;

                                //campi per gestione notifica evento
                                row["notify"] = azione.ChildNodes[5].InnerText;
                                row["notification"] = azione.ChildNodes[6].InnerText;
                                row["configurable"] = azione.ChildNodes[7].InnerText;
                                dataSet.Tables["AZIONI"].Rows.Add(row);
                                
						}

						DataView dv = dataSet.Tables["AZIONI"].DefaultView;
						dv.Sort = "oggetto ASC, descrizione ASC";
						dg_AbilitaLog.DataSource = dv;
						dg_AbilitaLog.DataBind();
                        

                        //MEV CONS 1.3
                        //se la chiave PGU_FE_DISABLE_AMM_GEST_CONS è abilitata
                        //i log conservazione sono nascosti
                        // INTEGRAZIONE PITRE-PARER
                        // I log conservazione sono nascosti se è attiva la conservazione PARER
                        for (int i = 0; i < dg_AbilitaLog.Items.Count; i++)
                        {
                            if (((this.DisableAmmGestCons() || this.IsConservazionePARER())&& (dg_AbilitaLog.Items[i].Cells[0].Text == "CONSERVAZIONE" || dg_AbilitaLog.Items[i].Cells[0].Text == "AREA_CONSERVAZIONE")))
                                dg_AbilitaLog.Items[i].Visible = false;

                            // INTEGRAZIONE PITRE-PARER
                            // filtro aggiuntivo per i log di invio in conservazione non associati agli oggetti CONSERVAZIONE ed AREA CONSERVAZIONE
                            if (this.IsConservazionePARER() && (
                                dg_AbilitaLog.Items[i].Cells[1].Text.Equals("Invio in conservazione del documento") ||
                                dg_AbilitaLog.Items[i].Cells[1].Text.Equals("Documento inserito in conservazione") ||
                                dg_AbilitaLog.Items[i].Cells[1].Text.Equals("Fascicolo inserito in conservazione")
                                ))
                                dg_AbilitaLog.Items[i].Visible = false;
                        }
                        
					}
					else 
					{
						IniDataSet();
						btn_modifica.Visible = false;
					}
				}	
				else
				{
					lbl_tit.Text = "ATTENZIONE! il file XML dei log è vuoto!";
					btn_modifica.Visible = false;
				}
			}
			catch
			{
				lbl_tit.Text = "ATTENZIONE! errore nel caricamento del file XML dei log!";
				btn_modifica.Visible = false;
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
            PGU_FE_DISABLE_AMM_GEST_CONS_Value = DocsPAWA.utils.InitConfigurationKeys.GetValue("0", "PGU_FE_DISABLE_AMM_GEST_CONS");
            result = ((PGU_FE_DISABLE_AMM_GEST_CONS_Value.Equals("0") || string.IsNullOrEmpty(PGU_FE_DISABLE_AMM_GEST_CONS_Value)) ? false : true);

            return result;

        }

        /// <summary>
        /// INTEGRAZIONE PITRE-PARER
        /// Determina se è attiva la conservazione PARER
        /// </summary>
        /// <returns></returns>
        protected bool IsConservazionePARER()
        {
            bool result = false;

            string IS_CONSERVAZIONE_PARER = string.Empty;
            IS_CONSERVAZIONE_PARER = DocsPAWA.utils.InitConfigurationKeys.GetValue("0", "FE_WA_CONSERVAZIONE");
            result = ((string.IsNullOrEmpty(IS_CONSERVAZIONE_PARER) || IS_CONSERVAZIONE_PARER.Equals("0")) ? false : true);

            return result;
        }

		/// <summary>
		/// Inizializza il dataset
		/// </summary>
		private void IniDataSet()
		{
			dataSet = new DataSet();

			dataSet.Tables.Add("AZIONI");

			DataColumn dc = new DataColumn("descrizione");
			dataSet.Tables["AZIONI"].Columns.Add(dc);

			dc = new DataColumn("oggetto");			
			dataSet.Tables["AZIONI"].Columns.Add(dc);

			dc = new DataColumn("attivo");			
			dataSet.Tables["AZIONI"].Columns.Add(dc);

			dc = new DataColumn("codice");			
			dataSet.Tables["AZIONI"].Columns.Add(dc);

            dc = new DataColumn("notify");
            dataSet.Tables["AZIONI"].Columns.Add(dc);

            dc = new DataColumn("notification");
            dataSet.Tables["AZIONI"].Columns.Add(dc);

            dc = new DataColumn("configurable");
            dataSet.Tables["AZIONI"].Columns.Add(dc);
		}

		/// <summary>
		/// ordina il datagrid
		/// </summary>
		/// <param name="dv"></param>
		/// <param name="sortColumn"></param>
		/// <returns></returns>
		private DataView OrdinaGrid(DataView dv, string sortColumn)
		{			
			dv.Sort = sortColumn + " ASC";			
			return dv;
		}

        protected void dg_AbilitaLog_DataBound(Object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                DropDownList ddl = (e.Item.Cells[3].FindControl("ddlNotify") as DropDownList);
                if (ddl != null)
                {
                    ddl.Items.Add(new ListItem() { Text = "Obbligatoria", Value = "OBB" });
                    ddl.Items.Add(new ListItem() { Text = "Configurabile", Value = "CON" });
                    ddl.Items.Add(new ListItem() { Text = "Non notificabile", Value = "NN" });
                    
                    string notify = (e.Item.Cells[3].FindControl("hdnNotify") as HiddenField).Value;
                    string notification = (e.Item.Cells[3].FindControl("hdnNotification") as HiddenField).Value;
                    string configurable = (e.Item.Cells[3].FindControl("hdnConfigurable") as HiddenField).Value;
                    bool active = (e.Item.Cells[2].FindControl("Chk") as CheckBox).Checked;
                    if (notification.Equals("0")) // evento non notificabile
                    {
                        ddl.SelectedIndex = 2;
                        ddl.Enabled = false;
                    }
                    else if (active) //evento notificabile ed attivo per l'amministrazione corrente
                    {
                        for(int i = 0; i < ddl.Items.Count;i++)
                        {
                            if (ddl.Items[i].Value.Equals(notify))
                            {
                                ddl.Items[i].Selected = true;
                                if(configurable.Equals("0")) // l'evento è obbligatorio in quanto non configurabile
                                ddl.Enabled = false;
                                break;
                            }
                        }
                    }
                    else if(configurable.Equals("0")) // evento notificabile ma non configurabile e non attivo per l'amm corrente.
                    {
                        ddl.SelectedIndex = 0;
                        ddl.Enabled = false;
                    }
                }
            }
        }

	#endregion

		#region modifica

		/// <summary>
		/// Tasto modifica
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_modifica_Click(object sender, System.EventArgs e)
		{			
			DataRow row;
			CheckBox cb;			

			IniDataSet();

			try
			{
				for(int i=0; i<this.dg_AbilitaLog.Items.Count; i++)
				{
					//carica il dataset
					row = dataSet.Tables[0].NewRow();		
					row["descrizione"] = dg_AbilitaLog.Items[i].Cells[1].Text;							
					row["oggetto"] = dg_AbilitaLog.Items[i].Cells[0].Text;	
				
					cb = (CheckBox) dg_AbilitaLog.Items[i].Cells[2].FindControl("Chk");	

					if(cb.Checked)
					{
						row["attivo"] = "1";
					}
					else
					{
						row["attivo"] = "0";
					}
					row["codice"] = dg_AbilitaLog.Items[i].Cells[4].Text;

                    row["notify"] = (dg_AbilitaLog.Items[i].Cells[3].FindControl("ddlNotify") as DropDownList).SelectedValue;
					dataSet.Tables["AZIONI"].Rows.Add(row);		
				}

				if(dataSet.Tables["AZIONI"].Rows.Count > 0)
				{
					string streamXml = dataSet.GetXml().ToUpper();

					// stream verso il WS
					AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
					if(!ws.SetXmlLog(streamXml, AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"],"0")))
					{
						lbl_tit.Text = "Attivazione Log - <b><font color='#ff0000'>ATTENZIONE! errore nella modifica!</font></b>";
						btn_modifica.Visible = false;
					}
					else
					{
						lbl_tit.Text = "Attivazione Log - Stato: <b>Modificato</b>";
						LoadDataGrid();
					}
				}
			}
			catch
			{
				lbl_tit.Text = "Attivazione Log - <b><font color='#ff0000'>ATTENZIONE! errore nella modifica!</font></b>";
				btn_modifica.Visible = false;
			}
		}
		#endregion
	
	}
}
