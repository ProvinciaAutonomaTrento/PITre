// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.addressbook;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using salvaModificaStatoFascRequest = Pi3.App.Legacy.WebApi.Application.Requests.salvaModificaStatoFasc;
using salvaDataScadenzaFascRequest = Pi3.App.Legacy.WebApi.Application.Requests.salvaDataScadenzaFasc;


namespace Pi3.App.Legacy.WebApi.Application.Handlers.salvaModificaStatoFasc
{
    public class salvaModificaStatoFascHandler : IRequestHandler<salvaModificaStatoFascRequest, salvaModificaStatoFascResult>
    {
        #region Public Members

        public salvaModificaStatoFascHandler(ILogger<salvaModificaStatoFascHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<salvaModificaStatoFascResult> Handle(salvaModificaStatoFascRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var idProjectAsLong = request.idProject.AsLong();
                var idStatoAsLong = request.idStato.AsLong();
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
                    if(trasmDiagrEntity != null)
                        this._dbContext.TrasmDiagrEntities.Remove(trasmDiagrEntity);

                    if(diagrammiEntity.ID_STATO != idStatoAsLong)
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
                        ID_DIAGRAMMA = request.diagramma.SYSTEM_ID
                    };

                    await this._dbContext.DiagrammiEntities.AddAsync(diagrammiEntity);

                    //Se stato iniziale impostp la data di scadenza
                    if(statoEntity.STATO_INIZIALE == 1)
                    {
                        var idTipoFasc = await this._dbContext.AssTemplatesFascEntities.AsNoTracking().Where(t => t.ID_PROJECT == request.idProject).Select(t => t.SYSTEM_ID).FirstOrDefaultAsync();
                        await this._mediator.Send(new salvaDataScadenzaFascRequest(request.idProject, request.dataScadenza, idTipoFasc.ToString()));
                    }
                }

                var diagrammiStoEntity = new DiagrammiStoEntity()
                {
                    ID_USER = request.idUtente,
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

            return new salvaModificaStatoFascResult();
        }

        #endregion

        #region Private Members

        protected readonly ILogger<salvaModificaStatoFascHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }
}
