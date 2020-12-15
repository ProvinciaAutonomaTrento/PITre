using System;
using System.Xml.Serialization;
using System.Collections;

namespace DocsPaVO.LibroFirma
{
    [XmlType("StatoElementoInLF")]
    [Serializable()]
    public class StatoElementoInLF
    {
        /// <summary>
        /// Nuovo/Proposto
        /// </summary>
        public static int PROPOSTO = 1;
        /// <summary>
        /// In attesa di firma
        /// </summary>
        public static int DA_FIRMARE = 2;
        /// <summary>
        /// In attesa di respingimento
        /// </summary>
        public static int DA_RESPINGERE = 3;
        /// <summary>
        /// Firma apposta
        /// </summary>
        public static int FIRMATO = 4;
        /// <summary>
        /// Respingimento eseguito
        /// </summary>
        public static int RESPINTO = 5;
        /// <summary>
        /// Interrotto
        /// </summary>
        public static int INTERROTTO = 6;
        /// <summary>
        /// Rifiutato perchè non di competenza
        /// </summary>
        public static int NO_COMPETENZA = 7;
    }
}
