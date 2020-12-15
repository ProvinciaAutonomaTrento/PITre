using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;
using DocsPaDocumentale.Interfaces;
using DocsPaVO.amministrazione;
using DocsPaVO.utente;
using DocsPaUtils.Data;

namespace DocsPaDocumentale_HERMES.Migrazione
{
    /// <summary>
    /// Gestione migrazione Titolario
    /// </summary>
    internal sealed class Titolario
    {
        /// <summary>
        /// 
        /// </summary>
        private const string CODICE_TITOLARIO = "T";

        /// <summary>
        /// 
        /// </summary>
        private Titolario()
        { }

        /// <summary>
        /// Implementazione della logica del task di migrazione dati
        /// per tutti i titolari di una singola amministrazione DocsPa  
        /// </summary>
        /// <param name="amministrazione"></param>
        
        public static void ImportaTitolari(InfoAmministrazione amministrazione)
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
                    // 2. Migrazione dati singola amministrazione
                    ImportaTitolari(infoUtente, amministrazione);

                    Log.GetInstance(amministrazione).Write("Procedura di migrazione completata con successo", false);
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
        /// Implementazione della logica del task di migrazione dati
        /// per tutti i titolari di una singola amministrazione DocsPa 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="amministrazione"></param>
        
        internal static void ImportaTitolari(InfoUtente infoUtente, InfoAmministrazione amministrazione)
        {
            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                // 1. Reperimento titolari per l'amministrazione
                using (DataSet ds = new DataSet())
                {
                    if (dbProvider.ExecuteQuery(ds, GetQueryTitolari(infoUtente, amministrazione)))
                    {
                        //DocsPaDocumentale_DOCUMENTUM.Documentale.TitolarioManager titolarioManager = new DocsPaDocumentale_DOCUMENTUM.Documentale.TitolarioManager(infoUtente);

                        foreach (DataRow row in ds.Tables[0].Rows)
                        {
                            string codice = DataReaderHelper.GetValue<string>(row, "CODICE", false);

                            if (codice == CODICE_TITOLARIO)
                            {
                                // 2. Import titolario
                                OrgTitolario titolario = GetTitolario(row, infoUtente, amministrazione);
                                /*
                                if (titolarioManager.SaveTitolario(titolario))
                                    Log.GetInstance(amministrazione).Write(string.Format("Migrazione titolario. Codice: '{0}' - Descrizione: '{1}'", titolario.Codice, titolario.Descrizione), false);
                                else
                                    // 2a. Errore nell'inserimento del titolario
                                    throw new ApplicationException(
                                        string.Format("Si è verificato un errore nell'import del titolario '{0}' per l'amministrazione '{1}'",
                                        titolario.Codice, amministrazione.Codice));*/
                            }
                            else
                            {
                                // 3. Import nodo titolario
                                OrgNodoTitolario nodoTitolario = GetNodoTitolario(row, infoUtente, amministrazione);
                                /*
                                if (titolarioManager.ContainsNodoTitolario(nodoTitolario.ID))
                                {
                                    // Nodo titolario già esistente, save dei dati con refresh delle entries dell'acl associata
                                    titolarioManager.SaveNodoTitolario(nodoTitolario, true);

                                    Log.GetInstance(amministrazione).Write(string.Format("Migrazione nodo titolario. Codice: '{0}' - Descrizione: '{1}'. Aggiornamento.", nodoTitolario.Codice, nodoTitolario.Descrizione), false);
                                }
                                else
                                {
                                    if (titolarioManager.SaveNodoTitolario(nodoTitolario))
                                        Log.GetInstance(amministrazione).Write(string.Format("Migrazione nodo titolario. Codice: '{0}' - Descrizione: '{1}'", nodoTitolario.Codice, nodoTitolario.Descrizione), false);
                                    else
                                        // 3a. Errore nell'inserimento del nodo titolario
                                        throw new ApplicationException(
                                            string.Format("Si è verificato un errore nell'import del nodo titolario '{0}' per l'amministrazione '{1}'",
                                            nodoTitolario.Codice, amministrazione.Codice));
                                }*/
                            }
                        }
                    }
                    else
                    {
                        // 1a. Errore nel reperimento dei titolari per l'amministrazione
                        throw new ApplicationException(
                            string.Format("Si è verificato un errore nel reperimento dei titolari per l'amministrazione '{0}'",
                            amministrazione.Codice));
                    }
                }
            }
        }
        
        /// <summary>
        /// Creazione oggetto titolario
        /// </summary>
        /// <param name="row"></param>
        /// <param name="infoUtente"></param>
        /// <param name="amministrazione"></param>
        /// <returns></returns>
        private static OrgTitolario GetTitolario(DataRow row, InfoUtente infoUtente, InfoAmministrazione amministrazione)
        {
            OrgTitolario titolario = new OrgTitolario();

            titolario.ID = DataReaderHelper.GetValue<object>(row, "IDRECORD", false).ToString();
            titolario.Codice = CODICE_TITOLARIO;
            titolario.CodiceAmministrazione = amministrazione.Codice;
            titolario.Descrizione = DataReaderHelper.GetValue<string>(row, "DESCRIZIONE", false);
            titolario.SetStatoTitolario(DataReaderHelper.GetValue<string>(row, "CHA_STATO", false));
            titolario.DataAttivazione = DataReaderHelper.GetValue<DateTime>(row, "DTA_ATTIVAZIONE", false).ToString();
            if (titolario.Stato == OrgStatiTitolarioEnum.Chiuso)
                titolario.DataCessazione = DataReaderHelper.GetValue<DateTime>(row, "DTA_CESSAZIONE", false).ToString();
            titolario.Commento = DataReaderHelper.GetValue<string>(row, "VAR_NOTE", false);

            return titolario;
        }

        /// <summary>
        /// Creazione oggetto nodo di titolario
        /// </summary>
        /// <param name="row"></param>
        /// <param name="infoUtente"></param>
        /// <param name="amministrazione"></param>
        /// <returns></returns>
        private static OrgNodoTitolario GetNodoTitolario(DataRow row, InfoUtente infoUtente, InfoAmministrazione amministrazione)
        {
            OrgNodoTitolario nodoTitolario = new OrgNodoTitolario();

            nodoTitolario.ID = DataReaderHelper.GetValue<decimal>(row, "IDRECORD", false).ToString();
            nodoTitolario.Codice = DataReaderHelper.GetValue<string>(row, "CODICE", false);
            nodoTitolario.CodiceAmministrazione = amministrazione.Codice;
            nodoTitolario.Descrizione = DataReaderHelper.GetValue<string>(row, "DESCRIZIONE", false);
            nodoTitolario.Livello = DataReaderHelper.GetValue<decimal>(row, "LIVELLO", false).ToString();

            string idRegistro = DataReaderHelper.GetValue<decimal>(row, "REGISTRO", true, 0).ToString();
            if (idRegistro != "0")
                nodoTitolario.IDRegistroAssociato = idRegistro.ToString();

            string idParent = DataReaderHelper.GetValue<decimal>(row, "IDPARENT", true, 0).ToString();
            if (idParent != "0")
                nodoTitolario.IDParentNodoTitolario = idParent.ToString();

            nodoTitolario.CodiceLivello = DataReaderHelper.GetValue<string>(row, "CODLIV", false);
            nodoTitolario.CountChildNodiTitolario = Int32.Parse(DataReaderHelper.GetValue<decimal>(row, "FIGLIO", false).ToString());
            nodoTitolario.NumeroMesiConservazione = Int32.Parse(DataReaderHelper.GetValue<decimal>(row, "NUMMESICONSERVAZIONE", true).ToString());

            string idTipoFasc = DataReaderHelper.GetValue<decimal>(row, "ID_TIPO_FASC", true).ToString();
            if (idTipoFasc != "0")
                nodoTitolario.ID_TipoFascicolo = idTipoFasc.ToString();

            nodoTitolario.bloccaTipoFascicolo = DataReaderHelper.GetValue<string>(row, "CHA_BLOCCA_FASC", true);

            string idTitolario = DataReaderHelper.GetValue<decimal>(row, "ID_TITOLARIO", true).ToString();
            if (idTitolario != "0")
                nodoTitolario.ID_Titolario = idTitolario.ToString();

            nodoTitolario.stato = DataReaderHelper.GetValue<string>(row, "CHA_STATO", true);

            DateTime dataAttivazione = DataReaderHelper.GetValue<DateTime>(row, "DTA_ATTIVAZIONE", true);
            if (default(DateTime) != dataAttivazione)
                nodoTitolario.dataAttivazione = dataAttivazione.ToString();

            DateTime dataCessazione = DataReaderHelper.GetValue<DateTime>(row, "DTA_CESSAZIONE", true);
            if (default(DateTime) != dataCessazione)
                nodoTitolario.dataCessazione = dataCessazione.ToString();

            nodoTitolario.note = DataReaderHelper.GetValue<string>(row, "VAR_NOTE", true);

            return nodoTitolario;
        }

        /// <summary>
        /// Reperimento query titolario
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="amministrazione"></param>
        /// <returns></returns>
        private static string GetQueryTitolari(InfoUtente infoUtente, InfoAmministrazione amministrazione)
        {
            return
                string.Format(
                "SELECT DISTINCT " +
                        "a.system_id as IDRECORD, " +
                        "a.var_codice as CODICE, " +
                        "a.description as DESCRIZIONE, " +
                        "a.num_livello as LIVELLO, " +
                        "a.id_registro as REGISTRO, " +
                        "a.id_parent as IDPARENT, " +
                        "a.var_cod_liv1 as CODLIV, " +
                        "cha_rw as RW, " +
                        "(SELECT count(b.system_id) " +
                        "FROM PROJECT b " +
                        "WHERE b.cha_tipo_proj = 'T' " +
                        "AND b.id_amm = 1 " +
                        "AND b.id_parent = a.system_id " +
                        ") as FIGLIO, " +
                        "a.NUM_MESI_CONSERVAZIONE as NUMMESICONSERVAZIONE, " +
                        "a.ID_TIPO_FASC, " +
                        "a.CHA_BLOCCA_FASC, " +
                        "a.ID_TITOLARIO, " +
                        "a.CHA_STATO, " +
                        "a.DTA_ATTIVAZIONE, " +
                        "a.DTA_CESSAZIONE, " +
                        "a.VAR_NOTE " +
              "FROM  	PROJECT a " +
              "WHERE 	a.cha_tipo_proj = 'T' " +
                        "AND a.id_amm = {0} " +
              "ORDER BY a.var_cod_liv1 ", amministrazione.IDAmm);
        }
    }
}
