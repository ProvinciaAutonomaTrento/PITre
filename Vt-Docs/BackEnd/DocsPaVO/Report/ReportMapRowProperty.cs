using System;
using System.Collections.Generic;
using System.Linq;

namespace DocsPaVO.Report
{
    /// <summary>
    /// Questo oggetto rappresenta una riga del report
    /// </summary>
    [Serializable()]
    public class ReportMapRowProperty
    {
        public ReportMapRowProperty()
        {
            this.Columns = new List<ReportMapColumnProperty>();
        }

        /// <summary>
        /// Lista delle proprietà delle colonne del report
        /// </summary>
        public List<ReportMapColumnProperty> Columns { get; set; }

        /// <summary>
        /// Accessore alle proprietà delle colonne
        /// </summary>
        /// <param name="index">Indice della colonna</param>
        /// <returns>Proprietà della colonna</returns>
        public ReportMapColumnProperty this[int index]
        {
            get 
            {
                return this.Columns[index];
            }
            set 
            {
                this.Columns[index] = value;
            }
        }

        /// <summary>
        /// Accessore per il recupero delle proprietà a partire dal nome originale
        /// </summary>
        /// <param name="name">Nome della colonna</param>
        /// <returns>Proprietà recuperata</returns>
        public ReportMapColumnProperty this[String name]
        {
            get
            {
                return this.Columns.Where(e => e.OriginalName.ToLower().Equals(name.ToLower())).FirstOrDefault();
            }
        }

        
        public override bool Equals(object obj)
        {
            return obj is ReportMapRowProperty && ((ReportMapRowProperty)obj).Columns.Equals(this.Columns); 
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() + this.Columns.GetHashCode();
        }

    }

}
