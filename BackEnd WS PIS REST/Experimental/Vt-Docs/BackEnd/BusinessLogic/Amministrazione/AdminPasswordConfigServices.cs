using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using DocsPaVO.amministrazione;
using log4net;

namespace BusinessLogic.Amministrazione
{
    /// <summary>
    /// Classe per la gestione delle password per l'amministrazione
    /// </summary>
    public sealed class AdminPasswordConfig
    {
        private ILog logger = LogManager.GetLogger(typeof(AdminPasswordConfig));
        /// <summary>
        /// 
        /// </summary>
        private AdminPasswordConfig()
        {}

        /// <summary>
        /// Indica se il documentale supporta la gestione delle configurazioni delle password
        /// </summary>
        /// <returns></returns>
        public static bool IsSupportedPasswordConfig()
        {
            DocsPaDocumentale.Documentale.AdminPasswordConfig pwdConfig = new DocsPaDocumentale.Documentale.AdminPasswordConfig();

            return pwdConfig.IsSupportedPasswordConfig();
        }

        /// <summary>
        /// Save dei dati per la gestione delle password in amministrazione
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="configurations"></param>
        /// <returns></returns>
        public static bool SavePasswordConfigurations(DocsPaVO.utente.InfoUtente infoUtente, PasswordConfigurations configurations)
        {
            DocsPaDocumentale.Documentale.AdminPasswordConfig pwdConfig = new DocsPaDocumentale.Documentale.AdminPasswordConfig();

            return pwdConfig.SavePasswordConfigurations(infoUtente, configurations);
        }

        /// <summary>
        /// Imposta come scadute tutte le password per tutti gli utenti dell'amministrazione.
        /// Al prossimo login, tutti gli utenti saranno costretti a reimpostare la propria password.
        /// </summary>
        /// <param name="idAmministrazione"></param>
        public static void ExpireAllPassword(DocsPaVO.utente.InfoUtente infoUtente, int idAmministrazione)
        {
            DocsPaDocumentale.Documentale.AdminPasswordConfig pwdConfig = new DocsPaDocumentale.Documentale.AdminPasswordConfig();

            pwdConfig.ExpireAllPassword(infoUtente, idAmministrazione);
        }

        /// <summary>
        /// Reperimento dei dati per la gestione della password in amministrazione
        /// </summary>
        /// <returns></returns>
        public static PasswordConfigurations GetPasswordConfigurations(DocsPaVO.utente.InfoUtente infoUtente, int idAmministrazione)
        {
            DocsPaDocumentale.Documentale.AdminPasswordConfig pwdConfig = new DocsPaDocumentale.Documentale.AdminPasswordConfig();

            return pwdConfig.GetPasswordConfigurations(infoUtente, idAmministrazione);
        }
    }
}
