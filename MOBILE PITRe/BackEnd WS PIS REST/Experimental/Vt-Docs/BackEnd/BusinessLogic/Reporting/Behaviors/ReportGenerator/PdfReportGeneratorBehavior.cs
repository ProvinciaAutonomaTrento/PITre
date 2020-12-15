using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DocsPaVO.Report;
using DocsPaVO.documento;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;

namespace BusinessLogic.Reporting.Behaviors.ReportGenerator
{
    /// <summary>
    /// Questa classe consente di realizzare reportistica in PDF
    /// </summary>
    public class PdfReportGeneratorBehavior : IReportGeneratorBehavior
    {
        /// <summary>
        /// Metodo per la generazione di un report PDF
        /// </summary>
        /// <param name="request">Request con le informazioni sul report da generare</param>
        /// <param name="reports">Report da esportare</param>
        /// <returns>Content del report in formato PDF</returns>
        public FileDocumento GenerateReport(PrintReportRequest request, List<DocsPaVO.Report.Report> reports)
        {
            // Risultato da restituire
            FileDocumento document = null;

            // Stream in cui produrre il file PDF
            using (MemoryStream stream = new MemoryStream())
            {
                // Creazione del documento PDF
                Document pdfDocument = new Document(PageSize.A4.Rotate(), 10, 10, 90, 10);

                // Per gestione margini nella stampa del registro di conservazione (GM 04/07/2013)
                if (request.ReportKey == "StampaConservazione")
                {
                    bool dummy = pdfDocument.SetMargins(10, 10, 20, 10);
                }

                // Creazione del writer per la generazione delle pagine
                PdfWriter writer = PdfWriter.GetInstance(pdfDocument, stream);

                // Aggiunta numerazione pagina nel footer
                Phrase phr = new Phrase("Pagina ");
                HeaderFooter footer = new HeaderFooter(phr, true);
                pdfDocument.Footer = footer;

                // Apertura del documento
                pdfDocument.Open();

                // Lo stream non deve essere chiuso
                writer.CloseStream = false;

                // Aggiunta del contenuto alle pagine
                foreach (var report in reports)
                    this.AddPage(pdfDocument, report);

                // Chiusura del documento
                pdfDocument.Close();

                // Generazione del risultato dell'export
                document = new FileDocumento();
                document.name = String.Format("Report_{0}.pdf", DateTime.Now.ToString("dd-MM-yyyy"));
                document.path = String.Empty;
                document.fullName = document.name;
                document.contentType = "application/octet-stream";
                document.content = stream.GetBuffer();

            }
            
            return document;
        }

        /// <summary>
        /// Metodo per l'aggiunta di una pagina al report
        /// </summary>
        /// <param name="pdfDocument">Documento per cui generare la pagina</param>
        /// <param name="report">Report da aggiungere</param>
        private void AddPage(Document pdfDocument, DocsPaVO.Report.Report report)
        {
            // Aggiunta nuova pagina
            pdfDocument.NewPage();

            // Aggiunta dell'intestazione alla pagina
            this.AddHeader(pdfDocument, report);

            // Aggiunta della tabella con i dati solo se ci sono dati da esportare
            if(report.ReportHeader.Count > 0 && report.ReportMapRow.Rows.Count > 0)
                this.AddReportData(pdfDocument, report);
            
        }

        /// <summary>
        /// Metodo per l'aggiunta dell'header ad un report
        /// </summary>
        /// <param name="pdfDocument">Documento a cui aggiungere l'intestazione</param>
        /// <param name="report">Report da cui estrarre i dati</param>
        private void AddHeader(Document pdfDocument, DocsPaVO.Report.Report report)
        {
            // Aggiunta di titolo, sottotitolo e summary
            pdfDocument.Add(new Paragraph(this.AddReportTitle(report.Title)));
            pdfDocument.Add(new Paragraph(this.AddReportSubtitle(report.Subtitle)));
            pdfDocument.Add(new Paragraph(this.AddReportAdditionalInformation(report.AdditionalInformation)));
            pdfDocument.Add(new Paragraph(this.AddReportSummary(report.Summary)));

        }

        /// <summary>
        /// Metodo per l'aggiunta dei dati di un report
        /// </summary>
        /// <param name="pdfDocument">Documento a cui aggiungere il report</param>
        /// <param name="report">Oggetto da cui estrarre i dati da inserire</param>
        private void AddReportData(Document pdfDocument, DocsPaVO.Report.Report report)
        {
            // Creazione di una tabella in cui inserire i dati del report
            PdfPTable pdfTable = new PdfPTable(report.ReportHeader.Count);
            
            // Impostazione delle proprietà della tabella
            pdfTable.DefaultCell.Padding = 3;
            pdfTable.WidthPercentage = 100;
            pdfTable.SpacingAfter = 20f;
            pdfTable.SpacingBefore = 30f;

            // Aggiunta dell'header del report
            this.AddReportHeader(pdfTable, report.ReportHeader);

            // Aggiunta dei dati del report
            this.AddData(pdfTable, report.ReportHeader, report.ReportMapRow.Rows);

            // Aggiunta della tabella al documento
            pdfDocument.Add(pdfTable);

        }

        /// <summary>
        /// Metodo per l'aggiunta dell'header alla tabella del report
        /// </summary>
        /// <param name="pdfTable">Tabella a cui aggiungere l'header</param>
        /// <param name="header">Proprietà dell'header da aggiungere</param>
        private void AddReportHeader(PdfPTable pdfTable, HeaderColumnCollection header) 
        {
            // Aggiunta delle colonne dell'header
            foreach (HeaderProperty col in header)
            {
                PdfPCell cell = new PdfPCell(this.GetHeaderPhrase(col.ColumnName));
                //if (col.ColumnSize != 0)
                //    cell.Width = float.Parse(col.ColumnSize.ToString());

                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;

                pdfTable.AddCell(cell);
            }

            // Terminazione header
            pdfTable.HeaderRows = 1; 

        }

        /// <summary>
        /// Metodo per l'aggiunta delle righe al report
        /// </summary>
        /// <param name="pdfTable">Tabella a cui aggiungere i dati</param>
        /// <param name="rows">Righe da aggiungere</param>
        private void AddData(PdfPTable pdfTable, HeaderColumnCollection headerCollection, List<ReportMapRowProperty> rows) 
        {
            // Bordo spesso 1 e testo centrato
            pdfTable.DefaultCell.BorderWidth = 1;

            // Aggiunta delle colonne dell'header
            foreach (ReportMapRowProperty row in rows)
                this.AddRow(pdfTable, row, headerCollection);
            
            
        }

        /// <summary>
        /// Metodo per l'aggiunta di una riga al report
        /// </summary>
        /// <param name="pdfTable">Tabella a cui aggiungere una riga</param>
        /// <param name="row">Riga da analizzare ed aggiungere</param>
        private void AddRow(PdfPTable pdfTable, ReportMapRowProperty row, HeaderColumnCollection headerCollection)
        {
            // Aggiunta delle colonne
            //foreach (var column in row.Columns)
            //    pdfTable.AddCell(this.GetDataPhrase(column.Value));
            foreach (var r in headerCollection)
            {
                if(row[r.OriginalName] != null)
                    pdfTable.AddCell(this.GetDataPhrase(row[r.OriginalName].Value));
                else
                    pdfTable.AddCell(this.GetDataPhrase(String.Empty));
            }
            
        }

        /// <summary>
        /// Metodo per l'aggiunta del titolo ad un report
        /// </summary>
        /// <param name="title">Titolo da impostare</param>
        /// <returns>Titolo formattato</returns>
        private Phrase AddReportTitle(String title)
        {
            return new Phrase(title, FontFactory.GetFont(FontFactory.HELVETICA, 16, Color.BLACK));
        }

        /// <summary>
        /// Metodo per l'aggiunta del sottotitolo ad un report
        /// </summary>
        /// <param name="subtitle">Sottotitolo da aggiungere</param>
        /// <returns>Sottotitolo dormattato</returns>
        private Phrase AddReportSubtitle(String subtitle)
        {
            return new Phrase(subtitle, FontFactory.GetFont(FontFactory.HELVETICA, 12, Color.BLACK));
        }

        /// <summary>
        /// Metodo per l'aggiunta del summary ad un report
        /// </summary>
        /// <param name="summary">Summary da aggiungere</param>
        /// <returns>Summary formattato</returns>
        private Phrase AddReportSummary(String summary)
        {
            return new Phrase(summary, FontFactory.GetFont(FontFactory.HELVETICA, 10, Color.BLACK));
        }

        /// <summary>
        /// Metodo per l'aggiunta di informazioni aggiutive al report
        /// </summary>
        /// <param name="additionalInformation">Informazioni da aggiungere</param>
        /// <returns>Informazini formattate</returns>
        private Phrase AddReportAdditionalInformation(String additionalInformation)
        {
            return new Phrase(additionalInformation, FontFactory.GetFont(FontFactory.HELVETICA, 10, Color.BLACK));
        }

        /// <summary>
        /// Metodo per la formattazione della colonna dell'header
        /// </summary>
        /// <param name="columnName">Nome da assegnare alla colonna</param>
        /// <returns>Cella formattata</returns>
        private PdfPCell GetHeaderPhrase(string columnName)
        {
            Font font = FontFactory.GetFont("Arial", BaseFont.CP1252, BaseFont.EMBEDDED, 8, Font.BOLD, Color.BLACK);
            PdfPCell cell = new PdfPCell(new Phrase(columnName, font));
            cell.BackgroundColor = new Color(192, 192, 192);
            cell.Border = 1;
            cell.BorderWidth = 2;
            cell.BorderColor = Color.BLACK;
            
            return cell;
            
        }

        /// <summary>
        /// Metodo per la generazione di una cella PDF contenente dati
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public PdfPCell GetDataPhrase(String columnName) 
        {
            PdfPCell cell = new PdfPCell(new Phrase(columnName, FontFactory.GetFont(FontFactory.HELVETICA, 8, Color.BLACK)));
            cell.BackgroundColor = Color.WHITE;
            cell.Border = 1;
            cell.BorderWidth = 0.5f;
            cell.BorderColor = Color.BLACK;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            return cell;
        }

    }
}
