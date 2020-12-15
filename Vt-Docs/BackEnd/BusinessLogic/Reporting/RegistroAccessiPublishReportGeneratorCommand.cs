using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BusinessLogic.Reporting.Behaviors.DataExtraction;
using BusinessLogic.Reporting.Behaviors.ReportGenerator;
using BusinessLogic.Reporting.Exceptions;
using DocsPaVO.Report;
using log4net;

namespace BusinessLogic.Reporting
{
    [ReportGenerator(Name = "Registro accessi per pubblicazione", ContextName = "RegistroAccessiPublish", Key = "RegistroAccessiPublish")]
    public class RegistroAccessiPublishReportGeneratorCommand : ReportGeneratorCommand
    {
        private ILog logger = LogManager.GetLogger(typeof(RegistroAccessiPublishReportGeneratorCommand));

        protected override HeaderColumnCollection GetExportableFieldsCollection()
        {
            throw new NotImplementedException();
        }

        protected override PrintReportResponse GenerateReport(PrintReportRequest request)
        {
            IReportGeneratorBehavior formatGenerator = base.CreateReportGenerator(request);
            IReportDataExtractionBehavior reportDataExtractor = new DatasetReportDataExtractionBehavior();

            return base.GenerateReport(request, reportDataExtractor, formatGenerator);
        }

        protected override HeaderColumnCollection GenerateReportHeader(DataSet dataSet, HeaderColumnCollection fieldsToExport)
        {
            logger.Debug("BEGIN");
            HeaderColumnCollection header = new HeaderColumnCollection();

            // Creo le colonne base
            #region Creazione colonne
            header.Add(new HeaderProperty()
            {
                ColumnName = "PROGRESSIVO",
                OriginalName = "Progressivo",
                ColumnSize = 70,
                Export = true,
                DataType = HeaderProperty.ContentDataType.String
            });
            header.Add(new HeaderProperty()
            {
                ColumnName = "DESCRIZIONE",
                OriginalName = "DESCRIZIONE",
                ColumnSize = 120,
                Export = true,
                DataType = HeaderProperty.ContentDataType.String
            });
            header.Add(new HeaderProperty()
            {
                ColumnName = "UFFICIO",
                OriginalName = "UFFICIO",
                ColumnSize = 140,
                Export = true,
                DataType = HeaderProperty.ContentDataType.String
            });
            #endregion

            // Aggiungo le colonne relative alla tipologia in esame
            this.GenerateAdditionalColumns(dataSet, header);

            logger.Debug("END");
            return header;
        }

        protected override IEnumerable<ReportMapRowProperty> GenerateReportRows(DataSet dataSet, HeaderColumnCollection reportHeader)
        {
            logger.Debug("BEGIN");

            // Lista delle righe del report
            List<ReportMapRowProperty> rows = new List<ReportMapRowProperty>();

            // Riga in corso di generazione
            ReportMapRowProperty row = null;
            // Id del fascicolo
            String folderId = String.Empty;

            foreach (DataRow dataRow in dataSet.Tables[0].Rows)
            {
                if (!String.IsNullOrEmpty(dataRow["ID_PROJECT"].ToString()))
                {
                    if (String.IsNullOrEmpty(folderId) || !dataRow["ID_PROJECT"].ToString().Equals(folderId))
                    {
                        // Se folderId non è valorizzato sto analizzando la prima riga del dataset
                        // Se il valore del campo ID_PROJECT del dataset non coincide con folderId sto analizzando una nuova riga
                        folderId = dataRow["ID_PROJECT"].ToString();
                        row = this.GenerateNewRow(dataRow, reportHeader);
                        rows.Add(row);
                    }
                    else
                    {
                        // Se il valore del campo ID_PROJECT coincide con folderId devo aggiungere il valore dell'i-esimo campo profilato
                        // alla riga del report
                        this.UpdateRow(row, dataRow, reportHeader);
                    }
                }
            }

            logger.Debug("END");
            return rows;
        }

        private void GenerateAdditionalColumns(DataSet dataSet, HeaderColumnCollection header)
        {
            logger.Debug("BEGIN");

            String firstFolderId = string.Empty;

            foreach (DataRow dataRow in dataSet.Tables[0].Rows)
            {
                if (string.IsNullOrEmpty(dataRow["ID_PROJECT"].ToString()))
                {
                    // Le prime righe con id_project = null contengono solo i campi profilati della tipologia
                    if (dataRow["NOME_CAMPO"] != null && dataRow["NOME_CAMPO"].ToString().ToUpper() != "PROGRESSIVO")
                    {
                        header.Add(new HeaderProperty() { ColumnName = dataRow["NOME_CAMPO"].ToString(), OriginalName = dataRow["NOME_CAMPO"].ToString(), ColumnSize = 80 });
                        logger.Debug("Aggiunta colonna " + dataRow["NOME_CAMPO"].ToString());
                    }
                }
                else
                {
                    // Se il valore della riga cambia sono passato all'elemento successivo e devo fermarmi
                    break;
                }
            }

            logger.Debug("END");
        }

        private ReportMapRowProperty GenerateNewRow(DataRow dataRow, HeaderColumnCollection reportHeader)
        {
            logger.Debug("BEGIN");

            ReportMapRowProperty row = new ReportMapRowProperty();

            if(dataRow["NOME_CAMPO"].ToString().ToUpper() == "PROGRESSIVO")
            {
                row.Columns.Add(this.GenerateHeaderColumn(dataRow["VALORE_CAMPO"].ToString(), "PROGRESSIVO", "Progressivo"));
            }
            row.Columns.Add(this.GenerateHeaderColumn(dataRow["DESCRIZIONE"].ToString(), "DESCRIZIONE", "DESCRIZIONE"));
            row.Columns.Add(this.GenerateHeaderColumn(dataRow["UFFICIO"].ToString(), "UFFICIO", "UFFICIO"));

            logger.Debug("END");

            return row;
        }

        private void UpdateRow(ReportMapRowProperty row, DataRow dataRow, HeaderColumnCollection reportHeader)
        {
            logger.Debug("BEGIN");

            // Da eseguire su tutti i campi eccetto il campo "Progressivo"
            if (dataRow["NOME_CAMPO"].ToString().ToUpper() != "PROGRESSIVO")
            {
                // Controllo esistenza nell'header
                if (reportHeader[dataRow["NOME_CAMPO"].ToString()] != null)
                {
                    // Controllo esistenza valore nella riga
                    if (row[dataRow["NOME_CAMPO"].ToString()] == null)
                    {
                        row.Columns.Add(this.GenerateHeaderColumn(dataRow["VALORE_CAMPO"].ToString(), dataRow["NOME_CAMPO"].ToString(), dataRow["NOME_CAMPO"].ToString()));
                    }
                }
            }

            logger.Debug("END");
        }

        private ReportMapColumnProperty GenerateHeaderColumn(string value, string columnName, String originalName)
        {
            return new ReportMapColumnProperty()
            {
                OriginalName = originalName,
                ColumnName = columnName,
                Value = value
            };
        }
    }
}
