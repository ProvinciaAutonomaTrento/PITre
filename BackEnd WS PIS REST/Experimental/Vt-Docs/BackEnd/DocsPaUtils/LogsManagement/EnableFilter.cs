using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net.Filter;
using log4net.Core;
using System.Reflection;

namespace DocsPaUtils.LogsManagement
{
    public class EnableFilter : FilterSkeleton
    {
        protected string m_environmentHandler;
        protected ILogEnvironmentHandler _handlerInstance;

        public string EnvironmentHandler
        {
            get { return m_environmentHandler; }
            set { m_environmentHandler = value; }
        }


        override public void ActivateOptions()
        {
            if (m_environmentHandler == null)
            {
                throw new ArgumentNullException("environmentHandler");
            }
            string[] split = m_environmentHandler.Split(new char[] { ',' });
            if (split.Length < 2) throw new Exception("Value has to be in format");

            Assembly ass = GetAssembly(split[1].Trim());
            if (ass == null)
            {
                throw new Exception("Assembly not found");
            }
            Type type = ass.GetType(split[0]);
            if (type == null) throw new Exception("Type not found");
            _handlerInstance = (ILogEnvironmentHandler) Activator.CreateInstance(type);
        }

        private Assembly GetAssembly(string assemblyName)
        {
            Assembly[] assemblyList = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly temp in assemblyList)
            {
                AssemblyName tempName = temp.GetName();
                if (tempName.Name.Equals(assemblyName)) return temp;
            }
            return null;
        }

        override public FilterDecision Decide(LoggingEvent loggingEvent)
        {
            if (_handlerInstance.Enabled)
            {
                return FilterDecision.Accept;
            }
            else
            {
                return FilterDecision.Deny;
            }
        }
    }
}
