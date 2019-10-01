using DocsPaVO.Report;
using System.Collections.Generic;
using BusinessLogic.Reporting.Behaviors.DataExtraction;
using BusinessLogic.Reporting.Behaviors.ReportGenerator;
using System.Data;

namespace BusinessLogic.Reporting
{
    [ReportGenerator(Name = "Report Notiche Pec", ContextName = "NotifichePec", Key = "NotificheSpedizione")]
    public class NotifichePecReportGenerator : ReportGeneratorCommand
    {
        protected override DocsPaVO.Report.HeaderColumnCollection GetExportableFieldsCollection()
        {
            return null;
        }

        protected override HeaderColumnCollection GenerateReportHeader(System.Data.DataSet dataSet, HeaderColumnCollection fieldsToExport)
        {
            HeaderColumnCollection header = base.GenerateReportHeaderFromDataSet(dataSet);
            header["tipo"].ColumnSize = 120;
            header["tipo"].ColumnName = "TIPO";
            header["tipo"].DataType = HeaderProperty.ContentDataType.String;

            header["destinatario"].ColumnSize = 120;
            header["destinatario"].ColumnName = "DESTINATARIO";
            header["destinatario"].DataType = HeaderProperty.ContentDataType.String;

            header["dettagli"].ColumnSize = 270;
            header["dettagli"].ColumnName = "DETTAGLI";
            header["dettagli"].DataType = HeaderProperty.ContentDataType.String;

            return header;
        }
        protected override System.Collections.Generic.IEnumerable<ReportMapRowProperty> GenerateReportRows(System.Data.DataSet dataSet, HeaderColumnCollection reportHeader)
        {            
                List<ReportMapRowProperty> rows = new List<ReportMapRowProperty>();

                //PALUMBO: modifica per fornire nell'export la descrizione del destinatario in caso di trasmissione per interoperabilita IS
                string dest = null;
                string pattern = "^(([a-zA-Z0-9_\\-\\.]+)@([a-zA-Z0-9_\\-\\.]+)\\.([a-zA-Z]{2,5}){1,25})+([;](([a-zA-Z0-9_\\-\\.]+)@([a-zA-Z0-9_\\-\\.]+)\\.([a-zA-Z]{2,5}){1,25})+)*$";
                DocsPaDB.Query_DocsPAWS.InteroperabilitaDatiCert iDc = new DocsPaDB.Query_DocsPAWS.InteroperabilitaDatiCert();

                foreach (DataRow row in dataSet.Tables[0].Rows)
                {
                    // Riga da aggiungere al report
                    ReportMapRowProperty reportRow = new ReportMapRowProperty();

                    // Creazione della riga
                    foreach (DataColumn dataColumn in row.Table.Columns)
                    {

                        // Se bisogna esportare il campo, viene aggiunta una colonna
                        if (reportHeader[dataColumn.ColumnName].Export)
                        {
                            //PALUMBO: modifica per fornire nell'export la descrizione del destinatario in caso di trasmissione per interoperabilita IS
                            if ((dataColumn.ColumnName.Equals("DESTINATARIO")) && (!System.Text.RegularExpressions.Regex.Match(row[dataColumn].ToString(), pattern).Success))
                            {
                                dest = iDc.GetDestinatarioPerIs(row[dataColumn].ToString());
                                reportRow.Columns.Add(new ReportMapColumnProperty()
                                {
                                    DataType = reportHeader[dataColumn.ColumnName].DataType,
                                    Value = dest,
                                    OriginalName = dataColumn.ColumnName
                                });
                            }
                            else if ((dataColumn.ColumnName.Equals("TIPO")) && (row[dataColumn].ToString().Contains("errore-consegna")))
                            {
                                reportRow.Columns.Add(new ReportMapColumnProperty()
                                {
                                    DataType = reportHeader[dataColumn.ColumnName].DataType,
                                    Value = "mancata-consegna",
                                    OriginalName = dataColumn.ColumnName
                                });
                            }
                            else
                                reportRow.Columns.Add(new ReportMapColumnProperty()
                                {
                                    DataType = reportHeader[dataColumn.ColumnName].DataType,
                                    Value = row[dataColumn].ToString(),
                                    OriginalName = dataColumn.ColumnName
                                });
                        }
                    }
                    rows.Add(reportRow);
                }

                return rows;
            
        }
    }
}