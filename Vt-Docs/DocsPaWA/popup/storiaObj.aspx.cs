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
	/// Summary description for storiaObj.
	/// </summary>
    public class storiaObj : DocsPAWA.CssPage
	{
		protected System.Web.UI.WebControls.Button btn_ok;
		protected System.Web.UI.WebControls.DataGrid DataGrid1;
		protected System.Web.UI.WebControls.Label Label2;
		protected DocsPAWA.dataSet.DataSetStoriaObj dataSetStoriaObj1;
		protected System.Web.UI.WebControls.Label lb_dettagli;
        protected System.Web.UI.WebControls.DataGrid Datagrid2;

		//my var
		protected ArrayList Dt_elem;
		protected DocsPAWA.DocsPaWR.SchedaDocumento schedaDocumento;
        protected DocsPAWA.DocsPaWR.Fascicolo fascicolo;
		protected DocsPAWA.DocsPaWR.DocumentoStoricoOggetto[] ListaStoriaOggetto;
		protected System.Web.UI.WebControls.DataGrid dg_CorrSto;
		protected System.Web.UI.HtmlControls.HtmlGenericControl div_storicoOggetto;
		protected System.Web.UI.HtmlControls.HtmlGenericControl div_storicoCorr;
        protected System.Web.UI.HtmlControls.HtmlGenericControl div_StoricoData;
        protected System.Web.UI.HtmlControls.HtmlGenericControl div_StoricoCampiProf;
        protected System.Web.UI.WebControls.DataGrid DataGridStoricoCampiProf;
		string tipoOggetto;

	
		private void Page_Load(object sender, System.EventArgs e)
		{
			if(!IsPostBack)
			{
                tipoOggetto = Request.QueryString["tipo"];
				// Put user code to initialize the page here
				schedaDocumento=(DocsPAWA.DocsPaWR.SchedaDocumento)DocumentManager.getDocumentoSelezionato(this) ;
                fascicolo = FascicoliManager.getFascicoloSelezionato(this);
                //se si tratta dello storico profilati fascicoli e le informazioni sul fascicolo non sono state correttamente reperite termino con un msg di errore
                if((fascicolo==null && tipoOggetto.Equals("campiProfilatiFasc")) || (fascicolo!=null && fascicolo.systemID==null))
                {
                    
                    this.lb_dettagli.Text = "Errore nel reperimento dei dati del fascicolo";
                    this.lb_dettagli.Visible = true;
                    return;
                }
                if ((schedaDocumento == null && (tipoOggetto.Equals("campiProfilati")) || (schedaDocumento!=null && schedaDocumento.docNumber==null)))
				{
					this.lb_dettagli.Text = "Errore nel reperimento dei dati del documento";
					this.lb_dettagli.Visible = true;
					return;
				}			
			
				/******   tipo di oggetto da ricercare  ******
								valori possibili:
					oggetto
					mit
					mitInt
					dest
                    data
			        campiProfilati
				 ******                                 *******/         
				if (tipoOggetto.Equals("oggetto"))
				{
					//Response.Write("<SCRIPT>var divStoricoCorr = document.getElementById('div_storicoCorr'); if(divStoricoCorr!=null){document.getElementById('div_storicoCorr').style.visibility = 'hidden';}</SCRIPT>");
					this.FindControl("div_storicoCorr").Visible=false;
	                this.FindControl("div_StoricoData").Visible=false;
                    this.FindControl("div_StoricoCampiProf").Visible = false;
					this.Label2.Text = "Storia delle modifiche sull'oggetto";
                    this.Title = "Storia oggetto";

					//prima di richiamare il metodo verifico che ci siano state delle modifiche
					if (schedaDocumento.modOggetto == null ||  schedaDocumento.modOggetto.Equals("0"))
					{
						this.lb_dettagli.Text = "Non sono state effettuate modifiche";
						this.lb_dettagli.Visible = true;
					}

					ListaStoriaOggetto = DocumentManager.getStoriaModifiche(this, schedaDocumento, tipoOggetto);
					//stava qui...	
					
					if(ListaStoriaOggetto == null || (ListaStoriaOggetto != null && ListaStoriaOggetto.Length<=0))
					{
						DataGrid1.Visible = false;
						this.lb_dettagli.Text = "Non sono state effettuate modifiche";;
						this.lb_dettagli.Visible = true;
					}
					
				
					string data;
					string ruolo;
					string utente;
					string modificaObj;
					for(int i=0;i<ListaStoriaOggetto.Length;i++)
					{
						data = ((DocsPAWA.DocsPaWR.DocumentoStoricoOggetto)ListaStoriaOggetto[i]).dataModifica;
						ruolo = ((DocsPAWA.DocsPaWR.DocumentoStoricoOggetto)ListaStoriaOggetto[i]).ruolo.descrizione;
						//ruolo = UserManager.getDecrizioneCorrispondente( ((DocsPAWA.DocsPaWR.DocumentoStoricoOggetto)ListaStoriaOggetto[i]).ruolo);
					
						utente = ((DocsPAWA.DocsPaWR.DocumentoStoricoOggetto)ListaStoriaOggetto[i]).utente.descrizione;
						modificaObj = ((DocsPAWA.DocsPaWR.DocumentoStoricoOggetto)ListaStoriaOggetto[i]).descrizione;
						this.dataSetStoriaObj1.element1.Addelement1Row(data,ruolo,utente,modificaObj);
					}	
					Session["Dg_storiaObj"]=this.dataSetStoriaObj1.Tables[0];
					this.DataGrid1.DataSource=this.dataSetStoriaObj1.Tables[0];
					this.DataGrid1.DataBind();		
				}

				if(tipoOggetto == "mit")
				{
					this.FindControl("div_storicoOggetto").Visible=false;
	                this.FindControl("div_StoricoData").Visible=false;
                     this.FindControl("div_StoricoCampiProf").Visible=false;
					//Recuperiamo lo storico del mittente
					this.Label2.Text = "Storia delle modifiche sul mittente";
                    this.Title = "Storia Mittente";
				
					DocsPaWR.DocumentoStoricoMittente[]	mittenti = DocumentManager.getStoriaModifiche(schedaDocumento,"M");
					
					if(mittenti!=null && mittenti.Length > 0)
					{
						DO_BindGrid(mittenti,tipoOggetto);
					}
					else
					{
						dg_CorrSto.Visible = false;
						this.lb_dettagli.Text = "Non sono state effettuate modifiche";
						this.lb_dettagli.Visible = true;
					}
				}
				else
				{
					if(tipoOggetto == "dest")
					{
						this.FindControl("div_storicoOggetto").Visible=false;
	                    this.FindControl("div_StoricoData").Visible=false;
                        this.FindControl("div_StoricoCampiProf").Visible = false;
						//Recuperiamo lo storico del destinatario
						this.Label2.Text = "Storia delle modifiche sul destinatario";
                        this.Title = "Storia Destinatario";

						DocsPaWR.DocumentoStoricoMittente[] destinatari = DocumentManager.getStoriaModifiche(schedaDocumento,"D");
						if(destinatari!=null && destinatari.Length > 0)
						{
							DO_BindGrid(destinatari,tipoOggetto);
						}
						else
						{
							dg_CorrSto.Visible = false;
							this.lb_dettagli.Text = "Non sono state effettuate modifiche";
							this.lb_dettagli.Visible = true;
						}
					}
					else
					{
						if(tipoOggetto == "mitInt")
						{
							this.FindControl("div_storicoOggetto").Visible=false;
	                        this.FindControl("div_StoricoData").Visible=false;
                            this.FindControl("div_StoricoCampiProf").Visible = false;
							//Recuperiamo lo storico del mittente Intermedio
							this.Label2.Text = "Storia delle modifiche sul mittente intermedio";
                            this.Title = "Storia Mittente Intermedio";
							DocsPaWR.DocumentoStoricoMittente[] mittenti = DocumentManager.getStoriaModifiche(schedaDocumento,"I");
							if(mittenti!=null && mittenti.Length > 0)
							{
								DO_BindGrid(mittenti,tipoOggetto);
							}
							else
							{
								dg_CorrSto.Visible = false;
								this.lb_dettagli.Text = "Non sono state effettuate modifiche";
								this.lb_dettagli.Visible = true;
							}
						}
						else
						{
							dg_CorrSto.Visible = false;
						}
					}
				}

                if (tipoOggetto == "mitMultipli")
                {
                    dg_CorrSto.Visible = true;
                    this.FindControl("div_storicoOggetto").Visible = false;
                    this.FindControl("div_StoricoCampiProf").Visible = false;
                    //Recuperiamo lo storico del mittente
                    this.Label2.Text = "Storia delle modifiche dettaglio mittenti";
                    this.Title = "Storia dettaglio mittenti";

                    DocsPaWR.DocumentoStoricoMittente[] mittenti = DocumentManager.getStoriaModifiche(schedaDocumento, "MD");

                    if (mittenti != null && mittenti.Length > 0)
                    {
                        DO_BindGrid(mittenti, tipoOggetto);
                    }
                    else
                    {
                        dg_CorrSto.Visible = false;
                        this.lb_dettagli.Text = "Non sono state effettuate modifiche";
                        this.lb_dettagli.Visible = true;
                    }
                }
                if (tipoOggetto == "data") 
                {
                    this.FindControl("div_StoricoData").Visible = true;
                    this.FindControl("div_storicoOggetto").Visible = false;
                    this.FindControl("div_storicoCorr").Visible = false;
                    this.FindControl("div_StoricoCampiProf").Visible = false;
                    this.Label2.Text = "Storia delle modifiche su Data/Ora Arrivo";
                    this.Title = "Storico Data/Ora arrivo";

                    DocsPaWR.DocumentoStoricoDataArrivo[] datearrivo = DocumentManager.getStoriaModifiche(schedaDocumento.docNumber);

                    if (datearrivo != null && datearrivo.Length > 0)
                    {
                        DO_BindGridData(datearrivo);
                    }
                    else
                    {
                        Datagrid2.Visible = false;
                        this.lb_dettagli.Text = "Non sono state effettuate modifiche";
                        this.lb_dettagli.Visible = true;
                    }
                }

                if (tipoOggetto.Equals("campiProfilati") || tipoOggetto.Equals("campiProfilatiFasc"))
                {
                    DocsPAWA.DocsPaWR.StoricoProfilati[] storico=null;
                    string storicoDocFasc = tipoOggetto;
                    this.FindControl("div_storicoOggetto").Visible = false;
                    this.FindControl("div_StoricoData").Visible = false;
                    this.FindControl("div_storicoCorr").Visible = false;
                    this.Label2.Text = "Storia delle modifiche sui campi profilati";
                    this.Title = "Storia campi profilati";
                    if(storicoDocFasc.Equals("campiProfilati"))
                        this.lb_dettagli.Text = "Storico del tipo documento: "+schedaDocumento.template.DESCRIZIONE;
                    else
                        this.lb_dettagli.Text = "Storico del tipo fascicolo: " + fascicolo.template.DESCRIZIONE;
                    this.lb_dettagli.Visible = true;
                                  
                    //controllo che ci siano informazioni nello storico
                    if (storicoDocFasc.Equals("campiProfilati"))
                        storico = DocumentManager.getStoriaProfilatiAtto(this, schedaDocumento.template,schedaDocumento.docNumber, UserManager.getRuolo().idGruppo);
                    else
                    {
                        storico=FascicoliManager.getStoriaProfilatiFasc(this, fascicolo.template, fascicolo.systemID);
                    }
                    if (storico != null && storico.Length > 0)
                    {
                        DO_BindGridStoricoProfilati(storico);
                    }
                    else
                    {
                        DataGridStoricoCampiProf.Visible = false;
                        this.lb_dettagli.Text = "Non sono state effettuate modifiche";
                    }
                }
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
			this.dataSetStoriaObj1 = new DocsPAWA.dataSet.DataSetStoriaObj();
			((System.ComponentModel.ISupportInitialize)(this.dataSetStoriaObj1)).BeginInit();
			this.DataGrid1.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.Datagrid1_pager);
			this.dg_CorrSto.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.dg_CorrSto_PageIndexChanged);
            this.Datagrid2.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.Datagrid2_pager);
            this.DataGridStoricoCampiProf.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.DataGridStoricoCampiProf_pager);
            this.btn_ok.Click += new System.EventHandler(this.btn_ok_Click);
			// 
			// dataSetStoriaObj1
			// 
			this.dataSetStoriaObj1.DataSetName = "DataSetStoriaObj";
			this.dataSetStoriaObj1.Locale = new System.Globalization.CultureInfo("en-US");
			this.Load += new System.EventHandler(this.Page_Load);
			((System.ComponentModel.ISupportInitialize)(this.dataSetStoriaObj1)).EndInit();

		}
		#endregion

		#region datagrid

		private void Datagrid1_pager(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
		{
			this.DataGrid1.CurrentPageIndex=e.NewPageIndex;
			if(Session["Dg_storiaObj"]!=null)
			{
				DataTable TDNew=(DataTable) Session["Dg_storiaObj"];			
				DataGrid1.DataSource=TDNew; 						
				DataGrid1.DataBind();
			}
		}

		private void dg_CorrSto_PageIndexChanged(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
		{
			this.dg_CorrSto.CurrentPageIndex=e.NewPageIndex;
			if(Session["Dg_storiaCorr"]!=null)
			{
				DataTable DataTableNew=(DataTable) Session["Dg_storiaCorr"];			
				dg_CorrSto.DataSource=DataTableNew; 						
				dg_CorrSto.DataBind();
			}
		}

        private void Datagrid2_pager(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
        {
            this.Datagrid2.CurrentPageIndex = e.NewPageIndex;
            if (Session["Dg_storiaData"] != null)
            {
                DataTable TDNew = (DataTable)Session["Dg_storiaData"];
                Datagrid2.DataSource = TDNew;
                Datagrid2.DataBind();
            }
        }

		public class Cols 
		{		
			private string data;
			private string ruolo;
			private string utente;
			private string modificaObj;
		
			public Cols(string data, string ruolo, string utente, string modificaObj)
			{
				this.ruolo=ruolo;
				this.utente= utente;
				this.data=data;
				this.modificaObj=modificaObj;			
			}			
			public string Data{get{return data;}}
			public string Ruolo{get{return ruolo;}}
			public string Utente{get{return utente;}}
			public string ModificaObj{get{return modificaObj;}}			
		}


		#endregion datagrid


		private void btn_ok_Click(object sender, System.EventArgs e)
		{
		
			Response.Write("<script>window.close();</script>");
			Session.Remove("Dg_storiaObj");
			Session.Remove("Dg_storiaCorr");
            Session.Remove("Dg_storiaData");
            Session.Remove("Dg_storiaCampiProfilati");
		}

		private void DO_BindGrid(DocsPAWA.DocsPaWR.DocumentoStoricoMittente[] list, string tipo)
		{
			//Rendiamo invisibile il primo DG
			DataGrid1.Visible = false;
            Datagrid2.Visible = false;

			DataTable dtCorrSto = new DataTable("CORRISPONDENTI");
			DataColumn _c1 = new DataColumn("DATA");
			DataColumn _c2 = new DataColumn("RUOLO");
			DataColumn _c3 = new DataColumn("UTENTE");
			DataColumn _c4 = new DataColumn("MODIFICA");

			dtCorrSto.Columns.Add(_c1);
			dtCorrSto.Columns.Add(_c2);
			dtCorrSto.Columns.Add(_c3);
			dtCorrSto.Columns.Add(_c4);

            if (tipo == "dest" || tipo == "mitMultipli")
			{
				list = DO_AggregaCorrSto(list);
			}

			foreach(DocsPAWA.DocsPaWR.DocumentoStoricoMittente sm in list)
			{
				if(sm!= null)
				{
					dtCorrSto.Rows.Add(corr2DataRow(sm,dtCorrSto));
				}
			}

			dg_CorrSto.DataSource = dtCorrSto;
			Session["Dg_storiaCorr"] = dtCorrSto;
			dg_CorrSto.DataBind();
			DataGrid1.Visible = false;

		}

		private DocsPAWA.DocsPaWR.DocumentoStoricoMittente[] DO_AggregaCorrSto(DocsPAWA.DocsPaWR.DocumentoStoricoMittente[] list)
		{
			DocsPaWR.DocumentoStoricoMittente[] newList = new DocsPAWA.DocsPaWR.DocumentoStoricoMittente[list.Length];
			try
			{
				for(int i = 0;i <list.Length;i++)
				{
					DocsPaWR.DocumentoStoricoMittente sm = new DocsPAWA.DocsPaWR.DocumentoStoricoMittente();

					sm = (DocsPAWA.DocsPaWR.DocumentoStoricoMittente)list[i];
					if(sm != null)
					{
						for(int j = 0;j<list.Length;j++)
						{
							if(list[j] != null)
							{
								if(sm.dataModifica == ((DocsPAWA.DocsPaWR.DocumentoStoricoMittente)list[j]).dataModifica && sm.descrizione != ((DocsPAWA.DocsPaWR.DocumentoStoricoMittente)list[j]).descrizione)
								{
									sm.descrizione += "; "+((DocsPAWA.DocsPaWR.DocumentoStoricoMittente)list[j]).descrizione;
									list[j] = null;
								}
							}
						}
						newList[i] = sm;
					}
				}
			}
			catch(Exception ex)
			{
				ErrorManager.redirectToErrorPage(this,ex);
				newList = null;
			}
			return newList;
		}

				
		private DataRow corr2DataRow(DocsPAWA.DocsPaWR.DocumentoStoricoMittente sm,DataTable dt)
		{
			DataRow dr = dt.NewRow();
			string[] tmpItemArray = new string[4];
			//DATA
			tmpItemArray[0] = sm.dataModifica;
			//RUOLO
			tmpItemArray[1] = ((DocsPAWA.DocsPaWR.Ruolo)sm.ruolo).descrizione;
			//UTENTE
			tmpItemArray[2] = ((DocsPAWA.DocsPaWR.Utente)sm.utente).descrizione;
			//MODIFICA --Elenco Destinatari
			tmpItemArray[3] = sm.descrizione;
			dr.ItemArray = tmpItemArray;

			return dr;
		}

        protected void dg_CorrSto_ItemCreated(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Pager)
            {

                if (e.Item.Cells.Count > 0)
                {

                    e.Item.Cells[0].Attributes.Add("colspan", e.Item.Cells[0].ColumnSpan.ToString());

                }

            }


        }

        

        private void DO_BindGridData(DocsPAWA.DocsPaWR.DocumentoStoricoDataArrivo[] list)
        {
            //Rendiamo invisibile le due DG
            DataGrid1.Visible = false;
            dg_CorrSto.Visible = false;

           
            DataTable dtdate = new DataTable("DATA ARRIVO");
            DataColumn _c1 = new DataColumn("DATA MODIFICA");
            DataColumn _c2 = new DataColumn("RUOLO");
            DataColumn _c3 = new DataColumn("UTENTE");
            DataColumn _c4 = new DataColumn("DATA ARRIVO");
            



            dtdate.Columns.Add(_c1);
            dtdate.Columns.Add(_c2);
            dtdate.Columns.Add(_c3);
            dtdate.Columns.Add(_c4);


            foreach (DocsPAWA.DocsPaWR.DocumentoStoricoDataArrivo sm in list)
            {
                if (sm != null)
                {
                    dtdate.Rows.Add(data2DataRow(sm, dtdate));
                }
            }

            Datagrid2.DataSource = dtdate;
            Session["Dg_storiaData"] = dtdate;
            Datagrid2.DataBind();
            DataGrid1.Visible = false;
            dg_CorrSto.Visible = false;

        }

        private DataRow data2DataRow(DocsPAWA.DocsPaWR.DocumentoStoricoDataArrivo sm, DataTable dt)
        {
            DataRow dr = dt.NewRow();
            string[] tmpItemArray = new string[4];

            tmpItemArray[0] = sm.dataModifica;
            //RUOLO
            tmpItemArray[1] = ((DocsPAWA.DocsPaWR.Ruolo)sm.ruolo).descrizione;
            //UTENTE
            tmpItemArray[2] = ((DocsPAWA.DocsPaWR.Utente)sm.utente).descrizione;
            //MODIFICA --Elenco date
            tmpItemArray[3] = sm.dta_arrivo;
            
            dr.ItemArray = tmpItemArray;

            return dr;
        }

        private void DO_BindGridStoricoProfilati(DocsPaWR.StoricoProfilati[] storico)
        {
            //preparo la tabella contenente lo storico
            DataTable profilazione = new DataTable("STORICO PROFILAZIONE");
            DataColumn _c1 = new DataColumn("DATA MODIFICA");
            DataColumn _c2 = new DataColumn("UTENTE");
            DataColumn _c3 = new DataColumn("RUOLO");
            DataColumn _c4 = new DataColumn("CAMPO");
            DataColumn _c5 = new DataColumn("MODIFICA");
            profilazione.Columns.Add(_c1);
            profilazione.Columns.Add(_c2);
            profilazione.Columns.Add(_c3);
            profilazione.Columns.Add(_c4);
            profilazione.Columns.Add(_c5);
            for (int i = 0; i < storico.Length; i++)
            {
                DataRow dr = profilazione.NewRow();
                string[] tmpItemArray = new string[5];
                tmpItemArray[0] = storico[i].dta_modifica;
                tmpItemArray[1] = storico[i].utente.descrizione;
                tmpItemArray[2] = storico[i].ruolo.descrizione;
                tmpItemArray[3] = storico[i].oggetto.DESCRIZIONE;
                //se si tratta di un campo di tipo casella di selezione(quindi possono essere selezionati più valori) eseguo uno split per ottenere i diversi valori coinvolti nella modifica
                if (storico[i].oggetto.TIPO.DESCRIZIONE_TIPO.Equals("CasellaDiSelezione") && (!String.IsNullOrEmpty(storico[i].oggetto.TIPO.DESCRIZIONE_TIPO)))
                {
                    string[] delimiter = { "*#?" };
                    string[] val = storico[i].var_desc_modifica.Split(delimiter, System.StringSplitOptions.None);
                    for (int x = 0; x < val.Length; x++)
                        tmpItemArray[4] += val[x] + "<br/>";
                }
                else
                    tmpItemArray[4] = storico[i].var_desc_modifica;
                dr.ItemArray = tmpItemArray;
                profilazione.Rows.Add(dr.ItemArray);
            }
            //metto come datasource del datagrid associato allo storico dei campi profilati la nostra dt 'profilo'
            DataGridStoricoCampiProf.DataSource = profilazione;
            Session["Dg_storiaCampiProfilati"] = profilazione;
            DataGridStoricoCampiProf.DataBind();    
        }

        private void DataGridStoricoCampiProf_pager(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
        {
            this.DataGridStoricoCampiProf.CurrentPageIndex = e.NewPageIndex;
            if (Session["Dg_storiaCampiProfilati"] != null)
            {
                DataTable TDNew = (DataTable)Session["Dg_storiaCampiProfilati"];
                DataGridStoricoCampiProf.DataSource = TDNew;
                DataGridStoricoCampiProf.DataBind();
            }
        }

	}
}
