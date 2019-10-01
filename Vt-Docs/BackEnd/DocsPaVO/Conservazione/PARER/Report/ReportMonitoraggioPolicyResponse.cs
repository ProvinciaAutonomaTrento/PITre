using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace DocsPaVO.Conservazione.PARER.Report
{
    [Serializable]
    public class ReportMonitoraggioPolicyResponse
    {
        public DocsPaVO.documento.FileDocumento Document { get; set; }
    }
}
