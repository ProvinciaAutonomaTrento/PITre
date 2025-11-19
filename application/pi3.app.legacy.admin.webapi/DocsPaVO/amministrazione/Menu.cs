// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace DocsPaVO.amministrazione
{
    [DataContract]
    public class Menu
    {
        [DataMember]
        public string IDMenu { get; set; } = string.Empty;
        [DataMember]
        public string Codice { get; set; } = string.Empty;
        [DataMember]
        public string Descrizione { get; set; } = string.Empty;
        [DataMember]
        public string Associato { get; set; } = string.Empty;
        [DataMember]
        public string Visibilita { get; set; } = string.Empty;
    }
}
