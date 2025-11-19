// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaUtils;
using DocsPaVO.ricerche;
using Serilog;
using System;
using System.Collections.Generic;

namespace DocsPaDB.Utils;

public class PagingQuery : DBProvider
{
    private ILogger logger = Serilog.Log.ForContext(typeof(PagingQuery));
    private Query _query;

    public PagingQuery(string countQueryName, string queryName, SearchPagingContext pagingContext, Dictionary<string, string> paramList,string orderField,bool desc)
    {
        Query countQuery = DocsPaUtils.InitQuery.getInstance().getQuery(countQueryName);
        addParams(countQuery, paramList);
        string countCommandText = countQuery.getSQL();
        logger.Debug("COUNT QUERY: " + countCommandText);
        string field = "";
        int retValue = 0;
        if (databaseProvider.ExecuteScalar(out field, countCommandText))
        {
            Int32.TryParse(field, out retValue);
            pagingContext.RecordCount = retValue;
        }
        int pageSize = pagingContext.PageSize;
        _query = DocsPaUtils.InitQuery.getInstance().getQuery(queryName);
        addParams(_query, paramList);
        // per query sqlserver:
        // il numero totale di righe da estrarre equivale 
        // al limite inferiore dell'ultima riga da estrarre
        int totalRowsSqlServer = (pagingContext.PageSize * pagingContext.Page);
        if ((retValue - totalRowsSqlServer) <= 0)
        {
            pageSize -= System.Math.Abs(retValue - totalRowsSqlServer);
        }
        _query.setParam("pageSize", "" + pageSize);
        _query.setParam("rowNum", "" + pagingContext.PageSize * pagingContext.Page);
        if(!string.IsNullOrEmpty(orderField)){
            if(desc){
                _query.setParam("order_desc",orderField);
                _query.setParam("order",orderField+" DESC");
            }else{
                _query.setParam("order",orderField);
                _query.setParam("order_desc", orderField + " DESC");
            }
        }
        //parametri specifici per ORACLE
        _query.setParam("startRow", ""+pagingContext.StartRow);
        _query.setParam("endRow",""+pagingContext.EndRow);
    }

    public PagingQuery(string countQueryName, string queryName, SearchPagingContext pagingContext, Dictionary<string, string> paramList)
        : this(countQueryName, queryName, pagingContext, paramList, null, false)
    {

    }

    public Query Query
    {
        get
        {
            return _query;
        }
    }

    private void addParams(Query query, Dictionary<string, string> paramList)
    {
        foreach (string key in paramList.Keys) query.setParam(key, paramList[key]);
    }

}
