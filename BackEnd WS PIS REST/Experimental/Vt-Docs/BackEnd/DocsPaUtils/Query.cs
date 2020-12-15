using System;
using System.Collections;
using System.Xml;
using System.Configuration;

namespace DocsPaUtils
{
	/// <summary>
	/// </summary>
	public class Query
	{
		private String queryString = null;
//		private String dateFormat; 
//		private String dateTimeFormat;

		/// <summary>
		/// Assegna i valori ricevuti come parametri agli attributi della classe stessa
		/// </summary>
		/// <param name="sql"></param>
		/// <param name="dateFormat"></param>
		/// <param name="dateTimeFormat"></param>
		public Query (string sql /*, string dateFormat, string dateTimeFormat*/ )
		{
			queryString = sql;
//			this.dateFormat = dateFormat;
//			this.dateTimeFormat = dateTimeFormat;
		}
	

		/// <summary>
		/// Setta l’attributo queryString alla query specificata dal parametro di input queryDef.
		/// </summary>
		/// <param name="queryDef"></param>
		public void setQuery(String queryDef)
		{
			queryString = queryDef;
		}
		
		/// <summary>
		/// Sostituisce all’interno della query rappresentata dall’attributo queryString
		/// tutte le occorrenze della stringa %param% con la stringa val. 
		/// </summary>
		/// <param name="param"></param>
		/// <param name="val"></param>
		public void setParam(String param, String val)
		{	
			queryString = queryString.Replace("@" + param + "@", val);	
		}

		/// <summary>
		/// Sostituisce all’interno della query rappresentata dall’attributo queryString 
		/// tutte le occorrenze della stringa %param% con la stringa value e sostituisce 
		/// le occorrenze del carattere ' con il carattere ". 
		/// </summary>
		/// <param name="param"></param>
		/// <param name="val"></param>
		public void setParamString(String param, String val)
		{			
			setParam(param, val);
			queryString = queryString.Replace("'","\"");	
		}
	
		#region Metodo Commentato
//		/// <summary>
//		/// Sostituisce all’interno della query rappresentata dall’attributo queryString
//		/// tutte le occorrenze della stringa %param% con la data specificata dal parametro 
//		/// di input "val" convertendola al formato specificato dall'attributo dateFormat.
//		/// </summary>
//		/// <param name="param"></param>
//		/// <param name="val"></param>
//		public void setParamDate(String param, DateTime val)
//		{	
//			queryString = queryString.Replace("@" + param + "@", val.ToString(dateFormat));	
//		}
		#endregion

		#region Metodo Commentato
//		/// <summary>
//		/// Sostituisce all’interno della query rappresentata dall’attributo queryString 
//		/// tutte le occorrenze della stringa %param% con la data e l'ora specificata dal 
//		/// parametro di input "val" convertendola al formato specificato dall'attributo 
//		/// dateTimeFormat.
//		/// </summary>
//		/// <param name="param"></param>
//		/// <param name="val"></param>
//		public void setParamDateTime(String param, DateTime val)
//		{	
//			queryString = queryString.Replace("@" + param + "@", val.ToString(dateTimeFormat));	
//		}
		#endregion

		/// <summary>
		/// Elimina dall'SQL tutte le stringhe %...%
		/// 
		/// </summary>
		public void purgeParam()
		{	
			int startPosition = -1;
			int endPosition;
			
			do
			{
				startPosition=queryString.IndexOf('@', startPosition + 1);
				if(startPosition!=-1 && (startPosition+1)<queryString.Length)
				{
					endPosition=queryString.IndexOf('@',startPosition+1);
                    if (endPosition != -1 && endPosition > (startPosition + 3) && endPosition < (startPosition + 9))
					{
						//verifica che il parametro sia del tipo: @PAR*@
						if(queryString.Substring(startPosition+1,5).ToUpper().Equals("PARAM")) 
						{
                            //determinata start ed end position -> elimina la sub stringa
                            queryString = queryString.Remove(startPosition, endPosition - startPosition + 1);
						}
					}
					else break;
				}
			}
			while(startPosition!=-1);
		}

		/// <summary>
		/// Restituisce la stringa SQL che è stata elaborata dall’oggetto.
		/// vengono eliminati dalla stringa gli eventuali parametri non sostituiti
		/// </summary>
		/// <returns></returns>
		public String getSQL()
		{
			purgeParam();
			return queryString;
		}	
	}
}
