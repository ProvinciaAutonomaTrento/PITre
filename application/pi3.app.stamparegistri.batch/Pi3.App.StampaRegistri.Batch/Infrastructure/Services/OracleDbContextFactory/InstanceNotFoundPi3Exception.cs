// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.StampaRegistri.Batch.Infrastructure.Services.OracleDbContextFactory
{
    public class InstanceNotFoundPi3Exception : NotFoundPi3Exception
    {
        public InstanceNotFoundPi3Exception()
            : base(ErrorDescriptions.InstanceNotFound, null, ErrorDescriptions.ResourceManager)
        {
                
        }
    }
}
