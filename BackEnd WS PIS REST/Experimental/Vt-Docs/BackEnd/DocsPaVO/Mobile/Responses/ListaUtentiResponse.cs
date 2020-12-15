using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Mobile.Responses
{
    public class ListaUtentiResponse
    {
        public ListaUtentiResponseCode Code
        {
            get; 
            set;
        }

        public List<UserInfo> Utenti
        {
            get; 
            set;
        }

        public static ListaUtentiResponse ErrorResponse{
            get
            {
                ListaUtentiResponse resp = new ListaUtentiResponse();
                resp.Code = ListaUtentiResponseCode.SYSTEM_ERROR;
                return resp;
            }
        }
    }

    public enum ListaUtentiResponseCode
    {
        OK,SYSTEM_ERROR
    }
}
