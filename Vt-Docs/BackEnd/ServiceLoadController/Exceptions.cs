using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Web.Services.Protocols;

namespace ServiceLoadController
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable()]
    public class ServiceBusyException : System.ApplicationException
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceName"></param>
        public ServiceBusyException(string serviceName) : this(serviceName, null)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="methodName"></param>
        public ServiceBusyException(string serviceName, string methodName)
        {
            this.ServiceName = serviceName;
            this.MethodName = methodName;
        }

        /// <summary>
        /// 
        /// </summary>
        public override string Message
        {
            get
            {
                if (!string.IsNullOrEmpty(this.MethodName))
                    return string.Format("Il metodo {0} del servizio {1} risulta temporaneamente occupato", this.MethodName, this.ServiceName);
                else
                    return string.Format("Il servizio {0} risulta temporaneamente occupato", this.ServiceName);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Code 
        {
            get
            {
                return "IsBusy";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string ServiceName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string MethodName { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public sealed class SoapExceptionFactory
    {
        /// <summary>
        /// 
        /// </summary>
        private SoapExceptionFactory()
        { }

        /// <summary>
        /// Creazione SoapException custom con i dettagli dell'eccezione
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public static SoapException Create(Exception exception)
        {
            if (exception.GetType() == typeof(ServiceBusyException))
            {
                ServiceBusyException busyEx = (ServiceBusyException)exception;

                throw new SoapException(busyEx.Message, new XmlQualifiedName(busyEx.Code));
            }
            else
            {
                throw new SoapException(exception.Message, SoapException.ServerFaultCode);
            }
        }
    }
}
