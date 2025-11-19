// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocsPaVO.Report;

namespace BusinessLogic.Reporting
{
    [ReportGenerator(Name = "Copia visibilità", ContextName = "CopiaVisibilita", Key = "CopiaVisibilita")]
    public class CopiaVisibilitaReportCommand : ReportGeneratorCommand
    {
        protected override DocsPaVO.Report.HeaderColumnCollection GetExportableFieldsCollection()
        {
            return null;
        }
    }
}
