// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.amministrazione;

namespace BusinessLogic.Amministrazione
{
    public class AdminPasswordConfig
    {
        /// <summary>
        /// Reperimento dei dati per la gestione della password in amministrazione
        /// </summary>
        /// <returns></returns>
        public static PasswordConfigurations GetPasswordConfigurations(DocsPaVO.utente.InfoUtente infoUtente, int idAmministrazione)
        {
            DocsPaDocumentale.Documentale.AdminPasswordConfig pwdConfig = new DocsPaDocumentale.Documentale.AdminPasswordConfig();

            return pwdConfig.GetPasswordConfigurations(infoUtente, idAmministrazione);
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

        public static bool IsSupportedPasswordConfig()
        {
            DocsPaDocumentale.Documentale.AdminPasswordConfig pwdConfig = new DocsPaDocumentale.Documentale.AdminPasswordConfig();

            return pwdConfig.IsSupportedPasswordConfig();
        }

    }
}
