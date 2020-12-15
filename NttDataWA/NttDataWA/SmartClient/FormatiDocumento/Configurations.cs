using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using NttDataWA.DocsPaWR;

namespace SmartClient.FormatiDocumento
{
    /// <summary>
    /// Classe per la gestione delle configurazioni dei formati documento
    /// </summary>
    public sealed class Configurations
    {
        private const string SUPPORTED_FILE_TYPES_ENABLED_SESSION_KEY = "Configurations.SupportedFileTypesEnabled";

        private Configurations() { }

        /// <summary>
        /// Verifica se risulta abilitata la gestione dei tipi file supportati
        /// </summary>
        public static bool SupportedFileTypesEnabled
        {
            get
            {
                if (HttpContext.Current.Session[SUPPORTED_FILE_TYPES_ENABLED_SESSION_KEY] == null)
                {
                    DocsPaWebService ws = new DocsPaWebService();
                    HttpContext.Current.Session.Add(SUPPORTED_FILE_TYPES_ENABLED_SESSION_KEY, ws.IsEnabledSupportedFileTypes());
                }

                return (bool)HttpContext.Current.Session[SUPPORTED_FILE_TYPES_ENABLED_SESSION_KEY];
            }
        }

        /// <summary>
        /// Restituisce la dimensione massima in kb dei file acquisibili.
        /// Valido solo per la gestione precedente a quella dei formati supportati
        /// e solamente se quest'ultima è non abilitata.
        /// La gestione precedente prevedeva un controllo solo a livello di frontend
        /// sulla dimensione massima dei file e indipendentemente dall'estensione.
        /// </summary>
        public static int FileAcquireSizeMax
        {
            get
            {
                int maxFileSize = 0;
                
                if (!SupportedFileTypesEnabled)
                    Int32.TryParse(ConfigurationManager.AppSettings[NttDataWA.Utils.WebConfigKeys.FILE_ACQ_SIZE_MAX.ToString()], out maxFileSize);
                
                if (maxFileSize == 0)
                    maxFileSize = Int32.MaxValue;

                return maxFileSize;
            }
        }
    }
}
