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

namespace DocsPAWA.TodoList
{
    /// <summary>
    /// Classe per la gestione della pagina di avviso della funzionalità di svuotamento della TDL
    /// 
    /// La pagina web (modale) viene chiamata tramite il JS [function svuotaTDLPage(noticeDays,oldTxt,tipoObjTrasm,isFunctionEnabled,datePost)]
    /// posto nella pagina DocspaWA > ricercaTrasm > tabRisultatiRicTrasm.aspx.
    /// 
    /// La pagina viene visualizzata in due modi:
    ///     1 -     in automatico, quando si verificano le seguenti condizioni: 
    ///             si visualizzaziono le 'cose da fare' nella homepage di docspa + 
    ///             la funzionalità di avviso è stata impostata in Amm.ne + 
    ///             esistono trasmissioni più vecchie dei giorni impostati dall'Amm.re
    /// 
    ///     2 -     quando si preme il tasto 'Rimuovi' nella homepage di docspa > lista delle 'cose da fare'
    /// 
    /// ----------------------------------------------------------------------------------------------------------------------------------------
    /// Parametri passati in queryString alla pagina web:
    /// ----------------------------------------------------------------------------------------------------------------------------------------
    ///     1 - noticeDays:         Numero dei giorni impostati dall'amm.re affinchè sia visualizzato l'avviso (campo DB)
    ///                             Il campo è Empty se la funzionalità non è stata abilitata tramite il tool di Amm.ne.
    ///     ....................................................................................................................................
    /// 
    ///     2 - oldTxt:             Numero delle trasmissioni che, alla data odierna, sono più vecchie dei giorni impostati dall'Amm.re.
    ///                             Il campo è Empty se la funzionalità non è stata abilitata tramite il tool di Amm.ne.
    ///     ....................................................................................................................................
    /// 
    ///     3 - tipoObjTrasm:       Tipo dell'oggetto trasmesso. Possibili valori: 'D' (documenti) o 'F' (fascicoli)
    ///                             Obbligatorio, sempre impostato.
    ///     ....................................................................................................................................
    /// 
    ///     4 - isFunctionEnabled:  Specifica se la funzionalità è stata abilitata in Amm.ne. Possibili valori: 'Y' (si) o 'N' (no)
    ///                             Obbligatorio, sempre impostato.
    ///     ....................................................................................................................................
    /// 
    ///     5 - datePost:           Indica una data scaturita dalla data odierna meno i giorni impostati in Amm.ne (noticeDays).
    ///                             Il campo è Empty se la funzionalità non è stata abilitata tramite il tool di Amm.ne.
    /// 
    /// 
    /// ----------------------------------------------------------------------------------------------------------------------------------------
    /// Campi impostati o nascosti
    /// ----------------------------------------------------------------------------------------------------------------------------------------
    ///     1 - hd_noticeDays:      HIDDEN - Valorizzato con il campo passato 'noticeDays'.
    ///                             Il campo è Empty se la funzionalità non è stata abilitata tramite il tool di Amm.ne.
    ///     ....................................................................................................................................
    /// 
    ///     2 - hd_dataSistema:     HIDDEN - Valorizzato con il campo passato 'datePost'.
    ///                             Il campo è Empty se la funzionalità non è stata abilitata tramite il tool di Amm.ne.
    ///     ....................................................................................................................................
    /// 
    ///     3 - hd_functionEnabled: HIDDEN - Valorizzato con il campo passato 'isFunctionEnabled'.
    ///                             Il campo è sempre impostato.
    ///     ....................................................................................................................................
    /// 
    ///     4 - hd_tipoObjTrasm:    HIDDEN - Valorizzato con il campo passato 'isFunctionEnabled'.
    ///                             Il campo è sempre impostato.
    ///     ....................................................................................................................................    
    /// 
    /// </summary>
    public partial class svuotaTDLPage : System.Web.UI.Page
    {
        protected IFormatProvider format = new System.Globalization.CultureInfo("it-IT", true);
        protected DateTime xdatePost;

        /// <summary>
        /// Page Load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //parametri
                string isFunctionEnabled = Request.QueryString["isFunctionEnabled"];
                string datePost = Request.QueryString["datePost"];

                if (isFunctionEnabled.Equals("Y"))
                {
                    //dati a video
                    this.pnl_functionEnabled.Visible = true;
                    string gg = Request.QueryString["noticeDays"];
                    this.lbl_noticeDays.Text = gg;
                    this.lbl_oldTxt.Text = Request.QueryString["oldTxt"];

                    xdatePost = Convert.ToDateTime(datePost, format);

                    //campi nascosti                    
                    this.hd_noticeDays.Value = gg;
                    this.hd_dataSistema.Value = this.calendar.txt_Data.Text;
                }
                else
                {
                    this.pnl_functionEnabled.Visible = false;
                    xdatePost = Convert.ToDateTime(System.DateTime.Now, format);
                }

                //campo input utente
                this.calendar.txt_Data.Text = xdatePost.ToShortDateString();

                //campi nascosti 
                this.hd_functionEnabled.Value = isFunctionEnabled;
                this.hd_tipoObjTrasm.Value = Request.QueryString["tipoObjTrasm"];

                //pulsanti
                this.btn_annulla.Attributes.Add("onclick", "window.returnValue = 'N'; window.close();");
            }
        }

        /// <summary>
        /// Tasto elimina
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_elimina_Click(object sender, EventArgs e)
        {
            string dataImpostata = string.Empty;

            DocsPAWA.DocsPaWR.Ruolo ruoloUtente = UserManager.getRuolo(this);
            DocsPAWA.DocsPaWR.Utente infoUtente = UserManager.getUtente(this);

            DocsPaWR.EsitoOperazione esito = new DocsPAWA.DocsPaWR.EsitoOperazione();

            try
            {
                if (!this.calendar.txt_Data.Text.Equals(string.Empty))
                {
                    if (!Utils.isDate(this.calendar.txt_Data.Text))
                    {
                        openAlertJS("inserire una data valida");
                        return;
                    }
                    else
                        dataImpostata = this.calendar.txt_Data.Text;
                }
                else
                {
                    if (this.hd_functionEnabled.Equals("Y"))
                        dataImpostata = this.hd_dataSistema.Value;
                    else
                    {
                        openAlertJS("inserire una data valida");
                        return;
                    }
                }

                xdatePost = Convert.ToDateTime(dataImpostata, format);
                dataImpostata = xdatePost.ToShortDateString();

                TodoList.TodoListManager manager = new TodoListManager(ruoloUtente, infoUtente, this.hd_tipoObjTrasm.Value, this.chk_noWF.Checked);
                DocsPAWA.DocsPaWR.FiltroRicerca[] filter = DocumentManager.getFiltroRicTrasm(this);
                esito = manager.svuotaTDL(dataImpostata, filter);
                if (esito.Codice > 0)
                {
                    openAlertJS(esito.Descrizione);
                    Session.Remove("data");
                }
                else
                    ClientScript.RegisterStartupScript(this.GetType(), "endJS", "<script>window.returnValue = 'Y'; window.close();</script>");
            }
            catch
            {
                openAlertJS("si è verificato un errore nel sistema");
            }
        }

        private void openAlertJS(string msg)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "alertJS", "<script>alert('Attenzione,\\n" + msg + "');</script>");
        }
    }
}
