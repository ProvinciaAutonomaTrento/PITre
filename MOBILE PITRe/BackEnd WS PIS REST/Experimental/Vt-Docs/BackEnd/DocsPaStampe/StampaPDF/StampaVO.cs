using System;
using System.Data;
using iTextSharp;
using iTextSharp.text.pdf;
using iTextSharp.text;
using ICSharpCode.SharpZipLib; 
using System.Collections;
using System.IO;
using System.Configuration;


namespace StampaPDF
{
	/// <summary>
	/// Summary description for StampaVO:
	///		Questo file contiene i Value Object utilizzati dall'applicativo e contenente i dati 
	///		per la formattazione del documento PDF
	/// </summary>
	/// 
	public class StampaVO
	{

		public class Document
		{
			public string title;
			public string fileName;
			public string pathName;
			public Page page = new Page();
			public ArrayList dataToPrint;


			public Document()
			{
//				//VERIFICARE ??? ERA UN TEST?
//				string PDFLogPath = ConfigurationManager.AppSettings["DEBUG_PATH"];
//				PDFLogPath = PDFLogPath.Replace("%DATA", DateTime.Now.ToString("yyyyMMdd"));
//				PDFLogPath += @"\PDF_Files";
//				if(!Directory.Exists(PDFLogPath))
//				{
//					Directory.CreateDirectory(PDFLogPath);
//				}
//				if(File.Exists(PDFLogPath + @"\report.pdf"))
//				{
//					File.Delete(PDFLogPath + @"\report.pdf");
//				}
//				this.fileName = PDFLogPath + @"\report.pdf";
			
			}
		}
		public class Page
		{
			public static int VERTICALE = 0;
			public static int ORIZZONTALE = 1;

			public string pageSize = "A4";  //A4 , A3, _11X17, LETTER
			public int orientation ;  //orizzontale   ---  verticale
			public float[] margins;  //left, right, top, bottom
			public Header headerPage;
			public Footer footerPage;
			public NumPagine numPagine;
			public DtaStampa dtaStampa;
			public Image logo;
			
		}

		public class Font
		{
			public string name;
			public float size;
			public string style;
			public string color;
		}
		public class Header
		{
			public string text;
			public string align;
			public string border;
			public string bgcolor;
			public NumPagine numPagine;
			public DtaStampa dtaStampa;
			public Font font;
		}

		public class Footer
		{
			public string text;
			public string align;
			public string border;
			public string bgcolor;
			public NumPagine numPagine;
			public DtaStampa dtaStampa;
			public Font font;
		}

		public class Image
		{
			public string fileImage;
			public string bgColor;
			public int border;
			public string align;
		}
		
		public class DataElement
		{
			public string text;
			public string align;
			public Font font;
			public string target;
		}
		public class DtaStampa
		{
			public string text;
			public string align;
			public Font font;
			public bool visible;
			public string target; 
			public string bgcolor;
		}
		public class NumPagine
		{
			public string text;
			public string align;
			public Font font;
			public bool visible;
			public string target; 
			public string bgcolor;
			public int total;
			public string separator;
			public string widthTot;
		}

		public class Paragraph: DataElement
		{
			public float indentationLeft;
			public float indentationRight;
		}

		public class Table: DataElement
		{
			public int padding;
			public int spacing;
			public float width;
			public float height;
			public int border;
			public int borderColor;
			public string bgColor;
			public StyleTable headerTable;
			public StyleTable dataTable;
			public Column[] columns;
			public DataTable dt;
		}

		public class Column
		{
			public static string COLUMN_WIDTH = "30";
			public string name;
			public string alias;
			public string align;
			public string vAlign;
			public Font font;
			public int rowSpan;
			public string bgColor;
			public string width;
			public bool visible;
		}

		public class StyleTable
		{
			public string align;
			public string vAlign;
			public Font font;
			public string bgColor;
		}

	
	}
}
