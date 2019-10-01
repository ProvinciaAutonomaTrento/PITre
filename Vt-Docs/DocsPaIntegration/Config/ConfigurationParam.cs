using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocsPaIntegration.ObjectTypes;

namespace DocsPaIntegration.Config
{
    [Serializable]
    public class ConfigurationParam
    {
        public string Name
        {
            get;
            set;
        }

        public string Value
        {
            get;
            set;
        }

        public ObjectType Type
        {
            get;
            set;
        }

        public bool Mandatory
        {
            get;
            set;
        }
    }
}
