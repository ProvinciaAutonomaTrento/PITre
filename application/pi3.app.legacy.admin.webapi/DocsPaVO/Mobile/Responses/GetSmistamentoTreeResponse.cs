// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Mobile.Responses
{
    public class GetSmistamentoTreeResponse
    {
        public SmistamentoTree Element
        {
            get;
            set;
        }
        public GetSmistamentoTreeResponseCode Code
        {
            get;
            set;
        }

        public static GetSmistamentoTreeResponse ErrorResponse
        {
            get
            {
                GetSmistamentoTreeResponse res = new GetSmistamentoTreeResponse();
                res.Code = GetSmistamentoTreeResponseCode.SYSTEM_ERROR;
                return res;
            }
        }
    }

    public enum GetSmistamentoTreeResponseCode
    {
        OK, SYSTEM_ERROR
    }
}
