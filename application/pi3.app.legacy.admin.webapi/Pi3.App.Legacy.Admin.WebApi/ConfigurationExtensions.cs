// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace Pi3.App.Legacy.Admin.WebApi
{
    public static class ConfigurationExtensions
    {
        public static Dictionary<string, string> GetConnectionStrings(this IConfiguration configuration)
        {
            var connectionStringsSection = configuration.GetSection("ConnectionStrings");
            var connectionStrings = new Dictionary<string, string>();

            foreach (var connectionString in connectionStringsSection.GetChildren())
            {
                connectionStrings[connectionString.Key] = connectionString.Value;
            }

            return connectionStrings;
        }
    }
}
