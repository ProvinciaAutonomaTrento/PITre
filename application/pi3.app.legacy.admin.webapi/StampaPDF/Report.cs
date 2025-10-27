// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.Import;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StampaPDF
{
    public class Report
    {
        DocumentPDF docPDF = null;
        StampaVO.Document docTemplate = null;

        public Report(System.IO.FileStream fileXML, string path) : this(fileXML, path, null, null)
        {

        }

        public Report(System.IO.FileStream fileXML, string path, string titolo) : this(fileXML, path, titolo, null)
        {

        }

        public Report(System.IO.FileStream fileXML, string path, string titolo, string overrideFooter)
        {
            try
            {//Esegue il parsing del file XML e riempie un apposito ValueObject
                docTemplate = StampaXML.CreateDocTemplate(fileXML, path);
                if (!string.IsNullOrEmpty(titolo)) docTemplate.page.headerPage.text = titolo;
            }
            catch (ReportException re)
            {//Si è verificato un errore nel parsing del file XML
             //Il dettaglio è contenuto nel messaggio che viene propagato.
                throw re;
            }
            catch (Exception e)
            {//Si è verificato un errore nel parsing del file XML
             //Il dettaglio è contenuto nel messaggio che viene propagato.
                throw new ReportException(ErrorCode.XmlFileNotFound, "Errore durante l'esame del file XML: " + e.Message);
            }
            try
            {
                docPDF = StampaPDF.createDocPDF(docTemplate, overrideFooter);
            }
            catch (ReportException re)
            {//Si è verificato un errore nella creazione del file PDF
             //Il dettaglio è contenuto nel messaggio che viene propagato.
                throw re;
            }
            catch (Exception e)
            {//Si è verificato un errore nella creazione del file PDF
             //Il dettaglio è contenuto nel messaggio che viene propagato.
                throw new ReportException(ErrorCode.BadPDFFile, "Errore durante la creazione del file PDF: " + e.Message);
            }
        }

        public void appendParagraph(string target, string testo, bool newPage)
        { //Sostituisce in testo specificato a quello contenuto nel paragrafo target del file XML
          //Appende il paragrafo individuato dal target al file PDF.
          //Il paragrafo viene stampato in una nuova pagina se newPage è true.
          //Il paragrafo sarà formattato usando la specifica contenuta nel file XML;
          //i parametri indicati saranno sostituiti con il contenuto dell'array parameters.
            try
            {
                if (docPDF == null)
                    throw new ReportException(ErrorCode.NullPDFFile, "Impossibile aggiungere informazioni sul file PDF. Il file non esiste");
#if false   // metodi del documenti di ItextSharp
                if (!docPDF.IsOpen())
                    docPDF.Open();
                docPDF = StampaPDF.appendData(testo, docPDF, docTemplate, target, newPage);

#endif
            }
            catch (ReportException re)
            {
                throw re;
            }
            catch (Exception e)
            {
                throw new ReportException(ErrorCode.BadPDFFile, "Impossibile aggiungere informazioni sul file PDF. " + e.Message);
            }
        }

        public void appendParagraph(String target, Hashtable parameters, bool newPage)
        { //Appende il paragrafo individuato dal target al file PDF.
          //Il paragrafo viene stampato in una nuova pagina se newPage è true.
          //Il paragrafo sarà formattato usando la specifica contenuta nel file XML;
          //i parametri indicati saranno sostituiti con il contenuto dell'array parameters.
            try
            {
                if (docPDF == null)
                    throw new ReportException(ErrorCode.NullPDFFile, "Impossibile aggiungere informazioni sul file PDF. Il file non esiste");
#if false   // metodi della document di itextsharp
                if (!docPDF.IsOpen())
                    docPDF.Open();
                docPDF = StampaPDF.appendData(parameters, docPDF, docTemplate, target, newPage);

#endif
            }
            catch (ReportException re)
            {
                throw re;
            }
            catch (Exception e)
            {
                throw new ReportException(ErrorCode.BadPDFFile, "Impossibile aggiungere informazioni sul file PDF. " + e.Message);
            }
        }

        public void appendTable(String target, DataTable dt, bool newPage)
        { //Appende la tabella individuata dal target al file PDF.
          //La tabella sarà formattata usando la specifica contenuta nel file XML e
          //popolata con i dati contenuti nel DataTable.
          //La tabella viene stampata in una nuova pagina se newPage è true
            try
            {
                if (dt == null)
                    return;
                if (docPDF == null)
                    throw new ReportException(ErrorCode.NullPDFFile, "Impossibile aggiungere informazioni sul file PDF. Il file non esiste");
#if false   // metodi della classe Document di itextsharp
                if (!docPDF.IsOpen())
                    docPDF.Open();
                docPDF = StampaPDF.appendData(dt, docPDF, docTemplate, target, newPage);

#endif
            }
            catch (ReportException re)
            {
                throw re;
            }
            catch (Exception e)
            {
                throw new ReportException(ErrorCode.BadPDFFile, "Impossibile aggiungere informazioni sul file PDF. " + e.Message);
            }
        }

        public MemoryStream getStream()
        {
#if false  // metodi di classe Document di Itextsharp
            if (docPDF != null)
                return docPDF.memoryStream;

#endif
            return null;
        }

        public MemoryStream close()
        {
            if (docPDF == null)
                throw new ReportException(ErrorCode.BadPDFFile, "Impossibile chiudere il file. Il file non è aperto");
            try
            {
#if false   // metodi della classe document di itextsharp
                if (docPDF.IsOpen())
                    docPDF.Close();

#endif
            }
            catch (Exception e)
            {

                //Si è verificato un errore nella creazione del file PDF
                //Il dettaglio è contenuto nel messaggio che viene propagato.
                throw new ReportException(ErrorCode.BadPDFFile, e.Message);

            }
            return docPDF.memoryStream;

        }

    }
}
