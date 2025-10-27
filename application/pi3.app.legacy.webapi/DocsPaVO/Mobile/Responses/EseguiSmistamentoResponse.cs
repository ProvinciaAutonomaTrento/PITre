// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Mobile.Responses
{
    public class EseguiSmistamentoResponse
    {
        public EseguiSmistamentoResponseCode Code
        {
            get;
            set;
        }

        public static EseguiSmistamentoResponse ErrorResponse
        {
            get
            {
                EseguiSmistamentoResponse res = new EseguiSmistamentoResponse();
                res.Code = EseguiSmistamentoResponseCode.SYSTEM_ERROR;
                return res;
            }
        }
    }

    public enum EseguiSmistamentoResponseCode
    {
        OK, SYSTEM_ERROR
    }
}
