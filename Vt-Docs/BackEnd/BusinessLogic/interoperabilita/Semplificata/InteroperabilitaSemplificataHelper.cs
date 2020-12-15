using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Interoperability.Domain;
using DocsPaVO.documento;
using DocsPaVO.utente;
using BusinessLogic.Documenti;
using BusinessLogic.Utenti;
using BusinessLogic.interoperabilita.Semplificata.Exceptions;

namespace BusinessLogic.interoperabilita.Semplificata
{
    /// <summary>
    /// Questa classe offre una serie di metodi utili per la creazione degli oggetti relativi all'interoperabilità
    /// semplificata
    /// </summary>
    public class InteroperabilitaSemplificataHelper
    {
        /// <summary>
        /// Metodo per la creazione di un messaggio di interoperabilità
        /// </summary>
        /// <param name="schedaDocumento">Informazioni sul documento da cui generare la richiesta</param>
        /// <param name="infoUtente">Informazioni sul mittente della spedizione</param>
        /// <param name="destinatari">Lista dei destinatari della spedizione</param>
        /// <returns>Messaggio di interoperabilità</returns>
        public InteroperabilityMessage CreateInteroperabilityMessage(SchedaDocumento schedaDocumento, InfoUtente infoUtente, Corrispondente[] destinatari)
        {
            InteroperabilityMessage interoperabilityMessage = new InteroperabilityMessage();

            interoperabilityMessage.Record = this.CreateRecordInfo(schedaDocumento);
            interoperabilityMessage.MainDocument = this.CreateMainDocumentInfo(schedaDocumento);
            interoperabilityMessage.Sender = this.CreateSendersInfo(schedaDocumento, infoUtente, destinatari);
            interoperabilityMessage.Receivers = this.CreateReceiversInfo(destinatari);
            interoperabilityMessage.Attachments = this.CreateAttachmentsInfo(schedaDocumento, infoUtente);

            // Se il sistema è configurato per spedire anche l'ultima nota visibile a tutti, viene reperita
            String value = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_NOTE_IN_SEGNATURA");
            if (!String.IsNullOrEmpty(value) && value == "1")
                interoperabilityMessage.Note = new DocsPaDB.Query_DocsPAWS.Documenti().GetUltimaNotaVisibileTuttiDocumento(schedaDocumento.systemId);

            interoperabilityMessage.IsPrivate = schedaDocumento.privato == "1";
            interoperabilityMessage.ReceiverAdministrationCode = destinatari[0].codiceAmm;
            
            return interoperabilityMessage;
        }

        /// <summary>
        /// Metodo per il recupero delle informazioni sul protocollo miittente
        /// </summary>
        /// <param name="schedaDocumento">Documento da cui estrarre le informazioni</param>
        /// <returns>Informazioni sul protocollo mittente</returns>
        private RecordInfo CreateRecordInfo(SchedaDocumento schedaDocumento)
        {
            RecordInfo recordInfo = new RecordInfo()
            {
                AdministrationCode = schedaDocumento.registro.codAmministrazione,
                AOOCode = schedaDocumento.registro.codRegistro,
                RecordDate = DateTime.Parse(ProtoManager.getDataOraProtocollo(schedaDocumento.systemId)),
                RecordNumber = schedaDocumento.protocollo.numero,
                Subject = schedaDocumento.oggetto.descrizione
            };
             
            return recordInfo;
        }

        /// <summary>
        /// Metodo per la generazione delle informazioni sul documento principale
        /// </summary>
        /// <param name="schedaDocumento">Documento da cui spedire</param>
        /// <returns>Informazioni sul documento principale</returns>
        private DocumentInfo CreateMainDocumentInfo(SchedaDocumento schedaDocumento)
        {
            Documento mainDocument = schedaDocumento.documenti[0] as Documento;
            string originalFileName = BusinessLogic.Documenti.FileManager.getOriginalFileName(null, mainDocument);
            if (String.IsNullOrEmpty(originalFileName))
                originalFileName= mainDocument.fileName;

            DocumentInfo documentInfo = new DocumentInfo()
            {
                DocumentNumber = mainDocument.docNumber,
                DocumentServerLocation = mainDocument.docServerLoc,
                FileName = originalFileName,
                FilePath = mainDocument.path,
                Name = mainDocument.fileName,
                VersionId = mainDocument.versionId,
                VersionLabel = mainDocument.versionLabel,
                Version = mainDocument.version
            };

            return documentInfo;
        }

        /// <summary>
        /// Metodo per la creazione delle informazioni sul mittente della spedizione
        /// </summary>
        /// <param name="schedaDocumento">Documento da cui estrarre le informazioni</param>
        /// <param name="infoUtente">Informazioni sull'utente richiedente</param>
        /// <param name="destinatari">Lista dei destinatari della spedizione</param>
        /// <returns>Informazioni sul mittente della spedizione</returns>
        private SenderInfo CreateSendersInfo(SchedaDocumento schedaDocumento, InfoUtente infoUtente, Corrispondente[] destinatari)
        {
            // Recupero del codice del mittente
            String senderCode = InteroperabilitaSemplificataManager.LoadSenderInfoFromUoId(((ProtocolloUscita)schedaDocumento.protocollo).mittente.systemId);

            // Se il mittente non è interoperante o se non è presente in RC, non si può procedere con la spedizione
            if(!InteroperabilitaSemplificataManager.IsCorrEnabledToInterop(
                    ((ProtocolloUscita)schedaDocumento.protocollo).mittente.systemId) ||
                    RubricaComune.RubricaServices.GetElementoRubricaComune(infoUtente, senderCode, false) == null)
                throw new SenderNotInteroperableException();

            SenderInfo senderInfo = new SenderInfo() 
            {
                AdministrationId = infoUtente.idAmministrazione,
                //Code = String.Format("{0}-{1}",
                //    ((ProtocolloUscita)schedaDocumento.protocollo).mittente.codiceAmm,
                //    ((ProtocolloUscita)schedaDocumento.protocollo).mittente.descrizione),
                Code = senderCode,
                Url = InteroperabilitaSemplificataManager.GetUrl(infoUtente.idAmministrazione), 
                UserId = infoUtente.userId,
                FileManagerUrl = InteroperabilitaSemplificataManager.GetFileManagerUrl(schedaDocumento.registro.idAmministrazione)
            };

            return senderInfo;
        }

        /// <summary>
        /// Metodo per la creazione delle informazioni sui destinatari della spedizione
        /// </summary>
        /// <param name="receivers">Destinatari da cui estrarre le informazioni di interesse</param>
        /// <returns>Lista con le informazioni sui destinatari della spedizione</returns>
        private List<ReceiverInfo> CreateReceiversInfo(Corrispondente[] receivers)
        {
            List<ReceiverInfo> reciversInfo = new List<ReceiverInfo>();
            foreach (var receiver in receivers)
            {
                // Se il corrispondente è storicizzato, ne viene recuperata l'ultima
                // versione
                String corrCode = receiver.codiceRubrica;
                if (!String.IsNullOrEmpty(receiver.dta_fine))
                    corrCode = addressBookManager.GetActualCorrCode(receiver.codiceRubrica);

                reciversInfo.Add(new ReceiverInfo()
                {
                    AdministrationCode = receiver.codiceAmm,
                    AOOCode = receiver.codiceAOO,
                    Code = corrCode
                });


                
            }

            return reciversInfo;
        }

        /// <summary>
        /// Metodo per la creazione delle informazioni sugli allegati del documento
        /// </summary>
        /// <param name="schedaDocumento">Documento da cui estrarre le informazioni</param>
        /// <param name="infoUtente">Informazioni sul richiedente</param>
        /// <returns>Lista delle informazioni sugli allegati</returns>
        private List<DocumentInfo> CreateAttachmentsInfo(SchedaDocumento schedaDocumento, InfoUtente infoUtente)
        {
            List<DocumentInfo> attachmentsInfo = new List<DocumentInfo>();
            foreach (Allegato allegato in ((Allegato[])schedaDocumento.allegati.ToArray(typeof(Allegato))).Where(
               a => AllegatiManager.getIsAllegatoIS(a.versionId) == "0" && AllegatiManager.getIsAllegatoPEC(a.versionId) == "0")
               )
            {
                string originalFileName = BusinessLogic.Documenti.FileManager.getOriginalFileName(null, allegato);
                if (String.IsNullOrEmpty(originalFileName))
                    originalFileName = allegato.fileName;

                attachmentsInfo.Add(new DocumentInfo()
                {
                    DocumentNumber = allegato.docNumber,
                    DocumentServerLocation = allegato.docServerLoc,
                    FileName = originalFileName,
                    FilePath = allegato.path,
                    Name = allegato.descrizione,
                    NumberOfPages = allegato.numeroPagine,
                    VersionId = allegato.versionId,
                    VersionLabel = allegato.versionLabel,
                    Version = allegato.version
                });
            }

            return attachmentsInfo;
        }
    }
}
