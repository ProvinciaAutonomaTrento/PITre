using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;

namespace DocsPAWA.popup
{
    public partial class areaConservazioneValidation : System.Web.UI.Page
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Response.Expires = -1;

            if (!this.IsPostBack)
            {
                // Reperimento dati istanza
                DocsPaWR.AreaConservazioneValidationResult data = GetData();

                this.ShowReport(data);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        private void ShowReport(DocsPaWR.AreaConservazioneValidationResult data)
        {
            this.txtReport.Text = this.GetReportInvalidItemsConservazione(data);           
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private DocsPaWR.AreaConservazioneValidationResult GetData()
        {
            DocsPaWR.AreaConservazioneValidationResult data = null;

            string sessionParam = this.Request.QueryString["sessionParam"];

            if (this.Session[sessionParam] != null)
            {
                data = this.Session[sessionParam] as DocsPaWR.AreaConservazioneValidationResult;

                this.Session.Remove(sessionParam);
            }

            return data;
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

            sb.AppendFormat("ISTANZA DI CONSERVAZIONE: {0}", this.Request.QueryString["idCons"]);
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

    }
}