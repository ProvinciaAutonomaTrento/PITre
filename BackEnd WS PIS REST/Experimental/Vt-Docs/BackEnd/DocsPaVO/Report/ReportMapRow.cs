using System.Collections.Generic;
using System.Linq;
using System;

namespace DocsPaVO.Report
{
    /// <summary>
    /// Questo oggetto rappresenta una riga del report
    /// </summary>
    [Serializable()]
    public class ReportMapRow
    {
        public ReportMapRow()
        {
            this.Rows = new List<ReportMapRowProperty>();
        }

        /// <summary>
        /// Insieme delle righe costituenti il report
        /// </summary>
        public List<ReportMapRowProperty> Rows { get; set; }

        /// <summary>
        /// Accessore alle proprietà delle righe
        /// </summary>
        /// <param name="index">Indice della proprietà</param>
        /// <returns>Proprietà della riga</returns>
        public ReportMapRowProperty this[int index]
        {
            get 
            {
                return this.Rows[index];
            }
            set 
            {
                this.Rows[index] = value;
            }
        }

        /// <summary>
        /// Recupero delle proprietà relative ad una riga
        /// </summary>
        /// <param name="property">Proprietà da recuperare</param>
        /// <returns>Proprietà della riga richiesta</returns>
        public ReportMapRowProperty this[ReportMapRowProperty property]
        {
            get
            {
                return this.Rows.Where(e => e.Equals(property)).FirstOrDefault();
            }

        }

    }
}
