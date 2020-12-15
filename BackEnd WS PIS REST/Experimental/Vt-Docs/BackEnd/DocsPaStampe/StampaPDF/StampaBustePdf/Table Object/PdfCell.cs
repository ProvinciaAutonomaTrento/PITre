using System;
using System.Drawing;
using StampaPDF.StampaBustePdf.PDFObject;


namespace StampaPDF.StampaBustePdf.Table_Object
{
	/// <summary>
	/// Cell of a PdfTable
	/// </summary>
	public class PdfCell
	{
		internal PdfDocument PdfDocument
		{
			get
			{
				return this.owner.PdfDocument;
			}
		}
		internal string stringFormat;
		internal double Height
		{
			get
			{
				return this.owner.Rows[row].Height;
			}
		}
		internal string text
		{
			get
			{
				string testo="";
				if (this.content!=null) testo=String.Format(this.stringFormat,this.content);
				return testo;
			}
		}
		internal double Width
		{
			get
			{
				return this.owner.Columns[column].CompensatedWidth;
			}
		}
		internal double cellPadding;
		internal int row,column;
		internal object content;
		/// <summary>
		/// gets the Content inside the Cell.
		/// </summary>
		public object Content
		{
			get
			{
				return this.content;
			}
		}
		internal int colSpan,rowSpan;
		/// <summary>
		/// gets or sets the Column Span of the Cell.
		/// </summary>
		public int ColSpan
		{
			get
			{
				return this.colSpan;
			}
			set
			{
				if (value<=0) throw new Exception("ColSpan must be grater than zero.");
				this.colSpan=value;
					for (int r=this.row;r<row+rowSpan;r++)
						for (int c=this.column+1;c<column+colSpan;c++)
							this.owner.Cell(r,c).isSpanned=true;
			
			}
		}
		/// <summary>
		/// gets or sets the Row Span of the Cell.
		/// </summary>
		public int RowSpan
		{
			get
			{
				return this.rowSpan;
			}
			set
			{
				if (value<=0) throw new Exception("RowSpan must be grater than zero.");
				this.rowSpan=value;
					for (int r=this.row+1;r<row+rowSpan;r++)
						for (int c=this.column;c<column+colSpan;c++)
							this.owner.Cell(r,c).isSpanned=true;
			
			}
		}
		internal PdfTable owner;
		internal Color foregroundColor;
		internal bool transparent;
		internal PdfArea area;
		private PdfArea _Area;
		internal PdfArea Area
		{
			get
			{
			//	if (_Area==null)
			//	{
			//		if ((this.rowSpan==1)&&(this.colSpan==1)) _Area=this.area;
					_Area=this.area.Merge(this.owner.Cell(row+rowSpan-1,column+colSpan-1).area);
			//	}
				return this._Area;
			}
		}
		internal Color backgroundColor;
		internal ContentAlignment ContentAlignment;
		internal Font Font;
		internal PdfCell(PdfTable owner,int row,int column,ContentAlignment ContentAlignment,Color ForegroundColor,Font Font,double CellPadding)
		{
			this.colSpan=1;
			this.rowSpan=1;
			this.row=row;
			this.stringFormat="{0}";
			this.transparent=true;
			this.Font=Font;
			this.owner=owner;
			this.column=column;
			this.ContentAlignment=ContentAlignment;
			this.foregroundColor=ForegroundColor;
			this.cellPadding=CellPadding;
		}
		
		internal bool isSpanned;
		private double _neededHeight=0;
		internal double neededHeight
		{
			get
			{
				if (this._neededHeight==0)
				{
					if (this.isSpanned) return 0;	
					this._neededHeight=this.minimumLines*this.Font.Size+this.cellPadding*2;
				}
				return this._neededHeight;
			}
		}
		internal PdfTextArea pcatemp;
		internal int minimumLines
		{
			get
			{
				double W=Width;
				if (this.colSpan>1)
				{
					for (int c=column;c<column+colSpan-1;c++)
						W+=this.owner.Cell(row,c).Width;
				}
				PdfArea pa=new PdfArea(this.PdfDocument,0,0,W,1000);
				this.pcatemp= new PdfTextArea(Font,foregroundColor,pa.InnerArea(this.cellPadding*2)
						,ContentAlignment,text);
				return this.pcatemp.RenderLines().Count;

			}
		}
		/// <summary>
		/// sets the font to be used in the Cell.
		/// </summary>
		/// <param name="Font"></param>
		public void SetFont(Font Font)
		{
			this.Font=Font;
		}
		/// <summary>
		/// sets the background color of the Cell.
		/// </summary>
		/// <param name="BackgroundColor"></param>
		public void SetBackgroundColor(Color BackgroundColor)
		{
			this.backgroundColor=BackgroundColor;
			this.transparent=false;
		}
		/// <summary>
		/// sets the foreground color of the Cell.
		/// </summary>
		/// <param name="ForegroundColor"></param>
		public void SetForegroundColor(Color ForegroundColor)
		{
			this.foregroundColor=ForegroundColor;
		}
		/// <summary>
		/// sets the Content Alignment of the Cell.
		/// </summary>
		/// <param name="ContentAlignment"></param>
		public void SetContentAlignment(ContentAlignment ContentAlignment)
		{
			this.ContentAlignment=ContentAlignment;
		}
		/// <summary>
		/// sets the background and foreground colors of the Cell.
		/// </summary>
		/// <param name="ForegroundColor"></param>
		/// <param name="BackgroundColor"></param>
		public void SetColors(Color ForegroundColor,Color BackgroundColor)
		{
			this.SetForegroundColor(ForegroundColor);
			this.SetBackgroundColor(BackgroundColor);
		}
		/// <summary>
		/// makes the Cell background a transparent layer.
		/// </summary>
		public void SetTransparent()
		{
			this.transparent=true;
		}
		/// <summary>
		/// sets the Padding of the Cell.
		/// </summary>
		/// <param name="CellPadding"></param>
		public void SetCellPadding(double CellPadding)
		{
			if (CellPadding<0) throw new Exception("CellPadding must be non-negative.");
			this.cellPadding=CellPadding;
		}
		/// <summary>
		/// Sets the content of the cell. 
		/// </summary>
		/// <param name="Content"></param>
		public void SetContent(object Content)
		{
			this.content=Content;
		}
		/// <summary>
		/// Sets the string Format of the Cell
		/// </summary>
		/// <param name="Format"></param>
		public void SetContentFormat(string Format)
		{
			this.stringFormat=Format;
		}
	}
}
