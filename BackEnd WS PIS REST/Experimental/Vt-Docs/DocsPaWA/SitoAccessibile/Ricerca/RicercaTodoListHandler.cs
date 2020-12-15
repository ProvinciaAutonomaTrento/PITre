using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocsPAWA.SitoAccessibile.Ricerca
{
    /// <summary>
    /// 
    /// </summary>
    public class RicercaTodoListHandler
    {
        /// <summary>
        /// 
        /// </summary>
        public RicercaTodoListHandler()
        { }

        /// <summary>
        /// Reperimento degli elementi nella todolist personale 
        /// </summary>
        /// <param name="pagingContext"></param>
        /// <param name="filters"></param>
        /// <returns></returns>
        public DocsPAWA.DocsPaWR.infoToDoList[] GetMyTodoList(Paging.PagingContext pagingContext, DocsPaWR.FiltroRicerca[] filters)
        {
            int totalRecordCount;
            int totalTrasmNonViste;

            string listRegistri = "0";

            foreach (DocsPAWA.DocsPaWR.Registro registro in UserManager.getRuolo().registri)
                listRegistri += "," + registro.systemId;

            DocsPAWA.DocsPaWR.infoToDoList[] retValue = ProxyManager.getWS().getMyNewTodoList(
                            UserManager.getInfoUtente(),
                            listRegistri,
                            filters,
                            pagingContext.PageNumber,
                            pagingContext.PageSize,
                            out totalRecordCount,
                            out totalTrasmNonViste);

            pagingContext.RecordCount = totalRecordCount;

            pagingContext.PageCount = (totalRecordCount / pagingContext.PageSize);
            if ((totalRecordCount % pagingContext.PageSize) > 0)
                pagingContext.PageCount++;

            return retValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoTodoListItem"></param>
        /// <returns></returns>
        public DocsPAWA.DocsPaWR.Trasmissione GetTrasmissione(DocsPaWR.infoToDoList infoTodoListItem)
        {
            DocsPAWA.DocsPaWR.Trasmissione[] list = ProxyManager.getWS().trasmGetDettaglioTrasmissione(UserManager.getUtente(), UserManager.getRuolo(), infoTodoListItem);

            if (list != null && list.Length > 0)
                return list[0];
            else
                return null;
        }
    }
}