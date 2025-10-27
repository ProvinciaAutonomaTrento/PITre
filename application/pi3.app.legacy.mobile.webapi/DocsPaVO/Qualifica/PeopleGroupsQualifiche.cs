// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace DocsPaVO.Qualifica
{
    [Serializable()]
    [DataContract]  
    public class PeopleGroupsQualifiche
    {
        [DataMember]
        public int SYSTEM_ID { get; set; }

        [DataMember]
        public int ID_AMM { get; set; }

        [DataMember]
        public int ID_UO { get; set; }

        [DataMember]
        public int ID_GRUPPO { get; set; }

        [DataMember]
        public int ID_PEOPLE { get; set; }

        [DataMember]
        public int ID_QUALIFICA { get; set; }

        [DataMember]
        public String CODICE { get; set; }

        [DataMember]
        public string DESCRIZIONE { get; set; }

        public PeopleGroupsQualifiche() { }
    } 
}
