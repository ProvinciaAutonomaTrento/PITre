using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace DocsPaIntegration.ObjectTypes.Attributes
{
    [System.AttributeUsage(System.AttributeTargets.Field | System.AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public abstract class IntegrationObjectTypeAttribute : Attribute
    {
        private string _name;
        private bool _mandatory;

        public IntegrationObjectTypeAttribute(string name, bool mandatory)
        {
            this._name = name;
            this._mandatory = mandatory;
        }

        public string Name
        {
            get
            {
                return _name;
            }
        }

        public abstract ObjectType Type
        {
            get;
        }

        public bool Mandatory
        {
            get
            {
                return _mandatory;
            }
        }

        public abstract object GetValue(string value);
    }
}
