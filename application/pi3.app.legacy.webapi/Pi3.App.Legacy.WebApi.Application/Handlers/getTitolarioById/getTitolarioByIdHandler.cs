// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using DocsPaVO.amministrazione;
using DocsPaVO.DiagrammaStato;
using DocsPaVO.ProfilazioneDinamicaLite;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.getTitolarioById
{
    public class getTitolarioByIdHandler : IRequestHandler<Application.Requests.getTitolarioById, GetTitolarioByIdResult>
    {
        protected readonly ILogger<getTitolarioByIdHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected IMapper _mapper;


        public getTitolarioByIdHandler(
            ILogger<getTitolarioByIdHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<GetTitolarioByIdResult> Handle(Application.Requests.getTitolarioById request, CancellationToken cancellationToken)
        {
            DocsPaVO.amministrazione.OrgTitolario result = null;
            try
            {
                result = await this._dbContext.ProjectEntities
                    .Join(this._dbContext.AmministraEntities, p => p.ID_AMM, a => a.SYSTEM_ID, (p, a) => new { p, a })
                    .Where(x => x.p.SYSTEM_ID == request.idTitolario.AsLong())
                    .Select(x => new DocsPaVO.amministrazione.OrgTitolario
                    {
                        ID = x.p.SYSTEM_ID.ToString(),
                        Codice = x.p.VAR_CODICE ?? string.Empty,
                        CodiceAmministrazione = x.a.VAR_CODICE_AMM ?? string.Empty,
                        Commento = x.p.VAR_NOTE ?? string.Empty,
                        DataAttivazione = x.p.DTA_ATTIVAZIONE.HasValue ? x.p.DTA_ATTIVAZIONE.AsDateTimeFormat() : null,
                        DataCessazione = x.p.DTA_CESSAZIONE.HasValue ? x.p.DTA_CESSAZIONE.AsDateTimeFormat() : null,
                        DescrizioneLite = x.p.DESCRIPTION ?? string.Empty,
                        Descrizione = GetDescription(x.p.CHA_STATO, x.p.DESCRIPTION, x.p.DTA_ATTIVAZIONE.AsDateFormat(), x.p.DTA_CESSAZIONE.HasValue ? x.p.DTA_CESSAZIONE.AsDateFormat() : null),
                        Stato = GetStato(x.p.CHA_STATO),
                        MaxLivTitolario = x.p.MAX_LIV_TIT ?? string.Empty,
                        EtichettaTit = x.p.ET_TITOLARIO ?? string.Empty,
                        EtichettaLiv1 = x.p.ET_LIVELLO1 ?? string.Empty,
                        EtichettaLiv2 = x.p.ET_LIVELLO2 ?? string.Empty,
                        EtichettaLiv3 = x.p.ET_LIVELLO3 ?? string.Empty,
                        EtichettaLiv4 = x.p.ET_LIVELLO4 ?? string.Empty,
                        EtichettaLiv5 = x.p.ET_LIVELLO5 ?? string.Empty,
                        EtichettaLiv6 = x.p.ET_LIVELLO6 ?? string.Empty
                    })
                    .FirstOrDefaultAsync() ?? throw new Exceptions.TitolarioNotFoundException(request.idTitolario.AsLong());

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);
            }

            return new GetTitolarioByIdResult(result);
        }

        private static OrgStatiTitolarioEnum GetStato(string stato)
        {
            switch (stato)
            {
                case "D":
                    return OrgStatiTitolarioEnum.InDefinizione;
                case "A":
                default:
                    return OrgStatiTitolarioEnum.Attivo;
                case "C":
                    return OrgStatiTitolarioEnum.Chiuso;
            }
        }

        private static string GetDescription(string stato, string descrizione, string dataAttivazione, string dataCessazione)
        {
            switch (stato)
            {
                case "D":
                    return string.Format("{0} - {1}", descrizione, Resources.DefinistionState);
                case "A":
                    return string.Format("{0} - {1}", descrizione, Resources.ActiveState);
                case "C":
                    string closedStateDesc = string.Format(Resources.ClosedState, dataAttivazione, dataCessazione);
                    return string.Format("{0} - {1}", descrizione, closedStateDesc);
                default:
                    return descrizione;
            }
        }
    }
}
