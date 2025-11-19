// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.documento;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.AggregazioneDocumentaleAggregate.Exceptions;
using Pi3.Infrastructure.Legacy.EF.Entities;
using Pi3.Infrastructure.Legacy.EF.Entities.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.ConsolidateDocumentById
{
    public class ConsolidateDocumentByIdHandler : IRequestHandler<Application.Requests.ConsolidateDocumentById, ConsolidateDocumentByIdResult>
    {
        #region Public members

        public ConsolidateDocumentByIdHandler(ILogger<ConsolidateDocumentByIdHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator,
            IPi3DbContext dbContext, IDocumentoAmministrativoRepository documentRepository, IWebMethodLoggerService webMethodLoggerService)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._documentRepository = documentRepository;
            this._webMethodLoggerService = webMethodLoggerService;
        }

        public async Task<ConsolidateDocumentByIdResult> Handle(Requests.ConsolidateDocumentById request, CancellationToken cancellationToken)
        {
            var idTenant = _claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant, true);
            var idPeople = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdUser, true);
            var idGroup = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdGroup, true);

            DocumentConsolidationStateInfo output = default;

            try
            {
                if (!await this._documentRepository.Exists(idTenant, request.idDocument))
                {
                    throw new DocumentoNotFoundPi3Exception(request.idDocument);
                }

                await this._dbContext.AssertSecurityRights(request.idDocument, idPeople, idGroup);

                var aggregate = await this._documentRepository.Get(idTenant, request.idDocument);

                var isPredisposto = await this._dbContext.ProfileEntities
                    .AnyAsync(x => x.SYSTEM_ID == request.idDocument.AsLong()
                    && (x.CHA_TIPO_PROTO == "A" || x.CHA_TIPO_PROTO == "P" || x.CHA_TIPO_PROTO == "I")
                    && x.CHA_DA_PROTO == "1");

                if (isPredisposto)
                {
                    throw new DocumentoAmministrativoPredispostoException(request.idDocument);
                }

                var finalState = await this._dbContext.DiagrammiEntities.AsNoTracking()
                    .Join(this._dbContext.StatoEntities, d => d.ID_STATO, s => s.SYSTEM_ID, (d, s) => new { d, s })
                    .Select(x => x.s.STATO_FINALE)
                    .FirstOrDefaultAsync();

                if (finalState.HasValue && finalState.Value == 1)
                {
                    throw new DocInFinalStateException(request.idDocument);
                }

                aggregate.Consolida(new Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects.Consolidamento
                {
                    Stato = request.toState.ToStatiConsolidamentoEnum(),
                    Data = DateTime.Now,
                    Autore = new Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects.Autore { Id = idPeople }
                });

                await this._documentRepository.Update(aggregate);

                output = new DocumentConsolidationStateInfo
                {
                    State = aggregate.Consolidamento.Stato.ToDocumentConsolidationState(),
                    Date = aggregate.Consolidamento.Data.ToString(),
                    Author = idPeople,
                    Role = idGroup
                };

                await this._webMethodLoggerService.LogOK("CONSOLIDADOCUMENTO", request.idDocument, await this.GetLoggerMessage(request.idDocument));
            }
            catch(Exception ex)
            {
                this._logger.LogError(ex, null, null);

                await this._webMethodLoggerService.LogKO("CONSOLIDADOCUMENTO", request.idDocument, string.Format(Resources.LogConsolidateNoSegnaturaOK, request.idDocument));
            }

            return new ConsolidateDocumentByIdResult(output);
        }

        #endregion

        #region Private members

        protected readonly ILogger<ConsolidateDocumentByIdHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IDocumentoAmministrativoRepository _documentRepository;
        protected readonly IWebMethodLoggerService _webMethodLoggerService;

        private async Task<string> GetLoggerMessage(string idDocument)
        {
            // Estrazione segnatura per logger
            var segnatura = await this._dbContext.ProfileEntities.AsNoTracking()
                .Where(x => x.SYSTEM_ID == idDocument.AsLong())
                .Select(x => x.VAR_SEGNATURA)
                .FirstAsync();

            return (!string.IsNullOrWhiteSpace(segnatura)) ? 
                string.Format(Resources.LogConsolidateSegnaturaOK, idDocument, segnatura) :
                string.Format(Resources.LogConsolidateNoSegnaturaOK, idDocument);
        }
        #endregion
    }
}
