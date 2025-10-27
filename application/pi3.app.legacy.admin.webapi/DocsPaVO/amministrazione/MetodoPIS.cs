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
    public class MetodoPIS
    {
        [DataMember]
        public string IdMetodo { get; set; } = string.Empty;

        [DataMember]
        public string MethodName { get; set; } = string.Empty;

        [DataMember]
        public string Description { get; set; } = string.Empty;

        [DataMember]
        public string FileSVC { get; set; } = string.Empty;
    }
}
