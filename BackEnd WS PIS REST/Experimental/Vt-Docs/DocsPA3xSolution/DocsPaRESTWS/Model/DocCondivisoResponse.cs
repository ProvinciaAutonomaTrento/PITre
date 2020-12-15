using DocsPaVO.Mobile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocsPaRESTWS.Model
{
    public class DocCondivisoResponse
    {
        public DocCondivisoResponse() { }

        public DocCondivisoResponse(DocCondivisoResponseCode code)
        {
            this.Code = code;
        }

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
            

        public DocCondivisoResponseCode Code
        {
            get;
            set;
        }

        public static DocCondivisoResponse ErrorResponse
        {
            get
            {
                DocCondivisoResponse resp = new DocCondivisoResponse(DocCondivisoResponseCode.SYSTEM_ERROR);
                return resp;
            }
        }

    }

    public enum DocCondivisoResponseCode
    {
        OK, WRONG_USER, EXPIRED, SYSTEM_ERROR
    }
}