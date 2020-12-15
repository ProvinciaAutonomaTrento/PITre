using System;
using System.Collections;
using System.Drawing;
using StampaPDF.StampaBustePdf.PDFObject;

namespace StampaPDF.StampaBustePdf.Table_Object
{
	/// <summary>
	/// a Row of a PdfTable
	/// </summary>
	public class PdfRow : PdfCellRange
	{
		internal double height=0;
		internal int index;
		/// <summary>
		/// returns the index of the row
		/// </summary>
		public int Index
		{
			get
			{
				return this.index;
			}
		}
		internal PdfRow(PdfTable owner,int index)
		{
			this.owner=owner;
			this.index=index;
			this.startRow=index;
			this.endRow=index;
			this.startColumn=0;
			this.endColumn=this.owner.columns-1;
		}
		/// <summary>
		/// returns a specific cell of the row
		/// </summary>
		public PdfCell this[int column]
		{
			get
			{
				PdfCell pc=this.owner.Cell(this.index,column);
				if (pc==null) throw new Exception("Column "+column+" does not exist");
				return pc;
			}
		}
		/// <summary>
		/// returns the Height of the row
		/// </summary>
		public double Height
		{
			get
			{
			//	if (this.height==0)
			//	{
					double min=0;
					foreach (PdfCell pc in this.Cells)
					{
						double nh=pc.neededHeight;
						if (nh>min) min=nh;
					}
					this.height=min;//+this.owner.cellPadding*2;
			//	}
				return this.height;
			}
			
		}
		/// <summary>
		/// forces the height of the Row
		/// </summary>
		/// <param name="RowHeight">the forced height of the row</param>
		public void SetRowHeight(double RowHeight)
		{
			if (RowHeight<=0) throw new Exception("RowHeight must be grater than zero.");
			this.height=RowHeight;
		}
	}
}
