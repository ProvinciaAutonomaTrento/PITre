// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Serilog;
using System;
using System.Data;

namespace DocsPaUtils.Data
{
    /// <summary>
    /// Classe per la gestione delle trasformazioni da/a DatSet tipizzato.
    /// </summary>
    /// <remarks>
    /// Questa non � la soluzione ottimale poich� la conversione/acquisizione dei dati �
    /// ancora troppo macchinosa. L'ideale sarebbe trovare un modo automatico per importare
    /// i dati del System.Data.DataSet nel corrispondente dataset tipizzato.
    /// </remarks>
    public class TypedDataSetManager
	{
        private static ILogger logger = Log.ForContext(typeof(TypedDataSetManager));
        /// <summary>
        /// Importa i dati di un System.DataidataSet in un dato dataset tipizzato
        /// </summary>
        /// <param name="source">DataSet dal quale acquisire i dati</param>
        /// <param name="destination">DataSet del dataset tipizzato da popolare</param>
        /// <returns>true: OK; false: Errore</returns>
        public static bool MakeTyped(DataSet source, DataSet destination)
		{
			bool result = true; // Presume successo

			try
			{
				/* Copia tutti i record di tutte le tabelle dal dataset
				 * sorgente a quello di destinazione
				 */
				foreach(DataTable table in source.Tables)
				{
					foreach(DataRow row in table.Rows)
					{
						destination.Tables[0].ImportRow(row);
					}
				}
			}
			catch(Exception exception)
			{
				logger.Error(exception, "Error MakeTyped");
				result = false;
			}

			return result;
		}
	}
}
