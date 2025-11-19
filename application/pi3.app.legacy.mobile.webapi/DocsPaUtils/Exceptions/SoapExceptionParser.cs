// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Xml;

namespace DocsPaUtils.Exceptions
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