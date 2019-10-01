using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaIntegration.Attributes
{
    [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class IntegrationAdapterAttribute : Attribute
    {
        private string _uniqueId;
        private string _name;
        private string _description;
        private Version _version;
        private bool _hasIcon;

        public IntegrationAdapterAttribute(string uniqueId, string name,string description,bool hasIcon)
        {
            this._uniqueId = uniqueId;
            this._name = name;
            this._description = description;
            this._hasIcon = hasIcon;
        }

        public IntegrationAdapterAttribute(string uniqueId, string name,string description,string version,bool hasIcon)
        {
            this._uniqueId = uniqueId;
            this._name = name;
            this._description = description;
            this._version = new Version(version);
            this._hasIcon = hasIcon;
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

        public bool HasIcon
        {
            get
            {
                return _hasIcon;
            }
        }
    }
}
