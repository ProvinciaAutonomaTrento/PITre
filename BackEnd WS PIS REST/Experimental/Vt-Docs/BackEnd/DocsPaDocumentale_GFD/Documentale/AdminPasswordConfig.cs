using System;
using System.Collections.Generic;
using System.Text;
using DocsPaDocumentale.Interfaces;
using DocsPaVO.amministrazione;
using DocsPaVO.utente;

namespace DocsPaDocumentale_GFD.Documentale
{
    public class AdminPasswordConfig : IAdminPasswordConfig
    {
        #region Ctors, constants, variables

        /// <summary>
        /// 
        /// </summary>
        private IAdminPasswordConfig _instanceETDOCS = null;

        /// <summary>
        /// </summary>
        /// <param name="infoUtente">Dati relativi all'utente</param>
        public AdminPasswordConfig()
        {
            //this._infoUtente = infoUtente;
        }

        #endregion

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
            return this.InstanceETDOCS.SavePasswordConfigurations(infoUtente, configurations);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idAmministrazione"></param>
        /// <returns></returns>
        public PasswordConfigurations GetPasswordConfigurations(DocsPaVO.utente.InfoUtente infoUtente, int idAmministrazione)
        {
            return this.InstanceETDOCS.GetPasswordConfigurations(infoUtente, idAmministrazione);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idAmministrazione"></param>
        public void ExpireAllPassword(DocsPaVO.utente.InfoUtente infoUtente, int idAmministrazione)
        {
            this.InstanceETDOCS.ExpireAllPassword(infoUtente, idAmministrazione);
        }

        #endregion

        #region Protected methods

        /// <summary>
        /// 
        /// </summary>
        protected IAdminPasswordConfig InstanceETDOCS
        {
            get
            {
                if (this._instanceETDOCS == null)
                    this._instanceETDOCS = new DocsPaDocumentale_ETDOCS.Documentale.AdminPasswordConfig();
                return this._instanceETDOCS;
            }
        }

        #endregion
    }
}
