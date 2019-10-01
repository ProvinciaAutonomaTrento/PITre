using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.SmartClient
{
    /// <summary>
    /// Configurazioni smartclient
    /// </summary>
    /// 
    public enum ComponentType
    {
        Default = 0,
        AtiveX = 1,
        SmartClient = 2,
        JavaApplet = 3,
        HTML5WebSocket = 4
    }

    [Serializable()]
    public class SmartClientConfigurations
    {
        /// <summary>
        /// Se true, l'utente è abilitato all'utilizzo dei componenti smartclient
        /// </summary>
        public bool IsEnabled
        {
            get { return (this.ComponentsType == "2"); }
            set { this.ComponentsType = "2"; }
        }

        /// <summary>
        /// Il tipo (AtiveX, SmartClient, JavaApplet) dei componenti.
        /// Può essere ripettivamente "0", "1" o "2"
        /// </summary>
        public string ComponentsType
        {
            get;
            set;
        }

        /// <summary>
        /// Indica il tipo di browser in uso
        /// </summary>
        public string BrowserType
        {
            get;
            set;
        }

        /// <summary>
        /// Se true, viene applicata la conversione pdf al documento scannerizzato
        /// </summary>
        public bool ApplyPdfConvertionOnScan
        {
            get;
            set;
        }
    }
}
