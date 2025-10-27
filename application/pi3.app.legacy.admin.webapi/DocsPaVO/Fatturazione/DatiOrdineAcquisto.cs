// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace DocsPaVO.Fatturazione
{
    [Serializable()]
    public class DatiOrdineAcquisto
    {
        public string RiferimentoNumeroLinea { get; set; }

        public string IdDocumento { get; set; }

        public DateTime? Data { get; set; }

        public string NumItem { get; set; }

        public string CodiceCommessaConvenzione { get; set; }

        public string CodiceCUP { get; set; }

        public string CodiceCIG { get; set; }

        public string NumeroLineaSAP { get; set; }
    }
}
