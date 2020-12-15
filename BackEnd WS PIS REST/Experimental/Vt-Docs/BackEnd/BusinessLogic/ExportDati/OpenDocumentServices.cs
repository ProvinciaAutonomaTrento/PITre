using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using AODL.Document.Content.Tables;
using AODL.Document.Content.Text;
using AODL.Document.SpreadsheetDocuments;
using AODL.Document.Styles;
using AODL.Document.TextDocuments;
using DocsPaVO.documento;
using log4net;
using BusinessLogic.ExportDati;

namespace OpenDocument
{
    public class OpenDocumentServices
    {
        private static ILog logger = LogManager.GetLogger(typeof(ExportDatiManager));
        private SpreadsheetDocument document;
        private Table table;
        private String title;
        private int currentRow;
        private String filePath;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        public OpenDocumentServices(String filePath)
        {
            this.filePath = filePath;
            this.document = new SpreadsheetDocument();
            document.New();
            //  document.SaveTo(filePath);
            //  this.document.Load(filePath);
            this.table = new Table(document, "Foglio di lavoro", "");
            this.document.TableCollection.Add(table);
            this.title = "";
            currentRow = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="title"></param>
        public void SetTitle(String title)
        {
            Cell titolo = new Cell(document, "cell001");
            titolo.OfficeValueType = "string";
            Paragraph titoloParagraph = ParagraphBuilder.CreateSpreadsheetParagraph(document);
            FormatedText fText = new FormatedText(document, "T1", title);
            fText.TextStyle.TextProperties.Bold = "bold";
            fText.TextStyle.TextProperties.FontSize = "20pt";
            titoloParagraph.TextContent.Add(fText);
            titolo.Content.Add(titoloParagraph);
            table.Rows.Add(new Row(table));
            table.Rows[0].Cells.Add(titolo);
            this.title = title;
            currentRow++;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="subtitle"></param>
        public void SetSubtitle(String subtitle)
        {
            if (!title.Equals(""))
            {
                Cell sottoTitolo = new Cell(document, "cell002");
                sottoTitolo.OfficeValueType = "string";
                Paragraph sottoTitoloParagraph = ParagraphBuilder.CreateSpreadsheetParagraph(document);
                FormatedText fText = new FormatedText(document, "T2", subtitle);
                fText.TextStyle.TextProperties.Bold = "bold";
                fText.TextStyle.TextProperties.FontSize = "15pt";
                sottoTitoloParagraph.TextContent.Add(fText);
                sottoTitolo.Content.Add(sottoTitoloParagraph);
                table.Rows.Add(new Row(table));
                table.Rows[1].Cells.Add(sottoTitolo);
                currentRow++;
            }
            else
            {
                throw new Exception("Try to insert subtitle before title, please insert title first");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="row"></param>
        private void SetColumnsName(DataRow row)
        {
            table.Rows.Add(new Row(table));
            foreach (String columnName in row.ItemArray)
            {
                Cell columnHeader = new Cell(document, "cell003");
                columnHeader.OfficeValueType = "string";
                columnHeader.CellStyle.CellProperties.Border = Border.HeavySolid;
                columnHeader.CellStyle.CellProperties.BackgroundColor = "#D0B1A1";
                Paragraph titoloParagraph = ParagraphBuilder.CreateSpreadsheetParagraph(document);
                FormatedText fText = new FormatedText(document, "T3", columnName);
                fText.TextStyle.TextProperties.Bold = "bold";
                fText.TextStyle.TextProperties.FontSize = "10pt";
                titoloParagraph.TextContent.Add(fText);
                columnHeader.Content.Add(titoloParagraph);
                table.Rows[currentRow].Cells.Add(columnHeader);
            }
            currentRow++;

        }



        /// <summary>
        /// First Row of data must contains columns' names
        /// </summary>
        /// <param name="data"></param>
        public void SetData(DataSet data)
        {
            int dataRows = 0;
            if (data.Tables[0].Rows[0] != null)
            {
                SetColumnsName(data.Tables[0].Rows[0]);

                foreach (DataRow row in data.Tables[0].Rows)
                {
                    if (dataRows != 0)
                    {
                        table.Rows.Add(new Row(table));
                        foreach (String columnName in row.ItemArray)
                        {
                            Cell columnItem = new Cell(document, "cell004");
                            columnItem.OfficeValueType = "string";
                            columnItem.CellStyle.CellProperties.Border = Border.HeavySolid;
                            //columnItem.CellStyle.CellProperties.BackgroundColor = "#D0B1A1";
                            Paragraph titoloParagraph = ParagraphBuilder.CreateSpreadsheetParagraph(document);
                            FormatedText fText = new FormatedText(document, "T4", columnName);
                            fText.TextStyle.TextProperties.Bold = "bold";
                            fText.TextStyle.TextProperties.FontSize = "10pt";
                            titoloParagraph.TextContent.Add(fText);
                            columnItem.Content.Add(titoloParagraph);
                            table.Rows[currentRow].Cells.Add(columnItem);
                        }
                        currentRow++;
                    }
                    dataRows++;
                }

            }
            else
            {
                throw new Exception("Columns' names not found");
            }


        }

        /// <summary>
        /// 
        /// </summary>
        public FileDocumento SaveAndExportData()
        {

            FileDocumento file = new FileDocumento();
            bool retValue = false;
            document.SaveTo(filePath);

            //Codice per generare l'export del file

            try
            {
                //string temporaryXSLFilePath = string.Empty;
                StreamWriter writer = null;
                StringBuilder sb = new StringBuilder();

                //Salva e chiudi il file
                //temporaryXSLFilePath = HttpContext.Current.Server.MapPath(filePath);
                //Crea il file
                FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                if (stream != null)
                {
                    byte[] contentODS = new byte[stream.Length];
                    stream.Read(contentODS, 0, contentODS.Length);
                    stream.Flush();
                    stream.Close();
                    stream = null;
                    file.content = contentODS;
                    file.length = contentODS.Length;
                    file.estensioneFile = "ods";
                    file.name = "ExportDocumenti";
                    file.contentType = "application/vnd.oasis.opendocument.spreadsheet";
                }
                retValue = true;
            }
            catch (Exception e)
            {
                // this.file = null;
                logger.Debug("Errore esportazione documenti : " + e.Message);
            }

            File.Delete(filePath);

            return file;
        }

        static void Main(String[] args)
        {
            OpenDocumentServices ODS = new OpenDocumentServices("C:\\prova.ods");
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();
            dataSet.Tables.Add(dataTable);

            String[] nomiColonne = new String[2];
            nomiColonne[0] = "aaaaaaaaaaaaaaa";
            nomiColonne[1] = "bbbbbbbbbbbbbbb";
            dataSet.Tables[0].Columns.Add();
            dataSet.Tables[0].Columns.Add();
            dataSet.Tables[0].Rows.Add(nomiColonne);

            String[] valoriColonne = new String[2];
            valoriColonne[0] = "iaseubfvdpaiuwhbvpu";
            valoriColonne[1] = "OIHPVOWJERPIHFPWE";
            dataSet.Tables[0].Rows.Add(valoriColonne);

            ODS.SetTitle("TITOLO");
            ODS.SetSubtitle("SOTTOTITOLO");
            ODS.SetData(dataSet);
            ODS.SaveAndExportData();
        }

    }
}
