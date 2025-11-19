// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Mobile.Responses
{
    public class RevocaDelegheResponse
    {
        public RevocaDelegheResponseCode Code
        {
            get;
            set;
        }

        public string Error
        {
            get;
            set;
        }

        public static RevocaDelegheResponse ErrorResponse
        {
            get
            {
                RevocaDelegheResponse resp = new RevocaDelegheResponse();
                resp.Code = RevocaDelegheResponseCode.SYSTEM_ERROR;
                return resp;
            }
        }
    }

    public enum RevocaDelegheResponseCode
    {
        OK, SYSTEM_ERROR, OPERATION_FAILED
    }
}
