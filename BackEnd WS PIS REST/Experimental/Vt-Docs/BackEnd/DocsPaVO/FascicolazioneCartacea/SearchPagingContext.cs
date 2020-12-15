//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Xml.Serialization;

//namespace DocsPaVO.FascicolazioneCartacea
//{
//    /// <summary>
//    /// Classe utilizzata per impostare e 
//    /// reperire informazioni sulla ricerca fulltext
//    /// </summary>
//    [XmlType("FascCartaceaSearchPagingContext")]
//    public class SearchPagingContext
//    {
//        /// <summary>
//        /// 
//        /// </summary>
//        public SearchPagingContext()
//        {
//            this.SearchResultList = new List<int>();
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <returns></returns>
//        public string GetPageFilter()
//        {
//            string pageFilter = string.Empty;

//            if (this.SearchResultList != null)
//            {
//                int startRow = ((this.RequestedPageNumber * this.PageSize) - this.PageSize);
//                int endRow = (this.RequestedPageNumber * this.PageSize) - 1;

//                for (int i = startRow; i < endRow && i < this.SearchResultList.Count; i++)
//                {
//                    if (pageFilter != string.Empty)
//                        pageFilter += ", ";
//                    pageFilter += this.SearchResultList[i].ToString();
//                }
//            }

//            return pageFilter;
//        }

//        /// <summary>
//        /// Array di stringhe contenente tutti gli id estratti
//        /// </summary>
//        public List<int> SearchResultList = null;

//        /// <summary>
//        /// Gestione paginazione, indice della pagina richiesta
//        /// </summary>
//        public int RequestedPageNumber = 0;

//        /// <summary>
//        /// 
//        /// </summary>
//        public int PageSize = 0;

//        /// <summary>
//        /// Gestione paginazione, numero totale delle pagine
//        /// </summary>
//        public int TotalPageNumber = 0;

//        /// <summary>
//        /// Numero totale di record restituiti, indipendentemente dalla paginazione
//        /// </summary>
//        public int TotalRecordCount = 0;
//    }
//}
