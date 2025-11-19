// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Extensions;
using Pi3.App.Legacy.WebApi.Application.Handlers.AmmGetListaRuoliAOO;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FunzioneEsistenteRequest = Pi3.App.Legacy.WebApi.Application.Requests.FunzioneEsistente;


namespace Pi3.App.Legacy.WebApi.Application.Handlers.FunzioneEsistente
{
    public class FunzioneEsistenteHandler : IRequestHandler<FunzioneEsistenteRequest, FunzioneEsistenteResult>
    {
        #region Private Members

        protected readonly ILogger<FunzioneEsistenteHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected IMapper _mapper = null;

        #endregion

        #region Public Members

        public FunzioneEsistenteHandler(ILogger<FunzioneEsistenteHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;

        }

        public async Task<FunzioneEsistenteResult> Handle(FunzioneEsistenteRequest request, CancellationToken cancellationToken)
        {
            bool result = false;
            try
            {
                result = this._dbContext.AnagraficaFunzioniEntities.AsNoTracking().Where(
                    row => row.COD_FUNZIONE.ToUpper() == request.codiceFunzione.ToUpper() && row.DISABLED == "N"
                    ).Count() > 0;
            }

            catch (Exception ex)
            {
                this._logger.LogWebMethodError(ex);

            }
            return new FunzioneEsistenteResult(result);
        }

        #endregion

    }


}
