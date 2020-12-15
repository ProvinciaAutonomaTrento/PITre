using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Mobile.Responses
{
    public class GetSmistamentoElementsResponse
    {
        public List<SmistamentoElement> Elements
        {
            get;
            set;
        }

        public GetSmistamentoElementsResponseCode Code
        {
            get;
            set;
        }

        public int TotalRecordCount
        {
            get;
            set;
        }

        public static GetSmistamentoElementsResponse ErrorResponse
        {
            get
            {
                GetSmistamentoElementsResponse resp = new GetSmistamentoElementsResponse();
                resp.Code = GetSmistamentoElementsResponseCode.SYSTEM_ERROR;
                return resp;
            }
        }
    }

    public enum GetSmistamentoElementsResponseCode
    {
        OK, SYSTEM_ERROR
    }
}
