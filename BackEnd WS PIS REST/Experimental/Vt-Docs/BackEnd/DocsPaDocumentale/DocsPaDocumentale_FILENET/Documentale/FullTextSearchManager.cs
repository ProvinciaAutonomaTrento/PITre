using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using DocsPaVO.ricerche;
using DocsPaVO.utente;
using DocsPaDocumentale_FILENET.FilenetLib;
using DocsPaDocumentale.Interfaces;
using log4net;

namespace DocsPaDocumentale_FILENET.Documentale
{
    /// <summary>
    /// Gestione della ricerca fulltext
    /// </summary>
    public class FullTextSearchManager : IFullTextSearchManager
    {
        private ILog logger = LogManager.GetLogger(typeof(FullTextSearchManager));
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
            numTotPage = 0;
            nRec = 0;
            string dst = this.InfoUtente.dst;
            ArrayList listaDoc = null;

            try
            {
                listaDoc = FilenetLib.DocumentManagement.RicercaFT(testo, idReg, this.InfoUtente, numPage, dst, out numTotPage, out nRec);
                if (listaDoc == null)
                    listaDoc = new ArrayList(1);
                return listaDoc;
            }
            catch (Exception ex)
            {
                IDMError.ErrorManager idmErrorManager = new IDMError.ErrorManager();
                logger.Debug(ex.Message);
                string msg = ex.Message;
                for (int i = 0; i < idmErrorManager.Errors.Count; i++)
                    msg += " " + idmErrorManager.Errors[i].Description;
                logger.Debug(msg);
                logger.Debug("Errore durante la ricerca Full Text - Filenet.", ex);
                throw ex;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public ArrayList FullTextSearch(ref FullTextSearchContext context)
        {
            int totalPageNumber, totalRecordCount;

            ArrayList retValue = this.FullTextSearch(context.TextToSearch,
                                                    string.Empty,
                                                    context.RequestedPageNumber,
                                                    out totalPageNumber,
                                                    out totalRecordCount);

            context.TotalPageNumber = totalPageNumber;
            context.TotalRecordCount = totalRecordCount;

            return retValue;
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
        int IFullTextSearchManager.GetMaxRowCount()
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
