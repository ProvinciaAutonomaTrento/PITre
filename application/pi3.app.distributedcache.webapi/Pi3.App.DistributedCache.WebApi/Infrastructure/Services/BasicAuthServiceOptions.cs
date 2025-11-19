// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
namespace Pi3.App.DistributedCache.WebApi.Infrastructure.Services
{
    public class BasicAuthServiceOptions
    {
        public string UserName { get; set; } = null!;

        public string Password { get; set; } = null!;
    }
}
