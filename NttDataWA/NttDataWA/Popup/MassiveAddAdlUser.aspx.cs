using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.DocsPaWR;
using NttDataWA.UIManager;
using NttDataWA.Utils;
using NttDatalLibrary;

namespace NttDataWA.Popup
{
    public partial class MassiveAddAdlUser : System.Web.UI.Page
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

            // Selezione della procedura da seguire in base al
            // tipo di oggetto
            if (!this.IsFasc)
            {
                this.MoveDocumentsInADL(MassiveOperationUtils.GetSelectedItems(), report);
            }
            else
            {
                this.MoveProjectsInADL(MassiveOperationUtils.GetSelectedItems(), report);
            }

            // Introduzione della riga di summary
            string[] pars = new string[] { "" + report.Worked, "" + report.NotWorked };
            if (!this.IsFasc)
                report.AddSummaryRow("Documenti lavorati: {0} - Documenti non lavorati: {1}", pars);
            else
                report.AddSummaryRow("Fascicoli lavorati: {0} - Fascicoli non lavorati: {1}", pars);

            this.generateReport(report, "Spostamento in ADL utente massivo");
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
            foreach (KeyValuePair<string,string> item in this.ListCheck)
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
            ScriptManager.RegisterStartupScript(this, this.GetType(), "closeAjaxModal", "parent.closeAjaxModal('MassiveAddAdlUser', '" + retValue + "');", true);
        }

        /// <summary>
        /// Funzione per lo spostamento di fascicoli nell'area di lavoro
        /// </summary>
        /// <param name="projectsId">Lista dei system id dei fascicoli da spostare nell'area di lavoro</param>
        /// <param name="report">Report dell'elaborazione</param>
        private void MoveProjectsInADL(List<MassiveOperationTarget> projectsId, MassiveOperationReport report)
        {
            // Lista delle informazioni sui documenti
            List<Fascicolo> projectsInfo = null;

            // Recupero delle informazioni sui fascicoli da aggiungere in ADL
            projectsInfo = this.LoadProjectInformation(projectsId, report);

            // Salvataggio dei fascicoli nell'area di lavoro
            this.MoveDocumentsInWorkingArea(projectsInfo, report);
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
        /// Funzione per il recupero delle informazioni sui fascicoli da spostare in ADL
        /// </summary>
        /// <param name="projectsId">Lista degli identificativi dei fascicoli</param>
        /// <param name="report">Report dell'esecuzione</param>
        /// <returns>Lista delle informazioni sui fascicoli</returns>
        private List<Fascicolo> LoadProjectInformation(List<MassiveOperationTarget> projectsId, MassiveOperationReport report)
        {
            // Lista da restituire
            List<Fascicolo> toReturn = new List<Fascicolo>();

            // Per ogni fascicolo...
            foreach (MassiveOperationTarget mot in projectsId)
                try
                {
                    // ...aggiunta del fascicolo alla lista dei fascicoli
                    toReturn.Add(ProjectManager.getFascicolo(
                        this,
                        mot.Id));
                }
                catch (Exception e)
                {
                    report.AddReportRow(
                        mot.Codice,
                        MassiveOperationReport.MassiveOperationResultEnum.KO,
                        "Errore durante il reperimento delle informazioni sul fascicolo.");
                }

            // Restituzione della lista con le informaizoni sui fascicoli
            return toReturn;
        }

        /// <summary>
        /// Funzione per lo spostamento di fascicoli nell'area di lavoro
        /// </summary>
        /// <param name="projectsInformation">Lista dei fascicoli da spostare</param>
        /// <param name="report">Report dell'esecuzione</param>
        private void MoveDocumentsInWorkingArea(List<Fascicolo> projectsInformation, MassiveOperationReport report)
        {
            // Per ogni fascicolo...
            foreach (Fascicolo prj in projectsInformation)
                try
                {
                    if (!prj.InAreaLavoro.Equals("1") && ProjectManager.isFascInADLRole(prj.systemID, this.Page) == 0)
                    {
                        // ...spostamento del fascicolo nell'area di lavoro
                        ProjectManager.addFascicoloInAreaDiLavoro(
                            this,
                            prj);

                        // ...aggiunta di un risultato positivo
                        report.AddReportRow(
                            prj.codice,
                            MassiveOperationReport.MassiveOperationResultEnum.OK,
                            "Fascicolo inserito correttamento nell'area di lavoro.");
                    }
                    else
                    {
                        report.AddReportRow(
                            prj.codice,
                            MassiveOperationReport.MassiveOperationResultEnum.KO,
                            "Fascicolo già inserito nell'area di lavoro.");
                    }
                }
                catch (Exception e)
                {
                    report.AddReportRow(
                        prj.codice,
                        MassiveOperationReport.MassiveOperationResultEnum.KO,
                        "Errore durante lo spostamento del fascicolo nell'area di lavoro.");

                }
        }

        /// <summary>
        /// Funzione per lo spostamento dei documenti selezionati nell'area di lavoro
        /// </summary>
        /// <param name="documentSystemId">Lista dei system id dei documenti da spostare</param>
        /// <param name="report">Il report dell'esecuzione</param>
        private void MoveDocumentsInADL(List<MassiveOperationTarget> documentSystemId, MassiveOperationReport report)
        {
            // Lista delle informazioni sui documenti
            List<InfoDocumento> documentsInfo = null;

            // Recupero delle informazioni sui documenti da aggiungere in ADL
            documentsInfo = this.LoadDocumentInformation(documentSystemId, report);

            // Salvataggio dei documenti nell'area di lavoro
            this.MoveDocumentsInWorkingArea(documentsInfo, report);

        }

        /// <summary>
        /// Funzione per il caricamento delle informaioni sui documenti
        /// </summary>
        /// <param name="documentSystemId">Lista dei system id dei documenti di cui recuperare le informazioni</param>
        /// <param name="report">Report dell'esecuzione</param>
        /// <returns>Lista delle informazioni sui documenti</returns>
        private List<InfoDocumento> LoadDocumentInformation(List<MassiveOperationTarget> documentSystemId, MassiveOperationReport report)
        {
            // L'oggetto da restituire
            List<InfoDocumento> toReturn = new List<InfoDocumento>();
            // Oggetto con le informazioni di base sul documento
            BaseInfoDoc baseInfoDoc;

            // Recupero delle informazioni sui documenti
            foreach (MassiveOperationTarget mot in documentSystemId)
            {
                try
                {
                    // Recupero delle informazioni di base sul documento
                    baseInfoDoc = DocumentManager.GetBaseInfoForDocument(
                        mot.Id,
                        String.Empty,
                        String.Empty).Where(e => e.IdProfile.Equals(mot.Id)).FirstOrDefault();

                    // Recuper dell'oggetto info documento
                    toReturn.Add(DocumentManager.GetInfoDocumento(
                        baseInfoDoc.IdProfile,
                        baseInfoDoc.DocNumber,
                        this));

                }
                catch (Exception e)
                {
                    report.AddReportRow(
                        mot.Codice,
                        MassiveOperationReport.MassiveOperationResultEnum.KO,
                        "Errore durante il recupero delle informazioni sul documento.");
                }

            }

            // Restiutuzione della lista delle informazioni sui documento da spostare nell'ADL
            return toReturn;

        }

        /// <summary>
        /// Funzione per lo spostamento dei documenti nell'area di lavoro
        /// </summary>
        /// <param name="documentsInfo">L'elenco dei documenti da spostare</param>
        /// <param name="report">Report del''esecuzione</param>
        private void MoveDocumentsInWorkingArea(List<InfoDocumento> documentsInfo, MassiveOperationReport report)
        {
            // Per ogni documento...
            foreach (InfoDocumento docInfo in documentsInfo)
            {
                string id = MassiveOperationUtils.getItem(docInfo.idProfile).Codice;
                // ...spostamento del documento nell'area di lavoro
                try
                {
                    if (DocumentManager.isDocInADL(docInfo.idProfile, this.Page) == 0 && DocumentManager.isDocInADLRole(docInfo.idProfile, this.Page) == 0)
                    {
                        SchedaDocumento sd = DocumentManager.getDocumentDetails(this, docInfo.docNumber, docInfo.docNumber);
                        DocumentManager.addAreaLavoro(this, sd);
                        report.AddReportRow(
                            id,
                            MassiveOperationReport.MassiveOperationResultEnum.OK,
                            "Documento inserito correttamente nell'area di lavoro.");
                    }
                    else
                    {
                        report.AddReportRow(
                            id,
                            MassiveOperationReport.MassiveOperationResultEnum.KO,
                            "Documento già inserito nell'area di lavoro.");
                    }
                }
                catch (Exception e)
                {
                    report.AddReportRow(
                        id,
                        MassiveOperationReport.MassiveOperationResultEnum.KO,
                        "Errore durante lo spostamento del documento nell'area di lavoro. Dettagli: " + e.Message);
                }
            }

        }

        #endregion

    }
}