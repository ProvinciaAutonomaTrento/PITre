// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.App.Legacy.WebApi.Application.Services.Principal;
using Pi3.Core.AggregateModels.TrasmissioneAggregate;
using Pi3.Core.AggregateModels.TrasmissioneAggregate.Repositories;
using Pi3.Core.AggregateModels.TrasmissioneAggregate.ValueObjects;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.TrasmissioneExecuteAccRif
{
    public class TrasmissioneExecuteAccRifHandler : IRequestHandler<Application.Requests.TrasmissioneExecuteAccRif, TrasmissioneExecuteAccRifResult>
    {
        #region Public Members

        public TrasmissioneExecuteAccRifHandler(ILogger<TrasmissioneExecuteAccRifHandler> logger, IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IDistributedCache distributedCache,
            IPi3DbContext dbContext,
            ITrasmissioneRepository trasmissioneRepository,
            IWebMethodLoggerService webMethodLoggerService)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._distributedCache = distributedCache;
            this._dbContext = dbContext;
            this._trasmissioneRepository = trasmissioneRepository;
            this._webMethodLoggerService = webMethodLoggerService;
        }




        public async Task<TrasmissioneExecuteAccRifResult> Handle(Application.Requests.TrasmissioneExecuteAccRif request, CancellationToken cancellationToken)
        {
            bool result = false;
            try
            {
                var idAmm = _claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant);
                var idUserDelegato = _claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.DelegatedIdUser);
                var idGruppo = _claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup);

                var trasmUtente = request.trasmissioneUtente;

                var idTrasmUtenteAsLong = trasmUtente.systemId.AsLong();
                var idTrasmSingola = await this._dbContext.TrasmUtenteEntities.AsNoTracking().Where(u => u.SYSTEM_ID == idTrasmUtenteAsLong).Select(u => u.ID_TRASM_SINGOLA).FirstAsync();

                var aggregate = await _trasmissioneRepository.Get(idAmm, request.idTrasmissione);
                if (trasmUtente.tipoRisposta == DocsPaVO.trasmissione.TipoRisposta.ACCETTAZIONE)
                {
                    aggregate.Accetta(idGruppo.ToString(), trasmUtente.utente.idPeople, new Accetta()
                    {
                        Data = DateTime.Now,
                        Note = new TextValue(trasmUtente.noteAccettazione),
                        IdDelegato = string.IsNullOrWhiteSpace(idUserDelegato) ? null : idUserDelegato
                    });

                    await _trasmissioneRepository.Update(aggregate);
                    result = true;
                }
                else
                {
                    aggregate.RifiutaTrasmissioneUtente(trasmUtente.systemId, new Rifiuta()
                    {
                        Data = DateTime.Now,
                        Note = new TextValue(trasmUtente.noteRifiuto),
                        IdDelegato = string.IsNullOrWhiteSpace(idUserDelegato) ? null : idUserDelegato
                    });

                    await _trasmissioneRepository.Update(aggregate);
                    result = true;
                }

                var method = string.Empty;
                var description = string.Empty;
                if (trasmUtente.tipoRisposta == DocsPaVO.trasmissione.TipoRisposta.ACCETTAZIONE)
                {
                    method = aggregate.OggettoTrasmesso.TipoOggetto == TipiOggettiTrasmessiEnum.DocumentoAmministrativo ? "ACCEPTTRASMDOCUMENT" : "ACCEPTTRASMFOLDER";
                    description = aggregate.OggettoTrasmesso.TipoOggetto == TipiOggettiTrasmessiEnum.DocumentoAmministrativo ? Resources.LogAccettazioneDocumento : Resources.LogAccettazioneFascicolo;
                }
                if(trasmUtente.tipoRisposta == DocsPaVO.trasmissione.TipoRisposta.RIFIUTO)
                {
                    method = aggregate.OggettoTrasmesso.TipoOggetto == TipiOggettiTrasmessiEnum.DocumentoAmministrativo ? "REJECTTRASMDOCUMENT" : "REJECTTRASMFOLDER";
                    description = aggregate.OggettoTrasmesso.TipoOggetto == TipiOggettiTrasmessiEnum.DocumentoAmministrativo ? Resources.LogRfiutoDocumento : Resources.LogRifiutoFascicolo;
                }

                await _webMethodLoggerService.LogOK(method, aggregate.OggettoTrasmesso.Id, string.Format(description, aggregate.OggettoTrasmesso.Id), idTrasmSingola.ToString());
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, null, null);
            }

            return new TrasmissioneExecuteAccRifResult(result, "");
        }

        #endregion

        #region Private Members

        protected readonly ILogger<TrasmissioneExecuteAccRifHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IDistributedCache _distributedCache;
        protected readonly ITrasmissioneRepository _trasmissioneRepository;
        protected readonly IWebMethodLoggerService _webMethodLoggerService;


        #endregion
    }
}
