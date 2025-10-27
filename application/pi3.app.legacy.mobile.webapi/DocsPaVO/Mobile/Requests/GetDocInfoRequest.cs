// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Mobile.Requests
{
    public class GetDocInfoRequest
    {
        public UserInfo UserInfo
        {
            get; 
            set;
        }

        public string IdTrasm
        {
            get;
            set; 
        }

        public string IdEvento
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

