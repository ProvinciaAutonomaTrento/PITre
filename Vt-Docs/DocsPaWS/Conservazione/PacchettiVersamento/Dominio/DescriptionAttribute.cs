using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocsPaWS.Conservazione.PacchettiVersamento.Dominio
{
    /// <summary>
    /// 
    /// </summary>
    public class DescriptionAttribute : Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        public string Value
        {
            get;
            set;
        }
    }
}