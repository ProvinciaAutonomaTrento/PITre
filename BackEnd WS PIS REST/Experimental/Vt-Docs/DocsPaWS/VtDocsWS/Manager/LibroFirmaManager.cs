using System;
using System.Collections.Generic;
using System.Linq;
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

                DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
                //Carico i dati del passo successivo dall'id ottenuto in request
                DocsPaVO.LibroFirma.IstanzaPassoDiFirma passo = libroFirma.GetIstanzaPassoDiFirma(request.IdPasso);
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
                                if (!string.IsNullOrEmpty(request.DocAll) && request.DocAll == "A")
                                {
                                    method2 = "CONCLUSIONE_PROCESSO_LF_ALLEGATO";
                                    description2 = "Conclusione del processo di firma per allegato";
                                }

                                BusinessLogic.UserLog.UserLog.WriteLog(infoUtente.userId, infoUtente.idPeople, infoUtente.idGruppo, infoUtente.idAmministrazione, method2, request.IdDocumento,
                                description2, DocsPaVO.Logger.CodAzione.Esito.OK, (infoUtente != null && infoUtente.delegato != null ? infoUtente.delegato : null), "1", "", request.DataEsecuzione);
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
                                    DocsPaVO.trasmissione.Trasmissione trasmWait = ExecuteTransmission(waitStepResult);

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
                        if (!string.IsNullOrEmpty(request.DocAll) && request.DocAll == "A")
                        {
                            method2 = "CONCLUSIONE_PROCESSO_LF_ALLEGATO";
                            description2 = "Conclusione del processo di firma per allegato";
                        }

                        BusinessLogic.UserLog.UserLog.WriteLog(infoUtente.userId, infoUtente.idPeople, infoUtente.idGruppo, infoUtente.idAmministrazione, method2, request.IdDocumento,
                        description2, DocsPaVO.Logger.CodAzione.Esito.OK, (infoUtente != null && infoUtente.delegato != null ? infoUtente.delegato : null), "1", "", request.DataEsecuzione);

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

        private static DocsPaVO.trasmissione.Trasmissione ExecuteTransmission(DocsPaVO.LibroFirma.WaitResponse waitStepResult)
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
    }
}
