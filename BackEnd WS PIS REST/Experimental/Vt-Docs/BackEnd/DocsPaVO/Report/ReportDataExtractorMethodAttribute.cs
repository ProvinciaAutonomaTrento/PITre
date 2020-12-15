using System;

namespace DocsPaVO.Report
{

    /// <summary>
    /// Attributo da utilizzare per la decorazione del metodo da richiamare per effettuare l'estrazione dati
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class ReportDataExtractorMethodAttribute : Attribute
    {
        /// <summary>
        /// Identifica il nome del report cui il metodo che utilizza l'attributo si riferisce
        /// </summary>
        public string ContextName { get; set; }
    }
}
