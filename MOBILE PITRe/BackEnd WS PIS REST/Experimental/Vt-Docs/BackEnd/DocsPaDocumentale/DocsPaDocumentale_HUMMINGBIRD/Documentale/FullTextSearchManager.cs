using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using DocsPaVO.ricerche;
using DocsPaVO.utente;
using DocsPaDocumentale_HUMMINGBIRD.HummingbirdLib;
using DocsPaDocumentale.Interfaces;

namespace DocsPaDocumentale_HUMMINGBIRD.Documentale
{
    /// <summary>
    /// Gestione della ricerca fulltext
    /// </summary>
    public class FullTextSearchManager : IFullTextSearchManager
    {
        private InfoUtente _infoUtente = null;

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
            RicercaFullText ricercaFullText = new RicercaFullText();
            return ricercaFullText.RicercaFT(testo, idReg, this.InfoUtente, numPage, out numTotPage, out nRec);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public ArrayList FullTextSearch(ref FullTextSearchContext context)
        {
            RicercaFullText ricercaFullText = new RicercaFullText();
            return ricercaFullText.RicercaFT(this.InfoUtente, ref context);
        }

        /// <summary>
        /// Necessario per impostare il documento come indicizzato
        /// nel documentale ai fini della ricerca fulltext
        /// </summary>
        /// <param name="docnumber"></param>
        /// <returns></returns>
        public bool SetDocumentAsIndexed(string docnumber)
        {
            // Impostazione valore nel campo FullText in tabella profile
            HummingbirdLib.RicercaFullText ricercaFullText = new HummingbirdLib.RicercaFullText();
            return ricercaFullText.SetProfileFT(docnumber, this.InfoUtente);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        int IFullTextSearchManager.GetMaxRowCount()
        {
            RicercaFullText ricercaFullText = new RicercaFullText();
            return ricercaFullText.GetMaxRows();
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
