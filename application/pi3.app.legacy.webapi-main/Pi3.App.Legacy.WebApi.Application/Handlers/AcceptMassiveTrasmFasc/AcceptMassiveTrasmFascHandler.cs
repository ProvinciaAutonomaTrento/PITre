// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.AggregateModels.TrasmissioneAggregate;
using Pi3.Core.AggregateModels.TrasmissioneAggregate.Repositories;
using Pi3.Core.AggregateModels.TrasmissioneAggregate.ValueObjects;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcceptMassiveTrasmFascRequest = Pi3.App.Legacy.WebApi.Application.Requests.AcceptMassiveTrasmFasc;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.AcceptMassiveTrasmFasc
{

    public class AcceptMassiveTrasmFascHandler : IRequestHandler<AcceptMassiveTrasmFascRequest, AcceptMassiveTrasmFascResult>
    {
        #region Public Members

        public AcceptMassiveTrasmFascHandler(ILogger<AcceptMassiveTrasmFascHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext,
            IWebMethodLoggerService webMethodLoggerService,
            ITrasmissioneRepository trasmissioneRepository)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._webMethodLoggerService = webMethodLoggerService;
            this._trasmissioneRepository = trasmissioneRepository;
        }

        public async Task<AcceptMassiveTrasmFascResult> Handle(AcceptMassiveTrasmFascRequest request, CancellationToken cancellationToken)
        {
            bool output = true;

            try
            {
                var idProjectAsLong = request.idProject.AsLong();
                var userId = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.UserId);
                var idGruppo = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup);
                var idPeople = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser);
                var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant);

                var idCorrGlobaliGruppo = await this._dbContext.CorrGlobaliEntities.AsNoTracking().Where(c => c.ID_GRUPPO == idGruppo).Select(c => c.SYSTEM_ID).FirstAsync();
                var idCorrGlobaliPeople = await this._dbContext.CorrGlobaliEntities.AsNoTracking().Where(c => c.ID_PEOPLE == idPeople).Select(c => c.SYSTEM_ID).FirstAsync();

                var listTrasmissioni = await this._dbContext.TrasmissioneEntities.AsNoTracking()
                    .Join(this._dbContext.TrasmSingolaEntities.AsNoTracking(), t => t.SYSTEM_ID, ts => ts.ID_TRASMISSIONE, (t, ts) => new { t, ts })
                    .Join(this._dbContext.RagioneTrasmissioneEntities.AsNoTracking(), j => j.ts.ID_RAGIONE, r => r.SYSTEM_ID, (j, r) => new
                    {
                        ID_TRASMISSIONE = j.t.SYSTEM_ID,
                        ID_TRASMISSIONE_SINGOLA = j.ts.SYSTEM_ID,
                        j.t.ID_PROJECT,
                        j.ts.ID_CORR_GLOBALE,
                        j.t.DTA_INVIO,
                        r.CHA_TIPO_RAGIONE
                    })
                    .Where(t => t.ID_PROJECT == idProjectAsLong && t.DTA_INVIO != null && t.CHA_TIPO_RAGIONE == "W"
                            && (t.ID_CORR_GLOBALE == idCorrGlobaliGruppo || t.ID_CORR_GLOBALE == idCorrGlobaliPeople))
                    .ToListAsync();

                foreach (var t in listTrasmissioni)
                {
                    var pending = await this._dbContext.TrasmUtenteEntities.AsNoTracking()
                        .AnyAsync(tu => tu.ID_TRASM_SINGOLA == t.ID_TRASMISSIONE_SINGOLA && tu.CHA_ACCETTATA == "0" && tu.CHA_RIFIUTATA == "0" && tu.ID_PEOPLE == idPeople);

                    if (pending)
                    {
                        var aggregate = await _trasmissioneRepository.Get(idTenant, t.ID_TRASMISSIONE.ToString());

                        aggregate.Accetta(idGruppo.ToString(), idPeople.ToString(),new Accetta()
                        {
                            Data = DateTime.Now,
                            IdDelegato = _claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.DelegatedIdUser),
                            UserIdDelegato = _claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.DelegatedUserId),
                            NomeDelegato = _claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.DelegatedUserName),
                            CognomeDelegato = _claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.DelegatedUserSurname)
                        });

                        await this._trasmissioneRepository.Update(aggregate);
                    }
                }

                await this._webMethodLoggerService.LogOK("ACCEPTTRASMFOLDER", request.idProject, string.Format(Resources.LogAccettazioneFascicolo, request.idProject, userId));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);
                output = false;
            }

            return new AcceptMassiveTrasmFascResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<AcceptMassiveTrasmFascHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected IWebMethodLoggerService _webMethodLoggerService;
        protected ITrasmissioneRepository _trasmissioneRepository;

        #endregion
    }
}
