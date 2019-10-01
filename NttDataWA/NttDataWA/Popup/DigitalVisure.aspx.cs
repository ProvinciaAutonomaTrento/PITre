using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.Utils;
using NttDataWA.DocsPaWR;
using NttDataWA.UIManager;

namespace NttDataWA.Popup
{
    public partial class DigitalVisure : System.Web.UI.Page
    {
        private string language;
        private FileRequest fileReq = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            //if (!this.IsPostBack)
            //{
                //this.InitSessionVal();
                this.initLabel();
                this.loadCheckOutData();
            //}
        }

        private void initLabel()
        {
            this.language = NttDataWA.UIManager.UserManager.GetUserLanguage();

            this.lblMessage.Text = Utils.Languages.GetLabelFromCode("lblDigitalVisureMessage", language);
            //this.lblTitNomeFile.Text = Utils.Languages.GetLabelFromCode("lblDigitalVisureTitNomeFile", language);
            //this.lblTitOggetto.Text = Utils.Languages.GetLabelFromCode("lblDigitalVisureTitOggetto", language);
            this.ConfirmButton.Text = Utils.Languages.GetLabelFromCode("btnDigitalVisureConfirmButton", language);
            this.UndoButton.Text = Utils.Languages.GetLabelFromCode("btnDigitalVisureUndoButton", language);
            //this.lblOggetto.Text = Utils.Languages.GetLabelFromCode("lblDigitalVisureOggetto", language);
            //this.lblNomeFile.Text = Utils.Languages.GetLabelFromCode("lblDigitalVisureNomeFile", language);
        }

        private void loadCheckOutData()
        {
            string strNomeFile = string.Empty;
            string strOggetto = string.Empty;

            CheckInOutApplet.CheckInOutServices.InitializeContext();

            NttDataWA.DocsPaWR.SchedaDocumento schedaDoc = CheckInOutApplet.CheckInOutServices.CurrentSchedaDocumento;
            //FileRequest fileReq = null;
            fileReq = null;
            if (UIManager.DocumentManager.getSelectedAttachId() == null)
            {
                if (schedaDoc != null)
                {
                    fileReq = schedaDoc.documenti[0];
                    //strOggetto = schedaDoc.oggetto.descrizione;
                    //strNomeFile = schedaDoc.documenti[0].path;
                }
            }
            else
            {
                if (UIManager.DocumentManager.GetSelectedAttachment() != null)
                {
                    fileReq = UIManager.DocumentManager.GetSelectedAttachment();
                    //Allegato allegato = UIManager.DocumentManager.GetSelectedAttachment();
                    //strOggetto = allegato.descrizione;
                    //strNomeFile = allegato.fileName;
                }
            }
            string details = Utils.Languages.GetMessageFromCode("DigitalVisurelblMessageDetails", language);
            details = details.Replace("@@", fileReq.version);

            if (fileReq.GetType().Equals(typeof(DocsPaWR.Documento)))
            {
                details = details.Replace("##", Utils.Languages.GetLabelFromCode("DigitalVisureLblMainDocument", UserManager.GetUserLanguage()));
            }
            else
            {
                details = details.Replace("##", Utils.Languages.GetLabelFromCode("DigitalVisureLblAttachment", UserManager.GetUserLanguage())).Replace("@@", fileReq.versionLabel);
            }
            string extensionFile = (fileReq.fileName.Split('.').Length > 1) ? (fileReq.fileName.Split('.'))[fileReq.fileName.Split('.').Length - 1] : string.Empty;
            details = details.Replace("@#", extensionFile);

            FileDocumento doc = FileManager.getInstance(schedaDoc.systemId).getInfoFile(this.Page, fileReq);
            if (doc != null && !string.IsNullOrEmpty(doc.nomeOriginale))
            {
                details = details.Replace("#@", doc.nomeOriginale);
            }

            this.lblMessageDetails.Text = details;

            //this.lblOggetto.Text = strOggetto;
            //this.lblNomeFile.Text = strNomeFile;
        }

        protected List<MassiveOperationTarget> GetSelectedDocuments()
        {
            // commentato per forzare l'istanziamento di una nuova lista altrimenti se si arriva dalla firma elettronica massiva
            // rimangono in sessione i documenti lì selezionati
            //List<MassiveOperationTarget> selectedItems = MassiveOperationUtils.GetSelectedItems();
            List<MassiveOperationTarget> selectedItems = new List<MassiveOperationTarget>();

            if (selectedItems.Count == 0)
            {
                DocsPaWR.SchedaDocumento schedaDocumento = NttDataWA.UIManager.DocumentManager.getSelectedRecord();

                if (schedaDocumento != null)
                {
                    fileReq = new FileRequest();
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
                    string codice = string.Empty;
                    MassiveOperationTarget mot;
                    if (fileReq == null || fileReq.docNumber.Equals(schedaDocumento.docNumber))
                    {
                        codice = (schedaDocumento.protocollo != null) ? schedaDocumento.protocollo.numero : schedaDocumento.docNumber;
                        mot = new MassiveOperationTarget(schedaDocumento.systemId, codice);

                        fileReq = schedaDocumento.documenti[0];
                    }
                    else
                    {
                        codice = fileReq.docNumber;
                        mot = new MassiveOperationTarget(codice, codice);
                    }
                    mot.Checked = true;
                    selectedItems.Add(mot);
                }
            }

            return selectedItems;
        }

        private void InitSessionVal()
        {
            if (System.Web.HttpContext.Current.Session["fileInAccettazione"] != null)
                System.Web.HttpContext.Current.Session["fileInAccettazione"] = null;
        }

        private void setSessionVal()
        {
            System.Web.HttpContext.Current.Session["fileInAccettazione"] = fileReq;
        }

        protected void UndoButton_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterClientScriptBlock(this.UpUpdateButtons, this.UpUpdateButtons.GetType(), "closeAJM", "parent.closeAjaxModal('DigitalVisureSelector','');", true);
        }

        protected void ConfirmButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue(UIManager.UserManager.GetInfoUser().idAmministrazione, DBKeys.FE_REQ_CONV_PDF.ToString())) || !Utils.InitConfigurationKeys.GetValue(UIManager.UserManager.GetInfoUser().idAmministrazione, DBKeys.FE_REQ_CONV_PDF.ToString()).Equals("1"))
            {
                if (!this.IsFormatSupportedForSign(fileReq))
                {
                    string msgDesc = "WarningDigitalVisureFileNonAmmessoAllaFirma";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + utils.FormatJs(msgDesc) + "', 'warning', '', '" + utils.FormatJs(msgDesc) + "');} else {parent.ajaxDialogModal('" + utils.FormatJs(msgDesc) + "', 'warning', '', '" + utils.FormatJs(msgDesc) + "');}", true);
                    return;
                }
            }
            setSessionVal();
            ScriptManager.RegisterClientScriptBlock(this.UpUpdateButtons, this.UpUpdateButtons.GetType(), "closeAJM", "parent.closeAjaxModal('DigitalVisureSelector','true');", true);
            return;
        }

        private bool IsFormatSupportedForSign(DocsPaWR.FileRequest fileRequest)
        {
            bool retValue = false;

            if (!NttDataWA.FormatiDocumento.Configurations.SupportedFileTypesEnabled)
            {
                retValue = true;
            }
            else
            {
                string extension = System.IO.Path.GetExtension(fileRequest.fileName);

                if (!string.IsNullOrEmpty(extension))
                {
                    // Rimozione del primo carattere dell'estensione (punto)
                    extension = extension.Substring(1);

                    DocsPaWR.SupportedFileType[] fileTypes = ProxyManager.GetWS().GetSupportedFileTypes(Convert.ToInt32(UIManager.UserManager.GetInfoUser().idAmministrazione));

                    retValue = fileTypes.Count(e => e.FileExtension.ToLower() == extension.ToLower() &&
                                                e.FileTypeUsed && e.FileTypeSignature) > 0;
                }
            }

            return retValue;
        }
    }
}