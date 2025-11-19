// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Mobile.Responses
{
    public class ListaUtentiResponse
    {
        public ListaUtentiResponseCode Code
        {
            get; 
            set;
        }

        public List<UserInfo> Utenti
        {
            get; 
            set;
        }

        public static ListaUtentiResponse ErrorResponse{
            get
            {
                ListaUtentiResponse resp = new ListaUtentiResponse();
                resp.Code = ListaUtentiResponseCode.SYSTEM_ERROR;
                return resp;
            }
        }
    }

    public enum ListaUtentiResponseCode
    {
        OK,SYSTEM_ERROR
    }
}
