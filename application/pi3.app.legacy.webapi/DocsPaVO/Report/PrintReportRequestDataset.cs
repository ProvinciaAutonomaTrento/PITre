// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace DocsPaVO.Report
{
    [Serializable()]
    public class PrintReportRequestDataset : PrintReportRequest
    {
        public DataSet InputDataset { get; set; }
    }
}
