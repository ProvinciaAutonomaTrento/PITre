using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ConservazioneWA.SmartClient
{
    public partial class VerificaSupporto : System.Web.UI.UserControl
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        [System.ComponentModel.Browsable(true)]
        public string IdPeople
        {
            get
            {
                WSConservazioneLocale.InfoUtente infoUtente = (WSConservazioneLocale.InfoUtente)Session["infoutCons"];

                return infoUtente.idPeople;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [System.ComponentModel.Browsable(true)]
        public string ServiceUrl
        {
            get
            {
                return ConservazioneWA.Properties.Settings.Default.VerificaIntegritaServices;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool Success
        {
            get
            {
                bool retValue;
                Boolean.TryParse(this.hdSuccess.Value, out retValue);
                return retValue;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string ErrorMessage
        {
            get
            {
                return this.hdErrorMessage.Value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected string HdSuccessClientId
        {
            get
            {
                return this.hdSuccess.ClientID;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected string HdErrorMessageClientId
        {
            get
            {
                return this.hdErrorMessage.ClientID;
            }
        }

    }
}