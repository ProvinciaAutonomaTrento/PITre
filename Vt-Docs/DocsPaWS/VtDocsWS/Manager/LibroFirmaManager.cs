using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Web;
using System.Web.Services;
using VtDocsWS.WebServices;
using log4net;

namespace VtDocsWS.Manager
{
    public class LibroFirmaManager
    {
        private static ILog logger = LogManager.GetLogger(typeof(LibroFirmaManager));

        /// <summary>
        /// Aggiungo elemento in libro firma da motore
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Response</returns>
        public static Services.LibroFirma.AddElementoInLF.AddElementoInLFResponse AddElementoInLF(Services.LibroFirma.AddElementoInLF.AddElementoInLFRequest request)
        {
            Services.LibroFirma.AddElementoInLF.AddElementoInLFResponse response = new Services.LibroFirma.AddElementoInLF.AddElementoInLFResponse();
            try
            {
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;
                DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();


                //Carico i dati del passo successivo dall'id ottenuto in request
                DocsPaVO.LibroFirma.IstanzaPassoDiFirma passo = libroFirma.GetIstanzaPassoDiFirma(request.IdPasso);

                //INC000001133628 APSS  in LF risulta la tx fatta da un ruolo diverso da quello dove è stata fatta la firma
                //Alcune volte non viene passato il codice del ruolo che ha effettuato l'operazione, in questo caso vado ad estrarlo dal DB altrimenti
                //il sistema va a prendere un ruolo a caso tra quelli dell'utente.
                if (string.IsNullOrEmpty(request.CodeRoleLogin))
                {
                    DocsPaVO.utente.Ruolo ruolo = libroFirma.GetRuoloTitolarePasso(passo.idIstanzaProcesso, passo.numeroSequenza - 1);
                    if (!string.IsNullOrEmpty(ruolo.codiceRubrica))
                        request.CodeRoleLogin = ruolo.codiceRubrica;
                }

                //Inizio controllo autenticazione utente
                infoUtente = Utils.CheckAuthentication(request, "AddDocInProject");

                utente = BusinessLogic.Utenti.UserManager.getUtenteById(infoUtente.idPeople);
                if (utente == null)
                {
                    //Utente non trovato
                    throw new PisException("USER_NO_EXIST");
                }

                if (!string.IsNullOrEmpty(request.Delegato))
                {
                    DocsPaVO.utente.Utente utenteDelegato = BusinessLogic.Utenti.UserManager.getUtenteById(request.Delegato);
                    infoUtente.delegato = new DocsPaVO.utente.InfoUtente(utenteDelegato, null);
                }

                
                //Eseguo insert elemento in libro firma a partire dal passo inserito

                if (!passo.Evento.TipoEvento.Equals("W"))//Se il passo è di tipo WAIT non inserisco in Libro Firma
                {
                    if (BusinessLogic.LibroFirma.LibroFirmaManager.InserisciElementoInLibroFirma(passo, infoUtente, request.Modalita))
                    {
                        if (!string.IsNullOrEmpty(request.IdPassoPrecedente))
                        {
                            libroFirma.EliminaElementoInLibroFirma(request.IdPassoPrecedente);
                        }

                        response.Success = true;
                    }
                    else
                        response.Success = false;
                }
                else
                {
                    if (!string.IsNullOrEmpty(request.IdPassoPrecedente))
                    {
                        libroFirma.EliminaElementoInLibroFirma(request.IdPassoPrecedente);
                    }

                    response.Success = true;
                }
            }
            catch (PisException pisEx)
            {
                logger.ErrorFormat("PISException: {0}, {1}", pisEx.ErrorCode, pisEx.Description);
                response.Error = new Services.ResponseError
                {
                    Code = pisEx.ErrorCode,
                    Description = pisEx.Description
                };

                response.Success = false;
            }
            return response;
        }

        /// <summary>
        /// Conclude il passo attuale e se esiste prende il successivo altrimenti conclude il processo.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Response</returns>
        public static Services.LibroFirma.ClosePassoAndGetNext.ClosePassoAndGetNextResponse ClosePassoAndGetNext(Services.LibroFirma.ClosePassoAndGetNext.ClosePassoAndGetNextRequest request)
        {
            Services.LibroFirma.ClosePassoAndGetNext.ClosePassoAndGetNextResponse response = new Services.LibroFirma.ClosePassoAndGetNext.ClosePassoAndGetNextResponse();
            try
            {
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;
                
                //Inizio controllo autenticazione utente
                infoUtente = Utils.CheckAuthentication(request, "ClosePassoAndGetNext");
                
                utente = BusinessLogic.Utenti.UserManager.getUtenteById(infoUtente.idPeople);
                if (utente == null)
                {
                    //Utente non trovato
                    throw new PisException("USER_NO_EXIST");
                }

                if (!string.IsNullOrEmpty(request.Delegato))
                {
                    DocsPaVO.utente.Utente utenteDelegato = BusinessLogic.Utenti.UserManager.getUtenteById(request.Delegato);
                    infoUtente.delegato = new DocsPaVO.utente.InfoUtente(utenteDelegato, null);
                }
                
                DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();

                //Impoato il passo attuale a close
                if (libroFirma.UpdateStatoIstanzaPasso(request.IdIstanzaPasso, request.IdVersione, DocsPaVO.LibroFirma.TipoStatoPasso.CLOSE.ToString(), infoUtente, request.DataEsecuzione))
                {
                    libroFirma.SetErroreIstanzaPassoFirma(string.Empty, request.IdIstanzaPasso, request.IdIstanzaProcesso, DocsPaVO.LibroFirma.TipoStatoProcesso.IN_EXEC);
                    
                    //Prendo il passo successivo
                    DocsPaVO.LibroFirma.IstanzaPassoDiFirma nextPasso = libroFirma.GetNextIstanzaPasso(request.IdIstanzaProcesso, request.OrdinePasso, request.IdVersione);

                    if (nextPasso != null && (!string.IsNullOrEmpty(nextPasso.idIstanzaPasso)) && nextPasso.Evento.TipoEvento.ToUpper() != "W")
                    {
                        response.IstanzaPasso = ConvertIstanzaPassoFirma(nextPasso);
                        libroFirma.UpdateStatoIstanzaPasso(response.IstanzaPasso.idIstanzaPasso, request.IdVersione, DocsPaVO.LibroFirma.TipoStatoPasso.LOOK.ToString(), infoUtente);
                    }
                    else if (nextPasso != null && nextPasso.Evento.TipoEvento.ToUpper() == "W" && nextPasso.statoPasso.ToString() != DocsPaVO.LibroFirma.TipoStatoPasso.CUT.ToString())
                    {
                        List<DocsPaVO.LibroFirma.IstanzaProcessoDiFirma> listAllInProcess = libroFirma.GetInfoProcessesStartedForDocument(request.IdDocumento);
                        listAllInProcess = (from l in listAllInProcess where l.docAll.Equals("A") select l).ToList();
                        if (listAllInProcess != null && listAllInProcess.Count > 0)
                        {
                            response.IstanzaPasso = ConvertIstanzaPassoFirma(nextPasso);
                            libroFirma.UpdateStatoIstanzaPasso(response.IstanzaPasso.idIstanzaPasso, request.IdVersione, DocsPaVO.LibroFirma.TipoStatoPasso.LOOK.ToString(), infoUtente);
                            libroFirma.EliminaElementoInLibroFirma(request.IdIstanzaPasso);
                        }
                        else
                        {
                            response.IstanzaPasso = ConvertIstanzaPassoFirma(nextPasso);
                            libroFirma.UpdateStatoIstanzaPasso(response.IstanzaPasso.idIstanzaPasso, request.IdVersione, DocsPaVO.LibroFirma.TipoStatoPasso.CLOSE.ToString(), infoUtente);
                            nextPasso = libroFirma.GetNextIstanzaPasso(nextPasso.idIstanzaProcesso, nextPasso.numeroSequenza, request.IdVersione);
                            if (nextPasso != null && (!string.IsNullOrEmpty(nextPasso.idIstanzaPasso)))
                            {
                                response.IstanzaPasso = ConvertIstanzaPassoFirma(nextPasso);
                                libroFirma.UpdateStatoIstanzaPasso(response.IstanzaPasso.idIstanzaPasso, request.IdVersione, DocsPaVO.LibroFirma.TipoStatoPasso.LOOK.ToString(), infoUtente);
                            }
                            else
                            {
                                if (libroFirma.SetProcesComplete(request.IdIstanzaProcesso, DocsPaVO.LibroFirma.TipoStatoProcesso.CLOSED, request.IdDocumento, request.DataEsecuzione))
                                {
                                    libroFirma.UpdateUtenteLockerInLibroFirma(request.IdIstanzaPasso, infoUtente);
                                    libroFirma.EliminaElementoInLibroFirma(request.IdIstanzaPasso);
                                }
                                else
                                    throw new PisException("USER_NO_EXIST");

                                string method2 = "CONCLUSIONE_PROCESSO_LF_DOCUMENTO";
                                string description2 = "Conclusione del processo di firma per documento";
                                if (libroFirma.IsIstanzaProcessoConPassiAutomatici(request.IdIstanzaProcesso))
                                {
                                    method2 = "CONCLUSIONE_PROCESSO_AUTOMATICO_LF";
                                    description2 = "Conclusione del processo automatico di firma per documento";
                                }
                                else
                                {
                                    if (!string.IsNullOrEmpty(request.DocAll) && request.DocAll == "A")
                                    {
                                        method2 = "CONCLUSIONE_PROCESSO_LF_ALLEGATO";
                                        description2 = "Conclusione del processo di firma per allegato";
                                    }
                                }
                                BusinessLogic.LibroFirma.LibroFirmaManager.SalvaStoricoIstanzaProcessoFirma(request.IdIstanzaProcesso, request.IdDocumento, description2, infoUtente);

                                BusinessLogic.UserLog.UserLog.WriteLog(infoUtente.userId, infoUtente.idPeople, infoUtente.idGruppo, infoUtente.idAmministrazione, method2, request.IdDocumento,
                                description2, DocsPaVO.Logger.CodAzione.Esito.OK, (infoUtente != null && infoUtente.delegato != null ? infoUtente.delegato : null), "1", "", request.DataEsecuzione);

                                //Nel caso di documento principale, se il processo è stato avviato per passaggio di stato e non ha subito troncamento, 
                                //vado allo stato succesivo se presente.
                                if (string.IsNullOrEmpty(request.DocAll) || request.DocAll != "A")
                                {
                                    if (BusinessLogic.LibroFirma.LibroFirmaManager.CheckCambioStatoDocDaLF(request.IdIstanzaProcesso))
                                    {
                                        //Recupero lo stato del documento e il diagramma a cui appartiene
                                        DocsPaDB.Query_DocsPAWS.DiagrammiStato diag = new DocsPaDB.Query_DocsPAWS.DiagrammiStato();
                                        DocsPaVO.DiagrammaStato.Stato statoDoc = diag.getStatoDoc(request.IdDocumento);
                                        DocsPaVO.DiagrammaStato.DiagrammaStato diagramma = diag.getDiagrammaById(Convert.ToString(statoDoc.ID_DIAGRAMMA));

                                        //Seleziono dai passi l'id dello stato automatico LF
                                        string idStatoAutomaticoSuccessivoLF = (from p in diagramma.PASSI.Cast<DocsPaVO.DiagrammaStato.Passo>()
                                                                                where p.STATO_PADRE.SYSTEM_ID == statoDoc.SYSTEM_ID
                                                                                select p.ID_STATO_AUTOMATICO_LF).FirstOrDefault();
                                        logger.Debug("ID_STATO_SUCCESSIVO_LF " + idStatoAutomaticoSuccessivoLF);
                                        DocsPaVO.DiagrammaStato.Stato statoAutomaticoSuccessivoLF = (from s in diagramma.STATI.Cast<DocsPaVO.DiagrammaStato.Stato>()
                                                                                                     where s.SYSTEM_ID.ToString().Equals(idStatoAutomaticoSuccessivoLF)
                                                                                                     select s).FirstOrDefault();

                                        BusinessLogic.LibroFirma.LibroFirmaManager.SalvaStatoAutomaticoLF(statoAutomaticoSuccessivoLF, diagramma, request.IdDocumento, infoUtente);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (libroFirma.SetProcesComplete(request.IdIstanzaProcesso, DocsPaVO.LibroFirma.TipoStatoProcesso.CLOSED, request.IdDocumento, request.DataEsecuzione))
                        {
                            libroFirma.UpdateUtenteLockerInLibroFirma(request.IdIstanzaPasso, infoUtente);

                            if (!string.IsNullOrEmpty(request.DocAll) && request.DocAll == "A")
                            {
                                DocsPaVO.LibroFirma.WaitResponse waitStepResult = libroFirma.GetFreeWaitStep(request.IdIstanzaPasso);
                                if (waitStepResult != null)
                                {
                                    DocsPaVO.LibroFirma.IstanzaPassoDiFirma istanzaPassoDocPrincipale = libroFirma.GetIstanzaPassoDiFirmaInAttesa(waitStepResult.idProcesso);
                                    DocsPaVO.trasmissione.Trasmissione trasmWait = ExecuteTransmission(waitStepResult, istanzaPassoDocPrincipale.IsAutomatico);

                                    string desc = string.Empty;
                                    string method = "TRASM_DOC_" + (trasmWait.trasmissioniSingole[0] as DocsPaVO.trasmissione.TrasmissioneSingola).ragione.descrizione.ToUpper().Replace(" ", "_");
                                    if (trasmWait.infoDocumento.segnatura == null)
                                        desc = "Trasmesso Documento : " + trasmWait.infoDocumento.docNumber.ToString();
                                    else
                                        desc = "Trasmesso Documento : " + trasmWait.infoDocumento.segnatura.ToString();
                                    if (trasmWait != null)
                                    {
                                        if (!string.IsNullOrEmpty(waitStepResult.idElementoInLF))
                                            libroFirma.UpdateIdTrasmInElementoLF(waitStepResult.idElementoInLF, (trasmWait.trasmissioniSingole[0] as DocsPaVO.trasmissione.TrasmissioneSingola).systemId, string.Empty);
                                        string checkNotify = libroFirma.EventToBeNotified(new DocsPaVO.LibroFirma.IstanzaPassoDiFirma() { idIstanzaPasso = waitStepResult.idPasso }, "INSERIMENTO_DOCUMENTO_LF") ? "1" : "0";
                                        if (waitStepResult.gruppoAzione.Trim().ToUpper() == "EVENT")
                                            checkNotify = "1";
                                        BusinessLogic.UserLog.UserLog.WriteLog(trasmWait.utente.userId, trasmWait.utente.idPeople, trasmWait.ruolo.idGruppo, trasmWait.utente.idAmministrazione, method, trasmWait.infoDocumento.docNumber, desc, DocsPaVO.Logger.CodAzione.Esito.OK,
                                            (infoUtente != null && infoUtente.delegato != null ? infoUtente.delegato : null), checkNotify, (trasmWait.trasmissioniSingole[0] as DocsPaVO.trasmissione.TrasmissioneSingola).systemId);

                                        if (istanzaPassoDocPrincipale.IsAutomatico)
                                        {
                                            //Se il passo è un passo proseguo con la sua esecuzione
                                            DocsPaVO.LibroFirma.IstanzaProcessoDiFirma istanzaProcesso = new DocsPaVO.LibroFirma.IstanzaProcessoDiFirma() { docNumber = trasmWait.infoDocumento.docNumber, statoProcesso = DocsPaVO.LibroFirma.TipoStatoProcesso.IN_EXEC };
                                            BusinessLogic.LibroFirma.LibroFirmaManager.EseguiPassoAutomaticoAsync(istanzaPassoDocPrincipale, istanzaProcesso);
                                        }
                                    }
                                    else
                                    {
                                        BusinessLogic.UserLog.UserLog.WriteLog(trasmWait.utente.userId, trasmWait.utente.idPeople, trasmWait.ruolo.idGruppo, trasmWait.utente.idAmministrazione, method, trasmWait.infoDocumento.docNumber, desc, DocsPaVO.Logger.CodAzione.Esito.KO,
                                               (infoUtente != null && infoUtente.delegato != null ? infoUtente.delegato : null), "0", (trasmWait.trasmissioniSingole[0] as DocsPaVO.trasmissione.TrasmissioneSingola).systemId);
                                    }
                                }
                            }

                            libroFirma.EliminaElementoInLibroFirma(request.IdIstanzaPasso);
                        }
                        else
                            throw new PisException("USER_NO_EXIST");

                        string method2 = "CONCLUSIONE_PROCESSO_LF_DOCUMENTO";
                        string description2 = "Conclusione del processo di firma per documento";
                        if (libroFirma.IsIstanzaProcessoConPassiAutomatici(request.IdIstanzaProcesso))
                        {
                            method2 = "CONCLUSIONE_PROCESSO_AUTOMATICO_LF";
                            description2 = "Conclusione del processo automatico di firma per documento";
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(request.DocAll) && request.DocAll == "A")
                            {
                                method2 = "CONCLUSIONE_PROCESSO_LF_ALLEGATO";
                                description2 = "Conclusione del processo di firma per allegato";
                            }
                        }
                        BusinessLogic.LibroFirma.LibroFirmaManager.SalvaStoricoIstanzaProcessoFirma(request.IdIstanzaProcesso, request.IdDocumento, description2, infoUtente);
                        BusinessLogic.UserLog.UserLog.WriteLog(infoUtente.userId, infoUtente.idPeople, infoUtente.idGruppo, infoUtente.idAmministrazione, method2, request.IdDocumento,
                        description2, DocsPaVO.Logger.CodAzione.Esito.OK, (infoUtente != null && infoUtente.delegato != null ? infoUtente.delegato : null), "1", "", request.DataEsecuzione);

                        //Nel caso di documento principale, se il processo è stato avviato per passaggio di stato e non ha subito troncamento, 
                        //vado allo stato succesivo se presente.
                        if (string.IsNullOrEmpty(request.DocAll) || request.DocAll != "A")
                        {
                            if (BusinessLogic.LibroFirma.LibroFirmaManager.CheckCambioStatoDocDaLF(request.IdIstanzaProcesso))
                            {
                                //Recupero lo stato del documento e il diagramma a cui appartiene
                                DocsPaDB.Query_DocsPAWS.DiagrammiStato diag = new DocsPaDB.Query_DocsPAWS.DiagrammiStato();
                                DocsPaVO.DiagrammaStato.Stato statoDoc = diag.getStatoDoc(request.IdDocumento);
                                DocsPaVO.DiagrammaStato.DiagrammaStato diagramma = diag.getDiagrammaById(Convert.ToString(statoDoc.ID_DIAGRAMMA));

                                //Seleziono dai passi l'id dello stato automatico LF
                                string idStatoAutomaticoSuccessivoLF = (from p in diagramma.PASSI.Cast<DocsPaVO.DiagrammaStato.Passo>()
                                                                        where p.STATO_PADRE.SYSTEM_ID == statoDoc.SYSTEM_ID
                                                                        select p.ID_STATO_AUTOMATICO_LF).FirstOrDefault();
                                logger.Debug("ID_STATO_SUCCESSIVO_LF " + idStatoAutomaticoSuccessivoLF);
                                DocsPaVO.DiagrammaStato.Stato statoAutomaticoSuccessivoLF = (from s in diagramma.STATI.Cast<DocsPaVO.DiagrammaStato.Stato>()
                                                                                             where s.SYSTEM_ID.ToString().Equals(idStatoAutomaticoSuccessivoLF)
                                                                                             select s).FirstOrDefault();

                                BusinessLogic.LibroFirma.LibroFirmaManager.SalvaStatoAutomaticoLF(statoAutomaticoSuccessivoLF, diagramma, request.IdDocumento, infoUtente);
                            }
                        }

                    }
                }

                response.Success = true;
            }
            catch (PisException pisEx)
            {
                logger.ErrorFormat("PISException: {0}, {1}", pisEx.ErrorCode, pisEx.Description);
                response.Error = new Services.ResponseError
                {
                    Code = pisEx.ErrorCode,
                    Description = pisEx.Description
                };

                response.Success = false;
            }

            return response;
        }

        private static DocsPaVO.trasmissione.Trasmissione ExecuteTransmission(DocsPaVO.LibroFirma.WaitResponse waitStepResult, bool isAutomatico)
        {
            //DocsPaVO.LibroFirma.IstanzaPassoDiFirma istanzaPasso;

            DocsPaVO.trasmissione.Trasmissione trasm = new DocsPaVO.trasmissione.Trasmissione();

            DocsPaDB.Query_DocsPAWS.Utenti u = new DocsPaDB.Query_DocsPAWS.Utenti();

            trasm.ruolo = u.GetRuoloByIdGruppo(waitStepResult.idRoleMit);//istanzaProcesso.RuoloProponente;
            trasm.utente = u.getUtenteById(waitStepResult.idPeopleMit);//istanzaProcesso.UtenteProponente;
            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();

            DocsPaVO.documento.InfoDocumento infoDoc = doc.GetInfoDocumentoLite(waitStepResult.idDocumento);
            trasm.infoDocumento = infoDoc;

            string notePasso = string.IsNullOrEmpty(waitStepResult.noteProcesso) ? waitStepResult.notePasso : (string.IsNullOrEmpty(waitStepResult.notePasso)) ? waitStepResult.noteProcesso : (waitStepResult.noteProcesso + " - " + waitStepResult.notePasso);

            string tipoPasso = string.Empty;
            if (waitStepResult.gruppoAzione.Trim().ToUpper() == "EVENT")
            {
                trasm.noteGenerali = "Azione richiesta " + waitStepResult.descAzione + ". " + notePasso;
                tipoPasso = waitStepResult.gruppoAzione;
            }
            else
            {
                trasm.noteGenerali = notePasso;
                tipoPasso = waitStepResult.codAzione;
            }

            if (isAutomatico)
                tipoPasso += "_AUTOMATICO";

            //INSERISCO LA RAGIONE DI TRASMISSIONE DI SISTEMA PER LIBRO FIRMA
            DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();
            DocsPaVO.utente.Ruolo ruolo = utenti.GetRuoloByIdGruppo(waitStepResult.idRoleDest);
            DocsPaVO.trasmissione.RagioneTrasmissione ragione = BusinessLogic.Trasmissioni.RagioniManager.GetRagioneByTipoOperazione(tipoPasso, ruolo.idAmministrazione);

            //CREO LA TRASMISSIONE SINGOLA
            DocsPaVO.trasmissione.TrasmissioneSingola trasmSing = new DocsPaVO.trasmissione.TrasmissioneSingola();
            trasmSing.ragione = ragione;
            trasmSing.tipoTrasm = "S";

            trasmSing.corrispondenteInterno = ruolo;
            trasmSing.tipoDest = DocsPaVO.trasmissione.TipoDestinatario.RUOLO;

            System.Collections.ArrayList listaUtenti = new System.Collections.ArrayList();
            DocsPaVO.addressbook.QueryCorrispondente qc = new DocsPaVO.addressbook.QueryCorrispondente();
            qc.codiceRubrica = ruolo.codiceRubrica;
            System.Collections.ArrayList registri = ruolo.registri;
            qc.tipoUtente = DocsPaVO.addressbook.TipoUtente.INTERNO;
            //qc.idRegistri = registri;
            qc.idAmministrazione = ruolo.idAmministrazione;
            qc.getChildren = true;
            qc.fineValidita = true;
            listaUtenti = BusinessLogic.Utenti.addressBookManager.listaCorrispondentiIntMethod(qc);
            System.Collections.ArrayList trasmissioniUt = new System.Collections.ArrayList();

            //Se l'id utente titolare non è specificato, il documento è stato inserito  nel libro firma di tutti gli utenti del ruolo,
            //e quindi la trasmissione andrà notificata a tutti gli utenti, altrimenti al solo utente titolare
            if (string.IsNullOrEmpty(waitStepResult.idPeopleDest))
            {
                for (int k = 0; k < listaUtenti.Count; k++)
                {
                    DocsPaVO.trasmissione.TrasmissioneUtente trUt = new DocsPaVO.trasmissione.TrasmissioneUtente();
                    trUt.utente = (DocsPaVO.utente.Utente)listaUtenti[k];
                    trasmissioniUt.Add(trUt);
                }
            }
            else
            {
                for (int k = 0; k < listaUtenti.Count; k++)
                {
                    DocsPaVO.trasmissione.TrasmissioneUtente trUt = new DocsPaVO.trasmissione.TrasmissioneUtente();
                    trUt.utente = (DocsPaVO.utente.Utente)listaUtenti[k];
                    trUt.daNotificare = (listaUtenti[k] as DocsPaVO.utente.Utente).idPeople.Equals(waitStepResult.idPeopleDest);
                    trasmissioniUt.Add(trUt);
                }
            }
            trasmSing.trasmissioneUtente = trasmissioniUt;
            trasm.trasmissioniSingole = new System.Collections.ArrayList() { trasmSing };
            return BusinessLogic.Trasmissioni.ExecTrasmManager.saveExecuteTrasmMethod("", trasm);
        }

        private static Domain.IstanzaPassoFirma ConvertIstanzaPassoFirma(DocsPaVO.LibroFirma.IstanzaPassoDiFirma passo)
        {
            Domain.IstanzaPassoFirma newPasso = new Domain.IstanzaPassoFirma();

            newPasso.CodiceTipoEvento = passo.CodiceTipoEvento;
            newPasso.dataEsecuzione = passo.dataEsecuzione;
            newPasso.dataScadenza = passo.dataScadenza;
            newPasso.descrizioneStatoPasso = passo.descrizioneStatoPasso;
            newPasso.idIstanzaPasso = passo.idIstanzaPasso;
            newPasso.idIstanzaProcesso = passo.idIstanzaProcesso;
            newPasso.idNotificaEffettuata = passo.idNotificaEffettuata;
            newPasso.idPasso = passo.idPasso;
            newPasso.IdTipoEvento = passo.IdTipoEvento;
            newPasso.motivoRespingimento = passo.motivoRespingimento;
            newPasso.Note = passo.Note;
            newPasso.numeroSequenza = passo.numeroSequenza;
            newPasso.IdRuoloCoinvolto = passo.RuoloCoinvolto.idGruppo;
            newPasso.statoPasso = passo.statoPasso.ToString();
            newPasso.TipoFirma = passo.TipoFirma;
            newPasso.IdUtenteCoinvolto = passo.UtenteCoinvolto.idPeople;

            return newPasso;
        }

        public static Services.LibroFirma.GetSignatureProcesses.GetSignatureProcessesResponse GetSignatureProcesses(Services.LibroFirma.GetSignatureProcesses.GetSignatureProcessesRequest request)
        {
            Services.LibroFirma.GetSignatureProcesses.GetSignatureProcessesResponse response = new Services.LibroFirma.GetSignatureProcesses.GetSignatureProcessesResponse();
            try
            {
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;

                //Inizio controllo autenticazione utente
                infoUtente = Utils.CheckAuthentication(request, "GetSignatureProcesses");

                utente = BusinessLogic.Utenti.UserManager.getUtenteById(infoUtente.idPeople);
                if (utente == null)
                {
                    //Utente non trovato
                    throw new PisException("USER_NO_EXIST");
                }
                //Fine controllo autenticazione utente

                List<DocsPaVO.LibroFirma.ProcessoFirma> listaProc= BusinessLogic.LibroFirma.LibroFirmaManager.GetProcessesSignatureVisibleRole(true, true, false, infoUtente);
                if (listaProc != null && listaProc.Count > 0)
                {
                    response.Processes = new Domain.SignBook.SignatureProcess[listaProc.Count];
                    Domain.SignBook.SignatureProcess spD = null;
                    int i = 0;
                    foreach (DocsPaVO.LibroFirma.ProcessoFirma proc in listaProc)
                    {
                        spD = new Domain.SignBook.SignatureProcess(proc);
                        response.Processes[i] = spD;
                        i++;
                    }
                }
                
                response.Success = true;

            }
            catch (PisException pisEx)
            {
                logger.ErrorFormat("PISException: {0}, {1}", pisEx.ErrorCode, pisEx.Description);
                response.Error = new Services.ResponseError
                {
                    Code = pisEx.ErrorCode,
                    Description = pisEx.Description
                };

                response.Success = false;
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Eccezione Generica: APPLICATION_ERROR, {0}", ex.Message);
                response.Error = new Services.ResponseError
                {
                    Code = "APPLICATION_ERROR",
                    Description = ex.Message
                };

                response.Success = false;
            }

            return response;
        }

        public static Services.LibroFirma.GetSignatureProcess.GetSignatureProcessResponse GetSignatureProcess(Services.LibroFirma.GetSignatureProcess.GetSignatureProcessRequest request)
        {
            Services.LibroFirma.GetSignatureProcess.GetSignatureProcessResponse response = new Services.LibroFirma.GetSignatureProcess.GetSignatureProcessResponse();
            try
            {
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;

                //Inizio controllo autenticazione utente
                infoUtente = Utils.CheckAuthentication(request, "GetSignatureProcess");

                utente = BusinessLogic.Utenti.UserManager.getUtenteById(infoUtente.idPeople);
                if (utente == null)
                {
                    //Utente non trovato
                    throw new PisException("USER_NO_EXIST");
                }
                //Fine controllo autenticazione utente
                DocsPaVO.LibroFirma.ProcessoFirma proc = BusinessLogic.LibroFirma.LibroFirmaManager.GetProcessoDiFirmaById(request.IdProcess, infoUtente);
                if (proc != null)
                {
                    response.SignatureProcess = new Domain.SignBook.SignatureProcess(proc);
                }
                else { throw new Exception("Signature Process not found"); }

                response.Success = true;

            }
            catch (PisException pisEx)
            {
                logger.ErrorFormat("PISException: {0}, {1}", pisEx.ErrorCode, pisEx.Description);
                response.Error = new Services.ResponseError
                {
                    Code = pisEx.ErrorCode,
                    Description = pisEx.Description
                };

                response.Success = false;
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Eccezione Generica: APPLICATION_ERROR, {0}", ex.Message);
                response.Error = new Services.ResponseError
                {
                    Code = "APPLICATION_ERROR",
                    Description = ex.Message
                };

                response.Success = false;
            }

            return response;
        }

        public static Services.LibroFirma.GetSignProcessInstance.GetSignProcessInstanceResponse GetSignProcessInstance(Services.LibroFirma.GetSignProcessInstance.GetSignProcessInstanceRequest request)
        {
            Services.LibroFirma.GetSignProcessInstance.GetSignProcessInstanceResponse response = new Services.LibroFirma.GetSignProcessInstance.GetSignProcessInstanceResponse();
            try
            {
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;

                //Inizio controllo autenticazione utente
                infoUtente = Utils.CheckAuthentication(request, "GetSignatureProcess");

                utente = BusinessLogic.Utenti.UserManager.getUtenteById(infoUtente.idPeople);
                if (utente == null)
                {
                    //Utente non trovato
                    throw new PisException("USER_NO_EXIST");
                }
                //Fine controllo autenticazione utente
                DocsPaVO.LibroFirma.IstanzaProcessoDiFirma istpdf = BusinessLogic.LibroFirma.LibroFirmaManager.GetIstanzaProcessoDiFirmaByIdIstanzaProcesso(request.IdProcessInstance,infoUtente);
                if (istpdf != null)
                {
                    response.ProcessInstance = new Domain.SignBook.SignatureProcessInstance(istpdf);
                }
                else { throw new Exception("Singature Process Instance not found"); }
                
                response.Success = true;

            }
            catch (PisException pisEx)
            {
                logger.ErrorFormat("PISException: {0}, {1}", pisEx.ErrorCode, pisEx.Description);
                response.Error = new Services.ResponseError
                {
                    Code = pisEx.ErrorCode,
                    Description = pisEx.Description
                };

                response.Success = false;
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Eccezione Generica: APPLICATION_ERROR, {0}", ex.Message);
                response.Error = new Services.ResponseError
                {
                    Code = "APPLICATION_ERROR",
                    Description = ex.Message
                };

                response.Success = false;
            }

            return response;
        }

        public static Services.LibroFirma.SearchSignProcessInstances.SearchSignProcessInstancesResponse SearchSignProcessInstances(Services.LibroFirma.SearchSignProcessInstances.SearchSignProcessInstancesRequest request)
        {
            Services.LibroFirma.SearchSignProcessInstances.SearchSignProcessInstancesResponse response = new Services.LibroFirma.SearchSignProcessInstances.SearchSignProcessInstancesResponse();
            try
            {
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;

                //Inizio controllo autenticazione utente
                infoUtente = Utils.CheckAuthentication(request, "GetSignatureProcess");

                utente = BusinessLogic.Utenti.UserManager.getUtenteById(infoUtente.idPeople);
                if (utente == null)
                {
                    //Utente non trovato
                    throw new PisException("USER_NO_EXIST");
                }
                //Fine controllo autenticazione utente

                List<DocsPaVO.LibroFirma.FiltroIstanzeProcessoFirma> filtri = new List<DocsPaVO.LibroFirma.FiltroIstanzeProcessoFirma>();
                DocsPaVO.LibroFirma.FiltroIstanzeProcessoFirma fTemp;
                List<DocsPaVO.LibroFirma.IstanzaProcessoDiFirma> istanze = new List<DocsPaVO.LibroFirma.IstanzaProcessoDiFirma>();
                int totnumpage = 0;
                if (request.Filters != null && request.Filters.Length > 0)
                {
                    foreach (Domain.Filter fPis in request.Filters)
                    {
                        fTemp = new DocsPaVO.LibroFirma.FiltroIstanzeProcessoFirma();
                        switch (fPis.Name)
                        {
                            case "PROCESS_ID":
                                fTemp.Argomento = "ID_PROCESSO";
                                fTemp.Valore = fPis.Value;
                                break;
                            case "DOC_NUMBER":
                                fTemp.Argomento = "DOCNUMBER";
                                fTemp.Valore = fPis.Value;
                                break;
                            case "NOTE":
                                fTemp.Argomento = "NOTE";
                                fTemp.Valore = fPis.Value;
                                break;
                            case "START_DATE":
                                fTemp.Argomento = "DATA_AVVIO_IL";
                                fTemp.Valore = fPis.Value;
                                break;
                            case "START_DATE_FROM":
                                fTemp.Argomento = "DATA_AVVIO_SUCCESSIVA_AL";
                                fTemp.Valore = fPis.Value;
                                break;
                            case "START_DATE_TO":
                                fTemp.Argomento = "DATA_AVVIO_PRECEDENTE_IL";
                                fTemp.Valore = fPis.Value;
                                break;
                            case "START_NOTES":
                                fTemp.Argomento = "NOTE_AVVIO";
                                fTemp.Valore = fPis.Value;
                                break;
                            case "END_DATE":
                                fTemp.Argomento = "DATA_CONCLUSIONE_IL";
                                fTemp.Valore = fPis.Value;
                                break;
                            case "END_DATE_FROM":
                                fTemp.Argomento = "DATA_CONCLUSIONE_SUCCESSIVA_AL";
                                fTemp.Valore = fPis.Value;
                                break;
                            case "END_DATE_TO":
                                fTemp.Argomento = "DATA_CONCLUSIONE_PRECEDENTE_IL";
                                fTemp.Valore = fPis.Value;
                                break;
                            case "INTERRUPTION_DATE":
                                fTemp.Argomento = "DATA_INTERRUZIONE_IL";
                                fTemp.Valore = fPis.Value;
                                break;
                            case "INTERRUPTION_DATE_FROM":
                                fTemp.Argomento = "DATA_INTERRUZIONE_SUCCESSIVA_AL";
                                fTemp.Valore = fPis.Value;
                                break;
                            case "INTERRUPTION_DATE_TO":
                                fTemp.Argomento = "DATA_INTERRUZIONE_PRECEDENTE_IL";
                                fTemp.Valore = fPis.Value;
                                break;
                            case "REFUSAL_NOTE":
                                fTemp.Argomento = "NOTE_RESPINGIMENTO";
                                fTemp.Valore = fPis.Value;
                                break;
                            case "IN_EXECUTION":
                                fTemp.Argomento = "STATO_IN_ESECUZIONE";
                                fTemp.Valore = fPis.Value;
                                break;
                            case "INTERRUPTED":
                                fTemp.Argomento = "STATO_INTERROTTO";
                                fTemp.Valore = fPis.Value;
                                break;
                            case "ENDED":
                                fTemp.Argomento = "STATO_CONCLUSO";
                                fTemp.Valore = fPis.Value;
                                break;
                            case "TRUNCATED":
                                fTemp.Argomento = "TRONCATO";
                                fTemp.Valore = fPis.Value;
                                break;

                        }
                        filtri.Add(fTemp);
                    }
                    
                int numpage= request.PageNumber ?? 1, numInPage= request.ElementsInPage ?? 20,  nrec;

                    //List<DocsPaVO.LibroFirma.IstanzaProcessoDiFirma> istanze = GetIstanzaProcessiDiFirmaByFilter(List<DocsPaVO.LibroFirma.FiltroIstanzeProcessoFirma> filtro, int numPage, int pageSize, out int numTotPage, out int nRec, DocsPaVO.utente.InfoUtente infoUtente);
                DataSet istanzeProcessi = null;
                istanze = BusinessLogic.LibroFirma.LibroFirmaManager.GetIstanzaProcessiDiFirmaByFilter(filtri, numpage, numInPage, out totnumpage, out nrec, infoUtente, out istanzeProcessi);
                
                }
                if(istanze!=null && istanze.Count>0)
                {
                    response.SignatureProcessInstances = new Domain.SignBook.SignatureProcessInstance[istanze.Count];
                    int indice = 0;
                    response.TotalNumber = istanze.Count;

                    foreach(DocsPaVO.LibroFirma.IstanzaProcessoDiFirma iX in istanze) {
                        response.SignatureProcessInstances[indice] = new Domain.SignBook.SignatureProcessInstance(iX);
                        indice++;
                    }
                }

                response.Success = true;


            }
            catch (PisException pisEx)
            {
                logger.ErrorFormat("PISException: {0}, {1}", pisEx.ErrorCode, pisEx.Description);
                response.Error = new Services.ResponseError
                {
                    Code = pisEx.ErrorCode,
                    Description = pisEx.Description
                };

                response.Success = false;
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Eccezione Generica: APPLICATION_ERROR, {0}", ex.Message);
                response.Error = new Services.ResponseError
                {
                    Code = "APPLICATION_ERROR",
                    Description = ex.Message
                };

                response.Success = false;
            }

            return response;
        }

        public static Services.LibroFirma.GetInstanceSearchFilters.GetInstanceSearchFiltersResponse GetInstanceSearchFilters(Services.Request request)
        {
            Services.LibroFirma.GetInstanceSearchFilters.GetInstanceSearchFiltersResponse response = new Services.LibroFirma.GetInstanceSearchFilters.GetInstanceSearchFiltersResponse();
            try
            {
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;

                //Inizio controllo autenticazione utente
                infoUtente = Utils.CheckAuthentication(request, "GetInstanceSearchFilters");

                utente = BusinessLogic.Utenti.UserManager.getUtenteById(infoUtente.idPeople);
                if (utente == null)
                {
                    //Utente non trovato
                    throw new PisException("USER_NO_EXIST");
                }
                //Fine controllo autenticazione utente
                response.Success = true;

                //TODO

                List<Domain.Filter> listaFiltri = new List<Domain.Filter>();

                listaFiltri.Add(new Domain.Filter() { Name = "PROCESS_ID", Description = "ID del processo associato all'istanza di firma", Type = Domain.FilterTypeEnum.Number });
                listaFiltri.Add(new Domain.Filter() { Name = "DOC_NUMBER", Description = "ID del documento associato all'istanza di firma", Type = Domain.FilterTypeEnum.Number });
                listaFiltri.Add(new Domain.Filter() { Name = "NOTE", Description = "Stringa contenuta nelle note dell'istanza", Type = Domain.FilterTypeEnum.String });
                listaFiltri.Add(new Domain.Filter() { Name = "START_DATE", Description = "Data di avvio dell'istanza di firma", Type = Domain.FilterTypeEnum.Date });
                listaFiltri.Add(new Domain.Filter() { Name = "START_DATE_FROM", Description = "Data di avvio dell'istanza di firma successiva a", Type = Domain.FilterTypeEnum.Date });
                listaFiltri.Add(new Domain.Filter() { Name = "START_DATE_TO", Description = "Data di avvio dell'istanza di firma precedende il", Type = Domain.FilterTypeEnum.Date });
                listaFiltri.Add(new Domain.Filter() { Name = "START_NOTES", Description = "Stringa contenuta nelle note di avvio dell'istanza", Type = Domain.FilterTypeEnum.String });
                listaFiltri.Add(new Domain.Filter() { Name = "END_DATE", Description = "Data di conclusione dell'istanza di firma", Type = Domain.FilterTypeEnum.Date });
                listaFiltri.Add(new Domain.Filter() { Name = "END_DATE_FROM", Description = "Data di conclusione dell'istanza di firma successiva a", Type = Domain.FilterTypeEnum.Date });
                listaFiltri.Add(new Domain.Filter() { Name = "END_DATE_TO", Description = "Data di conclusione dell'istanza di firma precedende il", Type = Domain.FilterTypeEnum.Date });
                listaFiltri.Add(new Domain.Filter() { Name = "INTERRUPTION_DATE", Description = "Data di interruzione dell'istanza di firma", Type = Domain.FilterTypeEnum.Date });
                listaFiltri.Add(new Domain.Filter() { Name = "INTERRUPTION_DATE_FROM", Description = "Data di interruzione dell'istanza di firma successiva a", Type = Domain.FilterTypeEnum.Date });
                listaFiltri.Add(new Domain.Filter() { Name = "INTERRUPTION_DATE_TO", Description = "Data di interruzione dell'istanza di firma precedende il", Type = Domain.FilterTypeEnum.Date });
                listaFiltri.Add(new Domain.Filter() { Name = "REFUSAL_NOTE", Description = "Stringa contenuta nelle note di respingimento dell'istanza", Type = Domain.FilterTypeEnum.String });
                listaFiltri.Add(new Domain.Filter() { Name = "IN_EXECUTION", Description = "Processo di firma in esecuzione", Type = Domain.FilterTypeEnum.Bool });
                listaFiltri.Add(new Domain.Filter() { Name = "INTERRUPTED", Description = "Processo di firma interrotto", Type = Domain.FilterTypeEnum.Bool });
                listaFiltri.Add(new Domain.Filter() { Name = "ENDED", Description = "Processo di firma concluso", Type = Domain.FilterTypeEnum.Bool });
                listaFiltri.Add(new Domain.Filter() { Name = "TRUNCATED", Description = "Processo di firma troncato", Type = Domain.FilterTypeEnum.Bool });

                response.Filters = listaFiltri.ToArray();


            }
            catch (PisException pisEx)
            {
                logger.ErrorFormat("PISException: {0}, {1}", pisEx.ErrorCode, pisEx.Description);
                response.Error = new Services.ResponseError
                {
                    Code = pisEx.ErrorCode,
                    Description = pisEx.Description
                };

                response.Success = false;
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Eccezione Generica: APPLICATION_ERROR, {0}", ex.Message);
                response.Error = new Services.ResponseError
                {
                    Code = "APPLICATION_ERROR",
                    Description = ex.Message
                };

                response.Success = false;
            }

            return response;
        }

        public static Services.LibroFirma.InterruptSignatureProcess.InterruptSignatureProcessResponse InterruptSignatureProcess(Services.LibroFirma.InterruptSignatureProcess.InterruptSignatureProcessRequest request)
        {
            Services.LibroFirma.InterruptSignatureProcess.InterruptSignatureProcessResponse response = new Services.LibroFirma.InterruptSignatureProcess.InterruptSignatureProcessResponse();
            try
            {
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;

                //Inizio controllo autenticazione utente
                infoUtente = Utils.CheckAuthentication(request, "GetInstanceSearchFilters");

                utente = BusinessLogic.Utenti.UserManager.getUtenteById(infoUtente.idPeople);
                if (utente == null)
                {
                    //Utente non trovato
                    throw new PisException("USER_NO_EXIST");
                }
                //Fine controllo autenticazione utente
                response.Success = true;

                //TODO
                DocsPaVO.utente.Ruolo ruolo = BusinessLogic.Utenti.UserManager.getRuoloByIdGruppo(infoUtente.idGruppo);
                DocsPaVO.LibroFirma.IstanzaProcessoDiFirma istpdf = BusinessLogic.LibroFirma.LibroFirmaManager.GetIstanzaProcessoDiFirmaByIdIstanzaProcesso(request.IdSignProcessInstance, infoUtente);
                bool result = BusinessLogic.LibroFirma.LibroFirmaManager.InterruptionSignatureProcessByProponent(istpdf, request.InterruptionNote, ruolo, infoUtente);
                string idDocPrincipale = "";
                
                //Se è andato a buon fine ed è un allegato
                if (result && (istpdf.docAll.Equals("A")))
                {
                    DocsPaVO.documento.Allegato allX = new DocsPaVO.documento.Allegato();
                    allX.docNumber= istpdf.docNumber;
                    idDocPrincipale = BusinessLogic.Documenti.AllegatiManager.getIdDocumentoPrincipale(allX);
                    BusinessLogic.LibroFirma.LibroFirmaManager.StopPassoWait(idDocPrincipale, infoUtente);
                }


            }
            catch (PisException pisEx)
            {
                logger.ErrorFormat("PISException: {0}, {1}", pisEx.ErrorCode, pisEx.Description);
                response.Error = new Services.ResponseError
                {
                    Code = pisEx.ErrorCode,
                    Description = pisEx.Description
                };

                response.Success = false;
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Eccezione Generica: APPLICATION_ERROR, {0}", ex.Message);
                response.Error = new Services.ResponseError
                {
                    Code = "APPLICATION_ERROR",
                    Description = ex.Message
                };

                response.Success = false;
            }

            return response;
        }

        public static Services.LibroFirma.StartSignatureProcess.StartSignatureProcessResponse StartSignatureProcess(Services.LibroFirma.StartSignatureProcess.StartSignatureProcessRequest request)
        {
            Services.LibroFirma.StartSignatureProcess.StartSignatureProcessResponse response = new Services.LibroFirma.StartSignatureProcess.StartSignatureProcessResponse();
            try
            {
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;

                //Inizio controllo autenticazione utente
                infoUtente = Utils.CheckAuthentication(request, "GetInstanceSearchFilters");

                utente = BusinessLogic.Utenti.UserManager.getUtenteById(infoUtente.idPeople);
                if (utente == null)
                {
                    //Utente non trovato
                    throw new PisException("USER_NO_EXIST");
                }
                //Fine controllo autenticazione utente
                response.Success = true;

                //TODO
                DocsPaVO.LibroFirma.ProcessoFirma procFirma;
                DocsPaVO.documento.FileRequest frequest;
                bool retBool = false;
                string idDocPrincipale="", modalita = "A";
                DocsPaVO.documento.SchedaDocumento documento = new DocsPaVO.documento.SchedaDocumento();
                
                if (!string.IsNullOrEmpty(request.IdDocument))
                {
                    documento= BusinessLogic.Documenti.DocManager.getDettaglio(infoUtente, request.IdDocument, request.IdDocument);
                    if(documento!= null)
                    {
                        frequest= (DocsPaVO.documento.FileRequest)documento.documenti[0];
                    }
                    else
                    {
                        throw new PisException("DOCUMENT_NOT_FOUND");
                    }
                }
                else
                {
                    throw new PisException("REQUIRED_ID"); 
                }
                if (string.IsNullOrEmpty(request.Note))
                {
                    throw new PisException("MISSING_PARAMETER");
                }

                if (string.IsNullOrEmpty(idDocPrincipale)) { }

                DocsPaVO.LibroFirma.ResultProcessoFirma resultAvvio ;

                if (request.SignatureProcess != null)
                {
                    procFirma = Utils.GetProcessoFirmaFromDomain(request.SignatureProcess);
                }
                else
                {
                    throw new PisException("MISSING_PARAMETER");
                }
                DocsPaVO.LibroFirma.OpzioniNotifica opzioniNotifica = new DocsPaVO.LibroFirma.OpzioniNotifica();
                opzioniNotifica.Notifica_concluso = false;
                opzioniNotifica.Notifica_interrotto = true;

                retBool = BusinessLogic.LibroFirma.LibroFirmaManager.StartProcessoDiFirma(procFirma, 
                    frequest, 
                    infoUtente, 
                    "A", request.Note, opzioniNotifica, 
                    out resultAvvio);

                if (retBool)
                {
                    string method = "AVVIATO_PROCESSO_DI_FIRMA_DOCUMENTO";
                    string description = "Avviato processo di firma per la versione " + frequest.version;
                    if (frequest.GetType().Equals(typeof(DocsPaVO.documento.Allegato)))
                    {
                        method = "AVVIATO_PROCESSO_DI_FIRMA_ALLEGATO";
                    }

                    BusinessLogic.UserLog.UserLog.WriteLog(infoUtente.userId, infoUtente.idPeople, infoUtente.idGruppo, infoUtente.idAmministrazione, method, frequest.docNumber,
                        description, DocsPaVO.Logger.CodAzione.Esito.OK, (infoUtente != null && infoUtente.delegato != null ? infoUtente.delegato : null), "1");
                }

                switch (resultAvvio)
                {
                    case DocsPaVO.LibroFirma.ResultProcessoFirma.DOCUMENTO_BLOCCATO:
                        throw new Exception("DOCUMENTO BLOCCATO");
                        break;
                    case DocsPaVO.LibroFirma.ResultProcessoFirma.DOCUMENTO_CONSOLIDATO:
                        throw new Exception("DOCUMENTO CONSOLIDATO");
                        break;
                    case DocsPaVO.LibroFirma.ResultProcessoFirma.DOCUMENTO_GIA_IN_LIBRO_FIRMA:
                        throw new Exception("DOCUMENTO GIA IN LIBRO FIRMA");
                        break;
                    case DocsPaVO.LibroFirma.ResultProcessoFirma.EXISTING_PROCESS_NAME:
                        throw new Exception("ERRORE GENERICO");
                        break;
                    case DocsPaVO.LibroFirma.ResultProcessoFirma.FILE_NON_AMMESSO_ALLA_FIRMA:
                        throw new Exception("FILE NON AMMESSO ALLA FIRMA");
                        break;
                    case DocsPaVO.LibroFirma.ResultProcessoFirma.KO:
                        throw new Exception("ERRORE GENERICO");
                        break;
                }
                if (!retBool) throw new Exception("ERRORE GENERICO");
                
            }
            catch (PisException pisEx)
            {
                logger.ErrorFormat("PISException: {0}, {1}", pisEx.ErrorCode, pisEx.Description);
                response.Error = new Services.ResponseError
                {
                    Code = pisEx.ErrorCode,
                    Description = pisEx.Description
                };

                response.Success = false;
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Eccezione Generica: APPLICATION_ERROR, {0}", ex.Message);
                response.Error = new Services.ResponseError
                {
                    Code = "APPLICATION_ERROR",
                    Description = ex.Message
                };

                response.Success = false;
            }

            return response;
        }



    }
}
