// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.EntityFrameworkCore;
using Pi3.App.AggregazioneDocumentale.WebApi.Application.Exceptions;
using Pi3.App.AggregazioneDocumentale.WebApi.Application.Queries.Carica;
using Pi3.App.AggregazioneDocumentale.WebApi.Application.Queries.IdAggregatoSafe;
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate.Repositories;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;

namespace Pi3.App.AggregazioneDocumentale.WebApi.Application.Commands.CollocazioneFisica
{
    public class CollocazioneFisicaCommandHandler : IRequestHandler<CollocazioneFisicaCommand, CollocazioneFisicaCommandResponse>
    {
        private readonly ILogger<CaricaAggregazioneQueryHandler> _logger;
        private readonly IClaimsPrincipalService _claimsPrincipalService;
        private readonly IPi3DbContext _context;
        private readonly IAggregazioneDocumentaleRepository _aggregazioneDocumentaleRepository;
        private readonly IMediator _mediator;

        public CollocazioneFisicaCommandHandler(
            ILogger<CaricaAggregazioneQueryHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IPi3DbContext context,
            IAggregazioneDocumentaleRepository aggregazioneDocumentaleRepository, 
            IMediator mediator) 
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._context = context;
            this._aggregazioneDocumentaleRepository = aggregazioneDocumentaleRepository;
            this._mediator = mediator;
        }

        public async Task<CollocazioneFisicaCommandResponse> Handle(CollocazioneFisicaCommand request, CancellationToken cancellationToken)
        {
            if (!request.Id.IsValidAggregateId())
                throw new IdAggregateNotFoundPi3Exception(request.Id);
            if (request.CollocazioneFisica == null)
                throw new CollocazioneFisicaNotFoundPi3Exception(request.Id);
            if (string.IsNullOrWhiteSpace(request.CollocazioneFisica.Codice))
                throw new CodiceCollocazioneFisicaNotFoundPi3Exception(request.Id);


            var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant, true);

            //if (!await _aggregazioneDocumentaleRepository.Exists(idTenant, request.Id))
            //    throw new AggregazioneDocumentaleNotFoundPi3Exception(request.Id);
            var safeId = await _mediator.Send(new IdAggregatoSafeQuery() { Id = request.Id });
            request.Id = safeId.Id;

            var aggregate = await _aggregazioneDocumentaleRepository.Get(idTenant!, request.Id);

            await DecodificaEAssegnaCollocazioneFisica();

            await _aggregazioneDocumentaleRepository.Update(aggregate);

            return new CollocazioneFisicaCommandResponse();

            async Task DecodificaEAssegnaCollocazioneFisica()
            {
                //var pianoConservazioneCollocazione = await this._context.CorrGlobaliEntities
                //    .AsNoTracking()
                //    .Where(entity => entity.VAR_CODICE == request.CollocazioneFisica.Codice
                //        && entity.CHA_TIPO_URP == "U"
                //        && entity.DTA_FINE == null)
                //    .SingleAsync();
                var pianoConservazioneCollocazione = await this._context.CorrGlobaliEntities.PianoConservazioneCollocazioneDaCodice(request.CollocazioneFisica.Codice);


                aggregate.AssignCollocazioneFisica(new Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate.ValueObjects.CollocazioneFisica()
                {
                    Id = pianoConservazioneCollocazione.SYSTEM_ID.ToString(),
                    Descrizione = new TextValue(pianoConservazioneCollocazione.VAR_DESC_CORR),
                    Cartaceo = request.CollocazioneFisica.Cartaceo,
                    DataCollocazione = DateTime.Now,
                });
            }

        }
    }
}
