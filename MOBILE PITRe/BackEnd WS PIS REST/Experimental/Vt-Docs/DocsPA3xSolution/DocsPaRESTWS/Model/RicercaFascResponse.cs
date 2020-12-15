using DocsPaVO.Mobile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocsPaRESTWS.Model
{
    public class RicercaFascResponse
    {
        public List<RicercaFascElement> Risultati
        {
            get;
            set;
        }

        public int TotalRecordCount
        {
            get;
            set;
        }

        public RicercaFascResponseCode Code
        {
            get;
            set;
        }

        public static RicercaFascResponse ErrorResponse
        {
            get
            {
                RicercaFascResponse resp = new RicercaFascResponse();
                resp.Code = RicercaFascResponseCode.SYSTEM_ERROR;
                return resp;
            }
        }
    }

    public enum RicercaFascResponseCode
    {
        OK, SYSTEM_ERROR
    }

    public class RicercaFascElement
    {
        public RicercaElement InfoElement { get; set; }
        public List<DocInfo> Documenti { get; set; }
    }
}