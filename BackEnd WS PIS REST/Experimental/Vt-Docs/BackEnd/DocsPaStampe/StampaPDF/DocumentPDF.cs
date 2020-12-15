using System;
using iTextSharp;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;


namespace StampaPDF
{
	/// <summary>
	/// Summary description for DocumentPDF.
	/// </summary>
	public class DocumentPDF:Document
	{
		public MemoryStream memoryStream;
		
		public DocumentPDF()
		{
			memoryStream = new MemoryStream();
		}

	}
}
