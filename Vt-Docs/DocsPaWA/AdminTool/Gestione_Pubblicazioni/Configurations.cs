using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocsPAWA.AdminTool.Gestione_Pubblicazioni
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class Configurations
    {
        /// <summary>
        /// 
        /// </summary>
        private Configurations()
        { }

        /// <summary>
        /// Indica se le pubblicazioni sono abilitate o meno per l'istanza
        /// </summary>
        public static bool PublisherEnabled
        {
            get
            {
                const string KEY = "BE_PUBBLICAZIONI";

                string value = DocsPAWA.utils.InitConfigurationKeys.GetValue("0", KEY);

                return (value == "1");
            }
        }
    }
}