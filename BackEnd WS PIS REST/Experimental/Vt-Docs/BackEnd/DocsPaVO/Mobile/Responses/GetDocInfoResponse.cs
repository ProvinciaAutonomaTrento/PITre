using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Mobile.Responses
{
    public class GetDocInfoResponse
    {
        public DocInfo DocInfo
        {
            get;
            set;
        }

        public List<DocInfo> Allegati
        {
            get;
            set;
        }

        public TrasmInfo TrasmInfo
        {
            get;
            set;
        }

        public GetDocInfoResponseCode Code
        {
            get;
            set;
        }

        public static GetDocInfoResponse ErrorResponse
        {
            get
            {
                GetDocInfoResponse resp = new GetDocInfoResponse();
                resp.Code = GetDocInfoResponseCode.SYSTEM_ERROR;
                return resp;
            }
        }

    }

    public enum GetDocInfoResponseCode
    {
        OK, SYSTEM_ERROR
    }
}
