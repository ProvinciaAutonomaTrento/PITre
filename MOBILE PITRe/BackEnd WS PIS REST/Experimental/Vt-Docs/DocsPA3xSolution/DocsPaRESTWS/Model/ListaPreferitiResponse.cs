using DocsPaVO.Mobile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocsPaRESTWS.Model
{
    public class ListaPreferitiResponse
    {
        public ListaPreferitiResponseCode Code { get; set; }

        public List<InfoPreferito> Preferiti
        {
            get;
            set;
        }

        public static ListaPreferitiResponse ErrorResponse
        {
            get
            {
                ListaPreferitiResponse resp = new ListaPreferitiResponse();
                resp.Code = ListaPreferitiResponseCode.SYSTEM_ERROR;
                return resp;
            }
        }
    }

    public enum ListaPreferitiResponseCode
    {
        OK, SYSTEM_ERROR
    }
}