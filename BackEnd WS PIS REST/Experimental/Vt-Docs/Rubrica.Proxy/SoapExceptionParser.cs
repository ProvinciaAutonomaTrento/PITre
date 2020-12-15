using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Services.Protocols;
using System.Xml;

namespace RubricaComune
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class SoapExceptionParser
    {
        /// <summary>
        /// 
        /// </summary>
        private SoapExceptionParser()
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static ApplicationException GetOriginalException(SoapException ex)
        {
            return new ApplicationException(ex.Detail.SelectSingleNode("//ExceptionMessage").InnerXml);
        }
    }
}