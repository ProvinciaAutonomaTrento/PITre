using System;
using System.Text;
using System.Drawing;
using StampaPDF.StampaBustePdf.Table_Object;

namespace StampaPDF.StampaBustePdf.PDFObject
{
	/// <summary>
	/// a generic Rectangle for a PdfPage
	/// </summary>
	public class PdfRectangle : PdfObject
	{
		
		internal PdfArea rectangleArea;
		/// <summary>
		/// gets the area of the rectangle
		/// </summary>
		public PdfArea RectangleArea
		{
			get
			{
				return this.rectangleArea;
			}
		}
		internal Color BorderColor=Color.Black,FillingColor;
		internal bool filled;
		internal double strokeWidth;
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
		/// creates a new rectangle
		/// </summary>
		/// <param name="RectangleArea">the area which will contains the rectangle</param>
		/// <param name="BorderColor"></param>
		public PdfRectangle(PdfDocument PdfDocument,PdfArea RectangleArea,Color BorderColor)
		{
			this.PdfDocument=PdfDocument;
			this.rectangleArea=RectangleArea;
			this.BorderColor=BorderColor;
			this.strokeWidth=1;
		}
		/// <summary>
		/// creates a new rectangle 
		/// </summary>
		/// <param name="RectangleArea"></param>
		/// <param name="BorderColor"></param>
		/// <param name="BorderWidth"></param>
		public PdfRectangle(PdfDocument PdfDocument,PdfArea RectangleArea,Color BorderColor,double BorderWidth)
		{
			this.PdfDocument=PdfDocument;
			if (BorderWidth<=0) throw new Exception("BorderWidth must be greater than zero.");
			this.rectangleArea=RectangleArea;
			this.BorderColor=BorderColor;
			this.strokeWidth=BorderWidth;
		}
		/// <summary>
		/// creates a new rectangle
		/// </summary>
		/// <param name="RectangleArea"></param>
		/// <param name="BorderColor"></param>
		/// <param name="FillingColor"></param>
		public PdfRectangle(PdfDocument PdfDocument,PdfArea RectangleArea,Color BorderColor,Color FillingColor)
		{
			this.PdfDocument=PdfDocument;
			this.rectangleArea=RectangleArea;
			this.BorderColor=BorderColor;
			this.FillingColor=FillingColor;
			this.filled=true;
			this.strokeWidth=1;
		}
		/// <summary>
		/// creates a new rectangle
		/// </summary>
		/// <param name="RectangleArea"></param>
		/// <param name="BorderColor"></param>
		/// <param name="BorderWidth"></param>
		/// <param name="FillingColor"></param>
		public PdfRectangle(PdfDocument PdfDocument,PdfArea RectangleArea,Color BorderColor,double BorderWidth,Color FillingColor)
		{
			this.PdfDocument=PdfDocument;
			if (BorderWidth<=0) throw new Exception("BorderWidth must be greater than zero.");
			this.rectangleArea=RectangleArea;
			this.BorderColor=BorderColor;
			this.FillingColor=FillingColor;
			this.filled=true;
			this.strokeWidth=BorderWidth;
		}
		/// <summary>
		/// fills the rectangle with a Color
		/// </summary>
		/// <param name="Color"></param>
		public void Fill(Color Color)
		{
			this.BorderColor=Color;
			this.FillingColor=Color;
			this.filled=true;
		}
		/// <summary>
		/// sets the color of rectangle's border
		/// </summary>
		/// <param name="Color"></param>
		public void Border(Color Color)
		{
			this.BorderColor=Color;
		}
		internal string ToColorAndWidthStream()
		{
			System.Text.StringBuilder sb=new StringBuilder();
			sb.Append(Utility.ColorRGLine(this.BorderColor));
			if (filled) sb.Append(Utility.ColorrgLine(this.FillingColor));
			sb.Append(this.strokeWidth.ToString("0.##").Replace(",",".")+" ");
			sb.Append("w\n");
			return sb.ToString();
		}
		internal string ToRectangleStream()
		{
			System.Text.StringBuilder sb=new StringBuilder();
			
			sb.Append(this.RectangleArea.PosX.ToString("0.##").Replace(",","."));
			sb.Append(" "+(this.PdfDocument.PH-this.rectangleArea.PosY-this.RectangleArea.Height).ToString("0.##").Replace(",",".")+" ");
			sb.Append(this.RectangleArea.Width.ToString("0.##").Replace(",","."));
			sb.Append(" "+this.RectangleArea.Height.ToString("0.##").Replace(",",".")+" re ");
			if (filled) sb.Append("B");else sb.Append("s");
			sb.Append("\n");
			return sb.ToString();
		}
		internal string ToLineStream()
		{
			return this.ToColorAndWidthStream()+this.ToRectangleStream();
		}
		internal override int StreamWrite(System.IO.Stream stream)
		{
			int num=this.id;

			string text=this.ToLineStream();
			Byte[] part2;
				
			if (PdfDocument.FlateCompression) part2=Utility.Deflate(text); else
				part2=System.Text.ASCIIEncoding.ASCII.GetBytes(text);

			string s1="";
			s1+=num.ToString()+" 0 obj\n";
			s1+="<< /Lenght "+part2.Length;
			if (PdfDocument.FlateCompression) s1+=" /Filter /FlateDecode";
			s1+=">>\n";
				
			s1+="stream\n";
				
			string s3="\nendstream\n";
			s3+="endobj\n";

			Byte[] part1=System.Text.ASCIIEncoding.ASCII.GetBytes(s1);
			Byte[] part3=System.Text.ASCIIEncoding.ASCII.GetBytes(s3);
			stream.Write(part1,0,part1.Length);
			stream.Write(part2,0,part2.Length);
			stream.Write(part3,0,part3.Length);
			return part1.Length+part2.Length+part3.Length;
		}

	}
}
