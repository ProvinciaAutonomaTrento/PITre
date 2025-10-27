// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.amministrazione
{
    public class SistemaEsterno
    {
        public string IdSistemaEsterno= string.Empty;

        public string CodiceApplicazione = string.Empty;

        public string DescApplicazione = string.Empty;

        public string Diritti = string.Empty;

        public string UserIdAssociato = string.Empty;

        public string idRuoloAssociato = string.Empty;

        public string idAmministrazione = string.Empty;

        public string DescEstesa = string.Empty;

        public int TokenPeriod = 0;
    }
}
