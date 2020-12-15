using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace printPen
{
    class Cells
    {

        Hashtable mCol = new Hashtable();
        public void AddCell(Cell cell, string sKey){
		if (sKey.Length == 0) {
			mCol.Add("", cell);
        }
		else
			mCol.Add(sKey, cell);
        }
	
	public Cell Add(string cellText, int rowIndex , int colIndex , string sKey ) {
		//'crea un nuovo oggetto
		Cell objNewMember = new Cell();

		
		//'imposta le proprietà passate al metodo
		objNewMember.cellText = cellText;
		objNewMember.colIndex = colIndex;
		objNewMember.rowIndex = rowIndex;
		AddCell(objNewMember, sKey);

		return objNewMember;
    }

	public Cell Item(int vntIndexKey) {
			return (Cell) mCol[vntIndexKey] ;
        }
	
	
	public int Count(){
        return mCol.Count;
    }	
	
	void Remove(int vntIndexKey){
        //'Utilizzata per rimuovere un elemento dall'insieme.
        //'vntIndexKey contiene l'indice o la chiave, e per questo
        //'motivo viene dichiarata come Variant.
        //'Sintassi: x.Remove(xyz)
		
		mCol.Remove(vntIndexKey);
	}

	public System.Collections.IEnumerator GetEnumerator() { 
		//'UPGRADE_TODO: Uncomment and change the following line to return the collection enumerator. Click for more: 'ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?keyword="95F9AAD0-1319-4921-95F0-B9D3C4FF7F1C"'
		return mCol.GetEnumerator();
	}
	
	
	
public string text
{
  get { 
      
      //Cell l_cell ;

			string retValue="";

      foreach (Cell l_cell in this){
				retValue = retValue + l_cell.cellText;
      }
      return retValue;   
  
  }
  set { 
      string myText = value; 
			int l_index = 1;
			
      foreach (Cell l_cell in this){
			l_cell.cellText = myText.Substring(l_index, 1);
          l_index++;
      }
  }
}


    }
}
