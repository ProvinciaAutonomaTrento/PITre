// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Conservazione.Batch.Infrastructure.Services.OracleDbContextFactory
{
    public class ConnectionStringNotFoundPi3Exception : NotFoundPi3Exception
    {
        public ConnectionStringNotFoundPi3Exception(string instance)
            : base(ErrorDescriptions.ConnectionStringNotFound, null, ErrorDescriptions.ResourceManager, instance)
        {
            this.Instance = instance;
        }

        public string Instance { get; init; }
    }
}
