using System;
using System.Data;

namespace DocsPaDB.Utils
{
	/// <summary>
	/// Summary description for Security.
	/// </summary>
	public class Security
	{
		public static bool isRuoloAutorizzato(string nomeFunzione,string idRuolo)
		{
			//DocsPaWS.Utils.Database db=DocsPaWS.Utils.dbControl.getDatabase();
			//DocsPaWS.Utils.Debug debug=new DocsPaWS.Utils.Debug();
			//DataSet dataSet=new DataSet();
			DataSet dataSet;
			bool result=false;
			try
			{

				//db.openConnection();
				/*
				string queryString="SELECT B.SYSTEM_ID FROM DPA_FUNZIONI A, DPA_TIPO_F_RUOLO B  WHERE";
				queryString=queryString+" B.ID_TIPO_FUNZ=A.ID_TIPO_FUNZIONE AND A.COD_FUNZIONE='"+nomeFunzione+"' AND B.ID_RUOLO_IN_UO="+idRuolo;
				db.fillTable(queryString,dataSet,"AUTORIZZAZIONI");
				*/
				DocsPaDB.Query_Utils.Utils obj = new DocsPaDB.Query_Utils.Utils();				
				obj.getFunzTipRuolo(out dataSet,nomeFunzione,idRuolo);

				if(dataSet.Tables["AUTORIZZAZIONI"].Rows.Count>0)
				{
					result=true;
				}
				else
				{
				    result=false;
				}
				//db.closeConnection();
			}
			catch(Exception )
			{
			   //db.closeConnection();
			   //throw e;
				result = false;
			}
            return result;
		}
	}
}
