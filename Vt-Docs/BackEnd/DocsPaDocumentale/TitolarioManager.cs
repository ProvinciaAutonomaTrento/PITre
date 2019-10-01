using System;
using System.Collections.Generic;
using System.Text;
using DocsPaDocumentale.Interfaces;
using DocsPaVO.amministrazione;
using System.Configuration;

namespace DocsPaDocumentale.Documentale
{
    /// <summary>
    /// Gestione del titolario in amministrazione per il documentale corrente
    /// </summary>
    public class TitolarioManager : ITitolarioManager
    { 
        #region Ctros, variables, constants

        /// <summary>
        /// Tipo documentale corrente
        /// </summary>
        private static Type _type = null;

        /// <summary>
        /// Oggetto documentale corrente
        /// </summary>
        private ITitolarioManager _instance = null;

        /// <summary>
        /// Reperimento del tipo relativo al documentale corrente
        /// </summary>
        static TitolarioManager()
        {
            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["documentale"]))
            {
                string documentale = ConfigurationManager.AppSettings["documentale"].ToLower();

                if (documentale.Equals(TipiDocumentaliEnum.Etnoteam.ToString().ToLower()))
                    _type = typeof(DocsPaDocumentale_ETDOCS.Documentale.TitolarioManager);
                else if (documentale.Equals(TipiDocumentaliEnum.Hummingbird.ToString().ToLower()))
                    _type = typeof(DocsPaDocumentale_HUMMINGBIRD.Documentale.TitolarioManager);
                else if (documentale.Equals(TipiDocumentaliEnum.Filenet.ToString().ToLower()))
                    _type = typeof(DocsPaDocumentale_FILENET.Documentale.TitolarioManager);
                else if (documentale.Equals(TipiDocumentaliEnum.Pitre.ToString().ToLower()))
                    _type = typeof(DocsPaDocumentale_PITRE.Documentale.TitolarioManager);
                else if (documentale.Equals(TipiDocumentaliEnum.CDC.ToString().ToLower()))
                    _type = typeof(DocsPaDocumentale_CDC.Documentale.TitolarioManager);
                else if (documentale.Equals(TipiDocumentaliEnum.GFD.ToString().ToLower()))
                    _type = typeof(DocsPaDocumentale_GFD.Documentale.TitolarioManager);
                
                //Giordano Iacozzilli  08/10/2012 Aggiunta strato SharePoint
                else if (documentale.Equals(TipiDocumentaliEnum.SharePoint.ToString().ToLower()))
                    _type = typeof(DocsPaDocumentale_CDC_SP.Documentale.TitolarioManager);
                //Fine
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public TitolarioManager(DocsPaVO.utente.InfoUtente infoUtente)
        {
            this._instance = (ITitolarioManager)Activator.CreateInstance(_type, infoUtente);
        }

        #endregion

        #region Public Members

        /// <summary>
        /// Attivazione di un titolario
        /// </summary>
        /// <param name="titolario"></param>
        /// <returns></returns>
        public bool AttivaTitolario(OrgTitolario titolario)
        {
            return this.Instance.AttivaTitolario(titolario);
        }

         /// <summary>
        /// 
        /// </summary>
        /// <param name="titolario"></param>
        /// <returns></returns>
        public bool SaveTitolario(OrgTitolario titolario)
        {
            return this.Instance.SaveTitolario(titolario);
        }

        /// <summary>
        /// Cancellazione struttura di classificazione
        /// </summary>
        /// <param name="titolario"></param>
        /// <returns></returns>
        public bool DeleteTitolario(OrgTitolario titolario)
        {
            return this.Instance.DeleteTitolario(titolario);
        }

        public bool SaveNodoTitolario(OrgNodoTitolario nodoTitolario)
        {
            return this.Instance.SaveNodoTitolario(nodoTitolario);
        }

        public bool DeleteNodoTitolario(OrgNodoTitolario nodoTitolario)
        {
            return this.Instance.DeleteNodoTitolario(nodoTitolario);
        }

        public bool SetAclRuoloNodoTitolario(OrgNodoTitolario nodoTitolario, OrgRuoloTitolario ruoloTitolario)
        {
            return this.Instance.SetAclRuoloNodoTitolario(nodoTitolario, ruoloTitolario);
        }

        /// <summary>
        /// Aggiornamento visibilità di più ruoli su un titolario
        /// </summary>
        /// <param name="nodoTitolario"></param>
        /// <param name="ruoliTitolario"></param>
        /// <returns></returns>
        public DocsPaVO.amministrazione.EsitoOperazione[] SetAclNodoTitolario(DocsPaVO.amministrazione.OrgNodoTitolario nodoTitolario, DocsPaVO.amministrazione.OrgRuoloTitolario[] ruoliTitolario)
        {
            return this.Instance.SetAclNodoTitolario(nodoTitolario, ruoliTitolario);
        }

        #endregion

        #region Protected methods

        /// <summary>
        /// Oggetto documentale corrente
        /// </summary>
        protected ITitolarioManager Instance
        {
            get
            {
                return this._instance;
            }
        }

        #endregion
    }
}
