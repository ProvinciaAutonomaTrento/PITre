using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocsPaVO.Report;
using BusinessLogic.Reporting.Behaviors.DataExtraction;
using BusinessLogic.Reporting.Behaviors.ReportGenerator;
using System.Data;
using BusinessLogic.Reporting.Exceptions;
using BusinessLogic.Documenti;
using DocsPaVO.utente;
using DocsPaVO.Report;

namespace BusinessLogic.Reporting
{
    /// <summary>
    /// Classe per la generazione del report sulle verifiche in fase di presa in carico
    /// </summary>
    [ReportGeneratorAttribute(Name = "Verifiche in fase di presa in carico delle istanze", ContextName = "ReportConservazione", Key = "ReportVerificheFaseInPresa")]
    class ReportVerificheFaseInPresaGeneratorCommand:ReportGeneratorCommand
    {
        /// <summary>
        /// Questo report non prevede la possibilità di selezionare le colonne da esportare
        /// </summary>
        /// <returns>Null</returns>
        protected override HeaderColumnCollection GetExportableFieldsCollection()
        {
            return null;
        }

        protected override HeaderColumnCollection GenerateReportHeader(System.Data.DataSet dataSet, HeaderColumnCollection fieldsToExport)
        {
            HeaderColumnCollection header = base.GenerateReportHeaderFromDataSet(dataSet);

            // Impostazione proprietà delle colonne
            // Id istanza
            header["ID_ISTANZA"].ColumnSize = 80;
            header["ID_ISTANZA"].ColumnName = "Id Istanza";
            header["ID_ISTANZA"].DataType = HeaderProperty.ContentDataType.Integer;

            // data invio 
            header["DATA_INVIO"].ColumnSize = 70;
            header["DATA_INVIO"].ColumnName = "Data Invio";
            header["DATA_INVIO"].DataType = HeaderProperty.ContentDataType.String;

            // numero documenti
            header["N_DOCUMENTI"].ColumnSize = 80;
            header["N_DOCUMENTI"].ColumnName = "N. documenti";
            header["N_DOCUMENTI"].DataType = HeaderProperty.ContentDataType.Integer;

            // numero documenti validi
            header["N_DOCUMENTI_VALIDI"].ColumnSize = 80;
            header["N_DOCUMENTI_VALIDI"].ColumnName = "N. documenti validi";
            header["N_DOCUMENTI_VALIDI"].DataType = HeaderProperty.ContentDataType.Integer;

            // numero documenti con formato non valido
            header["N_DOCUMENTI_FORM_NVALIDO"].ColumnSize = 80;
            header["N_DOCUMENTI_FORM_NVALIDO"].ColumnName = "N. documenti con formato non valido";
            header["N_DOCUMENTI_FORM_NVALIDO"].DataType = HeaderProperty.ContentDataType.Integer;

            // numero documenti con firma non valida
            header["N_DOCUMENTI_FIRMA_NVALIDA"].ColumnSize = 80;
            header["N_DOCUMENTI_FIRMA_NVALIDA"].ColumnName = "N. documenti con firma non valida";
            header["N_DOCUMENTI_FIRMA_NVALIDA"].DataType = HeaderProperty.ContentDataType.Integer;
           
            // numero documenti con marca non valida
            header["N_DOCUMENTI_MARCA_NVALIDA"].ColumnSize = 80;
            header["N_DOCUMENTI_MARCA_NVALIDA"].ColumnName = "N. documenti con marca non valida";
            header["N_DOCUMENTI_MARCA_NVALIDA"].DataType = HeaderProperty.ContentDataType.Integer;

            // numero documenti non conforme alla policy
            header["N_DOCUMENTI_PNONCONFORME"].ColumnSize = 80;
            header["N_DOCUMENTI_PNONCONFORME"].ColumnName = "N. documenti non conforme alla policy";
            header["N_DOCUMENTI_PNONCONFORME"].DataType = HeaderProperty.ContentDataType.Integer;

            //verifica di leggibilità della dimensione campione
            header["VER_LEGG_DIM_CAMP"].ColumnSize = 80;
            header["VER_LEGG_DIM_CAMP"].ColumnName = "Verifica leggibilità - dimensione campione";
            header["VER_LEGG_DIM_CAMP"].DataType = HeaderProperty.ContentDataType.Integer;

            //esito della verifica di leggibilità 
            header["VER_LEGG_ESITO"].ColumnSize = 80;
            header["VER_LEGG_ESITO"].ColumnName = "Verifica di leggibilità - esito";
            header["VER_LEGG_ESITO"].DataType = HeaderProperty.ContentDataType.Integer;
                         
            
            return header;
        }


    }
}
