using System;
using System.Text;
using StampaPDF.StampaBustePdf.Table_Object;

namespace StampaPDF.StampaBustePdf.PDFObject
{
	internal class PdfFont : PdfObject
	{
		private string name,typename;
		public string Name
		{
			get
			{
				return this.name;
			}
		}
		internal PdfFont(int id,string name,string typename)
		{
			this.name=name;
			this.typename=typename;
			this.id=id;
		}
		internal PdfFont(string name,string typename)
		{
			this.name=name;
			this.typename=typename;
		}
		internal static string FontToPdfType(System.Drawing.Font f)
		{
			string name="";
			switch (f.Name)
			{
				case "Times New Roman":
					if (f.Bold) name="Times-Bold"; else name="Times-Roman";
					break;
				case "Courier New":
					if (f.Bold) name="Courier-Bold"; else name="Courier";
					break;
				default:
					if (f.Bold) name="Helvetica-Bold"; else name="Helvetica";
					break;
			}
			return name;
		}
		
		internal override int StreamWrite(System.IO.Stream stream)
		{
			string text="";
			text+="/Type /Font\n";
			text+="/Subtype /Type1\n/";
			text+="Name /"+name+"\n";
			text+="/BaseFont /"+typename+"\n";
			text+="/Encoding /WinAnsiEncoding\n";
			
			string s="";
			s+=this.HeadObj;
			s+="<<\n";
			s+="/Type /Font\n/";
			s+="Subtype /Type1\n/";
			s+="Name /"+name+"\n";
			s+="/BaseFont /"+typename+"\n";
			s+="/Encoding /WinAnsiEncoding\n";
			s+=">>\n";
			s+="endobj\n";
			Byte[] b=ASCIIEncoding.ASCII.GetBytes(s);
			stream.Write(b,0,b.Length);
			return b.Length;
		}

	}
	
}
