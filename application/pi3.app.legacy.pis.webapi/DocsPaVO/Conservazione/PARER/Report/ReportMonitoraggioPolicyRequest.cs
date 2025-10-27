// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace DocsPaVO.Conservazione.PARER.Report
{
    [Serializable]
    public class ReportMonitoraggioPolicyRequest
    {
        public string IdAmm { get; set; }

        public string Codice { get; set; }

        public string Descrizione { get; set; }

        public string TipoDataEsecuzione { get; set; }

        public string DataEsecuzioneFrom { get; set; }

        public string DataEsecuzioneTo { get; set; }
    }
}
