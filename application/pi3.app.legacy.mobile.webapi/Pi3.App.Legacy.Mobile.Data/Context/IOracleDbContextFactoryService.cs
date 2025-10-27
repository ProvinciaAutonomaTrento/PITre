// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.Services;
using Pi3.Infrastructure.Legacy.EF.Oracle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.Mobile.Data.Context;
public interface IOracleDbContextFactoryService : IService
{
    OraclePi3DbContext CreateDbContext();
}
