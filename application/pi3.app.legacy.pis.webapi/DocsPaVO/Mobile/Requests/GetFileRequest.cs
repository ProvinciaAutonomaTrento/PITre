// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocsPaVO.documento;

namespace DocsPaVO.Mobile.Requests
{
    public class GetFileRequest
    {
        public UserInfo UserInfo
        {
            get;
            set;
        }

        public string IdDoc
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
    }
}
