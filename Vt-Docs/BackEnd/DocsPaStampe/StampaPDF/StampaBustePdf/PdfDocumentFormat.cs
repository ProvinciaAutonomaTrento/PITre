using System;
using StampaPDF.StampaBustePdf.PDFObject;
using StampaPDF.StampaBustePdf.Table_Object;

namespace StampaPDF.StampaBustePdf
{
	/// <summary>
	/// Summary description for PdfDocumentFormat.
	/// </summary>
	public class PdfDocumentFormat
	{
		internal double height,width;
		internal PdfDocumentFormat(double Width,double Height)
		{
			this.height=Height;
			this.width=Width;
		}
		/// <summary>
		/// gets the Classic A4 Letter.
		/// </summary>
		public static PdfDocumentFormat A4
		{
			get
			{
				return PdfDocumentFormat.InCentimeters(21,29.7);
			}
		}
		/// <summary>
		/// gets the Classic A4 Letter (Horizontal)
		/// </summary>
		public static PdfDocumentFormat A4_Horizontal
		{
			get
			{
				return PdfDocumentFormat.InCentimeters(29.7,21);
			}
		}
		/// <summary>
		/// gets the 8.5x11 American Letter.
		/// </summary>
		public static PdfDocumentFormat Letter_8_5x11
		{
			get
			{
				return PdfDocumentFormat.InInches(8.5,11);
			}
		}
		/// <summary>
		/// gets the 8.5x11 American Letter (Horizontal)
		/// </summary>
		public static PdfDocumentFormat Letter_8_5x11_Horizontal
		{
			get
			{
				return PdfDocumentFormat.InInches(11,8.5);
			}
		}
		/// <summary>
		/// creates a custom inches sized paper.
		/// </summary>
		/// <param name="Width"></param>
		/// <param name="Height"></param>
		/// <returns></returns>
		public static PdfDocumentFormat InInches(double Width,double Height)
		{
			if (Width<=0) throw new Exception("Width must be grater than zero.");
			if (Height<=0) throw new Exception("Height must be grater than zero.");
			return new PdfDocumentFormat(Width*72,Height*72);
		}
		/// <summary>
		/// creates a custom centimeters sized paper.
		/// </summary>
		/// <param name="Width"></param>
		/// <param name="Height"></param>
		/// <returns></returns>
		public static PdfDocumentFormat InCentimeters(double Width,double Height)
		{
			if (Width<=0) throw new Exception("Width must be grater than zero.");
			if (Height<=0) throw new Exception("Height must be grater than zero.");
			return new PdfDocumentFormat(Width*72/2.54,Height*72/2.54);
		}
	}
}
