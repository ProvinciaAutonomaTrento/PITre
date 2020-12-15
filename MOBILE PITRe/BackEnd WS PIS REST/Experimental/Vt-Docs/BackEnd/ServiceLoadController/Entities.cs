using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServiceLoadController
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable()]
    internal class ServiceLoadInfo
    {
        public ServiceLoadInfo()
        {
            this.Methods = new MethodLoadInfo[0];
        }

        /// <summary>
        /// 
        /// </summary>
        public string ServiceName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int ServiceCallInterval { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public long LastServiceExecution { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public MethodLoadInfo[] Methods { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable()]
    internal class MethodLoadInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public string MethodName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int MethodCallInterval { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public long LastMethodExecution { get; set; }
    }
}
