using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VTDocs.mobile.fe.wsrefs.VTDocsWSMobile;
using VTDocs.mobile.fe.datagrid;

namespace VTDocs.mobile.fe.commands
{
    public class ToDoListGridCommand : DataGridCommand<ToDoListElement>
    {

        public override DataGridResult<ToDoListElement> execute(DataGridInput input)
        {
            ToDoListRequest request = new ToDoListRequest();
            request.UserInfo = NavigationHandler.UserInfo;
            request.PageSize = input.NumResForPage;
            request.RequestedPage = input.NumPage;
            request.IdGruppo = NavigationHandler.RuoloInfo.IdGruppo;
            request.Registri = NavigationHandler.Registri;
            if (!string.IsNullOrEmpty(input.IdParent))
            {
                request.ParentFolderId = input.IdParent;
            }
            ToDoListResponse resp = WSStub.getTodoList(request);
            DataGridResult<ToDoListElement> res = new DataGridResult<ToDoListElement>();
            res.NumTotResults = resp.TotalRecordCount;
            res.Elements = new List<ToDoListElement>();
            foreach (ToDoListElement temp in resp.Elements) res.Elements.Add(temp);
            return res;
        }

    }
}