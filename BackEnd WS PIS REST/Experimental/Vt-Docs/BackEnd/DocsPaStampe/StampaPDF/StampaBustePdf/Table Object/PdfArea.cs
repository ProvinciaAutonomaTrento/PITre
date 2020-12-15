using System;
using System.Drawing;
using StampaPDF.StampaBustePdf.PDFObject;
using StampaPDF.StampaBustePdf.Table_Object;

namespace StampaPDF.StampaBustePdf
{
	/// <summary>
	/// Summary description for PdfArea.
	/// </summary>
	public class PdfArea
	{
		/// <summary>
		/// 
		/// </summary>
		internal PdfDocument PdfDocument;
		internal double posx,posy,width,height;
		/// <summary>
		/// gets or sets the X positioning of the Area
		/// </summary>
		public double PosX
		{
			get
			{
				return this.posx;
			}
			set
			{
				this.posx=value;
			}
		}
		/// <summary>
		/// gets or sets the Y positioning of the Area
		/// </summary>
		public double PosY
		{
			get
			{
				return this.posy;
			}
			set
			{
				this.posy=value;
			}
		}
		/// <summary>
		/// gets the X-coordinate of the center of the Area.
		/// </summary>
		public double CenterX
		{
			get
			{
				return this.posx+(this.BottomRightCornerX-this.posx)/2;
			}
			
		}
		/// <summary>
		/// gets the Y-coordinate of the center of the Area.
		/// </summary>
		public double CenterY
		{
			get
			{
				return this.posy+(this.BottomRightCornerY-this.posy)/2;
			}
			
		}
		/// <summary>
		/// gets or sets the Width of the Area.
		/// </summary>
		public double Width
		{
			get
			{
				return this.width;
			}
			set
			{
				if (value<=0) throw new Exception("Width must be grater than zero.");
				this.width=value;
			}
		}
		/// <summary>
		/// gets or sets the Height of the Area.
		/// </summary>
		public double Height
		{
			get
			{
				return this.height;
			}
			set
			{
				//if (value<=0) throw new Exception("Height must be grater than zero.");
				this.height=value;
			}
		}
		/// <summary>
		/// gets the coordinates struct of the Top Left Vertex of the Area
		/// </summary>
		public PointF TopLeftVertex
		{
			get
			{
				return new System.Drawing.PointF((float)this.posx,(float)this.posy);
			}
		}
		/// <summary>
		/// gets the coordinates struct of the Bottom Right Vertex of the Area
		/// </summary>
		public PointF BottomRightVertex
		{
			get
			{
				return new PointF((float)this.BottomRightCornerX,(float)this.BottomRightCornerY);
			}
		}
		/// <summary>
		/// gets the coordinates struct of the Bottom Left Vertex of the Area
		/// </summary>
		public PointF BottomLeftVertex
		{
			get
			{
				return new PointF((float)this.posx,(float)this.BottomRightCornerY);
			}
		}
		/// <summary>
		/// gets the coordinates struct of the Top Right Vertex of the Area
		/// </summary>
		public PointF TopRightVertex
		{
			get
			{
				return new PointF((float)this.BottomRightCornerX,(float)this.posy);
			}
		}
		internal string Key
		{
			get
			{
				return this.posx.ToString()+" "+this.posy.ToString()+" "+
					this.width.ToString()+" "+this.height.ToString();
			}
		}
		/// <summary>
		/// Creates a new Area for correctly placing objects into Pdf Pages
		/// </summary>
		/// <param name="posx">Top-Left Vertex X-coordinate</param>
		/// <param name="posy">Top-Left Vertex Y-coordinate</param>
		/// <param name="width">Width of the Area</param>
		/// <param name="height">Height of the Area</param>
		public PdfArea (PdfDocument PdfDocument,double posx,double posy,double width,double height)
		{
			this.PdfDocument=PdfDocument;
			if (width<=0) throw new Exception("Width must be grater than zero.");
			if (height<=0) throw new Exception("Height must be grater than zero.");
			this.PosX=(double)posx;
			this.PosY=(double)posy;
			this.Width=(double)width;
			this.Height=(double)height;
			
		}
		internal PdfArea (PdfDocument PdfDocument){this.PdfDocument=PdfDocument;}
		/// <summary>
		/// Generates a new Area inside the base one specifing the width difference.
		/// </summary>
		/// <param name="Difference">the Width difference of the inner Area</param>
		/// <returns></returns>
		public PdfArea InnerArea(double Difference)
		{
			this.PdfDocument=PdfDocument;
			if (Difference<0) throw new Exception("Difference must be non negative.");
			PdfArea pa=this.MemberwiseClone() as PdfArea;
			pa.width-=(double)Difference;
			pa.Height-=(double)Difference;
			pa.posx+=(double)Difference/2;
			pa.posy+=(double)Difference/2;
			return pa;
		}
		/// <summary>
		/// Generates a new Area inside the base one specifing the width difference.
		/// </summary>
		/// <param name="WidthDifference">the Width difference of the inner Area</param>
		/// <param name="HeightDifference">the Height difference of the inner Area</param>
		/// <returns></returns>
		public PdfArea InnerArea(double WidthDifference,double HeightDifference)
		{
			if (WidthDifference<0) throw new Exception("WidthDifference must be non negative.");
			if (HeightDifference<0) throw new Exception("HeightDifference must be non negative.");
			PdfArea pa=this.MemberwiseClone() as PdfArea;
			pa.width-=(double)WidthDifference;
			pa.Height-=(double)HeightDifference;
			pa.posx+=(double)WidthDifference/2;
			pa.posy+=(double)HeightDifference/2;
			return pa;
		}
		/// <summary>
		/// Generates a new Area outside the base one specifing the width difference
		/// </summary>
		/// <param name="Difference">the Width difference of the inner Area</param>
		/// <returns></returns>
		public PdfArea OuterArea(double Difference)
		{
			if (Difference<0) throw new Exception("Difference must be non negative.");
			PdfArea pa=this.MemberwiseClone() as PdfArea;
			pa.width+=(double)Difference;
			pa.Height+=(double)Difference;
			pa.posx-=(double)Difference/2;
			pa.posy-=(double)Difference/2;
			return pa;
		}
		/// <summary>
		/// Generates a new Area outside the base one specifing the width difference.
		/// </summary>
		/// <param name="WidthDifference">the Width difference of the inner Area</param>
		/// <param name="HeightDifference">the Height difference of the inner Area</param>
		/// <returns></returns>
		public PdfArea OuterArea(double WidthDifference,double HeightDifference)
		{
			if (WidthDifference<0) throw new Exception("WidthDifference must be non negative.");
			if (HeightDifference<0) throw new Exception("HeightDifference must be non negative.");
			PdfArea pa=this.MemberwiseClone() as PdfArea;
			pa.width+=(double)WidthDifference;
			pa.Height+=(double)HeightDifference;
			pa.posx-=(double)WidthDifference/2;
			pa.posy-=(double)HeightDifference/2;
			return pa;
		}
		/// <summary>
		/// Extends the area to the bounds of another area
		/// </summary>
		/// <param name="Area"></param>
		/// <returns></returns>
		public PdfArea Merge(PdfArea Area)
		{
			PdfArea res=this.MemberwiseClone() as PdfArea;

			if (this.PosX>Area.PosX) 
			{
				res.PosX=Area.PosX;
				res.Width=Area.Width;
			}
			if (this.PosY>Area.PosY) 
			{
				res.PosY=Area.PosY;
				res.Height=Area.Height;
			}

			if (this.BottomRightCornerX<Area.BottomRightCornerX)
			{
				res.BottomRightCornerX=Area.BottomRightCornerX;
			}
			if (this.BottomRightCornerY<Area.BottomRightCornerY)
			{
				res.BottomRightCornerY=Area.BottomRightCornerY;
			}
			return res;
		}
		/// <summary>
		/// get or sets the bottom-right bound X-coordinate
		/// </summary>
		public double BottomRightCornerX
		{
			get
			{
				return this.PosX+this.Width;
			}
			set
			{
				this.width=value-this.posx;
				if (width<=0) throw new Exception("Width must be grater than zero.");
			
			}
		}
		/// <summary>
		/// get or sets the bottom-right boud Y-coordinate
		/// </summary>
		public double BottomRightCornerY
		{
			get
			{
				return this.PosY+this.Height;
			}
			set
			{
				this.height=value-this.posy;
				if (height<=0) throw new Exception("Height must be grater than zero.");
			
			}
		}
		/// <summary>
		/// Creates a Circle inside the Area
		/// </summary>
		/// <param name="BorderColor"></param>
		/// <returns></returns>
		public PdfCircle InnerCircle(System.Drawing.Color BorderColor)
		{
			PdfCircle pc=new PdfCircle();
			pc.AxesArea=this;
			pc.BorderColor=BorderColor;
			return pc;
		}
		/// <summary>
		/// Creates a Circle outside the Area
		/// </summary>
		/// <param name="BorderColor"></param>
		/// <returns></returns>
		public PdfCircle OuterCircle(System.Drawing.Color BorderColor)
		{
			PdfCircle pc=new PdfCircle();
			pc.AxesArea=this.OuterArea(this.width*(double)0.40,this.height*(double)0.40);
			pc.BorderColor=BorderColor;
			return pc;
		}
		/// <summary>
		/// Creates a Circle inside the Area
		/// </summary>
		/// <param name="BorderColor"></param>
		/// <param name="StrokeWidth"></param>
		/// <returns></returns>
		public PdfCircle InnerCircle(System.Drawing.Color BorderColor,double StrokeWidth)
		{
			if (StrokeWidth<=0) throw new Exception("StrokeWidth must be grater than zero.");
			PdfCircle pc=new PdfCircle();
			pc.AxesArea=this;
			pc.BorderColor=BorderColor;
			pc.strokeWidth=StrokeWidth;
			return pc;
		}
		/// <summary>
		/// Creates a Circle outside the Area
		/// </summary>
		/// <param name="BorderColor"></param>
		/// <param name="StrokeWidth"></param>
		/// <returns></returns>
		public PdfCircle OuterCircle(System.Drawing.Color BorderColor,double StrokeWidth)
		{
			if (StrokeWidth<=0) throw new Exception("StrokeWidth must be grater than zero.");
			PdfCircle pc=new PdfCircle();
			pc.AxesArea=this.OuterArea(this.width*(double)0.40,this.height*(double)0.40);
			pc.BorderColor=BorderColor;
			pc.strokeWidth=StrokeWidth;
			return pc;
		}
		/// <summary>
		/// Creates a line which crosses the Area from the bottom-left to the top-right corner
		/// </summary>
		/// <param name="Color"></param>
		/// <returns></returns>
		public PdfLine SlashLine(Color Color)
		{
			return new PdfLine(this.PdfDocument,this.BottomLeftVertex,this.TopRightVertex,Color,1);
		}
		/// <summary>
		/// Creates a line which crosses the Area from the top-left to the bottom-right corner
		/// </summary>
		/// <param name="Color"></param>
		/// <returns></returns>
		public PdfLine BackSlashLine(Color Color)
		{
			return new PdfLine(this.PdfDocument,this.TopLeftVertex,this.BottomRightVertex,Color,1);
		}
		/// <summary>
		/// Creates a line which crosses the Area from the bottom-left to the top-right corner
		/// </summary>
		/// <param name="Color"></param>
		/// <param name="StrokeWidth"></param>
		/// <returns></returns>
		public PdfLine SlashLine(Color Color,double StrokeWidth)
		{
			if (StrokeWidth<=0) throw new Exception("StrokeWidth must be grater than zero.");
			return new PdfLine(this.PdfDocument,this.BottomLeftVertex,this.TopRightVertex,Color,(double)StrokeWidth);
		}
		/// <summary>
		/// Creates a line which crosses the Area from the top-left to the bottom-right corner
		/// </summary>
		/// <param name="Color"></param>
		/// <param name="StrokeWidth"></param>
		/// <returns></returns>
		public PdfLine BackSlashLine(Color Color,double StrokeWidth)
		{
			if (StrokeWidth<=0) throw new Exception("StrokeWidth must be grater than zero.");
			return new PdfLine(this.PdfDocument,this.TopLeftVertex,this.BottomRightVertex,Color,(double)StrokeWidth);
		}
		/// <summary>
		/// Creates a line corresponding to the Upper Bound of the Area
		/// </summary>
		/// <param name="Color"></param>
		/// <param name="StrokeWidth"></param>
		/// <returns></returns>
		public PdfLine UpperBound(Color Color,double StrokeWidth)
		{
			if (StrokeWidth<=0) throw new Exception("StrokeWidth must be grater than zero.");
			return new PdfLine(this.PdfDocument,this.TopLeftVertex,this.TopRightVertex,Color,StrokeWidth);
		}
		/// <summary>
		/// Creates a line corresponding to the lower bound of the Area
		/// </summary>
		/// <param name="Color"></param>
		/// <param name="StrokeWidth"></param>
		/// <returns></returns>
		public PdfLine LowerBound(Color Color,double StrokeWidth)
		{
			if (StrokeWidth<=0) throw new Exception("StrokeWidth must be grater than zero.");
			return new PdfLine(this.PdfDocument,this.BottomLeftVertex,this.BottomRightVertex,Color,StrokeWidth);
		}
		/// <summary>
		/// Creates a line corresponding to the left bound of the Area
		/// </summary>
		/// <param name="Color"></param>
		/// <param name="StrokeWidth"></param>
		/// <returns></returns>
	
		public PdfLine LeftBound(Color Color,double StrokeWidth)
		{
			if (StrokeWidth<=0) throw new Exception("StrokeWidth must be grater than zero.");
			return new PdfLine(this.PdfDocument,this.TopLeftVertex,this.BottomLeftVertex,Color,StrokeWidth);
		}
		/// <summary>
		/// Creates a line corresponding to the Horizontal Axe of the Area
		/// </summary>
		/// <param name="Color"></param>
		/// <param name="StrokeWidth"></param>
		/// <returns></returns>
		public PdfLine HorizontalAxe(Color Color,double StrokeWidth)
		{
			if (StrokeWidth<=0) throw new Exception("StrokeWidth must be grater than zero.");
			System.Drawing.PointF p1=this.TopLeftVertex;
			System.Drawing.PointF p2=this.TopRightVertex;
			p1.Y+=(float)this.height/2;
			p2.Y+=(float)this.height/2;
			return new PdfLine(this.PdfDocument,p1,p2,Color,StrokeWidth);
		}
		/// <summary>
		/// Creates a line corresponding to the Vertical Axe of the Area
		/// </summary>
		/// <param name="Color"></param>
		/// <param name="StrokeWidth"></param>
		/// <returns></returns>
	
		public PdfLine VerticalAxe(Color Color,double StrokeWidth)
		{
			if (StrokeWidth<=0) throw new Exception("StrokeWidth must be grater than zero.");
			System.Drawing.PointF p1=this.TopLeftVertex;
			System.Drawing.PointF p2=this.BottomLeftVertex;
			p1.X+=(float)this.width/2;
			p2.X+=(float)this.width/2;
			return new PdfLine(this.PdfDocument,p1,p2,Color,StrokeWidth);
		}
		/// <summary>
		/// Creates a line corresponding to the right bound of the Area.
		/// </summary>
		/// <param name="Color"></param>
		/// <param name="StrokeWidth"></param>
		/// <returns></returns>
		public PdfLine RightBound(Color Color,double StrokeWidth)
		{
			if (StrokeWidth<=0) throw new Exception("StrokeWidth must be grater than zero.");
			return new PdfLine(this.PdfDocument,this.TopRightVertex,this.BottomRightVertex,Color,StrokeWidth);
		}
		/// <summary>
		/// Creates a simple void rectangle delimited by this Area.
		/// </summary>
		/// <param name="BorderColor"></param>
		/// <returns></returns>
		public PdfRectangle ToRectangle(Color BorderColor)
		{
			PdfRectangle pr=new PdfRectangle(this.PdfDocument,this,BorderColor);
			return pr;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="BorderColor"></param>
		/// <param name="BorderWidth"></param>
		/// <returns></returns>
		public PdfRectangle ToRectangle(Color BorderColor,double BorderWidth)
		{
			if (BorderWidth<=0) throw new Exception("BorderWidth must be grater than zero.");
			PdfRectangle pr=new PdfRectangle(this.PdfDocument,this,BorderColor,BorderWidth);
			return pr;
		}
		/// <summary>
		/// Creates a simple void rectangle delimited by this Area.
		/// </summary>
		/// <param name="BorderColor"></param>
		/// <param name="FillingColor"></param>
		/// <returns></returns>
		public PdfRectangle ToRectangle(Color BorderColor,Color FillingColor)
		{
			PdfRectangle pr=new PdfRectangle(this.PdfDocument,this,BorderColor,FillingColor);
			return pr;
		}
		/// <summary>
		/// Creates a filled rectangle delimited by this Area.
		/// </summary>
		/// <param name="BorderColor"></param>
		/// <param name="BorderWidth"></param>
		/// <param name="FillingColor"></param>
		/// <returns></returns>
		public PdfRectangle ToRectangle(Color BorderColor,double BorderWidth,Color FillingColor)
		{
			if (BorderWidth<=0) throw new Exception("BorderWidth must be grater than zero.");
			PdfRectangle pr=new PdfRectangle(this.PdfDocument,this,BorderColor,BorderWidth,FillingColor);
			return pr;
		}
		internal PdfArea Clone()
		{
			return this.MemberwiseClone() as PdfArea;
		}
		
	}
}
