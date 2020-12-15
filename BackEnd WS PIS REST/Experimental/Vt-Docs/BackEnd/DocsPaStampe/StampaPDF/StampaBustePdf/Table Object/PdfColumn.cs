using System;
using System.Drawing;
using System.Collections;
using StampaPDF.StampaBustePdf.PDFObject;

namespace StampaPDF.StampaBustePdf.Table_Object
{
	/// <summary>
	/// a Column of a PdfTable
	/// </summary>
	public class PdfColumn : PdfCellRange
	{
		internal int index;
		internal double Width;
		private double compansatedWidth=-1;

		internal double CompensatedWidth
		{
			get
			{
				if (this.compansatedWidth==-1)
				{
					double sum=0;
					foreach (PdfColumn pc in this.owner.Columns)
					{
						sum+=pc.Width;
					}
					this.compansatedWidth=(this.owner.TableArea.width/sum)*this.Width;
				}
				return this.compansatedWidth;
			}
			set
			{
				this.compansatedWidth=value;
			}
		}
		internal PdfColumn(PdfTable owner,int index)
		{
			this.owner=owner;
			this.index=index;
			this.startColumn=index;
			this.endColumn=index;
			this.startRow=0;
			this.endRow=this.owner.rows-1;
		}
		/// <summary>
		/// sets the Relative Width of the Column. For example: if the relative widths of a 3 columns
		/// table are 10,10,30; the columns will respectivelly sized as 20%,20%,60% of the table size.
		/// </summary>
		/// <param name="RelativeWidth"></param>
		public void SetWidth(int RelativeWidth)
		{
			if (RelativeWidth<=0) throw new Exception("RelativeWidth must be grater than zero.");
			this.Width=RelativeWidth;
			if (this.owner.header!=null) this.owner.header.Columns[this.index].SetWidth(RelativeWidth);
		}
		
	}
}
