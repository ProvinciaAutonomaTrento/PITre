using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace printPen
{
    class Cols : System.Collections.IEnumerable{
	
	//'Variabile locale per memorizzare l'insieme.
	System.Collections.Hashtable  mCol = new System.Collections.Hashtable();

        
	public void initialize(int cols ){
		int l_index;
		Col l_newCol;

		for (l_index = 1; l_index < cols;l_index++){
			l_newCol = new Col();
			l_newCol.colIndex = l_index;
			mCol.Add(l_index, l_newCol);
		    } 
        }
	
	Col Add(Cells cells, int rowIndex ) {
		//'crea un nuovo oggetto
		Col objNewMember = new Col();
			
		//'imposta le proprietà passate al metodo
		objNewMember.cells = cells;
		
		mCol.Add(rowIndex, objNewMember);
		
		//'restituisce l'oggetto creato
		return objNewMember;
        //'UPGRADE_NOTE: Object objNewMember may not be destroyed until it is garbage collected. Click for more: 'ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
        //objNewMember = Nothing
	}
	
	public Col Item(int vntIndexKey) {
			return (Col) mCol[vntIndexKey] ;
        }
	
	
	public int Count(){
        return mCol.Count;
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
	
	
	void Remove(int vntIndexKey){
        //'Utilizzata per rimuovere un elemento dall'insieme.
        //'vntIndexKey contiene l'indice o la chiave, e per questo
        //'motivo viene dichiarata come Variant.
        //'Sintassi: x.Remove(xyz)
		
		mCol.Remove(vntIndexKey);
	}

    public System.Collections.IEnumerator GetEnumerator()
    {
        //'UPGRADE_TODO: Uncomment and change the following line to return the collection enumerator. Click for more: 'ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?keyword="95F9AAD0-1319-4921-95F0-B9D3C4FF7F1C"'
        return mCol.GetEnumerator();
    }

    //public void initialize(int cols){
		
    //    Col l_newCol;
    //    for (int l_index = 1; l_index < cols; l_index++){
    //        l_newCol = new Col();
    //        l_newCol.colIndex = l_index;
    //        mCol.Add(l_newCol, l_index);
    //        }
    //}

    //'UPGRADE_NOTE: Class_Initialize was upgraded to Class_Initialize_Renamed. Click for more: 'ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
    //Private Sub Class_Initialize_Renamed()
    //    On Error Resume Next
    //    If (m_objectSafe Is Nothing) Then
    //        m_objectSafe = New ObjectSafe
    //    End If
		
    //    'Crea l'insieme quando viene creata questa classe.
    //    mCol = New Collection
    //End Sub
    //Public Sub New()
    //    MyBase.New()
    //    Class_Initialize_Renamed()
    //End Sub
	
	
    //'UPGRADE_NOTE: Class_Terminate was upgraded to Class_Terminate_Renamed. Click for more: 'ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
    //Private Sub Class_Terminate_Renamed()
    //    On Error Resume Next
    //    If (Not m_objectSafe Is Nothing) Then
    //        'UPGRADE_NOTE: Object m_objectSafe may not be destroyed until it is garbage collected. Click for more: 'ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
    //        m_objectSafe = Nothing
    //    End If
    //    'Rimuove l'insieme quando la classe viene eliminata.
    //    'UPGRADE_NOTE: Object mCol may not be destroyed until it is garbage collected. Click for more: 'ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
    //    mCol = Nothing
    //End Sub
    //Protected Overrides Sub Finalize()
    //    Class_Terminate_Renamed()
    //    MyBase.Finalize()
    //End Sub
    }
}
