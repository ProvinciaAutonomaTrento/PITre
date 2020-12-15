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
    public partial class RimuoviVersioni : MassivePage
    {
        protected override string PageName
        {
            get
            {
                return "Rimuovi versioni";
            }
        }

        protected override bool IsFasc
        {
            get {
                return false;
            }
        }

        protected override bool btnConferma_Click(object sender, EventArgs e)
        {
            // Lista di system id degli elementi selezionati
            List<MassiveOperationTarget> selectedItem;

            // Recupero della lista dei system id dei documenti selezionati
            selectedItem = MassiveOperationUtils.GetSelectedItems();
            
            //tipo di rimozione
            string selectedValue = this.rbl_versioni.SelectedItem.Value;
            RemoveVersionType type=RemoveVersionType.ALL_BUT_THE_LAST;
            if ("opt_no_last_two".Equals(selectedValue))
            {
                type = RemoveVersionType.ALL_BUT_LAST_TWO;
            }
            MassiveOperationReport report = ProceedWithOperation(selectedItem,type);
            string[] pars = new string[] { "" + report.Worked, "" + report.NotWorked };
            report.AddSummaryRow("Documenti lavorati: {0} - Documenti non lavorati: {1}",pars);

            // Visualizzazione del report e termine della fase di attesa
            this.generateReport(report,"Rimozione versioni massiva");
            return true;
        }

        private MassiveOperationReport ProceedWithOperation(List<MassiveOperationTarget> selectedItem,RemoveVersionType type)
        {
            // Inizializzazione del report
            MassiveOperationReport report = new MassiveOperationReport();
            List<ImportResult> res=DocumentManager.RimuoviVersioniMassivo(selectedItem, type, this);
            foreach(ImportResult temp in res){
                string codice = MassiveOperationUtils.getItem(temp.IdProfile).Codice;
                report.AddReportRow(codice,(temp.Outcome == OutcomeEnumeration.OK) ? MassiveOperationReport.MassiveOperationResultEnum.OK : MassiveOperationReport.MassiveOperationResultEnum.KO, temp.Message);
            }
            return report;
        }
    }
}