// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.Services.WebMethodLogger
{
    public interface IWebMethodLoggerService : IService
    {
        Task LogOK(string webMethodName, string? idObject = null, string? objectDescription = null, string? idSingleTransmission = null, string? application = null, bool? bypassNotifications = null, string? idTenant = null, string? idUser = null, string? userId = null, string? idGroup = null, string? delegatedIdUser = null, DateTime? executionDateTime = null);

        Task LogKO(string webMethodName, string? idObject = null, string? objectDescription = null, string? idSingleTransmission = null, string? application = null, bool? bypassNotifications = null, string? idTenant = null, string? idUser = null, string? userId = null, string? idGroup = null, string? delegatedIdUser = null, DateTime? executionDateTime = null);
    }
}
