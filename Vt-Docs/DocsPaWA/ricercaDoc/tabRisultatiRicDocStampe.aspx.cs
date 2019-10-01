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
using DocsPAWA.DocsPaWR;

namespace DocsPAWA.ricercaDoc
{
	/// <summary>
	/// Summary description for tabRisultatiRicDocStampe.
	/// </summary>
	public class tabRisultatiRicDocStampe : DocsPAWA.CssPage
	{
		protected System.Web.UI.WebControls.DataGrid DataGrid1;
		protected DocsPAWA.dataSet.DataSetRDocStampeReg dataSetRDocStampeReg1;

		protected DocsPAWA.DocsPaWR.Ruolo userRuolo;
		protected DocsPAWA.DocsPaWR.InfoUtente Safe;
		protected DocsPAWA.DocsPaWR.FiltroRicerca[][] ListaFiltri;
		protected DocsPAWA.DocsPaWR.InfoDocumento InfoCurr;
		protected DocsPAWA.DocsPaWR.InfoDocumento[] ListaDoc;
		protected DocsPAWA.DocsPaWR.Utente userHome;
		protected DataView dv;
		protected DataTable datatable;
		protected ArrayList Rows;

	
		#region Dichiarazione Variabili

		private static int currentPage;
		private int numTotPage;
		protected System.Web.UI.WebControls.Label titolo;
		protected System.Web.UI.HtmlControls.HtmlTableRow trHeader;
		protected System.Web.UI.HtmlControls.HtmlTableRow tr1;
		protected System.Web.UI.HtmlControls.HtmlTableCell Td2;
		protected System.Web.UI.HtmlControls.HtmlTableRow trBody;
		private int nRec;
				
		private string currentSort
		{
			get
			{
				if (this.Session["SortNumDoc"] == null)
					this.Session["SortNumDoc"] = "ASC";	

				return this.Session["SortNumDoc"].ToString();
			}
			set
			{
				this.Session["SortNumDoc"]= value;
			}
		}

		#endregion

		private void Page_Load(object sender, System.EventArgs e) //Celeste
		{
		try
			{	
				//rimuovere la variabile di sessione
				Page.Response.Expires = 0;
				Utils.startUp(this);
				DocumentManager.removeDocumentoSelezionato(this);

				if(DocumentManager.getFiltroRicDoc(this)!=null)
				{
					Safe= new DocsPAWA.DocsPaWR.InfoUtente();
					Safe=UserManager.getInfoUtente(this);
					this.ListaFiltri=DocumentManager.getFiltroRicDoc(this);
				}
				
				if (!IsPostBack)
				{
                    this.AttatchGridPagingWaitControl();

					if(DocumentManager.getFiltroRicDoc(this)!=null)
					{
						this.ListaFiltri=DocumentManager.getFiltroRicDoc(this);						
						currentPage = 1;
						fillDatagrid(currentPage);						
					}
				}							
			}
		
			catch(System.Web.Services.Protocols.SoapException es) 
			{
				ErrorManager.redirect(this, es);
			} 
		}

		private void Page_PreRender(object sender,EventArgs e)
		{
			if (!this.IsPostBack)
			{
				this.RefreshCountDocumenti();

				this.SetControlsVisibility();
			}
		}

		#region Page_load Commentato Celeste
//		private void Page_Load(object sender, System.EventArgs e)
//		{
//			// Put user code to initialize the page here
//
//			try
//			{	
//				//rimuovere la variabile di sessione
//				Page.Response.Expires = 0;
//				Utils.startUp(this);
//				DocumentManager.removeDocumentoSelezionato(this);
//
//				userRuolo=UserManager.getRuolo(this);
//				//userReg= UserManager.getRegistroSelezionato(this);
//			
//				if(DocumentManager.getFiltroRicDoc(this)!=null)
//				{
//					this.ListaFiltri=DocumentManager.getFiltroRicDoc(this);
//					Safe= new DocsPAWA.DocsPaWR.InfoUtente();
//					Safe=UserManager.getInfoUtente(this);
//					//fare ogni volta la query o mettere tutto in session???
//					if(DocumentManager.getDatagridDocumento(this)==null)
//					{
//						//fare ogni volta la query o mettere tutto in session???
//						ListaDoc=DocumentManager.getQueryInfoDocumento(Safe.idGruppo, Safe.idPeople, this,this.ListaFiltri);
//						DocumentManager.setListaDocNonProt(this,ListaDoc);//Session["tabRicDoc.Listadoc"]=ListaDoc;
//					}
//					else
//						ListaDoc= DocumentManager.getListaDocNonProt(this);//Session["tabRicDoc.Listadoc"];
//
//
//					if (ListaDoc == null || ListaDoc.Length <=0)
//					{
//						this.lbl_message.Visible = true;
//						return;
//					} 
//					else
//						this.lbl_message.Visible = false;
//
//					if(!IsPostBack)
//					{
//						this.CaricaDatagridProt(this.DataGrid1,ListaDoc);
//					}
//					if(DocumentManager.getDatagridDocumento(this)!=null)
//						dv=((DataTable)DocumentManager.getDatagridDocumento(this)).DefaultView;
//					else
//					{
//						//non so' se funziona potrebbe essere necessaria 
//						dv=dataSetRDocStampeReg1.Tables[0].DefaultView;
//					}
//					if (dv.Table.Columns.Count>0)
//					{
//						dv.Sort="numDoc DESC";
//					}
//
//				}
//			}
//		
//			catch(System.Web.Services.Protocols.SoapException es) 
//			{
//				ErrorManager.redirect(this, es);
//			} 
//
//		}
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
			this.dataSetRDocStampeReg1 = new DocsPAWA.dataSet.DataSetRDocStampeReg();
			((System.ComponentModel.ISupportInitialize)(this.dataSetRDocStampeReg1)).BeginInit();
			this.DataGrid1.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.DataGrid1_OnPageIndexChanged);
			this.DataGrid1.SortCommand += new System.Web.UI.WebControls.DataGridSortCommandEventHandler(this.Sorting);
			this.DataGrid1.SelectedIndexChanged += new System.EventHandler(this.DataGrid1_SelectedIndexChanged);
			// 
			// dataSetRDocStampeReg1
			// 
			this.dataSetRDocStampeReg1.DataSetName = "DataSetRDocStampeReg";
			this.dataSetRDocStampeReg1.Locale = new System.Globalization.CultureInfo("en-US");
			this.Load += new System.EventHandler(this.Page_Load);
			this.PreRender += new System.EventHandler(this.Page_PreRender);
			((System.ComponentModel.ISupportInitialize)(this.dataSetRDocStampeReg1)).EndInit();

		}
		#endregion

		#region Class per il DataGrid

		private void fillDatagrid(int numberPage)
		{
			DocumentManager.removeListaDocProt(this);
            SearchResultInfo[] idProfileList;
			ListaDoc = DocumentManager.getQueryInfoDocumentoPaging(Safe.idGruppo, Safe.idPeople, this,this.ListaFiltri,numberPage,out numTotPage,out nRec,false,false, true, false, out idProfileList);
			DocumentManager.setListaDocNonProt(this,ListaDoc);
			this.DataGrid1.VirtualItemCount = nRec;
			this.DataGrid1.CurrentPageIndex = numberPage -1 ;
			this.CaricaDatagridProt(this.DataGrid1,ListaDoc,currentSort);			
		}

		private void CaricaDatagridProt(DataGrid dg,DocsPaWR.InfoDocumento[] listaDoc, string SortType)
		{
			
			try
			{
				InfoCurr= new DocsPAWA.DocsPaWR.InfoDocumento();
				Rows=new ArrayList();
				DataView dv;
					
				if(listaDoc!=null)
				{
					if (listaDoc.Length>0)
					{
						for(int i=0;i<listaDoc.Length;i++)
						{
							InfoCurr=(DocsPAWA.DocsPaWR.InfoDocumento)listaDoc[i];
							if(InfoCurr!=null && (InfoCurr.numProt==null ||InfoCurr.numProt.Equals("")) )
							{							
								//Rows.Add(new Row(InfoCurr.numProt,InfoCurr.segnatura,InfoCurr.dataApertura,InfoCurr.codRegistro,InfoCurr.tipoProto,MittDest,InfoCurr.oggetto,i));
								int docNumber;
								if (InfoCurr.docNumber == null || InfoCurr.docNumber.Equals(""))
									docNumber = 0;
								else
									docNumber = Int32.Parse(InfoCurr.docNumber);
								
								this.dataSetRDocStampeReg1.element1.Addelement1Row(docNumber,InfoCurr.dataApertura,InfoCurr.oggetto,i, InfoCurr.codRegistro);
							}
								
						}
						datatable=this.dataSetRDocStampeReg1.Tables[0];
						dv = datatable.DefaultView; //Celeste
						dv.Sort = string.Format("{0} {1}","numDoc", "DESC");
						DataGrid1.DataSource=dv; //Fine Celeste
						DataGrid1.DataBind();
					}
								
									
					else
					{
						DataGrid1.Visible=false;
					}
				}
							
				else
				{
					DataGrid1.Visible=false;
				}
				
			}
			catch(System.Web.Services.Protocols.SoapException es) 
			{
				ErrorManager.redirect(this, es);
			} 
		}
	
		private void DataGrid1_OnPageIndexChanged(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
		{
			//Celeste
			/*DataGrid1.CurrentPageIndex=e.NewPageIndex;
			DataGrid1.DataSource=DocumentManager.getDatagridDocumento(this);
			DataGrid1.DataBind();
			*/ 

			DataGrid1.CurrentPageIndex = e.NewPageIndex ;
			fillDatagrid(e.NewPageIndex + 1);			
			//Fine Celeste
		}

		private void DataGrid1_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			
			string key=((Label)this.DataGrid1.Items[this.DataGrid1.SelectedIndex].Cells[3].Controls[1]).Text;		
			DocsPaWR.InfoDocumento infoDoc=new DocsPAWA.DocsPaWR.InfoDocumento();
			//Celeste														  
			//infoDoc = (DocsPAWA.DocsPaWR.InfoDocumento)ListaDoc[Int32.Parse(key)];
			infoDoc = DocumentManager.getListaDocProt(this)[Int32.Parse(key)];
			//Fine Celeste
			DocsPaWR.SchedaDocumento schedaDoc=new DocsPAWA.DocsPaWR.SchedaDocumento();
			schedaDoc =	DocumentManager.getDettaglioDocumento(this,infoDoc.idProfile,infoDoc.docNumber);
			FileManager.setSelectedFileReg(this,schedaDoc.documenti[0], "../popup");
			string sval=@"../popup/ModalVisualStampaReg.aspx?id="+this.Session.SessionID;
			RegisterStartupScript("ApriModale","<script>OpenMyDialog('"+sval+"');</script>");

	
		}

		/*
		//Celeste
		//sembra non funzionare...
		private void Sorting(object source, System.Web.UI.WebControls.DataGridSortCommandEventArgs e)
		{

			if(e.SortExpression.Equals("numDoc"))
			{
				dv.Sort = e.SortExpression+" DESC";
				
			}
			if(e.SortExpression.Equals("oggetto"))
			{
				dv.Sort=e.SortExpression;
			}
			DataGrid1.DataSource = DocumentManager.getDatagridDocumento(this);
			DataGrid1.DataBind();
		}
		*/
		private void Sorting(object source, System.Web.UI.WebControls.DataGridSortCommandEventArgs e)
		{
			if (currentSort == "ASC")
				currentSort = "DESC";
			else
				currentSort = "ASC";

			fillDatagrid(DataGrid1.CurrentPageIndex +1);
		}

		//Fine Celeste

		
		//class per caricare il datagrid.	
#endregion

		/// <summary>
		/// 
		/// </summary>
		/// <param name="msg"></param>
		protected void PrintMsg(string msg)
		{
			this.titolo.Text=msg;
		}

		/// <summary>
		/// 
		/// </summary>
		private void RefreshCountDocumenti()
		{			 
			string msg="Elenco stampe";

			if (this.ListaDoc!=null)
				msg+=" - Trovati " + this.nRec.ToString() + " elementi.";

			this.PrintMsg(msg); 
		}

		/// <summary>
		/// Impostazione visibilità controlli
		/// </summary>
		private void SetControlsVisibility()
		{
			this.trHeader.Visible=true;

			this.trBody.Visible=(this.DataGrid1.Items.Count>0);
		}

        private void AttatchGridPagingWaitControl()
        {
            DataGridPagingWaitControl.DataGridID = this.DataGrid1.ClientID;
            DataGridPagingWaitControl.WaitScriptCallback = "WaitDataGridCallback(eventTarget,eventArgument);";
        }

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