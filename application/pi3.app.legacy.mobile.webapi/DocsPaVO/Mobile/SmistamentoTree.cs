// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Mobile
{
    public class SmistamentoTree
    {
        public SmistamentoUONode UOAppartenenza
        {
            get;
            set;
        }

        public List<SmistamentoUONode> AltreUO
        {
            get;
            set;
        }

        // MEV MOBILE - smistamento
        // navigazione UO inferiori/superiori
        public string idParent
        {
            get;
            set;
        }
    }
}
