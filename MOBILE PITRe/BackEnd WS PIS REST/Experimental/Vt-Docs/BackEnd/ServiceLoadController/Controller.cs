using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Configuration;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.IO;

namespace ServiceLoadController
{
    /// <summary>
    /// 
    /// </summary>
    public class LoadControllerManager
    {
        #region Private Members

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static ServiceLoadInfo[] GetInstance()
        {
            if (System.Web.HttpContext.Current.Cache["ServiceLoadController.istance"] == null)
            {
                ServiceLoadInfo[] istance = LoadData();

                System.Web.HttpContext.Current.Cache.Insert("ServiceLoadController.istance", 
                            istance, 
                            new System.Web.Caching.CacheDependency(Configurations.XmlFilePath));
            }

            return (ServiceLoadInfo[]) System.Web.HttpContext.Current.Cache["ServiceLoadController.istance"];
        }

        /// <summary>
        /// Caricamento dati da file xml
        /// </summary>
        /// <returns></returns>
        private static ServiceLoadInfo[] LoadData()
        {
            List<ServiceLoadInfo> data = new List<ServiceLoadInfo>();

            if (!string.IsNullOrEmpty(Configurations.XmlFilePath) &&
                System.IO.File.Exists(Configurations.XmlFilePath))
            {
                using (FileStream stream = new FileStream(Configurations.XmlFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    XmlReader reader = new XmlTextReader(stream);

                    ServiceLoadInfo info = null;

                    while (reader.Read())
                    {
                        if (reader.IsStartElement("services"))
                        {
                            // Verifica se i servizi di controllo agli accessi a tutti i servizi sono abilitati o meno
                            bool isEnabled;
                            bool.TryParse(reader.GetAttribute("enabled"), out isEnabled);
                            if (!isEnabled)
                                break;
                        }
                        else if (reader.IsStartElement("serviceLoad"))
                        {
                            info = new ServiceLoadInfo
                            {
                                ServiceName = reader.GetAttribute("serviceName"),
                                ServiceCallInterval = Int32.Parse(reader.GetAttribute("serviceInterval"))
                            };

                            data.Add(info);
                        }
                        else if (reader.IsStartElement("methodLoad"))
                        {
                            MethodLoadInfo mInfo = new MethodLoadInfo
                            {
                                MethodName = reader.GetAttribute("methodName"),
                                MethodCallInterval = Int32.Parse(reader.GetAttribute("methodInterval"))
                            };

                            List<MethodLoadInfo> methods = new List<MethodLoadInfo>(info.Methods);
                            methods.Add(mInfo);
                            info.Methods = methods.ToArray();
                        }
                    }
                }
            }
            
            return data.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        private static ServiceLoadInfo GetService(string serviceName)
        {
            ServiceLoadInfo[] instance = GetInstance();

            return instance.Where(e => e.ServiceName == serviceName).FirstOrDefault();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="methodName"></param>
        /// <returns></returns>
        private static MethodLoadInfo GetMethod(string serviceName, string methodName)
        {
            return GetMethod(GetService(serviceName), methodName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="service"></param>
        /// <param name="methodName"></param>
        /// <returns></returns>
        private static MethodLoadInfo GetMethod(ServiceLoadInfo service, string methodName)
        {
            return service.Methods.Where(e => e.MethodName == methodName).FirstOrDefault();
        }

        #endregion

        #region Public Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceName"></param>
        public static void CheckInterval(string serviceName)
        {
            ServiceLoadInfo serviceInfo = GetService(serviceName);

            if (serviceInfo != null)
            {
                // Reperimento ts memorizzato
                if (serviceInfo.LastServiceExecution > 0)
                {
                    DateTime now = DateTime.Now;

                    // Differenza tra ultima esecuzione e istante attuale
                    TimeSpan ts = TimeSpan.FromTicks(now.Ticks - serviceInfo.LastServiceExecution);

                    if (ts.TotalMilliseconds > ((double)serviceInfo.ServiceCallInterval))
                    {
                        // salva il TS attuale bloccando la risorsa condivisa per il tempo necessario al salvataggio
                        lock (serviceInfo)
                        {
                            serviceInfo.LastServiceExecution = now.Ticks;
                        }
                    }
                    else
                    {
                        throw SoapExceptionFactory.Create(new ServiceBusyException(serviceName, string.Empty));
                    }
                }
                else
                {
                    // Prima esecuzione del check, autorizzazione consentita

                    // salva il TS attuale bloccando la risorsa condivisa per il tempo necessario al salvataggio
                    lock (serviceInfo)
                    {
                        serviceInfo.LastServiceExecution = DateTime.Now.Ticks;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="methodName"></param>
        public static void CheckInterval(string serviceName, string methodName)
        {
            ServiceLoadInfo serviceInfo = GetService(serviceName);

            if (serviceInfo != null)
            {
                MethodLoadInfo methodInfo = GetMethod(serviceInfo, methodName);

                if (methodInfo != null)
                {
                    // Reperimento ts memorizzato
                    if (methodInfo.LastMethodExecution > 0)
                    {
                        DateTime now = DateTime.Now;
                        
                        // Differenza tra ultima esecuzione e istante attuale
                        TimeSpan ts = TimeSpan.FromTicks(now.Ticks - methodInfo.LastMethodExecution);

                        if (ts.TotalMilliseconds > ((double)methodInfo.MethodCallInterval))
                        {
                            // salva il TS attuale bloccando la risorsa condivisa per il tempo necessario al salvataggio
                            lock (methodInfo)
                            {
                                methodInfo.LastMethodExecution = now.Ticks;
                            }
                        }
                        else
                        {
                            throw SoapExceptionFactory.Create(new ServiceBusyException(serviceName, methodName));
                        }
                    }
                    else
                    {
                        // Prima esecuzione del check, autorizzazione consentita

                        // salva il TS attuale bloccando la risorsa condivisa per il tempo necessario al salvataggio
                        lock (methodInfo)
                        {
                            methodInfo.LastMethodExecution = DateTime.Now.Ticks;
                        }
                    }
                }
                else
                {
                    // Metodo non definito
                    CheckInterval(serviceName);
                }
            }
        }

        #endregion
    }
}
