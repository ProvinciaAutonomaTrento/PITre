using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.UIManager;
using NttDataWA.DocsPaWR;

namespace NttDataWA.Popup
{
    public partial class InformationFile : System.Web.UI.Page
    {

        #region Global variable
            private static FileRequest fileReq;
        #endregion

        #region Const

            private static string CHECK;
            private const string NO_CHECK = "NO";
            private const string IMAGE_CHECK = "../Images/Icons/identity_valid.png";
            private const string IMAGE_NO_CHECK = "../Images/Icons/identity_not_valid.png";

        #endregion

        #region Standard method
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                InitializeLanguage();
                InitializeContent();
            }
        }

        private void InitializeLanguage()
        {
            string language = UserManager.GetUserLanguage();
            this.BtnInformationFileClose.Text = Utils.Languages.GetLabelFromCode("BtnInformationFileClose", language);
            this.lblResultSearch.Text = Utils.Languages.GetLabelFromCode("InformationFileLblResultSearch", language);
            this.ltCheckPresenceMacro.Text = Utils.Languages.GetLabelFromCode("InformationFileLtCheckPresenceMacro", language);
            this.ltFormatCompliantToTheExtension.Text = Utils.Languages.GetLabelFromCode("InformationFileLtFormatCompliantToTheExtension", language);
            this.ltCheckSigned.Text = Utils.Languages.GetLabelFromCode("InformationFileLtCheckSigned", language);
            this.ltCheckCRL.Text = Utils.Languages.GetLabelFromCode("InformationFileLtCheckCRL", language);
            this.ltCheckTimestamp.Text = Utils.Languages.GetLabelFromCode("InformationFileLtCheckTimestamp", language);
            this.ltVersionPdf.Text = Utils.Languages.GetLabelFromCode("InformationFileLtVersionPdf", language);
            this.lblContainerResultRoles.Text = Utils.Languages.GetLabelFromCode("InformationFileLblContainerResultRoles", language);
            CHECK = Utils.Languages.GetLabelFromCode("InformationFileCheck", language);
            this.ltFormat.Text = Utils.Languages.GetLabelFromCode("InformationFileFormat", language);
            this.ltAdmittedToTheSigned.Text = Utils.Languages.GetLabelFromCode("InformationFileAdmittedToTheSigned", language);
            this.ltAdmittedToTheConservation.Text = Utils.Languages.GetLabelFromCode("InformationFileAdmittedToTheConservation", language);
            this.ltFilename.Text = Utils.Languages.GetLabelFromCode("InformationFileFileName", language);
            this.ltDateAcquiredFormatDetails.Text = Utils.Languages.GetLabelFromCode("InformationFileDateAcquiredFormatDetails", language);
            this.ltDateCheckUltimate.Text = Utils.Languages.GetLabelFromCode("InformationFileDateCheckUltimate", language);
            this.ltlCheckElectronicSignature.Text = Utils.Languages.GetLabelFromCode("InformationFileCheckElectronicSignature", language);
        }

        private void InitializeContent()
        {
            if (UIManager.DocumentManager.getSelectedAttachId() != null)
            {
                fileReq = FileManager.GetFileRequest(UIManager.DocumentManager.getSelectedAttachId());
            }
            else
            {
                fileReq = FileManager.GetFileRequest();
            }
            if (DocumentManager.getSelectedNumberVersion() != null && DocumentManager.ListDocVersions != null)
            {
                fileReq = (from v in DocumentManager.ListDocVersions where v.version.Equals(DocumentManager.getSelectedNumberVersion()) select v).FirstOrDefault();
            }
            FileInformation fileInformation = DocumentManager.GetFileInformation(fileReq, UserManager.GetInfoUser());
            InitializeDetailsFormat(fileInformation);
            InitializeDetailsCheck(fileInformation);
            CheckElectronicSignature(fileReq);
        }

        /// <summary>
        /// Popola il pannello relativo alle Informazioni del formato definite in amministrazione:
        /// </summary>
        /// <param name="fileInformation"></param>
        private void InitializeDetailsFormat(FileInformation fileInformation)
        {
            if (fileInformation!= null)
            {
                string typeFileReq = FileManager.getEstensioneIntoSignedFile(fileReq.fileName).ToUpper();
                this.lblFormatText.Text = FileManager.getEstensioneIntoSignedFile(fileReq.fileName).ToUpper();
                if (!fileInformation.AdminRefDate.Equals(DateTime.MinValue))
                {
                    this.lblDateAcquiredFormatDetailsText.Text = Utils.utils.formatDataDocsPa(fileInformation.AdminRefDate);
                }
                this.lblAdmittedToTheSignedText.Text = GetCheckDetails(fileInformation.Signable);
                this.lblAdmittedToTheConservationText.Text = GetCheckDetails(fileInformation.Preservable);
            }
        }

        /// <summary>
        /// Popola il pannello relativo ai risultati della verifica
        /// </summary>
        /// <param name="fileInformation"></param>
        private void InitializeDetailsCheck(FileInformation fileInformation)
        {
            string language = UserManager.GetUserLanguage();

            //Verifica presenza Macro e codice eseguibile
            if (fileInformation.NoMacroOrExe.Equals(VerifyStatus.Invalid) || fileInformation.NoMacroOrExe.Equals(VerifyStatus.Valid))
            {
                this.lblCheckPresenceMacro.Text = Utils.Languages.GetLabelFromCode("NoMacro" + fileInformation.NoMacroOrExe.ToString(), language);
                if (fileInformation.NoMacroOrExe.Equals(VerifyStatus.Invalid))
                    this.imgCheckPresenceMacroResult.ImageUrl = IMAGE_NO_CHECK;
                else
                    this.imgCheckPresenceMacroResult.ImageUrl = IMAGE_CHECK;
            }
            else
            {
                this.lblCheckPresenceMacro.Text = Utils.Languages.GetLabelFromCode(fileInformation.NoMacroOrExe.ToString(), language);
                this.imgCheckPresenceMacroResult.Visible = false;
            }               

            //Data della verifica
            if (!fileInformation.CrlRefDate.Equals(DateTime.MinValue))
            {
                this.lblDateCheckUltimateText.Text = fileInformation.CrlRefDate.ToShortDateString();
            }

            //Formato conforme all'estensione
            this.lblFormatCompliantToTheExtension.Text = Utils.Languages.GetLabelFromCode(fileInformation.FileFormatOK.ToString(), language);
            if (!string.IsNullOrEmpty(GetImageCheckDetails(fileInformation.FileFormatOK)))
                this.imgFormatCompliantToTheExtensionResult.ImageUrl = GetImageCheckDetails(fileInformation.FileFormatOK);
            else
                this.imgFormatCompliantToTheExtensionResult.Visible = false;
        
            //Verifica firma
            this.lblCheckSigned.Text = Utils.Languages.GetLabelFromCode(fileInformation.Signature.ToString(), language);
            if (!string.IsNullOrEmpty(GetImageCheckDetails(fileInformation.Signature)))
                this.imgCheckSignedResult.ImageUrl = GetImageCheckDetails(fileInformation.Signature);
            else
                this.imgCheckSignedResult.Visible = false;

            //Verifica CRL
            this.lblCRL.Text = Utils.Languages.GetLabelFromCode(fileInformation.CrlStatus.ToString(), language);
            if (!string.IsNullOrEmpty(GetImageCheckDetails(fileInformation.CrlStatus)))
                this.imgCheckCRLResult.ImageUrl = GetImageCheckDetails(fileInformation.CrlStatus);
            else
                this.imgCheckCRLResult.Visible = false;

            //Se il file è firmato sia CADES che PADES, la verifica non è applicabile.
            bool isCadesAndPades = this.CheckIsPresentCadesAndPades();
            if (isCadesAndPades)
            {
                this.lblCheckSigned.Text = Utils.Languages.GetLabelFromCode("DigitalSignDetailsNonApplicabile", language);
                this.imgCheckSignedResult.ImageUrl = string.Empty;
                this.lblCRL.Text = Utils.Languages.GetLabelFromCode("DigitalSignDetailsNonApplicabile", language);
                this.imgCheckCRLResult.ImageUrl = string.Empty;
            }

            //Verifica timestamp
            this.lblCheckTimestamp.Text = Utils.Languages.GetLabelFromCode(fileInformation.TimeStampStatus.ToString(), language);
            if (!string.IsNullOrEmpty(GetImageCheckDetails(fileInformation.TimeStampStatus)))
                this.imgCheckTimestampResult.ImageUrl = GetImageCheckDetails(fileInformation.TimeStampStatus);
            else
                this.imgCheckTimestampResult.Visible = false;

            //Verifica versione del PDF
            if (!string.IsNullOrEmpty(fileInformation.PdfVer))
            {
                this.panelVersionPdf.Visible = true;
                this.lblVersionPdf.Text = fileInformation.PdfVer;
            }
            else
            {
                this.panelVersionPdf.Visible = false;
            }

            //Nome del File originale
            SchedaDocumento selectedRecord = DocumentManager.getSelectedRecord();
            FileDocumento doc = FileManager.getInstance(selectedRecord.systemId).getInfoFile(this.Page, fileReq);
            if (string.IsNullOrEmpty(doc.nomeOriginale))
            {
                this.lblFilenameText.Text = GetFinalFileName();
            }
            else
            {
                this.lblFilenameText.Text = doc.nomeOriginale;
            }
         }

        private string GetCheckDetails(VerifyStatus status)
        {
            string result = string.Empty;
            if (status.Equals(VerifyStatus.Valid))
                result = CHECK;
            else
                if (status.Equals(VerifyStatus.Invalid))
                    result = NO_CHECK;
            return result;
        }

        private string GetImageCheckDetails(VerifyStatus status)
        {
            string result = string.Empty;
            if (status.Equals(VerifyStatus.Valid))
                result = IMAGE_CHECK;
            else
                if (status.Equals(VerifyStatus.Invalid))
                    result = IMAGE_NO_CHECK;
            return result;
        }

        private string GetFinalFileName()
        {
            string retVal = string.Empty;
            string pathAndName = fileReq.fileName;
            if (!string.IsNullOrEmpty(pathAndName))
            {
                string[] arrayString = pathAndName.Split('\\');
                if (arrayString.Length > 0)
                    retVal = arrayString[arrayString.Length - 1];
            }

            return retVal;
        }

        private void CheckElectronicSignature(FileRequest fileReq)
        {
            bool isSigned = DocumentManager.IsElectronicallySigned(fileReq.docNumber, fileReq.versionId);
            if (isSigned)
            {
                this.lblCheckElectronicSignature.Text = Utils.Languages.GetLabelFromCode("InformationFileIsElectronicallySigned", UserManager.GetUserLanguage());
            }
            else
            {
                this.lblCheckElectronicSignature.Text = Utils.Languages.GetLabelFromCode("InformationFileIsNotElectronicallySigned", UserManager.GetUserLanguage());            
            }

        }

        #region VERIFICA FILE CADES E PADES
        private bool CheckIsPresentCadesAndPades()
        {
            bool result = false;
            FileDocumento signedDocument = this.GetSignedDocument();
            bool cades = false;
            bool pades = false;
            if (signedDocument != null && signedDocument.signatureResult != null)
            {
                VerifySignatureResult signatureResult = signedDocument.signatureResult;
                foreach (PKCS7Document document in signatureResult.PKCS7Documents)
                {
                    if (document.SignatureType == SignType.PADES)
                        pades = true;
                    if (document.SignatureType == SignType.CADES)
                        cades = true;
                    if (cades && pades)
                        break;
                }
            }
            result = cades && pades;
            return result;
        }

        private FileDocumento GetSignedDocument()
        {
            DocsPaWR.FileRequest fileRequest = null;
            if (DocumentManager.getSelectedAttachId() != null) // ho aggiunto il file ad un allegato
            {
                fileRequest = FileManager.GetFileRequest(DocumentManager.getSelectedAttachId());
            }
            else // ho aggiunto il file al documento principale
            {
                fileRequest = FileManager.getSelectedFile();
            }
            //Aggiunto per le versioni
            if (!IsForwarded)
            {
                if (DocumentManager.getSelectedNumberVersion() != null && DocumentManager.ListDocVersions != null)
                {
                    fileRequest = (from v in DocumentManager.ListDocVersions where v.version.Equals(DocumentManager.getSelectedNumberVersion()) select v).FirstOrDefault();
                }
            }
            DocsPaWR.FileDocumento signedDocument = null;
            if (fileRequest != null && fileRequest.fileName != null && fileRequest.fileName != "")
            {
                signedDocument = this.DocumentoGetFileCached(fileRequest);
            }

            return signedDocument;
        }

        //meccanismo di caching per evitare di fare la getFile tutte le volte, con conseguente controllo del certificato
        private FileDocumento DocumentoGetFileCached(DocsPaWR.FileRequest fileRequest)
        {
            FileDocumento retval = null;
            if (HttpContext.Current.Session["FileRequest_Cached"] == null)
                HttpContext.Current.Session["FileDocumento_Cached"] = null;

            if (HttpContext.Current.Session["FileDocumento_Cached"] == null)
            {
                retval = DocumentManager.DocumentoGetFile(fileRequest);
                HttpContext.Current.Session["FileRequest_Cached"] = fileRequest;
                HttpContext.Current.Session["FileDocumento_Cached"] = retval;
            }
            else
            {
                if (HttpContext.Current.Session["FileRequest_Cached"] != fileRequest)
                {
                    //filerequest è cambiato
                    retval = DocumentManager.DocumentoGetFile(fileRequest);
                    HttpContext.Current.Session["FileRequest_Cached"] = fileRequest;
                    HttpContext.Current.Session["FileDocumento_Cached"] = retval;

                }
                else
                {
                    retval = (FileDocumento)HttpContext.Current.Session["FileDocumento_Cached"];
                }
            }
            return retval;
        }

        private bool IsForwarded
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["IsForwarded"] != null) result = (bool)HttpContext.Current.Session["IsForwarded"];
                return result;

            }
        }

        #endregion
        #endregion

        #region Event

        protected void BtnInformationFileClose_Click(object sender, EventArgs e)
        {
            try
            {
                ScriptManager.RegisterClientScriptBlock(this.UpPnlButtons, this.UpPnlButtons.GetType(), "closeAJM", "parent.closeAjaxModal('InformationFile','');", true);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        #endregion
    }
}