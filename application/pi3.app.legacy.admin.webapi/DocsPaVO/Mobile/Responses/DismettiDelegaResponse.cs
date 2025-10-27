// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Mobile.Responses
{
    public class DismettiDelegaResponse
    {
        public UserInfo UserInfo
        {
            get;
            set;
        }

        public DismettiDelegaResponseCode Code
        {
            get;
            set;
        }

        public static DismettiDelegaResponse ErrorResponse
        {
            get
            {
                DismettiDelegaResponse resp = new DismettiDelegaResponse();
                resp.Code = DismettiDelegaResponseCode.SYSTEM_ERROR;
                return resp;
            }
        }
    }

    public enum DismettiDelegaResponseCode
    {
        OK, SYSTEM_ERROR, OPERATION_FAILED
    }
}
