// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Indexer.Infrastructure.Services.OracleDbContextFactory
{
    public class OracleDbContextFactoryServiceOptions
    {
        [Required(AllowEmptyStrings = false)]
        public string Instance { get; set; }

        public bool EnableLogging { get; set; }

        public string UseOracleSQLCompatibility { get; set; }
    }
}
