using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocsPaRESTWS.Model
{
    public class ListaRuoliByUserResponse
    {
        public ListaRuoliByUserResponse(){}

        public ListaRuoliByUserResponse(ListaRuoliByUserResponseCode code)
        {
            this.Code = code;
            
        }

        public DocsPaVO.Mobile.RuoloInfo[] ListaRuoli { get; set; }

        public static ListaRuoliByUserResponse ErrorResponse
        {
            get
            {
                ListaRuoliByUserResponse res = new ListaRuoliByUserResponse(ListaRuoliByUserResponseCode.SYSTEM_ERROR);
                return res;
            }
        }

        public ListaRuoliByUserResponseCode Code {get;set;}


    }

    public enum ListaRuoliByUserResponseCode
    {
        OK, USER_NO_ROLES, SYSTEM_ERROR
    }
}