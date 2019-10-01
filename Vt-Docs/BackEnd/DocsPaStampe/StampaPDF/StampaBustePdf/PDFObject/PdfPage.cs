using System;
using System.Collections;
using System.Text;
using System.Drawing;
using StampaPDF.StampaBustePdf.Table_Object;

namespace StampaPDF.StampaBustePdf.PDFObject
{
	/// <summary>
	/// Summary description for PdfPage.
	/// </summary>
	public class PdfPage : PdfObject
	{
		private string FontsLine
		{
			get
			{
				string s= "Font << ";
				foreach (PdfFont pf in this.PdfDocument.FontList) s+="/"+pf.Name+" "+pf.HeadR;
				s+=">>\n";
				return s;
			}
		}
		private ArrayList Contents;
		internal ArrayList PagePdfObjects;
		
		internal string MediaBoxLine
		{
			get
			{
				return "/MediaBox [0 0 "+this.PdfDocument.PW.ToString("0.##").Replace(",",".")
					+" "+this.PdfDocument.PH.ToString("0.##").Replace(",",".")+"]\n";
			}
		}
		internal string ContentsLine
		{
			get
			{
				string s="/Contents [";
				foreach (PdfObject po in this.PagePdfObjects) 
				{
					if (po.GetType()!=typeof(PdfImage)) s+=po.HeadR;
					/*if (po.GetType()==typeof(PdfRectangle)) s+=po.HeadR;
					if (po.GetType()==typeof(PdfCircle)) s+=po.HeadR;
					if (po.GetType()==typeof(PdfLine)) s+=po.HeadR;
					//if (po.GetType()==typeof(PdfGrid)) s+=po.HeadR;
					//if (po.GetType()==typeof(PdfTable)) s+=po.HeadR;
					if (po.GetType()==typeof(PdfTablePage)) s+=po.HeadR;
					if (po.GetType()==typeof(PdfTextArea)) s+=po.HeadR;
					if (po.GetType()==typeof(PdfImageContent)) s+=po.HeadR;*/
				}
				s+="]\n";
				return s;
			}
		}
		internal string XObjectLine
		{
			get
			{
				string s="/XObject <<";
				foreach (PdfObject po in this.PagePdfObjects) 
				{
					if (po.GetType()==typeof(PdfImage)) s+="/I"+po.ID+" "+po.HeadR;
				}
				s+=" >>\n";
				return s;
			}
		}
		
		internal PdfPage()
		{
			Contents=new ArrayList();
			PagePdfObjects=new ArrayList();
		}
		/// <summary>
		/// adds a Pdf Element into this PdfPage.
		/// </summary>
		/// <param name="PdfImage"></param>
		/// <param name="posx"></param>
		/// <param name="posy"></param>
		public void Add(PdfImage PdfImage,double posx,double posy)
		{
			this.PagePdfObjects.Add(PdfImage);
			this.PagePdfObjects.Add(new PdfImageContent(PdfDocument.GetNextId,"I"+PdfImage.ID,PdfImage.ID,PdfImage.Width,PdfImage.Height,posx,this.PdfDocument.PH-posy,PdfImage.bmp.HorizontalResolution));
		}
		/// <summary>
		/// adds a Pdf Element into this PdfPage.
		/// </summary>
		/// <param name="PdfImage"></param>
		/// <param name="posx">The target X positioning of the picture.</param>
		/// <param name="posy">The target Y positioning of the picture.</param>
		/// <param name="DPI">The target resolution of the picture.</param>
		public void Add(PdfImage PdfImage,double posx,double posy,double DPI)
		{
			if (DPI<=0) throw new Exception("DPI must be greater than zero.");
			this.PagePdfObjects.Add(PdfImage);
			this.PagePdfObjects.Add(new PdfImageContent(PdfDocument.GetNextId,"I"+PdfImage.ID,PdfImage.ID,PdfImage.Width,PdfImage.Height,posx,this.PdfDocument.PH-posy,DPI));
		}
		/// <summary>
		/// adds a Pdf Element into this PdfPage.
		/// </summary>
		/// <param name="PdfRectangle"></param>
		public void Add(PdfRectangle PdfRectangle)
		{
			PdfRectangle.ID=this.PdfDocument.GetNextId;
			this.PagePdfObjects.Add(PdfRectangle);
		}
		/// <summary>
		/// adds a Pdf Element into this PdfPage.
		/// </summary>
		/// <param name="PdfCircle"></param>
		public void Add(PdfCircle PdfCircle)
		{
			PdfCircle.ID=this.PdfDocument.GetNextId;
			this.PagePdfObjects.Add(PdfCircle);
		}
		/// <summary>
		/// adds a Pdf Element into this PdfPage.
		/// </summary>
		/// <param name="PdfLine"></param>
		public void Add(PdfLine PdfLine)
		{
			PdfLine.ID=this.PdfDocument.GetNextId;
			this.PagePdfObjects.Add(PdfLine);
		}
		/// <summary>
		/// adds a Pdf Element into this PdfPage.
		/// </summary>
		/// <param name="PdfTablePage"></param>
		public void Add(PdfTablePage PdfTablePage)
		{
			if (PdfTablePage!=null)
			{
				
				PdfTablePage.ID=this.PdfDocument.GetNextId;
				this.PagePdfObjects.Add(PdfTablePage);
			}
		}
		/// <summary>
		/// adds a Pdf Element into this PdfPage.
		/// </summary>
		/// <param name="PdfTextArea"></param>
		public void Add(PdfTextArea PdfTextArea)
		{
			PdfTextArea.ID=this.PdfDocument.GetNextId;
			if (!this.PdfDocument.FontList.Contains(PdfTextArea.Font.Name))
				this.PdfDocument.AddFont(PdfTextArea.Font);
			this.PagePdfObjects.Add(PdfTextArea);
		}
				
		internal override int StreamWrite(System.IO.Stream stream)
		{
			string s="";
			s+=this.HeadObj;
			s+="<< /Type /Page\n/";
			s+="Parent "+PdfDocument.PdfRoot.HeadR+"\n";
			s+=this.MediaBoxLine;
			s+="/Resources\n";
			s+="<<\n/";
			s+=this.FontsLine;
			s+="/ProcSet [/PDF/ImageC/ImageI/ImageB/Text]\n";
			s+=this.XObjectLine;
			s+=">>\n";
			s+=this.ContentsLine;
			s+=" >>\n";
			s+="endobj\n";
			Byte[] b=ASCIIEncoding.ASCII.GetBytes(s);
			stream.Write(b,0,b.Length);
			return b.Length;
		}
		/// <summary>
		/// creates a copy of this page in order to use it as a template
		/// </summary>
		/// <returns></returns>
		public PdfPage CreateCopy()
		{
			PdfPage clone=new PdfPage();
			clone.PdfDocument=this.PdfDocument;
			foreach (PdfObject o in this.PagePdfObjects) clone.PagePdfObjects.Add(o);
			return clone;
		}
		/// <summary>
		/// save this page to the document and discard any further change
		/// </summary>
		public void SaveToDocument ()
		{
			/*if (!PdfDocument.Registered) 
				this.Add(new PdfTextArea(
					new System.Drawing.Font("Courier New",38,System.Drawing.FontStyle.Bold)
					,System.Drawing.Color.Red
					,new PdfArea(0,0,PdfDocument.PageWidth,PdfDocument.PageHeight)
					,ContentAlignment.MiddleCenter,"This is a Demo Library, please register!\nsee http://www.gios-tech.com"));
		*/	this.id=this.PdfDocument.GetNextId;
			this.PdfDocument.AddPdfObject(this);
			foreach (PdfObject o2 in this.PagePdfObjects) 
			{
				this.PdfDocument.AddPdfObject(o2);
			}
			
		}
	}
}
