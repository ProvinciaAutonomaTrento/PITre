// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocsPaVO.documento;

namespace DocsPaVO.RegistroAccessi
{
    [Serializable()]
    public class RegistroAccessiReportResponse
    {
        public Boolean success { get; set; }

        public FileDocumento document { get; set; }
    }
}
