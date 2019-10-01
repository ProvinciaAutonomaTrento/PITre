using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.DocsPaWR;
using NttDataWA.UIManager;
using System.ComponentModel;

using System.Web.SessionState;
using System.Web.UI.HtmlControls;
using NttDataWA.Utils;

namespace NttDataWA.UserControls
{
    public partial class HeaderDocument : System.Web.UI.UserControl
    {
        private const string NOTPROTOCOL = "N";
        private const string ATTACHMENT = "ALL";
        private const int CODE_ATTACH_USER = 1;
        private const int CODE_ATTACH_PEC = 2;
        private const int CODE_ATTACH_IS = 3;
        private const int CODE_ATTACH_EXT = 4;

        /// <summary>
        /// Indentifica se siamo nella situazione di apertura della popup(inserita per evitare che alla chiusura della popup riesegua il tutto il page_load)
        /// </summary>
        private bool OpenSignaturePopup
        {
            get
            {
                if (HttpContext.Current.Session["OpenSignaturePopup"] != null)
                    return (bool)HttpContext.Current.Session["OpenSignaturePopup"];
                else return false;
            }
            set
            {
                HttpContext.Current.Session["OpenSignaturePopup"] = value;
            }
        }

        private bool IsConservazioneSACER
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["isConservazioneSACER"] != null)
                {
                    return (bool)HttpContext.Current.Session["isConservazioneSACER"];
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["isConservazioneSACER"] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.InitializePage();
                this.KeyEnable();
                this.InitializeLanguage();
                this.VisibiltyRoleFunctions();
            }
            else
            {
                if (!string.IsNullOrEmpty(this.HistoryPreserved.ReturnValue))
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('HistoryPreserved','');", true);
                    Response.Redirect("Document.aspx");
                }
            }

            this.ReApplyPermitOnlyNumbersScript();
        }

        private void ClearSessionData()
        {
            Session["ChooseRFSegnature_code"] = null;
        }

        private void ManageSendReceipt()
        {
            this.DocumentImgSendReceipt.Enabled = false;

            SchedaDocumento doc = DocumentManager.getSelectedRecord();
            if (doc != null && !string.IsNullOrEmpty(doc.systemId))
            {
                //Andrea De Marco - Gestione Eccezioni PEC
                //In eseguiSenzaSegnatura prima del save della scheda documento, imposto sd.interop = E
                //Se schedaDoc.interop = E si è verificata un'eccezione nella gestione del file Segnatura.xml, pertanto il tasto di invio Conferma.xml deve essere disabilitato 
                //Per ripristino, commentare De Marco e decommentare il codice immediatamente sottostante
                if (!string.IsNullOrEmpty(doc.interop) && doc.interop.ToUpper().Equals("E"))
                {
                    this.DocumentImgSendReceipt.Enabled = false;
                }
                else
                {
                    if (
                        doc.protocollo != null
                        && !string.IsNullOrEmpty(doc.protocollo.segnatura)
                        && (doc.typeId == "MAIL" || doc.typeId == "INTEROPERABILITA")
                        && UserManager.IsAuthorizedFunctions("DO_INVIO_RICEVUTE")
                        )
                    {
                        this.DocumentImgSendReceipt.Enabled = true;
                    }
                }
                //End Andrea De Marco
            }
        }

        protected void VisibiltyRoleFunctions()
        {
            if (!UIManager.UserManager.IsAuthorizedFunctions("DO_CONS") || this.IsConservazioneSACER)
            {
                this.imgSeparator7.Visible = false;
                this.DocumentImgPreservation.Visible = false;
            }
            // INTEGRAZIONE PITRE-PARER
            if (!UIManager.UserManager.IsAuthorizedFunctions("DO_SACER_RECUPERO") || !this.IsConservazioneSACER)
            {
                this.DocumentImgRecupero.Visible = false;
            }

            //MEV LIBRO FIRMA
            if (!UserManager.IsAuthorizedFunctions("DO_STATE_SIGNATURE_PROCESS"))
            {
                this.DocumentImgInfoProcessiAvviati.Visible = false;
            }

            this.ManageSendReceipt();
        }

        protected void KeyEnable()
        {
            //***************************************************************
            //GIORDANO IACOZZILLI
            //17/07/2013
            //Gestione dell'icona della copia del docuemnto/fascicolo in deposito.
            //***************************************************************
            try
            {
                if (this.FlagCopyInArchive == "1")
                {
                    this.phNeutro.Visible = true;
                    this.cimgbttIsCopy.ToolTip = "Copia del documento in archivio corrente";
                }
            }
            catch
            {
                //Questo è per Veltri.
            }
            //***************************************************************
            //FINE
            //***************************************************************
            string key = Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_MULTI_STAMPA_ETICHETTA.ToString());
            if (string.IsNullOrEmpty(key) || key.Equals("0"))
            {
                this.TxtPrintLabel.Visible = false;
                this.DocumentLblPrintLabel.Visible = false;
            }

            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.LITEDOCUMENT.ToString()]) && bool.Parse(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.LITEDOCUMENT.ToString()]))
            {
                this.DocumentImgPrintLabel.Visible = false;
                this.DocumentImgPrintA4.Visible = false;
                this.DocumentImgPrintReceipt.Visible = false;
                this.imgSeparator6.Visible = false;
                this.imgSeparator7.Visible = false;
                this.DocumentImgSendReceipt.Visible = false;
            }

            if (string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_LIBRO_FIRMA.ToString())) || !Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_LIBRO_FIRMA.ToString()).Equals("1"))
            {
                this.DocumentImgInfoProcessiAvviati.Visible = false;
            }

        }

        protected void InitializePage()
        {

            if (!string.IsNullOrEmpty(this.TypeDocument))
            {
                switch (this.TypeDocument.ToUpper())
                {
                    case "A":
                        this.PProtocolType.InnerText = DocumentManager.GetCodeLabel("A");
                        this.PnlcontainerDocumentTopSx.Attributes.Add("class", "containerDocumentTopSxOrange");
                        this.containerDocumentTopDx.Attributes.Add("class", "containerDocumentTopDxOrange");
                        this.containerDocumentTopCxOrange.Attributes.Add("class", "containerDocumentTopCxOrangeBg");
                        this.SetTypeRecord();
                        break;
                    case "P":
                        this.PProtocolType.InnerText = DocumentManager.GetCodeLabel("P");
                        this.PnlcontainerDocumentTopSx.Attributes.Add("class", "containerDocumentTopSxGreen");
                        this.containerDocumentTopDx.Attributes.Add("class", "containerDocumentTopDxGreen");
                        this.containerDocumentTopCxOrange.Attributes.Add("class", "containerDocumentTopCxGreenBg");
                        this.SetTypeRecord();
                        break;
                    case "I":
                        this.PProtocolType.InnerText = DocumentManager.GetCodeLabel("I");
                        this.PnlcontainerDocumentTopSx.Attributes.Add("class", "containerDocumentTopSxBlue");
                        this.containerDocumentTopDx.Attributes.Add("class", "containerDocumentTopDxBlue");
                        this.containerDocumentTopCxOrange.Attributes.Add("class", "containerDocumentTopCxBlueBg");
                        this.SetTypeRecord();
                        break;
                    case NOTPROTOCOL:
                    case "G":
                    case "R":
                    case "C":
                        SchedaDocumento doc = DocumentManager.getSelectedRecord();

                        this.PProtocolType.InnerText = DocumentManager.GetCodeLabel("G");
                        if (doc != null && !string.IsNullOrEmpty(doc.systemId) && doc.ConsolidationState != null && doc.ConsolidationState.State == DocumentConsolidationStateEnum.Step2)
                        {
                            this.PnlcontainerDocumentTopSx.Attributes.Add("class", "containerDocumentTopSxGreyConsolidate");
                        }
                        else
                        {
                            this.PnlcontainerDocumentTopSx.Attributes.Add("class", "containerDocumentTopSxGrey");
                        }
                        this.containerDocumentTopDx.Attributes.Add("class", "containerDocumentTopDxGrey");
                        this.containerDocumentTopCxOrange.Attributes.Add("class", "containerDocumentTopCxGreyBg");
                        this.DocumentImgPrintA4.Visible = false;
                        break;
                    case ATTACHMENT:
                        bool found = false;
                        int typeAttach = Convert.ToInt32(this.Page.Request["typeAttachment"]);
                        switch (typeAttach)
                        {
                            case CODE_ATTACH_PEC:
                                this.PnlcontainerDocumentTopSx.Attributes.Add("class", "containerDocumentTopSxGreyPec");
                                found = true;
                                break;
                            case CODE_ATTACH_USER:
                                this.PnlcontainerDocumentTopSx.Attributes.Add("class", "containerDocumentTopSxGreyUser");
                                found = true;
                                break;
                            case CODE_ATTACH_IS:
                                this.PnlcontainerDocumentTopSx.Attributes.Add("class", "containerDocumentTopSxGreyPITRE");
                                found = true;
                                break;
                            case CODE_ATTACH_EXT:
                                this.PnlcontainerDocumentTopSx.Attributes.Add("class", "containerDocumentTopSxGreyExternal");
                                found = true;
                                break;
                        }

                        if (!found)
                        {
                            SchedaDocumento docTemp = DocumentManager.getSelectedRecord();
                            if (docTemp != null && docTemp.documenti.Length > 0)
                            {
                                string versionId = docTemp.documenti[0].versionId;
                                string allegatoPec = DocumentManager.AllegatoIsPEC(versionId);
                                if (!string.IsNullOrEmpty(allegatoPec) && !allegatoPec.Equals("0"))
                                {
                                    this.PnlcontainerDocumentTopSx.Attributes.Add("class", "containerDocumentTopSxGreyPec");
                                }
                                else
                                {
                                    string allegatoIS = DocumentManager.AllegatoIsIS(versionId);
                                    if (!string.IsNullOrEmpty(allegatoIS) && !allegatoIS.Equals("0"))
                                    {
                                        this.PnlcontainerDocumentTopSx.Attributes.Add("class", "containerDocumentTopSxGreyPITRE");
                                    }
                                    else
                                    {
                                        string allegatoExt = DocumentManager.AllegatoIsEsterno(versionId);
                                        if (!string.IsNullOrEmpty(allegatoExt) && !allegatoExt.Equals("0"))
                                        {
                                            this.PnlcontainerDocumentTopSx.Attributes.Add("class", "containerDocumentTopSxGreyExternal");
                                        }
                                        else
                                        {
                                            this.PnlcontainerDocumentTopSx.Attributes.Add("class", "containerDocumentTopSxGreyUser");
                                        }
                                    }
                                }
                            }

                        }



                        //Una volta completati i metodi sopra cancellare le righe sotto
                        //this.PProtocolType.InnerText = DocumentManager.GetCodeLabel("ALL");
                        //fine cancellazione
                        this.containerDocumentTopDx.Attributes.Add("class", "containerDocumentTopDxGrey");
                        this.containerDocumentTopCxOrange.Attributes.Add("class", "containerDocumentTopCxGreyBg");
                        this.DocumentImgSendReceipt.Visible = false;
                        break;
                }
            }

            this.RefreshDataDocument();
        }

        protected void SetTypeRecord()
        {
            if (!string.IsNullOrEmpty(this.TypeRecord))
            {
                SchedaDocumento doc = UIManager.DocumentManager.getSelectedRecord();
                DocsPaWR.ProtocolloAnnullato protAborted = null;
                bool consolidation = false;
                if (doc != null && doc.tipoProto != null && doc.protocollo != null && doc.protocollo.protocolloAnnullato != null)
                {
                    protAborted = doc.protocollo.protocolloAnnullato;
                }

                if (doc != null && doc.ConsolidationState != null && doc.ConsolidationState.State == DocumentConsolidationStateEnum.Step2)
                {
                    consolidation = true;
                }

                switch (this.TypeRecord.ToUpper())
                {
                    case "A":
                        this.PProtocolType.InnerText = DocumentManager.GetCodeLabel("A");
                        if (protAborted == null)
                        {
                            if ((doc == null || doc.protocollo == null) || (doc != null && !string.IsNullOrEmpty(doc.systemId) && doc.protocollo != null && !string.IsNullOrEmpty(doc.protocollo.segnatura)) || (doc != null && string.IsNullOrEmpty(doc.systemId) && doc.protocollo != null))
                            {
                                if (consolidation)
                                {
                                    this.PnlcontainerDocumentTopSx.Attributes.Add("class", "containerDocumentTopSxOrangeConsolidate");
                                }
                                else
                                {
                                    this.PnlcontainerDocumentTopSx.Attributes.Add("class", "containerDocumentTopSxOrange");
                                }
                                this.LblReferenceCode.Attributes.Remove("class");
                                this.LblReferenceCode.Attributes.Add("class", "referenceCode");
                                this.DocumentImgPrintReceipt.Visible = true;
                                this.DocumentImgPrintReceipt.Enabled = true;
                                if (doc != null && doc.protocollo == null) this.DocumentImgPrintReceipt.Enabled = false;
                            }
                            else
                            {
                                this.PnlcontainerDocumentTopSx.Attributes.Add("class", "containerDocumentTopSxOrangeWhite");
                            }

                            this.VisibiltyRoleFunctions();
                        }
                        else
                        {
                            this.PnlcontainerDocumentTopSx.Attributes.Add("class", "containerDocumentTopSxOrangeAbort");
                            this.LblReferenceCode.Attributes.Remove("class");
                            this.LblReferenceCode.Attributes.Add("class", "redStrike");
                        }
                        this.containerDocumentTopDx.Attributes.Add("class", "containerDocumentTopDxOrange");
                        this.containerDocumentTopCxOrange.Attributes.Add("class", "containerDocumentTopCxOrangeBg");
                        break;
                    case "P":
                        this.PProtocolType.InnerText = DocumentManager.GetCodeLabel("P");
                        if (protAborted == null)
                        {
                            if ((doc == null || doc.protocollo == null) || (doc != null && !string.IsNullOrEmpty(doc.systemId) && doc.protocollo != null && !string.IsNullOrEmpty(doc.protocollo.segnatura)) || (doc != null && string.IsNullOrEmpty(doc.systemId) && doc.protocollo != null && !this.IsForwarded))
                            {
                                if (consolidation)
                                {
                                    this.PnlcontainerDocumentTopSx.Attributes.Add("class", "containerDocumentTopSxGreenConsolidate");
                                }
                                else
                                {
                                    this.PnlcontainerDocumentTopSx.Attributes.Add("class", "containerDocumentTopSxGreen");
                                }
                                this.LblReferenceCode.Attributes.Remove("class");
                                this.LblReferenceCode.Attributes.Add("class", "referenceCode");
                            }
                            else
                            {
                                this.PnlcontainerDocumentTopSx.Attributes.Add("class", "containerDocumentTopSxGreenWhite");
                            }
                        }
                        else
                        {
                            this.PnlcontainerDocumentTopSx.Attributes.Add("class", "containerDocumentTopSxGreenAbort");
                            this.LblReferenceCode.Attributes.Remove("class");
                            this.LblReferenceCode.Attributes.Add("class", "redStrike");
                        }
                        this.containerDocumentTopDx.Attributes.Add("class", "containerDocumentTopDxGreen");
                        this.containerDocumentTopCxOrange.Attributes.Add("class", "containerDocumentTopCxGreenBg");
                        break;
                    case "I":
                        this.PProtocolType.InnerText = DocumentManager.GetCodeLabel("I");
                        if (protAborted == null)
                        {
                            if ((doc == null || doc.protocollo == null) || (doc != null && !string.IsNullOrEmpty(doc.systemId) && doc.protocollo != null && !string.IsNullOrEmpty(doc.protocollo.segnatura)) || (doc != null && string.IsNullOrEmpty(doc.systemId) && doc.protocollo != null))
                            {
                                if (consolidation)
                                {
                                    this.PnlcontainerDocumentTopSx.Attributes.Add("class", "containerDocumentTopSxBlueConsolidate");
                                }
                                else
                                {
                                    this.PnlcontainerDocumentTopSx.Attributes.Add("class", "containerDocumentTopSxBlue");
                                }
                                this.LblReferenceCode.Attributes.Remove("class");
                                this.LblReferenceCode.Attributes.Add("class", "referenceCode");
                            }
                            else
                            {
                                this.PnlcontainerDocumentTopSx.Attributes.Add("class", "containerDocumentTopSxBlueWhite");
                            }
                        }
                        else
                        {
                            this.PnlcontainerDocumentTopSx.Attributes.Add("class", "containerDocumentTopSxBlueAbort");
                            this.LblReferenceCode.Attributes.Remove("class");
                            this.LblReferenceCode.Attributes.Add("class", "redStrike");
                        }
                        this.containerDocumentTopDx.Attributes.Add("class", "containerDocumentTopDxBlue");
                        this.containerDocumentTopCxOrange.Attributes.Add("class", "containerDocumentTopCxBlueBg");
                        break;
                }

            }
        }

        protected void DocumentImgPrintReceipt_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_RICEVUTA_PROTOCOLLO_PDF.ToString())) && Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_RICEVUTA_PROTOCOLLO_PDF.ToString()) == "1")
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "PrintReceiptPDF", "ajaxModalPopupPrintReceiptPdf();", true);
                }
                else
                {
                    HttpContext.Current.Session["visualReportAlreadyDownloaded" + Session.SessionID] = null;
                    DocsPaWR.FileDocumento fileRep = DocumentManager.StampaRicevutaProtocolloRtf();
                    FileManager.setSelectedFileReport(this.Page, fileRep, "../popup");
                    if (fileRep != null)
                    {
                        exportDatiSessionManager session = new exportDatiSessionManager();
                        session.SetSessionExportFile(fileRep);
                    }
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }


        protected void DocumentImgPrintA4_Click(object sender, EventArgs e)
        {
            //Imposto la seguente variabile di sessione a true per risolvere un'anomalia di alcune versioni del browser, ovvero
            //alla chiusura della popup IsPostBack è false e quindi riesegue tutto il codice.
            OpenSignaturePopup = true;
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ajaxModalPopupSignature", "ajaxModalPopupSignatureA4();", true);
        }

        protected void InitializeLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.DocumentLblReferenceCodeLabel.Text = Utils.Languages.GetLabelFromCode("DocumentLblReferenceCodeLabel", language);
            this.DocumentLblRepertoryCodeLabel.Text = Utils.Languages.GetLabelFromCode("DocumentLblRepertoryCodeLabel", language);
            this.DocumentLblDocumentId.Text = Utils.Languages.GetLabelFromCode("DocumentLblDocumentId", language);
            this.DocumentLblPrintLabel.Text = Utils.Languages.GetLabelFromCode("DocumentLblPrintLabel", language);
            this.DocumentImgPrintLabel.ToolTip = Utils.Languages.GetLabelFromCode("DocumentImgPrintLabel", language);
            this.DocumentImgPreservation.AlternateText = Utils.Languages.GetLabelFromCode("DocumentImgPreservation", language);
            this.DocumentImgRecupero.AlternateText = Utils.Languages.GetLabelFromCode("DocumentImgPreservation", language);
            this.DocumentImgPreservation.ToolTip = Utils.Languages.GetLabelFromCode("DocumentImgPreservation", language);
            this.DocumentImgRecupero.ToolTip = Utils.Languages.GetLabelFromCode("DocumentImgPreservation", language);
            this.DocumentImgSendReceipt.AlternateText = Utils.Languages.GetLabelFromCode("DocumentImgSendReceipt", language);
            this.DocumentImgSendReceipt.ToolTip = Utils.Languages.GetLabelFromCode("DocumentImgSendReceipt", language);
            this.DocumentImgPrintA4.AlternateText = Utils.Languages.GetLabelFromCode("DocumentImgPrintA4", language);
            this.DocumentImgPrintA4.ToolTip = Utils.Languages.GetLabelFromCode("DocumentImgPrintA4", language);
            this.DocumentImgPrintReceipt.AlternateText = Utils.Languages.GetLabelFromCode("DocumentImgPrintReceipt", language);
            this.DocumentImgPrintReceipt.ToolTip = Utils.Languages.GetLabelFromCode("DocumentImgPrintReceipt", language);
            this.HistoryPreserved.Title = Utils.Languages.GetLabelFromCode("HistoryPreservedTitle", language);
            this.HistoryVers.Title = Utils.Languages.GetLabelFromCode("HistoryPreservedTitle", language);
            this.DocumentLitTypeDocumentHead.Text = Utils.Languages.GetLabelFromCode("DocumentLitTypeDocumentHead", language);
            this.DocumentLitRepertory.Text = Utils.Languages.GetLabelFromCode("DocumentLitRepertory", language);
            this.DocumentImgInfoProcessiAvviati.ToolTip = Utils.Languages.GetLabelFromCode("DocumentImgInfoProcessiAvviati", language);
            this.InfoSignatureProcessesStarted.Title = Utils.Languages.GetLabelFromCode("InfoSignatureProcessesStarted", language);
            //this.PrintLabel.Title = Utils.Languages.GetLabelFromCode("PrintLabelPopUpTitle", language);
        }

        public virtual void RefreshDataDocument()
        {
            SchedaDocumento doc = UIManager.DocumentManager.getSelectedRecord();

            if (doc != null && !string.IsNullOrEmpty(doc.systemId))
            {
                this.DocumentImgInfoProcessiAvviati.Enabled = true;
                this.DocumentLblDocumentId.Visible = true;
                this.LblIdDocument.Visible = true;

                this.LblIdDocument.Text = doc.systemId;
                if (doc.protocollo != null && !string.IsNullOrEmpty(doc.protocollo.segnatura))
                {
                    this.DocumentLblReferenceCodeLabel.Visible = true;
                    this.LblReferenceCode.Visible = true;
                    this.LblReferenceCode.Text = doc.protocollo.segnatura;
                    this.LblReferenceCode.ToolTip = doc.protocollo.segnatura;
                }

                if (doc.template != null)
                {
                    this.DocumentLitTypeDocumentHead.Visible = true;
                    this.DocumentLitTypeDocumentValue.Visible = true;
                    this.DocumentLitTypeDocumentValue.Text = doc.template.DESCRIZIONE;
                    string repertory = DocumentManager.getSegnaturaRepertorioNoHTML(doc.systemId, AdministrationManager.AmmGetInfoAmmCorrente(UserManager.GetInfoUser().idAmministrazione).Codice);

                    if (!string.IsNullOrEmpty(repertory))
                    {
                        OggettoCustom oggettoCustom = (from el in doc.template.ELENCO_OGGETTI where el.TIPO.DESCRIZIONE_TIPO.Equals("Contatore") && !string.IsNullOrEmpty(el.DATA_ANNULLAMENTO) select el).FirstOrDefault();

                        if (oggettoCustom != null)
                        {
                            DocumentLitRepertoryValue.Attributes.Add("class", "redStrike");
                        }

                        this.DocumentLitRepertoryValue.Visible = true;
                        this.DocumentLitRepertory.Visible = true;
                        this.DocumentLitRepertoryValue.Text = repertory;
                    }
                }

                this.UpcontainerDocumentTopDx.Update();
                this.UpcontainerDocumentTopCx.Update();

                this.EnableDocumentButtons();
            }
            else
            {
                this.DocumentLitTypeDocumentHead.Visible = false;
                this.DocumentLitTypeDocumentValue.Visible = false;
                this.DocumentLitRepertoryValue.Visible = false;
                this.DocumentLitRepertory.Visible = false;
            }
        }

        private void EnableDocumentButtons()
        {
            this.DocumentImgPrintLabel.Enabled = true;
            this.DocumentImgPreservation.Enabled = true;
            this.DocumentImgRecupero.Enabled = true;
            this.DocumentImgSendReceipt.Enabled = true;
            this.DocumentImgPrintA4.Enabled = true;
            this.TxtPrintLabel.Enabled = true;
            this.TxtPrintLabel.Text = "1";
            SchedaDocumento doc = DocumentManager.getSelectedRecord();

            if (doc != null && !string.IsNullOrEmpty(doc.systemId))
            {
                if (doc.tipoProto.Equals("A") || doc.tipoProto.Equals("P") || doc.tipoProto.Equals("I"))
                {
                    if (!UIManager.UserManager.IsAuthorizedFunctions("DO_PROT_SE_STAMPA"))
                    {
                        this.PlcDocumentPrintLabel.Visible = false;
                    }
                }

                if (doc.tipoProto.Equals("G"))
                {
                    if (!UIManager.UserManager.IsAuthorizedFunctions("DO_DOC_SE_GRIGIO"))
                    {
                        this.PlcDocumentPrintLabel.Visible = false;
                    }
                }
            }

            this.VisibiltyRoleFunctions();
        }

        public virtual void RefreshLayoutHeader()
        {
            this.SetTypeRecord();
            this.RefreshDataDocument();
            this.UpPnlButtonsHeader.Update();
            this.UpLetterTypeProtocol.Update();
            this.UpcontainerDocumentTopCx.Update();
            this.UpcontainerDocumentTopDx.Update();
        }

        protected void DocumentImgSendReceipt_Click(object sender, EventArgs e)
        {
            try
            {
                SchedaDocumento schedaDocumento = DocumentManager.getSelectedRecord();
                Registro reg = schedaDocumento.registro;
                // 4) calcolo gli RF associati al registro
                DocsPaWR.Registro[] listaRF = RegistryManager.GetListRegistriesAndRF(RoleManager.GetRoleInSession().systemId, "1", reg.systemId);
                if (listaRF != null && listaRF.Length > 0)
                {
                    // 5) nel caso di un solo RF associato al registro
                    if (listaRF.Length == 1)
                    {
                        schedaDocumento.id_rf_invio_ricevuta = listaRF[0].systemId;
                    }
                    else
                    {
                        // 6) caso di più RF associati al registro e con invio automatico
                        if (listaRF.Length > 1)
                        {
                            Session["ChooseRFSegnature_code"] = "ricev";
                            ScriptManager.RegisterStartupScript(this.UpcontainerDocumentTopCx, this.UpcontainerDocumentTopCx.GetType(), "ChooseRFSegnature", "ajaxModalPopupChooseRFSegnature();", true);
                            this.UpcontainerDocumentTopCx.Update();
                            return;
                        }
                    }
                }

                if (!string.IsNullOrEmpty(schedaDocumento.id_rf_invio_ricevuta))
                    reg = UserManager.getRegistroBySistemId(this.Page, schedaDocumento.id_rf_invio_ricevuta);

                bool resInvioRicevuta = DocumentManager.DocumentoInvioRicevuta(Page, schedaDocumento, reg);
                if (resInvioRicevuta)
                {
                    string msg = "InfoSendReceiptCorrectly";
                    ScriptManager.RegisterStartupScript(this.UpcontainerDocumentTopCx, this.UpcontainerDocumentTopCx.GetType(), "ajaxDialogModal", "ajaxDialogModal('" + msg.Replace("'", "\\'") + "', 'check');", true);
                    this.UpcontainerDocumentTopCx.Update();
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void DocumentImgPrintLabel_Click(object o, EventArgs e)
        {
            try
            {
                this.EnumLabel = this.TxtPrintLabel.Text;
                PrintLabel_alreadyPrinted = false;
                PrintLabel_alreadyPrinted2 = false;
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "print_labels", "ajaxModalPopupPrintLabel();", true);
                
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private void ReApplyPermitOnlyNumbersScript()
        {
            ScriptManager.RegisterClientScriptBlock(this.UpcontainerDocumentTopCx, this.UpcontainerDocumentTopCx.GetType(), "permit_onlynumbers", "permitOnlyNumbers('TxtPrintLabel');", true);
        }

        #region Session Utility
        //***************************************************************
        //GIORDANO IACOZZILLI
        //17/07/2013
        //Gestione dell'icona della copia del docuemnto/fascicolo in deposito.
        //***************************************************************
        [Browsable(true)]
        public string FlagCopyInArchive
        {
            get
            {
                string result = string.Empty;
                if (HttpContext.Current.Session["FlagCopyInArchive"] != null)
                {
                    result = HttpContext.Current.Session["FlagCopyInArchive"].ToString();
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["FlagCopyInArchive"] = value;
            }
        }
        //***************************************************************
        //FINE
        //***************************************************************

        [Browsable(true)]
        public string TypeDocument
        {
            get
            {
                string result = string.Empty;
                if (HttpContext.Current.Session["typeDocument"] != null)
                {
                    result = HttpContext.Current.Session["typeDocument"].ToString();
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["typeDocument"] = value;
            }
        }

        [Browsable(true)]
        public string TypeRecord
        {
            get
            {
                string result = string.Empty;
                if (HttpContext.Current.Session["typeRecord"] != null)
                {
                    result = HttpContext.Current.Session["typeRecord"].ToString();
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["typeRecord"] = value;
            }
        }

        private bool IsForwarded
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["IsForwarded"] != null) result = (bool)HttpContext.Current.Session["IsForwarded"];
                return result;

            }
            set
            {
                HttpContext.Current.Session["IsForwarded"] = value;
            }
        }

        public string EnumLabel
        {
            get
            {
                string result = string.Empty;
                if (HttpContext.Current.Session["enumLabel"] != null)
                {
                    result = HttpContext.Current.Session["enumLabel"].ToString();
                }
                return result;

            }
            set
            {
                HttpContext.Current.Session["enumLabel"] = value;
            }
        }

        public bool PrintLabel_alreadyPrinted
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["printlabel_alreadyprinted"] != null)
                {
                    result = Boolean.Parse(HttpContext.Current.Session["printlabel_alreadyprinted"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["printlabel_alreadyprinted"] = value.ToString();
            }
        }

        public bool PrintLabel_alreadyPrinted2
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["printlabel_alreadyprinted2"] != null)
                {
                    result = Boolean.Parse(HttpContext.Current.Session["printlabel_alreadyprinted2"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["printlabel_alreadyprinted2"] = value.ToString();
            }
        }

        #endregion

    }
}
