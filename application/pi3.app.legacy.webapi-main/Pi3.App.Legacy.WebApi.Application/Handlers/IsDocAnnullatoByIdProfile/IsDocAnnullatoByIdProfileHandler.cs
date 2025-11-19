// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Extensions;
using Pi3.App.Legacy.WebApi.Application.Handlers.FunzioneEsistente;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IsDocAnnullatoByIdProfileRequest = Pi3.App.Legacy.WebApi.Application.Requests.IsDocAnnullatoByIdProfile;


namespace Pi3.App.Legacy.WebApi.Application.Handlers.IsDocAnnullatoByIdProfile
{
    public class IsDocAnnullatoByIdProfileHandler: IRequestHandler<IsDocAnnullatoByIdProfileRequest, IsDocAnnullatoByIdProfileResult>
    {

        #region Private Members


        protected readonly ILogger<IsDocAnnullatoByIdProfileHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected IMapper _mapper = null;


        #endregion


        #region Public Members

        public IsDocAnnullatoByIdProfileHandler(ILogger<IsDocAnnullatoByIdProfileHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext
            )
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;


        }


        public async Task<IsDocAnnullatoByIdProfileResult> Handle(IsDocAnnullatoByIdProfileRequest request, CancellationToken cancellationToken)
        {
            bool result = false;
            try
            {
                var x = await this._dbContext.ProfileEntities.AsNoTracking().Where(
                    row => row.SYSTEM_ID == request.idProfile.AsLong() && row.DTA_ANNULLA != null
                ).Select(
                    row => new
                    {
                        SYSTEM_ID = row.SYSTEM_ID,
                    }
              
                    ).ToListAsync();

                result = x.Count() > 0;
            }

            catch (Exception ex)
            {
                this._logger.LogWebMethodError(ex);

            }
            return new IsDocAnnullatoByIdProfileResult(result);
        }

        #endregion


    }
}
