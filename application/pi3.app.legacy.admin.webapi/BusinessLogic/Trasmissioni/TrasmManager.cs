// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using BusinessLogic.RubricaComune;
using Serilog;
using System.Collections;
using System.Data;
using System.Text;

namespace BusinessLogic.Trasmissioni
{
    public class TrasmManager
    {
        private static ILogger logger = Log.ForContext(typeof(TrasmManager));
        /// <summary>
        /// </summary>
        /// <param name="objTrasm"></param>
        /// <returns></returns>
        public static DocsPaVO.trasmissione.Trasmissione saveTrasmMethod(DocsPaVO.trasmissione.Trasmissione objTrasm)
        {
            logger.Information("BEGIN");
            logger.Debug("saveTrasmMethod");

            checkInputData(objTrasm);

            try
            {
                objTrasm = saveTrasmissione(objTrasm);

                // aggiorno il campo assegnazione
                setAssegnazione(objTrasm);

                string basePathFiles = DocsPaVO.Settings.AppSettings.Instance.LOG_PATH;
                basePathFiles = basePathFiles.Replace("%DATA", DateTime.Now.ToString("yyyyMMdd"));
                basePathFiles = basePathFiles + "\\Interoperabilita";
            }
            catch (Exception e)
            {
                logger.Debug(e.Message);
                logger.Debug("Errore nella gestione delle trasmissioni (saveTrasmMethod)", e);
                throw e;
            }
            logger.Information("END");
            return objTrasm;
        }

        private static DocsPaVO.trasmissione.Trasmissione checkInputData(DocsPaVO.trasmissione.Trasmissione objTrasm)
        {
            logger.Debug("checkInputData");
            int i;
            // verifico i dati per le trasmissioni singole
            ArrayList tsList = objTrasm.trasmissioniSingole;
            for (i = 0; i < tsList.Count; i++)
            {
                DocsPaVO.trasmissione.TrasmissioneSingola ts = (DocsPaVO.trasmissione.TrasmissioneSingola)tsList[i];
                // verifico il campo CHA_TIPO_TRASM
                if (ts != null && ts.tipoTrasm != null)
                {
                    if (!ts.tipoTrasm.Equals("S") && !ts.tipoTrasm.Equals("T"))
                    {
                        #region Codice Commentato
                        //throw throwException(db, "Tipo Trasmissione non consentito - Valore attuale: " + ts.tipoTrasm);
                        //db.rollbackTransaction();
                        //db.closeConnection();
                        #endregion

                        logger.Debug("Errore nella gestione delle trasmissioni (checkInputData). Tipo Trasmissione non consentito - Valore attuale: " + ts.tipoTrasm);
                        throw new Exception("Tipo Trasmissione non consentito - Valore attuale: " + ts.tipoTrasm);
                    }
                }
                #region Codice Commentato
                // verifico il campo CHA_TIPO_DEST
                //   non dovrebbe essere necessario perchè tipoDest è un enum
                /*if (!ts.tipoDest.Equals("U") && !ts.tipoDest.Equals("R") && !ts.tipoDest.Equals("G"))
                    throw throwException("Tipo Destinatario non consentito - Valore attuale: " + ts.tipoDest);*/
                #endregion
            }
            return null;
        }

        private static DocsPaVO.trasmissione.Trasmissione saveTrasmissione(DocsPaVO.trasmissione.Trasmissione objTrasm)
        {
            logger.Information("BEGIN");
            bool inModifica = false;
            string id = string.Empty;
            DocsPaDB.Query_DocsPAWS.Trasmissione objQuery = new DocsPaDB.Query_DocsPAWS.Trasmissione();
            DocsPaVO.trasmissione.Trasmissione trasmInUscita = new DocsPaVO.trasmissione.Trasmissione();
            DocsPaVO.trasmissione.TrasmissioneSingola objTrasmSingola;

            logger.Debug("saveTrasmissione");

            // se la trasmissione è stata inviata non devo fare nulla
            if (objTrasm.dataInvio != null && !objTrasm.dataInvio.Equals(""))
                throw new Exception("Trasmissione già inviata");

            // verifica se il documento non è in cestino
            if (objTrasm.tipoOggetto == DocsPaVO.trasmissione.TipoOggetto.DOCUMENTO)
            {
                string incestino = BusinessLogic.Documenti.DocManager.checkdocInCestino(objTrasm.infoDocumento.docNumber);
                if (incestino != "" && incestino == "1")
                    throw new Exception("Il documento è stato rimosso, non è più possibile trasmetterlo");
            }

            if (objTrasm.systemId != null && !objTrasm.systemId.Equals(""))
            {
                inModifica = true;
                trasmInUscita = updateTrasmissione(objTrasm);
            }
            else
            {
                trasmInUscita = insertTrasmissione(objTrasm);
                objTrasm.systemId = trasmInUscita.systemId;
            }

            if (inModifica)
            {
                // elimina tutte trasm.ni utente e trasm.ni singole
                objQuery.DeleteTrasmSingola(objTrasm);
                // e deve inserire tutte le trasm.ni singole come se fossero nuove
                for (int i = 0; i < objTrasm.trasmissioniSingole.Count; i++)
                {
                    objTrasmSingola = (DocsPaVO.trasmissione.TrasmissioneSingola)objTrasm.trasmissioniSingole[i];
                    objTrasmSingola.systemId = "";
                    objTrasm.trasmissioniSingole[i] = objTrasmSingola;
                }
            }

            //for (int i = 0; i < objTrasm.trasmissioniSingole.Count; i++)
            ////foreach (DocsPaVO.trasmissione.TrasmissioneSingola trs in objTrasm.trasmissioniSingole)
            //{
            //    id = objQuery.insertTrasmSingola(trs, objTrasm.systemId);
            //    trs.systemId = id;
            //    logger.Debug("idTrasmSingola = " + id);

            //    foreach (DocsPaVO.trasmissione.TrasmissioneUtente tru in trs.trasmissioneUtente)
            //    {
            //        trasmUtenteInUscita = new DocsPaVO.trasmissione.TrasmissioneUtente();
            //        trasmUtenteInUscita = objQuery.insertTrasmUtente(tru, id);
            //        tru.systemId = trasmUtenteInUscita.systemId;
            //    }
            //}


            for (int i = 0; i < objTrasm.trasmissioniSingole.Count; i++)
            {
                objTrasmSingola = (DocsPaVO.trasmissione.TrasmissioneSingola)objTrasm.trasmissioniSingole[i];
                if (objTrasmSingola != null)
                    objTrasmSingola = saveTrasmSingola(objTrasmSingola, objTrasm.systemId);
                if (objTrasmSingola != null)
                    objTrasm.trasmissioniSingole[i] = objTrasmSingola;
            }
            //// elimino le trasmissioni singole che devono essere cancellate
            //for (int i=idDaEliminare.Count; i > 0 ; i--)
            //    objTrasm.trasmissioniSingole.RemoveAt((int)idDaEliminare[i-1]);

            // se non esistono trasmissioni singole associate alla trasmissione
            // viene eliminata anche la trasmissione stessa
            if (objTrasm.trasmissioniSingole.Count == 0)
                DeleteTrasmissione(objTrasm);
            logger.Information("END");
            return objTrasm;
        }

        private static void setAssegnazione(DocsPaVO.trasmissione.Trasmissione objTrasm)
        {
            DocsPaDB.Query_DocsPAWS.Trasmissione trasmissione = new DocsPaDB.Query_DocsPAWS.Trasmissione();
            trasmissione.SetAssegnazione(objTrasm);
        }

        private static DocsPaVO.trasmissione.Trasmissione updateTrasmissione(DocsPaVO.trasmissione.Trasmissione objTrasm)
        {
            #region Codice Commentato
            /*logger.Debug("updateTrasmissione");
			if (!objTrasm.daAggiornare)
				return	objTrasm;
			string sqlString =
				"UPDATE DPA_TRASMISSIONE SET CHA_TIPO_OGGETTO = '" + notNull((string)DocsPaVO.trasmissione.Trasmissione.oggettoStringa[objTrasm.tipoOggetto],true) +
				"', VAR_NOTE_GENERALI = '" + notNull(objTrasm.noteGenerali,true) + "' WHERE SYSTEM_ID=" + objTrasm.systemId;
			logger.Debug(sqlString);
			db.executeNonQuery(sqlString);
			return objTrasm;*/
            #endregion

            DocsPaDB.Query_DocsPAWS.Trasmissione trasmissione = new DocsPaDB.Query_DocsPAWS.Trasmissione();
            return trasmissione.updateTrasmissione(objTrasm);
        }

        private static DocsPaVO.trasmissione.Trasmissione insertTrasmissione(DocsPaVO.trasmissione.Trasmissione objTrasm)
        {
            logger.Debug("insertTrasmissione");

            DocsPaDB.Query_DocsPAWS.Trasmissione trasmissione = new DocsPaDB.Query_DocsPAWS.Trasmissione();
            String idTrasmissione = trasmissione.insertTrasmissione(objTrasm);
            trasmissione.Dispose();

            logger.Debug("idTrasmissione = " + idTrasmissione);
            objTrasm.systemId = idTrasmissione;
            return objTrasm;
        }

        private static DocsPaVO.trasmissione.TrasmissioneSingola saveTrasmSingola(DocsPaVO.trasmissione.TrasmissioneSingola objTrasmSingola, string idTrasmissione)
        {
            logger.Information("BEGIN");
            logger.Debug("saveTrasmSingola");
            if (objTrasmSingola.daEliminare)
                return deleteTrasmSingola(/*db,*/ objTrasmSingola);

            if (objTrasmSingola.systemId != null && !objTrasmSingola.systemId.Equals(""))
                objTrasmSingola = updateTrasmSingola(/*db,*/ objTrasmSingola);
            else
                objTrasmSingola = insertTrasmSingola(/*db,*/ objTrasmSingola, idTrasmissione);

            // aggiorno la DPA_TRASM_UTENTE
            for (int j = 0; j < objTrasmSingola.trasmissioneUtente.Count; j++)
            {
                DocsPaVO.trasmissione.TrasmissioneUtente objTrasmUtente = (DocsPaVO.trasmissione.TrasmissioneUtente)objTrasmSingola.trasmissioneUtente[j];
                bool isUserDisabled = string.IsNullOrEmpty(objTrasmUtente.utente.disabilitato) ? BusinessLogic.Utenti.UserManager.isUserDisabled(objTrasmUtente.utente.codiceRubrica, objTrasmUtente.utente.idAmministrazione)
                    : objTrasmUtente.utente.disabilitato.Equals("Y");
                if (!isUserDisabled)
                {
                    objTrasmUtente = saveTrasmUtente(/*db,*/ objTrasmUtente, objTrasmSingola.systemId);
                    objTrasmSingola.trasmissioneUtente[j] = objTrasmUtente;
                }
            }
            // aggiorno la DPA_TRASM_UTENTE relativa alla risposta
            if (objTrasmSingola.idTrasmUtente != null && !objTrasmSingola.idTrasmUtente.Equals(""))
                updateTrasmRispSing(/*db,*/ objTrasmSingola.idTrasmUtente, "'" + objTrasmSingola.systemId + "'");
            logger.Information("END");
            return objTrasmSingola;
        }

        /// <summary>
        /// Elimina la trasmissione in toto (trasm.ni singole e trasm.ni utente)
        /// </summary>
        /// <param name="objTrasmissione">oggetto trasmissione</param>
        /// <returns>true o false</returns>
        public static bool DeleteTrasmissione(DocsPaVO.trasmissione.Trasmissione objTrasmissione)
        {
            logger.Debug("deleteTrasmissione");

            bool retValue = true;

            try
            {
                if (objTrasmissione.systemId == null)
                    return false;

                DocsPaDB.Query_DocsPAWS.Trasmissione trasmissione = new DocsPaDB.Query_DocsPAWS.Trasmissione();

                // elimina prima le trasmissioni singole
                foreach (DocsPaVO.trasmissione.TrasmissioneSingola ts in objTrasmissione.trasmissioniSingole)
                    trasmissione.DeleteTrasmSingola(ts);

                // quindi elimina la trasmissione stessa
                trasmissione.DeleteTrasmissione(objTrasmissione);
            }
            catch
            {
                retValue = false;
            }

            return retValue;
        }

        private static DocsPaVO.trasmissione.TrasmissioneSingola deleteTrasmSingola(DocsPaVO.trasmissione.TrasmissioneSingola objTrasmSingola)
        {
            logger.Debug("deleteTrasmSingola");

            if (objTrasmSingola.systemId == null)
            {
                return null;
            }

            DocsPaDB.Query_DocsPAWS.Trasmissione trasmissione = new DocsPaDB.Query_DocsPAWS.Trasmissione();

            // se ho cancellato risposte singole devo aggiorare le relative ID_TRASM_RISP_SING
            if (objTrasmSingola.idTrasmUtente != null && !objTrasmSingola.Equals(""))
            {
                updateTrasmRispSing(/*db,*/ objTrasmSingola.idTrasmUtente, "null");
            }

            return null;
        }

        private static DocsPaVO.trasmissione.TrasmissioneSingola updateTrasmSingola(DocsPaVO.trasmissione.TrasmissioneSingola objTrasmSingola)
        {
            DocsPaDB.Query_DocsPAWS.Trasmissione trasmissione = new DocsPaDB.Query_DocsPAWS.Trasmissione();
            return trasmissione.updateTrasmSingola(objTrasmSingola);
        }

        private static DocsPaVO.trasmissione.TrasmissioneSingola insertTrasmSingola(DocsPaVO.trasmissione.TrasmissioneSingola objTrasmSingola, string idTrasmissione)
        {
            ArrayList idDaEliminare = new ArrayList();
            DocsPaDB.Query_DocsPAWS.Trasmissione trasmissione = new DocsPaDB.Query_DocsPAWS.Trasmissione();
            String idTrasmSingola = trasmissione.insertTrasmSingola(/*db,*/objTrasmSingola, idTrasmissione);
            trasmissione.Dispose();
            logger.Debug("idTrasmSingola = " + idTrasmSingola);
            objTrasmSingola.systemId = idTrasmSingola;
            for (int i = 0; i < objTrasmSingola.trasmissioneUtente.Count; i++)
            {
                if (((DocsPaVO.trasmissione.TrasmissioneUtente)objTrasmSingola.trasmissioneUtente[i]).daNotificare)
                {
                    DocsPaVO.utente.Utente user = ((DocsPaVO.trasmissione.TrasmissioneUtente)objTrasmSingola.trasmissioneUtente[i]).utente;
                    bool isUserDisabled = string.IsNullOrEmpty(user.disabilitato) ? BusinessLogic.Utenti.UserManager.isUserDisabled(user.codiceRubrica, user.idAmministrazione)
                        : user.disabilitato.Equals("Y");
                    if (!isUserDisabled)
                    {
                        objTrasmSingola.trasmissioneUtente[i] = insertTrasmUtente(/*db,*/ (DocsPaVO.trasmissione.TrasmissioneUtente)objTrasmSingola.trasmissioneUtente[i], objTrasmSingola.systemId);
                    }
                }
                else
                {
                    idDaEliminare.Add(i);
                }
            }

            // elimino le trasmissioni utente che devono essere cancellate
            for (int i = idDaEliminare.Count; i > 0; i--)
                objTrasmSingola.trasmissioneUtente.RemoveAt((int)idDaEliminare[i - 1]);

            return objTrasmSingola;
        }

        private static DocsPaVO.trasmissione.TrasmissioneUtente saveTrasmUtente(DocsPaVO.trasmissione.TrasmissioneUtente objTrasmUtente, string idTrasmissioneSingola)
        {
            logger.Information("BEGIN");
            logger.Debug("saveTrasmUtente");
            if (objTrasmUtente.systemId != null && !objTrasmUtente.systemId.Equals(""))
                objTrasmUtente = updateTrasmUtente(/*db,*/ objTrasmUtente);
            else
                objTrasmUtente = insertTrasmUtente(/*db,*/ objTrasmUtente, idTrasmissioneSingola);
            logger.Information("END");
            return objTrasmUtente;
        }

        private static void updateTrasmRispSing(string systemId, string idTrasmRispSingola)
        {
            DocsPaDB.Query_DocsPAWS.Trasmissione trasmissione = new DocsPaDB.Query_DocsPAWS.Trasmissione();
            trasmissione.UpdateTrasmRispSing(systemId, idTrasmRispSingola);
            trasmissione.Dispose();
        }

        private static DocsPaVO.trasmissione.TrasmissioneUtente insertTrasmUtente(DocsPaVO.trasmissione.TrasmissioneUtente objTrasmUtente, string idTrasmSingola)
        {
            DocsPaDB.Query_DocsPAWS.Trasmissione trasmissione = new DocsPaDB.Query_DocsPAWS.Trasmissione();
            return trasmissione.insertTrasmUtente(/*db,*/objTrasmUtente, idTrasmSingola);
        }

        private static DocsPaVO.trasmissione.TrasmissioneUtente updateTrasmUtente(DocsPaVO.trasmissione.TrasmissioneUtente objTrasmUtente)
        {
            DocsPaDB.Query_DocsPAWS.Trasmissione trasmissione = new DocsPaDB.Query_DocsPAWS.Trasmissione();
            return trasmissione.UpdateTrasmUtente(objTrasmUtente);
        }

        public static DocsPaVO.trasmissione.Trasmissione addTrasmissioneSingola(DocsPaVO.trasmissione.Trasmissione trasmissione, DocsPaVO.utente.Corrispondente corr, DocsPaVO.trasmissione.RagioneTrasmissione ragione, string note, string tipoTrasm)
        {
            //Controllo se il ruolo è disabilitato alla ricezione delle trasmissioni
            if (corr != null && corr.tipoCorrispondente == "R")
            {
                DocsPaDB.Query_DocsPAWS.Utenti utentiDb = new DocsPaDB.Query_DocsPAWS.Utenti();
                DocsPaVO.utente.Corrispondente corrispondente = utentiDb.getRuoloById(corr.systemId);
                if (corrispondente != null && corrispondente.disabledTrasm)
                    return trasmissione;
            }

            if (trasmissione.trasmissioniSingole != null)
            {
                // controllo se esiste la trasmissione singola associata a corrispondente selezionato
                for (int i = 0; i < trasmissione.trasmissioniSingole.Count; i++)
                {
                    DocsPaVO.trasmissione.TrasmissioneSingola ts = (DocsPaVO.trasmissione.TrasmissioneSingola)trasmissione.trasmissioniSingole[i];
                    if (ts != null && ts.corrispondenteInterno.systemId != null && corr!= null && ts.corrispondenteInterno.systemId.Equals(corr.systemId))
                    {
                        if (ts.daEliminare)
                        {
                            ((DocsPaVO.trasmissione.TrasmissioneSingola)trasmissione.trasmissioniSingole[i]).daEliminare = false;
                            return trasmissione;
                        }
                        else
                            return trasmissione;
                    }
                }
            }

            // Aggiungo la trasmissione singola
            DocsPaVO.trasmissione.TrasmissioneSingola trasmissioneSingola = new DocsPaVO.trasmissione.TrasmissioneSingola();
            trasmissioneSingola.tipoTrasm = tipoTrasm;
            trasmissioneSingola.corrispondenteInterno = corr;
            trasmissioneSingola.ragione = ragione;
            trasmissioneSingola.noteSingole = note;

            // Aggiungo la lista di trasmissioniUtente
            if (corr is DocsPaVO.utente.Ruolo)
            {
                trasmissioneSingola.tipoDest = DocsPaVO.trasmissione.TipoDestinatario.RUOLO;
                //DocsPaVO.utente.Corrispondente[] listaUtenti = queryUtenti(corr);
                ArrayList listaUtenti = queryUtenti(corr);
                //if (listaUtenti.Length == 0)
                if (listaUtenti.Count == 0)
                {
                    //Prova Andrea
                    trasmissione.listaDestinatariNonRaggiungibili.Add("Nessun utente per il ruolo " + corr.codiceCorrispondente + " (" + corr.descrizione + ").");
                    //End Andrea
                    trasmissioneSingola = null;
                }
                //ciclo per utenti se dest è gruppo o ruolo
                //for (int i = 0; i < listaUtenti.Length; i++)
                for (int i = 0; i < listaUtenti.Count; i++)
                {
                    DocsPaVO.trasmissione.TrasmissioneUtente trasmissioneUtente = new DocsPaVO.trasmissione.TrasmissioneUtente();
                    trasmissioneUtente.utente = (DocsPaVO.utente.Utente)listaUtenti[i];
                    //trasmissioneSingola.trasmissioneUtente = TrasmManager.addTrasmissioneUtente(trasmissioneSingola.trasmissioneUtente, trasmissioneUtente);
                    trasmissioneSingola.trasmissioneUtente.Add(trasmissioneUtente);
                }
            }

            if (corr is DocsPaVO.utente.Utente)
            {
                trasmissioneSingola.tipoDest = DocsPaVO.trasmissione.TipoDestinatario.UTENTE;
                DocsPaVO.trasmissione.TrasmissioneUtente trasmissioneUtente = new DocsPaVO.trasmissione.TrasmissioneUtente();
                trasmissioneUtente.utente = (DocsPaVO.utente.Utente)corr;
                //trasmissioneSingola.trasmissioneUtente = TrasmManager.addTrasmissioneUtente(trasmissioneSingola.trasmissioneUtente, trasmissioneUtente);
                trasmissioneSingola.trasmissioneUtente.Add(trasmissioneUtente);
            }

            if (corr is DocsPaVO.utente.UnitaOrganizzativa)
            {
                DocsPaVO.utente.UnitaOrganizzativa theUo = (DocsPaVO.utente.UnitaOrganizzativa)corr;
                DocsPaVO.addressbook.QueryCorrispondenteAutorizzato qca = new DocsPaVO.addressbook.QueryCorrispondenteAutorizzato();
                qca.ragione = trasmissioneSingola.ragione;
                //qca.ruolo = UserManager.getRuolo();
                qca.ruolo = trasmissione.ruolo;
                qca.queryCorrispondente = new DocsPaVO.addressbook.QueryCorrispondente();
                qca.queryCorrispondente.fineValidita = true;

                //DocsPaVO.utente.Ruolo[] ruoli = Utenti.addressBookManager.getRuoliRiferimentoAutorizzati(qca, theUo);
                ArrayList ruoli = Utenti.addressBookManager.getRuoliRiferimentoAutorizzati(qca, theUo);
                //Andrea
                if (ruoli == null || ruoli.Count == 0)
                {
                    //Popola una lista con tutti i destinatari non raggiungibili
                    trasmissione.listaDestinatariNonRaggiungibili.Add("Manca un ruolo di riferimento per la UO: " + corr.codiceCorrispondente + " (" + corr.descrizione + ") " + ".");
                }
                //End Andrea
                if (ruoli != null)
                {
                    foreach (DocsPaVO.utente.Ruolo r in ruoli)
                        trasmissione = addTrasmissioneSingola(trasmissione, r, ragione, note, tipoTrasm);
                }

                return trasmissione;
            }

            if (trasmissioneSingola != null)
                //trasmissione.trasmissioniSingole = TrasmManager.addTrasmissioneSingola(trasmissione.trasmissioniSingole, trasmissioneSingola);
                trasmissione.trasmissioniSingole.Add(trasmissioneSingola);
            return trasmissione;
        }

        public static ArrayList queryUtenti(DocsPaVO.utente.Corrispondente corr)
        {

            //costruzione oggetto queryCorrispondente
            DocsPaVO.addressbook.QueryCorrispondente qco = new DocsPaVO.addressbook.QueryCorrispondente();

            qco.codiceRubrica = corr.codiceRubrica;
            qco.getChildren = true;

            //qco.idAmministrazione = UserManager.getInfoUtente(this).idAmministrazione;
            qco.idAmministrazione = corr.idAmministrazione;

            //corrispondenti interni
            qco.tipoUtente = DocsPaVO.addressbook.TipoUtente.INTERNO;
            qco.fineValidita = true;

            //return Utenti.addressBookManager.getListaCorrispondenti(qco);
            return Utenti.addressBookManager.getListaCorrispondenti(qco);
        }

        /// <summary>
        /// Metodo utilizzato per verificare se la to do list di un ruolo contiene trasmissioni rivolte a ruolo
        /// pendenti
        /// </summary>
        /// <param name="roleIdCorrGlob">Id corr globali del ruolo da verificare</param>
        /// <returns>True se ci sono trasmissioni a ruolo pendenti</returns>
        public static bool CheckIfUserHaveRoleTransmission(String roleIdCorrGlob)
        {
            DocsPaDB.Query_DocsPAWS.Trasmissione trasm = new DocsPaDB.Query_DocsPAWS.Trasmissione();
            return trasm.CheckIfUserHaveRoleTransmission(roleIdCorrGlob);

        }

        public static DocsPaVO.amministrazione.EsitoOperazione AccettazioneMassivaInTdl_SP(string idGruppo, string idCorrBlobaliRuolo, string note)
        {
            DocsPaVO.amministrazione.EsitoOperazione retVal = new DocsPaVO.amministrazione.EsitoOperazione();
            bool result = false;
            string message = string.Empty;

            DocsPaDB.Query_DocsPAWS.Trasmissione trasm = new DocsPaDB.Query_DocsPAWS.Trasmissione();
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                result = trasm.AccettazioneMassivaInTdl_SP(idGruppo, idCorrBlobaliRuolo, note);
                if (result)
                    transactionContext.Complete();
                else
                    transactionContext.Dispose();
            }

            // Costruzione e restituzione dell'esito
            retVal.Codice = result ? 0 : -1;
            retVal.Descrizione = message.ToString();

            return retVal;
        }

        /// <summary>
        /// Metodo per l'accettazione massiva di trasmissioni presenti nella to do list
        /// di uno specifico utente
        /// </summary>
        /// <param name="user">Utente di cui analizzare la to do list</param>
        /// <param name="role">Ruolo</param>
        /// <param name="notes">Note generali da impostare</param>
        /// <returns>Esito dell'operazione</returns>
        public static DocsPaVO.amministrazione.EsitoOperazione AccettazioneMassivaInTdl(DocsPaVO.utente.Utente user, DocsPaVO.utente.Ruolo role, String notes, IBaseRubricaComuneService rubricaComuneService)
        {
            // Recupero della to do list dell'utente
            DocsPaVO.trasmissione.infoToDoList[] userToDoList = (DocsPaVO.trasmissione.infoToDoList[])(new DocsPaDB.Query_DocsPAWS.Trasmissione().getMyNewTodoListMigrazione(user.systemId, role.idGruppo).ToArray(typeof(DocsPaVO.trasmissione.infoToDoList)));

            if (userToDoList == null)
                userToDoList = new DocsPaVO.trasmissione.infoToDoList[0];

            // Recupero dei dettagli delle trasmissioni in to do list
            List<DocsPaVO.trasmissione.Trasmissione> trasmsDetails = GetTrasmDetails(userToDoList, role, user);

            // Oggetto da restituire all'utente
            DocsPaVO.amministrazione.EsitoOperazione retVal = new DocsPaVO.amministrazione.EsitoOperazione();

            // Messaggio da restituire all'utente
            StringBuilder message = new StringBuilder();
            String msg;

            // Risultato dell'elaborazione
            bool result = true;

            // Analisi delle trasmissioni
            foreach (var trasm in trasmsDetails)
            {
                // Prelevamento di tutte le trasmissioni singole dirette a RUOLO
                IEnumerable<DocsPaVO.trasmissione.TrasmissioneSingola> roleTransmissions = ((DocsPaVO.trasmissione.TrasmissioneSingola[])trasm.trasmissioniSingole.ToArray(typeof(DocsPaVO.trasmissione.TrasmissioneSingola))).Where(e => e.tipoDest == DocsPaVO.trasmissione.TipoDestinatario.RUOLO);

                // Analisi delle trasmissioni singole che compongono la trasmissione
                foreach (DocsPaVO.trasmissione.TrasmissioneSingola singleTrasm in roleTransmissions)
                {
                    DocsPaDB.Query_DocsPAWS.Trasmissione tManager = new DocsPaDB.Query_DocsPAWS.Trasmissione();

                    // Se la ragione di trasmissione prevede accettazione (Workflow), viene verificato di che tipo di trasmissione
                    // si tratta (Singola o Tutti) e si procede con le accettazioni
                    if (singleTrasm.ragione.tipo.ToUpper().Equals("W"))
                    {
                        switch (singleTrasm.tipoTrasm.ToUpper())
                        {
                            case "S":   // Singola
                                // Se la trasmissione singola non è già stata accettata, si procede con l'accettazione
                                if (tManager.checkTrasm_UNO_AccettataRifiutata(singleTrasm.systemId))
                                {
                                    DocsPaVO.trasmissione.TrasmissioneUtente userTrasm = singleTrasm.trasmissioneUtente[0] as DocsPaVO.trasmissione.TrasmissioneUtente;
                                    userTrasm.noteAccettazione = notes;
                                    trasm.utente.idAmministrazione = user.idAmministrazione;
                                    result &= AcceptTransmission(userTrasm, role, user, trasm, rubricaComuneService, out msg);
                                    if (!String.IsNullOrEmpty(msg))
                                        message.AppendLine(msg);
                                }
                                break;
                            case "T":   // Tutti
                                // Se la trasmissione non è già stata accettata, si procede con l'accettazione
                                if (tManager.checkTrasm_TUTTI_AccettataRifiutata(singleTrasm.systemId))
                                    foreach (DocsPaVO.trasmissione.TrasmissioneUtente tu in singleTrasm.trasmissioneUtente)
                                    {
                                        tu.noteAccettazione = notes;
                                        result &= AcceptTransmission(tu, role, user, trasm, rubricaComuneService, out msg);
                                        if (!String.IsNullOrEmpty(msg))
                                            message.AppendLine(msg);

                                    }
                                break;
                        }

                    }

                    // Impostazione della data vista
                    if (trasm.tipoOggetto.ToString().Equals("DOCUMENTO"))
                        //new DocsPaDB.Query_DocsPAWS.Documenti().SetDataVistaSP_TASTOVISTO(user.systemId, trasm.infoDocumento.idProfile);
                        new DocsPaDB.Query_DocsPAWS.Documenti().SetDataVistaSP_TASTOVISTO(BusinessLogic.Utenti.UserManager.GetInfoUtente(user, role), trasm.infoDocumento.idProfile, "D");
                    else
                        //new DocsPaDB.Query_DocsPAWS.Fascicoli().SetDataVista(user.systemId, trasm.infoFascicolo.idFascicolo);
                        new DocsPaDB.Query_DocsPAWS.Documenti().SetDataVistaSP_TASTOVISTO(BusinessLogic.Utenti.UserManager.GetInfoUtente(user, role), trasm.infoFascicolo.idFascicolo, "F");
                }

            }

            // Costruzione e restituzione dell'esito
            retVal.Codice = result ? 0 : -1;
            retVal.Descrizione = message.ToString();

            return retVal;

        }

        /// <summary>
        /// Metodo per il reperimento dei dettagli delle trasmissioni relative ad un utente
        /// </summary>
        /// <param name="userTodoList">To do list dell'utente</param>
        /// <param name="role">Ruolo per cui ricercare le trasmissioni</param>
        /// <param name="user">Informazioni sull'utente di cui ricercare le trasmissioni da accettare</param>
        /// <returns>Lista con i dettagli delle trasmissioni pendenti</returns>
        private static List<DocsPaVO.trasmissione.Trasmissione> GetTrasmDetails(DocsPaVO.trasmissione.infoToDoList[] userTodoList, DocsPaVO.utente.Ruolo role, DocsPaVO.utente.Utente user)
        {
            // Lista delle trasmissioni
            List<DocsPaVO.trasmissione.Trasmissione> trasms = new List<DocsPaVO.trasmissione.Trasmissione>();

            // Oggetto della trasmissione
            DocsPaVO.trasmissione.OggettoTrasm trasmObj = new DocsPaVO.trasmissione.OggettoTrasm();

            // Per ogni oggetto nella to do list
            foreach (var toDo in userTodoList)
            {
                // Se la trasmissione si riferisce ad un documento, viene recuperata l'info documento altrimenti l'info fascicolo
                if (!String.IsNullOrEmpty(toDo.sysIdDoc))
                    using (DocsPaDB.Query_DocsPAWS.Documenti dbDoc = new DocsPaDB.Query_DocsPAWS.Documenti())
                        trasmObj.infoDocumento = dbDoc.GetInfoDocumento(role.idGruppo, user.idPeople, toDo.sysIdDoc, false);
                else
                    trasmObj.infoFascicolo = new DocsPaVO.fascicolazione.InfoFascicolo { idFascicolo = toDo.sysIdFasc };

                // Aggiunta del dettaglio della trasmissione e alla lista delle trasmissioni
                trasms.AddRange((DocsPaVO.trasmissione.Trasmissione[])QueryTrasmManager.getQueryDettaglioTrasmMethod(trasmObj, user, role, null, toDo.sysIdTrasm).ToArray(typeof(DocsPaVO.trasmissione.Trasmissione)));
            }

            // Restituzione della lista delle trasmissioni
            return trasms;
        }

        /// <summary>
        /// Metodo per l'accettazione di una trasmissione e per l'impostazione della data vista
        /// </summary>
        /// <param name="userTrasm">Trasmissone utente da accettare</param>
        /// <param name="role">Ruolo con cui accettare</param>
        /// <param name="userInfo">Informazioni sull'utente che deve compiere l'accettazione</param>
        /// <param name="trasm">Tramsissione da accettare</param>
        /// <param name="errorMessage">Eventuale messaggio di errore</param>
        /// <returns>True se non ci sono stati errori bloccanti nell'effettuazione dell'accettazione</returns>
        private static bool AcceptTransmission(DocsPaVO.trasmissione.TrasmissioneUtente userTrasm, DocsPaVO.utente.Ruolo role, DocsPaVO.utente.Utente user, DocsPaVO.trasmissione.Trasmissione trasm,IBaseRubricaComuneService rubricaComuneService, out String errorMessage)
        {
            string mode;
            string idObj;
            string msg;
            const string ACCETTATADOC = "ACCETTATA_DOCUMENTO";
            const string ACCETTATAFASC = "ACCETTATA_FASCICOLO";
            const string RIFIUTATADOC = "RIFIUTATA_DOCUMENTO";
            const string RIFIUTATAFASC = "RIFIUTATA_FASCICOLO";

            // Costruzione dell'info utente
            DocsPaVO.utente.InfoUtente userInfo = BusinessLogic.Utenti.UserManager.GetInfoUtente(user, role);

            // Accettazione, se non è già stata accettata
            bool retVal = true;
            retVal = ExecTrasmManager.executeAccRifMethod(userTrasm, trasm.systemId, role, userInfo, rubricaComuneService, out errorMessage, out mode, out idObj);
            if (retVal && !string.IsNullOrEmpty(mode))
            {
                DocsPaVO.trasmissione.TrasmissioneSingola sing = BusinessLogic.Trasmissioni.ExecTrasmManager.getTrasmSingola(userTrasm);
                switch (mode)
                {
                    case ACCETTATADOC:
                        msg = "Accettazione della trasmissione. Id documento: " + idObj;
                        BusinessLogic.UserLog.UserLog.WriteLog(user.userId, user.idPeople, role.idGruppo, user.idAmministrazione, "ACCEPTTRASMDOCUMENT", idObj, msg, DocsPaVO.Logger.CodAzione.Esito.OK, null, "1", sing.systemId);
                        break;
                    case ACCETTATAFASC:
                        msg = "Accettazione della trasmissione. Id fascicolo: " + idObj;
                        BusinessLogic.UserLog.UserLog.WriteLog(user.userId, user.idPeople, role.idGruppo, user.idAmministrazione, "ACCEPTTRASMFOLDER", idObj, msg, DocsPaVO.Logger.CodAzione.Esito.OK, null, "1", sing.systemId);
                        break;
                    case RIFIUTATAFASC:
                        msg = "Rifiuto della trasmissione. Id fascicolo: " + idObj;
                        BusinessLogic.UserLog.UserLog.WriteLog(user.userId, user.idPeople, role.idGruppo, user.idAmministrazione, "REJECTTRASMFOLDER", idObj, msg, DocsPaVO.Logger.CodAzione.Esito.OK, null, "1", sing.systemId);
                        break;
                    case RIFIUTATADOC:
                        msg = "Rifiuto della trasmissione. Id documento: " + idObj;
                        BusinessLogic.UserLog.UserLog.WriteLog(user.userId, user.idPeople, role.idGruppo, user.idAmministrazione, "REJECTTRASMDOCUMENT", idObj, msg, DocsPaVO.Logger.CodAzione.Esito.OK, null, "1", sing.systemId);
                        break;
                }
            }

            return retVal;

        }

        public static bool checkTrasm_UNO_TUTTI_AccettataRifiutata(DocsPaVO.trasmissione.TrasmissioneSingola trasmSingola)
        {
            bool retValue = true;

            if (trasmSingola.tipoDest == DocsPaVO.trasmissione.TipoDestinatario.RUOLO)
            {
                DocsPaDB.Query_DocsPAWS.Trasmissione queryObj = new DocsPaDB.Query_DocsPAWS.Trasmissione();
                switch (trasmSingola.tipoTrasm)
                {
                    case "S":
                        retValue = queryObj.checkTrasm_UNO_AccettataRifiutata(trasmSingola.systemId);
                        break;

                    case "T":
                        retValue = queryObj.checkTrasm_TUTTI_AccettataRifiutata(trasmSingola.systemId);
                        break;
                }
            }

            return retValue;
        }

        /// <summary>
        /// Crea un oggetto trasmissione tramite la system_id di una trasmissione e system_id trasmissione utente
        /// </summary>
        /// <param name="idTrasmissione">system_id della trasmissione e system_id trasmissione utente</param>
        /// <returns></returns>
        public static DocsPaVO.trasmissione.Trasmissione CreateObjTrasmissioneByIDLite(string idTrasmissione, string idTrasmissioneUtente)
        {
            logger.Debug("INIZIO : crea oggetto TRASMISSIONE_LITE");
            DocsPaDB.Query_DocsPAWS.Trasmissione queryTrasm = new DocsPaDB.Query_DocsPAWS.Trasmissione();
            DocsPaVO.trasmissione.Trasmissione newTrasmissione = new DocsPaVO.trasmissione.Trasmissione(); ;
            DocsPaVO.trasmissione.TrasmissioneSingola newTrasSingola;
            DocsPaVO.trasmissione.RagioneTrasmissione newRagioneTrasm;
            DocsPaVO.utente.Utente newUser = new DocsPaVO.utente.Utente();
            DocsPaVO.utente.Ruolo newRole = new DocsPaVO.utente.Ruolo();
            DocsPaVO.utente.Ruolo newRuolo = new DocsPaVO.utente.Ruolo();
            DocsPaVO.utente.Utente newUtente = new DocsPaVO.utente.Utente();
            DocsPaVO.trasmissione.TrasmissioneUtente newTrasmUtente = new DocsPaVO.trasmissione.TrasmissioneUtente();
            DocsPaDB.Query_DocsPAWS.Utenti queryUt = new DocsPaDB.Query_DocsPAWS.Utenti();
            DocsPaDB.Query_DocsPAWS.Documenti queryDoc = new DocsPaDB.Query_DocsPAWS.Documenti();
            DocsPaDB.Query_DocsPAWS.Fascicoli queryFasc = new DocsPaDB.Query_DocsPAWS.Fascicoli();

            DataSet dsDati;
            DataSet dsDatiUtente;

            dsDati = queryTrasm.GetDatiTrasmissioneByIDLite(idTrasmissione, idTrasmissioneUtente);

            if (dsDati.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow rowTX in dsDati.Tables[0].Rows)
                {
                    newTrasmissione.systemId = idTrasmissione;
                    if (rowTX["cha_tipo_oggetto"].ToString().Equals("D"))
                    {
                        newTrasmissione.tipoOggetto = DocsPaVO.trasmissione.TipoOggetto.DOCUMENTO;
                        DocsPaVO.documento.InfoDocumento infoDoc = queryDoc.GetInfoDocumento(null, null, rowTX["id_profile"].ToString(), false);
                        newTrasmissione.infoDocumento = infoDoc;
                    }
                    else
                    {
                        newTrasmissione.tipoOggetto = DocsPaVO.trasmissione.TipoOggetto.FASCICOLO;
                        DocsPaVO.fascicolazione.InfoFascicolo infoFasc = new DocsPaVO.fascicolazione.InfoFascicolo();
                        infoFasc.idFascicolo = rowTX["id_project"].ToString();
                        infoFasc.idClassificazione = queryFasc.GetProjectData("ID_PARENT", "WHERE SYSTEM_ID = " + rowTX["id_project"].ToString());
                        infoFasc.idRegistro = queryFasc.GetProjectData("ID_REGISTRO", "WHERE SYSTEM_ID = " + rowTX["id_project"].ToString());
                        infoFasc.privato = queryFasc.GetProjectData("CHA_PRIVATO", "WHERE SYSTEM_ID = " + rowTX["id_project"].ToString());
                        newTrasmissione.infoFascicolo = infoFasc;
                    }
                    newUser.idPeople = rowTX["id_people"].ToString();
                    if (rowTX["id_amm_people"] != DBNull.Value)
                        newUser.idAmministrazione = rowTX["id_amm_people"].ToString();
                    newTrasmissione.utente = newUser;
                    newRole.systemId = rowTX["id_ruolo_in_uo"].ToString();
                    newRole.idGruppo = rowTX["id_gruppo"].ToString();
                    newTrasmissione.ruolo = newRole;

                    newTrasSingola = new DocsPaVO.trasmissione.TrasmissioneSingola();

                    newTrasSingola.systemId = rowTX["system_id"].ToString();
                    newTrasSingola.tipoTrasm = rowTX["cha_tipo_trasm"].ToString();

                    if (rowTX["cha_tipo_dest"].ToString().Equals("R"))
                    {
                        newTrasSingola.tipoDest = DocsPaVO.trasmissione.TipoDestinatario.RUOLO;
                        newRuolo = new DocsPaVO.utente.Ruolo();
                        newRuolo = BusinessLogic.Utenti.UserManager.getRuoloEnabledAndDisabled(rowTX["id_corr_globale"].ToString());
                        newTrasSingola.corrispondenteInterno = newRuolo;
                    }
                    if (rowTX["cha_tipo_dest"].ToString().Equals("U"))
                    {
                        newTrasSingola.tipoDest = DocsPaVO.trasmissione.TipoDestinatario.UTENTE;
                        newUtente = new DocsPaVO.utente.Utente();
                        newUtente = BusinessLogic.Utenti.UserManager.getUtenteNoFiltroDisabled(queryUt.GetFromCorrGlobGeneric("ID_PEOPLE", "SYSTEM_ID = " + rowTX["id_corr_globale"].ToString()));
                        newTrasSingola.corrispondenteInterno = newUtente;
                    }

                    // verifica il settaggio del campo eredita in base alla scelta utente
                    string setEredita = rowTX["cha_set_eredita"].ToString();
                    if (string.IsNullOrEmpty(setEredita))
                    {
                        setEredita = "1";
                    }

                    // ragione di trasmissione
                    newRagioneTrasm = new DocsPaVO.trasmissione.RagioneTrasmissione();
                    newRagioneTrasm.tipo = rowTX["cha_tipo_ragione"].ToString();
                    newRagioneTrasm.systemId = rowTX["id_ragione"].ToString();
                    newRagioneTrasm.azioneRichiesta = rowTX["CHA_PROC_RES"].ToString();

                    if (setEredita == "1")
                    {
                        newRagioneTrasm.eredita = rowTX["cha_eredita"].ToString();
                    }
                    else
                    {
                        newRagioneTrasm.eredita = "0";
                    }

                    if (rowTX["cha_tipo_diritti"].ToString().Equals("W"))
                        newRagioneTrasm.tipoDiritti = DocsPaVO.trasmissione.TipoDiritto.WRITE;
                    if (rowTX["cha_tipo_diritti"].ToString().Equals("N"))
                        newRagioneTrasm.tipoDiritti = DocsPaVO.trasmissione.TipoDiritto.NONE;
                    if (rowTX["cha_tipo_diritti"].ToString().Equals("R"))
                        newRagioneTrasm.tipoDiritti = DocsPaVO.trasmissione.TipoDiritto.READ;
                    if (rowTX["cha_tipo_dest_rag"].ToString().Equals("I"))
                        newRagioneTrasm.tipoDestinatario = DocsPaVO.trasmissione.TipoGerarchia.INFERIORE;
                    if (rowTX["cha_tipo_dest_rag"].ToString().Equals("P"))
                        newRagioneTrasm.tipoDestinatario = DocsPaVO.trasmissione.TipoGerarchia.PARILIVELLO;
                    if (rowTX["cha_tipo_dest_rag"].ToString().Equals("S"))
                        newRagioneTrasm.tipoDestinatario = DocsPaVO.trasmissione.TipoGerarchia.SUPERIORE;
                    if (rowTX["cha_tipo_dest_rag"].ToString().Equals("T"))
                        newRagioneTrasm.tipoDestinatario = DocsPaVO.trasmissione.TipoGerarchia.TUTTI;
                    newTrasSingola.ragione = newRagioneTrasm;

                    // trasmissioni utente
                    dsDatiUtente = queryTrasm.GetTrasmissioniUtenteByIDTrasmSingola(newTrasSingola.systemId);
                    if (dsDatiUtente.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow rowUt in dsDatiUtente.Tables[0].Rows)
                        {
                            newTrasmUtente = new DocsPaVO.trasmissione.TrasmissioneUtente();

                            newTrasmUtente.systemId = rowUt["system_id"].ToString();
                            newTrasmUtente.dataAccettata = rowUt["dta_accettata"].ToString();
                            newTrasmUtente.dataRifiutata = rowUt["dta_rifiutata"].ToString();
                            newTrasmUtente.dataVista = rowUt["dta_vista"].ToString();
                            newTrasmUtente.dataRisposta = rowUt["dta_risposta"].ToString();
                            newTrasmUtente.noteAccettazione = rowUt["var_note_acc"].ToString();
                            newTrasmUtente.noteRifiuto = rowUt["var_note_rif"].ToString();
                            newTrasmUtente.valida = rowUt["cha_valida"].ToString();
                            newTrasmUtente.idTrasmRispSing = rowUt["id_trasm_risp_sing"].ToString();
                            newTrasmUtente.dataRimossaTDL = rowUt["dta_rimozione_todolist"].ToString();
                            newTrasmUtente.utente = queryUt.GetUtente(rowUt["id_people"].ToString());

                            newTrasSingola.trasmissioneUtente.Add(newTrasmUtente);
                        }
                    }

                    newTrasmissione.trasmissioniSingole.Add(newTrasSingola);
                }
                logger.Debug("FINE : crea oggetto TRASMISSIONE");

            }
            return newTrasmissione;
        }

        /// <summary>
        /// Funzione per l'invio di una trasmissione da modello tramite specifica del codice del modello
        /// </summary>
        /// <param name="userInfo">Le informazioni sull'utente</param>
        /// <param name="serverPath">Il path dell'applicazione WA</param>
        /// <param name="scheda">La scheda del documento da trasmettere</param>
        /// <param name="modelCode">Il codice del modello</param>
        /// <param name="role">Il ruolo dell'utente</param>
        /// <param name="trasmModel">Variabile di out, conterrà il modello recuperato</param>
        /// <returns>True se la trasmissione avviene con successo, false altrimenti.
        ///          Se il risultato è false provare a controllare il valore della variabile di out.
        ///          Se è null, significa che non è stato possibile recuperare il modello, altrimenti se
        ///          è valorizzato controllare il tipo di oggetto da trasmettere cui si applica il modello
        ///          ("D" = Documento, "F" = Fascicolo)
        /// </returns>
        public static bool TransmissionExecuteDocTransmFromModelCodeSoloConNotifica(DocsPaVO.utente.InfoUtente userInfo, string serverPath, DocsPaVO.documento.SchedaDocumento scheda, string modelCode, DocsPaVO.utente.Ruolo role, out DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione trasmModel, IBaseRubricaComuneService rubricaComuneService)
        {
            #region

            // L'indice dell'ultimo _
            int lastUnderscore = -1;

            // L'id del modello da recuperare
            string modelId = String.Empty;

            // Il modello recuperato
            DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione model = null;

            // L'oggetto trasmissione
            DocsPaVO.trasmissione.Trasmissione transmission;

            // Un oggetto ragione destinatario utilizzato durante la creazione delle
            // trasmissioni singole
            DocsPaVO.Modelli_Trasmissioni.RagioneDest ragDest;

            // L'array list dei destinatari
            ArrayList destinatari;

            // Le informazioni sul corrispondente cui inviare la trasmissione
            DocsPaVO.utente.Corrispondente corr;

            // La ragione di trasmissione
            DocsPaVO.trasmissione.RagioneTrasmissione ragione;

            // Il valore da restituire
            bool toReturn = false;

            #endregion

            try
            {
                // Individuazione dell'id del modello a partire dal codice
                lastUnderscore = modelCode.LastIndexOf('_');

                // Se è stato trovato un '_' si preleva l'id altrimenti l'id è pari al codice
                if (lastUnderscore != -1)
                    modelId = modelCode.Substring(lastUnderscore + 1);
                else
                    modelId = modelCode;

                // Richiesta del modello di trasmissione
                model = BusinessLogic.Trasmissioni.ModelliTrasmissioni.getModelloByIDSoloConNotifica(role.idAmministrazione, modelId);

                // Se è stato trovato un modello è tale modello si applica ai tocumenti,
                // si procede con la trasmissione
                if (model != null && model.CHA_TIPO_OGGETTO == "D")
                {
                    // Creazione dell'oggetto trasmissione
                    transmission = new DocsPaVO.trasmissione.Trasmissione();

                    // Impostazione dei parametri della trasmissione
                    // Note generali
                    transmission.noteGenerali = model.VAR_NOTE_GENERALI;

                    // Tipo oggetto
                    transmission.tipoOggetto = DocsPaVO.trasmissione.TipoOggetto.DOCUMENTO;

                    // Informazioni sul documento
                    transmission.infoDocumento = BusinessLogic.Documenti.DocManager.getInfoDocumento(scheda);

                    // Utente mittente della trasmissione
                    transmission.utente = BusinessLogic.Utenti.UserManager.getUtente(userInfo.idPeople);

                    // Ruolo mittente
                    transmission.ruolo = BusinessLogic.Utenti.UserManager.getRuolo(userInfo.idCorrGlobali);

                    transmission.NO_NOTIFY = model.NO_NOTIFY;

                    string generaNotifica = "1";

                    if (!string.IsNullOrEmpty(model.NO_NOTIFY) && model.NO_NOTIFY.Equals("1"))
                    {
                        generaNotifica = "0";
                    }

                    // Impostazione dei parametri per le trasmissioni singole
                    for (int i = 0; i < model.RAGIONI_DESTINATARI.Count; i++)
                    {
                        // Recupero delli ragioni di trasmissione
                        ragDest = (DocsPaVO.Modelli_Trasmissioni.RagioneDest)model.RAGIONI_DESTINATARI[i];

                        // Creazione della lista dei destinatari
                        destinatari = new ArrayList(ragDest.DESTINATARI);

                        // Per ogni destinatario...
                        foreach (DocsPaVO.Modelli_Trasmissioni.MittDest mittDest in destinatari)
                        {
                            // ...prelevamento dei dati sul corrispondente
                            corr = BusinessLogic.Utenti.UserManager.getCorrispondenteByCodRubrica(
                                mittDest.VAR_COD_RUBRICA, userInfo, rubricaComuneService);

                            // ...prelevamento della ragione di trasmissione
                            ragione = BusinessLogic.Trasmissioni.QueryTrasmManager.getRagioneById(
                                mittDest.ID_RAGIONE.ToString());

                            // ...aggiunta della trasmissione singola
                            transmission = BusinessLogic.Trasmissioni.TrasmManager.addTrasmissioneSingolaSoloConNotifica(
                                transmission,
                                corr,
                                ragione,
                                mittDest.VAR_NOTE_SING,
                                mittDest.CHA_TIPO_TRASM,
                                mittDest);
                        }
                    }

                    // Se l'utente è un delegato...
                    if (userInfo.delegato != null)
                        // ...impostazione dell'idPeople del delegato
                        transmission.delegato = ((DocsPaVO.utente.InfoUtente)(userInfo.delegato)).idPeople;

                    // Trasmissione
                    transmission = BusinessLogic.Trasmissioni.ExecTrasmManager.saveExecuteTrasmMethod(
                        serverPath, transmission);
                    string desc = string.Empty;
                    if (transmission != null)
                    {
                        // LOG per documento
                        if (transmission.infoDocumento != null && !string.IsNullOrEmpty(transmission.infoDocumento.idProfile))
                        {
                            foreach (DocsPaVO.trasmissione.TrasmissioneSingola single in transmission.trasmissioniSingole)
                            {
                                string method = "TRASM_DOC_" + single.ragione.descrizione.ToUpper().Replace(" ", "_");
                                if (transmission.infoDocumento.segnatura == null)
                                    desc = "Trasmesso Documento : " + transmission.infoDocumento.docNumber.ToString();
                                else
                                    desc = "Trasmesso Documento : " + transmission.infoDocumento.segnatura.ToString();

                                BusinessLogic.UserLog.UserLog.WriteLog(transmission.utente.userId, transmission.utente.idPeople, transmission.ruolo.idGruppo, transmission.utente.idAmministrazione, method, transmission.infoDocumento.docNumber, desc, DocsPaVO.Logger.CodAzione.Esito.OK, userInfo.delegato, generaNotifica, single.systemId);
                            }
                        }
                        // LOG per fascicolo
                        if (transmission.infoFascicolo != null && !string.IsNullOrEmpty(transmission.infoFascicolo.idFascicolo))
                        {
                            foreach (DocsPaVO.trasmissione.TrasmissioneSingola single in transmission.trasmissioniSingole)
                            {
                                string method = "TRASM_FOLDER_" + single.ragione.descrizione.ToUpper().Replace(" ", "_");
                                desc = "Trasmesso Fascicolo ID: " + transmission.infoFascicolo.idFascicolo.ToString();
                                BusinessLogic.UserLog.UserLog.WriteLog(transmission.utente.userId, transmission.utente.idPeople, transmission.ruolo.idGruppo, transmission.utente.idAmministrazione, method, transmission.infoFascicolo.idFascicolo, desc, DocsPaVO.Logger.CodAzione.Esito.OK, userInfo.delegato, generaNotifica, single.systemId);
                            }
                        }

                    }
                    // Il risultato da restituire è successo
                    toReturn = true;
                }
            }
            catch (Exception e)
            {
                // Il risulto è insuccesso
                toReturn = false;

            }

            // Impostazione del modello di trasmissione
            trasmModel = model;

            // Restituzione dell'esito
            return toReturn;
        }

        public static DocsPaVO.trasmissione.Trasmissione addTrasmissioneSingolaSoloConNotifica(DocsPaVO.trasmissione.Trasmissione trasmissione, DocsPaVO.utente.Corrispondente corr, DocsPaVO.trasmissione.RagioneTrasmissione ragione, string note, string tipoTrasm, DocsPaVO.Modelli_Trasmissioni.MittDest destinatari)
        {
            //Controllo se il ruolo è disabilitato alla ricezione delle trasmissioni
            if (corr != null && corr.tipoCorrispondente == "R")
            {
                DocsPaDB.Query_DocsPAWS.Utenti utentiDb = new DocsPaDB.Query_DocsPAWS.Utenti();
                DocsPaVO.utente.Corrispondente corrispondente = utentiDb.getRuoloById(corr.systemId);
                if (corrispondente != null && corrispondente.disabledTrasm)
                    return trasmissione;
            }

            if (trasmissione.trasmissioniSingole != null)
            {
                // controllo se esiste la trasmissione singola associata a corrispondente selezionato
                for (int i = 0; i < trasmissione.trasmissioniSingole.Count; i++)
                {
                    DocsPaVO.trasmissione.TrasmissioneSingola ts = (DocsPaVO.trasmissione.TrasmissioneSingola)trasmissione.trasmissioniSingole[i];
                    if (ts != null && ts.corrispondenteInterno.systemId != null && corr != null && ts.corrispondenteInterno.systemId.Equals(corr.systemId))
                    {
                        if (ts.daEliminare)
                        {
                            ((DocsPaVO.trasmissione.TrasmissioneSingola)trasmissione.trasmissioniSingole[i]).daEliminare = false;
                            return trasmissione;
                        }
                        else
                            return trasmissione;
                    }
                }
            }

            // Aggiungo la trasmissione singola
            DocsPaVO.trasmissione.TrasmissioneSingola trasmissioneSingola = new DocsPaVO.trasmissione.TrasmissioneSingola();
            trasmissioneSingola.tipoTrasm = tipoTrasm;
            trasmissioneSingola.corrispondenteInterno = corr;
            trasmissioneSingola.ragione = ragione;
            trasmissioneSingola.noteSingole = note;

            // Aggiungo la lista di trasmissioniUtente
            if (corr is DocsPaVO.utente.Ruolo)
            {
                trasmissioneSingola.tipoDest = DocsPaVO.trasmissione.TipoDestinatario.RUOLO;
                //DocsPaVO.utente.Corrispondente[] listaUtenti = queryUtenti(corr);
                ArrayList listaUtenti = queryUtenti(corr);
                //if (listaUtenti.Length == 0)
                if (destinatari.UTENTI_NOTIFICA.Count == 0)
                    trasmissioneSingola = null;

                //ciclo per utenti se dest è gruppo o ruolo
                //for (int i = 0; i < listaUtenti.Length; i++)
                // DocsPaVO.Modelli_Trasmissioni.MittDest mittDest in ragioneDest.DESTINATARI
                for (int i = 0; i < destinatari.UTENTI_NOTIFICA.Count; i++)
                {
                    DocsPaVO.trasmissione.TrasmissioneUtente trasmissioneUtente = new DocsPaVO.trasmissione.TrasmissioneUtente();
                    DocsPaVO.Modelli_Trasmissioni.UtentiConNotificaTrasm utTemp = new DocsPaVO.Modelli_Trasmissioni.UtentiConNotificaTrasm();
                    utTemp = (DocsPaVO.Modelli_Trasmissioni.UtentiConNotificaTrasm)destinatari.UTENTI_NOTIFICA[i];
                    if (listaUtenti != null && listaUtenti.Count > 0)
                    {
                        for (int j = 0; j < listaUtenti.Count; j++)
                        {
                            if (utTemp.ID_PEOPLE == ((DocsPaVO.utente.Utente)listaUtenti[j]).idPeople)
                                trasmissioneUtente.utente = (DocsPaVO.utente.Utente)listaUtenti[j];
                        }
                    }
                    else
                    {
                        // non riesce a prendere la notifica via mail
                        trasmissioneUtente.utente = Utenti.UserManager.getUtenteById(utTemp.ID_PEOPLE);
                    }
                    //trasmissioneSingola.trasmissioneUtente = TrasmManager.addTrasmissioneUtente(trasmissioneSingola.trasmissioneUtente, trasmissioneUtente);
                    trasmissioneSingola.trasmissioneUtente.Add(trasmissioneUtente);
                }
            }

            if (corr is DocsPaVO.utente.Utente)
            {
                trasmissioneSingola.tipoDest = DocsPaVO.trasmissione.TipoDestinatario.UTENTE;
                DocsPaVO.trasmissione.TrasmissioneUtente trasmissioneUtente = new DocsPaVO.trasmissione.TrasmissioneUtente();
                trasmissioneUtente.utente = (DocsPaVO.utente.Utente)corr;
                //trasmissioneSingola.trasmissioneUtente = TrasmManager.addTrasmissioneUtente(trasmissioneSingola.trasmissioneUtente, trasmissioneUtente);
                trasmissioneSingola.trasmissioneUtente.Add(trasmissioneUtente);
            }

            if (corr is DocsPaVO.utente.UnitaOrganizzativa)
            {
                DocsPaVO.utente.UnitaOrganizzativa theUo = (DocsPaVO.utente.UnitaOrganizzativa)corr;
                DocsPaVO.addressbook.QueryCorrispondenteAutorizzato qca = new DocsPaVO.addressbook.QueryCorrispondenteAutorizzato();
                qca.ragione = trasmissioneSingola.ragione;
                //qca.ruolo = UserManager.getRuolo();
                qca.ruolo = trasmissione.ruolo;

                //DocsPaVO.utente.Ruolo[] ruoli = Utenti.addressBookManager.getRuoliRiferimentoAutorizzati(qca, theUo);
                ArrayList ruoli = Utenti.addressBookManager.getRuoliRiferimentoAutorizzati(qca, theUo);
                foreach (DocsPaVO.utente.Ruolo r in ruoli)
                    trasmissione = addTrasmissioneSingola(trasmissione, r, ragione, note, tipoTrasm);

                return trasmissione;
            }

            if (trasmissioneSingola != null)
                //trasmissione.trasmissioniSingole = TrasmManager.addTrasmissioneSingola(trasmissione.trasmissioniSingole, trasmissioneSingola);
                trasmissione.trasmissioniSingole.Add(trasmissioneSingola);
            return trasmissione;
        }
    }
}
