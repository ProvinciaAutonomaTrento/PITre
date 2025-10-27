// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GetIdRuoloRespConservazioneRequest = Pi3.App.Legacy.WebApi.Application.Requests.GetIdRuoloRespConservazione;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GetIdRuoloRespConservazione
{
    public class GetIdRuoloRespConservazioneHandler : IRequestHandler<GetIdRuoloRespConservazioneRequest, GetIdRuoloRespConservazioneResult>
    {
        #region Public Members

        public GetIdRuoloRespConservazioneHandler(ILogger<GetIdRuoloRespConservazioneHandler> logger, 
            IClaimsPrincipalService claimsPrincipalService, 
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<GetIdRuoloRespConservazioneResult> Handle(GetIdRuoloRespConservazioneRequest request, CancellationToken cancellationToken)
        {
            var idAmmAsLong = request.idAmm.AsLong();
            long? idRuoloResp = null;

            if(!string.IsNullOrEmpty(request.idAOO))
            {
                var idAOOAsLong = request.idAOO.AsLong();
                idRuoloResp = await this._dbContext.RespConsAooEntities.AsNoTracking().Where(r => r.ID_AMM == idAmmAsLong && r.ID_REGISTRO == idAOOAsLong).Select(r => r.ID_GRUPPO_RESP_CONS).FirstOrDefaultAsync();
            }
            else
            {
                idRuoloResp = await this._dbContext.AmministraEntities.AsNoTracking().Where(a => a.SYSTEM_ID == idAmmAsLong).Select(a => a.ID_RUOLO_RESP_CONS).FirstOrDefaultAsync();
            }

            var output = idRuoloResp != null ? idRuoloResp.ToString() : string.Empty;

            return new GetIdRuoloRespConservazioneResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetIdRuoloRespConservazioneHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }
}
