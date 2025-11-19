// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaDB.Query_DocsPAWS;
using DocsPaVO.utente;
using Serilog;
using System.Data;

namespace BusinessLogic.Interoperabilita;

public partial class InteroperabilitaSegnatura
{
    private static ILogger logger = Serilog.Log.ForContext(typeof(InteroperabilitaSegnatura));

    /// <summary>
    /// Codice della chiave per l'abilitazione/disabilitazione della funzionalità
    /// </summary>
    public const String INTEROP_INTERNA_TRASMISSIONE_SELETTIVA = "INTEROP_INT_TRASM_SELETTIVA";

    /// <summary>
    /// Metodo utilizzato per verificare se, per l'amministrazione corrente è attiva la funzionalità di 
    /// trasmissione selettiva
    /// </summary>
    /// <param name="adminId">Id dell'amministrazione</param>
    /// <returns>Flag che indica lo stato di attivazione della funzionalità</returns>
    public static bool IsEnabledSelectiveTransmission(String adminId)
    {
        string enabled = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(adminId, INTEROP_INTERNA_TRASMISSIONE_SELETTIVA);
        if (String.IsNullOrEmpty(enabled))
            enabled = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", INTEROP_INTERNA_TRASMISSIONE_SELETTIVA);

        return enabled == "1";

    }

    /// /// <param name="recipient">Destinatario della spedizione. Viene utilizzato se è attivo la funzionalità di trasmissione selettiva, per determinare i corrispondente cui effettuare la trasmissione</param>
    /// <returns></returns>
    public static bool eseguiSegnaturaNoMail(DocsPaVO.utente.InfoUtente infoUtenteFisico, string serverName, DocsPaVO.utente.Registro reg, DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.utente.Ruolo ruolo, DocsPaVO.documento.SchedaDocumento sch, out string err, out DocsPaVO.Interoperabilita.DatiInteropAutomatica dia, string mailAddress, Corrispondente recipient)
    //public static bool eseguiSegnaturaNoMail(DocsPaVO.utente.InfoUtente infoUtenteFisico, string serverName, DocsPaVO.utente.Registro reg, DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.utente.Ruolo ruolo, DocsPaVO.documento.SchedaDocumento sch, out string err, out DocsPaVO.Interoperabilita.DatiInteropAutomatica dia, string mailAddress)
    {
        err = string.Empty;
        System.IO.FileStream fs = null;
        System.IO.FileStream fsAll = null;
        DocsPaVO.documento.SchedaDocumento sd = null;
        bool daAggiornareUffRef = false;
        string filepath = "";
        string docPrincipaleName = "";
        dia = null;
        DocsPaVO.documento.FileDocumento fd = null;
        try
        {
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

            sd.idPeople = infoUtente.idPeople;
            sd.userId = infoUtente.userId;
            //sd.note=infoDestinatari; 

            string statoReg = BusinessLogic.Utenti.RegistriManager.getStatoRegistro(reg);

            sd.oggetto = sch.oggetto;
            if (reg.autoInterop != null && reg.autoInterop.Equals("2") && statoReg == "V")
            {
                //registro AUTOMATICO: creo direttamente un protocollo sulla AOO destinataria
                //solo se il registro di destinazione è in verde, altrim creo il predisposto
                sd.predisponiProtocollazione = false;

            }
            else
            {
                //registro SEMIAUTOMATICO O MANUALE: comportamento rimane inalterato
                sd.predisponiProtocollazione = true;
            }


            sd.registro = reg;
            sd.tipoProto = "A";
            sd.typeId = "INTEROPERABILITA";
            sd.interop = "I";
            sd.descMezzoSpedizione = "INTEROPERABILITA";
            sd.mezzoSpedizione = BusinessLogic.Documenti.InfoDocManager.getIdMezzoSpedizioneByDesc("INTEROPERABILITA");
            //aggiunta protocollo entrata

            DocsPaVO.documento.ProtocolloEntrata protEntr = new DocsPaVO.documento.ProtocolloEntrata();
            if (((DocsPaVO.documento.ProtocolloUscita)(sch.protocollo)).mittente != null)
            {
                DocsPaVO.utente.Corrispondente corr = ((DocsPaVO.documento.ProtocolloUscita)(sch.protocollo)).mittente;
                protEntr.mittente = corr;
            }
            else
            {
                DocsPaVO.utente.Ruolo corr = BusinessLogic.Utenti.UserManager.getRuolo(sch.protocollatore.ruolo_idCorrGlobali);
                protEntr.mittente = corr;
            }
            protEntr.dataProtocolloMittente = sch.protocollo.dataProtocollazione;
            // reg.codRegistro + ...
            protEntr.invioConferma = "1";
            protEntr.descrizioneProtocolloMittente = sch.registro.codRegistro + DocsPaDB.Utils.Personalization.getInstance(reg.idAmministrazione).getSepSegnatura() + sch.protocollo.numero;  //METTERE CARATTERE DELL'AMMINISTRAZIONE


            sd.protocollo = protEntr;

            //dati utente/ruolo/Uo del creatore.
            sd.protocollatore = new DocsPaVO.documento.Protocollatore(infoUtente, ruolo);

            try
            {
                sd = BusinessLogic.Documenti.DocSave.addDocGrigia(sd, infoUtente, ruolo);
                BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCUMENTOADDDOCGRIGIA", sd.systemId, string.Format("{0} {1}", "N.ro Doc.: ", sd.docNumber), DocsPaVO.Logger.CodAzione.Esito.OK);
            }
            catch (Exception excp)
            {
                logger.Error("Errore nella creazione del documento . " + excp.Message);
                BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCUMENTOADDDOCGRIGIA", "", string.Format("{0} {1}", "N.ro Doc.: ", ""), DocsPaVO.Logger.CodAzione.Esito.KO);
            }

            logger.Debug("Salvataggio doc...");


            if (DocsPaUtils.Configuration.CustomConfigurationBaseManager.isEnableRiferimentiMittente())
                sd.riferimentoMittente = sch.riferimentoMittente;

            if (!string.IsNullOrEmpty(DocsPaUtils.Configuration.CustomConfigurationBaseManager.isEnableProtocolloTitolario()))
                sd.riferimentoMittente = sch.protocolloTitolario;

            sd = BusinessLogic.Documenti.DocSave.save(infoUtente, sd, false, out daAggiornareUffRef, ruolo);
            if (sd.tipoProto != "G")
                BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCUMENTOSAVEDOCUMENTO", sd.systemId, string.Format("{0} {1}", "Aggiornamento Protocollo Numero ", sd.protocollo.segnatura), DocsPaVO.Logger.CodAzione.Esito.OK);
            else
                BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCUMENTOSAVEDOCUMENTO", sd.systemId, string.Format("{0} {1}", "Aggiornamento Documento Numero ", sd.docNumber), DocsPaVO.Logger.CodAzione.Esito.OK);

            logger.Debug("Salvataggio eseguito");
            //}
            DocsPaVO.documento.FileDocumento fdNew = null;
            if (sch.documenti != null && sch.documenti[0] != null &&
                Int32.Parse(((DocsPaVO.documento.FileRequest)sch.documenti[0]).fileSize) > 0)
            {
                try
                {
                    fd = BusinessLogic.Documenti.FileManager.getFileFirmato((DocsPaVO.documento.FileRequest)sch.documenti[0], infoUtente, false);
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
                    else
                        BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCUMENTOPUTFILE", fr.docNumber, string.Format("{0} {1}", "Acquisito documento N.ro:", fr.docNumber), DocsPaVO.Logger.CodAzione.Esito.OK);

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
                    logger.Error(err);
                    throw ex;
                    //se non si riesce a prener eil file per problemi vari continuo comunque per arrivare a eseguiTrasmissione
                }
            }

            //ricerca degli allegati
            //				logger.addMessage("Inserimento degli allegati");
            logger.Debug("Inserimento degli allegati");
            DocsPaDB.Query_DocsPAWS.Documenti docWs = new DocsPaDB.Query_DocsPAWS.Documenti();
            for (int i = 0; sch.allegati != null && i < sch.allegati.Count; i++)
            {
                //estrazione dati dell'allegato
                DocsPaVO.documento.Allegato documentoAllegato = (DocsPaVO.documento.Allegato)sch.allegati[i];

                //Salto il tipo allegato Derivato essendo lui estratto dal docprinc o da un altro allegato
                if (docWs.GetTipologiaAllegato(documentoAllegato.versionId) == "D")
                    continue;

                filepath = documentoAllegato.docServerLoc + documentoAllegato.path;
                string nomeAllegato = documentoAllegato.fileName;
                string numPagine = documentoAllegato.numeroPagine.ToString();
                string titoloDoc = documentoAllegato.descrizione;
                //					logger.addMessage("Inserimento allegato "+nomeAllegato);
                logger.Debug("Inserimento allegato " + nomeAllegato);

                DocsPaVO.documento.Allegato all = new DocsPaVO.documento.Allegato();
                //					logger.addMessage("docnumber="+sd.docNumber);
                logger.Debug("docnumber=" + sd.docNumber);

                all.docNumber = sd.docNumber;
                //all.applicazione=getApp(nomeAllegato,logger);
                all.fileName = getFileName(nomeAllegato);
                all.version = "0";

                all.descrizione = "allegato " + i;
                if (!String.IsNullOrEmpty(all.fileName))
                    all.descrizione = all.fileName;

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

                DocsPaVO.documento.Allegato res = null;
                try
                {
                    res = BusinessLogic.Documenti.AllegatiManager.aggiungiAllegato(infoUtente, all);
                    if (res != null)
                    {
                        BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCUMENTOAGGIUNGIALLEGATO", all.docNumber, string.Format("{0}{1}{2}{3}", "Aggiunto al N.ro Doc.: ", all.docNumber, " il N.ro Allegato: ", res.versionLabel), DocsPaVO.Logger.CodAzione.Esito.OK);
                        BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCNEWALLEGATO", all.docNumber, string.Format("{0}{1}{2}{3}", "Aggiunto al N.ro Doc.: ", all.docNumber, " il N.ro Allegato: ", res.versionLabel), DocsPaVO.Logger.CodAzione.Esito.OK);
                    }
                }
                catch (Exception e)
                {
                    err = "errore nel metodo aggiungiAllegato per l'allegato n. " + Convert.ToString(i + 1);
                    BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCUMENTOAGGIUNGIALLEGATO", all.docNumber, string.Format("{0}{1}", "Errore in inserimento allegato al N.ro Doc.: ", all.docNumber), DocsPaVO.Logger.CodAzione.Esito.KO);
                    BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCNEWALLEGATO", all.docNumber, string.Format("{0}{1}", "Errore in inserimento allegato al N.ro Doc.: ", all.docNumber), DocsPaVO.Logger.CodAzione.Esito.KO);
                    logger.Error(err);
                    throw e;
                }


                #region Codice Commentato
                //					logger.addMessage("Allegato id="+all.versionId);
                //					logger.addMessage("Allegato version label="+all.versionLabel);
                //					logger.addMessage("Inserimento nel filesystem");
                #endregion

                logger.Debug("Allegato id=" + all.versionId);
                logger.Debug("Allegato version label=" + all.versionLabel);
                logger.Debug("Inserimento nel filesystem");

                DocsPaVO.documento.FileDocumento fdAllNew = new DocsPaVO.documento.FileDocumento();
                DocsPaVO.documento.FileDocumento fdAll = null;
                if (Int32.Parse(documentoAllegato.fileSize) > 0)
                {
                    try
                    {
                        fdAll = BusinessLogic.Documenti.FileManager.getFileFirmato((DocsPaVO.documento.FileRequest)sch.allegati[i], infoUtente, false);
                        if (fdAll == null)
                            throw new Exception("Errore nel reperimento dell'allegato numero" + i.ToString());
                        fdAllNew.content = fdAll.content;
                        fdAllNew.length = fdAll.length;
                        fdAllNew.name = fdAll.name;
                        fdAllNew.fullName = fdAll.fullName;
                        if (!String.IsNullOrEmpty(fdAll.nomeOriginale))
                            fdAllNew.nomeOriginale = fdAll.nomeOriginale;
                        fdAllNew.contentType = fdAll.contentType;
                        DocsPaVO.documento.FileRequest fr = (DocsPaVO.documento.FileRequest)all;
                        if (!BusinessLogic.Documenti.FileManager.putFile(ref fr, fdAllNew, infoUtente, out err))
                            throw new Exception(err);
                        else
                            BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCUMENTOPUTFILE", fr.docNumber, string.Format("{0} {1}", "Acquisito documento N.ro:", fr.docNumber), DocsPaVO.Logger.CodAzione.Esito.OK);

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
                        logger.Error(err);//se non si riesce a prener eil file per problemi vari continuo comunque per arrivare a eseguiTrasmissione
                        throw ex;
                    }
                }
            }

            //***********************************************************************************************************//
            /*  MODIFICA Giordano Iacozzilli Data: 27/04/2012                                                            */
            //***********************************************************************************************************// 
            //Modifca relativa all'invio in allegato diun File XML in caso di Interop Semplificata //
            Interoperabilita.InteroperabilitaSendXmlInAttach _intXsendXML = new Interoperabilita.InteroperabilitaSendXmlInAttach();
            bool _esito = _intXsendXML.ManageAttachXML(sch, sd, infoUtenteFisico.idGruppo, infoUtente);
            // la bool esito al momento non la uso, devo capire se metterla o meno nel log applicativo.
            //***********************************************************************************************************//
            /*  FINE: MODIFICA Giordano Iacozzilli Data: 27/04/2012                                                      */
            //***********************************************************************************************************// 


            //nuova gestione interoperabilità AUTOMATICA

            if ((reg.autoInterop != null && !reg.autoInterop.Equals("2"))
                || (reg.autoInterop != null && reg.autoInterop.Equals("2") && !statoReg.Equals("V")))
            {
                //TRASMISSIONE PER INTEROPERABILITA
                logger.Debug("Esegui trasmissione...");
                try
                {
                    //eseguiTrasmissione(infoUtente.idPeople,serverName, sd,infoDestinatari,reg,ruolo);
                    // correzione furnari eseguiTrasmissione(infoUtente.idPeople, serverName, sd, "INTEROPERABILITA", reg, ruolo, infoUtente.dst, mailAddress, infoUtente);

                    // S. Furnari - 16/01/2013 - Sviluppo trasmissione documento solo a ruoli nella UO destinataria della spedizione e non a tutta la AOO
                    //eseguiTrasmissione(infoUtente.idPeople, serverName, sd, "INTEROPERABILITA", reg, ruolo, infoUtente.dst, mailAddress);
                    eseguiTrasmissione(infoUtente.idPeople, serverName, sd, "INTEROPERABILITA", reg, ruolo, infoUtente.dst, mailAddress, infoUtente, recipient);


                    if (!checkExecTrasm(sd.systemId, "INTEROPERABILITA"))
                    {
                        //codint1 = true;
                        //err = "CODINTEROP1 Trasmissione per INTEROPERABILITA del documento con id:" + sd.docNumber + " non effettuata. Il documento può essere trovato tra i predisposti.";
                        //throw new Exception("(CODINTEROP1) Trasmissione per INTEROPERABILITA del documento con id:" + sd.docNumber + "non effettuata!");
                        // S. Furnari - 09/01/2013 - Quando viene catturata l'eccezione scatenata di seguito, viene cancellata la scheda
                        // documento, quindi non la si può trovare con una ricerca documenti predisposti.
                        //err = "CODINTEROP1 Trasmissione per INTEROPERABILITA del documento con id:" + sd.docNumber + " non effettuata. Il documento può essere trovato tra i predisposti.";
                        err = "CODINTEROP1 Trasmissione del documento non riuscita.";
                        throw new Exception("(CODINTEROP1) Trasmissione per INTEROPERABILITA del documento con id:" + sd.docNumber + "non effettuata!");

                    }
                }
                catch (Exception ex)
                {
                    if (sd != null && sd.systemId != null && sd.systemId != "")
                    {
                        BusinessLogic.Documenti.DocManager.ExecRimuoviSchedaMethod(infoUtente, sd);
                        logger.Debug("Eseguita rimozione profilo");
                    }
                    logger.Error(err);
                    throw ex;
                }
            }
            //luluciani: viene fatto alla fine, per  poter gestire il rollback manuale.

            if ((reg.autoInterop != null && reg.autoInterop.Equals("2")) &&
                (statoReg == "V"))
            {
                //registro AUTOMATICO: creo direttamente un protocollo sulla AOO destinataria
                logger.Debug("Protocollazione documento in caso di registro AUTOMATICO");
                //dafault OK
                DocsPaVO.documento.ResultProtocollazione resultProtocollazione = DocsPaVO.documento.ResultProtocollazione.OK;
                //

                sd = BusinessLogic.Documenti.ProtoManager.protocolla(sd, ruolo, infoUtente, out resultProtocollazione, null, "", "");
                string VarDescOggetto = string.Empty;
                if (sd != null)
                {
                    if (sd.protocollo != null)
                        VarDescOggetto = string.Format("{0}{1} / {2}{3}", "N.ro Doc.: ", sd.docNumber, "Segnatura: ", sd.protocollo.segnatura);
                    else
                        VarDescOggetto = string.Format("{0}{1}", "N.ro Doc.: ", sd.docNumber);

                    BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCUMENTOPROTOCOLLA", sd.systemId, VarDescOggetto, DocsPaVO.Logger.CodAzione.Esito.OK);
                }

                // Popolamento oggetto DIA (Dati Interop Automatica)
                dia = new DocsPaVO.Interoperabilita.DatiInteropAutomatica();
                //esito protocollazione su registro automatico
                dia.esitoProtocollazione = resultProtocollazione;
                //ruolo e infoUtente gestore del registro
                dia.ruolo = ruolo;
                dia.infoUtente = infoUtente;

                //systemId protocollo in arrivo appena creato su registro automatico
                if (sd != null && sd.systemId != null)
                    dia.schedaDoc = sd;

                //registro automatico
                dia.registro = reg;
            }

            return true;
        }
        catch (Exception ex)
        {
            if (sd != null && sd.docNumber != null)
            {
                //err="errore "+err+" Docnumber predisposto: "+sd.docNumber+" "+ex.Message.ToString();
                err = "errore " + err + "  " + ex.Message.ToString();
            }
            else
                err = "errore " + err + " " + ex.Message.ToString();
            logger.Error(err);

            return false;
        }
    }

    public static string getFileName(string fileName)
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

    private static void eseguiTrasmissione(string idPeople, string serverName, DocsPaVO.documento.SchedaDocumento sd, string noteGenerali, DocsPaVO.utente.Registro reg, DocsPaVO.utente.Ruolo ruolo, string dst, string mailAddress, InfoUtente infoUtente = null, Corrispondente recipient = null)
    {
        //DocsPa_V15_Utils.DBAgent db=new DocsPa_V15_Utils.DBAgent();
        try
        {
            // Per gestione pendenti tramite PEC
            bool MailPendente = InteroperabilitaUtils.MantieniMailRicevutePendenti(reg.systemId, mailAddress);

            DocsPaVO.trasmissione.Trasmissione trasm = new DocsPaVO.trasmissione.Trasmissione();
            trasm.ruolo = ruolo;
            //db.openConnection();
            trasm.utente = BusinessLogic.Utenti.UserManager.getUtente(idPeople);
            trasm.utente.dst = dst;//aggiunto dst il 27/10/2005 per errore in HM
            //db.closeConnection();
            trasm.noteGenerali = noteGenerali;
            DocsPaVO.documento.InfoDocumento infoDoc = new DocsPaVO.documento.InfoDocumento();
            infoDoc.idProfile = sd.systemId;
            infoDoc.docNumber = sd.docNumber;
            infoDoc.oggetto = sd.oggetto.descrizione;
            infoDoc.tipoProto = "A";
            infoDoc.idRegistro = reg.systemId;
            trasm.infoDocumento = infoDoc;
            //costruzione singole trasmissioni
            DocsPaVO.trasmissione.RagioneTrasmissione ragione = getRagioneTrasm(ruolo.idAmministrazione, "I");
            //				logger.addMessage("RAGIONE :"+ragione.tipo+" "+ragione.tipoDestinatario);
            logger.Debug("RAGIONE :" + ragione.tipo + " " + ragione.tipoDestinatario);

            // S. Furnari - 16/01/2013 - Sviluppo trasmissione documento ricevuto per interop interno solo a ruoli nella UO
            // destinataria della spedizione e non a tutta la AOO.
            //System.Collections.ArrayList ruoliDest = getRuoliDestTrasm(reg, mailAddress);
            System.Collections.ArrayList ruoliDest = null;
            // Se il destinatario è interno, è attiva l'interoperabilità interna, ed è attiva la funzionalità
            // di tramimssione solo ai ruoli nella UO destinataria della spedizione, devono essere ricercati
            // solo i ruoli in "recipient" che, nel caso in cui il destinatario originale sia un ruolo, sarà valorizzata
            // con la UO in cui è definito il ruolo.
            if (recipient != null && InteroperabilitaSegnatura.IsEnabledSelectiveTransmission(recipient.idAmministrazione))
                ruoliDest = new System.Collections.ArrayList(GetRecipients(reg, recipient.systemId));
            else
                ruoliDest = getRuoliDestTrasm(reg, mailAddress);

            //commentato furnari System.Collections.ArrayList ruoliDest = getRuoliDestTrasm(reg, mailAddress);

            //if (MailPendente)
            //{
            //    bool found = false;
            //    foreach (DocsPaVO.utente.Ruolo r in ruoliDest)
            //    {                        
            //        if (r.systemId == ruolo.systemId)
            //        {
            //            found = true;
            //        }

            //    }
            //    ruoliDest.Clear();
            //    if (found) ruoliDest.Add(ruolo);
            //}
            System.Collections.ArrayList trasmissioniSing = new System.Collections.ArrayList();
            if (ruoliDest.Count > 0)
            {
                for (int i = 0; i < ruoliDest.Count; i++)
                {
                    //						logger.addMessage("Aggiunta trasmissione singola");
                    logger.Debug("Aggiunta trasmissione singola");

                    DocsPaVO.trasmissione.TrasmissioneSingola trSing = new DocsPaVO.trasmissione.TrasmissioneSingola();
                    trSing.ragione = ragione;
                    trSing.corrispondenteInterno = (DocsPaVO.utente.Ruolo)ruoliDest[i];
                    // S. Furnari - 18/01/2013 - Se recipient è valorizzato e quindi la spedizione è per interop interna, 
                    // il tipo di trasmissione è determinato dal valore della chiave TRASM_UNO_TUTTI_INTEROP
                    //trSing.tipoTrasm =  "S";
                    trSing.tipoTrasm = recipient != null ? InteroperabilitaSegnatura.GetInteropTrasmType(recipient.idAmministrazione) : "S";

                    //furnari trSing.tipoTrasm = "S";
                    trSing.tipoDest = DocsPaVO.trasmissione.TipoDestinatario.RUOLO;
                    if (MailPendente)
                    {
                        trSing.ragione.eredita = "0";
                    }

                    //ricerca degli utenti del ruolo
                    System.Collections.ArrayList utenti = new System.Collections.ArrayList();
                    //						logger.addMessage("Ricerca utenti del ruolo per codice e registro");
                    logger.Debug("Ricerca utenti del ruolo per codice e registro");

                    DocsPaVO.addressbook.QueryCorrispondente qc = new DocsPaVO.addressbook.QueryCorrispondente();
                    qc.codiceRubrica = ((DocsPaVO.utente.Ruolo)ruoliDest[i]).codiceRubrica;
                    System.Collections.ArrayList registri = new System.Collections.ArrayList();
                    registri.Add(reg.systemId);
                    qc.idRegistri = registri;
                    qc.idAmministrazione = reg.idAmministrazione;
                    qc.getChildren = true;

                    //LULUCIANI 28/07/2009 
                    qc.fineValidita = true;

                    utenti = BusinessLogic.Utenti.addressBookManager.listaCorrispondentiIntMethod(qc);
                    System.Collections.ArrayList trasmissioniUt = new System.Collections.ArrayList();
                    for (int k = 0; k < utenti.Count; k++)
                    {
                        //							logger.addMessage("aggiunta trasmissione utente");
                        logger.Debug("aggiunta trasmissione utente");

                        DocsPaVO.trasmissione.TrasmissioneUtente trUt = new DocsPaVO.trasmissione.TrasmissioneUtente();
                        trUt.utente = (DocsPaVO.utente.Utente)utenti[k];
                        trasmissioniUt.Add(trUt);
                    }
                    trSing.trasmissioneUtente = trasmissioniUt;
                    trasmissioniSing.Add(trSing);
                }
                trasm.trasmissioniSingole = trasmissioniSing;
                //BusinessLogic.Trasmissioni.TrasmManager.saveTrasmMethod(trasm);
                ////					logger.addMessage("Trasmissione salvata");
                //logger.Debug(" ");

                //BusinessLogic.Trasmissioni.ExecTrasmManager.executeTrasmMethod(serverName, trasm);
                ////					logger.addMessage("Trasmissione eseguita");
                //logger.Debug("Trasmissione eseguita");
                DocsPaVO.trasmissione.Trasmissione result = null;
                string desc = string.Empty;
                string method;
                result = BusinessLogic.Trasmissioni.ExecTrasmManager.saveExecuteTrasmMethod(serverName, trasm);
                logger.Debug("Trasmissione salvata ed eseguita");

                if (result != null)
                {
                    // LOG per documento
                    foreach (DocsPaVO.trasmissione.TrasmissioneSingola single in result.trasmissioniSingole)
                    {
                        method = "TRASM_DOC_" + single.ragione.descrizione.ToUpper().Replace(" ", "_");
                        if (result.infoDocumento.segnatura == null)
                            desc = "Trasmesso Documento ID: " + result.infoDocumento.docNumber.ToString();
                        else
                            desc = "Trasmesso Documento ID: " + result.infoDocumento.segnatura.ToString();
                        BusinessLogic.UserLog.UserLog.WriteLog(result.utente.userId, result.utente.idPeople, result.ruolo.idGruppo, result.utente.idAmministrazione, method, result.infoDocumento.docNumber, desc, DocsPaVO.Logger.CodAzione.Esito.OK,
                            (infoUtente != null && infoUtente.delegato != null ? infoUtente.delegato : null), "1", single.systemId);
                    }
                }
            }
            else
            {
                //					logger.addMessage("Trasmissione non eseguita per mancanza di ruoli destinatari");
                logger.Debug("Trasmissione non eseguita per mancanza di ruoli destinatari");
            }
        }
        catch (Exception e)
        {
            //db.closeConnection();
            logger.Error("Errore nella gestione dell'interoperabilità. (eseguiTrasmissione)", e);
            throw e;
        }
    }

    public static DocsPaVO.trasmissione.RagioneTrasmissione getRagioneTrasm(string idAmm, String tipoRagione)
    {
        logger.Debug("getRagioneTrasm");
        //DocsPa_V15_Utils.Database db=DocsPa_V15_Utils.dbControl.getDatabase();
        //DataSet ds=new DataSet();
        DataSet ds;
        DocsPaVO.trasmissione.RagioneTrasmissione rt = new DocsPaVO.trasmissione.RagioneTrasmissione();
        try
        {
            #region Codice Commentato
            /*
				string queryString="SELECT * FROM DPA_RAGIONE_TRASM WHERE CHA_TIPO_RAGIONE='N' AND CHA_TIPO_DIRITTI='W'";
				db.fillTable(queryString,ds,"RAGIONE");
				*/
            #endregion

            DocsPaDB.Query_DocsPAWS.Interoperabilita obj = new DocsPaDB.Query_DocsPAWS.Interoperabilita();
            obj.getRagTrasm(out ds, idAmm, tipoRagione);

            DataRow ragione = ds.Tables["RAGIONE"].Rows[0];
            rt.descrizione = ragione["VAR_DESC_RAGIONE"].ToString();
            rt.risposta = ragione["CHA_RISPOSTA"].ToString();
            rt.systemId = ragione["SYSTEM_ID"].ToString();
            rt.tipo = "N";
            rt.tipoDestinatario = (DocsPaVO.trasmissione.TipoGerarchia)DocsPaDB.Utils.HashTableManager.GetKeyFromValue(DocsPaVO.trasmissione.RagioneTrasmissione.tipoGerarchiaStringa, ragione["CHA_TIPO_DEST"].ToString());
            rt.tipoDiritti = DocsPaVO.trasmissione.TipoDiritto.WRITE;
            rt.eredita = ragione["CHA_EREDITA"].ToString();
        }
        catch (Exception e)
        {
            logger.Debug(e.ToString());
            //db.closeConnection();
            logger.Error("Errore nella gestione dell'interoperabilità. (getRagioneTrasm)", e);
            throw e;
        }
        return rt;
    }

    private static System.Collections.ArrayList getRuoliDestTrasm(DocsPaVO.utente.Registro reg, string mailAddress)
    {
        logger.Debug("getRuoliDestTrasm");
        //DocsPa_V15_Utils.Database db=DocsPa_V15_Utils.dbControl.getDatabase();
        DataSet ds = new DataSet();
        System.Collections.ArrayList ruoliDestTrasm = new System.Collections.ArrayList();
        try
        {
            #region Codice Commentato
            //db.openConnection();
            /*
            string queryString="SELECT A.SYSTEM_ID, A.VAR_CODICE, A.VAR_COD_RUBRICA, D.VAR_DESC_RUOLO, A.ID_GRUPPO, D.NUM_LIVELLO, E.VAR_DESC_CORR FROM DPA_CORR_GLOBALI A, DPA_TIPO_F_RUOLO B,DPA_TIPO_FUNZIONE C,DPA_TIPO_RUOLO D,DPA_CORR_GLOBALI E, DPA_L_RUOLO_REG F  WHERE A.CHA_TIPO_URP='R' AND F.ID_REGISTRO="+reg.systemId+" AND B.ID_RUOLO_IN_UO=A.SYSTEM_ID AND B.ID_TIPO_FUNZ=C.SYSTEM_ID AND C.VAR_COD_TIPO='PRAU' AND D.SYSTEM_ID=A.ID_TIPO_RUOLO AND E.SYSTEM_ID=A.ID_UO AND F.ID_RUOLO_IN_UO=A.SYSTEM_ID";
            db.fillTable(queryString,ds,"RUOLI");
            */
            #endregion

            DocsPaDB.Query_DocsPAWS.Interoperabilita obj = new DocsPaDB.Query_DocsPAWS.Interoperabilita();
            obj.getCorrRuoloFun(out ds, reg, mailAddress);

            for (int i = 0; i < ds.Tables["RUOLI"].Rows.Count; i++)
            {
                DataRow ruoloRow = ds.Tables["RUOLI"].Rows[i];
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();
                ruolo.systemId = ruoloRow["SYSTEM_ID"].ToString();
                ruolo.codiceCorrispondente = ruoloRow["VAR_CODICE"].ToString();
                ruolo.codiceRubrica = ruoloRow["VAR_COD_RUBRICA"].ToString();
                ruolo.descrizione = ruoloRow["VAR_DESC_RUOLO"].ToString() + " " + ruoloRow["VAR_DESC_CORR"].ToString();
                ruolo.livello = ruoloRow["NUM_LIVELLO"].ToString();
                ruolo.idGruppo = ruoloRow["ID_GRUPPO"].ToString();
                DocsPaVO.utente.UnitaOrganizzativa uoDest = new DocsPaVO.utente.UnitaOrganizzativa();
                ruolo.uo = uoDest;
                ruolo.uo.systemId = ruoloRow["ID_UO"].ToString();
                ruolo.tipoCorrispondente = "R";
                ruoliDestTrasm.Add(ruolo);
            }
        }
        catch (Exception e)
        {
            logger.Debug(e.ToString());
            //db.closeConnection();
            logger.Error("Errore nella gestione dell'interoperabilità. (getRuoliDestTrasm)", e);
            throw e;
        }
        return ruoliDestTrasm;
    }

    /// <summary>
    /// Questo metodo restituisce la lista dei corrispondenti destinatari della trasmissione.
    /// Nel caso in cui il destinatario sia una UO, restituisce la lista dei ruoli, 
    /// definiti nella UO abilitati alla ricezione di trasmissioni.
    /// Nel caso in cui il destinatario sia un ruolo, restituisce la lista dei ruoli,
    /// definiti nella uo di appartenenza del ruolo, abilitati alla ricezione di trasmissioni.
    /// </summary>
    /// <param name="reg">AOO destinataria della spedizione</param>
    /// <param name="uoId">Identificativo della UO destinataria della spedizione</param>
    /// <returns>Lista dei ruoli cui trasmettere il documento</returns>
    private static List<Ruolo> GetRecipients(Registro reg, String uoId)
    {
        List<Ruolo> retVal = new List<Ruolo>();
        try
        {

            var obj = new DocsPaDB.Query_DocsPAWS.Interoperabilita();

            DataSet ds = new DataSet();
            obj.GetSelectiveRecipients(out ds, reg, uoId);

            for (int i = 0; i < ds.Tables["RUOLI"].Rows.Count; i++)
            {
                DataRow ruoloRow = ds.Tables["RUOLI"].Rows[i];
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();
                ruolo.systemId = ruoloRow["SYSTEM_ID"].ToString();
                ruolo.codiceCorrispondente = ruoloRow["VAR_CODICE"].ToString();
                ruolo.codiceRubrica = ruoloRow["VAR_COD_RUBRICA"].ToString();
                ruolo.descrizione = ruoloRow["VAR_DESC_RUOLO"].ToString() + " " + ruoloRow["VAR_DESC_CORR"].ToString();
                ruolo.livello = ruoloRow["NUM_LIVELLO"].ToString();
                ruolo.idGruppo = ruoloRow["ID_GRUPPO"].ToString();
                DocsPaVO.utente.UnitaOrganizzativa uoDest = new DocsPaVO.utente.UnitaOrganizzativa();
                ruolo.uo = uoDest;
                ruolo.uo.systemId = ruoloRow["ID_UO"].ToString();
                ruolo.tipoCorrispondente = "R";
                retVal.Add(ruolo);
            }
        }
        catch (Exception e)
        {
            logger.Debug(e.ToString());
            //db.closeConnection();
            logger.Debug("Errore nella gestione dell'interoperabilità. (GetRecipients)", e);
            throw e;
        }

        return retVal;
    }

    public static bool checkExecTrasm(string sysid, string tipoTrasm)
    {
        return BusinessLogic.Trasmissioni.ExecTrasmManager.checkExecTrasm(sysid, tipoTrasm);

    }
}
