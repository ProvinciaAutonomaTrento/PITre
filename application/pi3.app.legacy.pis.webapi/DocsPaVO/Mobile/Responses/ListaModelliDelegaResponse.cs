// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Mobile.Responses
{
    public class ListaModelliDelegaResponse
    {
        public List<ModelloDelegaInfo> Modelli
        {
            get;
            set;
        }

        public ListaModelliDelegaResponseCode Code
        {
            get;
            set;
        }

        public static ListaModelliDelegaResponse ErrorResponse
        {
            get
            {
                ListaModelliDelegaResponse res = new ListaModelliDelegaResponse();
                res.Code = ListaModelliDelegaResponseCode.SYSTEM_ERROR;
                return res;
            }
        }
    }

    public enum ListaModelliDelegaResponseCode
    {
        OK, SYSTEM_ERROR
    }
}
