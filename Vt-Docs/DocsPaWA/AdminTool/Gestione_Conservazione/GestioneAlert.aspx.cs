using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DocsPAWA.AdminTool.Gestione_Conservazione
{
    public partial class GestioneAlert : System.Web.UI.Page
    {

        private DocsPaWR.DocsPaWebService _wsInstance = new DocsPAWA.DocsPaWR.DocsPaWebService();
        
        protected bool logLeggibilitaAttivo;
        protected bool logDownloadAttivo;
        protected bool logSfogliaAttivo;

        protected string leggibilitaScadenza;
        protected string leggibilitaNotifiche;

        protected string idAmm;

        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                Session["idAmm"] = this.IdAmministrazione.ToString();
                this.idAmm = Session["idAmm"].ToString();

                //chiavi di configurazione
                Session["leggibilitaScadenza"] = DocsPAWA.utils.InitConfigurationKeys.GetValue(this.idAmm, "BE_CONS_VER_LEG_INTERVALLO");
                Session["leggibilitaNotifiche"] = DocsPAWA.utils.InitConfigurationKeys.GetValue(this.idAmm, "BE_CONS_VER_LEG_GG_NOTIFICHE");

                //carico la configurazione da db
                this.GetData();

                //controllo se i log sono attivi
                Session["logLeggibilitaAttivo"] = _wsInstance.IsLogAttivatoAlert(this.idAmm, "LEGGIBILITA");
                Session["logDownloadAttivo"] = _wsInstance.IsLogAttivatoAlert(this.idAmm, "DOWNLOAD_ISTANZA");
                Session["logSfogliaAttivo"] = _wsInstance.IsLogAttivatoAlert(this.idAmm, "SFOGLIA_ISTANZA");

                this.GestioneCheckBox();
            }

            this.idAmm = Session["idAmm"].ToString();

            

        }

        protected void CheckedChanged(object sender, EventArgs e)
        {
            this.GestioneCheckBox();
        }

        protected void GetData()
        {
            DocsPaWR.AlertConservazione alertData = _wsInstance.GetAlertValues(this.idAmm);
            Session["config_alert"] = alertData;

            //i parametri dell'alert sull'esecuzione anticipata delle verifiche di leggibilità
            //sono gestiti attraverso chiavi di configurazione
            //non possono essere modificati in questa pagina
            //alertData.scadenzaTermine = DocsPAWA.utils.InitConfigurationKeys.GetValue(this.idAmm, "BE_CONS_VER_LEG_INTERVALLO");
            //alertData.scadenzaTolleranza = DocsPAWA.utils.InitConfigurationKeys.GetValue(this.idAmm, "BE_CONS_VER_LEG_GG_NOTIFICHE");

            //this.txtVerAntScadenza.Text = alertData.scadenzaTermine;
            //this.txtVerAntTolleranza.Text = alertData.scadenzaTolleranza;

            this.txtVerAntScadenza.Text = Session["leggibilitaScadenza"].ToString();
            this.txtVerAntTolleranza.Text = Session["leggibilitaNotifiche"].ToString();

            if (alertData != null)
            {
                alertData.scadenzaTermine = this.txtVerAntScadenza.Text;
                alertData.scadenzaTolleranza = this.txtVerAntTolleranza.Text;

                //checkbox alert
                this.chkVerificaAnticipata.Checked = (alertData.chaLeggibilitaScadenza == "1");
                this.chkVerificaDimensioni.Checked = (alertData.chaLeggibilitaMaxDoc == "1");
                this.chkVerLegDocumento.Checked = (alertData.chaSingoloDoc == "1");
                this.chkDownloadIstanza.Checked = (alertData.chaDownload == "1");
                this.chkSfogliaIstanza.Checked = (alertData.chaSfoglia == "1");

                //parametri alert
                if (this.chkVerificaAnticipata.Checked)
                {   
                    this.txtVerAntScadenza.Text = alertData.scadenzaTermine;
                    this.txtVerAntTolleranza.Text = alertData.scadenzaTolleranza;
                }
                if (this.chkVerificaDimensioni.Checked)
                {
                    this.txtVerDimPercentuale.Text = alertData.percentualeMaxDoc;
                }
                if (this.chkVerLegDocumento.Checked)
                {
                    this.txtVerLegDocumento_max.Text = alertData.maxOperSingoloDoc;
                    this.txtVerLegDocumento_per.Text = alertData.periodoSingoloDoc;
                }
                if (this.chkDownloadIstanza.Checked)
                {
                    this.txtDownloadIstanza_max.Text = alertData.maxOperDownload;
                    this.txtDownloadIstanza_per.Text = alertData.periodoDownload;
                }
                if (this.chkSfogliaIstanza.Checked)
                {
                    this.txtSfogliaIstanza_max.Text = alertData.maxOperSfoglia;
                    this.txtSfogliaIstanza_per.Text = alertData.periodoSfoglia;
                }


                //parametri server smtp
                this.txtMailServer.Text = alertData.serverSMTP;
                this.txtMailPorta.Text = alertData.portaSMTP;
                this.chkMailSSL.Checked = (alertData.chaSSL == "1");
                this.txtMailUserID.Text = alertData.userID;
                this.txtMailPwd.Text = alertData.pwd;
                this.txtMailFrom.Text = alertData.fromField;
                this.txtMailTo.Text = alertData.toField;

            }
        }
        
        protected void GestioneCheckBox()
        {
            bool.TryParse(Session["logLeggibilitaAttivo"].ToString(), out logLeggibilitaAttivo);
            bool.TryParse(Session["logDownloadAttivo"].ToString(), out logDownloadAttivo);
            bool.TryParse(Session["logSfogliaAttivo"].ToString(), out logSfogliaAttivo);

            DocsPaWR.AlertConservazione alert = (DocsPaWR.AlertConservazione)Session["config_alert"];

            #region checkbox attivi/disattivi

            if (!logLeggibilitaAttivo)
            {
                this.chkVerificaAnticipata.Enabled = false;
                this.chkVerificaDimensioni.Enabled = false;
                this.chkVerLegDocumento.Enabled = false;                
            }
            else
            {
                this.chkVerificaAnticipata.Enabled = true;
                this.chkVerificaDimensioni.Enabled = true;
                this.chkVerLegDocumento.Enabled = true;
            }

            string leggibilitaScadenza = Session["leggibilitaScadenza"].ToString();
            string leggibilitaNotifiche = Session["leggibilitaNotifiche"].ToString();

            if ((string.IsNullOrEmpty(leggibilitaScadenza)) || (string.IsNullOrEmpty(leggibilitaNotifiche)))
            {
                this.chkVerificaAnticipata.Enabled = false;
                this.chkVerificaAnticipata.ToolTip = "Per attivare questo alert è necessario configurare le chiavi 'BE_CONS_VER_LEG_INTERVALLO' e 'BE_CONS_VER_LEG_GG_NOTIFICHE'.";
            }

            if (!logDownloadAttivo)
                this.chkDownloadIstanza.Enabled = false;
            else
                this.chkDownloadIstanza.Enabled = true;

            if (!logSfogliaAttivo)
                this.chkSfogliaIstanza.Enabled = false;
            else
                this.chkSfogliaIstanza.Enabled = true;


            #endregion



            if (chkVerificaAnticipata.Enabled)
            {

                if (!chkVerificaAnticipata.Checked)
                {
                    this.txtVerAntScadenza.Visible = false;
                    this.txtVerAntTolleranza.Visible = false;
                    this.lblVerAntScadenza.Visible = false;
                    this.lblVerAntTolleranza.Visible = false;
                    this.chkVerificaAnticipata.ToolTip = "Attiva l'alert";
                }
                else
                {
                    this.txtVerAntScadenza.Visible = true;
                    this.txtVerAntTolleranza.Visible = true;
                    this.lblVerAntScadenza.Visible = true;
                    this.lblVerAntTolleranza.Visible = true;
                    this.chkVerificaAnticipata.ToolTip = "Disattiva l'alert";
                }
            }
            if (!chkVerificaDimensioni.Checked)
            {
                this.txtVerDimPercentuale.Visible = false;
                this.lblVerDimPercentuale.Visible = false;
                this.chkVerificaDimensioni.ToolTip = "Attiva l'alert";
            }
            else
            {
                this.txtVerDimPercentuale.Visible = true;
                this.lblVerDimPercentuale.Visible = true;
                this.chkVerificaDimensioni.ToolTip = "Disattiva l'alert";
            }

            if (!chkVerLegDocumento.Checked)
            {
                this.txtVerLegDocumento_max.Visible = false;
                this.txtVerLegDocumento_per.Visible = false;
                this.lblVerLegDocumento_max.Visible = false;
                this.lblVerLegDocumento_per.Visible = false;
                this.chkVerLegDocumento.ToolTip = "Attiva l'alert";
            }
            else
            {
                this.txtVerLegDocumento_max.Visible = true;
                this.txtVerLegDocumento_per.Visible = true;
                this.lblVerLegDocumento_max.Visible = true;
                this.lblVerLegDocumento_per.Visible = true;
                this.chkVerLegDocumento.ToolTip = "Disattiva l'alert";
            }

            if (!chkDownloadIstanza.Checked)
            {
                this.txtDownloadIstanza_max.Visible = false;
                this.txtDownloadIstanza_per.Visible = false;
                this.lblDownloadIstanza_max.Visible = false;
                this.lblDownloadIstanza_per.Visible = false;
                this.chkDownloadIstanza.ToolTip = "Attiva l'alert";
            }
            else
            {
                this.txtDownloadIstanza_max.Visible = true;
                this.txtDownloadIstanza_per.Visible = true;
                this.lblDownloadIstanza_max.Visible = true;
                this.lblDownloadIstanza_per.Visible = true;
                this.chkDownloadIstanza.ToolTip = "Disattiva l'alert";
            }

            if (!chkSfogliaIstanza.Checked)
            {
                this.txtSfogliaIstanza_max.Visible = false;
                this.txtSfogliaIstanza_per.Visible = false;
                this.lblSfogliaIstanza_max.Visible = false;
                this.lblSfogliaIstanza_per.Visible = false;
                this.chkSfogliaIstanza.ToolTip = "Attiva l'alert";
            }
            else
            {
                this.txtSfogliaIstanza_max.Visible = true;
                this.txtSfogliaIstanza_per.Visible = true;
                this.lblSfogliaIstanza_max.Visible = true;
                this.lblSfogliaIstanza_per.Visible = true;
                this.chkSfogliaIstanza.ToolTip = "Disattiva l'alert";
            }

        }

        protected void btn_salva_alert_Click(object sender, EventArgs e)
        {

            #region controllo campi obbligatori e validazione

            //se è attivo almeno un alert devono essere configurati i parametri della casella
            if (this.chkVerificaAnticipata.Checked || this.chkVerificaDimensioni.Checked || this.chkVerLegDocumento.Checked || this.chkDownloadIstanza.Checked || this.chkSfogliaIstanza.Checked)
            {

                //server SMTP
                if (string.IsNullOrEmpty(this.txtMailServer.Text.Trim()))
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "noServerField", "alert('Il campo Server SMTP è obbligatorio.');", true);
                    return;
                }
                //porta SMTP
                if (string.IsNullOrEmpty(this.txtMailPorta.Text.Trim()))
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "noPortaField", "alert('Il campo Porta è obbligatorio.');", true);
                    return;
                }
                // MODIFICA - i campi user e password non devono essere obbligatori
                // è richiesto per caselle come relay.infotn.it
                ////userid
                //if (string.IsNullOrEmpty(this.txtMailUserID.Text.Trim()))
                //{
                //    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "noUserField", "alert('Il campo User ID è obbligatorio.');", true);
                //    return;
                //}
                ////password
                //if (string.IsNullOrEmpty(this.txtMailPwd.Text.Trim()))
                //{
                //    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "noPassField", "alert('Il campo Password è obbligatorio.');", true);
                //    return;
                //}
                //mail from
                if (string.IsNullOrEmpty(this.txtMailFrom.Text.Trim()))
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "noFromField", "alert('Il campo Mittente è obbligatorio.');", true);
                    return;
                }
                //mail to
                if (string.IsNullOrEmpty(this.txtMailTo.Text.Trim()))
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "noToField", "alert('Il campo Destinatario è obbligatorio');", true);
                    return;
                }
            }

            //alert esecuzione anticipata verifica leggibilità
            if (chkVerificaAnticipata.Checked)
            {
                if (!this.ValidateParam(this.txtVerAntScadenza.Text, "Esecuzione anticipata della verifica periodica di leggibilità > Scadenza"))
                    return;
                if (!this.ValidateParam(this.txtVerAntTolleranza.Text, "Esecuzione anticipata della verifica periodica di leggibilità > Tolleranza"))
                    return;
            }
            //alert esecuzione verifica leggibilità su campione di dimensioni maggiori del consentito
            if (chkVerificaDimensioni.Checked)
            {
                if (string.IsNullOrEmpty(this.txtVerDimPercentuale.Text))
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "noVerDim", "alert('Il campo Max Documenti Verificabili è obbligatorio.');", true);
                    return;
                }
                if (!ValidateParam(this.txtVerDimPercentuale.Text, "Max Documenti Verificabili"))
                    return;
            }
            //alert numero esecuzioni verifica leggibilità su singolo documento
            if (chkVerLegDocumento.Checked)
            {
                if (string.IsNullOrEmpty(this.txtVerLegDocumento_max.Text))
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "noVerLeg1", "alert('Il campo Frequenza utilizzo verifica leggibilità su singolo documento > Max Operazioni è obbligatorio.');", true);
                    return;
                }
                if (string.IsNullOrEmpty(this.txtVerLegDocumento_per.Text))
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "noVerLeg2", "alert('Il campo Frequenza utilizzo verifica leggibilità su singolo documento  > Periodo Monitoraggio è obbligatorio.');", true);
                    return;
                }
                if (!this.ValidateParam(this.txtVerLegDocumento_max.Text, "Frequenza utilizzo verifica leggibilità su singolo documento > Max Operazioni"))
                    return;
                if (!this.ValidateParam(this.txtVerLegDocumento_per.Text, "Frequenza utilizzo verifica leggibilità su singolo documento > Periodo Monitoraggio"))
                    return;
            }
            //alert numero esecuzioni download istanza
            if (chkDownloadIstanza.Checked)
            {
                if (string.IsNullOrEmpty(this.txtDownloadIstanza_max.Text))
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "noDownIst1", "alert('Il campo Frequenza utilizzo download istanza > Max Operazioni è obbligatorio.');", true);
                    return;
                }
                if (string.IsNullOrEmpty(this.txtDownloadIstanza_per.Text))
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "noDownIst2", "alert('Il campo Frequenza utilizzo download istanza > Periodo Monitoraggio è obbligatorio.');", true);
                    return;
                }
                if (!this.ValidateParam(this.txtDownloadIstanza_max.Text, "Frequenza utilizzo download istanza > Max Operazioni"))
                    return;
                if (!this.ValidateParam(this.txtDownloadIstanza_per.Text, "Frequenza utilizzo download istanza > Periodo Monitoraggio"))
                    return;
            }
            //alert numero esecuzioni sfoglia istanza
            if (chkSfogliaIstanza.Checked)
            {
                if (string.IsNullOrEmpty(this.txtSfogliaIstanza_max.Text))
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "noSfogliaIst1", "alert('Il campo Frequenza utilizzo sfoglia istanza > Max Operazioni è obbligatorio.');", true);
                    return;
                }
                if (string.IsNullOrEmpty(this.txtSfogliaIstanza_per.Text))
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "noSfogliaIst2", "alert('Il campo Frequenza utilizzo sfoglia istanza > Periodo Monitoraggio è obbligatorio.');", true);
                    return;
                }
                if (!this.ValidateParam(this.txtSfogliaIstanza_max.Text, "Frequenza utilizzo sfoglia istanza > Max Operazioni"))
                    return;
                if (!this.ValidateParam(this.txtSfogliaIstanza_per.Text, "Frequenza utilizzo sfoglia istanza > Periodo Monitoraggio"))
                    return;
            }

            #endregion

            #region validazione campi



            #endregion

            //verifica password
            if (this.VerificaPassword())
            {
                //costruisco l'oggetto coi parametri di configurazione
                DocsPaWR.AlertConservazione configAlert = new DocsPaWR.AlertConservazione();

                configAlert.idAmm = this.idAmm;

                configAlert.chaLeggibilitaScadenza = (this.chkVerificaAnticipata.Checked) ? "1" : "0";
                configAlert.chaLeggibilitaMaxDoc = (this.chkVerificaDimensioni.Checked) ? "1" : "0";
                configAlert.chaSingoloDoc = (this.chkVerLegDocumento.Checked) ? "1" : "0";
                configAlert.chaDownload = (this.chkDownloadIstanza.Checked) ? "1" : "0";
                configAlert.chaSfoglia = (this.chkSfogliaIstanza.Checked) ? "1" : "0";

                if (configAlert.chaLeggibilitaScadenza == "1")
                {
                    configAlert.scadenzaTermine = this.txtVerAntScadenza.Text;
                    configAlert.scadenzaTolleranza = this.txtVerAntTolleranza.Text;
                }
                if (configAlert.chaLeggibilitaMaxDoc == "1")
                {
                    configAlert.percentualeMaxDoc = this.txtVerDimPercentuale.Text;
                }
                if (configAlert.chaSingoloDoc == "1")
                {
                    configAlert.maxOperSingoloDoc = this.txtVerLegDocumento_max.Text;
                    configAlert.periodoSingoloDoc = this.txtVerLegDocumento_per.Text;
                }
                if (configAlert.chaDownload == "1")
                {
                    configAlert.maxOperDownload = this.txtDownloadIstanza_max.Text;
                    configAlert.periodoDownload = this.txtDownloadIstanza_per.Text;
                }
                if (configAlert.chaSfoglia == "1")
                {
                    configAlert.maxOperSfoglia = this.txtSfogliaIstanza_max.Text;
                    configAlert.periodoSfoglia = this.txtSfogliaIstanza_per.Text;
                }

                configAlert.serverSMTP = this.txtMailServer.Text.Trim();
                configAlert.portaSMTP = this.txtMailPorta.Text;
                configAlert.chaSSL = (this.chkMailSSL.Checked) ? "1" : "0";
                configAlert.userID = this.txtMailUserID.Text.Trim();
                configAlert.pwd = this.txtMailPwd.Text.Trim();
                configAlert.fromField = this.txtMailFrom.Text.Trim();
                configAlert.toField = this.txtMailTo.Text.Trim();

                bool esito = _wsInstance.SaveAlertValues(configAlert.idAmm, configAlert);

                if (!esito)
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "saveKO", "alert('Errore nel salvataggio della configurazione.');", true);
                else
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "saveOK", "alert('La configurazione è stata salvata con successo.');", true);
            }
        }

        /// <summary>
        /// Reperimento idamministrazione corrente
        /// </summary>
        protected int IdAmministrazione
        {
            get
            {
                string idAmministrazione = AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "3");

                return Convert.ToInt32(idAmministrazione);
            }
        }

        /// <summary>
        /// Verifica che i campi password e conferma password contengano lo stesso valore
        /// </summary>
        /// <returns></returns>
        protected bool VerificaPassword()
        {
            bool retVal = true;

            if ((!string.IsNullOrEmpty(this.txtMailPwd.Text.Trim())) || (!string.IsNullOrEmpty(this.txtMailPwdConferma.Text.Trim())))
            {
                if (this.txtMailPwd.Text.Trim() != this.txtMailPwdConferma.Text.Trim())
                {
                    retVal = false;
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "verifcaPwd", "alert('I campi password e conferma password non coincidono.');", true);
                }

            }

            return retVal;

        }

        protected bool ValidateParam(string value, string name)
        {
            bool retVal = true;
            string msg = string.Format("Il valore inserito per il parametro {0} non è valido", name);
            int v;

            bool esito = Int32.TryParse(value, out v);
            if (!esito)
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "notValid", string.Format("alert('{0}');", msg), true);
                return false;
            }


            if (name == "Max Documenti Verificabili")
            {
                if (v > 0 && v <= 100)
                    retVal = true;
                else
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "notValid", string.Format("alert('{0}');", msg), true);
                    return false;
                }
            }
            else if (name == "Esecuzione anticipata verifica leggibilità > Tolleranza")
            {
                int range;
                Int32.TryParse(this.txtVerAntScadenza.Text, out range);
                if (v > 0 && v <= range)
                    retVal = true;
                else
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "notValid", string.Format("alert('{0}');", msg), true);
                    return false;
                }
            }
            else
            {

                if (v > 0)
                    retVal = true;
                else
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "notValid", string.Format("alert('{0}');", msg), true);
                    return false;
                }

            }
            return retVal;
        }

    }
}