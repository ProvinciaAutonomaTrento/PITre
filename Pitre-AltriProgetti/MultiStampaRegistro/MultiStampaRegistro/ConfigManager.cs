using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MultiStampaRegistro
{
    class ConfigManager
    {        
        private string configFile;
        private Dictionary<string, string> configCollection;

        public ConfigManager()
        {            
        }

        public ConfigManager(string filePath)
        {
            this.LoadFile(filePath);
        }

        public bool LoadFile(string filePath)
        {
            // check file existence
            if (!File.Exists(filePath))
                return false;

            this.configFile = filePath;
            this.ParseConfig();

            return true;
        }

        // config must be in in format KEY=VALUE
        // line starts with # ; ' will be ignored
        // values within "val" or 'val' will be cleared
        private void ParseConfig()
        {
            configCollection = new Dictionary<string, string>();
            // parsing config file           
            foreach (string line in File.ReadAllLines(configFile))
            {
                if ((!string.IsNullOrEmpty(line)) &&
                    (!line.Trim().StartsWith(";")) &&
                    (!line.Trim().StartsWith("#")) &&
                    (!line.Trim().StartsWith("'")) && (line.Contains("=")))
                {
                    int index = line.IndexOf('=');
                    string key = line.Substring(0, index).Trim();
                    string value = line.Substring(index + 1).Trim();

                   
                    if ((value.StartsWith("\"") && value.EndsWith("\"")) ||
                        (value.StartsWith("'") && value.EndsWith("'")))
                    {
                        value = value.Substring(1, value.Length - 2);
                    }
                    configCollection.Add(key, value);                   
                }
            }            
        }

        public string GetValue(string key)
        {
            if (configCollection == null)
                return null;

            string value = "";
            configCollection.TryGetValue(key, out value);

            return value;
        }

        public bool ExistKey(string key)
        {
            return configCollection.ContainsKey(key);         
        }
    }
}
 