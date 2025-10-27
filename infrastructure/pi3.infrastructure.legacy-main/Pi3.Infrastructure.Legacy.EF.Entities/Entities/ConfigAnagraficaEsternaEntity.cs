// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class ConfigAnagraficaEsternaEntity
    {
        public long? SYSTEM_ID { get; set; }

        public string? NOME_ANAGRAFICA_ESTERNA { get; set; }

        public string? WSURL_ANAGRAFICA_ESTERNA { get; set; }

        public string? INTEGRATION_ADAPTER_ID { get; set; }

        public string? INTEGRATION_ADAPTER_VERSION { get; set; }

        public string? CONFIG_PROVENIENZA { get; set; }

        public string? CONFIG_MATRICOLA { get; set; }

        public string? CONFIG_PASSWORD { get; set; }

        public string? CONFIG_APPLICAZIONE { get; set; }

        public string? CONFIG_RUOLO { get; set; }

        public string? NOTE { get; set; }

        public string? ENABLED { get; set; }
    }
}
