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
