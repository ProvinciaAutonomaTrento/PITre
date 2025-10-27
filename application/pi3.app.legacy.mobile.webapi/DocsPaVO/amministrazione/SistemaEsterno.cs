// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace DocsPaVO.amministrazione
{
    [DataContract]
    public class SistemaEsterno
    {
        [DataMember]
        public string IdSistemaEsterno { get; set; } = string.Empty;

        [DataMember]
        public string CodiceApplicazione { get; set; } = string.Empty;

        [DataMember]
        public string DescApplicazione { get; set; } = string.Empty;

        [DataMember]
        public string Diritti { get; set; } = string.Empty;

        [DataMember]
        public string UserIdAssociato { get; set; } = string.Empty;

        [DataMember]
        public string idRuoloAssociato { get; set; } = string.Empty;

        [DataMember]
        public string idAmministrazione { get; set; } = string.Empty;

        [DataMember]
        public string DescEstesa { get; set; } = string.Empty;

        [DataMember]
        public int TokenPeriod { get; set; } = 0;
    }
}
