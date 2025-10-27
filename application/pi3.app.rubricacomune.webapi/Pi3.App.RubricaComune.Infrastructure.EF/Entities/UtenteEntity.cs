// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.RubricaComune.Infrastructure.EF.Entities
{
    public class UtenteEntity
    {
        public long ID { get; set; }

        public string NOME { get; set; } = null!;

        public string PASSWORD { get; set; } = null!;

        public DateTime DATACREAZIONE { get; set; }

        public DateTime DATAULTIMAMODIFICA { get; set; }

        public string AMMINISTRATORE { get; set; } = null!;
    }
}
