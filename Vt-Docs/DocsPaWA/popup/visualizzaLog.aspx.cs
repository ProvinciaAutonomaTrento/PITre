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
	/// Summary description for visualizzaLog.
	/// </summary>
	public class visualizzaLog : DocsPAWA.CssPage
	{
		// protected System.Web.UI.WebControls.Button btn_ok;
		protected System.Web.UI.WebControls.DataGrid DataGrid1;
		protected System.Web.UI.WebControls.Label Label2;
		protected DocsPAWA.dataSet.DataSetStoriaObj dataSetStoriaObj1;
		protected System.Web.UI.WebControls.Label lb_dettagli;

		//my var
		protected ArrayList Dt_elem;
		protected DocsPAWA.DocsPaWR.SchedaDocumento schedaDocumento;
        protected DocsPAWA.DocsPaWR.DocumentoLogDocumento[] ListaLogDocumento;
        protected DocsPAWA.DocsPaWR.DocumentoLogDocumento[] ListaLogFascicolo;
        protected DocsPAWA.DocsPaWR.DocumentoLogDocumento[] ListaLogFolder;
        protected System.Web.UI.WebControls.DataGrid dg_CorrSto;
		protected System.Web.UI.HtmlControls.HtmlGenericControl div_storicoOggetto;
		protected System.Web.UI.HtmlControls.HtmlGenericControl div_storicoCorr;
		string tipoOggetto;
        public DocsPAWA.DocsPaWR.Fascicolo Fasc;
        public DocsPAWA.DocsPaWR.Folder Folder;
        protected System.Web.UI.WebControls.Button Btn_ok;
        protected System.Web.UI.WebControls.ImageButton ibRoleHistory;
	
		private void Page_Load(object sender, System.EventArgs e)
		{
            tipoOggetto = Request.QueryString["tipo"];

            if (!IsPostBack)
			{
                Btn_ok.Attributes.Add("onClick", "javascript:window.close();");
                if (tipoOggetto.Equals("D"))
                {
                    schedaDocumento = (DocsPAWA.DocsPaWR.SchedaDocumento)DocumentManager.getDocumentoSelezionato(this);
                    if (schedaDocumento == null)
                    {
                        this.lb_dettagli.Text = "Errore nel reperimento dei dati del documento";
                        this.lb_dettagli.Visible = true;
                        return;
                    }

                    // Se l'id del ruolo creatore del documento risulta storicizzato, viene mostrato un pulsante che mostra
                    // la storia delle modifiche del ruolo
                    if (Utils.CheckIfCreatorRoleIsDisabled(schedaDocumento))
                    {
                        this.ibRoleHistory.Visible = true;
                        this.ibRoleHistory.OnClientClick = popup.RoleHistory.GetScriptToOpenWindow(schedaDocumento.creatoreDocumento.idCorrGlob_Ruolo, schedaDocumento.creatoreDocumento.idCorrGlob_Ruolo);
                    }
                    else
                        this.ibRoleHistory.Visible = false;


                    if(schedaDocumento.tipoProto.Equals("G"))
                        this.Label2.Text = "Storia del documento " + schedaDocumento.docNumber;
                    else
                        if(schedaDocumento.protocollo != null && schedaDocumento.protocollo.segnatura != null && !schedaDocumento.protocollo.segnatura.Equals(""))
                            this.Label2.Text = "Storia del documento " + schedaDocumento.protocollo.segnatura;
                        else
                            this.Label2.Text = "Storia del documento " + schedaDocumento.docNumber;
                    ListaLogDocumento = DocumentManager.getStoriaLog(this, schedaDocumento.systemId, "DOCUMENTO");
                    if (ListaLogDocumento == null || (ListaLogDocumento != null && ListaLogDocumento.Length <= 0))
                    {
                        DataGrid1.Visible = false;
                        this.lb_dettagli.Text = "Non ci sono attività che riguardano il documento in esame"; ;
                        this.lb_dettagli.Visible = true;
                    }
                    else
                    {
                        string data;
                        string utente;
                        string idPeopleOPeratore;
                        string idGruppoOperatore;
                        string idAmm;
                        string modificaObj;
                        string codAzione;
                        string ruolo;
                        string chaEsito;
                        for (int i = 0; i < ListaLogDocumento.Length; i++)
                        {
                            data = ((DocsPAWA.DocsPaWR.DocumentoLogDocumento)ListaLogDocumento[i]).dataAzione;
                            data = data.Replace(".", ":");
                            utente = ((DocsPAWA.DocsPaWR.DocumentoLogDocumento)ListaLogDocumento[i]).userIdOperatore;
                            idPeopleOPeratore = ((DocsPAWA.DocsPaWR.DocumentoLogDocumento)ListaLogDocumento[i]).idPeopleOPeratore;
                            ruolo = ((DocsPAWA.DocsPaWR.DocumentoLogDocumento)ListaLogDocumento[i]).idGruppoOperatore;
                            idAmm = ((DocsPAWA.DocsPaWR.DocumentoLogDocumento)ListaLogDocumento[i]).idAmm;
                            modificaObj = ((DocsPAWA.DocsPaWR.DocumentoLogDocumento)ListaLogDocumento[i]).descrOggetto;
                            codAzione = ((DocsPAWA.DocsPaWR.DocumentoLogDocumento)ListaLogDocumento[i]).codAzione;
                            chaEsito = ((DocsPAWA.DocsPaWR.DocumentoLogDocumento)ListaLogDocumento[i]).chaEsito;
                            utente += " (" + ruolo + ")";
                            //if (ruolo.Equals("1"))
                            //    ruolo = "OK";
                            //else
                            //    ruolo = "KO";
                            if(chaEsito.Equals("1"))
                                this.dataSetStoriaObj1.element1.Addelement1Row(data, ruolo, utente, modificaObj);
                        }
                        Session["Dg_Log"] = this.dataSetStoriaObj1.Tables[0];
                        this.DataGrid1.DataSource = this.dataSetStoriaObj1.Tables[0];
                        this.DataGrid1.DataBind();
                    }
                }
                else
                {
                    Fasc = FascicoliManager.getFascicoloSelezionato(this);
                    Folder = FascicoliManager.getFolder(this, Fasc);
                    if (Fasc == null)
                    {
                        this.lb_dettagli.Text = "Errore nel reperimento dei dati del fascicolo";
                        this.lb_dettagli.Visible = true;
                        return;
                    }

                    // Se l'id del ruolo creatore del documento risulta storicizzato, viene mostrato un pulsante che mostra
                    // la storia delle modifiche del ruolo
                    if (Utils.CheckIfCreatorRoleIsDisabled(Fasc))
                    {
                        this.ibRoleHistory.Visible = true;
                        this.ibRoleHistory.OnClientClick = popup.RoleHistory.GetScriptToOpenWindow(Fasc.creatoreFascicolo.idCorrGlob_Ruolo, Fasc.creatoreFascicolo.idCorrGlob_Ruolo);
                    }
                    else
                        this.ibRoleHistory.Visible = false;

                    this.Label2.Text = "Storia del fascicolo " + Fasc.codice;
                    ListaLogFascicolo = FascicoliManager.getStoriaLog(this, Fasc.systemID, "FASCICOLO");
                    ListaLogFolder = FascicoliManager.getStoriaLog(this, Folder.systemID, "FOLDER");

                    if ((ListaLogFascicolo == null || (ListaLogFascicolo != null && ListaLogFascicolo.Length <= 0)) &&
                        (ListaLogFolder == null || (ListaLogFolder != null && ListaLogFolder.Length <= 0)))
                    {
                        DataGrid1.Visible = false;
                        this.lb_dettagli.Text = "Non ci sono attività che riguardano il fascicolo in esame"; ;
                        this.lb_dettagli.Visible = true;
                    }
                    else
                    {
                        string data;
                        string utente;
                        string idPeopleOPeratore;
                        string idGruppoOperatore;
                        string idAmm;
                        string modificaObj;
                        string codAzione;
                        string ruolo;
                        string chaEsito;
                        for (int i = 0; i < ListaLogFascicolo.Length; i++)
                        {
                            data = ((DocsPAWA.DocsPaWR.DocumentoLogDocumento)ListaLogFascicolo[i]).dataAzione;
                            data = data.Replace(".", ":");
                            utente = ((DocsPAWA.DocsPaWR.DocumentoLogDocumento)ListaLogFascicolo[i]).userIdOperatore;
                            idPeopleOPeratore = ((DocsPAWA.DocsPaWR.DocumentoLogDocumento)ListaLogFascicolo[i]).idPeopleOPeratore;
                            ruolo = ((DocsPAWA.DocsPaWR.DocumentoLogDocumento)ListaLogFascicolo[i]).idGruppoOperatore;
                            idAmm = ((DocsPAWA.DocsPaWR.DocumentoLogDocumento)ListaLogFascicolo[i]).idAmm;
                            modificaObj = ((DocsPAWA.DocsPaWR.DocumentoLogDocumento)ListaLogFascicolo[i]).descrOggetto;
                            codAzione = ((DocsPAWA.DocsPaWR.DocumentoLogDocumento)ListaLogFascicolo[i]).codAzione;
                            chaEsito = ((DocsPAWA.DocsPaWR.DocumentoLogDocumento)ListaLogFascicolo[i]).chaEsito;
                            utente += " (" + ruolo + ")";
                            //if (ruolo.Equals("1"))
                            //    ruolo = "OK";
                            //else
                            //    ruolo = "KO";
                            if(chaEsito.Equals("1"))
                                this.dataSetStoriaObj1.element1.Addelement1Row(data, ruolo, utente, modificaObj);
                        }

                        for (int i = 0; i < ListaLogFolder.Length; i++)
                        {
                            data = ((DocsPAWA.DocsPaWR.DocumentoLogDocumento)ListaLogFolder[i]).dataAzione;
                            data = data.Replace(".", ":");
                            utente = ((DocsPAWA.DocsPaWR.DocumentoLogDocumento)ListaLogFolder[i]).userIdOperatore;
                            idPeopleOPeratore = ((DocsPAWA.DocsPaWR.DocumentoLogDocumento)ListaLogFolder[i]).idPeopleOPeratore;
                            ruolo = ((DocsPAWA.DocsPaWR.DocumentoLogDocumento)ListaLogFolder[i]).idGruppoOperatore;
                            idAmm = ((DocsPAWA.DocsPaWR.DocumentoLogDocumento)ListaLogFolder[i]).idAmm;
                            modificaObj = ((DocsPAWA.DocsPaWR.DocumentoLogDocumento)ListaLogFolder[i]).descrOggetto;
                            codAzione = ((DocsPAWA.DocsPaWR.DocumentoLogDocumento)ListaLogFolder[i]).codAzione;
                            chaEsito = ((DocsPAWA.DocsPaWR.DocumentoLogDocumento)ListaLogFolder[i]).chaEsito;
                            utente += " (" + ruolo + ")";
                            //if (ruolo.Equals("1"))
                            //    ruolo = "OK";
                            //else
                            //    ruolo = "KO";
                            if(chaEsito.Equals("1"))
                                this.dataSetStoriaObj1.element1.Addelement1Row(data, ruolo, utente, modificaObj);
                        }

                        Session["Dg_Log"] = this.dataSetStoriaObj1.Tables[0];
                        this.DataGrid1.DataSource = this.dataSetStoriaObj1.Tables[0];
                        this.DataGrid1.DataBind();
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
			//this.DataGrid1.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.Datagrid1_pager);
			//this.dg_CorrSto.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.dg_CorrSto_PageIndexChanged);
            //this.btn_ok.Click += new System.EventHandler(this.btn_ok_Click);
			// 
			// dataSetStoriaObj1
			// 
			//this.dataSetStoriaObj1.DataSetName = "DataSetStoriaObj";
			//this.dataSetStoriaObj1.Locale = new System.Globalization.CultureInfo("en-US");
			this.Load += new System.EventHandler(this.Page_Load);
			((System.ComponentModel.ISupportInitialize)(this.dataSetStoriaObj1)).EndInit();
           // this.Btn_ok.Click += new System.EventHandler(this.Btn_ok_Click);

		}
		#endregion

        //private void Btn_ok_Click(object sender, System.EventArgs e)
        //{
        //    Response.Write("<script>window.close();</script>");
        //}

		#region datagrid

		private void Datagrid1_pager(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
		{
			this.DataGrid1.CurrentPageIndex=e.NewPageIndex;
			if(Session["Dg_Log"]!=null)
			{
				DataTable TDNew=(DataTable) Session["Dg_Log"];			
				DataGrid1.DataSource=TDNew; 						
				DataGrid1.DataBind();
			}
		}

        protected void DataGrid1_PageIndexChanged(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
		{
            this.DataGrid1.CurrentPageIndex = e.NewPageIndex;
			if(Session["Dg_Log"]!=null)
			{
				DataTable DataTableNew=(DataTable) Session["Dg_Log"];
                DataGrid1.DataSource = DataTableNew;
                DataGrid1.DataBind();
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


        //private void btn_ok_Click(object sender, System.EventArgs e)
        //{
        //    Response.Write("<script>this.window.close();</script>");
        //    Session.Remove("Dg_Log");
        //    //Session.Remove("Dg_storiaCorr");
        //}

		private void DO_BindGrid(DocsPAWA.DocsPaWR.DocumentoStoricoMittente[] list, string tipo)
		{
			//Rendiamo invisibile il primo DG
			DataGrid1.Visible = false;

			DataTable dtCorrSto = new DataTable("CORRISPONDENTI");
			DataColumn _c1 = new DataColumn("DATA");
			DataColumn _c2 = new DataColumn("RUOLO");
			DataColumn _c3 = new DataColumn("UTENTE");
			DataColumn _c4 = new DataColumn("MODIFICA");

			dtCorrSto.Columns.Add(_c1);
			dtCorrSto.Columns.Add(_c2);
			dtCorrSto.Columns.Add(_c3);
			dtCorrSto.Columns.Add(_c4);

            //if(tipo == "dest")
            //{
            //    list = DO_AggregaCorrSto(list);
            //}

            //foreach(DocsPAWA.DocsPaWR.DocumentoStoricoMittente sm in list)
            //{
            //    if(sm!= null)
            //    {
            //        dtCorrSto.Rows.Add(corr2DataRow(sm,dtCorrSto));
            //    }
            //}

			dg_CorrSto.DataSource = dtCorrSto;
			Session["Dg_Log"] = dtCorrSto;
			dg_CorrSto.DataBind();
			DataGrid1.Visible = false;
		}

        //private DocsPAWA.DocsPaWR.DocumentoStoricoMittente[] DO_AggregaCorrSto(DocsPAWA.DocsPaWR.DocumentoStoricoMittente[] list)
        //{
        //    DocsPaWR.DocumentoStoricoMittente[] newList = new DocsPAWA.DocsPaWR.DocumentoStoricoMittente[list.Length];
        //    try
        //    {
        //        for(int i = 0;i <list.Length;i++)
        //        {
        //            DocsPaWR.DocumentoStoricoMittente sm = new DocsPAWA.DocsPaWR.DocumentoStoricoMittente();

        //            sm = (DocsPAWA.DocsPaWR.DocumentoStoricoMittente)list[i];
        //            if(sm != null)
        //            {
        //                for(int j = 0;j<list.Length;j++)
        //                {
        //                    if(list[j] != null)
        //                    {
        //                        if(sm.dataModifica == ((DocsPAWA.DocsPaWR.DocumentoStoricoMittente)list[j]).dataModifica && sm.descrizione != ((DocsPAWA.DocsPaWR.DocumentoStoricoMittente)list[j]).descrizione)
        //                        {
        //                            sm.descrizione += "; "+((DocsPAWA.DocsPaWR.DocumentoStoricoMittente)list[j]).descrizione;
        //                            list[j] = null;
        //                        }
        //                    }
        //                }
        //                newList[i] = sm;
        //            }
        //        }
        //    }
        //    catch(Exception ex)
        //    {
        //        ErrorManager.redirectToErrorPage(this,ex);
        //        newList = null;
        //    }
        //    return newList;
        //}

				
        //private DataRow corr2DataRow(DocsPAWA.DocsPaWR.DocumentoStoricoMittente sm,DataTable dt)
        //{
        //    DataRow dr = dt.NewRow();
        //    string[] tmpItemArray = new string[4];
        //    //DATA
        //    tmpItemArray[0] = sm.dataModifica;
        //    //RUOLO
        //    tmpItemArray[1] = ((DocsPAWA.DocsPaWR.Ruolo)sm.ruolo).descrizione;
        //    //UTENTE
        //    tmpItemArray[2] = ((DocsPAWA.DocsPaWR.Utente)sm.utente).descrizione;
        //    //MODIFICA --Elenco Destinatari
        //    tmpItemArray[3] = sm.descrizione;
        //    dr.ItemArray = tmpItemArray;

        //    return dr;
        //}

        //protected void dg_CorrSto_ItemCreated(object sender, DataGridItemEventArgs e)
        //{
        //    if (e.Item.ItemType == ListItemType.Pager)
        //    {
        //        if (e.Item.Cells.Count > 0)
        //        {
        //            e.Item.Cells[0].Attributes.Add("colspan", e.Item.Cells[0].ColumnSpan.ToString());
        //        }
        //    }
        //}
	}
}
