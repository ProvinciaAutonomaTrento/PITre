using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NttDataWA.CheckInOut
{
    public partial class CheckOutModel : System.Web.UI.Page
    {
        private string language;

        protected NttDataWA.ActivexWrappers.ClientModelProcessor clientModelProcessor;
        protected NttDataWA.DocsPaWR.SchedaDocumento schedaCorrente;

        protected void Page_Load(object sender, EventArgs e)
        {
            this.schedaCorrente = NttDataWA.UIManager.DocumentManager.getSelectedRecord();
            this.language = NttDataWA.UIManager.UserManager.GetUserLanguage();

            if (!IsPostBack && !ModelProcessed)
            {
                this.litInformationMessage.Text = NttDataWA.Utils.Languages.GetLabelFromCode("CheckOutModel_ActiveX_Whait", language);
                ModelProcessed = true;
                HttpContext.Current.Session["isCheckOutModel"] = true;
                ScriptManager.RegisterStartupScript(this, this.GetType(), "InitializeCtrlScript", "if (confirmAction()) parent.closeAjaxModal('CheckOutModelActiveX','up'); else parent.closeAjaxModal('CheckOutModelActiveX', '');", true);
            }
        }

        protected bool ModelProcessed
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["ModelProcessed"] != null)
                {
                    result = (bool)HttpContext.Current.Session["ModelProcessed"];
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["ModelProcessed"] = value;
            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected bool HasModelProcessorSelected()
        {
            return this.clientModelProcessor.HasModelProcessorSelected;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected string GetModelProcessorSupportedExtensions()
        {
            return this.clientModelProcessor.SupportedExtensions;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected string GetDocumentId()
        {
            string retVal = string.Empty;
            if (string.IsNullOrEmpty(NttDataWA.UIManager.DocumentManager.getSelectedAttachId()))
                retVal = this.schedaCorrente.systemId;
            else
            {
                if (NttDataWA.UIManager.DocumentManager.GetSelectedAttachment() != null)
                    retVal = NttDataWA.UIManager.DocumentManager.GetSelectedAttachment().docNumber;
            }
            return retVal;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected string GetDocumentNum()
        {
            string retVal = string.Empty;
            if (string.IsNullOrEmpty(NttDataWA.UIManager.DocumentManager.getSelectedAttachId()))
                retVal = this.schedaCorrente.docNumber;
            else
            {
                if (NttDataWA.UIManager.DocumentManager.GetSelectedAttachment() != null)
                    retVal = NttDataWA.UIManager.DocumentManager.GetSelectedAttachment().docNumber;
            }
            return retVal;
        }

        /// <summary>
        /// Calcolo del modello per la stampa del documento correntemente in lavorazione
        /// </summary>
        protected string ModelloDocumentoCorrente
        {
            get
            {
                string modello = string.Empty;

                if (this.schedaCorrente != null && !string.IsNullOrEmpty(this.schedaCorrente.systemId))
                {
                    if (this.schedaCorrente.documentoPrincipale != null)
                    {
                        //Modello di allegato
                        modello = "A1";
                    }
                    else
                    {
                        //se sono in allegati
                        if (!string.IsNullOrEmpty(NttDataWA.UIManager.DocumentManager.getSelectedAttachId()))
                        {
                            // Modello allegato da tab allegati della scheda documento principale
                            modello = "A1";
                        }
                        else
                        {
                            // Reperimento versione corrente del documento
                            DocsPaWR.FileRequest fileRequest = this.schedaCorrente.documenti[0];

                            if (fileRequest != null &&
                                Convert.ToInt32(fileRequest.version) >= 1 &&
                                Convert.ToInt32(fileRequest.fileSize) > 0)
                            {
                                if (System.Configuration.ConfigurationManager.AppSettings["MODELLO_DOCUMENTO"] != null &&
                                    System.Configuration.ConfigurationManager.AppSettings["MODELLO_DOCUMENTO"] == "2")
                                    //Modello Due
                                    modello = "2";
                                else
                                    // Modello Uno
                                    modello = "1";
                            }
                            else
                            {
                                // Modello Uno
                                modello = "1";
                            }
                        }
                    }
                }

                return modello;
            }
        }

    }

}