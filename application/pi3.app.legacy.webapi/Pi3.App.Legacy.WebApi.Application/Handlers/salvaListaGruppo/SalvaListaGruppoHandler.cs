// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Extensions;
using Pi3.App.Legacy.WebApi.Application.Handlers.ModificaListaCorr;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.AggregateModels.ListaDistribuzioneAggregate;
using Pi3.Core.AggregateModels.ListaDistribuzioneAggregate.Repositories;
using Pi3.Core.AggregateModels.ListaDistribuzioneAggregate.ValueObjects;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.salvaListaGruppo
{
    public class SalvaListaGruppoHandler : IRequestHandler<Requests.salvaListaGruppo, salvaListaGruppoResult>
    {
        #region Public members
        public SalvaListaGruppoHandler(ILogger<SalvaListaGruppoHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IPi3DbContext dbContext, IListaDistribuzioneRepository repository,
            IWebMethodLoggerService webMethodLoggerService)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._repository = repository;
            this._webMethodLoggerService = webMethodLoggerService;
        }

        public async Task<salvaListaGruppoResult> Handle(Requests.salvaListaGruppo request, CancellationToken cancellationToken)
        {
            try
            {
                var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant, true);

                var aggregate = new ListaDistribuzione(idTenant!, DateTime.Now, new(request.codiceLista), new(request.nomeLista));

                if (request.idUtente is not null)
                {
                    if (request.gruppo == "yes") aggregate.AssignAutoreRuolo(request.idUtente);
                    else aggregate.AssignAutorePersona(request.idUtente);
                }

                foreach (DataRow row in request.dsCorrLista.Tables[0].Rows)
                {
                    var id = row[0].ToString() ?? string.Empty;
                    var cgEntity = await this._dbContext.CorrGlobaliEntities.AsNoTracking().FirstOrDefaultAsync(c => c.SYSTEM_ID == id.AsLong());

                    if (cgEntity is not null) aggregate.AddDestinatario(cgEntity.SYSTEM_ID.ToString(), this.GetTipoDestinatarioEnum(cgEntity.CHA_TIPO_URP), new(cgEntity.VAR_DESC_CORR), (cgEntity.CHA_TIPO_IE.ToUpper() == "E"));
                }

                await this._repository.Add(aggregate);

                await this._webMethodLoggerService.LogOK("TRASMISSIONELISTADISTR", aggregate.Id, string.Format(Resources.LogAddDistributionList, request.nomeLista));
            }
            catch(Exception ex)
            {
                this._logger.LogWebMethodError(ex);
                await this._webMethodLoggerService.LogKO("TRASMISSIONELISTADISTR", null, string.Format(Resources.LogAddDistributionList, request.nomeLista));
            }

            return new salvaListaGruppoResult();
        }
        #endregion

        #region Private members
        protected ILogger<SalvaListaGruppoHandler> _logger;
        protected IClaimsPrincipalService _claimsPrincipalService;
        protected IMediator _mediator;
        protected IPi3DbContext _dbContext;
        protected IListaDistribuzioneRepository _repository;
        protected IWebMethodLoggerService _webMethodLoggerService;

        private TipiDestinatariEnum GetTipoDestinatarioEnum(string code)
        {
            switch (code.ToUpper())
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
