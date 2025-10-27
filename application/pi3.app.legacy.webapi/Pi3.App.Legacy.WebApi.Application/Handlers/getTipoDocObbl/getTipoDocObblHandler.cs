// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Extensions;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using getTipoDocObblRequest = Pi3.App.Legacy.WebApi.Application.Requests.getTipoDocObbl;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.getTipoDocObbl
{
    public class getTipoDocObblHandler : IRequestHandler<getTipoDocObblRequest, getTipoDocObblResult>
    {
        protected readonly IPi3DbContext _dbContext;
        protected readonly ILogger<getTipoDocObblHandler> _logger;

        public getTipoDocObblHandler(
            IPi3DbContext dbContext,
            ILogger<getTipoDocObblHandler> logger
            )
        {
            this._dbContext = dbContext;
            this._logger = logger;
        }

        public async Task<getTipoDocObblResult> Handle(getTipoDocObblRequest request , CancellationToken cancellationToken)
        {
            string? output = "0";
            try
            {
                output = await this._dbContext.AmministraEntities.AsNoTracking().Where(am => am.SYSTEM_ID == request.idAmministrazione.AsLong()).Select( am => am.TIPO_DOC_OBBL).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                this._logger.LogWebMethodError(ex);
            }
            return new(output);
        }
    }
}
