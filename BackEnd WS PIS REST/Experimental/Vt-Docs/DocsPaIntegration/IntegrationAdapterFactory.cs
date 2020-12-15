using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using DocsPaIntegration.Attributes;
using DocsPaIntegration.Config;
using System.IO;
using System.Security.Policy;
using log4net;

namespace DocsPaIntegration
{
    public class IntegrationAdapterFactory
    {
        private static Dictionary<string, IntegrationAdapterInfo> _adaptersMap;
        private static string _errorMessage;
        private static bool _initError=false;
        private static IntegrationAdapterFactory _instance;
        private static ILog logger = LogManager.GetLogger(typeof(IntegrationAdapterFactory));

        static IntegrationAdapterFactory()
        {
            LoadMap();
            if(_initError==false) _instance=new IntegrationAdapterFactory();
        }

        public static IntegrationAdapterFactory Instance
        {
            get
            {
                if (_initError) throw new Exception(_errorMessage);
                return _instance;
            }
        }

        public List<IntegrationAdapterInfo> AdapterInfos
        {
            get{
                return _adaptersMap.Values.ToList<IntegrationAdapterInfo>();
            }
        }

        private static void LoadMap(){
            try
            {
                logger.Info("BEGIN");
                _adaptersMap = new Dictionary<string, IntegrationAdapterInfo>();
                foreach (string path in IntegrationAdapterAssemblies)
                {
                    ProcessAssembly(path);
                }
                logger.Info("END");
            }
            catch (Exception e)
            {
                _initError = true;
                _errorMessage = e.Message;
            }
        }

        private static void ProcessAssembly(string path)
        {
            logger.Debug("path: " + path);
            Assembly ass = Assembly.LoadFile(path);
            Type[] types = ass.GetTypes();
            foreach (Type type in types)
            {
                if (_initError) break;
                Type[] interfaces = type.GetInterfaces();
                foreach (Type interf in interfaces)
                {
                    if (interf == typeof(IIntegrationAdapter))
                    {
                        logger.Debug("Found adapter: " + type.Name);
                        PutTypeInMap(type);
                        break;
                    }
                }
            }
        }

        private static void PutTypeInMap(Type type)
        {
            IntegrationAdapterAttribute[] attrs=(IntegrationAdapterAttribute[]) type.GetCustomAttributes(typeof(IntegrationAdapterAttribute), false);
            if (attrs.Length > 0)
            {
                IntegrationAdapterAttribute attr = attrs[0];
                string key = buildKey(attr.UniqueId,attr.Version);
                if (_adaptersMap.ContainsKey(key))
                {
                    logger.Debug("Presente altro tipo con UniqueId " + attr.UniqueId+" e Version "+attr.Version);
                }
                else
                {
                    _adaptersMap.Add(key, new IntegrationAdapterInfo(attr.UniqueId,attr.Name,attr.Description,attr.Version,type,attr.HasIcon));
                }
            }
        }

        public static List<string> IntegrationAdapterAssemblies
        {
            get
            {
                List<string> res=new List<string>();
                string path = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
                logger.Debug("path to check: " + path);
                string[] paths=Directory.GetFiles(path, "*.dll",SearchOption.AllDirectories);
                foreach (string dll in paths)
                {
                    logger.Debug("Checking dll with loadFrom " + dll);
                    try{
                        Assembly _assembly = Assembly.Load(System.IO.File.ReadAllBytes(dll));
                        IntegrationAdapterAssemblyAttribute[] attrs = (IntegrationAdapterAssemblyAttribute[])_assembly.GetCustomAttributes(typeof(IntegrationAdapterAssemblyAttribute), false);
                        if (attrs.Length > 0)
                        {
                            logger.Debug("Dll " + dll + " contains adapters");
                            res.Add(dll);
                        }
                    }catch(Exception e){
                        logger.Error("Error in checking " + dll + ": " + e);
                    };
                }
                logger.Debug("Dll found: " + res.Count);
                return res;
            }
        }

        private static string buildKey(string uniqueId,Version version)
        {
            string key = uniqueId;
            if (version!=null)
            {
                key = key + "||||" + version;
            }
            return key;
        }

        public IIntegrationAdapter GetAdapter(string uniqueId)
        {
            return GetAdapter(uniqueId, null);
        }

        public IIntegrationAdapter GetAdapter(string uniqueId,Version version)
        {
            string key = buildKey(uniqueId, version);
            if (!_adaptersMap.ContainsKey(key)) throw new Exception("Proxy con UniqueId " + uniqueId + " e Version "+version+" non trovato");
            IntegrationAdapterInfo adapterInfo = _adaptersMap[key];
            IIntegrationAdapter instance = (IIntegrationAdapter)Activator.CreateInstance(adapterInfo.Type);
            return instance;
        }

        public IIntegrationAdapter GetAdapterConfigured(ConfigurationInfo configInfo)
        {
            IIntegrationAdapter adapter = null;
            if (configInfo.AdapterVersion != null)
            {
                adapter = this.GetAdapter(configInfo.AdapterUniqueId, configInfo.AdapterVersion);
            }
            else
            {
                adapter = this.GetAdapter(configInfo.AdapterUniqueId);
            }
            adapter.Init(configInfo);
            return adapter;
        }

    }
}
