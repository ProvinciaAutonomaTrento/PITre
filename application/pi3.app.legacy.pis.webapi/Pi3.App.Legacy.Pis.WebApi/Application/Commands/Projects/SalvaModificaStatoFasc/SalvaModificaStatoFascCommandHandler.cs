// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.EntityFrameworkCore;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects.SalvaDataScadenzaFasc;
using Pi3.App.Legacy.Pis.WebApi.Infrastructure.Services.Principal;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects.SalvaModificaStatoFasc
{
    public class SalvaModificaStatoFascCommandHandler : IRequestHandler<SalvaModificaStatoFascCommand, SalvaModificaStatoFascCommandResponse>
    {
        public SalvaModificaStatoFascCommandHandler(ILogger<SalvaModificaStatoFascCommandHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<SalvaModificaStatoFascCommandResponse> Handle(SalvaModificaStatoFascCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var idProjectAsLong = request.IdProject.AsLong();
                var idStatoAsLong = request.IdStato.AsLong();
                var idPeople = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser);
                var idGruppo = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup);
                var idCorrGlobaliGruppo = await this._dbContext.CorrGlobaliEntities.AsNoTracking().Where(c => c.ID_GRUPPO == idGruppo).Select(c => c.SYSTEM_ID).FirstAsync();

                var diagrammiEntity = await this._dbContext.DiagrammiEntities
                    .FirstOrDefaultAsync(d => d.ID_PROJECT == idProjectAsLong);

                var statoEntity = await this._dbContext.StatoEntities.AsNoTracking().FirstOrDefaultAsync(s => s.SYSTEM_ID == idStatoAsLong);
                var descOldStato = statoEntity.VAR_DESCRIZIONE;

                //Faccio Update
                if (diagrammiEntity != null)
                {
                    var trasmDiagrEntity = await this._dbContext.TrasmDiagrEntities
                        .FirstOrDefaultAsync(t => t.ID_STATO == diagrammiEntity.ID_STATO && t.ID_PROJECT == idProjectAsLong);
                    if (trasmDiagrEntity != null)
                        this._dbContext.TrasmDiagrEntities.Remove(trasmDiagrEntity);

                    if (diagrammiEntity.ID_STATO != idStatoAsLong)
                    {
                        var idStatoOld = diagrammiEntity.ID_STATO;
                        diagrammiEntity.ID_STATO = idStatoAsLong;

                        descOldStato = await this._dbContext.StatoEntities.AsNoTracking().Where(s => s.SYSTEM_ID == idStatoOld).Select(s => s.VAR_DESCRIZIONE).FirstOrDefaultAsync();
                    }
                }
                else //Faccio inserimento
                {
                    diagrammiEntity = new DiagrammiEntity()
                    {
                        ID_PROJECT = idProjectAsLong,
                        ID_STATO = statoEntity.SYSTEM_ID,
                        ID_DIAGRAMMA = request.Diagramma.SYSTEM_ID
                    };

                    await this._dbContext.DiagrammiEntities.AddAsync(diagrammiEntity);

                    //Se stato iniziale impostp la data di scadenza
                    if (statoEntity.STATO_INIZIALE == 1)
                    {
                        var idTipoFasc = await this._dbContext.AssTemplatesFascEntities.AsNoTracking().Where(t => t.ID_PROJECT == request.IdProject).Select(t => t.SYSTEM_ID).FirstOrDefaultAsync();
                        await this._mediator.Send(new SalvaDataScadenzaFascCommand()
                        {
                            IdProject = request.IdProject,
                            DataScadenza = request.DataScadenza,
                            IdTipoFasc = idTipoFasc.ToString()
                        });
                    }
                }

                var diagrammiStoEntity = new DiagrammiStoEntity()
                {
                    ID_USER = request.IdUtente,
                    ID_PROJECT = idProjectAsLong,
                    DTA_DATE = await _dbContext.GetSystemDateTime(),
                    VAR_DESC_OLD_STATO = descOldStato,
                    VAR_DESC_NEW_STATO = statoEntity.VAR_DESCRIZIONE,
                    ID_PEOPLE = idPeople,
                    ID_RUOLO = idCorrGlobaliGruppo
                };

                await this._dbContext.DiagrammiStoEntities.AddAsync(diagrammiStoEntity);

                await ((DbContext)_dbContext).SaveChangesAsync();
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, null, null);

            }

            return new ();
        }


        #region Private Members

        protected readonly ILogger<SalvaModificaStatoFascCommandHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion

    }
}
