using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WcfRouting
{
    public static class ConfigurationMgr
    {
        /*
         * PArametri nel config per test
         *     
         * <add key="ClientCert" value="CN=Pi3RouterTest"/>
           <add key="DefaultServerCert" value="CN=Pi3IntegrationIstanceTest"/>
           <add key ="BindingPerPis" value="WsHttpCertText"/>
           <add key ="UseSecurity" value="true"/>
        *  <add key ="dnsIdentity" value="Pi3IntegrationIstanceTest"/>
         */
        public static bool useSecurity { get; set; }
        public static string clientCer { get; set; }
        public static string serverCer { get; set; }
        public static string bindingPerPis { get; set; }
        public static string dnsIdentity { get; set; }
        public static string endpointConfigurationName { get; set; }
        public static string UseLocalFileRouting_string { get; set; }
        public static string UseAppMapping_string { get; set; }
        public static string filePath { get; set; }

        static ConfigurationMgr()
        {
            useSecurity = bool.Parse(System.Web.Configuration.WebConfigurationManager.AppSettings["UseSecurity"]);
            clientCer = System.Web.Configuration.WebConfigurationManager.AppSettings["ClientCert"];
            serverCer = System.Web.Configuration.WebConfigurationManager.AppSettings["DefaultServerCert"];
            bindingPerPis = System.Web.Configuration.WebConfigurationManager.AppSettings["BindingPerPis"];
            dnsIdentity = System.Web.Configuration.WebConfigurationManager.AppSettings["dnsIdentity"];
            endpointConfigurationName = System.Web.Configuration.WebConfigurationManager.AppSettings["addresToRouteBasic"];
            UseLocalFileRouting_string = System.Web.Configuration.WebConfigurationManager.AppSettings["UseLocalFileRouting"];
            UseAppMapping_string = System.Web.Configuration.WebConfigurationManager.AppSettings["UseAppMapping"];
            if (System.Web.Configuration.WebConfigurationManager.AppSettings["XML_ROUTING_MAPPING_PATH"] != null)
                filePath = System.Web.Configuration.WebConfigurationManager.AppSettings["XML_ROUTING_MAPPING_PATH"].ToString();
            else filePath = String.Empty;

        }
    }
}