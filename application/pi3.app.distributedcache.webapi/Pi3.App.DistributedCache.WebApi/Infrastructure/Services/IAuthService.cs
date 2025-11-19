// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
namespace Pi3.App.DistributedCache.WebApi.Infrastructure.Services
{
    public interface IAuthService 
    {
        Task<bool> Authenticate(string userName, string password);
    }
}
