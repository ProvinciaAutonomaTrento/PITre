using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DocsPaUtils;
using DocsPaVO.documento;
using DocsPaVO.filtri;
using DocsPaVO.utente;
using DocsPaDB.Query_DocsPAWS;
using BusinessLogic.Documenti;
using iTextSharp.text;
using iTextSharp.text.pdf;
using log4net;


namespace BusinessLogic.Conservazione
{
    public class EsibizioneManager
    {

        private static ILog logger = LogManager.GetLogger(typeof(EsibizioneManager));

        public string GeneraCertificazione(InfoUtente infoUtente, string idEsibizione)
        {

            string retVal;

            try
            {

                List<FiltroRicerca> filters = new List<FiltroRicerca>() {
                    new FiltroRicerca() {argomento="systemId", valore=idEsibizione}
                };

                //il proprietario del documento è l'utenteconservazione dell'amministrazione corrente:
                InfoUtente infoUC = this.GetInfoUtenteConservazione(infoUtente.idAmministrazione);
                
                if (!(infoUC != null))
                    infoUC = infoUtente;

                //reperimento del ruolo
                //Ruolo role = BusinessLogic.Utenti.UserManager.getRuoloByIdGruppo(infoUtente.idGruppo);
                Ruolo role = new Ruolo();
                role.idGruppo = "0";
                role.idAmministrazione = infoUtente.idAmministrazione;

                //contenuti documenti
                string amministrazione = Amministrazione.AmministraManager.AmmGetInfoAmmCorrente(infoUtente.idAmministrazione).Descrizione;
                string testoDoc = string.Empty;
                testoDoc += "Si attesta che per i documenti sottoelencati è stato correttamente eseguito il processo ";
                testoDoc += "di conservazione secondo le modalità previste dalla normativa vigente.";

                //creazione documento
                FileDocumento fd = BusinessLogic.Reporting.ReportGeneratorCommand.GetReport(
                    new DocsPaVO.Report.PrintReportRequest()
                    {
                        UserInfo = infoUC,
                        SearchFilters = filters,
                        ReportType = DocsPaVO.Report.ReportTypeEnum.PDF,
                        ReportKey = "CertificazioneEsibizione",
                        ContextName = "CertificazioneEsibizione",
                        Title = amministrazione,
                        SubTitle = "Certificazione Istanza di Esibizione numero "+idEsibizione,
                        AdditionalInformation = testoDoc
                    }).Document;

                //inserimento nel gestore documentale

                //apertura contesto transazionale
                using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
                {
                    if (infoUC != null && string.IsNullOrEmpty(infoUC.dst))
                        infoUC.dst = BusinessLogic.Utenti.UserManager.getSuperUserAuthenticationToken();

                    //creazione scheda documento
                    SchedaDocumento scheda = this.InitializeDocument(infoUC, idEsibizione);

                    //salvataggio documento
                    string docNumber = this.SaveDocument(infoUC, role, scheda, fd);

                    if (String.IsNullOrEmpty(docNumber))
                        throw new Exception("Errore durante la creazione del documento di certificazione");

                    logger.Debug(docNumber);

                    //chiusura transazione
                    transactionContext.Complete();

                    retVal = docNumber;
                }

                //modifica tipo_proto nella tabella profile
                if (!this.UpdateTipoProto(retVal))
                    throw new Exception("Errore nell'aggiornamento del tipo_proto");
            
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message, ex);
                retVal = string.Empty;

            }

            return retVal;

        }

        private SchedaDocumento InitializeDocument(InfoUtente infoUtente, string idEsibizione)
        {

            SchedaDocumento retVal = Documenti.DocManager.NewSchedaDocumento(infoUtente);

            //proprietà del documento
            retVal.appId = ((DocsPaVO.documento.Applicazione)BusinessLogic.Documenti.FileManager.getApplicazioni("PDF")[0]).application;
            retVal.userId = infoUtente.userId;
            retVal.idPeople = infoUtente.idPeople;
            retVal.typeId = "LETTERA";
            retVal.personale = "1";

            retVal.oggetto = new Oggetto() { descrizione = "Documento di Certificazione Istanza di Esibizione numero " + idEsibizione };

            return retVal;
        }

        private string SaveDocument(InfoUtente infoUtente, Ruolo role, SchedaDocumento schedaDoc, FileDocumento fileDoc)
        {

            //Salvataggio del documento
            DocsPaDocumentale.Documentale.DocumentManager docManager = new DocsPaDocumentale.Documentale.DocumentManager(infoUtente);

            //Salvataggio dell'oggetto
            schedaDoc = ProtoManager.addOggettoLocked(infoUtente.idAmministrazione, schedaDoc);

            Ruolo[] ruoliSuperiori;

            if (docManager.CreateDocumentoGrigio(schedaDoc, role, out ruoliSuperiori))
            {
                //Salvataggio del file associato al documento
                DocsPaVO.documento.FileRequest fileRequest = (DocsPaVO.documento.FileRequest)schedaDoc.documenti[0];

                fileRequest = BusinessLogic.Documenti.FileManager.putFile(fileRequest, fileDoc, infoUtente);
                if(fileRequest==null)
                    throw new ApplicationException("Si è verificato un errore nell'upload del documento di certificazione");

            }

            /*
            superiori = ruoliSuperiori;
            if (superiori == null)
                superiori = new Ruolo[] { };
             * */

            //restituzione numero documento
            return schedaDoc.docNumber;
        }

        private bool UpdateTipoProto(string docNumber)
        {
            bool retVal = false;

            Query query = InitQuery.getInstance().getQuery("U_REG_CONS_TIPO_PROTO");
            //tipo_proto da impostare per il documento
            string tipo = "N";
            query.setParam("tipo_proto", tipo);
            query.setParam("docnumber", docNumber);

            string commandText = query.getSQL();
            logger.Debug("QUERY - SQL: " + commandText);

            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                retVal = dbProvider.ExecuteNonQuery(commandText);

            }


            return retVal;


        }

        private InfoUtente GetInfoUtenteConservazione(string idAmm)
        {
            DocsPaVO.utente.Utente u = null;
            InfoUtente infoUC = new InfoUtente();

            try
            {
                //u = Utenti.UserManager.getUtente("UTENTECONSERVAZIONE", idAmm);
                DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();
                u = utenti.GetUtente("UTENTECONSERVAZIONE", idAmm, "CENTRO_SERVIZI");

                infoUC.idPeople = u.idPeople;
                infoUC.idAmministrazione = u.idAmministrazione;
                infoUC.codWorkingApplication = "CS";
                infoUC.idGruppo = "0";
                infoUC.idCorrGlobali = "0";
                infoUC.userId = "UTENTECONSERVAZIONE";
            }

            catch (Exception ex)
            {
                logger.Debug("Errore nel reperimento dell'utente di sistema - ", ex);
                infoUC = null;
            }

            return infoUC;

        }

        public FileDocumento CreaIndiceDoc(DocsPaVO.areaConservazione.InfoEsibizione infoEs, DocsPaVO.areaConservazione.ItemsEsibizione[] items)
        {
            // Risultato da restituire
            FileDocumento document = null;

            // Stream in cui produrre il file PDF
            using (MemoryStream stream = new MemoryStream())
            {
                // Creazione del documento PDF
                Document pdfDocument = new Document(PageSize.A4, 10, 10, 10, 10);

                PdfWriter writer = PdfWriter.GetInstance(pdfDocument, stream);
                pdfDocument.Open();
                logger.Debug(stream.Length);

                // Lo stream non deve essere chiuso
                writer.CloseStream = false;
                pdfDocument.NewPage();

                
                pdfDocument.Add(new Paragraph("Indice dei documenti", FontFactory.GetFont(FontFactory.HELVETICA, 16, Color.BLACK)));

                //intestazione con dati istanza
                Font fontheader = FontFactory.GetFont(FontFactory.HELVETICA, 12, Color.BLACK);
                pdfDocument.Add(new Paragraph(string.Format("Istanza di Esibizione n. {0}", infoEs.SystemID), fontheader));
                pdfDocument.Add(new Paragraph(string.Format("Descrizione: {0}", infoEs.Descrizione), fontheader));
                if (!string.IsNullOrEmpty(infoEs.Note))
                    pdfDocument.Add(new Paragraph(string.Format("Note: {0}", infoEs.Note), fontheader));


                //tabella con documenti
                PdfPTable table = new PdfPTable(6);
                table.WidthPercentage = 100;
                //table.LockedWidth = true;
                //float[] widths = new float[] { 50f, 100f, 300f, 200f };
                float[] widths = new float[] {70f, 100f, 80f, 220f, 120f, 50f};
                table.SetWidths(widths);
                table.SpacingAfter = 20f;
                table.SpacingBefore = 30f;
                table.DefaultCell.BorderWidth = 0;

                //header
                Font fontHeaderTable = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10, Color.BLACK);
                table.AddCell(this.addCell(new Phrase("Tipo", fontHeaderTable)));
                table.AddCell(this.addCell(new Phrase("ID/Segnatura", fontHeaderTable)));
                table.AddCell(this.addCell(new Phrase("Cod. Fasc.", fontHeaderTable)));
                table.AddCell(this.addCell(new Phrase("Oggetto", fontHeaderTable)));
                table.AddCell(this.addCell(new Phrase("Data creazione\n protocollazione", fontHeaderTable)));
                table.AddCell(this.addCell(new Phrase("Rapporto", fontHeaderTable)));

                foreach (DocsPaVO.areaConservazione.ItemsEsibizione itm in items)
                {
                    logger.Debug(itm.numProt_or_id);
                    logger.Debug(itm.relative_path);

                    string relativePathReport = Path.Combine(itm.ID_Conservazione, "rapporto_versamento.xml");
                    logger.DebugFormat("Path rapporto versmamento: {0}", relativePathReport);

                    //font per doc principale
                    //Font link = FontFactory.GetFont("Arial", 10, Font.UNDERLINE, new Color(0, 0, 255));
                    Font link = FontFactory.GetFont(FontFactory.HELVETICA, 10, Font.UNDEFINED, new Color(0, 0, 255));
                    Font font = FontFactory.GetFont(FontFactory.HELVETICA, 10, Color.BLACK);

                    //font per allegato
                    Font linkall = FontFactory.GetFont(FontFactory.HELVETICA, 9, Font.UNDEFINED, new Color(0, 0, 255));
                    Font fontall = FontFactory.GetFont(FontFactory.HELVETICA, 9, Color.BLACK);

                    // hyperlink al documento
                    Anchor anchor = new Anchor(itm.numProt_or_id, link);
                    anchor.Reference = itm.relative_path;
                    Phrase phrase = new Phrase();
                    phrase.Add(anchor);

                    // link al rapporto di versamento
                    Anchor reportAnchor = new Anchor("vedi", link);
                    reportAnchor.Reference = relativePathReport;
                    Phrase reportPhrase = new Phrase();
                    reportPhrase.Add(reportAnchor);               

                    //tipo (doc/all)
                    table.AddCell(this.addCell(new Phrase("Documento", font)));
                    // id/segnatura (con hyperlink)
                    table.AddCell(this.addCell(phrase));
                    // codice fascicolo
                    string codfasc = " ";
                    if (!string.IsNullOrEmpty(itm.CodFasc))
                        codfasc = itm.CodFasc;
                    table.AddCell(this.addCell(new Phrase(codfasc, font)));
                    // oggetto
                    table.AddCell(this.addCell(new Phrase(itm.desc_oggetto, font)));
                    // data creazione o protocollazione
                    table.AddCell(this.addCell(new Phrase(itm.data_prot_or_create, font)));
                    // rapporto di versamento
                    table.AddCell(this.addCell(reportPhrase));

                    //se ci sono allegati li accodo sotto il doc principale
                    int numAllegati = Convert.ToInt32(itm.numAllegati);
                    if (numAllegati > 0)
                    {
                        
                        int i = 1;
                        foreach(string infoAll in itm.path_allegati)
                        {

                            string idAll = infoAll.Split('§')[0];
                            string pathAll = infoAll.Split('§')[1];

                            //costruisco il link
                            Anchor anchorAll = new Anchor(idAll, linkall);
                            anchorAll.Reference = pathAll;
                            Phrase phraseAll = new Phrase();
                            phraseAll.Add(anchorAll);

                            // tipo
                            table.AddCell(this.addCell(new Phrase(string.Format("Allegato {0}", i), fontall)));
                            // id/segnatura
                            table.AddCell(this.addCell(phraseAll));
                            // cod fasc
                            table.AddCell(this.addCell(new Phrase(" ", fontall)));
                            // oggetto
                            table.AddCell(this.addCell(new Phrase(" ", fontall)));
                            // data creazione o protocollazione
                            table.AddCell(this.addCell(new Phrase(" ", fontall)));
                            // rapporto di versamento
                            table.AddCell(this.addCell(new Phrase(" ", fontall)));
                            i++;
                        }

                    }

                }
                pdfDocument.Add(table);

                pdfDocument.Close();

                document = new FileDocumento();
                document.name = String.Format("Indice.pdf");
                document.path = String.Empty;
                document.fullName = document.name;
                document.contentType = "application/pdf";
                document.content = stream.GetBuffer();
                document.length = Convert.ToInt32(stream.Length);

                logger.Debug(document.content.Length + " _ " + document.length);
            }

            return document;

        }

        private PdfPCell addCell(Phrase phrase)
        {
            PdfPCell cell = null;
            cell = new PdfPCell(phrase);
            cell.BackgroundColor = Color.WHITE;
            cell.Border = 0;
            cell.BorderWidth = 0f;
            cell.BorderColor = Color.WHITE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            //cell.Width = size;

            return cell;

        }

    }

}
