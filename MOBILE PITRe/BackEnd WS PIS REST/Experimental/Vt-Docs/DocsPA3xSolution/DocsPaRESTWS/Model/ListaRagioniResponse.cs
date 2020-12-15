using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocsPaRESTWS.Model
{
    public class ListaRagioniResponse
    {
        public ListaRagioniResponseCode Code { get; set; }

        public List<DocsPaVO.trasmissione.RagioneTrasmissione> Ragioni
        {
            get;
            set;
        }

        public static ListaRagioniResponse ErrorResponse
        {
            get
            {
                ListaRagioniResponse resp = new ListaRagioniResponse();
                resp.Code = ListaRagioniResponseCode.SYSTEM_ERROR;
                return resp;
            }
        }
    }

    public enum ListaRagioniResponseCode
    {
        OK, SYSTEM_ERROR
    }
}