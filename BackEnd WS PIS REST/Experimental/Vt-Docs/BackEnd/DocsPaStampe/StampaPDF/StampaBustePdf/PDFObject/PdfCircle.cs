using System;
using System.Drawing;
using System.Text;
using StampaPDF.StampaBustePdf.Table_Object;

namespace StampaPDF.StampaBustePdf.PDFObject
{
	/// <summary>
	/// a circle to put inside a PdfPage
	/// </summary>
	public class PdfCircle : PdfObject
	{
		private PdfArea axesArea;
		/// <summary>
		/// gets or sets the Area of defined by the horizontal and vertical axes
		/// </summary>
		public PdfArea AxesArea
		{
			get
			{
				return this.axesArea;
			}
			set
			{
				this.axesArea=value;
			}
		}
		internal double strokeWidth;
		/// <summary>
		/// gets or sets the stroke width (default value is 1)
		/// </summary>
		public double StrokeWidth
		{
			set
			{
				if (value<=0) throw new Exception("StrokeWidth must be grater than zero.");
				this.strokeWidth=value;
			}
			get
			{
				return this.strokeWidth;
			}
		}		
		internal Color BorderColor;
		/// <summary>
		/// gets the center coordinates
		/// </summary>
		public System.Drawing.PointF Center
		{
			get
			{
				return new PointF((float)(this.axesArea.PosX+(this.axesArea.Width/2))
					,(float)(this.axesArea.PosY+(this.axesArea.Height/2)));
			}
		}
		internal PdfCircle()
		{
			this.strokeWidth=1;
		}
		/// <summary>
		/// creates a new circle
		/// </summary>
		/// <param name="posx">center's x coordinate</param>
		/// <param name="posy">center's y coordinate</param>
		/// <param name="ray">ray measure</param>
		/// <param name="Color">circumference color</param>
		public PdfCircle(double posx,double posy,double ray,Color Color)
		{
			if (ray<=0) throw new Exception("Ray must be grater than zero.");
			this.axesArea=new PdfArea(this.PdfDocument,posx-ray,posy-ray,ray*2,ray*2);
			this.BorderColor=Color;
			this.strokeWidth=1;
		}
		/// <summary>
		/// creates a circle with different x and y diamaters (ellipse)
		/// </summary>
		/// <param name="posx">center's x coordinate</param>
		/// <param name="posy">center's y coordinate</param>
		/// <param name="XDiameter">x diameter measure</param>
		/// <param name="YDiameter">y diameter measure</param>
		/// <param name="Color">circumference color</param>
		public PdfCircle(double posx,double posy,double XDiameter,double YDiameter,Color Color)
		{
			if (XDiameter<=0) throw new Exception("XDiameter must be grater than zero.");
			if (YDiameter<=0) throw new Exception("YDiameter must be grater than zero.");

			this.axesArea=new PdfArea(this.PdfDocument,posx-XDiameter/2,posy-YDiameter/2,XDiameter,YDiameter);
			this.BorderColor=Color;
			this.strokeWidth=1;
		}
		/// <summary>
		/// sets the diameters lenght of the circle.
		/// </summary>
		/// <param name="XDiameter"></param>
		/// <param name="YDiameter"></param>
		public void SetDiameters(double XDiameter,double YDiameter)
		{
			if (XDiameter<=0) throw new Exception("XDiameter must be grater than zero.");
			if (YDiameter<=0) throw new Exception("YDiameter must be grater than zero.");

			this.axesArea=new PdfArea(this.PdfDocument,this.axesArea.CenterX-XDiameter/2,this.axesArea.CenterY-YDiameter/2,XDiameter,YDiameter);
		}
		/// <summary>
		/// sets the ray lenght of the circle.
		/// </summary>
		/// <param name="Ray"></param>
		public void SetRay(double Ray)
		{
			if (Ray<=0) throw new Exception("Ray must be grater than zero.");
			this.SetDiameters(Ray*2,Ray*2);
		}
		internal string ToLineStream()
		{
			string text="";
			
			text+=Utility.ColorRGLine(this.BorderColor);
			
			text+=this.strokeWidth.ToString("0.##")+" ";
			text+="w\n";
			
			text+=this.Center.X.ToString("0.##")+" ";
			text+=(this.PdfDocument.PH-this.axesArea.PosY).ToString("0.##")+" m\n";

			text+=(this.axesArea.BottomRightCornerX+this.axesArea.Width/6).ToString("0.##")+" ";
			text+=(this.PdfDocument.PH-this.axesArea.PosY).ToString("0.##")+" ";
			text+=(this.axesArea.BottomRightCornerX+this.axesArea.Width/6).ToString("0.##")+" ";
			text+=(this.PdfDocument.PH-this.axesArea.BottomRightCornerY).ToString("0.##")+" ";
			text+=this.Center.X.ToString("0.##")+" ";
			text+=(this.PdfDocument.PH-this.axesArea.BottomRightCornerY).ToString("0.##")+" c \n";
			
			text+=(this.axesArea.PosX-this.axesArea.Width/6).ToString("0.##")+" ";
			text+=(this.PdfDocument.PH-this.axesArea.BottomRightCornerY).ToString("0.##")+" ";
			text+=(this.axesArea.PosX-this.axesArea.Width/6).ToString("0.##")+" ";
			text+=(this.PdfDocument.PH-this.axesArea.PosY).ToString("0.##")+" ";
			text+=this.Center.X.ToString("0.##")+" ";
			text+=(this.PdfDocument.PH-this.axesArea.PosY).ToString("0.##")+" c\n";

			text+="s\n";
		
			text=text.Replace(",",".");
			return text;
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
			
			string s3="";
			s3+="\nendstream\n";
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
