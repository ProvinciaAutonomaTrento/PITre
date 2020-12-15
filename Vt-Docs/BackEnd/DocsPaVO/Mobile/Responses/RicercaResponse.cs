using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Mobile.Responses
{
    public class RicercaResponse
    {
        public string IdRicercaSalvata
        {
            get;
            set;
        }

        public List<RicercaElement> Risultati
        {
            get;
            set;
        }

        public int TotalRecordCount
        {
            get;
            set;
        }

        public RicercaResponseCode Code
        {
            get;
            set;
        }

        public static RicercaResponse ErrorResponse
        {
            get
            {
                RicercaResponse resp = new RicercaResponse();
                resp.Code = RicercaResponseCode.SYSTEM_ERROR;
                return resp;
            }
        }
    }

    public enum RicercaResponseCode
    {
        OK,SYSTEM_ERROR
    }
}
