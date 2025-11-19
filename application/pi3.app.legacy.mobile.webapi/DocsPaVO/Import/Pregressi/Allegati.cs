// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace DocsPaVO.Import.Pregressi
{
    [Serializable()]
    [DataContract]
    public class Allegati
    {
        [DataMember]
        public string systemId { get; set; } = string.Empty;
        [DataMember]
        public string idItem { get; set; } = string.Empty;
        [DataMember]
        public string errore { get; set; } = string.Empty;
        [DataMember]
        public string esito { get; set; } = string.Empty;
        [DataMember]
        public string ordinale { get; set; } = string.Empty;
        [DataMember]
        public string descrizione { get; set; } = string.Empty;
        [DataMember]
        public string pathname { get; set; } = string.Empty;

        public Allegati()
        { }
    }
}
