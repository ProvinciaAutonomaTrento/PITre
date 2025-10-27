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
using DocumentoIsPrimaIstanzaConsRequest = Pi3.App.Legacy.WebApi.Application.Requests.DocumentoIsPrimaIstanzaCons;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.DocumentoIsPrimaIstanzaCons
{
    public class DocumentoIsPrimaIstanzaConsHandler : IRequestHandler<DocumentoIsPrimaIstanzaConsRequest, DocumentoIsPrimaIstanzaConsResult>
    {
        #region Public Members

        public DocumentoIsPrimaIstanzaConsHandler(ILogger<DocumentoIsPrimaIstanzaConsHandler> logger,
            IClaimsPrincipalService claimsPrincipalService, 
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<DocumentoIsPrimaIstanzaConsResult> Handle(DocumentoIsPrimaIstanzaConsRequest request, CancellationToken cancellationToken)
        {
            var output = 0;

            try
            {
                var idPeople = request.idPeople.AsLong();
                var idGruppo = request.idGruppo.AsLong();
                var idRuoloInUO = await this._dbContext.CorrGlobaliEntities.AsNoTracking().Where(c => c.ID_GRUPPO == idGruppo).Select(c => c.SYSTEM_ID).FirstOrDefaultAsync();

                var areaConservazioneEntity = await this._dbContext.AreaConservazioneEntities.AsNoTracking()
                    .AnyAsync(a => a.ID_PEOPLE == idPeople && a.ID_RUOLO_IN_UO == idRuoloInUO && a.CHA_STATO == "N");

                output = areaConservazioneEntity ? 0 : 1;

            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, null, null);
                output = -1;
            }

            return new DocumentoIsPrimaIstanzaConsResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<DocumentoIsPrimaIstanzaConsHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        #endregion
    }
}
