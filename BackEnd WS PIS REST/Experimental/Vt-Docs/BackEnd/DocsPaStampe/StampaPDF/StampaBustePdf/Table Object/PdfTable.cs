using System;
using System.Data;
using System.Collections;
using System.Text;
using System.Drawing;
using System.IO;
using StampaPDF.StampaBustePdf.PDFObject;

namespace StampaPDF.StampaBustePdf.Table_Object
{
	/// <summary>
	/// Summary description for PdfTable.
	/// </summary>
	public class PdfTable : PdfCellRange
	{
		internal int columns,rows;
		internal Hashtable cells;
		internal PdfArea TableArea;
		internal PdfTable header=null;
		internal PdfDocument PdfDocument;
		internal int renderingIndex;
		internal int renderingRows;
		internal bool visibleHeaders=true;
		internal ArrayList pdfRows;
		internal ArrayList pdfColumns;
		/// <summary>
		/// gets or sets if table headers will be visible.
		/// </summary>
		public bool VisibleHeaders
		{
			get
			{
				return this.visibleHeaders;
			}
			set
			{
				this.visibleHeaders=value;
			}
		}
		/// <summary>
		/// returns the header of the the Table. It will be considered as a usual PdfRow.
		/// </summary>
		public PdfRow HeadersRow
		{
			get
			{
				return this.header.Rows[0];
			}
		}
		private PdfRow[] _Rows;
		/// <summary>
		/// the Collection of the Rows of the Table.
		/// </summary>
		public PdfRow[] Rows
		{
			get
			{
				if (this._Rows==null) this._Rows=this.pdfRows.ToArray(typeof(PdfRow)) as PdfRow[];
				return this._Rows;
			}
		}
		private PdfColumn[] _Columns;
		/// <summary>
		/// the Collection of the Columns of the Table.
		/// </summary>
		public PdfColumn[] Columns
		{
			get
			{
				if (this._Columns==null) this._Columns=this.pdfColumns.ToArray(typeof(PdfColumn)) as PdfColumn[];
				return this._Columns;
			}
		}
		private double borderWidth;
		private Color borderColor;
		private BorderType borderType;
		/// <summary>
		/// sets the borders style of the Table.
		/// </summary>
		/// <param name="BorderColor"></param>
		/// <param name="BorderWidth"></param>
		/// <param name="BorderType"></param>
		public void SetBorders(Color BorderColor,double BorderWidth,BorderType BorderType)
		{
			if (BorderWidth<=0) throw new Exception("BorderWidth must be grater than zero.");
			this.borderColor=BorderColor;
			this.borderType=BorderType;
			this.borderWidth=BorderWidth;
			if (this.header!=null)
			{
				this.header.borderColor=BorderColor;
				this.header.borderType=BorderType;
				this.header.borderWidth=BorderWidth;
			}
		}
		/// <summary>
		/// sets the widths of the Columns.
		/// </summary>
		/// <param name="ColumnsWidthArray"></param>
		public void SetColumnsWidth(int[] ColumnsWidthArray)
		{
			if (ColumnsWidthArray.Length>this.columns) throw new Exception("Table has only "+this.columns+" columns.");
			for (int index=0;index<ColumnsWidthArray.Length;index++)
			{
				if (ColumnsWidthArray[index]<=0) throw new Exception("Column size must be greater than zero.");
				this.Columns[index].SetWidth(ColumnsWidthArray[index]);
			}
		}
		/// <summary>
		/// gets a Cell of the Table.
		/// </summary>
		/// <param name="Row"></param>
		/// <param name="Column"></param>
		/// <returns></returns>
		public PdfCell Cell(int Row,int Column)
		{
			object o=this.cells[Row+","+Column];
			if (o==null) throw new Exception("Cell ["+Row+","+Column+"] does not exist in the Table.");
			return o as PdfCell;
		}
		/// <summary>
		/// gets a range of Cells of the Table.
		/// </summary>
		/// <param name="startRow"></param>
		/// <param name="startColumn"></param>
		/// <param name="endRow"></param>
		/// <param name="endColumn"></param>
		/// <returns></returns>
		public PdfCellRange CellRange(int startRow,int startColumn,int endRow,int endColumn)
		{
			return new PdfCellRange(this,startRow,startColumn,endRow,endColumn);
		}
		internal PdfTable(PdfDocument PdfDocument,ContentAlignment DefaultContentAlignment,Font Font,Color DefaultForegroundColor,int Rows
			,int Columns,double CellPadding)
		{
			cells=new Hashtable();
			pdfRows=new ArrayList();
			this.owner=this;
			this.PdfDocument=PdfDocument;
			this.borderWidth=0;
			this.pdfColumns=new ArrayList();
			this.rows=Rows;
			this.startRow=0;
			this.startColumn=0;
			this.endColumn=Columns-1;
			this.endRow=Rows-1;
			this.columns=Columns;
			
			for (int c=0;c<columns;c++)
				for (int r=0;r<rows;r++)
				{
					this.cells[r+","+c]=new PdfCell(this,r,c,DefaultContentAlignment,DefaultForegroundColor
						,Font,CellPadding);
				}
			for (int r=0;r<rows;r++)
			{
				this.pdfRows.Add(new PdfRow(this,r));
			}
			for (int c=0;c<columns;c++)
			{
				PdfColumn pc=new PdfColumn(this,c);
				pc.Width=100/this.columns;
				this.pdfColumns.Add(pc);
			}
		}
		/// <summary>
		/// sets the Height of the Rows of the Table. We suggest you not to use it
		/// unless each rows contains more or less the same quantity of Text.
		/// </summary>
		/// <param name="RowHeight"></param>
		public void SetRowHeight(double RowHeight)
		{
			if (RowHeight<=0) throw new Exception("Row Height must be grater than zero.");
			foreach (PdfRow pr in this.Rows) pr.SetRowHeight(RowHeight);
		}
		/// <summary>
		/// Imports text from a datatable.
		/// </summary>
		/// <param name="dt">The Source Datatable</param>
		/// <param name="PdfTableStartRow">the starting row of the Pdf Table that will import datas.</param>
		/// <param name="PdfTableStartColumn">the starting column of the Pdf Table that will import datas.</param>
		/// <param name="DataTableStartRow">the starting row of the DataTable that will export datas.</param>
		/// <param name="DataTableEndRow">the ending row of the DataTable that will export datas.</param>
		public void ImportDataTable(DataTable dt,int PdfTableStartRow,int PdfTableStartColumn,
			int DataTableStartRow,int DataTableEndRow)
		{
			for (int r=DataTableStartRow;((r<dt.Rows.Count)&&(r<=DataTableEndRow)&&
				(r+PdfTableStartRow-DataTableStartRow<this.rows));r++)
			{
				for (int c=0;((c<dt.Columns.Count)&&(c+PdfTableStartColumn<this.columns));c++)
				{
					if (c+PdfTableStartColumn>=0&&c+PdfTableStartColumn<dt.Columns.Count) 
					{
						Cell(r+PdfTableStartRow-DataTableStartRow,c+PdfTableStartColumn)
							.SetContent(dt.Rows[r][c]);	
					}
				}
			}
			for (int c=0;((c<dt.Columns.Count)&&(c+PdfTableStartColumn<this.columns));c++)
			{
				if (c+PdfTableStartColumn>=0&&c+PdfTableStartColumn<dt.Columns.Count) 
				{
					this.HeadersRow[c+PdfTableStartColumn].SetContent(dt.Columns[c].ColumnName);
				}
			}
		}
		/// <summary>
		/// Imports text from a datatable.
		/// </summary>
		/// <param name="dt">The Source Datatable</param>
		/// <param name="PdfTableStartRow">the starting row of the Pdf Table that will import datas.</param>
		/// <param name="PdfTableStartColumn">the starting column of the Pdf Table that will import datas.</param>
		public void ImportDataTable(DataTable dt,int PdfTableStartRow,int PdfTableStartColumn)
		{
			ImportDataTable(dt,PdfTableStartRow,PdfTableStartColumn,0,999999);
		}
		/// <summary>
		/// Imports text from a datatable.
		/// </summary>
		/// <param name="dt">The Source Datatable</param>
		public void ImportDataTable(DataTable dt)
		{
			ImportDataTable(dt,0,0,0,999999);
		}
		/// <summary>
		/// returns true if all the pages of the Table are already rasterized to a TablePage.
		/// </summary>
		public bool AllTablePagesCreated
		{
			get
			{
				if (this.renderingIndex+this.renderingRows>this.rows-1) return true;
				return false;
			}
		}
		/// <summary>
		/// Creates the TablePage, the rasterized page of a Table.
		/// </summary>
		/// <param name="PageArea"></param>
		/// <returns></returns>
		public PdfTablePage CreateTablePage(PdfArea PageArea)
		{
			this.TableArea=PageArea.Clone();
			PdfTablePage ptp;
			if (!this.visibleHeaders)
				ptp=this.createTablePage();
			else
			
			{
				this.header.TableArea=PageArea.Clone();
				this.header.TableArea.height=this.HeadersRow.Height;
				this.TableArea.posy+=this.HeadersRow.Height;
				this.TableArea.height-=this.HeadersRow.Height;
				
				ptp=this.createTablePage();
				ptp.stream+=this.header.createTablePage().stream;

				this.header.renderingIndex=0;
			}

				
			foreach (PdfColumn pc in this.Columns) pc.CompensatedWidth=-1;
			foreach (PdfColumn pc in this.header.Columns) pc.CompensatedWidth=-1;
			return ptp;
		}
		internal PdfTablePage createTablePage()
		{
			int index=this.renderingIndex;
			double h=this.Rows[index].Height;
			double oh=0;
			while ((h<=this.TableArea.height)&&(index<this.rows))
			{
				index++;
				oh=h;
				if (index<this.rows) h+=this.Rows[index].Height;
			}
			this.renderingRows=index-this.renderingIndex;
			this.TableArea.height=(double)oh;
			PdfTablePage ptp=new PdfTablePage(renderingIndex,renderingIndex+renderingRows-1,columns);
			
			//ptp.stream="";
			//caculates areas
			double y=this.TableArea.posy;
			for (int rowIndex=this.renderingIndex;(rowIndex<this.renderingIndex+this.renderingRows);rowIndex++)
			{
				double x=this.TableArea.posx;
				for (int columnIndex=0;columnIndex<this.columns;columnIndex++)
				{
					PdfCell pc=this.Cell(rowIndex,columnIndex);
					double width=pc.Width;
					pc.area=new PdfArea(this.PdfDocument,x,y,width,pc.Height);
					x+=width;
				}
				y+=this.Rows[rowIndex].Height;
			}
			
			for (int rowIndex=this.renderingIndex;(rowIndex<this.renderingIndex+this.renderingRows);rowIndex++)
				for (int columnIndex=0;columnIndex<this.columns;columnIndex++)
				{
					PdfCell pc=this.Cell(rowIndex,columnIndex);
					if (!pc.isSpanned)	ptp.cellAreas.Add(pc.row+","+pc.column,pc.Area);
				}

			ptp.stream=this.ToLineStream();
			ptp.SetArea();
			
			this.renderingIndex=index;
			this.renderingRows=0;
			return ptp;
		}
	
		internal string ToLineStream()
		{
			System.Text.StringBuilder sb=new StringBuilder();
			// draw background rectangles
			for (int rowIndex=this.renderingIndex;(rowIndex<this.renderingIndex+this.renderingRows);rowIndex++)
			{
				for (int columnIndex=0;columnIndex<this.columns;columnIndex++)
				{
					PdfCell pc=this.Cell(rowIndex,columnIndex);
					if (!pc.isSpanned)
					{
						if (!pc.transparent)
							sb.Append(pc.Area.InnerArea(1).ToRectangle(pc.backgroundColor
								,pc.backgroundColor).ToLineStream());
					}
					
				}
			}
			sb.Append("BT\n");
			
			Font actualFont=null;
			Color actualColor=Color.Black;
			sb.Append(Utility.ColorrgLine(Color.Black));
			for (int rowIndex=this.renderingIndex;(rowIndex<this.renderingIndex+this.renderingRows);rowIndex++)
			{
				for (int columnIndex=0;columnIndex<this.columns;columnIndex++)
				{
					PdfCell pc=this.Cell(rowIndex,columnIndex);
					if (!pc.isSpanned)
					{
						PdfTextArea pt=new PdfTextArea(pc.Font,pc.foregroundColor
							,pc.Area.InnerArea(pc.cellPadding*2),pc.ContentAlignment,pc.text);
						
						if (pc.Font!=actualFont)
						{
							string actualFontLine=Utility.FontToFontLine(pc.Font);
							if (!this.PdfDocument.FontNameList.Contains(PdfFont.FontToPdfType(pc.Font)))
								this.PdfDocument.AddFont(pc.Font);
							sb.Append(actualFontLine);
							actualFont=pc.Font;
						}
						if (pc.foregroundColor!=actualColor)
						{
							sb.Append(Utility.ColorrgLine(pc.foregroundColor));
							actualColor=pc.foregroundColor;
						}
						sb.Append(pt.ToLineStream());
					}
				}
				
			}
			sb.Append("ET\n");
			
			if (this.borderWidth>0)
			{
				sb.Append(new PdfRectangle(this.PdfDocument,new PdfArea(this.PdfDocument,0,0,1,1),this.borderColor
					,this.borderWidth).ToColorAndWidthStream());
				int bt=(int)this.borderType;
				if ((bt==1)||(bt==3)||(bt==5)||(bt==6))
					sb.Append(this.TableArea.ToRectangle(this.borderColor,this.borderWidth).ToRectangleStream());
				for (int rowIndex=this.renderingIndex;(rowIndex<this.renderingIndex+this.renderingRows);rowIndex++)
					for (int columnIndex=0;columnIndex<this.columns;columnIndex++)
					{
						PdfCell pc=this.Cell(rowIndex,columnIndex);
						if (!pc.isSpanned)
						{
							if (rowIndex!=this.renderingIndex)
							{
								if ((bt==6)||(bt==2)||(bt==3)||(bt==7))
									sb.Append(pc.Area.UpperBound(this.borderColor,this.borderWidth).ToLineStream());
							}
							if (columnIndex!=0)
							{
								if ((bt==6)||(bt==4)||(bt==5)||(bt==7))
									sb.Append(pc.Area.LeftBound(this.borderColor,this.borderWidth).ToLineStream());
							}
						}
					}
			}
			return sb.ToString();
		}
	}
}
