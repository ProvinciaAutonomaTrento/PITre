using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Mobile.Responses
{
    public class DelegheResponse
    {
        public List<Delega> Elements
        {
            get;
            set;
        }

        public int TotalRecordCount
        {
            get;
            set;
        }

        public DelegheResponseCode Code
        {
            get;
            set;
        }

        public static DelegheResponse ErrorResponse
        {
            get
            {
                DelegheResponse resp = new DelegheResponse();
                resp.Code = DelegheResponseCode.SYSTEM_ERROR;
                return resp;
            }
        }
    }

    public enum DelegheResponseCode
    {
        OK,SYSTEM_ERROR
    }
}
