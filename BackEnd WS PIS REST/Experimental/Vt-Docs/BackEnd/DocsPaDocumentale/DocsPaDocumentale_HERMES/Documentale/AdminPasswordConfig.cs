using System;
using System.Collections.Generic;
using System.Text;
using DocsPaDocumentale.Interfaces;
using DocsPaVO.amministrazione;

namespace DocsPaDocumentale_HERMES.Documentale
{
    /// <summary>
    /// Gestione configurazioni password per il documentale ETDOCS
    /// </summary>
    public class AdminPasswordConfig : IAdminPasswordConfig
    {
        #region Public methods

        /// <summary>
        /// Indica se il documentale supporta la gestione delle configurazioni delle password
        /// </summary>
        /// <returns></returns>
        public bool IsSupportedPasswordConfig()
        {
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configurations"></param>
        /// <returns></returns>
        public bool SavePasswordConfigurations(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.amministrazione.PasswordConfigurations configurations)
        {
            return DocsPaPwdServices.AdminPasswordConfigServices.SavePasswordConfigurations(configurations);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idAmministrazione"></param>
        /// <returns></returns>
        public PasswordConfigurations GetPasswordConfigurations(DocsPaVO.utente.InfoUtente infoUtente, int idAmministrazione)
        {
            return DocsPaPwdServices.AdminPasswordConfigServices.GetPasswordConfigurations(idAmministrazione);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idAmministrazione"></param>
        public void ExpireAllPassword(DocsPaVO.utente.InfoUtente infoUtente, int idAmministrazione)
        {
            DocsPaPwdServices.AdminPasswordConfigServices.ExpireAllPassword(idAmministrazione);
        }

        #endregion
    }
}
