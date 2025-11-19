// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Mobile.Requests
{
    public class LibroFirmaRequest
    {
        public UserInfo UserInfo
        {
            get;
            set;
        }

        public string IdGruppo
        {
            get;
            set;
        }

        public int RequestedPage
        {
            get;
            set;
        }

        public int PageSize
        {
            get;
            set;
        }

        public string Testo
        {
            get;
            set;
        }

        public RicercaType TipoRicerca
        {
            get;
            set;
        }
    }
}
