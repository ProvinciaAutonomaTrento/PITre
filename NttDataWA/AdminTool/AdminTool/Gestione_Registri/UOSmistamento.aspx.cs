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

namespace SAAdminTool.AdminTool.Gestione_Registri
{
	/// <summary>
	/// Summary description for UOSmistamento.
	/// </summary>
	public class UOSmistamento : System.Web.UI.Page
	{
		protected System.Web.UI.WebControls.Label lbl_tit;
		protected System.Web.UI.WebControls.Button btn_modifica;
        protected System.Web.UI.WebControls.Button btn_selDeselTutti;
		//protected System.Web.UI.WebControls.CheckBox chk_all;
		protected System.Web.UI.WebControls.DataGrid dg_AbilitaUO;
		protected System.Web.UI.HtmlControls.HtmlInputHidden hd_idRegistro;
		protected System.Web.UI.WebControls.RadioButton rb_attivi;
		protected System.Web.UI.WebControls.RadioButton rb_nonAttivi;
		protected System.Web.UI.WebControls.RadioButton rb_tutti;
		protected DataSet dataSet;
	
		private void Page_Load(object sender, System.EventArgs e)
		{
			Response.Expires = -1;
			Response.Buffer = false;

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
				this.hd_idRegistro.Value = Request.QueryString["idRegistro"].ToString();				

				//this.chk_all.Attributes.Add("onclick","CheckAllDataGridCheckBoxes('Chk',document.forms[0].chk_all.checked)");
                this.btn_selDeselTutti_Click(null,null);
				this.LoadDataGrid();				
			} 
		}

		private void LoadDataGrid()
		{			
			XmlDocument xmlDoc = new XmlDocument();	
			DataRow row; 

			try
			{
				AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
				string xmlStream = ws.GetXMLUOSmistamento(this.hd_idRegistro.Value);

				if(xmlStream != null && xmlStream != "")
				{
					XmlDocument doc = new XmlDocument();
					doc.LoadXml(xmlStream);
					
					XmlNode lista = doc.SelectSingleNode("NewDataSet");
					if(lista.ChildNodes.Count > 0)
					{
						IniDataSet();
						foreach (XmlNode nodo in lista.ChildNodes)
						{
							//carica il dataset
							row = dataSet.Tables[0].NewRow();	
							row["idCorrGlob"] = nodo.ChildNodes[0].InnerText;							
							row["livello"] = nodo.ChildNodes[1].InnerText;	
							row["descrizione"] = nodo.ChildNodes[2].InnerText.ToUpper();
							if(nodo.ChildNodes[3].InnerText == "1")
							{
								row["attivo"] = "true";
							}
							else
							{
								row["attivo"] = "false";
							}	

							// verifica cosa visualizzare rispetto alla selezione RadioButton
							if(this.rb_tutti.Checked) // tutti
								dataSet.Tables["UOSmista"].Rows.Add(row);	
							else if(this.rb_attivi.Checked)	// attivi
							{
								if(nodo.ChildNodes[3].InnerText == "1")
									dataSet.Tables["UOSmista"].Rows.Add(row);
							}
							else
							{	//non attivi
								if(nodo.ChildNodes[3].InnerText == "0")
									dataSet.Tables["UOSmista"].Rows.Add(row);
							}													
						}

						DataView dv = dataSet.Tables["UOSmista"].DefaultView;
						dv.Sort = "livello ASC, descrizione ASC";
						this.dg_AbilitaUO.DataSource = dv;
						this.dg_AbilitaUO.DataBind();
					}
					else 
					{			
						this.lbl_tit.Text = "Nessuna unità organizzativa associata a questo registro!";
						this.btn_modifica.Visible = false;
					}
				}	
				else
				{
					this.lbl_tit.Text = "Nessuna unità organizzativa associata a questo registro!";
					this.btn_modifica.Visible = false;
				}
			}
			catch
			{
				this.lbl_tit.Text = "ATTENZIONE! si è verificato un errore nel reperimento dati";
				this.btn_modifica.Visible = false;
			}
		}

		/// <summary>
		/// Inizializza il dataset
		/// </summary>
		private void IniDataSet()
		{
			dataSet = new DataSet();

			dataSet.Tables.Add("UOSmista");

			DataColumn dc = new DataColumn("idCorrGlob");
			dataSet.Tables["UOSmista"].Columns.Add(dc);

			dc = new DataColumn("livello");			
			dataSet.Tables["UOSmista"].Columns.Add(dc);

			dc = new DataColumn("descrizione");			
			dataSet.Tables["UOSmista"].Columns.Add(dc);

			dc = new DataColumn("attivo");			
			dataSet.Tables["UOSmista"].Columns.Add(dc);
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
			this.btn_modifica.Click += new System.EventHandler(this.btn_modifica_Click);
			this.btn_selDeselTutti.Click += new System.EventHandler(this.btn_selDeselTutti_Click); 
            this.rb_attivi.CheckedChanged += new System.EventHandler(this.rb_attivi_CheckedChanged);
			this.rb_nonAttivi.CheckedChanged += new System.EventHandler(this.rb_nonAttivi_CheckedChanged);
			this.rb_tutti.CheckedChanged += new System.EventHandler(this.rb_tutti_CheckedChanged);
			this.Load += new System.EventHandler(this.Page_Load);
        }
		#endregion

        private void btn_selDeselTutti_Click(object sender, System.EventArgs e)
        {
            if (btn_selDeselTutti.Text.Equals("Seleziona tutti"))
            {
                foreach (DataGridItem item in this.dg_AbilitaUO.Items)
                {
                    CheckBox checkBox = this.GetCheckBoxAssociazioneUO(item);

                    if (checkBox != null)
                        checkBox.Checked = true;
                }
                btn_selDeselTutti.Text = "Deseleziona tutti";
            }
            else
            {
                foreach (DataGridItem item in this.dg_AbilitaUO.Items)
                {
                    CheckBox checkBox = this.GetCheckBoxAssociazioneUO(item);

                    if (checkBox != null)
                        checkBox.Checked = false;
                }
                btn_selDeselTutti.Text = "Seleziona tutti";
            }

        }
       
        private CheckBox GetCheckBoxAssociazioneUO(DataGridItem item)
        {
            return item.Cells[3].FindControl("Chk") as CheckBox;
        }


		private void btn_modifica_Click(object sender, System.EventArgs e)
		{
			DataRow row;
			CheckBox cb;			

			this.IniDataSet();

			try
			{
				for(int i=0; i<this.dg_AbilitaUO.Items.Count; i++)
				{
					//carica il dataset
					row = dataSet.Tables["UOSmista"].NewRow();	
					row["idCorrGlob"] = this.dg_AbilitaUO.Items[i].Cells[0].Text;
					row["livello"] = this.dg_AbilitaUO.Items[i].Cells[1].Text;
					row["descrizione"] = this.dg_AbilitaUO.Items[i].Cells[2].Text;												
				
					cb = (CheckBox) this.dg_AbilitaUO.Items[i].Cells[3].FindControl("Chk");	

					if(cb.Checked)
					{
						row["attivo"] = "1";
					}
					else
					{
						row["attivo"] = "0";
					}
					

					dataSet.Tables["UOSmista"].Rows.Add(row);		
				}

				if(dataSet.Tables["UOSmista"].Rows.Count > 0)
				{
					string streamXml = dataSet.GetXml().ToUpper();

					// stream verso il WS
					AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
					if(!ws.SetXMLUOSmistamento(streamXml,this.hd_idRegistro.Value))															
					{
						this.lbl_tit.Text = "ATTENZIONE! si è verificato un errore nella modifica dei dati";
						this.btn_modifica.Visible = false;
					}
					else
					{
						this.lbl_tit.Text = "Stato: <b>Effettuata modifica</b>";
						this.LoadDataGrid();
					}
				}
			}
			catch
			{
				this.lbl_tit.Text = "ATTENZIONE! si è verificato un errore nella modifica dei dati";
				this.btn_modifica.Visible = false;
			}
		}

		private void rb_nonAttivi_CheckedChanged(object sender, System.EventArgs e)
		{
			this.lbl_tit.Text="";
            btn_selDeselTutti.Text = "Seleziona tutti";
			this.LoadDataGrid();
		}

		private void rb_attivi_CheckedChanged(object sender, System.EventArgs e)
		{
			this.lbl_tit.Text="";
            btn_selDeselTutti.Text = "Deseleziona tutti";
			this.LoadDataGrid();
		}

		private void rb_tutti_CheckedChanged(object sender, System.EventArgs e)
		{
			this.lbl_tit.Text="";
            btn_selDeselTutti.Text = "Seleziona tutti";
			this.LoadDataGrid();
		}
	}
}
