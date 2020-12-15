using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SAAdminTool.AdminTool.Gestione_Utenti
{
    public partial class LdapSyncronizationHistory : System.Web.UI.Page
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

                this.RegisterScrollKeeper(this.divContainer.ClientID);

                if (!this.IsPostBack)
                {
                    this.FetchData();
                }
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
                this.SetMessage(string.Format("Sincronizzazioni LDAP effettuate: {0}", this.grdHistory.Items.Count));

                this.SetControlsVisibility();
            }
            catch (Exception ex)
            {
                this.ShowErrorMessage(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        protected void grdHistory_ItemCommand(object source, DataGridCommandEventArgs e)
        {
            try
            {
                if (e.CommandName == "SELECT_ITEM")
                {
                    this.SelectItemCommand(e.Item.Cells[0].Text);
                }
                else if (e.CommandName == "DELETE_ITEM")
                {
                    this.DeleteItemCommand(e.Item.Cells[0].Text);
                }
            }
            catch (Exception ex)
            {
                this.ShowErrorMessage(ex.Message);
            }
        }

        #endregion

        #region Gestione controlli grafici

        /// <summary>
        /// 
        /// </summary>
        private void RegisterScrollKeeper(string divID)
        {
            AdminTool.UserControl.ScrollKeeper scrollKeeper = new AdminTool.UserControl.ScrollKeeper();
            scrollKeeper.WebControl = divID;
            this.Form.Controls.Add(scrollKeeper);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        protected void SetMessage(string message)
        {
            this.lblMessage.Text = message;
        }

        /// <summary>
        /// 
        /// </summary>
        protected void SetControlsVisibility()
        {
            this.grdHistory.Visible = (this.grdHistory.Items.Count > 0);
        }

        #endregion

        #region Gestione dati

        /// <summary>
        /// Caricamento dati
        /// </summary>
        protected virtual void FetchData()
        {
            this.grdHistory.DataSource = this.WsInstance.GetLdapSyncHistory(this.InfoUtente, this.IdAmministrazione);
            this.grdHistory.DataBind();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        protected virtual void SelectItemCommand(string id)
        {
            this.txtReturnValue.Value = id;

            this.RegisterClientScript("CloseDialog", "CloseDialog()");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        protected virtual void DeleteItemCommand(string id)
        {
            this.WsInstance.DeleteLdapSyncHistoryItem(this.InfoUtente, Convert.ToInt32(id));

            this.FetchData();
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

        #endregion

    }
}
