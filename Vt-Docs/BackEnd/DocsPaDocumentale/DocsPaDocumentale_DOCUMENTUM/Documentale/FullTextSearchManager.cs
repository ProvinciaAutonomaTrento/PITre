using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Configuration;
using DocsPaVO.ricerche;
using DocsPaVO.utente;
using DocsPaDocumentale.Interfaces;
using DocsPaDocumentale_DOCUMENTUM.DocsPaServices;
using DocsPaDocumentale_DOCUMENTUM.DocsPaObjectTypes;
using DocsPaDocumentale_DOCUMENTUM.DctmServices;
using Emc.Documentum.FS.DataModel.Core;
using Emc.Documentum.FS.Services.Core;
using Emc.Documentum.FS.Services.Search;
using Emc.Documentum.FS.DataModel.Core.Properties;
using Emc.Documentum.FS.DataModel.Core.Profiles;
using Emc.Documentum.FS.DataModel.Core.Content;
using Emc.Documentum.FS.DataModel.Core.Query;
using log4net;

namespace DocsPaDocumentale_DOCUMENTUM.Documentale
{
    /// <summary>
    /// Gestione della ricerca fulltext
    /// </summary>
    public class FullTextSearchManager : IFullTextSearchManager
    {
        private ILog logger = LogManager.GetLogger(typeof(FullTextSearchManager));
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
                    StructuredQuery strQuery = new StructuredQuery();

                    strQuery.AddRepository(DctmConfigurations.GetRepositoryName());
                    strQuery.ObjectType = ObjectTypes.DOCUMENTO;
                    
                    strQuery.IsDatabaseSearch = false;
                    strQuery.IsIncludeAllVersions = false;
                    strQuery.IsIncludeHidden = false;
                     
                    // Inserisce nella ricerca il solo cabinet dell'amministrazione
                    RepositoryScope repositoryScope = new RepositoryScope();
                    repositoryScope.RepositoryName = DctmConfigurations.GetRepositoryName();
                    repositoryScope.LocationPath = DocsPaAdminCabinet.getRootAmministrazione(this.InfoUtente);
                    repositoryScope.IsDescend = true;
                    strQuery.Scopes.Add(repositoryScope);

                    ExpressionSet set = new ExpressionSet();
                    set.AddExpression(new FullTextExpression(context.TextToSearch));
                    strQuery.RootExpressionSet = set;

                    // Query execution
                    int startIndex = (context.RequestedPageNumber * PAGE_SIZE) - PAGE_SIZE;
                    int maxResults = this.GetMaxRowCount();
                    QueryExecution queryExec = new QueryExecution(startIndex, maxResults, maxResults);
                    
                    ISearchService searchService = DctmServiceFactory.GetServiceInstance<ISearchService>(this.InfoUtente.dst);
                    QueryResult queryResult = searchService.Execute(strQuery, queryExec, null);
                    
                    QueryStatus queryStatus = queryResult.QueryStatus;
                    RepositoryStatusInfo repStatusInfo = queryResult.QueryStatus.RepositoryStatusInfos[0];

                    if (repStatusInfo.Status == Status.FAILURE)
                        throw new ApplicationException("QueryResult: Status.FAILURE");

                    fullTextResult = new List<string>();

                    foreach (DataObject dataObject in queryResult.DataObjects)
                    {
                        // Reperimento docnumber
                        string docNumber = dataObject.Properties.Get(TypeDocumento.DOC_NUMBER).GetValueAsString();

                        if (!fullTextResult.Contains(docNumber)) // Eliminazione dei risultati duplicati
                            fullTextResult.Add(docNumber);
                    }

                    context.SearchResultList = fullTextResult.ToArray();
                    context.TotalPageNumber = (fullTextResult.Count / PAGE_SIZE);
                    context.TotalRecordCount = fullTextResult.Count;
                }
                
                // Paginazione dei risultati
                if (fullTextResult != null && fullTextResult.Count > 0)
                {
                    int startIndex = (context.RequestedPageNumber * PAGE_SIZE) - PAGE_SIZE;
                    int count = PAGE_SIZE;
                    if (fullTextResult.Count < count)
                        count = fullTextResult.Count;
                    List<string> pageContent = fullTextResult.GetRange(startIndex, count);
                    result = this.GetDocuments(pageContent.ToArray(), InfoUtente);
                }
            }
            catch (Exception ex)
            {
                result.Clear();
                logger.Debug(string.Format("Errore in Documentum.FullTextSearch:\n{0}", ex.ToString()));
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

        #endregion
    }
}
