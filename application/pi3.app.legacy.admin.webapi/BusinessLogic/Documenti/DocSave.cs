// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.utente;
using Serilog;
using System.Collections;

namespace BusinessLogic.Documenti;

public class DocSave
{
    private static ILogger logger = Log.ForContext(typeof(DocSave));
    public static Mutex semProtNuovo = new Mutex();

    public static DocsPaVO.documento.SchedaDocumento save(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.documento.SchedaDocumento schedaDoc, bool enableUffRef, out bool daAggiornareUffRef, DocsPaVO.utente.Ruolo ruolo)
    {
        logger.Information("BEGIN");

        //SOSTITUISCO IL CARATTERE SPECIALE
        if (schedaDoc.oggetto.descrizione.Contains("–"))
            schedaDoc.oggetto.descrizione = schedaDoc.oggetto.descrizione.Replace("–", "-");

        // Controllo su stato congelato del documento
        DocumentConsolidation.CanChangeMetadata(infoUtente, schedaDoc, true);

        DocsPaVO.documento.SchedaDocumento savedDocument = null;

        daAggiornareUffRef = false;
        string incestino = string.Empty;

        //controllo se doc in cestino
        incestino = BusinessLogic.Documenti.DocManager.checkdocInCestino(schedaDoc.docNumber);

        if (incestino != ""
            && incestino == "1")
            throw new Exception("Il documento è stato rimosso, non è più possibile modificarlo");

        //Verifico se il documento è in libro firma e se è prevista la repertoriazione del documento
        #region CHECK_LIBRO_FIRMA
        if (LibroFirma.LibroFirmaManager.IsDocInLibroFirma(schedaDoc.docNumber))
        {
            bool daRepertoriare = false;
            if (schedaDoc.template != null && !string.IsNullOrEmpty(schedaDoc.template.ID_TIPO_ATTO) && schedaDoc.template.ELENCO_OGGETTI != null && schedaDoc.template.ELENCO_OGGETTI.Count > 0)
            {
                DocsPaVO.ProfilazioneDinamica.OggettoCustom ogg = (from o in schedaDoc.template.ELENCO_OGGETTI.Cast<DocsPaVO.ProfilazioneDinamica.OggettoCustom>()
                                                                   where o.TIPO.DESCRIZIONE_TIPO.Equals("Contatore") && o.REPERTORIO.Equals("1")
                                                                   && o.CONTATORE_DA_FAR_SCATTARE && string.IsNullOrEmpty(o.VALORE_DATABASE)
                                                                   select o).FirstOrDefault();
                if (ogg != null && !LibroFirma.LibroFirmaManager.IsTitolarePassoInAttesa(schedaDoc.docNumber, infoUtente, DocsPaVO.LibroFirma.Azione.DOCUMENTO_REPERTORIATO))
                {
                    throw new Exception("Non è possibile procedere con la repertoriazione poichè il documento è in Libro Firma");
                }
            }
        }
        #endregion

        // Contesto transazionale
        using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
        {
            if (schedaDoc.repositoryContext != null)
            {
                // Cerazione del documento grigio se è stata fornita una scheda documento in repository context
                schedaDoc = addDocGrigia(schedaDoc, infoUtente, ruolo);
            }

            if (schedaDoc.systemId != null && !schedaDoc.systemId.Equals(""))
            {
                if (schedaDoc.predisponiProtocollazione || (schedaDoc.protocollo != null && (schedaDoc.protocollo.segnatura == null || (schedaDoc.protocollo.segnatura != null && schedaDoc.protocollo.segnatura == "")) && schedaDoc.protocollo.ModUffRef == true && enableUffRef))
                {
                    savedDocument = predisponiAllaProtocollazione(infoUtente, schedaDoc);
                }
                else
                {
                    savedDocument = salvaModifiche(infoUtente, schedaDoc, enableUffRef, out daAggiornareUffRef);
                }
            }
            else
            {
                savedDocument = schedaDoc;
            }

            //Richiamo il metodo per il calcolo della atipicità del documento
            DocsPaDB.Query_DocsPAWS.Documentale documentale = new DocsPaDB.Query_DocsPAWS.Documentale();
            schedaDoc.InfoAtipicita = documentale.CalcolaAtipicita(infoUtente, schedaDoc.docNumber, DocsPaVO.Security.InfoAtipicita.TipoOggettoAtipico.DOCUMENTO);

            if (savedDocument != null)
            {
                // La transazione viene completata, se le modifiche sono state effettuate correttamente
                transactionContext.Complete();
            }
        }
        logger.Information("END");
        return savedDocument;
    }

    public static DocsPaVO.documento.SchedaDocumento addDocGrigia(DocsPaVO.documento.SchedaDocumento schedaDoc, DocsPaVO.utente.InfoUtente objSicurezza, DocsPaVO.utente.Ruolo objRuolo)
    {
        logger.Information("BEGIN");
        try
        {
            semProtNuovo.WaitOne();

            // Avvio del contesto transazionale
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                //SOSTITUISCO IL CARATTERE SPECIALE
                if (schedaDoc.oggetto.descrizione.Contains("–"))
                    schedaDoc.oggetto.descrizione = schedaDoc.oggetto.descrizione.Replace("–", "-");

                // verifico i dati di ingresso
                ProtoManager.checkInputData(objSicurezza.idAmministrazione, schedaDoc);
                logger.Debug("nomeUtente=" + schedaDoc.userId);

                //add massimo digregorio carica dati protocollatore in schedaProtocollo
                schedaDoc = getDatiProtocollatore(schedaDoc, objRuolo, objSicurezza);
                schedaDoc = getDatiCreatore(schedaDoc, objRuolo, objSicurezza);

                // creo il nuovo documento
                DocsPaDocumentale.Documentale.DocumentManager documentManager = new DocsPaDocumentale.Documentale.DocumentManager(objSicurezza);

                DocsPaVO.utente.Ruolo[] ruoliSuperiori;

                List<DocsPaVO.documento.FileRequest> versions = new List<DocsPaVO.documento.FileRequest>();

                if (schedaDoc.documenti != null && schedaDoc.documenti.Count > 0)
                {
                    versions = new List<DocsPaVO.documento.FileRequest>((DocsPaVO.documento.FileRequest[])schedaDoc.documenti.ToArray(typeof(DocsPaVO.documento.FileRequest)));

                    // Ordinamento versioni
                    versions.Sort(
                            delegate (DocsPaVO.documento.FileRequest x, DocsPaVO.documento.FileRequest y)
                            {
                                int versionX, versionY;
                                Int32.TryParse(x.version, out versionX);
                                Int32.TryParse(y.version, out versionY);

                                return (versionX.CompareTo(versionY));
                            }
                        );
                }

                //2023: se è un grigio e non ho selezionato il registro associo il registro associato al ruolo
                if (schedaDoc.protocollo == null && schedaDoc.registro == null)
                {
                    if (objRuolo.registri == null || objRuolo.registri.Count == 0)
                        objRuolo.registri = BusinessLogic.Utenti.RegistriManager.getRegistriRuolo(objRuolo.systemId);

                    if (objRuolo.registri == null || objRuolo.registri.Count == 0)
                    {
                        logger.Error("Errore nella creazione del documento grigio: il ruolo non è associato a nessun registro.");
                        throw new ApplicationException("Errore nella creazione del documento grigio: il ruolo non è associato a nessun registro.");
                    }

                    schedaDoc.registro = objRuolo.registri[0] as Registro;
                }

                if (!documentManager.CreateDocumentoGrigio(schedaDoc, objRuolo, out ruoliSuperiori))
                    throw new ApplicationException("Errore nella creazione del documento grigio");
                else
                {
                    // Notifica evento documento creato
                    DocsPaDocumentale.Interfaces.IAclEventListener eventsNotification = new DocsPaDocumentale.Documentale.AclEventListener(objSicurezza);

                    eventsNotification.DocumentoCreatoEventHandler(schedaDoc, objRuolo, ruoliSuperiori);

                    // Sincronizzazione repository
                    if (schedaDoc.repositoryContext != null)
                    {
                        SessionRepositoryFileManager fileManager = SessionRepositoryFileManager.GetFileManager(schedaDoc.repositoryContext);

                        // In fase di inserimento di un repository temporaneo, 
                        // possono essere creati:
                        // - la prima versione del documento e, qualora sia stato acquisito un file
                        //   e firmato, anche la seconda versione firmata del documento
                        // - la prima versione di n allegati
                        // La prima versione del documento è creata automaticamente con la creazione del documento stesso.
                        // In caso di seconda versione firmata del documento, è necessario procedre alla creazione.
                        foreach (DocsPaVO.documento.FileRequest v in versions)
                        {
                            int version;
                            Int32.TryParse(v.version, out version);

                            DocsPaVO.documento.FileRequest savedVersion = null;

                            if (version > 1)
                            {
                                // Seconda versione firmata del documento,
                                // impostazione dell'id del documento di appartenenza
                                v.docNumber = schedaDoc.docNumber;

                                // Inserimento delle versioni del documento,
                                // acquisite oltre alla versione principale
                                if (!documentManager.AddVersion(v, false))
                                    throw new ApplicationException(string.Format("Errore nella creazione della versione {0} del documento con id {1}", version, schedaDoc.systemId));

                                savedVersion = SessionRepositorySyncronizer.CopyToRepository(fileManager, v);

                                // Inserimento della nuova versione come primo elemento della lista documenti
                                schedaDoc.documenti.Insert(0, savedVersion);

                            }
                            else
                            {
                                // La versione principale del documento è già stata creata al momento dell'inserimento,
                                // pertanto è necessario copiare solamente il file acquisito nel repository
                                savedVersion = SessionRepositorySyncronizer.CopyToRepository(fileManager, (DocsPaVO.documento.FileRequest)schedaDoc.documenti[0]);

                                // Aggiornamento istanza documento principale
                                schedaDoc.documenti[0] = savedVersion;

                            }
                        }

                        if (schedaDoc.allegati != null && schedaDoc.allegati.Count > 0)
                        {
                            // Gli allegati e le rispettive versioni andranno create manualmente
                            foreach (DocsPaVO.documento.Allegato allegato in schedaDoc.allegati)
                            {
                                string oldVersionLabel = allegato.versionLabel;

                                // Impostazione del docnumber del documento principale
                                // cui sarà associato l'allegato
                                allegato.docNumber = schedaDoc.docNumber;

                                if (!documentManager.AddAttachment(allegato, "N"))
                                    throw new ApplicationException(string.Format("Errore nella creazione dell'allegato {0} del documento con id {1}", allegato.position, schedaDoc.systemId));

                                allegato.versionLabel = oldVersionLabel;
                            }

                            DocsPaVO.documento.FileRequest[] allegati = (DocsPaVO.documento.FileRequest[])schedaDoc.allegati.ToArray(typeof(DocsPaVO.documento.Allegato));

                            if (allegati.Length > 0)
                                schedaDoc.allegati = new ArrayList(SessionRepositorySyncronizer.CopyToRepository(fileManager, allegati));
                        }

                        // Se è presente un repository temporaneo, viene effettuato l'inserimento del file nel repository del documentale
                        // Imposta il repository come scaduto
                        fileManager.Delete();

                        schedaDoc.repositoryContext = null;
                    }
                }

                //Richiamo il metodo per il calcolo della atipicità del documento
                DocsPaDB.Query_DocsPAWS.Documentale documentale = new DocsPaDB.Query_DocsPAWS.Documentale();
                schedaDoc.InfoAtipicita = documentale.CalcolaAtipicita(objSicurezza, schedaDoc.docNumber, DocsPaVO.Security.InfoAtipicita.TipoOggettoAtipico.DOCUMENTO);

                if (schedaDoc != null)
                {
                    // Impostazione della transazione come completata,
                    // solamente se il documento è stato creato correttamente
                    transactionContext.Complete();
                }
            }
            logger.Information("END");
            return schedaDoc;
        }
        catch (Exception ex)
        {
            throw ex;

        }
        finally
        {
            semProtNuovo.ReleaseMutex();
        }
    }

    public static DocsPaVO.documento.SchedaDocumento predisponiAllaProtocollazione(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.documento.SchedaDocumento schedaDoc)
    {
        // Verifica stato di consolidamento del documento
        DocumentConsolidation.CanExecuteAction(infoUtente, schedaDoc.systemId, DocumentConsolidation.ConsolidationActionsDeniedEnum.PrepareProtocol, true);

        logger.Debug("predisponiAllaProtocollazione");

        string incestino = string.Empty;
        //controllo se doc in cestino
        incestino = BusinessLogic.Documenti.DocManager.checkdocInCestino(schedaDoc.docNumber);

        if (incestino != ""
            && incestino == "1")
            throw new Exception("Il documento è stato rimosso, non è più possibile modificarlo");


        DocsPaDocumentale.Documentale.DocumentManager documentManager = new DocsPaDocumentale.Documentale.DocumentManager(infoUtente);
        if (!documentManager.PredisponiProtocollazione(schedaDoc))
        {
            string message = "Non è stato possibile predisporre il documento alla protocollazione";
            logger.Debug(message);
            throw new ApplicationException(message);
        }

        if (schedaDoc.documenti != null &&
          !string.IsNullOrEmpty(((DocsPaVO.documento.Documento)schedaDoc.documenti[0]).dataArrivo))
        {
            string firstParam = "DTA_ARRIVO =" + DocsPaDbManagement.Functions.Functions.ToDate(((DocsPaVO.documento.Documento)schedaDoc.documenti[0]).dataArrivo);
            new DocsPaDB.Query_DocsPAWS.Documenti().UpdateVersions(firstParam, schedaDoc.docNumber);
        }

        return schedaDoc;
    }

    private static DocsPaVO.documento.SchedaDocumento salvaModifiche(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.documento.SchedaDocumento schedaDoc, bool enableUffRef, out bool saveUffRef)
    {
        if (!AllegatiManager.isEnabledProfilazioneAllegati() && schedaDoc.documentoPrincipale != null)
        {
            string errorMessage = "Errore nell'operazione 'BusinessLogic.Documenti.DocSave.salvaModifiche': la profilazione dell'allegato non è attivata.";

            logger.Debug(errorMessage);
            throw new ApplicationException(errorMessage);
        }

        logger.Debug("salvaModifiche");
        bool daAggiornareUffRef = false;

        DocsPaDocumentale.Documentale.DocumentManager documentManager = new DocsPaDocumentale.Documentale.DocumentManager(infoUtente);
        if (!documentManager.SalvaDocumento(schedaDoc, enableUffRef, out daAggiornareUffRef))
        {
            string msg = "Errore nel'operazione di salva modifiche del documento";
            logger.Debug(msg);
            throw new ApplicationException(msg);
        }

        saveUffRef = daAggiornareUffRef;
        //AS400
        if (schedaDoc != null && schedaDoc.protocollo != null
            && schedaDoc.protocollo.segnatura != null
            && schedaDoc.protocollo.segnatura != "")
            AS400.AS400.setAs400(schedaDoc, DocsPaAS400.Constants.CREATE_MODIFY_OPERATION);

        return schedaDoc;

    }

    internal static DocsPaVO.documento.SchedaDocumento getDatiProtocollatore(DocsPaVO.documento.SchedaDocumento schedaDoc, DocsPaVO.utente.Ruolo objRuolo, DocsPaVO.utente.InfoUtente objUtente)
    {
        if (schedaDoc.protocollatore == null || schedaDoc.protocollatore.utente_idPeople.Equals(String.Empty))
        {
            schedaDoc.protocollatore = new DocsPaVO.documento.Protocollatore(objUtente, objRuolo);
        }
        return schedaDoc;
    }

    internal static DocsPaVO.documento.SchedaDocumento getDatiCreatore(DocsPaVO.documento.SchedaDocumento schedaDoc, DocsPaVO.utente.Ruolo objRuolo, DocsPaVO.utente.InfoUtente objUtente)
    {
        if (schedaDoc.creatoreDocumento == null || schedaDoc.creatoreDocumento.idPeople.Equals(String.Empty))
        {
            schedaDoc.creatoreDocumento = new DocsPaVO.documento.CreatoreDocumento(objUtente, objRuolo);
        }
        return schedaDoc;
    }
}
