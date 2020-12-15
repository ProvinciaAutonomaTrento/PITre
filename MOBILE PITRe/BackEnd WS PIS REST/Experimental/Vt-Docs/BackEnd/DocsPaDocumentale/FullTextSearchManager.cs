using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Collections;
using DocsPaVO.utente;
using DocsPaDocumentale.Interfaces;

namespace DocsPaDocumentale.Documentale
{
    /// <summary>
    /// 
    /// </summary>
    public class FullTextSearchManager : IFullTextSearchManager
    {
        #region Ctros, variables, constants

        /// <summary>
        /// Tipo documentale corrente
        /// </summary>
        private static Type _type = null;

        /// <summary>
        /// Oggetto documentale corrente
        /// </summary>
        private IFullTextSearchManager _instance = null;

        /// <summary>
        /// Reperimento del tipo relativo al documentale corrente
        /// </summary>
        static FullTextSearchManager()
        {
            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["documentale"]))
            {
                string documentale = ConfigurationManager.AppSettings["documentale"].ToLower();

                if (documentale.Equals(TipiDocumentaliEnum.Etnoteam.ToString().ToLower()))
                    _type = typeof(DocsPaDocumentale_ETDOCS.Documentale.FullTextSearchManager);
                else if (documentale.Equals(TipiDocumentaliEnum.Pitre.ToString().ToLower()))
                    _type = typeof(DocsPaDocumentale_PITRE.Documentale.FullTextSearchManager);
                else if (documentale.Equals(TipiDocumentaliEnum.CDC.ToString().ToLower()))
                    _type = typeof(DocsPaDocumentale_CDC.Documentale.FullTextSearchManager);
                //Giordano Iacozzilli  08/10/2012 Aggiunta strato SharePoint
                else if (documentale.Equals(TipiDocumentaliEnum.SharePoint.ToString().ToLower()))
                    _type = typeof(DocsPaDocumentale_CDC_SP.Documentale.FullTextSearchManager);
                //Fine
                //else if (documentale.Equals(TipiDocumentaliEnum.GFD.ToString().ToLower()))
                //    _type = typeof(DocsPaDocumentale_GFD.Documentale.TitolarioManager);
                //else if (documentale.Equals(TipiDocumentaliEnum.Hummingbird.ToString().ToLower()))
                //    _type = typeof(DocsPaDocumentale_HUMMINGBIRD.Documentale.TitolarioManager);
                //else if (documentale.Equals(TipiDocumentaliEnum.Filenet.ToString().ToLower()))
                //    _type = typeof(DocsPaDocumentale_FILENET.Documentale.TitolarioManager);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected FullTextSearchManager()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        public FullTextSearchManager(DocsPaVO.utente.InfoUtente infoUtente)
        {
            this._instance = (IFullTextSearchManager)Activator.CreateInstance(_type, infoUtente);
        }

        #endregion

        #region Protected Members

        /// <summary>
        /// 
        /// </summary>
        protected IFullTextSearchManager Instance
        {
            get
            {
                return this._instance;
            }
        }

        #endregion

        #region Public Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="testo"></param>
        /// <param name="idReg"></param>
        /// <param name="numPage"></param>
        /// <param name="numTotPage"></param>
        /// <param name="nRec"></param>
        /// <returns></returns>
        public ArrayList FullTextSearch(string testo, string idReg, int numPage, out int numTotPage, out int nRec)
        {
            return this.Instance.FullTextSearch(testo, idReg, numPage, out numTotPage, out nRec);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public ArrayList FullTextSearch(ref DocsPaVO.ricerche.FullTextSearchContext context)
        {
            return this.Instance.FullTextSearch(ref context);
        }

        /// <summary>
        /// Necessario per impostare il documento come indicizzato
        /// nel documentale ai fini della ricerca fulltext
        /// </summary>
        /// <param name="docnumber"></param>
        /// <returns></returns>
        public bool SetDocumentAsIndexed(string docnumber)
        {
            return this.Instance.SetDocumentAsIndexed(docnumber);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int GetMaxRowCount()
        {
            return this.Instance.GetMaxRowCount();
        }

        #region NewCode RicercaFullText

        /// <summary>
        /// Restituisce la lista dei version_id dei documenti trovati nella ricerca fulltext
        /// Metodo primitivo da raffinare
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public ArrayList simpleFullTextSearch(ref DocsPaVO.ricerche.FullTextSearchContext context)
        {
            return this.Instance.simpleFullTextSearch(ref context);
        }

        #endregion

        #endregion
    }
}