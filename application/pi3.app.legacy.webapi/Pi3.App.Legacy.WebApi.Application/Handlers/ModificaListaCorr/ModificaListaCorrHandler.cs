// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Extensions;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.AggregateModels.ListaDistribuzioneAggregate;
using Pi3.Core.AggregateModels.ListaDistribuzioneAggregate.Repositories;
using Pi3.Core.AggregateModels.ListaDistribuzioneAggregate.ValueObjects;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.ModificaListaCorr
{
    public class ModificaListaCorrHandler : IRequestHandler<modificaListaCorr, modificaListaCorrResult>
    {
        #region Public members
        public ModificaListaCorrHandler(ILogger<ModificaListaCorrHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IPi3DbContext dbContext, IListaDistribuzioneRepository repository)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._repository = repository;
        }
        public async Task<modificaListaCorrResult> Handle(modificaListaCorr request, CancellationToken cancellationToken)
        {
            try
            {
                var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant, true);

                var aggregate = await this._repository.Get(idTenant, request.idLista);

                if (aggregate is null) throw new ListaDistribuzioneNotFoundException(request.idLista);

                List<string> newCorrs = new List<string>();

                foreach (DataRow row in request.dsCorrLista.Tables[0].Rows) newCorrs.Add(row[0].ToString() ?? string.Empty);

                var toAdd = newCorrs.Where(x => !aggregate.Destinatari.Any(d => d.Id == x)).ToList();

                var toRemove = aggregate.Destinatari.Where(d => !newCorrs.Contains(d.Id)).Select(d => d.Id).ToList();

                if (toRemove is not null && toRemove.Any()) toRemove.ForEach(x => aggregate.RemoveDestinatario(x));

                if (toAdd is not null && toAdd.Any())
                {
                    foreach (string id in toAdd)
                    {
                        var corrEntity = await this._dbContext.CorrGlobaliEntities.FirstOrDefaultAsync(c => c.SYSTEM_ID == id.AsLong());
                        if (corrEntity is not null) aggregate.AddDestinatario(corrEntity.SYSTEM_ID.ToString(), this.GetTipoDestinatarioEnum(corrEntity.CHA_TIPO_URP), new Core.SeedWork.TextValue(corrEntity.VAR_DESC_CORR), (corrEntity.CHA_TIPO_IE.ToUpper() == "E"));
                    }
                }

                await this._repository.Update(aggregate);
            }
            catch (Exception ex)
            {
                this._logger.LogWebMethodError(ex);
            }

            return new modificaListaCorrResult();

        }
        #endregion

        #region Private members

        protected readonly ILogger<ModificaListaCorrHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IListaDistribuzioneRepository _repository;

        private TipiDestinatariEnum GetTipoDestinatarioEnum(string code)
        {
            switch(code.ToUpper())
            {
                case "U":
                case "F":
                    return TipiDestinatariEnum.Ufficio;
                case "R":
                    return TipiDestinatariEnum.Gruppo;
                case "P":
                    return TipiDestinatariEnum.Persona;
                default:
                    throw new TipoCorrispondenteNotSupportedException(code);
            }
        }

        #endregion
    }
}
