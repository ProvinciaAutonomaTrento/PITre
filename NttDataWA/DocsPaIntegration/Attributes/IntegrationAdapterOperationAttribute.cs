using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaIntegration.Attributes
{
    [System.AttributeUsage(System.AttributeTargets.Method, AllowMultiple = false)]
    public class IntegrationAdapterOperationAttribute : Attribute
    {
        private string _name;

        public IntegrationAdapterOperationAttribute(string name){
            this._name=name;
        }

        public string Name
        {
            get
            {
                return _name;
            }
        }
    }
}
