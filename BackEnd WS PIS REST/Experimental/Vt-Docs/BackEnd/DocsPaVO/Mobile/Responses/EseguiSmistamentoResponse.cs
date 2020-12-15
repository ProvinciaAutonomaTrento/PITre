using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Mobile.Responses
{
    public class EseguiSmistamentoResponse
    {
        public EseguiSmistamentoResponseCode Code
        {
            get;
            set;
        }

        public static EseguiSmistamentoResponse ErrorResponse
        {
            get
            {
                EseguiSmistamentoResponse res = new EseguiSmistamentoResponse();
                res.Code = EseguiSmistamentoResponseCode.SYSTEM_ERROR;
                return res;
            }
        }
    }

    public enum EseguiSmistamentoResponseCode
    {
        OK, SYSTEM_ERROR
    }
}
