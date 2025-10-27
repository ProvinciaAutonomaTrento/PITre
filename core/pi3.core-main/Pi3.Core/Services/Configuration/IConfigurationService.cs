// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.Services.Configuration
{
    public interface IConfigurationService : IService
    {
        Task<(T?, bool)> TryGetValue<T>(string key);

        Task<T> GetValue<T>(string key, bool? throwIfNotExists = false, T? defaultValue = default(T));

        Task<(T?, bool)> TryGetValue<T>(string idTenant, string key);

        Task<T> GetValue<T>(string idTenant, string key, bool? throwIfNotExists = false, T? defaultValue = default(T));
    }
}
