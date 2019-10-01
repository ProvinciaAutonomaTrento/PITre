using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocsPaVO.filtri;

namespace DocsPaVO.RegistroAccessi
{
    [Serializable()]
    public class RegistroAccessiReportRequest
    {
        public RequestType requestType { get; set; }

        public List<FiltroRicerca> filters { get; set; }

    }

    [Serializable()]
    public enum RequestType
    {
        EXPORT,
        PUBLISH
    }
}
