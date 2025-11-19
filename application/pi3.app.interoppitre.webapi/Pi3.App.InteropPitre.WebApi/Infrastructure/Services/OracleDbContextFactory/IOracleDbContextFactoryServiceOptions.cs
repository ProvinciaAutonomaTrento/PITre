// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
namespace Pi3.App.InteropPitre.WebApi.Infrastructure.Services.OracleDbContextFactory
{
    public interface IOracleDbContextFactoryServiceOptions
    {
        string InstanceName { get; }

        bool EnableLogging { get; }

        string UseOracleSQLCompatibility { get; }
    }
}
