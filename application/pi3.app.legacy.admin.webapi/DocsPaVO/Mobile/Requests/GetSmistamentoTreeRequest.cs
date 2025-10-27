// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Mobile.Requests
{
    public class GetSmistamentoTreeRequest
    {
        public UserInfo UserInfo
        {
            get;
            set;
        }

        public RuoloInfo Ruolo
        {
            get;
            set;
        }

        // MEV MOBILE - smistamento
        // per navigazione UO inf/sup
        public string IdUO
        {
            get;
            set;
        }
    }
}
