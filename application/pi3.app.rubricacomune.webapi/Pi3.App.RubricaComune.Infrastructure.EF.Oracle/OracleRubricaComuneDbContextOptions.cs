// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.RubricaComune.Infrastructure.EF.Oracle
{
    public class OracleRubricaComuneDbContextOptions
    {
        public string ConnectionString { get; set; } = null!;

        public bool? EnableLogging { get; set; } = false;

        public string? UseOracleSQLCompatibility { get; set; }
    }
}
