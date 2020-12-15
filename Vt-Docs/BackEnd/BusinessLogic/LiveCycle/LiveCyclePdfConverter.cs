using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Xml;
using log4net;
using DocsPaVO.documento;
using System.ServiceModel;


namespace BusinessLogic.LiveCycle
{
    /// <summary>
    /// Classe per la gestione della conversione in PDF lato server tramite i servizi adobe livecycle
    /// </summary>
    public sealed class LiveCyclePdfConverter
    {
        private static ILog logger = LogManager.GetLogger(typeof(LiveCyclePdfConverter));
        /// <summary>
        /// 
        /// </summary>
        private LiveCyclePdfConverter()
        { }

        #region Public Members

        /// <summary>
        /// Inserisce un documento nella coda di conversione pdf lato server
        /// </summary>
        /// <returns></returns>
        /*
        public static void EnqueueServerPdfConversion(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.documento.ObjServerPdfConversion objServerPdfConversion)
        {
            logger.Debug(string.Format("INIZIO: EnqueueServerPdfConversion, Documento '{0}', Utente '{1}'", objServerPdfConversion.idProfile, infoUtente.userId));

            // 
            bool retValue = false;

            bool lockInserted = false;

            try
            {
                // 1. Verifica se il file non è di tipo PDF
                if (BusinessLogic.Documenti.FileManager.getExtFileFromPath(objServerPdfConversion.fileName).ToUpper() != "PDF".ToUpper())
                {
                    // 2. Verifica che il documento non sia già in stato di conversione PDF
                    if (!BusinessLogic.Documenti.DocManager.isDocInConversionePdf(objServerPdfConversion.idProfile))
                    {
                        // 3. Inserimento del documento nella tabella di richiesta conversione
                        // NB. L'inserimento del lock in tabella deve essere eseguito prima della creazione del contesto transazionale,
                        //     ciò per evitare di accettare altre richieste di conversione sullo stesso documento da parte di altri utenti
                        //     mentre la prima richiesta è ancora in fase di completamento
                        logger.Debug(string.Format("INIZIO - Inserimento documento '{0}' nella tabella di richiesta conversione", objServerPdfConversion.idProfile));

                        if (BusinessLogic.Documenti.DocManager.insDocRichiestaConversionePdf(objServerPdfConversion.idProfile))
                        {
                            // Lock in tabella di conversione inserito, da questo momento nessun altro utente
                            // potrà richiedere una conversione pdf per lo stesso documento
                            lockInserted = true;

                            logger.Debug(string.Format("FINE - Inserimento documento '{0}' nella tabella di richiesta conversione", objServerPdfConversion.idProfile));

                            // 4. Avvio del contesto transazionale
                            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
                            {
                                // 4. Creazione file xml per la conversione
                                logger.Debug("INIZIO - Creazione file xml per conversione PDF");
                                byte[] xmlByteArray = BusinessLogic.Documenti.FileManager.creaXmlCOnversionePdfServer(infoUtente, objServerPdfConversion);
                                logger.Debug("FINE - Creazione file xml per conversione PDF");

                                logger.Debug("Creazione file da convertire");
                                byte[] docByteArray = objServerPdfConversion.content;

                                // 5. Invio file di metadati e file da convertire nella coda di stampa
                                logger.Debug("INIZIO - Invio file da convertire alla coda di conversione");
                                WebClient webClient = new WebClient();
                                string addressServerCodaPdf = System.Configuration.ConfigurationManager.AppSettings["ADDRESS_SERVER_CODA_PDF"];
                                string fileName = Guid.NewGuid().ToString();
                                string urlDoc = string.Format("{0}{1}.{2}", addressServerCodaPdf, fileName, BusinessLogic.Documenti.FileManager.getExtFileFromPath(objServerPdfConversion.fileName));
                                webClient.UploadData(urlDoc, "PUT", docByteArray);
                                string urlXml = string.Format("{0}{1}.{2}", addressServerCodaPdf, fileName, "");
                                webClient.UploadData(urlXml, "PUT", xmlByteArray);
                                logger.Debug("FINE - Invio file da convertire alla coda di conversione");

                                // 6. CheckOut del documento
                                logger.Debug("INIZIO - Checkout del documento da convertire");
                                DocsPaVO.CheckInOut.CheckOutStatus checkOutStatus;
                                DocsPaVO.Validations.ValidationResultInfo checkOutRetValue = BusinessLogic.CheckInOut.CheckInOutServices.CheckOut(objServerPdfConversion.idProfile, objServerPdfConversion.docNumber, null, null, infoUtente, out checkOutStatus);
                                logger.Debug("FINE - Checkout del documento da convertire");

                                if (!checkOutRetValue.Value)
                                {
                                    // 6a. Si è verificato un errore in fase di CheckOut del documento
                                    string checkOutErrorMessage = string.Empty;

                                    // In caso di errore nel checkout del documento
                                    if (checkOutRetValue.BrokenRules != null && checkOutRetValue.BrokenRules.Count > 0)
                                        checkOutErrorMessage = ((DocsPaVO.Validations.BrokenRule)checkOutRetValue.BrokenRules[0]).Description;
                                    else
                                        checkOutErrorMessage = string.Format("Si è verificato un errore nel CheckOut del documento '{0}'", objServerPdfConversion.idProfile);

                                    logger.Debug(checkOutErrorMessage);
                                }
                                else
                                {
                                    // 7. Completamento delle modifiche (in tabella di conversione e checkout nel documentale)
                                    transactionContext.Complete();

                                    retValue = true;
                                }
                            }
                        }
                        else
                        {
                            // 3a. Errore in inserimento coda di conversione (tabella DPA_CONVERT_PDF_SERVER)
                            logger.Debug(string.Format("FINE - Errore in inserimento documento '{0}' in tabella di richiesta conversione", objServerPdfConversion.idProfile));
                        }
                    }
                    else
                    {
                        // 2a. Il documento è già in stato di conversione conversione PDF
                        logger.Debug(string.Format("Il documento '{0}' è già in stato di conversione PDF", objServerPdfConversion.idProfile));
                    }
                }
                else
                {
                    // 1a. File già in formato PDF
                    logger.Debug(string.Format("Il file associato al documento '{0}' è già in formato PDF", objServerPdfConversion.idProfile));
                }
            }
            catch (Exception ex)
            {
                logger.Debug("Errore in BusinessLogic.LiveCycle.LiveCyclePdfConverter.cs - metodo: EnqueueServerPdfConversion", ex);
            }
            finally
            {
                if (!retValue && lockInserted)
                {
                    // Se l'inserimento nella coda di conversione non è andato a buon fine,
                    // rimozione del lock in tabella di richiesta conversione
                    BusinessLogic.Documenti.DocManager.delDocRichiestaConversionePdf(objServerPdfConversion.idProfile);

                    lockInserted = false;
                }

                logger.Debug(string.Format("FINE: EnqueueServerPdfConversion, Documento '{0}', Utente '{1}'", objServerPdfConversion.idProfile, infoUtente.userId));
            }
        }
        */

        // Invio al servizio DPA di LC o tramite 
        static bool UploadData(string AddressServerCodaPdf, string FileName, string Ext, byte[] content)
        {
            //switch.. se voglio streamed.
            bool streamed = true;
            if (AddressServerCodaPdf.ToLower().EndsWith (".svc"))
            {
                BasicHttpBinding bin = new BasicHttpBinding();
                bin.TransferMode = TransferMode.Streamed;
                bin.TextEncoding = UTF8Encoding.UTF8;
                bin.MessageEncoding = WSMessageEncoding.Mtom;
                EndpointAddress ep = new EndpointAddress(AddressServerCodaPdf);
                LCDocsPaService.Services.IDpaConnector dpa = ChannelFactory<LCDocsPaService.Services.IDpaConnector>.CreateChannel(bin, ep);

              

                string fileName = string.Format("{1}.{2}", FileName, Ext);

                if (streamed)
                {
                    //Streamed
                    //Andrebbe usato un filestream, ma ottengo solo il content come buffer 
                    using (MemoryStream ms = new MemoryStream(content))
                    {
                        LCDocsPaService.Services.UploadResponse resp = dpa.UploadFileStream(new LCDocsPaService.Services.RemoteFileInfo { FileName = fileName, FileByteStream = ms });
                        return resp.UploadSucceeded;
                    }

                }
                else
                {
                    //buffered
                    return dpa.UploadFileArray(fileName, content);
                }
            } else 
            {
                WebClient webClient = new WebClient();
                string url = string.Format("{0}{1}.{2}", AddressServerCodaPdf, FileName, Ext);
                webClient.UploadData(url, "PUT", content);
                return true;
            }
        }
    
    
     
        #region MEV 1.5 F02_01
        /// <summary>
        /// Inserisce un documento nella coda di conversione pdf lato server (versione per le chiamate da backend)
        /// </summary>
        /// <returns></returns>
        public static void EnqueueServerPdfConversion(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.documento.ObjServerPdfConversion objServerPdfConversion, System.Web.HttpContext context)
        {
            logger.Debug(string.Format("INIZIO: EnqueueServerPdfConversion, Documento '{0}', Utente '{1}'", objServerPdfConversion.idProfile, infoUtente.userId));

            // 
            bool retValue = false;

            bool lockInserted = false;

            try
            {
                // 1. Verifica se il file non è di tipo PDF
                if (BusinessLogic.Documenti.FileManager.getExtFileFromPath(objServerPdfConversion.fileName).ToUpper() != "PDF".ToUpper())
                {
                    // 2. Verifica che il documento non sia già in stato di conversione PDF
                    if (!BusinessLogic.Documenti.DocManager.isDocInConversionePdf(objServerPdfConversion.idProfile))
                    {
                        // 3. Inserimento del documento nella tabella di richiesta conversione
                        // NB. L'inserimento del lock in tabella deve essere eseguito prima della creazione del contesto transazionale,
                        //     ciò per evitare di accettare altre richieste di conversione sullo stesso documento da parte di altri utenti
                        //     mentre la prima richiesta è ancora in fase di completamento
                        logger.Debug(string.Format("INIZIO - Inserimento documento '{0}' nella tabella di richiesta conversione", objServerPdfConversion.idProfile));

                        if (BusinessLogic.Documenti.DocManager.insDocRichiestaConversionePdf(objServerPdfConversion.idProfile))
                        {
                            // Lock in tabella di conversione inserito, da questo momento nessun altro utente
                            // potrà richiedere una conversione pdf per lo stesso documento
                            lockInserted = true;

                            logger.Debug(string.Format("FINE - Inserimento documento '{0}' nella tabella di richiesta conversione", objServerPdfConversion.idProfile));

                            // 4. Avvio del contesto transazionale
                            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
                            {
                                // 4. Creazione file xml per la conversione
                                logger.Debug("INIZIO - Creazione file xml per conversione PDF");
                                byte[] xmlByteArray = BusinessLogic.Documenti.FileManager.creaXmlConversionePdfServer(infoUtente, objServerPdfConversion, context);
                                logger.Debug("FINE - Creazione file xml per conversione PDF");

                                logger.Debug("Creazione file da convertire");
                                byte[] docByteArray = objServerPdfConversion.content;

                                // 5. Invio file di metadati e file da convertire nella coda di stampa
                                logger.Debug("INIZIO - Invio file da convertire alla coda di conversione");

                                string addressServerCodaPdf = System.Configuration.ConfigurationManager.AppSettings["ADDRESS_SERVER_CODA_PDF"];
                                string fileName = Guid.NewGuid().ToString();
                                //string urlDoc = string.Format("{0}{1}.{2}", addressServerCodaPdf, fileName, BusinessLogic.Documenti.FileManager.getExtFileFromPath(objServerPdfConversion.fileName));

                                UploadData(addressServerCodaPdf, fileName, BusinessLogic.Documenti.FileManager.getExtFileFromPath(objServerPdfConversion.fileName), docByteArray);

                                //UploadData(urlDoc, "PUT", docByteArray);
                                //string urlXml = string.Format("{0}{1}.{2}", addressServerCodaPdf, fileName, "");
                                
                                //UploadData(urlXml, "PUT", xmlByteArray);
                                UploadData(addressServerCodaPdf, fileName, "", xmlByteArray);
                                logger.Debug("FINE - Invio file da convertire alla coda di conversione");

                                // 6. CheckOut del documento
                                logger.Debug("INIZIO - Checkout del documento da convertire");
                                DocsPaVO.CheckInOut.CheckOutStatus checkOutStatus;
                                DocsPaVO.Validations.ValidationResultInfo checkOutRetValue;
                                if (context != null)
                                    checkOutRetValue = BusinessLogic.CheckInOut.CheckInOutServices.CheckOutNoSecurity(objServerPdfConversion.idProfile, objServerPdfConversion.docNumber, null, null, infoUtente, out checkOutStatus);
                                else 
                                    checkOutRetValue = BusinessLogic.CheckInOut.CheckInOutServices.CheckOut(objServerPdfConversion.idProfile, objServerPdfConversion.docNumber, null, null, infoUtente, out checkOutStatus);

                                logger.Debug("FINE - Checkout del documento da convertire");

                                if (!checkOutRetValue.Value)
                                {
                                    // 6a. Si è verificato un errore in fase di CheckOut del documento
                                    string checkOutErrorMessage = string.Empty;

                                    // In caso di errore nel checkout del documento
                                    if (checkOutRetValue.BrokenRules != null && checkOutRetValue.BrokenRules.Count > 0)
                                        checkOutErrorMessage = ((DocsPaVO.Validations.BrokenRule)checkOutRetValue.BrokenRules[0]).Description;
                                    else
                                        checkOutErrorMessage = string.Format("Si è verificato un errore nel CheckOut del documento '{0}'", objServerPdfConversion.idProfile);

                                    logger.Debug(checkOutErrorMessage);
                                }
                                else
                                {
                                    // 7. Completamento delle modifiche (in tabella di conversione e checkout nel documentale)
                                    transactionContext.Complete();

                                    retValue = true;
                                }
                            }
                        }
                        else
                        {
                            // 3a. Errore in inserimento coda di conversione (tabella DPA_CONVERT_PDF_SERVER)
                            logger.Debug(string.Format("FINE - Errore in inserimento documento '{0}' in tabella di richiesta conversione", objServerPdfConversion.idProfile));
                        }
                    }
                    else
                    {
                        // 2a. Il documento è già in stato di conversione conversione PDF
                        logger.Debug(string.Format("Il documento '{0}' è già in stato di conversione PDF", objServerPdfConversion.idProfile));
                    }
                }
                else
                {
                    // 1a. File già in formato PDF
                    logger.Debug(string.Format("Il file associato al documento '{0}' è già in formato PDF", objServerPdfConversion.idProfile));
                }
            }
            catch (Exception ex)
            {
                logger.Debug("Errore in BusinessLogic.LiveCycle.LiveCyclePdfConverter.cs - metodo: EnqueueServerPdfConversion", ex);
            }
            finally
            {
                if (!retValue && lockInserted)
                {
                    // Se l'inserimento nella coda di conversione non è andato a buon fine,
                    // rimozione del lock in tabella di richiesta conversione
                    BusinessLogic.Documenti.DocManager.delDocRichiestaConversionePdf(objServerPdfConversion.idProfile);

                    lockInserted = false;
                }

                logger.Debug(string.Format("FINE: EnqueueServerPdfConversion, Documento '{0}', Utente '{1}'", objServerPdfConversion.idProfile, infoUtente.userId));
            }
        }
        #endregion
        /// <summary>
        /// Elimina il documento della coda di conversione pdf lato server
        /// </summary>
        /// <returns></returns>
        public static void DequeueServerPdfConversion(string nameDocConvertito, string nameFileXml, byte[] docConvertito, byte[] xml, ref DocsPaVO.utente.InfoUtente infoUser, ref string noteLog, ref DocsPaVO.documento.SchedaDocumento sDoc)
        {
            logger.Debug(
                string.Format("INIZIO: EnqueueServerPdfConversion, nameDocConvertito: '{0}', nameFileXml: '{1}', docConvertiro: '{2}', xml: '{3}'",
                            nameDocConvertito,
                            nameFileXml,
                            (docConvertito != null ? docConvertito.Length.ToString() : "NULL"),
                            (xml != null ? xml.Length.ToString() : "NULL")));

            DocsPaVO.utente.InfoUtente infoUtente = null;
            DocsPaVO.documento.SchedaDocumento schedaDocPrincipale = null;

            //string noteGeneraliTrasmissione = string.Empty;
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    logger.Debug("INIZIO - Lettura file xml di metadati");

                    // 1. Lettura file xml di metadati
                    MemoryStream ms = new MemoryStream(xml);
                    XmlDocument xDoc = new XmlDocument();
                    xDoc.Load(ms);
                    XmlNode node = xDoc.DocumentElement;
                    
                    logger.Debug("Creazione infoUtente");
                    //Creazione infoUtente
                    infoUtente = new DocsPaVO.utente.InfoUtente();
                    infoUtente.dst = node.SelectSingleNode("DST").InnerText.ToString();
                    infoUtente.idAmministrazione = node.SelectSingleNode("ID_AMM").InnerText.ToString();
                    infoUtente.idCorrGlobali = node.SelectSingleNode("ID_CORR_GLOBALI").InnerText.ToString();
                    infoUtente.idGruppo = node.SelectSingleNode("ID_GRUPPO").InnerText.ToString();
                    infoUtente.idPeople = node.SelectSingleNode("ID_PEOPLE").InnerText.ToString();
                    infoUtente.sede = node.SelectSingleNode("SEDE").InnerText.ToString();
                    infoUtente.urlWA = node.SelectSingleNode("URLWA").InnerText.ToString();
                    infoUtente.userId = node.SelectSingleNode("USERID").InnerText.ToString();
 
                    if (node.SelectSingleNode("ID_PEOPLE_DELEGATO") != null && !string.IsNullOrEmpty(node.SelectSingleNode("ID_PEOPLE_DELEGATO").ToString()))
                    {
                        infoUtente.delegato = new DocsPaVO.utente.InfoUtente() { idPeople = node.SelectSingleNode("ID_PEOPLE_DELEGATO").InnerText.ToString() };
                    }
                    infoUser = infoUtente;
                    logger.Debug("Creazione fileRequest");
                    //Creazione FileRequest
                    DocsPaVO.documento.FileRequest fileRequest = new DocsPaVO.documento.FileRequest();
                    fileRequest.autore = node.SelectSingleNode("AUTORE").InnerText.ToString();
                    fileRequest.dataInserimento = node.SelectSingleNode("DATA_INSERIMENTO").InnerText.ToString();
                    fileRequest.descrizione = node.SelectSingleNode("DESCRIZIONE").InnerText.ToString();
                    fileRequest.docNumber = node.SelectSingleNode("DOCNUMBER").InnerText.ToString();

                    string OriginalfileName = null;
                    try
                    {
                        //potrebbe non esistere
                        OriginalfileName=node.SelectSingleNode("ORIGINAL_FILE_NAME").InnerText.ToString();
                    }
                    catch
                    {
                        logger.Debug("ORIGINAL_FILE_NAME non presente");
                    }

                    //Faillace : provo nel caso sia codificato in base 64 (modifica del 4-6-2014)
                    try
                    {
                        if (OriginalfileName != null)
                        {
                            byte[] ofnBytes = Convert.FromBase64String(OriginalfileName);
                            if (ofnBytes !=null)
                                OriginalfileName = System.Text.UTF8Encoding.UTF8.GetString(ofnBytes);
                        }
                    }
                    catch
                    {
                        // se entro nella catch il filename non è codificato base64, non defo fare nulla perchè il nome file sarà
                        // in chiaro, ma avrà le accentate errate in quanto convertito ascii
                        // se prensente un punto interrogativo (non ammesso nel filename ma introdotto dal convertitore) lo converto in underscore.
                        if (OriginalfileName != null)
                            OriginalfileName = OriginalfileName.Replace('?', '_');

                    }

                    logger.Debug("Informazioni documento");
                    //Recupero informazioni del documento 
                    string idProfile = node.SelectSingleNode("ID_PROFILE").InnerText.ToString();
                    string docNumber = node.SelectSingleNode("DOCNUMBER").InnerText.ToString();
                    logger.Debug("FINE - Lettura file xml di metadati");

                    // 2. Verifica se il documento è in stato di conversione PDF
                    if (BusinessLogic.Documenti.DocManager.isDocInConversionePdf(idProfile))
                    {
                        // 3. Controllo che il documento in stato di conversione sia in checkout dallo stesso utente
                        logger.Debug("INIZIO - Reperimento dello stato di CheckOut del documento '{0}'");
                        DocsPaVO.CheckInOut.CheckOutStatus statusCheckOut = null;

                        try
                        {
                            statusCheckOut = BusinessLogic.CheckInOut.CheckInOutServices.GetCheckOutStatus(idProfile, docNumber, infoUtente);
                        }
                        catch (Exception ex)
                        {
                            // 3a. Si è verificato un errore nel reperimento delle informazioni di stato del documento nel sistema documentale
                            // In tali frangenti, non è possibile effettuare l'undo checkout nel sistema documentale e la conseguente rimozione del lock di conversione.
                            // Pertanto sarà notificata una trasmissione di fallimento all'utente segnalando, tra le note individuali, che c'è stato un problema
                            // nel sistema documentale e che il documento dovrà essere sbloccato a mano.
                            //noteGeneraliTrasmissione = string.Format("Problemi durante la conversione in PDF per il documento '{0}'. " +
                            //                                         "Sarà necessario sbloccare manualmente il documento e ripetere l'operazione.", idProfile);
                            noteLog = string.Format("Problemi durante la conversione in PDF per il documento '{0}'. " +
                                "Sarà necessario sbloccare manualmente il documento e ripetere l'operazione.", idProfile);

                            //throw new ApplicationException(noteGeneraliTrasmissione, ex);
                            throw new ApplicationException(noteLog, ex);
                        }

                        logger.Debug("FINE - Reperimento dello stato di CheckOut del documento '{0}'");

                        if (statusCheckOut != null &&
                            statusCheckOut.DocumentNumber == fileRequest.docNumber &&
                            statusCheckOut.UserName.ToUpper() == infoUtente.userId.ToUpper())
                        {
                            logger.Debug("DocNumber CheckOut : " + statusCheckOut.DocumentNumber);
                            logger.Debug("DocNumber FileRequest : " + fileRequest.docNumber);
                            logger.Debug("IdRole CheckOut : " + statusCheckOut.IDRole);
                            logger.Debug("IdCorrGlobali infoUtente : " + infoUtente.idCorrGlobali);
                            logger.Debug("UserName CheckOut : " + statusCheckOut.UserName);
                            logger.Debug("UserName infoUtente : " + infoUtente.userId);

                            statusCheckOut.DocumentLocation = nameDocConvertito;

                            bool undoCheckOut = false;

                            // Se il parametro "docConvertito" è null allora ci sono stati problemi nella conversione
                            if (docConvertito != null)
                            {
                                // 4. CheckIn del documento
                                if (!CheckIn(infoUtente, statusCheckOut, docConvertito, "Documento convertito in pdf lato server"))
                                {
                                    // 4a. Errore in CheckIn, viene effettuato l'UndoCheckOut
                                    UndoCheckOut(infoUtente, statusCheckOut);
                                    undoCheckOut = true;
                                }
                                else
                                {
                                    if (!String.IsNullOrEmpty(OriginalfileName))
                                    {
                                        //documento convertito, gli rimetto il nome originale
                                        OriginalfileName = System.IO.Path.GetFileNameWithoutExtension(OriginalfileName)+".pdf";
                                        DocsPaVO.documento.FileRequest fr = (DocsPaVO.documento.FileRequest)BusinessLogic.Documenti.DocManager.getDettaglioNoSecurity(infoUtente, docNumber).documenti[0];
                                        BusinessLogic.Documenti.FileManager.setOriginalFileName(infoUtente, fr, OriginalfileName,true);
                                    }
                                }
                                
                            }
                            else
                            {
                                // 4a. File non convertito, UndoCheckOut del documento
                                UndoCheckOut(infoUtente, statusCheckOut);
                                undoCheckOut = true;
                            }

                            logger.Debug("Recupero scheda documento");

                            DocsPaVO.documento.SchedaDocumento schDoc = null;

                            try
                            {
                                // 5. Reperimento scheda del documento convertito
                                schDoc = BusinessLogic.Documenti.DocManager.getDettaglioNoSecurity(infoUtente, docNumber);
                            }
                            catch (Exception ex)
                            {
                                // Errore nel reperimento del dettaglio del documento
                                logger.Debug(ex);

                                // NB. Nonostante l'errore, il documento è stato correttamente convertito (o meno)
                                // e il checkin (o undocheckout) è stato effettauto correttamente.
                                // Pertanto il reperimento della scheda è ininfluente e come nota generale
                                // viene impostato l'id del documento, senza distinguere se principale o allegato
                                //noteGeneraliTrasmissione = string.Format("Il documento '{0}' è stato convertito in PDF.", docNumber);
                                noteLog = string.Format("Il documento '{0}' è stato convertito in PDF.", docNumber);
                            }

                            if (schDoc != null)
                            {
                                // Verifico se il doc convertito è un allegato
                                bool isAllegato = (schDoc.documentoPrincipale != null);

                                if (isAllegato)
                                {
                                    try
                                    {
                                        // 5a. Reperimento scheda doc principale per l'allegato
                                        schedaDocPrincipale = BusinessLogic.Documenti.DocManager.getDettaglioNoSecurity(
                                                        infoUtente,
                                                        schDoc.documentoPrincipale.docNumber);
                                    }
                                    catch (Exception ex)
                                    {
                                        // Errore nel reperimento del dettaglio del documento allegato
                                        logger.Debug(ex);
                                    }

                                    if (undoCheckOut)
                                        //noteGeneraliTrasmissione = string.Format("Problemi durante la conversione in PDF per l'allegato '{0}'. Ripetere l'operazione.", schDoc.docNumber);
                                        noteLog = string.Format("Problemi durante la conversione in PDF per l'allegato '{0}'. Ripetere l'operazione.", schDoc.docNumber);
                                    else
                                        // Impostazione note generali di trasmissione
                                        //noteGeneraliTrasmissione = string.Format("Allegato '{0}' convertito in PDF.", schDoc.docNumber);
                                        noteLog = string.Format("Allegato '{0}' convertito in PDF.", schDoc.docNumber);
                                }

                                else
                                {
                                    schedaDocPrincipale = schDoc;

                                    // Impostazione note generali di trasmissione
                                    string docName = string.Empty;

                                    if (schedaDocPrincipale.protocollo != null)
                                        docName = schedaDocPrincipale.protocollo.segnatura;
                                    else
                                        docName = schedaDocPrincipale.docNumber;

                                    if (undoCheckOut)
                                        //noteGeneraliTrasmissione = string.Format("Problemi durante la conversione in PDF per il documento '{0}'. Ripetere l'operazione.", docName);
                                        noteLog = string.Format("Problemi durante la conversione in PDF per il documento '{0}'. Ripetere l'operazione.", docName);
                                    else
                                        //noteGeneraliTrasmissione = string.Format("Il documento '{0}' è stato convertito in PDF.", docName);
                                        noteLog = string.Format("Il documento '{0}' è stato convertito in PDF.", docName);
                                }
                            }
                        }
                        else
                        {
                            logger.Debug(string.Format("Il documento '{0}' non è più bloccato nel sistema documentale per la conversione PDF", idProfile));
                        }
                    }
                    else
                    {
                        logger.Debug(string.Format("Il documento '{0}' non è più in coda di conversione PDF", idProfile));
                    }
                }
                catch (Exception ex)
                {
                    logger.Debug("Errore in BusinessLogic.Documenti.LifeCyclePdfConverter.cs - metodo: DequeueServerPdfConversion", ex);
                }
                finally
                {
                    sDoc = schedaDocPrincipale;
                    //ExecuteTrasmissione(infoUtente, schedaDocPrincipale, noteGeneraliTrasmissione);

                    // 6. Completamento della transazione in ogni caso
                    transactionContext.Complete();

                    logger.Debug(string.Format("FINE: EnqueueServerPdfConversion, nameDocConvertito: '{0}', nameFileXml: '{1}', docConvertiro: '{2}', xml: '{3}'",
                                nameDocConvertito,
                                nameFileXml,
                                (docConvertito != null ? docConvertito.Length.ToString() : "NULL"),
                                (xml != null ? xml.Length.ToString() : "NULL")));
                    logger.Debug("Tramissione di notifica conversione");

                }
            }
        }

        #endregion

        #region Private Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="status"></param>
        /// <param name="content"></param>
        /// <param name="checkInComment"></param>
        private static bool CheckIn(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.CheckInOut.CheckOutStatus status, byte[] content, string checkInComment)
        {
            DocsPaVO.Validations.ValidationResultInfo result = BusinessLogic.CheckInOut.CheckInOutServices.CheckIn(status, infoUtente, content, checkInComment);

            // Rimozione documento da tabella di conversione
            logger.Debug("Eliminazione documento da DocRichiestaConversionePdf");
            BusinessLogic.Documenti.DocManager.delDocRichiestaConversionePdf(status.IDDocument);

            DocsPaDB.Query_DocsPAWS.CheckInOut checkInOutDb = new DocsPaDB.Query_DocsPAWS.CheckInOut();

            // Reperimento dell'ultima versione del documento
            FileRequest fileRequest = checkInOutDb.GetFileRequest(status.IDDocument);

            BusinessLogic.Documenti.FileManager.processFileInformation(fileRequest, infoUtente);

            return result.Value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="status"></param>
        private static void UndoCheckOut(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.CheckInOut.CheckOutStatus status)
        {
            DocsPaVO.Validations.ValidationResultInfo result = BusinessLogic.CheckInOut.CheckInOutServices.UndoCheckOut(status, infoUtente);

            if (!result.Value)
            {
                // Errore nell'undocheckout del documento, viene effettuato un nuovo tentativo
                result = BusinessLogic.CheckInOut.CheckInOutServices.UndoCheckOut(status, infoUtente);
            }

            // Rimozione documento da tabella di conversione
            logger.Debug("Eliminazione documento da DocRichiestaConversionePdf");
            BusinessLogic.Documenti.DocManager.delDocRichiestaConversionePdf(status.IDDocument);
        }

        /// <summary>
        /// Trasmissione di notifica di conversione in PDF terminata (con successo o meno)
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="documentoConvertito"></param>
        /// <param name="noteGenerali">
        /// Note generali da inserire nella trasmissione
        /// </param>
        /// <param name="noteIndividuali"></param>
        private static void ExecuteTrasmissione(DocsPaVO.utente.InfoUtente infoUtente,
                                                DocsPaVO.documento.SchedaDocumento documentoConvertito,
                                                string noteGenerali)
        {
            try
            {
                if (documentoConvertito == null)
                    throw new ApplicationException("Scheda Documento non disponibile per la trasmissione");

                DocsPaVO.utente.Utente utente = BusinessLogic.Utenti.UserManager.getUtente(infoUtente.idPeople);

                //Effettuo una trasmissione all'utente per comunicare l'esito della conversione pdf
                DocsPaVO.trasmissione.Trasmissione trasmissione = new DocsPaVO.trasmissione.Trasmissione();

                trasmissione.utente = utente;
                trasmissione.tipoOggetto = DocsPaVO.trasmissione.TipoOggetto.DOCUMENTO;
                trasmissione.noteGenerali = noteGenerali;

                string commandText = "(SELECT SYSTEM_ID FROM DPA_CORR_GLOBALI WHERE ID_GRUPPO = " + infoUtente.idGruppo + ")";
                trasmissione.ruolo = BusinessLogic.Utenti.UserManager.getRuolo(commandText);
                trasmissione.infoDocumento = BusinessLogic.Documenti.DocManager.getInfoDocumento(documentoConvertito);

                DocsPaVO.trasmissione.RagioneTrasmissione ragione = BusinessLogic.Trasmissioni.RagioniManager.getRagioneNotifica(trasmissione.ruolo.idAmministrazione);
                DocsPaVO.utente.Corrispondente corr=null;
                  string tipoNotify = "";
              if( !string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["PDF_CONVERT_SERVER_NOTIFY"].ToString()))
              {
                  tipoNotify = System.Configuration.ConfigurationManager.AppSettings["PDF_CONVERT_SERVER_NOTIFY"].ToString();
              }

              if (tipoNotify != "" || tipoNotify!="0")
              {


                  if (tipoNotify == "RS")
                  {


                      corr = (DocsPaVO.utente.Corrispondente)BusinessLogic.Utenti.UserManager.getRuolo(commandText);
                      trasmissione = BusinessLogic.Trasmissioni.TrasmManager.addTrasmissioneSingola(trasmissione, corr, ragione, string.Empty, "S");
                      

                  }
                  if (tipoNotify == "RT")
                  {


                      corr = (DocsPaVO.utente.Corrispondente)BusinessLogic.Utenti.UserManager.getRuolo(commandText);
                      trasmissione = BusinessLogic.Trasmissioni.TrasmManager.addTrasmissioneSingola(trasmissione, corr, ragione, string.Empty, "T");


                  }
                  if (tipoNotify == "US")
                  {


                      //corr = (DocsPaVO.utente.Corrispondente) utente;
                      corr = BusinessLogic.Utenti.UserManager.getCorrispondenteByIdPeople(utente.idPeople, DocsPaVO.addressbook.TipoUtente.INTERNO, infoUtente);
                      trasmissione = BusinessLogic.Trasmissioni.TrasmManager.addTrasmissioneSingola(trasmissione, corr, ragione, string.Empty, "S");

                  }

                  trasmissione = BusinessLogic.Trasmissioni.ExecTrasmManager.saveExecuteTrasmMethod(infoUtente.urlWA, trasmissione);
                  if (trasmissione == null)
                      throw new ApplicationException();
              }
            }
            catch (Exception ex)
            {
                string errorMessage = string.Format("Si è verificato un errore nell'invio della trasmissione di notifica all'utente '{0}' di conversione in PDF del documento. NoteGenerali di trasmissione: '{1}', Note individuali di trasmissione: '{2}'",
                                                       infoUtente.userId,
                                                       noteGenerali,
                                                       string.Empty);

                logger.Debug(errorMessage, ex);
            }
        }

        #endregion
    }
}
