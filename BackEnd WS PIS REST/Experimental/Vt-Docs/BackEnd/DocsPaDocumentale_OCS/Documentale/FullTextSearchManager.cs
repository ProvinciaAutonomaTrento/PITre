using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Configuration;
using DocsPaVO.ricerche;
using DocsPaVO.utente;
using DocsPaDocumentale.Interfaces;
using DocsPaUtils.LogsManagement;
using DocsPaDocumentale_OCS.CorteContentServices;
using DocsPaDocumentale_OCS.OCSServices;

namespace DocsPaDocumentale_OCS.Documentale
{
    /// <summary>
    /// Gestione della ricerca fulltext
    /// </summary>
    public class FullTextSearchManager : IFullTextSearchManager
    {
        #region Ctors, constants, variables

        /// <summary>
        /// Dimensione pagina
        /// </summary>
        private const int PAGE_SIZE = 20;

        /// <summary>
        /// 
        /// </summary>
        private const string FULL_TEXT_MAX_ROWS = "FULL_TEXT_MAX_ROWS";

        /// <summary>
        /// 
        /// </summary>
        private InfoUtente _infoUtente = null;

        /// <summary>
        /// 
        /// </summary>
        private CorteContentServices.DocumentManagementSOAPHTTPBinding _wsDocumentInstance = null;

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

        #endregion

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
            FullTextSearchContext context = new FullTextSearchContext();
            context.TextToSearch = testo;
            context.RequestedPageNumber = numPage;

            ArrayList list = this.FullTextSearch(ref context);

            numTotPage = context.TotalPageNumber;
            nRec = context.TotalRecordCount;

            
            return list;
        }

        /// <summary>
        /// Ricerca fulltext nell'oggetto document
        /// 
        /// nb: da fare anche ricerca allegati
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public ArrayList FullTextSearch(ref FullTextSearchContext context)
        {
            ArrayList result = new ArrayList();

            try
            {
                List<string> fullTextResult = null;

                if (context.SearchResultList != null && context.SearchResultList.Length > 0)
                {
                    // Ricerca già effettuata, reperimento dall'oggetto di contesto
                    // dei risultati precedenti evitando così una dispendiosa 
                    // chiamata al sistema documentale
                    fullTextResult = new List<string>(context.SearchResultList);
                }
                else
                {
                    //esegue la ricerca su OCS 
                    CorteContentServices.FilterSearchRequestType searchDocumentRequest = new CorteContentServices.FilterSearchRequestType();
                    //costruisco la stringa di ricerca

                    //inserire i filtri di ricerca
                    //1. il path da cui cercare
                    //2. l'ordinamento
                    //3. solo i documenti del protocollo
                    //4. ...
                    searchDocumentRequest.userCredentials = OCSServices.OCSUtils.getUserCredentials(_infoUtente);
                    searchDocumentRequest.filter = new CorteContentServices.FilterSearchType();
                    searchDocumentRequest.filter.folderPath = OCSConfigurations.GetDocRootFolder();
                    searchDocumentRequest.filter.includeSubFolders = true;
                    searchDocumentRequest.filter.includeSubFoldersSpecified = true;

                    searchDocumentRequest.filter.sortClause = "CREATE_DATE DESC";
                    // Query execution
                    int startIndex = (context.RequestedPageNumber * PAGE_SIZE) - PAGE_SIZE;
                    int maxResults = this.GetMaxRowCount();
                    searchDocumentRequest.filter.count = maxResults;
                    searchDocumentRequest.filter.countSpecified = true;
                    //l'indice in OCS parte da 1 e non da 0
                    searchDocumentRequest.filter.offset = startIndex + 1;
                    searchDocumentRequest.filter.offsetSpecified = true;
                    searchDocumentRequest.filter.searchExpress = new DocsPaDocumentale_OCS.CorteContentServices.searchExpress(); //TODO: da fare!!!
                    searchDocumentRequest.filter.searchExpress.SearchExprType = new DocsPaDocumentale_OCS.CorteContentServices.SearchExprType();
                    string operatCr1 = "CONTAINS";
                    string rightOpCr1 = context.TextToSearch;

                    searchDocumentRequest.filter.searchExpress.SearchExprType.@operator = operatCr1;
                    searchDocumentRequest.filter.searchExpress.SearchExprType.rightOperand = rightOpCr1;
                    
                    //SABRINA: provo ad eliminare la catgoria in modo da ottenere tutti i documenti (anche allegati e stampe registro)

                    bool profilezioneDinamica = DocsPaDB.Query_DocsPAWS.Documenti.isEnabledProfilazioneAllegati;
                    if (!profilezioneDinamica)
                    {
                        searchDocumentRequest.filter.searchExpress.SearchExprType.sameLevelOperator = "AND";

                        searchDocumentRequest.filter.searchExpress.SearchExprType.SearchExprType1 = new DocsPaDocumentale_OCS.CorteContentServices.SearchExprType();
                        string operatCr2 = "HAS_CATEGORY";
                        string rightOpCr2 = "[" + DocsPaObjectType.ObjectTypes.CATEGOTY_PROTOCOLLO + "]";

                        searchDocumentRequest.filter.searchExpress.SearchExprType.SearchExprType1.@operator = operatCr2;
                        searchDocumentRequest.filter.searchExpress.SearchExprType.SearchExprType1.rightOperand = rightOpCr2;
                    }
                    CorteContentServices.ItemSearchListResponseType listResult;
                    listResult = this.WsDocumentInstance.SearchDocuments(searchDocumentRequest);

                    OCSUtils.throwExceptionIfInvalidResult(listResult.result);

                    fullTextResult = new List<string>();

                    foreach (CorteContentServices.ItemSearchType item in listResult.items)
                    {
                        // Reperimento docnumber
                        
                        //NB: momentaneamente ricavo il docNumber dall'oggetto info
                        string docNumber = item.info.name.Substring(0, item.info.name.IndexOf("."));

                        if (!fullTextResult.Contains(docNumber)) // Eliminazione dei risultati duplicati
                            fullTextResult.Add(docNumber);
                    }
                }

                // Paginazione dei risultati
                if (fullTextResult != null && fullTextResult.Count > 0)
                {
                    int startIndex = (context.RequestedPageNumber * PAGE_SIZE) - PAGE_SIZE;
                    int count = PAGE_SIZE;
                    if ((fullTextResult.Count - startIndex) < count)
                        count = (fullTextResult.Count - startIndex);

                    result = this.GetDocuments(fullTextResult.GetRange(startIndex, count).ToArray(), InfoUtente);
                    
                    int pageCount = (fullTextResult.Count / PAGE_SIZE);                    
                    if ((fullTextResult.Count % PAGE_SIZE) > 0) pageCount++;

                    context.SearchResultList = fullTextResult.ToArray();
                    context.TotalPageNumber = pageCount;
                    context.TotalRecordCount = fullTextResult.Count;
                }
            }
            catch (Exception ex)
            {
                result.Clear();
            }

            return result;
        }

        /// <summary>
        /// Reperimento documenti
        /// </summary>
        /// <param name="keys"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        protected virtual ArrayList GetDocuments(string[] keys, InfoUtente infoUtente)
        {
            DocsPaDB.Query_DocsPAWS.RicercaFullText ricercaFT = new DocsPaDB.Query_DocsPAWS.RicercaFullText();
            return ricercaFT.GetDocumentiETDOCS(keys, infoUtente);
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
        /// Reperimento del numero massimo di righe che è possibile restituire dalla ricerca
        /// </summary>
        /// <returns></returns>
        public int GetMaxRowCount()
        {
            int retValue = 0;

            string configValue = ConfigurationManager.AppSettings[FULL_TEXT_MAX_ROWS];

            if (!string.IsNullOrEmpty(configValue))
                Int32.TryParse(configValue, out retValue);

            return retValue;
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

        /// <summary>
        /// Reperimento istanza webservices per la gestione dei documenti
        /// </summary>
        protected DocumentManagementSOAPHTTPBinding WsDocumentInstance
        {
            get
            {
                if (this._wsDocumentInstance == null)
                    this._wsDocumentInstance = OCSServiceFactory.GetDocumentServiceInstance<DocumentManagementSOAPHTTPBinding>();
                return this._wsDocumentInstance;
            }
        }

        //il metodo ora commentato serve per ricavare il docnumber dei docuemnti in base alle informazioni sulla categoria degli stessi.
        //poichè il metodo OCS non funzionava è stato trovato un sistema più semplice che utilizza il nome del file eliminando il punto e l'estensione

        //public static string getCategoryMetadata(CorteContentServices.CategoryType[] listCategory, string categoryName, string metadataName)
        //{
        //    //TODO: da aggiustare per considerare i campi multivalore
        //    string stringValue = null;
        //    try
        //    {
        //        CorteContentServices.CategoryType category = null;
        //        bool trovato = false;
        //        for (int i = 0; (i < listCategory.Length || trovato); i++ )
        //        {
        //            if (listCategory[i].name.Equals(categoryName))
        //            {
        //                category = listCategory[i];
        //                trovato = true;
        //            }
        //        }
        //        if (trovato && category != null)
        //        {
        //            bool isPresent = false;
        //            for (int j = 0; (j < listCategory.Length || isPresent); j++)
        //            {
        //                if (category.metadataList[j].name.Equals(metadataName))
        //                {
        //                    stringValue = category.metadataList[j].value[0];
        //                    isPresent = true;
        //                }
        //            }
        //        }
        //        return stringValue;

        //    }
        //    catch (Exception ex)
        //    {
        //        stringValue = null;
        //        Debugger.Write(string.Format("Errore in OCS.getMetadataValue:\n{0}", ex.ToString()));
        //    }
        //    return stringValue;
        //}

        #endregion
    }
}
