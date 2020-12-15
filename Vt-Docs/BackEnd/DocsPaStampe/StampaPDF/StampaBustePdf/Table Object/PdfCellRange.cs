using System;
using System.Drawing;
using System.Collections;
using StampaPDF.StampaBustePdf.PDFObject;

namespace StampaPDF.StampaBustePdf.Table_Object
{
	/// <summary>
	/// This class represent a range of Cell. Each method called will be applied to each Cell.
	/// </summary>
	public class PdfCellRange
	{
		internal int startRow,endRow,startColumn,endColumn;
		internal PdfTable owner;
		internal PdfCellRange(){}
		internal PdfCellRange(PdfTable owner,int startRow,int startColumn,int endRow,int endColumn)
		{
			object o=owner.Cell(startRow,startColumn);
			o=owner.Cell(endRow,endColumn);
			this.owner=owner;
			this.startColumn=startColumn;
			this.startRow=startRow;
			this.endColumn=endColumn;
			this.endRow=endRow;
		}
		internal PdfArea Area
		{
			get
			{
				return this.owner.Cell(startRow,startColumn).Area
					.Merge(this.owner.Cell(endRow,endColumn).Area);
			}
		}
		/// <summary>
		/// The entire collection of Cells.
		/// </summary>
		public ArrayList Cells
		{
			get
			{
				ArrayList al=new ArrayList();
				for (int r=startRow;r<=endRow;r++)
					for (int c=startColumn;c<=endColumn;c++)
						al.Add(this.owner.cells[r+","+c] as PdfCell);
                return al;
			}
		}
		/// <summary>
		/// Sets this Content Format to each Cell of the CellRange.
		/// </summary>
		/// <param name="Format"></param>
		public void SetContentFormat(string Format)
		{
			foreach (PdfCell pc in this.Cells) pc.SetContentFormat(Format);
		}
		/// <summary>
		/// sets this Content to each Cell of the CellRange.
		/// </summary>
		public void SetContent(object Content)
		{
			foreach (PdfCell pc in this.Cells) pc.SetContent(Content);
		}
		/// <summary>
		/// sets this Color as background to each Cell of the CellRange
		/// </summary>
		/// <param name="BackgroundColor"></param>
		public void SetBackgroundColor(Color BackgroundColor)
		{
			foreach (PdfCell rc in this.Cells)	rc.SetBackgroundColor(BackgroundColor);
		}
		/// <summary>
		/// sets those two colors as alternating backgrounds to each Row of the CellRange
		/// </summary>
		/// <param name="BackgroundColor"></param>
		/// <param name="AlternateBackgroundColor"></param>
		public void SetBackgroundColor(Color BackgroundColor,Color AlternateBackgroundColor)
		{
			this.SetBackgroundColor(AlternateBackgroundColor);
			for (int r=this.startRow;r<=this.endRow;r+=2)
				for (int c=this.startColumn;c<=this.endColumn;c++)
					this.owner.Cell(r,c).SetBackgroundColor(BackgroundColor);
		}
		/// <summary>
		/// sets this Color as Foreground Color to each Cell of the CellRange.
		/// </summary>
		/// <param name="Color"></param>
		public void SetForegroundColor(Color Color)
		{
			foreach (PdfCell rc in this.Cells)	rc.SetForegroundColor(Color);
		}
		/// <summary>
		/// sets Foreground and Background Colors of each Cell of the CellRange.
		/// </summary>
		/// <param name="ForegroundColor"></param>
		/// <param name="BackgroundColor"></param>
		/// <param name="AlternateBackgroundColor"></param>
		public void SetColors(Color ForegroundColor,Color BackgroundColor,Color AlternateBackgroundColor)
		{
			foreach (PdfCell rc in this.Cells)	rc.SetForegroundColor(ForegroundColor);
			this.SetBackgroundColor(AlternateBackgroundColor);
			for (int r=this.startRow;r<=this.endRow;r+=2)
				for (int c=this.startColumn;c<=this.endColumn;c++)
					this.owner.Cell(r,c).SetBackgroundColor(BackgroundColor);
		}
		/// <summary>
		/// sets this Foreground and Background Colors to each Cell of the CellRange.
		/// </summary>
		/// <param name="ForegroundColor"></param>
		/// <param name="BackgroundColor"></param>
		public void SetColors(Color ForegroundColor,Color BackgroundColor)
		{
			foreach (PdfCell rc in this.Cells)	rc.SetForegroundColor(ForegroundColor);
			foreach (PdfCell rc in this.Cells)	rc.SetBackgroundColor(BackgroundColor);
		}
		/// <summary>
		/// sets this content alignment to each Cell of the CellRange.
		/// </summary>
		/// <param name="ContentAlignment"></param>
		public void SetContentAlignment(ContentAlignment ContentAlignment)
		{
			foreach (PdfCell rc in this.Cells)	rc.ContentAlignment=ContentAlignment;
		}
		/// <summary>
		/// sets this font to each Cell of the CellRange.
		/// </summary>
		/// <param name="Font"></param>
		public void SetFont(Font Font)
		{
				foreach (PdfCell rc in this.Cells) rc.Font=Font;
		}
		/// <summary>
		/// sets transparent each Cell of the CellRange.
		/// </summary>
		public void SetTransparent()
		{
			foreach (PdfCell rc in this.Cells) rc.transparent=true;
		}
		/// <summary>
		/// sets this CellPadding to each Cell of the CellRange.
		/// </summary>
		/// <param name="CellPadding"></param>
		public void SetCellPadding(double CellPadding)
		{
			foreach (PdfCell pc in this.Cells) pc.SetCellPadding(CellPadding);
		}
		/// <summary>
		/// The CellRange will be collapsed into a single Cell (with the lowest row and column index)
		/// This method will automatically sets the colspan and rowspan of the first Cell of the Range.
		/// </summary>
		public void MergeCells()
		{
			try
			{
				PdfCell pc=this.owner.Rows[startRow][startColumn];
				pc.RowSpan=endRow-startRow+1;
				pc.ColSpan=endColumn-startColumn+1;
			}
			catch {throw new Exception("Impossible to merge the CellAreas");}
		}
	}
}
