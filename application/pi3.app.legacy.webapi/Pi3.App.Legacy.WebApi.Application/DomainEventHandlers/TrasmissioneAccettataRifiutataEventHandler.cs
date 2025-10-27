// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using DocsPaVO.LibroFirma;
using DocsPaVO.utente;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.Core.AggregateModels.TrasmissioneAggregate.Events;
using Pi3.Core.AggregateModels.TrasmissioneAggregate.Repositories;
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

namespace Pi3.App.Legacy.WebApi.Application.DomainEventHandlers
{

    public class TrasmissioneAccettataRifiutataEventHandler : IEventHandler<TrasmissioneUtenteRifiutataEvent>, IEventHandler<TrasmissioneUtenteAccettataEvent>
    {
        #region Public Members

        public TrasmissioneAccettataRifiutataEventHandler(ILogger<TrasmissioneAccettataRifiutataEventHandler> logger,
            IPi3DbContext dbContext,
            IMediator mediator,
            IClaimsPrincipalService claimsPrincipalService,
            IWebMethodLoggerService webMethodLoggerService,
            ITrasmissioneRepository trasmissioneRepository)
        {
            this._logger = logger;
            this._dbContext = dbContext;
            this._mediator = mediator;
            this._webMethodLoggerService = webMethodLoggerService;
            this._trasmissioneRepository = trasmissioneRepository;
            this._claimsPrincipalService = claimsPrincipalService;
        }

        public async Task Handle(TrasmissioneUtenteRifiutataEvent e)
        {
            var idTrasmUtenteAsLong = e.IdTrasmissioneUtente.AsLong();
            var idTrasmSingola = await this._dbContext.TrasmUtenteEntities.Where(u => u.SYSTEM_ID == idTrasmUtenteAsLong).Select(u => u.ID_TRASM_SINGOLA).FirstAsync();

            var idPeople = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser);
            long? idPeopleDelegato = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.DelegatedIdUser);

            var idGruppo = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup);

            //Se esistono elementi in libro firma legati alla trasmissione che si stà rifiutando, scrivo nel log l'evento di interruzione, cloncludo l'istanza del processo e rimuovo li elementi
            var elementoLibroFirmaEntities = await this._dbContext.ElementoInLibroFirmaEntities.Where(e => e.ID_TRASM_SINGOLA == idTrasmUtenteAsLong).ToListAsync();
            if (elementoLibroFirmaEntities != null && elementoLibroFirmaEntities.Any())
            {
                InfoUtente infoUtente = new InfoUtente()
                {
                    delegato = idPeopleDelegato != 0 ? new InfoUtente()
                    {
                        idPeople = idPeopleDelegato.ToString()
                    } : null,
                    idPeople = idPeople.ToString(),
                    idGruppo = idGruppo.ToString(),
                };

                foreach (var elemento in elementoLibroFirmaEntities)
                {
                    await this._mediator.Send(new Application.Requests.InterruzioneProcessoFirma(elemento.DOC_NUMBER.ToString(), e.Rifiuta.Note.Value, "T", infoUtente));
                }
            }
        }

        public async Task Handle(TrasmissioneUtenteAccettataEvent e)
        {
            var idPeople = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser);
            var idTrasmUtenteAsLong = e.IdTrasmissioneUtente.AsLong();
            var idTrasmSingola = await this._dbContext.TrasmUtenteEntities.Where(u => u.SYSTEM_ID == idTrasmUtenteAsLong).Select(u => u.ID_TRASM_SINGOLA).FirstAsync();

            //Se esiste l'elemento in libro firma, aggiorno la data di accettazione
            var elementoLibroFirmaEntity = await this._dbContext.ElementoInLibroFirmaEntities
                .Where(e => e.ID_TRASM_SINGOLA == idTrasmSingola && e.DTA_ACCETTAZIONE == null)
                .FirstOrDefaultAsync();

            if(elementoLibroFirmaEntity != null)
            {
                elementoLibroFirmaEntity.DTA_ACCETTAZIONE = DateTime.Now;
                elementoLibroFirmaEntity.ID_UTENTE_LOCKER = idPeople;
                if (elementoLibroFirmaEntity.STATO_FIRMA == "PROPOSTO")
                    elementoLibroFirmaEntity.STATO_FIRMA = "DA_FIRMARE";
            }

            await ((DbContext)this._dbContext).SaveChangesAsync();
        }

        #endregion

        #region Private Members

        protected readonly ILogger<TrasmissioneAccettataRifiutataEventHandler> _logger;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IMediator _mediator;
        protected readonly IWebMethodLoggerService _webMethodLoggerService;
        protected readonly ITrasmissioneRepository _trasmissioneRepository;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;

        #endregion
    }
}
