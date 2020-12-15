using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Report
{
    /// <summary>
    /// Request utilizzabile per la realizzazione di report a partire da dati contenuti in un oggetto
    /// </summary>
    [Serializable()]
    public class PrintReportObjectTransformationRequest : PrintReportRequest
    {
        /// <summary>
        /// Oggetto con i dati da esportare
        /// </summary>
        public List<Object> DataObject { get; set; }

    }

    /// <summary>
    /// Attributo con cui marcare le proprietà di un oggetto che devono essere esportate
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
    public sealed class PropertyToExportAttribute : Attribute 
    {
        /// <summary>
        /// Nome del campo da stampare
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// Tipo di dato
        /// </summary>
        public Type Type { get; set; }

    }

}
