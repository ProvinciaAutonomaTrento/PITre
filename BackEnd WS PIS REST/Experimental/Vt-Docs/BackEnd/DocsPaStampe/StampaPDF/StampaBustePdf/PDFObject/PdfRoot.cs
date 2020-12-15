using System;
using System.Text;
using StampaPDF.StampaBustePdf.Table_Object;

namespace StampaPDF.StampaBustePdf.PDFObject
{
	internal class PdfRoot : PdfObject
	{
		public PdfRoot(PdfDocument PdfDocument)
		{
			this.PdfDocument=PdfDocument;
			this.id=this.PdfDocument.GetNextId;

		}
		private string KidsLine
		{
			get
			{
				string s="";
				for (int x=1;x<this.PdfDocument._nextid;x++)
				{
					object o=this.PdfDocument.PdfObjects[x.ToString()+" 0 obj\n"];
					if (o!=null)
						if (o.GetType()==typeof(PdfPage))s+=((PdfObject)o).HeadR;
				}
				return "/Kids ["+s+"]\n";
			}
		}
		internal override int StreamWrite(System.IO.Stream stream)
		{
			string s="";
			s+=this.HeadObj;
			s+="<<\n";
			s+="/Type /Pages\n/Count "+this.PdfDocument.PageCount+"\n"+this.KidsLine;
			s+=">>\n";
			s+="endobj\n";

			Byte[] b=ASCIIEncoding.ASCII.GetBytes(s);
			stream.Write(b,0,b.Length);
			return b.Length;
		}

	}
}
