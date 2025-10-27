// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF;
using Pi3.Infrastructure.Legacy.EF.Entities;
using Pi3.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Infrastructure.EF.Oracle.Handlers.GetCorrByEmail
{
    public class GetCorrByEmailHandler : IRequestHandler<Application.Requests.GetCorrByEmail, GetCorrByEmailResult>
    {

        #region public method

        public GetCorrByEmailHandler(IPi3DbContext dbContext, ILogger<GetCorrByEmailHandler> logger, IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator)
        {
            this._logger = logger;
            this._dbContext = dbContext;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
        }

        public async Task<GetCorrByEmailResult> Handle(Application.Requests.GetCorrByEmail request, CancellationToken cancellationToken)
        {
            var idTenant = _claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant);
            var email = request.email != null ? request.email.ToUpper() : string.Empty;

            long[] idRegistri = new long[0] { };

            if (!string.IsNullOrEmpty(request.idRegistri))
                idRegistri = request.idRegistri.Contains(',') ? request.idRegistri.Replace("'", "").Split(',')?.Select(id => id.AsLong()).ToArray() : new long[] { request.idRegistri.Replace("'", "").AsLong() };

            var query = _dbContext.CorrGlobaliEntities.AsNoTracking()
                .Join(_dbContext.MailCorrEsterniEntities.AsNoTracking(),
                    c => c.SYSTEM_ID,
                    m => m.ID_CORR,
                    (c, m) => new { c, m })
                .Where(j => j.m.VAR_EMAIL.ToUpper().Equals(email)
                    && ((j.c.ID_REGISTRO == null && j.c.ID_AMM == idTenant) || idRegistri.Contains(j.c.ID_REGISTRO.GetValueOrDefault()))
                    && j.c.DTA_FINE == null
                    && !j.c.VAR_COD_RUBRICA.ToUpper().StartsWith("INTEROP")
                    && !j.c.VAR_COD_RUBRICA.ToUpper().Contains("@")
                    && j.c.CHA_TIPO_CORR != "O")
                .Select(j => new
                {
                    j.c.SYSTEM_ID,
                    j.c.ID_AMM,
                    j.c.VAR_COD_RUBRICA,
                    j.c.VAR_DESC_CORR,
                    j.c.VAR_CODICE,
                    j.c.ID_REGISTRO,
                    j.c.VAR_INSERT_BY_INTEROP,
                    VAR_COD_REGISTRO = IPi3DbContextMappedFunctions.GetCodReg(j.c.ID_REGISTRO.GetValueOrDefault())
                });

            var list = await query.ToListAsync();

            return new GetCorrByEmailResult(list.AsDataSet("CorrispondentiByMail", "CorrispondentiByMail"));

        }

        #endregion

        #region Private Method

        protected readonly ILogger<GetCorrByEmailHandler> _logger;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IMediator _mediator;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;

        #endregion
    }
}
