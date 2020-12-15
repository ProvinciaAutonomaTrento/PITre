
namespace DocsPaUtils.Interfaces.DbManagement
{
	/// <summary>
	/// </summary>
	public interface IFunctions
	{	
		/// <summary>
		/// Ritorna la stringa SQL relativa all'acquisizione di una data
		/// </summary>
		/// <returns>
		/// </returns>
		string GetDate();

		/// <summary>
		/// Ritorna la stringa SQL relativa all'acquisizione di una data
		/// </summary>
		/// <param name="flgTime"></param>
		/// <returns></returns>
		string GetDate(bool flgTime);

		/// <summary>
		/// Ritorna la stringa SQL relativa all'acquisizione di un anno
		/// </summary>
		/// <param name="date"></param>
		/// <returns></returns>
		string GetYear(string date);

		/// <summary>
		/// Converte una stringa nel formato SQL relativo ad una data
		/// </summary>
		/// <param name="date"></param>
		/// <returns></returns>
		string ToDate(string date);

		/// <summary>
		/// </summary>
		/// <returns></returns>
		string GetSystemIdColName();

		/// <summary>
		/// </summary>
		/// <returns></returns>
		string GetVersionIdColName();
        
        /// <summary>
        /// implementa nvl / isNull
        /// </summary>
        /// <param name="val1"></param>
        /// <param name="val2"></param>
        /// <returns></returns>
        string GetNVL(string val1, string val2);
		/// <summary>
		/// </summary>
		/// <param name="tableName"></param>
		/// <returns></returns>
		string GetSystemIdNextVal(string tableName);

		/// <summary>
		/// </summary>
		/// <param name="queryString"></param>
		/// <returns></returns>
		/// <remarks>modificato per ricerca Top N Documenti</remarks>
		string SelectTop(string queryString);

		/// <summary>
		/// </summary>
		/// <param name="queryString"></param>
		/// <param name="numRighe"></param>
		/// <returns></returns>
		/// <remarks>modificato per ricerca Top N Documenti</remarks>
		string SelectTop(string queryString, string numRighe);

		/// <summary>
		/// </summary>
		/// <param name="colName"></param>
		/// <param name="time"></param>
		/// <returns></returns>
		string ToChar(string colName, bool time);
		
		/// <summary>
		/// </summary>
		/// <param name="dateString"></param>
		/// <returns></returns>
		string ToDbDate(string dateString);

		// Metodi aggiunti per uniformità
		string ToDateBetween(string date, bool iniziogiornata);

		string ToDate(string columndate, bool time);

        string ToDateColumn(string columnName);

		string ConcatStr();
		
		string SubStr(string expr, string start, string length);

		string GetQueryLastSystemIdInserted();

		string GetQueryLastSystemIdInserted(string tableName);

		string GetSystemKeyHumm();

        /// <summary>
        /// Generazione della stringa di query relativa alla ricerca di un 
        /// valore in un campo impostato come fulltext
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        string GetContainsTextQuery(string fieldName, params DocsPaVO.filtri.SearchTextItem[] items);

        /// <summary>
        /// Generazione della stringa di query relativa alla ricerca di un 
        /// valore in un campo impostato come fulltext
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        string GetContainsTextQuery(string fieldName, string value);

        /// <summary>
        /// Reperimento utente della sessione corrente
        /// </summary>
        /// <returns></returns>
        string GetDbUserSession();

        /// <summary>
        /// Generazione della stringa di query relativa alla ricerca di un valore intero
        /// Se il parametro è un campo di tabello "isColumn" è true
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        string ToInt(string value, bool isColumn);

        /// <summary>
        /// gestione carattere °
        /// </summary>
        /// <returns></returns>
        string convertDegre();
    }
}
