using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Configuration;
using System.Text.RegularExpressions;
using DocsPaVO.Validations;
using DocsPaVO.utente;

namespace DocsPaPwdServices
{
    /// <summary>
    /// Classe per la gestione della validazione delle password dell'utente.
    /// 
    /// Gestisce la nuova funzionalità supportata in amministrazione relativa
    /// alla validità temporale delle password.
    /// Se la gestione non è supportata dall'amministrazione
    /// la classe, nei servizi richiesti, effettuerà le validazioni
    /// come per la gestione attuale
    /// </summary>
    public sealed class UserPasswordServices
    {
        /// <summary>
        /// Id amministrazione null
        /// </summary>
        private const int ID_AMM_NULL = 0;

        /// <summary>
        /// 
        /// </summary>
        private UserPasswordServices()
        { }

        #region Public methods

        /// <summary>
        /// Verifica se la gestione scadenza password è abilitata per l'utente
        /// censito in un'amministrazione 
        /// </summary>
        /// <param name="idAmministrazione"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static bool PasswordExpirationEnabled(int idAmministrazione, string userId)
        {
            // Verifica se per l'utente l'autenticazione impostata è quella di dominio
            bool retValue = (!DomainAuthenticationEnabled(idAmministrazione, userId));

            if (retValue)
            {
                // Verifica se, da amministrazione, è abilitata o meno la gestione della scadenza password
                retValue = DocsPaDB.Query_DocsPAWS.AdminPasswordConfig.GetPasswordConfigurations(idAmministrazione).ExpirationEnabled;
            }

            return retValue;
        }

        /// <summary>
        /// Verifica se la gestione scadenza password è abilitata per l'utente
        /// censito in un'amministrazione
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static bool PasswordExpirationEnabled(string userId)
        {
            return PasswordExpirationEnabled(GetIdAmministrazione(userId), userId);
        }

        /// <summary>
        /// Validazione della password per il processo di login dell'utente
        /// </summary>
        /// <param name="idAmministrazione"></param>
        /// <param name="userId"></param>
        /// <param name="password"></param>        
        /// <returns></returns>
        public static bool IsValidForLogon(string userId, string password)
        {
            //MEV utenti multi-amministrazione
            DocsPaDB.Query_DocsPAWS.UserPassword userPassword = new DocsPaDB.Query_DocsPAWS.UserPassword(userId);

            return userPassword.CheckPasswordEquality(EncryptPassword(password));
        }

        /// <summary>
        /// Validazione della password per il processo di login dell'utente in una amministrazione
        /// </summary>
        /// <param name="idAmministrazione"></param>
        /// <param name="userId"></param>
        /// <param name="password"></param>        
        /// <returns></returns>
        public static bool IsValidForLogon(string userId, string password, string codAmm)
        {
            //MEV utenti multi-amministrazione
            DocsPaDB.Query_DocsPAWS.UserPassword userPassword = new DocsPaDB.Query_DocsPAWS.UserPassword(userId, codAmm);

            return userPassword.CheckPasswordEqualityInAmministrazione(EncryptPassword(password));
        }


        /// <summary>
        /// Verifica se la password per l'utente è scaduta
        /// </summary>
        /// <remarks>
        /// La password è considerata automaticamente scaduta nel
        /// caso in cui la password è stata impostata dall'amministratore
        /// </remarks>
        /// <param name="idAmministrazione"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static bool IsPasswordExpired(string userId)
        {
            DocsPaDB.Query_DocsPAWS.UserPassword userPassword = new DocsPaDB.Query_DocsPAWS.UserPassword(userId); // idAmministrazione);

            return userPassword.IsPasswordExpired();
        }

        //MEV utenti multi-amministrazione
        /// <summary>
        /// Verifica se la password per l'utente di un'amministrazione è scaduta 
        /// </summary>
        /// <remarks>
        /// La password è considerata automaticamente scaduta nel
        /// caso in cui la password è stata impostata dall'amministratore
        /// </remarks>
        /// <param name="idAmministrazione"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static bool IsPasswordExpired(string userId, string codAmm)
        {
            DocsPaDB.Query_DocsPAWS.UserPassword userPassword = new DocsPaDB.Query_DocsPAWS.UserPassword(userId, codAmm);

            return userPassword.IsPasswordUserInAmmExpired();
        }



        /// <summary>
        /// Reperimento del giorno di scadenza della password per l'utente
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static DateTime GetPasswordExpirationDate(string userId)
        {
            DocsPaDB.Query_DocsPAWS.UserPassword userPassword = new DocsPaDB.Query_DocsPAWS.UserPassword(userId);

            return userPassword.GetPasswordExpirationDate();
        }

        /// <summary>
        /// Reperimento del numero di giorni rimanenti di validità della password
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static int GetPasswordRemainingDays(string userId)
        {
            DocsPaDB.Query_DocsPAWS.UserPassword userPassword = new DocsPaDB.Query_DocsPAWS.UserPassword(userId);

            return userPassword.GetPasswordRemainingDays();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idAmministrazione"></param>
        /// <param name="userId"></param>
        /// <param name="password"></param>
        /// <param name="isAdminPassword"></param>
        /// <param name="force">
        /// Se true, sono disabilitati i controlli sulla validità formale della password
        /// </param>
        /// <returns></returns>
        public static ValidationResultInfo SetPassword(int idAmministrazione, string userId, string password, bool isAdminPassword, bool force, string modulo="")
        {
            ValidationResultInfo result = new ValidationResultInfo();

            if (!isAdminPassword && DomainAuthenticationEnabled(idAmministrazione, userId))
            {
                // La password non può essere modificata dall'utente qualora 
                // per esso sia stata attivata l'autenticazione di dominio
                result.BrokenRules.Add(CreateBrokenRule("DOMAIN_AUTH_ENABLED", "La password non può essere modificata in quanto per l'utente è attivata l'autenticazione di dominio", BrokenRule.BrokenRuleLevelEnum.Error));
            }
            else
            {
                //MEV utenti multi-amministrazione - aggiunto parametro idAmministrazione
                DocsPaDB.Query_DocsPAWS.UserPassword userPassword = new DocsPaDB.Query_DocsPAWS.UserPassword(userId,idAmministrazione.ToString());

                if (!force)
                {
                    // Verifica se la password immessa non sia uguale a quella attualmente impostata
                    if (userPassword.CheckPasswordEquality(EncryptPassword(password)))
                    {
                        result.BrokenRules.Add(CreateBrokenRule("PASSWORD_EQUALITY", "La password deve essere differente rispetto a quella precedente", BrokenRule.BrokenRuleLevelEnum.Error));
                    }

                    if (result.BrokenRules.Count == 0 && !idAmministrazione.Equals(ID_AMM_NULL))
                    {
                        // Vengono effettuati i controlli di validità sulla password,
                        // solo se l'utente è legato ad un'amministrazione (se non lo è, è un superadmin)
                        result.BrokenRules.AddRange(CheckPasswordString(password, idAmministrazione).BrokenRules);
                    }
                }

                if (result.BrokenRules.Count == 0)
                {
                    try
                    {
                        //MEV utenti multi-amministrazione - aggiunto chiamata a nuovo metodo
                        userPassword.SetPasswordForUserinAmm(EncryptPassword(password),modulo);
                    }
                    catch (Exception ex)
                    {
                        // Errore nell'impostazione della password
                        result.BrokenRules.Add(CreateBrokenRule("SET_PASSWORD_ERROR", ex.Message, BrokenRule.BrokenRuleLevelEnum.Error));
                    }
                }
            }

            result.Value = (result.BrokenRules.Count == 0);

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idAmministrazione"></param>
        /// <param name="userId"></param>
        /// <param name="password"></param>
        /// <param name="isAdminPassword"></param>
        /// <returns></returns>
        public static ValidationResultInfo SetPassword(int idAmministrazione, string userId, string password, bool isAdminPassword)
        {
            return SetPassword(idAmministrazione, userId, password, isAdminPassword, false,"Amministrazione");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="password"></param>
        /// <param name="isAdminPassword"></param>
        /// <param name="force"></param>
        /// <returns></returns>
        public static ValidationResultInfo SetPassword(string userId, string password, bool isAdminPassword, bool force)
        {
            return SetPassword(GetIdAmministrazione(userId), userId, password, isAdminPassword, force);
        }

        /// <summary>
        /// Impostazione di una nuova password per l'utente
        /// </summary>
        /// <param name="idAmministrazione"></param>
        /// <param name="userId"></param>
        /// <param name="password"></param>
        /// <param name="isAdminPassword"></param>
        /// <returns></returns>
        public static ValidationResultInfo SetPassword(string userId, string password, bool isAdminPassword)
        {
            return SetPassword(userId, password, isAdminPassword, false);
        }

        /// <summary>
        /// Impostazione di una nuova password per l'utente
        /// </summary>
        /// <param name="idAmministrazione"></param>
        /// <param name="userId"></param>
        /// <param name="password"></param>
        /// <param name="isAdminPassword"></param>
        /// <returns></returns>
        public static ValidationResultInfo SetPassword(DocsPaVO.utente.UserLogin user, bool isAdminPassword)
        {
            int idAmm = 0;
            if (!string.IsNullOrEmpty(user.IdAmministrazione))
            {
                idAmm = Convert.ToInt32(user.IdAmministrazione);
            }
            return SetPassword(idAmm, user.UserName, user.Password, isAdminPassword, false, user.Modulo);
        }

        /// <summary>
        /// Impostazione dell'opzione "Nessuna scadenza password" per l'utente
        /// </summary>
        /// <param name="neverExpire"></param>
        /// <returns></returns>
        public static void SetPasswordNeverExpireOption(bool neverExpire, string userId) // int idAmministrazione
        {
            DocsPaDB.Query_DocsPAWS.UserPassword userPassword = new DocsPaDB.Query_DocsPAWS.UserPassword(userId);

            userPassword.SetPasswordNeverExpireOption(neverExpire);
        }

        /// <summary>
        /// Verifica se l'opzione "Nessuna scadenza password" è abilitata per l'utente
        /// </summary>
        /// <param name="idAmministrazione"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static bool PasswordNeverExpire(string userId) // int idAmministrazione
        {
            DocsPaDB.Query_DocsPAWS.UserPassword userPassword = new DocsPaDB.Query_DocsPAWS.UserPassword(userId);

            return userPassword.PasswordNeverExpire();
        }

        /// <summary>
        /// Verifica se la password dell'utente è stata definita dall'utente stesso
        /// o dall'amministratore
        /// </summary>
        /// <param name="idAmministrazione"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static bool IsUserPassword(string userId) // int idAmministrazione
        {
            DocsPaDB.Query_DocsPAWS.UserPassword userPassword = new DocsPaDB.Query_DocsPAWS.UserPassword(userId);

            return userPassword.IsUserPassword();
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Verifica della validità formale della password
        /// </summary>
        /// <param name="password"></param>
        /// <param name="idAmministrazione"></param>
        /// <returns></returns>
        private static ValidationResultInfo CheckPasswordString(string password, int idAmministrazione)
        {
            ValidationResultInfo result = new ValidationResultInfo();

            // Verifica della presenza di caratteri non validi
            if (!CheckPasswordInvalidChars(password))
                result.BrokenRules.Add(CreateBrokenRule("PASSWORD_INVALID_CHARS", "La password contiene caratteri non ammessi", BrokenRule.BrokenRuleLevelEnum.Error));

            // Verifica della lunghezza della password fornita
            DocsPaVO.amministrazione.PasswordConfigurations config = AdminPasswordConfigServices.GetPasswordConfigurations(idAmministrazione);

            if (password.Length < config.MinLength)
                result.BrokenRules.Add(CreateBrokenRule("PASSWORD_LENGHT", string.Format("La lunghezza della password deve essere di almeno {0} caratteri", config.MinLength.ToString()), BrokenRule.BrokenRuleLevelEnum.Error));

            // Verifica la presenza di caratteri speciali obbligatori
            if (config.SpecialCharacters.Length > 0 && password.IndexOfAny(config.SpecialCharacters) == -1)
                result.BrokenRules.Add(CreateBrokenRule("PASSWORD_REQUIRED_SPECIAL_CHARS", string.Format("La password deve contenere almeno uno tra i seguenti caratteri speciali:{0}", " " + new string(config.SpecialCharacters)), BrokenRule.BrokenRuleLevelEnum.Error));
            //result.BrokenRules.Add(CreateBrokenRule("PASSWORD_REQUIRED_SPECIAL_CHARS", "La password deve contenere almeno uno tra i caratteri speciali previsti in amministrazione", BrokenRule.BrokenRuleLevelEnum.Error));

            return result;
        }

        /// <summary>
        /// Verifica della presenza di caratteri non validi della password
        /// - Spazio
        /// - Backslash
        /// - Slash
        /// 
        /// </summary>
        /// <param name="clearPassword"></param>
        /// <returns></returns>
        private static bool CheckPasswordInvalidChars(string clearPassword)
        {
            return true;
        }

        /// <summary>
        /// Encryption della password in chiaro
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        private static string EncryptPassword(string clearPassword)
        {
            return DocsPaUtils.Security.CryptographyManager.CalcolaImpronta(System.Text.Encoding.Unicode.GetBytes(clearPassword));
        }

        /// <summary>
        /// Creazione oggetto BrokenRule
        /// </summary>
        /// <param name="id"></param>
        /// <param name="description"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        private static BrokenRule CreateBrokenRule(string id, string description, BrokenRule.BrokenRuleLevelEnum level)
        {
            BrokenRule brokenRule = new BrokenRule();
            brokenRule.ID = id;
            brokenRule.Description = description;
            brokenRule.Level = level;
            return brokenRule;
        }

        /// <summary>
        /// Verifica se per l'utente (o, più in generale, per l'amministrazione)
        /// è attiva l'autenticazione di dominio. 
        /// In tal caso, l'intera gestione scadenza password verrà ignorata.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        private static bool DomainAuthenticationEnabled(int idAmministrazione, string userId)
        {
            DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();

            return (!string.IsNullOrEmpty(utenti.GetUtente(userId, IdAmministrazioneToString(idAmministrazione)).dominio));
        }

        /// <summary>
        /// Reperimento valore stringa idamministrazione
        /// </summary>
        /// <param name="idAmministrazione"></param>
        /// <returns></returns>
        private static string IdAmministrazioneToString(int idAmministrazione)
        {
            if (!idAmministrazione.Equals(ID_AMM_NULL))
                return idAmministrazione.ToString();
            else
                return string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        private static int GetIdAmministrazione(string userId)
        {
            int idAmministrazione = ID_AMM_NULL;

            // Se non è stato immesso alcun valore per l'amministrazione, 
            // è necessario reperire automaticamente l'id in base alla userid fornita
            DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();
            ArrayList ammList;
            utenti.GetIdAmmUtente(out ammList, userId);

            if (ammList != null && ammList.Count == 1)
                Int32.TryParse(ammList[0].ToString(), out idAmministrazione);

            return idAmministrazione;
        }

        #endregion
    }
}