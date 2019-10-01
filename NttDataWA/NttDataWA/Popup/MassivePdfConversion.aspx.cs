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
    public partial class MassivePdfConversion : System.Web.UI.Page
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

        /// <summary>
        /// Lista delle informazioni di base sui documenti da convertire in PDF
        /// </summary>
        private List<BaseInfoDoc> DocumentsInfo
        {
            get
            {
                return HttpContext.Current.Session["InfoDocumentList"] as List<BaseInfoDoc>;
            }

            set
            {
                HttpContext.Current.Session["InfoDocumentList"] = value;
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

            // Inserimento dei documenti in coda
            report = this.ConvertInPdf(this.DocumentsInfo);

            // Introduzione della riga di summary
            string[] pars = new string[] { "" + report.Worked, "" + report.NotWorked };
            report.AddSummaryRow("Documenti lavorati: {0} - Documenti non lavorati: {1}", pars);

            // Generazione del report da esportare
            this.generateReport(report, "Conversione PDF massiva");
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
            this.InitializeDocuments();

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

        private void InitializeDocuments()
        {
            // La lista dei system id dei documenti selezionati
            List<MassiveOperationTarget> documentsIdProfile;

            // Lista delle informazioni di base sui documenti da convertire
            List<BaseInfoDoc> documents;

            // Recupero dei system id dei documenti da convertire
            documentsIdProfile = MassiveOperationUtils.GetSelectedItems();

            // Inizializzazione della lista delle informazioni sui documenti da convertire
            documents = new List<BaseInfoDoc>();

            // Per ogni id profile, vengono recuperate le informazioni di base sul
            // documento
            foreach (MassiveOperationTarget temp in documentsIdProfile)
            {
                // ...recupero del contenuto delle informazioni di base sul documento
                try
                {
                    documents.Add(DocumentManager.GetBaseInfoForDocument(
                       temp.Id,
                       String.Empty,
                       String.Empty).Where(e => e.IdProfile.Equals(temp.Id)).FirstOrDefault());
                }
                catch (Exception e)
                {
                    throw new Exception(String.Format(
                        "Errore durante il reperimento delle informazioni sul documento {0}", temp.Codice));

                }
            }

            // Salvataggio delle informazioni sui documenti
            this.DocumentsInfo = documents;
        }

        private void RefreshScript()
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshTipsy", "tooltipTipsy();", true);
        }

        private void CloseMask(bool withReturnValue)
        {
            string retValue = withReturnValue ? "true" : "false";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "closeAjaxModal", "parent.closeAjaxModal('MassiveConversion', '" + retValue + "');", true);
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
        /// Funzione per la conversione dei documenti in PDF
        /// </summary>
        /// <param name="documentsInfo">La lista con le informazioni sui documenti da convertire</param>
        /// <returns>Il report dell'elaborazione</returns>
        public MassiveOperationReport ConvertInPdf(List<BaseInfoDoc> documentsInfo)
        {
            // Il report da restituire
            MassiveOperationReport report;

            // Il risultato della messa in conversione di un documento
            MassiveOperationReport.MassiveOperationResultEnum tempResult;

            // Il messaggio da inserire nel report
            string message;

            // Informazioni sull'utente che ha lanciato la procedura
            InfoUtente userInfo;

            // Il file associato al documento
            byte[] content = null;

            // Inizializzazione del report
            report = new MassiveOperationReport();

            // Recupero delle informazioni sull'utente
            userInfo = UserManager.GetInfoUser();
            foreach (BaseInfoDoc doc in documentsInfo)
            {
                // Inizializzazione del messaggio
                string codice = MassiveOperationUtils.getItem(doc.IdProfile).Codice;
                message = "Documento inserito correttamente nella coda di conversione PDF.";

                //Recupero i diritti sul documento
                string ar = DocumentManager.getAccessRightDocBySystemID(doc.IdProfile, UserManager.GetInfoUser());

                // Verifica delle informazioni sul documento
                tempResult = this.ValidateDocumentInformation(doc, out message);

                if (ar.Equals("20"))
                {
                    message = "Il documento è in attesa di accettazione, quindi non può essere convertito";
                    tempResult = MassiveOperationReport.MassiveOperationResultEnum.KO;
                }
                if (doc.Firmato.Equals("1"))
                {
                    message = "Il documento è firmato, quindi non può essere convertito";
                    tempResult = MassiveOperationReport.MassiveOperationResultEnum.KO;
                }
                //Verifico che il documento non sia consolidato
                NttDataWA.DigitalSignature.DigitalSignManager mng = new NttDataWA.DigitalSignature.DigitalSignManager();
                DocsPaWR.SchedaDocumento schedaDocumento = mng.GetSchedaDocumento(doc.DocNumber);

                if (schedaDocumento != null)
                {
                    if (schedaDocumento.ConsolidationState != null &&
                       schedaDocumento.ConsolidationState.State > DocsPaWR.DocumentConsolidationStateEnum.None)
                    {
                        // Il documento risulta consolidato, non può essere firmato digitalmente
                        message = "Il documento risulta consolidato";
                        tempResult = MassiveOperationReport.MassiveOperationResultEnum.KO;
                    }
                }

                // Se il risultato di validazione delle informazioni è OK, si pruò procedere
                if (tempResult == MassiveOperationReport.MassiveOperationResultEnum.OK)
                {
                    try
                    {
                        // Recupero delle informazioni sul file da convertire
                        content = FileManager.getInstance(Session.SessionID).GetFile(
                            this,
                            doc.VersionId,
                            doc.VersionNumber.ToString(),
                            doc.DocNumber,
                            doc.Path,
                            doc.FileName,
                            false).content;
                    }
                    catch (Exception e)
                    {
                        message = "Errore durante il reperimento del file associato al documento.";
                        tempResult = MassiveOperationReport.MassiveOperationResultEnum.KO;
                    }
                }

                // Se si può procedere si mette in coda il file da convertire
                if (tempResult == MassiveOperationReport.MassiveOperationResultEnum.OK)
                {
                    try
                    {
                        // Avvio della conversione
                        FileManager.EnqueueServerPdfConversionAM(
                            this,
                            userInfo,
                            content,
                            doc.FileName,
                            new SchedaDocumento()
                            {
                                systemId = doc.IdProfile,
                                docNumber = doc.DocNumber
                            });
                    }
                    catch (Exception e)
                    {
                        tempResult = MassiveOperationReport.MassiveOperationResultEnum.KO;
                        message = "Errore durante l'inserimento del documento nella coda di conversione.";
                    }

                }
                // Inserimento di una nuova riga nel report
                report.AddReportRow(
                    codice,
                    tempResult,
                    message);

            }

            // Restituzione del report
            return report;
        }

        /// <summary>
        /// Funzione per la validazione delle informazioni sul documento
        /// </summary>
        /// <param name="documentInfo">Le informazioni sul documento</param>
        /// <param name="message">L'eventuale messaggio di errore</param>
        /// <returns>Risultato della validazione</returns>
        private MassiveOperationReport.MassiveOperationResultEnum ValidateDocumentInformation(
            BaseInfoDoc documentInfo,
            out string message)
        {
            // Il risultato da restituire
            MassiveOperationReport.MassiveOperationResultEnum toReturn;

            // Inizializzazione del risultato e del messaggio
            toReturn = MassiveOperationReport.MassiveOperationResultEnum.OK;
            message = String.Empty;

            // Se non è stato acquisito un file per il documento, il risultato è negativo
            if (!documentInfo.HaveFile)
            {
                toReturn = MassiveOperationReport.MassiveOperationResultEnum.KO;
                message = "Nessun file acquisito per il documento.";
            }
            else
                // Altrimenti se il file è già un PDF il risultato è un AlreadyWorked
                if (Path.GetExtension(documentInfo.FileName).ToUpper().Equals(".PDF"))
                {
                    toReturn = MassiveOperationReport.MassiveOperationResultEnum.AlreadyWorked;
                    message = "Documento già convertito in PDF.";
                }

            // PALUMBO: Commentato in quanto se dualFileWriting = fals, non recupera i file.
            // Se il file risulta acquisito ma non è reperibile, il risultato è negativo
            //if (toReturn == MassiveOperationReport.MassiveOperationResultEnum.OK &&
            //    !File.Exists(documentInfo.FileName))
            //{
            //    toReturn = MassiveOperationReport.MassiveOperationResultEnum.KO;
            //    message = "Impossibile recuperare il file associato al documento.";
            //}

            // Restituzione del risultato
            return toReturn;
        }

        #endregion

    }
}