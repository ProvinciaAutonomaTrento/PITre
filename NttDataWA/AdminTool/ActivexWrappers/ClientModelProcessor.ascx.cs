using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SAAdminTool.ActivexWrappers
{
    /// <summary>
    /// UserControl per la creazione di un modello di stampa unione documento ClientSide
    /// </summary>
    public partial class ClientModelProcessor : System.Web.UI.UserControl
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        { }

        /// <summary>
        /// Reperimento Url del WebService di DocsPa
        /// </summary>
        protected string WebServiceUrl
        {
            get
            {
                string webServiceUrl = utils.InitConfigurationKeys.GetValue("0", "BE_WEBSERVICE_URL");

                if (string.IsNullOrEmpty(webServiceUrl))
                    webServiceUrl = SAAdminTool.Properties.Settings.Default.AdminTool_DocsPaWR_DocsPaWebService;

                return webServiceUrl;
            }
        }

        /// <summary>
        /// Reperimento InfoUtente corrente
        /// </summary>
        protected DocsPaWR.InfoUtente InfoUtente
        {
            get
            {
                return UserManager.getInfoUtente();
            }
        }

        /// <summary>
        /// Reperimento di tutti i model processor disponibili nel sistema
        /// </summary>
        /// <returns></returns>
        public DocsPaWR.ModelProcessorInfo[] ModelProcessors
        {
            get
            {
                if (this.ViewState["GetModelProcessors"] == null)
                {
                    using (SAAdminTool.DocsPaWR.DocsPaWebService ws = new SAAdminTool.DocsPaWR.DocsPaWebService())
                        this.ViewState["GetModelProcessors"] = ws.GetModelProcessors(UserManager.getInfoUtente());
                }
                return (DocsPaWR.ModelProcessorInfo[]) this.ViewState["GetModelProcessors"];
            }
        }

        /// <summary>
        /// Reperimento del ModelProcessor per l'utente corrente
        /// </summary>
        /// <returns></returns>
        public DocsPaWR.ModelProcessorInfo CurrentModelProcessor
        {
            get
            {
                if (this.ViewState["GetCurrentModelProcessor"] == null)
                {
                    using (SAAdminTool.DocsPaWR.DocsPaWebService ws = new SAAdminTool.DocsPaWR.DocsPaWebService())
                        this.ViewState["GetCurrentModelProcessor"] = ws.GetCurrentModelProcessor(UserManager.getInfoUtente());
                }
                return (DocsPaWR.ModelProcessorInfo)this.ViewState["GetCurrentModelProcessor"];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool HasModelProcessorSelected
        {
            get
            {
                return (this.CurrentModelProcessor != null);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string SupportedExtensions
        {
            get
            {
                if (this.CurrentModelProcessor != null)
                    return this.CurrentModelProcessor.supportedExtensions;
                else
                    return string.Empty;
            }
        }

        /// <summary>
        /// Reperimento dell'estensione predefinita per i file
        /// elaborati dal word processor predefinito
        /// </summary>
        /// <returns></returns>
        public string DefaultExtension
        {
            get
            {
                string defaultExt = string.Empty;

                if (this.CurrentModelProcessor != null)
                {
                    string[] exts = this.CurrentModelProcessor.supportedExtensions.Split('|');
                    if (exts.Length > 0) defaultExt = exts[0];
                }

                return defaultExt;
            }
        }
    }
}