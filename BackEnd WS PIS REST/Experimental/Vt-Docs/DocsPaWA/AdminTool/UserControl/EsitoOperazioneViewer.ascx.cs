using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DocsPAWA.AdminTool.UserControl
{
    /// <summary>
    /// User control per la visualizzazione degli elementi "EsitoOperazione" 
    /// risultanti dai servizi dell'amministrazione
    /// </summary>
    public partial class EsitoOperazioneViewer : System.Web.UI.UserControl
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
        /// <param name="result"></param>
        public void Fetch(DocsPaWR.EsitoOperazione[] result)
        {
            this.grdResult.DataSource = result;
            this.grdResult.DataBind();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="esitoOperazione"></param>
        /// <returns></returns>
        protected string GetCodice(DocsPaWR.EsitoOperazione esitoOperazione)
        { 
            if (esitoOperazione.Codice.Equals(CodiceEsitoOK))
                return "OK";
            else
                return string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="esitoOperazione"></param>
        /// <returns></returns>
        protected string GetDescrizione(DocsPaWR.EsitoOperazione esitoOperazione)
        {
            return esitoOperazione.Descrizione;
        }

        /// <summary>
        /// Valore del codice dell'oggetto "EsitoOperazione" che rappresenta lo stato dell'operazione andata a buon fine
        /// </summary>
        [System.ComponentModel.Browsable(true)]
        public int CodiceEsitoOK
        {
            get
            {
                int codiceEsito;
                if (this.ViewState["CodiceEsitoOK"] != null &&
                    Int32.TryParse(this.ViewState["CodiceEsitoOK"].ToString(), out codiceEsito))
                    return codiceEsito;
                else
                    return 0;
            }
            set
            {
                this.ViewState["CodiceEsitoOK"] = value;
            }
        }
    }
}