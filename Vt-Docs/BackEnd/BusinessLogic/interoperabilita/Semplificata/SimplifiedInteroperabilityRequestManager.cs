using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Interoperability.Domain;
using BusinessLogic.Utenti;
using DocsPaVO.utente;
using DocsPaVO.Interoperabilita.Semplificata;
using BusinessLogic.interoperabilita.Semplificata.Exceptions;
using DocsPaVO.documento;
using BusinessLogic.Documenti;
using BusinessLogic.Interoperabilità;
using DocsPaVO.amministrazione;
using DocsPaVO.trasmissione;
using BusinessLogic.Trasmissioni;
using DocsPaVO.addressbook;
using System.Collections;
using log4net;

namespace BusinessLogic.interoperabilita.Semplificata
{
    /// <summary>
    /// Questa classe fornisce metodi per l'analisi di una nuova richiesta di interoperabilità.
    /// Fornisce metodi per l'analisi della richiesta e per la generazione del predisposto o del 
    /// protocollo.
    /// </summary>
    public class SimplifiedInteroperabilityRequestManager
    {

        private static ILog logger = LogManager.GetLogger(typeof(SimplifiedInteroperabilityRequestManager));

        /// <summary>
        /// Metodo per il salvataggio di una reference per il messaggio di interoperabilità
        /// </summary>
        /// <param name="profileId">Id del documento creato a seguito della richiesta di interoperabilità</param>
        /// <param name="interoperabilityMessage">Informazioni sul messaggio di interoperabilità</param>
        /// <returns>Identificativo del messaggio</returns>
        public static String SaveMessageReference(InteroperabilityMessage interoperabilityMessage)
        {
            // Creazione di un riga nel registro del'IS e restituzione dell'id del messaggio
            String messageId = Guid.NewGuid().ToString();

            StringBuilder receiverCode = new StringBuilder();
            foreach (var rec in interoperabilityMessage.Receivers)
                receiverCode.AppendFormat("'{0}', ", rec.Code);
            receiverCode = receiverCode.Remove(receiverCode.Length - 2, 2);

            try
            {
                SimplifiedInteroperabilityLogAndRegistryManager.InsertItemInRegistry(
                    messageId,
                    interoperabilityMessage.IsPrivate,
                    interoperabilityMessage.Record.Subject,
                    interoperabilityMessage.Sender.Code,
                    interoperabilityMessage.Sender.Url,
                    interoperabilityMessage.Record.AdministrationCode,
                    interoperabilityMessage.Record.AOOCode,
                    interoperabilityMessage.Record.RecordDate,
                    interoperabilityMessage.Record.RecordNumber,
                    receiverCode.ToString());
            }
            catch (Exception e)
            {
                throw new SaveMessageReferenceException("Errore durante il salvataggio delle informazioni sulla richiesta di interoperabilità.");
            }

            return messageId;
        }

        /// <summary>
        /// Metodo per l'analisi di una richiesta di interoperabilità. Al termine dell'esecuzione di questo metodo
        /// nel sistema saranno presenti dei documenti in ingresso contenenti i dati prelevati dalla richiesta.
        /// </summary>
        /// <param name="interoperabilityMessage">Informazioni sulla richiesta di interoperabilità</param>
        /// <param name="messageId">Guid del messaggio di interoperabilità in fase di analisi</param>
        public static InfoDocumentDelivered AnalyzeInteroperabilityMessage(InteroperabilityMessage interoperabilityMessage, String messageId)
        {
            // Mittente della spedizione
            Corrispondente senderInfo = null;

            // Utente e ruolo da utilizzare per la creazione del predisposto
            Utente user = null;
            Ruolo userRole = null;
            InfoDocumentDelivered infoDocDelivered = null;
            
            // E' necessario creare un predisposto per ogni AOO, quindi bisogna analizzare tutti i destinatari e
            // capire quanti crearne. 
            // Caricamento delle impostazioni sulle AOO destinatarie del messaggio
            Dictionary<InteroperabilitySettings, List<ReceiverInfo>> interopSettings = null;
            try
            {
                interopSettings = InteroperabilitaSemplificataManager.LoadSettings(interoperabilityMessage.Receivers, interoperabilityMessage.ReceiverAdministrationCode);
            }
            catch (Exception e)
            {
                SimplifiedInteroperabilityLogAndRegistryManager.InsertItemInLog(String.Empty, true,
                    String.Format("{0} Id messaggio: {1}", e.Message, messageId));
                throw new SimplifiedInteroperabilityLoadSettingsException("Errore durante il caricamento delle impostazioni sui registri coinvolti dalla richiesta di interoperabilità");
            }

            // Eventuale eccezione da sollevare
            SimplifiedInteroperabilityException simplifiedInteroperabilityException = new SimplifiedInteroperabilityException();

            // Creazione di un predisposto per ogni AOO
            foreach (var settings in interopSettings.Keys)
            {
                try
                {
                    // Caricamento di utente e ruolo da utilizzare per la creazione del documento
                    user = UserManager.getUtenteById(settings.UserId.ToString());
                    user.dst = UserManager.getSuperUserAuthenticationToken();
                    userRole = UserManager.getRuoloByIdGruppo(settings.RoleId.ToString());

                    if (user == null || userRole == null)
                    {
                       
                      //pork.. per generare l'eccezione almeno nel log di IS scrive che non è configurato bene il ruolo, altrimenti
                        //scrive solo errore generico..
                        string Eccezione= userRole.systemId;
                        Eccezione = user.systemId;

                    }

                    logger.DebugFormat("interopSettings[settings]: {0} - messageId: {1} - interoperabilityMessage: {2} - user: {3}",
                        interopSettings[settings],
                        messageId,
                        interoperabilityMessage.Sender.Url,
                        user.userId);

                }
                catch (Exception retrivingUserAndRole)
                {
                    // Errore durante il recupero delle informazioni su utente e ruolo per la creazione del predisposto.
                    simplifiedInteroperabilityException.Requests.Add(
                        new SingleRequest()
                        {
                            ErrorMessage = "Destinatario non configurato correttamente",
                            ReceiverInfoes = interopSettings[settings]
                        });
                    
                    SimplifiedInteroperabilityLogAndRegistryManager.InsertItemInLog(
                        String.Empty,
                        true,
                        "Errore durante il recupero di utente e ruolo per la creazione del predisposto.");
                }

                try
                {

                    // Recupero delle informazioni sul mittente del messaggio di interoperabilità. Il corrispondente
                    // deve essere censito in Rubrica Comune, quindi viene effettuata una ricerca mirata.
                    // (Viene fatta solo se non è già stato scaricato)
                    if(senderInfo == null)
                        senderInfo = UserManager.getCorrispondenteByCodRubricaRubricaComune(
                            interoperabilityMessage.Sender.Code,
                            UserManager.GetInfoUtente(user, userRole));

                    if (senderInfo == null)
                        throw new RetrivingSenderInfoException(String.Format("Errore durante il reperimento del mittente del messaggio di interoperabilità. Id messaggio: {0}, Codice mittente: {1}",
                            messageId,
                            interoperabilityMessage.Sender.Code));
                }
                catch (Exception e)
                {
                    SimplifiedInteroperabilityLogAndRegistryManager.InsertItemInLog(String.Empty, true, e.Message);
                    throw new RetrivingSenderInfoException(e.Message);
                }

                try
                {

                    infoDocDelivered = GenerateInteroperabilityDocument(settings, senderInfo, interoperabilityMessage, interopSettings[settings], messageId, user, userRole);
                }
                catch (RegistryNotInteroperableException registryNotInteroperable)
                {
                    // Il registro non è interoperante
                    simplifiedInteroperabilityException.Requests.Add(
                        new SingleRequest()
                        {
                            ErrorMessage = registryNotInteroperable.Message,
                            ReceiverInfoes = interopSettings[settings]
                        });
                }
                catch (CreatingDocumentException creatingDocumentException)
                {
                    // Errore durante la creazione del documento
                    simplifiedInteroperabilityException.Requests.Add(
                        new SingleRequest()
                        {
                            ErrorMessage = creatingDocumentException.Message,
                            ReceiverInfoes = interopSettings[settings]
                        });
                }
                catch (DownloadDocumentException downloadDocumentException)
                {
                    // Errore durante l'associazione di un file ad un documento
                    simplifiedInteroperabilityException.Requests.Add(
                        new SingleRequest()
                        {
                            ErrorMessage = downloadDocumentException.Message,
                            ReceiverInfoes = interopSettings[settings]
                        });
                }
                catch (RetrivingTransmissionReceiverException retrivingTransmissionReceiver)
                {
                    // Errore durante il recupero dei destinatati cui trasmettere il predisposto.
                    simplifiedInteroperabilityException.Requests.Add(
                        new SingleRequest()
                        {
                            ErrorMessage = retrivingTransmissionReceiver.Message,
                            ReceiverInfoes = interopSettings[settings]
                        });
                }
                catch (RetrivingSomeTransmissionReceiverException retrivingSomeTransmissionReceiverException)
                {
                    // Errore durante il recupero di alcuni destinatari cui trasmettere il predisposto.
                    simplifiedInteroperabilityException.Requests.Add(
                        new SingleRequest()
                        {
                            ErrorMessage = retrivingSomeTransmissionReceiverException.Message,
                            ReceiverInfoes = retrivingSomeTransmissionReceiverException.Receivers
                        });
                }
                catch (ExecuteTransmissionException executeTransmission)
                {
                    // Errore durante la trasmissione del documento ai destinatari interni.
                    simplifiedInteroperabilityException.Requests.Add(
                        new SingleRequest()
                        {
                            ErrorMessage = executeTransmission.Message,
                            ReceiverInfoes = interopSettings[settings]
                        });
                }
                catch (Exception general)
                {
                    // Eccezione generica
                    simplifiedInteroperabilityException.Requests.Add(
                        new SingleRequest()
                        {
                            ErrorMessage = "Errore non identificato",
                            ReceiverInfoes = interopSettings[settings]
                        });
                }

            }

            // Se per almeno un destinatario è stata generata una eccezione, viene rilanciata l'eccezione SimplifiedInteroperabilityException
            if (simplifiedInteroperabilityException.Requests.Count > 0)
                throw simplifiedInteroperabilityException;

            return infoDocDelivered;
        }

        /// <summary>
        /// Metodo per la creazione del predisposto e per la sua trasmissione agli utenti impostati nel sistema
        /// </summary>
        /// <param name="settings">Impostazioni relative al particolare registro su cui creare il predisposto</param>
        /// <param name="senderInfo">Informazioni sul mittente del messaggio</param>
        /// <param name="interoperabilityMessage">Informazioni sulla richiesta di interoperabilità</param>
        /// 
        private static InfoDocumentDelivered GenerateInteroperabilityDocument(InteroperabilitySettings settings, Corrispondente senderInfo, InteroperabilityMessage interoperabilityMessage, List<ReceiverInfo> receivers, String messageId, Utente user, Ruolo userRole)
        {
            // Se per il registro è disabilitata l'interoperabilità, non si può procedere.
            if (!settings.IsEnabledInteroperability)
                throw new RegistryNotInteroperableException(String.Format("Registro {0} non interoperante", RegistriManager.getRegistro(settings.RegistryId).codice));

            // Creazione della scheda documento
            SchedaDocumento document = CreateDocument(settings, senderInfo, interoperabilityMessage, userRole, user, messageId);

            // Trasmissione dei documenti ai ruoli
            List<ReceiverInfo> uneachableReceivers = TransmitDocument(settings, document, userRole, user, interoperabilityMessage.IsPrivate, receivers);

            // Se il documento è stato marcato privato a causa delle impostazioni sulla gestione
            // viene eliminato il flag privato
            if (!interoperabilityMessage.IsPrivate && document.privato == "1")
                DocManager.SetNotPrivate(document.systemId);

            InfoDocumentDelivered infoDocDelivered = BuildInfoDocumentDelivered(document);

            // Se per il registro è impostata la modalità automatica, si procede con la protocollazione del documento
            if (settings.ManagementMode == ManagementType.A)
                try
                {
                    InfoUtente userInfo = UserManager.GetInfoUtente(user, userRole);

                    // Impostazione della data e ora di protocollazione
                    document.protocollo.dataProtocollazione = DateTime.Now.ToString("dd/MM/yyyy");
                    document.protocollo.anno = DateTime.Now.Year.ToString();

                    ResultProtocollazione protoResult = ResultProtocollazione.OK;
                    ProtoManager.protocolla(document, userRole, userInfo, out protoResult);

                    // Invio della ricevuta di conferma di ricezione al mittente per tutti i destinatari
                    SimplifiedInteroperabilityProtoManager.SendDocumentReceivedProofToSender(
                        interoperabilityMessage,
                        new DocsPaVO.Interoperabilita.Semplificata.RecordInfo()
                        {
                            AdministrationCode = document.registro.codAmministrazione,
                            AOOCode = document.registro.codRegistro,
                            RecordDate = DateTime.Parse(document.protocollo.dataProtocollazione),
                            RecordNumber = document.protocollo.numero
                        },
                        document.systemId,
                        userInfo);


                }
                catch (Exception e)
                {
                    SimplifiedInteroperabilityLogAndRegistryManager.InsertItemInLog(
                        document.systemId,
                        true,
                        "Errore durante la creazione del protocollo");
                    throw new CreatingDocumentException("Errore durante la protocollazione del documento predisposto");
                }

            // Se qualche corrispondente non è stato trovato, viene sollevata un'eccezione 
            // di tipo RetrivingSomeTransmissionReceiverException
            if (uneachableReceivers != null && uneachableReceivers.Count > 0)
                throw new RetrivingSomeTransmissionReceiverException(
                    "Nessun destinatario trovato per la trasmissione del documento",
                    uneachableReceivers);
            
            return infoDocDelivered;
        }

        /// <summary>
        /// Creazione del documento nel sistema
        /// </summary>
        /// <param name="settings">Impostazioni relative al registro su cui creare il documento</param>
        /// <param name="senderInfo">Informazioni sul mittente della richiesta</param>
        /// <param name="interoperabilityMessage">Messaggio con le informazioni sulla richiesta di interoperabilità</param>
        /// <param name="userRole">Ruolo dell'utente da utilizzare per la creazione del documento</param>
        /// <param name="user">Utente da utilizzare per la creazione del documento</param>
        /// <returns>Scheda del documento creato</returns>
        private static SchedaDocumento CreateDocument(InteroperabilitySettings settings, Corrispondente senderInfo, InteroperabilityMessage interoperabilityMessage, Ruolo userRole, Utente user, String messageId)
        {
            // Recupero delle informazioni sull'utente da utilizzare come creatore del documento
            InfoUtente userInfo = UserManager.GetInfoUtente(user, userRole);

            // Creazione della scheda documento da restituire
            SchedaDocumento document = DocManager.NewSchedaDocumento(userInfo);

            // Recupero del registro su cui creare il predisposto
            Registro registry = RegistriManager.getRegistro(settings.RegistryId);

            // Recupero del mezzo di spedizione
            MezzoSpedizione channel = InfoDocManager.GetMezzoSpedizioneDaCodice("SIMPLIFIEDINTEROPERABILITY");

            // Impostazione delle proprietà del documento
            try
            {
                if (!String.IsNullOrEmpty(interoperabilityMessage.MainDocument.FileName))
                    document.appId = InteroperabilitaSegnatura.getApp(interoperabilityMessage.MainDocument.FileName).application;
            }
            catch (Exception e) {}

            document.idPeople = user.idPeople;
            document.userId = user.userId;
            document.oggetto = new Oggetto() { descrizione = interoperabilityMessage.Record.Subject };
            document.predisponiProtocollazione = true;
            document.registro = registry;
            document.tipoProto = "A";
            document.typeId = "SIMPLIFIEDINTEROPERABILITY";
            document.mezzoSpedizione = channel.IDSystem;
            document.descMezzoSpedizione = channel.Descrizione;
            document.interop = "S";
            document.protocollatore = new Protocollatore(userInfo, userRole);
            document.privato = interoperabilityMessage.IsPrivate ? "1" : "0";

            // Se la gestione dell'interoperabilità è manuale con mantenimento 
            // del documento pendente, il documento viene creato privato e subito dopo
            // la creazione viene rimarcato come non privato
            if (settings.ManagementMode == ManagementType.M && settings.KeepPrivate)
                document.privato = "1";

            // Preparazione dell'oggetto protocollo entrata
            ProtocolloEntrata proto = new ProtocolloEntrata();
            proto.mittente = senderInfo;
            proto.dataProtocolloMittente = interoperabilityMessage.Record.RecordDate.ToString("dd/MM/yyyy HH:mm:ss");
            proto.descrizioneProtocolloMittente = String.Format("{0}{1}{2}",
                interoperabilityMessage.Record.AOOCode,
                DocsPaDB.Utils.Personalization.getInstance(registry.idAmministrazione).getSepSegnatura(),
                interoperabilityMessage.Record.RecordNumber);
            document.protocollo = proto;

            // Se per l'amministrazione è configurata l'aggiunta di una nota visibile a tutti,
            // ne viene aggiunta una
            String value = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_NOTE_IN_SEGNATURA");
            if (!String.IsNullOrEmpty(value) && value == "1" && !String.IsNullOrEmpty(interoperabilityMessage.Note))
                document.noteDocumento = new List<DocsPaVO.Note.InfoNota>() { new DocsPaVO.Note.InfoNota(interoperabilityMessage.Note, userInfo.idPeople, userInfo.idGruppo, DocsPaVO.Note.TipiVisibilitaNotaEnum.Tutti, String.Empty) { DaInserire = true } };

            // Salvataggio del documento
            try
            {
                document = DocSave.addDocGrigia(document, userInfo, userRole);

                // Associazione del canale di spedizione al documento
                ProtoManager.collegaMezzoSpedizioneDocumento(userInfo, channel.IDSystem, document.systemId);
            }
            catch (Exception e)
            {
                SimplifiedInteroperabilityLogAndRegistryManager.InsertItemInLog(String.Empty, true, e.Message + " Id messaggio: " + messageId);
                throw new CreatingDocumentException("Errore durante la creazione del documento");

            }
            // Se tutto è andato bene, la scheda documento ha ora il campo system id popolato, quindi viene aggiunta
            // una voce al log e viene aggiornato l'id profile per la voce nel registro dei messaggi ricevuti
            SimplifiedInteroperabilityLogAndRegistryManager.InsertItemInLog(document.systemId, false,
                String.Format("Documento relativo alla richiesta con id {0}, creato correttamente.", messageId));
            SimplifiedInteroperabilityLogAndRegistryManager.SetIdProfileForMessage(messageId, document.systemId);

            // Impostazione delle informazioni aggiuntive e salvataggio del documento
            ((Documento)document.documenti[0]).dataArrivo = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            bool toUpdate;
            document = DocSave.save(userInfo, document, false, out toUpdate, userRole);
            string errPutFile = "";
            try
            {

                // Associazione dell'immagine al documento
                String err = String.Empty;
                FileRequest request = document.documenti[0] as FileRequest;
                logger.DebugFormat("IS - Associa documento. Versionid {0}, Versionlabel {1}, version {2}",
                    request.versionId, request.versionLabel, request.version);

                if (!String.IsNullOrEmpty(interoperabilityMessage.MainDocument.FileName))
                    SimplifiedInteroperabilityFileManager.DownloadFile(
                        interoperabilityMessage.MainDocument,
                        interoperabilityMessage.Sender.AdministrationId,
                        request,
                        userInfo,
                        interoperabilityMessage.Sender.FileManagerUrl,
                        out errPutFile);
                if (!string.IsNullOrEmpty(errPutFile))
                {
                    throw new Exception("Errore durante l'associazione dell'immagine al documento principale");
                }
            }
            catch (Exception e)
            {
                SimplifiedInteroperabilityLogAndRegistryManager.InsertItemInLog(
                    document.systemId,
                    true,
                    String.Format("Errore durante l'associazione dell'immagine per il documento principale. Messaggio eccezione: {0}",
                    e.Message));
                if (!string.IsNullOrEmpty(errPutFile) && errPutFile.Contains("formato file"))
                {
                    throw new DownloadDocumentException("Errore durante l'associazione dell'immagine al documento principale." + errPutFile+ " destinataria.");
                }
                else
                    throw new DownloadDocumentException("Errore durante l'associazione dell'immagine al documento principale");

            }
            AddAttachments(document.docNumber, interoperabilityMessage.Attachments, userInfo, interoperabilityMessage.Sender.AdministrationId, interoperabilityMessage.Sender.FileManagerUrl);
            // Restituzione del documento salvato
            return document;

        }

        /// <summary>
        /// Metodo per la costruzione dell'oggetto InfoDocumentDelivered contenente informazioni
        /// su ciò che è stato effettivamente recapitato a destinazione.
        /// </summary>
        /// <param name="schedaDocumento"></param>
        /// <returns></returns>
        private static InfoDocumentDelivered BuildInfoDocumentDelivered(SchedaDocumento schedaDocumento)
        {
            InfoDocumentDelivered infoDocDelivered = new InfoDocumentDelivered();

            if (schedaDocumento != null && !string.IsNullOrEmpty(schedaDocumento.docNumber))
            {
                try
                {
                    //Aggiungo le informazioni per il documento principale
                    FileRequest fileReq = (Documento)schedaDocumento.documenti[0];
                    DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
                    string impronta = string.Empty;
                    if (Int32.Parse(fileReq.fileSize) > 0)
                    {
                        doc.GetImpronta(out impronta, fileReq.versionId, fileReq.docNumber);
                    }
                    infoDocDelivered.MainDocument = new DocumentInfo
                    {
                        Name = schedaDocumento.oggetto.descrizione,
                        FileName = doc.GetNomeOriginale(fileReq.versionId, fileReq.docNumber),
                        Fingerprint = impronta
                    };

                    //Aggiungo le informazioni per gli allegati
                    infoDocDelivered.Attachments = new List<DocumentInfo>();
                    schedaDocumento.allegati = doc.GetAllegati(schedaDocumento.docNumber, "user");
                    if (schedaDocumento.allegati != null && schedaDocumento.allegati.Count > 0)
                    {
                        DocumentInfo infoAttach;
                        foreach (Allegato allegato in schedaDocumento.allegati)
                        {
                            impronta = string.Empty;
                            if (Int32.Parse(allegato.fileSize) > 0)
                            {
                                doc.GetImpronta(out impronta, allegato.versionId, allegato.docNumber);
                            }

                            infoAttach = new DocumentInfo
                            {
                                Name = allegato.descrizione,
                                VersionLabel = allegato.versionLabel,
                                FileName = doc.GetNomeOriginale(allegato.versionId, allegato.docNumber),
                                Fingerprint = impronta
                            };

                            infoDocDelivered.Attachments.Add(infoAttach);
                        }
                    }
                }
                catch(Exception e)
                {
                    logger.Error("BusinessLogic.interoperabilita.Semplificata.SimplifiedInteroperabilityRequestManager.BuildInfoDocumentDelivered. Errore durante la costruzione dell'oggetto contenente l'infomazione della spedizione : " + e.Message);
                }
            }
            return infoDocDelivered;
        }

        /// <summary>
        /// Metodo per l'aggiunta degli allegati ad un documento
        /// </summary>
        /// <param name="docNumber">Numero del documento cui aggiungere gli allegati</param>
        /// <param name="attachments">Informazioni sugli allegati da creare</param>
        /// <param name="userInfo">Informazioni sull'utente utilizzato per la creazione del predisposto in ingresso</param>
        /// <param name="senderAdministrationId">Id dell'amministrazione mittente della richiesta di interoperabilità</param>
        /// <param name="senderFileManagerUrl">Url del file manager per il download dell'eventuale file associato ad un allegato</param>
        private static void AddAttachments(String docNumber, List<DocumentInfo> attachments, InfoUtente userInfo, String senderAdministrationId, String senderFileManagerUrl)
        {
            int i = 0;
            foreach (DocumentInfo attachment in attachments)
            {
                // Creazione dell'oggetto allegato
                Allegato a = new Allegato();

                // Impostazione delle proprietà dell'allegato
                if (String.IsNullOrEmpty(attachment.Name))
                    a.descrizione = String.Format("Allegato {0}", i);
                else
                    a.descrizione = attachment.Name;

                a.docNumber = docNumber;
                a.fileName = InteroperabilitaSegnatura.getFileName(attachment.FileName);
                a.version = "0";
                a.numeroPagine = attachment.NumberOfPages;

                // Aggiunta dell'allegato al documento principale
                try
                {
                    a = AllegatiManager.aggiungiAllegato(userInfo, a);
                }
                catch (Exception ex)
                {
                    SimplifiedInteroperabilityLogAndRegistryManager.InsertItemInLog(
                        docNumber,
                        true,
                        String.Format("Errore durante l'aggiunta dell'allegato '{0}'. Messaggio eccezione: {1}",
                        a.descrizione,
                        ex.Message));

                    throw new CreatingDocumentException(
                        String.Format("Errore durante l'aggiunta dell'allegato '{0}'", attachment.Name));
                }
                string errPutFile = "";
                try
                {
                    // Associazione dell'immagine all'allegato
                    FileRequest request = a as FileRequest;
                    if (!String.IsNullOrEmpty(attachment.FileName))
                        SimplifiedInteroperabilityFileManager.DownloadFile(
                            attachment,
                            senderAdministrationId,
                            request,
                            userInfo,
                            senderFileManagerUrl,
                            out errPutFile);

                    if (!string.IsNullOrEmpty(errPutFile))
                    {
                        throw new Exception("Errore durante l'associazione dell'immagine al documento principale");
                    }
                }
                catch (Exception e)
                {
                    SimplifiedInteroperabilityLogAndRegistryManager.InsertItemInLog(
                        a.docNumber,
                        true,
                        String.Format("Errore durante l'associazione dell'immagine per l'allegato '{0}'. Messaggio eccezione: {1}",
                        a.descrizione,
                        e.Message));
                    if (!string.IsNullOrEmpty(errPutFile) && errPutFile.Contains("formato file"))
                    {
                        throw new DownloadDocumentException(
                            String.Format("Errore durante l'associazione dell'immagine all'allegato '{0}'. {1} destinataria.", attachment.Name,errPutFile));
                    }
                    else
                    {
                        throw new DownloadDocumentException(
                                                    String.Format("Errore durante l'associazione dell'immagine all'allegato '{0}'", attachment.Name));                    
                    }
                }
                i++;
            }
        }

        /// <summary>
        /// Metodo per la trasmissione del documento ai destinatari interni (utenti che si trovano in ruoli cui sono associate
        /// le microfunzioni relative all'IS)
        /// </summary>
        /// <param name="settings">Impostazioni relative all'interoperabilità semplificata per il registro su cui è stato creato il documento</param>
        /// <param name="document">Documento da trasmettere</param>
        /// <param name="userRole">Ruolo utilizzato per la trasmissione</param>
        /// <param name="user">Utente mittente della trasmissione</param>
        /// <param name="privateDocument">Flag utilizzato per indicare se il documento è stato ricevuto marcato come privato dal mittente della spedizione</param>
        /// <param name="receiversInfo">Informaizoni sui destinari da contattare</param>
        /// <returns>Lista dei destinatari per cui si è verificato un problema</returns>
        private static List<ReceiverInfo> TransmitDocument(InteroperabilitySettings settings, SchedaDocumento document, Ruolo userRole, Utente user, bool privateDocument, List<ReceiverInfo> receiversInfo)
        {
            logger.Debug("RAFFREDDORE - Start");
            logger.Debug("RAFFREDDORE - " + (document == null ? "Documento nullo" : document.systemId));
            // Recupero delle informazioni sui corrispondenti interni all'amministrazione che devono ricevere
            // la trasmissione del documento
            List<Corrispondente> corrs = null;

            // Lista dei destinatari della spedizione per cui non sono stati individuati destinatari
            List<ReceiverInfo> uneachableReceivers = new List<ReceiverInfo>();

            try
            {
                logger.Debug("RAFFREDDORE - prima del caricamento destinatari");
                corrs = InteroperabilitaSemplificataManager.LoadTransmissionReceivers(settings, privateDocument, receiversInfo, out uneachableReceivers);

                if (corrs == null || corrs.Count == 0)
                {
                    logger.Error("Nessun destinatario trovato per la trasmissione del documento");
                    throw new RetrivingTransmissionReceiverException("Nessun destinatario trovato per la trasmissione del documento");
                }
            }
            catch (Exception e)
            {
                SimplifiedInteroperabilityLogAndRegistryManager.InsertItemInLog(document.systemId, true, e.Message);
                throw new RetrivingTransmissionReceiverException(e.Message);
            }

            // Creazione dell'oggetto trasmissione
            Trasmissione transmission = new Trasmissione();
            transmission.ruolo = userRole;
            transmission.utente = user;
            transmission.noteGenerali = receiversInfo.Select(r => r.Code).Aggregate((r1, r2) => String.Format("{0} {1}", r1, r2));
            transmission.infoDocumento = new InfoDocumento(document);

            // Creazione della ragione di trasmissione per interoperabilità semplificata
            RagioneTrasmissione reason = InteroperabilitaSegnatura.getRagioneTrasm(user.idAmministrazione, "S");
            if (reason != null && privateDocument)
                reason.eredita = "0";

            if(reason !=  null)
                logger.DebugFormat("IS - Effettuato caricamento della ragione di trasmissione per interoperabilità. Id ragione: {0}; Eredita: {1}", reason.systemId, reason.eredita);

            // Creazione delle trasmissioni singole
            transmission.trasmissioniSingole = new System.Collections.ArrayList();
            foreach (var corr in corrs)
            {
                // Creazione della trasmissione singola
                TrasmissioneSingola singleTransmission = new TrasmissioneSingola()
                {
                    ragione = reason,
                    corrispondenteInterno = corr,
                    tipoTrasm = "S",
                    tipoDest = TipoDestinatario.RUOLO
                };

                // Caricamento degli utenti del ruolo
                singleTransmission.trasmissioneUtente = new System.Collections.ArrayList();

                // Caricamento utenti del ruolo
                QueryCorrispondente qc = new QueryCorrispondente();
                qc.codiceRubrica = ((DocsPaVO.utente.Ruolo)corr).codiceRubrica;
                qc.idRegistri = new System.Collections.ArrayList();
                qc.idRegistri.Add(settings.RegistryId);
                qc.idAmministrazione = user.idAmministrazione;
                qc.getChildren = true;
                qc.fineValidita = true;
                ArrayList users = addressBookManager.listaCorrispondentiIntMethod(qc);

                // Costruzione delle trasmissioni utente
                foreach (Utente u in users)
                {
                    u.dst = user.dst;
                    singleTransmission.trasmissioneUtente.Add(new TrasmissioneUtente()
                    {
                        utente = u
                    });
                }

                transmission.trasmissioniSingole.Add(singleTransmission);
            }

            // Esecuzione della trasmissione
            try
            {
                // INCIDENT 104707
                // modifica PALUMBO per consentire invio mail di notifica in caso di trasmissione ricevuta per IS
                // necessaria chiave "URL_PATH_IS" webconfig ws .
                string path = String.Empty;
                if (System.Configuration.ConfigurationManager.AppSettings["URL_PATH_IS"] != null)
                    path = System.Configuration.ConfigurationManager.AppSettings["URL_PATH_IS"].ToString();

                logger.Debug("RAFFREDDORE - url path is " + path);
                //ExecTrasmManager.saveExecuteTrasmMethod(String.Empty, transmission);
                Trasmissione resultTrasm = ExecTrasmManager.saveExecuteTrasmMethod(path, transmission);
                logger.Debug("RAFFREDDORE - Trasmissione nulla? " + (resultTrasm == null?"Nulla":"NON nulla"));
                //traccio l'evento di trasmssione
                string method = string.Empty, desc = string.Empty;
                if (resultTrasm != null &&
                    resultTrasm.infoDocumento != null && 
                    !string.IsNullOrEmpty(resultTrasm.infoDocumento.docNumber))
                {
                    List<string> includedList = new List<string>();
                    string idCorr = string.Empty;

                    foreach (DocsPaVO.trasmissione.TrasmissioneSingola single in resultTrasm.trasmissioniSingole)
                    {
                        logger.Warn("DEBUG: WriteLog-IS- " + resultTrasm.utente.userId + " " +
                         resultTrasm.infoDocumento.docNumber + " " + resultTrasm.utente.idPeople + " " + resultTrasm.ruolo.idGruppo + " " + resultTrasm.utente.idAmministrazione + " " +
                         resultTrasm.utente.idPeople + " " + method + " " + desc + " " + single.systemId);
                        method = "TRASM_DOC_" + single.ragione.descrizione.ToUpper().Replace(" ", "_");
                        desc = "Trasmesso Documento predisposto in arrivo : " + resultTrasm.infoDocumento.docNumber.ToString();
                        idCorr = single.corrispondenteInterno.systemId;
                        if (!includedList.Contains(idCorr))
                        {
                            BusinessLogic.UserLog.UserLog.WriteLog(resultTrasm.utente.userId, resultTrasm.utente.idPeople,
                                resultTrasm.ruolo.idGruppo, resultTrasm.utente.idAmministrazione, method,
                                resultTrasm.infoDocumento.docNumber, desc, DocsPaVO.Logger.CodAzione.Esito.OK, null, "1", single.systemId);
                            includedList.Add(idCorr);
                        }
                        else
                        {
                            BusinessLogic.UserLog.UserLog.WriteLog(resultTrasm.utente.userId, resultTrasm.utente.idPeople,
                                resultTrasm.ruolo.idGruppo, resultTrasm.utente.idAmministrazione, method,
                                resultTrasm.infoDocumento.docNumber, desc, DocsPaVO.Logger.CodAzione.Esito.OK, null, "0", single.systemId);
                        }
                        logger.Warn("DEBUG: WriteLog-IS- OK");
                    }
                }

            }
            catch (Exception e)
            {
                logger.Error("RAFFREDDORE - Errore durante la trasmissione del documento ai destinatari");
                SimplifiedInteroperabilityLogAndRegistryManager.InsertItemInLog(document.systemId, true, "Errore nella trasmissione del documento ai ruoli designati");
                throw new ExecuteTransmissionException("Errore durante la trasmissione del documento ai destinatari");
            }

            // Se si è arrivati fin qui la trasmissione è stata effettuata correttamente, quindi viene aggiunta
            // una voce al log
            SimplifiedInteroperabilityLogAndRegistryManager.InsertItemInLog(document.systemId, false,
                String.Format("Documento con id {0} trasmesso correttamente ai destinatari", document.systemId));

            // Restituzione dei corrispondenti non troviati
            return uneachableReceivers;

        }


    }
}
