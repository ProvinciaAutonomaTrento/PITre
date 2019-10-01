using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocsPaVO.filtri;

namespace DocsPaVO.Mobile.Requests
{
    public class ToDoListRequest
    {
        public UserInfo UserInfo
        {
            get; 
            set;
        }

        public string IdGruppo
        {
            get; 
            set; 
        }

        public string IdCorrGlobali
        {
            get;
            set;
        }

        public string Registri
        {
            get; 
            set; 
        }

        public int RequestedPage
        {
            get; 
            set;
        }

        public int PageSize
        {
            get;
            set;
        }

        public string ParentFolderId
        {
            get; 
            set;
        }

        public string FascId
        {
            get;
            set;
        }


    }
}
