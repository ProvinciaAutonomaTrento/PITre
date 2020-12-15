using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace NttDataWA.AcrobatIntegration
{
    /// <summary>
    /// UserControl di utility per l'utilizzo delle librerie
    /// per l'integrazione della conversione in pdf con l'sdk di adobe acrobat.
    /// Il controllo fornisce un livello di astrazione verso la versione
    /// di acrobat utilizzata.
    /// Espone un'insieme di servizi javascript necessari per:
    /// - acquisire un documento da scanner mediante l'sdk di acrobat
    /// - convertire in pdf un documento (con la possibilità di fare ocr) mediante l'sdk di acrobat
    /// 
    /// Sono state implementate le librerie che si interfacciano con l'sdk di acrobat
    /// per le versioni 5, 6, 7.
    /// </summary>
    public partial class ClientController : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        #region Configurazioni integrazione acrobat

        /// <summary>
        /// Verifica se effettuare l'acquisizione da scanner
        /// usando direttamente l'integrazione adobe acrobat
        /// </summary>
        protected bool ScanWithAcrobatIntegration
        {
            get
            {
                return NttDataWA.AcrobatIntegration.Configurations.ScanWithAcrobatIntegration;
            }
        }

        /// <summary>
        /// Verifica se è abilitata o meno la funzionalità
        /// di riconoscimento OCR con adobe acrobat
        /// </summary>
        protected bool IsEnabledRecognizeText
        {
            get
            {
                return NttDataWA.AcrobatIntegration.Configurations.IsEnabledRecognizeText;
            }
        }

        /// <summary>
        /// Verifica se la libreria installata per l'integrazione adobe acrobat
        /// è per la versione 6 (compatibile con la 5)
        /// </summary>
        protected bool IsAcrobat6Interop
        {
            get
            {
                return (this.AcrobatIntegrationClassId.ToLower().Equals("acrobat6interop.acrobatservices"));
            }
        }

        /// <summary>
        /// Verifica se è attiva la configurazione
        /// relativa all'integrazione adobe acrobat
        /// </summary>
        protected bool IsIntegrationActive
        {
            get
            {
                return NttDataWA.AcrobatIntegration.Configurations.IsIntegrationActive;
            }
        }

        /// <summary>
        /// Id della classe che implementa l'integrazione adobe acrobat
        /// </summary>
        public string AcrobatIntegrationClassId
        {
            get
            {
                return NttDataWA.AcrobatIntegration.Configurations.AcrobatIntegrationClassId;
            }
        }

        #endregion
    }
}