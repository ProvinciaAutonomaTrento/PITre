// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Infrastructure.EF.Oracle.Handlers.GetCorrByEmailAndDescr
{
    public class GetCorrByEmailAndDescHandler : IRequestHandler<Application.Requests.GetCorrByEmailAndDescr, GetCorrByEmailAndDescrResult>
    {

        public GetCorrByEmailAndDescHandler(ILogger<GetCorrByEmailAndDescHandler> logger, IPi3DbContext dbContext, IMediator mediator, IClaimsPrincipalService claimsPrincipalService)
        {
            this._logger = logger;
            this._dbContext = dbContext;
            this._mediator = mediator;
            this._claimsPrincipalService = claimsPrincipalService;
        }

        public async Task<GetCorrByEmailAndDescrResult> Handle(Application.Requests.GetCorrByEmailAndDescr request, CancellationToken cancellationToken)
        {
            var idTenant = _claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant);
            var email = request.email != null ? request.email.ToUpper() : string.Empty;
            var descr = request.descr.ToUpper();

            long[] idRegistri = new long[0] {};

            if (!string.IsNullOrEmpty(request.idRegistri))
                idRegistri = request.idRegistri.Contains(',') ? request.idRegistri.Replace("'", "").Split(',')?.Select(id => id.AsLong()).ToArray() : new long[] { request.idRegistri.Replace("'", "").AsLong() };

            var corrGlobaliEntities = await _dbContext.CorrGlobaliEntities.AsNoTracking()
                .Join(_dbContext.MailCorrEsterniEntities.AsNoTracking(),
                    c => c.SYSTEM_ID,
                    m => m.ID_CORR,
                    (c, m) => new { c, m })
                .Where(j => j.m.VAR_EMAIL.ToUpper().Equals(email)
                    && j.c.VAR_DESC_CORR_OLD.ToUpper().Equals(descr)
                    && ((j.c.ID_REGISTRO == null && j.c.ID_AMM == idTenant) || (idRegistri.Contains(j.c.ID_REGISTRO.GetValueOrDefault())))
                    && j.c.DTA_FINE == null 
                    && !j.c.VAR_COD_RUBRICA.ToUpper().Contains("INTEROP")
                    && j.c.CHA_TIPO_CORR != "O")
                .Select(j => new
                {
                    j.c.SYSTEM_ID,
                    j.c.ID_AMM,
                    j.c.VAR_COD_RUBRICA,
                    j.c.VAR_DESC_CORR,
                    j.c.VAR_CODICE,
                    j.c.VAR_INSERT_BY_INTEROP,
                    VAR_COD_REGISTRO = IPi3DbContextMappedFunctions.GetCodReg(j.c.ID_REGISTRO.GetValueOrDefault())
                })
                .ToListAsync();

            return new GetCorrByEmailAndDescrResult(corrGlobaliEntities.AsDataSet("CorrispondentiByMailAndDescr", "CorrispondentiByMailAndDescr"));

        }

        protected IPi3DbContext _dbContext;
        protected IMediator _mediator;
        protected IClaimsPrincipalService _claimsPrincipalService;
        protected ILogger<GetCorrByEmailAndDescHandler> _logger;
    }
}
