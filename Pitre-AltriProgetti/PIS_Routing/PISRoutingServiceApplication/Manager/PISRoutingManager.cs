using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Xml.Linq;
using System.IO;

namespace PISRoutingServiceApplication.Manager
{
    public class PISRoutingManager
    {
        public static string GetEndpoint(string administrationCode)
        {
            string endpoint = string.Empty;
            string filePath = string.Empty;

            // Verifica configurazione
            //
            try
            {
                filePath = WebConfigurationManager.AppSettings["XML_ROUTING_MAPPING_PATH"].ToString();
            }
            catch (Exception e)
            {
                throw new Exception("Chiave [XML_ROUTING_MAPPING_PATH] non trovata nel file di configurazione (web.config)");
            }

            try
            {
                // Verifica che la chiave di configurazione non sia vuota
                //
                if (string.IsNullOrEmpty(filePath))
                {
                    throw new Exception("Chiave di configurazione [XML_ROUTING_MAPPING_PATH] nulla");
                }
                else
                {
                    // Accede all'elemento
                    //
                    var elements =
                        from a in XElement.Load(filePath).Elements("Administration")
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
            string filePath = string.Empty;
            string endpoint = string.Empty;

            // Verifica configurazione
            //
            try
            {
                filePath = WebConfigurationManager.AppSettings["XML_ROUTING_MAPPING_PATH"].ToString();
            }
            catch (Exception e)
            {
                throw new Exception("Chiave [XML_ROUTING_MAPPING_PATH] non trovata nel file di configurazione (web.config)");
            }


            try
            {
                // Verifica che la chiave di configurazione non sia vuota
                //
                if (string.IsNullOrEmpty(filePath))
                {
                    throw new Exception("Chiave di configurazione [XML_ROUTING_MAPPING_PATH] nulla");
                }
                else
                {
                    // Accede all'elemento
                    //
                    var elements =
                        from a in XElement.Load(filePath).Elements("Administration")
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