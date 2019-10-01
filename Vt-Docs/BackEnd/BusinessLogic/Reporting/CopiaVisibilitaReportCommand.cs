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
