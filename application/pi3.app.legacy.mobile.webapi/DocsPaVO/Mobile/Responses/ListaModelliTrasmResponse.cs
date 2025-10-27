// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Mobile.Responses
{
    public class ListaModelliTrasmResponse
    {
        public ListaModelliTrasmResponseCode Code
        {
            get;
            set;
        }

        public List<ModelloTrasm> Modelli
        {
            get;
            set;
        }

        public static ListaModelliTrasmResponse ErrorResponse
        {
            get
            {
                ListaModelliTrasmResponse res = new ListaModelliTrasmResponse();
                res.Code = ListaModelliTrasmResponseCode.SYSTEM_ERROR;
                return res;
            }
        }
    }

    public enum ListaModelliTrasmResponseCode
    {
        OK,SYSTEM_ERROR
    }
}
