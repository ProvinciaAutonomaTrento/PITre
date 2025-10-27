// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.Services;
using Pi3.Infrastructure.Legacy.EF.Oracle;

namespace Pi3.App.AggregazioneDocumentale.WebApi.Infrastructure.Services.OracleDbContextFactory
{
    public interface IOracleDbContextFactoryService : IService
    {
        OraclePi3DbContext CreateDbContext();
    }
}
