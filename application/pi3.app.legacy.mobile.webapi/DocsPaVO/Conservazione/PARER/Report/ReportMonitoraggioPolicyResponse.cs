// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

namespace DocsPaVO.Conservazione.PARER.Report
{
    [Serializable]
    [DataContract]
    public class ReportMonitoraggioPolicyResponse
    {
        [DataMember]
        public DocsPaVO.documento.FileDocumento Document { get; set; }
    }
}
