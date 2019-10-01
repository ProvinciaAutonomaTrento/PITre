using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Mobile.Responses
{
    public class CreaDelegaDaModelloResponse
    {
        public CreaDelegaDaModelloResponseCode Code
        {
            get;
            set;
        }

        public static CreaDelegaDaModelloResponse ErrorResponse
        {
            get
            {
                CreaDelegaDaModelloResponse resp = new CreaDelegaDaModelloResponse();
                resp.Code = CreaDelegaDaModelloResponseCode.SYSTEM_ERROR;
                return resp;
            }
        }
    }

    public enum CreaDelegaDaModelloResponseCode
    {
        OK,NOT_CREATED,SYSTEM_ERROR
    }
}
