// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{

    public class SalvaRicercaEntity
    {
        public long SYSTEM_ID { get; set; }
        public long? ID_PEOPLE { get; set; }
        public long? ID_GRUPPO { get; set; }
        public string? VAR_DESCRIZIONE { get; set; }
        public string VAR_PAGINA_RIC { get; set; }
        public string? VAR_FILTRI_RIC { get; set; }
        public string CHA_IN_ADL { get; set; }
        public string TIPO { get; set; }
        public long? GRID_ID { get; set; }
    }

}
