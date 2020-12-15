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
	/// Summary description for areaDiLavoroFasc.
	/// </summary>
	public class areaDiLavoroFasc : DocsPAWA.CssPage
	{
		#region WebControls
		protected System.Web.UI.WebControls.Label Label1;
		protected System.Web.UI.WebControls.Label lbl_message;
		protected System.Web.UI.WebControls.Panel pnl_ADL;
		protected System.Web.UI.WebControls.DataGrid DataGrid1;
		protected System.Web.UI.WebControls.Button btn_chiudi;
		protected System.Web.UI.WebControls.ImageButton btn_deleteADL;
		protected DocsPAWA.dataSet.DataSetRFasc dataSetRFasc1;
		private DataTable m_dataTableFascicoli;
		protected System.Web.UI.WebControls.Button btn_elimina;
		protected System.Web.UI.WebControls.Label lbl_message2;
		private Hashtable m_hashTableFascicoli;
		#endregion

		#region Page Load
		private void Page_Load(object sender, System.EventArgs e)
		{
			try
			{
				Utils.startUp(this);
				
				getParametri();
			
				gestioneParametri();

				if (!this.IsPostBack)
					this.AttatchGridPagingWaitControl();
			}
			catch(System.Exception es) 
			{
				ErrorManager.redirect(this, es);
			} 
		}
		#endregion

		#region Parametri
		private void getParametri()
		{			
			try
			{							
				if (!IsPostBack)
				{					
					
					creazioneDataTableFascicoliDaAreaLavoro();	
					setParametriPaginaInSession();
				}	
				else
				{
					getParametriPaginaInSession();
				}
			}
			catch(System.Exception es) 
			{
				ErrorManager.redirect(this, es);
			} 
		}

		private void setParametriPaginaInSession()
		{
			try
			{
				FascicoliManager.setDatagridFascicolo(this,m_dataTableFascicoli);
				FascicoliManager.setHashFascicoli(this,m_hashTableFascicoli);
			}
			catch(System.Exception es) 
			{
				ErrorManager.redirect(this, es);
			} 
		}
		
		private void gestioneParametri()
		{
			try
			{
				if (!IsPostBack)
				{
					bindDataGrid();	
				}
			}
			catch(System.Exception es) 
			{
				ErrorManager.redirect(this, es);
			} 
		}

		private void bindDataGrid()
		{
			if (m_dataTableFascicoli!=null)
			{
				DataGrid1.DataSource = m_dataTableFascicoli.DefaultView;
				DataGrid1.DataBind();
			}	
		}

		private void getParametriPaginaInSession()
		{
			try
			{
				m_dataTableFascicoli=FascicoliManager.getDatagridFascicolo(this);
				m_hashTableFascicoli=FascicoliManager.getHashFascicoli(this);
			}
			catch(System.Exception es) 
			{
				ErrorManager.redirect(this, es);
			} 
		}

		private void creazioneDataTableFascicoliDaAreaLavoro()
		{
			DocsPaWR.Fascicolo[] listaFascicoli=FascicoliManager.getFascicoliInAreaLavoro(this);
			if(listaFascicoli!=null && listaFascicoli.Length>0)
			{				
				m_hashTableFascicoli=new Hashtable(listaFascicoli.Length);
				for(int i=0;i<listaFascicoli.Length;i++)
				{
					DocsPaWR.Fascicolo fasc=listaFascicoli[i];
					DocsPaWR.FascicolazioneClassifica[] gerClassifica=FascicoliManager.getGerarchia(this,fasc.idClassificazione,UserManager.getUtente(this).idAmministrazione);
					string codiceGerarchia=gerClassifica[gerClassifica.Length-1].codice;
					
					m_hashTableFascicoli.Add(i,fasc);
					
					string dataApertura=fasc.apertura;
					string dataChiusura=fasc.chiusura;
                    this.dataSetRFasc1.element1.Addelement1Row(this.getDecodeForStato(fasc.stato), fasc.descrizione, dataApertura, dataChiusura, fasc.tipo, i, codiceGerarchia, fasc.codice, fasc.codLegislatura, fasc.systemID,fasc.contatore, fasc.inConservazione);

				}
				m_dataTableFascicoli=this.dataSetRFasc1.Tables[0];	
				this.btn_elimina.Visible = true;
			} 
			else
			{
				lbl_message.Text="Nessun fascicolo presente in ADL!";
				this.btn_elimina.Visible = false;
				pnl_ADL.Visible=false;
				this.btn_elimina.Visible = false;
				this.lbl_message2.Visible=false;
				this.btn_deleteADL.Visible = false;
			}
		}		

		public string getDecodeForStato(string stato)
		{
			return FascicoliManager.decodeStatoFasc(this,stato);
		}
		#endregion

		#region Datagrid
		private void DataGrid1_PageIndexChanged(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
		{
			this.DataGrid1.SelectedIndex=-1;

			this.DataGrid1.CurrentPageIndex=e.NewPageIndex;
			this.DataGrid1.SelectedIndex=-1;
			FascicoliManager.removeFascicoloSelezionato(this);
			bindDataGrid();	
		}

		private void DataGrid1_SortCommand(object source, System.Web.UI.WebControls.DataGridSortCommandEventArgs e)
		{
			string sortExpression;
			string direction;
			sortExpression=e.SortExpression;
			direction=this.DirectionSorting;
			ChangeDirectionSorting(direction);
			m_dataTableFascicoli.DefaultView.Sort=sortExpression+" "+direction;
			bindDataGrid();
		}

		private void DataGrid1_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			DataGrid grid=(DataGrid)sender;
			DataGridItem row=grid.SelectedItem;
			string keyFasc=((Label)grid.Items[grid.SelectedIndex].Cells[7].Controls[1]).Text;
			int i=Int32.Parse(keyFasc);

//			string keyFasc = ((Label) this.DataGrid1.SelectedItem.Cells[7].Controls[1]).Text;
//			int i = Int32.Parse(keyFasc);
			DocsPaWR.Fascicolo fascicolo=(DocsPAWA.DocsPaWR.Fascicolo)m_hashTableFascicoli[i];
			FascicoliManager.setFascicoloSelezionato(this,fascicolo);


		}

		private void ChangeDirectionSorting(string oldDirection)
		{
			string newValue;
			if (oldDirection!=null && oldDirection.Equals("ASC"))
			{
				newValue="DESC";
			}
			else
			{
				newValue="ASC";
			}
			DirectionSorting=newValue;
		}

		private string DirectionSorting
		{
			get
			{
				string retValue;
				if (ViewState["directionSorting"]==null)
				{
					ViewState["directionSorting"]="ASC";
				}

				retValue=(string)ViewState["directionSorting"];
				return retValue;
			}
			set
			{
				ViewState["directionSorting"]=value;
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
			this.dataSetRFasc1 = new DocsPAWA.dataSet.DataSetRFasc();
			((System.ComponentModel.ISupportInitialize)(this.dataSetRFasc1)).BeginInit();
			// 
			// dataSetRFasc1
			// 
			this.dataSetRFasc1.DataSetName = "DataSetRFasc";
			this.dataSetRFasc1.Locale = new System.Globalization.CultureInfo("en-US");
			this.btn_deleteADL.Click += new System.Web.UI.ImageClickEventHandler(this.btn_deleteADL_Click);
			this.DataGrid1.ItemCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.DataGrid1_ItemCommand);
			this.DataGrid1.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.DataGrid1_PageIndexChanged);
			this.DataGrid1.SortCommand += new System.Web.UI.WebControls.DataGridSortCommandEventHandler(this.DataGrid1_SortCommand);
			this.DataGrid1.SelectedIndexChanged += new System.EventHandler(this.DataGrid1_SelectedIndexChanged);
			this.btn_elimina.Click += new System.EventHandler(this.btn_elimina_Click);
			this.btn_chiudi.Click += new System.EventHandler(this.btn_chiudi_Click);
			this.Load += new System.EventHandler(this.Page_Load);
			((System.ComponentModel.ISupportInitialize)(this.dataSetRFasc1)).EndInit();

		}
		#endregion

		#region Tasti
		private void btn_chiudi_Click(object sender, System.EventArgs e)
		{
			Response.Write("<script>window.close();</script>");	
		}
			
		private void btn_deleteADL_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
			for(int i=0;i<m_hashTableFascicoli.Count;i++)
			{
				DocumentManager.eliminaDaAreaLavoro(this, null, (DocsPAWA.DocsPaWR.Fascicolo)m_hashTableFascicoli[i]);
			}
			m_hashTableFascicoli.Clear();

			m_dataTableFascicoli.Clear();

			FascicoliManager.removeDatagridFascicolo(this);

			FascicoliManager.removeHashFascicoli(this);

			DataGrid1.Visible = false;
			lbl_message.Text="Nessun fascicolo presente in ADL!";
			this.btn_elimina.Visible = false;
			pnl_ADL.Visible=false;
			this.btn_elimina.Visible = false;
			this.lbl_message2.Visible=false;
			this.btn_deleteADL.Visible = false;

		}

		#endregion

		//		private void DataGrid1_PreRender(object sender, System.EventArgs e)
		//		{
		//			
		//			DataGrid dg=((DataGrid) sender);
		//			for(int i=0;i<dg.Items.Count;i++)
		//			{
		//				if(dg.Items[i].ItemIndex>=0)
		//					{	
		//						switch(dg.Items[i].ItemType.ToString().Trim())
		//							{
		//								case "Item":
		//								{
		//									//dg.Items[i].Attributes.Add("onmouseover","this.className='bg_grigioS'");
		//									//dg.Items[i].Attributes.Add("onmouseout","this.style.color='#d9d9d9'");
		//									
		//									break;
		//								}
		//								case "AlternatingItem":
		//								
		//								{
		//									//dg.Items[i].Attributes.Add("onmouseover","this.className='bg_grigioS'");
		//									//dg.Items[i].Attributes.Add("onmouseout","this.style.color='#f2f2f2'");
		//									break;
		//								}
		//									
		//							}
		//					 }
		//							
		//			}
		//		 }
	
	
		private void DataGrid1_ItemCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			if(e.CommandName == "Update")
			{
				DataGrid grid =(DataGrid)source;
				DataGridItem row = e.Item;
				if(row.ItemIndex>=0)
				{
					string keyFasc=((Label)grid.Items[row.ItemIndex].Cells[7].Controls[1]).Text;
					int i=Int32.Parse(keyFasc);
			
					DocsPaWR.Fascicolo fascicolo=(DocsPAWA.DocsPaWR.Fascicolo)m_hashTableFascicoli[i];
					//luluciani 17/11/2006 cambiata gestione pulizia session Back, non si fa più da testata
					Utils.CleanSessionMemoria(this);

					FascicoliManager.setFascicoloSelezionato(this,fascicolo);
					
					string newUrl="../fascicolo/gestioneFasc.aspx?tab=documenti";
					Response.Write("<script language='javascript'>window.open('"+newUrl+"','principale');</script>");
					Response.Write("<script>window.close();</script>");
				}
			}

		}

		private void btn_elimina_Click(object sender, System.EventArgs e)
		{
			string str_indexSel = "";
			if(this.DataGrid1.Items.Count == 0)
			{
				return;
			}
			if(this.DataGrid1.SelectedIndex >=0)
			{
				try
				{
					str_indexSel = ((Label) this.DataGrid1.SelectedItem.Cells[7].Controls[1]).Text;
				}
				catch(Exception)
				{
					this.DataGrid1.SelectedIndex=-1;
					return ;
				}
				int indexSel = Int32.Parse(str_indexSel);
				if (indexSel < 0) 
					return;
				DocsPaWR.Fascicolo fascicolo=(DocsPAWA.DocsPaWR.Fascicolo)m_hashTableFascicoli[indexSel];

				//elimina il doc (per ora gestiamo solo i doc)
				DocumentManager.eliminaDaAreaLavoro(this, null, fascicolo);
				//aggiorno il dataGrid
				m_hashTableFascicoli.Remove(indexSel);
				DataRow[] dr =  m_dataTableFascicoli.DefaultView.Table.Select("Chiave = " + indexSel);
				

				m_dataTableFascicoli.DefaultView.Table.Rows.Remove(dr[0]);
				if(m_dataTableFascicoli.DefaultView.Count != 0)
				{
					if(this.DataGrid1.Items.Count == 1 && DataGrid1.CurrentPageIndex > 0 )
					{
						DataGrid1.CurrentPageIndex = DataGrid1.CurrentPageIndex - 1;
					}
					this.bindDataGrid();
				}
				else
				{
					creazioneDataTableFascicoliDaAreaLavoro();
					DataGrid1.Visible = false;
				}
				this.DataGrid1.SelectedIndex=-1;
			}
			else
			{
				Response.Write("<script>alert('Attenzione: selezionare un fascicolo!');</script>");	
			}
		}

		/// <summary>
		/// 
		/// </summary>
		private void AttatchGridPagingWaitControl()
		{
			DataGridPagingWaitControl.DataGridID=this.DataGrid1.ClientID;
			DataGridPagingWaitControl.WaitScriptCallback="WaitDataGridCallback();";
		}

		/// <summary>
		/// 
		/// </summary>
		private waiting.DataGridPagingWait DataGridPagingWaitControl
		{
			get
			{
				return this.FindControl("DataGridPagingWait1") as waiting.DataGridPagingWait;
			}
		}

        protected void DataGrid1_ItemCreated(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Pager)
            {

                if (e.Item.Cells.Count > 0)
                {

                    e.Item.Cells[0].Attributes.Add("colspan", e.Item.Cells[0].ColumnSpan.ToString());

                }

            }


        }
	}
}
