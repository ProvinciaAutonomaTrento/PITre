// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocsPaVO.documento;

namespace DocsPaVO.Mobile.Requests
{
    public class GetPreviewRequest
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

        public int RequestedPage
        {
            get;
            set;
        }

        public int DimX
        {
            get;
            set;
        }

        public int DimY
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
