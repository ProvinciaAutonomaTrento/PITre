using System;
using DocsPaUtils.Interfaces.DbManagement;

namespace DocsPaDbManagement.Functions.SqlServer
{
	/// <summary>
	/// Summary description for Functions.
	/// </summary>
	public class Functions : DocsPaUtils.Interfaces.DbManagement.IFunctions
	{
		/// <summary>
		/// Ritorna la stringa SQL relativa all'acquisizione di una data
		/// </summary>
		/// <returns>
		/// </returns>
		public string GetDate() 
		{
			return " GETDATE()";
		}
        /// <summary>
        /// implementa nvl su oracle e isnull su sql nvl(val1,val2) o isnull(val1,val2)
        /// </summary>
        /// <param name="?"></param>
        /// <returns></returns>
        public  string GetNVL(string val1, string val2)
        {
            return "IsNull(" + val1 + "," + val2 + ")";
        }
		public string GetDate(bool flgTime) 
		{
			if(flgTime)
			{
				//inserisce in un campo  data il getdate formattato dd/mm/yyyy HH24:MI:SS
				return " GETDATE()";	
			}
			else
			{
				//string formatStr = "DD/MM/YYYY";
				//inserisce in un campo  data il getdate formattato dd/mm/yyyy
				return " GETDATE()";				
			}
		}

		/// <summary>
		/// Ritorna la stringa SQL relativa all'acquisizione di un anno
		/// </summary>
		/// <param name="date"></param>
		/// <returns></returns>
		public string GetYear(string date) 
		{
			return " YEAR(" + date + ") ";
		}


        /// <summary>
        /// Converte un campo del db o un valore in un intero
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public string ToInt(string value, bool isColumn)
        {
            string convert = string.Empty;
            if (isColumn)
            {
                convert = "CONVERT(int," + value + ")";
            }
            else
            {
                convert = "CONVERT(int,'" + value + "')";
            }
            return convert;
        }

        /// <summary>
        /// Converte un campo del db in una data
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public string ToDateColumn(string columnName)
        {
            if (columnName == null || columnName.Equals(""))
            {
                return "null";
            }

            string formatStr = "103";

            string cmdStr = " convert(datetime," + columnName + "," + formatStr + ") ";

            return cmdStr;
        }


		/// <summary>
		/// Converte una stringa nel formato SQL relativo ad una data
		/// </summary>
		/// <param name="date"></param>
		/// <returns></returns>
		public string ToDate(string date) 
		{
			if(date == null || date.Equals(""))
			{
				return "null";
			}

			string formatStr = "103";

			string cmdStr = " convert(datetime,'" + date.Replace(".", ":") + "'," + formatStr + ") ";
			
			return cmdStr;
		}

		/// <summary>
		/// gestione delle date utilizzate per gli intervalli between
		/// </summary>
		/// <param name="date">data</param>
		/// <param name="iniziogiornata">possibili valori: true, false</param>
		/// <returns>string</returns>
		public string ToDateBetween(string date, bool iniziogiornata)
		{
			if(date == null || date.Equals(""))
			{
				return "null";
			}

			string formatStr = "103";

			string cmdStr;			
			
			if(iniziogiornata)
			{			
				cmdStr = " convert(datetime,'" + date.Replace(".", ":") + " 00:00:00'," + formatStr + ") ";
			}
			else
			{				
				cmdStr = " convert(datetime,'" + date.Replace(".", ":") + " 23:59:59'," + formatStr + ") ";
			}

			return cmdStr;
		}

		public string ToDate(string date, bool time) 
		{
			//non è un errore deve essere la stessa del ToDate(string columndate);
			if(date == null || date.Equals(""))
			{
				return "null";
			}

			string formatStr = "103";

			string cmdStr = " convert(datetime,'" + date.Replace(".", ":") + "'," + formatStr + ") ";
			
			return cmdStr;
		}

		/// <summary>
		/// </summary>
		/// <returns></returns>
		public string GetSystemIdColName() 
		{
			return "";
		}

		/// <summary>
		/// </summary>
		/// <returns></returns>
		public string GetVersionIdColName() 
		{
			return "";
		}
		
		/// <summary>
		/// </summary>
		/// <param name="tableName"></param>
		/// <returns></returns>
		public string GetSystemIdNextVal(string tableName) 
		{
			return "";
		}

		/// <summary>
		/// </summary>
		/// <param name="queryString"></param>
		/// <returns></returns>
		/// <remarks>modificato per ricerca Top N Documenti</remarks>
		public string SelectTop(string queryString) 
		{
			string numRighe = System.Configuration.ConfigurationManager.AppSettings["numeroMaxRisultatiQuery"];
			string disableSelectTop = System.Configuration.ConfigurationManager.AppSettings["DisableSelectTop"]; //0=no top;
			
			if(disableSelectTop==null || disableSelectTop.Equals("0"))
				queryString = SelectTop(queryString, numRighe);

			return queryString;
		}

		/// <summary>
		/// </summary>
		/// <param name="queryString"></param>
		/// <param name="numRighe"></param>
		/// <returns></returns>
		/// <remarks>modificato per ricerca Top N Documenti</remarks>
		public string SelectTop(string queryString, string numRighe) 
		{
			string resSQL = "" ;
			if (!(numRighe != null && !numRighe.Equals("0")))
			{
				//return queryString;
				resSQL = queryString ;
			}
			if(queryString.Trim().Substring(0,15).ToUpper().EndsWith("DISTINCT"))
			{
				//return "SELECT DISTINCT TOP " + numRighe + " " + queryString.Substring(16);
				//resSQL = "SELECT DISTINCT TOP " + numRighe + " " + queryString.Substring(16);
				resSQL = queryString.Trim().Insert(16," TOP " + numRighe + " ");
			}
			else
			{
				string qry = queryString.Trim();
				//resSQL = "SELECT TOP " + numRighe + " " + qry.Substring(15);
				resSQL = qry.Insert(6," TOP " + numRighe + " ");
			}
			//return "SELECT TOP " + numRighe + " " + queryString.Substring(20);
			return resSQL ;
		}

		/// <summary>
		/// </summary>
		/// <param name="colName"></param>
		/// <param name="time"></param>
		/// <returns></returns>
		public string ToChar(string colName, bool time) 
		{
			string formatStr = "103";
			string cmdStr;

			if(time) 
			{
				cmdStr = " convert(datetime," + colName + "," + formatStr + ") ";
			}
			else 
			{
				cmdStr = " substring(convert(char," + colName + "," + formatStr + "),1,11) ";
			}

			return cmdStr;
		}
		
		/// <summary>
		/// </summary>
		/// <param name="dateString"></param>
		/// <returns></returns>
		public string ToDbDate(string dateString)
		{
			DateTime dateTime=Convert.ToDateTime(dateString);
			System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("it-IT");
			ci.DateTimeFormat.DateSeparator="/";
			string res=dateTime.ToString("dd/MM/yyyy HH:mm:ss",ci.DateTimeFormat);
			
			return res;
		}

		/// <summary>		
		/// </summary>
		/// <returns></returns>
		public string ConcatStr()
		{
			return " + ";
		}

		/// <summary>		
		/// </summary>
		/// <param name="expr"></param>
		/// <param name="start"></param>
		/// <param name="length"></param>
		/// <returns></returns>
		public string SubStr(string expr, string start, string length)
		{
			return "SUBSTRING("+expr+", "+start+", "+length+")";
		}

		/// <summary>
		/// Restiruisce la query per ricavare l'ultimo valore della SYSTEM_ID inserita
		/// </summary>
		/// <returns></returns>
		public string GetQueryLastSystemIdInserted()
		{
			return "SELECT scope_identity()";
		}

		/// <summary>
		/// Restiruisce la query per ricavare l'ultimo valore della SYSTEM_ID inserita
		/// </summary>
		/// <param name="tableName"></param>
		/// <returns></returns>
		public string GetQueryLastSystemIdInserted(string tableName)
		{
			return this.GetQueryLastSystemIdInserted();
		}

		/// <summary>
		/// gestione della SYSTEMKEY (vedi Hummingbird) per inserire il valore della system_id in PEOPLE e GROUPS
		/// </summary>
		/// <returns>ultimo id</returns>
		public string GetSystemKeyHumm()
		{	
			string retValue = null;

			switch (System.Configuration.ConfigurationManager.AppSettings["documentale"].ToString())
			{
				case "HUMMINGBIRD":
					DocsPaDbManagement.Database.SqlServer.Database db=new DocsPaDbManagement.Database.SqlServer.Database();
					retValue=db.GetNextSystemId();
					break;

                case "ETNOTEAM":
                case "FILENET":
                case "PITRE":
                case "CDC":
                case "DOCUMENTUM":
                case "GFD":
					retValue = "";
					break;
			}			

			return retValue;
		}

        /// <summary>
        /// Generazione della stringa di query relativa alla ricerca di un 
        /// valore in un campo impostato come fulltext
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        public string GetContainsTextQuery(string fieldName, params DocsPaVO.filtri.SearchTextItem[] items)
        {
            string retValue = "CONTAINS(" + fieldName + ", ";

            string searchText = "'";

            bool append = false;

            DocsPaUtils.Data.FullTextUtils fullTextUtils = new SqlFullTextUtils();

            foreach (DocsPaVO.filtri.SearchTextItem item in items)
            {
                if (append)
                    searchText += " AND ";

                searchText += "''";

                if (item.SearchOption == DocsPaVO.filtri.SearchTextOptionsEnum.WholeWord)
                    searchText += fullTextUtils.ParseTextSpecialChars(item.TextToSearch.Trim());
                else if (item.SearchOption == DocsPaVO.filtri.SearchTextOptionsEnum.InitWithWord)
                    searchText += fullTextUtils.ParseTextSpecialChars(item.TextToSearch.Trim()) + "*";
                else if (item.SearchOption == DocsPaVO.filtri.SearchTextOptionsEnum.ContainsWord)
                    searchText += "*" + fullTextUtils.ParseTextSpecialChars(item.TextToSearch.Trim()) + "*";

                searchText += "''";

                append = true;
            }

            retValue += searchText + "')";

            return retValue;
        }

        /// <summary>
        /// Generazione della stringa di query relativa alla ricerca di un 
        /// valore in un campo impostato come fulltext
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public string GetContainsTextQuery(string fieldName, string value)
        {
            string retValue = "contains(" + fieldName + ", ";
            string searchText = "'";

            DocsPaUtils.Data.FullTextUtils fullTextUtils = new SqlFullTextUtils();

            searchText += value.Trim();
            searchText = fullTextUtils.ParseTextSpecialChars(searchText);

            retValue += searchText + "') > 0";

            return retValue;
        }

        /// <summary>
        /// Reperimento utente della sessione corrente
        /// </summary>
        /// <returns></returns>
        public string GetDbUserSession()
        {
            string userSession = string.Empty;

            using (Database.SqlServer.Database db = new DocsPaDbManagement.Database.SqlServer.Database())
            {
                db.ExecuteScalar(out userSession, "SELECT SESSION_USER");
            }

            return userSession;
        }

        /// <summary>
        /// gestione carattere °
        /// </summary>
        /// <returns></returns>
        public string convertDegre()
        {
            return "' + char(ASCII('°')) + '";
        }
    }
}
