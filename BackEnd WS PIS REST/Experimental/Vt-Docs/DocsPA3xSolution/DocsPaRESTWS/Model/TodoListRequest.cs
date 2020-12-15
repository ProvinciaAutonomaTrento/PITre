using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocsPaRESTWS.Model
{
    public class TodoListRequest
    {
        public int RequestedPage { get; set; }
        public int PageSize { get; set; }
    }
}