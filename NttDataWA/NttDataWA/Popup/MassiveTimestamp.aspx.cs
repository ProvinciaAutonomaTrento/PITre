using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.DocsPaWR;
using NttDataWA.UIManager;
using NttDataWA.Utils;
using NttDatalLibrary;

namespace NttDataWA.Popup
{
    public partial class MassiveTimestamp : System.Web.UI.Page
    {

        #region Properties

        private bool IsFasc
        {
            get
            {
                return Request.QueryString["objType"].Equals("D") ? false : true;
            }
        }

        protected Dictionary<String, String> ListCheck
        {
            get
            {
                Dictionary<String, String> result = null;
                if (HttpContext.Current.Session["listCheck"] != null)
                {
                    result = HttpContext.Current.Session["listCheck"] as Dictionary<String, String>;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["listCheck"] = value;
            }
        }

        #endregion

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.InitializePage();
            }
            else
            {
                this.ReadRetValueFromPopup();
            }
            this.RefreshScript();
        }

        protected void BtnClose_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "reallowOp", "reallowOp();", true);
            this.CloseMask(true);
        }

        protected void BtnConfirm_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "reallowOp", "reallowOp();", true);

            // Il report da visualizzare
            MassiveOperationReport report;

            // Inizializzazione del report
            report = new MassiveOperationReport();

            // Esecuzione dell'applicazione del timestamp
            this.ApplyTimeStampToDocuments(MassiveOperationUtils.GetSelectedItems(), report);

            // Introduzione della riga di summary
            string[] pars = new string[] { "" + report.Worked, "" + report.NotWorked };
            report.AddSummaryRow("Documenti lavorati: {0} - Documenti non lavorati: {1}", pars);

            // Generazione del report da esportare
            this.generateReport(report, "Timestamp massivo");
        }

        protected void BtnReport_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "reallowOp", "reallowOp();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "visualReport", "parent.ajaxModalPopupMassiveReport();", true);
        }

        #endregion

        #region Methods

        private void ReadRetValueFromPopup()
        {
            if (!string.IsNullOrEmpty(this.MassiveReport.ReturnValue))
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('MassiveReport','');", true);
            }
        }

        protected void InitializePage()
        {
            this.InitializeLanguage();
            this.InitializeList();

            this.BtnReport.Visible = false;
        }

        private void InitializeLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.BtnConfirm.Text = Utils.Languages.GetLabelFromCode("MassiveAddAdlUserBtnConfirm", language);
            this.BtnClose.Text = Utils.Languages.GetLabelFromCode("MassiveAddAdlUserBtnClose", language);
            this.BtnReport.Text = Utils.Languages.GetLabelFromCode("MassiveAddAdlUserBtnReport", language);
            this.MassiveReport.Title = Utils.Languages.GetLabelFromCode("MassiveAddAdlUserBtnReport", language);
            this.litMessage.Text = Utils.Languages.GetLabelFromCode("MassiveAddAdlUserAskConfirm", language);
            this.grdReport.Columns[1].HeaderText = Utils.Languages.GetLabelFromCode("MassiveAddAdlUserGridResult", language);
            this.grdReport.Columns[2].HeaderText = Utils.Languages.GetLabelFromCode("MassiveAddAdlUserGridDetails", language);
        }

        public void InitializeList()
        {
            Dictionary<String, MassiveOperationTarget> temp = new Dictionary<string, MassiveOperationTarget>();

            // Inizializzazione della mappa con i system id degli oggetti e lo stato
            // di checking (in fase di inizializzazione tutti gli item sono deselezionati)
            foreach (KeyValuePair<string, string> item in this.ListCheck)
                if (!temp.Keys.Contains(item.Key))
                    temp.Add(item.Key, new MassiveOperationTarget(item.Key, item.Value));

            // Salvataggio del dizionario
            MassiveOperationUtils.ItemsStatus = temp;
        }

        private void RefreshScript()
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshTipsy", "tooltipTipsy();", true);
        }

        private void CloseMask(bool withReturnValue)
        {
            string retValue = withReturnValue ? "true" : "false";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "closeAjaxModal", "parent.closeAjaxModal('MassiveTimestamp', '" + retValue + "');", true);
        }

        protected void generateReport(MassiveOperationReport report, string titolo)
        {
            this.generateReport(report, titolo, IsFasc);
        }

        public void generateReport(MassiveOperationReport report, string titolo, bool isFasc)
        {
            this.grdReport.DataSource = report.GetDataSet();
            this.grdReport.DataBind();
            this.pnlReport.Visible = true;
            this.upReport.Update();

            string template = (isFasc) ? "../xml/massiveOp_formatPdfExport_fasc.xml" : "../xml/massiveOp_formatPdfExport.xml";
            report.GenerateDataSetForExport(Server.MapPath(template), titolo);

            this.plcMessage.Visible = false;
            this.UpPnlMessage.Update();

            this.BtnConfirm.Enabled = false;
            this.BtnReport.Visible = true;
            this.UpPnlButtons.Update();
        }

        /// <summary>
        /// Funzione per l'applicazione del timestamp a tutti i documenti selezionati
        /// </summary>
        /// <param name="selectedItem">Lista dei system id dei documenti selezionati</param>
        /// <param name="report">Report di esecuzione</param>
        private void ApplyTimeStampToDocuments(List<MassiveOperationTarget> selectedItem, MassiveOperationReport report)
        {
            // Lista delle informazioni di base sul documento
            // contenete il file a cui aggiungere il timestamp
            BaseInfoDoc[] baseInfoDocs = null;

            // Le informazioni sul documento di interesse
            BaseInfoDoc tmpDoc;

            // Il file a cui applicare il timestamp
            string convertedFile;

            // Messaggio da aggiungere al report
            string message;

            // Risultato dell'applicazione del timestamp ad un documento
            MassiveOperationReport.MassiveOperationResultEnum result;


            // Per ogni documento a cui bisogna applicare il timestamp...
            foreach (MassiveOperationTarget temp in selectedItem)
            {
                // ...inizializzazione del messaggio e dell'esito
                message = String.Empty;
                result = MassiveOperationReport.MassiveOperationResultEnum.OK;
                string accessRight = DocumentManager.getAccessRightDocBySystemID(temp.Id, UserManager.GetInfoUser());
                if (accessRight.Equals("20"))
                {
                    message = "Il documento è in attesa di accettazione, quindi non può essere effettuato il Timestamp";
                    result = MassiveOperationReport.MassiveOperationResultEnum.KO;
                }

                // ...recupero del contenuto delle informazioni di base sul documento
                if (result == MassiveOperationReport.MassiveOperationResultEnum.OK)
                {
                    try
                    {
                        SchedaDocumento sd = DocumentManager.getDocumentDetails(this, temp.Id, temp.Id);
                        if (accessRight.Equals("20"))
                        {
                            message = "Il documento è in attesa di accettazione, quindi non può essere effettuato il Timestamp";
                            result = MassiveOperationReport.MassiveOperationResultEnum.KO;
                        }
                        else if (sd.protocollo != null && sd.protocollo.protocolloAnnullato != null)
                        {
                            message = "Il protocollo è annullato, quindi non può essere effettuato il Timestamp";
                            result = MassiveOperationReport.MassiveOperationResultEnum.KO;
                        }
                        else
                        {
                            baseInfoDocs = DocumentManager.GetBaseInfoForDocument(
                                temp.Id,
                                String.Empty,
                                String.Empty);
                        }
                    }
                    catch (Exception e)
                    {
                        message = "Errore durante il recupero dei dati sul documento.";
                        result = MassiveOperationReport.MassiveOperationResultEnum.KO;

                        return;
                    }

                    // ...se baseInfoDocs non contiene elementi o non contiene un documento
                    // con idProfile ugual a idProfile -> errore
                    if (result == MassiveOperationReport.MassiveOperationResultEnum.OK)
                    {
                        if (baseInfoDocs != null
                            && baseInfoDocs.Where(e => e.IdProfile.Equals(temp.Id)).Count() == 1
                            )
                        {
                            try
                            {
                                // Recupero delle informazioni sul documento
                                tmpDoc = baseInfoDocs.Where(e => e.IdProfile.Equals(temp.Id)).FirstOrDefault();

                                // Recupero del contenuto del file a cui applicare il timestamp
                                convertedFile = this.GetFileForDocument(tmpDoc);

                                // Recupero del file e applicazione del time stamp
                                message = this.ApplyTimeStampToDocument(
                                    convertedFile,
                                    tmpDoc.DocNumber,
                                    tmpDoc.VersionId);
                            }
                            catch (Exception e)
                            {
                                message = e.Message;
                                result = MassiveOperationReport.MassiveOperationResultEnum.KO;
                            }
                        }
                        else
                        {
                            message = "Errore durante il recupero dei dati sul documento .";
                            result = MassiveOperationReport.MassiveOperationResultEnum.KO;
                        }
                    }
                }
                // Aggiunta della riga al report
                report.AddReportRow(
                    temp.Codice,
                    result,
                    message);

            }
        }

        /// <summary>
        /// Funzione per il recupero e la conversione del file a cui applicare il timestamp
        /// </summary>
        /// <param name="baseInfoDocs">Le informazioni di base sul documento</param>
        /// <returns>Il contenuto del file convertito</returns>
        private string GetFileForDocument(BaseInfoDoc baseInfoDocs)
        {
            // Il contenuto del file
            byte[] fileContent;

            // Il risultato dell'elaborazione
            string toReturn;

            // Se il documento non ha un file associato, viene lanciata
            // un'eccezione
            if (!baseInfoDocs.HaveFile)
                throw new Exception("Il documento non ha un file associato");

            // Se il documento non è reperibile, viene sollevata un'eccezione
            //if (!File.Exists(baseInfoDocs.FileName))
            //    throw new Exception("Errore durante il reperimento del file");

            // Recupero del contenuto del file
            try
            {
                /*
   fileContent = FileManager.getInstance(Session.SessionID).GetFile(
       this,
       baseInfoDocs.VersionId,
       baseInfoDocs.VersionNumber.ToString(),
       baseInfoDocs.DocNumber,
      baseInfoDocs.Path,
       baseInfoDocs.FileName,
       false).content;
 */
                FileRequest fileRequest = new FileRequest
                {
                    versionId = baseInfoDocs.VersionId,
                    version = baseInfoDocs.VersionNumber.ToString(),
                    docNumber = baseInfoDocs.DocNumber,
                    path = baseInfoDocs.Path,
                    fileName = baseInfoDocs.FileName
                };

                fileContent = FileManager.getInstance(Session.SessionID).GetFile(this, fileRequest, false, true).content;

            }
            catch (Exception e)
            {
                throw new Exception("Errore durante il reperimento del file associato al documento.");
            }

            // Conversione del file
            toReturn = BitConverter.ToString(fileContent);
            toReturn = toReturn.Replace("-", "");

            // Restituzione del risultato dell'operazione
            return toReturn;
        }

        /// <summary>
        /// Applicazione del time stamp ad un documento
        /// </summary>
        /// <param name="convertedFile">Contenuto del file a cui applicare il timestamp</param>
        /// <param name="docNumber">Doc number del documento a cui applicare il timestamp</param>
        /// <param name="versionId">Id della versione del documento a cui applicare il timestamp</param>
        /// <returns>Esito dell'applicazione del timestamp</returns>
        private string ApplyTimeStampToDocument(string convertedFile, string docNumber, string versionId)
        {
            // L'oggetto di input marca
            InputMarca inputMarca;

            // Informazioni sull'utente
            InfoUtente userInfo;

            // Il file request
            FileRequest fileRequest;

            // La stringa descrittiva del risultato
            string toReturn = String.Empty;

            // Recupero delle informazioni sull'utente
            userInfo = UserManager.GetInfoUser();

            // Impostazione di doc number e version id nel file request
            fileRequest = new FileRequest();
            fileRequest.docNumber = docNumber;
            fileRequest.versionId = versionId;


            // Creazione e inizializzazione dell'oggetto per l'input marca
            inputMarca = new DocsPaWR.InputMarca();
            inputMarca.applicazione = userInfo.urlWA;
            inputMarca.file_p7m = convertedFile;
            inputMarca.riferimento = userInfo.userId;

            // Applicazione del time stamp
            try
            {
                DocumentManager.ApplyTimeStampAM(
                    userInfo,
                    inputMarca,
                    fileRequest,
                    out toReturn);
            }
            catch (Exception e)
            {
                throw new Exception("Errore durante l'applicazione del timestamp.");
            }

            // Restituzione del risultato
            return toReturn;
        }

        #endregion

    }
}