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
    }

    public enum AddToADLResponseCode
    {
        OK, SYSTEM_ERROR
    }
}
