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
	/// Summary description for areaDiLavoro.
	/// </summary>
	public class areaDiLavoro : DocsPAWA.CssPage
	{
		/// <summary>
		/// </summary>
		public class Cols 
		{		
			private string data;
			private string segnatura;
			private string oggetto;
			private string tipo;
			private int chiave;
			private string dataAnnullamento;

			/// <summary>
			/// </summary>
			/// <param name="data"></param>
			/// <param name="segnatura"></param>
			/// <param name="oggetto"></param>
			/// <param name="tipo"></param>
			/// <param name="chiave"></param>
			public Cols(string data, string segnatura, string oggetto, string tipo, int chiave, string dataAnnull)
			{
				this.data = data;
				this.segnatura = segnatura;
				this.oggetto = oggetto;
				this.tipo = tipo;
				this.chiave = chiave;
				this.dataAnnullamento = dataAnnull;
			}
				
			/// <summary>
			/// </summary>
			public string Data{get{return data;}}
			public string Segnatura{get{return segnatura;}}
			public string Oggetto{get{return oggetto;}}
			public string Tipo{get{return tipo;}}
			public int    Chiave{get{return chiave;}}		
			public string    DataAnnullamento{get{return dataAnnullamento;}}		
		}

		protected System.Web.UI.WebControls.Button btn_chiudi;
		protected System.Web.UI.WebControls.Button btn_ok;
		protected System.Web.UI.WebControls.Label lbl_message;
		protected System.Web.UI.WebControls.Label Label1;
		protected DocsPAWA.DocsPaWR.AreaLavoro areaLav;
		protected ArrayList Dt_elem;
		protected System.Web.UI.WebControls.Button btn_elimina;
		protected System.Web.UI.WebControls.ImageButton btn_deleteAllADL;
		protected System.Web.UI.WebControls.Panel pnl_ADL;
		protected System.Web.UI.WebControls.DataGrid DataGrid1;
		string action;
		protected static int currentPage ;
		protected System.Web.UI.WebControls.Label pageNumberLabel;
		protected System.Web.UI.WebControls.TextBox txtIdRegistro;
		protected int numTotPage ;

		/// <summary>
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Page_Load(object sender, System.EventArgs e)
		{
			
			getParametersFromQueryString();
			
			// Chiama il metodo che crea la lista degli oggetti presenti in AreaLavoro e disegna il risultato
			if (!Page.IsPostBack)
			{
				this.AttatchGridPagingWaitControl();

				//controllo se visualizzare o nascondere il tasto 'Elimina', 'Ok', etc
				currentPage = 1 ;
				//this.pageNumberLabel.Text = currentPage.ToString();

				if (action!=null && action.Equals("gestAreaLav"))
				{
					this.btn_elimina.Visible = true;
					this.btn_ok.Visible = false;
				}
				else
				{
					this.btn_elimina.Visible = false;
					this.btn_ok.Visible = true;
				}

				// Aggiorna i dati
				this.LoadData(true);

				#region Metodo Commentato
				/*
				areaLav = DocumentManager.getListaAreaLavoro(this, tipoDoc, currentPage, out numTotPage, out nRec);

				if (areaLav != null)
				{
					DataGrid1.VirtualItemCount = nRec;
					areaLav.TotalRecs = nRec;

					Session["areaDiLavoro.listaAreaLav"] = areaLav;

					this.BindGrid(areaLav);
				}
				*/
				#endregion

				if (areaLav == null || areaLav.lista == null || areaLav.lista.Length <= 0)
				{
					this.lbl_message.Text = "Documenti non trovati";
					this.pnl_ADL.Visible=false;
					this.lbl_message.Visible = true;
					this.btn_ok.Visible = false;
					this.btn_elimina.Visible = false;
				}
                //gestione visualizzazione unificata
                if ((System.Configuration.ConfigurationManager.AppSettings["GRD_VIS_UNIFICATA"] != null) && (System.Configuration.ConfigurationManager.AppSettings["GRD_VIS_UNIFICATA"] == "0"))
                {
                    this.DataGrid1.Columns[8].Visible = false;
                }
               
			}
		}

		#region datagrid
		/// <summary>
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Datagrid1_PreRender(object sender, System.EventArgs e)
		{
//			try
//			{
//				for(int i=0;i<this.DataGrid1.Items.Count;i++)
//				{
//					if(this.DataGrid1.Items[i].ItemIndex>=0)
//					{	
//						switch(this.DataGrid1.Items[i].ItemType.ToString().Trim())
//						{
//							case "Item":
//							{
//								this.DataGrid1.Items[i].Attributes.Add("onmouseover","this.className='bg_grigioS'");
//								this.DataGrid1.Items[i].Attributes.Add("onmouseout","this.className='bg_grigioN'");
//								break;
//							}
//							case "AlternatingItem":
//					
//							{
//								this.DataGrid1.Items[i].Attributes.Add("onmouseover","this.className='bg_grigioS'");
//								this.DataGrid1.Items[i].Attributes.Add("onmouseout","this.className='bg_grigioA'");
//								break;
//							}
//				
//						}
//					}
//				}
//			}
//			catch(Exception ex)
//			{
//				ErrorManager.redirectToErrorPage(this,ex);
//			}
		}

		/// <summary>
		/// </summary>
		/// <param name="areaLav"></param>
		public void BindGrid(DocsPAWA.DocsPaWR.AreaLavoro areaLav, int currentPage)
		{
			DocsPaWR.InfoDocumento curDoc;

			if (areaLav != null && areaLav.lista!=null && areaLav.lista.Length > 0 )
			{
				//Costruisco il datagrid
				Dt_elem = new ArrayList();

				for(int i= 0; i< areaLav.lista.Length ; i++)
				{					
					curDoc = ((DocsPAWA.DocsPaWR.InfoDocumento) areaLav.lista[i]);
					string dataApertura = "";

					if (curDoc.dataApertura != null && curDoc.dataApertura .Length > 0)
					{
						dataApertura = curDoc.dataApertura.Substring(0,10);
					}
					string segnId =curDoc.segnatura;
					if((curDoc.tipoProto!=null 
						&& curDoc.tipoProto.Equals("G")) //grigio
                        || //oppure
                            (!(curDoc.numProt!=null && curDoc.numProt!="") 
                            && curDoc.tipoProto!="G")) //predisposto
						segnId=curDoc.docNumber;


					Dt_elem.Add(new Cols(dataApertura, segnId, curDoc.oggetto, curDoc.tipoProto, i, curDoc.dataAnnullamento));
				}
				
				DocumentManager.setDataGridAreaLavoro(this,Dt_elem);

				#region Metodo Commentato
				//controllo perchè quando si eliminano i documenti va in errore
				//				double indx = (areaLav.lista.Length % 10);
				//				
				//				if(indx * 10 < areaLav.lista.Length)
				//				{
				//					this.DataGrid1.CurrentPageIndex = (areaLav.TotalRecs / 10) - 1;
				//				}
				#endregion
				
				this.DataGrid1.DataSource=Dt_elem;
				this.DataGrid1.CurrentPageIndex = currentPage-1;
				this.DataGrid1.DataBind();

                for (int i = 0; i < areaLav.lista.Length; i++)
                {
                    curDoc = ((DocsPAWA.DocsPaWR.InfoDocumento)areaLav.lista[i]);

                    if (curDoc.acquisitaImmagine != null && curDoc.acquisitaImmagine == "0")
                    {
                        ImageButton imgbtn = new ImageButton();
                        if (this.DataGrid1.Items[i].Cells[8].Controls[1].GetType().Equals(typeof(ImageButton)))
                        {
                            imgbtn = (ImageButton)this.DataGrid1.Items[i].Cells[8].Controls[1];
                            imgbtn.Visible = false;
                        }                       
                    }
                }
			}
			else
			{
		
				//se ho una pagina sola alloara significa che i doc in ADL sono finiti
				this.lbl_message.Text = "Documenti non trovati";
				this.pnl_ADL.Visible=false;
				this.DataGrid1.Visible = false;
				this.lbl_message.Visible = true;
				this.btn_ok.Visible = false;
				this.btn_elimina.Visible = false;


				DocumentManager.removeDataGridAreaLavoro(this);
                
			}		
		}

		/// <summary>
		/// </summary>
		/// <param name="source"></param>
		/// <param name="e"></param>
		private void DataGrid1_PageChange(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
		{
			this.DataGrid1.SelectedIndex=-1;

			this.DataGrid1.CurrentPageIndex=e.NewPageIndex;
			ArrayList ColNew=new ArrayList();
			ColNew=(ArrayList) DocumentManager.getDataGridAreaLavoro(this); 
			
			DataGrid1.DataSource=ColNew; 					
			DataGrid1.DataBind();
		}

		/// <summary>
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void DataGrid1_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			//			((ImageButton) this.DataGrid1.SelectedItem.Cells[4].Controls[1]).ImageUrl = "../images/proto/select_y.gif";
			
			//setto il file allegato da poter visualizzare
			string str_indexSel = ((Label) this.DataGrid1.SelectedItem.Cells[5].Controls[1]).Text;
			int indexSel = Int32.Parse(str_indexSel);
		}

		/// <summary>
		/// </summary>
		/// <param name="source"></param>
		/// <param name="e"></param>
		private void DataGrid1_ItemCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			if (e.CommandName == "Update")
			{
				//attualmente non vengono gestiti i fascicoli 
				DocsPaWR.InfoDocumento infoDoc;
			

				if(e.Item.ItemIndex >=0)
				{
					string str_indexSel = ((Label) this.DataGrid1.Items[e.Item.ItemIndex].Cells[5].Controls[1]).Text;
					int indexSel = Int32.Parse(str_indexSel);
					areaLav = (DocsPAWA.DocsPaWR.AreaLavoro) Session["areaDiLavoro.listaAreaLav"];
			
					if (indexSel > -1) 
						infoDoc = (DocsPAWA.DocsPaWR.InfoDocumento) areaLav.lista[indexSel];
					else 
					{
						infoDoc = null;
						return;
					}
					//luluciani 17/11/2006 cambiata gestione pulizia session Back, non si fa più da testata
					Utils.CleanSessionMemoria(this);
					DocumentManager.setRisultatoRicerca(this, infoDoc);
					//Session.Remove("areaDiLavoro.listaAreaLav");
					Response.Write("<script>window.open('../documento/gestionedoc.aspx?tab=protocollo','principale');</script>");

					Response.Write("<script>window.close();</script>");
				}
			}
            //gestione visualizzatore unificato
            if (e.CommandName == "VisDoc")
            {
                //attualmente non vengono gestiti i fascicoli 
				DocsPaWR.InfoDocumento infoDoc;

                if (e.Item.ItemIndex >= 0)
                {
                    string str_indexSel = ((Label)this.DataGrid1.Items[e.Item.ItemIndex].Cells[5].Controls[1]).Text;
                    int indexSel = Int32.Parse(str_indexSel);
                    areaLav = (DocsPAWA.DocsPaWR.AreaLavoro)Session["areaDiLavoro.listaAreaLav"];

                    if (indexSel > -1)
                    {
                        infoDoc = (DocsPAWA.DocsPaWR.InfoDocumento)areaLav.lista[indexSel];

                        //vis unificata
                        DocsPAWA.DocsPaWR.SchedaDocumento schedaSel = DocumentManager.getDettaglioDocumento(this, infoDoc.idProfile, infoDoc.docNumber);
                        DocumentManager.setDocumentoSelezionato(this, schedaSel);
                        FileManager.setSelectedFile(this, schedaSel.documenti[0], false);
                        ClientScript.RegisterStartupScript(this.GetType(), "lanciaVIs", "loadvisualizzaDoc('" + Session.SessionID + "','" + schedaSel.docNumber + "','" + schedaSel.systemId + "');", true);
                        //luluciani 17/11/2006 cambiata gestione pulizia session Back, non si fa più da testata
                        Utils.CleanSessionMemoria(this);
                        DocumentManager.setRisultatoRicerca(this, infoDoc);
                    }
                    else
                    {
                        infoDoc = null;
                        return;
                    }

                }

           
            }
		}
		#endregion datagrid

		/// <summary>
		/// </summary>
		private void getParametersFromQueryString()
		{
			action=Request.QueryString["action"];
			//AMMETTE I VALORI:
			//	'addDocToFolder':  se chiamato da fascDocumenti.aspx.cs per
			//	inserire il documento da area di lavoro selezionato
			//	nella folder
			//	'setDoc':   se chiamato da docProtocollo.aspx per impostare il 
			//	numero di risposta al protocollo
			//  'gestAreaLav': se chiamato dal menu di testata per la cancellazione dei doc dall'area di lavoro
		} 

		/// <summary>
		/// </summary>
		private void setDocAreaDiLavoro()
		{
			DocsPaWR.InfoDocumento infoDoc;
			DocsPaWR.SchedaDocumento schedaDoc;
   
            if(this.DataGrid1.SelectedIndex >=0)
			{
				string str_indexSel = ((Label) this.DataGrid1.SelectedItem.Cells[5].Controls[1]).Text;
				int indexSel = Int32.Parse(str_indexSel);

				areaLav = (DocsPAWA.DocsPaWR.AreaLavoro) Session["areaDiLavoro.listaAreaLav"];
			
				if (indexSel > -1) 
					infoDoc = (DocsPAWA.DocsPaWR.InfoDocumento) areaLav.lista[indexSel];
				else 
					infoDoc = null;

				string pageToRefresh="";
				string frameToRefresh="";
				if (action!=null && !action.Equals("setDoc") && !action.Equals("gestAreaLav"))
				{
					//entra  se action è diverso da null oppure 
					//		se è contemporaneamente diverso da null e diverso da 'setDoc'
					pageToRefresh="../fascicolo/fascDocumenti.aspx";
					frameToRefresh="IframeTabs";

					string folderId=Request.QueryString["folderId"];
					bool outValue = false;
					if (folderId!=null && folderId != "")
					{
                        String message = String.Empty;
                        DocumentManager.addDocumentoInFolder(this,infoDoc.idProfile, folderId,  false, out outValue, out message);
					}
				}
				else
				{
					pageToRefresh="../documento/docProtocollo.aspx";
					frameToRefresh="IframeTabs";

					schedaDoc = DocumentManager.getDocumentoInLavorazione(this);
					schedaDoc.rispostaDocumento = infoDoc;
					DocumentManager.setDocumentoInLavorazione(this, schedaDoc);
				}

				DocumentManager.removeDataGridAreaLavoro(this);
				Session.Remove("areaDiLavoro.listaAreaLav");

				Response.Write("<script>var k=window.open('"+pageToRefresh+"','"+frameToRefresh+"'); window.close();</script>");
			}
		}

		/// <summary>
		/// </summary>
		private void eliminaDocDaADL()
		{
			try
			{
				//attualmente non vengono gestiti i fascicoli 
				DocsPaWR.InfoDocumento infoDoc;
				
				if(this.DataGrid1.SelectedIndex >= 0)
				{
					string str_indexSel = ((Label) this.DataGrid1.SelectedItem.Cells[5].Controls[1]).Text;
					int indexSel = Int32.Parse(str_indexSel);
					areaLav = (DocsPAWA.DocsPaWR.AreaLavoro) Session["areaDiLavoro.listaAreaLav"];
			
					if (indexSel > -1) 
					{
						infoDoc = (DocsPAWA.DocsPaWR.InfoDocumento) areaLav.lista[indexSel];
					}
					else 
					{
						infoDoc = null;
						return;
					}

					//elimino il doc (per ora gestiamo solo i doc)
					DocumentManager.eliminaDaAreaLavoro(this, infoDoc.idProfile, null);
					
					if(Session["listInArea"]!=null)
					{
						Hashtable listInArea = new Hashtable();
						listInArea =(System.Collections.Hashtable)Session["listInArea"];
						listInArea.Remove(infoDoc.docNumber);
						Session["listInArea"] = listInArea ;
					}
					if(currentPage>1 && this.DataGrid1.Items.Count==1)
					{
						currentPage=currentPage-1;
					}
					//aggiorno il dataGrid
					this.LoadData(true);
					this.DataGrid1.SelectedIndex=-1;
				}
				else
				{
					Response.Write("<script>alert('Attenzione: selezionare un documento!');</script>");	
				}
			} 
			catch (Exception ex) 
			{
				ErrorManager.redirect(this, ex);
			}
		}

		#region Web Form Designer generated code
		/// <summary>
		/// </summary>
		/// <param name="e"></param>
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
			this.btn_deleteAllADL.Click += new System.Web.UI.ImageClickEventHandler(this.btn_deleteAllADL_Click);
			this.DataGrid1.ItemCreated += new System.Web.UI.WebControls.DataGridItemEventHandler(this.DataGrid1_ItemCreated);
			this.DataGrid1.ItemCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.DataGrid1_ItemCommand);
			this.DataGrid1.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.DataGrid1_PageIndexChanged);
			this.DataGrid1.ItemDataBound += new System.Web.UI.WebControls.DataGridItemEventHandler(this.DataGrid1_ItemDataBound);
			this.DataGrid1.SelectedIndexChanged += new System.EventHandler(this.DataGrid1_SelectedIndexChanged);
			this.btn_elimina.Click += new System.EventHandler(this.btn_elimina_Click);
			this.btn_ok.Click += new System.EventHandler(this.btn_ok_Click);
			this.btn_chiudi.Click += new System.EventHandler(this.btn_chiudi_Click);
			this.txtIdRegistro.TextChanged += new System.EventHandler(this.txtIdRegistro_TextChanged);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		/// <summary>
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_ok_Click(object sender, System.EventArgs e)
		{
			setDocAreaDiLavoro();
		}

		/// <summary>
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_chiudi_Click(object sender, System.EventArgs e)
		{
			Session.Remove("areaDiLavoro.listaAreaLav");
			Response.Write("<script>window.close();</script>");	
		}

		/// <summary>
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_elimina_Click(object sender, System.EventArgs e)
		{
			eliminaDocDaADL();
		}

		/// <summary>
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_deleteAllADL_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
			try
			{
				DocsPaWR.InfoDocumento infoDoc;
				areaLav = (DocsPAWA.DocsPaWR.AreaLavoro) Session["areaDiLavoro.listaAreaLav"];

				if (areaLav == null)
				{
					return;
				}
	
				if(DataGrid1.PageCount > 0)
				{
					for(int i = DataGrid1.PageCount; i > 0; i--)
					{
						currentPage = i;

						// Carica i dati senza aggiornare la visualizzazione
						this.LoadData(false);

						int tot = areaLav.lista.Length - 1;	

						for(int f = tot; f >= 0; f--)
						{
							infoDoc = (DocsPAWA.DocsPaWR.InfoDocumento) areaLav.lista[f];

							//elimino il doc (per ora gestiamo solo i doc)
							DocumentManager.eliminaDaAreaLavoro(this, infoDoc.idProfile, null);

							//							//aggiorno il dataGrid
							//							areaLav = DocumentManager.rimuoviDaListaAreaLav(areaLav, f);
						}
					}
				}
					
				// Carica i dati e aggiorna la visualizzazione
				this.LoadData(true);
			}
			catch (Exception ex) 
			{
				ErrorManager.redirect(this, ex);
			}
		}

		/// <summary>
		/// </summary>
		/// <param name="source"></param>
		/// <param name="e"></param>
		private void DataGrid1_PageIndexChanged(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
		{
			this.DataGrid1.SelectedIndex=-1;

			DataGrid1.CurrentPageIndex = e.NewPageIndex;
			currentPage = e.NewPageIndex + 1;

			this.pageNumberLabel.Text = currentPage.ToString();
			string tipoDoc = Request.QueryString["tipoDoc"];
			
			// Aggiorno i dati
			this.LoadData(true);
		}

		/// <summary>
		/// Questo metodo carica e mostra la pagina selezionata
		/// </summary>
		private void LoadData(bool updateGrid)
		{
			int nRec;
			int numRec;
			string tipoDoc = Request.QueryString["tipoDoc"];
			/*massimo digregorio 
				descrizione: visualizzazione dei DOCUMENTI in ADL filtrati per Registro del FASCICOLO. 
				Utilizzato da: "gestione fascicolo" button "INSERISCI DOC DA ADL"
				new:*/
			string idReg =null;
			if (!Page.IsPostBack)
			{
				idReg = Request.QueryString["idReg"];
				this.txtIdRegistro.Text = idReg;
			}else
				idReg=this.txtIdRegistro.Text;

			if (idReg == null) idReg= "";
			areaLav = DocumentManager.getListaAreaLavoro(this, tipoDoc,null, currentPage, out numRec, out nRec, idReg);
			
			if (areaLav != null)
			{
				DataGrid1.VirtualItemCount = nRec;
				areaLav.TotalRecs = nRec;

				Session["areaDiLavoro.listaAreaLav"] = areaLav;

				if(updateGrid)
				{
					this.BindGrid(areaLav, currentPage);
				}

			}
		}

		/// <summary>
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void DataGrid1_ItemCreated(object sender, System.Web.UI.WebControls.DataGridItemEventArgs e)
		{
			TableCell pager = e.Item.Cells[0];
			foreach ( object o in pager.Controls)
			{
				if(o is LinkButton)
				{
					LinkButton lb = (LinkButton) o ;
					lb.Text = "[" + lb.Text + "]";
				}
			}
		}	

		/// <summary>		
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void VisualFasc_Clicked(object sender, System.EventArgs e)
		{									
			this.LoadData(true);			
		}

		private void DataGrid1_ItemDataBound(object sender, System.Web.UI.WebControls.DataGridItemEventArgs e)
		{
			if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem) 
			{
				string dataAnnull = ((TableCell) e.Item.Cells[7]).Text;
				try 
				{
					DateTime dt = Convert.ToDateTime(dataAnnull);
					e.Item.ForeColor = Color.Red;
					e.Item.Font.Bold=true;
					e.Item.Font.Strikeout=true;
				} 
				catch {}
			}
            
         
		}

		private void txtIdRegistro_TextChanged(object sender, System.EventArgs e)
		{
		
		}

		/// <summary>
		/// 
		/// </summary>
		private void AttatchGridPagingWaitControl()
		{
			DataGridPagingWaitControl.DataGridID=this.DataGrid1.ClientID;
			DataGridPagingWaitControl.WaitScriptCallback="WaitGridPagingAction();";
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

        protected void DataGrid1_ItemCreated1(object sender, DataGridItemEventArgs e)
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
