using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Report
{

    /// <summary>
    /// Questo attributo viene utilizzato per decorare le classi responsabile della generazione di un report
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public sealed class ReportGeneratorAttribute : Attribute
    {
        /// <summary>
        /// Nome del report
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// Nome del contesto
        /// </summary>
        public String ContextName { get; set; }

        /// <summary>
        /// Chiave identificativa del report
        /// </summary>
        public String Key { get; set; }

    }

}
