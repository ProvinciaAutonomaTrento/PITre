using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Mobile.Responses
{
    public class EseguiTrasmResponse
    {
        public EseguiTrasmResponseCode Code
        {
            get;
            set;
        }

        public static EseguiTrasmResponse ErrorResponse
        {
            get
            {
                EseguiTrasmResponse res = new EseguiTrasmResponse();
                res.Code = EseguiTrasmResponseCode.SYSTEM_ERROR;
                return res;
            }
        }
    }

    public enum EseguiTrasmResponseCode
    {
        OK,SYSTEM_ERROR
    }
}
