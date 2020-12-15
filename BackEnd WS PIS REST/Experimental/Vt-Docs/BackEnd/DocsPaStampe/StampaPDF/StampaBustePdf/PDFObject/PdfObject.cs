using System;
using StampaPDF.StampaBustePdf.Table_Object;

namespace StampaPDF.StampaBustePdf.PDFObject
{
	/// <summary>
	/// the abstract pdf object class
	/// </summary>
	public abstract class PdfObject
	{
		internal PdfObject()
		{
			
		}
		internal PdfObject(Byte[] buffer,int id)
		{
			this.buffer=buffer;
			this.id=id;
		}
		
		#region properties
		internal PdfDocument PdfDocument;
		/// <summary>
		/// 
		/// </summary>
		protected int id;
		internal int ID
		{
			get
			{
				return this.id;
			}
			set
			{
				this.id=value;
			}
		}
		private Byte[] buffer;
		/// <summary>
		/// 
		/// </summary>
		protected string type;
		internal string Type
		{
			get
			{
				return type;
			}
		}
		internal string HeadR
		{
			get
			{
				return this.id.ToString()+" 0 R ";
			}
		}
		internal string HeadObj
		{
			get
			{
				return this.id.ToString()+" 0 obj\n";
			}
		}
		
		#endregion
		internal virtual int StreamWrite(System.IO.Stream stream){return 0;}
		internal PdfObject Clone()
		{
			return this.MemberwiseClone() as PdfObject;
		}
	}
}
