using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace printPen
{
    class Rows : System.Collections.IEnumerable
    {
        System.Collections.Hashtable mRow = new System.Collections.Hashtable();


        public void initialize(int rows)
        {
            int l_index;
            Row l_newRow;

            for (l_index = 1; l_index < rows; l_index++)
            {
                l_newRow = new Row();
                l_newRow.rowIndex = l_index;
                mRow.Add(l_index, l_newRow);
            }
        }

        Row Add(Cells cells, int rowIndex)
        {
            //'crea un nuovo oggetto
            Row objNewMember = new Row();

            //'imposta le proprietà passate al metodo
            objNewMember.cells = cells;

            mRow.Add(rowIndex, objNewMember);

            //'restituisce l'oggetto creato
            return objNewMember;
            //'UPGRADE_NOTE: Object objNewMember may not be destroyed until it is garbage collected. Click for more: 'ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
            //objNewMember = Nothing
        }

        public Row Item(int vntIndexKey)
        {
            return (Row)mRow[vntIndexKey];
        }


        public int Count()
        {
            return mRow.Count;
        }

        //	'UPGRADE_NOTE: NewEnum property was commented out. Click for more: 'ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?keyword="B3FC1610-34F3-43F5-86B7-16C984F0E88E"'
        //'Public ReadOnly Property NewEnum() As stdole.IUnknown
        //    'Get
        //        'Questa proprietà consente di enumerare l'insieme
        //        'corrente con la sintassi For...Each.
        //        'NewEnum = mCol._NewEnum
        //    'End Get
        //'End Property

        //Public Function GetEnumerator() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator
        //    'UPGRADE_TODO: Uncomment and change the following line to return the collection enumerator. Click for more: 'ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?keyword="95F9AAD0-1319-4921-95F0-B9D3C4FF7F1C"'
        //    'GetEnumerator = mCol.GetEnumerator
        //End Function


        void Remove(int vntIndexKey)
        {
            //'Utilizzata per rimuovere un elemento dall'insieme.
            //'vntIndexKey contiene l'indice o la chiave, e per questo
            //'motivo viene dichiarata come Variant.
            //'Sintassi: x.Remove(xyz)

            mRow.Remove(vntIndexKey);
        }

        public System.Collections.IEnumerator GetEnumerator()
        {
            //'UPGRADE_TODO: Uncomment and change the following line to return the collection enumerator. Click for more: 'ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?keyword="95F9AAD0-1319-4921-95F0-B9D3C4FF7F1C"'
            return mRow.GetEnumerator();
        }



        ////public void initialize(int rows)
        ////{

        ////    Row l_newRow;
        ////    for (int l_index = 1; l_index < rows; l_index++)
        ////    {
        ////        l_newRow = new Row();
        ////        l_newRow.rowIndex = l_index;
        ////        mRow.Add(l_newRow, l_index);
        ////    }
        ////}
    }
}
