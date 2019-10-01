using System;
using System.Collections;
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
    public partial class MassiveVers : System.Web.UI.Page
    {

        #region Properties

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
        }

        protected void BtnClose_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "reallowOp", "reallowOp();", true);
            this.CloseMask(true);
        }

        protected void BtnConfirm_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "reallowOp", "reallowOp();", true);

            // creazione e inizializzazione report
            MassiveOperationReport report = new MassiveOperationReport();

            this.ExecuteVersamento(MassiveOperationUtils.GetSelectedItems(), report);

            // summary del report
            string[] pars = new string[] { "" + report.Worked, "" + report.NotWorked };
            report.AddSummaryRow("Documenti lavorati: {0} - Documenti non lavorati: {1}", pars);

            //metodo generatereport da fare
            this.GenerateReport(report, "titolo");
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

        private void InitializePage()
        {
            this.InitializeLanguage();
            this.InitializeList();

            // se alcuni documenti non sono stati consolidati devo visualizzare il messaggio con il warning
            ArrayList list = new ArrayList();
            foreach (KeyValuePair<string, MassiveOperationTarget> item in MassiveOperationUtils.ItemsStatus)
            {
                list.Add(item.Key);
            }
            this.pnlWarning.Visible = DocumentManager.checkConsolidamentoDoc(list, UserManager.GetInfoUser());
        }

        private void InitializeLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.BtnConfirm.Text = Utils.Languages.GetLabelFromCode("MassiveAddAdlUserBtnConfirm", language);
            this.BtnClose.Text = Utils.Languages.GetLabelFromCode("MassiveAddAdlUserBtnClose", language);
            this.litMessage.Text = Utils.Languages.GetLabelFromCode("MassiveAddAdlUserAskConfirm", language);
            this.litMassiveVersConsolidationWarning.Text = Utils.Languages.GetLabelFromCode("LitMassiveVersConsolidationWarning", language);
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
            ScriptManager.RegisterStartupScript(this, this.GetType(), "closeAjaxModal", "parent.closeAjaxModal('MassiveVersPARER', '" + retValue + "');", true);
        }

        private void ExecuteVersamento(List<MassiveOperationTarget> documentsId, MassiveOperationReport report)
        {
            foreach (MassiveOperationTarget target in documentsId)
            {
                try
                {
                    // Recupero della scheda documento
                    SchedaDocumento documentDetails = DocumentManager.getDocumentDetails(this.Page, target.Id, string.Empty);

                    if (documentDetails != null)
                    {
                        if (documentDetails.documentoPrincipale != null)
                        {
                            report.AddReportRow(target.Codice, MassiveOperationReport.MassiveOperationResultEnum.KO, "Non è possibile versare allegati.");
                        }
                        else
                        {
                            if (this.isPredisposto(documentDetails))
                            {
                                report.AddReportRow(target.Codice, MassiveOperationReport.MassiveOperationResultEnum.KO, "Non è possibile versare documenti predisposti.");
                            }
                            else
                            {
                                if (documentDetails.checkOutStatus != null && !string.IsNullOrEmpty(documentDetails.checkOutStatus.ID))
                                {
                                    report.AddReportRow(target.Codice, MassiveOperationReport.MassiveOperationResultEnum.KO, "Non è possibile versare documenti bloccati.");
                                }
                                else
                                {
                                    if (this.checkRepertorio(documentDetails))
                                    {
                                        report.AddReportRow(target.Codice, MassiveOperationReport.MassiveOperationResultEnum.KO, "Non è possibile versare repertori il cui contatore non sia scattato.");
                                    }
                                    else
                                    {
                                        if (documentDetails.documenti[0] != null)
                                        {
                                            if (!(string.IsNullOrEmpty(documentDetails.documenti[0].fileSize)) && Convert.ToInt32(documentDetails.documenti[0].fileSize) > 0)
                                            {
                                                string result = DocumentManager.AddDocToQueueCons(target.Id, UserManager.GetInfoUser());
                                                this.InsertRow(result, report, target.Id);
                                            }
                                            else
                                            {
                                                report.AddReportRow(target.Codice, MassiveOperationReport.MassiveOperationResultEnum.KO, "Il documento principale non risulta essere stato acquisito.");
                                            }
                                        }
                                        else
                                        {
                                            report.AddReportRow(target.Codice, MassiveOperationReport.MassiveOperationResultEnum.KO, "Il documento principale non risulta essere stato acquisito.");
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        report.AddReportRow(target.Codice, MassiveOperationReport.MassiveOperationResultEnum.KO, "Non è stato possibile recuperare il dettaglio sul documento.");
                    }
                }
                catch (Exception ex)
                {
                    report.AddReportRow(target.Codice, MassiveOperationReport.MassiveOperationResultEnum.KO, "Errore durante il recupero dei dettagli sul documento.");
                }
            }
        }

        private void InsertRow(string result, MassiveOperationReport report, string codice)
        {
            switch (result)
            {
                case "OK":
                    report.AddReportRow(codice, MassiveOperationReport.MassiveOperationResultEnum.OK, "Documento inserito correttamente nella coda di versamento.");
                    break;

                case "IN_QUEUE":
                    report.AddReportRow(codice, MassiveOperationReport.MassiveOperationResultEnum.AlreadyWorked, "Il documento è già in attesa di versamento.");
                    break;

                case "DOC_CONS":
                    report.AddReportRow(codice, MassiveOperationReport.MassiveOperationResultEnum.AlreadyWorked, "Il documento è già stato preso in carico dal sistema di conservazione.");
                    break;

                case "INS_ERR":
                    report.AddReportRow(codice, MassiveOperationReport.MassiveOperationResultEnum.KO, "Errore durante l'inserimento nella coda di versamento");
                    break;

                case "CONS_ERR":
                    report.AddReportRow(codice, MassiveOperationReport.MassiveOperationResultEnum.KO, "Non è stato possibile consolidare il documento.");
                    break;

                case "STATE_ERR":
                    report.AddReportRow(codice, MassiveOperationReport.MassiveOperationResultEnum.KO, "Non è stato possibile reperire lo stato di conservazione del documento.");
                    break;

            }
        }

        private void GenerateReport(MassiveOperationReport report, string titolo)
        {
            this.grdReport.DataSource = report.GetDataSet();
            this.grdReport.DataBind();
            this.pnlReport.Visible = true;
            this.upReport.Update();

            this.plcMessage.Visible = false;
            this.UpPnlMessage.Update();

            this.BtnConfirm.Enabled = false;
            this.UpPnlButtons.Update();
        }

        private bool isPredisposto(SchedaDocumento doc)
        {
            bool result = false;

            if (doc.tipoProto.Equals("A") || doc.tipoProto.Equals("P") || doc.tipoProto.Equals("I"))
            {
                if (!(doc.protocollo != null && !(string.IsNullOrEmpty(doc.protocollo.segnatura))))
                    result = true;
            }

            return result;
        }

        /// <summary>
        /// Verifica se un contatore di repertorio non è scattato
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        private bool checkRepertorio(SchedaDocumento doc)
        {
            bool result = false;

            if (doc.template != null && doc.template.ELENCO_OGGETTI != null)
            {
                foreach (OggettoCustom ogg in doc.template.ELENCO_OGGETTI)
                {
                    if (ogg.TIPO.DESCRIZIONE_TIPO.ToUpper().Equals("CONTATORE") && !string.IsNullOrEmpty(ogg.CONS_REPERTORIO) && ogg.CONS_REPERTORIO.Equals("1"))
                    {
                        if (string.IsNullOrEmpty(DocumentManager.getSegnaturaRepertorio(doc.docNumber, AdministrationManager.AmmGetInfoAmmCorrente(UserManager.GetInfoUser().idAmministrazione).Codice)))
                            result = true;
                    }
                }
            }

            return result;
        }

        #endregion

    }
}