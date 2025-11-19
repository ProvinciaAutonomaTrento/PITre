// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.utente
{
    public class InfoComune
    {
        private string cap;
        private string comune;
        private string provincia;

        public string CAP
        {
            get
            {
                return cap;
            }
            set
            {
                cap = value;
            }
        }

        public string COMUNE
        {
            get
            {
                return comune;
            }
            set
            {
                comune = value;
            }
        }

        public string PROVINCIA
        {
            get
            {
                return provincia;
            }
            set
            {
                provincia = value;
            }
        }
    }
}
