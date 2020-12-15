using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DocsPAWA.AdminTool.Gestione_Conservazione
{
    public partial class StampaRegistro : System.Web.UI.Page
    {
        private DocsPaWR.DocsPaWebService _wsInstance = new DocsPAWA.DocsPaWR.DocsPaWebService();
        private string _dataStampa = string.Empty;
        private string _oraStampa = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            this.GetDataOraInfo();
            if (!IsPostBack)
            {
                //
                // Imposto il valore dalla checkbox cbAttivazioneStampaReg
                this.setValues();

                // Carico la ddl con l'ora di stampa
                this.caricaDdlOraStampa();

                //
                // Defisco i campi visibili
                if (this.cbAttivazioneStampaReg.Checked)
                {
                    this.ddlFreqStampa.Enabled = true;
                    this.ddlOraStampa.Enabled = true;
                }
                else
                {
                    this.ddlFreqStampa.Enabled = false;
                    this.ddlOraStampa.Enabled = false;
                }
            }
        }

        private void setValues()
        {
            //
            // Recupero i valori della tabella DPA_CONFIG_STAMPA_CONS
            // Valori recuperati: valoreFrequenzaStampa_valoreDisabled
            string stampaRegValues = this.getStampaRegValues();

            this.ddlFreqStampa.SelectedValue = stampaRegValues.Split('_')[0];

            if (!string.IsNullOrEmpty(stampaRegValues))
                this.cbAttivazioneStampaReg.Checked = (stampaRegValues.Split('_')[1].Equals("1") ? false : true);
            else
                this.cbAttivazioneStampaReg.Checked = false;
        }

        private string getStampaRegValues()
        {
            //
            // i valori ritornati se presenti sono nella seguente forma:
            // valoreFrequenzaStampa_valoreDisabled
            string result = string.Empty;

            //
            // Chiamata al BE per prelevare il valore 
            result = _wsInstance.GetStampaRegistroValues(IdAmministrazione.ToString());

            return result;
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
        /// checkbox di Attivazione Stampa Registri
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void cbAttivazioneStampaReg_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cbAttivazioneStampaReg = (CheckBox)sender;

            if (cbAttivazioneStampaReg.Enabled)
            {
                if (cbAttivazioneStampaReg.Checked)
                {
                    this.ddlFreqStampa.Enabled = true;
                    this.ddlOraStampa.Enabled = true;
                }
                else
                {
                    this.ddlFreqStampa.Enabled = false;
                    this.ddlOraStampa.Enabled = false;
                }
            }
        }

        /// <summary>
        /// Bottone salva del pannello Stampa Registri
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSalvaStampaRegistri_Click(object sender, EventArgs e)
        {
            #region input parameters declaration
            //
            // Parametri da passare al metodo che salva i valori nella tabella DPA_CONFIG_STAMPA_CONS
            string freqStampaValue = string.Empty;
            string oraStampaValue = string.Empty;
            string disabled = string.Empty;
            string idAmm = string.Empty;
            #endregion

            //
            // esito dell'operazione di salvataggio
            bool result = false;

            //
            // Recupero i valori per il salvataggio della configurazione

            disabled = (!this.cbAttivazioneStampaReg.Checked?"1":"0");
            freqStampaValue = this.ddlFreqStampa.SelectedValue;
            oraStampaValue = this.ddlOraStampa.SelectedValue;
            
            
            idAmm = IdAmministrazione.ToString();

            if (!string.IsNullOrEmpty(freqStampaValue) && !string.IsNullOrEmpty(idAmm))
            {
                //
                // Chiamata al webMethod per il salvataggio della configurazione
                // Return Value = true (Operazione andata a buon fine)
                // Return Value = false (Operazione non andata a buon fine)
                result = _wsInstance.SaveStampaRegistroValues(idAmm, disabled, freqStampaValue, oraStampaValue);
            }

            if (!result)
            {
                //
                // Operazione non avvenuta
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "save_stampa_registro_KO", "alert('Salvataggio configurazione Stampa Registro non andato a buon fine');", true);

                //
                // MEV CS 1.3 - LOG Stampa Registro
                try
                {
                    DocsPAWA.AdminTool.Manager.SessionManager sessionManager = new DocsPAWA.AdminTool.Manager.SessionManager();
                    _wsInstance.WriteLog(sessionManager.getUserAmmSession(), "AMM_STAMPA_REGISTRO_CONS", string.Empty, "Attivazione stampa registro per Ente " + AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "0"), false);
                }
                catch (Exception ex) 
                {
                }
                // End MEV CS 1.3 - LOG Stampa Registro
                //
                
            }
            else
            {
                //
                // Perazione avvenuta con successo
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "save_stampa_registro_OK", "alert('Salvataggio configurazione Stampa Registro avvenuto correttamente');", true);
                this.GetDataOraInfo();

                //
                // MEV CS 1.3 - LOG Stampa Registro
                try
                {
                    DocsPAWA.AdminTool.Manager.SessionManager sessionManager = new DocsPAWA.AdminTool.Manager.SessionManager();
                    _wsInstance.WriteLog(sessionManager.getUserAmmSession(), "AMM_STAMPA_REGISTRO_CONS", string.Empty, "Attivazione stampa registro per Ente " + AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "0"), true);
                }
                catch (Exception ex) 
                {
                }
                // End MEV CS 1.3 - LOG Stampa Registro
                //
            }
        }

        protected void GetDataOraInfo()
        {
            string dataOraInfo = _wsInstance.GetStampaRegistroOraStampa(IdAmministrazione.ToString());
            if (!string.IsNullOrEmpty(dataOraInfo))
            {
                this._dataStampa = dataOraInfo.Split('_')[0];
                this._oraStampa = dataOraInfo.Split('_')[1];
                this.lblDate.Text = string.Format("Prossima stampa: {0} ore {1}", this._dataStampa, this._oraStampa);
            }

        }

        protected void caricaDdlOraStampa()
        {
            //recupero il valore memorizzato
            //string defaultValue = _wsInstance.GetStampaRegistroOraStampa(IdAmministrazione.ToString());
            //string dataOraInfo = _wsInstance.GetStampaRegistroOraStampa(IdAmministrazione.ToString());
            //string dataStampa = dataOraInfo.Split('_')[0];
            //string defaultValue = dataOraInfo.Split('_')[1];
            //this.lblDate.Text = string.Format("Prossima stampa: {0} ore {1}", this._dataStampa, this._oraStampa);
            

            for (int i = 0; i < 24; i++)
            {
                bool selValue = (i.ToString().Equals(this._oraStampa));
                ListItem l = new ListItem(i.ToString(), i.ToString());
                this.ddlOraStampa.Items.Add(l);
                if (selValue)
                    this.ddlOraStampa.SelectedIndex = i;

            }

        }
    }
}
