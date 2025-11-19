// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
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
using Pi3.App.Legacy.WebApi.Application.Handlers.Albo_GetFileDaPubblicare;

namespace Pi3.App.Legacy.WebApi.Infrastructure.EF.Oracle.Handlers.isOggettoModificato
{
    public class isOggettoModificatoHandler : IRequestHandler<IsOggettoModificato, IsOggettoModificatoResult>
    {
        public isOggettoModificatoHandler(ILogger<isOggettoModificatoHandler> logger, IPi3DbContext DbContext, 
            IClaimsPrincipalService claimsPrincipalService, IMediator Mediator )
        {
            this._logger = logger;
            this._dbContext = DbContext;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = Mediator;
        }

        public async Task<IsOggettoModificatoResult> Handle(IsOggettoModificato request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.idProfile))
                return new IsOggettoModificatoResult(false);

            var idDocument = request.idProfile.AsLong();

            var isOggettoModificato =
                    await (from p in _dbContext.ProfileEntities.AsNoTracking()
                           join o in _dbContext.OggettiStoEntities.AsNoTracking() on p.SYSTEM_ID equals o.ID_PROFILE
                           where p.SYSTEM_ID == idDocument
                               && p.VAR_SEGNATURA != null
                               && o.DTA_MODIFICA > p.DTA_PROTO
                           select new { p.SYSTEM_ID })
                        .AnyAsync();

            if (!isOggettoModificato)
            {
                isOggettoModificato =
                    await (from p in _dbContext.ProfileEntities.AsNoTracking()
                           join oggettiSto in _dbContext.OggettiStoEntities.AsNoTracking() on p.SYSTEM_ID equals oggettiSto.ID_PROFILE
                           join ass in _dbContext.AssociazioneTemplatesEntities.AsNoTracking() on p.DOCNUMBER equals Convert.ToInt32(ass.DOC_NUMBER)
                           join oggCustom in _dbContext.OggettiCustomEntities.AsNoTracking() on ass.ID_OGGETTO equals oggCustom.SYSTEM_ID
                           where ass.DOC_NUMBER == request.idProfile
                                   && oggCustom.REPERTORIO == 1
                                   && ass.VALORE_OGGETTO_DB != null
                                   && oggettiSto.DTA_MODIFICA > ass.DTA_INS
                           select new
                           {
                               p.SYSTEM_ID
                           })
                        .AnyAsync();
            }

            return new IsOggettoModificatoResult(isOggettoModificato);
        }

        protected readonly ILogger<isOggettoModificatoHandler> _logger;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IMediator _mediator;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
    }
}
