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
    public partial class NoteImport : System.Web.UI.Page
    {

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.InitializePage();
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

            byte[] dati = null;
            if (this.uploadedFile.HasFile)
            {
                string fileName = Server.HtmlEncode(this.uploadedFile.FileName);
                string extension = System.IO.Path.GetExtension(fileName);

                if (extension == ".xls")
                {
                    ViewState.Add("fileExcel", this.uploadedFile.PostedFile.FileName);
                    HttpPostedFile p = this.uploadedFile.PostedFile;
                    System.IO.Stream fs = p.InputStream;
                    dati = new byte[fs.Length];
                    fs.Read(dati, 0, (int)fs.Length);
                    fs.Close();
                    ViewState.Add("DatiExcel", dati);
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "dialog", "ajaxDialogModal('WarningNoteImportFile', 'warning', '');", true);
                    return;
                }
            }

            if (dati != null)
            {
                this.plcFilename.Visible = false;
                this.plcReport.Visible = true;
                this.BtnConfirm.Enabled = false;

                DocsPaWR.InfoUtente infoUt = UserManager.GetInfoUser();
                string message = "";
                DocsPaWR.ImportResult[] report = null;
                try
                {
                    report = NoteManager.InsertNotaInElencoDaExcel(dati, ViewState["fileExcel"].ToString());
                    ViewState.Remove("DatiExcel");
                }
                catch (Exception ex)
                {
                    // Creazione di un array du result con un solo elemento
                    // che conterrà il dettaglio dell'eccezione
                    report = new DocsPaWR.ImportResult[1];
                    report[0] = new DocsPaWR.ImportResult()
                    {
                        Outcome = DocsPaWR.OutcomeEnumeration.KO,
                        Message = ex.Message,
                    };
                }

                this.grdRisExcel.DataSource = report;
                this.grdRisExcel.DataBind();
            }
        }

        #endregion

        #region Methods

        protected void InitializePage()
        {
            this.InitializeLanguage();
        }

        private void InitializeLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.BtnConfirm.Text = Utils.Languages.GetLabelFromCode("GenericBtnOk", language);
            this.BtnClose.Text = Utils.Languages.GetLabelFromCode("GenericBtnClose", language);
            this.lblTemplate.Text = Utils.Languages.GetLabelFromCode("ManageNotesTemplateXLS", language);
            this.lblFilename.Text = Utils.Languages.GetLabelFromCode("ManageNotesFilename", language);
            this.lnkTemplate.Text = Utils.Languages.GetLabelFromCode("ManageNotesDownloadTemplateXLS", language);
            this.grdRisExcel.Columns[0].HeaderText = Utils.Languages.GetLabelFromCode("ManageNotesImportResultDescription", language);
            this.grdRisExcel.Columns[1].HeaderText = Utils.Languages.GetLabelFromCode("ManageNotesImportResultResult", language);
            this.grdRisExcel.Columns[2].HeaderText = Utils.Languages.GetLabelFromCode("ManageNotesImportResultDetails", language);
        }

        private void RefreshScript()
        {
            //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshSelect", "refreshSelect();", true);
        }

        private void CloseMask(bool withReturnValue)
        {
            string retValue = withReturnValue ? "true" : "false";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "closeAjaxModal", "parent.closeAjaxModal('NoteImport', '" + retValue + "');", true);
        }

        /// <summary>
        /// Funzione per la restituzione dell'esito dell'operazione
        /// </summary>
        /// <param name="result">L'oggetto result associato alla riga corrente</param>
        /// <returns>L'esito</returns>
        protected string GetResult(DocsPaWR.ImportResult result)
        {
            string toReturn;

            // A seconda dell'esito bisogna visualizzarlo in rosso, in verde o in giallo
            switch (result.Outcome)
            {
                case DocsPaWR.OutcomeEnumeration.KO:
                    toReturn = String.Format("<span style=\"color:Red;\">{0}</span>",
                        result.Outcome);
                    break;

                case DocsPaWR.OutcomeEnumeration.OK:
                    toReturn = String.Format("<span style=\"color:Green;\">{0}</span>",
                        result.Outcome);
                    break;

                case DocsPaWR.OutcomeEnumeration.Warnings:
                    toReturn = String.Format("<span style=\"color:Yellow;\">{0}</span>",
                        result.Outcome);
                    break;

                default:
                    toReturn = String.Format("<span style=\"color:Green;\">{0}</span>",
                        result.Outcome);
                    break;
            }

            // Restituzione del testo
            return toReturn;
        }

        /// Funzione per la restituzione dei dettagli sull'esito
        /// </summary>
        /// <param name="result">L'oggetto result associato alla riga corrente</param>
        /// <returns>Gli eventuali dettagli sull'esito</returns>
        protected string GetDetails(DocsPaWR.ImportResult result)
        {
            string toReturn;

            System.Text.StringBuilder message = new System.Text.StringBuilder();

            // Se ci sono dettagli da mostrare
            if (result.OtherInformation != null)
            {
                // ...aggiunta del tag di inizio numerazione
                message.AppendLine("<ul>");

                // ...per ogni dettaglio...
                foreach (string str in result.OtherInformation)
                    // ...aggiunta dell'item 
                    message.AppendFormat("<li>{0}</li>",
                        str);

                // ...aggiunta del tag di chiusura della lista
                message.AppendLine("</ul>");

            }

            // Restituzione dei dettagli
            toReturn = message.ToString();

            // Restituzione del testo
            return toReturn;
        }

        #endregion

    }
}