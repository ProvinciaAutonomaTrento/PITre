using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace RESTServices.Manager
{
    public class RoutingManager
    {
        private static ILog logger = LogManager.GetLogger(typeof(RoutingManager));
        private static XElement routingMapping;
        static RoutingManager()
        {
            try
            {
                string filePath = "";

                //if (System.Web.Configuration.WebConfigurationManager.AppSettings["XML_ROUTING_MAPPING_PATH"] != null)
                //    filePath = System.Web.Configuration.WebConfigurationManager.AppSettings["XML_ROUTING_MAPPING_PATH"].ToString();

                filePath = "xml/RoutingMapping.xml";
                if (string.IsNullOrWhiteSpace(filePath))
                {
                    routingMapping = null;
                }
                else
                {
                    routingMapping = XElement.Load(HttpContext.Current.Server.MapPath(filePath));
                }
            }
            catch (Exception ex)
            {
                routingMapping = null;
                logger.Error(ex);
            }
        }

        public static string[] GetEndpointAndApps(string administrationCode)
        {
            List<string> retval = new List<string>();

            string endpoint = string.Empty;

            try
            {
                // Verifica che la chiave di configurazione non sia vuota

                if (routingMapping == null)
                {
                    throw new Exception("File di routing non trovato");
                }
                else
                {
                    // Accede all'elemento

                    var elements =
                        from a in routingMapping.Elements("Administration")
                        where a.Element("Code").Value.ToUpper() == administrationCode.ToUpper()
                        select a;

                    if (elements != null && elements.Count() > 0)
                        endpoint = elements.First().Element("Node").Value;

                    retval.Add(endpoint);

                    if (elements != null && elements.Count() > 0)
                    {
                        var apps = elements.Elements("Apps");
                        if (apps != null && apps.Count() > 0)
                        {
                            var appsLst = apps.Elements("App");
                            List<string> appLst = (from a in appsLst select a.Value).ToList();
                            retval.AddRange(appLst);
                        }
                    }

                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return retval.ToArray();
        }

        public static string GetEndpoint(string codeAdm, string AppName)
        {
            String[] EndPointAndApps = GetEndpointAndApps(codeAdm);
            
            string uriRoot = EndPointAndApps.FirstOrDefault();
            if (string.IsNullOrEmpty(uriRoot))
            {
                logger.Error("Amministrazione non configurata nel file locale");
                throw new Exception("Amministrazione non configurata nel file locale");
            }

            List<String> Apps = EndPointAndApps.Skip(1).ToList();

            if (!Apps.Contains(AppName))
            {
                uriRoot = null;
            }

            return uriRoot;
        }
    }
}