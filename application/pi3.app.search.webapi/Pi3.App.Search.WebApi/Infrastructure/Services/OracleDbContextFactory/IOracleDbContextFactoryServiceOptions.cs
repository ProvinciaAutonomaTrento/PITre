// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
namespace Pi3.App.Search.WebApi.Infrastructure.Services.OracleDbContextFactory
{
    public interface IOracleDbContextFactoryServiceOptions
    {
        string InstanceName { get; }

        bool EnableLogging { get; }

        string UseOracleSQLCompatibility { get; }
    }
}
