using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using DocsPaVO.amministrazione;

namespace DocsPaPwdServices
{
    /// <summary>
    /// Classe per la gestione della scadenza delle password da amministrazione
    /// </summary>
    public sealed class AdminPasswordConfigServices
    {
        private AdminPasswordConfigServices()
        { }

        #region Public methods

        /// <summary>
        /// Save dei dati per la gestione delle password in amministrazione
        /// </summary>
        /// <param name="configurations"></param>
        /// <returns></returns>
        public static bool SavePasswordConfigurations(PasswordConfigurations configurations)
        {
            return DocsPaDB.Query_DocsPAWS.AdminPasswordConfig.SavePasswordConfigurations(configurations);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idAmministrazione"></param>
        public static void ExpireAllPassword(int idAmministrazione)
        {
            DocsPaDB.Query_DocsPAWS.AdminPasswordConfig.ExpireAllPassword(idAmministrazione);
        }

        /// <summary>
        /// Reperimento dei dati per la gestione della password in amministrazione
        /// </summary>
        /// <returns></returns>
        public static PasswordConfigurations GetPasswordConfigurations(int idAmministrazione)
        {
            return DocsPaDB.Query_DocsPAWS.AdminPasswordConfig.GetPasswordConfigurations(idAmministrazione);
        }

        /// <summary>
        /// Verifica se, da configurazioni dell'amministrazione, 
        /// risulta abilitata l'autenticazione di dominio per tutti gli utenti
        /// </summary>
        /// <param name="idAmministrazione"></param>
        /// <returns></returns>
        public static bool DomainAuthenticationEnabled(int idAmministrazione)
        {
            // Verifica se, per l'amministrazione, è abilitata o meno l'autenticazione di dominio
            return DocsPaDB.Query_DocsPAWS.AdminPasswordConfig.DomainAuthenticationEnabled(idAmministrazione);
        }

        #endregion
    }
}