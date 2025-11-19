// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace DocsPaVO.Qualifica
{
    [Serializable()]
    [DataContract]
    public class Qualifica
    {
        [DataMember]
        public int SYSTEM_ID { get; set; }

        [DataMember]
        public string CODICE { get; set; }

        [DataMember]
        public string DESCRIZIONE { get; set; }

        [DataMember]
        public int ID_AMM { get; set; }

        public Qualifica() { }	
    }
}
