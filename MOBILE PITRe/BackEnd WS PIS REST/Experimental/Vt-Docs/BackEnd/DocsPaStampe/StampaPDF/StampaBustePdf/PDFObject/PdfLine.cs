using System;
using System.Drawing;
using System.Text;
using StampaPDF.StampaBustePdf.Table_Object;

namespace StampaPDF.StampaBustePdf.PDFObject
{
	/// <summary>
	/// a generic Line for a PdfPage
	/// </summary>
	public class PdfLine : PdfObject
	{
		internal PointF start,end;
		internal Color color;
		internal double strokeWidth;
		/// <summary>
		/// gets or sets the Color of the line
		/// </summary>
		public Color Color
		{
			set
			{
				this.color=value;
			}
			get
			{
				return this.color;
			}
		}
		/// <summary>
		/// gets or sets the width of the stroke
		/// </summary>
		public double StrokeWidth
		{
			set
			{
				if (value<=0) throw new Exception("StrokeWidth must be greater than zero.");
				this.strokeWidth=value;
			}
			get
			{
				return this.strokeWidth;
			}
		}
		/// <summary>
		/// gets or sets the starting point of the line
		/// </summary>
		public PointF Start
		{
			get
			{
				return this.start;
			}
			set
			{
				this.start=value;
			}
		}
		/// <summary>
		/// gets or sets the ending point of the line
		/// </summary>
		public PointF End
		{
			get
			{
				return this.end;
			}
			set
			{
				this.end=value;
			}
		}
		/// <summary>
		/// created a new line to put inside a PdfPage
		/// </summary>
		/// <param name="Start">the starting point of the line</param>
		/// <param name="End">the ending point of the line</param>
		/// <param name="Color">the Color of the line</param>
		/// <param name="StrokeWidth">the width of the stroke</param>
		public PdfLine(PdfDocument PdfDocument,PointF Start,PointF End,Color Color,double StrokeWidth)
		{
			this.PdfDocument=PdfDocument;
			if (StrokeWidth<=0) throw new Exception("StrokeWidth must be greater than zero.");
			this.start=Start;
			this.end=End;
			this.color=Color;
			this.strokeWidth=StrokeWidth;
		}
		
		internal string ToLineStream()
		{
			System.Text.StringBuilder sb=new StringBuilder();
			
			sb.Append(this.start.X.ToString("0.##").Replace(",",".")+" ");
			sb.Append((this.PdfDocument.PH-this.start.Y).ToString("0.##").Replace(",",".")+" ");
			sb.Append("m ");
			
			sb.Append(this.end.X.ToString("0.##").Replace(",",".")+" ");
			sb.Append((this.PdfDocument.PH-this.end.Y).ToString("0.##").Replace(",",".")+" ");
			sb.Append("l ");

			sb.Append("S\n");
			return sb.ToString();
			
		}
		internal string ToLineStreamWithColorAndWidth()
		{
			System.Text.StringBuilder sb=new StringBuilder();
			sb.Append(Utility.ColorRGLine(this.color));
			sb.Append(this.strokeWidth.ToString("0.##").Replace(",",".")+" ");
			sb.Append("w\n");
			sb.Append(this.ToLineStream());
			return sb.ToString();
		}
		
		internal override int StreamWrite(System.IO.Stream stream)
		{
			int num=this.id;
			string text="";
			text+=this.ToLineStreamWithColorAndWidth();		
			System.Text.StringBuilder sb=new StringBuilder();
			sb.Append(num.ToString()+" 0 obj\n");
			sb.Append("<< /Lenght "+text.Length+" >>\n");
			sb.Append("stream\n");
			sb.Append(text);
			sb.Append("endstream\n");
			sb.Append("endobj\n");
			Byte[] b=ASCIIEncoding.ASCII.GetBytes(sb.ToString());
			stream.Write(b,0,b.Length);
			return b.Length;
		}

	}
}
