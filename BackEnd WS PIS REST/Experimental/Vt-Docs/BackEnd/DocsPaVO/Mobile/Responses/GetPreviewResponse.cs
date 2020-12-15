using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Mobile.Responses
{
    public class GetPreviewResponse
    {
        public FileInfo File
        {
            get;
            set;
        }

        public GetPreviewResponseCode Code
        {
            get;
            set;
        }

        public static GetPreviewResponse ErrorResponse
        {
            get
            {
                GetPreviewResponse resp = new GetPreviewResponse();
                resp.Code = GetPreviewResponseCode.SYSTEM_ERROR;
                return resp;
            }
        }
    }

    public enum GetPreviewResponseCode
    {
        OK,SYSTEM_ERROR,NO_PREVIEW_FOR_EXTENSION,PAGE_NOT_FOUND
    }
}
