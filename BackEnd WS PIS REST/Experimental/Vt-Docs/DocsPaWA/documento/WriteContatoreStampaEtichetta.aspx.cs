using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DocsPAWA.documento
{
    /// <summary>
    /// 
    /// </summary>
    public partial class WriteContatoreStampaEtichetta : System.Web.UI.Page
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            this.WriteContatoreStampeEffettuate();
        }

        /// <summary>
        /// 
        /// </summary>
        protected void WriteContatoreStampeEffettuate() //se la stampa multipla è abilitata allora gestisco l'aggiornamento del contatore prints_num
        {
            string abilita_multi_stampa_etichetta = utils.InitConfigurationKeys.GetValue("0", "FE_MULTI_STAMPA_ETICHETTA");
            if (abilita_multi_stampa_etichetta.Equals("1"))
            {
                DocsPaWR.SchedaDocumento schedaDocumento = DocumentManager.getDocumentoSelezionato();
                DocsPaWR.DocsPaWebService ws = ProxyManager.getWS();
                try
                {
                    ws.updateStampeDocumentoEffettuate(UserManager.getInfoUtente(),
                            this.NumeroStampeEffettuate,
                            this.NumeroStampeDaEffettuare,
                            schedaDocumento.systemId);
                }
                catch (Exception e) { }
                string numStampe = (this.NumeroStampeEffettuate + this.NumeroStampeDaEffettuare).ToString();
                schedaDocumento.protocollo.stampeEffettuate = numStampe;
            }
        }

        /// <summary>
        /// Il numero di stampe del documento effettuate finora, prima di questa chiamata
        /// </summary>
        protected int NumeroStampeEffettuate
        {
            get
            {
                int retValue;
                Int32.TryParse(this.Request.QueryString["numeroStampeEffettuate"], out retValue);
                return retValue;
            }
        }

        /// <summary>
        /// Il numero di stampe del documento da effettuare
        /// </summary>
        protected int NumeroStampeDaEffettuare
        {
            get
            {
                int retValue;
                Int32.TryParse(this.Request.QueryString["numeroStampeDaEffettuare"], out retValue);
                return retValue;
            }
        }
    }
}