// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.utente;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Extensions;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GetListaRuoliUtenteByIdCorrRequest = Pi3.App.Legacy.WebApi.Application.Requests.GetListaRuoliUtenteByIdCorr;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GetListaRuoliUtenteByIdCorr
{
    public class GetListaRuoliUtenteByIdCorrHandler : IRequestHandler<GetListaRuoliUtenteByIdCorrRequest, GetListaRuoliUtenteByIdCorrResult>
    {
        #region Public Members

        public GetListaRuoliUtenteByIdCorrHandler(ILogger<GetListaRuoliUtenteByIdCorrHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext
        )
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<GetListaRuoliUtenteByIdCorrResult> Handle(GetListaRuoliUtenteByIdCorrRequest request, CancellationToken cancellationToken)
        {
            Ruolo[] output = null;

            try
            {
                var idCorrAsLong = request.idCorr.AsLong();
                var idPeople = await this._dbContext.CorrGlobaliEntities.AsNoTracking()
                    .Where(c => c.SYSTEM_ID == idCorrAsLong)
                    .Select(c => c.ID_PEOPLE)
                    .FirstOrDefaultAsync();

                var ruoliEntity = await this._dbContext.PeopleGroupEntities.AsNoTracking()
                    .Join(this._dbContext.CorrGlobaliEntities.AsNoTracking(), pg => pg.GROUPS_SYSTEM_ID, g => g.ID_GRUPPO, (pg, g) => new {pg, g})
                    .Join(this._dbContext.TipoRuoloEntities.AsNoTracking(), j => j.g.ID_TIPO_RUOLO, t => t.SYSTEM_ID, (j, t) => new {j.pg, j.g, t})
                    .Where(j => j.pg.PEOPLE_SYSTEM_ID == idPeople && j.pg.DTA_FINE == null && j.g.DTA_FINE == null)
                    .Select(j => new
                    {
                        j.pg.PEOPLE_SYSTEM_ID,
                        j.pg.CHA_PREFERITO,
                        j.g.SYSTEM_ID,
                        j.g.ID_GRUPPO,
                        j.g.ID_UO,
                        j.g.VAR_COD_RUBRICA,
                        j.g.ID_REGISTRO,
                        j.g.ID_AMM,
                        j.g.VAR_DESC_CORR,
                        j.g.CHA_RIFERIMENTO,
                        j.g.CHA_RESPONSABILE,
                        j.g.CHA_SEGRETARIO,
                        j.t.NUM_LIVELLO,
                        j.t.VAR_CODICE,
                        j.t.VAR_DESC_RUOLO
                    })
                    .OrderByDescending(c => c.CHA_PREFERITO != null)
                    .ToListAsync();

                if (ruoliEntity != null)
                {
                    output = new Ruolo[ruoliEntity.Count];
                    for (int i = 0; i < ruoliEntity.Count; i++)
                    {
                        output[i] = new Ruolo()
                        {
                            systemId = ruoliEntity[i].SYSTEM_ID.ToString(),
                            descrizione = ruoliEntity[i].VAR_DESC_CORR,
                            codice = ruoliEntity[i].VAR_CODICE,
                            livello = ruoliEntity[i].NUM_LIVELLO.ToString(),
                            idGruppo = ruoliEntity[i].ID_GRUPPO.ToString(),
                            tipoRuolo = new TipoRuolo()
                            {
                                codice = ruoliEntity[i].VAR_CODICE,
                                descrizione = ruoliEntity[i].VAR_DESC_RUOLO
                            },
                            codiceRubrica = ruoliEntity[i].VAR_COD_RUBRICA,
                            idRegistro = ruoliEntity[i].ID_REGISTRO.ToString(),
                            idAmministrazione = ruoliEntity[i].ID_AMM.ToString(),
                            tipoCorrispondente = "R",
                            Responsabile = ruoliEntity[i].CHA_RESPONSABILE == "1",
                            Segretario = ruoliEntity[i].CHA_SEGRETARIO == "1",
                            selezionato = ruoliEntity[i].CHA_PREFERITO == "1"
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                this._logger.LogWebMethodError(ex);
            }

            return new GetListaRuoliUtenteByIdCorrResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetListaRuoliUtenteByIdCorrHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }
}
