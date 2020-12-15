using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Drawing;
using StampaPDF.StampaBustePdf.PDFObject;
using StampaPDF.StampaBustePdf.Table_Object;

namespace StampaPDF.StampaBustePdf
{
	/// <summary>
	/// Summary description for PdfDocument.
	/// </summary>
	public class PdfDocument
	{
		internal static bool FlateCompression=true;
		/// <summary>
		/// sets the page format for the Pdf Documents (default is A4)
		/// </summary>
		/// <param name="PdfDocumentFormat"></param>
		public void SetPageFormat(PdfDocumentFormat PdfDocumentFormat)
		{
			this.PdfDocumentFormat=PdfDocumentFormat;
		}
		
		internal double PH
		{
			get
			{
				return this.PdfDocumentFormat.height;
			}
		}
		internal double PW
		{
			get
			{
				return this.PdfDocumentFormat.width;
			}
		}
		internal PdfDocumentFormat PdfDocumentFormat=PdfDocumentFormat.A4;
		
		/// <summary>
		/// gets othe height of the page in points (using 72 dpi depth)
		/// </summary>
		public double PageHeight
		{
			get
			{
				return PH;
			}
			
		}
		/// <summary>
		/// gets the width of the page in points (using 72 dpi depth)
		/// </summary>
		public double PageWidth
		{
			get
			{
				return PW;
			}
			
		}
		#region properties
		internal int _nextid=0;
		internal int GetNextId
		{
			get
			{
				_nextid++;
				return _nextid;
			}
		}
		internal Hashtable PdfObjects;
		private PdfObject Header
		{
			get
			{
				foreach (PdfObject po in this.PdfObjects.Values)
					if (po.GetType()==typeof(PdfHeader)) return po;
				return null;
			}
			set
			{
				foreach (PdfObject po in this.PdfObjects.Values)
					if (po.GetType()==typeof(PdfHeader))
					{
						this.PdfObjects.Remove(po.HeadObj);
						break;
					}
				this.PdfObjects.Add(value.HeadObj,value);
			}
		}
		internal PdfObject Catalog
		{
			get
			{
				foreach (PdfObject po in this.PdfObjects.Values)
					if (po.GetType()==typeof(PdfCatalog)) return po;
				return null;
			}
		}
		internal PdfObject PdfRoot
		{
			get
			{
				foreach (PdfObject po in this.PdfObjects.Values)
					if (po.GetType()==typeof(PdfRoot)) return po;
				return null;
			}
		}
		/// <summary>
		/// gets the current saved pages number of the document.
		/// </summary>
		public int PageCount
		{
			get
			{
				int count=0;
				foreach (PdfObject po in this.PdfObjects.Values)
					if (po.GetType()==typeof(PdfPage)) count++;
				return count;
			}
		}
		
		internal ArrayList FontList
		{
			get
			{
				ArrayList al=new ArrayList();
				foreach (PdfObject po in this.PdfObjects.Values)
					if (po.GetType()==typeof(PdfFont)) al.Add(po);
				return al;
			}
		}
		internal bool ContainsFont(PdfFont pf)
		{
			bool r=false;
			foreach (PdfFont pf2 in this.FontList)
			{
				if (pf2.Name==pf.Name) r=true;
			}
			return r;
		}
		internal ArrayList FontNameList
		{
			get
			{
				ArrayList al=new ArrayList();
				foreach (PdfObject po in this.PdfObjects.Values)
					if (po.GetType()==typeof(PdfFont)) al.Add((po as PdfFont).Name);
				return al;
			}
		}
		
		System.IO.Stream ms;
		#endregion
		/// <summary>
		/// set Document Subject and Title.
		/// </summary>
		public void SetHeaders(string subject,string title,string author)
		{
			if (subject==null) throw new Exception("string Subject cannot be null.");
			if (title==null) throw new Exception("string Title cannot be null.");
			this.Header=new PdfHeader(this,subject,title,author);
		}
		/// <summary>
		/// Creates a new Pdf Document.
		/// </summary>
		public PdfDocument()
		{
			PdfObjects=new Hashtable();
			this.AddPdfObject(new PdfHeader(this,"","",""));
			
		}
		public PdfDocument(PdfDocumentFormat DocumentFormat)
		{
			this.SetPageFormat(DocumentFormat);
			PdfObjects=new Hashtable();
			this.AddPdfObject(new PdfHeader(this,"","",""));
			
			
		}
		internal void AddPdfObject(PdfObject po)
		{
			po.PdfDocument=this;
			if (!this.PdfObjects.ContainsKey(po.HeadObj))
			{
				this.PdfObjects.Add(po.HeadObj,po);
			}
		}
		private void Send(string strMsg)
		{
			Byte[] buffer=null;
			buffer=ASCIIEncoding.ASCII.GetBytes(strMsg);
			ms.Write(buffer,0,buffer.Length); 
		}
		/// <summary>
		/// Outputs the Complete Pdf Document to a Generic Stream
		/// </summary>
		/// <param name="m">
		/// The Generic Stream to Output the Pdf Document
		/// </param>

		public void SaveToStream(System.IO.Stream m)
		{
			this.AddPdfObject(new PdfCatalog(this));
			this.AddPdfObject(new PdfRoot(this));
			this.ms=m;
			try {Send("%PDF-1.4\n");}
			catch {throw new Exception("Error writing to the output stream.");}
			
			string xref="xref\n0 "+this.PdfObjects.Count.ToString()+"\n0000000000 65535 f \n";
			long pos=0;
			for (int x=1;x<=this._nextid;x++)
			{
				PdfObject o=this.PdfObjects[x.ToString()+" 0 obj\n"] as PdfObject;
				if (o!=null)
				{
					xref+=Utility.xRefFormatting(pos)+" 00000 n \n";
					try	{pos+=o.StreamWrite(ms);}
					catch(Exception ex)
					{
						throw ex;
					}
				}
			}
			
			long startxref=pos;
			Send(xref);
			Send("trailer\n<<\n/Size "+ (PdfObjects.Count)+"\n/Root "+this.Catalog.HeadR+"\n/Info "+this.Header.HeadR+"\n>>\n");
			Send("startxref\n" + startxref+"\n");
			Send("%%EOF\n");
			
		}

		/// <summary>
		/// Outputs the complete PDF Document to a file
		/// </summary>
		/// <param name="file"></param>
		public void SaveToFileStream(string file)
		{
			System.IO.FileStream fs;
			try
			{
				fs=new FileStream(file,FileMode.OpenOrCreate,FileAccess.ReadWrite);
				
			}
			catch
			{throw new Exception("Error opening destination file");}
			this.SaveToStream(fs);
			fs.Close();

		}
		internal void AddFont(System.Drawing.Font f)
		{
			string name=PdfFont.FontToPdfType(f);
			
			PdfFont pf=new PdfFont(name,name);
			if (!this.ContainsFont(pf)) 
			{
				pf.ID=this.GetNextId;
				this.AddPdfObject(pf);
			}
		}
		/// <summary>
		/// Creates a New Page for the Pdf Document
		/// </summary>
		/// <returns>
		/// 
		/// </returns>
		public PdfPage NewPage()
		{
			PdfPage p=new PdfPage();
			p.PdfDocument=this;
			return p;
		}
		/// <summary>
		/// Insert the Image Object into the Document before placing it in a document page
		/// </summary>
		/// <param name="file">
		/// the Color 72dpi Jpeg Image to insert into the document
		/// </param>
		/// <returns>
		/// 
		///</returns>
		public PdfImage NewImage(string file)
		{
			PdfImage pi;
			try {pi=new PdfImage(this.GetNextId,file);}
			catch {throw new Exception("Error opening the Image File");}
			this.AddPdfObject(pi);
			return pi;
		}
		/// <summary>
		/// Instantiates a new PdfTable setting the default specs.
		/// </summary>
		/// <param name="DefaultContentAlignment"></param>
		/// <param name="DefaultFont"></param>
		/// <param name="DefaultForegroundColor"></param>
		/// <param name="Rows"></param>
		/// <param name="Columns"></param>
		/// <param name="CellPadding"></param>
		/// <returns></returns>
		public PdfTable NewTable(ContentAlignment DefaultContentAlignment,Font DefaultFont,Color DefaultForegroundColor,int Rows
			,int Columns,double CellPadding)
		{
			if (Rows<=0) throw new Exception("Rows must be grater than zero.");
			if (Columns<=0) throw new Exception("Columns must be grater than zero.");
			if (CellPadding<0) throw new Exception("CellPadding must be non-negative.");
			PdfTable pt=new PdfTable(this,DefaultContentAlignment,DefaultFont,DefaultForegroundColor,Rows
				,Columns,CellPadding);
			pt.header=new PdfTable(this,ContentAlignment.MiddleCenter,DefaultFont,Color.Black,1
				,Columns,CellPadding);

			return pt;
		}
		/// <summary>
		/// Instantiates a new PdfTable setting the default specs.
		/// </summary>
		/// <param name="DefaultFont"></param>
		/// <param name="Rows"></param>
		/// <param name="Columns"></param>
		/// <param name="CellPadding"></param>
		/// <returns></returns>
		public PdfTable NewTable(Font DefaultFont,int Rows,int Columns,double CellPadding)
		{
			if (Rows<=0) throw new Exception("Rows must be grater than zero.");
			if (Columns<=0) throw new Exception("Columns must be grater than zero.");
			if (CellPadding<0) throw new Exception("CellPadding must be non-negative.");
			PdfTable pt=new PdfTable(this,ContentAlignment.TopCenter,DefaultFont,Color.Black,Rows
				,Columns,CellPadding);
			pt.header=new PdfTable(this,ContentAlignment.MiddleCenter,DefaultFont,Color.Black,1
				,Columns,CellPadding);
			
			return pt;
		}
		
		
	}
}
