using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace printPen
{
    class Row
    {
        int mvarRowIndex;//'Copia locale.

        public int rowIndex
        {
            get { return mvarRowIndex; }
            set { mvarRowIndex = value; }
        }
        Cells mvarCells;

        public Cells cells
        {
            get { return mvarCells; }
            set { mvarCells = value; }
        }

    }
}
