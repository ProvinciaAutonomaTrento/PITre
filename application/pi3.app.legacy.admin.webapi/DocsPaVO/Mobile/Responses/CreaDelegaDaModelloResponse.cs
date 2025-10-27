// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Mobile.Responses
{
    public class CreaDelegaDaModelloResponse
    {
        public CreaDelegaDaModelloResponseCode Code
        {
            get;
            set;
        }

        public static CreaDelegaDaModelloResponse ErrorResponse
        {
            get
            {
                CreaDelegaDaModelloResponse resp = new CreaDelegaDaModelloResponse();
                resp.Code = CreaDelegaDaModelloResponseCode.SYSTEM_ERROR;
                return resp;
            }
        }
    }

    public enum CreaDelegaDaModelloResponseCode
    {
        OK,NOT_CREATED,SYSTEM_ERROR
    }
}
