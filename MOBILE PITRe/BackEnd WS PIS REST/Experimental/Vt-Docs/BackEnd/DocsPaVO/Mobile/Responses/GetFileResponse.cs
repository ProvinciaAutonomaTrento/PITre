using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Mobile.Responses
{
    public class GetFileResponse
    {
        public FileInfo File
        {
            get;
            set;
        }

        public GetFileResponseCode Code
        {
            get;
            set;
        }

        public static GetFileResponse ErrorResponse
        {
            get
            {
                GetFileResponse resp = new GetFileResponse();
                resp.Code = GetFileResponseCode.SYSTEM_ERROR;
                return resp;
            }
        }
    }

    public enum GetFileResponseCode
    {
        OK,SYSTEM_ERROR
    }
}
