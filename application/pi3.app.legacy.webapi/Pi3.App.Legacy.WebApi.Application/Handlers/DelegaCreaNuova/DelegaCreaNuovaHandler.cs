// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.AggregateModels.DelegaAggregate;
using Pi3.Core.AggregateModels.DelegaAggregate.Repositories;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DelegaCreaNuovaRequest = Pi3.App.Legacy.WebApi.Application.Requests.DelegaCreaNuova;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.DelegaCreaNuova
{
    public class DelegaCreaNuovaHandler : IRequestHandler<DelegaCreaNuovaRequest, DelegaCreaNuovaResult>
    {
        #region Public Members

        public DelegaCreaNuovaHandler(ILogger<DelegaCreaNuovaHandler> logger, 
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IDelegaRepository repository,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._repository = repository;
            this._dbContext = dbContext;
        }

        public async Task<DelegaCreaNuovaResult> Handle(DelegaCreaNuovaRequest request, CancellationToken cancellationToken)
        {
            bool output = true;
            var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant);

            try
            {
                var idCorrGlobaliDelegato = request.delega.id_ruolo_delegato.AsLong();
                var idCorrGlobaliDelegante = request.delega.id_ruolo_delegante.AsLong();

                var delegatoEnity = await this._dbContext.CorrGlobaliEntities.AsNoTracking()
                    .Where(c => c.SYSTEM_ID == idCorrGlobaliDelegato)
                    .Select(c => new
                    {
                        c.ID_GRUPPO,
                        c.VAR_COD_RUBRICA,
                        c.VAR_DESC_CORR
                    })
                    .FirstAsync();

                RuoloDeleganteEntity deleganteEnity = null;
                if (idCorrGlobaliDelegante != 0)
                {
                    deleganteEnity = await this._dbContext.CorrGlobaliEntities.AsNoTracking()
                        .Where(c => c.SYSTEM_ID == idCorrGlobaliDelegante)
                        .Select(c => new RuoloDeleganteEntity()
                        {
                            ID_GRUPPO = c.ID_GRUPPO,
                            VAR_COD_RUBRICA = c.VAR_COD_RUBRICA,
                            VAR_DESC_CORR = c.VAR_DESC_CORR
                        })
                        .FirstAsync();
                }
                else
                {
                    deleganteEnity = new RuoloDeleganteEntity()
                    {
                        ID_GRUPPO = 0,
                        VAR_COD_RUBRICA = "TUTTI"
                    };
                }

                var aggregate = new Delega(idTenant, DateTime.Now, null, null);
                aggregate.AssignGruppoDelegante(deleganteEnity.ID_GRUPPO.ToString(), deleganteEnity.VAR_COD_RUBRICA, deleganteEnity.VAR_DESC_CORR);
                aggregate.AssignGruppoDelegato(delegatoEnity.ID_GRUPPO.ToString(), delegatoEnity.VAR_COD_RUBRICA, delegatoEnity.VAR_DESC_CORR);
                aggregate.AssignUtenteDelegante(request.delega.id_utente_delegante, request.delega.cod_utente_delegante, null, null);
                aggregate.AssignUtenteDelegato(request.delega.id_utente_delegato, request.delega.cod_utente_delegato, null, null);
                aggregate.ChangeDataDecorrenza(request.delega.dataDecorrenza.AsDateTime());
                if(!string.IsNullOrEmpty(request.delega.dataScadenza))
                 aggregate.ChangeDataScadenza(request.delega.dataScadenza.AsDateTime());

                await this._repository.Add(aggregate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);
                output = false;
            }

            return new DelegaCreaNuovaResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<DelegaCreaNuovaHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IDelegaRepository _repository;
        protected readonly IPi3DbContext _dbContext;

        protected class RuoloDeleganteEntity
        {
            public long? ID_GRUPPO { get; set; }
            public string? VAR_COD_RUBRICA { get; set; }
            public string? VAR_DESC_CORR { get; set; }
        }
        #endregion
    }
}
