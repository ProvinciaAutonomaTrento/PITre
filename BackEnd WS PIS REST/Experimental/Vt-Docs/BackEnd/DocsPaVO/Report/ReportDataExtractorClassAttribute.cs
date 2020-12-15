using System;

namespace DocsPaVO.Report
{
    /// <summary>
    /// Attributo da utilizzare per la decorazione di classi che contengono metodi per la
    /// generazione di reportistica
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public sealed class ReportDataExtractorClassAttribute : Attribute
    {
    }
}
