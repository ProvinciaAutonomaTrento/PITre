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

namespace DocsPAWA
{
	/// <summary>
	/// Summary description for RicercaTrasm_cn_Down.
	/// </summary>
	public class RisultatiRicTrasm : DocsPAWA.CssPage
	{

		//my var
		DocsPaWR.Trasmissione trasmSel;
		protected System.Web.UI.WebControls.Table tbl_trasmSel;
		string tipoTrasm;
        protected int red = 0;
        protected int green = 0;
        protected int blu = 0;

		private void Page_Load(object sender, System.EventArgs e)
		{
			try
			{
                // prelevo l'informazione del colore del tema dal DB
                string Tema = GetCssAmministrazione();
                string color = string.Empty;
                if (Tema != null && !Tema.Equals(""))
                {
                    string[] realTema = Tema.Split('^');
                    color = realTema[2];
                }
                else
                    color = "810d06";

                red = Convert.ToInt32(color.Substring(0, 2), 16);
                green = Convert.ToInt32(color.Substring(2, 2), 16);
                blu = Convert.ToInt32(color.Substring(4), 16);
                
                // Put user code to initialize the page here
				Utils.startUp(this);
				Page.Response.Expires = 0;

				//Leggo la trasmissione dalla sessione
				trasmSel = TrasmManager.getDocTrasmSel(this);;
				if (trasmSel != null)
				{
					//cerco il tipo di trasmissione
					tipoTrasm = Request.QueryString["tipoRic"];
					if (tipoTrasm == "R")
					{
						drawDettagliTrasmRic(trasmSel);
					} 
					else
						if (tipoTrasm == "E")
					{
						drawDettagliTrasmEff(trasmSel);
					}

				}
			}
			catch(System.Exception ex)
			{
				
				ErrorManager.redirect(this,ex);
			}
		}

		//  funzione per il disegno dei dettagli di una trasmissione Ricevuta
		protected void drawDettagliTrasmRic(DocsPAWA.DocsPaWR.Trasmissione trasmRic)
		{
			//proprietà tabella			
			this.tbl_trasmSel.CssClass="testo_grigio";
			this.tbl_trasmSel.CellPadding=1;
			this.tbl_trasmSel.CellSpacing=1;
			this.tbl_trasmSel.BorderWidth=1;
			this.tbl_trasmSel.BackColor=Color.FromArgb(255,255,255);

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
			//triga.BackColor=Color.FromArgb(149,149,149);
            triga.BackColor = Color.FromArgb(red, green, blu);
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
			this.tbl_trasmSel.Rows.Add(triga);


			triga=new TableRow();
			triga.Height=15;
			//triga.BackColor=Color.FromArgb(149,149,149);
            triga.BackColor = Color.FromArgb(red, green, blu);
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
			this.tbl_trasmSel.Rows.Add(triga);

			//3 RIGA DESTINATARIO
			
			triga=new TableRow();
			triga.Height=15;
			//triga.BackColor=Color.FromArgb(149,149,149);
            triga.BackColor = Color.FromArgb(red, green, blu);
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
			//triga.BackColor=Color.FromArgb(149,149,149);
            triga.BackColor = Color.FromArgb(red, green, blu);
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
			this.tbl_trasmSel.Rows.Add(triga);

			//5° RIGA - DATA ACCETTAZ - DATA RIFIUTO
			triga=new TableRow();
			triga.Height=15;
			//triga.BackColor=Color.FromArgb(149,149,149);
            triga.BackColor = Color.FromArgb(red, green, blu);
			//val data accettazione
			tcell=new TableCell();
			tcell.CssClass="menu_1_bianco";
			tcell.Width=Unit.Percentage(20);
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
			this.tbl_trasmSel.Rows.Add(triga);

			//6° RIGA - NOTE ACC/RIF

			triga=new TableRow();
			triga.Height=15;
			//triga.BackColor=Color.FromArgb(149,149,149);
            triga.BackColor = Color.FromArgb(red, green, blu);
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
			this.tbl_trasmSel.Rows.Add(triga);
			
		}

		protected void drawDettagliTrasmEff(DocsPAWA.DocsPaWR.Trasmissione trasmSel)
		{

			//prendo le trasm singole dalla Trasm
			DocsPaWR.TrasmissioneSingola[] listTrasmSing;
			listTrasmSing=trasmSel.trasmissioniSingole;
				

			//stampa delle note generali
			drawNoteTrasmEff(this.tbl_trasmSel,trasmSel);
			//stampa delle trasmissioni singole
			for(int g=0;g<listTrasmSing.Length;g++)
			{	
				DocsPaWR.TrasmissioneSingola trasmSing=(DocsPAWA.DocsPaWR.TrasmissioneSingola) listTrasmSing[g];	   	
				drawTableTrasmEff(this.tbl_trasmSel, trasmSing,g);
			}

		}



		protected void drawNoteTrasmEff(System.Web.UI.WebControls.Table tbl, DocsPAWA.DocsPaWR.Trasmissione trasm)
		{
			TableRow tr=new TableRow();
			TableCell tc=new TableCell();
			tc.CssClass="menu_1_bianco";
			tc.ColumnSpan=6;
			//tc.BackColor=Color.FromArgb(192,134,130);
            tc.BackColor = Color.FromArgb(red, green, blu);
			tc.Text="Note Generali";
			tc.HorizontalAlign=HorizontalAlign.Left;
			tc.Width=Unit.Percentage(100);
			tc.Height=Unit.Pixel(20);

			tr.Cells.Add(tc);

			tbl.Rows.Add(tr);

			tr=new TableRow();
			tc=new TableCell();
			tc.ColumnSpan=9;
			tc.CssClass="testo_grigio";
			tc.BackColor=Color.FromArgb(217,217,217);
			tc.Text=trasm.noteGenerali;
		
			tc.Width=Unit.Percentage(100);
			tc.Height=Unit.Pixel(40);

			tr.Cells.Add(tc);
			tr.Cells.Add(tc);
			tbl.Rows.Add(tr);		
		
		}

        private string GetCssAmministrazione()
        {
            string Tema = string.Empty;
            if ((string)Session["AMMDATASET"] != null)
            {
                string idAmm = AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "3");
                DocsPAWA.UserManager userM = new UserManager();
                Tema = userM.getCssAmministrazione(idAmm);
            }
            else
            {
                if (UserManager.getInfoUtente() != null)
                {
                    string idAmm = UserManager.getInfoUtente().idAmministrazione;
                    DocsPAWA.UserManager userM = new UserManager();
                    Tema = userM.getCssAmministrazione(idAmm);
                }
            }
            return Tema;
        }

		protected void drawTableTrasmEff(System.Web.UI.WebControls.Table tbl, DocsPAWA.DocsPaWR.TrasmissioneSingola trasmSing,int h)
		{
			
			//sto solo visualizzando le trasmissioni singole e utente
				
			this.drawHeaderTrasmSing(tbl);
			drawRowTxVisual(tbl, trasmSing,h);
		}
		
		protected void drawHeaderTrasmSing(System.Web.UI.WebControls.Table tbl)
		{
			// Put user code to initialize the page here
			TableRow tr=new TableRow();
			TableCell tc=new TableCell();
			tc.CssClass="menu_1_bianco";
			//tc.BackColor=Color.FromArgb(75,75,75);
            tc.BackColor = Color.FromArgb(red, green, blu);
			tc.Text="Destinatario";
			tc.ColumnSpan=2;
			tc.Width=Unit.Percentage(20);

			tr.Cells.Add(tc);

			tc=new TableCell();
			tc.CssClass="menu_1_bianco";
			//tc.BackColor=Color.FromArgb(75,75,75);
            tc.BackColor = Color.FromArgb(red, green, blu);
            tc.Text = "Ragione";
			tc.ColumnSpan=2;
			tc.Width=Unit.Percentage(20);

			tr.Cells.Add(tc);

			
			//Tipo c'è solo per determinate ragioni
			tc=new TableCell();
			tc.CssClass="menu_1_bianco";
			//tc.BackColor=Color.FromArgb(75,75,75);
            tc.BackColor = Color.FromArgb(red, green, blu);
            tc.Text = "Tipo";
			tc.Height=Unit.Pixel(15);
			tc.Width=Unit.Percentage(10);

			tr.Cells.Add(tc);

			tc=new TableCell();
			tc.CssClass="menu_1_bianco";
			//tc.BackColor=Color.FromArgb(75,75,75);
            tc.BackColor = Color.FromArgb(red, green, blu);
            tc.Text = "Note Individuali";
			tc.Height=Unit.Pixel(15);
			
			tc.Width=Unit.Percentage(30);
			tr.Cells.Add(tc);
	
			tc=new TableCell();
			tc.CssClass="menu_1_bianco";
			//tc.BackColor=Color.FromArgb(75,75,75);
            tc.BackColor = Color.FromArgb(red, green, blu);
            tc.Text = "Data scad.";
			
			tc.Height=Unit.Pixel(15);
			tc.Width=Unit.Percentage(10);

			tr.Cells.Add(tc);

			tc=new TableCell();
			tc.CssClass="menu_1_bianco";
			//tc.BackColor=Color.FromArgb(75,75,75);
            tc.BackColor = Color.FromArgb(red, green, blu);
            tc.Text = "In Risp.a";
			tc.Height=Unit.Pixel(15);
			tc.ColumnSpan=2;
			tc.Width=Unit.Percentage(10);

			tr.Cells.Add(tc);

			tbl.Rows.Add(tr);
		}
		//fine Trasm Singola 


		protected void drawHeaderTrasmUt(System.Web.UI.WebControls.Table tbl)
		{	
			TableRow tr=new TableRow();

			TableCell tc=new TableCell();
			tc.CssClass="menu_1_bianco";
			tc.BackColor=Color.FromArgb(255,255,255);
			tc.Text="&nbsp;";
			tc.Width=Unit.Percentage(5);

			tr.Cells.Add(tc);

			tc=new TableCell();
			tc.CssClass="menu_1_bianco";
			//tc.BackColor=Color.FromArgb(149,149,149);
            tc.BackColor = Color.FromArgb(red, green, blu);
			tc.Text="Utente";
			tc.Width=Unit.Percentage(15);

			tr.Cells.Add(tc);

			tc=new TableCell();
			tc.CssClass="menu_1_bianco";
			//tc.BackColor=Color.FromArgb(149,149,149);
            tc.BackColor = Color.FromArgb(red, green, blu);
            tc.Text = "data vista";
			tc.Width=Unit.Percentage(10);

			tr.Cells.Add(tc);

			
			//Tipo c'è solo per determinate ragioni
			tc=new TableCell();
			tc.CssClass="menu_1_bianco";
			//tc.BackColor=Color.FromArgb(149,149,149);
            tc.BackColor = Color.FromArgb(red, green, blu);
            tc.Text = "Data acc.";
			tc.Height=Unit.Pixel(15);
			tc.Width=Unit.Percentage(10);

			tr.Cells.Add(tc);

			tc=new TableCell();
			tc.CssClass="menu_1_bianco";
			//tc.BackColor=Color.FromArgb(149,149,149);
            tc.BackColor = Color.FromArgb(red, green, blu);
            tc.Text = "Data Rif.";
			tc.Height=Unit.Pixel(15);
			tc.Width=Unit.Percentage(10);

			tr.Cells.Add(tc);

		
			tc=new TableCell();
			tc.CssClass="menu_1_bianco";
			//tc.BackColor=Color.FromArgb(149,149,149);
            tc.BackColor = Color.FromArgb(red, green, blu);
            tc.Text = "Info Acc. / Info Rif.";
			tc.Height=Unit.Pixel(15);
			
			tc.Width=Unit.Percentage(30);

			tr.Cells.Add(tc);

			tc=new TableCell();
			tc.CssClass="menu_1_bianco";
			//tc.BackColor=Color.FromArgb(149,149,149);
            tc.BackColor = Color.FromArgb(red, green, blu);
            tc.Text = "Data risp.";
			tc.Height=Unit.Pixel(15);
			tc.Width=Unit.Percentage(10);

			tr.Cells.Add(tc);

			tc=new TableCell();
			tc.CssClass="menu_1_bianco";
			//tc.BackColor=Color.FromArgb(149,149,149);
            tc.BackColor = Color.FromArgb(red, green, blu);
            tc.Text = "&nbsp;";
			tc.Height=Unit.Pixel(15);
			tc.Width=Unit.Percentage(5);

			tr.Cells.Add(tc);

			tc=new TableCell();
			tc.CssClass="menu_1_bianco";
			tc.BackColor=Color.FromArgb(255,255,255);
			tc.Text="&nbsp;";
			tc.Height=Unit.Pixel(15);
			tc.Width=Unit.Percentage(5);

			tr.Cells.Add(tc);
			tbl.Rows.Add(tr);

		}
		
			
		protected void drawRowTxVisual(System.Web.UI.WebControls.Table tbl, DocsPAWA.DocsPaWR.TrasmissioneSingola trasmSing,int f)
		{
			
			try
			{	
				//Cell Black
				TableRow tr=new TableRow();
				TableCell tc=new TableCell();
				tc.CssClass="testo_grigio";
				tc.BackColor=Color.FromArgb(217,217,217);
				tc.Text="&nbsp";
				tc.Width=Unit.Percentage(5); 
				//Destinatario		
				
				tc=new TableCell();
				tc.CssClass="testo_grigio";
				tc.BackColor=Color.FromArgb(217,217,217);
				tc.Text=trasmSing.corrispondenteInterno.descrizione;// è vuoto;
				tc.ColumnSpan=2;
				tc.Width=Unit.Percentage(20);

				tr.Cells.Add(tc);
				// Ragione
				tc=new TableCell();
							

				tc.CssClass="testo_grigio";
				tc.Text=trasmSing.ragione.descrizione;
				tc.BackColor=Color.FromArgb(217,217,217);
				tc.Height=Unit.Pixel(20);
				tc.Width=Unit.Percentage(20);
				tc.ColumnSpan=2;
				//	tc.Controls.Add(ddl_Ragione);
				tr.Cells.Add(tc);

	
				//Tipo    c'è solo per trasmSing indirizzate a Ruoli
				tc=new TableCell();
				tc.CssClass="testo_grigio";
				tc.BackColor=Color.FromArgb(217,217,217);
				tc.Height=Unit.Pixel(15);
				tc.Width=Unit.Percentage(10);

				if(trasmSing.tipoTrasm.Equals("T"))
				{
					tc.Text="TUTTI";
				}
				else
					if(trasmSing.tipoTrasm.Equals("S"))
				{
					tc.Text="UNO";
				}
				else
				{
					tc.Text="&nbsp;"; 
				}
				//	tc.Controls.Add(ddl_Tipo);
				tr.Cells.Add(tc);

				//Note Individuali
				tc=new TableCell();
				tc.CssClass="testo_grigio";
				tc.BackColor=Color.FromArgb(217,217,217);
				tc.Text=trasmSing.noteSingole;
				tc.Height=Unit.Pixel(15);
				tc.Width=Unit.Percentage(30);

				tr.Cells.Add(tc);

				//DATA SCAD.		
				tc=new TableCell();
				tc.CssClass="testo_grigio";
				tc.BackColor=Color.FromArgb(217,217,217);
				tc.Text=trasmSing.dataScadenza;
				//tc.ColumnSpan=2;
				tc.Height=Unit.Pixel(15);
				tc.Width=Unit.Percentage(10);

				tr.Cells.Add(tc);
				//In Risp. a c'è se la Ragone è RISPOSTA
				tc=new TableCell();
				tc.CssClass="testo_grigio";
				tc.BackColor=Color.FromArgb(217,217,217);
				//cotrollo se la ragione = RISPOSTA.
				if(trasmSing.ragione.descrizione.ToUpper()=="RISPOSTA")
					tc.Text="In Rips. a";
				else
					tc.Text="**********";
				tc.Height=Unit.Pixel(15);
				tc.ColumnSpan=2;
				tc.Width=Unit.Percentage(10);
				tr.Cells.Add(tc);
				tbl.Rows.Add(tr);


				//************************************************************************
				//************ROW TRASM UTENTE********************************************
				//************************************************************************				
				for(int i=0;i<trasmSing.trasmissioneUtente.Length;i++)
				{
					drawHeaderTrasmUt(tbl);
					
					DocsPaWR.TrasmissioneUtente TrasmUt=(DocsPAWA.DocsPaWR.TrasmissioneUtente)trasmSing.trasmissioneUtente[i];
					
					//Cella Bianca 	
					tr=new TableRow();
					tc=new TableCell();
					tc.CssClass="testo_grigio";
					tc.BackColor=Color.FromArgb(255,255,255);
					tc.Text="&nbsp;";
					tc.Width=Unit.Percentage(5);

					tr.Cells.Add(tc);
					//Utente		
			
					tc=new TableCell();
					tc.CssClass="testo_grigio";
					tc.BackColor=Color.FromArgb(242,242,242);
					tc.Text=TrasmUt.utente.descrizione;
					tc.Width=Unit.Percentage(15);

					tr.Cells.Add(tc);
					//Data Vista
					tc=new TableCell();
					tc.CssClass="testo_grigio";
					tc.BackColor=Color.FromArgb(242,242,242);
					tc.Text=TrasmUt.dataVista;
					tc.Width=Unit.Percentage(10);

					tr.Cells.Add(tc);

					//Data Acc.		
					//Tipo c'è solo per determinate ragioni
					tc=new TableCell();
					tc.CssClass="testo_grigio";
					tc.BackColor=Color.FromArgb(242,242,242);
					tc.Text=TrasmUt.dataAccettata;
					tc.Height=Unit.Pixel(15);
					tc.Width=Unit.Percentage(10);

					tr.Cells.Add(tc);

					//Data Rif.		
					
					tc=new TableCell();
					tc.CssClass="testo_grigio";
					tc.BackColor=Color.FromArgb(242,242,242);
					tc.Text=TrasmUt.dataRifiutata;
					tc.Height=Unit.Pixel(15);
					tc.Width=Unit.Percentage(10);

					tr.Cells.Add(tc);

					//Info 	Acc. / Info Rif.	
		
					tc=new TableCell();
					//tc.ColumnSpan=2;
					
					tc.CssClass="testo_grigio";
					
					
					tc.BackColor=Color.FromArgb(242,242,242);
					//					TextBox txtInf=new TextBox();
					//					string nomTxt="txt_Inf"+i.ToString();
					//						txtInf.CssClass="testo_grigio";
					//					txtInf.Enabled=false;

					//controllo se in Acc. o in rif.
					if(TrasmUt.dataAccettata!="")
						//						txtInf.Text=TrasmUt.noteAccettazione;
						tc.Text=TrasmUt.noteAccettazione;
					else
						if  (TrasmUt.dataRifiutata!="")
						//						txtInf.Text=TrasmUt.noteRifiuto;
						tc.Text=TrasmUt.noteRifiuto;
					else 
						//						txtInf.Text="";
						tc.Text="&nbsp;";
					//					tc.Controls.Add(txtInf);
					tc.Height=Unit.Pixel(15);
					tc.Width=Unit.Percentage(30);

					tr.Cells.Add(tc);

					//					tc=new TableCell();
					//					tc.CssClass="testo_grigio";
					//					tc.BackColor=Color.FromArgb(242,242,242);
					//					tc.Text="Info Rif.";
					//					tc.Height=Unit.Pixel(15);
					//					tc.Width=Unit.Percentage(17.5);

					//					tr.Cells.Add(tc);
					//Data Risp. è un link che porta al documento 
					tc=new TableCell();
					tc.CssClass="testo_grigio";
					tc.BackColor=Color.FromArgb(242,242,242);
					//creo obj-aspx Hyperlink 
					HyperLink hLink = new HyperLink();
					string nomeHlink="hl_"+i.ToString();
					hLink.CssClass="testo_grigio";
					hLink.Height=Unit.Pixel(15);
					
					tc.Height=Unit.Pixel(15);
					tc.Width=Unit.Percentage(10);

					tr.Cells.Add(tc);
					//cell con il colore della riga dati 
					tc=new TableCell();
					tc.CssClass="testo_grigio";
					tc.BackColor=Color.FromArgb(242,242,242);
					tc.Text=" ";
					tc.Height=Unit.Pixel(15);
					tc.Width=Unit.Percentage(5);

					tr.Cells.Add(tc);

					//cell bianca
					tc=new TableCell();
					tc.CssClass="testo_grigio";
					tc.BackColor=Color.FromArgb(255,255,255);
					tc.Text=" ";
					tc.Height=Unit.Pixel(15);
					tc.Width=Unit.Percentage(5);

					tr.Cells.Add(tc);

					tbl.Rows.Add(tr);
				}
				//fine row utente
		
			
			}
			catch(System.Exception ex)
			{
				ErrorManager.redirect(this,ex);
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
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion
	}
}
