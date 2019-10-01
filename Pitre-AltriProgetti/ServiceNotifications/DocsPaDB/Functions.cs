using System;
using System.Web;
using System.Configuration;
using DocsPaUtils.Interfaces.DbManagement;

namespace DocsPaDbManagement.Functions
{
    public class Functions
    {
        #region Variabili di classe

        protected static string dbType = "";
        protected static IFunctions providerFunctions = null;

        #endregion

        #region "Costruttori"

        static Functions()
        {
            dbType = ConfigurationManager.AppSettings["dbType"].ToUpper();

            switch (dbType)
            {
                case "SQL":
                    providerFunctions = new DocsPaDbManagement.Functions.SqlServer.Functions();
                    break;
                case "ORACLE":
                    providerFunctions = new DocsPaDbManagement.Functions.Oracle.Functions();
                    break;
                //case "ORACLENATIVE":
                //    providerFunctions = new DocsPaDbManagement.Functions.OracleNative.Functions();
                //    break;
            }
        }

        #endregion

        #region IFunctions members

        public static string GetDate()
        {
            return providerFunctions.GetDate();
        }
        /// <summary>
        /// implementa nvl su oracle e isnull su sql, val1 è la colonna
        /// val2 il valore che deve sostituire se la colonna a valore null.
        /// Attenzione, se la colonna è varchar o char, allora inserire val2 tra apici.
        /// </summary>
        /// <param name="val1"></param>
        /// <param name="val2"></param>
        /// <returns></returns>
        public static string getNVL(string val1, string val2)
        {
            return providerFunctions.GetNVL(val1, val2);
        }
        public static string GetDate(bool flgTime)
        {
            return providerFunctions.GetDate(flgTime);
        }

        public static string GetYear(string date)
        {
            return providerFunctions.GetYear(date);
        }

        public static string ToDate(string date)
        {
            return providerFunctions.ToDate(date);
        }

        public static string GetSystemIdColName()
        {
            return providerFunctions.GetSystemIdColName();
        }

        public static string GetVersionIdColName()
        {
            return providerFunctions.GetVersionIdColName();
        }

        public static string GetSystemIdNextVal(string tableName)
        {
            return providerFunctions.GetSystemIdNextVal(tableName);
        }

        public static string SelectTop(string queryString)
        {
            return providerFunctions.SelectTop(queryString);
        }

        public static string SelectTop(string queryString, string numRighe)
        {
            return providerFunctions.SelectTop(queryString, numRighe);
        }

        public static string ToChar(string colName, bool time)
        {
            return providerFunctions.ToChar(colName, time);
        }

        public static string ToDbDate(string dateString)
        {
            return providerFunctions.ToDbDate(dateString);
        }

        public static string ToDateBetween(string date, bool iniziogiornata)
        {
            return providerFunctions.ToDateBetween(date, iniziogiornata);
        }

        public static string ToDate(string columndate, bool time)
        {
            return providerFunctions.ToDate(columndate, time);
        }

        public static string ConcatStr()
        {
            return providerFunctions.ConcatStr();
        }

        public static string SubStr(string expr, string start, string length)
        {
            return providerFunctions.SubStr(expr, start, length);
        }

        public static string GetQueryLastSystemIdInserted()
        {
            return providerFunctions.GetQueryLastSystemIdInserted();
        }

        public static string GetQueryLastSystemIdInserted(string tableName)
        {
            return providerFunctions.GetQueryLastSystemIdInserted(tableName);
        }

        public static string GetSystemKeyHumm()
        {
            return providerFunctions.GetSystemKeyHumm();
        }

        public static string GetContainsTextQuery(string fieldName, params DocsPaVO.filtri.SearchTextItem[] items)
        {
            return providerFunctions.GetContainsTextQuery(fieldName, items);
        }

        public static string GetContainsTextQuery(string fieldName, string value)
        {
            return providerFunctions.GetContainsTextQuery(fieldName, value);
        }
        /// <summary>
        /// Reperimento utente della sessione corrente
        /// </summary>
        /// <returns></returns>
        public static string GetDbUserSession()
        {
            return providerFunctions.GetDbUserSession();
        }

        public static string ToDateColumn(string columnName)
        {
            return providerFunctions.ToDateColumn(columnName);
        }

        public static string ToInt(string value, bool isColumn)
        {
            return providerFunctions.ToInt(value, isColumn);
        }

        public static string convertDegre()
        {
            return providerFunctions.convertDegre();
        }
        #endregion
    }
}
