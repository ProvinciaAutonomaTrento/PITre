// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.Services;
using Pi3.Infrastructure.Legacy.EF.Oracle;

namespace Pi3.App.InteropPitre.WebApi.Infrastructure.Services.OracleDbContextFactory
{
    public interface IOracleDbContextFactoryService : IService
    {
        OraclePi3DbContext CreateDbContext();
    }
}
