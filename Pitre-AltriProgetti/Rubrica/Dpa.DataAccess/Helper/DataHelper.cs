using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Dpa.DataAccess.Helper
{
    /// <summary>
    /// Classe di utilità per la gestione del reperimento dei dati
    /// </summary>
    public sealed class DataReaderHelper
    {
        /// <summary>
        /// Reperimento del valore da un oggetto DataReader
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <param name="fieldName"></param>
        /// <param name="nullable"></param>
        /// <returns></returns>
        public static T GetValue<T>(IDataReader reader, string fieldName, bool nullable)
        {
            return GetValue<T>(reader, fieldName, nullable, default(T));
        }

        /// <summary>
        /// Reperimento del valore da un oggetto DataReader
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <param name="fieldName"></param>
        /// <param name="nullable"></param>
        /// <param name="defaultValue">Valore di default in caso il valore del campo è null</param>
        /// <returns></returns>
        public static T GetValue<T>(IDataReader reader, string fieldName, bool nullable, T defaultValue)
        {
            int ordinal = reader.GetOrdinal(fieldName);

            if (!nullable && reader.IsDBNull(ordinal))
                throw new ApplicationException(string.Format("Valore richiesto per il campo '{0}' del DataReader", fieldName));
            else
            {
                if (reader.IsDBNull(ordinal))
                    return defaultValue;
                else
                    return (T)reader.GetValue(ordinal);
            }
        }

        /// <summary>
        /// Reperimento del valore da un oggetto DataRow
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="row"></param>
        /// <param name="fieldName"></param>
        /// <param name="nullable"></param>
        /// <returns></returns>
        public static T GetValue<T>(DataRow row, string fieldName, bool nullable)
        {
            return GetValue<T>(row, fieldName, nullable, default(T));
        }

        /// <summary>
        /// Reperimento del valore da un oggetto DataRow
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="row"></param>
        /// <param name="fieldName"></param>
        /// <param name="nullable"></param>
        /// <param name="defaultValue">Valore di default in caso il valore del campo è null</param>
        /// <returns></returns>
        public static T GetValue<T>(DataRow row, string fieldName, bool nullable, T defaultValue)
        {
            if (!nullable && row[fieldName] == DBNull.Value)
                throw new ApplicationException(string.Format("Valore richiesto per il campo '{0}' del DataRow", fieldName));
            else
            {
                if (row[fieldName] == DBNull.Value)
                    return defaultValue;
                else
                    return (T)row[fieldName];
            }
        }
    }
}