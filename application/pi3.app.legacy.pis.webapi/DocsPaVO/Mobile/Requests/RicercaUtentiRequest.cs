// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Mobile.Requests
{
    public class RicercaUtentiRequest
    {
        public string Descrizione
        {
            get;
            set;
        }

        public UserInfo UserInfo
        {
            get;
            set;
        }

        public int NumMaxResults
        {
            get;
            set;
        }

        public RuoloInfo Ruolo
        {
            get;
            set;
        }
    }
}
