using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using DocsPAWA.DocsPaWR;

namespace DocsPAWA.Import.ReportGenerator
{
    public class ReportGenerator
    {
        /// <summary>
        /// Funzione per la generazione del file documento con il report dell'esportazione
        /// </summary>
        /// <param name="templateFilePath">Il path in cui reperire il template per l'espoertazione</param>
        /// <param name="dataSet">Il data set con le informazioni sul report</param>
        /// <returns>L'oggetto file documento con il report</returns>
        public static FileDocumento GetReport(string templateFilePath, DataSet dataSet,string titolo)
        {
            #region Dichiarazione Variabili

            // L'oggetto file documento con le informazioni sul report generato
            FileDocumento toReturn;

            #endregion

            // Creazione oggetto file documento
            toReturn = global::ProspettiRiepilogativi.Frontend.PdfReport.do_MakePdfReport(
                global::ProspettiRiepilogativi.Frontend.ReportDisponibili.ReportLogMassiveImport,
                templateFilePath,
                titolo,
                dataSet,
                null);

            // Restituzione oggetto
            return toReturn;
            
        }
    }
}
