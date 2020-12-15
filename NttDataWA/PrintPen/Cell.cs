using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace printPen
{
    class Cell
    {

        string mvarcellText = "";//'Copia locale.

        public string cellText
        {
            get { return mvarcellText; }
            set { mvarcellText = value; }
        }
        int mvarColIndex; //'Copia locale.

        public int colIndex
        {
            get { return mvarColIndex; }
            set { mvarColIndex = value; }
        }
        int mvarRowIndex;//'Copia locale.

        public int rowIndex
        {
            get { return mvarRowIndex; }
            set { mvarRowIndex = value; }
        }

    }
}
