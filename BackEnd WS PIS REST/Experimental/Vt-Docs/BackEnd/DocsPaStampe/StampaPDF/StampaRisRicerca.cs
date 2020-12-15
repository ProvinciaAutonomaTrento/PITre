using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using System.Collections;

namespace StampaPDF
{
    /// <summary>
    /// Questa classe fornisce una funzione per la creazione del PDF
    /// composto da dati derivanti dall'esportazione dei risultati di una ricerca
    /// </summary>
    public class StampaRisRicerca
    {
        public DocsPaVO.documento.FileDocumento GetFileDocumento(
            string templateFileName,
            string titolo,
            string descrizioneAmm,
            string totRighe,
            DataTable tabella)
        {
            // 1. Apertura dello stream in lettura sul file del template
            FileStream fs = new FileStream(
                this.GetCompleteTemplatePath(templateFileName),
                FileMode.Open, FileAccess.Read);

            // 3. Creazione dell'oggetto per la stampa
            Report report = new Report(fs, this.GetSchemaPath(),null);

            // 4. Compilazione del primo paragrafo del documento (contiene: nome amministrazione,
            //    eventuale titolo del documento, numero di documenti trovati
            Hashtable pars;

            // 4.1 Impostazione descrizione amministrazione
            pars = new Hashtable();
            pars.Add("@amm", descrizioneAmm);
            report.appendParagraph("PAR_AMM", pars, false);

            // 4.2 Impostazione titolo
            pars = new Hashtable();
            pars.Add("@title", titolo);
            report.appendParagraph("PAR_TITLE", pars, false);

            // 4.3 Impostazione numero righe stampate
            pars = new Hashtable();
            pars.Add("@param0", totRighe);
            report.appendParagraph("PAR_TOT_RIGHE", pars, false);

            // 5. Aggiunta della tabella con le informazioni sugli oggetti
            report.appendTable("TABLE", tabella, false);

            // 6. Chiusura stream del template
            fs.Close();

            // 7. Prelevamento del memory stream generato
            MemoryStream ms = report.getStream();

            // 8. Chiusura del report generato
            report.close();

            // 9. Prelevamento del buffer dallo stream
            Byte[] content = ms.GetBuffer();

            // 10. Chiusura dello stream
            ms.Close();

            // 11. Creazione del FileDocumento da restituire
            DocsPaVO.documento.FileDocumento res = new DocsPaVO.documento.FileDocumento();

            // 12. Impostazione del content del documento
            res.content = content;

            // 13. Impostazione dell'estensione del file
            res.estensioneFile = "pdf";

            // 14. Impostazione del nome del file
            res.name = "export";

            // 15. Impostazione del full name del documento
            res.fullName = "export.pdf";

            // 16. Impostazione della grandezza del file
            res.length = (int)content.Length;

            // 17. Impostazione del mime-type
            res.contentType = "application/pdf";

            // 18. Restituzione del documento pdf generato
            return res;
        }

        /// <summary>
        /// Funzione per la creazione del path completo per il reperimento
        /// del file con la definizione del template per la stampa
        /// </summary>
        /// <param name="templateName">Il nome del file con la definizione del template</param>
        /// <returns>Il path completo</returns>
        private string GetCompleteTemplatePath(string templateName)
        {
            return AppDomain.CurrentDomain.BaseDirectory + "report/TemplateXML/" + templateName;

        }

        /// <summary>
        /// Funzione per la creazione del path per il reperimento del file con la definizione
        /// dello schema per la stampa
        /// </summary>
        /// <returns></returns>
        private string GetSchemaPath()
        {
            return AppDomain.CurrentDomain.BaseDirectory + "report/TemplateXML/XMLReport.xsd";
        }
    }

}
