using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using DocsPaDocumentale.Interfaces;
using DocsPaDocumentale_DOCUMENTUM.Documentale;
using DocsPaVO.amministrazione;
using DocsPaVO.utente;
using log4net;

namespace DocsPaDocumentale_PITRE.Migrazione
{
    /// <summary>
    /// 
    /// </summary>
    internal sealed class Organigramma
    {
        private static ILog logger = LogManager.GetLogger(typeof(Organigramma));
        /// <summary>
        /// Constante che riporta il codice utilizzato dall'oggetto EsitoOperazione
        /// per identificare l'operazione andata a buon fine
        /// </summary>
        private const int RESULT_CODE_OK = 0;

        /// <summary>
        /// Nella migrazione degli utenti, tutte le password verranno
        /// impostate come scadute e verrà impostata una password di default
        /// a livello di amministrazione
        /// </summary>
        public const string DEFAULT_USER_PASSWORD = "password";

        /// <summary>
        /// 
        /// </summary>
        private const string DEFAULT_USER_MAIL = "migrazionepitre@valueteam.it";

        #region Gestione migrazione ruoli

        /// <summary>
        /// Implementazione della logica del task di migrazione dati
        /// per tutti i ruoli di una singola amministrazione DocsPa 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="amministrazione"></param>
        public static void ImportaRuoli(InfoUtente infoUtente, InfoAmministrazione amministrazione, out List<string> ruoliImportati)
        {
            ruoliImportati = new List<string>();

            // 1. Reperimento ruoli dell'amministrazione
            OrgRuolo[] roles = GetRuoli(infoUtente, amministrazione);

            if (roles.Length > 0)
            {
                Log.GetInstance(amministrazione).Write(string.Format("Reperimento dei ruoli dell'amministrazione. Numero ruoli: {0}", roles.Length.ToString()), false);

                // 2. Import di ciascun ruolo in DCTM
                DocsPaDocumentale_DOCUMENTUM.Documentale.OrganigrammaManager organigrammaManager = new DocsPaDocumentale_DOCUMENTUM.Documentale.OrganigrammaManager(infoUtente);

                foreach (OrgRuolo role in roles)
                {
                    // 3. Verifica esistenza ruolo
                    if (organigrammaManager.ContainsGroup(role.Codice))
                    {
                        Log.GetInstance(amministrazione).Write(string.Format("Ruolo '{0}' già esistente", role.Codice), false);
                    }
                    else
                    {
                        EsitoOperazione result = organigrammaManager.InserisciRuolo(role, false);

                        if (result.Codice == RESULT_CODE_OK)
                        {
                            Log.GetInstance(amministrazione).Write(string.Format("Migrazione ruolo '{0}'", role.Codice), false);

                            ruoliImportati.Add(role.Codice);
                        }
                        else
                        {
                            // 2a. Errore nell'import del ruolo (migrazione interrotta)
                            throw new ApplicationException(
                                    string.Format("Si è verificato un errore nell'import del ruolo '{0}' per l'amministrazione '{1}'. Codice: {2} - Descrizione: {3}",
                                    role.Codice, amministrazione.Codice, result.Codice.ToString(), result.Descrizione)
                                    );
                        }
                    }
                }
            }
            else
            {
                // 1a. Errore nella migrazione dei ruoli (migrazione interrotta)
                throw new ApplicationException(
                    string.Format("Si è verificato un errore nell'import dei ruoli per l'amministrazione '{0}'. Descrizione: Nessun ruolo trovato",
                    amministrazione.Codice));
            }
        }

        /// <summary>
        /// Reperimento di tutti i ruoli dell'amministrazione
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="amministrazione"></param>
        /// <returns></returns>
        private static OrgRuolo[] GetRuoli(InfoUtente infoUtente, InfoAmministrazione amministrazione)
        {
            List<OrgRuolo> list = new List<OrgRuolo>();

            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                string commandText = string.Format("SELECT * FROM DPA_CORR_GLOBALI WHERE CHA_TIPO_URP = 'R' AND ID_AMM = {0} AND NOT ID_GRUPPO IS NULL", amministrazione.IDAmm);

                using (IDataReader reader = dbProvider.ExecuteReader(commandText))
                {
                    while (reader.Read())
                    {
                        list.Add(GetRuolo(reader));
                    }
                }
            }

            return list.ToArray();
        }

        /// <summary>
        /// Creazione oggetto ruolo
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private static OrgRuolo GetRuolo(IDataReader reader)
        {
            OrgRuolo role = new OrgRuolo();

            role.IDCorrGlobale = DocsPaUtils.Data.DataReaderHelper.GetValue<object>(reader, "SYSTEM_ID", false).ToString();
            role.IDAmministrazione = DocsPaUtils.Data.DataReaderHelper.GetValue<object>(reader, "ID_AMM", true).ToString();
            role.Codice = DocsPaUtils.Data.DataReaderHelper.GetValue<string>(reader, "VAR_ORIGINAL_CODE", true);
            role.CodiceRubrica = role.Codice;
            role.Descrizione = DocsPaUtils.Data.DataReaderHelper.GetValue<string>(reader, "VAR_DESC_CORR", true);
            role.IDGruppo = DocsPaUtils.Data.DataReaderHelper.GetValue<object>(reader, "ID_GRUPPO", true).ToString();
         
            return role;
        }

        #endregion

        #region Gestione migrazione utenti

        /// <summary>
        /// Implementazione della logica del task di migrazione dati
        /// per tutti gli utenti di una singola amministrazione DocsPa
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="amministrazione"></param>
        public static void ImportaUtenti(InfoUtente infoUtente, InfoAmministrazione amministrazione)
        {
            // 1. Reperimento utenti dell'amministrazione
            OrgUtente[] users = GetUtenti(infoUtente, amministrazione);

            if (users.Length > 0)
            {
                Log.GetInstance(amministrazione).Write(string.Format("Reperimento degli utenti dell'amministrazione. Numero utenti: {0}", users.Length.ToString()), false);

                DocsPaDocumentale_DOCUMENTUM.Documentale.OrganigrammaManager organigrammaManager = new DocsPaDocumentale_DOCUMENTUM.Documentale.OrganigrammaManager(infoUtente);
                
                foreach (OrgUtente user in users)
                {
                    // 2. Verifica esistenza utente
                    if (organigrammaManager.ContainsUser(user.UserId))
                    {
                        Log.GetInstance(amministrazione).Write(string.Format("Utente '{0}' già esistente", user.UserId), false);
                    }
                    else
                    {
                        // 3. Impostazione password di tipo amministratore per l'utente
                        Log.GetInstance(amministrazione).Write(string.Format("Impostazione password di tipo amministratore per l'utente '{0}'. Nuova password: '{1}'", user.UserId, DEFAULT_USER_PASSWORD), false);
                        user.Password = DEFAULT_USER_PASSWORD;

                        // 4. Import di ciascun utente in DCTM
                        EsitoOperazione result = organigrammaManager.InserisciUtente(user);

                        if (result.Codice == RESULT_CODE_OK)
                        {
                            Log.GetInstance(amministrazione).Write(string.Format("Migrazione utente '{0}'", user.UserId), false);
                        }
                        else
                        {
                            // 3a. Errore nell'import dell'utente (migrazione interrotta)
                            throw new ApplicationException(string.Format("Si è verificato un errore nell'import dell'utente '{0}' per l'amministrazione '{1}'. Codice: {2} - Descrizione: {3}", user.UserId, amministrazione.Codice, result.Codice.ToString(), result.Descrizione));
                        }
                    }
                }
            }
            else
            {
                // 1a. Errore nella migrazione degli utenti (migrazione interrotta)
                throw new ApplicationException(string.Format("Si è verificato un errore nell'import degli utenti per l'amministrazione '{0}'. Descrizione: Nessun utente trovato", amministrazione.Codice));
            }
        }

        /// <summary>
        /// Implementazione della logica del task di migrazione dati
        /// per associare tutti gli utenti a tutti i ruoli per una singola amministrazione DocsPa 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="amministrazione"></param>
        public static void ImportaAssociazioniUtentiRuoli(InfoUtente infoUtente, InfoAmministrazione amministrazione)
        {
            // 1. Reperimento dati associazione
            UserRoleEntry[] entries = GetUserRoleEntries(infoUtente, amministrazione);

            DocsPaDocumentale_DOCUMENTUM.Documentale.OrganigrammaManager organigrammaManager = new DocsPaDocumentale_DOCUMENTUM.Documentale.OrganigrammaManager(infoUtente);

            foreach (UserRoleEntry entry in entries)
            {
                if (organigrammaManager.ContainsGroupUserById(entry.IdGroup, entry.IdPeople))
                {
                    // L'utente è già stato associato al ruolo
                    Log.GetInstance(amministrazione).Write(string.Format("Utente con id {0} già associato al gruppo con id {1}", entry.IdPeople, entry.IdGroup), false);
                }
                else
                {
                    Log.GetInstance(amministrazione).Write(string.Format("Reperimento delle associazioni utenti / ruoli dell'amministrazione. Numero associazioni: {0}", entries.Length.ToString()), false);

                    // 2. Associazione utente con il ruolo
                    EsitoOperazione retValue = organigrammaManager.InserisciUtenteInRuolo(entry.IdPeople.ToString(), entry.IdGroup.ToString());

                    if (retValue.Codice == RESULT_CODE_OK)
                    {
                        Log.GetInstance(amministrazione).Write(string.Format("Migrazione associazione utente / ruolo. IdPeople: {0} - IdGroup: {1}", entry.IdPeople.ToString(), entry.IdGroup.ToString()), false);
                    }
                    else
                    {
                        // 2a. Errore nell'associazione utente - ruolo
                        throw new ApplicationException(
                            string.Format("Si è verificato un errore nell'inserimento dell'utente con Id '{0}' nel gruppo con Id '{1}' per l'amministrazione '{2}'. Codice: {3} - Descrizione: {4}",
                            entry.IdPeople.ToString(), entry.IdGroup.ToString(), amministrazione.Codice, retValue.Codice.ToString(), retValue.Descrizione)
                            );
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="amministrazione"></param>
        /// <returns></returns>
        private static UserRoleEntry[] GetUserRoleEntries(InfoUtente infoUtente, InfoAmministrazione amministrazione)
        {
            List<UserRoleEntry> entries = new List<UserRoleEntry>();

            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                string commandText = string.Format("SELECT PG.PEOPLE_SYSTEM_ID, PG.GROUPS_SYSTEM_ID FROM PEOPLEGROUPS PG INNER JOIN PEOPLE P ON PG.PEOPLE_SYSTEM_ID = P.SYSTEM_ID WHERE P.ID_AMM = {0} AND DTA_FINE IS NULL ORDER BY PG.PEOPLE_SYSTEM_ID ASC, PG.GROUPS_SYSTEM_ID ASC", amministrazione.IDAmm);

                using (IDataReader reader = dbProvider.ExecuteReader(commandText))
                {
                    while (reader.Read())
                    {
                        entries.Add(new UserRoleEntry(reader));
                    }
                }
            }

            return entries.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        private sealed class UserRoleEntry
        {
            public UserRoleEntry(IDataReader reader)
            {
                this.IdGroup = DocsPaUtils.Data.DataReaderHelper.GetValue<object>(reader, "GROUPS_SYSTEM_ID", false).ToString();
                this.IdPeople = DocsPaUtils.Data.DataReaderHelper.GetValue<object>(reader, "PEOPLE_SYSTEM_ID", false).ToString();
            }

            public string IdGroup;
            public string IdPeople;
        }

        /// <summary>
        /// Reperimento di tutti gli utenti dell'amministrazione
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="amministrazione"></param>
        /// <returns></returns>
        private static OrgUtente[] GetUtenti(InfoUtente infoUtente, InfoAmministrazione amministrazione)
        {
            List<OrgUtente> list = new List<OrgUtente>();

            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                string commandText = string.Format("SELECT * FROM PEOPLE WHERE ID_AMM = {0} AND UPPER(USER_ID) != UPPER('{1}')", amministrazione.IDAmm, infoUtente.userId);

                using (IDataReader reader = dbProvider.ExecuteReader(commandText))
                {
                    while (reader.Read())
                    {
                        list.Add(GetUtente(reader));
                    }
                }
            }

            return list.ToArray();
        }

        /// <summary>
        /// Creazione oggetto utente
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private static OrgUtente GetUtente(IDataReader reader)
        {
            OrgUtente user = new OrgUtente();
            user.IDPeople = DocsPaUtils.Data.DataReaderHelper.GetValue<object>(reader, "SYSTEM_ID", false).ToString();
            logger.Debug("LEGGO UTENTI");
            user.UserId = DocsPaUtils.Data.DataReaderHelper.GetValue<string>(reader, "USER_ID", false);
            user.Codice = user.UserId;
            user.Password = DEFAULT_USER_PASSWORD; // Password di default impostata dall'amministratore

            string email = DocsPaUtils.Data.DataReaderHelper.GetValue<string>(reader, "EMAIL_ADDRESS", true);
            if (string.IsNullOrEmpty(email))
                user.Email = DEFAULT_USER_MAIL;
            else
                user.Email = email;

            string disabled = DocsPaUtils.Data.DataReaderHelper.GetValue<string>(reader, "DISABLED", true);
            if (!string.IsNullOrEmpty(disabled) && disabled == "N")
                user.Abilitato = "1";
            else
                user.Abilitato = "0";

            user.IDAmministrazione = DocsPaUtils.Data.DataReaderHelper.GetValue<object>(reader, "ID_AMM", true).ToString();
            user.Amministratore = DocsPaUtils.Data.DataReaderHelper.GetValue<string>(reader, "CHA_AMMINISTRATORE", true);

            return user;
        }

        #endregion
    }
}
