using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Domain
{
    /// <summary>
    /// Enumerato tipo filtro
    /// </summary>
    //[DataContract(Namespace = "FilterType")]
    [DataContract(Namespace = "http://nttdata.com/2012/Pi3", Name = "FilterType")]
    public enum FilterTypeEnum
    {
        [EnumMember]
        String,
        [EnumMember]
        Bool,
        [EnumMember]
        Number,
        [EnumMember]
        Date
    }

    /// <summary>
    /// Filtro di ricerca
    /// </summary>
    [DataContract(Namespace = "http://nttdata.com/2012/Pi3")]
    public class Filter
    {
        /// <summary>
        /// Nome del filtro
        /// </summary>
        [DataMember]
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Valore del filtro
        /// </summary>
        [DataMember]
        public string Value
        {
            get;
            set;
        }

        /// <summary>
        /// Nel caso di template popolare il template con i campi di interesse
        /// </summary>
        [DataMember]
        public Template Template
        {
            get;
            set;
        }

        /// <summary>
        /// Descrizione del filtro
        /// </summary>
        [DataMember]
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// Tipo filtro
        /// </summary>
        [DataMember]
        public FilterTypeEnum Type
        {
            get;
            set;
        }
    }
}