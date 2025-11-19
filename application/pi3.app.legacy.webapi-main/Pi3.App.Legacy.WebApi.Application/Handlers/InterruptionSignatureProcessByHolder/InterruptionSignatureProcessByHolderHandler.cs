// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.LibroFirma;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Core.AggregateModels.TrasmissioneAggregate.Repositories;
using Pi3.Core.AggregateModels.TrasmissioneAggregate.ValueObjects;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterruptionSignatureProcessByHolderRequest = Pi3.App.Legacy.WebApi.Application.Requests.InterruptionSignatureProcessByHolder;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.InterruptionSignatureProcessByHolder
{
    public class InterruptionSignatureProcessByHolderHandler : IRequestHandler<InterruptionSignatureProcessByHolderRequest, InterruptionSignatureProcessByHolderResult>
    {
        #region Public Members

        public InterruptionSignatureProcessByHolderHandler(ILogger<InterruptionSignatureProcessByHolderHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext,
            ITrasmissioneRepository trasmissioneRepository,
            IDocumentoAmministrativoRepository documentoAmministrativoRepository,
            IWebMethodLoggerService webMethodLoggerService)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._trasmissioneRepository = trasmissioneRepository;
            this._documentoAmministrativoRepository = documentoAmministrativoRepository;
            this._webMethodLoggerService = webMethodLoggerService;
        }

        public async Task<InterruptionSignatureProcessByHolderResult> Handle(InterruptionSignatureProcessByHolderRequest request, CancellationToken cancellationToken)
        {
            var output = false;
            var errore = string.Empty;
            var elemento = request.elemento;

            try
            {
                var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant);
                var idPeople = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser);
                var userIdPeople = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.UserId);
                var idGruppo = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup);
                var idCorrGlobaliGruppo = await this._dbContext.CorrGlobaliEntities.Where(c => c.ID_GRUPPO == idGruppo).Select(c => c.SYSTEM_ID).FirstAsync();
                var idPeopleDelegato = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.DelegatedIdUser);
                var idTrasmSingola = elemento.IdTrasmSingola.AsLong();

                var idTrasmissione = await this._dbContext.TrasmSingolaEntities.Where(s => s.SYSTEM_ID == idTrasmSingola).Select(s => s.ID_TRASMISSIONE).FirstAsync();
                var idTrasmUtente = await this._dbContext.TrasmUtenteEntities.Where(s => s.ID_TRASM_SINGOLA == idTrasmSingola && s.ID_PEOPLE == idPeople).Select(s => s.SYSTEM_ID).FirstAsync();
                var aggregate = await this._trasmissioneRepository.Get(idTenant, idTrasmissione.ToString());

                aggregate.RifiutaTrasmissioneUtente(idTrasmUtente.ToString(), new Rifiuta()
                {
                    Data = DateTime.Now,
                    Note = new TextValue(Resources.NoteRifiutoNonDiCompetenza),
                    IdDelegato = idPeopleDelegato != null ? idPeopleDelegato.ToString() : null
                });

                await _trasmissioneRepository.Update(aggregate);
            }
            catch ( Exception ex )
            {
                this._logger.LogError(ex, null, null);
                output = false;
            }

            return new InterruptionSignatureProcessByHolderResult(output, errore);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<InterruptionSignatureProcessByHolderHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly ITrasmissioneRepository _trasmissioneRepository;
        protected readonly IDocumentoAmministrativoRepository _documentoAmministrativoRepository;
        protected readonly IWebMethodLoggerService _webMethodLoggerService;

        #endregion
    }
}
