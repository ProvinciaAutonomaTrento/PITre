using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace ServiceLoadController
{
    /// <summary>
    /// 
    /// </summary>
    internal sealed class Configurations
    {
        ///// <summary>
        ///// 
        ///// </summary>
        //public static bool RunLocal
        //{
        //    get
        //    {
        //        return !string.IsNullOrEmpty(XmlFilePath);
        //    }
        //}

        /// <summary>
        /// 
        /// </summary>
        public static string XmlFilePath
        {
            get
            {
                return DocsPaUtils.InitQuery.getKey("QueryFilePath") + "/ServiceLoadController.xml";
                    //ConfigurationManager.AppSettings["SERVICE_LOAD_CONTROLLER_FILE_PATH"];
            }
        }

        ///// <summary>
        ///// 
        ///// </summary>
        //public static string ServiceLoadUrl
        //{
        //    get
        //    {
        //        return ConfigurationManager.AppSettings["SERVICE_LOAD_CONTROLLER_URL"];
        //    }
        //}
    }
}
