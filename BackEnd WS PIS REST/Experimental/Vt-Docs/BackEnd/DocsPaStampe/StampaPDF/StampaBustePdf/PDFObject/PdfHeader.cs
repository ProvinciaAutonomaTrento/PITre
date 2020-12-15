using System;
using System.Text;
using StampaPDF.StampaBustePdf.Table_Object;

namespace StampaPDF.StampaBustePdf.PDFObject
{
	internal class PdfHeader : PdfObject
	{
		private string subject,title,author,creationdate;
		internal PdfHeader(PdfDocument PdfDocument,string subject,string title,string author)
		{
			this.PdfDocument=PdfDocument;
			this.id=this.PdfDocument.GetNextId;
			this.subject=subject;
			this.title=title;
			this.author=author;
			this.creationdate=DateTime.Today.ToShortDateString();
		}
		internal override int StreamWrite(System.IO.Stream stream)
		{
			string s="";
			s+=this.HeadObj;
			s+="<<\n";
			s+="/Subject ("+subject+")\n/Title ("+title+")\n/Creator (Gios Pdf.NET Library)\n";
			s+="/Producer(Paolo Gios - http://www.paologios.com)\n";
			s+="/Author ("+author+")\n/CreationDate ("+creationdate+")\n";
			s+=">>\n";
			s+="endobj\n";
			Byte[] b=ASCIIEncoding.ASCII.GetBytes(s);
			stream.Write(b,0,b.Length);
			return b.Length;
		}

	}
}