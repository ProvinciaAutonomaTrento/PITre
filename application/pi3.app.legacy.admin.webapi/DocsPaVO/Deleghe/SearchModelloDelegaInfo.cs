// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Deleghe
{
    public class SearchModelloDelegaInfo
    {
        public string Nome
        {
            get;
            set;
        }

        public string NomeDelegato
        {
            get;
            set;
        }

        public string IdRuoloDelegante
        {
            get;
            set;
        }

        public DateTime DataInizio
        {
            get;
            set;
        }

        public StatoModelloDelega StatoModelloDelega
        {
            get;
            set;
        }

        public Boolean StatoModelloDelegaSpec
        {
            get;
            set;
        }
    }
}
