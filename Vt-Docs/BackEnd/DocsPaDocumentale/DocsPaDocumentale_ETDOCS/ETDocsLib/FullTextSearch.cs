using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using DocsPaVO.ricerche;
using DocsPaVO.utente;
using DocsPaDB.Query_DocsPAWS;
using DocsPaDocumentale.FullTextSearch;

namespace DocsPaDocumentale_ETDOCS.ETDocsLib
{
    /// <summary>
    /// Classe per la gestione della ricerca fulltext per il documentale ETDOCS
    /// </summary>
    public class FullTextSearch
    {
        /// <summary>
        /// Dimensione pagina
        /// </summary>
        private const int PAGE_SIZE = 20;

        public FullTextSearch()
        {

        }

        /// <summary>
        /// Ricerca fulltext
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="context">Informazioni di contesto per la ricerca</param>
        /// <returns></returns>
        public ArrayList Search(InfoUtente infoUtente, ref FullTextSearchContext context)
        {
            string[] fullTextResult = null;

            if (context.SearchResultList != null && context.SearchResultList.Length > 0)
            {
                // Ricerca già effettuata, reperimento dall'oggetto di contesto
                // dei risultati precedenti evitando così una dispendiosa 
                // chiamata al sistema documentale
                fullTextResult = context.SearchResultList;
            }
            else
            {
                // Prima ricerca, chiamata al documentale
                fullTextResult = this.SearchETDOCS(infoUtente, context);

                // Reperimento identificativi dei documenti in docspa
                // che contengono uno o più file reperiti dalla ricerca fulltext
                fullTextResult = this.GetIDDocumentiETDOCS(fullTextResult, infoUtente);
            }

            // Estrazione dei soli id relativi alla pagina richiesta
            string[] documentsToRead = this.ExtractPageData(fullTextResult, context.RequestedPageNumber);

            // Calcolo numero pagine e record estratti
            int numDocs = fullTextResult.Length;
            int numTotPage = (numDocs / PAGE_SIZE);

            if (numTotPage * PAGE_SIZE < numDocs)
                numTotPage++;

            context.SearchResultList = fullTextResult;
            context.TotalPageNumber = numTotPage;
            context.TotalRecordCount = numDocs;

            // Reperimento dettagli sui documenti
            return this.GetDocuments(documentsToRead, infoUtente);
        }

        /// <summary>
        /// Ricerca fulltext per il documentale ETDOCS
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        protected virtual string[] SearchETDOCS(InfoUtente infoUtente, FullTextSearchContext context)
        {
            ArrayList files = new ArrayList();

            FullTextIndexingServices services = new FullTextIndexingServices();
            string catalog = !string.IsNullOrEmpty(DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_FULLTEXT_CATALOG")) ?
                DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_FULLTEXT_CATALOG") : "CATALOG_MIT";
            int maxResultRows = !string.IsNullOrEmpty(DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_FULLTEXT_MAXROWS")) ?
                int.Parse(DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_FULLTEXT_MAXROWS")) : 200;

            FullTextResultInfo[] fullTextResult = services.Search(context.TextToSearch, catalog, maxResultRows);
            //FullTextResultInfo[] fullTextResult = services.Search(context.TextToSearch, this.GetConfig(FULLTEXT_INDEX_CATALOG), this.MaxRows);

            foreach (DocsPaDocumentale.FullTextSearch.FullTextResultInfo item in fullTextResult)
                files.Add(item.Name);

            return (string[])files.ToArray(typeof(string));
        }

        /// <summary>
        /// Reperimento numero documenti in docspa che corrispondono alla ricerca fulltext
        /// </summary>
        /// <returns></returns>
        private string[] GetIDDocumentiETDOCS(string[] files, InfoUtente infoUtente)
        {
            RicercaFullText ricercaFT = new RicercaFullText();

            return ricercaFT.GetIDDocumentiETDOCS(files, infoUtente);
        }

        /// <summary>
        /// Reperimento documenti
        /// </summary>
        /// <param name="files"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        private ArrayList GetDocuments(string[] files, InfoUtente infoUtente)
        {
            RicercaFullText ricercaFT = new RicercaFullText();

            return ricercaFT.GetDocumentiETDOCS(files, infoUtente);
        }

        /// <summary>
        /// Estrazione dei soli file che devono essere inclusi nella pagina richiesta
        /// </summary>
        /// <param name="files"></param>
        /// <param name="requestedPage"></param>
        /// <returns></returns>
        private string[] ExtractPageData(string[] files, int requestedPage)
        {
            ArrayList retValue = new ArrayList();

            int startRow = (requestedPage * PAGE_SIZE) - PAGE_SIZE;
            for (int i = startRow; i < (startRow + PAGE_SIZE); i++)
            {
                if (files.Length <= i)
                    break;

                retValue.Add(files[i]);
            }

            return (string[])retValue.ToArray(typeof(string));
        }

        #region NewCode RicercaFullText

        /// <summary>
        /// Ricerca semplice fulltext
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="context">Informazioni di contesto per la ricerca</param>
        /// <returns></returns>
        public ArrayList simpleSearch(InfoUtente infoUtente, ref FullTextSearchContext context)
        {
            ArrayList result = new ArrayList();

            FullTextIndexingServices services = new FullTextIndexingServices();
            string catalog = !string.IsNullOrEmpty(DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_FULLTEXT_CATALOG")) ?
                DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_FULLTEXT_CATALOG") : "CATALOG_MIT";
            int maxResultRows = !string.IsNullOrEmpty(DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_FULLTEXT_MAXROWS")) ?
                int.Parse(DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_FULLTEXT_MAXROWS")) : 200;

            FullTextResultInfo[] fullTextResult = services.Search(context.TextToSearch, catalog, maxResultRows);
            //FullTextResultInfo[] fullTextResult = services.Search(context.TextToSearch, this.GetConfig(FULLTEXT_INDEX_CATALOG), this.MaxRows);

            foreach (DocsPaDocumentale.FullTextSearch.FullTextResultInfo item in fullTextResult)
                result.Add(item.Name);

            return result;
        }

        #endregion

        #region Gestione configurazione

        /// <summary>
        /// Catalogo per la ricerca FullText
        /// </summary>
        private const string FULLTEXT_INDEX_CATALOG = "FULLTEXT_INDEX_CATALOG";
        /// <summary>
        /// Numero massimo di righe da estrarre
        /// </summary>
        private const string FULL_TEXT_MAX_ROWS = "FULL_TEXT_MAX_ROWS";

        /// <summary>
        /// Valore predefinito per il numero massimo di righe da estrarre
        /// </summary>
        private const int DEFAULT_FULL_TEXT_MAX_ROWS = 200;

        /// <summary>
        /// Reperimento valore configurazione
        /// </summary>
        /// <param name="configName"></param>
        /// <returns></returns>
        protected string GetConfig(string configName)
        {
            string configValue = ConfigurationManager.AppSettings[configName];

            if (configValue == null)
                configValue = string.Empty;

            return configValue;
        }

        /// <summary>
        /// Reperimento limite massimo di record che è possibile estrarre dal documentale
        /// </summary>
        /// <returns></returns>
        protected virtual int MaxRows
        {
            get
            {
                int retValue = DEFAULT_FULL_TEXT_MAX_ROWS;

                string configValue = System.Configuration.ConfigurationManager.AppSettings[FULL_TEXT_MAX_ROWS];

                if (configValue != null && configValue != string.Empty)
                {
                    try
                    {
                        retValue = Convert.ToInt32(configValue);
                    }
                    catch
                    {
                    }
                }

                return retValue;
            }
        }

        #endregion
    }
}