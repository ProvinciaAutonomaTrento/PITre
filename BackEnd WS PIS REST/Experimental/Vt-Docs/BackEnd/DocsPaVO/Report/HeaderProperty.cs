using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace DocsPaVO.Report
{
    /// <summary>
    /// Header del report
    /// </summary>
    public class HeaderProperty
    {
        /// <summary>
        /// Nome originale del campo
        /// </summary>
        public String OriginalName { get; set; }

        /// <summary>
        /// Nome della colonna
        /// </summary>
        public String ColumnName { get; set; }

        public bool Export { get; set; }

        public override bool Equals(object obj)
        {
            HeaderProperty hp = obj as HeaderProperty;
            return base.Equals(obj) &&
                hp != null && hp.OriginalName.Equals(this.OriginalName);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() + this.OriginalName.GetHashCode();
        }
        
        /// <summary>
        /// Dimensione della colonna
        /// </summary>
        public int ColumnSize { get; set; }

        /// <summary>
        /// Enumerazione di possibili tipi di dato contenuti in una cella
        /// </summary>
        public enum ContentDataType
        {
            Boolean,
            DateTime,
            Integer,
            Number,
            String
        }


        /// <summary>
        /// Tipo di dati contenuti nella colonna
        /// </summary>
        [DefaultValue(ContentDataType.String)]
        public ContentDataType DataType { get; set; }


    }

}
