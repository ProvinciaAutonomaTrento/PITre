// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
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
