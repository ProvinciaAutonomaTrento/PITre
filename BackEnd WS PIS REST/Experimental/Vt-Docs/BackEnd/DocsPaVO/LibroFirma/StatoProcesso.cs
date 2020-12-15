using System;
using System.Xml.Serialization;
using System.Collections;

namespace DocsPaVO.LibroFirma
{
    [XmlType("StatoProcesso")]
    public class StatoProcesso
    {
        /// <summary>
        /// In esecuzione
        /// </summary>
        public static string IN_EXEC = "IN_EXEC";
        /// <summary>
        /// Interrotto
        /// </summary>
        public static string STOPPED = "STOPPED";
        /// <summary>
        /// Terminato
        /// </summary>
        public static string CLOSED = "CLOSED";
    }
}
