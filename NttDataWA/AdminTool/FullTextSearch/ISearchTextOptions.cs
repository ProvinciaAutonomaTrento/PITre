using System;
using System.Collections.Generic;
using System.Text;
using SAAdminTool.DocsPaWR;

namespace SAAdminTool.FullTextSearch
{
    /// <summary>
    /// 
    /// </summary>
    public interface ISearchTextOptions
    {
        /// <summary>
        /// Reperimento modalità di ricerca testuale
        /// </summary>
        /// <returns></returns>
        SearchTextOptionsEnum GetSearchTextOptions();

        /// <summary>
        /// Impostazione modalità di reperimento testuale
        /// </summary>
        /// <param name="searchOptions"></param>
        void SetSearchTextOptions(SearchTextOptionsEnum searchOptions);
    }
}
