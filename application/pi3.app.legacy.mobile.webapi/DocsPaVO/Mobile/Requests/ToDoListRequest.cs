// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
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
