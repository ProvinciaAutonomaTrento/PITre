// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Mobile.Responses
{
    public class RicercaUtentiResponse
    {
        public RicercaUtentiResponseCode Code { get; set; }

        public List<LightUserInfo> Risultati
        {
            get;
            set;
        }

        public static RicercaUtentiResponse ErrorResponse
        {
            get
            {
                RicercaUtentiResponse resp = new RicercaUtentiResponse();
                resp.Code = RicercaUtentiResponseCode.SYSTEM_ERROR;
                return resp;
            }
        }
    }

    public enum RicercaUtentiResponseCode
    {
        OK,SYSTEM_ERROR
    }
}
