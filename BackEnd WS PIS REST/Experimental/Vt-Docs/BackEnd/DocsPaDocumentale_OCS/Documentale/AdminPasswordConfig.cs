using System;
using System.Collections.Generic;
using System.Text;
using DocsPaDocumentale.Interfaces;
using DocsPaVO.amministrazione;

namespace DocsPaDocumentale_OCS.Documentale
{
    public class AdminPasswordConfig : IAdminPasswordConfig
    {
        #region Public methods

        /// <summary>
        /// Indica se il documentale supporta la gestione delle configurazioni delle password
        /// </summary>
        /// <returns></returns>
        public bool IsSupportedPasswordConfig()
        {
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configurations"></param>
        /// <returns></returns>
        public bool SavePasswordConfigurations(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.amministrazione.PasswordConfigurations configurations)
        {
            throw new NotSupportedException("Servizio non supportato dal documentale");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idAmministrazione"></param>
        /// <returns></returns>
        public PasswordConfigurations GetPasswordConfigurations(DocsPaVO.utente.InfoUtente infoUtente, int idAmministrazione)
        {
            // Inizializzazione oggetto configurations
            PasswordConfigurations pwdConfigurations = new PasswordConfigurations();
            pwdConfigurations.IdAmministrazione = idAmministrazione;
            pwdConfigurations.ExpirationEnabled = false;
            pwdConfigurations.ValidityDays = 0;
            pwdConfigurations.MinLength = 0;
            pwdConfigurations.SpecialCharacters = new char[0];
            return pwdConfigurations;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idAmministrazione"></param>
        public void ExpireAllPassword(DocsPaVO.utente.InfoUtente infoUtente, int idAmministrazione)
        {
            throw new NotSupportedException("Servizio non supportato dal documentale");
        }

        #endregion
    }
}
