using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace DocsPAWA.AcrobatIntegration
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class Configurations
    {
        /// <summary>
        /// Classe di default per l'integrazione, compatibile con la versione 7 di acrobat
        /// </summary>
        private const string DEFAULT_ACROBAT_INTEGRATION_CLASS_ID = "AcrobatInterop.AcrobatServices";

        private Configurations()
        {
        }

        /// <summary>
        /// Verifica se effettuare l'acquisizione da scanner
        /// usando direttamente l'integrazione adobe acrobat
        /// </summary>
        public static bool ScanWithAcrobatIntegration
        {
            get
            {
                bool retValue = false;
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["SCAN_WITH_ADOBE_ACROBAT_INTEGRATION"]))
                    bool.TryParse(ConfigurationManager.AppSettings["SCAN_WITH_ADOBE_ACROBAT_INTEGRATION"], out retValue);
                return retValue;
            }
        }

        /// <summary>
        /// Verifica se è abilitata o meno la funzionalità
        /// di riconoscimento OCR con adobe acrobat
        /// </summary>
        public static bool IsEnabledRecognizeText
        {
            get
            {
                // Per default è abilitata la funzionalità ocr
                bool retValue = true;

                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["ADOBE_ACROBAT_INTEGRATION_ENABLE_RECOGNIZE_TEXT"]))
                    bool.TryParse(ConfigurationManager.AppSettings["ADOBE_ACROBAT_INTEGRATION_ENABLE_RECOGNIZE_TEXT"], out retValue);
                return retValue;
            }
        }

        /// <summary>
        /// Verifica se è attiva la configurazione
        /// relativa all'integrazione adobe acrobat
        /// </summary>
        public static bool IsIntegrationActive
        {
            get
            {
                bool isActive = false;
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["ADOBE_ACROBAT_INTEGRATION"]))
                    bool.TryParse(ConfigurationManager.AppSettings["ADOBE_ACROBAT_INTEGRATION"], out isActive);
                return isActive;
            }
        }

        /// <summary>
        /// Id della classe che implementa l'integrazione adobe acrobat
        /// </summary>
        public static string AcrobatIntegrationClassId
        {
            get
            {
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["ADOBE_ACROBAT_INTEGRATION_CLASS_ID"]))
                    return ConfigurationManager.AppSettings["ADOBE_ACROBAT_INTEGRATION_CLASS_ID"];
                else
                    return DEFAULT_ACROBAT_INTEGRATION_CLASS_ID;
            }
        }
    }
}
