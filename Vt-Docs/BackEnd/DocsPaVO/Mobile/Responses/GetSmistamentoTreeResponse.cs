using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Mobile.Responses
{
    public class GetSmistamentoTreeResponse
    {
        public SmistamentoTree Element
        {
            get;
            set;
        }
        public GetSmistamentoTreeResponseCode Code
        {
            get;
            set;
        }

        public static GetSmistamentoTreeResponse ErrorResponse
        {
            get
            {
                GetSmistamentoTreeResponse res = new GetSmistamentoTreeResponse();
                res.Code = GetSmistamentoTreeResponseCode.SYSTEM_ERROR;
                return res;
            }
        }
    }

    public enum GetSmistamentoTreeResponseCode
    {
        OK, SYSTEM_ERROR
    }
}
