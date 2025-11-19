// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Mobile.Responses
{
    public class EseguiTrasmResponse
    {
        public EseguiTrasmResponseCode Code
        {
            get;
            set;
        }

        public static EseguiTrasmResponse ErrorResponse
        {
            get
            {
                EseguiTrasmResponse res = new EseguiTrasmResponse();
                res.Code = EseguiTrasmResponseCode.SYSTEM_ERROR;
                return res;
            }
        }
    }

    public enum EseguiTrasmResponseCode
    {
        OK,SYSTEM_ERROR
    }
}
