// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Runtime.Serialization;

namespace DocsPaVO.ProfilazioneDinamicaLite
{
    [Serializable()]
    [DataContract]
    public class TemplateLite
    {
        [DataMember]
        public string system_id { get; set; } = string.Empty;
        [DataMember]
        public string name { get; set; } = string.Empty;
        [DataMember]
        public string idDiagram { get; set; } = string.Empty;
    }
}
