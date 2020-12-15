using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.PrjDocImport
{
    /// <summary>
    /// Classe per la rappresentazione dei dati relativi ad un protocollo di emergenza
    /// </summary>
    public class RDEDocumentRowData : DocumentRowData
    {
        /// Data da assegnare al protocollo di emergenza
        /// </summary>
        public string EmergencyProtocolDate { get; set; }

        /// <summary>
        /// Ora da assegnare al protocollo di emergenza
        /// <summary>
        public string EmergencyProtocolTime { get; set; }

        /// <summary>
        /// La segnatura del documento di emergenza
        /// </summary>
        public string EmergencyProtocolSignature { get; set; }

        /// <summary>
        /// Data del protocollo mittente
        /// </summary>
        public string SenderProtocolDate { get; set; }

        /// <summary>
        /// Il numero di protocollo mittente
        /// </summary>
        public string SenderProtocolNumber { get; set; }

        /// <summary>
        /// La data di arrivo del protocollo mittente
        /// </summary>
        public string ArrivalDate { get; set; }

        /// <summary>
        /// Ora di arrivo del protocollo mittente
        /// </summary>
        public string ArrivalTime { get; set; }


    }
}
