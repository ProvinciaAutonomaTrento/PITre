using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DocsPAWA.utils;
using DocsPAWA.DocsPaWR;

namespace DocsPAWA.MassiveOperation
{
    public partial class DocumentConsolidation : MassivePage
    {
        protected override string PageName
        {
            get
            {
                return "Consolidamento documenti";
            }
        }

        protected override bool IsFasc
        {
            get
            {
                return false;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected bool IsMetadati
        {
            get
            {
                string context = Request["context"];
                if ("metadati".Equals(context)) return true;
                return false;
            }
        }

        protected string Alert
        {
            get
            {
                if (IsMetadati)
                {
                    return "Attenzione:<br/>" +
                            "Si stanno per consolidare i documenti selezionati nei loro metadati fondamentali.<br/>" +
                            "Inoltre, per ognuno, non sarà più possibile acquisire o modificare versioni.<br/>" +
                            "L'operazione ha carattere di irreversibilità. Si desidera continuare?";
                }
                else
                {
                    return "Attenzione:<br />Si stanno per consolidare i documenti selezionati, per ognuno non sarà più possibile acquisire o modificare versioni.<br />L'operazione ha carattere di irreversibilità. Si desidera continuare?";
                }
            }
        }

        protected override bool btnConferma_Click(object sender, EventArgs e)
        {
            DocsPaWR.InfoUtente userInfo = UserManager.getInfoUtente();
            DocumentConsolidationHandler dch = new DocumentConsolidationHandler(null, userInfo);
            DocumentConsolidationStateEnum state=DocumentConsolidationStateEnum.Step1;
            if (IsMetadati)
            {
                state=DocumentConsolidationStateEnum.Step2;
            }
            MassiveOperationReport report = dch.ConsolidateDocumentMassive(state, MassiveOperationUtils.GetSelectedItems());
            string[] pars = new string[] { "" + report.Worked, "" + report.NotWorked };
            report.AddSummaryRow("Documenti lavorati: {0} - Documenti non lavorati: {1}", pars);
            this.generateReport(report, "Consolidamento documenti");
            return true;
        }
    }
}