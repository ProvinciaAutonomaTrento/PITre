using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Globalization;
using DocsPaVO.documento;
using DocsPaVO.utente;
using DocsPaVO.fascicolazione;
using System.Threading;
using System.Security.AccessControl;
using System.IO;

namespace BusinessLogic.Conservazione.Policy
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class PolicyManager
    {
        /// <summary>
        /// Creazione nuova policy
        /// </summary>
        /// <param name="policy"></param>
        /// <returns></returns>
        public static bool InsertNewPolicy(DocsPaVO.Conservazione.Policy policy)
        {
            bool result = false;

            DocsPaDB.Query_DocsPAWS.PolicyConservazione conservazione = new DocsPaDB.Query_DocsPAWS.PolicyConservazione();
            result = conservazione.InsertNewPolicy(policy);
            return result;
        }

        //Get lista policy documenti
        public static DocsPaVO.Conservazione.Policy[] GetListaPolicy(int idAmm, string tipo)
        {
            DocsPaVO.Conservazione.Policy[] result = null;

            DocsPaDB.Query_DocsPAWS.PolicyConservazione conservazione = new DocsPaDB.Query_DocsPAWS.PolicyConservazione();
            result = conservazione.GetListaPolicy(idAmm, tipo);

            return result;
        }

        //Cancella Policy
        public static bool DeletePolicy(string idPolicy)
        {
            bool result = false;

            DocsPaDB.Query_DocsPAWS.PolicyConservazione conservazione = new DocsPaDB.Query_DocsPAWS.PolicyConservazione();
            result = conservazione.DeletePolicy(idPolicy);
            return result;
        }

        //Svuota la cache delle Policy
        public static bool SvuotaCachePolicy(string idAmm, string tipo)
        {
            bool result = false;

            DocsPaDB.Query_DocsPAWS.PolicyConservazione conservazione = new DocsPaDB.Query_DocsPAWS.PolicyConservazione();
            result = conservazione.SvuotaCachePolicy(idAmm, tipo);
            return result;
        }

        //Get lista policy documenti
        public static DocsPaVO.Conservazione.Policy GetPolicyById(string idPolicy)
        {
            DocsPaVO.Conservazione.Policy result = null;

            DocsPaDB.Query_DocsPAWS.PolicyConservazione conservazione = new DocsPaDB.Query_DocsPAWS.PolicyConservazione();
            result = conservazione.GetPolicyById(idPolicy);

            return result;
        }

        /// <summary>
        /// Reperimento della policy con cui è stata eventualmente creata un'istanza di conservazione
        /// </summary>
        /// <param name="idIstanzaConservazione"></param>
        /// <returns></returns>
        public static DocsPaVO.Conservazione.Policy GetPolicyByIdIstanzaConservazione(string idIstanzaConservazione)
        {
            DocsPaDB.Query_DocsPAWS.PolicyConservazione conservazione = new DocsPaDB.Query_DocsPAWS.PolicyConservazione();

            return conservazione.GetPolicyByIdIstanzaConservazione(idIstanzaConservazione);
        }

        //Salva la periodicità della polici
        public static bool SavePeriodPolicy(DocsPaVO.Conservazione.Policy policy)
        {
            bool result = false;

            DocsPaDB.Query_DocsPAWS.PolicyConservazione conservazione = new DocsPaDB.Query_DocsPAWS.PolicyConservazione();
            result = conservazione.SavePeriodPolicy(policy);
            return result;
        }

        //Creazione nuova policy
        public static bool ModifyNewPolicy(DocsPaVO.Conservazione.Policy policy)
        {
            bool result = false;

            DocsPaDB.Query_DocsPAWS.PolicyConservazione conservazione = new DocsPaDB.Query_DocsPAWS.PolicyConservazione();
            result = conservazione.ModifyNewPolicy(policy);
            return result;
        }

        static object _PolicyLockObject = new object();
        public static void ExecutePolicy()
        {
            Mutex mutex = null;

            try
            {
                //Creazione o reperimento del mutex
                //mutex = GetOrCreateMutex("MUTEX");

                //if (mutex.WaitOne())
                //invece di usare il mutex proviamo a usare il lock, dato che non è system wide
                lock (_PolicyLockObject)
                {
                    // 1. Iteazione di tutte le amministrazioni dell'istanza
                    ArrayList listaAmm = BusinessLogic.Amministrazione.AmministraManager.GetAmministrazioni();
                    DocsPaDB.Query_DocsPAWS.PolicyConservazione conservazione = new DocsPaDB.Query_DocsPAWS.PolicyConservazione();

                    DocsPaVO.Logger.CodAzione.Esito logResponse = DocsPaVO.Logger.CodAzione.Esito.OK;

                    // 2. Per ogni amministrazione, reperisce le policy di conservazione attivate nella loro esecuzione temporale
                    foreach (DocsPaVO.utente.Amministrazione tempAmm in listaAmm)
                    {
                        DocsPaVO.Conservazione.Policy[] result = null;

                        //System.IO.File.AppendAllText("d:\\PolicyManager.txt", "Amm: " + tempAmm.codice + ";");

                        //3. Esegue le Policy per i documenti
                        result = conservazione.GetListaPolicy(Convert.ToInt32(tempAmm.systemId), "D");

                        //System.IO.File.AppendAllText("d:\\PolicyManager.txt", "Policy Documenti: " + result.Length.ToString() + ";");

                        if (result != null && result.Length > 0)
                        {
                            for (int i = 0; i < result.Length; i++)
                            {
                                DocsPaVO.Conservazione.ExecutionPolicy execution = conservazione.GetLastExecutionPolicyByIdPolicy(result[i].system_id);
                                if (CheckPeriod(result[i], execution))
                                {
                                    //Prende i documenti interessati
                                    InfoDocumento[] listaDoc = null;
                                    InfoUtente userInfo = new InfoUtente();
                                    DocsPaVO.Conservazione.ExecutionPolicy finalResult = new DocsPaVO.Conservazione.ExecutionPolicy();
                                    finalResult.startExecutionDate = DateTime.Now.ToString();
                                    DocsPaVO.utente.Utente utente = BusinessLogic.Utenti.UserManager.getUtente(result[i].codiceUtente, result[i].idAmministrazione);

                                    userInfo = new DocsPaVO.utente.InfoUtente(utente, BusinessLogic.Utenti.UserManager.getRuoloByIdGruppo(result[i].idGruppo));


                                    //GESTIONE UTENZA DI SISTEMA PER LA VERIFICA AUTOMATICA
                                    string userId = "UTENTECONSERVAZIONE";
                                    DocsPaVO.utente.Utente u = BusinessLogic.Utenti.UserManager.getUtente(userId, userInfo.idAmministrazione);

                                    // System.IO.File.AppendAllText("d:\\PolicyManager.txt", "UtenteConservazione: " + result.Length + ";");

                                    if (u == null || string.IsNullOrEmpty(u.systemId))
                                    {
                                        //System.IO.File.AppendAllText("d:\\PolicyManager.txt", "UtenteConservazione non trovato - uso utente : " + utente.userId + ";");
                                        u = utente;
                                        //userId = utente.userId;
                                    }
                                    DocsPaVO.utente.InfoUtente infoUtSistema = new DocsPaVO.utente.InfoUtente();
                                    infoUtSistema.idPeople = u.idPeople;
                                    infoUtSistema.idAmministrazione = u.idAmministrazione;
                                    infoUtSistema.codWorkingApplication = "CS";
                                    infoUtSistema.idGruppo = "0";
                                    infoUtSistema.idCorrGlobali = "0";
                                    infoUtSistema.userId = userId;

                                    DocsPaDB.Query_DocsPAWS.Documenti documenti = new DocsPaDB.Query_DocsPAWS.Documenti();
                                    string idLastId = "0";
                                    if (execution != null)
                                    {
                                        idLastId = execution.idLastDocumentProcessed;
                                    }

                                    listaDoc = documenti.GetListaDocumentiPolicyConservazione(result[i], idLastId);

                                    string firstDocumentId = "-1";
                                    string lastDocumentId = "-1";
                                    string idIstanza = "-1";
                                    string totDocProcessati = "0";
                                    if (listaDoc != null && listaDoc.Length > 0)
                                    {
                                        totDocProcessati = (listaDoc.Length).ToString();
                                        string stato = "N";
                                        if (result[i].statoInviato)
                                        {
                                            stato = "I";
                                        }
                                        idIstanza = BusinessLogic.Documenti.areaConservazioneManager.CreateIstanzaConservazione(userInfo, result[i].nome, string.Empty, result[i].system_id, stato, result[i].consolidazione, result[i].tipoConservazione);
                                        if (string.IsNullOrEmpty(idIstanza))
                                        {
                                            return;
                                        }
                                        if (stato == "I")
                                        {
                                            BusinessLogic.UserLog.UserLog.WriteLog(userInfo, "INVIO_ISTANZA", idIstanza, String.Format("Invio automatico in conservazione dell' istanza {0}", idIstanza), logResponse);
                                        }

                                        firstDocumentId = listaDoc[0].idProfile;

                                        //Crea una nuova istanza di conservazione

                                        for (int t = 0; t < listaDoc.Length; t++)
                                        {
                                            bool success = false;
                                            //Inserisci i documenti in conservazione
                                            // documenti.addAreaConservazione(listaDoc[t].idProfile, null, listaDoc[t].docNumber, userInfo, "D");
                                            lastDocumentId = listaDoc[t].idProfile;
                                            //if (result[i].consolidazione)
                                            //{
                                            //    if (!BusinessLogic.Documenti.DocumentConsolidation.IsDocumentConsoldated(userInfo, listaDoc[t].idProfile, DocumentConsolidationStateEnum.Step2))
                                            //    {
                                            //        BusinessLogic.Documenti.DocumentConsolidation.ConsolidateNoSecurity(userInfo, listaDoc[t].idProfile, DocumentConsolidationStateEnum.Step2);
                                            //    }

                                            //}

                                            try
                                            {
                                                BusinessLogic.Documenti.areaConservazioneManager.AddDocInAreaConservazioneWithPolicy(userInfo, idIstanza, listaDoc[t].idProfile, null, listaDoc[t].docNumber, "D", stato, getPolicyDocumentoValidato(result[i]));
                                                success = conservazione.InsertDettExecutionPolicy(result[i].system_id, listaDoc[t].idProfile, null, "1", null);
                                            }
                                            catch
                                            {
                                                success = conservazione.InsertDettExecutionPolicy(result[i].system_id, listaDoc[t].idProfile, null, "0", null);
                                            }
                                        }

                                    }

                                    finalResult.docNumberProcessed = totDocProcessati;
                                    finalResult.endExecutionDate = DateTime.Now.ToString();
                                    finalResult.idAmm = tempAmm.systemId;
                                    finalResult.idFirstDocumentProcessed = firstDocumentId;
                                    finalResult.idIstanza = idIstanza;
                                    finalResult.idLastDocumentProcessed = lastDocumentId;
                                    finalResult.idPolicy = result[i].system_id;
                                    conservazione.InsertExecutionPolicy(finalResult);
                                }
                            }
                        }

                        //4. Esegue le Policy per i fascicoli
                        result = conservazione.GetListaPolicy(Convert.ToInt32(tempAmm.systemId), "F");
                        if (result != null && result.Length > 0)
                        {
                            for (int i = 0; i < result.Length; i++)
                            {
                                bool trovato = false;
                                string firstFascId = "-1";
                                string lastFascId = "-1";
                                string idIstanza = "-1";
                                string totDocProcessati = "0";

                                DocsPaVO.Conservazione.ExecutionPolicy execution = conservazione.GetLastExecutionPolicyByIdPolicy(result[i].system_id);
                                if (CheckPeriod(result[i], execution))
                                {
                                    //Prende i fascicoli interessati
                                    Fascicolo[] listaFasc = null;
                                    InfoDocumento[] listaDoc = null;
                                    InfoUtente userInfo = new InfoUtente();
                                    DocsPaVO.Conservazione.ExecutionPolicy finalResult = new DocsPaVO.Conservazione.ExecutionPolicy();
                                    finalResult.startExecutionDate = DateTime.Now.ToString();

                                    DocsPaVO.utente.Utente utente = BusinessLogic.Utenti.UserManager.getUtente(result[i].codiceUtente, result[i].idAmministrazione);

                                    userInfo = new DocsPaVO.utente.InfoUtente(utente, BusinessLogic.Utenti.UserManager.getRuoloByIdGruppo(result[i].idGruppo));

                                    DocsPaDB.Query_DocsPAWS.Documenti documenti = new DocsPaDB.Query_DocsPAWS.Documenti();
                                    DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli();
                                    string idLastId = "0";
                                    if (execution != null)
                                    {
                                        idLastId = execution.idLastDocumentProcessed;
                                    }
                                    listaFasc = fascicoli.GetListaFascicoliPolicyConservazione(result[i], idLastId);

                                    if (listaFasc != null && listaFasc.Length > 0)
                                    {

                                        firstFascId = listaFasc[0].systemID;
                                        lastFascId = listaFasc[listaFasc.Length - 1].systemID;

                                        string stato = "N";
                                        totDocProcessati = (listaFasc.Length).ToString();
                                        for (int x = 0; x < listaFasc.Length; x++)
                                        {

                                            listaDoc = documenti.GetListaDocumentiInFascicoliPolicyConservazione(result[i], "0", listaFasc[x].systemID);
                                            if (listaDoc != null && listaDoc.Length > 0)
                                            {
                                                for (int t = 0; t < listaDoc.Length; t++)
                                                {
                                                    if (!trovato)
                                                    {
                                                        stato = "N";
                                                        if (result[i].statoInviato)
                                                        {
                                                            stato = "I";
                                                        }
                                                        idIstanza = BusinessLogic.Documenti.areaConservazioneManager.CreateIstanzaConservazione(userInfo, result[i].nome, string.Empty, result[i].system_id, stato, result[i].consolidazione, result[i].tipoConservazione);

                                                        if (string.IsNullOrEmpty(idIstanza))
                                                            return;
                                                        if (stato == "I")
                                                        {
                                                            BusinessLogic.UserLog.UserLog.WriteLog(userInfo, "INVIO_ISTANZA", idIstanza, String.Format("Ricezione dell' istanza {0}", idIstanza), logResponse);
                                                        }

                                                        trovato = true;
                                                    }
                                                    ////Inserisci i documenti in conservazione
                                                    //if (result[i].consolidazione)
                                                    //{
                                                    //    if (!BusinessLogic.Documenti.DocumentConsolidation.IsDocumentConsoldated(userInfo, listaDoc[t].idProfile, DocumentConsolidationStateEnum.Step2))
                                                    //    {
                                                    //        BusinessLogic.Documenti.DocumentConsolidation.ConsolidateNoSecurity(userInfo, listaDoc[t].idProfile, DocumentConsolidationStateEnum.Step2);
                                                    //    }

                                                    //}
                                                    try
                                                    {
                                                        BusinessLogic.Documenti.areaConservazioneManager.AddDocInAreaConservazioneWithPolicy(userInfo, idIstanza, listaDoc[t].idProfile, listaFasc[x].idClassificazione, listaDoc[t].docNumber, "F", stato, getPolicyDocumentoValidato(result[i]));
                                                        conservazione.InsertDettExecutionPolicy(result[i].system_id, listaDoc[t].idProfile, listaFasc[x].idClassificazione, "1", string.Empty);
                                                    }
                                                    catch
                                                    {
                                                        conservazione.InsertDettExecutionPolicy(result[i].system_id, listaDoc[t].idProfile, listaFasc[x].idClassificazione, "0", string.Empty);
                                                    }
                                                }

                                            }
                                        }

                                    }
                                    finalResult.docNumberProcessed = totDocProcessati;
                                    finalResult.endExecutionDate = DateTime.Now.ToString();
                                    finalResult.idAmm = tempAmm.systemId;
                                    finalResult.idFirstDocumentProcessed = firstFascId;
                                    finalResult.idIstanza = idIstanza;
                                    finalResult.idLastDocumentProcessed = lastFascId;
                                    finalResult.idPolicy = result[i].system_id;
                                    conservazione.InsertExecutionPolicy(finalResult);
                                }
                            }
                        }
                        //5. Esegue le Policy per le stampe registro
                        result = conservazione.GetListaPolicy(Convert.ToInt32(tempAmm.systemId), "R");
                        if (result != null && result.Length > 0)
                        {
                            for (int i = 0; i < result.Length; i++)
                            {
                                DocsPaVO.Conservazione.ExecutionPolicy execution = conservazione.GetLastExecutionPolicyByIdPolicy(result[i].system_id);
                                if (CheckPeriod(result[i], execution))
                                {
                                    //Prende i documenti interessati
                                    InfoDocumento[] listaDoc = null;
                                    InfoUtente userInfo = new InfoUtente();
                                    DocsPaVO.Conservazione.ExecutionPolicy finalResult = new DocsPaVO.Conservazione.ExecutionPolicy();
                                    finalResult.startExecutionDate = DateTime.Now.ToString();
                                    DocsPaVO.utente.Utente utente = BusinessLogic.Utenti.UserManager.getUtente(result[i].codiceUtente, result[i].idAmministrazione);

                                    userInfo = new DocsPaVO.utente.InfoUtente(utente, BusinessLogic.Utenti.UserManager.getRuoloByIdGruppo(result[i].idGruppo));

                                    DocsPaDB.Query_DocsPAWS.Documenti documenti = new DocsPaDB.Query_DocsPAWS.Documenti();
                                    string idLastId = "0";
                                    if (execution != null)
                                    {
                                        idLastId = execution.idLastDocumentProcessed;
                                    }

                                    listaDoc = documenti.GetListaStampePolicyConservazione(result[i], idLastId, "R");

                                    string firstDocumentId = "-1";
                                    string lastDocumentId = "-1";
                                    string idIstanza = "-1";
                                    string totDocProcessati = "0";
                                    if (listaDoc != null && listaDoc.Length > 0)
                                    {
                                        totDocProcessati = (listaDoc.Length).ToString();
                                        string stato = "N";
                                        if (result[i].statoInviato)
                                        {
                                            stato = "I";
                                        }
                                        idIstanza = BusinessLogic.Documenti.areaConservazioneManager.CreateIstanzaConservazione(userInfo, result[i].nome, string.Empty, result[i].system_id, stato, result[i].consolidazione, result[i].tipoConservazione);

                                        if (string.IsNullOrEmpty(idIstanza))
                                            return;
                                        if (stato == "I")
                                        {
                                            BusinessLogic.UserLog.UserLog.WriteLog(userInfo, "INVIO_ISTANZA", idIstanza, String.Format("Ricezione in conservazione dell' istanza {0}", idIstanza), logResponse);
                                        }

                                        firstDocumentId = listaDoc[0].idProfile;

                                        //Crea una nuova istanza di conservazione

                                        for (int t = 0; t < listaDoc.Length; t++)
                                        {
                                            bool success = false;
                                            lastDocumentId = listaDoc[t].idProfile;

                                            try
                                            {
                                                BusinessLogic.Documenti.areaConservazioneManager.AddDocInAreaConservazioneWithPolicy(userInfo, idIstanza, listaDoc[t].idProfile, null, listaDoc[t].docNumber, "D", stato, getPolicyDocumentoValidato(result[i]));
                                                success = conservazione.InsertDettExecutionPolicy(result[i].system_id, listaDoc[t].idProfile, null, "1", null);
                                            }
                                            catch
                                            {
                                                success = conservazione.InsertDettExecutionPolicy(result[i].system_id, listaDoc[t].idProfile, null, "0", null);
                                            }
                                        }

                                    }

                                    finalResult.docNumberProcessed = totDocProcessati;
                                    finalResult.endExecutionDate = DateTime.Now.ToString();
                                    finalResult.idAmm = tempAmm.systemId;
                                    finalResult.idFirstDocumentProcessed = firstDocumentId;
                                    finalResult.idIstanza = idIstanza;
                                    finalResult.idLastDocumentProcessed = lastDocumentId;
                                    finalResult.idPolicy = result[i].system_id;
                                    conservazione.InsertExecutionPolicy(finalResult);
                                }
                            }
                        }

                        //6. Esegue le Policy per le stampe repertori
                        result = conservazione.GetListaPolicy(Convert.ToInt32(tempAmm.systemId), "C");
                        if (result != null && result.Length > 0)
                        {
                            for (int i = 0; i < result.Length; i++)
                            {
                                DocsPaVO.Conservazione.ExecutionPolicy execution = conservazione.GetLastExecutionPolicyByIdPolicy(result[i].system_id);
                                if (CheckPeriod(result[i], execution))
                                {
                                    //Prende i documenti interessati
                                    InfoDocumento[] listaDoc = null;
                                    InfoUtente userInfo = new InfoUtente();
                                    DocsPaVO.Conservazione.ExecutionPolicy finalResult = new DocsPaVO.Conservazione.ExecutionPolicy();
                                    finalResult.startExecutionDate = DateTime.Now.ToString();
                                    DocsPaVO.utente.Utente utente = BusinessLogic.Utenti.UserManager.getUtente(result[i].codiceUtente, result[i].idAmministrazione);

                                    userInfo = new DocsPaVO.utente.InfoUtente(utente, BusinessLogic.Utenti.UserManager.getRuoloByIdGruppo(result[i].idGruppo));

                                    DocsPaDB.Query_DocsPAWS.Documenti documenti = new DocsPaDB.Query_DocsPAWS.Documenti();
                                    string idLastId = "0";
                                    if (execution != null)
                                    {
                                        idLastId = execution.idLastDocumentProcessed;
                                    }

                                    listaDoc = documenti.GetListaStampePolicyConservazione(result[i], idLastId, "C");

                                    string firstDocumentId = "-1";
                                    string lastDocumentId = "-1";
                                    string idIstanza = "-1";
                                    string totDocProcessati = "0";
                                    if (listaDoc != null && listaDoc.Length > 0)
                                    {
                                        totDocProcessati = (listaDoc.Length).ToString();
                                        string stato = "N";
                                        if (result[i].statoInviato)
                                        {
                                            stato = "I";
                                        }
                                        idIstanza = BusinessLogic.Documenti.areaConservazioneManager.CreateIstanzaConservazione(userInfo, result[i].nome, string.Empty, result[i].system_id, stato, result[i].consolidazione, result[i].tipoConservazione);

                                        if (string.IsNullOrEmpty(idIstanza))
                                            return;

                                        if (stato == "I")
                                        {
                                            BusinessLogic.UserLog.UserLog.WriteLog(userInfo, "INVIO_ISTANZA", idIstanza, String.Format("Ricezione in conservazione dell' istanza {0}", idIstanza), logResponse);
                                        }

                                        firstDocumentId = listaDoc[0].idProfile;

                                        //Crea una nuova istanza di conservazione

                                        for (int t = 0; t < listaDoc.Length; t++)
                                        {
                                            bool success = false;
                                            lastDocumentId = listaDoc[t].idProfile;

                                            try
                                            {
                                                BusinessLogic.Documenti.areaConservazioneManager.AddDocInAreaConservazioneWithPolicy(userInfo, idIstanza, listaDoc[t].idProfile, null, listaDoc[t].docNumber, "D", stato, getPolicyDocumentoValidato(result[i]));
                                                success = conservazione.InsertDettExecutionPolicy(result[i].system_id, listaDoc[t].idProfile, null, "1", null);
                                            }
                                            catch
                                            {
                                                success = conservazione.InsertDettExecutionPolicy(result[i].system_id, listaDoc[t].idProfile, null, "0", null);
                                            }
                                        }

                                    }

                                    finalResult.docNumberProcessed = totDocProcessati;
                                    finalResult.endExecutionDate = DateTime.Now.ToString();
                                    finalResult.idAmm = tempAmm.systemId;
                                    finalResult.idFirstDocumentProcessed = firstDocumentId;
                                    finalResult.idIstanza = idIstanza;
                                    finalResult.idLastDocumentProcessed = lastDocumentId;
                                    finalResult.idPolicy = result[i].system_id;
                                    conservazione.InsertExecutionPolicy(finalResult);
                                }
                            }
                        }
                    }
                    if (mutex != null)
                    {
                        mutex.ReleaseMutex();
                        mutex.Close();
                        mutex = null;
                    }
                }// fine lock o mutex
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (mutex != null)
                {
                    mutex.ReleaseMutex();
                    mutex.Close();
                    mutex = null;
                }
            }

        }

        #region MEV CS 1.5 - F03_01
        // Nuovo metodo per esecuzione policy con Vincoli su istanze
        public static void ExecutePolicy_WithConstraint(System.Web.HttpContext context)
        {
            Mutex mutex = null;

            try
            {
                //lista id istanze cui devono essere inviate automaticamente
                List<string> idIstanzeInvioAutomatico = new List<string>();
                //lista id istanze che devono essere convertite automaticamente nel caso 
                //in cui l'esito della verifica è "documenti direttamente o indirettamente convertibili"
                List<string> idIstanzeConvAutomatica = new List<string>();
                
                //Creazione o reperimento del mutex
                //mutex = GetOrCreateMutex("MUTEX");

                //if (mutex.WaitOne())
                //invece di usare il mutex proviamo a usare il lock, dato che non è system wide
                lock (_PolicyLockObject)
                {
                    // 1. Iteazione di tutte le amministrazioni dell'istanza
                    ArrayList listaAmm = BusinessLogic.Amministrazione.AmministraManager.GetAmministrazioni();
                    DocsPaDB.Query_DocsPAWS.PolicyConservazione conservazione = new DocsPaDB.Query_DocsPAWS.PolicyConservazione();

                    DocsPaVO.Logger.CodAzione.Esito logResponse = DocsPaVO.Logger.CodAzione.Esito.OK;

                    // 2. Per ogni amministrazione, reperisce le policy di conservazione attivate nella loro esecuzione temporale
                    foreach (DocsPaVO.utente.Amministrazione tempAmm in listaAmm)
                    {
                        DocsPaVO.Conservazione.Policy[] result = null;

                        //System.IO.File.AppendAllText("d:\\PolicyManager.txt", "Amm: " + tempAmm.codice + ";");

                        //3. Esegue le Policy per i documenti
                        #region Policy Documenti
                        result = conservazione.GetListaPolicy(Convert.ToInt32(tempAmm.systemId), "D");

                        //System.IO.File.AppendAllText("d:\\PolicyManager.txt", "Policy Documenti: " + result.Length.ToString() + ";");

                        if (result != null && result.Length > 0)
                        {
                            for (int i = 0; i < result.Length; i++)
                            {
                                DocsPaVO.Conservazione.ExecutionPolicy execution = conservazione.GetLastExecutionPolicyByIdPolicy(result[i].system_id);
                                if (CheckPeriod(result[i], execution))
                                {
                                    //Prende i documenti interessati
                                    InfoDocumento[] listaDoc = null;
                                    InfoUtente userInfo = new InfoUtente();
                                    //DocsPaVO.Conservazione.ExecutionPolicy finalResult = new DocsPaVO.Conservazione.ExecutionPolicy();
                                    //finalResult.startExecutionDate = DateTime.Now.ToString();
                                    DocsPaVO.utente.Utente utente = BusinessLogic.Utenti.UserManager.getUtente(result[i].codiceUtente, result[i].idAmministrazione);

                                    userInfo = new DocsPaVO.utente.InfoUtente(utente, BusinessLogic.Utenti.UserManager.getRuoloByIdGruppo(result[i].idGruppo));

                                    
                                    //GESTIONE UTENZA DI SISTEMA PER LA VERIFICA AUTOMATICA
                                    string userId = "UTENTECONSERVAZIONE";
                                    DocsPaVO.utente.Utente u = BusinessLogic.Utenti.UserManager.getUtente(userId, userInfo.idAmministrazione);

                                    // System.IO.File.AppendAllText("d:\\PolicyManager.txt", "UtenteConservazione: " + result.Length + ";");

                                    if (u == null || string.IsNullOrEmpty(u.systemId))
                                    {
                                        //System.IO.File.AppendAllText("d:\\PolicyManager.txt", "UtenteConservazione non trovato - uso utente : " + utente.userId + ";");
                                        u = utente;
                                        //userId = utente.userId;
                                    }
                                    DocsPaVO.utente.InfoUtente infoUtSistema = new DocsPaVO.utente.InfoUtente();
                                    infoUtSistema.idPeople = u.idPeople;
                                    infoUtSistema.idAmministrazione = u.idAmministrazione;
                                    infoUtSistema.codWorkingApplication = "CS";
                                    infoUtSistema.idGruppo = "0";
                                    infoUtSistema.idCorrGlobali = "0";
                                    infoUtSistema.userId = userId;

                                    DocsPaDB.Query_DocsPAWS.Documenti documenti = new DocsPaDB.Query_DocsPAWS.Documenti();
                                    string idLastId = "0";
                                    if (execution != null)
                                    {
                                        idLastId = execution.idLastDocumentProcessed;
                                    }

                                    // Lista documenti interessati
                                    listaDoc = documenti.GetListaDocumentiPolicyConservazione(result[i], idLastId);

                                    //
                                    // Lista di DocsPaVO.Conservazione.ExecutionPolicy
                                    List<DocsPaVO.Conservazione.ExecutionPolicy> listExecutionPolicy = new List<DocsPaVO.Conservazione.ExecutionPolicy>();

                                    // Lista di ID degli elementi da rimuovere nella listExecutionPolicy
                                    List<string> listIdToRemoveExecutionPolicy = new List<string>();

                                    if (listaDoc != null && listaDoc.Length > 0)
                                    {
                                        //lo stato è sempre N nuova poichè bisogna effettuare le verifiche dei 
                                        //formati 
                                        string stato = "N";
                                        //if (result[i].statoInviato)
                                        //{
                                        //    stato = "I";
                                        //}

                                        // Se posso creare istanza di conservazione la creo e faccio tutto
                                        bool vincoloPrimoOK = true;

                                        // Get Parametri validi per istanze di conservazione
                                        #region Mev CS 1.5 - F03_01 Get dei parametri
                                        //
                                        // Get Dimensione massima istanza in termini di Byte
                                        int dimMaxIstByte = BusinessLogic.Documenti.areaConservazioneManager.getDimensioneMassimaIstanze_Byte(result[i].idAmministrazione);
                                        //
                                        // Get numero massimo di doc in istanza
                                        int numDocMaxIst = BusinessLogic.Documenti.areaConservazioneManager.getDimensioneMassimaIstanze_NumDoc(result[i].idAmministrazione);
                                        //
                                        // Get percentuale di tolleranza in istanza
                                        int percentualeTolleranza = BusinessLogic.Documenti.areaConservazioneManager.getPercentualeTolleranzaDinesioneIstanze(result[i].idAmministrazione);

                                        #endregion

                                        // Verifico se posso inserire il primo elemento
                                        #region MEV CS 1.5 - F03_01 Controllo vincoli istanza per il primo elemento
                                        SchedaDocumento sd_primo = BusinessLogic.Documenti.DocManager.getDettaglioNoSecurity(userInfo, listaDoc[0].docNumber);
                                        int total_size_primo = 0;
                                        if (sd_primo != null)
                                        {
                                            int doc_size_primo = Convert.ToInt32(((DocsPaVO.documento.Documento)sd_primo.documenti[0]).fileSize);

                                            int numeroAllegati_primo = sd_primo.allegati.Count;
                                            string fileName_primo = ((DocsPaVO.documento.Documento)sd_primo.documenti[0]).fileName;
                                            string tipoFile_primo = Path.GetExtension(fileName_primo);
                                            int size_allegati_primo = 0;
                                            for (int j = 0; j < sd_primo.allegati.Count; j++)
                                            {
                                                size_allegati_primo = size_allegati_primo + Convert.ToInt32(((DocsPaVO.documento.Allegato)sd_primo.allegati[j]).fileSize);
                                            }
                                            total_size_primo = size_allegati_primo + doc_size_primo;
                                        }

                                        // Calcolo dimensioni prox nell'istanza
                                        int dimProx_primo = total_size_primo;
                                        int numdocProx_primo = 1;

                                        //
                                        // Invoco il metodo per la verifica della violazione del vincolo sul primo elemento
                                        bool vincoloDimIstViol_primo = BusinessLogic.Documenti.areaConservazioneManager.isVincoloDimensioneIstanzaViolato(dimProx_primo, dimMaxIstByte, percentualeTolleranza);
                                        bool vincoloNumDocIstViol_primo = BusinessLogic.Documenti.areaConservazioneManager.isVincoloNumeroDocumentiIstanzaViolato(numdocProx_primo, numDocMaxIst);
                                        #endregion

                                        // Se posso inserire il primo doc procedo, altrimenti non faccio nulla
                                        if (vincoloDimIstViol_primo || vincoloNumDocIstViol_primo)
                                            vincoloPrimoOK = false;

                                        // Vincolo per verificare se posso inserire almeno un documento
                                        if (vincoloPrimoOK)
                                        {
                                            // vincoli per dimensioni istanza
                                            bool vincoloDimIstViol = false;
                                            bool vincoloNumDocIstViol = false;

                                            string idIstanza = "-1";
                                            bool success = false;

                                            // creo prima istanza
                                            idIstanza = BusinessLogic.Documenti.areaConservazioneManager.CreateIstanzaConservazione(userInfo, result[i].nome, string.Empty, result[i].system_id, stato, result[i].consolidazione, result[i].tipoConservazione);

                                            #region Modifiche F02_01 controllo formati
                                            //controllo se la policy prevede l'invio e la conversione in automatico dell'istanza 
                                            if (!string.IsNullOrEmpty(idIstanza))
                                            {
                                                if (result[i].statoInviato)
                                                    idIstanzeInvioAutomatico.Add(idIstanza);

                                                if (result[i].statoConversione)
                                                    idIstanzeConvAutomatica.Add(idIstanza);

                                            }
                                            //if (stato == "I")
                                            //{
                                            //    BusinessLogic.UserLog.UserLog.getInstance().WriteLog(userInfo, "INVIO_ISTANZA", idIstanza, String.Format("Invio automatico in conservazione dell' istanza {0}", idIstanza), logResponse);
                                            //}
                                            #endregion

                                            //Crea una nuova istanza di conservazione
                                            string firstDocumentId = "-1";
                                            string lastDocumentId = "-1";

                                            string totDocProcessati = "0";
                                            // Contatore per il numero di documenti processati
                                            int countDocProc = 0;

                                            // Primo documento
                                            firstDocumentId = listaDoc[0].idProfile;

                                            // Oggetto finalResult
                                            DocsPaVO.Conservazione.ExecutionPolicy finalResult = new DocsPaVO.Conservazione.ExecutionPolicy();
                                            finalResult.startExecutionDate = DateTime.Now.ToString();

                                            bool isPrimaIstanza = true;
                                            DocsPaVO.Conservazione.ExecutionPolicy finalResultNew = null;

                                            for (int t = 0; t < listaDoc.Length; t++)
                                            {
                                                //
                                                // Controllo sempre se il prossimo elemento viola il vincolo delle istanze
                                                #region MEV CS 1.5 Controllo vincoli Prossimo elemento
                                                SchedaDocumento sd_prox = BusinessLogic.Documenti.DocManager.getDettaglioNoSecurity(userInfo, listaDoc[t].docNumber);
                                                int total_size_prox = 0;
                                                if (sd_prox != null)
                                                {
                                                    int doc_size_prox = Convert.ToInt32(((DocsPaVO.documento.Documento)sd_prox.documenti[0]).fileSize);

                                                    int numeroAllegati_prox = sd_prox.allegati.Count;
                                                    string fileName_prox = ((DocsPaVO.documento.Documento)sd_prox.documenti[0]).fileName;
                                                    string tipoFile_prox = Path.GetExtension(fileName_prox);
                                                    int size_allegati_prox = 0;
                                                    for (int j = 0; j < sd_prox.allegati.Count; j++)
                                                    {
                                                        size_allegati_prox = size_allegati_prox + Convert.ToInt32(((DocsPaVO.documento.Allegato)sd_prox.allegati[j]).fileSize);
                                                    }
                                                    total_size_prox = size_allegati_prox + doc_size_prox;
                                                }

                                                //
                                                // Get sizeItem e numDoc in Istanza di Conservazione
                                                int dimCorrenteInIstanza_prox = 0;
                                                int numDocCorrentiInIstanza_prox = 0;

                                                dimCorrenteInIstanza_prox = BusinessLogic.Documenti.areaConservazioneManager.getDimensioneIstanza_Byte(idIstanza);
                                                numDocCorrentiInIstanza_prox = BusinessLogic.Documenti.areaConservazioneManager.getNumeroDocIstanza(idIstanza);

                                                // Calcolo dimensioni prox nell'istanza
                                                int dimProx_prox = dimCorrenteInIstanza_prox + total_size_prox;
                                                int numdocProx_prox = numDocCorrentiInIstanza_prox + 1;

                                                //
                                                // Invoco il metodo per la verifica della violazione del vincolo
                                                vincoloDimIstViol = BusinessLogic.Documenti.areaConservazioneManager.isVincoloDimensioneIstanzaViolato(dimProx_prox, dimMaxIstByte, percentualeTolleranza);
                                                vincoloNumDocIstViol = BusinessLogic.Documenti.areaConservazioneManager.isVincoloNumeroDocumentiIstanzaViolato(numdocProx_prox, numDocMaxIst);
                                                #endregion

                                                // Se vincolo violato per documento i-esimo, creo istanza e inserisco
                                                if (vincoloDimIstViol || vincoloNumDocIstViol)
                                                {
                                                    // Se posso creare istanza di conservazione la creo e faccio tutto
                                                    bool vincolo_i_esimoOK = true;
                                                    isPrimaIstanza = false;

                                                    #region Controllo se i-esimo elemento viola vincolo
                                                    SchedaDocumento sd_i_esimo = BusinessLogic.Documenti.DocManager.getDettaglioNoSecurity(userInfo, listaDoc[t].docNumber);
                                                    int total_size_i_esimo = 0;
                                                    if (sd_i_esimo != null)
                                                    {
                                                        int doc_size_i_esimo = Convert.ToInt32(((DocsPaVO.documento.Documento)sd_i_esimo.documenti[0]).fileSize);

                                                        int numeroAllegati_i_esimo = sd_i_esimo.allegati.Count;
                                                        string fileName_i_esimo = ((DocsPaVO.documento.Documento)sd_i_esimo.documenti[0]).fileName;
                                                        string tipoFile_i_esimo = Path.GetExtension(fileName_i_esimo);
                                                        int size_allegati_i_esimo = 0;
                                                        for (int j = 0; j < sd_i_esimo.allegati.Count; j++)
                                                        {
                                                            size_allegati_i_esimo = size_allegati_i_esimo + Convert.ToInt32(((DocsPaVO.documento.Allegato)sd_i_esimo.allegati[j]).fileSize);
                                                        }
                                                        total_size_i_esimo = size_allegati_i_esimo + doc_size_i_esimo;
                                                    }

                                                    // Calcolo dimensioni prox nell'istanza
                                                    int dimProx_i_esimo = total_size_i_esimo;
                                                    int numdocProx_i_esimo = 1;

                                                    //
                                                    // Invoco il metodo per la verifica della violazione del vincolo sul primo elemento
                                                    bool vincoloDimIstViol_i_esimo = BusinessLogic.Documenti.areaConservazioneManager.isVincoloDimensioneIstanzaViolato(dimProx_i_esimo, dimMaxIstByte, percentualeTolleranza);
                                                    bool vincoloNumDocIstViol_i_esimo = BusinessLogic.Documenti.areaConservazioneManager.isVincoloNumeroDocumentiIstanzaViolato(numdocProx_i_esimo, numDocMaxIst);

                                                    if (vincoloDimIstViol_i_esimo || vincoloNumDocIstViol_i_esimo)
                                                        vincolo_i_esimoOK = false;
                                                    #endregion

                                                    if (vincolo_i_esimoOK)
                                                    {
                                                        // Oggetto finalResult
                                                        finalResultNew = new DocsPaVO.Conservazione.ExecutionPolicy();
                                                        finalResultNew.startExecutionDate = DateTime.Now.ToString();

                                                        // creo istanza
                                                        idIstanza = BusinessLogic.Documenti.areaConservazioneManager.CreateIstanzaConservazione(userInfo, result[i].nome, string.Empty, result[i].system_id, stato, result[i].consolidazione, result[i].tipoConservazione);

                                                        // creo una nuova istanza, quindi azzero per questa nuova istanza il numero di doc processati
                                                        countDocProc = 0;

                                                        #region Modifiche F02_01 controllo formati
                                                        //controllo se la policy prevede l'invio e la conversione in automatico dell'istanza 
                                                        if (!string.IsNullOrEmpty(idIstanza))
                                                        {
                                                            if (result[i].statoInviato)
                                                                idIstanzeInvioAutomatico.Add(idIstanza);

                                                            if (result[i].statoConversione)
                                                                idIstanzeConvAutomatica.Add(idIstanza);

                                                        }
                                                        //if (string.IsNullOrEmpty(idIstanza))
                                                        //{
                                                        //    //return;
                                                        //}

                                                        //if (stato == "I")
                                                        //{
                                                        //    BusinessLogic.UserLog.UserLog.getInstance().WriteLog(userInfo, "INVIO_ISTANZA", idIstanza, String.Format("Invio automatico in conservazione dell' istanza {0}", idIstanza), logResponse);
                                                        //}
                                                        #endregion

                                                        // inserisco in istanza creata
                                                        try
                                                        {
                                                            BusinessLogic.Documenti.areaConservazioneManager.AddDocInAreaConservazioneWithPolicy(userInfo, idIstanza, listaDoc[t].idProfile, null, listaDoc[t].docNumber, "D", stato, getPolicyDocumentoValidato(result[i]));
                                                            success = conservazione.InsertDettExecutionPolicy(result[i].system_id, listaDoc[t].idProfile, null, "1", null);
                                                            countDocProc++;
                                                        }
                                                        catch
                                                        {
                                                            success = conservazione.InsertDettExecutionPolicy(result[i].system_id, listaDoc[t].idProfile, null, "0", null);
                                                        }

                                                        totDocProcessati = countDocProc.ToString();

                                                        lastDocumentId = listaDoc[t].idProfile;
                                                        firstDocumentId = listaDoc[t].idProfile;

                                                        finalResultNew.docNumberProcessed = totDocProcessati;
                                                        finalResultNew.endExecutionDate = DateTime.Now.ToString();
                                                        finalResultNew.idAmm = tempAmm.systemId;
                                                        finalResultNew.idFirstDocumentProcessed = firstDocumentId;
                                                        finalResultNew.idIstanza = idIstanza;
                                                        finalResultNew.idLastDocumentProcessed = lastDocumentId;
                                                        finalResultNew.idPolicy = result[i].system_id;
                                                    }
                                                }
                                                else
                                                {
                                                    // inserisco in istanza già creata
                                                    try
                                                    {
                                                        BusinessLogic.Documenti.areaConservazioneManager.AddDocInAreaConservazioneWithPolicy(userInfo, idIstanza, listaDoc[t].idProfile, null, listaDoc[t].docNumber, "D", stato, getPolicyDocumentoValidato(result[i]));
                                                        success = conservazione.InsertDettExecutionPolicy(result[i].system_id, listaDoc[t].idProfile, null, "1", null);
                                                        countDocProc++;
                                                    }
                                                    catch
                                                    {
                                                        success = conservazione.InsertDettExecutionPolicy(result[i].system_id, listaDoc[t].idProfile, null, "0", null);
                                                    }

                                                    if (isPrimaIstanza)
                                                    {
                                                        totDocProcessati = countDocProc.ToString();

                                                        lastDocumentId = listaDoc[t].idProfile;

                                                        finalResult.docNumberProcessed = totDocProcessati;
                                                        finalResult.endExecutionDate = DateTime.Now.ToString();
                                                        finalResult.idAmm = tempAmm.systemId;
                                                        finalResult.idFirstDocumentProcessed = firstDocumentId;
                                                        finalResult.idIstanza = idIstanza;
                                                        finalResult.idLastDocumentProcessed = lastDocumentId;
                                                        finalResult.idPolicy = result[i].system_id;
                                                    }
                                                    else
                                                    {
                                                        if (finalResultNew != null)
                                                        {
                                                            totDocProcessati = countDocProc.ToString();

                                                            lastDocumentId = listaDoc[t].idProfile;

                                                            finalResultNew.docNumberProcessed = totDocProcessati;
                                                            finalResultNew.endExecutionDate = DateTime.Now.ToString();
                                                            finalResultNew.idAmm = tempAmm.systemId;
                                                            finalResultNew.idFirstDocumentProcessed = firstDocumentId;
                                                            finalResultNew.idIstanza = idIstanza;
                                                            finalResultNew.idLastDocumentProcessed = lastDocumentId;
                                                            finalResultNew.idPolicy = result[i].system_id;
                                                        }
                                                    }
                                                }

                                                #region Gestione finalResult
                                                // Se è la prima istanza oppure ne ho splittata una nuova, aggiungo l'oggetto finalResult
                                                if (isPrimaIstanza)
                                                {
                                                    if (listExecutionPolicy.Count > 0)
                                                    {
                                                        foreach (DocsPaVO.Conservazione.ExecutionPolicy ep in listExecutionPolicy)
                                                        {
                                                            if (ep.idIstanza == finalResult.idIstanza)
                                                            {
                                                                // La rimozione nel for è causa di eccezione
                                                                // popolo una lista di idIstanza che devono essere rimossi dalla lista
                                                                //listExecutionPolicy.Remove(ep);
                                                                if (!listIdToRemoveExecutionPolicy.Contains(ep.idIstanza))
                                                                    listIdToRemoveExecutionPolicy.Add(ep.idIstanza);
                                                            }
                                                        }

                                                        // Rimuovo dalla lista listExecutionPolicy gli elementi della lista listIdToRemoveExecutionPolicy
                                                        foreach (string id in listIdToRemoveExecutionPolicy)
                                                        {
                                                            // Controllo Preventivo sull'esistenza dell'elemento
                                                            if (listExecutionPolicy.Contains(listExecutionPolicy.FirstOrDefault(itm => itm.idIstanza == id)))
                                                                listExecutionPolicy.Remove(listExecutionPolicy.FirstOrDefault(itm => itm.idIstanza == id));
                                                        }

                                                        listExecutionPolicy.Add(finalResult);
                                                    }
                                                    else
                                                        // Aggiungo la lista dei risultati
                                                        listExecutionPolicy.Add(finalResult);
                                                }
                                                else
                                                {
                                                    if (finalResultNew != null)
                                                    {

                                                        if (listExecutionPolicy.Count > 0)
                                                        {
                                                            foreach (DocsPaVO.Conservazione.ExecutionPolicy ep in listExecutionPolicy)
                                                            {
                                                                if (ep.idIstanza == finalResultNew.idIstanza)
                                                                {
                                                                    // La rimozione nel for è causa di eccezione
                                                                    // popolo una lista di idIstanza che devono essere rimossi dalla lista
                                                                    //listExecutionPolicy.Remove(ep);
                                                                    if (!listIdToRemoveExecutionPolicy.Contains(ep.idIstanza))
                                                                        listIdToRemoveExecutionPolicy.Add(ep.idIstanza);
                                                                }
                                                            }

                                                            // Rimuovo dalla lista listExecutionPolicy gli elementi della lista listIdToRemoveExecutionPolicy
                                                            foreach (string id in listIdToRemoveExecutionPolicy)
                                                            {
                                                                // Controllo Preventivo sull'esistenza dell'elemento
                                                                if (listExecutionPolicy.Contains(listExecutionPolicy.FirstOrDefault(itm => itm.idIstanza == id)))
                                                                    listExecutionPolicy.Remove(listExecutionPolicy.FirstOrDefault(itm => itm.idIstanza == id));
                                                            }

                                                            listExecutionPolicy.Add(finalResultNew);
                                                        }
                                                        else
                                                            listExecutionPolicy.Add(finalResultNew);
                                                    }
                                                }
                                                #endregion
                                            }
                                            //end for
                                        }
                                        // End controllo vincolo del primo documento
                                    }
                                    // End if listaDoc

                                    if (listExecutionPolicy != null && listExecutionPolicy.Count > 0)
                                    {
                                        foreach (DocsPaVO.Conservazione.ExecutionPolicy itemFinalResult in listExecutionPolicy)
                                        {
                                            conservazione.InsertExecutionPolicy(itemFinalResult);
                                        }
                                    }
                                }
                                // End if CheckPeriod
                            }
                            // End For Policy
                        }
                        #endregion

                        //4. Esegue le Policy per i fascicoli
                        #region Policy Fascicoli
                        result = conservazione.GetListaPolicy(Convert.ToInt32(tempAmm.systemId), "F");
                        if (result != null && result.Length > 0)
                        {
                            for (int i = 0; i < result.Length; i++)
                            {
                                bool trovato = false;
                                string firstFascId = "-1";
                                string lastFascId = "-1";
                                string idIstanza = "-1";
                                string totDocProcessati = "0";

                                DocsPaVO.Conservazione.ExecutionPolicy execution = conservazione.GetLastExecutionPolicyByIdPolicy(result[i].system_id);
                                if (CheckPeriod(result[i], execution))
                                {
                                    //Prende i fascicoli interessati
                                    Fascicolo[] listaFasc = null;
                                    InfoDocumento[] listaDoc = null;
                                    InfoUtente userInfo = new InfoUtente();

                                    //DocsPaVO.Conservazione.ExecutionPolicy finalResult = new DocsPaVO.Conservazione.ExecutionPolicy();
                                    //finalResult.startExecutionDate = DateTime.Now.ToString();

                                    DocsPaVO.utente.Utente utente = BusinessLogic.Utenti.UserManager.getUtente(result[i].codiceUtente, result[i].idAmministrazione);

                                    userInfo = new DocsPaVO.utente.InfoUtente(utente, BusinessLogic.Utenti.UserManager.getRuoloByIdGruppo(result[i].idGruppo));

                                    DocsPaDB.Query_DocsPAWS.Documenti documenti = new DocsPaDB.Query_DocsPAWS.Documenti();
                                    DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli();
                                    string idLastId = "0";
                                    if (execution != null)
                                    {
                                        idLastId = execution.idLastDocumentProcessed;
                                    }
                                    listaFasc = fascicoli.GetListaFascicoliPolicyConservazione(result[i], idLastId);

                                    //
                                    // Lista di DocsPaVO.Conservazione.ExecutionPolicy
                                    List<DocsPaVO.Conservazione.ExecutionPolicy> listExecutionPolicy = new List<DocsPaVO.Conservazione.ExecutionPolicy>();

                                    // Lista di ID degli elementi da rimuovere nella listExecutionPolicy
                                    List<string> listIdToRemoveExecutionPolicy = new List<string>();

                                    if (listaFasc != null && listaFasc.Length > 0)
                                    {

                                        firstFascId = listaFasc[0].systemID;
                                        lastFascId = listaFasc[listaFasc.Length - 1].systemID;

                                        string stato = "N";
                                        // Totale Facsicoli processati
                                        totDocProcessati = (listaFasc.Length).ToString();

                                        for (int x = 0; x < listaFasc.Length; x++)
                                        {

                                            listaDoc = documenti.GetListaDocumentiInFascicoliPolicyConservazione(result[i], "0", listaFasc[x].systemID);
                                            if (listaDoc != null && listaDoc.Length > 0)
                                            {
                                                // Se posso creare istanza di conservazione la creo e faccio tutto
                                                bool vincoloPrimoOK = true;

                                                // Get Parametri validi per istanze di conservazione
                                                #region Mev CS 1.5 - F03_01 Get dei parametri
                                                //
                                                // Get Dimensione massima istanza in termini di Byte
                                                int dimMaxIstByte = BusinessLogic.Documenti.areaConservazioneManager.getDimensioneMassimaIstanze_Byte(result[i].idAmministrazione);
                                                //
                                                // Get numero massimo di doc in istanza
                                                int numDocMaxIst = BusinessLogic.Documenti.areaConservazioneManager.getDimensioneMassimaIstanze_NumDoc(result[i].idAmministrazione);
                                                //
                                                // Get percentuale di tolleranza in istanza
                                                int percentualeTolleranza = BusinessLogic.Documenti.areaConservazioneManager.getPercentualeTolleranzaDinesioneIstanze(result[i].idAmministrazione);

                                                #endregion

                                                // Verifico se posso inserire il primo elemento
                                                #region MEV CS 1.5 - F03_01 Controllo vincoli istanza per il primo elemento
                                                SchedaDocumento sd_primo = BusinessLogic.Documenti.DocManager.getDettaglioNoSecurity(userInfo, listaDoc[0].docNumber);
                                                int total_size_primo = 0;
                                                if (sd_primo != null)
                                                {
                                                    int doc_size_primo = Convert.ToInt32(((DocsPaVO.documento.Documento)sd_primo.documenti[0]).fileSize);

                                                    int numeroAllegati_primo = sd_primo.allegati.Count;
                                                    string fileName_primo = ((DocsPaVO.documento.Documento)sd_primo.documenti[0]).fileName;
                                                    string tipoFile_primo = Path.GetExtension(fileName_primo);
                                                    int size_allegati_primo = 0;
                                                    for (int j = 0; j < sd_primo.allegati.Count; j++)
                                                    {
                                                        size_allegati_primo = size_allegati_primo + Convert.ToInt32(((DocsPaVO.documento.Allegato)sd_primo.allegati[j]).fileSize);
                                                    }
                                                    total_size_primo = size_allegati_primo + doc_size_primo;
                                                }

                                                // Calcolo dimensioni prox nell'istanza
                                                int dimProx_primo = total_size_primo;
                                                int numdocProx_primo = 1;

                                                //
                                                // Invoco il metodo per la verifica della violazione del vincolo sul primo elemento
                                                bool vincoloDimIstViol_primo = BusinessLogic.Documenti.areaConservazioneManager.isVincoloDimensioneIstanzaViolato(dimProx_primo, dimMaxIstByte, percentualeTolleranza);
                                                bool vincoloNumDocIstViol_primo = BusinessLogic.Documenti.areaConservazioneManager.isVincoloNumeroDocumentiIstanzaViolato(numdocProx_primo, numDocMaxIst);
                                                #endregion

                                                // Se posso inserire il primo doc procedo, altrimenti non faccio nulla
                                                if (vincoloDimIstViol_primo || vincoloNumDocIstViol_primo)
                                                    vincoloPrimoOK = false;

                                                // Vincolo per verificare se posso inserire almeno un documento
                                                if (vincoloPrimoOK)
                                                {
                                                    // vincoli per dimensioni istanza
                                                    bool vincoloDimIstViol = false;
                                                    bool vincoloNumDocIstViol = false;

                                                    idIstanza = "-1";
                                                    bool success = false;

                                                    //if (result[i].statoInviato)
                                                    //{
                                                    //    stato = "I";
                                                    //}
                                                    // creo prima istanza
                                                    idIstanza = BusinessLogic.Documenti.areaConservazioneManager.CreateIstanzaConservazione(userInfo, result[i].nome, string.Empty, result[i].system_id, stato, result[i].consolidazione, result[i].tipoConservazione);

                                                    #region Modifiche F02_01 controllo formati
                                                    //controllo se la policy prevede l'invio e la conversione in automatico dell'istanza 
                                                    if (!string.IsNullOrEmpty(idIstanza))
                                                    {
                                                        if (result[i].statoInviato)
                                                            idIstanzeInvioAutomatico.Add(idIstanza);

                                                        if (result[i].statoConversione)
                                                            idIstanzeConvAutomatica.Add(idIstanza);

                                                    }

                                                    //if (stato == "I")
                                                    //{
                                                    //    BusinessLogic.UserLog.UserLog.getInstance().WriteLog(userInfo, "INVIO_ISTANZA", idIstanza, String.Format("Invio automatico in conservazione dell' istanza {0}", idIstanza), logResponse);
                                                    //}
                                                    #endregion

                                                    //Crea una nuova istanza di conservazione
                                                    string firstDocumentId = "-1";
                                                    string lastDocumentId = "-1";

                                                    totDocProcessati = "0";
                                                    // Contatore per il numero di documenti processati
                                                    int countDocProc = 0;

                                                    // Primo documento
                                                    firstDocumentId = listaDoc[0].idProfile;

                                                    // Oggetto finalResult
                                                    DocsPaVO.Conservazione.ExecutionPolicy finalResult = new DocsPaVO.Conservazione.ExecutionPolicy();
                                                    finalResult.startExecutionDate = DateTime.Now.ToString();

                                                    bool isPrimaIstanza = true;
                                                    DocsPaVO.Conservazione.ExecutionPolicy finalResultNew = null;

                                                    for (int t = 0; t < listaDoc.Length; t++)
                                                    {
                                                        //
                                                        // Controllo sempre se il prossimo elemento viola il vincolo delle istanze
                                                        #region MEV CS 1.5 Controllo vincoli Prossimo elemento
                                                        SchedaDocumento sd_prox = BusinessLogic.Documenti.DocManager.getDettaglioNoSecurity(userInfo, listaDoc[t].docNumber);
                                                        int total_size_prox = 0;
                                                        if (sd_prox != null)
                                                        {
                                                            int doc_size_prox = Convert.ToInt32(((DocsPaVO.documento.Documento)sd_prox.documenti[0]).fileSize);

                                                            int numeroAllegati_prox = sd_prox.allegati.Count;
                                                            string fileName_prox = ((DocsPaVO.documento.Documento)sd_prox.documenti[0]).fileName;
                                                            string tipoFile_prox = Path.GetExtension(fileName_prox);
                                                            int size_allegati_prox = 0;
                                                            for (int j = 0; j < sd_prox.allegati.Count; j++)
                                                            {
                                                                size_allegati_prox = size_allegati_prox + Convert.ToInt32(((DocsPaVO.documento.Allegato)sd_prox.allegati[j]).fileSize);
                                                            }
                                                            total_size_prox = size_allegati_prox + doc_size_prox;
                                                        }

                                                        //
                                                        // Get sizeItem e numDoc in Istanza di Conservazione
                                                        int dimCorrenteInIstanza_prox = 0;
                                                        int numDocCorrentiInIstanza_prox = 0;

                                                        dimCorrenteInIstanza_prox = BusinessLogic.Documenti.areaConservazioneManager.getDimensioneIstanza_Byte(idIstanza);
                                                        numDocCorrentiInIstanza_prox = BusinessLogic.Documenti.areaConservazioneManager.getNumeroDocIstanza(idIstanza);

                                                        // Calcolo dimensioni prox nell'istanza
                                                        int dimProx_prox = dimCorrenteInIstanza_prox + total_size_prox;
                                                        int numdocProx_prox = numDocCorrentiInIstanza_prox + 1;

                                                        //
                                                        // Invoco il metodo per la verifica della violazione del vincolo
                                                        vincoloDimIstViol = BusinessLogic.Documenti.areaConservazioneManager.isVincoloDimensioneIstanzaViolato(dimProx_prox, dimMaxIstByte, percentualeTolleranza);
                                                        vincoloNumDocIstViol = BusinessLogic.Documenti.areaConservazioneManager.isVincoloNumeroDocumentiIstanzaViolato(numdocProx_prox, numDocMaxIst);
                                                        #endregion

                                                        // Se vincolo violato per documento i-esimo, creo istanza e inserisco
                                                        if (vincoloDimIstViol || vincoloNumDocIstViol)
                                                        {
                                                            // Se posso creare istanza di conservazione la creo e faccio tutto
                                                            bool vincolo_i_esimoOK = true;
                                                            isPrimaIstanza = false;

                                                            #region Controllo se i-esimo elemento viola vincolo
                                                            SchedaDocumento sd_i_esimo = BusinessLogic.Documenti.DocManager.getDettaglioNoSecurity(userInfo, listaDoc[t].docNumber);
                                                            int total_size_i_esimo = 0;
                                                            if (sd_i_esimo != null)
                                                            {
                                                                int doc_size_i_esimo = Convert.ToInt32(((DocsPaVO.documento.Documento)sd_i_esimo.documenti[0]).fileSize);

                                                                int numeroAllegati_i_esimo = sd_i_esimo.allegati.Count;
                                                                string fileName_i_esimo = ((DocsPaVO.documento.Documento)sd_i_esimo.documenti[0]).fileName;
                                                                string tipoFile_i_esimo = Path.GetExtension(fileName_i_esimo);
                                                                int size_allegati_i_esimo = 0;
                                                                for (int j = 0; j < sd_i_esimo.allegati.Count; j++)
                                                                {
                                                                    size_allegati_i_esimo = size_allegati_i_esimo + Convert.ToInt32(((DocsPaVO.documento.Allegato)sd_i_esimo.allegati[j]).fileSize);
                                                                }
                                                                total_size_i_esimo = size_allegati_i_esimo + doc_size_i_esimo;
                                                            }

                                                            // Calcolo dimensioni prox nell'istanza
                                                            int dimProx_i_esimo = total_size_i_esimo;
                                                            int numdocProx_i_esimo = 1;

                                                            //
                                                            // Invoco il metodo per la verifica della violazione del vincolo sul primo elemento
                                                            bool vincoloDimIstViol_i_esimo = BusinessLogic.Documenti.areaConservazioneManager.isVincoloDimensioneIstanzaViolato(dimProx_i_esimo, dimMaxIstByte, percentualeTolleranza);
                                                            bool vincoloNumDocIstViol_i_esimo = BusinessLogic.Documenti.areaConservazioneManager.isVincoloNumeroDocumentiIstanzaViolato(numdocProx_i_esimo, numDocMaxIst);

                                                            if (vincoloDimIstViol_i_esimo || vincoloNumDocIstViol_i_esimo)
                                                                vincolo_i_esimoOK = false;
                                                            #endregion

                                                            if (vincolo_i_esimoOK)
                                                            {
                                                                // Oggetto finalResult
                                                                finalResultNew = new DocsPaVO.Conservazione.ExecutionPolicy();
                                                                finalResultNew.startExecutionDate = DateTime.Now.ToString();

                                                                // creo istanza
                                                                idIstanza = BusinessLogic.Documenti.areaConservazioneManager.CreateIstanzaConservazione(userInfo, result[i].nome, string.Empty, result[i].system_id, stato, result[i].consolidazione, result[i].tipoConservazione);

                                                                // creo una nuova istanza, quindi azzero per questa nuova istanza il numero di doc processati
                                                                countDocProc = 0;

                                                                #region Modifiche F02_01 controllo formati
                                                                //controllo se la policy prevede l'invio e la conversione in automatico dell'istanza 
                                                                if (!string.IsNullOrEmpty(idIstanza))
                                                                {
                                                                    if (result[i].statoInviato)
                                                                        idIstanzeInvioAutomatico.Add(idIstanza);

                                                                    if (result[i].statoConversione)
                                                                        idIstanzeConvAutomatica.Add(idIstanza);

                                                                }
                                                                //if (string.IsNullOrEmpty(idIstanza))
                                                                //{
                                                                //    //return;
                                                                //}

                                                                //if (stato == "I")
                                                                //{
                                                                //    BusinessLogic.UserLog.UserLog.getInstance().WriteLog(userInfo, "INVIO_ISTANZA", idIstanza, String.Format("Invio automatico in conservazione dell' istanza {0}", idIstanza), logResponse);
                                                                //}
                                                                #endregion

                                                                // inserisco in istanza creata
                                                                try
                                                                {
                                                                    BusinessLogic.Documenti.areaConservazioneManager.AddDocInAreaConservazioneWithPolicy(userInfo, idIstanza, listaDoc[t].idProfile, listaFasc[x].idClassificazione, listaDoc[t].docNumber, "F", stato, getPolicyDocumentoValidato(result[i]));
                                                                    success = conservazione.InsertDettExecutionPolicy(result[i].system_id, listaDoc[t].idProfile, listaFasc[x].idClassificazione, "1", string.Empty);
                                                                    countDocProc++;
                                                                }
                                                                catch
                                                                {
                                                                    success = conservazione.InsertDettExecutionPolicy(result[i].system_id, listaDoc[t].idProfile, listaFasc[x].idClassificazione, "0", string.Empty);
                                                                }

                                                                //totDocProcessati = countDocProc.ToString();

                                                                //lastDocumentId = listaDoc[t].idProfile;
                                                                //firstDocumentId = listaDoc[t].idProfile;

                                                                finalResultNew.docNumberProcessed = totDocProcessati;
                                                                finalResultNew.endExecutionDate = DateTime.Now.ToString();
                                                                finalResultNew.idAmm = tempAmm.systemId;
                                                                //finalResultNew.idFirstDocumentProcessed = firstDocumentId;
                                                                finalResultNew.idFirstDocumentProcessed = firstFascId;
                                                                finalResultNew.idIstanza = idIstanza;
                                                                //finalResultNew.idLastDocumentProcessed = lastDocumentId;
                                                                finalResultNew.idLastDocumentProcessed = lastFascId;
                                                                finalResultNew.idPolicy = result[i].system_id;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            // inserisco in istanza già creata
                                                            try
                                                            {
                                                                BusinessLogic.Documenti.areaConservazioneManager.AddDocInAreaConservazioneWithPolicy(userInfo, idIstanza, listaDoc[t].idProfile, listaFasc[x].idClassificazione, listaDoc[t].docNumber, "F", stato, getPolicyDocumentoValidato(result[i]));
                                                                success = conservazione.InsertDettExecutionPolicy(result[i].system_id, listaDoc[t].idProfile, listaFasc[x].idClassificazione, "1", string.Empty);
                                                                countDocProc++;
                                                            }
                                                            catch
                                                            {
                                                                success = conservazione.InsertDettExecutionPolicy(result[i].system_id, listaDoc[t].idProfile, listaFasc[x].idClassificazione, "0", string.Empty);
                                                            }

                                                            if (isPrimaIstanza)
                                                            {
                                                                //totDocProcessati = countDocProc.ToString();

                                                                //lastDocumentId = listaDoc[t].idProfile;

                                                                finalResult.docNumberProcessed = totDocProcessati;
                                                                finalResult.endExecutionDate = DateTime.Now.ToString();
                                                                finalResult.idAmm = tempAmm.systemId;
                                                                //finalResult.idFirstDocumentProcessed = firstDocumentId;
                                                                finalResult.idFirstDocumentProcessed = firstFascId;
                                                                finalResult.idIstanza = idIstanza;
                                                                //finalResult.idLastDocumentProcessed = lastDocumentId;
                                                                finalResult.idLastDocumentProcessed = lastFascId;
                                                                finalResult.idPolicy = result[i].system_id;
                                                            }
                                                            else
                                                            {
                                                                if (finalResultNew != null)
                                                                {
                                                                    //totDocProcessati = countDocProc.ToString();

                                                                    //lastDocumentId = listaDoc[t].idProfile;

                                                                    finalResultNew.docNumberProcessed = totDocProcessati;
                                                                    finalResultNew.endExecutionDate = DateTime.Now.ToString();
                                                                    finalResultNew.idAmm = tempAmm.systemId;
                                                                    //finalResultNew.idFirstDocumentProcessed = firstDocumentId;
                                                                    finalResultNew.idFirstDocumentProcessed = firstFascId;
                                                                    finalResultNew.idIstanza = idIstanza;
                                                                    //finalResultNew.idLastDocumentProcessed = lastDocumentId;
                                                                    finalResultNew.idLastDocumentProcessed = lastFascId;
                                                                    finalResultNew.idPolicy = result[i].system_id;
                                                                }
                                                            }
                                                        }

                                                        #region Gestione finalResult
                                                        // Se è la prima istanza oppure ne ho splittata una nuova, aggiungo l'oggetto finalResult
                                                        if (isPrimaIstanza)
                                                        {
                                                            if (listExecutionPolicy.Count > 0)
                                                            {
                                                                foreach (DocsPaVO.Conservazione.ExecutionPolicy ep in listExecutionPolicy)
                                                                {
                                                                    if (ep.idIstanza == finalResult.idIstanza)
                                                                    {
                                                                        // La rimozione nel for è causa di eccezione
                                                                        // popolo una lista di idIstanza che devono essere rimossi dalla lista
                                                                        //listExecutionPolicy.Remove(ep);
                                                                        if (!listIdToRemoveExecutionPolicy.Contains(ep.idIstanza))
                                                                            listIdToRemoveExecutionPolicy.Add(ep.idIstanza);
                                                                    }
                                                                }

                                                                // Rimuovo dalla lista listExecutionPolicy gli elementi della lista listIdToRemoveExecutionPolicy
                                                                foreach (string id in listIdToRemoveExecutionPolicy)
                                                                {
                                                                    // Controllo Preventivo sull'esistenza dell'elemento
                                                                    if (listExecutionPolicy.Contains(listExecutionPolicy.FirstOrDefault(itm => itm.idIstanza == id)))
                                                                        listExecutionPolicy.Remove(listExecutionPolicy.FirstOrDefault(itm => itm.idIstanza == id));
                                                                }

                                                                listExecutionPolicy.Add(finalResult);
                                                            }
                                                            else
                                                                // Aggiungo la lista dei risultati
                                                                listExecutionPolicy.Add(finalResult);
                                                        }
                                                        else
                                                        {
                                                            if (finalResultNew != null)
                                                            {

                                                                if (listExecutionPolicy.Count > 0)
                                                                {
                                                                    foreach (DocsPaVO.Conservazione.ExecutionPolicy ep in listExecutionPolicy)
                                                                    {
                                                                        if (ep.idIstanza == finalResultNew.idIstanza)
                                                                        {
                                                                            // La rimozione nel for è causa di eccezione
                                                                            // popolo una lista di idIstanza che devono essere rimossi dalla lista
                                                                            //listExecutionPolicy.Remove(ep);
                                                                            if (!listIdToRemoveExecutionPolicy.Contains(ep.idIstanza))
                                                                                listIdToRemoveExecutionPolicy.Add(ep.idIstanza);
                                                                        }
                                                                    }

                                                                    // Rimuovo dalla lista listExecutionPolicy gli elementi della lista listIdToRemoveExecutionPolicy
                                                                    foreach (string id in listIdToRemoveExecutionPolicy)
                                                                    {
                                                                        // Controllo Preventivo sull'esistenza dell'elemento
                                                                        if (listExecutionPolicy.Contains(listExecutionPolicy.FirstOrDefault(itm => itm.idIstanza == id)))
                                                                            listExecutionPolicy.Remove(listExecutionPolicy.FirstOrDefault(itm => itm.idIstanza == id));
                                                                    }

                                                                    listExecutionPolicy.Add(finalResultNew);
                                                                }
                                                                else
                                                                    listExecutionPolicy.Add(finalResultNew);
                                                            }
                                                        }
                                                        #endregion

                                                    }
                                                    // end for documenti
                                                }
                                                // end if vincolo primo ok
                                            }
                                            // End if lista Doc

                                            if (listExecutionPolicy != null && listExecutionPolicy.Count > 0)
                                            {
                                                foreach (DocsPaVO.Conservazione.ExecutionPolicy itemFinalResult in listExecutionPolicy)
                                                {
                                                    conservazione.InsertExecutionPolicy(itemFinalResult);
                                                }
                                            }
                                        }
                                        // End for Facsicoli
                                    }
                                    // end if lista fasc
                                }
                                //end if CheckPeriod
                            }
                            // end for lista policy
                        }
                        #endregion

                        //5. Esegue le Policy per le stampe registro
                        #region Policy Stampe Registro
                        result = conservazione.GetListaPolicy(Convert.ToInt32(tempAmm.systemId), "R");
                        if (result != null && result.Length > 0)
                        {
                            for (int i = 0; i < result.Length; i++)
                            {
                                DocsPaVO.Conservazione.ExecutionPolicy execution = conservazione.GetLastExecutionPolicyByIdPolicy(result[i].system_id);
                                if (CheckPeriod(result[i], execution))
                                {
                                    //Prende i documenti interessati
                                    InfoDocumento[] listaDoc = null;
                                    InfoUtente userInfo = new InfoUtente();
                                    //DocsPaVO.Conservazione.ExecutionPolicy finalResult = new DocsPaVO.Conservazione.ExecutionPolicy();
                                    //finalResult.startExecutionDate = DateTime.Now.ToString();
                                    DocsPaVO.utente.Utente utente = BusinessLogic.Utenti.UserManager.getUtente(result[i].codiceUtente, result[i].idAmministrazione);

                                    userInfo = new DocsPaVO.utente.InfoUtente(utente, BusinessLogic.Utenti.UserManager.getRuoloByIdGruppo(result[i].idGruppo));

                                    DocsPaDB.Query_DocsPAWS.Documenti documenti = new DocsPaDB.Query_DocsPAWS.Documenti();
                                    string idLastId = "0";
                                    if (execution != null)
                                    {
                                        idLastId = execution.idLastDocumentProcessed;
                                    }

                                    listaDoc = documenti.GetListaStampePolicyConservazione(result[i], idLastId, "R");

                                    //
                                    // Lista di DocsPaVO.Conservazione.ExecutionPolicy
                                    List<DocsPaVO.Conservazione.ExecutionPolicy> listExecutionPolicy = new List<DocsPaVO.Conservazione.ExecutionPolicy>();

                                    // Lista di ID degli elementi da rimuovere nella listExecutionPolicy
                                    List<string> listIdToRemoveExecutionPolicy = new List<string>();

                                    //string firstDocumentId = "-1";
                                    //string lastDocumentId = "-1";
                                    //string idIstanza = "-1";
                                    //string totDocProcessati = "0";
                                    if (listaDoc != null && listaDoc.Length > 0)
                                    {
                                        //totDocProcessati = (listaDoc.Length).ToString();
                                        string stato = "N";
                                        //if (result[i].statoInviato)
                                        //{
                                        //    stato = "I";
                                        //}

                                        // Se posso creare istanza di conservazione la creo e faccio tutto
                                        bool vincoloPrimoOK = true;

                                        // Get Parametri validi per istanze di conservazione
                                        #region Mev CS 1.5 - F03_01 Get dei parametri
                                        //
                                        // Get Dimensione massima istanza in termini di Byte
                                        int dimMaxIstByte = BusinessLogic.Documenti.areaConservazioneManager.getDimensioneMassimaIstanze_Byte(result[i].idAmministrazione);
                                        //
                                        // Get numero massimo di doc in istanza
                                        int numDocMaxIst = BusinessLogic.Documenti.areaConservazioneManager.getDimensioneMassimaIstanze_NumDoc(result[i].idAmministrazione);
                                        //
                                        // Get percentuale di tolleranza in istanza
                                        int percentualeTolleranza = BusinessLogic.Documenti.areaConservazioneManager.getPercentualeTolleranzaDinesioneIstanze(result[i].idAmministrazione);

                                        #endregion

                                        // Verifico se posso inserire il primo elemento
                                        #region MEV CS 1.5 - F03_01 Controllo vincoli istanza per il primo elemento
                                        SchedaDocumento sd_primo = BusinessLogic.Documenti.DocManager.getDettaglioNoSecurity(userInfo, listaDoc[0].docNumber);
                                        int total_size_primo = 0;
                                        if (sd_primo != null)
                                        {
                                            int doc_size_primo = Convert.ToInt32(((DocsPaVO.documento.Documento)sd_primo.documenti[0]).fileSize);

                                            int numeroAllegati_primo = sd_primo.allegati.Count;
                                            string fileName_primo = ((DocsPaVO.documento.Documento)sd_primo.documenti[0]).fileName;
                                            string tipoFile_primo = Path.GetExtension(fileName_primo);
                                            int size_allegati_primo = 0;
                                            for (int j = 0; j < sd_primo.allegati.Count; j++)
                                            {
                                                size_allegati_primo = size_allegati_primo + Convert.ToInt32(((DocsPaVO.documento.Allegato)sd_primo.allegati[j]).fileSize);
                                            }
                                            total_size_primo = size_allegati_primo + doc_size_primo;
                                        }

                                        // Calcolo dimensioni prox nell'istanza
                                        int dimProx_primo = total_size_primo;
                                        int numdocProx_primo = 1;

                                        //
                                        // Invoco il metodo per la verifica della violazione del vincolo sul primo elemento
                                        bool vincoloDimIstViol_primo = BusinessLogic.Documenti.areaConservazioneManager.isVincoloDimensioneIstanzaViolato(dimProx_primo, dimMaxIstByte, percentualeTolleranza);
                                        bool vincoloNumDocIstViol_primo = BusinessLogic.Documenti.areaConservazioneManager.isVincoloNumeroDocumentiIstanzaViolato(numdocProx_primo, numDocMaxIst);
                                        #endregion

                                        // Se posso inserire il primo doc procedo, altrimenti non faccio nulla
                                        if (vincoloDimIstViol_primo || vincoloNumDocIstViol_primo)
                                            vincoloPrimoOK = false;

                                        // Vincolo per verificare se posso inserire almeno un documento
                                        if (vincoloPrimoOK)
                                        {
                                            // vincoli per dimensioni istanza
                                            bool vincoloDimIstViol = false;
                                            bool vincoloNumDocIstViol = false;

                                            string idIstanza = "-1";
                                            bool success = false;

                                            // creo prima istanza
                                            idIstanza = BusinessLogic.Documenti.areaConservazioneManager.CreateIstanzaConservazione(userInfo, result[i].nome, string.Empty, result[i].system_id, stato, result[i].consolidazione, result[i].tipoConservazione);

                                            #region Modifiche F02_01 controllo formati
                                            //controllo se la policy prevede l'invio e la conversione in automatico dell'istanza 
                                            if (!string.IsNullOrEmpty(idIstanza))
                                            {
                                                if (result[i].statoInviato)
                                                    idIstanzeInvioAutomatico.Add(idIstanza);

                                                if (result[i].statoConversione)
                                                    idIstanzeConvAutomatica.Add(idIstanza);

                                            }
                                            //if (stato == "I")
                                            //{
                                            //    BusinessLogic.UserLog.UserLog.getInstance().WriteLog(userInfo, "INVIO_ISTANZA", idIstanza, String.Format("Invio automatico in conservazione dell' istanza {0}", idIstanza), logResponse);
                                            //}
                                            #endregion

                                            //Crea una nuova istanza di conservazione
                                            string firstDocumentId = "-1";
                                            string lastDocumentId = "-1";

                                            string totDocProcessati = "0";
                                            // Contatore per il numero di documenti processati
                                            int countDocProc = 0;

                                            // Primo documento
                                            firstDocumentId = listaDoc[0].idProfile;

                                            // Oggetto finalResult
                                            DocsPaVO.Conservazione.ExecutionPolicy finalResult = new DocsPaVO.Conservazione.ExecutionPolicy();
                                            finalResult.startExecutionDate = DateTime.Now.ToString();

                                            bool isPrimaIstanza = true;
                                            DocsPaVO.Conservazione.ExecutionPolicy finalResultNew = null;

                                            for (int t = 0; t < listaDoc.Length; t++)
                                            {
                                                //
                                                // Controllo sempre se il prossimo elemento viola il vincolo delle istanze
                                                #region MEV CS 1.5 Controllo vincoli Prossimo elemento
                                                SchedaDocumento sd_prox = BusinessLogic.Documenti.DocManager.getDettaglioNoSecurity(userInfo, listaDoc[t].docNumber);
                                                int total_size_prox = 0;
                                                if (sd_prox != null)
                                                {
                                                    int doc_size_prox = Convert.ToInt32(((DocsPaVO.documento.Documento)sd_prox.documenti[0]).fileSize);

                                                    int numeroAllegati_prox = sd_prox.allegati.Count;
                                                    string fileName_prox = ((DocsPaVO.documento.Documento)sd_prox.documenti[0]).fileName;
                                                    string tipoFile_prox = Path.GetExtension(fileName_prox);
                                                    int size_allegati_prox = 0;
                                                    for (int j = 0; j < sd_prox.allegati.Count; j++)
                                                    {
                                                        size_allegati_prox = size_allegati_prox + Convert.ToInt32(((DocsPaVO.documento.Allegato)sd_prox.allegati[j]).fileSize);
                                                    }
                                                    total_size_prox = size_allegati_prox + doc_size_prox;
                                                }

                                                //
                                                // Get sizeItem e numDoc in Istanza di Conservazione
                                                int dimCorrenteInIstanza_prox = 0;
                                                int numDocCorrentiInIstanza_prox = 0;

                                                dimCorrenteInIstanza_prox = BusinessLogic.Documenti.areaConservazioneManager.getDimensioneIstanza_Byte(idIstanza);
                                                numDocCorrentiInIstanza_prox = BusinessLogic.Documenti.areaConservazioneManager.getNumeroDocIstanza(idIstanza);

                                                // Calcolo dimensioni prox nell'istanza
                                                int dimProx_prox = dimCorrenteInIstanza_prox + total_size_prox;
                                                int numdocProx_prox = numDocCorrentiInIstanza_prox + 1;

                                                //
                                                // Invoco il metodo per la verifica della violazione del vincolo
                                                vincoloDimIstViol = BusinessLogic.Documenti.areaConservazioneManager.isVincoloDimensioneIstanzaViolato(dimProx_prox, dimMaxIstByte, percentualeTolleranza);
                                                vincoloNumDocIstViol = BusinessLogic.Documenti.areaConservazioneManager.isVincoloNumeroDocumentiIstanzaViolato(numdocProx_prox, numDocMaxIst);
                                                #endregion

                                                // Se vincolo violato per documento i-esimo, creo istanza e inserisco
                                                if (vincoloDimIstViol || vincoloNumDocIstViol)
                                                {
                                                    // Se posso creare istanza di conservazione la creo e faccio tutto
                                                    bool vincolo_i_esimoOK = true;
                                                    isPrimaIstanza = false;

                                                    #region Controllo se i-esimo elemento viola vincolo
                                                    SchedaDocumento sd_i_esimo = BusinessLogic.Documenti.DocManager.getDettaglioNoSecurity(userInfo, listaDoc[t].docNumber);
                                                    int total_size_i_esimo = 0;
                                                    if (sd_i_esimo != null)
                                                    {
                                                        int doc_size_i_esimo = Convert.ToInt32(((DocsPaVO.documento.Documento)sd_i_esimo.documenti[0]).fileSize);

                                                        int numeroAllegati_i_esimo = sd_i_esimo.allegati.Count;
                                                        string fileName_i_esimo = ((DocsPaVO.documento.Documento)sd_i_esimo.documenti[0]).fileName;
                                                        string tipoFile_i_esimo = Path.GetExtension(fileName_i_esimo);
                                                        int size_allegati_i_esimo = 0;
                                                        for (int j = 0; j < sd_i_esimo.allegati.Count; j++)
                                                        {
                                                            size_allegati_i_esimo = size_allegati_i_esimo + Convert.ToInt32(((DocsPaVO.documento.Allegato)sd_i_esimo.allegati[j]).fileSize);
                                                        }
                                                        total_size_i_esimo = size_allegati_i_esimo + doc_size_i_esimo;
                                                    }

                                                    // Calcolo dimensioni prox nell'istanza
                                                    int dimProx_i_esimo = total_size_i_esimo;
                                                    int numdocProx_i_esimo = 1;

                                                    //
                                                    // Invoco il metodo per la verifica della violazione del vincolo sul primo elemento
                                                    bool vincoloDimIstViol_i_esimo = BusinessLogic.Documenti.areaConservazioneManager.isVincoloDimensioneIstanzaViolato(dimProx_i_esimo, dimMaxIstByte, percentualeTolleranza);
                                                    bool vincoloNumDocIstViol_i_esimo = BusinessLogic.Documenti.areaConservazioneManager.isVincoloNumeroDocumentiIstanzaViolato(numdocProx_i_esimo, numDocMaxIst);

                                                    if (vincoloDimIstViol_i_esimo || vincoloNumDocIstViol_i_esimo)
                                                        vincolo_i_esimoOK = false;
                                                    #endregion

                                                    if (vincolo_i_esimoOK)
                                                    {
                                                        // Oggetto finalResult
                                                        finalResultNew = new DocsPaVO.Conservazione.ExecutionPolicy();
                                                        finalResultNew.startExecutionDate = DateTime.Now.ToString();

                                                        // creo istanza
                                                        idIstanza = BusinessLogic.Documenti.areaConservazioneManager.CreateIstanzaConservazione(userInfo, result[i].nome, string.Empty, result[i].system_id, stato, result[i].consolidazione, result[i].tipoConservazione);

                                                        // creo una nuova istanza, quindi azzero per questa nuova istanza il numero di doc processati
                                                        countDocProc = 0;

                                                        #region Modifiche F02_01 controllo formati
                                                        //controllo se la policy prevede l'invio e la conversione in automatico dell'istanza 
                                                        if (!string.IsNullOrEmpty(idIstanza))
                                                        {
                                                            if (result[i].statoInviato)
                                                                idIstanzeInvioAutomatico.Add(idIstanza);

                                                            if (result[i].statoConversione)
                                                                idIstanzeConvAutomatica.Add(idIstanza);

                                                        }
                                                        //if (string.IsNullOrEmpty(idIstanza))
                                                        //{
                                                        //    //return;
                                                        //}

                                                        //if (stato == "I")
                                                        //{
                                                        //    BusinessLogic.UserLog.UserLog.getInstance().WriteLog(userInfo, "INVIO_ISTANZA", idIstanza, String.Format("Invio automatico in conservazione dell' istanza {0}", idIstanza), logResponse);
                                                        //}
                                                        #endregion

                                                        // inserisco in istanza creata
                                                        try
                                                        {
                                                            BusinessLogic.Documenti.areaConservazioneManager.AddDocInAreaConservazioneWithPolicy(userInfo, idIstanza, listaDoc[t].idProfile, null, listaDoc[t].docNumber, "D", stato, getPolicyDocumentoValidato(result[i]));
                                                            success = conservazione.InsertDettExecutionPolicy(result[i].system_id, listaDoc[t].idProfile, null, "1", null);
                                                            countDocProc++;
                                                        }
                                                        catch
                                                        {
                                                            success = conservazione.InsertDettExecutionPolicy(result[i].system_id, listaDoc[t].idProfile, null, "0", null);
                                                        }

                                                        totDocProcessati = countDocProc.ToString();

                                                        lastDocumentId = listaDoc[t].idProfile;
                                                        firstDocumentId = listaDoc[t].idProfile;

                                                        finalResultNew.docNumberProcessed = totDocProcessati;
                                                        finalResultNew.endExecutionDate = DateTime.Now.ToString();
                                                        finalResultNew.idAmm = tempAmm.systemId;
                                                        finalResultNew.idFirstDocumentProcessed = firstDocumentId;
                                                        finalResultNew.idIstanza = idIstanza;
                                                        finalResultNew.idLastDocumentProcessed = lastDocumentId;
                                                        finalResultNew.idPolicy = result[i].system_id;
                                                    }
                                                }
                                                else
                                                {
                                                    // inserisco in istanza già creata
                                                    try
                                                    {
                                                        BusinessLogic.Documenti.areaConservazioneManager.AddDocInAreaConservazioneWithPolicy(userInfo, idIstanza, listaDoc[t].idProfile, null, listaDoc[t].docNumber, "D", stato, getPolicyDocumentoValidato(result[i]));
                                                        success = conservazione.InsertDettExecutionPolicy(result[i].system_id, listaDoc[t].idProfile, null, "1", null);
                                                        countDocProc++;
                                                    }
                                                    catch
                                                    {
                                                        success = conservazione.InsertDettExecutionPolicy(result[i].system_id, listaDoc[t].idProfile, null, "0", null);
                                                    }

                                                    if (isPrimaIstanza)
                                                    {
                                                        totDocProcessati = countDocProc.ToString();

                                                        lastDocumentId = listaDoc[t].idProfile;

                                                        finalResult.docNumberProcessed = totDocProcessati;
                                                        finalResult.endExecutionDate = DateTime.Now.ToString();
                                                        finalResult.idAmm = tempAmm.systemId;
                                                        finalResult.idFirstDocumentProcessed = firstDocumentId;
                                                        finalResult.idIstanza = idIstanza;
                                                        finalResult.idLastDocumentProcessed = lastDocumentId;
                                                        finalResult.idPolicy = result[i].system_id;
                                                    }
                                                    else
                                                    {
                                                        if (finalResultNew != null)
                                                        {
                                                            totDocProcessati = countDocProc.ToString();

                                                            lastDocumentId = listaDoc[t].idProfile;

                                                            finalResultNew.docNumberProcessed = totDocProcessati;
                                                            finalResultNew.endExecutionDate = DateTime.Now.ToString();
                                                            finalResultNew.idAmm = tempAmm.systemId;
                                                            finalResultNew.idFirstDocumentProcessed = firstDocumentId;
                                                            finalResultNew.idIstanza = idIstanza;
                                                            finalResultNew.idLastDocumentProcessed = lastDocumentId;
                                                            finalResultNew.idPolicy = result[i].system_id;
                                                        }
                                                    }
                                                }

                                                #region Gestione finalResult
                                                // Se è la prima istanza oppure ne ho splittata una nuova, aggiungo l'oggetto finalResult
                                                if (isPrimaIstanza)
                                                {
                                                    if (listExecutionPolicy.Count > 0)
                                                    {
                                                        foreach (DocsPaVO.Conservazione.ExecutionPolicy ep in listExecutionPolicy)
                                                        {
                                                            if (ep.idIstanza == finalResult.idIstanza)
                                                            {
                                                                // La rimozione nel for è causa di eccezione
                                                                // popolo una lista di idIstanza che devono essere rimossi dalla lista
                                                                //listExecutionPolicy.Remove(ep);
                                                                if (!listIdToRemoveExecutionPolicy.Contains(ep.idIstanza))
                                                                    listIdToRemoveExecutionPolicy.Add(ep.idIstanza);
                                                            }
                                                        }

                                                        // Rimuovo dalla lista listExecutionPolicy gli elementi della lista listIdToRemoveExecutionPolicy
                                                        foreach (string id in listIdToRemoveExecutionPolicy)
                                                        {
                                                            // Controllo Preventivo sull'esistenza dell'elemento
                                                            if (listExecutionPolicy.Contains(listExecutionPolicy.FirstOrDefault(itm => itm.idIstanza == id)))
                                                                listExecutionPolicy.Remove(listExecutionPolicy.FirstOrDefault(itm => itm.idIstanza == id));
                                                        }

                                                        listExecutionPolicy.Add(finalResult);
                                                    }
                                                    else
                                                        // Aggiungo la lista dei risultati
                                                        listExecutionPolicy.Add(finalResult);
                                                }
                                                else
                                                {
                                                    if (finalResultNew != null)
                                                    {

                                                        if (listExecutionPolicy.Count > 0)
                                                        {
                                                            foreach (DocsPaVO.Conservazione.ExecutionPolicy ep in listExecutionPolicy)
                                                            {
                                                                if (ep.idIstanza == finalResultNew.idIstanza)
                                                                {
                                                                    // La rimozione nel for è causa di eccezione
                                                                    // popolo una lista di idIstanza che devono essere rimossi dalla lista
                                                                    //listExecutionPolicy.Remove(ep);
                                                                    if (!listIdToRemoveExecutionPolicy.Contains(ep.idIstanza))
                                                                        listIdToRemoveExecutionPolicy.Add(ep.idIstanza);
                                                                }
                                                            }

                                                            // Rimuovo dalla lista listExecutionPolicy gli elementi della lista listIdToRemoveExecutionPolicy
                                                            foreach (string id in listIdToRemoveExecutionPolicy)
                                                            {
                                                                // Controllo Preventivo sull'esistenza dell'elemento
                                                                if (listExecutionPolicy.Contains(listExecutionPolicy.FirstOrDefault(itm => itm.idIstanza == id)))
                                                                    listExecutionPolicy.Remove(listExecutionPolicy.FirstOrDefault(itm => itm.idIstanza == id));
                                                            }

                                                            listExecutionPolicy.Add(finalResultNew);
                                                        }
                                                        else
                                                            listExecutionPolicy.Add(finalResultNew);
                                                    }
                                                }
                                                #endregion
                                            }
                                            // end for
                                        }
                                        // End controllo vincolo del primo documento
                                    }
                                    // End if lista doc

                                    if (listExecutionPolicy != null && listExecutionPolicy.Count > 0)
                                    {
                                        foreach (DocsPaVO.Conservazione.ExecutionPolicy itemFinalResult in listExecutionPolicy)
                                        {
                                            conservazione.InsertExecutionPolicy(itemFinalResult);
                                        }
                                    }
                                }
                                // End if CheckPeriod
                            }
                            // End For Policy
                        }
                        #endregion

                        //6. Esegue le Policy per le stampe repertori
                        #region Policy Stampe Repertori
                        result = conservazione.GetListaPolicy(Convert.ToInt32(tempAmm.systemId), "C");
                        if (result != null && result.Length > 0)
                        {
                            for (int i = 0; i < result.Length; i++)
                            {
                                DocsPaVO.Conservazione.ExecutionPolicy execution = conservazione.GetLastExecutionPolicyByIdPolicy(result[i].system_id);
                                if (CheckPeriod(result[i], execution))
                                {
                                    //Prende i documenti interessati
                                    InfoDocumento[] listaDoc = null;
                                    InfoUtente userInfo = new InfoUtente();
                                    //DocsPaVO.Conservazione.ExecutionPolicy finalResult = new DocsPaVO.Conservazione.ExecutionPolicy();
                                    //finalResult.startExecutionDate = DateTime.Now.ToString();
                                    DocsPaVO.utente.Utente utente = BusinessLogic.Utenti.UserManager.getUtente(result[i].codiceUtente, result[i].idAmministrazione);

                                    userInfo = new DocsPaVO.utente.InfoUtente(utente, BusinessLogic.Utenti.UserManager.getRuoloByIdGruppo(result[i].idGruppo));

                                    DocsPaDB.Query_DocsPAWS.Documenti documenti = new DocsPaDB.Query_DocsPAWS.Documenti();
                                    string idLastId = "0";
                                    if (execution != null)
                                    {
                                        idLastId = execution.idLastDocumentProcessed;
                                    }

                                    listaDoc = documenti.GetListaStampePolicyConservazione(result[i], idLastId, "C");

                                    //
                                    // Lista di DocsPaVO.Conservazione.ExecutionPolicy
                                    List<DocsPaVO.Conservazione.ExecutionPolicy> listExecutionPolicy = new List<DocsPaVO.Conservazione.ExecutionPolicy>();

                                    // Lista di ID degli elementi da rimuovere nella listExecutionPolicy
                                    List<string> listIdToRemoveExecutionPolicy = new List<string>();

                                    //string firstDocumentId = "-1";
                                    //string lastDocumentId = "-1";
                                    //string idIstanza = "-1";
                                    //string totDocProcessati = "0";

                                    if (listaDoc != null && listaDoc.Length > 0)
                                    {
                                        //totDocProcessati = (listaDoc.Length).ToString();
                                        string stato = "N";
                                        //if (result[i].statoInviato)
                                        //{
                                        //    stato = "I";
                                        //}

                                        // Se posso creare istanza di conservazione la creo e faccio tutto
                                        bool vincoloPrimoOK = true;

                                        // Get Parametri validi per istanze di conservazione
                                        #region Mev CS 1.5 - F03_01 Get dei parametri
                                        //
                                        // Get Dimensione massima istanza in termini di Byte
                                        int dimMaxIstByte = BusinessLogic.Documenti.areaConservazioneManager.getDimensioneMassimaIstanze_Byte(result[i].idAmministrazione);
                                        //
                                        // Get numero massimo di doc in istanza
                                        int numDocMaxIst = BusinessLogic.Documenti.areaConservazioneManager.getDimensioneMassimaIstanze_NumDoc(result[i].idAmministrazione);
                                        //
                                        // Get percentuale di tolleranza in istanza
                                        int percentualeTolleranza = BusinessLogic.Documenti.areaConservazioneManager.getPercentualeTolleranzaDinesioneIstanze(result[i].idAmministrazione);

                                        #endregion

                                        // Verifico se posso inserire il primo elemento
                                        #region MEV CS 1.5 - F03_01 Controllo vincoli istanza per il primo elemento
                                        SchedaDocumento sd_primo = BusinessLogic.Documenti.DocManager.getDettaglioNoSecurity(userInfo, listaDoc[0].docNumber);
                                        int total_size_primo = 0;
                                        if (sd_primo != null)
                                        {
                                            int doc_size_primo = Convert.ToInt32(((DocsPaVO.documento.Documento)sd_primo.documenti[0]).fileSize);

                                            int numeroAllegati_primo = sd_primo.allegati.Count;
                                            string fileName_primo = ((DocsPaVO.documento.Documento)sd_primo.documenti[0]).fileName;
                                            string tipoFile_primo = Path.GetExtension(fileName_primo);
                                            int size_allegati_primo = 0;
                                            for (int j = 0; j < sd_primo.allegati.Count; j++)
                                            {
                                                size_allegati_primo = size_allegati_primo + Convert.ToInt32(((DocsPaVO.documento.Allegato)sd_primo.allegati[j]).fileSize);
                                            }
                                            total_size_primo = size_allegati_primo + doc_size_primo;
                                        }

                                        // Calcolo dimensioni prox nell'istanza
                                        int dimProx_primo = total_size_primo;
                                        int numdocProx_primo = 1;

                                        //
                                        // Invoco il metodo per la verifica della violazione del vincolo sul primo elemento
                                        bool vincoloDimIstViol_primo = BusinessLogic.Documenti.areaConservazioneManager.isVincoloDimensioneIstanzaViolato(dimProx_primo, dimMaxIstByte, percentualeTolleranza);
                                        bool vincoloNumDocIstViol_primo = BusinessLogic.Documenti.areaConservazioneManager.isVincoloNumeroDocumentiIstanzaViolato(numdocProx_primo, numDocMaxIst);
                                        #endregion

                                        // Se posso inserire il primo doc procedo, altrimenti non faccio nulla
                                        if (vincoloDimIstViol_primo || vincoloNumDocIstViol_primo)
                                            vincoloPrimoOK = false;
                                        // Vincolo per verificare se posso inserire almeno un documento
                                        if (vincoloPrimoOK)
                                        {
                                            // vincoli per dimensioni istanza
                                            bool vincoloDimIstViol = false;
                                            bool vincoloNumDocIstViol = false;

                                            string idIstanza = "-1";
                                            bool success = false;

                                            // creo prima istanza
                                            idIstanza = BusinessLogic.Documenti.areaConservazioneManager.CreateIstanzaConservazione(userInfo, result[i].nome, string.Empty, result[i].system_id, stato, result[i].consolidazione, result[i].tipoConservazione);

                                            #region Modifiche F02_01 controllo formati
                                            //controllo se la policy prevede l'invio e la conversione in automatico dell'istanza 
                                            if (!string.IsNullOrEmpty(idIstanza))
                                            {
                                                if (result[i].statoInviato)
                                                    idIstanzeInvioAutomatico.Add(idIstanza);

                                                if (result[i].statoConversione)
                                                    idIstanzeConvAutomatica.Add(idIstanza);

                                            }
                                            //if (stato == "I")
                                            //{
                                            //    BusinessLogic.UserLog.UserLog.getInstance().WriteLog(userInfo, "INVIO_ISTANZA", idIstanza, String.Format("Invio automatico in conservazione dell' istanza {0}", idIstanza), logResponse);
                                            //}
                                            #endregion

                                            //Crea una nuova istanza di conservazione
                                            string firstDocumentId = "-1";
                                            string lastDocumentId = "-1";

                                            string totDocProcessati = "0";
                                            // Contatore per il numero di documenti processati
                                            int countDocProc = 0;

                                            // Primo documento
                                            firstDocumentId = listaDoc[0].idProfile;

                                            // Oggetto finalResult
                                            DocsPaVO.Conservazione.ExecutionPolicy finalResult = new DocsPaVO.Conservazione.ExecutionPolicy();
                                            finalResult.startExecutionDate = DateTime.Now.ToString();

                                            bool isPrimaIstanza = true;
                                            DocsPaVO.Conservazione.ExecutionPolicy finalResultNew = null;

                                            for (int t = 0; t < listaDoc.Length; t++)
                                            {
                                                //
                                                // Controllo sempre se il prossimo elemento viola il vincolo delle istanze
                                                #region MEV CS 1.5 Controllo vincoli Prossimo elemento
                                                SchedaDocumento sd_prox = BusinessLogic.Documenti.DocManager.getDettaglioNoSecurity(userInfo, listaDoc[t].docNumber);
                                                int total_size_prox = 0;
                                                if (sd_prox != null)
                                                {
                                                    int doc_size_prox = Convert.ToInt32(((DocsPaVO.documento.Documento)sd_prox.documenti[0]).fileSize);

                                                    int numeroAllegati_prox = sd_prox.allegati.Count;
                                                    string fileName_prox = ((DocsPaVO.documento.Documento)sd_prox.documenti[0]).fileName;
                                                    string tipoFile_prox = Path.GetExtension(fileName_prox);
                                                    int size_allegati_prox = 0;
                                                    for (int j = 0; j < sd_prox.allegati.Count; j++)
                                                    {
                                                        size_allegati_prox = size_allegati_prox + Convert.ToInt32(((DocsPaVO.documento.Allegato)sd_prox.allegati[j]).fileSize);
                                                    }
                                                    total_size_prox = size_allegati_prox + doc_size_prox;
                                                }

                                                //
                                                // Get sizeItem e numDoc in Istanza di Conservazione
                                                int dimCorrenteInIstanza_prox = 0;
                                                int numDocCorrentiInIstanza_prox = 0;

                                                dimCorrenteInIstanza_prox = BusinessLogic.Documenti.areaConservazioneManager.getDimensioneIstanza_Byte(idIstanza);
                                                numDocCorrentiInIstanza_prox = BusinessLogic.Documenti.areaConservazioneManager.getNumeroDocIstanza(idIstanza);

                                                // Calcolo dimensioni prox nell'istanza
                                                int dimProx_prox = dimCorrenteInIstanza_prox + total_size_prox;
                                                int numdocProx_prox = numDocCorrentiInIstanza_prox + 1;

                                                //
                                                // Invoco il metodo per la verifica della violazione del vincolo
                                                vincoloDimIstViol = BusinessLogic.Documenti.areaConservazioneManager.isVincoloDimensioneIstanzaViolato(dimProx_prox, dimMaxIstByte, percentualeTolleranza);
                                                vincoloNumDocIstViol = BusinessLogic.Documenti.areaConservazioneManager.isVincoloNumeroDocumentiIstanzaViolato(numdocProx_prox, numDocMaxIst);
                                                #endregion

                                                // Se vincolo violato per documento i-esimo, creo istanza e inserisco
                                                if (vincoloDimIstViol || vincoloNumDocIstViol)
                                                {
                                                    // Se posso creare istanza di conservazione la creo e faccio tutto
                                                    bool vincolo_i_esimoOK = true;
                                                    isPrimaIstanza = false;

                                                    #region Controllo se i-esimo elemento viola vincolo
                                                    SchedaDocumento sd_i_esimo = BusinessLogic.Documenti.DocManager.getDettaglioNoSecurity(userInfo, listaDoc[t].docNumber);
                                                    int total_size_i_esimo = 0;
                                                    if (sd_i_esimo != null)
                                                    {
                                                        int doc_size_i_esimo = Convert.ToInt32(((DocsPaVO.documento.Documento)sd_i_esimo.documenti[0]).fileSize);

                                                        int numeroAllegati_i_esimo = sd_i_esimo.allegati.Count;
                                                        string fileName_i_esimo = ((DocsPaVO.documento.Documento)sd_i_esimo.documenti[0]).fileName;
                                                        string tipoFile_i_esimo = Path.GetExtension(fileName_i_esimo);
                                                        int size_allegati_i_esimo = 0;
                                                        for (int j = 0; j < sd_i_esimo.allegati.Count; j++)
                                                        {
                                                            size_allegati_i_esimo = size_allegati_i_esimo + Convert.ToInt32(((DocsPaVO.documento.Allegato)sd_i_esimo.allegati[j]).fileSize);
                                                        }
                                                        total_size_i_esimo = size_allegati_i_esimo + doc_size_i_esimo;
                                                    }

                                                    // Calcolo dimensioni prox nell'istanza
                                                    int dimProx_i_esimo = total_size_i_esimo;
                                                    int numdocProx_i_esimo = 1;

                                                    //
                                                    // Invoco il metodo per la verifica della violazione del vincolo sul primo elemento
                                                    bool vincoloDimIstViol_i_esimo = BusinessLogic.Documenti.areaConservazioneManager.isVincoloDimensioneIstanzaViolato(dimProx_i_esimo, dimMaxIstByte, percentualeTolleranza);
                                                    bool vincoloNumDocIstViol_i_esimo = BusinessLogic.Documenti.areaConservazioneManager.isVincoloNumeroDocumentiIstanzaViolato(numdocProx_i_esimo, numDocMaxIst);

                                                    if (vincoloDimIstViol_i_esimo || vincoloNumDocIstViol_i_esimo)
                                                        vincolo_i_esimoOK = false;
                                                    #endregion

                                                    if (vincolo_i_esimoOK)
                                                    {
                                                        // Oggetto finalResult
                                                        finalResultNew = new DocsPaVO.Conservazione.ExecutionPolicy();
                                                        finalResultNew.startExecutionDate = DateTime.Now.ToString();

                                                        // creo istanza
                                                        idIstanza = BusinessLogic.Documenti.areaConservazioneManager.CreateIstanzaConservazione(userInfo, result[i].nome, string.Empty, result[i].system_id, stato, result[i].consolidazione, result[i].tipoConservazione);

                                                        // creo una nuova istanza, quindi azzero per questa nuova istanza il numero di doc processati
                                                        countDocProc = 0;

                                                        #region Modifiche F02_01 controllo formati
                                                        //controllo se la policy prevede l'invio e la conversione in automatico dell'istanza 
                                                        if (!string.IsNullOrEmpty(idIstanza))
                                                        {
                                                            if (result[i].statoInviato)
                                                                idIstanzeInvioAutomatico.Add(idIstanza);

                                                            if (result[i].statoConversione)
                                                                idIstanzeConvAutomatica.Add(idIstanza);

                                                        }
                                                        //if (string.IsNullOrEmpty(idIstanza))
                                                        //{
                                                        //    //return;
                                                        //}

                                                        //if (stato == "I")
                                                        //{
                                                        //    BusinessLogic.UserLog.UserLog.getInstance().WriteLog(userInfo, "INVIO_ISTANZA", idIstanza, String.Format("Invio automatico in conservazione dell' istanza {0}", idIstanza), logResponse);
                                                        //}
                                                        #endregion

                                                        // inserisco in istanza creata
                                                        try
                                                        {
                                                            BusinessLogic.Documenti.areaConservazioneManager.AddDocInAreaConservazioneWithPolicy(userInfo, idIstanza, listaDoc[t].idProfile, null, listaDoc[t].docNumber, "D", stato, getPolicyDocumentoValidato(result[i]));
                                                            success = conservazione.InsertDettExecutionPolicy(result[i].system_id, listaDoc[t].idProfile, null, "1", null);
                                                            countDocProc++;
                                                        }
                                                        catch
                                                        {
                                                            success = conservazione.InsertDettExecutionPolicy(result[i].system_id, listaDoc[t].idProfile, null, "0", null);
                                                        }

                                                        totDocProcessati = countDocProc.ToString();

                                                        lastDocumentId = listaDoc[t].idProfile;
                                                        firstDocumentId = listaDoc[t].idProfile;

                                                        finalResultNew.docNumberProcessed = totDocProcessati;
                                                        finalResultNew.endExecutionDate = DateTime.Now.ToString();
                                                        finalResultNew.idAmm = tempAmm.systemId;
                                                        finalResultNew.idFirstDocumentProcessed = firstDocumentId;
                                                        finalResultNew.idIstanza = idIstanza;
                                                        finalResultNew.idLastDocumentProcessed = lastDocumentId;
                                                        finalResultNew.idPolicy = result[i].system_id;
                                                    }
                                                }
                                                else
                                                {
                                                    // inserisco in istanza già creata
                                                    try
                                                    {
                                                        BusinessLogic.Documenti.areaConservazioneManager.AddDocInAreaConservazioneWithPolicy(userInfo, idIstanza, listaDoc[t].idProfile, null, listaDoc[t].docNumber, "D", stato, getPolicyDocumentoValidato(result[i]));
                                                        success = conservazione.InsertDettExecutionPolicy(result[i].system_id, listaDoc[t].idProfile, null, "1", null);
                                                        countDocProc++;
                                                    }
                                                    catch
                                                    {
                                                        success = conservazione.InsertDettExecutionPolicy(result[i].system_id, listaDoc[t].idProfile, null, "0", null);
                                                    }

                                                    if (isPrimaIstanza)
                                                    {
                                                        totDocProcessati = countDocProc.ToString();

                                                        lastDocumentId = listaDoc[t].idProfile;

                                                        finalResult.docNumberProcessed = totDocProcessati;
                                                        finalResult.endExecutionDate = DateTime.Now.ToString();
                                                        finalResult.idAmm = tempAmm.systemId;
                                                        finalResult.idFirstDocumentProcessed = firstDocumentId;
                                                        finalResult.idIstanza = idIstanza;
                                                        finalResult.idLastDocumentProcessed = lastDocumentId;
                                                        finalResult.idPolicy = result[i].system_id;
                                                    }
                                                    else
                                                    {
                                                        if (finalResultNew != null)
                                                        {
                                                            totDocProcessati = countDocProc.ToString();

                                                            lastDocumentId = listaDoc[t].idProfile;

                                                            finalResultNew.docNumberProcessed = totDocProcessati;
                                                            finalResultNew.endExecutionDate = DateTime.Now.ToString();
                                                            finalResultNew.idAmm = tempAmm.systemId;
                                                            finalResultNew.idFirstDocumentProcessed = firstDocumentId;
                                                            finalResultNew.idIstanza = idIstanza;
                                                            finalResultNew.idLastDocumentProcessed = lastDocumentId;
                                                            finalResultNew.idPolicy = result[i].system_id;
                                                        }
                                                    }
                                                }

                                                #region Gestione finalResult
                                                // Se è la prima istanza oppure ne ho splittata una nuova, aggiungo l'oggetto finalResult
                                                if (isPrimaIstanza)
                                                {
                                                    if (listExecutionPolicy.Count > 0)
                                                    {
                                                        foreach (DocsPaVO.Conservazione.ExecutionPolicy ep in listExecutionPolicy)
                                                        {
                                                            if (ep.idIstanza == finalResult.idIstanza)
                                                            {
                                                                // La rimozione nel for è causa di eccezione
                                                                // popolo una lista di idIstanza che devono essere rimossi dalla lista
                                                                //listExecutionPolicy.Remove(ep);
                                                                if (!listIdToRemoveExecutionPolicy.Contains(ep.idIstanza))
                                                                    listIdToRemoveExecutionPolicy.Add(ep.idIstanza);
                                                            }
                                                        }

                                                        // Rimuovo dalla lista listExecutionPolicy gli elementi della lista listIdToRemoveExecutionPolicy
                                                        foreach (string id in listIdToRemoveExecutionPolicy)
                                                        {
                                                            // Controllo Preventivo sull'esistenza dell'elemento
                                                            if (listExecutionPolicy.Contains(listExecutionPolicy.FirstOrDefault(itm => itm.idIstanza == id)))
                                                                listExecutionPolicy.Remove(listExecutionPolicy.FirstOrDefault(itm => itm.idIstanza == id));
                                                        }

                                                        listExecutionPolicy.Add(finalResult);
                                                    }
                                                    else
                                                        // Aggiungo la lista dei risultati
                                                        listExecutionPolicy.Add(finalResult);
                                                }
                                                else
                                                {
                                                    if (finalResultNew != null)
                                                    {

                                                        if (listExecutionPolicy.Count > 0)
                                                        {
                                                            foreach (DocsPaVO.Conservazione.ExecutionPolicy ep in listExecutionPolicy)
                                                            {
                                                                if (ep.idIstanza == finalResultNew.idIstanza)
                                                                {
                                                                    // La rimozione nel for è causa di eccezione
                                                                    // popolo una lista di idIstanza che devono essere rimossi dalla lista
                                                                    //listExecutionPolicy.Remove(ep);
                                                                    if (!listIdToRemoveExecutionPolicy.Contains(ep.idIstanza))
                                                                        listIdToRemoveExecutionPolicy.Add(ep.idIstanza);
                                                                }
                                                            }

                                                            // Rimuovo dalla lista listExecutionPolicy gli elementi della lista listIdToRemoveExecutionPolicy
                                                            foreach (string id in listIdToRemoveExecutionPolicy)
                                                            {
                                                                // Controllo Preventivo sull'esistenza dell'elemento
                                                                if (listExecutionPolicy.Contains(listExecutionPolicy.FirstOrDefault(itm => itm.idIstanza == id)))
                                                                    listExecutionPolicy.Remove(listExecutionPolicy.FirstOrDefault(itm => itm.idIstanza == id));
                                                            }

                                                            listExecutionPolicy.Add(finalResultNew);
                                                        }
                                                        else
                                                            listExecutionPolicy.Add(finalResultNew);
                                                    }
                                                }
                                                #endregion
                                            }
                                            // end for
                                        }
                                        // End controllo vincolo del primo documento
                                    }
                                    // end lista doc

                                    if (listExecutionPolicy != null && listExecutionPolicy.Count > 0)
                                    {
                                        foreach (DocsPaVO.Conservazione.ExecutionPolicy itemFinalResult in listExecutionPolicy)
                                        {
                                            conservazione.InsertExecutionPolicy(itemFinalResult);
                                        }
                                    }
                                }
                            }
                            // End for lista policy
                        }
                        #endregion

                    }
                }
                //7. Effettuo la verifica dei formati su ogni istanza con invio automatico
                #region F02_01 controllo formati e conversione in automatico
                ConservazioneManager consManger = new ConservazioneManager();
                int esitoVerifica = (int)DocsPaVO.areaConservazione.InfoConservazione.EsitoVerifica.NonEffettuata;

                foreach (string idIstanza in idIstanzeInvioAutomatico)
                {
                    try
                    {
                        //aggiorno lo stato dell'istanza in IN_VERIFICA
                        consManger.updateStatoConservazione(idIstanza, ConservazioneManager.StatoIstanza.IN_VERIFICA);
                        esitoVerifica = consManger.startCheckAndValidateIstanzaConservazione(idIstanza);

                        switch (esitoVerifica)
                        {
                            case ((int)DocsPaVO.areaConservazione.InfoConservazione.EsitoVerifica.Successo):
                                //SUCCESSO: invio al CS
                                consManger.updateStatoConservazione(idIstanza, ConservazioneManager.StatoIstanza.INVIATA);
                                //BusinessLogic.UserLog.UserLog.getInstance().WriteLog(userInfo, "INVIO_ISTANZA", idIstanza, String.Format("Invio automatico in conservazione dell' istanza {0}", idIstanza), logResponse);
                                break;

                            case ((int)DocsPaVO.areaConservazione.InfoConservazione.EsitoVerifica.DirettamenteConvertibili):
                                //DIRETTAMENTE CONVERTIBILI DALL'UTENTE ASSOCIATO ALL'ISTANZA : controllo se è prevista 
                                //la conversione e invio automatico e la avvio altrimenti non faccio nulla
                                if (idIstanzeConvAutomatica.Contains(idIstanza))
                                {
                                    consManger.updateStatoConservazione(idIstanza, ConservazioneManager.StatoIstanza.IN_CONVERSIONE);
                                    consManger.convertAndSendForConservation(idIstanza, false, context);
                                }
                                break;

                            case ((int)DocsPaVO.areaConservazione.InfoConservazione.EsitoVerifica.IndirettamenteConvertibili):
                                //NON DIRETTAMENTE CONVERTIBILI DALL'UTENTE ASSOCIATO ALL'ISTANZA : (COME I DIRETTAMENTE)
                                //poichè da questo punto di vista si omette i controlli sulla security, quindi controllo se è prevista 
                                //la conversione e invio automatico e la avvio altrimenti non faccio nulla
                                if (idIstanzeConvAutomatica.Contains(idIstanza))
                                {
                                    consManger.updateStatoConservazione(idIstanza, ConservazioneManager.StatoIstanza.IN_CONVERSIONE);
                                    consManger.convertAndSendForConservation(idIstanza, false, context);
                                }
                                break;
                        }
                    }
                    catch (Exception e)
                    {
                        consManger.updateStatoConservazione(idIstanza, ConservazioneManager.StatoIstanza.ERRORE_CONVERSIONE);
                    }
                }
                #endregion

                if (mutex != null)
                {
                    mutex.ReleaseMutex();
                    mutex.Close();
                    mutex = null;
                }
            }// fine lock o mutex
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (mutex != null)
                {
                    mutex.ReleaseMutex();
                    mutex.Close();
                    mutex = null;
                }
            }

        }
        #endregion

        /// <summary>
        /// Costruisce le info per il mask di validazione policy da associare ad un documento/fascicoli/stampe registro/stampe repertorio che risulta valido per una specifica policy 
        /// il processo di validazione automatico.
        /// </summary>
        /// <param name="policy">Oggetto policy di riferimento</param>
        /// <returns></returns>
        private static DocsPaVO.areaConservazione.ItemPolicyValidator getPolicyDocumentoValidato(DocsPaVO.Conservazione.Policy policy)
        {
            DocsPaVO.areaConservazione.ItemPolicyValidator itemPolicyValidator = new DocsPaVO.areaConservazione.ItemPolicyValidator();
            try
            {
                // filtro AooCreator
                if (!string.IsNullOrEmpty(policy.idAOO) && !(policy.idAOO).Equals("-1"))
                    itemPolicyValidator.AooCreator = DocsPaVO.areaConservazione.ItemPolicyValidator.StatusPolicyValidator.Valid;
                else
                    itemPolicyValidator.AooCreator = DocsPaVO.areaConservazione.ItemPolicyValidator.StatusPolicyValidator.Unsetting;
                // filtro Classificazione
                if (!string.IsNullOrEmpty(policy.classificazione) && !(policy.classificazione).Equals("-1"))
                    itemPolicyValidator.Classificazione = DocsPaVO.areaConservazione.ItemPolicyValidator.StatusPolicyValidator.Valid;
                else
                    itemPolicyValidator.Classificazione = DocsPaVO.areaConservazione.ItemPolicyValidator.StatusPolicyValidator.Unsetting;
                // filtro DocArrivo
                itemPolicyValidator.DocArrivo = (policy.arrivo) ? DocsPaVO.areaConservazione.ItemPolicyValidator.StatusPolicyValidator.Valid : DocsPaVO.areaConservazione.ItemPolicyValidator.StatusPolicyValidator.Unsetting;
                // filtro DocPartenza
                itemPolicyValidator.DocPartenza = (policy.partenza) ? DocsPaVO.areaConservazione.ItemPolicyValidator.StatusPolicyValidator.Valid : DocsPaVO.areaConservazione.ItemPolicyValidator.StatusPolicyValidator.Unsetting;
                // filtro Doc Interno
                itemPolicyValidator.DocInterno = (policy.interno) ? DocsPaVO.areaConservazione.ItemPolicyValidator.StatusPolicyValidator.Valid : DocsPaVO.areaConservazione.ItemPolicyValidator.StatusPolicyValidator.Unsetting;
                // filtro Doc NP
                itemPolicyValidator.DocNP = (policy.grigio) ? DocsPaVO.areaConservazione.ItemPolicyValidator.StatusPolicyValidator.Valid : DocsPaVO.areaConservazione.ItemPolicyValidator.StatusPolicyValidator.Unsetting;
                // filtro DocDataCreazione
                if (!string.IsNullOrEmpty(policy.tipoDataCreazione))
                    itemPolicyValidator.DocDataCreazione = DocsPaVO.areaConservazione.ItemPolicyValidator.StatusPolicyValidator.Valid;
                else
                    itemPolicyValidator.DocDataCreazione = DocsPaVO.areaConservazione.ItemPolicyValidator.StatusPolicyValidator.Unsetting;
                // filtro Data Protocollazione
                if (!string.IsNullOrEmpty(policy.tipoDataProtocollazione))
                    itemPolicyValidator.DocDataProtocollazione = DocsPaVO.areaConservazione.ItemPolicyValidator.StatusPolicyValidator.Valid;
                else
                    itemPolicyValidator.DocDataProtocollazione = DocsPaVO.areaConservazione.ItemPolicyValidator.StatusPolicyValidator.Unsetting;
                // filtro solo documenti digitali
                if (policy.soloDigitali)
                    itemPolicyValidator.DocDigitale = DocsPaVO.areaConservazione.ItemPolicyValidator.StatusPolicyValidator.Valid;
                else
                    itemPolicyValidator.DocDigitale = DocsPaVO.areaConservazione.ItemPolicyValidator.StatusPolicyValidator.Unsetting;
                // filtro solo documenti firmati
                if (policy.soloFirmati)
                    itemPolicyValidator.DocFirmato = DocsPaVO.areaConservazione.ItemPolicyValidator.StatusPolicyValidator.Valid;
                else
                    itemPolicyValidator.DocFirmato = DocsPaVO.areaConservazione.ItemPolicyValidator.StatusPolicyValidator.Unsetting;
                // filtro Formato Documento 
                if (policy.FormatiDocumento != null && policy.FormatiDocumento.Count > 0)
                    itemPolicyValidator.DocFormato = DocsPaVO.areaConservazione.ItemPolicyValidator.StatusPolicyValidator.Valid;
                else
                    itemPolicyValidator.DocFormato = DocsPaVO.areaConservazione.ItemPolicyValidator.StatusPolicyValidator.Unsetting;
                // filtro RF
                if (!string.IsNullOrEmpty(policy.idRf) && !(policy.idRf).Equals("-1"))
                    itemPolicyValidator.Rf_Creator = DocsPaVO.areaConservazione.ItemPolicyValidator.StatusPolicyValidator.Valid;
                else
                    itemPolicyValidator.Rf_Creator = DocsPaVO.areaConservazione.ItemPolicyValidator.StatusPolicyValidator.Unsetting;
                // filtro StatoDocumento
                if (!string.IsNullOrEmpty(policy.idStatoDiagramma) && !(policy.idStatoDiagramma).Equals("-1"))
                    itemPolicyValidator.StatoDocumento = DocsPaVO.areaConservazione.ItemPolicyValidator.StatusPolicyValidator.Valid;
                else
                    itemPolicyValidator.StatoDocumento = DocsPaVO.areaConservazione.ItemPolicyValidator.StatusPolicyValidator.Unsetting;
                // filtro Tipologia Documento (Template)
                if (!string.IsNullOrEmpty(policy.idTemplate) && !(policy.idTemplate).Equals("-1"))
                    itemPolicyValidator.TipologiaDocumento = DocsPaVO.areaConservazione.ItemPolicyValidator.StatusPolicyValidator.Valid;
                else
                    itemPolicyValidator.TipologiaDocumento = DocsPaVO.areaConservazione.ItemPolicyValidator.StatusPolicyValidator.Unsetting;
                // filtro Titolario
                itemPolicyValidator.Titolario = DocsPaVO.areaConservazione.ItemPolicyValidator.StatusPolicyValidator.Unsetting;
                // filtro UO
                if (!string.IsNullOrEmpty(policy.idUoCreatore) && !(policy.idUoCreatore).Equals("-1"))
                    itemPolicyValidator.Uo_Creator = DocsPaVO.areaConservazione.ItemPolicyValidator.StatusPolicyValidator.Valid;
                else
                    itemPolicyValidator.Uo_Creator = DocsPaVO.areaConservazione.ItemPolicyValidator.StatusPolicyValidator.Unsetting;

                return itemPolicyValidator;
            }
            catch
            {
                return null;
            }
        }



        public static bool CheckPeriod(DocsPaVO.Conservazione.Policy policy, DocsPaVO.Conservazione.ExecutionPolicy execution)
        {
            bool result = false;
            string startExecute = string.Empty;
            string endExecute = string.Empty;
            DocsPaDB.Query_DocsPAWS.PolicyConservazione conservazione = new DocsPaDB.Query_DocsPAWS.PolicyConservazione();
            int giornoOggi = Convert.ToInt32(DateTime.Now.ToString("dd"));
            int meseOggi = Convert.ToInt32(DateTime.Now.ToString("MM"));
            int annoOggi = Convert.ToInt32(DateTime.Now.ToString("yyyy"));
            int minutiOggi = Convert.ToInt32(DateTime.Now.ToString("mm"));
            int oreOggi = Convert.ToInt32(DateTime.Now.ToString("HH"));
            int secondiOggi = Convert.ToInt32(DateTime.Now.ToString("ss"));
            string nomeGiornoOggi = DateTime.Now.ToString("ddd", new CultureInfo("it-IT")).ToLower();
            //Controllo se il periodo di esecuzione è corretto
            if (policy.periodoAttivo)
            {
                if (!string.IsNullOrEmpty(policy.tipoPeriodo))
                {
                    int giornoEsecuzione = 0;
                    int meseEsecuzione = 0;
                    int annoEsecuzione = 0;
                    int oraEsecuzione = 0;
                    int minutiEsecuzione = 0;

                    //Se esiste un esecuzione della policy precedente prendo la data di inizio
                    //SAB: và gestita bene in SQL dà eccezione il formato ...
                    //cambiata query nel queryList. 





                    if (execution != null && !string.IsNullOrEmpty(execution.startExecutionDate))
                    {
                        string[] splitUno = (execution.startExecutionDate).Split(' ');
                        if (splitUno.Length > 0)
                        {
                            string data = splitUno[0];
                            string ora = splitUno[1];
                            string[] splitDue = data.Split('/');
                            int.TryParse(splitDue[0], out giornoEsecuzione);
                            int.TryParse(splitDue[1], out meseEsecuzione);
                            int.TryParse(splitDue[2], out annoEsecuzione);

                            string[] splitTre = ora.Split(':');
                            int.TryParse(splitTre[1], out minutiEsecuzione);
                            int.TryParse(splitTre[0], out oraEsecuzione);
                        }
                    }

                    if (policy.tipoPeriodo.Equals("giorno"))
                    {
                        if (giornoOggi != giornoEsecuzione && Convert.ToInt32(policy.periodoGiornalieroOre) <= oreOggi && Convert.ToInt32(policy.periodoGiornalieroMinuti) <= minutiOggi)
                        {
                            result = true;
                        }
                        else
                        {
                            double resto = (oreOggi - oraEsecuzione) / (Convert.ToInt32(policy.periodoGiornalieroNGiorni));
                            if (resto >= 1)
                            {

                                if (Convert.ToInt32(policy.periodoGiornalieroOre) < oreOggi)
                                {
                                    result = true;
                                }
                                if (Convert.ToInt32(policy.periodoGiornalieroOre) == oreOggi && Convert.ToInt32(policy.periodoGiornalieroMinuti) <= minutiOggi)
                                {
                                    result = true;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (policy.tipoPeriodo.Equals("settimana"))
                        {
                            DateTime dateValue = new DateTime();
                            string tempVal = string.Empty;
                            if (annoEsecuzione != 0 && meseEsecuzione != 0 && giornoEsecuzione != 0)
                            {
                                dateValue = new DateTime(annoEsecuzione, meseEsecuzione, giornoEsecuzione);
                                tempVal = dateValue.ToString("ddd", new CultureInfo("it-IT")).ToLower();
                            }
                            if (annoEsecuzione == annoOggi && meseEsecuzione == meseOggi && giornoEsecuzione == giornoOggi)
                            {
                                return false;
                            }
                            else
                            {
                                if ((Convert.ToInt32(policy.periodoSettimanaleOre) < oreOggi) || (Convert.ToInt32(policy.periodoSettimanaleOre) == oreOggi && Convert.ToInt32(policy.periodoSettimanaleMinuti) <= minutiOggi))
                                {
                                    if (policy.periodoSettimanaleLunedi)
                                    {
                                        if (nomeGiornoOggi.Equals("lun"))
                                        {

                                            result = true;

                                        }
                                    }
                                    if (policy.periodoSettimanaleMartedi)
                                    {
                                        if (nomeGiornoOggi.Equals("mar"))
                                        {

                                            result = true;

                                        }
                                    }
                                    if (policy.periodoSettimanaleMercoledi)
                                    {
                                        if (nomeGiornoOggi.Equals("mer"))
                                        {

                                            result = true;

                                        }
                                    }
                                    if (policy.periodoSettimanaleGiovedi)
                                    {
                                        if (nomeGiornoOggi.Equals("gio"))
                                        {

                                            result = true;

                                        }
                                    }
                                    if (policy.periodoSettimanaleVenerdi)
                                    {
                                        if (nomeGiornoOggi.Equals("ven"))
                                        {
                                            result = true;
                                        }
                                    }
                                    if (policy.periodoSettimanaleSabato)
                                    {
                                        if (nomeGiornoOggi.Equals("sab"))
                                        {

                                            result = true;

                                        }
                                    }
                                    if (policy.periodoSettimanaleDomenica)
                                    {
                                        if (nomeGiornoOggi.Equals("dom"))
                                        {

                                            result = true;

                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (policy.tipoPeriodo.Equals("mese"))
                            {
                                if ((Convert.ToInt32(policy.periodoMensileGiorni) < giornoOggi) || (Convert.ToInt32(policy.periodoMensileGiorni) == giornoOggi && (Convert.ToInt32(policy.periodoMensileOre) < oreOggi) || (Convert.ToInt32(policy.periodoMensileOre) == oreOggi && Convert.ToInt32(policy.periodoMensileMinuti) <= minutiOggi)))
                                {
                                    if (meseEsecuzione == 0)
                                    {
                                        result = true;
                                    }
                                    else
                                    {
                                        if (meseOggi != meseEsecuzione)
                                        {
                                            result = true;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                //ANNUALE
                                if ((Convert.ToInt32(policy.periodoAnnualeMese) < meseOggi) || (Convert.ToInt32(policy.periodoAnnualeMese) == meseOggi && (Convert.ToInt32(policy.periodoAnnualeGiorno) < oreOggi) || (Convert.ToInt32(policy.periodoAnnualeGiorno) == giornoOggi && Convert.ToInt32(policy.periodoAnnualeMinuti) <= minutiOggi)))
                                {
                                    if (annoEsecuzione == 0)
                                    {
                                        return true;
                                    }
                                    else
                                    {
                                        if (annoOggi != annoEsecuzione)
                                        {
                                            return true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Reperimento o creazione del Mutext per l'istanza corrente
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        private static Mutex GetOrCreateMutex(string instance)
        {
            string mutexName = instance;

            Mutex m = null;
            bool doesNotExist = false;
            bool unauthorized = false;

            // The value of this variable is set by the mutex
            // constructor. It is true if the named system mutex was
            // created, and false if the named mutex already existed.
            //
            bool mutexWasCreated = false;

            // Attempt to open the named mutex.
            try
            {
                // Open the mutex with (MutexRights.Synchronize |
                // MutexRights.Modify), to enter and release the
                // named mutex.
                //
                m = Mutex.OpenExisting(mutexName);
            }
            catch (WaitHandleCannotBeOpenedException)
            {
                Console.WriteLine("Mutex does not exist.");
                doesNotExist = true;
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine("Unauthorized access: {0}", ex.Message);
                unauthorized = true;
            }

            // There are three cases: (1) The mutex does not exist.
            // (2) The mutex exists, but the current user doesn't 
            // have access. (3) The mutex exists and the user has
            // access.
            //
            if (doesNotExist)
            {
                // The mutex does not exist, so create it.

                // Create an access control list (ACL) that denies the
                // current user the right to enter or release the 
                // mutex, but allows the right to read and change
                // security information for the mutex.
                //
                string user = Environment.UserDomainName + "\\" + Environment.UserName;
                MutexSecurity mSec = new MutexSecurity();

                MutexAccessRule rule = new MutexAccessRule(user,
                    MutexRights.Synchronize | MutexRights.Modify,
                    AccessControlType.Deny);
                mSec.AddAccessRule(rule);

                rule = new MutexAccessRule(user,
                    MutexRights.ReadPermissions | MutexRights.ChangePermissions,
                    AccessControlType.Allow);
                mSec.AddAccessRule(rule);

                // Create a Mutex object that represents the system
                // mutex named by the constant 'mutexName', with
                // initial ownership for this thread, and with the
                // specified security access. The Boolean value that 
                // indicates creation of the underlying system object
                // is placed in mutexWasCreated.
                //
                m = new Mutex(true, mutexName, out mutexWasCreated, mSec);

                // If the named system mutex was created, it can be
                // used by the current instance of this program, even 
                // though the current user is denied access. The current
                // program owns the mutex. Otherwise, exit the program.
                // 
                if (mutexWasCreated)
                {
                    Console.WriteLine("Created the mutex.");
                }
                else
                {
                    Console.WriteLine("Unable to create the mutex.");
                    throw new ApplicationException("Unable to create the mutex.");
                }

            }
            else if (unauthorized)
            {
                // Open the mutex to read and change the access control
                // security. The access control security defined above
                // allows the current user to do this.
                //
                try
                {
                    m = Mutex.OpenExisting(mutexName,
                        MutexRights.ReadPermissions | MutexRights.ChangePermissions);

                    // Get the current ACL. This requires 
                    // MutexRights.ReadPermissions.
                    MutexSecurity mSec = m.GetAccessControl();

                    string user = Environment.UserDomainName + "\\"
                        + Environment.UserName;

                    // First, the rule that denied the current user 
                    // the right to enter and release the mutex must
                    // be removed.
                    MutexAccessRule rule = new MutexAccessRule(user,
                         MutexRights.Synchronize | MutexRights.Modify,
                         AccessControlType.Deny);
                    mSec.RemoveAccessRule(rule);

                    // Now grant the user the correct rights.
                    // 
                    rule = new MutexAccessRule(user,
                        MutexRights.Synchronize | MutexRights.Modify,
                        AccessControlType.Allow);
                    mSec.AddAccessRule(rule);

                    // Update the ACL. This requires
                    // MutexRights.ChangePermissions.
                    m.SetAccessControl(mSec);

                    Console.WriteLine("Updated mutex security.");

                    // Open the mutex with (MutexRights.Synchronize 
                    // | MutexRights.Modify), the rights required to
                    // enter and release the mutex.
                    //
                    m = Mutex.OpenExisting(mutexName);

                }
                catch (UnauthorizedAccessException ex)
                {
                    Console.WriteLine("Unable to change permissions: {0}", ex.Message);
                    throw new ApplicationException(string.Format("Unable to change permissions: {0}", ex.Message));
                }
            }

            return m;
        }

        //Invia istanza in conservazione
        public static bool InviaIstanzaInConservazione(DocsPaVO.areaConservazione.ItemsConservazione[] items, DocsPaVO.utente.InfoUtente infoUtente)
        {
            bool result = false;
            try
            {
                foreach (DocsPaVO.areaConservazione.ItemsConservazione tempItem in items)
                {
                    if (!BusinessLogic.Documenti.DocumentConsolidation.IsDocumentConsoldated(infoUtente, tempItem.ID_Profile, DocumentConsolidationStateEnum.Step2))
                    {
                        BusinessLogic.Documenti.DocumentConsolidation.ConsolidateNoSecurity(infoUtente, tempItem.ID_Profile, DocumentConsolidationStateEnum.Step2);
                    }
                }
                result = true;
            }
            catch
            {

                result = false;
            }
            return result;
        }

    }
}
