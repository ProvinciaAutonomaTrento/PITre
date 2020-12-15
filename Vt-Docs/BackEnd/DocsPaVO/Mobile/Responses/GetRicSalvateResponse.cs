using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Mobile.Responses
{
    public class GetRicSalvateResponse
    {

        public GetRicSalvateResponseCode Code
        {
            get;
            set;
        }

        public List<RicercaSalvata> Risultati
        {
            get;
            set;
        }

        public static GetRicSalvateResponse ErrorResponse
        {
            get
            {
                GetRicSalvateResponse resp = new GetRicSalvateResponse();
                resp.Code = GetRicSalvateResponseCode.SYSTEM_ERROR;
                return resp;
            }
        }
    }

    public enum GetRicSalvateResponseCode
    {
        OK,SYSTEM_ERROR
    }
}
