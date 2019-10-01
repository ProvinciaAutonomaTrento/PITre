using System;
using DocsPaUtils.Interfaces.DbManagement;

namespace DocsPaDbManagement.Functions.Oracle
{
	/// <summary>
	/// Funzioni per la versione con DB Oracle.
	/// </summary>
	public class Functions : DocsPaUtils.Interfaces.DbManagement.IFunctions
	{
		/// <summary>
		/// Ritorna la stringa SQL relativa all'acquisizione della data di sistema
		/// </summary>
		/// <returns>
		/// </returns>
		public string GetDate() 
		{
			return " SYSDATE";
		}
        /// <summary>
        /// implementa nvl su oracle e isnull su sql
        /// </summary>
        /// <param name="?"></param>
        /// <returns></returns>
        public  string GetNVL(string val1, string val2)
        {
            return "NVL(" + val1 + "," + val2 + ")";
        }
		public string GetDate(bool flgTime) 
		{	
			if(flgTime)
			{
				//inserisce in un campo  data il sysdate formattato dd/mm/yyyy HH24:MI:SS
				return " SYSDATE";	
			}
			else
			{
				//string formatStr = "DD/MM/YYYY";
				//inserisce in un campo  data il sysdate formattato dd/mm/yyyy
				//return " to_date(TO_CHAR(SYSDATE),'" + formatStr + "') ";
				return " SYSDATE";
			}
		}

		/// <summary>
		/// Ritorna la stringa SQL relativa all'acquisizione di un anno
		/// </summary>
		/// <param name="date"></param>
		/// <returns></returns>
		public string GetYear(string date) 
		{
			return " TO_CHAR(" + date + ", 'YYYY') ";
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
                convert = "TO_NUMBER("+ value +")";
            }
            else
            {
                convert = "TO_NUMBER('" + value + "')";
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

            if (columnName.EndsWith("PM") || columnName.EndsWith("AM"))
            {
                //System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("it-IT");
                //System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("en-US");
                DateTime dateTime = Convert.ToDateTime(columnName);

                columnName = dateTime.ToString("dd/MM/yyyy hh:mm:ss");
                //date = dateTime.ToString("dd/MM/yyyy hh:mm");			
            }

            string formatStr = "dd/mm/yyyy HH24:mi:ss";
            //string formatStr = "dd/mm/yyyy HH24:mi";
            string cmdStr = " to_date(" + columnName + ",'" + formatStr + "') ";

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

			if (date.EndsWith("PM") || date.EndsWith("AM"))
			{
				//System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("it-IT");
				//System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("en-US");
				DateTime dateTime=Convert.ToDateTime(date);
				
				date = dateTime.ToString("dd/MM/yyyy hh:mm:ss");	
				//date = dateTime.ToString("dd/MM/yyyy hh:mm");			
			}

			string formatStr = "dd/mm/yyyy HH24:mi:ss";
			//string formatStr = "dd/mm/yyyy HH24:mi";
			string cmdStr = " to_date('" + date + "','" + formatStr + "') ";

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

			if (date.EndsWith("PM") || date.EndsWith("AM"))
			{
				//System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("it-IT");
				//System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("en-US");
				DateTime dateTime=Convert.ToDateTime(date);

				date = dateTime.ToString("dd/MM/yyyy hh:mm:ss");				
			}

			string cmdStr;
			
			if(iniziogiornata)
			{
				cmdStr = " to_date('" + date + " 00:00:00','dd/mm/yyyy HH24:mi:ss') ";
			}
			else
			{
				cmdStr = " to_date('" + date + " 23:59:59','dd/mm/yyyy HH24:mi:ss') ";
			}

			return cmdStr;
		}

		public string ToDate(string columndate, bool time) 
		{
			string formatStr;

			if(columndate == null || columndate.Equals(""))
			{
				return "null";
			}

			if (time) 
			{
				formatStr = "dd/mm/yyyy HH24:mi:ss";
			}
			else					
			{
				formatStr = "dd/mm/yyyy";
			}
	
			
			string cmdStr = " to_date('" + columndate + "','" + formatStr + "') ";

			return cmdStr;
		}

		/// <summary>
		/// </summary>
		/// <returns></returns>
		public string GetSystemIdColName() 
		{
			return "SYSTEM_ID, ";
		}

		/// <summary>
		/// </summary>
		/// <returns></returns>
		public string GetVersionIdColName() 
		{
			return "VERSION_ID, ";
		}
		
		/// <summary>
		/// </summary>
		/// <param name="tableName"></param>
		/// <returns></returns>
		public string GetSystemIdNextVal(string tableName) 
		{
			string retValue=string.Empty;

			string defaultSequenceName=System.Configuration.ConfigurationManager.AppSettings["oracleSequenceName"];

			if (tableName==null || tableName.Equals(string.Empty))
			{
				retValue=defaultSequenceName;
			}
			else
			{
				ISequenceMapper mapper=new OracleSequenceMapper();
				string sequenceName=mapper.GetSequenceName(tableName);

				if (sequenceName==string.Empty)
					// Nel caso in cui per il nome della tabella fornita come parametro
					// non sia stato effettuato il mapping ad una sequence particolare,
					// viene preso il nome della sequence di default
					sequenceName=defaultSequenceName;

				retValue=sequenceName;
			}

			return retValue + ".nextval, ";
		}
		
		/// <summary>
		/// </summary>
		/// <param name="queryString"></param>
		/// <returns></returns>
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
		public string SelectTop(string queryString, string numRighe) 
		{
			if (!(numRighe != null && !numRighe.Equals("0")))
			{
				return queryString;
			}
			return "SELECT * FROM (" + queryString + ") WHERE ROWNUM <= " + numRighe;			
		}

		/// <summary>
		/// </summary>
		/// <param name="colName"></param>
		/// <param name="time"></param>
		/// <returns></returns>
		public string ToChar(string colName, bool time) 
		{
			string formatStr = "";
			string cmdStr;
			if (time) 
			{
				formatStr = "dd/mm/yyyy HH24:mi:ss";
			}
			else					
			{
				formatStr = "dd/mm/yyyy";
			}

			cmdStr = " to_char("+colName+",'" + formatStr + "') ";
		
			return cmdStr;		
		}
		
		/// <summary>
		/// </summary>
		/// <param name="dateString"></param>
		/// <returns></returns>
		public string ToDbDate(string dateString)
		{
			DateTime dateTime = Convert.ToDateTime(dateString);
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
			return " || ";
		}

		/// <summary>		
		/// </summary>
		/// <param name="expr"></param>
		/// <param name="start"></param>
		/// <param name="length"></param>
		/// <returns></returns>
		public string SubStr(string expr, string start, string length)
		{
			return "SUBSTR("+expr+", "+start+", "+length+")";
		}

		/// <summary>
		/// Restiruisce la query per ricavare l'ultimo valore della SYSTEM_ID inserita
		/// </summary>
		/// <returns></returns>
		public string GetQueryLastSystemIdInserted()
		{
			return "SELECT " + System.Configuration.ConfigurationManager.AppSettings["oracleSequenceName"] + ".currval FROM dual";
		}

		/// <summary>
		/// Restiruisce la query per ricavare l'ultimo valore della system_id
		/// inserita per una tabella
		/// </summary>
		/// <param name="tableName"></param>
		/// <returns></returns>
		public string GetQueryLastSystemIdInserted(string tableName)
		{
			string retValue=string.Empty;

			if (tableName!=null && tableName!=string.Empty)
			{
				ISequenceMapper mapper=new OracleSequenceMapper();
				string sequenceName=mapper.GetSequenceName(tableName);
			//TODO: verranno gestite le nuove sequence inserendole sia nel db e nel xml (luciani) 
				if(sequenceName!=null && sequenceName!="")
				{
					retValue=string.Concat("SELECT ",sequenceName, ".currval FROM dual");
				}
				else
				{
					// Se non è stata fornita una tableName,
					// viene reperita la systemId inserita per la sequence di default
					retValue=this.GetQueryLastSystemIdInserted();
				}
			}
			else
			{
				// Se non è stata fornita una tableName,
				// viene reperita la systemId inserita per la sequence di default
				retValue=this.GetQueryLastSystemIdInserted();
			}
			
			return retValue;
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
				case "ETNOTEAM":
                case "PITRE":
                case "DOCUMENTUM":
                case "CDC":
                case "FILENET":
                case "GFD":
					retValue = GetSystemIdNextVal(null);
					break;

				case "HUMMINGBIRD":
					retValue = "SEQSYSTEMKEY.nextval, ";
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
            string retValue = "contains(" + fieldName + ", ";

            string searchText = "'";

            bool append = false;

            DocsPaUtils.Data.FullTextUtils fullTextUtils = new OracleFullTextUtils();

            foreach (DocsPaVO.filtri.SearchTextItem item in items)
            {
                if (append)
                    searchText += " AND ";

                //if (item.SearchOption == DocsPaVO.filtri.SearchTextOptionsEnum.WholeWord)
                //    searchText += fullTextUtils.ParseTextSpecialChars(item.TextToSearch.Trim());
                //else if (item.SearchOption == DocsPaVO.filtri.SearchTextOptionsEnum.InitWithWord)
                //    searchText += fullTextUtils.ParseTextSpecialChars(item.TextToSearch.Trim()) + "*";
                //else if (item.SearchOption == DocsPaVO.filtri.SearchTextOptionsEnum.ContainsWord)
                //    searchText += " " + fullTextUtils.ParseTextSpecialChars(item.TextToSearch.Trim()) + "*";

                searchText += item.TextToSearch.Trim();

                append = true;
            }

            retValue += searchText + "') > 0";

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

            DocsPaUtils.Data.FullTextUtils fullTextUtils = new OracleFullTextUtils();

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
            return string.Empty;
        }
	}
}
