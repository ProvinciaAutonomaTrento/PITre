using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.IO;
using System.Xml.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using DocsPaDocumentale.Interfaces;
using DocsPaDocumentale_DOCUMENTUM.Documentale;
using DocsPaVO.amministrazione;
using DocsPaVO.utente;
using DocsPaUtils.Data;

namespace DocsPaDocumentale_PITRE.Migrazione
{
    /// <summary>
    /// Gestione migrazione documento
    /// </summary>
    public class Documento
    {
        /// <summary>
        /// 
        /// </summary>
        private Documento()
        { }

        /// <summary>
        /// Reperimento del numero di documenti estratti dal filtro migrazione
        /// </summary>
        /// <param name="amministrazione"></param>
        /// <param name="filtro"></param>
        /// <returns></returns>
        private static int GetCountDocumentiFiltroMigrazione(InfoAmministrazione amministrazione, FiltroMigrazioneDocumento filtro)
        {
            int count = 0;

            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                string commandText = string.Format("SELECT COUNT(p.system_id) FROM profile p INNER JOIN people pl ON p.author = pl.system_id WHERE {0}", filtro.ToString());

                string field;
                if (dbProvider.ExecuteScalar(out field, commandText))
                    count = Convert.ToInt32(field);
            }

            return count;
        }

        /// <summary>
        /// Reperimento filtro valido applicabile alla migrazione documenti 
        /// in base allo stato corrente della migrazione
        /// </summary>
        /// <param name="amministrazione"></param>        
        /// <param name="numeroDocumenti">
        /// Numero documenti estratti dal filtro
        /// </param>
        /// <returns></returns>
        public static FiltroMigrazioneDocumento GetFiltroMigrazione(InfoAmministrazione amministrazione, FiltroMigrazioneDocumento filtro, out int numeroDocumenti)
        {
            numeroDocumenti = 0;

            if (filtro == null)
                filtro = new FiltroMigrazioneDocumento();
            
            filtro.IdAmministrazione = amministrazione.IDAmm;
            
            InfoStatoMigrazione statoMigrazione = StatoMigrazione.Get(amministrazione);

            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                string commandText = string.Empty;

                if (statoMigrazione.DocumentiMigrazione.Count > 0)
                {
                    // Se è stato migrato almeno un documento,
                    // viene reperito il primo docnumber successivo da migrare
                    commandText = DocsPaDbManagement.Functions.Functions.SelectTop(
                                        string.Format("SELECT p.docnumber FROM profile p INNER JOIN people pl ON p.author = pl.system_id WHERE p.docnumber > {0} AND pl.id_amm = {1} ORDER BY p.docnumber ASC", statoMigrazione.DocumentiMigrazione[0].DocNumber, statoMigrazione.IdAmministrazione), "1");

                    string field;
                    if (dbProvider.ExecuteScalar(out field, commandText))
                    {
                        filtro.DocNumberIniziale = field;
                        filtro.DataCreazioneIniziale = GetDataCreazioneDocumento(amministrazione, filtro.DocNumberIniziale);
                    }
                }
                else
                {
                    // In caso di prima migrazione, vengono reperiti il docnumber e la data creazione
                    // del primo documento disponibile nell'amministrazione
                    commandText = string.Format("SELECT MIN(p.creation_date) FROM profile p INNER JOIN people pl on p.author = pl.system_id WHERE pl.id_amm = {0} AND p.cha_tipo_proto IN ('G', 'P', 'I', 'A')", amministrazione.IDAmm);

                    string field;
                    if (dbProvider.ExecuteScalar(out field, commandText))
                    {
                        DateTime dataCreazioneIniziale;
                        if (DateTime.TryParse(field, out dataCreazioneIniziale))
                        {
                            filtro.DataCreazioneIniziale = dataCreazioneIniziale.ToString("dd/MM/yyyy");
                            filtro.DocNumberIniziale = GetIdDocumento(amministrazione, filtro.DataCreazioneIniziale, true);
                        }
                    }
                }
            }

            if (string.IsNullOrEmpty(filtro.DataCreazioneIniziale) ||
                string.IsNullOrEmpty(filtro.DataCreazioneFinale))
            {
                // Se la data di creazione finale non è stata fornita, 
                // viene impostata la data di creazione iniziale
                filtro.DataCreazioneFinale = filtro.DataCreazioneIniziale;
            }
            // Se la data di creazione iniziale è maggiore di quella finale,
            // viene impostata la data di creazione iniziale
            else if (Convert.ToDateTime(filtro.DataCreazioneIniziale) > 
                     Convert.ToDateTime(filtro.DataCreazioneFinale))
            {
                filtro.DataCreazioneFinale = filtro.DataCreazioneIniziale;
            }

            filtro.DocNumberFinale = GetIdDocumento(amministrazione, filtro.DataCreazioneFinale, false);

            if (!string.IsNullOrEmpty(filtro.DataCreazioneIniziale))
            {
                // Reperimento numero documenti estratti dal filtro
                numeroDocumenti = GetCountDocumentiFiltroMigrazione(amministrazione, filtro);
            }
            
            return filtro;
        }

        /// <summary>
        /// Reperimento data di creazione dall'id del documento
        /// </summary>
        /// <param name="amministrazione"></param>
        /// <param name="docNumber"></param>
        /// <returns></returns>
        private static string GetDataCreazioneDocumento(DocsPaVO.amministrazione.InfoAmministrazione amministrazione, string docNumber)
        {
            string dataCreazione = string.Empty;

            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                string commandText = string.Format("SELECT p.creation_date FROM profile p WHERE p.docnumber = {0}", docNumber);

                if (dbProvider.ExecuteScalar(out dataCreazione, commandText))
                {
                    DateTime date;
                    if (DateTime.TryParse(dataCreazione, out date))
                        dataCreazione = date.ToString("dd/MM/yyyy");
                }
            }

            return dataCreazione;
        }

        /// <summary>
        /// Reperimento id del documento in base alla data di creazione
        /// </summary>
        /// <param name="dataCreazioneDocumento"></param>
        /// <param name="idPrimoDocumento"></param>
        private static string GetIdDocumento(DocsPaVO.amministrazione.InfoAmministrazione amministrazione, string dataCreazione, bool primoDocumento)
        {
            string idDocumento = string.Empty;

            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                string commandText = "SELECT {0}(p.docnumber) FROM profile p INNER JOIN people pl on p.author = pl.system_id WHERE p.creation_date {1} {2} AND pl.id_amm = {3} AND p.cha_tipo_proto IN ('G', 'P', 'I', 'A')";

                if (primoDocumento)
                    commandText = string.Format(commandText, "MIN", ">=", DocsPaDbManagement.Functions.Functions.ToDateBetween(dataCreazione, true), amministrazione.IDAmm);
                else
                    commandText = string.Format(commandText, "MAX", "<=", DocsPaDbManagement.Functions.Functions.ToDateBetween(dataCreazione, false), amministrazione.IDAmm);

                dbProvider.ExecuteScalar(out idDocumento, commandText);
            }

            return idDocumento;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="amministrazione"></param>
        /// <param name="opzioniMigrazione"></param>
        private static void ImportaDocumentiStampaRegistri(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.amministrazione.InfoAmministrazione amministrazione)
        {
            // Reperimento stato corrente migrazione
            InfoStatoMigrazione statoMigrazione = StatoMigrazione.Get(amministrazione);

            try
            {
                // Viene assegnato all'infoutente l'id dell'amministrazione fornita come parametro
                // per fare in modo che l'utente che esegue la migrazione si impersonifichi 
                // come utente dell'amministrazione
                string idAmm = infoUtente.idAmministrazione;
                infoUtente.idAmministrazione = amministrazione.IDAmm;

                using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                {
                    using (DataSet ds = new DataSet())
                    {
                        // 1. Reperimento dati documenti
                        if (dbProvider.ExecuteQuery(ds, GetQueryStampaRegistri(amministrazione)))
                        {
                            Log.GetInstance(amministrazione).Write(string.Format("Reperimento documenti stampa registro in amministrazione. Numero documenti: {0}", ds.Tables[0].Rows.Count.ToString()), false);

                            int index = 1;

                            foreach (DataRow row in ds.Tables[0].Rows)
                            {
                                Log.GetInstance(amministrazione).Write(string.Format("Migrazione documento stampa registro {0} di {1} in corso...", index.ToString(), ds.Tables[0].Rows.Count), false);

                                // 2. Importa documento stampa registro
                                ImportaDocumentoStampaRegistro(row, infoUtente, amministrazione, statoMigrazione);

                                index++;
                            }
                        }
                    }
                }

                infoUtente.idAmministrazione = idAmm;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                StatoMigrazione.Save(statoMigrazione, amministrazione);
            }
        }

        /// <summary>
        /// Implementazione della logica del task di migrazione dati dei documenti stampa registro
        /// per una singola amministrazione DocsPa 
        /// </summary>
        /// <param name="amministrazione"></param>
        /// <param name="opzioniMigrazione"></param>
        public static void ImportaDocumentiStampaRegistri(DocsPaVO.amministrazione.InfoAmministrazione amministrazione)
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
                    // 2. Migrazione di tutti i documenti stampa registro in amministrazione
                    ImportaDocumentiStampaRegistri(infoUtente, amministrazione);

                    Log.GetInstance(amministrazione).Write("Procedura di migrazione completata", false);
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
        /// <param name="row"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        private static DocsPaVO.documento.SchedaDocumento GetSchedaDocumentoStampaRegistro(DataRow row, InfoUtente infoUtente)
        {
            DocsPaVO.documento.SchedaDocumento schedaDocumento = new DocsPaVO.documento.SchedaDocumento();
            schedaDocumento.systemId = DocsPaUtils.Data.DataReaderHelper.GetValue<object>(row, "system_id", false).ToString();
            schedaDocumento.docNumber = DocsPaUtils.Data.DataReaderHelper.GetValue<object>(row, "docnumber", false).ToString();
            schedaDocumento.tipoProto = "R";
            schedaDocumento.oggetto = new DocsPaVO.documento.Oggetto();
            schedaDocumento.oggetto.descrizione = DocsPaUtils.Data.DataReaderHelper.GetValue<string>(row, "var_prof_oggetto", false);
            schedaDocumento.dataCreazione = DocsPaUtils.Data.DataReaderHelper.GetValue<DateTime>(row, "creation_date", false).ToString();
            schedaDocumento.registro = new Registro();
            schedaDocumento.registro.systemId = DocsPaUtils.Data.DataReaderHelper.GetValue<object>(row, "id_registro", false).ToString();

            DocsPaVO.documento.FileRequest fileRequest = new DocsPaVO.documento.FileRequest();
            fileRequest.docNumber = schedaDocumento.docNumber;
            fileRequest.descrizione = DocsPaVO.documento.Documento.STAMPA_REGISTRO;
            fileRequest.versionId = DocsPaUtils.Data.DataReaderHelper.GetValue<object>(row, "version_id", false).ToString();
            fileRequest.subVersion = DocsPaUtils.Data.DataReaderHelper.GetValue<object>(row, "subversion", false).ToString();
            fileRequest.versionLabel = DocsPaUtils.Data.DataReaderHelper.GetValue<string>(row, "version_label", false);
            fileRequest.path = DocsPaUtils.Data.DataReaderHelper.GetValue<string>(row, "path", false);
            fileRequest.fileSize = DocsPaUtils.Data.DataReaderHelper.GetValue<object>(row, "file_size", false).ToString();
            fileRequest.fileName = System.IO.Path.GetFileName(fileRequest.path);
            schedaDocumento.documenti = new ArrayList();
            schedaDocumento.documenti.Add(fileRequest);

            return schedaDocumento;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="row"></param>
        /// <param name="infoUtente"></param>
        /// <param name="amministrazione"></param>
        /// <param name="statoMigrazione"></param>
        /// <param name="opzioniMigrazione"></param>
        private static void ImportaDocumentoStampaRegistro(DataRow row, InfoUtente infoUtente, InfoAmministrazione amministrazione, InfoStatoMigrazione statoMigrazione)
        {
            DocsPaDocumentale.Interfaces.IDocumentManager documentManager = null;
            DocsPaVO.documento.SchedaDocumento schedaDocumento = null;
            InfoDocumentoMigrazione docMigrazione = null;

            try
            {
                documentManager = new DocsPaDocumentale_DOCUMENTUM.Documentale.DocumentManager(infoUtente);

                // 1. Reperimento utente e ruolo creatore del documento
                Ruolo ruoloCreatore;
                DocsPaVO.utente.InfoUtente infoUtenteCreatore = GetInfoUtenteCreatoreDocumento(row, out ruoloCreatore);

                // 2. Reperimento oggetto scheda documento
                schedaDocumento = GetSchedaDocumentoStampaRegistro(row, infoUtenteCreatore);

                if (((DocsPaDocumentale_DOCUMENTUM.Documentale.DocumentManager)documentManager).ContainsDocumentoStampaRegistro(schedaDocumento.docNumber))
                {
                    // La stampa registro è già esistente
                    Log.GetInstance(amministrazione).Write(string.Format("Stampa registro '{0}' per documento con Id '{1}' già esistente", schedaDocumento.oggetto.descrizione, schedaDocumento.systemId), false);
                }
                else
                {
                    docMigrazione = new InfoDocumentoMigrazione(schedaDocumento);
                    docMigrazione.HashDocumento = GetHashDocumento(schedaDocumento,true);

                    // 3. Creazione documento in documentum
                    Ruolo[] ruoliSuperiori;
                    if (documentManager.CreateDocumentoStampaRegistro(schedaDocumento, ruoloCreatore, out ruoliSuperiori))
                    {
                        Log.GetInstance(amministrazione).Write(string.Format("Creazione stampa registro '{0}' per documento con Id '{1}'", schedaDocumento.oggetto.descrizione, schedaDocumento.systemId), false);

                        // 4. Acquisizione file stampa registro in dctm
                        UploadFile(amministrazione, (DocsPaVO.documento.FileRequest)schedaDocumento.documenti[0], schedaDocumento, infoUtente, documentManager);

                        Log.GetInstance(amministrazione).Write(string.Format("Upload stampa registro '{0}' per documento con Id '{1}'", schedaDocumento.oggetto.descrizione, schedaDocumento.systemId), false);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.GetInstance(amministrazione).Write(ex.Message, true);

                docMigrazione.EsitoMigrazione = false;
                docMigrazione.ErroreMigrazione = ex.Message;
            }
            finally
            {
                docMigrazione.DataMigrazione = DateTime.Now.ToString();

                statoMigrazione.SetDocumentoMigrazione(docMigrazione);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="amministrazione"></param>
        /// <param name="opzioniMigrazione"></param>
        /// <returns></returns>
        public static InfoDocumentoMigrazione[] VerificaDocumenti(DocsPaVO.amministrazione.InfoAmministrazione amministrazione, Documento.OpzioniMigrazioneDocumento opzioniMigrazione)
        {
            InfoDocumentoMigrazione[] list = new InfoDocumentoMigrazione[0];

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
                    // 2. Verifica di tutti i documenti in amministrazione
                    list = VerificaDocumenti(infoUtente, amministrazione, opzioniMigrazione);

                    Log.GetInstance(amministrazione).Write("Procedura di verifica migrazione completata", false);
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

            return list;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="amministrazione"></param>
        /// <param name="opzioniMigrazione"></param>
        /// <returns></returns>
        private static InfoDocumentoMigrazione[] VerificaDocumenti(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.amministrazione.InfoAmministrazione amministrazione, Documento.OpzioniMigrazioneDocumento opzioniMigrazione)
        {
            // Reperimento stato corrente migrazione
            InfoStatoMigrazione statoMigrazione = StatoMigrazione.Get(amministrazione);

            List<InfoDocumentoMigrazione> list = new List<InfoDocumentoMigrazione>();

            // Viene assegnato all'infoutente l'id dell'amministrazione fornita come parametro
            // per fare in modo che l'utente che esegue la migrazione si impersonifichi 
            // come utente dell'amministrazione
            string idAmm = infoUtente.idAmministrazione;
            infoUtente.idAmministrazione = amministrazione.IDAmm;

            try
            {
                using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                {
                    using (DataSet ds = new DataSet())
                    {
                        // 1. Reperimento dati documenti
                        if (dbProvider.ExecuteQuery(ds, GetQueryDocumenti(amministrazione, opzioniMigrazione.Filtro)))
                        {
                            Log.GetInstance(amministrazione).Write(string.Format("Reperimento documenti in amministrazione. Numero documenti: {0}", ds.Tables[0].Rows.Count.ToString()), false);

                            int index = 1;

                            DocsPaDocumentale_DOCUMENTUM.Documentale.DocumentManager documentManager = new DocsPaDocumentale_DOCUMENTUM.Documentale.DocumentManager(infoUtente);

                            foreach (DataRow row in ds.Tables[0].Rows)
                            {
                                Log.GetInstance(amministrazione).Write(string.Format("Verifica documento {0} di {1} in corso...", index.ToString(), ds.Tables[0].Rows.Count), false);

                                // 2. Verifica documento
                                InfoDocumentoMigrazione info = VerificaDocumento(row, infoUtente, amministrazione, documentManager);

                                if (info != null)
                                {
                                    list.Add(info);
                                    statoMigrazione.SetDocumentoMigrazione(info);
                                }

                                if (_interrompiMigrazione)
                                {
                                    _interrompiMigrazione = false;
                                    Log.GetInstance(amministrazione).Write(string.Format("Verifica documenti interrotta al documento {0} di {1}", index.ToString(), ds.Tables[0].Rows.Count), false);
                                    break;
                                }

                                index++;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                infoUtente.idAmministrazione = idAmm;
                StatoMigrazione.Save(statoMigrazione, amministrazione);
            }

            return list.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="row"></param>
        /// <param name="infoUtente"></param>
        /// <param name="amministrazione"></param>
        /// <returns></returns>
        private static InfoDocumentoMigrazione VerificaDocumento(DataRow row, InfoUtente infoUtente, InfoAmministrazione amministrazione, DocsPaDocumentale_DOCUMENTUM.Documentale.DocumentManager documentManager)
        {
            InfoDocumentoMigrazione retValue = null;

            // 1. Reperimento utente e ruolo creatore del documento
            // Ruolo ruoloCreatore;
            // DocsPaVO.utente.InfoUtente infoUtenteCreatore = GetInfoUtenteCreatoreDocumento(row, out ruoloCreatore);

            // 2. Reperimento oggetto scheda documento
            // DocsPaVO.documento.SchedaDocumento schedaDocumento = GetSchedaDocumento(row, infoUtenteCreatore);
            
            if (!documentManager.ContainsDocumento(row["docnumber"].ToString()))
            {   
                retValue = new InfoDocumentoMigrazione();
                retValue.DocNumber = row["docnumber"].ToString();
                retValue.DataCreazione = row["creation_date"].ToString();
                if (row["var_segnatura"] != DBNull.Value)
                    retValue.Segnatura = row["var_segnatura"].ToString();
                retValue.EsitoMigrazione = false;
                retValue.ErroreMigrazione = string.Format("Il documento con Id '{0}' non risulta migrato", row["docnumber"].ToString());
            }

            return retValue;
        }

        /// <summary>
        /// Verifica se i documenti già oggetto di migrazione siano stati modificati in PITRE e non in DCTM
        /// </summary>
        /// <param name="amministrazione"></param>
        /// <param name="documentiMigrazione"></param>
        /// <returns></returns>
        public static InfoDocumentoMigrazione[] VerificaDocumentiModificati(DocsPaVO.amministrazione.InfoAmministrazione amministrazione, InfoDocumentoMigrazione[] documentiMigrazione)
        {
            // Reperimento stato corrente migrazione
            InfoStatoMigrazione statoMigrazione = StatoMigrazione.Get(amministrazione);

            List<InfoDocumentoMigrazione> list = new List<InfoDocumentoMigrazione>();

            int index = 1;

            try
            {
                foreach (InfoDocumentoMigrazione item in documentiMigrazione)
                {
                    Log.GetInstance(amministrazione).Write(string.Format("Verifica modifiche al documento {0} di {1} in corso...", index.ToString(), documentiMigrazione.Length.ToString()), false);

                    try
                    {
                        DocsPaVO.utente.Utente utenteCreatore = DocsPaDocumentale_DOCUMENTUM.DocsPaServices.DocsPaQueryHelper.getUtente(item.IdUtenteCreatore);
                        DocsPaVO.utente.Ruolo ruoloCreatore = DocsPaDocumentale_DOCUMENTUM.DocsPaServices.DocsPaQueryHelper.getRuolo(item.IdRuoloCreatore);
                        DocsPaVO.utente.InfoUtente infoUtenteCreatore = new InfoUtente(utenteCreatore, ruoloCreatore);
                        infoUtenteCreatore.idPeople = utenteCreatore.systemId;

                        DocsPaVO.documento.SchedaDocumento schedaDocumento = GetSchedaDocumento(item.DocNumber, infoUtenteCreatore);

                        string hash = GetHashDocumento(schedaDocumento,true);
                        string hash128 = GetHashDocumento(schedaDocumento,false);

                        if (!hash.Equals(item.HashDocumento))
                        {
                            if (!hash128.Equals(item.HashDocumento))
                            {
                                item.HashDocumento = hash;
                                list.Add(item);

                                statoMigrazione.SetDocumentoMigrazione(item);

                                Log.GetInstance(amministrazione).Write(string.Format("Il documento con Id '{0}' risulta essere stato modificato rispetto alla versione migrata", item.DocNumber), false);
                            }
                        }

                        index++;
                    }
                    catch (Exception ex)
                    {
                        Log.GetInstance(amministrazione).Write(ex.Message, true);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.GetInstance(amministrazione).Write(ex.Message, true);
            }
            finally
            {
                Log.GetInstance(amministrazione).Write("Verifica modifiche documenti terminata", false);
                Log.GetInstance(amministrazione).Flush();

                StatoMigrazione.Save(statoMigrazione, amministrazione);
            }

            return list.ToArray();
        }

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
        /// 
        /// </summary>
        /// <param name="amministrazione"></param>
        /// <param name="documenti"></param>
        public static void ImportaDocumentiSelezionati(DocsPaVO.amministrazione.InfoAmministrazione amministrazione, Migrazione.InfoDocumentoMigrazione[] documenti)
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
                    // 2. Migrazione di tutti i documenti in amministrazione
                    ImportaDocumentiSelezionati(infoUtente, amministrazione, documenti);

                    Log.GetInstance(amministrazione).Write("Procedura di migrazione completata", false);
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
        /// Implementazione della logica del task di migrazione dati dei documenti
        /// per una singola amministrazione DocsPa
        /// </summary>
        /// <param name="amministrazione"></param>
        /// <param name="opzioniMigrazione"></param>
        /// <returns></returns>
        public static void ImportaDocumenti(DocsPaVO.amministrazione.InfoAmministrazione amministrazione, Documento.OpzioniMigrazioneDocumento opzioniMigrazione)
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
                    // 2. Migrazione di tutti i documenti in amministrazione
                    ImportaDocumenti(infoUtente, amministrazione, opzioniMigrazione);
                    
                    Log.GetInstance(amministrazione).Write("Procedura di migrazione completata", false);
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
        /// <param name="documenti"></param>
        private static void ImportaDocumentiSelezionati(InfoUtente infoUtente, InfoAmministrazione amministrazione, Migrazione.InfoDocumentoMigrazione[] documenti)
        {
            // Reperimento stato corrente migrazione
            InfoStatoMigrazione statoMigrazione = StatoMigrazione.Get(amministrazione);

            try
            {
                // Viene assegnato all'infoutente l'id dell'amministrazione fornita come parametro
                // per fare in modo che l'utente che esegue la migrazione si impersonifichi 
                // come utente dell'amministrazione
                string idAmm = infoUtente.idAmministrazione;
                infoUtente.idAmministrazione = amministrazione.IDAmm;

                using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                {
                    int index = 1;

                    foreach (Migrazione.InfoDocumentoMigrazione doc in documenti)
                    {
                        Log.GetInstance(amministrazione).Write(string.Format("Migrazione documento {0} di {1} in corso...", index.ToString(), documenti.Length.ToString()), false);

                        using (DataSet ds = new DataSet())
                        {
                            // Creazione filtro per caricare i dati del singolo documento da migrare
                            FiltroMigrazioneDocumento filtro = new FiltroMigrazioneDocumento();
                            filtro.IdAmministrazione = amministrazione.IDAmm;
                            filtro.DocNumberIniziale = doc.DocNumber;
                            filtro.DocNumberFinale = doc.DocNumber;

                            OpzioniMigrazioneDocumento opzioni = new OpzioniMigrazioneDocumento();
                            opzioni.Filtro = filtro;
                            opzioni.SovrascriviDocumentiEsistenti = true;

                            // 2. Reperimento dati documento
                            if (dbProvider.ExecuteQuery(ds, GetQueryDocumenti(amministrazione, filtro)))
                            {
                                if (ds.Tables[0].Rows.Count == 1)
                                {
                                    // 3. Importa documento
                                    ImportaDocumento(ds.Tables[0].Rows[0], infoUtente, amministrazione, statoMigrazione, opzioni);
                                }
                            }
                        }

                        if (_interrompiMigrazione)
                        {
                            _interrompiMigrazione = false;
                            Log.GetInstance(amministrazione).Write(string.Format("Migrazione documenti interrotta al documento {0} di {1}", index.ToString(), documenti.Length.ToString()), false);
                            break;
                        }

                        index++;
                    }
                }

                infoUtente.idAmministrazione = idAmm;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                StatoMigrazione.Save(statoMigrazione, amministrazione);
            }
        }

        /// <summary>
        /// Implementazione della logica del task di migrazione dati
        /// per tutti i documenti di un'amministrazione
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="amministrazione"></param>
        /// <param name="opzioniMigrazione"></param>
        private static void ImportaDocumenti(InfoUtente infoUtente, InfoAmministrazione amministrazione, Documento.OpzioniMigrazioneDocumento opzioniMigrazione)
        {
            // Reperimento stato corrente migrazione
            InfoStatoMigrazione statoMigrazione = StatoMigrazione.Get(amministrazione);

            try
            {
                // Viene assegnato all'infoutente l'id dell'amministrazione fornita come parametro
                // per fare in modo che l'utente che esegue la migrazione si impersonifichi 
                // come utente dell'amministrazione
                string idAmm = infoUtente.idAmministrazione;
                infoUtente.idAmministrazione = amministrazione.IDAmm;

                using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                {
                    using (DataSet ds = new DataSet())
                    {
                        // 1. Reperimento dati documenti
                        if (dbProvider.ExecuteQuery(ds, GetQueryDocumenti(amministrazione, opzioniMigrazione.Filtro)))
                        {
                            Log.GetInstance(amministrazione).Write(string.Format("Reperimento documenti in amministrazione. Numero documenti: {0}", ds.Tables[0].Rows.Count.ToString()), false);

                            int index = 1;

                            foreach (DataRow row in ds.Tables[0].Rows)
                            {
                                Log.GetInstance(amministrazione).Write(string.Format("Migrazione documento {0} di {1} in corso...", index.ToString(), ds.Tables[0].Rows.Count), false);
                                
                                // 2. Importa documento
                                ImportaDocumento(row, infoUtente, amministrazione, statoMigrazione, opzioniMigrazione);

                                if (_interrompiMigrazione)
                                {
                                    _interrompiMigrazione = false;
                                    Log.GetInstance(amministrazione).Write(string.Format("Migrazione documenti interrotta al documento {0} di {1}", index.ToString(), ds.Tables[0].Rows.Count), false);
                                    break;
                                }

                                index++;
                            }
                        }
                    }
                }

                infoUtente.idAmministrazione = idAmm;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                StatoMigrazione.Save(statoMigrazione, amministrazione);
            }
        }

        ///// <summary>
        ///// Solo per test
        ///// </summary>
        ///// <param name="row"></param>
        ///// <param name="infoUtente"></param>
        ///// <param name="amministrazione"></param>
        //private static void TestUploadFile(DataRow row, InfoUtente infoUtente, InfoAmministrazione amministrazione)
        //{
        //    byte[] content = null;

        //    using (FileStream stream = new FileStream(@"c:\quaderno21.pdf", FileMode.Open, FileAccess.Read, FileShare.Read))
        //    {
        //        content = new byte[stream.Length];
        //        stream.Read(content, 0, content.Length);
        //    }

        //    // 1. Impersonate come l'utente che ha creato il documento
        //    Ruolo ruolo;
        //    InfoUtente infoUtenteCreatoreDocumento = ImpersonateCreatoreDocumento(row, amministrazione, out ruolo);

        //    DocsPaDocumentale.Interfaces.IDocumentManager documentManager = new DocsPaDocumentale_ETDOCS.Documentale.DocumentManager(infoUtenteCreatoreDocumento);

        //    // 2. Reperimento oggetto scheda documento
        //    DocsPaVO.documento.SchedaDocumento schedaDocumento = GetSchedaDocumento(row, infoUtenteCreatoreDocumento);

        //    foreach (DocsPaVO.documento.FileRequest fileRequest in schedaDocumento.documenti)
        //    {
        //        // 4a. Reperimento file acquisito nel documentale etdocs
        //        DocsPaVO.documento.FileDocumento fileDocumento = new DocsPaVO.documento.FileDocumento();
        //        fileDocumento.content = content;
        //        fileDocumento.contentType = "application/pdf";
        //        fileDocumento.estensioneFile = "pdf";
        //        fileDocumento.length = content.Length;

        //        documentManager.PutFile(fileRequest, fileDocumento, "pdf");
        //    }
        //}

        /// <summary>
        /// Migrazione di un singolo documento
        /// </summary>
        /// <param name="row"></param>
        /// <param name="infoUtente"></param>
        /// <param name="amministrazione"></param>
        /// <param name="statoMigrazione"></param>
        /// <param name="opzioniMigrazione"></param>
        private static void ImportaDocumento(DataRow row, InfoUtente infoUtente, InfoAmministrazione amministrazione, InfoStatoMigrazione statoMigrazione, Documento.OpzioniMigrazioneDocumento opzioniMigrazione)
        {
            DocsPaDocumentale.Interfaces.IDocumentManager documentManager = null;
            DocsPaVO.documento.SchedaDocumento schedaDocumento = null;
            InfoDocumentoMigrazione docMigrazione = null;
            bool documentoCreato = false;

            try
            {
                documentManager = new DocsPaDocumentale_DOCUMENTUM.Documentale.DocumentManager(infoUtente);

                // 1. Reperimento utente e ruolo creatore del documento
                Ruolo ruoloCreatore;
                DocsPaVO.utente.InfoUtente infoUtenteCreatore = GetInfoUtenteCreatoreDocumento(row, out ruoloCreatore);

                // 2. Reperimento oggetto scheda documento
                schedaDocumento = GetSchedaDocumento(row, infoUtenteCreatore);
                
                docMigrazione = new InfoDocumentoMigrazione(schedaDocumento);
                docMigrazione.HashDocumento = GetHashDocumento(schedaDocumento,true);

                Ruolo[] ruoliSuperiori = null;

                bool ignoraDocumento = false;

                if (opzioniMigrazione.IgnoraSeEsistente)
                {
                    // Indica di ignorare il documento in quanto già esistente
                    ignoraDocumento = (((DocsPaDocumentale_DOCUMENTUM.Documentale.DocumentManager)documentManager).ContainsDocumento(schedaDocumento.docNumber));

                    docMigrazione.EsitoMigrazione = false;
                    docMigrazione.ErroreMigrazione = "Documento già migrato e ignorato";

                }
                else if (opzioniMigrazione.SovrascriviDocumentiEsistenti)
                {
                /* commentato x sicurezza   if (((DocsPaDocumentale_DOCUMENTUM.Documentale.DocumentManager)documentManager).ContainsDocumento(schedaDocumento.docNumber))
                    {
                        // 2a. Rimozione documento esistente, qualora richiesto nelle opzioni e qualora esista
                        if (!documentManager.Remove(new DocsPaVO.documento.InfoDocumento(schedaDocumento)))
                        {
                            throw new ApplicationException(
                                string.Format("Si è verificato un errore nella rimozione del documento '{0}' per l'amministrazione '{1}'",
                                        schedaDocumento.systemId, amministrazione.Codice));
                        }
                    } */
                }

                if (!ignoraDocumento)
                {
                    // 3. Creazione documento in documentum
                    if (schedaDocumento.tipoProto == "G")
                    {
                        documentoCreato = documentManager.CreateDocumentoGrigio(schedaDocumento, ruoloCreatore, out ruoliSuperiori);
                    }
                    else
                    {
                        DocsPaVO.documento.ResultProtocollazione result;
                        documentoCreato = documentManager.CreateProtocollo(schedaDocumento, ruoloCreatore, out result, out ruoliSuperiori);
                    }

                    if (documentoCreato)
                    {
                        // 4. Evento di notifica documento creato
                        DocsPaDocumentale.Interfaces.IAclEventListener aclEventListener = new DocsPaDocumentale_DOCUMENTUM.Documentale.AclEventListener(infoUtente);
                        aclEventListener.DocumentoCreatoEventHandler(schedaDocumento, ruoloCreatore, ruoliSuperiori);

                        // 5. Creazione delle versioni per il documento
                        ImportaVersioniDocumento(amministrazione, schedaDocumento, infoUtenteCreatore, documentManager, opzioniMigrazione.ImportaSoloMetadatiDocumenti);

                        // 6. Creazione allegati del documento
                        ImportaAllegatiDocumento(amministrazione, schedaDocumento, infoUtenteCreatore, infoUtente, statoMigrazione, opzioniMigrazione.ImportaSoloMetadatiDocumenti);

                        // 7. Inserimento del documento nei fascicoli di appartenenza
                        ImportaDocumentoFascicoli(amministrazione, schedaDocumento, infoUtente);

                        // 8. Impostazione ownership del documento
                        ((DocsPaDocumentale_DOCUMENTUM.Documentale.DocumentManager)documentManager).SetOwnershipDocumento
                            (schedaDocumento.systemId,
                            DocsPaDocumentale_DOCUMENTUM.DocsPaObjectTypes.TypeUtente.getUserName(infoUtenteCreatore),
                            DocsPaDocumentale_DOCUMENTUM.DocsPaObjectTypes.TypeGruppo.GetGroupName(ruoloCreatore));

                        // 9. Se il documento è in stato checkout in etdocs, viene impostatato in checkout in documentum
                        DocsPaVO.CheckInOut.CheckOutStatus checkOutStatus = schedaDocumento.checkOutStatus;

                        if (checkOutStatus != null)
                        {
                            CheckInOutDocumentManager checkInOutMng = new CheckInOutDocumentManager(infoUtente);

                            bool result = checkInOutMng.CheckOut(checkOutStatus.IDDocument, checkOutStatus.DocumentNumber, checkOutStatus.DocumentLocation, checkOutStatus.MachineName, out checkOutStatus);
                        }

                        docMigrazione.EsitoMigrazione = true;
                    }
                    else
                    {
                        // 4a. Errore nella creazione del documento
                        throw new ApplicationException(
                            string.Format("Si è verificato un errore nella creazione del documento '{0}' per l'amministrazione '{1}'",
                            schedaDocumento.systemId, amministrazione.Codice));
                    }
                }
            }
            catch (Exception ex)
            {
                Log.GetInstance(amministrazione).Write(ex.Message, true);

                docMigrazione.EsitoMigrazione = false;
                docMigrazione.ErroreMigrazione = ex.Message;

                // Rimozione del documento creato               
                if (documentoCreato)
                    documentManager.Remove(new DocsPaVO.documento.InfoDocumento(schedaDocumento));
            }
            finally
            {
                docMigrazione.DataMigrazione = DateTime.Now.ToString();

                statoMigrazione.SetDocumentoMigrazione(docMigrazione);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="documenti"></param>
        /// <returns></returns>
        private static DocsPaVO.documento.FileRequest[] OrdinaDocumenti(ArrayList documenti)
        {
            List<DocsPaVO.documento.FileRequest> list = new List<DocsPaVO.documento.FileRequest>
                ((DocsPaVO.documento.FileRequest[]) documenti.ToArray(typeof(DocsPaVO.documento.FileRequest)));
            list.Sort(new Comparison<DocsPaVO.documento.FileRequest>(CompareFileRequest));
            return list.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private static int CompareFileRequest(DocsPaVO.documento.FileRequest x, DocsPaVO.documento.FileRequest y)
        {   
            return Convert.ToInt32(x.version).CompareTo(Convert.ToInt32(y.version));
        }

        /// <summary>
        /// Creazione delle versioni per il documento
        /// </summary>
        /// <param name="amministrazione"></param>
        /// <param name="schedaDocumento"></param>
        /// <param name="infoUtente"></param>
        /// <param name="documentManager"></param>
        /// <param name="importaSoloMetadati"></param>
        private static void ImportaVersioniDocumento(DocsPaVO.amministrazione.InfoAmministrazione amministrazione, DocsPaVO.documento.SchedaDocumento schedaDocumento, DocsPaVO.utente.InfoUtente infoUtente, IDocumentManager documentManager, bool importaSoloMetadati)
        {
            // 4. Creazione versioni documento
            foreach (DocsPaVO.documento.FileRequest fileRequest in OrdinaDocumenti(schedaDocumento.documenti))
            {
                if (Convert.ToInt32(fileRequest.version) > 1)
                {
                    if (documentManager.AddVersion(fileRequest, false))
                    {
                        Log.GetInstance(amministrazione).Write(string.Format("Migrazione versione con Id '{0}' per documento con Id '{1}'",
                                    fileRequest.versionId, schedaDocumento.systemId), false);

                        // 4a. Acquisizione file in dctm
                        if (!importaSoloMetadati)
                            UploadFile(amministrazione, fileRequest, schedaDocumento, infoUtente, documentManager);
                    }
                }
                else
                {
                    // 4a. Acquisizione file in dctm per la versione principale
                    if (!importaSoloMetadati)
                        UploadFile(amministrazione, fileRequest, schedaDocumento, infoUtente, documentManager);
                }
            }
        }

        /// <summary>
        /// Migrazione degli allegati di un documento
        /// </summary>
        /// <param name="schedaDocumento"></param>
        /// <param name="creatoreDocumento"></param>
        /// <param name="infoUtente"></param>
        /// <param name="documentManager"></param>
        /// <param name="statoMigrazione"></param>
        private static void ImportaAllegatiDocumento(DocsPaVO.amministrazione.InfoAmministrazione amministrazione, DocsPaVO.documento.SchedaDocumento schedaDocumento, DocsPaVO.utente.InfoUtente infoUtenteCreatoreDocumento, DocsPaVO.utente.InfoUtente infoUtente, InfoStatoMigrazione statoMigrazione, bool importaSoloMetadati)
        {
            foreach (DocsPaVO.documento.Allegato allegato in schedaDocumento.allegati)
            {
                ImportaAllegatoDocumento(amministrazione, schedaDocumento, allegato, infoUtenteCreatoreDocumento, infoUtente, statoMigrazione, importaSoloMetadati);
            }
        }

        /// <summary>
        /// Migrazione di un allegato
        /// </summary>
        /// <param name="schedaDocumentoPrincipale"></param>
        /// <param name="allegato"></param>
        /// <param name="infoUtente"></param>
        /// <param name="statoMigrazione"></param>
        /// <param name="importaSoloMetadati">
        /// Se true, indica se importare solo i metadati
        /// </param>
        private static void ImportaAllegatoDocumento(DocsPaVO.amministrazione.InfoAmministrazione amministrazione, DocsPaVO.documento.SchedaDocumento schedaDocumentoPrincipale, DocsPaVO.documento.Allegato allegato, DocsPaVO.utente.InfoUtente infoUtenteCreatoreDocumento, DocsPaVO.utente.InfoUtente infoUtente, InfoStatoMigrazione statoMigrazione, bool importaSoloMetadati)
        {
            

            // Reperimento scheda documento per l'allegato, con le credenziali del creatore del documento principale
            DocsPaVO.documento.SchedaDocumento schedaAllegato = GetSchedaDocumento(allegato.docNumber, infoUtenteCreatoreDocumento);

            Ruolo ruoloCreatore;
            DocsPaVO.utente.InfoUtente infoUtenteCreatoreAllegato = GetInfoUtenteCreatoreDocumento(schedaAllegato, out ruoloCreatore);
            infoUtenteCreatoreAllegato.dst = infoUtente.dst;

            IDocumentManager documentManager = new DocsPaDocumentale_DOCUMENTUM.Documentale.DocumentManager(infoUtenteCreatoreAllegato);

            // check esistenza allegato
            bool allegatoPresente = (((DocsPaDocumentale_DOCUMENTUM.Documentale.DocumentManager)documentManager).ContainsDocumento(allegato.docNumber));
            if (allegatoPresente)
            {
                Log.GetInstance(amministrazione).Write(string.Format("Migrazione allegato con Id '{0}' per documento con Id '{1}' tralasciata in quanto esistente!",
                            allegato.docNumber,
                            schedaDocumentoPrincipale.systemId), false);
                return;
            }

            bool allegatoCreato = documentManager.AddAttachment(allegato, string.Empty);

            

            if (allegatoCreato)
            {
                Log.GetInstance(amministrazione).Write(string.Format("Migrazione allegato con Id '{0}' per documento con Id '{1}'",
                            allegato.docNumber,
                            schedaDocumentoPrincipale.systemId), false);

                foreach (DocsPaVO.documento.FileRequest fileRequest in OrdinaDocumenti(schedaAllegato.documenti))
                {
                    if (Convert.ToInt32(fileRequest.version) > 1)
                    {
                        // 2. Creazione delle versioni associate al documento
                        if (documentManager.AddVersion(fileRequest, false))
                        {
                            Log.GetInstance(amministrazione).Write(string.Format("Migrazione versione con Id '{0}' per allegato con Id '{1}'",
                                        fileRequest.versionId, schedaAllegato.systemId), false);

                            // 3. Acquisizione file in dctm
                            if (!importaSoloMetadati)
                                UploadFile(amministrazione, fileRequest, schedaAllegato, infoUtenteCreatoreAllegato, documentManager);
                        }
                        else
                        {
                            // 2a. Errore nella migrazione della versione per l'allegato
                            throw new ApplicationException(
                                string.Format("Si è verificato un errore nell'inserimento della versione con Id '{0}' per l'allegato con Id '{1}'",
                                                fileRequest.versionId, schedaAllegato.systemId));

                        }
                    }
                    else
                    {
                        // 1a. Acquisizione file in dctm per la versione principale
                        if (!importaSoloMetadati)
                            UploadFile(amministrazione, fileRequest, schedaAllegato, infoUtenteCreatoreAllegato, documentManager);
                    }
                }

                // 2. Impostazione ownership del documento
                ((DocsPaDocumentale_DOCUMENTUM.Documentale.DocumentManager)documentManager).SetOwnershipDocumento
                    (schedaAllegato.systemId,
                    DocsPaDocumentale_DOCUMENTUM.DocsPaObjectTypes.TypeUtente.getUserName(infoUtenteCreatoreAllegato),
                    DocsPaDocumentale_DOCUMENTUM.DocsPaObjectTypes.TypeGruppo.GetGroupName(ruoloCreatore));
            }
            else
                throw new ApplicationException(string.Format("Si è verificato un errore nella creazione dell'allegato con Id '{0}' per il documento con Id '{1}'", schedaAllegato.systemId, schedaDocumentoPrincipale.systemId));
        }

        /// <summary>
        /// Inserimento del documento nei fascicoli di appartenenza
        /// </summary>
        /// <param name="schedaDocumento"></param>
        /// <param name="infoUtenteCreatoreDocumento"></param>
        private static void ImportaDocumentoFascicoli(DocsPaVO.amministrazione.InfoAmministrazione amministrazione, DocsPaVO.documento.SchedaDocumento schedaDocumento, DocsPaVO.utente.InfoUtente infoUtenteCreatoreDocumento)
        {
            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                DataSet ds = new DataSet();

                if (dbProvider.ExecuteQuery(ds, GetQueryDocumentiFascicoli(infoUtenteCreatoreDocumento, schedaDocumento.systemId)))
                {
                    DocsPaDocumentale.Interfaces.IProjectManager projectManager = new DocsPaDocumentale_DOCUMENTUM.Documentale.ProjectManager(infoUtenteCreatoreDocumento);

                    foreach (DataRow rowDocFasc in ds.Tables[0].Rows)
                    {
                        string projectId = DataReaderHelper.GetValue<object>(rowDocFasc, "project_id", false).ToString();

                        // 6. Inserimento del documento negli eventuali fascicoli
                        if (projectManager.AddDocumentInFolder(schedaDocumento.systemId, projectId))
                        {
                            Log.GetInstance(amministrazione).Write(string.Format("Inserimento documento con Id '{0}' in fascicolo con Id '{1}'", schedaDocumento.systemId, projectId), false);
                        }
                        else
                        {
                            // 6a. Errore nell'inserimento del documento negli eventuali fascicoli
                            throw new ApplicationException(
                                string.Format("Si è verificato un errore nell'inserimento del documento con id {0} nel fascicolo con id {1}",
                                schedaDocumento.systemId, projectId));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileRequest"></param>
        /// <param name="schedaDocumento"></param>
        /// <param name="infoUtenteCreatoreDocumento"></param>
        /// <param name="documentManager"></param>
        private static void UploadFile(DocsPaVO.amministrazione.InfoAmministrazione amministrazione, 
                                        DocsPaVO.documento.FileRequest fileRequest,
                                        DocsPaVO.documento.SchedaDocumento schedaDocumento,
                                        DocsPaVO.utente.InfoUtente infoUtenteCreatoreDocumento,
                                        IDocumentManager documentManager)
        {
            // 1. Reperimento file acquisito nel documentale etdocs
            DocsPaVO.documento.FileDocumento fileDocumento = GetFileDocumento(infoUtenteCreatoreDocumento, fileRequest);
            
            if (fileDocumento != null)
            {
                // 2. Acquisizione file in dctm
                if (documentManager.PutFile(fileRequest, fileDocumento, fileDocumento.estensioneFile))
                {
                    Log.GetInstance(amministrazione).Write(
                        string.Format(
                            "Migrazione file per la versione '{0}' del documento con Id '{1}'. Nome file: '{2}' - Dimensione: '{3} byte'",
                            fileRequest.version,
                            schedaDocumento.systemId,
                            fileRequest.fileName,
                            fileDocumento.content.Length.ToString()), false);
                }
                else
                {
                    // 2a. Errore nella migrazione del file
                    throw new ApplicationException(string.Format("Si è verificato un errore nell'upload del file per la versione '{0}' per il documento con Id '{1}'", fileRequest.version, schedaDocumento.systemId));
                }
            }
        }                                        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="docNumber"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        private static DocsPaVO.documento.SchedaDocumento GetSchedaDocumento(string docNumber, InfoUtente infoUtente)
        {
            using (DocsPaDB.Query_DocsPAWS.Documenti documentiDb = new DocsPaDB.Query_DocsPAWS.Documenti())
            {
                // Reperimento dettaglio del documento
                return documentiDb.GetDettaglioNoSecurity(infoUtente, DocsPaDocumentale_DOCUMENTUM.DocsPaServices.DocsPaQueryHelper.getIdProfile(docNumber), docNumber);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="row"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        private static DocsPaVO.documento.SchedaDocumento GetSchedaDocumento(DataRow row, InfoUtente infoUtente)
        {
            string idProfile = DataReaderHelper.GetValue<object>(row, "system_id", false).ToString();
            string docNumber = DataReaderHelper.GetValue<object>(row, "docnumber", false).ToString();

            using (DocsPaDB.Query_DocsPAWS.Documenti documentiDb = new DocsPaDB.Query_DocsPAWS.Documenti())
            {
                // Reperimento dettaglio del documento
                return documentiDb.GetDettaglioNoSecurity(infoUtente, idProfile, docNumber);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="schedaDocumento"></param>
        /// <param name="ruoloCreatore"></param>
        /// <returns></returns>
        private static InfoUtente GetInfoUtenteCreatoreDocumento(DocsPaVO.documento.SchedaDocumento schedaDocumento, out Ruolo ruoloCreatore)
        {
            ruoloCreatore = null;
            InfoUtente infoUtente = null;

            if (schedaDocumento.creatoreDocumento != null)
            {
                ruoloCreatore = DocsPaDocumentale_DOCUMENTUM.DocsPaServices.DocsPaQueryHelper.getRuolo(schedaDocumento.creatoreDocumento.idCorrGlob_Ruolo);
                
                //if (ruoloCreatore == null)
                //    // 1a. Impossibile reperire il ruolo creatore del documento, 
                //    throw new ApplicationException(string.Format("Si è verificato un errore nel reperimento del ruolo creatore del documento con Id '{0}'", schedaDocumento.systemId));

                // 2. Per creare il documento, è necessario fare connettersi in dctm come l'utente creatore
                DocsPaVO.utente.Utente utente = DocsPaDocumentale_DOCUMENTUM.DocsPaServices.DocsPaQueryHelper.getUtente(schedaDocumento.creatoreDocumento.idPeople);
                
                infoUtente = new InfoUtente(utente, ruoloCreatore);
                infoUtente.idPeople = utente.systemId;
            }

            return infoUtente;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="row"></param>
        /// <param name="ruoloCreatore"></param>
        /// <returns></returns>
        private static InfoUtente GetInfoUtenteCreatoreDocumento(DataRow row, out Ruolo ruoloCreatore)
        {
            // 1. Reperimento del ruolo creatore del documento
            string idProfile = DataReaderHelper.GetValue<object>(row, "system_id", false).ToString();
            string author = DataReaderHelper.GetValue<object>(row, "author", false).ToString();
            string idRuoloCreatore = DataReaderHelper.GetValue<object>(row, "id_ruolo_creatore", true, string.Empty).ToString();
            
            if (string.IsNullOrEmpty(idRuoloCreatore))
                // Ricerca del ruolo creatore in security, qualora non sia stato trovato tramite le info nella profile
                ruoloCreatore = DocsPaDocumentale_DOCUMENTUM.DocsPaServices.DocsPaQueryHelper.getRuoloCreatore(idProfile);
            else
                ruoloCreatore = DocsPaDocumentale_DOCUMENTUM.DocsPaServices.DocsPaQueryHelper.getRuolo(idRuoloCreatore);

            //if (ruoloCreatore == null)
            //    // 1a. Impossibile reperire il ruolo creatore del documento, 
            //    throw new ApplicationException(string.Format("Si è verificato un errore nel reperimento del ruolo creatore del documento con Id '{0}'", idProfile));

            // 2. Per creare il documento, è necessario fare connettersi in dctm come l'utente creatore
            DocsPaVO.utente.Utente utente = DocsPaDocumentale_DOCUMENTUM.DocsPaServices.DocsPaQueryHelper.getUtente(author);

            InfoUtente infoUtente = new InfoUtente(utente, ruoloCreatore);
            infoUtente.idPeople = utente.systemId;
            return infoUtente;
        }

        ///// <summary>
        ///// Impersonate con le credenziali dell'utente creatore di un documento
        ///// </summary>
        ///// <param name="row"></param>
        ///// <param name="amministrazione"></param>
        ///// <param name="ruoloCreatore"></param>
        ///// <returns></returns>
        //private static InfoUtente ImpersonateCreatoreDocumento(DataRow row, InfoAmministrazione amministrazione, out Ruolo ruoloCreatore)
        //{
        //    // 1. Reperimento del ruolo creatore del documento
        //    string idProfile = DataReaderHelper.GetValue<object>(row, "system_id", false).ToString();
        //    string author = DataReaderHelper.GetValue<object>(row, "author", false).ToString();
        //    string idRuoloCreatore = DataReaderHelper.GetValue<object>(row, "id_ruolo_creatore", true, string.Empty).ToString();

        //    if (string.IsNullOrEmpty(idRuoloCreatore))
        //        // Ricerca del ruolo creatore in security, qualora non sia stato trovato tramite le info nella profile
        //        ruoloCreatore = DocsPaDocumentale_DOCUMENTUM.DocsPaServices.DocsPaQueryHelper.getRuoloCreatore(idProfile);
        //    else
        //        ruoloCreatore = DocsPaDocumentale_DOCUMENTUM.DocsPaServices.DocsPaQueryHelper.getRuolo(idRuoloCreatore);

        //    if (ruoloCreatore == null)
        //        // 1a. Impossibile reperire il ruolo creatore del documento, 
        //        throw new ApplicationException(string.Format("Si è verificato un errore nel reperimento del ruolo creatore del documento con Id '{0}'", idProfile));

        //    // 2. Per creare il documento, è necessario fare connettersi in dctm come l'utente creatore
        //    DocsPaVO.utente.Utente utente = DocsPaDocumentale_DOCUMENTUM.DocsPaServices.DocsPaQueryHelper.getUtente(author);

        //    UserLogin.LoginResult loginResult;
        //    string authenticationToken = LoginServices.LoginUserDctm(utente.userId, Organigramma.DEFAULT_USER_PASSWORD, amministrazione.IDAmm, out loginResult);

        //    if (string.IsNullOrEmpty(authenticationToken))
        //    {
        //        // 2a. Errore nella login dell'utente creatore del documento
        //        throw new ApplicationException(string.Format("Si è verificato un errore nella connessione come l'utente creatore del documento '{0}'", idProfile));
        //    }
        //    else
        //    {
        //        Log.Write(string.Format("Connessione al sistema come l'utente '{0}' creatore del documento con Id '{1}'", utente.userId, idProfile), false);
        //    }

        //    InfoUtente infoUtente = new InfoUtente(utente, ruoloCreatore);
        //    infoUtente.idPeople = utente.systemId;
        //    infoUtente.dst = authenticationToken;
        //    return infoUtente;
        //}

        /// <summary>
        /// Verifica se la versione risulta acquisita
        /// </summary>
        /// <param name="fileRequest"></param>
        /// <returns></returns>
        private static bool IsFileAcquisito(DocsPaVO.documento.FileRequest fileRequest)
        {
            return (fileRequest != null && fileRequest.fileName != null && fileRequest.fileName != string.Empty &&
                    fileRequest.fileSize != null && fileRequest.fileSize != "0");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileRequest"></param>
        /// <returns></returns>
        private static DocsPaVO.documento.FileDocumento GetFileDocumento(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.documento.FileRequest fileRequest)
        {
            DocsPaVO.documento.FileDocumento fileDocumento = null;

            if (IsFileAcquisito(fileRequest))
            {   
                // Reperimento file acquisito nel documentale etdocs
                IDocumentManager documentManagerEtdocs = new DocsPaDocumentale_ETDOCS.Documentale.DocumentManager(infoUtente);
                
                byte[] content = documentManagerEtdocs.GetFile(fileRequest.docNumber, fileRequest.version, fileRequest.versionId, fileRequest.versionLabel);

                fileDocumento = new DocsPaVO.documento.FileDocumento();
                fileDocumento.fullName = System.IO.Path.GetFileName(fileRequest.fileName);
                fileDocumento.content = content;
                
                string estensione = System.IO.Path.GetExtension(fileRequest.fileName);
                if (estensione.StartsWith("."))
                    estensione = estensione.Substring(1);
                fileDocumento.estensioneFile = estensione;
            }

            return fileDocumento;
        }

        /// <summary>
        /// Reperimento query documenti
        /// </summary>
        /// <param name="amministrazione"></param>
        /// <param name="filtro"></param>
        /// <returns></returns>
        private static string GetQueryDocumenti(InfoAmministrazione amministrazione, Documento.FiltroMigrazioneDocumento filtro)
        {
            string filterString = string.Empty;

            if (filtro != null)
            {
                // Impostazione parametri di filtro
                filterString = filtro.ToString();
            }

            string query = string.Format(
                "SELECT p.system_id, p.docnumber, p.var_segnatura, p.cha_tipo_proto, p.creation_date, p.author, pl.user_id, p.id_ruolo_creatore, g.var_cod_rubrica, g.id_gruppo as id_ruolo_creatore_groups " +
                "FROM profile p " +
                "INNER JOIN people pl ON p.author = pl.system_id  " +
                "INNER JOIN dpa_corr_globali g ON P.id_ruolo_creatore = g.system_id " +
                "WHERE {0} " + 
                "ORDER BY p.system_id ASC", 
                filterString);

            return query;
        }

        /// <summary>
        /// Reperimento query dei documenti in fascicolo
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        private static string GetQueryDocumentiFascicoli(InfoUtente infoUtente, string idprofile)
        {
            return string.Format(
                "SELECT pc.project_id " +
                "FROM project_components pc " +
                "INNER JOIN project pj ON pc.project_id = pj.system_id " +
                "INNER JOIN profile pr ON pc.link = pr.system_id " +
                "WHERE pr.system_id = {0}", idprofile);
        }


        /// <summary>
        /// Reperimento query per documenti di tipo stampa registro e repertorio
        /// </summary>
        /// <param name="amministrazione"></param>
        /// <returns></returns>
        private static string GetQueryStampaRegistri(InfoAmministrazione amministrazione)
        {
            return string.Format(
                "SELECT p.system_id, p.docnumber, p.var_prof_oggetto, p.creation_date, p.author, '' as id_ruolo_creatore, pl.user_id, p.id_registro,  " +
                "v.version_id, v.version, v.subversion, v.version_label, c.path, c.file_size, c.var_impronta " +
                "FROM profile p  " +
                "INNER JOIN people pl ON p.author = pl.system_id   " +
                "INNER JOIN versions v ON p.docnumber = v.docnumber " +
                "INNER JOIN components c ON p.docnumber = c.docnumber " +
                "WHERE p.cha_tipo_proto IN ('R', 'C') AND pl.id_amm = {0} " + //and to_char(creation_date, 'DD-MM-YYYY') = '21-07-2014' " +
                "ORDER BY p.system_id ASC", amministrazione.IDAmm);
        }

        /// <summary>
        /// Creazione hash del documento
        /// </summary>
        /// <param name="schedaDocumento"></param>
        /// <returns></returns>
        private static string GetHashDocumento(DocsPaVO.documento.SchedaDocumento schedaDocumento,bool sha256)
        {
            string retValue = string.Empty;

            BinaryFormatter formatter = new BinaryFormatter();

            SchedaDocumentoHash schedaHash = new SchedaDocumentoHash();
            schedaHash.Documento = schedaDocumento;
            schedaHash.Fascicoli = GetListaFascicoliDocumento(schedaDocumento.systemId);
            schedaHash.SecurityItems = DocsPaDocumentale_DOCUMENTUM.DocsPaServices.DocsPaQueryHelper.getSecurityItems(schedaDocumento.systemId);

            using (MemoryStream stream = new MemoryStream())
            {
                formatter.Serialize(stream, schedaHash);
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
        /// <param name="idProfile"></param>
        /// <returns></returns>
        private static string[] GetListaFascicoliDocumento(string idProfile)
        {
            List<string> list = new List<string>();

            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                using (IDataReader reader = dbProvider.ExecuteReader(GetQueryDocumentiFascicoli(null, idProfile)))
                {
                    while (reader.Read())
                        list.Add(DataReaderHelper.GetValue<object>(reader, "project_id", false).ToString());
                }
            }

            return list.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        [Serializable()]
        private class SchedaDocumentoHash
        {
            public DocsPaVO.documento.SchedaDocumento Documento;
            public string[] Fascicoli = new string[0];
            public DocsPaDocumentale_DOCUMENTUM.DocsPaServices.DocsPaQueryHelper.DocsPaSecurityItem[] SecurityItems = new DocsPaDocumentale_DOCUMENTUM.DocsPaServices.DocsPaQueryHelper.DocsPaSecurityItem[0];
        }

        /// <summary>
        /// 
        /// </summary>
        [Serializable()]
        public class FiltroMigrazioneDocumento
        {
            public string IdAmministrazione;
            public string DocNumberIniziale;
            public string DocNumberFinale;
            public string DataCreazioneIniziale;
            public string DataCreazioneFinale;

            public override string ToString()
            {
                // Impostazione parametri di filtro
                string filter = string.Format("pl.id_amm = {0} AND p.cha_tipo_proto IN ('G', 'I', 'A', 'P') AND p.id_documento_principale IS NULL", this.IdAmministrazione);

                if (!string.IsNullOrEmpty(this.DocNumberIniziale) && !string.IsNullOrEmpty(this.DocNumberFinale))
                    filter = string.Format("{0} AND p.docnumber >= {1} AND p.docnumber <= {2}", filter, this.DocNumberIniziale, this.DocNumberFinale);
                else if (!string.IsNullOrEmpty(this.DocNumberIniziale))
                    filter = string.Format("{0} AND p.docnumber >= {1}", filter, this.DocNumberIniziale);
                else if (!string.IsNullOrEmpty(this.DocNumberFinale))
                    filter = string.Format("{0} AND p.docnumber <= {1}", filter, this.DocNumberFinale);

                return filter;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [Serializable()]
        public class OpzioniMigrazioneDocumento
        {
            public bool IgnoraSeEsistente;
            public bool SovrascriviDocumentiEsistenti;
            public bool ImportaSoloMetadatiDocumenti;
            public FiltroMigrazioneDocumento Filtro;
        }
    }
}