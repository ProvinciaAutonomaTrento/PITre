using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using DocsPaIntegration.Attributes;

namespace DocsPaIntegration
{
    [Serializable]
    public class AssemblyLoader
    {
        public bool Load(string path)
        {
            Assembly _assembly = Assembly.LoadFile(path);
            IntegrationAdapterAssemblyAttribute[] attrs = (IntegrationAdapterAssemblyAttribute[])_assembly.GetCustomAttributes(typeof(IntegrationAdapterAssemblyAttribute), false);
            if (attrs.Length > 0) return true;
            return false;
        }

    }
}
