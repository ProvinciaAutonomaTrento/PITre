// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace DocsPaVO.fascicolazione
{
    [DataContract]
    public class VoceIndiceSistematico
    {
        [DataMember]
        public string systemId { get; set; }
        [DataMember]
        public string idProject { get; set; }
        [DataMember]
        public string idAmm { get; set; }
        [DataMember]
        public string idTitolario { get; set; }
        [DataMember]
        public string codiceNodo { get; set; }
        [DataMember]
        public string voceIndice { get; set; }
        [DataMember]
        public string descrizioneNodo { get; set; }
    }
}
