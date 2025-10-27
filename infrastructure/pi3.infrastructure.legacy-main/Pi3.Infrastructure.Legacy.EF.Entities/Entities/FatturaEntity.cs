// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class FatturaEntity
    {
        public string? IDAMMINISTRAZIONE { get; set; }
        public string? TEMPLATEXML { get; set; }
        public string? FORMATO_TRASMISSIONE { get; set; }
        public string? ESIGIBILITA_IVA { get; set; }
        public string? CONDIZIONI_PAGAMENTO { get; set; }
        public string? IDTIPOATTO { get; set; }

    }
}
