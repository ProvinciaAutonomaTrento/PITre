// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Mobile.Responses
{
    public class LogoutResponse
    {
        public LogoutResponse()
        {
        }

        public LogoutResponse(LogoutResponseCode code)
        {
            this.Code = code;
        }

        public LogoutResponseCode Code
        {
            get;
            set;
        }

        public static LogoutResponse ErrorResponse
        {
            get
            {
                LogoutResponse res = new LogoutResponse(LogoutResponseCode.SYSTEM_ERROR);
                return res;
            }
        }

    }

    public enum LogoutResponseCode
    {
        OK, SYSTEM_ERROR
    }
}
