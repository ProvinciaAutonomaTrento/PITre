using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Xml.Linq;
using System.IO;

namespace WcfRouting.LocalManager
{
    public static class PISRoutingManager
    {
        private static XElement routingMapping;
        static PISRoutingManager()
        {
            if (string.IsNullOrEmpty(ConfigurationMgr.filePath))
            {
                routingMapping = null;
            }
            else
            {
                routingMapping = XElement.Load(ConfigurationMgr.filePath);
            }
        }

        public static string GetEndpoint(string administrationCode)
        {
            string endpoint = string.Empty;



            try
            {
                // Verifica che la chiave di configurazione non sia vuota
                if (routingMapping == null)
                {
                    throw new Exception("Chiave di configurazione [XML_ROUTING_MAPPING_PATH] nulla");
                }
                else
                {
                    // Accede all'elemento

                    var elements =
                        from a in routingMapping.Elements("Administration")
                        where a.Element("Code").Value == administrationCode
                        select a;

                    if (elements != null && elements.Count() > 0)
                        endpoint = elements.First().Element("Node").Value;
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return endpoint;
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
                    throw new Exception("Chiave di configurazione [XML_ROUTING_MAPPING_PATH] nulla");
                }
                else
                {
                    // Accede all'elemento

                    var elements =
                        from a in routingMapping.Elements("Administration")
                        where a.Element("Code").Value == administrationCode
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
    }
}