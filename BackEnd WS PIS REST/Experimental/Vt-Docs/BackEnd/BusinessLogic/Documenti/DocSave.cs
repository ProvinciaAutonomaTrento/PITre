using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using log4net;

namespace BusinessLogic.Documenti
{
    /// <summary>
    /// </summary>
    public class DocSave
    {
        private static ILog logger = LogManager.GetLogger(typeof(DocSave));

        public static Mutex semProtNuovo = new Mutex();

        public static DocsPaVO.documento.SchedaDocumento InoltraDoc(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.utente.Ruolo ruolo, DocsPaVO.documento.SchedaDocumento schedaOLD)
        {
            string err = "";
            DocsPaVO.documento.SchedaDocumento schedaNEW = null;
            bool daAggiornareUffRef = false;
            string filepath = "";
            DocsPaVO.documento.FileDocumento fd = null;

            //se arriva sch con solo system_id e docnumber la ricerco
            if (schedaOLD != null && schedaOLD.protocollo == null)
            {
                schedaOLD = BusinessLogic.Documenti.DocManager.getDettaglio(infoUtente, schedaOLD.systemId, schedaOLD.docNumber);
            }
            schedaNEW = new DocsPaVO.documento.SchedaDocumento();
            
            schedaNEW.systemId = null;
            schedaNEW.docNumber = null;
            schedaNEW.oggetto = schedaOLD.oggetto;
            schedaNEW.idPeople = infoUtente.idPeople;
            schedaNEW.userId = infoUtente.userId;
            schedaNEW.registro = schedaOLD.registro;
            //schedaNEW.tipoProto = "P";
            schedaNEW.typeId = "LETTERA";
            schedaNEW.privato = schedaOLD.privato;
            schedaNEW.mezzoSpedizione = schedaOLD.mezzoSpedizione;
            schedaNEW.descMezzoSpedizione = schedaOLD.descMezzoSpedizione;
            schedaNEW.tipologiaAtto = schedaOLD.tipologiaAtto;
            schedaNEW.template = schedaOLD.template;
            //schedaNEW.predisponiProtocollazione = true;

            //creo un nuovo documento grigio
            logger.Debug("Creazione doc...");
            schedaNEW = BusinessLogic.Documenti.DocSave.addDocGrigia(schedaNEW, infoUtente, ruolo);

            //Dopo aver creato il documento grigio lo predispongo alla protocollazione
            logger.Debug("Predisponi doc...");
            schedaNEW.tipoProto = "P";
            schedaNEW.typeId = schedaOLD.typeId;
            schedaNEW.predisponiProtocollazione = true;

            schedaNEW.protocollo = new DocsPaVO.documento.ProtocolloUscita();

            DocsPaVO.utente.Corrispondente corr = ruolo.uo;
            ((DocsPaVO.documento.ProtocolloUscita)schedaNEW.protocollo).mittente = corr;                   
            //((DocsPaVO.documento.ProtocolloUscita)schedaNEW.protocollo).mittente = ((DocsPaVO.documento.ProtocolloUscita)schedaOLD.protocollo).mittente;
            //((DocsPaVO.documento.ProtocolloUscita)schedaNEW.protocollo).destinatari = ((DocsPaVO.documento.ProtocolloUscita)schedaOLD.protocollo).destinatari;
            //((DocsPaVO.documento.ProtocolloUscita)schedaNEW.protocollo).destinatariConoscenza = ((DocsPaVO.documento.ProtocolloUscita)schedaOLD.protocollo).destinatariConoscenza;
                 
               
            schedaNEW = BusinessLogic.Documenti.DocSave.save(infoUtente, schedaNEW, false, out daAggiornareUffRef, ruolo);
            logger.Debug("Creato documento grigio e predisposto alla protocollazione");

            //Copia del documento principale e degli allegati del vecchio documento
            //nel nuovo documento come allegati
            #region ricerca e copia degli allegati
            logger.Debug("Inserimento degli allegati");
            for (int i = 0; schedaOLD.allegati != null && i < schedaOLD.allegati.Count; i++)
            {
                //estrazione dati dell'allegato
                DocsPaVO.documento.Allegato documentoAllegato = (DocsPaVO.documento.Allegato)schedaOLD.allegati[i];
                filepath = documentoAllegato.docServerLoc + documentoAllegato.path;
                string nomeAllegato = "Inoltro " + documentoAllegato.fileName;
                string numPagine = documentoAllegato.numeroPagine.ToString();
                string titoloDoc = documentoAllegato.descrizione;
                logger.Debug("Inserimento allegato " + nomeAllegato);
                DocsPaVO.documento.Allegato all = new DocsPaVO.documento.Allegato();
                all.descrizione = "allegato " + i;
                logger.Debug("docnumber=" + schedaNEW.docNumber);
                all.docNumber = schedaNEW.docNumber;
                all.fileName = getFileName(nomeAllegato);
                all.version = "0";
                //numero pagine
                if (numPagine != null && !numPagine.Trim().Equals(""))
                {
                    all.numeroPagine = Int32.Parse(numPagine);
                }
                //descrizione allegato
                if (titoloDoc != null && !titoloDoc.Trim().Equals(""))
                {
                    all.descrizione = "Inoltro " + titoloDoc;
                }
                BusinessLogic.Documenti.AllegatiManager.aggiungiAllegato(infoUtente, all);
                logger.Debug("Allegato id=" + all.versionId);
                logger.Debug("Allegato version label=" + all.versionLabel);
                logger.Debug("Inserimento nel filesystem");
                DocsPaVO.documento.FileDocumento fdAllNew = new DocsPaVO.documento.FileDocumento();
                DocsPaVO.documento.FileDocumento fdAll = null;
                if (Int32.Parse(documentoAllegato.fileSize) > 0)
                {
                    try
                    {
                        fdAll = BusinessLogic.Documenti.FileManager.getFile((DocsPaVO.documento.FileRequest)schedaOLD.allegati[i], infoUtente);
                        if (fdAll == null)
                            throw new Exception("Errore nel reperimento dell'allegato numero" + i.ToString());
                        fdAllNew.content = fdAll.content;
                        fdAllNew.length = fdAll.length;
                        fdAllNew.name = fdAll.name;
                        fdAllNew.fullName = fdAll.fullName;
                        fdAllNew.contentType = fdAll.contentType;
                        DocsPaVO.documento.FileRequest fr = (DocsPaVO.documento.FileRequest)all;
                        if (!BusinessLogic.Documenti.FileManager.putFile(ref fr, fdAllNew, infoUtente, out err))
                            throw new Exception(err);
                        logger.Debug("Allegato " + i + " inserito");
                    }
                    catch (Exception ex)
                    {
                        err = "Errore nel reperimento dell'allegato numero " + i.ToString() + " : " + ex.Message;
                        if (schedaNEW != null && schedaNEW.systemId != null && schedaNEW.systemId != "")
                        {
                            //se il putFile va in errore devo rimuovere il profile (predisposto appena inserito)
                            BusinessLogic.Documenti.DocManager.ExecRimuoviSchedaMethod(infoUtente, schedaNEW);

                            logger.Debug("Eseguita rimozione profilo");
                        }
                        logger.Debug(err);//se non si riesce a prener eil file per problemi vari continuo comunque per arrivare a eseguiTrasmissione
                        throw ex;
                    }
                }
            }
            #endregion
            #region copia documento principale in allegato
            //DocsPaVO.documento.FileDocumento fdNew = null;
            if (schedaOLD.documenti != null && schedaOLD.documenti[0] != null &&
                Int32.Parse(((DocsPaVO.documento.FileRequest)schedaOLD.documenti[0]).fileSize) > 0)
            {
                try
                {
                    fd = BusinessLogic.Documenti.FileManager.getFile((DocsPaVO.documento.FileRequest)schedaOLD.documenti[0], infoUtente);
                    if (fd == null)
                        throw new Exception("Errore nel reperimento del file principale.");
                    //copio in un nuovo filerequest perchè putfile lo vuole senza

                    DocsPaVO.documento.Allegato allDocPrinc = new DocsPaVO.documento.Allegato();
                    allDocPrinc.descrizione = "Inoltro documento principale id: " + schedaOLD.docNumber;
                    allDocPrinc.docNumber = schedaNEW.docNumber;
                    allDocPrinc.fileName = getFileName(fd.name);
                    allDocPrinc.version = "0";
                    BusinessLogic.Documenti.AllegatiManager.aggiungiAllegato(infoUtente, allDocPrinc);

                    DocsPaVO.documento.FileDocumento fdAllNew = new DocsPaVO.documento.FileDocumento();
                    DocsPaVO.documento.FileDocumento fdAll = null;
                    fdAll = BusinessLogic.Documenti.FileManager.getFile((DocsPaVO.documento.FileRequest)schedaOLD.documenti[0], infoUtente);
                    if (fdAll == null)
                        throw new Exception("Errore nel reperimento del file principale come allegato");
                    fdAllNew.content = fdAll.content;
                    fdAllNew.length = fdAll.length;
                    fdAllNew.name = fdAll.name;
                    fdAllNew.fullName = fdAll.fullName;
                    fdAllNew.contentType = fdAll.contentType;
                    DocsPaVO.documento.FileRequest fr = (DocsPaVO.documento.FileRequest)allDocPrinc;
                    if (!BusinessLogic.Documenti.FileManager.putFile(ref fr, fdAllNew, infoUtente, out err))
                        throw new Exception(err);
                    logger.Debug("Doc principale inserito come allegato");

                    DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
                    schedaNEW.allegati = doc.GetAllegati(schedaNEW.docNumber, string.Empty);
                }
                catch (Exception ex)
                {
                    err = "Errore nel reperimento del file principale : " + ex.Message;
                    if (schedaNEW != null && schedaNEW.systemId != null && schedaNEW.systemId != "")
                    {
                        //se il putFile va in errore devo rimuovere il profile (predisposto appena inserito)
                        BusinessLogic.Documenti.DocManager.ExecRimuoviSchedaMethod(infoUtente, schedaNEW);
                        logger.Debug("Eseguita rimozione profilo");
                    }
                    logger.Debug(err);
                    throw ex;
                }
            }
            #endregion


           

            err = err + " " + schedaNEW.docNumber;
            if (schedaNEW != null && schedaNEW.docNumber != null)
            {
                err = "errore " + err + " documento  rimosso: " + schedaNEW.docNumber;
            }
            else
                err = "errore " + err;
            logger.Debug(err);
            schedaNEW.predisponiProtocollazione = true;
            return schedaNEW;
        }



        /// <summary>
        /// Riproponi avanzato: ripropone il documento con la copia dell'eventuale documento 
        /// principale e degli eventuali allegati
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="ruolo"></param>
        /// <param name="sch"></param>
        /// <returns></returns>
        public static DocsPaVO.documento.SchedaDocumento riproponiConCopiaDoc(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.utente.Ruolo ruolo, DocsPaVO.documento.SchedaDocumento sch)
        {
            string err = "";
            DocsPaVO.documento.SchedaDocumento sd = null;
            bool daAggiornareUffRef = false;
            string filepath = "";
            DocsPaVO.documento.FileDocumento fd = null;

            //se arriva sch con solo system_id e docnumber la ricerco
            if (sch != null && sch.protocollo == null)
            {
                sch = BusinessLogic.Documenti.DocManager.getDettaglio(infoUtente, sch.systemId, sch.docNumber);
            }
            sd = new DocsPaVO.documento.SchedaDocumento();
            if (sch.documenti != null && sch.documenti[0] != null &&
                Int32.Parse(((DocsPaVO.documento.FileRequest)sch.documenti[0]).fileSize) > 0)
            {
                sd.appId = sch.appId;
            }
            else
                sd.appId = "ACROBAT";
            if (sd.appId == null)
                sd.appId = "ACROBAT";

            sd.systemId = null;
            sd.docNumber = null;
            sd.oggetto = sch.oggetto;
            sd.idPeople = infoUtente.idPeople;
            sd.userId = infoUtente.userId;
            sd.registro = sch.registro;
            sd.tipoProto = "G";
            sd.typeId = "LETTERA";
            sd.privato = sch.privato;
            sd.mezzoSpedizione = sch.mezzoSpedizione;
            sd.descMezzoSpedizione = sch.descMezzoSpedizione;
            sd.tipologiaAtto = sch.tipologiaAtto;
            sd.predisponiProtocollazione = true;
            
            //creo un nuovo documento grigio
            logger.Debug("Creazione doc...");
            sd = BusinessLogic.Documenti.DocSave.addDocGrigia(sd, infoUtente, ruolo);

            //Dopo aver creato il documento grigio lo predispongo alla protocollazione
            logger.Debug("Predisponi doc...");
            sd.tipoProto = sch.tipoProto;
            sd.typeId = sch.typeId;
            sd.predisponiProtocollazione = true;
            switch (sd.tipoProto)
            {
                case "A":
                    sd.protocollo = new DocsPaVO.documento.ProtocolloEntrata();
                    ((DocsPaVO.documento.ProtocolloEntrata)sd.protocollo).mittente = ((DocsPaVO.documento.ProtocolloEntrata)sch.protocollo).mittente;
                    ((DocsPaVO.documento.ProtocolloEntrata)sd.protocollo).mittenteIntermedio = ((DocsPaVO.documento.ProtocolloEntrata)sch.protocollo).mittenteIntermedio;
                    //if (eUffRef)
                    //{
                    //    ((DocsPaVO.documento.ProtocolloEntrata)sd.protocollo).ufficioReferente = ((DocsPaVO.documento.ProtocolloEntrata)sch.protocollo).ufficioReferente;
                    //}
                    break;
                case "P":
                    sd.protocollo = new DocsPaVO.documento.ProtocolloUscita();
                    ((DocsPaVO.documento.ProtocolloUscita)sd.protocollo).mittente = ((DocsPaVO.documento.ProtocolloUscita)sch.protocollo).mittente;
                    ((DocsPaVO.documento.ProtocolloUscita)sd.protocollo).destinatari = ((DocsPaVO.documento.ProtocolloUscita)sch.protocollo).destinatari;
                    ((DocsPaVO.documento.ProtocolloUscita)sd.protocollo).destinatariConoscenza = ((DocsPaVO.documento.ProtocolloUscita)sch.protocollo).destinatariConoscenza;
                    //if (eUffRef)
                    //{
                    //    ((DocsPaVO.documento.ProtocolloUscita)sd.protocollo).ufficioReferente = ((DocsPaVO.documento.ProtocolloUscita)sch.protocollo).ufficioReferente;
                    //}
                    break;
                case "I":
                    sd.protocollo = new DocsPaVO.documento.ProtocolloInterno();
                    ((DocsPaVO.documento.ProtocolloInterno)sd.protocollo).mittente = ((DocsPaVO.documento.ProtocolloInterno)sch.protocollo).mittente;
                    ((DocsPaVO.documento.ProtocolloInterno)sd.protocollo).destinatari = ((DocsPaVO.documento.ProtocolloInterno)sch.protocollo).destinatari;
                    ((DocsPaVO.documento.ProtocolloInterno)sd.protocollo).destinatariConoscenza = ((DocsPaVO.documento.ProtocolloInterno)sch.protocollo).destinatariConoscenza;
                    //if (eUffRef)
                    //{
                    //    ((DocsPaVO.documento.ProtocolloInterno)sd.protocollo).ufficioReferente = ((DocsPaVO.documento.ProtocolloInterno)sch.protocollo).ufficioReferente;
                    //}
                    break;
            }
            sd = BusinessLogic.Documenti.DocSave.save(infoUtente, sd, false, out daAggiornareUffRef, ruolo);
            logger.Debug("Creato documento grigio e predisposto alla protocollazione");
           
            //Copia del documento principale e degli allegati del vecchio documento
            //nel nuovo documento
            #region copia documento principale e allegati
            DocsPaVO.documento.FileDocumento fdNew = null;
            if (sch.documenti != null && sch.documenti[0] != null &&
                Int32.Parse(((DocsPaVO.documento.FileRequest)sch.documenti[0]).fileSize) > 0)
            {
                try
                {
                    fd = BusinessLogic.Documenti.FileManager.getFile((DocsPaVO.documento.FileRequest)sch.documenti[0], infoUtente);
                    if (fd == null)
                        throw new Exception("Errore nel reperimento del file principale.");
                    //copio in un nuovo filerequest perchè putfile lo vuole senza
                    fdNew = new DocsPaVO.documento.FileDocumento();
                    fdNew.content = fd.content;
                    fdNew.length = fd.length;
                    fdNew.name = fd.name;
                    fdNew.fullName = fd.fullName;
                    fdNew.contentType = fd.contentType;
                    DocsPaVO.documento.FileRequest fr = (DocsPaVO.documento.FileRequest)sd.documenti[0];
                    if (!BusinessLogic.Documenti.FileManager.putFile(ref fr, fdNew, infoUtente, out err))
                        throw new Exception(err);
                    logger.Debug("file principale inserito");
                }
                catch (Exception ex)
                {
                    err = "Errore nel reperimento del file principale : " + ex.Message;
                    if (sd != null && sd.systemId != null && sd.systemId != "")
                    {
                        //se il putFile va in errore devo rimuovere il profile (predisposto appena inserito)
                        BusinessLogic.Documenti.DocManager.ExecRimuoviSchedaMethod(infoUtente, sd);
                        logger.Debug("Eseguita rimozione profilo");
                    }
                    logger.Debug(err);
                    throw ex;
                }
            }
            //ricerca degli allegati
            logger.Debug("Inserimento degli allegati");
            for (int i = 0; sch.allegati != null && i < sch.allegati.Count; i++)
            {
                //estrazione dati dell'allegato
                DocsPaVO.documento.Allegato documentoAllegato = (DocsPaVO.documento.Allegato)sch.allegati[i];
                filepath = documentoAllegato.docServerLoc + documentoAllegato.path;
                string nomeAllegato = documentoAllegato.fileName;
                string numPagine = documentoAllegato.numeroPagine.ToString();
                string titoloDoc = documentoAllegato.descrizione;
                logger.Debug("Inserimento allegato " + nomeAllegato);
                DocsPaVO.documento.Allegato all = new DocsPaVO.documento.Allegato();
                all.descrizione = "allegato " + i;
                logger.Debug("docnumber=" + sd.docNumber);
                all.docNumber = sd.docNumber;
                all.fileName = getFileName(nomeAllegato);
                all.version = "0";
                //numero pagine
                if (numPagine != null && !numPagine.Trim().Equals(""))
                {
                    all.numeroPagine = Int32.Parse(numPagine);
                }
                //descrizione allegato
                if (titoloDoc != null && !titoloDoc.Trim().Equals(""))
                {
                    all.descrizione = titoloDoc;
                }
                BusinessLogic.Documenti.AllegatiManager.aggiungiAllegato(infoUtente, all);
                logger.Debug("Allegato id=" + all.versionId);
                logger.Debug("Allegato version label=" + all.versionLabel);
                logger.Debug("Inserimento nel filesystem");
                DocsPaVO.documento.FileDocumento fdAllNew = new DocsPaVO.documento.FileDocumento();
                DocsPaVO.documento.FileDocumento fdAll = null;
                if (Int32.Parse(documentoAllegato.fileSize) > 0)
                {
                    try
                    {
                        fdAll = BusinessLogic.Documenti.FileManager.getFile((DocsPaVO.documento.FileRequest)sch.allegati[i], infoUtente);
                        if (fdAll == null)
                            throw new Exception("Errore nel reperimento dell'allegato numero" + i.ToString());
                        fdAllNew.content = fdAll.content;
                        fdAllNew.length = fdAll.length;
                        fdAllNew.name = fdAll.name;
                        fdAllNew.fullName = fdAll.fullName;
                        fdAllNew.contentType = fdAll.contentType;
                        DocsPaVO.documento.FileRequest fr = (DocsPaVO.documento.FileRequest)all;
                        if (!BusinessLogic.Documenti.FileManager.putFile(ref fr, fdAllNew, infoUtente, out err))
                            throw new Exception(err);
                        logger.Debug("Allegato " + i + " inserito");
                    }
                    catch (Exception ex)
                    {
                        err = "Errore nel reperimento dell'allegato numero " + i.ToString() + " : " + ex.Message;
                        if (sd != null && sd.systemId != null && sd.systemId != "")
                        {
                            //se il putFile va in errore devo rimuovere il profile (predisposto appena inserito)
                            BusinessLogic.Documenti.DocManager.ExecRimuoviSchedaMethod(infoUtente, sd);

                            logger.Debug("Eseguita rimozione profilo");
                        }
                        logger.Debug(err);//se non si riesce a prener eil file per problemi vari continuo comunque per arrivare a eseguiTrasmissione
                        throw ex;
                    }
                }
            }
            #endregion

            err = err + " " + sd.docNumber;
            if (sd != null && sd.docNumber != null)
            {
                err = "errore " + err + " documento  rimosso: " + sd.docNumber;
            }
            else
                err = "errore " + err;
            logger.Debug(err);
            sd.predisponiProtocollazione = true;
            return sd;
        }


        private static string getFileName(string fileName)
        {
            logger.Debug("getFileName");
            string res = null;
            if (fileName.Substring(fileName.LastIndexOf(".") + 1).ToUpper().Equals("P7M"))
            {
                res = fileName.Substring(0, fileName.LastIndexOf("."));
            }
            else
            {
                res = fileName;
            }
            return res;
        }

        /// <summary>
        /// </summary>
        /// <param name="schedaDoc"></param>
        /// <param name="objSicurezza"></param>
        /// <param name="debug"></param>
        /// <returns></returns>
        public static DocsPaVO.documento.SchedaDocumento save(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.documento.SchedaDocumento schedaDoc, bool enableUffRef, out bool daAggiornareUffRef, DocsPaVO.utente.Ruolo ruolo)
        {
            logger.Info("BEGIN");

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
            logger.Info("END");
            return savedDocument;
        }
        /// <summary>
        /// add massimo digregorio gestione protocollatore
        /// </summary>
        /// <param name="schedaDoc"></param>
        /// <param name="objRuolo"></param>
        /// <param name="objUtente"></param>
        /// <returns></returns>
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

        /// <summary>
        /// </summary>
        /// <param name="schedaDoc"></param>
        /// <param name="objSicurezza"></param>
        /// <param name="objRuolo"></param>
        /// <param name="debug"></param>
        /// <returns></returns>
        public static DocsPaVO.documento.SchedaDocumento addDocGrigia(DocsPaVO.documento.SchedaDocumento schedaDoc, DocsPaVO.utente.InfoUtente objSicurezza, DocsPaVO.utente.Ruolo objRuolo)
        {
            logger.Info("BEGIN");
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
                                delegate(DocsPaVO.documento.FileRequest x, DocsPaVO.documento.FileRequest y)
                                {
                                    int versionX, versionY;
                                    Int32.TryParse(x.version, out versionX);
                                    Int32.TryParse(y.version, out versionY);

                                    return (versionX.CompareTo(versionY));
                                }
                            );
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
                                    savedVersion = SessionRepositorySyncronizer.CopyToRepository(fileManager, (DocsPaVO.documento.FileRequest) schedaDoc.documenti[0]);

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

                                DocsPaVO.documento.FileRequest[] allegati = (DocsPaVO.documento.FileRequest[]) schedaDoc.allegati.ToArray(typeof(DocsPaVO.documento.Allegato));

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
                logger.Info("END");
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

        // public static DocsPaVO.documento.SchedaDocumento predisponiAllaProtocollazione(DocsPaVO.utente.InfoUtente infoUtente,  //string idAmministrazione, string idPeople, string idCorrGlobali, DocsPaVO.documento.SchedaDocumento schedaDoc, string sede) 
        /// <summary>
        /// </summary>
        /// <param name="schedaDoc"></param>
        /// <param name="objSicurezza"></param>
        /// <param name="debug"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="schedaDoc"></param>
        /// <param name="enableUffRef"></param>
        /// <param name="saveUffRef"></param>
        /// <returns></returns>
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

        /// <summary>
        /// </summary>
        /// <param name="db"></param>
        /// <param name="idProfile"></param>
        /// <param name="columnName"></param>
        /// <param name="debug"></param>
        private static void updateModProfile(/*DocsPaWS.Utils.Database db,*/ string idProfile, string columnName)
        {
            #region Codice Commentato
            /*string updateString =
				"UPDATE PROFILE SET " + columnName + " = '1' " + 
				" WHERE SYSTEM_ID=" + idProfile;
			logger.Debug(queryString);
			db.executeLocked(queryString);*/
            #endregion

            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
            doc.UpdateModProfile(columnName, idProfile);
        }

        /// <summary>
        /// </summary>
        /// <param name="db"></param>
        /// <param name="idProfile"></param>
        /// <param name="tipoCorr"></param>
        /// <param name="infoUtente"></param>
        /// <param name="debug"></param>
        private static void deleteDocArrivoPar(string idPeople, string idCorrGlobali, string idProfile, string tipoCorr)
        {
            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
            doc.DeleteDocArrivoPar(idPeople, idCorrGlobali, idProfile, tipoCorr);

            #region Codice Commentato
            /*string deleteString;
			// Cancellazione da DPA_STATO_INVIO 
			deleteString =
				"DELETE FROM DPA_STATO_INVIO WHERE ID_DOC_ARRIVO_PAR IN " +
				"(SELECT SYSTEM_ID FROM DPA_DOC_ARRIVO_PAR WHERE CHA_TIPO_MITT_DEST='" + 
				tipoCorr + "' AND ID_PROFILE =" + idProfile + ")";
			logger.Debug(deleteString);
			db.executeLocked(deleteString);

			//string now = DocsPaWS.Utils.DateControl.getDate(true);
			//string data = DocsPaWS.Utils.dbControl.toDate(now, true);
			//aggiornamento tabella storico
			string insertString = 
				"INSERT INTO DPA_CORR_STO " +
				"(" + DocsPaWS.Utils.dbControl.getSystemIdColName() + 
				" ID_PROFILE, ID_MITT_DEST, CHA_TIPO_MITT_DEST, DTA_MODIFICA, ID_PEOPLE, ID_RUOLO_IN_UO) " +
				"SELECT " + DocsPaWS.Utils.dbControl.getSystemIdNextVal("DPA_CORR_STO") + 
				"ID_PROFILE, ID_MITT_DEST, CHA_TIPO_MITT_DEST, " + Utils.dbControl.getDate() + ", " + infoUtente.idPeople + "," + infoUtente.idCorrGlobali + 
				" FROM DPA_DOC_ARRIVO_PAR WHERE CHA_TIPO_MITT_DEST='" + 
				tipoCorr + "' AND ID_PROFILE =" + idProfile;
			logger.Debug(insertString);
			db.executeNonQuery(insertString);
		

			deleteString =
				"DELETE FROM DPA_DOC_ARRIVO_PAR WHERE CHA_TIPO_MITT_DEST='" + 
				tipoCorr + "' AND ID_PROFILE =" + idProfile;
			logger.Debug(deleteString);
			db.executeLocked(deleteString);*/
            #endregion
        }

        /// <summary>
        /// </summary>
        /// <param name="db"></param>
        /// <param name="idProfile"></param>
        /// <param name="debug"></param>
        private static void deleteParoleChiave(string idProfile)
        {
            #region Codice Commentato
            /*string deleteString =
				"DELETE FROM DPA_PROF_PAROLE WHERE ID_PROFILE =" + idProfile;
			logger.Debug(deleteString);
			db.executeLocked(deleteString);*/
            #endregion

            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
            doc.DeleteParoleChiave(idProfile);
        }

        //aggiunto per la gestione dell'aggiornamento dei destinatari
        /// <summary>
        /// </summary>
        /// <param name="db"></param>
        /// <param name="corrispondente"></param>
        /// <param name="idDocArrivoPar"></param>
        /// <param name="idRegistro"></param>
        /// <param name="idProfile"></param>
        /// <param name="debug"></param>
        private static void updateStatoInvio(/*DocsPaWS.Utils.Database db, */DocsPaVO.utente.Corrispondente corrispondente, string idDocArrivoPar, string idRegistro, string idProfile, string mail)
        {
            ArrayList listaIdCorrInStatoInvio = ProtocolloUscitaManager.getIdCorrInStatoInvio(idProfile);

            if (inListaCorr(listaIdCorrInStatoInvio, corrispondente.systemId))
            {
                ProtocolloUscitaManager.updateTipoSpedizione(corrispondente, idProfile);
            }
            else
            {
                ProtocolloUscitaManager.addStatoInvio(corrispondente, idDocArrivoPar, idRegistro, idProfile, mail);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="lista"></param>
        /// <param name="idCorr"></param>
        /// <returns></returns>
        private static bool inListaCorr(ArrayList lista, string idCorr)
        {
            bool trovato = false;

            if (lista != null)
            {
                for (int i = 0; i < lista.Count && !trovato; i++)
                {
                    if (lista[i].ToString().Equals(idCorr))
                    {
                        trovato = true;
                    }
                }
            }

            return trovato;
        }

    }
}
