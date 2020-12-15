using System;
using System.Web.Services.Protocols;
using System.Xml;

namespace VtDocsAdapter
{
    public class SoapExceptionParser
    {

        /// <summary>
        /// Metodo per il rilancio dell'eccezione originale
        /// </summary>
        /// <param name="ex">Eccezione da analizzare</param>
        public static void ThrowOriginalException(Exception ex)
        {
            throw GetOriginalException(ex);
        }

        /// <summary>
        /// Metodo per il recupero dell'eccezione originale a partire da una eccezione SOAP
        /// </summary>
        /// <param name="ex">Eccezione SOAP da spacchettare</param>
        /// <returns>Eccezione originale contenuta all'interno dell'eccezione SOAP</returns>
        public static ApplicationException GetOriginalException(Exception ex)
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
    }
}
