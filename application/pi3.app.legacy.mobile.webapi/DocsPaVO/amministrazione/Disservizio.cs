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
    public class Disservizio
    {
        [DataMember]
        public string system_id { get; set; } = string.Empty;

        [DataMember]
        public string stato { get; set; } = string.Empty;

        [DataMember]
        public string testo_notifica { get; set; } = string.Empty;

        [DataMember]
        public string testo_cortesia { get; set; } = string.Empty;

        [DataMember]
        public string testo_email_notifica { get; set; } = string.Empty;

        [DataMember]
        public string testo_email_ripresa { get; set; } = string.Empty;

        [DataMember]
        public string notificato { get; set; } = string.Empty;

    }
}
