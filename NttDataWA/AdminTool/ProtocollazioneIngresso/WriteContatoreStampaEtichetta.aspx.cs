using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SAAdminTool.DocsPaWR;
using SAAdminTool;

namespace ProtocollazioneIngresso
{
    public partial class WriteContatoreStampaEtichetta : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.WriteContatoreStampeEffettuate();
        }

        /// <summary>
        /// 
        /// </summary>
        protected void WriteContatoreStampeEffettuate()
        {
            SAAdminTool.DocsPaWR.SchedaDocumento schedaDocumento = DocumentManager.getDocumentoInLavorazione();

            SAAdminTool.DocsPaWR.DocsPaWebService ws = ProxyManager.getWS();
            ws.updateStampeDocumentoEffettuate(UserManager.getInfoUtente(),
                    this.NumeroStampeEffettuate,
                    this.NumeroStampeDaEffettuare,
                    schedaDocumento.systemId);

            string numStampe = (this.NumeroStampeEffettuate + this.NumeroStampeDaEffettuare).ToString();

            schedaDocumento.protocollo.stampeEffettuate = numStampe;
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