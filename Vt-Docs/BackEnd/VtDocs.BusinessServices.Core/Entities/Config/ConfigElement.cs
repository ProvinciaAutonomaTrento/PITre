using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VtDocs.BusinessServices.Entities.Config
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable()]
    public class ConfigElement
    {
        /// <summary>
        /// Nome chiave di configurazione
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Valore chiave di configurazione
        /// </summary>
        public string Value
        {
            get;
            set;
        }
    }
}
