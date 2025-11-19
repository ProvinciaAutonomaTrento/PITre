// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace DocsPaVO.documento 
{
    /// <summary>
    /// </summary>
    [Serializable()]
    [DataContract]
    public class Documento : FileRequest
    {
        /// <summary>
        /// Costante che identifica un tipo documento come stampa registro
        /// </summary>
        [DataMember]
        public const string STAMPA_REGISTRO = "STAMPA_REGISTRO";

        [DataMember]
        public string daInviare { get; set; }
        [DataMember]
        public string dataArrivo { get; set; }
        [DataMember]
        public TipologiaCanale tipologia { get; set; }

        /// <summary>
        /// Data di archiviazione del documento in fascicolo cartaceo
        /// </summary>
        [DataMember]
        public string dataArchiviazione { get; set; }
    }
}