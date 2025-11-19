// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Microsoft.Extensions.Logging;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Services.WebMethodMockLogger
{
    public class WebMethodEmptyLoggerService : Pi3.Infrastructure.Legacy.EF.Services.WebMethodLogger.WebMethodLoggerEFService
    {
        public WebMethodEmptyLoggerService(ILogger<WebMethodEmptyLoggerService> logger, IClaimsPrincipalService claimsPrincipalService, IPi3DbContext dbContext) : base(logger, claimsPrincipalService, dbContext)
        {
        }

        protected override async Task Log(bool result, string webMethodName, string? idObject = null, string? objectDescription = null, string? idSingleTransmission = null, string? application = null, bool? bypassNotifications = null, string? idTenant = null, string? idUser = null, string? userId = null, string? idGroup = null, string? delegatedIdUser = null, DateTime? executionDateTime = null)
        {
        }
    }
}
