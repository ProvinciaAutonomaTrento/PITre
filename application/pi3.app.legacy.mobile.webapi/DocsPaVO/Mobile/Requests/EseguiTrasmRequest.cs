// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Mobile.Requests
{
    public class EseguiTrasmRequest
    {
        public UserInfo UserInfo
        {
            get;
            set;
        }

        public string IdModelloTrasm
        {
            get;
            set;
        }

        public string IdDoc
        {
            get;
            set;
        }

        public string IdFasc
        {
            get;
            set;
        }

        public string IdCorrGlobali
        {
            get;
            set;
        }

        public string Note
        {
            get;
            set;
        }

        public string Path
        {
            get;
            set;
        }

    }
}
