// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocsPaVO.filtri
{
    public class FiltroMail
    {
        public TipoFiltroMail Tipo
        {
            get;
            set;
        }

        public string Valore
        {
            get;
            set;
        }
    }


    public enum TipoFiltroMail
    {
        MITTENTE,OGGETTO
    }
}