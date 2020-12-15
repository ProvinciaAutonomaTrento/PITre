using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using DocsPaVO.LibroFirma;
using System.Data;

namespace BusinessLogic.Task
{
    public class TaskManager
    {
        private static ILog logger = LogManager.GetLogger(typeof(TaskManager));

        /// <summary>
        /// Metodo per la creazione del task
        /// </summary>
        /// <param name="task"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public static bool CreateTask(DocsPaVO.Task.Task task, DocsPaVO.utente.InfoUtente infoUtente)
        {
            logger.Debug("Creazione del TASK in BusinessLogic.Task.TaskManager  - metodo: CreateTask ");
            bool result = false;
            try
            {
                DocsPaDB.Query_DocsPAWS.Task t = new DocsPaDB.Query_DocsPAWS.Task();
                result = t.InsertTask(task, infoUtente);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in BusinessLogic.Task.TaskManager  - metodo: CreateTask ", e);
                result = false;
            }

            return result;
        }

        /// <summary>
        /// Metodo per la rimozione del TASK
        /// </summary>
        /// <param name="idTask"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public static bool RimuoviTask(string idTask, DocsPaVO.utente.InfoUtente infoUtente)
        {
            logger.Debug("Rimozione del TASK in BusinessLogic.Task.TaskManager  - metodo: RimuoviTask ");
            bool result = false;
            try
            {
                DocsPaDB.Query_DocsPAWS.Task t = new DocsPaDB.Query_DocsPAWS.Task();
                result = t.RimuoviTask(idTask, infoUtente);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in BusinessLogic.Task.TaskManager  - metodo: RimuoviTask ", e);
                result = false;
            }

            return result;
        }

        /// <summary>
        /// Chiude il task
        /// </summary>
        /// <param name="task"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public static bool ChiudiTask(DocsPaVO.Task.Task task, DocsPaVO.utente.InfoUtente infoUtente)
        {
            logger.Debug("Chiusura del TASK in BusinessLogic.Task.TaskManager  - metodo: ChiudiTask ");
            bool result = false;
            try
            {
                DocsPaDB.Query_DocsPAWS.Task t = new DocsPaDB.Query_DocsPAWS.Task();
                result = t.ChiudiTask(task.STATO_TASK.ID_STATO_TASK, infoUtente);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in BusinessLogic.Task.TaskManager  - metodo: ChiudiTask ", e);
                result = false;
            }

            return result;
        }


        /// <summary>
        /// Metodo per la chiusura del TASK
        /// </summary>
        /// <param name="idTask"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public static bool ChiudiLavorazioneTask(DocsPaVO.Task.Task task, string note, DocsPaVO.utente.InfoUtente infoUtente)
        {
            logger.Debug("Chiusura del TASK in BusinessLogic.Task.TaskManager  - metodo: ChiudiLavorazioneTask ");
            bool result = false;
            try
            {
                DocsPaDB.Query_DocsPAWS.Task t = new DocsPaDB.Query_DocsPAWS.Task();
                result = t.ChiudiLavorazioneTask(task.STATO_TASK.ID_STATO_TASK, task.STATO_TASK.NOTE_LAVORAZIONE, infoUtente);

                if (!result)
                {
                    throw new Exception("Errore nella chiusura della lavorazione del task ID=" + task.ID_TASK);
                }

                // Se è presente un documento in risposta aggiorno nella profile lo stato del task
                if (!string.IsNullOrEmpty(task.ID_PROFILE_REVIEW))
                {
                    result = t.UpdateTaskStatusInProfile(task.ID_PROFILE_REVIEW, DocsPaDB.Query_DocsPAWS.Task.TaskStatus.CLOSED);
                }

                //Aggiorno le note nella dpa_trasm_utente
                if (result)
                    result = t.UpdateNoteTrasmUtente(task.ID_TRASM_SINGOLA, note, infoUtente);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in BusinessLogic.Task.TaskManager  - metodo: CloseTask ", e);
                result = false;
            }

            return result;
        }

        /// <summary>
        /// Associa il nuovo documento creato al task di origine
        /// </summary>
        /// <param name="idTask"></param>
        /// <param name="docnumber"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public static bool AssociaContributoAlTask(DocsPaVO.Task.Task task, DocsPaVO.documento.SchedaDocumento schedaDoc, DocsPaVO.utente.InfoUtente infoUtente)
        {
            logger.Debug("Chiusura del TASK in BusinessLogic.Task.TaskManager  - metodo: AssociaContributoAlTask ");
            bool result = false;
            try
            {
                DocsPaDB.Query_DocsPAWS.Task t = new DocsPaDB.Query_DocsPAWS.Task();
                result = t.AssociaContributoAlTask(task.ID_TASK, schedaDoc.docNumber, infoUtente);

                // Aggiorno la profile
                result = t.UpdateTaskStatusInProfile(schedaDoc.docNumber, DocsPaDB.Query_DocsPAWS.Task.TaskStatus.IN_PROGRESS);

                //Dò visibilità al ruolo mittente del task.Se il contributo viene inserito nel fascicolo da cui è partito il task non è necessario
                if (result && !string.IsNullOrEmpty(task.ID_PROFILE))
                {
                    DocsPaVO.trasmissione.Trasmissione resultTrasm = ExecuteTransmission(schedaDoc, task.RUOLO_MITTENTE.idGruppo, task.UTENTE_MITTENTE.idPeople, infoUtente);

                    string desc = string.Empty;
                    string method = "TRASM_DOC_" + (resultTrasm.trasmissioniSingole[0] as DocsPaVO.trasmissione.TrasmissioneSingola).ragione.descrizione.ToUpper().Replace(" ", "_");
                    if (resultTrasm.infoDocumento.segnatura == null)
                        desc = "Trasmesso Documento : " + resultTrasm.infoDocumento.docNumber.ToString();
                    else
                        desc = "Trasmesso Documento : " + resultTrasm.infoDocumento.segnatura.ToString();
                    if (resultTrasm != null)
                    {
                        BusinessLogic.UserLog.UserLog.WriteLog(resultTrasm.utente.userId, resultTrasm.utente.idPeople, resultTrasm.ruolo.idGruppo, resultTrasm.utente.idAmministrazione, method, resultTrasm.infoDocumento.docNumber, desc, DocsPaVO.Logger.CodAzione.Esito.OK,
                            (infoUtente != null && infoUtente.delegato != null ? infoUtente.delegato : null), "0", (resultTrasm.trasmissioniSingole[0] as DocsPaVO.trasmissione.TrasmissioneSingola).systemId);

                    }
                    else
                    {
                        BusinessLogic.UserLog.UserLog.WriteLog(resultTrasm.utente.userId, resultTrasm.utente.idPeople, resultTrasm.ruolo.idGruppo, resultTrasm.utente.idAmministrazione, method, resultTrasm.infoDocumento.docNumber, desc, DocsPaVO.Logger.CodAzione.Esito.KO,
                               (infoUtente != null && infoUtente.delegato != null ? infoUtente.delegato : null), "0", (resultTrasm.trasmissioniSingole[0] as DocsPaVO.trasmissione.TrasmissioneSingola).systemId);
                    }
                }
            }
            catch (Exception e)
            {
                logger.Debug("Errore in BusinessLogic.Task.TaskManager  - metodo: AssociaContributoAlTask ", e);
                result = false;
            }

            return result;
        }


        private static DocsPaVO.trasmissione.Trasmissione ExecuteTransmission(DocsPaVO.documento.SchedaDocumento schedaDoc, string idGruppoDest, string idPeopleDest, DocsPaVO.utente.InfoUtente infoUtente)
        {
            DocsPaVO.trasmissione.Trasmissione trasm = new DocsPaVO.trasmissione.Trasmissione();

            DocsPaDB.Query_DocsPAWS.Utenti u = new DocsPaDB.Query_DocsPAWS.Utenti();

            trasm.ruolo = u.GetRuoloByIdGruppo(infoUtente.idGruppo);//istanzaProcesso.RuoloProponente;
            trasm.utente = u.getUtenteById(infoUtente.idPeople);//istanzaProcesso.UtenteProponente;
            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();

            trasm.infoDocumento = schedaDoc.InfoDocumento;

            //INSERISCO LA RAGIONE DI TRASMISSIONE DI SISTEMA PER LIBRO FIRMA
            DocsPaVO.trasmissione.RagioneTrasmissione ragione = getRagioneTrasm(infoUtente.idAmministrazione, "D");

            //CREO LA TRASMISSIONE SINGOLA
            DocsPaVO.trasmissione.TrasmissioneSingola trasmSing = new DocsPaVO.trasmissione.TrasmissioneSingola();
            trasmSing.ragione = ragione;
            trasmSing.tipoTrasm = "S";


            DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();
            DocsPaVO.utente.Ruolo ruolo = utenti.GetRuoloByIdGruppo(idGruppoDest);
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

            for (int k = 0; k < listaUtenti.Count; k++)
            {
                DocsPaVO.trasmissione.TrasmissioneUtente trUt = new DocsPaVO.trasmissione.TrasmissioneUtente();
                trUt.utente = (DocsPaVO.utente.Utente)listaUtenti[k];
                trUt.daNotificare = (listaUtenti[k] as DocsPaVO.utente.Utente).idPeople.Equals(idPeopleDest);
                trasmissioniUt.Add(trUt);
            }

            trasmSing.trasmissioneUtente = trasmissioniUt;
            trasm.trasmissioniSingole = new System.Collections.ArrayList() { trasmSing };
            return BusinessLogic.Trasmissioni.ExecTrasmManager.saveExecuteTrasmMethod("", trasm);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static DocsPaVO.trasmissione.RagioneTrasmissione getRagioneTrasm(string idAmm, String tipoRagione)
        {
            logger.Debug("getRagioneTrasm");
            //DocsPa_V15_Utils.Database db=DocsPa_V15_Utils.dbControl.getDatabase();
            //DataSet ds=new DataSet();
            DataSet ds;
            DocsPaVO.trasmissione.RagioneTrasmissione rt = new DocsPaVO.trasmissione.RagioneTrasmissione();
            try
            {

                DocsPaDB.Query_DocsPAWS.Task obj = new DocsPaDB.Query_DocsPAWS.Task();
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
                logger.Error("Errore nella gestione della visibilità del contributo. (getRagioneTrasm)", e);
                throw e;
            }
            return rt;
        }

        /// <summary>
        /// Restituisce la lista dei TASK ricevuti
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public static List<DocsPaVO.Task.Task> GetListaTaskRicevuti(bool incluteCompletedTasks, DocsPaVO.utente.InfoUtente infoUtente)
        {
            List<DocsPaVO.Task.Task> listaTask = new List<DocsPaVO.Task.Task>();
            try
            {
                DocsPaDB.Query_DocsPAWS.Task t = new DocsPaDB.Query_DocsPAWS.Task();
                listaTask = t.GetListaTask(incluteCompletedTasks, DocsPaVO.Task.TipoTask.RICEVUTI, infoUtente);
            }
            catch (Exception e)
            {
                logger.Error("Errore in BusinessLogic.Task.TaskManager  - metodo: GetListaTaskRicevuti ", e);
            }
            return listaTask;
        }

        /// <summary>
        /// Restituisce la lista dei TASK assegnati
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public static List<DocsPaVO.Task.Task> GetListaTaskAssegnati(DocsPaVO.utente.InfoUtente infoUtente)
        {
            List<DocsPaVO.Task.Task> listaTask = new List<DocsPaVO.Task.Task>();
            try
            {
                DocsPaDB.Query_DocsPAWS.Task t = new DocsPaDB.Query_DocsPAWS.Task();
                listaTask = t.GetListaTask(false, DocsPaVO.Task.TipoTask.ASSEGNATI, infoUtente);
            }
            catch (Exception e)
            {
                logger.Error("Errore in BusinessLogic.Task.TaskManager  - metodo: GetListaTaskAssegnati ", e);
            }
            return listaTask;
        }

        public static DocsPaVO.Task.Task GetTaskByTrasmSingola(string idTrasmSingola, DocsPaVO.utente.InfoUtente infoUtente)
        {
            DocsPaVO.Task.Task task = new DocsPaVO.Task.Task();
            try
            {
                DocsPaDB.Query_DocsPAWS.Task t = new DocsPaDB.Query_DocsPAWS.Task();
                task = t.GetTaskByTrasmSingola(idTrasmSingola, infoUtente);
            }
            catch (Exception e)
            {
                logger.Error("Errore in BusinessLogic.Task.TaskManager  - metodo: GetTaskByTrasmSingola ", e);
            }
            return task;
        }

        public static bool RiapriLavorazione(DocsPaVO.Task.Task task, DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.utente.Ruolo ruolo)
        {
            bool retVal = false;
            try
            {
                // Contesto transazionale
                using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
                {

                    if (!string.IsNullOrEmpty(task.ID_PROJECT) && !string.IsNullOrEmpty(task.ID_PROFILE_REVIEW))
                    {
                        //Inserisco il contributo creato nella precedente lavorazione all'interno di un sottofascicolo
                        string descNewFolder = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_NOME_SOTTOFASCICOLO");
                        DocsPaVO.fascicolazione.Fascicolo fascicolo = BusinessLogic.Fascicoli.FascicoloManager.getFascicoloById(task.ID_PROJECT, infoUtente);
                        DocsPaVO.fascicolazione.Folder folder = BusinessLogic.Fascicoli.FolderManager.getFolderByIdFascicolo(infoUtente.idPeople, infoUtente.idGruppo, fascicolo);
                        DocsPaVO.fascicolazione.Folder folderContributo = (from f in folder.childs.Cast<DocsPaVO.fascicolazione.Folder>() where f.descrizione.Equals(descNewFolder) select f).FirstOrDefault();
                        if (folderContributo == null)
                        {
                            DocsPaVO.fascicolazione.Folder newFolder = new DocsPaVO.fascicolazione.Folder();
                            newFolder.idFascicolo = folder.idFascicolo;
                            newFolder.idParent = folder.systemID;
                            newFolder.descrizione = descNewFolder;
                            DocsPaVO.fascicolazione.ResultCreazioneFolder result;
                            folderContributo = BusinessLogic.Fascicoli.FolderManager.newFolder(newFolder, infoUtente, ruolo, out result);
                        }
                        if (folderContributo != null && !string.IsNullOrEmpty(folderContributo.systemID))
                        {
                            bool isInFolder = BusinessLogic.Fascicoli.FascicoloManager.IsDocumentoInFolder(task.ID_PROFILE_REVIEW, folderContributo.systemID);
                            if (!isInFolder)
                            {
                                string msg;
                                //Rimuovo dal vecchio fascicolo
                                BusinessLogic.Fascicoli.FolderManager.RemoveDocumentFromFolder(infoUtente, task.ID_PROFILE_REVIEW, folder, "", out msg);
                                System.Collections.ArrayList listFolder = BusinessLogic.Fascicoli.FolderManager.GetFoldersDocument(task.ID_PROFILE_REVIEW, task.ID_PROJECT);
                                foreach (DocsPaVO.fascicolazione.Folder f in listFolder)
                                {
                                    BusinessLogic.Fascicoli.FolderManager.RemoveDocumentFromFolder(infoUtente, task.ID_PROFILE_REVIEW, f, "", out msg);
                                }
                                //Iserisco nel sottofascicolo indicato
                                retVal = BusinessLogic.Fascicoli.FolderManager.addDocFolder(infoUtente, task.ID_PROFILE_REVIEW, folderContributo.systemID, false, out msg);
                            }
                        }
                        if (retVal)
                        {
                            //Riapro la lavorazione del task
                            DocsPaDB.Query_DocsPAWS.Task t = new DocsPaDB.Query_DocsPAWS.Task();
                            retVal = t.RiapriLavorazioneTask(task);
                        }

                    }
                    else
                    {
                        //Riapro la lavorazione del task
                        DocsPaDB.Query_DocsPAWS.Task t = new DocsPaDB.Query_DocsPAWS.Task();
                        retVal = t.RiapriLavorazioneTask(task);
                    }
                    if (retVal)
                    {
                        transactionContext.Complete();
                    }
                    else
                    {
                        transactionContext.Dispose();
                    }
                }
            }
            catch (Exception e)
            {
                logger.Error("Errore in BusinessLogic.Task.TaskManager  - metodo: RiapriLavorazione ", e);
            }
            return retVal;
        }
    }
}
