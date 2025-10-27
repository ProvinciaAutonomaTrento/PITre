// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class LogInstallEntity
    {
        public long ID { get; set; }
        public DateTime DATA_OPERAZIONE { get; set; }
        public string COMANDO_RICHIESTO { get; set; }
        public string VERSIONE_CD { get; set; }
        public string ESITO_OPERAZIONE { get; set; }
    }
}
