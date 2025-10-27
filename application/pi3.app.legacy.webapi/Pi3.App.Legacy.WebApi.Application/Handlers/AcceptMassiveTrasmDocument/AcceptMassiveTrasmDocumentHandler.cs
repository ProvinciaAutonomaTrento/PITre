// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
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

namespace Pi3.App.Legacy.WebApi.Application.Handlers.AcceptMassiveTrasmDocument
{

    public class AcceptMassiveTrasmDocumentHandler : IRequestHandler<Application.Requests.AcceptMassiveTrasmDocument, AcceptMassiveTrasmDocumentResult>
    {
        #region Public Members

        public AcceptMassiveTrasmDocumentHandler(ILogger<AcceptMassiveTrasmDocumentHandler> logger, 
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator, 
            IPi3DbContext dbContext, 
            IDistributedCache distributedCache, 
            IConfiguration configuration,
            IWebMethodLoggerService webMethodLoggerService,
            ITrasmissioneRepository trasmissioneRepository)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._distributedCache = distributedCache;
            this._configuration = configuration;
            this._trasmissioneRepository = trasmissioneRepository;
            this._webMethodLoggerService = webMethodLoggerService;
        }

        public async Task<AcceptMassiveTrasmDocumentResult> Handle(Application.Requests.AcceptMassiveTrasmDocument request, CancellationToken cancellationToken)
        {
            var output = false;

            try
            {
                var idProfileAsLong = request.idProfile.AsLong();
                var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant);
                var idUser = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser);
                var idRuoloInUO = request.ruolo.systemId.AsLong();
                var idGruppo = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup);
                var idCorrGlobaliPeople = await this._dbContext.CorrGlobaliEntities.AsNoTracking().Where(c => c.ID_PEOPLE == idUser).Select(c => c.SYSTEM_ID).FirstAsync();

                var listTrasmissioni = await this._dbContext.TrasmissioneEntities.AsNoTracking()
                    .Join(this._dbContext.TrasmSingolaEntities.AsNoTracking(), t => t.SYSTEM_ID, ts => ts.ID_TRASMISSIONE, (t, ts) => new { t, ts })
                    .Join(this._dbContext.RagioneTrasmissioneEntities.AsNoTracking(), j => j.ts.ID_RAGIONE, r => r.SYSTEM_ID, (j, r) => new
                    {
                        ID_TRASMISSIONE = j.t.SYSTEM_ID,
                        ID_TRASMISSIONE_SINGOLA = j.ts.SYSTEM_ID,
                        j.t.CHA_TIPO_OGGETTO,
                        j.t.ID_PROFILE,
                        j.ts.ID_CORR_GLOBALE,
                        j.t.DTA_INVIO,
                        r.CHA_TIPO_RAGIONE
                    })
                    .Where(t => t.ID_PROFILE == idProfileAsLong && t.DTA_INVIO != null && t.CHA_TIPO_RAGIONE == "W" && t.CHA_TIPO_OGGETTO == "D"
                            && (t.ID_CORR_GLOBALE == idRuoloInUO || t.ID_CORR_GLOBALE == idCorrGlobaliPeople))
                    .ToListAsync();

                foreach (var t in listTrasmissioni)
                {
                    var pending = await this._dbContext.TrasmUtenteEntities.AsNoTracking()
                        .AnyAsync(tu => tu.CHA_VALIDA == "1" && tu.ID_TRASM_SINGOLA == t.ID_TRASMISSIONE_SINGOLA && tu.CHA_ACCETTATA == "0" && tu.CHA_RIFIUTATA == "0" && tu.ID_PEOPLE == idUser);

                    if (pending)
                    {
                        var aggregate = await _trasmissioneRepository.Get(idTenant, t.ID_TRASMISSIONE.ToString());

                        aggregate.Accetta(idGruppo.ToString(), idUser.ToString(), new Accetta()
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

                output = true;

                await this._webMethodLoggerService.LogOK("ACCEPTTRASMDOCUMENT", request.idProfile, string.Format(Resources.LogAccettazioneMassiva, request.idProfile));

            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, ex.Message);
                output = false;
            }

            return new AcceptMassiveTrasmDocumentResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<AcceptMassiveTrasmDocumentHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IDistributedCache _distributedCache;
        protected readonly IConfiguration _configuration;
        protected readonly ITrasmissioneRepository _trasmissioneRepository;
        protected IWebMethodLoggerService _webMethodLoggerService;

        #endregion
    }

}
