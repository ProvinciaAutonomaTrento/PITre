using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using NttDataWA.DocsPaWR;
using NttDataWA.UIManager;

namespace NttDataWA.Popup
{
    public partial class ConservationAreaValidation : System.Web.UI.Page
    {
        #region Properties

        private string IdInstance
        {
            get
            {
                if (HttpContext.Current.Session["IdInstance"] != null)
                    return (string)HttpContext.Current.Session["IdInstance"];
                else
                    return null;
            }
        }

        private AreaConservazioneValidationResult ResultValidateInstance
        {
            get
            {
                if (HttpContext.Current.Session["ResultValidateInstance"] != null)
                    return (AreaConservazioneValidationResult)HttpContext.Current.Session["ResultValidateInstance"];
                else
                    return null;
            }
        }

        #endregion

        #region Standard Method

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
            this.BtnConservationAreaValidationClose.Text = Utils.Languages.GetLabelFromCode("BtnConservationAreaValidationClose", language);
            this.BtnConservationAreaValidationSend.Text = Utils.Languages.GetLabelFromCode("BtnConservationAreaValidationSend", language);
        }

        private void InitializeContent()
        {
            this.ShowReport(ResultValidateInstance);
        }

        #endregion


        #region Event

        protected void BtnConservationAreaValidationSend_Click(object sender, EventArgs e)
        {
            try
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "closeAJM", "parent.closeAjaxModal('ConservationAreaValidation', 'up', parent);", true);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void BtnConservationAreaValidationClose_Click(object sender, EventArgs e)
        {
            try
            {
                //ScriptManager.RegisterClientScriptBlock(this.UpPnlButtons, this.UpPnlButtons.GetType(), "closeAJM", "parent.parent.closeAjaxModal('ConservationAreaValidation','');", true);
                ScriptManager.RegisterStartupScript(this, this.GetType(), "closeAJM", "parent.closeAjaxModal('ConservationAreaValidation', '', parent);", true);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        #endregion

        #region Utils

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        private void ShowReport(DocsPaWR.AreaConservazioneValidationResult data)
        {
            this.txtReport.Text = this.GetReportInvalidItemsConservazione(data);
        }

        /// <summary>
        /// Creazione report dei dettagli dell'istanza di conservazione non valida
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        private string GetReportInvalidItemsConservazione(DocsPaWR.AreaConservazioneValidationResult result)
        {

            const string AVVISO = "ATTENZIONE!\nL'istanza contiene documenti il cui formato non risponde alle policy di conservazione concordate con il Centro Servizi.";
            const string INTEST = "ELENCO DEI DOCUMENTI NON RISPONDENTI ALLE POLICY DI CONSERVAZIONE CONCORDATE CON IL CENTRO SERVIZI";
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("ISTANZA DI CONSERVAZIONE: {0}", IdInstance);
            sb.AppendLine().AppendLine();
            sb.AppendLine(AVVISO);
            sb.AppendLine();
            sb.AppendLine(INTEST);
            sb.AppendLine();

            sb.AppendLine(new string('-', 240));

            sb.AppendLine(
                    this.GetReportFieldValue("DATA", 20) +
                    this.GetReportFieldValue("OGGETTO", 50) +
                    this.GetReportFieldValue("ID/SEGNATURA", 50) +
                    this.GetReportFieldValue("TIPO ERRORE", 120));

            sb.AppendLine(new string('-', 240));

            foreach (DocsPaWR.InvalidItemConservazione item in result.Items)
            {
                sb.AppendLine(
                        this.GetReportFieldValue(item.Item.data_prot_or_create, 20) +
                        this.GetReportFieldValue(item.Item.desc_oggetto, 50) +
                        this.GetReportFieldValue(item.Item.numProt_or_id, 50) +
                        this.GetReportFieldValue(item.ErrorMessage, 120));
            }

            return sb.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="fieldLength"></param>
        /// <returns></returns>
        private string GetReportFieldValue(string value, int fieldLength)
        {
            if (value.Length > fieldLength)
                value = string.Concat(value.Substring(0, (fieldLength - 3)), "...");

            return value.PadRight(fieldLength, ' ');
        }

        #endregion
    }
}