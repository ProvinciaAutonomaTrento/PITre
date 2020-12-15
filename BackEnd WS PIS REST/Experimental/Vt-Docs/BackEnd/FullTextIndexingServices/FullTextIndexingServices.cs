using System;
using System.Data;
using System.Data.OleDb;
using System.Collections;
using System.IO;
using System.Configuration;
using System.Diagnostics;


namespace DocsPaDocumentale.FullTextSearch
{
	/// <summary>
	/// Servizio per la ricerca fulltext mediante
	/// il servizio "MicrosoftIndexService"
	/// </summary>
	public class FullTextIndexingServices
	{
		#region Gestione configurazioni

        /// <summary>
        /// Tipo del motore di fill text search  /valori search o index (default su search)
        /// </summary>
        private const string FULLTEXT_ENGINE_TYPE = "FULLTEXT_ENGINE_TYPE";

        /// Url del servizio web utilizzato per la ricerca fulltext
        /// </summary>
        private const string FULLTEXT_INDEXING_WS = "FULLTEXT_INDEXING_WS";

        /// <summary>
        /// Catalogo per la ricerca FullText
        /// </summary>
        private const string FULLTEXT_INDEX_CATALOG = "FULLTEXT_INDEX_CATALOG";
        /// <summary>
        /// Numero massimo di righe da estrarre
        /// </summary>
        private const string FULL_TEXT_MAX_ROWS = "FULL_TEXT_MAX_ROWS";

        /// <summary>
        /// path del log
        /// </summary>
        private const string PATH_LOG = "DEBUG_PATH";
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
			string configValue=ConfigurationManager.AppSettings[configName];

			if (configValue==null)
				configValue=string.Empty;

			return configValue;
		}


        string Engine
        {
            get
            {
                string type = GetConfig("FULLTEXT_ENGINE_TYPE");
                if (String.IsNullOrEmpty(type))
                    return "";

                return type.ToLower();
            }
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

                string configValue = this.GetConfig(FULL_TEXT_MAX_ROWS);

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

        /// <summary>
        /// Verifica se il servizio di indicizzazione deve
        /// essere eseguito in modalità remota
        /// </summary>
        /// <param name="fullTextWS"></param>
        /// <returns></returns>
        protected bool IsRemoteFullTextEnabled(out string fullTextWS)
        {
            fullTextWS = this.GetConfig(FULLTEXT_INDEXING_WS);

            //fullTextWS = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_FULLTEXT_MAXROWS");
            return (fullTextWS != string.Empty);
        }


		#endregion

		public FullTextIndexingServices()
		{	
		}

		#region Gestione ricerca FullText

		/// <summary>
		/// Ricerca FullText
		/// </summary>
		/// <param name="textQuery">Testo da ricercare</param>
		/// <returns></returns>
		public FullTextResultInfo[] Search(string textToSearch)
		{


			return this.Search(textToSearch,this.GetConfig(FULLTEXT_INDEX_CATALOG));
		}

		/// <summary>
		/// Ricerca FullText
		/// </summary>
		/// <param name="textQuery">Testo da ricercare</param>
		/// <returns></returns>
		public FullTextResultInfo[] Search(string textToSearch,int maxRowCount)
		{
			return this.Search(textToSearch,this.GetConfig(FULLTEXT_INDEX_CATALOG),maxRowCount);
		}

		/// <summary>
		/// Ricerca FullText
		/// </summary>
		/// <param name="textQuery">Testo da ricercare</param>
		/// <param name="indexCatalogName">Nome del catalogo utilizzato per la ricerca</param>
		/// <returns></returns>
		public FullTextResultInfo[] Search(string textToSearch,string indexCatalogName)
		{
			return this.Search(textToSearch,indexCatalogName,this.MaxRows);
		}

		/// <summary>
		/// Ricerca FullText
		/// </summary>
		/// <param name="textQuery">Testo da ricercare</param>
		/// <param name="indexCatalogName">Nome del catalogo utilizzato per la ricerca</param>
		/// <param name="maxRowCount">Numero massimo di righe da estrarre dalla ricerca</param>
		/// <returns></returns>
		public FullTextResultInfo[] Search(string textToSearch,string indexCatalogName,int maxRowCount)
		{
			if (indexCatalogName==null || indexCatalogName==string.Empty)
				indexCatalogName=this.GetConfig(FULLTEXT_INDEX_CATALOG);

			if (maxRowCount<=0)
				maxRowCount=this.MaxRows;

			FullTextResultInfo[] retValue=null;

			string fullTextWS;
			
			if (this.IsRemoteFullTextEnabled(out fullTextWS))
			{
				// Ricerca FullText da contesto remoto
				FullTextIndexingWS.FullTextWS ws=new FullTextIndexingWS.FullTextWS();
				ws.Url=fullTextWS;
				FullTextIndexingWS.FullTextResultInfo[] result=ws.Search(textToSearch,indexCatalogName,maxRowCount);

				ArrayList list=new ArrayList();

				foreach (FullTextIndexingWS.FullTextResultInfo item in result)
				{
					FullTextResultInfo info=new FullTextResultInfo();

					info.Characterization=item.Characterization;
					info.DocTitle=item.DocTitle;
					info.FileName=item.FileName;
					info.Name=item.Name;
					info.Rank=item.Rank;
					info.VPath=item.VPath;
					info.Write=item.Write;

					list.Add(info);
				}
				
				retValue=(FullTextResultInfo[]) list.ToArray(typeof(FullTextResultInfo));
			}
			else
			{
				// Ricerca FullText da contesto locale
                string engine = Engine;
                if (engine=="index")
                {
                    IndexServer isvr = new IndexServer();
                    retValue = isvr.FullTextSearch(textToSearch, indexCatalogName, maxRowCount);
                }
                else if (engine == "search")
                {
                    SearchServer isvr = new SearchServer();
                    
                    retValue = isvr.FullTextSearch(textToSearch, indexCatalogName, maxRowCount);

                }
                else if (engine == "solr")
                {
                    SolrServer isrv = new SolrServer();
                    retValue = isrv.FullTextSearch(textToSearch, indexCatalogName, maxRowCount);
                }
				
			}

			return retValue;
		}

		#endregion
	}
}