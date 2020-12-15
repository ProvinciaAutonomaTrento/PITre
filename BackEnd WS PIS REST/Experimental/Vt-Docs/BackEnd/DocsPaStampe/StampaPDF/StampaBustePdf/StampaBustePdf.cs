using System;
using System.Data;
using System.Drawing;
using System.IO;
using StampaPDF.StampaBustePdf.PDFObject;
using StampaPDF.StampaBustePdf.Table_Object;





namespace StampaPDF.StampaBustePdf
{
	/// <summary>
	/// Summary description for StampaBustePdf.
	/// </summary>
	public class StampaBustePdf
	{
		public StampaBustePdf()
		{
			//
			// TODO: Add constructor logic here
			//
		}
		
		public System.IO.FileStream GeneraBustaPdf(DataTable dtProfile,string userId)
		{
			
			System.IO.FileStream fsout = null;
			try
			{
				PdfDocument myPdfDocument=new PdfDocument(PdfDocumentFormat.InCentimeters(22.9,16.2));
			
				foreach(DataRow row in dtProfile.Rows)
				{
					//Creo la una tabella di 3 righe, 1 colonnA e 5 ppunti di Padding.
					PdfTable myPdfTable=myPdfDocument.NewTable(new Font("Times New Roman",10),3,1,5);

					myPdfTable.ImportDataTable(Table(row));
				
					myPdfTable.SetColors(Color.Black,Color.White);
					myPdfTable.SetBorders(Color.Black,1,BorderType.None);
					myPdfTable.SetColumnsWidth(new int[]{100});
			
					myPdfTable.VisibleHeaders=false;
					myPdfTable.SetContentAlignment(ContentAlignment.MiddleLeft);
		
					while (!myPdfTable.AllTablePagesCreated)
					{
				
						PdfPage newPdfPage=myPdfDocument.NewPage();
						PdfArea newPdfArea = new  PdfArea(myPdfDocument,240.0,230.0,350.0,200.0);
						PdfTablePage newPdfTablePage=myPdfTable.CreateTablePage(newPdfArea);
				
						PdfTextArea pta=new PdfTextArea(new Font("Times New Roman",10,FontStyle.Regular),Color.Black,new PdfArea(myPdfDocument,55,145,250,80),ContentAlignment.TopCenter,row["VAR_DESC_REGISTRO"].ToString().Trim());
//						PdfTextArea pta=new PdfTextArea(new Font("Times New Roman",10,FontStyle.Regular),Color.Black,new PdfArea(myPdfDocument,55,145,250,80),ContentAlignment.TopCenter,"dasd sd asf af fdf f sadf asd fas fds fas dfsad fdsa fads fds f dsf dsaf dsa fsad fsa f sadf sad fsda fd fdsa fewsafrew fre gfre gf re erw gewr er rebre br be beb reb ber be b erb re bre ber be rb erbv vf ff f ff f ff fffvgvg");
						newPdfPage.Add(newPdfTablePage);
						newPdfPage.Add(pta);				
	
						newPdfPage.SaveToDocument();	
					}
				}
				
				
				string path = System.Configuration.ConfigurationManager.AppSettings["REPORTS_PATH"];
				//path = path.Replace("%DATA", "StampaB_" + userId + DateTime.Now.ToString("yyyyMMdd")); 
				path = path.Replace("%DATA", "StampaBuste"); 
				try
				{
					if(!System.IO.Directory.Exists(path))
					{
						System.IO.Directory.CreateDirectory(path);
					}
				}
				catch(Exception ex)
				{
					throw ex;
				}

				string nomeFile = userId + DateTime.Now.ToString("yyyyMMdd") + ".pdf";
				path = path + @"\" + nomeFile;
				System.IO.FileInfo nfile = new FileInfo(path);
				try
				{
					if(nfile.Exists)
					{
						nfile.Delete();
					}
				}
				catch(Exception ex)
				{
					throw ex;
				}
				myPdfDocument.SaveToFileStream(path);
				
				fsout = new FileStream(path,System.IO.FileMode.Open,System.IO.FileAccess.Read);
				
			}
			catch(Exception ex)
			{
				if(fsout!=null)
				{
					fsout.Close();
				}
				throw ex;	
			}

			return fsout;
			
		}

		static DataTable Table(DataRow rowProfile)
		{
		
				DataTable dt=new DataTable();
				dt.Columns.Add("col1");
				dt.Columns.Add("col2");
				dt.Columns.Add("col3");
				

				DataRow dr1=dt.NewRow();
				dr1["col1"]=rowProfile["VAR_DESC_CORR"].ToString().Trim();
				dt.Rows.Add(dr1);
				
				DataRow dr2=dt.NewRow();
				dr2["col1"]=rowProfile["VAR_INDIRIZZO"].ToString().Trim();
				dt.Rows.Add(dr2);
				
				DataRow dr3=dt.NewRow();
				dr3["col1"]=rowProfile["VAR_CAP"].ToString().Trim() + "   " +rowProfile["VAR_CITTA"].ToString().Trim() + "   " +rowProfile["VAR_PROVINCIA"].ToString().Trim();
				dt.Rows.Add(dr3);

				return dt;
			
		}


	}
}
