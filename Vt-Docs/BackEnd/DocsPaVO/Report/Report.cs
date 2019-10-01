using System;
using System.Collections.Generic;

namespace DocsPaVO.Report
{
    /// <summary>
    /// Questo oggetto rapprersenta un report
    /// </summary>
    [Serializable()]
    public class Report
    {
        public Report()
        {
            this.ReportMapRow = new ReportMapRow();
        }

        /// <summary>
        /// Titolo del report
        /// </summary>
        public String Title { get; set; }

        /// <summary>
        /// Riepilogo dei dati generati dal report
        /// </summary>
        public String Summary { get; set; }

        /// <summary>
        /// Data di creazione del report
        /// </summary>
        public DateTime CreationDate { get; set; }

        /// <summary>
        /// Sottotitolo
        /// </summary>
        public String Subtitle { get; set; }

        /// <summary>
        /// Informazioni aggiuntive
        /// </summary>
        public String AdditionalInformation { get; set; }

        /// <summary>
        /// Lista delle proprietà delle colonne
        /// </summary>
        public HeaderColumnCollection ReportHeader { set; get; }

        /// <summary>
        /// Insieme dei mapping dati fra sorgente dati e righe del report
        /// </summary>
        public ReportMapRow ReportMapRow { get; set; }

        /// <summary>
        /// Nel caso di report su più fogli (e.g. Excel) permette di impostare il nome del foglio
        /// </summary>
        public String SectionName { get; set; }

        /// <summary>
        /// Se false, non mostra le informazioni(title, subtitle...) in cima alla tabella
        /// </summary>
        public bool showHeaderRow  = true;

        public bool ShowHeaderRow
        {
            get
            {
                return showHeaderRow;
            }
            set
            {
                showHeaderRow = value;
            }
        }
    }
}
