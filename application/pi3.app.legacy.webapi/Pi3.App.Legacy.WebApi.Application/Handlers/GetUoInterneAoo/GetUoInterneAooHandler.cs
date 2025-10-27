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
using static System.Net.Mime.MediaTypeNames;
using Newtonsoft.Json;

namespace Pi3.App.Legacy.WebApi.Infrastructure.EF.Oracle.Handlers.GetUoInterneAoo
{

    // Richiede libreria MediatR
    public class GetUoInterneAooHandler : IRequestHandler<Application.Requests.GetUoInterneAoo, GetUoInterneAooResult>
    {
        #region Public Members

        public GetUoInterneAooHandler(ILogger<GetUoInterneAooHandler> logger, IPi3DbContext dbContext,
            IMediator mediator, IClaimsPrincipalService claimsPrincipalService)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<GetUoInterneAooResult> Handle(Application.Requests.GetUoInterneAoo request, CancellationToken cancellationToken)
        {
            long idReg = long.Parse(request.id_reg);
            

            var j = _dbContext.CorrGlobaliEntities.Join(_dbContext.UoRegEnties, c => c.SYSTEM_ID, u => u.ID_UO, (c, u) => new { c, u });
            string[] result = j.Where(a => a.u.ID_REGISTRO == idReg).OrderBy(a => a.c.VAR_COD_RUBRICA).OrderBy(a => a.c.VAR_DESC_CORR).Select(a => a.c.VAR_COD_RUBRICA).ToArray();

            return new GetUoInterneAooResult(result);
            
        }



        #endregion

        #region Private Members

        protected readonly ILogger<GetUoInterneAooHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        
        #endregion
    }

}
