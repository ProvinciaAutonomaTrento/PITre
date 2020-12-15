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
using System.Xml.XPath;
using System.Xml;

namespace DocsPAWA.documento
{
	/// <summary>
	/// Summary description for StampaDocTrasmx.
	/// </summary>
	public class StampaDocTrasmx : System.Web.UI.Page
	{
		protected System.Web.UI.WebControls.Table tblTrasmx;
	
		private void Page_Load(object sender, System.EventArgs e)
		{
			//this.DataGrid1.DataSource = Session["docTrasmissioni.trasmQueryEff"];
			DocsPaWR.Trasmissione[] trx = (DocsPAWA.DocsPaWR.Trasmissione[])Session["docTrasmissioni.trasmQueryEff"];
			TableRow trHead=new TableRow();
			TableCell tc=new TableCell();
			//string[] intestazioni = new string[] ;//{"UTENTE","DATA INVIO","RUOLO"};
			try
			{
				
				XPathDocument Doc = new XPathDocument(Server.MapPath("TableHeaders.xml"));
				XPathNavigator nav = Doc.CreateNavigator();
				XPathNodeIterator Iterator = nav.Select("./intestazioni/MainHeader/mhvoce");
				string[] intestazioni = new string[Iterator.Count];
				int iCount = 0;
				while (Iterator.MoveNext())
				{
					intestazioni[iCount]=Iterator.Current.Value;
					iCount++;
				}
			
				//string[] subHeader = new string[] {"RAGIONE","CORRISPONDENTE","DATA SCADENZA"};

				trHead.BackColor=Color.Brown;
				tc.CssClass=".titolo_rosso";
				
				tc.ColumnSpan=6;
				tc.Text="Report Trasmissioni";
				tc.HorizontalAlign=HorizontalAlign.Center;
				tc.Width=Unit.Percentage(100);
				tc.Height=Unit.Pixel(20);
				trHead.Cells.Add(tc);

				tblTrasmx.Rows.Add(trHead);

				foreach(DocsPAWA.DocsPaWR.Trasmissione trSing in trx)
				{
					DisegnaIntestazioni(Iterator.Count,intestazioni);
				
					string[] contenutoMain = {trSing.utente.descrizione.ToString(),
												 trSing.dataInvio.ToString(),
												 trSing.ruolo.descrizione.ToString()};
					#region commento
					/*TableRow tr = new TableRow();
				
					TableCell tcUtente = new TableCell();
					tcUtente.Text=trSing.utente.descrizione.ToString();
					tr.Cells.Add(tcUtente);
					tblTrasmx.Rows.Add(tr);

					TableCell tcDtInvio = new TableCell();
					tcDtInvio.Text=trSing.dataInvio.ToString();
					tr.Cells.Add(tcDtInvio);
					tblTrasmx.Rows.Add(tr);

					TableCell tcRuolo = new TableCell();
					tcRuolo.Text=trSing.ruolo.descrizione.ToString();
					tr.Cells.Add(tcRuolo);*/
					#endregion commento
					tblTrasmx.Rows.Add(SetContentCell(contenutoMain));

					Iterator = nav.Select("./intestazioni/SubHeader/shvoce");
					iCount = 0;
					string[] subHeader = new string[Iterator.Count];
					while (Iterator.MoveNext())
					{
						subHeader[iCount]=Iterator.Current.Value;
						iCount++;
					}
					DisegnaIntestazioni(Iterator.Count,subHeader);
					foreach(DocsPAWA.DocsPaWR.TrasmissioneSingola ts in trSing.trasmissioniSingole)
					{
						#region commento
						/*TableRow trTrasmSing = new TableRow();
					
						TableCell tcTrasmSing = new TableCell();
						tcTrasmSing.Text = ts.ragione.descrizione.ToString();
						tcTrasmSing.BackColor=Color.Aquamarine;
						trTrasmSing.Cells.Add(tcTrasmSing);

						TableCell tcTSCorr = new TableCell();
						tcTSCorr.Text = ts.corrispondenteInterno.descrizione.ToString();
						tcTSCorr.BackColor=Color.Aquamarine;
						trTrasmSing.Cells.Add(tcTSCorr);
					
						TableCell tcUK = new TableCell();
						tcUK.Text = ts.dataScadenza.ToString();
						tcUK.BackColor=Color.Aquamarine;
						trTrasmSing.Cells.Add(tcUK);*/
						#endregion commento
					
						//ts.tipoTrasm
						//ts.noteSingole
						string[] contenuto = {ts.ragione.descrizione.ToString(),
												 ts.corrispondenteInterno.descrizione.ToString(),
												 ts.dataScadenza.ToString()} ;
						tblTrasmx.Rows.Add(SetContentCell(contenuto));
					}
				}
			
				//			tc.Text = "prova";
				//			tr.Cells.Add(tc);
				//			tblTrasmx.Rows.Add(tr);
				Response.Buffer = true;
				Response.ContentType = "text/html";//"application/vnd.ms-excel";

			}
			catch(Exception ex)
			{
				string strDebug = ex.Message;
				return;
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

		/// <summary>
		/// crea dinamicamente un'intestazione
		/// basata su una riga di tabella
		/// contenente n celle con il numero
		/// di celle passate in ingresso ed 
		/// un array di stringhe che costituiscono
		/// il contenuto delle celle.
		/// </summary>
		/// <param name="numeroCelle"></param>
		/// <param name="intestazioni"></param>
		private void DisegnaIntestazioni(int numeroCelle,string[] intestazioni)
		{
			
			TableRow tr = new TableRow();
			try
			{
				for(int i=0;i<numeroCelle;i++)
				{
					tr.Cells.Add(new TableCell());
					tr.Cells[i].Text=intestazioni[i];
					tr.Cells[i].BackColor=Color.LightGray;
				}
				
			}
			catch(System.IndexOutOfRangeException OutOfRange)
			{
				string str = OutOfRange.Message ;
			}
			this.tblTrasmx.Rows.Add(tr);
		}
		
		/// <summary>
		/// imposta il contenuto della cella
		/// </summary>
		/// <param name="contenuto">contenuto della cella</param>
		private TableRow SetContentCell(string[] contenuto)
		{
			TableRow tr = new TableRow();
			foreach(string str in contenuto)
			{
				TableCell tc = new TableCell();
				tc.Text = str ;
				tc.BackColor = Color.Gray;
				tr.Cells.Add(tc);
			}
			
			return tr;
		}
		#endregion
	}
}
