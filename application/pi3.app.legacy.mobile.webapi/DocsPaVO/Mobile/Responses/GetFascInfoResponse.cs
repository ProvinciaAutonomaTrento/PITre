// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Mobile.Responses
{
    public class GetFascInfoResponse{
        public FascInfo FascInfo
        {
            get;
            set;
        }

        public TrasmInfo TrasmInfo
        {
            get;
            set;
        }

        public GetFascInfoResponseCode Code
        {
            get;
            set;
        }

        public static GetFascInfoResponse ErrorResponse
        {
            get
            {
                GetFascInfoResponse resp = new GetFascInfoResponse();
                resp.Code = GetFascInfoResponseCode.SYSTEM_ERROR;
                return resp;
            }
        }

    }

    public enum GetFascInfoResponseCode
    {
        OK, SYSTEM_ERROR
    }
}
