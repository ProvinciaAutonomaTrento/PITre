// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace DocsPaVO.ProfilazioneDinamica
{
    [Serializable()]
    [DataContract]
    public class Contatore
    {
        [DataMember]
        public string SYSTEM_ID { get; set; }
        [DataMember]
        public string ID_OGG { get; set; }
        [DataMember]
        public string ID_TIPOLOGIA { get; set; }
        [DataMember]
        public string ID_AOO { get; set; }
        [DataMember]
        public string ID_RF { get; set; }
        [DataMember]
        public string VALORE { get; set; }
        [DataMember]
        public string ABILITATO { get; set; }
        [DataMember]
        public string ANNO { get; set; }
        [DataMember]
        public string VALORE_SC { get; set; }
        [DataMember]
        public string CODICE_RF_AOO { get; set; }
        [DataMember]
        public string DESC_RF_AOO { get; set; }

        public Contatore() { }	
    }
}
