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
	/// Summary description for trasmissioniXRisp.
	/// </summary>
    public class trasmissioniXRisp : DocsPAWA.CssPage
	{
		protected System.Web.UI.WebControls.DataGrid Datagrid2;

		protected DocsPAWA.DocsPaWR.Trasmissione[] listaTrasmissioni;
		protected DocsPAWA.DocsPaWR.SchedaDocumento schedaDocumento;
		protected DocsPAWA.DocsPaWR.TrasmissioneOggettoTrasm oggettoTrasm;
		protected DocsPAWA.DocsPaWR.Ruolo userRuolo;
		protected System.Web.UI.WebControls.Table tbl_trasmRic;
		protected System.Web.UI.WebControls.Label lbl_message;
		protected System.Web.UI.WebControls.Button btn_ok;
		protected System.Web.UI.WebControls.Button btn_chiudi;
		protected DocsPAWA.dataSet.DataSetTrasmXRisp dataSetTrasmXRisp1;
		protected DocsPAWA.DocsPaWR.Utente userHome;
		protected DataView dv;
	
		private void Page_Load(object sender, System.EventArgs e)
		{
			// Put user code to initialize the page here
			try
			{
				schedaDocumento = DocumentManager.getDocumentoSelezionato(this);
				userHome = UserManager.getUtente(this);
				userRuolo =  UserManager.getRuolo(this);
				if (schedaDocumento != null)
				{
					if(!IsPostBack)
					{
						queryTrasmissioni();
						for(int i= 0; i< listaTrasmissioni.Length ; i++)
						{
							CaricaDataGrid((DocsPAWA.DocsPaWR.Trasmissione)listaTrasmissioni[i],i);
						}
						
						if (listaTrasmissioni.Length>0)
						{
							this.Datagrid2.DataSource=this.dataSetTrasmXRisp1;
							Session["dataSetTrasmXrisp"]=this.dataSetTrasmXRisp1;
							dv=this.dataSetTrasmXRisp1.Tables[0].DefaultView;
							this.Datagrid2.DataBind();
						}
						else
						{
							this.lbl_message.Text = "Non sono state trovate trasmissioni";
							this.btn_ok.Visible = false;
						}
					}
				}
			}
			catch (Exception ex) 
			{
				ErrorManager.redirect(this, ex);
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
			this.dataSetTrasmXRisp1 = new DocsPAWA.dataSet.DataSetTrasmXRisp();
			((System.ComponentModel.ISupportInitialize)(this.dataSetTrasmXRisp1)).BeginInit();
			this.Datagrid2.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.DataGrid2_OnPageIndexChanged);
			this.Datagrid2.PreRender += new System.EventHandler(this.Datagrid2_PreRender);
			this.Datagrid2.SortCommand += new System.Web.UI.WebControls.DataGridSortCommandEventHandler(this.Datagrid2_SortCommand);
			this.Datagrid2.SelectedIndexChanged += new System.EventHandler(this.DataGrid2_SelectedIndexChanged);
			this.btn_ok.Click += new System.EventHandler(this.btn_ok_Click);
			this.btn_chiudi.Click += new System.EventHandler(this.btn_chiudi_Click);
			// 
			// dataSetTrasmXRisp1
			// 
			this.dataSetTrasmXRisp1.DataSetName = "DataSetTrasmXRisp";
			this.dataSetTrasmXRisp1.Locale = new System.Globalization.CultureInfo("en-US");
			this.dataSetTrasmXRisp1.Namespace = "http://tempuri.org/DataSetTrasmXRisp.xsd";
			this.Load += new System.EventHandler(this.Page_Load);
			((System.ComponentModel.ISupportInitialize)(this.dataSetTrasmXRisp1)).EndInit();

		}
		#endregion

		#region datagrid

		protected void queryTrasmissioni()
		{
			try
			{	
				oggettoTrasm = new DocsPAWA.DocsPaWR.TrasmissioneOggettoTrasm();
				oggettoTrasm.infoDocumento = DocumentManager.getInfoDocumento(schedaDocumento);

				DocsPaWR.FiltroRicerca[] listaFiltri = new DocsPAWA.DocsPaWR.FiltroRicerca[1];
				DocsPaWR.FiltroRicerca filtroTrasm = new DocsPAWA.DocsPaWR.FiltroRicerca();

				filtroTrasm.argomento = DocsPAWA.DocsPaWR.FiltriTrasmissioneNascosti.IN_RISPOSTA.ToString();
				filtroTrasm.valore = "";
				
				listaFiltri[0] = filtroTrasm;
				listaTrasmissioni = TrasmManager.getQueryRicevute(this,oggettoTrasm, this.userHome,this.userRuolo,listaFiltri);
				//listaTrasmissioni = TrasmManager.getQueryRicevute(this,oggettoTrasm, this.userHome,this.userRuolo,null);

				TrasmManager.setDocTrasmQueryRic(this,listaTrasmissioni);
			}
			catch(System.Web.Services.Protocols.SoapException es) 
			{
				ErrorManager.redirect(this, es);
			} 
		}

		protected void CaricaDataGrid(DocsPAWA.DocsPaWR.Trasmissione trasm,int index)
		{
			try
			{
				DocsPaWR.TrasmissioneSingola trasmSing   = new DocsPAWA.DocsPaWR.TrasmissioneSingola();
				DocsPaWR.TrasmissioneUtente trasmUtente = new DocsPAWA.DocsPaWR.TrasmissioneUtente();
				DocsPaWR.TrasmissioneSingola[] listaTrasmSing;

				listaTrasmSing = trasm.trasmissioniSingole;
				string ragione = "";
				string dataScad = "";
				string oggetto = "";
				string segnatura ="";
				if (listaTrasmSing.Length > 0)
				{
					trasmSing = (DocsPAWA.DocsPaWR.TrasmissioneSingola) listaTrasmSing[0];
					ragione = trasmSing.ragione.descrizione;
					dataScad = trasmSing.dataScadenza;

					// riempio il campo oggetto
					// per doc. grigia       - oggetto + data
					// per doc. protocollata - oggetto + segnatura
//					if(schedaDocumento.protocollo == null)
//						segnatura = schedaDocumento.dataCreazione;
//					else
					if(trasm.infoDocumento!=null)
					{
						if((trasm.infoDocumento.numProt!=null) &&( !trasm.infoDocumento.numProt.Equals("")) )
							segnatura = trasm.infoDocumento.segnatura;
						else
							segnatura = trasm.infoDocumento.dataApertura;
				
						oggetto = trasm.infoDocumento.oggetto;
					}
					this.dataSetTrasmXRisp1.element1.Addelement1Row(trasm.dataInvio,trasm.utente.descrizione,trasm.ruolo.descrizione,ragione,dataScad,oggetto,index,segnatura);

				}
			}
			catch (Exception ex) 
			{
				ErrorManager.redirect(this, ex);
			}
		}
	

		private void DataGrid2_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			
			DocsPaWR.Trasmissione[] listaRic=TrasmManager.getDocTrasmQueryRic(this);

			string key=((Label)this.Datagrid2.Items[this.Datagrid2.SelectedIndex].Cells[6].Controls[1]).Text;
			
			//disegna tabella dei dettagli
			drawBorderRow();
			drawTableDettagli(listaRic[Int32.Parse(key)]);
			drawBorderRow();


		}

		private void Datagrid2_PreRender(object sender, System.EventArgs e)
		{
			
			for(int i=0;i<this.Datagrid2.Items.Count;i++)
			{
				if(this.Datagrid2.Items[i].ItemIndex>=0)
				{	
					switch(this.Datagrid2.Items[i].ItemType.ToString().Trim())
					{
						case "Item":
						{
							this.Datagrid2.Items[i].Attributes.Add("onmouseover","this.className='bg_grigioS'");
							this.Datagrid2.Items[i].Attributes.Add("onmouseout","this.className='bg_grigioN'");
							break;
						}
						case "AlternatingItem":
					
						{
							this.Datagrid2.Items[i].Attributes.Add("onmouseover","this.className='bg_grigioS'");
							this.Datagrid2.Items[i].Attributes.Add("onmouseout","this.className='bg_grigioA'");
							break;
						}
				
					}
				}
			}
		
	
		}

		private void DataGrid2_OnPageIndexChanged(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
		{
			this.Datagrid2.CurrentPageIndex=e.NewPageIndex;
			this.Datagrid2.DataSource=(DocsPAWA.dataSet.DataSetTrasmXRisp)Session["dataSetTrasmXrisp"];
			this.Datagrid2.DataBind();
			//cancella il contenuto della tabella dei dettagli
			this.tbl_trasmRic.Rows.Clear();
		}


		
		#endregion

		#region disegna tabella dettagli
		private void drawBorderRow()
		{
			//riga separatrice(color rosa)
			TableRow tr=new TableRow();
			TableCell tc = new TableCell();
			tr.CssClass="bg_grigioS";
			tc.ColumnSpan=4;
			
			tc.Width=Unit.Percentage(100);
			tc.Height=Unit.Pixel(10);
			tr.Cells.Add(tc);
			//Aggiungo Row alla table1
			this.tbl_trasmRic.Rows.Add(tr);
		}
		
		protected void drawTableDettagli(DocsPAWA.DocsPaWR.Trasmissione trasmRic) 
		{
			//proprietà tabella			
			this.tbl_trasmRic.CssClass="testo_grigio";
			this.tbl_trasmRic.CellPadding=1;
			this.tbl_trasmRic.CellSpacing=1;
			this.tbl_trasmRic.BorderWidth=1;
			this.tbl_trasmRic.BackColor=Color.FromArgb(255,255,255);

			//Instanzio l'oggetto trasmissione singola e trasmissione utente
			DocsPaWR.TrasmissioneSingola trasmSing   = new DocsPAWA.DocsPaWR.TrasmissioneSingola();
			DocsPaWR.TrasmissioneUtente  trasmUtente = new DocsPAWA.DocsPaWR.TrasmissioneUtente();
			DocsPaWR.TrasmissioneSingola[] listaTrasmSing;
			DocsPaWR.TrasmissioneUtente[] listaTrasmUtente;

			listaTrasmSing = trasmRic.trasmissioniSingole;
			if (listaTrasmSing.Length > 0 ) 
			{
				trasmSing = (DocsPAWA.DocsPaWR.TrasmissioneSingola) listaTrasmSing[0];
				listaTrasmUtente = trasmSing.trasmissioneUtente;
				if (listaTrasmUtente.Length > 0)
					trasmUtente = (DocsPAWA.DocsPaWR.TrasmissioneUtente) trasmSing.trasmissioneUtente[0];
			}
				
			//crea tr e td
			TableRow triga=new TableRow();
			TableCell tcell=new TableCell();
			string txt_val;
			
			//1 - 2 RIGA CON LE NOTE 
			triga=new TableRow();
			triga.Height=15;
			triga.BackColor=Color.FromArgb(149,149,149);
			//testo note generali
			tcell=new TableCell();
			tcell.CssClass="menu_1_bianco";
			tcell.Width=Unit.Percentage(20);
			tcell.Text="Note generali";
			triga.Cells.Add(tcell);
			//val note generali
			tcell=new TableCell();
			tcell.CssClass="testo_grigio";
			tcell.BackColor = Color.FromArgb(242,242,242);
			tcell.Width=Unit.Percentage(80);
			tcell.ColumnSpan = 3;
			tcell.Text=trasmRic.noteGenerali;
			triga.Cells.Add(tcell);
			this.tbl_trasmRic.Rows.Add(triga);


			triga=new TableRow();
			triga.Height=15;
			triga.BackColor=Color.FromArgb(149,149,149);
			//testo note individuali
			tcell=new TableCell();
			tcell.CssClass="menu_1_bianco";
			tcell.Width=Unit.Percentage(20);
			tcell.Text="Note individuali";
			triga.Cells.Add(tcell);
			//val note individuali
			tcell=new TableCell();
			tcell.CssClass="testo_grigio";
			tcell.BackColor = Color.FromArgb(242,242,242);
			tcell.Width=Unit.Percentage(80);
			tcell.ColumnSpan = 3;

			txt_val = trasmSing.noteSingole;
			if (!(txt_val != null && !txt_val.Equals(""))) txt_val = "";
			tcell.Text= null;
			
			triga.Cells.Add(tcell);
			this.tbl_trasmRic.Rows.Add(triga);

			//3 RIGA DESTINATARIO
			
			triga=new TableRow();
			triga.Height=15;
			triga.BackColor=Color.FromArgb(149,149,149);
			//testo destinatario
			tcell=new TableCell();
			tcell.CssClass="menu_1_bianco";
			tcell.Width=Unit.Percentage(20);
			tcell.Text="Destinatario";
			triga.Cells.Add(tcell);
			//val destinatario
			tcell=new TableCell();
			tcell.CssClass="testo_grigio";
			tcell.BackColor = Color.FromArgb(242,242,242);
			tcell.Width=Unit.Percentage(80);
			tcell.ColumnSpan = 3;
			
			txt_val = trasmSing.corrispondenteInterno.descrizione;
			if (!(txt_val != null && !txt_val.Equals(""))) txt_val = "";
			tcell.Text= txt_val;
			triga.Cells.Add(tcell);

			//4° RIGA DATA VISTA - DATA RISPOSTA
			triga=new TableRow();
			triga.Height=15;
			triga.BackColor=Color.FromArgb(149,149,149);
			//testo data vista
			tcell=new TableCell();
			tcell.CssClass="menu_1_bianco";
			tcell.Width=Unit.Percentage(20);
			tcell.Text="Data Vista";
			triga.Cells.Add(tcell);
			//val data vista
			tcell=new TableCell();
			tcell.CssClass="testo_grigio";
			tcell.BackColor=Color.FromArgb(242,242,242);
			tcell.Width=Unit.Percentage(20);

			txt_val = trasmUtente.dataVista;
//			if (!(txt_val != null && !txt_val.Equals(""))) txt_val = "xx/xx/xxxx";
			tcell.Text=txt_val;
			
			triga.Cells.Add(tcell);

			//testo data risposta
			tcell=new TableCell();
			tcell.CssClass="menu_1_bianco";
			tcell.Width=Unit.Percentage(20);
			tcell.Text="Data Risposta";
			triga.Cells.Add(tcell);
			//val data risposta
			tcell=new TableCell();
			tcell.CssClass="testo_grigio";
			tcell.BackColor = Color.FromArgb(242,242,242);
			tcell.Width=Unit.Percentage(20);
			
			txt_val = trasmUtente.dataRisposta;  //da correggere
//			if (!(txt_val != null && !txt_val.Equals(""))) txt_val = "xx/xx/xxxx";
			tcell.Text=txt_val;

			triga.Cells.Add(tcell);
			this.tbl_trasmRic.Rows.Add(triga);

			//5° RIGA - DATA ACCETTAZ - DATA RIFIUTO
			triga=new TableRow();
			triga.Height=15;
			triga.BackColor=Color.FromArgb(149,149,149);
			//val data accettazione
			tcell=new TableCell();
			tcell.CssClass="menu_1_bianco";
			tcell.Width=Unit.Percentage(15);
			tcell.Text="Data Acc";
			triga.Cells.Add(tcell);
			//testo data accettazione
			tcell=new TableCell();
			tcell.CssClass="testo_grigio";
			tcell.BackColor = Color.FromArgb(242,242,242);
			tcell.Width=Unit.Percentage(20);

			txt_val = trasmUtente.dataAccettata;
//			if (!(txt_val != null && !txt_val.Equals(""))) txt_val = "xx/xx/xxxx";
			tcell.Text=txt_val;

			triga.Cells.Add(tcell);

			//val data rifiuto
			tcell=new TableCell();
			tcell.CssClass="menu_1_bianco";
			tcell.Width=Unit.Percentage(20);
			tcell.Text="Data Rif";
			triga.Cells.Add(tcell);
			//testo data rifiuto
			tcell=new TableCell();
			tcell.CssClass="testo_grigio";
			tcell.BackColor = Color.FromArgb(242,242,242);
			tcell.Width=Unit.Percentage(20);
			
			txt_val = trasmUtente.dataRifiutata;
//			if (!(txt_val != null && !txt_val.Equals(""))) txt_val = "xx/xx/xxxx";
			tcell.Text=txt_val;

			triga.Cells.Add(tcell);
			this.tbl_trasmRic.Rows.Add(triga);

			//6° RIGA - NOTE ACC/RIF

			triga=new TableRow();
			triga.Height=15;
			triga.BackColor=Color.FromArgb(149,149,149);
			//testo note acc/rif
			tcell=new TableCell();
			tcell.CssClass="menu_1_bianco";
			tcell.Width=Unit.Percentage(20);
			tcell.Text="Note Acc/Rif";
			triga.Cells.Add(tcell);
			//val note acc/rif
			tcell=new TableCell();
			tcell.CssClass="testo_grigio";
			tcell.BackColor = Color.FromArgb(242,242,242);
			tcell.Width=Unit.Percentage(80);
			tcell.ColumnSpan = 3;
			
			if (trasmUtente.dataRifiutata != null && !trasmUtente.dataRifiutata.Equals(""))	
				tcell.Text = trasmUtente.noteRifiuto;
			else
				if (trasmUtente.dataAccettata != null && !trasmUtente.dataAccettata.Equals(""))	
				tcell.Text = trasmUtente.noteAccettazione;
			

			triga.Cells.Add(tcell);
			this.tbl_trasmRic.Rows.Add(triga);
			
		}

		#endregion disegna tabella dettagli

		private void btn_ok_Click(object sender, System.EventArgs e)
		{
			try
			{
				int indice = this.Datagrid2.SelectedIndex;
				if (indice >=0)
				{
					string key=((Label)this.Datagrid2.Items[this.Datagrid2.SelectedIndex].Cells[6].Controls[1]).Text;

					if(Int32.Parse(key)>=0)
					{
						//Costruisce l'oggetto trasmissione singola da aggiungere alla trasmissione
						DocsPaWR.Trasmissione[] listaRic=TrasmManager.getDocTrasmQueryRic(this);
						DocsPaWR.Trasmissione trasmissione = TrasmManager.getGestioneTrasmissione(this);
						DocsPaWR.Trasmissione trasmSel;
						trasmSel = listaRic[Int32.Parse(key)];
						trasmissione = addTrasmissioneSingola(trasmissione,trasmSel);
						TrasmManager.setGestioneTrasmissione(this, trasmissione);
					}
					TrasmManager.removeDocTrasmQueryRic(this);
					Response.Write("<script>window.open('../trasmissione/trasmDatiTrasm_dx.aspx','iFrame_dx'); window.close();</script>");
			
				}
			} 
			catch (Exception ex) 
			{
				ErrorManager.redirect(this, ex);
			}
		}

		private DocsPAWA.DocsPaWR.Trasmissione addTrasmissioneSingola(DocsPAWA.DocsPaWR.Trasmissione trasmissione, DocsPAWA.DocsPaWR.Trasmissione trasmOrig)
		{
			DocsPaWR.Corrispondente corr;
			corr = trasmOrig.ruolo;

			if(trasmissione.trasmissioniSingole != null)
			{
				for(int i = 0; i < trasmissione.trasmissioniSingole.Length; i++) 
				{
					//il ruolo della trasmissione a cui sto rispondendo diventa il destinatario della trasmissione risposta
				
					DocsPaWR.TrasmissioneSingola ts = (DocsPAWA.DocsPaWR.TrasmissioneSingola)trasmissione.trasmissioniSingole[i];
					if (!ts.daEliminare && ts.corrispondenteInterno.systemId.Equals(corr.systemId))
						return trasmissione;
				}			
			}
			// Aggiungo la trasmissione singola
			DocsPaWR.TrasmissioneSingola trasmissioneSingola = new DocsPAWA.DocsPaWR.TrasmissioneSingola();
			trasmissioneSingola.tipoTrasm = "S";
			trasmissioneSingola.corrispondenteInterno = corr;
			trasmissioneSingola.tipoDest = DocsPAWA.DocsPaWR.TrasmissioneTipoDestinatario.RUOLO;
		
		
			trasmissioneSingola.ragione = TrasmManager.getRagioneSel(this);

			trasmissioneSingola.idTrasmUtente = getIdTrasmUtente(trasmOrig);

			//id della trasmissione utente alla quale si sta rispondendo
			DocsPaWR.TrasmissioneUtente trasmissioneUtente = new DocsPAWA.DocsPaWR.TrasmissioneUtente(); 
			trasmissioneUtente.utente = trasmOrig.utente;
			trasmissioneSingola.trasmissioneUtente = new DocsPAWA.DocsPaWR.TrasmissioneUtente[1];
			trasmissioneSingola.trasmissioneUtente[0] = trasmissioneUtente;
			

			trasmissione.trasmissioniSingole = Utils.addToArrayTrasmissioneSingola(trasmissione.trasmissioniSingole, trasmissioneSingola);
			
			return trasmissione;
		}

		
		private string getIdTrasmUtente(DocsPAWA.DocsPaWR.Trasmissione trasmOrigine) 
		{
			string idTrasmUtente = "";
			string idRuolo;

			// cerco all'interno della trasmissione Origine la trasmissione singola associata
			// al ruolo attualmente scelto dall'utente corrente
			for (int i = 0; i < trasmOrigine.trasmissioniSingole.Length; i++) 
			{
				DocsPaWR.TrasmissioneSingola ts = (DocsPAWA.DocsPaWR.TrasmissioneSingola)trasmOrigine.trasmissioniSingole[i];
				idRuolo = ts.corrispondenteInterno.systemId;
				//if (idRuolo.Equals(((DocsPAWA.DocsPaWR.Corrispondente)userRuolo).systemId) || idRuolo.Equals(((DocsPAWA.DocsPaWR.Corrispondente)userHome).systemId)) 
				{
					// Cerco all'interno della trasmissione singola la trasmissione utente
					// associata all'utente corrente
					for (int j = 0; j < ts.trasmissioneUtente.Length; j++) 
					{
						DocsPaWR.TrasmissioneUtente tu = (DocsPAWA.DocsPaWR.TrasmissioneUtente)ts.trasmissioneUtente[j];
						if (tu.utente.idPeople.Equals(userHome.idPeople))
							return tu.systemId;
					}
				}
			}
			return idTrasmUtente;
		}

		private void btn_chiudi_Click(object sender, System.EventArgs e)
		{
			try 
			{			
				TrasmManager.removeDocTrasmQueryRic(this);
				Response.Write("<script>window.close();</script>");
			} 
			catch (Exception ex) 
			{
				ErrorManager.redirect(this, ex);
			}
		}

		private void Datagrid2_SortCommand(object source, System.Web.UI.WebControls.DataGridSortCommandEventArgs e)
		{
			dv=((DocsPAWA.dataSet.DataSetTrasmXRisp)Session["dataSetTrasmXrisp"]).Tables[0].DefaultView;
				if(e.SortExpression.Equals("utente"))
				{
					dv.Sort = e.SortExpression+" DESC";
				
				}
				if(e.SortExpression.Equals("oggetto"))
				{
					dv.Sort=e.SortExpression;
				}
				Datagrid2.DataSource =dv;
				Datagrid2.DataBind();
			
		}


	}
}
