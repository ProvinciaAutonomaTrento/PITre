using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace printPen
{
    class Buffer
    {
        public enum BufferRowsConfigurationEnum
{
	BufferRows_5 = 5,
	BufferRows_10 = 10
}

public enum BufferColsConfigurationEnum
{
	BufferCols_16 = 16,
	BufferCols_32 = 32
}

public enum BufferConfigurationEnum
{
	Buffer_5x16,
	Buffer_5x32,
	Buffer_10x16,
	Buffer_10x32
}

private Cols mvarCols;
private Rows mvarRows;
private Cells mvarCells;
//******************************************************


public short getRowsByBuffer(BufferConfigurationEnum bufferConfiguration)
{
	short retValue = 0;

	if ((bufferConfiguration == BufferConfigurationEnum.Buffer_5x16 | bufferConfiguration == BufferConfigurationEnum.Buffer_5x32)) {
		retValue = 5;

	} else if ((bufferConfiguration == BufferConfigurationEnum.Buffer_10x16 | bufferConfiguration == BufferConfigurationEnum.Buffer_10x32)) {
		retValue = 10;
	}

	return retValue;
}

public short getColsByBuffer(BufferConfigurationEnum bufferConfiguration)
{
	short retValue = 0;

	if ((bufferConfiguration == BufferConfigurationEnum.Buffer_5x16 | bufferConfiguration == BufferConfigurationEnum.Buffer_10x16)) {
		retValue = 16;

	} else if ((bufferConfiguration == BufferConfigurationEnum.Buffer_5x32 | bufferConfiguration == BufferConfigurationEnum.Buffer_10x32)) {
		retValue = 32;
	}

	return retValue;
}

public void initialize(BufferConfigurationEnum bufferConfiguration, string varString)
{
	short l_cols = 0;
	short l_rows = 0;
	short l_rowIndex = 0;
	short l_colIndex = 0;
	

	
	l_cols = this.getColsByBuffer(bufferConfiguration);
	l_rows = this.getRowsByBuffer(bufferConfiguration);
	mvarCols.initialize(l_cols);
	mvarRows.initialize(l_rows);

    foreach (Row l_row in mvarRows)
    {

        //'esegue un ciclo sulle righe generando le celle
        //'per ogni colonna della riga
        l_row.cells = new Cells();
        for (l_colIndex = 1; l_colIndex <= l_cols; l_colIndex++)
        {
            l_row.cells.Add("", l_row.rowIndex, l_colIndex, "");
        }
    }

	this.bufferString = varString;

	foreach ( Col l_col in mvarCols) {
		//'esegue un ciclo sulle colonne, recuperando la collection
		//'delle celle in base a quelle generate per le righe
        int ci=0;
		l_col.cells = pf_getColCellsById(ref ci);

        l_col.colIndex = ci;
	}

}

private Cells pf_getColCellsById(ref int colIndex)
{
	Cells l_cells = new Cells();
	foreach (Row l_row in mvarRows) {
		Cell l_cell = l_row.cells.Item(colIndex);
		l_cells.AddCell(l_cell, "");
	}
	return l_cells;
}




public Rows rows {
	get {

		if (mvarRows == null) {
			mvarRows = new Rows();
		}

		return mvarRows;
	}
	set { mvarRows = value; }
}


public Cols cols {
	get {
		if (mvarCols == null) {
			mvarCols = new Cols();
		}

		return mvarCols;
	}
	set { mvarCols = value; }
}


public string bufferString {
	get {
		string retValue = null;
		string l_separatore = null;
		retValue = "";
		l_separatore = "";
		foreach ( Row l_row in this.rows) {
			retValue = retValue + l_separatore + l_row.cells.text;
			l_separatore = Environment.NewLine;
		}
		return retValue;
	}
	set {
		string l_partialString = null;
		string l_remainingString = null;
		bool l_continue = false;
		int l_indexRow = 0;
		int l_indexToCut = 0;

		l_indexRow = 0;
		l_remainingString = value;
		l_continue = true;
		while (l_continue) {
			l_indexRow = l_indexRow + 1;
			l_partialString = l_remainingString.Substring(1, this.cols.Count());
			l_indexToCut = l_remainingString.IndexOf(Environment.NewLine);
			if (l_indexToCut != 0) {
                l_partialString = l_remainingString.Substring(1, l_indexToCut - 1);
				l_indexToCut = l_indexToCut + 2;
			} else {
				l_indexToCut = l_partialString.Length + 1;
			}
			l_remainingString = l_remainingString.Substring(1, l_indexToCut);
			this.rows.Item(l_indexRow).cells.text = l_partialString;
			l_continue = (!string.IsNullOrEmpty(l_remainingString)) & (l_indexRow < this.rows.Count());
		}

	}
}

//UPGRADE_NOTE: Class_Terminate was upgraded to Class_Terminate_Renamed. Click for more: 'ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
private void Class_Terminate_Renamed()
{
	 // ERROR: Not supported in C#: OnErrorStatement
     
    //if (((m_objectSafe != null))) {
    //    //UPGRADE_NOTE: Object m_objectSafe may not be destroyed until it is garbage collected. Click for more: 'ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
    //    m_objectSafe = null;
    //}

	//UPGRADE_NOTE: Object mvarCells may not be destroyed until it is garbage collected. Click for more: 'ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
	mvarCells = null;
	//UPGRADE_NOTE: Object mvarRows may not be destroyed until it is garbage collected. Click for more: 'ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
	mvarRows = null;
	//UPGRADE_NOTE: Object mvarCols may not be destroyed until it is garbage collected. Click for more: 'ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
	mvarCols = null;
}

    }
}
