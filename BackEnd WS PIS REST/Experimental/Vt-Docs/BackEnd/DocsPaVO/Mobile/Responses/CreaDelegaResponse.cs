using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Mobile.Responses
{
    public class CreaDelegaResponse
    {
        public CreaDelegaResponseCode Code
        {
            get;
            set;
        }

        public static CreaDelegaResponse ErrorResponse
        {
            get
            {
                CreaDelegaResponse resp = new CreaDelegaResponse();
                resp.Code = CreaDelegaResponseCode.SYSTEM_ERROR;
                return resp;
            }
        }
    }

    public enum CreaDelegaResponseCode
    {
        OK, SYSTEM_ERROR, NOT_CREATED, OVERLAPPING_PERIODS
    }
}
