using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocsPaWS.Mobile.PdfToImage
{
    public class PDFConvertException : Exception
    {
        private PDFConvertExceptionCode _code;

        public PDFConvertException()
        {
            _code = PDFConvertExceptionCode.SYSTEM_ERROR;
        }

        public PDFConvertException(PDFConvertExceptionCode code)
        {
            _code = code;
        }

        public PDFConvertExceptionCode Code
        {
            get
            {
                return _code;
            }
        }
    }

    public enum PDFConvertExceptionCode
    {
        PAGE_NOT_FOUND,SYSTEM_ERROR
    }
}