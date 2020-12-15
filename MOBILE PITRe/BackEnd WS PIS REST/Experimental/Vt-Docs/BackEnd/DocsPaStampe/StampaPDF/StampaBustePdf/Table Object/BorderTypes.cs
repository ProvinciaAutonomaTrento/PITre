using System;
using StampaPDF.StampaBustePdf.PDFObject;


namespace StampaPDF.StampaBustePdf.Table_Object
{
	/// <summary>
	/// describes a kind of border for a PdfTable
	/// </summary>
	public enum BorderType
	{
		/// <summary>
		/// table without borders
		/// </summary>
		None=0,
		/// <summary>
		/// table boreder on the bounds (just the encloding rectangle)
		/// </summary>
Bounds=1,
		/// <summary>
		/// only the row (horizontal) lines
		/// </summary>
Rows=2,
		/// <summary>
		/// table boreder on the bounds with horizontal lines
		/// </summary>
RowsAndBounds=3,
		/// <summary>
		/// only the column (vertical) lines
		/// </summary>
Columns=4,
		/// <summary>
		/// table boreder on the bounds with vertical lines
		/// </summary>
ColumnsAndBounds=5,
		/// <summary>
		/// table completely bordered (rectangle, rows and lines)
		/// </summary>
CompleteGrid=6,
		/// <summary>
		/// table with horizontal and vertical lines without the encloding rectangle
		/// </summary>
		RowsAndColumns=7
	}
}
