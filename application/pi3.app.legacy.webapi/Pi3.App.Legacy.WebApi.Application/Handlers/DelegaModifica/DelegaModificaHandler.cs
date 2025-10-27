// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.AggregateModels.DelegaAggregate;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DelegaModificaRequest = Pi3.App.Legacy.WebApi.Application.Requests.DelegaModifica;
using DelegaCreaNuovaRequest = Pi3.App.Legacy.WebApi.Application.Requests.DelegaCreaNuova;
using DocsPaVO.Mobile;
using Pi3.Core.Extensions;
using Microsoft.EntityFrameworkCore;
using Pi3.Core.AggregateModels.DelegaAggregate.Repositories;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.DelegaModifica
{
    public class DelegaModificaHandler : IRequestHandler<DelegaModificaRequest, DelegaModificaResult>
    {
        #region Public Members

        public DelegaModificaHandler(ILogger<DelegaModificaHandler> logger, 
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

        public async Task<DelegaModificaResult> Handle(DelegaModificaRequest request, CancellationToken cancellationToken)
        {
            bool output = true;
            var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant);
           
            try
            {
                var idCorrGlobaliDelegato = request.delega.id_ruolo_delegato.AsLong();
                var idUtenteDelegato = request.delega.id_utente_delegato.AsLong();

                var idCorrGlobaliDelegante = request.delega.id_ruolo_delegante.AsLong();

                var gruppoDelegato = await this._dbContext.CorrGlobaliEntities.AsNoTracking().Where(c => c.SYSTEM_ID == idCorrGlobaliDelegato).Select(c => new { c.ID_GRUPPO, c.VAR_COD_RUBRICA, c.VAR_DESC_CORR }).FirstAsync();
                RuoloDeleganteEntity gruppoDelegante = null;
                if (idCorrGlobaliDelegante != 0)
                {
                    gruppoDelegante = await this._dbContext.CorrGlobaliEntities.AsNoTracking()
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
                    gruppoDelegante = new RuoloDeleganteEntity()
                    {
                        ID_GRUPPO = 0,
                        VAR_COD_RUBRICA = "TUTTI"
                    };
                }
                var aggregate = await this._repository.Get(idTenant, request.delega.id_delega);

                if (aggregate == null)
                    throw new DelegaNotFoundPi3Exception(request.delega.id_delega);

                if (!aggregate.GruppoDelegato.Id.Equals(gruppoDelegato.ID_GRUPPO.ToString()))
                    aggregate.AssignGruppoDelegato(gruppoDelegato.ID_GRUPPO.ToString(), gruppoDelegato.VAR_COD_RUBRICA, gruppoDelegato.VAR_DESC_CORR);

                if (!aggregate.GruppoDelegante.Id.Equals(gruppoDelegante.ID_GRUPPO.ToString()))
                    aggregate.AssignGruppoDelegante(gruppoDelegante.ID_GRUPPO.ToString(), gruppoDelegante.VAR_COD_RUBRICA, gruppoDelegante.VAR_DESC_CORR);

                if (!aggregate.UtenteDelegato.Id.Equals(idUtenteDelegato.ToString()))
                    aggregate.AssignUtenteDelegato(request.delega.id_utente_delegato, null, null, null);

                if(aggregate.Stato != Core.AggregateModels.DelegaAggregate.ValueObjects.StatoDelegaEnum.Attiva)
                     aggregate.ChangeDataDecorrenza(request.delega.dataDecorrenza.AsDateTime());

                aggregate.ChangeDataScadenza(!string.IsNullOrEmpty(request.delega.dataScadenza) ? request.delega.dataScadenza.AsDateTime() : null);

                this._repository.Update(aggregate);
               
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);
                output = false;
            }

            return new DelegaModificaResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<DelegaModificaHandler> _logger;
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
