using System.Linq;
using System;
using DocsPaVO.Report;
using System.Data;
using System.Collections.Generic;
using System.Xml.Linq;
namespace BusinessLogic.Reporting
{
    /// <summary>
    /// Classe per la generazione dei report relativi ai report spedizioni
    /// </summary>
    [ReportGenerator(Name = "Risultati ricerca spedizioni", ContextName = "ReportSpedizioni", Key = "ReportSpedizioni")]
    public class ElencoSpedizioniReportGeneratorCommand : ReportGeneratorCommand
    {


        /// <summary>
        /// Questo report prevede la possibilità di selezionare i campi da esportare
        /// </summary>
        /// <returns>Collezione con la lista dei campi che costituiscono il report</returns>
        protected override HeaderColumnCollection GetExportableFieldsCollection()
        {
            HeaderColumnCollection retList = new HeaderColumnCollection();
            retList.Add(this.GetHeaderColumn("Protocollo", 100, "PROTOCOLLO"));
            retList.Add(this.GetHeaderColumn("Descr. Oggetto", 400, "OGGETTO"));
            retList.Add(this.GetHeaderColumn("Nominativo Destinatario", 400, "NOMINATIVO_DESTINATARIO"));
            retList.Add(this.GetHeaderColumn("Tipo Dest.", 100, "TIPO_DESTINATARIO"));
            retList.Add(this.GetHeaderColumn("Mezzo Spedizione", 200, "MEZZO_SPEDIZIONE"));
            retList.Add(this.GetHeaderColumn("Mail Mittente", 200, "MAIL_MITTENTE"));
            retList.Add(this.GetHeaderColumn("Mail Destinatario", 200, "MAIL_DESTINATARIO"));
            retList.Add(this.GetHeaderColumn("Data Spedizione", 100, "DATA_SPEDIZIONE"));
            retList.Add(this.GetHeaderColumn("Accettazione", 100, "ACCETTAZIONE"));
            retList.Add(this.GetHeaderColumn("Consegna", 100, "CONSEGNA"));
            retList.Add(this.GetHeaderColumn("Conferma", 100, "CONFERMA"));
            retList.Add(this.GetHeaderColumn("Annullamento", 100, "ANNULLAMENTO"));
            retList.Add(this.GetHeaderColumn("Eccezione", 100, "ECCEZIONE"));
            retList.Add(this.GetHeaderColumn("Azione", 100, "AZIONE_INFO"));
            return retList;
        }

        /// <summary>
        /// Metodo per la creazione di un oggetto con le informazioni su una colonna
        /// </summary>
        /// <param name="columnName">Nome da assegnare alla colonna</param>
        /// <param name="columnWidth">Larghezza da assegnare alla colonna</param>
        /// <returns>Proprietà della colonna</returns>
        private HeaderProperty GetHeaderColumn(String columnName, int columnWidth, String originalColumnName)
        {

            return new HeaderProperty()
            {
                ColumnName = columnName,
                OriginalName = originalColumnName,
                ColumnSize = columnWidth,
                Export = true
            };

        }

    }
}
