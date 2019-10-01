using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Mobile.Responses
{
    public class RevocaDelegheResponse
    {
        public RevocaDelegheResponseCode Code
        {
            get;
            set;
        }

        public string Error
        {
            get;
            set;
        }

        public static RevocaDelegheResponse ErrorResponse
        {
            get
            {
                RevocaDelegheResponse resp = new RevocaDelegheResponse();
                resp.Code = RevocaDelegheResponseCode.SYSTEM_ERROR;
                return resp;
            }
        }
    }

    public enum RevocaDelegheResponseCode
    {
        OK, SYSTEM_ERROR, OPERATION_FAILED
    }
}
