using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SAAdminTool.utils;

namespace SAAdminTool.UserControls
{
    /// <summary>
    /// User Control per la gestione del consolidamento di un documento
    /// </summary>
    public partial class DocumentConsolidation : System.Web.UI.UserControl
    {

        public bool HasItemChecked
        {
            get
            {
                if (this.ViewState["HasItemChecked"] == null) return false;
                return (bool) this.ViewState["HasItemChecked"];
            }
            set
            {
                this.ViewState["HasItemChecked"] = value;
                if (value)
                {
                    this.btnConsolidateStep1.OnClientClick = "openConsolidation('document');";
                    this.btnConsolidateStep2.OnClientClick = "openConsolidation('metadati');";
                }
                else
                {
                    this.btnConsolidateStep1.OnClientClick = string.Empty;
                    this.btnConsolidateStep2.OnClientClick = string.Empty;

                }
            }
        }

        public delegate void UpdateSavedCheckingStatusDelegate();

        public UpdateSavedCheckingStatusDelegate UpdateSavedCheckingStatus
        {
            get;
            set;
        }

        public delegate void ShowMessageDelegate(string message);

        public ShowMessageDelegate ShowMessageDel
        {
            get;
            set;
        }


        /// <summary>
        /// Evento di consolidamento del documento
        /// </summary>
        [System.ComponentModel.Browsable(true)]
        public event DocumentConsolidatedDelegate Consolidated = null;

        protected string ConsolidationMassivePage
        {
            get
            {
                return String.Format("{0}/MassiveOperation/DocumentConsolidation.aspx",
                    Utils.getHttpFullPath(this.Page));
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {   
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (!MassiveOperationContext) {
                this.btnConsolidateStep1.OnClientClick = "if (confirmConsolidateStep1()) { window.document.body.style.cursor='wait'; return true; } else { return false; }";
                this.btnConsolidateStep2.OnClientClick = "if (confirmConsolidateStep2()) { window.document.body.style.cursor='wait'; return true; } else { return false; }";
            }
            this.RefreshControlsEnabled();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnConsolidateStep1_OnClick(object sender, EventArgs e)
        {
            try
            {
                if (MassiveOperationContext)
                {
                    if (!HasItemChecked)
                    {
                        ShowMessage("Selezionare almeno un documento per avviare il consolidamento");
                    }
                }
                else
                {
                    this.ConsolidateDocument(DocsPaWR.DocumentConsolidationStateEnum.Step1);
                }
            }
            catch (Exception ex)
            {
                ShowMessage(ex.Message);
            }
        }

        protected void ShowMessage(string message)
        {
            if (ShowMessageDel != null)
            {
                ShowMessageDel(message);
            }
            else
            {
                this.Page.ClientScript.RegisterStartupScript(this.Page.GetType(),
                            "message",
                            string.Format("alert('{0}');", message.Replace("'", @"\'")), true);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnConsolidateStep2_OnClick(object sender, EventArgs e)
        {
            try
            {
                if (MassiveOperationContext)
                {
                    if (!HasItemChecked)
                    {
                        ShowMessage("Selezionare almeno un documento per avviare il consolidamento");
                    }
                }
                else
                {
                    this.ConsolidateDocument(DocsPaWR.DocumentConsolidationStateEnum.Step2);
                }
            }
            catch (Exception ex)
            {
                ShowMessage(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="toState"></param>
        protected void ConsolidateDocument(DocsPaWR.DocumentConsolidationStateEnum toState)
        {
            try
            {
                DocsPaWR.InfoUtente userInfo = UserManager.getInfoUtente();
                DocumentConsolidationHandler dch = new DocumentConsolidationHandler(Consolidated, userInfo);
                dch.ConsolidateDocument(this.Document, toState);
            }
            catch (Exception ex)
            {
                DocsPaUtils.Exceptions.SoapExceptionParser.ThrowOriginalException(ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected string ConsolidateMessage1
        {
            get
            {
                if (!MassiveOperationContext)
                {
                    return "Attenzione:\\n" +
                                "Si sta per consolidare il documento, non sarà più possibile acquisire o modificare versioni.\\n" +
                                "L'operazione ha carattere di irreversibilità. Si desidera continuare?";
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected string ConsolidateMessage2
        {
            get
            {
                string message = string.Empty;
                if (this.Document!= null && this.Document.ConsolidationState != null)
                {
                    if (!MassiveOperationContext)
                    {
                        message = "Attenzione:\\n" +
                                    "Si sta per consolidare il documento nei suoi metadati fondamentali.";
                        if (this.Document.ConsolidationState.State == DocsPaWR.DocumentConsolidationStateEnum.None)
                        {
                            message += "\\nInoltre non sarà più possibile acquisire o modificare versioni.";
                        }
                        message += "\\nL'operazione ha carattere di irreversibilità. Si desidera continuare?";
                    }
                }
                return message;
            }
        }

        /// <summary>
        /// True, se lo usercontrol è istanziato nel contesto di azioni massive
        /// </summary>
        protected bool MassiveOperationContext
        {
            get
            {
                return SAAdminTool.utils.MassiveOperationUtils.IsCollectionInitialized();
            }
        }

        /// <summary>
        /// Documento correntemente selezionato
        /// </summary>
        protected DocsPaWR.SchedaDocumento Document
        {
            get
            {
               return DocumentManager.getDocumentoSelezionato();
            }
        }

        /// <summary>
        /// Verifica se il consolidamento è abilitato in amministrazione
        /// </summary>
        protected bool HasDocumentConsolidationRights
        {
            get
            {
                if (this.ViewState["HasDocumentConsolidationRights"] == null)
                    this.ViewState["HasDocumentConsolidationRights"] = ProxyManager.getWS().HasDocumentConsolidationRights(UserManager.getInfoUtente());
                return (bool)this.ViewState["HasDocumentConsolidationRights"];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="functionName"></param>
        /// <returns></returns>
        protected bool HasFunction(string functionName)
        {
            DocsPaWR.Ruolo role = UserManager.getRuolo();

            return role.funzioni.Count(e => e.codice == functionName) > 0;
        }

        /// <summary>
        /// Gestione abilitazione pulsanti
        /// </summary>
        protected void RefreshControlsEnabled()
        {
            if (!HasDocumentConsolidationRights)
            {
                // Consolidamento disabilitato
                this.btnConsolidateStep1.Visible = false;
                this.btnConsolidateStep2.Visible = false;
            }
            else
            {
                if (this.MassiveOperationContext)
                {
                    this.btnConsolidateStep1.Visible = this.HasFunction("DO_CONSOLIDAMENTO");
                    this.btnConsolidateStep2.Visible = this.HasFunction("DO_CONSOLIDAMENTO_METADATI");
                }
                else
                {
                    if (this.Document != null && !string.IsNullOrEmpty(this.Document.systemId))
                    {
                        if (this.Document.documentoPrincipale != null)
                        {
                            // Gli allegati non possono gestire direttamente il consolidamento
                            this.btnConsolidateStep1.Visible = false;
                            this.btnConsolidateStep2.Visible = false;
                        }
                        else
                        {
                            this.btnConsolidateStep1.Visible = this.HasFunction("DO_CONSOLIDAMENTO") &&
                                                        (this.Document.ConsolidationState.State == DocsPaWR.DocumentConsolidationStateEnum.None);
                            this.btnConsolidateStep2.Visible = this.HasFunction("DO_CONSOLIDAMENTO_METADATI") &&
                                                        (this.Document.ConsolidationState.State < DocsPaWR.DocumentConsolidationStateEnum.Step2);
                        }
                    }
                    else
                    {
                        this.btnConsolidateStep1.Visible = false;
                        this.btnConsolidateStep2.Visible = false;
                    }
                }
            }
        }
    }

}