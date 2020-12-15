using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Mobile.Responses
{
    public class ADLActionResponse
    {
        public AddToADLResponseCode Code
        {
            get;
            set;
        }
        public static ADLActionResponse ErrorResponse
        {
            get
            {
                ADLActionResponse resp = new ADLActionResponse();
                resp.Code = AddToADLResponseCode.SYSTEM_ERROR;
                return resp;
            }
        }
    
    }

    public enum AddToADLResponseCode
    {
        OK, SYSTEM_ERROR
    }
}
