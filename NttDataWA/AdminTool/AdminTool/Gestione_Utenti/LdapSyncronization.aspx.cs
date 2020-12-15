using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SAAdminTool;

namespace SAAdminTool.AdminTool.Gestione_Utenti
{
    /// <summary>
    /// 
    /// </summary>
    public partial class LdapSyncronization : System.Web.UI.Page
    {
        #region Handler eventi pagina

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                this.Response.Expires = -1;

                if (!this.IsPostBack)
                {
                    this.FetchData();
                }
            }
            catch (System.Web.Services.Protocols.SoapException soapEx)
            {
                ApplicationException originalException = DocsPaUtils.Exceptions.SoapExceptionParser.GetOriginalException(soapEx);

                this.ShowErrorMessage(originalException.Message);
            }
            catch (Exception ex)
            {
                this.ShowErrorMessage(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_PreRender(object sender, EventArgs e)
        {
            try
            {
                this.SetControlVisibility();
            }
            catch (System.Web.Services.Protocols.SoapException soapEx)
            {
                ApplicationException originalException = DocsPaUtils.Exceptions.SoapExceptionParser.GetOriginalException(soapEx);

                this.ShowErrorMessage(originalException.Message);
            }
            catch (Exception ex)
            {
                this.ShowErrorMessage(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSaveLdapConfigurations_Click(object sender, EventArgs e)
        {
            try
            {
                this.SaveData();
            }
            catch (System.Web.Services.Protocols.SoapException soapEx)
            {
                ApplicationException originalException = DocsPaUtils.Exceptions.SoapExceptionParser.GetOriginalException(soapEx);

                this.ShowErrorMessage(originalException.Message);
            }
            catch (Exception ex)
            {
                this.ShowErrorMessage(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnImport_Click(object sender, EventArgs e)
        {
            try
            {
                this.Import();

                this.btnSaveLdapConfigurations.Enabled = true;
                this.btnCancel.Enabled = true;
            }
            catch (System.Web.Services.Protocols.SoapException soapEx)
            {
                ApplicationException originalException = DocsPaUtils.Exceptions.SoapExceptionParser.GetOriginalException(soapEx);

                this.ShowErrorMessage(originalException.Message);
            }
            catch (Exception ex)
            {
                this.ShowErrorMessage(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnHistory_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(this.txtIdHistory.Value))
                {
                    DocsPaWR.LdapSyncronizationResponse response = this.WsInstance.GetLdapSyncResponse(this.InfoUtente, Convert.ToInt32(this.txtIdHistory.Value));
                    
                    if (response == null)
                        throw new ApplicationException("Non è stato possibile reperire i dettagli sulla sincronizzazione selezionata");

                    this.FetchSyncronizationResponse(response);

                    this.txtIdHistory.Value = string.Empty;
                }
                else
                    throw new ApplicationException("Nessuna sincronizzazione selezionata");
            }
            catch (System.Web.Services.Protocols.SoapException soapEx)
            {
                ApplicationException originalException = DocsPaUtils.Exceptions.SoapExceptionParser.GetOriginalException(soapEx);

                this.ShowErrorMessage(originalException.Message);
            }
            catch (Exception ex)
            {
                this.ShowErrorMessage(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void txtNomeUtente_TextChanged(object sender, EventArgs e)
        {
            this.IsChangedNomeUtenteDominio = true;
        }

        #endregion

        #region Gestione dati

        /// <summary>
        /// 
        /// </summary>
        /// <param name="validationMessage"></param>
        /// <returns></returns>
        protected virtual bool ValidateData(out string firstInvalidControlId, out string validationMessage)
        {
            firstInvalidControlId = string.Empty;
            validationMessage = string.Empty;

            List<string> validationItems = new List<string>();
            
            if (this.IsRequiredValueMissing("Server LDAP", this.txtServerLdap.Text, validationItems))
                if (firstInvalidControlId == string.Empty) firstInvalidControlId = this.txtServerLdap.ClientID;

            if (this.IsRequiredValueMissing("Gruppo LDAP", this.txtRuoloLdap.Text, validationItems))
                if (firstInvalidControlId == string.Empty) firstInvalidControlId = this.txtRuoloLdap.ClientID;

            string invalidControl;
            if (this.IsValidDomainUserCredentials(validationItems, out invalidControl))
            {
                if (firstInvalidControlId == string.Empty)
                    firstInvalidControlId = invalidControl;
            }

            /*
            if (this.IsRequiredValueMissing("UserId", this.txtUserIdAttributeName.Text, validationItems))
                if (firstInvalidControlId == string.Empty) firstInvalidControlId = this.txtUserIdAttributeName.ClientID;

            if (this.IsRequiredValueMissing("Email", this.txtEmailAttributeName.Text, validationItems))
                if (firstInvalidControlId == string.Empty) firstInvalidControlId = this.txtEmailAttributeName.ClientID;

            if (this.IsRequiredValueMissing("Matricola", this.txtMatricolaAttributeName.Text, validationItems))
                if (firstInvalidControlId == string.Empty) firstInvalidControlId = this.txtMatricolaAttributeName.ClientID;

            if (this.IsRequiredValueMissing("Nome", this.txtNomeAttributeName.Text, validationItems))
                if (firstInvalidControlId == string.Empty) firstInvalidControlId = this.txtNomeAttributeName.ClientID;

            if (this.IsRequiredValueMissing("Cognome", this.txtCognomeAttributeName.Text, validationItems))
                if (firstInvalidControlId == string.Empty) firstInvalidControlId = this.txtCognomeAttributeName.ClientID;

            if (this.IsRequiredValueMissing("Sede", this.txtSedeAttributeName.Text, validationItems))
                if (firstInvalidControlId == string.Empty) firstInvalidControlId = this.txtSedeAttributeName.ClientID;
            */
              
            foreach (string item in validationItems)
            {
                if (validationMessage != string.Empty)
                    validationMessage += "\\n";
                else
                    validationMessage = "Sono state riscontrate le seguenti anomalie:\\n\\n";

                validationMessage += " - " + item;
            }

            return (validationItems.Count == 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="validationItems"></param>
        /// <param name="firstInvalidControlId"></param>
        /// <returns></returns>
        private bool IsValidDomainUserCredentials(List<string> validationItems, out string firstInvalidControlId)
        {
            firstInvalidControlId = string.Empty;

            if (this.txtNomeUtente.Text.Trim() != string.Empty)
            {
                if (this.IsChangedNomeUtenteDominio)
                {
                    if (this.IsRequiredValueMissing("Password", this.txtPassword.Text, validationItems))
                        firstInvalidControlId = this.txtPassword.ClientID;

                    if (this.IsRequiredValueMissing("Conferma password", this.txtConfermaPassword.Text, validationItems))
                        if (firstInvalidControlId == string.Empty) firstInvalidControlId = this.txtConfermaPassword.ClientID;
                }

                if (this.txtPassword.Text != this.txtConfermaPassword.Text)
                {
                    validationItems.Add("I valori dei campi Password e Conferma password non coincidono");

                    if (firstInvalidControlId != string.Empty)
                        firstInvalidControlId = this.txtPassword.Text;
                }
            }

            return string.IsNullOrEmpty(firstInvalidControlId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="validationItems"></param>
        /// <returns></returns>
        private bool IsRequiredValueMissing(string name, string value, List<string> validationItems)
        {
            bool missing = false;

            if (value.Trim() == string.Empty)
            {
                missing = true;
                validationItems.Add(string.Format("{0}: dato obbligatorio", name));
            }

            return missing;
        }

        /// <summary>
        /// Save dei dati di configurazione
        /// </summary>
        protected virtual bool SaveData()
        {
            string firstInvalidControl;
            string validationMessage;

            if (this.ValidateData(out firstInvalidControl, out validationMessage))
            {
                DocsPaWR.LdapConfig ldapInfo = new DocsPaWR.LdapConfig();

                ldapInfo.ServerName = this.txtServerLdap.Text;
                ldapInfo.GroupDN = this.txtRuoloLdap.Text;
                ldapInfo.DomainUserName = this.txtNomeUtente.Text;
                ldapInfo.DomainUserPassword = this.txtPassword.Text;

                // Impostazione attributi per il mapping dei campi ldap
                DocsPaWR.LdapUserAttributes ldapAttributes = new DocsPaWR.LdapUserAttributes();
                ldapAttributes.UserId = this.txtUserIdAttributeName.Text;
                ldapAttributes.Matricola = this.txtMatricolaAttributeName.Text;
                ldapAttributes.Email = this.txtEmailAttributeName.Text;
                ldapAttributes.Nome = this.txtNomeAttributeName.Text;
                ldapAttributes.Cognome = this.txtCognomeAttributeName.Text;
                ldapAttributes.Sede = this.txtSedeAttributeName.Text;
                ldapInfo.UserAttributes = ldapAttributes;

                this.WsInstance.SaveLdapConfig(this.InfoUtente, this.IdAmministrazione, ldapInfo);

                this.IsChangedNomeUtenteDominio = false;

                return true;
            }
            else
            {
                this.ShowErrorMessage(validationMessage);

                // Impostazione focus sul primo campo non valido
                this.SetControlFocus(firstInvalidControl);

                return false;
            }
        }

        /// <summary>
        /// Task di import degli utenti
        /// </summary>
        protected virtual void Import()
        {
            if (this.SaveData())
            {
                DocsPaWR.LdapSyncronizationRequest request = new DocsPaWR.LdapSyncronizationRequest
                {
                    InfoUtente = this.InfoUtente,
                    IdAmministrazione = this.IdAmministrazione
                };

                DocsPaWR.LdapSyncronizationResponse response = this.WsInstance.SyncronizeLdapUsers(request);

                this.FetchSyncronizationResponse(response);

                if (string.IsNullOrEmpty(response.ErrorDetails) && !this.IsDirty)
                    this.IsDirty = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected bool IsDirty
        {
            get
            {
                if (string.IsNullOrEmpty(this.txtReturnValue.Value))
                    return false;
                else
                    return Convert.ToBoolean(this.txtReturnValue.Value);
            }
            set
            {
                this.txtReturnValue.Value = value.ToString().ToLower();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected bool IsChangedNomeUtenteDominio
        {
            get
            {
                if (this.ViewState["IsChangedNomeUtenteDominio"] == null)
                    return false;
                else
                    return Convert.ToBoolean(this.ViewState["IsChangedNomeUtenteDominio"]);
            }
            set
            {
                this.ViewState["IsChangedNomeUtenteDominio"] = value;
            }
        }

        /// <summary>
        /// Caricamento dati esito sincronizzazione
        /// </summary>
        /// <param name="response"></param>
        protected virtual void FetchSyncronizationResponse(DocsPaWR.LdapSyncronizationResponse response)
        {
            this.grdEsitoSincronizzazione.DataSource = response.Items;
            this.grdEsitoSincronizzazione.DataBind();

            string message = string.Empty;

            if (string.IsNullOrEmpty(response.ErrorDetails))
                message = string.Format("Utenti sincronizzati: {0}", response.ItemsSyncronized.ToString());
            else
                message = response.ErrorDetails;

            this.SetMessage(message);
        }

        /// <summary>
        /// Caricamento dati
        /// </summary>
        protected virtual void FetchData()
        {
            this.ClearData();
            
            DocsPaWR.LdapConfig ldapInfo = this.WsInstance.GetLdapConfig(this.InfoUtente, this.IdAmministrazione);

            if (ldapInfo != null)
            {
                this.txtServerLdap.Text = ldapInfo.ServerName;
                this.txtRuoloLdap.Text = ldapInfo.GroupDN;
                this.txtNomeUtente.Text = ldapInfo.DomainUserName;

                if (ldapInfo.UserAttributes != null)
                {
                    this.txtUserIdAttributeName.Text = ldapInfo.UserAttributes.UserId;
                    this.txtMatricolaAttributeName.Text = ldapInfo.UserAttributes.Matricola;
                    this.txtEmailAttributeName.Text = ldapInfo.UserAttributes.Email;
                    this.txtNomeAttributeName.Text = ldapInfo.UserAttributes.Nome;
                    this.txtCognomeAttributeName.Text = ldapInfo.UserAttributes.Cognome;
                    this.txtSedeAttributeName.Text = ldapInfo.UserAttributes.Sede;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected string GetUtenteCellText(DocsPaWR.LdapSyncronizationResponseItem item)
        {
            return item.UserId;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected string GetEsitoSincronizzazioneCellText(DocsPaWR.LdapSyncronizationResponseItem item)
        {
            string esito = string.Empty;

            if (item.Result == SAAdminTool.DocsPaWR.LdapSyncronizationResultEnum.Inserted)
                esito = "Inserito";
            else if (item.Result == SAAdminTool.DocsPaWR.LdapSyncronizationResultEnum.Updated)
                esito = "Modificato";
            else if (item.Result == SAAdminTool.DocsPaWR.LdapSyncronizationResultEnum.Deleted)
                esito = "Cancellato";
            else if (item.Result == SAAdminTool.DocsPaWR.LdapSyncronizationResultEnum.Error)
                esito = "Errore";

            if (!string.IsNullOrEmpty(item.Details))
                esito = string.Concat(esito, string.Format("<BR />{0}", item.Details));

            return esito;
        }   

        /// <summary>
        /// Rimozione dati
        /// </summary>
        protected virtual void ClearData()
        {
            this.txtServerLdap.Text = string.Empty;
            this.txtRuoloLdap.Text = string.Empty;
            this.txtNomeUtente.Text = string.Empty;
            this.txtUserIdAttributeName.Text = string.Empty;
            this.txtMatricolaAttributeName.Text = string.Empty;
            this.txtEmailAttributeName.Text = string.Empty;
            this.txtNomeAttributeName.Text = string.Empty;
            this.txtCognomeAttributeName.Text = string.Empty;
            this.txtSedeAttributeName.Text = string.Empty;
        }

        /// <summary>
        /// Reperimento id amministrazione corrente
        /// </summary>
        protected string IdAmministrazione
        {
            get
            {
                return AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "3");
            }
        }

        /// <summary>
        /// Reperimento infoutente corrente
        /// </summary>
        protected DocsPaWR.InfoUtente InfoUtente
        {
            get
            {
                AdminTool.Manager.SessionManager sessionManager = new AdminTool.Manager.SessionManager();
                return sessionManager.getUserAmmSession();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private DocsPaWR.DocsPaWebService _instance = null;

        /// <summary>
        /// 
        /// </summary>
        protected DocsPaWR.DocsPaWebService WsInstance
        {
            get
            {
                if (this._instance == null)
                {
                    this._instance = new DocsPaWR.DocsPaWebService();
                    this._instance.Timeout = System.Threading.Timeout.Infinite;
                }

                return this._instance;
            }
        }

        #endregion

        #region Gestione controlli grafici

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        protected void SetMessage(string message)
        {
            this.lblMessage.Text = message;
        }

        /// <summary>
        /// Impostazione visibilità controlli grafici
        /// </summary>
        protected virtual void SetControlVisibility()
        {
            this.grdEsitoSincronizzazione.Visible = (this.grdEsitoSincronizzazione.Items.Count > 0);
        }

        #endregion

        #region Gestione JavaScript

        /// <summary>
        /// Registrazione script client
        /// </summary>
        /// <param name="scriptKey"></param>
        /// <param name="scriptValue"></param>
        private void RegisterClientScript(string scriptKey, string scriptValue)
        {
            if (!this.Page.ClientScript.IsStartupScriptRegistered(this.GetType(), scriptKey))
            {
                string scriptString = "<SCRIPT>" + scriptValue + "</SCRIPT>";
                this.Page.ClientScript.RegisterStartupScript(this.GetType(), scriptKey, scriptString);
            }
        }

        /// <summary>
        /// Impostazione messaggio di errore
        /// </summary>
        /// <param name="errorMessage"></param>
        private void ShowErrorMessage(string errorMessage)
        {
            this.RegisterClientScript("ErrorMessage", "alert('" + errorMessage.Replace("'", "\\'") + "')");
        }

        /// <summary>
        /// Impostazione del focus su un controllo
        /// </summary>
        /// <param name="controlID"></param>
        private void SetControlFocus(string controlID)
        {
            this.RegisterClientScript("SetFocus", "SetControlFocus('" + controlID + "');");
        }

        #endregion
    }
}