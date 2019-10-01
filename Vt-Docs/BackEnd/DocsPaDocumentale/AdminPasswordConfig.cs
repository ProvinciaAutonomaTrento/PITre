using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using DocsPaDocumentale.Interfaces;
using DocsPaVO.amministrazione;

namespace DocsPaDocumentale.Documentale
{
    /// <summary>
    /// Gestione configurazioni password per il documentale corrente
    /// </summary>
    public class AdminPasswordConfig : IAdminPasswordConfig
    {
        #region Ctros, variables, constants

        /// <summary>
        /// Tipo documentale corrente
        /// </summary>
        private static Type _type = null;

        /// <summary>
        /// Oggetto documentale corrente
        /// </summary>
        private IAdminPasswordConfig _instance = null;

        /// <summary>
        /// Reperimento del tipo relativo al documentale corrente
        /// </summary>
        static AdminPasswordConfig()
        {
            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["documentale"]))
            {
                string documentale = ConfigurationManager.AppSettings["documentale"].ToLower();

                if (documentale.Equals(TipiDocumentaliEnum.Etnoteam.ToString().ToLower()))
                    _type = typeof(DocsPaDocumentale_ETDOCS.Documentale.AdminPasswordConfig);

                else if (documentale.Equals(TipiDocumentaliEnum.Hummingbird.ToString().ToLower()))
                    _type = typeof(DocsPaDocumentale_HUMMINGBIRD.Documentale.AdminPasswordConfig);
                
                else if (documentale.Equals(TipiDocumentaliEnum.Filenet.ToString().ToLower()))
                    _type = typeof(DocsPaDocumentale_FILENET.Documentale.AdminPasswordConfig);
                
                else if (documentale.Equals(TipiDocumentaliEnum.Pitre.ToString().ToLower()))
                    _type = typeof(DocsPaDocumentale_PITRE.Documentale.AdminPasswordConfig);

                else if (documentale.Equals(TipiDocumentaliEnum.CDC.ToString().ToLower()))
                    _type = typeof(DocsPaDocumentale_CDC.Documentale.AdminPasswordConfig);

                else if (documentale.Equals(TipiDocumentaliEnum.GFD.ToString().ToLower()))
                    _type = typeof(DocsPaDocumentale_GFD.Documentale.AdminPasswordConfig);

             

                //Giordano Iacozzilli  08/10/2012 Aggiunta strato SharePoint
                else if (documentale.Equals(TipiDocumentaliEnum.SharePoint.ToString().ToLower()))
                    _type = typeof(DocsPaDocumentale_CDC_SP.Documentale.AdminPasswordConfig);
                //Fine

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        public AdminPasswordConfig()
        {
            this._instance = (IAdminPasswordConfig)Activator.CreateInstance(_type);
        }

        #endregion

        #region Protected methods

        /// <summary>
        /// Reperimento istanza oggetto "IAdminPasswordConfig"
        /// relativamente al documentale correntemente configurato
        /// </summary>
        protected IAdminPasswordConfig Instance
        {
            get
            {
                return this._instance;
            }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Indica se il documentale supporta la gestione delle configurazioni delle password
        /// </summary>
        /// <returns></returns>
        public bool IsSupportedPasswordConfig()
        {
            return this.Instance.IsSupportedPasswordConfig();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configurations"></param>
        /// <returns></returns>
        public bool SavePasswordConfigurations(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.amministrazione.PasswordConfigurations configurations)
        {
            return this.Instance.SavePasswordConfigurations(infoUtente, configurations);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idAmministrazione"></param>
        /// <returns></returns>
        public PasswordConfigurations GetPasswordConfigurations(DocsPaVO.utente.InfoUtente infoUtente, int idAmministrazione)
        {
            return this.Instance.GetPasswordConfigurations(infoUtente, idAmministrazione);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idAmministrazione"></param>
        public void ExpireAllPassword(DocsPaVO.utente.InfoUtente infoUtente, int idAmministrazione)
        {
            this.Instance.ExpireAllPassword(infoUtente, idAmministrazione);
        }

        #endregion
    }
}