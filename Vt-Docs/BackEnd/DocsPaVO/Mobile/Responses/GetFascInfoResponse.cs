using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Mobile.Responses
{
    public class GetFascInfoResponse{
        public FascInfo FascInfo
        {
            get;
            set;
        }

        public TrasmInfo TrasmInfo
        {
            get;
            set;
        }

        public GetFascInfoResponseCode Code
        {
            get;
            set;
        }

        public static GetFascInfoResponse ErrorResponse
        {
            get
            {
                GetFascInfoResponse resp = new GetFascInfoResponse();
                resp.Code = GetFascInfoResponseCode.SYSTEM_ERROR;
                return resp;
            }
        }

    }

    public enum GetFascInfoResponseCode
    {
        OK, SYSTEM_ERROR
    }
}
