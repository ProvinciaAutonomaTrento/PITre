// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Authenticate.Authenticate;
using Serilog;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Utils
{
    public class RoutingUtils: IRoutingUtils
    {
        public RoutingUtils(ILogger<RoutingUtils> logger, IConfiguration config)
        {
            this._logger = logger;
            this._config = config;
            try
            {
                string basePathFiles = "datafiles/RoutingMapping_TEST.xml";
                if(_config!= null && !string.IsNullOrWhiteSpace(_config.GetValue<string>("RestRoutingOptions:RoutingMappingPath")))
                {
                    basePathFiles = _config.GetValue<string>("RestRoutingOptions:RoutingMappingPath");
                }
                // compatibilità con linux
                string filepath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location) ?? "", basePathFiles);
                //string filepath = Path.Combine(basePathDirectory, basePathFile);

                _logger.LogDebug(filepath);
                //filepath = "xml/RoutingMapping.xml";
                if (string.IsNullOrWhiteSpace(filepath))
                {
                    _routingMapping = null;
                }
                else
                {
                    _routingMapping = XElement.Load(filepath);
                }
            }
            catch (Exception ex)
            {
                _routingMapping = null;
                _logger.LogError(ex, "Errore nel caricamento del file di routing");

            }
        }

        public string[] GetEndpointAndApps(string administrationCode)
        {
            List<string> retval = new List<string>();

            string endpoint = string.Empty;
            string instance = string.Empty;
            string filepath = string.Empty;
            try
            {
                // Verifica che la chiave di configurazione non sia vuota
                if (_routingMapping == null)
                {
                    try
                    {
                        
                        string basePathFiles = "datafiles/RoutingMapping_TEST.xml";
                        if (_config != null && !string.IsNullOrWhiteSpace(_config.GetValue<string>("RestRoutingOptions:RoutingMappingPath")))
                        {
                            basePathFiles = _config.GetValue<string>("RestRoutingOptions:RoutingMappingPath");
                        }
                        // compatibilità con linux
                        filepath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location) ?? "", basePathFiles);
                        
                        _routingMapping = XElement.Load(filepath);
                        if (_routingMapping == null) throw new Exception("Errore nel caricamento del file di routing");
                    }
                    catch (Exception exrouting)
                    {
                        throw new Exception("File di routing non trovato nel path: " + filepath);
                    }
                }

                // Accede all'elemento

                var elements =
                    from a in _routingMapping.Elements("Administration")
                    where a.Element("Code").Value.ToUpper() == administrationCode.ToUpper()
                    select a;

                if (elements != null && elements.Count() > 0)
                    endpoint = elements.First().Element("Node").Value;

                retval.Add(endpoint);

                if (elements != null && elements.Count() > 0)
                    instance = elements.First().Element("Instance").Value;
                retval.Add(instance);

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
            catch (Exception e)
            {
                throw e;
            }
            return retval.ToArray();
        }

        public string GetEndpointAndInstance(string codeAdm, string AppName, out string instance)
        {
            String[] EndPointAndApps = GetEndpointAndApps(codeAdm);

            string uriRoot = EndPointAndApps.FirstOrDefault();
            if (string.IsNullOrEmpty(uriRoot))
            {
                throw new Exception("Amministrazione non configurata nel file locale");
            }

            instance = EndPointAndApps.Skip(1).FirstOrDefault();
            List<String> Apps = EndPointAndApps.Skip(2).ToList();

            if (!Apps.Contains(AppName))
            {
                uriRoot = null;
            }

            return uriRoot;
        }

        protected readonly ILogger<RoutingUtils> _logger;
        private readonly IConfiguration _config;
        private static XElement? _routingMapping;


    }
}
