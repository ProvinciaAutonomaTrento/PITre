using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaIntegration
{
    [Serializable]
    public class IntegrationAdapterInfo
    {
        private string _uniqueId;
        private string _name;
        private string _description;
        private Version _version;
        private Type _type;
        private bool _hasIcon;

        public IntegrationAdapterInfo(string uniqueId, string name,string description, Version version,Type type,bool hasIcon)
        {
            _uniqueId = uniqueId;
            _name = name;
            _description = description;
            _version = version;
            _type = type;
            _hasIcon = hasIcon;
        }

        public string UniqueId
        {
            get
            {
                return _uniqueId;
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
        }

        public string Description
        {
            get
            {
                return _description;
            }
        }

        public Version Version
        {
            get
            {
                return _version;
            }
        }

        public Type Type
        {
            get
            {
                return _type;
            }
        }
        public bool HasIcon
        {
            get
            {
                return _hasIcon;
            }
        }
    }
}
