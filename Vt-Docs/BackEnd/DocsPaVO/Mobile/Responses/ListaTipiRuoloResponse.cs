using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Mobile.Responses
{
    public class ListaTipiRuoloResponse
    {
        public ListaRuoliResponseCode Code
        {
            get; 
            set;
        }

        public List<TipoRuoloInfo> TipiRuolo
        {
            get;
            set;
        }

        public static ListaTipiRuoloResponse ErrorResponse
        {
            get
            {
                ListaTipiRuoloResponse resp = new ListaTipiRuoloResponse();
                resp.Code = ListaRuoliResponseCode.OK;
                return resp;
            }
        }
    }

    public enum ListaRuoliResponseCode
    {
        OK,SYSTEM_ERROR
    }
}
