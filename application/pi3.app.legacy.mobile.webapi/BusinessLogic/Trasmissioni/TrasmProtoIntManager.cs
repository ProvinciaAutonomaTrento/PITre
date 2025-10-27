// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Serilog;
using System.Collections;

namespace BusinessLogic.Trasmissioni
{
    public class TrasmProtoIntManager
    {
        private static ILogger logger = Log.ForContext(typeof(TrasmProtoIntManager));
        /// <summary>
        /// Eccezione che indica che le ragioni trasmissioni dedicate alla protocollazione 
        /// interna non sono state trovate sul DB.
        /// </summary>
        public class RagioniNotFoundException : Exception
        {
            public override string Message
            {
                get { return "Ragioni per protocollazione interna non trovate."; }
            }
        }


        /// <summary>
        /// Trasmette il protocollo creato automaticamente su un registro automatico ai destinatari del protocollo
        /// in partenza
        /// </summary>
        /// <param name="schedaDoc"></param>
        /// <param name="ruolo"></param>
        /// <param name="serverName"></param>
        /// <param name="isEnableRef"></param>
        /// <param name="RagioniVerificate"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static bool TrasmissioneProtocolloAutomatico(DocsPaVO.documento.SchedaDocumento schedaDocArrivo, string idRegistro, DocsPaVO.documento.SchedaDocumento schedaDocPartenza, DocsPaVO.utente.Ruolo ruolo, DocsPaVO.utente.InfoUtente infoUtente, string serverName, bool isEnableRef, out bool RagioniVerificate, out string message)
        {
            bool result = true;
            message = "";
            RagioniVerificate = true;
            bool destTo = false;
            bool destCC = false;
            message = "";
            try
            {

                if (((DocsPaVO.documento.ProtocolloUscita)(schedaDocPartenza.protocollo)).destinatari != null && ((DocsPaVO.documento.ProtocolloUscita)(schedaDocPartenza.protocollo)).destinatari.Count > 0)
                {
                    destTo = true;
                    if (((DocsPaVO.documento.ProtocolloUscita)(schedaDocPartenza.protocollo)).destinatariConoscenza != null && ((DocsPaVO.documento.ProtocolloUscita)(schedaDocPartenza.protocollo)).destinatariConoscenza.Count > 0)
                    {
                        destCC = true;
                    }
                }

                if (!VerificaRagioni(ruolo.idAmministrazione, false, schedaDocPartenza.tipoProto, destTo, destCC, out message)) throw new RagioniNotFoundException();

                // Predispone le trasmissioni ai destinatari del documento in partenza
                DocsPaVO.trasmissione.Trasmissione trasmissione = SetCorrispondentiTrasmissioneAutomatica(schedaDocArrivo, idRegistro, schedaDocPartenza, ruolo, infoUtente);


                if (trasmissione == null) throw new Exception();

                trasmissione.infoDocumento.tipoProto = schedaDocArrivo.tipoProto; //"I";

                if (trasmissione.trasmissioniSingole.Count > 0)
                {
                    // Salvataggio e Esecuzione della trasmissione solamente se il destinatario != mittente
                    if (infoUtente.delegato != null)
                        trasmissione.delegato = ((DocsPaVO.utente.InfoUtente)(infoUtente.delegato)).idPeople;

                    //BusinessLogic.Trasmissioni.TrasmManager.saveTrasmMethod(trasmissione);
                    //BusinessLogic.Trasmissioni.ExecTrasmManager.executeTrasmMethod(serverName, trasmissione);

                    BusinessLogic.Trasmissioni.ExecTrasmManager.saveExecuteTrasmMethod(serverName, trasmissione);

                }
            }
            catch (RagioniNotFoundException)
            {
                RagioniVerificate = false;
            }
            catch (Exception)
            {
                result = false;
            }

            return result;
        }

        public static bool VerificaRagioni(string idAmm, bool isEnableRef, string tipoProto, bool destTo, bool destCC, out string message)
        {
            bool result;
            DocsPaDB.Query_DocsPAWS.Trasmissione vtr = new DocsPaDB.Query_DocsPAWS.Trasmissione();

            result = vtr.VerificaRagProtIntBis(idAmm, isEnableRef, tipoProto, destTo, destCC, out message);

            return result;
        }

        public static DocsPaVO.trasmissione.Trasmissione SetCorrispondentiTrasmissioneAutomatica(DocsPaVO.documento.SchedaDocumento schedaDocArrivo, string idRegistro, DocsPaVO.documento.SchedaDocumento schedaDocUscita, DocsPaVO.utente.Ruolo ruolo, DocsPaVO.utente.InfoUtente infoUtente)
        {
            DocsPaVO.trasmissione.Trasmissione trasmissione = null;
            DocsPaVO.trasmissione.RagioneTrasmissione ragTrasmDestTO = null;
            DocsPaVO.trasmissione.RagioneTrasmissione ragTrasmDestCC = null;
            try
            {
                trasmissione = new DocsPaVO.trasmissione.Trasmissione();
                trasmissione.ruolo = ruolo;
                trasmissione.utente = BusinessLogic.Utenti.UserManager.getUtente(infoUtente.idPeople);
                trasmissione.utente.dst = infoUtente.dst;
                trasmissione.infoFascicolo = null;
                trasmissione.utente.idAmministrazione = infoUtente.idAmministrazione;
                trasmissione.infoDocumento = new DocsPaVO.documento.InfoDocumento(schedaDocArrivo);

                //creo l'oggetto qca in caso di trasmissioni a UO
                DocsPaVO.addressbook.QueryCorrispondente qco = new DocsPaVO.addressbook.QueryCorrispondente();
                qco.fineValidita = true;
                DocsPaVO.addressbook.QueryCorrispondenteAutorizzato qca = new DocsPaVO.addressbook.QueryCorrispondenteAutorizzato();

                qca.ruolo = ruolo;
                qca.queryCorrispondente = qco;
                qca.idRegistro = idRegistro;

                // Acquisisci le ragioni per la trasmissione del protocollo in arrivo ai vari destinatari	
                ragTrasmDestTO = BusinessLogic.Trasmissioni.RagioniManager.GetRagione("TO", ruolo.idAmministrazione);
                ragTrasmDestCC = BusinessLogic.Trasmissioni.RagioniManager.GetRagione("CC", ruolo.idAmministrazione);

                #region protocollo in uscita

                DocsPaVO.documento.ProtocolloUscita protoUscTO = (DocsPaVO.documento.ProtocolloUscita)schedaDocUscita.protocollo;
                if (protoUscTO != null && protoUscTO.destinatari != null)
                {
                    foreach (object destinatario in protoUscTO.destinatari)
                    {
                        trasmissione.noteGenerali = "";
                        qca.ragione = ragTrasmDestTO;
                        DocsPaVO.utente.Corrispondente corr = (DocsPaVO.utente.Corrispondente)destinatario;


                        if (corr.tipoIE == "I")
                        {
                            // se siamo di fronte ad una UO, ne individuiamo i ruoli e trasmettiamo ai relativi utenti
                            if (corr.GetType() == typeof(DocsPaVO.utente.UnitaOrganizzativa))
                            {
                                //prendiamo i ruoli di riferimento autorizzati
                                ArrayList listaRuoliRiferimento = BusinessLogic.Utenti.addressBookManager.getRuoliRiferimentoAutorizzati(qca, (DocsPaVO.utente.UnitaOrganizzativa)corr);
                                string id_reg_proto = ((DocsPaVO.utente.Registro)ruolo.registri[0]).codRegistro;

                                if (listaRuoliRiferimento != null && listaRuoliRiferimento.Count > 0)
                                {
                                    foreach (object objRuolo in listaRuoliRiferimento)
                                    {
                                        /*
                                         * modificato sabrina per eliminare il controllo che non consentiva di inviare trasmissioni ai ruoli autorizzati
                                         * il controllo sul codice del corrispondente che deve coincidere con il codice del registro è stato messo forse per qualche cliente particolare
                                         * ma non và bene!!!
                                         */
                                        //if (((DocsPaVO.utente.Ruolo)objRuolo).codiceCorrispondente.StartsWith(id_reg_proto))
                                        trasmissione = AddTrasmissioneSingola(trasmissione, (DocsPaVO.utente.Ruolo)objRuolo, qca.ragione);
                                    }
                                }

                            }
                            else if (corr.GetType() == typeof(DocsPaVO.utente.Ruolo))
                            {
                                if (verificaRuoloAutorizzato(corr.systemId, idRegistro))
                                {
                                    trasmissione = AddTrasmissioneSingola(trasmissione, corr, qca.ragione);
                                }

                            }
                            else if (corr.GetType() == typeof(DocsPaVO.utente.Utente))
                            {
                                bool IsAutorized = false;
                                //si prendono i ruoli dell'utente destinatario del protocollo
                                ArrayList listaRuoliUtente = BusinessLogic.Utenti.UserManager.getRuoliUtente(((DocsPaVO.utente.Utente)corr).idPeople);
                                //si verifica se tali ruoli sono autorizzati sul registro corrente
                                if (listaRuoliUtente != null && listaRuoliUtente.Count > 0)
                                {
                                    foreach (object objRuolo in listaRuoliUtente)
                                    {
                                        if (verificaRuoloAutorizzato(((DocsPaVO.utente.Ruolo)objRuolo).systemId, idRegistro))
                                        {
                                            IsAutorized = true;
                                            break;

                                        }
                                    }

                                }
                                //se almeno uno dei ruoli dell'utente è autorizzato sul registro corrente
                                //si effettua la trasmissione.
                                if (IsAutorized)
                                {
                                    trasmissione = AddTrasmissioneSingola(trasmissione, corr, qca.ragione);
                                }
                            }

                        }

                    }
                }
                #endregion

                #region Creazione trasmissioni singole ai destinatari CC

                #region protocollo in uscita
                DocsPaVO.documento.ProtocolloUscita protoUscCC = (DocsPaVO.documento.ProtocolloUscita)schedaDocUscita.protocollo;

                if (protoUscCC != null && protoUscCC.destinatariConoscenza != null)
                {
                    foreach (object destinatario in protoUscCC.destinatariConoscenza)
                    {
                        trasmissione.noteGenerali = "";
                        qca.ragione = ragTrasmDestCC;
                        DocsPaVO.utente.Corrispondente corr = (DocsPaVO.utente.Corrispondente)destinatario;

                        if (corr.tipoIE == "I")
                        {
                            if (corr.GetType() == typeof(DocsPaVO.utente.UnitaOrganizzativa))
                            {
                                // se siamo di fronte ad una UO, ne individuiamo i ruoli e trasmettiamo ai relativi utenti
                                ArrayList listaRuoli = BusinessLogic.Utenti.addressBookManager.getRuoliRiferimentoAutorizzati(qca, (DocsPaVO.utente.UnitaOrganizzativa)corr);
                                string id_reg_proto = ((DocsPaVO.utente.Registro)ruolo.registri[0]).codRegistro;


                                if (listaRuoli != null && listaRuoli.Count > 0)
                                {
                                    foreach (object objRuolo in listaRuoli)
                                    {
                                        /*
                                      * modificato sabrina per eliminare il controllo che non consentiva di inviare trasmissioni ai ruoli autorizzati
                                      * il controllo sul codice del corrispondente che deve coincidere con il codice del registro è stato messo forse per qualche cliente particolare
                                      * ma non và bene!!!
                                      */
                                        //  if (((DocsPaVO.utente.Ruolo)objRuolo).codiceCorrispondente.StartsWith(id_reg_proto))
                                        trasmissione = AddTrasmissioneSingola(trasmissione, (DocsPaVO.utente.Ruolo)objRuolo, qca.ragione);
                                    }
                                }
                            }
                            else if (corr.GetType() == typeof(DocsPaVO.utente.Ruolo))
                            {
                                if (verificaRuoloAutorizzato(corr.systemId, idRegistro))
                                {
                                    trasmissione = AddTrasmissioneSingola(trasmissione, corr, qca.ragione);
                                }

                            }
                            else if (corr.GetType() == typeof(DocsPaVO.utente.Utente))
                            {
                                bool IsAutorized = false;
                                //si prendono i ruoli dell'utente destinatario del protocollo
                                ArrayList listaRuoliUtente = BusinessLogic.Utenti.UserManager.getRuoliUtente(((DocsPaVO.utente.Utente)corr).idPeople);
                                //si verifica se tali ruoli sono autorizzati sul registro corrente
                                if (listaRuoliUtente != null && listaRuoliUtente.Count > 0)
                                {
                                    foreach (object objRuolo in listaRuoliUtente)
                                    {
                                        if (verificaRuoloAutorizzato(((DocsPaVO.utente.Ruolo)objRuolo).systemId, idRegistro))
                                        {
                                            IsAutorized = true;
                                            break;

                                        }
                                    }

                                }
                                //se almeno uno dei ruoli dell'utente è autorizzato sul registro corrente
                                //si effettua la trasmissione.
                                if (IsAutorized)
                                {
                                    trasmissione = AddTrasmissioneSingola(trasmissione, corr, qca.ragione);
                                }
                            }
                        }

                    }
                }
                #endregion

                #endregion
            }
            catch (Exception)
            {
                trasmissione = null;
            }

            return trasmissione;
        }

        private static DocsPaVO.trasmissione.Trasmissione AddTrasmissioneSingola(DocsPaVO.trasmissione.Trasmissione trasmissione, DocsPaVO.utente.Corrispondente corr, DocsPaVO.trasmissione.RagioneTrasmissione ragTrasm)
        {
            if (trasmissione.trasmissioniSingole != null)
            {
                // controllo se esiste la trasmissione singola associata a corrispondente selezionato
                foreach (object objTrasm in trasmissione.trasmissioniSingole)
                {
                    DocsPaVO.trasmissione.TrasmissioneSingola ts = (DocsPaVO.trasmissione.TrasmissioneSingola)objTrasm;

                    if (ts != null && ts.corrispondenteInterno.systemId == corr.systemId)
                    {
                        if (ts.daEliminare) ts.daEliminare = false;
                        return trasmissione;
                    }
                }
            }

            // Aggiungo la trasmissione singola
            DocsPaVO.trasmissione.TrasmissioneSingola trasmissioneSingola = new DocsPaVO.trasmissione.TrasmissioneSingola();
            trasmissioneSingola.tipoTrasm = "S";
            trasmissioneSingola.corrispondenteInterno = corr;
            trasmissioneSingola.ragione = ragTrasm;

            // Aggiungo la lista di trasmissioniUtente
            if (corr.GetType() == typeof(DocsPaVO.utente.Ruolo))
            {
                // Gestione Ruoli/UO
                trasmissioneSingola.tipoDest = DocsPaVO.trasmissione.TipoDestinatario.RUOLO;
                if (corr.idAmministrazione == null) corr.idAmministrazione = ((DocsPaVO.utente.Corrispondente)(((DocsPaVO.utente.Ruolo)(corr)).uo)).idAmministrazione;
                ArrayList listaUtenti = QueryUtenti(corr.codiceRubrica, corr.idAmministrazione);

                //if (listaUtenti == null || listaUtenti.Count == 0) return trasmissione;
                if (listaUtenti.Count == 0)
                    trasmissioneSingola = null;

                // Ciclo per utenti se dest è gruppo o ruolo
                foreach (object objUtente in listaUtenti)
                {
                    DocsPaVO.utente.Corrispondente utente = (DocsPaVO.utente.Corrispondente)objUtente;

                    DocsPaVO.trasmissione.TrasmissioneUtente trasmissioneUtente = new DocsPaVO.trasmissione.TrasmissioneUtente();
                    trasmissioneUtente.utente = (DocsPaVO.utente.Utente)utente;
                    if (ragTrasm.descrizione == "RISPOSTA") trasmissioneUtente.idTrasmRispSing = trasmissioneSingola.systemId;
                    trasmissioneSingola.trasmissioneUtente.Add(trasmissioneUtente);
                }
            }
            else
            {
                trasmissioneSingola.tipoDest = DocsPaVO.trasmissione.TipoDestinatario.UTENTE;
                DocsPaVO.trasmissione.TrasmissioneUtente trasmissioneUtente = new DocsPaVO.trasmissione.TrasmissioneUtente();
                trasmissioneUtente.utente = (DocsPaVO.utente.Utente)corr;
                trasmissioneSingola.trasmissioneUtente.Add(trasmissioneUtente);
            }

            trasmissione.trasmissioniSingole.Add(trasmissioneSingola);

            return trasmissione;
        }

        public static bool verificaRuoloAutorizzato(string idcorr, string registro)
        {
            bool result = false;
            try
            {

                result = BusinessLogic.Utenti.addressBookManager.VerificaAutorizzazioneRuolo(idcorr, registro);
            }
            catch
            {

            }
            return result;
        }

        private static ArrayList QueryUtenti(string codiceRubrica, string idAmministrazione)
        {

            // costruzione oggetto queryCorrispondente
            DocsPaVO.addressbook.QueryCorrispondente qco = new DocsPaVO.addressbook.QueryCorrispondente();

            qco.codiceRubrica = codiceRubrica;
            qco.getChildren = true;
            qco.fineValidita = true;

            qco.idAmministrazione = idAmministrazione;

            // corrispondenti interni
            qco.tipoUtente = DocsPaVO.addressbook.TipoUtente.INTERNO;

            ArrayList listaCorrispondenti = BusinessLogic.Utenti.addressBookManager.getListaCorrispondenti(qco);

            return listaCorrispondenti;
        }

    }
}
