using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Services.Protocols;
using System.Xml;

namespace NttDataWA.Utils
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
        public static void ThrowOriginalException(Exception ex)
        {
            throw GetOriginalException(ex);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static ApplicationException GetOriginalException(Exception ex)
        {
            try
            {
                SoapException soapEx = ex as SoapException;

                if (soapEx != null)
                {
                    string customExMessage = string.Empty;

                    XmlNode detailNode = soapEx.Detail.SelectSingleNode("//ExceptionMessage");

                    if (detailNode != null)
                        customExMessage = detailNode.InnerXml;
                    else
                        customExMessage = soapEx.Message;

                    return new ApplicationException(customExMessage);
                }
                else
                    return new ApplicationException(ex.Message);
            }
            catch (System.Exception exc)
            {
                UIManager.AdministrationManager.DiagnosticError(exc);
                return null;
            }
        }
    }
}