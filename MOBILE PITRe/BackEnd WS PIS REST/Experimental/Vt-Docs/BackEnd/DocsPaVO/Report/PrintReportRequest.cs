using System;
using System.Collections.Generic;
using DocsPaVO.filtri;
using DocsPaVO.utente;

namespace DocsPaVO.Report
{

    /// <summary>
    /// Enumerazione dei possibile report generabili
    /// </summary>
    [Serializable()]
    public enum ReportTypeEnum
    {
        /// <summary>
        /// Report PDF
        /// </summary>
        PDF,
        /// <summary>
        /// Report Excel
        /// </summary>
        Excel,
        /// <summary>
        /// Fogli di calcolo Open Office
        /// </summary>
        ODS
    }
    
    /// <summary>
    /// Questo oggetto rappresenta la richiesta da inviare al servizio di generazione report.
    /// Nota per gli sviluppatori: Chi volesse creare una response personalizzata e specifica
    /// per un determinato caso d'uso, può estendere questa classe.
    /// </summary>
    [Serializable()]
    public class PrintReportRequest
    {
        /// <summary>
        /// Informazioni sull'utente richiedente
        /// </summary>
        public InfoUtente UserInfo { get; set; }

        /// <summary>
        /// Lista dei filtri di ricerca da utilizzare per filtrare i dati da esportare
        /// </summary>
        public List<FiltroRicerca> SearchFilters { get; set; }

        /// <summary>
        /// Contesto in cui viene invocata l'esportazione (Ad esempio TransmissionModel)
        /// </summary>
        public String ContextName { get; set; }

        /// <summary>
        /// Tipo di report da generare
        /// </summary>
        public ReportTypeEnum ReportType { get; set; }

        /// <summary>
        /// Titolo da assegnare al report
        /// </summary>
        public String Title { get; set; }

        /// <summary>
        /// Sottotitolo del report
        /// </summary>
        public String SubTitle { get; set; }

        /// <summary>
        /// Ulteriori informazioni da inserire nel report
        /// </summary>
        public String AdditionalInformation { get; set; }

        /// <summary>
        /// Chiave identificativa del report
        /// </summary>
        public String ReportKey { get; set; }

        /// <summary>
        /// Collezione delle colonne da esportare
        /// </summary>
        public HeaderColumnCollection ColumnsToExport { get; set; }


    }
}
