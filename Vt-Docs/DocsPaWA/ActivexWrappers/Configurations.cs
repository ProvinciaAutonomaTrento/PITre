using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace DocsPAWA.ActivexWrappers
{
    /// <summary>
    /// Classe per la verifica delle configurazioni relative
    /// all'utilizzo del componente "DocsPa_ActivexWrappers.dll" 
    /// che incapsula alcuni servizi utilizzati da docspa lato client
    /// </summary>
    public sealed class Configurations
    {
        private Configurations()
        { }

        /// <summary>
        /// Se true, viene utilizzato il componente "DocsPa_ActivexWrappers"
        /// </summary>
        public static bool UseActivexWrappersControl
        {
            get
            {
                bool result = false;

                if (ConfigurationManager.AppSettings["USE_SECURE_ACTIVEX"] != null)
                    bool.TryParse(ConfigurationManager.AppSettings["USE_SECURE_ACTIVEX"], out result);

                return result;
            }
        }

        /// <summary>
        /// Se true, viene utilizzato il componente "AxSpell" per il controllo ortografico
        /// </summary>
        public static bool UseSpellWrapperControl
        {
            get
            {
                bool result = false;

                if (ConfigurationManager.AppSettings["USE_SPELL_ACTIVEX"] != null)
                    bool.TryParse(ConfigurationManager.AppSettings["USE_SPELL_ACTIVEX"], out result);

                return result;
            }
        }

        /// <summary>
        /// Se true, viene attivato (quindi scaricato) per l'utilizzo il componente per 
        /// l'elaborazione dei modelli clientside
        /// </summary>
        public static bool UserClientModelProcessorControl
        {
            get
            {
                bool result = false;

                if (ConfigurationManager.AppSettings["USE_CLIENT_MODEL_PROCESSOR_ACTIVEX"] != null)
                    bool.TryParse(ConfigurationManager.AppSettings["USE_CLIENT_MODEL_PROCESSOR_ACTIVEX"], out result);

                return result;
            }
        }

        /// <summary>
        /// Se true, viene attivato (quindi scaricato) per l'utilizzo il componente per 
        /// il salvataggio di fascicoli in locale
        /// </summary>
        public static bool ProjectToFSControl
        { 
            get
            {
                bool result = false;

                if (ConfigurationManager.AppSettings["USE_PROJECT_TO_FS"] != null)
                    bool.TryParse(ConfigurationManager.AppSettings["USE_PROJECT_TO_FS"], out result);

                return result;
            }
        }

        public static bool CacheControl
        { 
            get
            {
                bool result = false;
                if (ConfigurationManager.AppSettings["USE_CACHE"] != null)
                    bool.TryParse(ConfigurationManager.AppSettings["USE_CACHE"], out result);


                return result;
            }
        }
    }
}
