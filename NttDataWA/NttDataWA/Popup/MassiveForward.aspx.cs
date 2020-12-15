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
    public partial class MassiveForward : System.Web.UI.Page
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

            // Lista di system id degli elementi selezionati
            List<MassiveOperationTarget> selectedItem;

            // Errore verificatosi in fase di creazione della scheda
            String error = String.Empty;

            // La scheda documento creata
            SchedaDocumento document;

            // Recupero della lista dei system id dei documenti selezionati
            selectedItem = MassiveOperationUtils.GetSelectedItems();

            // Generazione della scheda
            MassiveOperationReport report = new MassiveOperationReport();
            //Se per uno dei documenti selezionati è attivo un processo di firma, blocco l'operazione di inoltro
            bool inLibroFirma = false;
            try
            {
                foreach (MassiveOperationTarget temp in selectedItem)
                {
                    if(LibroFirmaManager.IsDocOrAllInLibroFirma(temp.Id))
                    {
                        report.AddReportRow("-", MassiveOperationReport.MassiveOperationResultEnum.KO, "Il documento non è stato creato poichè per uno dei documenti selezionati, o suoi allegati, è attivo un processo di firma");
                        inLibroFirma = true;
                        break;
                    }
                }
                if (!inLibroFirma)
                {
                    document = this.GenerateDocumentScheda(selectedItem, out error);
                    if (document != null)
                    {
                        report.AddReportRow("-", MassiveOperationReport.MassiveOperationResultEnum.OK, "Il documento è stato creato correttamente");
                    }
                    else
                    {
                        report.AddReportRow("-", MassiveOperationReport.MassiveOperationResultEnum.KO, "Il documento non è stato creato");
                    }
                }
            }
            catch (Exception ex)
            {
                report.AddReportRow("-", MassiveOperationReport.MassiveOperationResultEnum.KO, "Errore nella creazione del documento");
            }
            string[] pars = new string[] { "" + report.Worked, "" + report.NotWorked };
            report.AddSummaryRow("Documenti lavorati: {0} - Documenti non lavorati: {1}", pars);
            this.generateReport(report, "Inoltra massivo");

            // set session to manage document.aspx
            HttpContext.Current.Session["IsForwarded"] = true;
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
            ScriptManager.RegisterStartupScript(this, this.GetType(), "closeAjaxModal", "parent.closeAjaxModal('MassiveForward', '" + retValue + "');", true);
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
        /// Funzione per a creazione della scheda
        /// </summary>
        /// <param name="selectedItem">Lista dei system id dei documenti selezionati</param>
        /// <param name="error"></param>
        /// <returns>La scheda documento creata</returns>
        private SchedaDocumento GenerateDocumentScheda(List<MassiveOperationTarget> selectedItem, out String error)
        {
            SchedaDocumento document = null;
            List<string> ids = new List<string>();
            foreach (MassiveOperationTarget temp in selectedItem) ids.Add(temp.Id);
            document = DocumentManager.GetSchedaDocumentoInoltroMassivo(
                ids,
                UserManager.GetInfoUser(),
                RoleManager.GetRoleInSession(),
                out error);

            DocumentManager.setSelectedRecord(document);

            return document;


        }

        #endregion

    }
}