// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Conservazione.Batch.Infrastructure.Services.Conservazione
{
    internal class ProfileSearchAppendContext
    {
        public ProfileSearchAppendContext(IPi3DbContext dbContext, IQueryable<ProfileEntity> query)
        {
            DbContext = dbContext;
            Query = query;
        }

        public IPi3DbContext DbContext { get; set; }

        public IQueryable<ProfileEntity> Query { get; set; }
    }
}
