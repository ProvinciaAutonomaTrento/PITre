// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

namespace DocsPaVO.Conservazione.PARER
{
    [Serializable()]
    [DataContract]
    public class EsecuzionePolicy
    {
        [DataMember]
        public string idPolicy { get; set; }
        [DataMember]
        public string dataUltimaEsecuzione { get; set; }
        [DataMember]
        public string dataProssimaEsecuzione { get; set; }
        [DataMember]
        public string numeroEsecuzioni { get; set; }
    }
}
