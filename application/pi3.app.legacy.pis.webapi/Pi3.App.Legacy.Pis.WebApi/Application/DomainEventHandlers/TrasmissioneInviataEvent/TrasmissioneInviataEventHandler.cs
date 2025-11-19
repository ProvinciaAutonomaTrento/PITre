// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.EntityFrameworkCore;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using Pi3.Core.AggregateModels.TrasmissioneAggregate.Events;
using Pi3.App.Legacy.Pis.WebApi.Infrastructure.Services.RabbitMQ;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Transmissions.InviaEmailTrasmissione;

namespace Pi3.App.Legacy.Pis.WebApi.Application.DomainEventHandlers.TrasmissioneInviataEvent
{
    public class TrasmissioneInviataEventHandler : IEventHandler<Pi3.Core.AggregateModels.TrasmissioneAggregate.Events.TrasmissioneInviataEvent>
    {
        #region Public Members

        public TrasmissioneInviataEventHandler(ILogger<TrasmissioneInviataEventHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext pi3DbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._pi3DbContext = pi3DbContext;
        }

        public async Task Handle(Pi3.Core.AggregateModels.TrasmissioneAggregate.Events.TrasmissioneInviataEvent e)
        {
            long id = Pi3.Core.Extensions.StringExtensions.AsLong(e.Id);

            if (e.InviaBehavior! == null! || !(e.InviaBehavior.InibisciNotifiche ?? false))
            {
                var trasmSingoleEntities = await this._pi3DbContext.TrasmSingolaEntities.AsNoTracking()
                .Where(x => x.ID_TRASMISSIONE == id)
                .ToListAsync();

                trasmSingoleEntities.ForEach(async s =>
                {
                    var ragioneEntity = await this._pi3DbContext.RagioneTrasmissioneEntities.FirstAsync(r => r.SYSTEM_ID == s.ID_RAGIONE);

                    if (!(ragioneEntity?.VAR_NOTIFICA_TRASM == "NN"))
                    {
                        var trasmUtenteEntities = await this._pi3DbContext.TrasmUtenteEntities.AsNoTracking()
                        .Where(x => x.ID_TRASM_SINGOLA == s.SYSTEM_ID)
                        .ToListAsync();

                        trasmUtenteEntities.ForEach(async u =>
                        {
                            var tipoNotifica = ragioneEntity!.VAR_NOTIFICA_TRASM;

                            await this._mediator.Send(
                                new MessageQueueCommandWrapper(
                                    new InviaEmailTrasmissioneCommand(this._claimsPrincipalService.Current)
                                    {
                                        IdTrasmissioneUtente = u.SYSTEM_ID,
                                        TipoNotifica = tipoNotifica
                                    }));
                        });
                    }
                });
            }
        }

        #endregion

        #region Private Members

        protected readonly ILogger<TrasmissioneInviataEventHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _pi3DbContext;

        #endregion
    }
}
