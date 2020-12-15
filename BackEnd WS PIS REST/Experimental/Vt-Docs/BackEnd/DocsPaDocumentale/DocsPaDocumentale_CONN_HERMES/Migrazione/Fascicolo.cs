using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using DocsPaDocumentale.Interfaces;
using DocsPaVO.amministrazione;
using DocsPaVO.utente;
using DocsPaUtils.Data;
using log4net;
using DocsPaDocumentale_HERMES.Documentale;

namespace DocsPaDocumentale_HERMES.Migrazione
{
    /// <summary>
    /// Gestione migrazione Fascicolo
    /// </summary>
    public class Fascicolo
    {
        private static ILog logger = LogManager.GetLogger(typeof(Fascicolo));
        /// <summary>
        /// 
        /// </summary>
        private Fascicolo()
        { }

        /// <summary>
        /// 
        /// </summary>
        private static bool _interrompiMigrazione = false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="amministrazione"></param>
        public static void Interrompi(DocsPaVO.amministrazione.InfoAmministrazione amministrazione)
        {
            _interrompiMigrazione = true;
        }

        /// <summary>
        /// Implementazione della logica del task di migrazione dati dei fascicoli
        /// per una singola amministrazione DocsPa
        /// </summary>
        /// <param name="amministrazione"></param>
        /// <param name="opzioniMigrazione"></param>
        /// <returns></returns>
        /*
        public static void ImportaFascicoli(DocsPaVO.amministrazione.InfoAmministrazione amministrazione, OpzioniMigrazioneFascicolo opzioniMigrazione)
        {
            try
            {
                // 1. Connessione al sistema come utente amministratore
                string userName = DocsPaDocumentale_DOCUMENTUM.DctmServices.DctmConfigurations.GetDocumentumSuperUser();
                string password = DocsPaDocumentale_DOCUMENTUM.DctmServices.DctmConfigurations.GetDocumentumSuperUserPwd();

                Log.GetInstance(amministrazione).Write(string.Format("Connessione al sistema come utente amministratore. UserName: '{0}' - Password: '{1}'", userName, password), false);

                UserLogin.LoginResult loginResult;
                InfoUtenteAmministratore infoUtente = LoginServices.LoginAdmin(userName, password, out loginResult);

                if (loginResult == UserLogin.LoginResult.OK)
                {
                    // 2. Migrazione di tutti i fascicoli in amministrazione
                    Fascicolo.ImportaFascicoli(infoUtente, amministrazione, opzioniMigrazione);
                }
                else
                {
                    // 1a. Utente non autenticato
                    throw new ApplicationException(
                        string.Format("Errore nell'autenticazione dell'utente '{0}'. Esito:{1}",
                        userName, loginResult.ToString()));
                }
            }
            catch (Exception ex)
            {
                // Migrazione annullata
                Log.GetInstance(amministrazione).Write(ex.Message, true);
            }
            finally
            {
                Log.GetInstance(amministrazione).Flush();
            }
        }
        */
        /// <summary>
        /// 
        /// </summary>
        /// <param name="amministrazione"></param>
        /// <param name="fascicoli"></param>
        public static void ImportaFascicoliSelezionati(DocsPaVO.amministrazione.InfoAmministrazione amministrazione, Migrazione.InfoFascicoloMigrazione[] fascicoli)
        {
            try
            {
                // 1. Connessione al sistema come utente amministratore
                //string userName = DocsPaDocumentale_DOCUMENTUM.DctmServices.DctmConfigurations.GetDocumentumSuperUser();
                //string password = DocsPaDocumentale_DOCUMENTUM.DctmServices.DctmConfigurations.GetDocumentumSuperUserPwd();
                string userName = "HERMES";
                string password = "HERMES";
                Log.GetInstance(amministrazione).Write(string.Format("Connessione al sistema come utente amministratore. UserName: '{0}' - Password: '{1}'", userName, password), false);

                UserLogin.LoginResult loginResult;
                InfoUtenteAmministratore infoUtente = LoginServices.LoginAdmin(userName, password, out loginResult);

                if (loginResult == UserLogin.LoginResult.OK)
                {
                    // 2. Migrazione di tutti i fascicoli in amministrazione
                    Fascicolo.ImportaFascicoliSelezionati(infoUtente, amministrazione, fascicoli);
                }
                else
                {
                    // 1a. Utente non autenticato
                    throw new ApplicationException(
                        string.Format("Errore nell'autenticazione dell'utente '{0}'. Esito:{1}",
                        userName, loginResult.ToString()));
                }
            }
            catch (Exception ex)
            {
                // Migrazione annullata
                Log.GetInstance(amministrazione).Write(ex.Message, true);
            }
            finally
            {
                Log.GetInstance(amministrazione).Flush();
            }
        }

        /// <summary>
        /// Aggiornamento delle associazioni fascicoli / documenti
        /// </summary>
        /// <param name="amministrazione"></param>
        /// <param name="opzioniMigrazione"></param>
        public static void AggiornaAssociazioniFascicoliDocumenti(DocsPaVO.amministrazione.InfoAmministrazione amministrazione, OpzioniMigrazioneFascicolo opzioniMigrazione)
        {
            try
            {
                // 1. Connessione al sistema come utente amministratore
                //string userName = DocsPaDocumentale_DOCUMENTUM.DctmServices.DctmConfigurations.GetDocumentumSuperUser();
                //string password = DocsPaDocumentale_DOCUMENTUM.DctmServices.DctmConfigurations.GetDocumentumSuperUserPwd();
                
                string userName = "HERMES";
                string password = "HERMES";

                Log.GetInstance(amministrazione).Write(string.Format("Connessione al sistema come utente amministratore. UserName: '{0}' - Password: '{1}'", userName, password), false);
            
                UserLogin.LoginResult loginResult;
                InfoUtenteAmministratore infoUtente = LoginServices.LoginAdmin(userName, password, out loginResult);

                if (loginResult == UserLogin.LoginResult.OK)
                {
                    // Viene assegnato all'infoutente l'id dell'amministrazione fornita come parametro
                    // per fare in modo che l'utente che esegue la migrazione si impersonifichi 
                    // come utente dell'amministrazione
                    string idAmm = infoUtente.idAmministrazione;
                    infoUtente.idAmministrazione = amministrazione.IDAmm;

                    ProjectManager projectManager = new ProjectManager(infoUtente);
                    
                    // 2. Reperimento di tutti i fascicoli in PITRE
                    using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                    {
                        using (DataSet ds = new DataSet())
                        {
                            // 1. Reperimento fascicoli per il titolario
                            if (dbProvider.ExecuteQuery(ds, GetQueryFascicoli(infoUtente, opzioniMigrazione.Filtro)))
                            {
                                Log.GetInstance(amministrazione).Write("Reperimento fascicoli in amministrazione.", false);

                                int index = 1;

                                foreach (DataRow row in ds.Tables[0].Rows)
                                {
                                    if (_interrompiMigrazione)
                                    {
                                        _interrompiMigrazione = false;
                                        string log = string.Format("Aggiornamento associazione fascicoli documenti al fascicolo {0} di {1}", index.ToString(), ds.Tables[0].Rows.Count);
                                        Log.GetInstance(amministrazione).Write(log, false);
                                        logger.Debug(log);
                                        break;
                                    }

                                    DocsPaVO.fascicolazione.Fascicolo fascicolo = GetFascicolo(row, infoUtente);

                                    // 2. Aggiornamento fascicolazioni del fascicolo
                                    //AggiornaAssociazioniFascicoliDocumenti(infoUtente, amministrazione, fascicolo, projectManager);
                                     
                                    index++;                                    
                                }
                            }
                            else
                                // 1a. Errore nel reperimento dei fascicoli
                                throw new ApplicationException(
                                    string.Format("Si è verificato un errore nel reperimento dei fascicolo per l'amministrazione '{0}'",
                                    amministrazione.Codice));
                        }
                    }

                    infoUtente.idAmministrazione = idAmm;
                }
                else
                {
                    // 1a. Utente non autenticato
                    throw new ApplicationException(
                        string.Format("Errore nell'autenticazione dell'utente '{0}'. Esito:{1}",
                        userName, loginResult.ToString()));
                }
            }
            catch (Exception ex)
            {
                // Migrazione annullata
                Log.GetInstance(amministrazione).Write(ex.Message, true);
            }
            finally
            {
                Log.GetInstance(amministrazione).Flush();
            }
        }

        /// <summary>
        /// Aggiornamento delle associazioni fascicoli / documenti
        /// </summary>
        /// <param name="amministrazione"></param>
        /// <param name="fascicoli"></param>
        public static void AggiornaAssociazioniFascicoliSelezionatiDocumenti(DocsPaVO.amministrazione.InfoAmministrazione amministrazione, Migrazione.InfoFascicoloMigrazione[] fascicoli)
        {
            try
            {
                // 1. Connessione al sistema come utente amministratore
                //string userName = DocsPaDocumentale_DOCUMENTUM.DctmServices.DctmConfigurations.GetDocumentumSuperUser();
                //string password = DocsPaDocumentale_DOCUMENTUM.DctmServices.DctmConfigurations.GetDocumentumSuperUserPwd();

                string userName = "HERMES";
                string password = "HERMES";

                Log.GetInstance(amministrazione).Write(string.Format("Connessione al sistema come utente amministratore. UserName: '{0}' - Password: '{1}'", userName, password), false);

                UserLogin.LoginResult loginResult;
                InfoUtenteAmministratore infoUtente = LoginServices.LoginAdmin(userName, password, out loginResult);

                if (loginResult == UserLogin.LoginResult.OK)
                {
                    // Viene assegnato all'infoutente l'id dell'amministrazione fornita come parametro
                    // per fare in modo che l'utente che esegue la migrazione si impersonifichi 
                    // come utente dell'amministrazione
                    string idAmm = infoUtente.idAmministrazione;
                    infoUtente.idAmministrazione = amministrazione.IDAmm;

                    ProjectManager projectManager = new ProjectManager(infoUtente);

                    int index = 1;

                    foreach (Migrazione.InfoFascicoloMigrazione item in fascicoli)
                    {
                        if (_interrompiMigrazione)
                        {
                            _interrompiMigrazione = false;
                            Log.GetInstance(amministrazione).Write(string.Format("Aggiornamento associazione fascicoli documenti al fascicolo {0} di {1}", index.ToString(), fascicoli.Length), false);
                            break;
                        }

                        //infoUtente.idGruppo = DocsPaDocumentale_DOCUMENTUM.DocsPaServices.DocsPaQueryHelper.getRuoloCreatore(item.Id).idGruppo;
                        infoUtente.idGruppo = null;
                        DocsPaVO.fascicolazione.Fascicolo fascicolo = GetFascicolo(item.Id, infoUtente);

                        // 2. Aggiornamento fascicolazioni del fascicolo
                        //AggiornaAssociazioniFascicoliDocumenti(infoUtente, amministrazione, fascicolo, projectManager);

                        index++;
                    }

                    infoUtente.idAmministrazione = idAmm;
                }
                else
                {
                    // 1a. Utente non autenticato
                    throw new ApplicationException(
                        string.Format("Errore nell'autenticazione dell'utente '{0}'. Esito:{1}",
                        userName, loginResult.ToString()));
                }
            }
            catch (Exception ex)
            {
                // Migrazione annullata
                Log.GetInstance(amministrazione).Write(ex.Message, true);
            }
            finally
            {
                Log.GetInstance(amministrazione).Flush();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="amministrazione"></param>
        /// <param name="folder"></param>
        /// <param name="projectManager"></param>
        /*private static void AggiornaAssociazioniFolderDocumenti(DocsPaVO.utente.InfoUtente infoUtente, 
                    DocsPaVO.amministrazione.InfoAmministrazione amministrazione, 
                    DocsPaVO.fascicolazione.Folder folder, 
                    DocsPaDocumentale_DOCUMENTUM.Documentale.ProjectManager projectManager)
        {
            // 3. Aggiornamento fascicolazioni per tutti i sottofascicoli contenuti
            foreach (DocsPaVO.fascicolazione.Folder fld in folder.childs)
            {
                if (projectManager.UpdateFascicolazioni(fld.systemID))
                    Log.GetInstance(amministrazione).Write(string.Format("Aggiornamento fascicolazioni per il sottofascicolo con id '{0}'", fld.systemID), false);
                else
                    Log.GetInstance(amministrazione).Write(string.Format("Errore nell'aggiornamento fascicolazioni per il sottofascicolo con id '{0}'", fld.systemID), false);

                AggiornaAssociazioniFolderDocumenti(infoUtente, amministrazione, fld, projectManager);
            }
        }
        */
        /// <summary>
        /// 
        /// </summary>
        /// <param name="amministrazione"></param>
        /// <param name="fascicolo"></param>
        /// <param name="projectManager"></param>
        /*
        private static void AggiornaAssociazioniFascicoliDocumenti(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.amministrazione.InfoAmministrazione amministrazione, DocsPaVO.fascicolazione.Fascicolo fascicolo, DocsPaDocumentale_DOCUMENTUM.Documentale.ProjectManager projectManager)
        {
            // 2. Aggiornamento fascicolazioni del fascicolo
            if (projectManager.UpdateFascicolazioni(fascicolo.systemID))
            {
                string log = string.Format("Aggiornamento fascicolazioni per il fascicolo con id '{0}' e codice '{1}'", fascicolo.systemID, fascicolo.codice);
                Log.GetInstance(amministrazione).Write(log, false);
                logger.Debug(log);

                DocsPaVO.fascicolazione.Folder[] folders = GetFolders(infoUtente, fascicolo);

                // 3. Aggiornamento fascicolazioni per tutti i sottofascicoli contenuti
                foreach (DocsPaVO.fascicolazione.Folder folder in folders)
                {
                    if (projectManager.UpdateFascicolazioni(folder.systemID))
                        Log.GetInstance(amministrazione).Write(string.Format("Aggiornamento fascicolazioni per il sottofascicolo con id '{0}'", folder.systemID), false);
                    else
                        Log.GetInstance(amministrazione).Write(string.Format("Errore nell'aggiornamento fascicolazioni per il sottofascicolo con id '{0}'", folder.systemID), false);

                    AggiornaAssociazioniFolderDocumenti(infoUtente, amministrazione, folder, projectManager);
                }
            }
            else
                Log.GetInstance(amministrazione).Write(string.Format("Errore nell'aggiornamento fascicolazioni per il fascicolo con id '{0}'", fascicolo.systemID), false);
        }
        */
        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="amministrazione"></param>
        /// <param name="fascicoli"></param>
        private static void ImportaFascicoliSelezionati(InfoUtente infoUtente, DocsPaVO.amministrazione.InfoAmministrazione amministrazione, Migrazione.InfoFascicoloMigrazione[] fascicoli)
        {
            InfoStatoMigrazione statoMigrazione = StatoMigrazione.Get(amministrazione);

            try
            {
                // Viene assegnato all'infoutente l'id dell'amministrazione fornita come parametro
                // per fare in modo che l'utente che esegue la migrazione si impersonifichi 
                // come utente dell'amministrazione
                string idAmm = infoUtente.idAmministrazione;
                infoUtente.idAmministrazione = amministrazione.IDAmm;

                int index = 1;

                foreach (Migrazione.InfoFascicoloMigrazione infoFascicolo in fascicoli)
                {
                    if (_interrompiMigrazione)
                    {
                        _interrompiMigrazione = false;
                        Log.GetInstance(amministrazione).Write(string.Format("Migrazione fascicoli interrotta al fascicolo {0} di {1}", index.ToString(), fascicoli.Length), false);
                        break;
                    }

                    // 1. Reperimento oggetto fascicolo
                    //infoUtente.idGruppo = DocsPaDocumentale_DOCUMENTUM.DocsPaServices.DocsPaQueryHelper.getRuoloCreatore(infoFascicolo.Id).idGruppo;
                    infoUtente.idGruppo = "HERMES";
                    DocsPaVO.fascicolazione.Fascicolo fascicolo = GetFascicolo(infoFascicolo.Id, infoUtente);

                    // 2. Migrazione dei singoli fascicoli
                    ImportaFascicolo(fascicolo, infoUtente, amministrazione, statoMigrazione);

                    index++;
                }

                infoUtente.idAmministrazione = idAmm;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                // Save dello stato migrazione
                StatoMigrazione.Save(statoMigrazione, amministrazione);
            }
        }

        /// <summary>
        /// Implementazione della logica del task di migrazione dati
        /// per tutti i fascicoli di un'amministrazione
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="amministrazione"></param>
        /// <param name="opzioniMigrazione"></param>
        private static void ImportaFascicoli(InfoUtente infoUtente, InfoAmministrazione amministrazione, OpzioniMigrazioneFascicolo opzioniMigrazione)
        {
            InfoStatoMigrazione statoMigrazione = StatoMigrazione.Get(amministrazione);
                        
            try
            {
                using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                {
                    // Viene assegnato all'infoutente l'id dell'amministrazione fornita come parametro
                    // per fare in modo che l'utente che esegue la migrazione si impersonifichi 
                    // come utente dell'amministrazione
                    string idAmm = infoUtente.idAmministrazione;
                    infoUtente.idAmministrazione = amministrazione.IDAmm;
                    
                    using (DataSet ds = new DataSet())
                    {
                        // 1. Reperimento fascicoli per il titolario
                        if (dbProvider.ExecuteQuery(ds, GetQueryFascicoli(infoUtente, opzioniMigrazione.Filtro)))
                        {
                            Log.GetInstance(amministrazione).Write("Reperimento fascicoli in amministrazione.", false);

                            int index = 1;

                            foreach (DataRow row in ds.Tables[0].Rows)
                            {
                                if (_interrompiMigrazione)
                                {
                                    _interrompiMigrazione = false;
                                    Log.GetInstance(amministrazione).Write(string.Format("Migrazione fascicoli interrotta al fascicolo {0} di {1}", index.ToString(), ds.Tables[0].Rows.Count), false);
                                    break;
                                }

                                // 2. Migrazione dei singoli fascicoli
                                ImportaFascicolo(row, infoUtente, amministrazione, statoMigrazione);

                                index++;
                            }
                        }
                        else
                            // 1a. Errore nel reperimento dei fascicoli
                            throw new ApplicationException(
                                string.Format("Si è verificato un errore nel reperimento dei fascicolo per l'amministrazione '{0}'",
                                amministrazione.Codice));
                    }

                    infoUtente.idAmministrazione = idAmm;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                // Save dello stato migrazione
                StatoMigrazione.Save(statoMigrazione, amministrazione);
            }
        }

        /// <summary>
        /// Migrazione di un singolo fascicolo
        /// </summary>
        /// <param name="fascicolo"></param>
        /// <param name="infoUtente"></param>
        /// <param name="amministrazione"></param>
        /// <param name="statoMigrazione"></param>
        private static void ImportaFascicolo(DocsPaVO.fascicolazione.Fascicolo fascicolo, InfoUtente infoUtente, InfoAmministrazione amministrazione, InfoStatoMigrazione statoMigrazione)
        {
            
            //DocsPaDocumentale_DOCUMENTUM.Documentale.ProjectManager projectManager = new DocsPaDocumentale_DOCUMENTUM.Documentale.ProjectManager(infoUtente);
            //DocsPaDocumentale.Interfaces.IAclEventListener aclEventListener = new DocsPaDocumentale_DOCUMENTUM.Documentale.AclEventListener(infoUtente);

            // 1. Reperimento del ruolo creatore del fascicolo
            //DocsPaVO.utente.Ruolo ruolo = DocsPaDocumentale_DOCUMENTUM.DocsPaServices.DocsPaQueryHelper.getRuoloCreatore(fascicolo.systemID);
            DocsPaVO.utente.Ruolo ruolo = null;
            // NB: il reperimento dei fascicoli generali serve solo per migrare
            // gli eventuali sottofascicoli, non per migrare il fascicolo stesso,
            // che è già stato creato in sede di creazione del nodo di titolario
            if (fascicolo.tipo == "G")
            {
                // 2. Migrazione dei sottofascicoli, in caso di fascicolo generale (già creato in sede di creazione del titolario)
                ImportaSottofascicoli(infoUtente, amministrazione, fascicolo, null, ruolo, statoMigrazione);
            }
            else
            {
                InfoFascicoloMigrazione fascicoloMigrazione = new InfoFascicoloMigrazione(fascicolo);

                // Calcolo dell'hash del fascicolo
                fascicoloMigrazione.HashFascicolo = GetHashFascicolo(fascicolo,true);

                // 2. Creazione del corrispondente fascicolo in documentum
                //DocsPaVO.fascicolazione.ResultCreazioneFascicolo result;
                //DocsPaVO.utente.Ruolo[] ruoliSuperiori;
                /*
                if (projectManager.ContainsFascicoloProcedimentale(fascicolo.systemID))
                {
                    // Fascicolo è già esistente, save dei dati con refresh delle entries dell'acl associata
                    fascicoloMigrazione.EsitoMigrazione = projectManager.ModifyProject(fascicolo, true);

                    if (!fascicoloMigrazione.EsitoMigrazione)
                        fascicoloMigrazione.ErroreMigrazione = string.Format("Si è verificato un errore nella modifica del fascicolo '{0}' per l'amministrazione '{1}'", fascicolo.codice, amministrazione.Codice);
                    else
                        // Migrazione dei sottofascicoli
                        ImportaSottofascicoli(infoUtente, amministrazione, fascicolo, null, ruolo, statoMigrazione);

                    Log.GetInstance(amministrazione).Write(string.Format("Migrazione fascicolo. Codice: '{0}' - Descrizione: '{1}'. Aggiornamento.", fascicolo.codice, fascicolo.descrizione), false);
                }
                else if (projectManager.CreateProject(null,
                                                fascicolo,
                                                ruolo,
                                                false,
                                                out result,
                                                out ruoliSuperiori))
                {
                    if (result == DocsPaVO.fascicolazione.ResultCreazioneFascicolo.OK)
                    {
                        fascicoloMigrazione.EsitoMigrazione = true;

                        Log.GetInstance(amministrazione).Write(string.Format("Migrazione fascicolo. Codice: '{0}' - Descrizione: '{1}'", fascicolo.codice, fascicolo.descrizione), false);

                        // 3. Notifica evento di creazione fascicolo completato
                        aclEventListener.FascicoloCreatoEventHandler(null, fascicolo, ruolo, ruoliSuperiori);

                        try
                        {
                            // 4. Migrazione dei sottofascicoli
                            ImportaSottofascicoli(infoUtente, amministrazione, fascicolo, null, ruolo, statoMigrazione);
                        }
                        catch (Exception ex)
                        {
                            //4a. Errore nella migrazione dei sottofascicoli, viene rimosso il fascicolo creato
                            ((DocsPaDocumentale_DOCUMENTUM.Documentale.ProjectManager)projectManager).DeleteProject(fascicolo);

                            fascicoloMigrazione.EsitoMigrazione = false;
                            fascicoloMigrazione.ErroreMigrazione = ex.Message;
                        }

                        // 5. Impostazione ownership del fascicolo
                        string utenteCreatore = DocsPaDocumentale_DOCUMENTUM.DocsPaObjectTypes.TypeUtente.NormalizeUserName(DocsPaDocumentale_DOCUMENTUM.DocsPaServices.DocsPaQueryHelper.getUtenteCreatore(fascicolo.systemID).userId);
                        ((DocsPaDocumentale_DOCUMENTUM.Documentale.ProjectManager)projectManager).SetOwnershipFascicolo(fascicolo.systemID, utenteCreatore);
                    }
                    else
                    {
                        // 3a. Errore nella creazione del fascicolo
                        fascicoloMigrazione.EsitoMigrazione = false;
                        fascicoloMigrazione.ErroreMigrazione = string.Format("Si è verificato un errore nella creazione del fascicolo '{0}' per l'amministrazione '{1}'", fascicolo.codice, amministrazione.Codice);
                    }
                }*/

                fascicoloMigrazione.DataMigrazione = DateTime.Now.ToString();
                statoMigrazione.SetFascicoloMigrazione(fascicoloMigrazione);
            }
            
        }

        /// <summary>
        /// Migrazione di un singolo fascicolo
        /// </summary>
        /// <param name="row"></param>
        /// <param name="infoUtente"></param>
        /// <param name="amministrazione"></param>
        /// <param name="statoMigrazione"></param>
        private static void ImportaFascicolo(DataRow row, InfoUtente infoUtente, InfoAmministrazione amministrazione, InfoStatoMigrazione statoMigrazione)
        {
            // 1. Reperimento oggetto fascicolo
            DocsPaVO.fascicolazione.Fascicolo fascicolo = GetFascicolo(row, infoUtente);

            // 2. Migrazione fascicolo
            ImportaFascicolo(fascicolo, infoUtente, amministrazione, statoMigrazione);
        }

        /// <summary>
        /// Creazione hash del fascicolo
        /// </summary>
        /// <param name="fascicolo"></param>
        /// <returns></returns>
        private static string GetHashFascicolo(DocsPaVO.fascicolazione.Fascicolo fascicolo,bool sha256)
        {
            string retValue = string.Empty;

            BinaryFormatter formatter = new BinaryFormatter();

            FascicoloHash fascicoloHash = new FascicoloHash();
            fascicoloHash.Fascicolo = fascicolo;
            fascicoloHash.Sottofascicoli = GetListaSottofascicoli(fascicolo.systemID);
            //fascicoloHash.SecurityItems = DocsPaDocumentale_DOCUMENTUM.DocsPaServices.DocsPaQueryHelper.getSecurityItems(fascicolo.systemID);

            using (MemoryStream stream = new MemoryStream())
            {
                formatter.Serialize(stream, fascicoloHash);
                stream.Position = 0;

                byte[] data = new byte[stream.Length];
                stream.Write(data, 0, data.Length);
                if (sha256)
                {
                    retValue = DocsPaUtils.Security.CryptographyManager.CalcolaImpronta256(data);
                }
                else
                {
                    retValue = DocsPaUtils.Security.CryptographyManager.CalcolaImpronta(data);
                }
            }

            return retValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idFascicolo"></param>
        /// <returns></returns>
        private static string[] GetListaSottofascicoli(string idFascicolo)
        {
            List<string> list = new List<string>();

            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                using (IDataReader reader = dbProvider.ExecuteReader(GetQueryFolders(null, idFascicolo)))
                {
                    while (reader.Read())
                        list.Add(DataReaderHelper.GetValue<object>(reader, "system_id", false).ToString());
                }
            }

            return list.ToArray();
        }

        /// <summary>
        /// Import dei sottofascicoli per un fascicolo
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="amministrazione"></param>
        /// <param name="fascicolo"></param>
        /// <param name="ruolo"></param>
        /// <param name="statoMigrazione"></param>
        /// <param name="folders"></param>
        private static void ImportaSottofascicoli(InfoUtente infoUtente, InfoAmministrazione amministrazione, DocsPaVO.fascicolazione.Fascicolo fascicolo, DocsPaVO.fascicolazione.Folder[] folders, DocsPaVO.utente.Ruolo ruolo, InfoStatoMigrazione statoMigrazione)
        {
            //DocsPaDocumentale_DOCUMENTUM.Documentale.ProjectManager projectManager = new DocsPaDocumentale_DOCUMENTUM.Documentale.ProjectManager(infoUtente);

            // Fascicolo precedentementei importato
            DocsPaVO.fascicolazione.Folder previousFolder = null;
            int countRenamed = 0;

            if (folders == null)
                folders = GetFolders(infoUtente, fascicolo);
            /*
            // 1. Reperimento sottofascicoli contenuti nel fascicolo
            foreach (DocsPaVO.fascicolazione.Folder folder in folders)
            {   
                if (projectManager.ContainsSottofascicolo(folder.systemID))
                {
                    // Inserimento dei metadati di migrazione per il folder
                    InfoFascicoloMigrazione infoFascicolo = new InfoFascicoloMigrazione(fascicolo, folder);

                    // Sottofascicolo è già esistente, save dei dati
                    infoFascicolo.EsitoMigrazione = projectManager.ModifyFolder(folder);

                    if (!infoFascicolo.EsitoMigrazione)
                        infoFascicolo.ErroreMigrazione = string.Format("Errore nella modifica del sottofascicolo: '{0}'", folder.descrizione);
                    
                    statoMigrazione.SetFascicoloMigrazione(infoFascicolo);

                    if (folder.childs.Count > 0)
                    {
                        // Import dei sottofascicoli figli
                        ImportaSottofascicoli(infoUtente, amministrazione, fascicolo,
                                            (DocsPaVO.fascicolazione.Folder[])folder.childs.ToArray(typeof(DocsPaVO.fascicolazione.Folder)),
                                            ruolo, statoMigrazione);
                    }

                    // Sottofascicolo già esistente
                    Log.GetInstance(amministrazione).Write(string.Format("Sottofascicolo con codice {0} già esistente. Aggiornamento.", folder.descrizione), false);
                }
                else
                {
                    // Il sottofascicolo non ha descrizione, viene impostata la system_id
                    if (string.IsNullOrEmpty(folder.descrizione))
                    {
                        folder.descrizione = folder.systemID;
                    }

                    string oldDescription = folder.descrizione;

                    // Verifica se, nell'ambito dello stesso fascicolo, esistono sottofascicoli con descrizione duplicata
                    // if (DocsPaDocumentale_DOCUMENTUM.DctmServices.Dfs4DocsPa.containsSottofascicoloByDescription(folder.idFascicolo, folder.descrizione))
                    // {
                        folder.descrizione = string.Format("{0} ({1})", folder.descrizione, folder.systemID);

                        // Log.GetInstance(amministrazione).Write(string.Format("Migrazione sottofascicolo. Rinominato sottofascicolo duplicato '{0}' in '{1}'", oldDescription, folder.descrizione), false);
                    // }

                    DocsPaVO.fascicolazione.ResultCreazioneFolder resultCreazioneFolder;

                    // 3. Creazione oggetto Folder
                    if (projectManager.CreateFolder(folder, ruolo, out resultCreazioneFolder))
                    {
                        // Inserimento dei metadati di migrazione per il folder
                        InfoFascicoloMigrazione infoFascicolo = new InfoFascicoloMigrazione(fascicolo, folder);
                        infoFascicolo.EsitoMigrazione = true;
                        statoMigrazione.SetFascicoloMigrazione(infoFascicolo);

                        Log.GetInstance(amministrazione).Write(string.Format("Migrazione sottofascicolo. Descrizione sottofascicolo: '{0}' - Codice: '{1}' - Descrizione: '{2}'", folder.descrizione, fascicolo.codice, fascicolo.descrizione), false);

                        folder.descrizione = oldDescription;
                        previousFolder = folder;

                        if (folder.childs.Count > 0)
                        {
                            // Import dei sottofascicoli figli
                            ImportaSottofascicoli(infoUtente, amministrazione, fascicolo,
                                                (DocsPaVO.fascicolazione.Folder[])folder.childs.ToArray(typeof(DocsPaVO.fascicolazione.Folder)),
                                                ruolo, statoMigrazione);
                        }
                    }
                    else
                    {
                        // 2a. Errore nell'inserimento del folder
                        throw new ApplicationException(
                                string.Format("Si è verificato un errore nell'inserimento del sottofascicolo '{0}' per il fascicolo '{1}' per l'amministrazione '{2}'",
                                folder.descrizione, fascicolo.codice, amministrazione.Codice));
                    }
                }
            }*/
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        private static DocsPaVO.fascicolazione.Fascicolo GetFascicolo(string id, InfoUtente infoUtente)
        {
            using (DocsPaDB.Query_DocsPAWS.Fascicoli fasc = new DocsPaDB.Query_DocsPAWS.Fascicoli())
                return fasc.GetFascicoloById(id, infoUtente);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="row"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        private static DocsPaVO.fascicolazione.Fascicolo GetFascicolo(DataRow row, InfoUtente infoUtente)
        {
            DocsPaVO.fascicolazione.Fascicolo fascicolo = new DocsPaVO.fascicolazione.Fascicolo();

            fascicolo.systemID = DataReaderHelper.GetValue<object>(row, "SYSTEM_ID", false).ToString();
            fascicolo.apertura = DataReaderHelper.GetValue<object>(row, "DTA_APERTURA", true, string.Empty).ToString();
            fascicolo.chiusura = DataReaderHelper.GetValue<object>(row, "DTA_CHIUSURA", true, string.Empty).ToString();
            fascicolo.codice = DataReaderHelper.GetValue<object>(row, "VAR_CODICE", false).ToString();
            fascicolo.descrizione = DataReaderHelper.GetValue<object>(row, "DESCRIPTION", false).ToString();
            fascicolo.stato = DataReaderHelper.GetValue<object>(row, "CHA_STATO", true, string.Empty).ToString();
            fascicolo.tipo = DataReaderHelper.GetValue<object>(row, "CHA_TIPO_FASCICOLO", false).ToString();
            fascicolo.idClassificazione = DataReaderHelper.GetValue<object>(row, "ID_PARENT", true, string.Empty).ToString();
            fascicolo.codUltimo = DataReaderHelper.GetValue<object>(row, "VAR_COD_ULTIMO", true, string.Empty).ToString();
            fascicolo.idRegistroNodoTit = DataReaderHelper.GetValue<object>(row, "ID_REGISTRO", true, string.Empty).ToString();
            fascicolo.codiceRegistroNodoTit = DataReaderHelper.GetValue<object>(row, "CODREG", true, string.Empty).ToString();
            fascicolo.idTitolario = DataReaderHelper.GetValue<object>(row, "ID_PARENT", true, string.Empty).ToString();
            fascicolo.cartaceo = (Convert.ToInt32(DataReaderHelper.GetValue<object>(row, "CARTACEO", true, 0)) > 0);
            fascicolo.privato = DataReaderHelper.GetValue<object>(row, "CHA_PRIVATO", true, string.Empty).ToString();
            
            return fascicolo;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="row"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        private static DocsPaVO.fascicolazione.Folder GetFolder(DataRow row, InfoUtente infoUtente)
        {
            DocsPaVO.fascicolazione.Folder folder = new DocsPaVO.fascicolazione.Folder();

            folder.systemID = DataReaderHelper.GetValue<object>(row, "SYSTEM_ID", false).ToString();
            folder.descrizione = DataReaderHelper.GetValue<object>(row, "DESCRIPTION", true, string.Empty).ToString();
            folder.idFascicolo = DataReaderHelper.GetValue<object>(row, "ID_FASCICOLO", false).ToString();
            folder.idParent = DataReaderHelper.GetValue<object>(row, "ID_PARENT", false).ToString();

            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                DataSet dsFolderChilds = new DataSet();

                if (dbProvider.ExecuteQuery(dsFolderChilds, GetQueryFolderChilds(infoUtente, folder.systemID)))
                {
                    foreach (DataRow rowChild in dsFolderChilds.Tables[0].Rows)
                    {
                        DocsPaVO.fascicolazione.Folder child = GetFolder(rowChild, infoUtente);

                        folder.childs.Add(child);
                    }
                }
            }

            return folder;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="fascicolo"></param>
        /// <returns></returns>
        private static DocsPaVO.fascicolazione.Folder[] GetFolders(InfoUtente infoUtente, DocsPaVO.fascicolazione.Fascicolo fascicolo)
        {
            List<DocsPaVO.fascicolazione.Folder> folders = new List<DocsPaVO.fascicolazione.Folder>();

            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                DataSet dsFolder = new DataSet();

                if (dbProvider.ExecuteQuery(dsFolder, GetQueryFolders(infoUtente, fascicolo.systemID)))
                {
                    foreach (DataRow row in dsFolder.Tables[0].Rows)
                    {
                        folders.Add(GetFolder(row, infoUtente));
                    }
                }
            }

            return folders.ToArray();
        }

        /// <summary>
        /// Reperimento query fascicoli di un titolario
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="filtro"></param>
        /// <returns></returns>
        private static string GetQueryFascicoli(InfoUtente infoUtente, FiltroMigrazioneFascicolo filtro)
        {
            string filterString = string.Empty;

            if (filtro != null)
            {
                // Impostazione parametri di filtro
                filterString = filtro.ToString();
            }

            if (!string.IsNullOrEmpty(filterString))
                filterString = string.Concat(" AND ", filterString);

            return string.Format(
                "SELECT  a.system_id, " +
                    "a.description, " +
                    "a.cha_tipo_proj, " +
                    "a.var_codice, " +
                    "a.id_amm, " +
                    "a.num_livello, " +
                    "a.cha_tipo_fascicolo, " +
                    "a.id_fascicolo, " +
                    "a.id_parent, " +
                    "a.var_cod_ultimo, " +
                    "to_char(A.DTA_APERTURA,'dd/mm/yyyy') AS dta_apertura, " +
                    "to_char(A.DTA_CHIUSURA,'dd/mm/yyyy') AS dta_chiusura, " +
                    "a.cha_stato,  " +
                    "a.id_tipo_proc, " +
                    "a.id_registro , " +
                    "getcodreg(a.id_registro) as codReg, " +
                    "a.dta_creazione,  " +
                    "a.cartaceo, " +
                    "a.cha_privato, " +
                    "getCodtit(a.id_parent) as codTit " +
                    "FROM project a " +
                    "WHERE a.cha_tipo_proj = 'F' AND a.id_amm = {0} {1}" +
                    "ORDER BY a.system_id ASC", infoUtente.idAmministrazione, filterString);
        }

        /// <summary>
        /// Reperimento query ricerca sottofascicoli in tutti i fascicoli, sia generali che procedimentali
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idFascicolo"></param>
        /// <returns></returns>
        private static string GetQueryFolders(InfoUtente infoUtente, string idFascicolo)
        {
            string innerCommandText = string.Format("SELECT system_id FROM project p WHERE p.id_fascicolo = {0} AND p.id_fascicolo = p.id_parent AND p.cha_tipo_proj = 'C'", idFascicolo);

            return string.Format("SELECT * FROM PROJECT A WHERE A.id_parent = ({0}) AND A.ID_PARENT != A.ID_FASCICOLO AND CHA_TIPO_PROJ = 'C' ORDER BY A.DESCRIPTION ASC", innerCommandText);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idParentFolder"></param>
        /// <returns></returns>
        private static string GetQueryFolderChilds(InfoUtente infoUtente, string idParentFolder)
        {
            return string.Format("SELECT * FROM PROJECT A WHERE A.id_parent = {0} AND A.ID_PARENT != A.ID_FASCICOLO AND CHA_TIPO_PROJ = 'C' ORDER BY A.DESCRIPTION ASC", idParentFolder);
        }

        /// <summary>
        /// 
        /// </summary>
        [Serializable()]
        private class FascicoloHash
        {
            public DocsPaVO.fascicolazione.Fascicolo Fascicolo;
            public string[] Sottofascicoli = new string[0];
            //public DocsPaDocumentale_DOCUMENTUM.DocsPaServices.DocsPaQueryHelper.DocsPaSecurityItem[] SecurityItems = new DocsPaDocumentale_DOCUMENTUM.DocsPaServices.DocsPaQueryHelper.DocsPaSecurityItem[0];
        }

        /// <summary>
        /// 
        /// </summary>
        [Serializable()]
        public class OpzioniMigrazioneFascicolo
        {
            /// <summary>
            /// 
            /// </summary>
            public bool IgnoraSeEsistente;

            /// <summary>
            /// 
            /// </summary>
            public FiltroMigrazioneFascicolo Filtro;
        }

        /// <summary>
        /// 
        /// </summary>
        [Serializable()]
        public class FiltroMigrazioneFascicolo
        {
            public string IdAmministrazione;
            public string IdFascicoloIniziale;
            public string IdFascicoloFinale;
            public string DataCreazioneIniziale;
            public string DataCreazioneFinale;

            public override string ToString()
            {
                // Impostazione parametri di filtro
                string filter = string.Format("a.id_amm = {0}", this.IdAmministrazione);

                if (!string.IsNullOrEmpty(this.IdFascicoloIniziale) && !string.IsNullOrEmpty(this.IdFascicoloFinale))
                    filter = string.Format("{0} AND a.system_id between {1} AND {2}", filter, 
                                this.IdFascicoloIniziale, this.IdFascicoloFinale);
                else if (!string.IsNullOrEmpty(this.IdFascicoloIniziale))
                    filter = string.Format("{0} AND a.system_id >= {1}", filter, this.IdFascicoloIniziale);
                else if (!string.IsNullOrEmpty(this.IdFascicoloFinale))
                    filter = string.Format("{0} AND a.system_id <= {1}", filter, this.IdFascicoloFinale);

                return filter;
            }
        }
    }
}
