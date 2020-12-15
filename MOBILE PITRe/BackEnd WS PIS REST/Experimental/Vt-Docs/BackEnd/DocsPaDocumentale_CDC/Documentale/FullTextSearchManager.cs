using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using DocsPaVO.ricerche;
using DocsPaVO.utente;
using DocsPaDocumentale.Interfaces;
using DocsPaUtils.LogsManagement;

namespace DocsPaDocumentale_CDC.Documentale
{
    /// <summary>
    /// Gestione della ricerca fulltext
    /// </summary>
    public class FullTextSearchManager : IFullTextSearchManager
    {
        /// <summary>
        /// 
        /// </summary>
        private InfoUtente _infoUtente = null;

        /// <summary>
        /// 
        /// </summary>
        private IFullTextSearchManager _instanceCDC = null;

        /// <summary>
        /// 
        /// </summary>
        protected FullTextSearchManager()
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        public FullTextSearchManager(InfoUtente infoUtente)
        {
            this._infoUtente = infoUtente;
        }

        #region Public methods

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
            return FullTextSearch(testo, idReg, numPage, out numTotPage, out nRec);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public ArrayList FullTextSearch(ref FullTextSearchContext context)
        {
            return this.InstanceCDC.FullTextSearch(ref context);
        }

        /// <summary>
        /// Necessario per impostare il documento come indicizzato
        /// nel documentale ai fini della ricerca fulltext
        /// </summary>
        /// <param name="docnumber"></param>
        /// <returns></returns>
        public bool SetDocumentAsIndexed(string docnumber)
        {
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int GetMaxRowCount()
        {
            return 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public ArrayList simpleFullTextSearch(ref FullTextSearchContext context)
        {
            return new ArrayList();
        }

        #endregion

        #region Protected methods

        /// <summary>
        /// Reperimento istanza gestore fulltext per documentum
        /// </summary>
        protected IFullTextSearchManager InstanceCDC
        {
            get
            {
                if (this._instanceCDC == null)
                    this._instanceCDC = new DocsPaDocumentale_OCS.Documentale.FullTextSearchManager(this.InfoUtente);
                return this._instanceCDC;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected InfoUtente InfoUtente
        {
            get
            {
                return this._infoUtente;
            }
        }

        #endregion
    }
}
